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
    public static WaterBallPool pool => PoolCore.Instance.waterBall;
    ParticlePool.CreateInfo _psSplashCI = new ParticlePool.CreateInfo
    {
      spawnSpace = eopSpawnSpace.Global,
      referenceUsage = eopReferenceUsage.Global,
    };

    protected override void Blueprint()
    {
      _psSplashCI.reference = transform;
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
      Pool.psImpactSplash.Spawn(_psSplashCI,
        timeToDespawn: TimeSpan.FromSeconds(3f));
      pool.Despawn(this);
    }
  }
}