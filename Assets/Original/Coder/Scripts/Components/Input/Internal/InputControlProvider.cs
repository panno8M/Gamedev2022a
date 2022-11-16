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

    public ReadOnlyReactiveProperty<bool> RespawnInput;
    public IObservable<Unit> Respawn;

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
      RespawnInput = input.Player.Respawn.AsButton();

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

      Respawn = RespawnInput
          .Where(x => x)
          .AsUnitObservable()
          .BatchFrame(0, FrameCountType.FixedUpdate)
          .Share();

      MousePosStage = Camera.main.transform
          .ObserveEveryValueChanged(x => x.position)
          .Select(_ => MousePosInput.Value)
          .Merge(MousePosInput)
          .Select(pos => MousePos_ScreenToGameStage(pos) ?? MousePosStage?.Value ?? Vector3.zero)
          .ToReadOnlyReactiveProperty();

      Interact = InteractInput
          .Where(x => x)
          .AsUnitObservable()
          .BatchFrame(0, FrameCountType.FixedUpdate)
          .Share();
    }

    static LayerMask stsc = new Layers(Layer.ScreenToStageConverter);
    public static Vector3? MousePos_ScreenToGameStage(Vector3 screenPos)
    {
      screenPos.z = 1.0f;
      var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
      var camPos = Camera.main.transform.position;
      if (Physics.Raycast(camPos, (worldPos - camPos), out RaycastHit hit, 100f, stsc))
      {
        Debug.DrawRay(camPos, hit.point, Color.red);
        return new Vector3(hit.point.x, hit.point.y, 0);
      }
      else
      {
        return null;
      }
    }
  }
}
