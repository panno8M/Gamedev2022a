using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Damage;
using Assembly.Components.Pools;

namespace Assembly.Components.StageGimmicks
{

  public class Bomb : DiBehavior, IPoolCollectable
  {
    public IDespawnable despawnable { get; set; }
    ParticleExplosionPool psExplosionPool;

    [Zenject.Inject]
    public void DepsInject(ParticleExplosionPool psExplosionPool)
    {
      this.psExplosionPool = psExplosionPool;
    }

    [SerializeField] ParticleSystem _psBurnUp;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] Holdable _holdable;
    [SerializeField] float secExplosionDelay = 4;

    ParticlePool.CreateInfo _psExplCI = new ParticlePool.CreateInfo
    {
      transformUsage = new TransformUsage
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      },
      transformInfo = new TransformInfo { },
    };

    public void Assemble()
    {
      _psBurnUp.Stop();
      SetHoldable(locked: false);
      SetBurnUp(burning: false);
      OnHold(holding: false);
    }
    public void Disassemble()
    {
      ResetRigidbody();
      _damagable.Repair();
    }

    protected override void Blueprint()
    {
      _psExplCI.transformInfo.reference = transform;
      _damagable.OnBroken
        .Subscribe(_ => SetHoldable(locked: true))
        .AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5))
          .Subscribe(_ => SetBurnUp(burning: true))
          .AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(secExplosionDelay))
          .Subscribe(_ =>
          {
            SetBurnUp(burning: false);
            psExplosionPool.Spawn(_psExplCI, TimeSpan.FromSeconds(1));
            despawnable.Despawn();
          })
          .AddTo(this);

      _holdable.OnHold.Subscribe(_ => OnHold(holding: true));
      _holdable.OnRelease.Subscribe(_ => OnHold(holding: false));
      _holdable.OnRelease
          .Delay(TimeSpan.FromMilliseconds(100))
          .Subscribe(_ => ResetRigidbody());
    }
    void ResetRigidbody()
    {
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;

    }
    void OnHold(bool holding)
    {
      rigidbody.useGravity = !holding;
      rigidbody.isKinematic = holding;
    }

    void SetHoldable(bool locked)
    {
      _holdable.enabled = !locked;
    }
    void SetBurnUp(bool burning)
    {
      if (burning)
      { _psBurnUp.Play(); }
      else
      { _psBurnUp.Stop(); }
    }
  }
}