using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;

namespace Assembly.Components.Effects
{
  public class WaterBall : DiBehavior, IPoolCollectable
  {
    public IDespawnable despawnable { get; set; }
    ParticleImpactSplashPool psImpactSplashPool;

    [Zenject.Inject]
    public void DepsInject(
      ParticleImpactSplashPool psImpactSplashPool)
    {
      this.psImpactSplashPool = psImpactSplashPool;
    }

    ParticlePool.CreateInfo _psSplashCI = new ParticlePool.CreateInfo
    {
      transformUsage = new TransformUsage
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      },
      transformInfo = new TransformInfo { },
    };

    protected override void Blueprint()
    {
      _psSplashCI.transformInfo.reference = transform;
      this.FixedUpdateAsObservable()
      .Subscribe(_ =>
      {
        rigidbody.AddForce(new Vector3(0, -2f, 0), ForceMode.Acceleration);
      });
    }

    public void Assemble() { }
    public void Disassemble()
    {
      rigidbody.velocity = Vector3.zero;
      rigidbody.isKinematic = false;
    }

    void OnCollisionEnter(Collision other)
    {
      psImpactSplashPool.Spawn(_psSplashCI,
        timeToDespawn: TimeSpan.FromSeconds(3f));
      despawnable.Despawn();
    }
  }
}