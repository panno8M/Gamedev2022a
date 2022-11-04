using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public static float G = -9.8f;

    [System.Serializable]
    public struct BehaviourScale {
        public BehaviourScale(float jumpHeight, float soarHeight, float moveSpeed) {
            this.jumpHeight = jumpHeight;
            this.soarHeight = soarHeight;
            this.moveSpeed = moveSpeed;
        }
        public float jumpHeight;
        public float soarHeight;
        public float moveSpeed;
    }
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
    [SerializeField] BehaviourScale _scaleBehaviour;
    [SerializeField] GravityScale _scaleGravity;

    void Reset() {
        _scaleBehaviour  = new BehaviourScale(5f, 3f, 3.5f);
        _scaleGravity  = new GravityScale(1.3f, .1f);
    }
    #endregion

    #region assets
    Rigidbody rb;
    #endregion

    void Awake() {
        rb = GetComponent<Rigidbody>();
        var player = Global.Player;
        this.FixedUpdateAsObservable()
            .Subscribe(_ => {
                rb.AddForce((G*_scaleGravity.normal)._y_(), ForceMode.Acceleration);
            });

        player.OnJump.Subscribe(_ => {
            rb.AddForce(_scaleBehaviour.jumpHeight._y_(), ForceMode.Impulse);
        }).AddTo(this);

        player.OnFlapWhileFalling.Subscribe(_ => {
            rb.velocity = rb.velocity.x_z();
            rb.AddForce(_scaleBehaviour.jumpHeight._y_(), ForceMode.Impulse);
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
            .Where(hmi => hmi != 0)
            .Subscribe(hmi => {
                MoveHorizontal(hmi);
            }).AddTo(this);
        
        Global.Control.MousePosStage
            .Subscribe(breathFire.transform.LookAt)
            .AddTo(this);
        player.LookDir
            .Subscribe(_ => breathFire.transform.LookAt(Global.Control.MousePosStage.Value))
            .AddTo(this);

        Global.Control.DoBreath
            .Subscribe(b => {
                if (b){ breathFire.Play(); }
                else  { breathFire.Stop(); }
            }).AddTo(this);

        Global.Control.HorizontalMoveInput
            .Where(hmi => hmi == 0)
            .Subscribe(_ => MoveHorizontal(0))
            .AddTo(this);
        Global.Control.HorizontalMoveInput
            .Subscribe(hmi => anim.SetBool("Walk", hmi != 0))
            .AddTo(this);
    }

    void MoveHorizontal(float hmi) {
        rb.velocity = new Vector3(
            hmi * _scaleBehaviour.moveSpeed,
            rb.velocity.y,
            0);
    }
}
