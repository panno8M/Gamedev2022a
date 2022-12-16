using System;
using UnityEngine;

using UniRx;
using Senses.Sight;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(AlarmUser))]
  [RequireComponent(typeof(ObserveAimModule))]
  [RequireComponent(typeof(ObservePatrolWaypointModule))]
  [RequireComponent(typeof(ObserveFollowObjectModule))]
  [RequireComponent(typeof(ObserveLifeModule))]
  public class ObserveDrone : ActorCore<ObserveDrone>
  {
    public AlarmUser alarmUser;
    public ObserveAimModule aim;
    public ObservePatrolWaypointModule patrol;
    public ObserveFollowObjectModule follow;
    public ObserveLifeModule life;
    public AlarmMgr alarmMgr;

    public AiSight sight;
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