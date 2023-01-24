using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem.Input;

namespace Assembly.GameSystem.Internal
{
  public class InputControlProvider
  {
    Camera _mainCamera;
    Camera mainCamera => _mainCamera ? _mainCamera : (_mainCamera = Camera.main);

    public InputSystem input;

    public ReactiveProperty<float> HorizontalMoveInput = new ReactiveProperty<float>();
    public ReactiveProperty<bool> GoUpInput = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> BreathInput = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> RespawnInput = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> PauseInput = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> InteractInput = new ReactiveProperty<bool>();
    public ReactiveProperty<Vector2> MousePosInput = new ReactiveProperty<Vector2>();
    public ReactiveProperty<Vector3> MousePosStage = new ReactiveProperty<Vector3>();

    public Subject<Unit> GoUpFixed = new Subject<Unit>();
    public Subject<Unit> BreathPressFixed = new Subject<Unit>();
    public Subject<Unit> BreathReleaseFixed = new Subject<Unit>();
    public Subject<Unit> RespawnFixed = new Subject<Unit>();
    public Subject<Unit> PauseFixed = new Subject<Unit>();
    public Subject<Unit> InteractFixed = new Subject<Unit>();


    public InputControlProvider()
    {
      input = new InputSystem();
      input.Enable();

      input.Player.HorizontalMove.AsAxis(HorizontalMoveInput);
      input.Player.MousePos.AsAxis2d(MousePosInput);
      input.Player.GoUp.AsButton(GoUpInput, GoUpFixed);
      input.Player.Breath.AsButton(BreathInput, BreathPressFixed);
      input.Player.Interact.AsButton(InteractInput, InteractFixed);
      input.Player.Respawn.AsButton(RespawnInput, RespawnFixed);
      input.Player.Pause.AsButton(PauseInput, PauseFixed);


      Observable.EveryUpdate()
        .Where(_ => mainCamera)
        .Select(_ => mainCamera.transform.position)
        .DistinctUntilChanged()
        .Subscribe(_ =>
          MousePos_ScreenToGameStage(MousePosInput, MousePosStage));
      MousePosInput.Subscribe(_ =>
        MousePos_ScreenToGameStage(MousePosInput, MousePosStage));
    }

    static LayerMask stsc = new Layers(Layer.ScreenToStageConverter);
    RaycastHit hit;
    void MousePos_ScreenToGameStage(
      ReactiveProperty<Vector2> screenPos,
      ReactiveProperty<Vector3> stagePos)
    {
      var worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.Value.x, screenPos.Value.y, 1));
      var camPos = mainCamera.transform.position;
      if (Physics.Raycast(camPos, (worldPos - camPos), out hit, 100f, stsc))
      {
        Debug.DrawRay(camPos, hit.point, Color.red);
        stagePos.Value = new Vector3(hit.point.x, hit.point.y, 0);
      }
    }
  }
}
