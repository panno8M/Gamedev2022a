using System;
using UnityEngine;
using UniRx;
using Utilities;

namespace Assembly.Components.Actors.Player
{
  [RequireComponent(typeof(Rigidbody))]
  public class PlayerBehavior : ActorBehavior<PlayerAct>
  {
    public enum Mobility { Normal, knackered }
    float jumpHeight = 5;
    float moveSpeedNormal = 3.5f;
    float moveSpeedKnackered = 1.8f;
    Mobility _mobility = Mobility.Normal;
    EzLerp moveSpeedLerp = new EzLerp(1);
    public float MoveSpeed => moveSpeedLerp.UpdMix(moveSpeedNormal, moveSpeedKnackered);
    public IObservable<Unit> OnJump => _actor.ctl.Up.Where(_ => _actor.physics.isOnGround);
    public IObservable<int> OnFlap1 => _actor.wings.FlapCount.Where(x => x == 1);
    public Mobility mobility => _mobility;

    public void SetAsNormal()
    {
      _mobility = Mobility.Normal;
      moveSpeedLerp.SetAsDecrease();
    }
    public void SetAsKnackered()
    {
      _mobility = Mobility.knackered;
      moveSpeedLerp.SetAsIncrease();
    }


    protected override void Blueprint()
    {
      OnJump.Subscribe(_ => Jump()).AddTo(this);
      OnFlap1.Subscribe(_ => Jump()).AddTo(this);

      _actor.physics.BehaviorUpdate
          .Subscribe(MoveHorizontal);
    }

    void Jump()
    {
      rigidbody.velocity = rigidbody.velocity.x_z();
      rigidbody.AddForce(jumpHeight._v_(), ForceMode.Impulse);
    }


    void MoveHorizontal(Unit _)
    {
      if (_actor.ctl.horizontal == 0)
      {
        rigidbody.velocity = rigidbody.velocity._yz();
        return;
      }

      if (_actor.physics.obstacleColliding)
      {
        if (_actor.physics.obstacleClimbable)
        {
          rigidbody.velocity = _actor.physics.obstacleTangent * MoveSpeed;
        }
        else
        {
          rigidbody.velocity = rigidbody.velocity._yz();
        }
      }
      else
      {
        rigidbody.velocity = new Vector3(
          _actor.ctl.lookDirection * MoveSpeed,
          rigidbody.velocity.y,
          rigidbody.velocity.z);
      }
    }
  }
}