using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Actors.Behaviour
{
  [RequireComponent(typeof(Rigidbody))]
  public class PlayerBehaviour : MonoBehaviour
  {
    public static float G = -9.8f;

    [System.Serializable]
    public struct BehaviourScale
    {
      public BehaviourScale(float jumpHeight, float soarHeight, float moveSpeed)
      {
        this.jumpHeight = jumpHeight;
        this.soarHeight = soarHeight;
        this.moveSpeed = moveSpeed;
      }
      public float jumpHeight;
      public float soarHeight;
      public float moveSpeed;
    }
    [System.Serializable]
    public struct GravityScale
    {
      public GravityScale(float normal, float flying)
      {
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

    void Reset()
    {
      _scaleBehaviour = new BehaviourScale(5f, 3f, 3.5f);
      _scaleGravity = new GravityScale(1.3f, .1f);
    }
    #endregion

    #region assets
    Rigidbody rb;
    #endregion

    void Awake()
    {
      rb = GetComponent<Rigidbody>();
      var player = Global.Player;

      sbsc_AddGravity();

      player.OnJump.Subscribe(_ => Jump()).AddTo(this);
      player.OnFlapWhileFalling.Subscribe(_ => Jump()).AddTo(this);

      this.FixedUpdateAsObservable()
          .Select(_ => Global.Control.HorizontalMoveInput.Value)
          .Where(hmi => hmi != 0)
          .Where(_ => !player.isOnGround.Value)
          .Where(hmi => hmi != player.wallCollidingBias)
          .Subscribe(MoveHorizontal);

      this.FixedUpdateAsObservable()
          .Select(_ => Global.Control.HorizontalMoveInput.Value)
          .Where(hmi => hmi != 0)
          .Where(_ => player.isOnGround.Value)
          .Where(hmi => hmi != player.wallCollidingBias)
          .Subscribe(MoveHorizontal).AddTo(this);

      this.FixedUpdateAsObservable()
          .Where(_ => Global.Control.BreathInput.Value)
          .Subscribe(_ =>
          {
            breathFire.transform.LookAt(Global.Control.MousePosStage.Value);
          }).AddTo(this);

      #region breath
      player.OnBreathStart
          .Subscribe(_ => breathFire.Play()).AddTo(this);
      player.OnBreathStop
          .Subscribe(_ => breathFire.Stop()).AddTo(this);
      #endregion

      Global.Control.HorizontalMoveInput
          .Where(hmi => hmi == 0)
          .Subscribe(MoveHorizontal)
          .AddTo(this);
      Global.Control.HorizontalMoveInput
          .Subscribe(hmi => anim.SetBool("Walk", hmi != 0))
          .AddTo(this);

      #region hold
      this.FixedUpdateAsObservable()
          .Select(_ => player.interactor.holder.HoldingItem.Value)
          .Where(item => item)
          .Subscribe(item => item.rb.MovePosition(player.interactor.holder.transform.position));
      #endregion
    }

    void sbsc_AddGravity()
    {
      this.FixedUpdateAsObservable()
          .Subscribe(_ =>
          {
            rb.AddForce((G * _scaleGravity.normal)._y_(), ForceMode.Acceleration);
          });
    }

    void Jump()
    {
      rb.velocity = rb.velocity.x_z();
      rb.AddForce(_scaleBehaviour.jumpHeight._y_(), ForceMode.Impulse);
    }


    void MoveHorizontal(float hmi)
    {
      rb.velocity = new Vector3(
          hmi * _scaleBehaviour.moveSpeed,
          rb.velocity.y,
          0);
    }
  }
}