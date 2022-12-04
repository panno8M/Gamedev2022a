using System;
using UnityEngine;

using UniRx;
using Senses.Pain;
using Senses.Sight;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(PatrolWaypoint))]
  public class HostileDrone : MonoBehaviour
  {
    [SerializeField] ParticleSystem psWater;
    [SerializeField] ParticleSystem psBurnUp;
    [SerializeField] AiSight sight;
    [SerializeField] DamagableComponent damagable;
    [SerializeField] PatrolWaypoint patrol;

    void Start()
    {
      sight.OnSeen
          .Subscribe(_ =>
          {
            if (damagable.isBroken) { return; }
            psWater.Play();
            patrol.Stop();
          }).AddTo(this);
      sight.OnLost
          .Subscribe(_ =>
          {
            if (damagable.isBroken) { return; }
            psWater.Stop();
            patrol.Play();
          }).AddTo(this);

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
      GetComponent<Rigidbody>().useGravity = true;
      GetComponent<Rigidbody>().isKinematic = false;
      patrol.Stop();
      psWater.Stop();
    }
  }
}