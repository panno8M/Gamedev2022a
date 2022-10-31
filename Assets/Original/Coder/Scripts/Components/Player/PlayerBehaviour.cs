using UnityEngine;
using UniRx;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public static float G = 9.8f;
    public enum FlyMode{HuwaHuwa, Kirby, Pit}

    [System.Serializable]
    public struct GravityScale {
        public GravityScale(float normal = 5f, float flying = .6f) {
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
        _scaleJumpHeight = 20f;
        _scaleSoarHeight = 6f;
        _scaleMoveSpeed  = 3f;
        _scaleGravity  = new GravityScale(5f, .6f);
    }
    #endregion

    #region forPrototyping
    [SerializeField] FlyMode _flyMode;
    #endregion

    #region assets
    Rigidbody rb;
    #endregion

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        Player.Instance.OnJump.Subscribe(_ => {
            var jumpForce = Vector3.up * _scaleGravity.normal * _scaleJumpHeight;
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }).AddTo(this);

        Player.Instance.OnFlapWhileFalling.Subscribe(_ => {
            anim.SetBool("Fly", true);
            switch(_flyMode) {
                case FlyMode.HuwaHuwa:
                    break;
                case FlyMode.Kirby:
                    //一瞬慣性を止める
                    rb.velocity = Vector3.zero;
                    break;
                case FlyMode.Pit:
                    //一瞬慣性を止める
                    rb.velocity = Vector3.zero;
                    break;
            }
        }).AddTo(this);

        Player.Instance.OnFlap.Subscribe(_ => {
            switch(_flyMode) {
                case FlyMode.HuwaHuwa:
                    rb.velocity = new Vector3(rb.velocity.x, _scaleGravity.flying * _scaleSoarHeight, rb.velocity.z);
                    break;
                case FlyMode.Kirby:
                    //落下中なら一瞬で上昇開始
                    if (rb.velocity.y < 0f){
                        Vector3 verticalStop = rb.velocity;
                        verticalStop.y = _scaleSoarHeight;
                        rb.velocity = verticalStop;
                    }

                    rb.velocity = new Vector3(rb.velocity.x, _scaleGravity.flying * _scaleSoarHeight, rb.velocity.z);
                    break;
                case FlyMode.Pit:
                    rb.velocity = new Vector3(rb.velocity.x, 10f, rb.velocity.z);
                    break;
            }
        }).AddTo(this);

        Player.Instance.WhileFlying
            .WithLatestFrom(Player.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                rb.AddForce(G * _scaleGravity.flying._Y_(), ForceMode.Acceleration);
                if (hmi == 0){
                    rb.velocity = rb.velocity._y_();
                } else {
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
