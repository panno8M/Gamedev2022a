using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Player : UniqueBehaviour<Player> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }
    public enum Direction {Left = -1, Right = 1}

#region forBehaviourControling
    public ReactiveProperty<Direction> LookDir = new ReactiveProperty<Direction>(Direction.Right);
#endregion

#region params
    [SerializeField] float groundNormalDegreeThreshold;
    [SerializeField] Damagable damagable;
    [SerializeField] AiVisible aiVisible;
#endregion
    public Damagable Damagable => damagable;
    public AiVisible AiVisible => aiVisible;
    public float wallCollidingBias => _wallCollidingBias;
    public bool isFlapping => _isFlapping;
    public ReadOnlyReactiveProperty<bool> isOnGround => _isOnGround.ToReadOnlyReactiveProperty();

#region statements
    [SerializeField] ReactiveProperty<bool> _isOnGround = new ReactiveProperty<bool>();
    [SerializeField] bool _isFlapping;
    [SerializeField] float _wallCollidingBias;

    public IObservable<Unit> OnJump;

    public IObservable<Unit> OnFlapWhileFalling;
    public IObservable<Unit> OnFlap;
    public IObservable<Unit> OnLand;

    public IObservable<long> WhileLanding;
#endregion

    void Awake() {

        OnJump = Global.Control.GoUp
            .Where(_ => _isOnGround.Value);

        OnFlap = Global.Control.GoUp
            .Where(_ => !_isOnGround.Value);

        OnFlapWhileFalling = OnFlap
            .Where(_ => !_isFlapping);

        OnLand = _isOnGround
            .Where(x => x)
            .AsUnitObservable();

    }

    void Start() {
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
    }
}
