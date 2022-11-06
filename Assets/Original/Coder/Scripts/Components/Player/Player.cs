using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

using DamageTraits.UnityBridge;

public class Player : UniqueBehaviour<Player> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }
    public enum Direction {Left = -1, Right = 1}

#region forBehaviourControlling
    IObservable<Unit> _onJump;
    IObservable<Unit> _onFlapWhileFalling;
    IObservable<Unit> _onFlap;
    IObservable<Unit> _onLand;

    IObservable<Unit> _onBreathStart;
    IObservable<Unit> _onBreathStop;

    public ReactiveProperty<Direction> LookDir = new ReactiveProperty<Direction>(Direction.Right);

    public IObservable<Unit> OnJump => _onJump ??
        (_onJump = Global.Control.GoUp
            .Where(_ => _isOnGround.Value));
    public IObservable<Unit> OnFlapWhileFalling => _onFlapWhileFalling ??
        (_onFlapWhileFalling = OnFlap
            .Where(_ => !_isFlapping));
    public IObservable<Unit> OnFlap => _onFlap ??
        (_onFlap = Global.Control.GoUp
            .Where(_ => !_isOnGround.Value));
    public IObservable<Unit> OnLand => _onLand ??
        (_onLand = _isOnGround
            .Where(x => x)
            .AsUnitObservable());

    public IObservable<Unit> OnBreathStart => _onBreathStart ??
        (_onBreathStart = Global.Control.BreathPress
            .Where(_ => !Interactor.HoldingItem.Value));

    public IObservable<Unit> OnBreathStop => _onBreathStop ??
        (_onBreathStop = Observable.Merge(
            Global.Control.BreathRelease,
            Interactor.OnHoldRequested.AsUnitObservable()));
#endregion

    #region editable params
    [SerializeField] float groundNormalDegreeThreshold;
    [SerializeField] DamagableWrapper damagable;
    [SerializeField] AiVisible aiVisible;
    [SerializeField] Interactor interactor;
    #endregion

    #region behaviour statements
    [SerializeField] ReactiveProperty<bool> _isOnGround = new ReactiveProperty<bool>();
    [SerializeField] bool _isFlapping;
    [SerializeField] float _wallCollidingBias;
    #endregion

    #region accessors
    public DamagableWrapper Damagable => damagable;
    public AiVisible AiVisible => aiVisible;
    public Interactor Interactor => interactor;
    public float wallCollidingBias => _wallCollidingBias;
    public bool isFlapping => _isFlapping;
    public ReadOnlyReactiveProperty<bool> isOnGround => _isOnGround.ToReadOnlyReactiveProperty();
    #endregion


    void Awake() {
        this.OnCollisionStayAsObservable()
            .Subscribe(collision => {
                foreach (var contact in collision.contacts) {
                    if (Vector2.Dot(contact.normal, Vector3.up) >= Mathf.Cos(groundNormalDegreeThreshold * Mathf.PI / 360f)) {
                        _isOnGround.Value = true;
                        _isFlapping = false;
                    } else {
                        _wallCollidingBias =
                            (contact.normal.x > 0) ? -1 :
                            (contact.normal.x < 0) ?  1 :
                            0;
                    }
                }
            });
        this.OnCollisionExitAsObservable()
            .Subscribe(collision => {
                _isOnGround.Value = false;
                _wallCollidingBias = 0;
            });

        OnFlapWhileFalling
            .Subscribe(_ => _isFlapping = true)
            .AddTo(this);

        Global.Control.HorizontalMoveInput
            .Select(hmi =>
                    (hmi ==  1) ? Direction.Right :
                    (hmi == -1) ? Direction.Left  :
                    LookDir.Value)
            .Where(dir => dir != LookDir.Value)
            .Subscribe(dir => {
                transform.localScale *= new Vector2(-1, 1);
                LookDir.Value = dir;
                })
            .AddTo(this);

        Damagable.OnBroken
            .Subscribe(_ => Interactor.HoldingItem.Value = null);

        Global.Control.Interact
            .Subscribe(_ => Interactor.Interact())
            .AddTo(this);

    }
}
