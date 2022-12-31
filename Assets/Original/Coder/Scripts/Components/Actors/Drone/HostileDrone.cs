using UnityEngine;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(WaterEmitterModule))]
  public class HostileDrone : DroneAct
  {
    HostileDronePool pool;

    [Zenject.Inject]
    public void DepsInject(
      HostileDronePool pool,
      WaterBallPool waterBallPool,
      ParticleExplosionPool psExplosionPool,
      ParticleImpactSplashPool psImpactSplashPool)
    {
      this.pool = pool;
      base.DepsInject(psExplosionPool);
      emitter.DepsInject(waterBallPool, psImpactSplashPool);
    }

    public WaterEmitterModule emitter;

    protected override void Prepare()
    {
      base.Prepare();
      emitter.Initialize();
    }
    public override void Despawn()
    {
      pool.Despawn(this);
    }
  }
}