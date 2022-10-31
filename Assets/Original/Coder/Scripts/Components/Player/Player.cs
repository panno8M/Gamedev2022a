using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(PlayerControl))]
public class Player : MonoBehaviour
{
#region singleton
    static Player instance;
    static PlayerControl control;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() {
        instance = null;
        control = null;
    }

    public static Player Instance => instance ?? (instance = (Player)FindObjectOfType(typeof(Player)));
    public static PlayerControl Control => control ?? (control = Instance.GetComponent<PlayerControl>());
#endregion

    enum Direction {Left = -1, Right = 1}

#region forBehaviourControling
    Direction lookAt = Direction.Right;
#endregion

#region params
    [SerializeField] float groundNormalDegreeThreshold;
    [SerializeField] Damagable damagable;
    [SerializeField] AiVisible aiVisible;
#endregion
    public Damagable Damagable => damagable;
    public AiVisible AiVisible => aiVisible;
    public float wallCollidingBias => _wallCollidingBias;

#region statements
    [SerializeField] ReactiveProperty<bool> _isOnGround = new ReactiveProperty<bool>();
    [SerializeField] bool _isFlapping;
    [SerializeField] float _wallCollidingBias;

    public IObservable<Unit> OnJump;

    public IObservable<Unit> OnFlapWhileFalling;
    public IObservable<Unit> OnFlap;
    public IObservable<Unit> OnLand;

    public IObservable<long> WhileFlying;
    public IObservable<long> WhileNotFlying;
    public IObservable<long> WhileLanding;

#endregion

    void Awake() {
        OnJump = Control.GoUp
            .Where(_ => _isOnGround.Value);

        OnFlap = Control.GoUp
            .Where(_ => !_isOnGround.Value);

        OnFlapWhileFalling = OnFlap
            .Where(_ => !_isFlapping);

        OnLand = _isOnGround
            .Where(x => x)
            .AsUnitObservable();

        WhileFlying = Observable
            .EveryFixedUpdate()
            .Where(_ => _isFlapping);

        WhileNotFlying = Observable
            .EveryFixedUpdate()
            .Where(_ => !_isFlapping);

        WhileLanding = Observable
            .EveryFixedUpdate()
            .Where(_ => _isOnGround.Value);
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

        Control.HorizontalMoveInput
            .Select(hmi =>
                    (hmi ==  1) ? Direction.Right :
                    (hmi == -1) ? Direction.Left  :
                    lookAt)
            .Where(dir => dir != lookAt)
            .Subscribe(dir => {
                transform.localScale *= new Vector2(-1, 1);
                lookAt = dir;
                })
            .AddTo(this);
    }
}
