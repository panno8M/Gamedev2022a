using UnityEngine;
using UniRx;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public static float G = 9.8f;

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
    [SerializeField] float _scaleJumpHeight;
    [SerializeField] float _scaleSoarHeight;
    [SerializeField] float _scaleMoveSpeed;
    [SerializeField] GravityScale _scaleGravity;

    void Reset() {
        _scaleJumpHeight = 10f;
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
        Player.Instance.OnJump.Subscribe(_ => {
            rb.AddForce(_scaleJumpHeight._y_(), ForceMode.Impulse);
        }).AddTo(this);

        Player.Instance.OnFlapWhileFalling.Subscribe(_ => {
            anim.SetBool("Fly", true);
        }).AddTo(this);

        Player.Instance.OnFlap.Subscribe(_ => {
            rb.velocity = new Vector3(
                rb.velocity.x,
                _scaleSoarHeight,
                rb.velocity.z);
        }).AddTo(this);

        Player.Instance.WhileFlying
            .WithLatestFrom(Player.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                rb.AddForce(G * _scaleGravity.flying._Y_(), ForceMode.Acceleration);
                if (hmi == 0){
                    rb.velocity = rb.velocity._y_();
                } else {
                    if (Player.Instance.wallCollidingBias != hmi)
                        MoveHorizontal(hmi);
                }
            }).AddTo(this);

        Player.Instance.WhileNotFlying.Subscribe(_ => {
            rb.AddForce(G * _scaleGravity.normal._Y_(), ForceMode.Acceleration);
        }).AddTo(this);

        Player.Instance.OnLand.Subscribe(_ => {
            anim.SetBool("Fly", false);
        }).AddTo(this);

        Player.Instance.WhileLanding
            .WithLatestFrom(Player.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                anim.SetBool("Walk", hmi != 0);
                if (hmi == 0) return;
                MoveHorizontal(hmi);
            }).AddTo(this);
    }

    public void MoveHorizontal(float hmi) {
        rb.velocity = new Vector3(
            hmi * _scaleMoveSpeed,
            rb.velocity.y,
            0);
    }

}
