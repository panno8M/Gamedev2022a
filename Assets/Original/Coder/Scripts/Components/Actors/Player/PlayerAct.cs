using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Ex.InteractionTraits.Core;
using Senses.Pain;

namespace Assembly.Components.Actors
{
  public class PlayerAct : ActorCore<PlayerAct>
  {
    public enum ControlMethods { ActiveAll, IgnoreAnyInput }
    public enum HoriMoveStat { Left = -1, Idle = 0, Right = 1 }

    #region forBehaviourControlling
    IObservable<Unit> _onJump;
    IObservable<Unit> _onLand;

    Subject<PlayerAct> _Behavior = new Subject<PlayerAct>();
    public IObservable<PlayerAct> Behavior => _Behavior;


    public ReactiveProperty<ControlMethods> ControlMethod = new ReactiveProperty<ControlMethods>();
    public bool isControlAccepting => ControlMethod.Value != ControlMethods.IgnoreAnyInput;

    public IObservable<Unit> OnJump => _onJump ??
        (_onJump = Global.Control.GoUp
            .Where(_ => isOnGround));
    public IObservable<int> OnFlapWhileFalling => OnFlap.Where(x => x == 1);
    public IObservable<int> OnFlap => _flapCtl.OnFlap;
    public IObservable<Unit> OnLand => _onLand ??
        (_onLand = IsOnGround
            .Where(x => x)
            .AsUnitObservable());
    #endregion

    #region editable params
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] Interactor _interactor;

    [SerializeField] PlayerParam _param = new PlayerParam();
    public PlayerParam param => _param;

    A2Dir _lookDirection = new A2Dir();
    public A2Dir lookDirection => _lookDirection;
    public Vector3 lookVector => (Vector3)_lookDirection;
    #endregion

    #region behaviour statements
    bool _isOnGroundSwap;
    bool _obstacleCollidingSwap;
    [SerializeField] ReactiveProperty<bool> _IsOnGround = new ReactiveProperty<bool>();
    public IObservable<bool> IsOnGround => _IsOnGround;
    public bool isOnGround => _IsOnGround.Value;

    [SerializeField] PlayerFlapCtl _flapCtl = new PlayerFlapCtl(1);
    ReactiveProperty<HoriMoveStat> _HoriMove = new ReactiveProperty<HoriMoveStat>();
    public IObservable<HoriMoveStat> HoriMove => _HoriMove;

    public bool obstacleColliding;
    public bool obstacleClimbable;
    public Vector3 obstacleTangent;
    #endregion

    #region accessors
    public IDamagable damagable => _damagable;
    public Interactor interactor => _interactor;
    public PlayerFlapCtl flapCtl => _flapCtl;

    public HoriMoveStat horiMove
    {
      set
      {
        _HoriMove.Value = value;
        var newDir = _lookDirection.CalcNewDirection((int)value);
        if (newDir != _lookDirection.current)
        {
          transform.localScale = transform.localScale.Xyz();
          _lookDirection.current = newDir;
        }
      }
      get
      {
        return _HoriMove.Value;
      }
    }
    #endregion

    protected override void OnAssemble()
    {
      ControlMethod.Value = ControlMethods.ActiveAll;
      transform.position = Global.PlayerPool.activeSpawnPoint.position;

      _HoriMove.Value = 0;
      _lookDirection.Clear();
      obstacleColliding = false;
      obstacleClimbable = false;
      obstacleTangent = Vector3.zero;

      transform.localScale = _lookDirection.SignX(transform.localScale);
    }

    protected override void Blueprint()
    {
      this.OnCollisionStayAsObservable()
          .Subscribe(collision =>
          {
            foreach (var contact in collision.contacts)
            {
              if (Vector3.Angle(contact.normal, Vector3.up) < param.degreeClimbableObstacle)
              {
                _isOnGroundSwap = true;
                _flapCtl.ResetCount();
              }
              else if (Vector3.Angle(contact.normal, -lookVector) < param.degreeUnclimbableObstacle)
              {
                _obstacleCollidingSwap = true;
                obstacleTangent = Quaternion.FromToRotation(Vector3.up, contact.normal) * lookVector;
              }
            }
          });
      this.OnCollisionExitAsObservable()
          .Subscribe(collision =>
          {
            _IsOnGround.Value = false;
            obstacleColliding = false;
          });

      this.FixedUpdateAsObservable()
        .Subscribe(_ =>
        {

          _IsOnGround.Value = _isOnGroundSwap;
          obstacleColliding = _obstacleCollidingSwap;

          _isOnGroundSwap = false;
          _obstacleCollidingSwap = false;

          if (!isControlAccepting) { return; }

          _Behavior.OnNext(this);

          obstacleClimbable = obstacleColliding && !Physics.CheckBox(
            transform.position + lookDirection.FollowX(param.steppableBoxCenter),
            param.steppableBoxExtents,
            Quaternion.identity, new Layers(Layer.Stage, Layer.Dynamics));
        });

      sbsc_MoveAndDirect();

      Global.Control.Interact
          .Where(_ => isControlAccepting)
              .Subscribe(_ =>
              {
                _interactor.Process();
              }).AddTo(this);

      Global.Control.GoUp
          .Where(_ => isControlAccepting)
              .Where(_ => !isOnGround)
              .Subscribe(_ =>
              {
                _flapCtl.Inc();
              }).AddTo(this);

      _interactor.holder.HoldingItem
          .Subscribe(item =>
              {
                if (item)
                {
                  _flapCtl.TightenLimit(0);
                }
                else
                {
                  _flapCtl.ResetLimit();
                }
              }).AddTo(this);

    }

    void sbsc_MoveAndDirect()
    {
      Global.Control.HorizontalMoveInput
        .Where(hmi => isControlAccepting)
        .Subscribe(hmi => horiMove = (HoriMoveStat)hmi)
        .AddTo(this);
      ControlMethod
        .Where(x => x == ControlMethods.IgnoreAnyInput)
        .Subscribe(hmi => horiMove = 0);
    }

    void OnDrawGizmos()
    {
      Gizmos.color = Color.cyan;
      if (lookDirection != null)
        Gizmos.DrawWireCube(
          transform.position + lookDirection.FollowX(param.steppableBoxCenter),
          param.steppableBoxExtents * 2);
    }
  }
}