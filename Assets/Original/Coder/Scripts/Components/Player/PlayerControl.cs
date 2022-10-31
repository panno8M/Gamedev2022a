using System;
using UnityEngine;
using UniRx;

public class PlayerControl : MonoBehaviour
{
    public InputControl input;
    public ReactiveProperty<float> _horizontalMove = new ReactiveProperty<float>();
    public IObservable<float> HorizontalMoveInput => _horizontalMove;
    public IObservable<float> WhileHorizontalMoving;

    Subject<Unit> _goUp = new Subject<Unit>();
    IObservable<Unit> GoUp_internal;
    public IObservable<Unit> GoUp => GoUp_internal ??
        (GoUp_internal = _goUp
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .Share());

    void Awake() {
        input = new InputControl();
        input.Enable();
        input.Player.HorizontalMove.performed += context => {
            _horizontalMove.Value = context.ReadValue<float>();
        };
        input.Player.GoUp.performed += context => {
            if (context.ReadValue<float>() == 1)
                _goUp.OnNext(Unit.Default);
        };

        WhileHorizontalMoving = Observable
            .EveryFixedUpdate()
            .WithLatestFrom(_horizontalMove, (_,hmi) => hmi)
            .Share();

    }
}
