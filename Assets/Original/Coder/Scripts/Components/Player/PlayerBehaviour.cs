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
    [SerializeField] Vector3 _mousePos;
    [SerializeField] int _canHitRayCastLayerNum;

    void Reset() {
        _scaleJumpHeight = 10f;
        _scaleSoarHeight = 3f;
        _scaleMoveSpeed  = 3.5f;
        _scaleGravity  = new GravityScale(1.3f, .1f);
        _canHitRayCastLayerNum = 11;
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
        player.OnJump.Subscribe(_ => {
            rb.AddForce(_scaleJumpHeight._y_(), ForceMode.Impulse);
        }).AddTo(this);

        player.OnFlapWhileFalling.Subscribe(_ => {
            anim.SetBool("Fly", true);
        }).AddTo(this);

        player.OnFlap.Subscribe(_ => {
            rb.velocity = new Vector3(
                rb.velocity.x,
                _scaleSoarHeight,
                rb.velocity.z);
        }).AddTo(this);

        player.WhileFlying
            .WithLatestFrom(Global.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                rb.AddForce(G * _scaleGravity.flying._Y_(), ForceMode.Acceleration);
                if (hmi == 0){
                    rb.velocity = rb.velocity._y_();
                } else {
                    if (player.wallCollidingBias != hmi)
                        MoveHorizontal(hmi);
                }
            }).AddTo(this);

        player.WhileNotFlying.Subscribe(_ => {
            rb.AddForce(G * _scaleGravity.normal._Y_(), ForceMode.Acceleration);
        }).AddTo(this);

        player.OnLand.Subscribe(_ => {
            anim.SetBool("Fly", false);
        }).AddTo(this);

        player.WhileLanding
            .WithLatestFrom(Global.Control.HorizontalMoveInput, (_, hmi) => hmi)
            .Subscribe(hmi => {
                anim.SetBool("Walk", hmi != 0);
                if (hmi == 0) return;
                MoveHorizontal(hmi);
            }).AddTo(this);
        
        Global.Control.MousePos
            .Subscribe(hmi => {
                MousePosToWorldPoint(hmi);
                })
            .AddTo(this);;

        player.IsBreath
            .Subscribe(pos => {Debug.Log(_mousePos);}).AddTo(this);

    }

    public void MoveHorizontal(float hmi) {
        rb.velocity = new Vector3(
            hmi * _scaleMoveSpeed,
            rb.velocity.y,
            0);
    }

    public void MousePosToWorldPoint(Vector3 hmi){
        RaycastHit hit;
        hmi.z = 1f;
        hmi = Camera.main.ScreenToWorldPoint(hmi);
        if (Physics.Raycast(Camera.main.transform.position, (hmi - Camera.main.transform.position), out hit, Mathf.Infinity,  1 << _canHitRayCastLayerNum)){
            Debug.DrawRay(Camera.main.transform.position, (hmi - Camera.main.transform.position) * hit.distance, Color.red);
            _mousePos = hit.point;
            _mousePos.z = 0.0f;
        }
    }

}
