using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Ex.InteractionTraits.Core;
using Assembly.Components.Senses;
using Assembly.Components.Actors.Player.Pure;

namespace Assembly.Components.Actors
{
  public class PlayerAct : UniqueBehaviour<PlayerAct>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }


    public enum Direction { Left = -1, Right = 1 }

    #region forBehaviourControlling
    IObservable<Unit> _onJump;
    IObservable<Unit> _onLand;

    public ReactiveProperty<Direction> LookDir = new ReactiveProperty<Direction>(Direction.Right);

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
    [SerializeField] DamagableWrapper damagable;
    [SerializeField] AiVisible aiVisible;
    [SerializeField] Interactor _interactor;
    #endregion

    #region behaviour statements
    [SerializeField] ReactiveProperty<bool> _isOnGround = new ReactiveProperty<bool>();
    [SerializeField] PlayerFlapCtl _flapCtl = new PlayerFlapCtl(1);
    [SerializeField] float _wallCollidingBias;
    #endregion

    #region accessors
    public DamagableWrapper Damagable => damagable;
    public AiVisible AiVisible => aiVisible;
    public Interactor interactor => _interactor;
    public float wallCollidingBias => _wallCollidingBias;
    public PlayerFlapCtl flapCtl => _flapCtl;
    public ReadOnlyReactiveProperty<bool> isOnGround => _isOnGround.ToReadOnlyReactiveProperty();
    #endregion


    void Awake()
    {
      this.OnCollisionStayAsObservable()
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
                _wallCollidingBias =
                      (contact.normal.x > 0) ? -1 :
                      (contact.normal.x < 0) ? 1 :
                      0;
              }
            }
          });
      this.OnCollisionExitAsObservable()
          .Subscribe(collision =>
          {
            _isOnGround.Value = false;
            _wallCollidingBias = 0;
          });

      Global.Control.HorizontalMoveInput
          .Select(hmi =>
                  (hmi == 1) ? Direction.Right :
                  (hmi == -1) ? Direction.Left :
                  LookDir.Value)
          .Where(dir => dir != LookDir.Value)
          .Subscribe(dir =>
          {
            transform.localScale *= new Vector2(-1, 1);
            LookDir.Value = dir;
          })
          .AddTo(this);

      Damagable.OnBroken
          .Subscribe(_ => _interactor.Forget());

      Global.Control.Interact
          .Subscribe(_ =>
          {
            _interactor.Process();
          }).AddTo(this);

      Global.Control.Respawn
          .Subscribe(_ =>
          {
            Damagable.Break();
          }).AddTo(this);

      Global.Control.GoUp
          .Where(_ => !_interactor.holder.hasItem)
          .Where(_ => !_isOnGround.Value)
          .Subscribe(_ =>
          {
            _flapCtl.Inc();
          }).AddTo(this);

    }
  }
}