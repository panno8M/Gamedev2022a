using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Actors.Player
{
  [RequireComponent(typeof(Rigidbody))]
  public class PlayerBehaviour : MonoBehaviour
  {
    public static float G = -9.8f;

    [System.Serializable]
    public class BehaviourParams
    {
      public enum Mobility { Normal, knackered }
      public BehaviourParams(float jumpHeight, float soarHeight, float moveSpeedNormal, float moveSpeedKnackered)
      {
        this.jumpHeight = jumpHeight;
        this.soarHeight = soarHeight;
        this.moveSpeedNormal = moveSpeedNormal;
        this.moveSpeedKnackered = moveSpeedKnackered;
        this.mobility = Mobility.Normal;
      }
      public float jumpHeight;
      public float soarHeight;
      public float moveSpeedNormal;
      public float moveSpeedKnackered;
      public Mobility mobility;

      [Range(0f, 1f)] float moveSpeedBlend;
      [SerializeField] float secTransitionSpeed = 1;

      float latestCallTime;

      void CalcBlend()
      {
        var delta = Time.time - latestCallTime;
        if (delta < 0.001) { return; }
        latestCallTime = Time.time;
        moveSpeedBlend = Mathf.Clamp01(moveSpeedBlend + (mobility == Mobility.Normal ? -1 : 1) * delta / secTransitionSpeed);
      }

      public float MoveSpeed()
      {
        CalcBlend();
        return moveSpeedNormal * (1 - moveSpeedBlend) + moveSpeedKnackered * moveSpeedBlend;
      }
      public void SetAsNormal()
      {
        mobility = Mobility.Normal;
      }
      public void SetAsKnackered()
      {
        mobility = Mobility.knackered;
      }
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
    [SerializeField] BehaviourParams _bp = new BehaviourParams(5f, 3f, 3.5f, 1.8f);
    [SerializeField] GravityScale _scaleGravity;

    public BehaviourParams behaviourParams => _bp;

    void Reset()
    {
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


      Global.Control.HorizontalMoveInput
          .Where(hmi => hmi == 0)
          .Subscribe(MoveHorizontal)
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
      rb.AddForce(_bp.jumpHeight._y_(), ForceMode.Impulse);
    }


    void MoveHorizontal(float hmi)
    {
      rb.velocity = new Vector3(
          hmi * _bp.MoveSpeed(),
          rb.velocity.y,
          0);
    }
  }
}