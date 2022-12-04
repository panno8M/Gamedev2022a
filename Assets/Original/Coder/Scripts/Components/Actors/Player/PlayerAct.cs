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
    public enum Direction { Left = -1, Right = 1 }
    public enum ControlMethods { ActiveAll, IgnoreAnyInput }

    #region forBehaviourControlling
    IObservable<Unit> _onJump;
    IObservable<Unit> _onLand;

    Subject<Direction> _whileWalking = new Subject<Direction>();
    Subject<Direction> _whileSkywalking = new Subject<Direction>();
    Subject<Unit> _afterBehavior = new Subject<Unit>();

    public IObservable<Direction> WhileWalking => _whileWalking;
    public IObservable<Direction> WhileSkywalking => _whileSkywalking;
    public IObservable<Unit> AfterBehavior => _afterBehavior;

    public ReactiveProperty<float> _HorizontalMove = new ReactiveProperty<float>();
    public IObservable<float> HorizontalMove => _HorizontalMove;
    public float horizontalMove => _HorizontalMove.Value;

    public ReactiveProperty<Direction> _LookDir = new ReactiveProperty<Direction>(Direction.Right);
    public IObservable<Direction> LookDir => _LookDir;
    public Direction lookDir => _LookDir.Value;

    public ReactiveProperty<ControlMethods> ControlMethod = new ReactiveProperty<ControlMethods>();
    public bool isControlAccepting => ControlMethod.Value != ControlMethods.IgnoreAnyInput;

    public IObservable<Unit> OnJump => _onJump ??
        (_onJump = Global.Control.GoUp
            .Where(_ => _isOnGround.Value));
    public IObservable<int> OnFlapWhileFalling => OnFlap.Where(x => x == 1);
    public IObservable<int> OnFlap => _flapCtl.OnFlap;
    public IObservable<Unit> OnLand => _onLand ??
        (_onLand = _isOnGround
            .Where(x => x)
            .AsUnitObservable());
    #endregion

    #region editable params
    [SerializeField] float groundNormalDegreeThreshold;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] Interactor _interactor;

    [SerializeField] PlayerParam _param = new PlayerParam();
    public PlayerParam param => _param;
    #endregion

    #region behaviour statements
    [SerializeField] ReactiveProperty<bool> _isOnGround = new ReactiveProperty<bool>();
    [SerializeField] PlayerFlapCtl _flapCtl = new PlayerFlapCtl(1);
    [SerializeField] float _wallCollidingDirection;
    #endregion

    #region accessors
    public IDamagable damagable => _damagable;
    public Interactor interactor => _interactor;
    public PlayerFlapCtl flapCtl => _flapCtl;
    public ReadOnlyReactiveProperty<bool> isOnGround => _isOnGround.ToReadOnlyReactiveProperty();
    #endregion

    protected override void OnAssemble()
    {
      ControlMethod.Value = ControlMethods.ActiveAll;
      transform.position = Global.PlayerPool.activeSpawnPoint.position;
      var ls = transform.localScale;
      transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
      _LookDir.Value = Direction.Right;
      _wallCollidingDirection = 0;
    }

    protected override void Blueprint()
    {
      this.OnCollisionStayAsObservable()
          .Where(_ => isControlAccepting)
          .Subscribe(collision =>
          {
            foreach (var contact in collision.contacts)
            {
              if (Vector2.Dot(contact.normal, Vector3.up) >= Mathf.Cos(groundNormalDegreeThreshold * Mathf.PI / 360f))
              {
                _isOnGround.Value = true;
                _flapCtl.ResetCount();
              }
              else
              {
                _wallCollidingDirection =
                      (contact.normal.x > 0) ? -1 :
                      (contact.normal.x < 0) ? 1 :
                      0;
              }
            }
          });
      this.OnCollisionExitAsObservable()
          .Where(_ => isControlAccepting)
          .Subscribe(collision =>
          {
            _isOnGround.Value = false;
            _wallCollidingDirection = 0;
          });

      this.FixedUpdateAsObservable()
          .Where(_ => isControlAccepting)
          .Select(_ => horizontalMove)
          .Subscribe(hmi =>
          {
            if (hmi != 0 && hmi != _wallCollidingDirection)
            {
              if (isOnGround.Value)
              {
                _whileWalking.OnNext(lookDir);
              }
              else
              {
                _whileSkywalking.OnNext(lookDir);
              }
            }
            _afterBehavior.OnNext(Unit.Default);
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
          .Where(_ => !_isOnGround.Value)
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
        .Subscribe(hmi => _HorizontalMove.Value = hmi)
        .AddTo(this);
      ControlMethod
        .Where(x => x == ControlMethods.IgnoreAnyInput)
        .Subscribe(hmi => _HorizontalMove.Value = 0);

      HorizontalMove
          .Select(CalcCurrentDirection)
          .Where(dir => dir != lookDir)
          .Subscribe(dir =>
          {
            transform.localScale = transform.localScale.Xyz();
            _LookDir.Value = dir;
          })
          .AddTo(this);

      Direction CalcCurrentDirection(float hmi)
      {
        return hmi != 0 ? (Direction)hmi : lookDir;
      }
    }
  }
}