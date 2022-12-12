using System;
using UnityEngine;

using UniRx;
using Senses.Sight;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(WaterEmitterModule))]
  [RequireComponent(typeof(AimModule))]
  [RequireComponent(typeof(PatrolWaypointModule))]
  [RequireComponent(typeof(FollowObjectModule))]
  [RequireComponent(typeof(LifeModule))]
  public class HostileDrone : ActorCore<HostileDrone>
  {
    public WaterEmitterModule emitter;
    public AimModule aim;
    public PatrolWaypointModule patrol;
    public FollowObjectModule follow;
    public LifeModule life;

    public AiSight sight;
    public new Rigidbody rigidbody;
    [SerializeField] float moveSpeed = 1f;

    public Transform target;

    protected override void Blueprint()
    {
      sight.InSight
          .Subscribe(visible =>
          {
            if (life.damagable.isBroken) { return; }
            patrol.enabled = !visible;
            follow.enabled = visible;
            target = visible ? visible.center : null;
            aim.Target(target);
          }).AddTo(this);
    }

    public void Move(Vector3 UnnormalizedDirection)
    {
      transform.position += UnnormalizedDirection.normalized * moveSpeed * Time.deltaTime;
    }
  }
}