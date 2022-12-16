using System;
using UnityEngine;

using UniRx;
using Senses.Sight;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.Actors
{
  public class ObserveLifeModule : ActorBehavior<ObserveDrone>
  {
    public DamagableComponent damagable;
    [SerializeField] ParticleSystem psBurnUp;
    protected override void Blueprint()
    {
      damagable.TotalDamage
          .Where(total => total == 1)
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => psBurnUp.Play())
            .AddTo(this);

      damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => Dead())
            .AddTo(this);
    }
    void Dead()
    {
      rigidbody.useGravity = true;
      rigidbody.isKinematic = false;
      _actor.patrol.enabled = false;
      _actor.follow.enabled = false;
      _actor.alarmUser.enabled = false;
      _actor.aim.enabled = false;
    }
  }
}
