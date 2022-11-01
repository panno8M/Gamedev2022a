using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using ReactiveInput;

public class Control: UniqueBehaviour<Control> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    [Serializable]
    struct Inspector {
        public float horizontalMove;
        public bool goUp;
        public bool doBreath;
        public Vector2 mousePos;
    }
    [SerializeField]
    Inspector inspector;

    public InputControl input;

    public ReadOnlyReactiveProperty<float> HorizontalMoveInput;
    public IObservable<float> WhileHorizontalMoving;

    public ReadOnlyReactiveProperty<bool> GoUpInput;
    public IObservable<Unit> GoUp;

    public ReadOnlyReactiveProperty<bool> DoBreathInput;
    public IObservable<Unit> DoBreath;

    public ReadOnlyReactiveProperty<Vector2> MousePosInput;
    public IObservable<Vector2> MousePos;

    void Awake() {
        input = new InputControl();
        input.Enable();

        HorizontalMoveInput = input.Player.HorizontalMove.AsAxis();
        GoUpInput = input.Player.GoUp.AsButton();
        DoBreathInput = input.Player.DoBreath.AsButton();
        MousePosInput = input.Player.MousePos.AsPos();

        WhileHorizontalMoving = Observable
            .EveryFixedUpdate()
            .WithLatestFrom(HorizontalMoveInput, (_,hmi) => hmi)
            .Share();

        GoUp = GoUpInput
            .Where(x => x)
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share();

        DoBreath = DoBreathInput
            .Where(x => x)
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share();

         MousePos = Observable
             .EveryFixedUpdate()
             .WithLatestFrom(MousePosInput, (_,hmi) => hmi)
             .Share();

        HorizontalMoveInput.Subscribe(x => inspector.horizontalMove = x).AddTo(this);
        GoUpInput.Subscribe(x => inspector.goUp = x).AddTo(this);
        DoBreathInput.Subscribe(x => inspector.doBreath = x).AddTo(this);
        MousePosInput.Subscribe(x => inspector.mousePos = x).AddTo(this);
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
        public static ReadOnlyReactiveProperty<Vector2> AsPos(this InputAction inputAction) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h)
                .Select(x => x.ReadValue<Vector2>())
                .ToReadOnlyReactiveProperty();
        }
    }
}
