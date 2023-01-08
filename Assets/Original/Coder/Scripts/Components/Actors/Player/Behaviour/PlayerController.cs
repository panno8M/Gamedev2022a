using System;
using UniRx;
using Utilities;
using Assembly.GameSystem.Input;

namespace Assembly.Components.Actors.Player
{
  public class PlayerController : ActorBehavior<PlayerAct>
  {
    InputControl control;
    [Zenject.Inject]
    public void DepsInject(InputControl control)
    {
      this.control = control;
    }

    public enum Methods { AllOperable, IgnoreAll }
    ReactiveProperty<Methods> ControlMethod = new ReactiveProperty<Methods>();
    public bool isAllOperable => ControlMethod.Value == Methods.AllOperable && isActiveAndEnabled;
    public bool isIgnoreAll => ControlMethod.Value == Methods.IgnoreAll || !isActiveAndEnabled;

    public static int Left = -1;
    public static int Idle = 0;
    public static int Right = 1;


    Subject<Unit> _Up = new Subject<Unit>();
    public IObservable<Unit> Up => _Up;
    Subject<Unit> _Interact = new Subject<Unit>();
    public IObservable<Unit> Interact => _Interact;
    Subject<Unit> _Respawn = new Subject<Unit>();
    public IObservable<Unit> Respawn => _Respawn;
    Subject<Unit> _BreathPress = new Subject<Unit>();
    public IObservable<Unit> BreathPress => _BreathPress;

    ReactiveProperty<UnityEngine.Vector3> _MousePosStage = new ReactiveProperty<UnityEngine.Vector3>();
    public IObservable<UnityEngine.Vector3> MousePosStage => _MousePosStage;
    public UnityEngine.Vector3 mousePosStage
    {
      get => _MousePosStage.Value;
      private set => _MousePosStage.Value = value;
    }

    ReactiveProperty<float> _Horizontal = new ReactiveProperty<float>(0);
    public IObservable<float> Horizontal => _Horizontal;
    public float horizontal
    {
      set
      {
        _Horizontal.Value = value;
        lookDirection = (int)value;
      }
      get
      {
        return _Horizontal.Value;
      }
    }

    int _lookDirection = Right;
    public int lookDirection
    {
      get { return _lookDirection; }
      private set
      {
        if (value != -1 && value != 1) { return; }

        if (value != lookDirection)
        {
          transform.localScale = transform.localScale.SignX(value);
          _lookDirection = value;
        }
      }
    }

    protected override void OnAssemble()
    {
      ControlMethod.Value = Methods.AllOperable;
      horizontal = Idle;
      lookDirection = Right;
    }

    protected override void Blueprint()
    {
      control.GoUpFixed
        .Where(_ => isAllOperable)
        .Multicast(_Up)
        .Connect()
        .AddTo(this);

      control.InteractFixed
        .Where(_ => isAllOperable)
        .Multicast(_Interact)
        .Connect()
        .AddTo(this);

      control.RespawnFixed
        .Where(_ => isAllOperable)
        .Multicast(_Respawn)
        .Connect()
        .AddTo(this);

      control.BreathPressFixed
        .Where(_ => isAllOperable)
        .Multicast(_BreathPress)
        .Connect()
        .AddTo(this);

      control.MousePosStage
        .Where(_ => isAllOperable)
        .Subscribe(pos => mousePosStage = pos)
        .AddTo(this);

      control.HorizontalMoveInput
        .Where(_ => isAllOperable)
        .Subscribe(hmi => horizontal = hmi)
        .AddTo(this);
      ControlMethod
        .Where(_ => isIgnoreAll)
        .Subscribe(hmi => horizontal = 0);
    }
  }
}