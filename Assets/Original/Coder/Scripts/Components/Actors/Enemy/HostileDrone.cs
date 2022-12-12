using System;
using UnityEngine;

using UniRx;
using Senses.Sight;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(PatrolWaypoint))]
  public class HostileDrone : MonoBehaviour
  {
    [SerializeField] WaterEmitter _emitter;
    [SerializeField] ParticleSystem psBurnUp;
    [SerializeField] AiSight sight;
    [SerializeField] DamagableComponent damagable;
    [SerializeField] PatrolWaypoint patrol;
    [SerializeField] float moveSpeed = 1f;

    void Start()
    {
      sight.OnSeen
          .Subscribe(_ =>
          {
            if (damagable.isBroken) { return; }
            _emitter.Launch();
            patrol.enabled = false;
          }).AddTo(this);
      sight.OnLost
          .Subscribe(_ =>
          {
            if (damagable.isBroken) { return; }
            patrol.enabled = true;
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
    public void Move(Vector3 UnnormalizedDirection)
    {
      transform.position += UnnormalizedDirection.normalized * moveSpeed * Time.deltaTime;
    }

    void Dead()
    {
      GetComponent<Rigidbody>().useGravity = true;
      GetComponent<Rigidbody>().isKinematic = false;
      patrol.enabled = false;
    }
  }
}