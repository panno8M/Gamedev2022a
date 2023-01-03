using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.PathNetwork;
using Assembly.Components.Actors;

namespace Assembly.Components.Pools
{
  public class HostileDronePool : GameObjectPool<HostileDrone>
  {
    [System.Serializable]
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

    protected override void Blueprint()
    {
    }
    protected override HostileDrone CreateInstance()
    {
      return prefab.Instantiate<HostileDrone>();
    }
  }
}