using System;
using UnityEngine;
using UniRx;
using UniRx.Ex.Inputs;

namespace Assembly.Components.Input.Internal
{
  public class InputControlProvider
  {
    public InputSystem input;

    public ReadOnlyReactiveProperty<float> HorizontalMoveInput;

    public ReadOnlyReactiveProperty<bool> GoUpInput;
    public IObservable<Unit> GoUp;

    public ReadOnlyReactiveProperty<bool> BreathInput;
    public IObservable<Unit> BreathPress;
    public IObservable<Unit> BreathRelease;

    public ReadOnlyReactiveProperty<Vector2> MousePosInput;
    public ReadOnlyReactiveProperty<Vector3> MousePosStage;

    public ReadOnlyReactiveProperty<bool> InteractInput;
    public IObservable<Unit> Interact;

    public InputControlProvider()
    {
      input = new InputSystem();
      input.Enable();

      HorizontalMoveInput = input.Player.HorizontalMove.AsAxis();
      GoUpInput = input.Player.GoUp.AsButton();
      BreathInput = input.Player.Breath.AsButton();
      MousePosInput = input.Player.MousePos.As2dAxis();
      InteractInput = input.Player.Interact.AsButton();

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
    }

    static LayerMask stsc = new Layers(Layer.ScreenToStageConverter);
    public static bool MousePos_ScreenToGameStage(Vector3 screenPos, out Vector3 stagePos)
    {
      screenPos.z = 1.0f;
      var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
      var camPos = Camera.main.transform.position;
      if (Physics.Raycast(camPos, (worldPos - camPos), out RaycastHit hit, 100f, stsc))
      {
        Debug.DrawRay(camPos, hit.point, Color.red);
        stagePos = hit.point;
        stagePos.z = 0.0f;
        return true;
      }
      else
      {
        stagePos = Vector3.zero;
        return false;
      }
    }
  }
}
