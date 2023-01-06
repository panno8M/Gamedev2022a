using UnityEngine;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.PathNetwork;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(WaterEmitterModule))]
  public class HostileDrone : DroneAct
  {
    public class CreateInfo : ObjectCreateInfo<HostileDrone>
    {
      public PathNode baseNode;
      public WaterBallPool waterBallPool;
      public ParticleExplosionPool psExplosionPool;
      public ParticleImpactSplashPool psImpactSplashPool;
      public override void Infuse(HostileDrone instance)
      {
        base.Infuse(instance);
        instance.DepsInject(
          waterBallPool: waterBallPool,
          psExplosionPool: psExplosionPool,
          psImpactSplashPool: psImpactSplashPool);
        instance.launcher.baseNode = baseNode;
      }
    }

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