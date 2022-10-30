using UnityEngine;
using UniRx;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviour : MonoBehaviour
{
    public enum FlyMode{HuwaHuwa, Kirby, Pit}

    #region editable params
    [SerializeField] Animator anim;
    [SerializeField] float _scaleJumpHeight;
    [SerializeField] float _scaleSoarHeight;
    [SerializeField] float _scaleMoveSpeed;

    void Reset() {
        _scaleJumpHeight = 6f;
        _scaleSoarHeight = 6f;
        _scaleMoveSpeed  = 3f;
    }
    #endregion

    #region forPrototyping
    [SerializeField] FlyMode _flyMode;
    #endregion

    #region assets
    Rigidbody2D rb;
    #endregion

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        Player.Instance.OnJump.Subscribe(_ => {
            var jumpForce = Vector2.up * rb.gravityScale * _scaleJumpHeight;
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
        }).AddTo(this);

        Player.Instance.OnFlapWhileFalling.Subscribe(_ => {
            anim.SetBool("Fly", true);
            switch(_flyMode) {
                case FlyMode.HuwaHuwa:
                    break;
                case FlyMode.Kirby:
                    //一瞬慣性を止める
                    rb.velocity = Vector2.zero;
                    break;
                case FlyMode.Pit:
                    //一瞬慣性を止める
                    rb.velocity = Vector2.zero;
                    break;
            }
        }).AddTo(this);

        Player.Instance.OnFlap.Subscribe(_ => {
            rb.gravityScale = 0.6f;

            switch(_flyMode) {
                case FlyMode.HuwaHuwa:
                    rb.velocity = new Vector2(rb.velocity.x, rb.gravityScale * _scaleSoarHeight);
                    break;
                case FlyMode.Kirby:
                    //落下中なら一瞬で上昇開始
                    if (rb.velocity.y < 0f){
                        Vector2 verticalStop = rb.velocity;
                        verticalStop.y = _scaleSoarHeight;
                        rb.velocity = verticalStop;
                    }

                    //現在の移動速度の差分だけ加速させることで無限加速を阻止
                    rb.AddForce(new Vector2(0f,2.5f * rb.gravityScale * (_scaleSoarHeight - rb.velocity.y)));
                    break;
                case FlyMode.Pit:
                    rb.velocity = new Vector2(rb.velocity.x, 10f);
                    break;
            }
        }).AddTo(this);

        Player.Instance.WhileFlying
            .WithLatestFrom(Player.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                if (hmi == 0){
                    Vector2 horizontalStop = rb.velocity;
                    horizontalStop.x = Vector2.zero.x;
                    rb.velocity = horizontalStop;
                } else {
                    MoveHorizontal(hmi);
                }
            }).AddTo(this);

        Player.Instance.OnLand.Subscribe(_ => {
            // FIXME:
            rb.gravityScale = 5f;
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
        rb.velocity = new Vector2(
            hmi * _scaleMoveSpeed,
            rb.velocity.y);
    }

}
