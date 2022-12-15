using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;

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

    protected override void Blueprint()
    {
      _actor = Global.Player;

      sbsc_AddGravity();

      _actor.OnJump.Subscribe(_ => Jump()).AddTo(this);
      _actor.OnFlapWhileFalling.Subscribe(_ => Jump()).AddTo(this);

      _actor.Behavior
          .Subscribe(MoveHorizontal);
    }

    void sbsc_AddGravity()
    {
      this.FixedUpdateAsObservable()
          .Subscribe(_ =>
          {
            rigidbody.AddForce((G * _scaleGravity.normal)._y_(), ForceMode.Acceleration);
          });
    }

    void Jump()
    {
      rigidbody.velocity = rigidbody.velocity.x_z();
      rigidbody.AddForce(_actor.param.jumpHeight._y_(), ForceMode.Impulse);
    }


    void MoveHorizontal(PlayerAct pa)
    {
      if (pa.horiMove == 0)
      {
        rigidbody.velocity = rigidbody.velocity._yz();
        return;
      }

      if (pa.obstacleColliding)
      {
        if (pa.obstacleClimbable)
        {
          rigidbody.velocity = pa.obstacleTangent * pa.param.MoveSpeed;
        }
        else
        {
          rigidbody.velocity = rigidbody.velocity._yz();
        }
      }
      else
      {
        rigidbody.velocity = new Vector3(
          (float)pa.lookDirection.current * pa.param.MoveSpeed,
          rigidbody.velocity.y,
          rigidbody.velocity.z);
      }
    }
  }
}