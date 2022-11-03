using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public static float G = -9.8f;

    [System.Serializable]
    public struct GravityScale {
        public GravityScale(float normal, float flying) {
            this.normal = normal;
            this.flying = flying;
        }
        public float normal;
        public float flying;
    }

    #region editable params
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem breathFire;
    [SerializeField] float _scaleJumpHeight;
    [SerializeField] float _scaleSoarHeight;
    [SerializeField] float _scaleMoveSpeed;
    [SerializeField] GravityScale _scaleGravity;

    void Reset() {
        _scaleJumpHeight = 5f;
        _scaleSoarHeight = 3f;
        _scaleMoveSpeed  = 3.5f;
        _scaleGravity  = new GravityScale(1.3f, .1f);
    }
    #endregion

    #region assets
    Rigidbody rb;
    #endregion

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        var player = Global.Player;
        this.FixedUpdateAsObservable()
            .Subscribe(_ => {
                rb.AddForce((G*_scaleGravity.normal)._y_(), ForceMode.Acceleration);
            });

        player.OnJump.Subscribe(_ => {
            rb.AddForce(_scaleJumpHeight._y_(), ForceMode.Impulse);
        }).AddTo(this);

        player.OnFlapWhileFalling.Subscribe(_ => {
            rb.velocity = rb.velocity.x_z();
            rb.AddForce(_scaleJumpHeight._y_(), ForceMode.Impulse);
        }).AddTo(this);

        this.FixedUpdateAsObservable()
            .Where(_ => player.isFlapping)
            .WithLatestFrom(Global.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                if (hmi == 0 || player.wallCollidingBias != hmi) {
                    MoveHorizontal(hmi);
                }
            });

        this.FixedUpdateAsObservable()
            .Where(_ => player.isOnGround.Value)
            .WithLatestFrom(Global.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                anim.SetBool("Walk", hmi != 0);
                MoveHorizontal(hmi);
            }).AddTo(this);
        
        Global.Control.MousePosStage
            .Subscribe(breathFire.transform.LookAt)
            .AddTo(this);

        Global.Control.DoBreath
            .Subscribe(b => {
                if (b){ breathFire.Play(); }
                else  { breathFire.Stop(); }
            }).AddTo(this);


        player.Damagable.OnBroken
            .Subscribe(_ => Debug.Log("PLAYER DEAD"))
            .AddTo(this);


    }

    void MoveHorizontal(float hmi) {
        rb.velocity = new Vector3(
            hmi * _scaleMoveSpeed,
            rb.velocity.y,
            0);
    }
}
