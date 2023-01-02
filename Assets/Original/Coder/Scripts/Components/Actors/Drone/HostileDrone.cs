using UnityEngine;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(WaterEmitterModule))]
  public class HostileDrone : DroneAct
  {
    [Zenject.Inject]
    public void DepsInject(
      WaterBallPool waterBallPool,
      ParticleExplosionPool psExplosionPool,
      ParticleImpactSplashPool psImpactSplashPool)
    {
      base.DepsInject(psExplosionPool);
      emitter.DepsInject(waterBallPool, psImpactSplashPool);
    }

    public WaterEmitterModule emitter;

    protected override void Prepare()
    {
      base.Prepare();
      emitter.Initialize();
    }
  }
}