using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using ReactiveInput;

public class InputControl: UniqueBehaviour<InputControl> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    [Serializable]
    struct Inspector {
        public float horizontalMove;
        public bool goUp;
        public bool breath;
        public Vector2 mousePos;
        public bool interact;
    }
    [SerializeField]
    Inspector inspector;

    public InputSystem input;

    public ReadOnlyReactiveProperty<float> HorizontalMoveInput;
    public IObservable<float> WhileHorizontalMoving;

    public ReadOnlyReactiveProperty<bool> GoUpInput;
    public IObservable<Unit> GoUp;

    public ReadOnlyReactiveProperty<bool> BreathInput;
    public IObservable<Unit> BreathPress;
    public IObservable<Unit> BreathRelease;

    public ReadOnlyReactiveProperty<Vector2> MousePosInput;
    public ReadOnlyReactiveProperty<Vector3> MousePosStage;

    public ReadOnlyReactiveProperty<bool> InteractInput;
    public IObservable<Unit> Interact;

    void Awake() {
        input = new InputSystem();
        input.Enable();

        HorizontalMoveInput = input.Player.HorizontalMove.AsAxis();
        GoUpInput = input.Player.GoUp.AsButton();
        BreathInput = input.Player.Breath.AsButton();
        MousePosInput = input.Player.MousePos.As2dAxis();
        InteractInput = input.Player.Interact.AsButton();

        WhileHorizontalMoving = Observable
            .EveryFixedUpdate()
            .WithLatestFrom(HorizontalMoveInput, (_,hmi) => hmi)
            .Share();

        GoUp = GoUpInput
            .Where(x => x)
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share();

        BreathPress = BreathInput
            .Where(x => x)
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share();

        BreathRelease = BreathInput
            .Where(x => !x)
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share();

        MousePosStage = Observable
            .Merge(MousePosInput,
                   Camera.main.transform.ObserveEveryValueChanged(x => x.position)
                   .Select(_ => MousePosInput.Value))
            .Select(pos => MousePos_ScreenToGameStage(pos, out Vector3 stagePos)
                    ? stagePos
                    : MousePosStage.Value)
            .ToReadOnlyReactiveProperty();

        Interact = InteractInput
            .Where(x => x)
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share();


        HorizontalMoveInput.Subscribe(x => inspector.horizontalMove = x).AddTo(this);
        GoUpInput.Subscribe(x => inspector.goUp = x).AddTo(this);
        BreathInput.Subscribe(x => inspector.breath = x).AddTo(this);
        MousePosInput.Subscribe(x => inspector.mousePos = x).AddTo(this);
        InteractInput.Subscribe(x => inspector.interact = x).AddTo(this);
    }

    static LayerMask stsc = new Layers(Layer.ScreenToStageConverter);
    public static bool MousePos_ScreenToGameStage(Vector3 screenPos, out Vector3 stagePos){
        screenPos.z = 1.0f;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        var camPos = Camera.main.transform.position;
        if (Physics.Raycast(camPos, (worldPos - camPos), out RaycastHit hit, 100f, stsc)){
            Debug.DrawRay(camPos, hit.point, Color.red);
            stagePos = hit.point;
            stagePos.z = 0.0f;
            return true;
        }
        else {
            stagePos = Vector3.zero;
            return false;
        }
    }
}

namespace ReactiveInput {
    public static class InputSystemExtension {
        public static ReadOnlyReactiveProperty<bool> AsButton(this InputAction inputAction) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h)
                .Select(x => x.ReadValueAsButton())
                .ToReadOnlyReactiveProperty(false);
        }
        public static ReadOnlyReactiveProperty<float> AsAxis(this InputAction inputAction) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h)
                .Select(x => x.ReadValue<float>())
                .ToReadOnlyReactiveProperty(0f);
        }
        public static ReadOnlyReactiveProperty<Vector2> As2dAxis(this InputAction inputAction) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h)
                .Select(x => x.ReadValue<Vector2>())
                .ToReadOnlyReactiveProperty(Vector2.zero);
        }
    }
}
