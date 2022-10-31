using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using ReactiveInput;

public class Control: UniqueBehaviour<Control> {

    [Serializable]
    struct Inspector {
        public float horizontalMove;
        public bool goUp;
    }
    [SerializeField]
    Inspector inspector;

    public InputControl input;

    public ReadOnlyReactiveProperty<float> HorizontalMoveInput;
    public IObservable<float> WhileHorizontalMoving;

    public ReadOnlyReactiveProperty<bool> GoUpInput;
    public IObservable<Unit> GoUp;

    void Awake() {
        input = new InputControl();
        input.Enable();

        HorizontalMoveInput = input.Player.HorizontalMove.AsAxis();
        GoUpInput = input.Player.GoUp.AsButton();

        WhileHorizontalMoving = Observable
            .EveryFixedUpdate()
            .WithLatestFrom(HorizontalMoveInput, (_,hmi) => hmi)
            .Share();

        GoUp = GoUpInput
            .Where(x => x)
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share();

        HorizontalMoveInput.Subscribe(x => inspector.horizontalMove = x).AddTo(this);
        GoUpInput.Subscribe(x => inspector.goUp = x).AddTo(this);

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
    }
}