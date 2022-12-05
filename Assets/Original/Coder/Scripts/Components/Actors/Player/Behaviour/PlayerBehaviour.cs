using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Actors
{
  [RequireComponent(typeof(Rigidbody))]
  public class PlayerBehaviour : ActorBehavior<PlayerAct>
  {
    public static float G = -9.8f;

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
    [SerializeField] GravityScale _scaleGravity;

    protected override void OnResetInEditor()
    {
      _scaleGravity = new GravityScale(1.3f, .1f);
    }
    #endregion

    #region assets
    Rigidbody rb;
    #endregion

    protected override void Blueprint()
    {
      rb = GetComponent<Rigidbody>();
      _actor = Global.Player;

      sbsc_AddGravity();

      _actor.OnJump.Subscribe(_ => Jump()).AddTo(this);
      _actor.OnFlapWhileFalling.Subscribe(_ => Jump()).AddTo(this);

      _actor.Behavior
          .Subscribe(MoveHorizontal);

      _actor.interactor.holder.RequestHold
          .Subscribe(_actor.interactor.holder.Grab);
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
      rb.AddForce(_actor.param.jumpHeight._y_(), ForceMode.Impulse);
    }


    void MoveHorizontal(PlayerAct pa)
    {
      if (pa.horiMove == 0)
      {
        rb.velocity = rb.velocity._yz();
        return;
      }

      if (pa.obstacleColliding)
      {
        if (pa.obstacleClimbable)
        {
          rb.velocity = pa.obstacleTangent * pa.param.MoveSpeed;
        }
        else
        {
          rb.velocity = rb.velocity._yz();
        }
      }
      else
      {
        rb.velocity = new Vector3(
          (float)pa.lookDirection.current * pa.param.MoveSpeed,
          rb.velocity.y,
          rb.velocity.z);
      }
    }
  }
}