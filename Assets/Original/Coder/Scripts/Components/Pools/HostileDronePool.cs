using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Actors;

namespace Assembly.Components.Pools
{
  public class HostileDronePool : GameObjectPool<HostileDrone>
  {
    [System.Serializable]
    public class CreateInfo : ObjectCreateInfo<HostileDrone>
    {
      public DroneHatch hatch;
      public WaterBallPool waterBallPool;
      public ParticleExplosionPool psExplosionPool;
      public ParticleImpactSplashPool psImpactSplashPool;
      public override void Infuse(HostileDrone instance)
      {
        base.Infuse(instance);
        instance.DepsInject(waterBallPool, psExplosionPool, psImpactSplashPool);
        instance.launcher.baseNode = hatch;
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