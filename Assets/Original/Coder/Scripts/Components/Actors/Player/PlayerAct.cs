using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Ex.InteractionTraits.Core;
using Senses;
using Senses.Pain;
using Assembly.Components.Actors.Player.Pure;

namespace Assembly.Components.Actors
{
  public class PlayerAct : UniqueBehaviour<PlayerAct>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }


    public enum Direction { Left = -1, Right = 1 }
    public enum ControlMethod { ActiveAll, IgnoreAnyInput }

    #region forBehaviourControlling
    IObservable<Unit> _onJump;
    IObservable<Unit> _onLand;

    Subject<Direction> _whileWalking = new Subject<Direction>();
    Subject<Direction> _whileSkywalking = new Subject<Direction>();
    Subject<Unit> _afterBehavior = new Subject<Unit>();
    ReadOnlyReactiveProperty<float> _horizontalMove;

    public IObservable<Direction> WhileWalking => _whileWalking;
    public IObservable<Direction> WhileSkywalking => _whileSkywalking;
    public IObservable<Unit> AfterBehavior => _afterBehavior;

    public ReadOnlyReactiveProperty<float> HorizontalMove => _horizontalMove ?? (_horizontalMove = Global.Control.HorizontalMoveInput
        .Where(hmi => isControlAccepting)
        .Merge(controlMethod.Where(x => x == ControlMethod.IgnoreAnyInput).Select(x => 0f))
        .ToReadOnlyReactiveProperty()
        );

    public ReactiveProperty<Direction> LookDir = new ReactiveProperty<Direction>(Direction.Right);
    public ReactiveProperty<ControlMethod> controlMethod = new ReactiveProperty<ControlMethod>();
    public bool isControlAccepting => controlMethod.Value != ControlMethod.IgnoreAnyInput;

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

    public void InitializeCondition()
    {
      damagable.Repair();
      controlMethod.Value = ControlMethod.ActiveAll;
      transform.position = Global.PlayerRespawn.activeSpawnPoint.position;
      var ls = transform.localScale;
      transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
      LookDir.Value = Direction.Right;
      _wallCollidingDirection = 0;
    }

    void Awake()
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
          .Select(_ => HorizontalMove.Value)
          .Subscribe(hmi =>
          {
            if (hmi != 0 && hmi != _wallCollidingDirection)
            {
              if (isOnGround.Value)
              {
                _whileWalking.OnNext(LookDir.Value);
              }
              else
              {
                _whileSkywalking.OnNext(LookDir.Value);
              }
            }
            _afterBehavior.OnNext(Unit.Default);
          });


      HorizontalMove
          .Select(hmi => (hmi != 0)
                    ? (Direction)hmi
                    : LookDir.Value)
          .Where(dir => dir != LookDir.Value)
          .Subscribe(dir =>
          {
            transform.localScale = transform.localScale.Xyz();
            LookDir.Value = dir;
          })
          .AddTo(this);

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
  }
}