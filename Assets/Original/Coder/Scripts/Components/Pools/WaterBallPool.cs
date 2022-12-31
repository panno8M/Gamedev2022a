using Assembly.Components.Effects;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public class WaterBallPool : GameObjectPool<WaterBall>
  {
    public class CreateInfo : ObjectCreateInfo<WaterBall>
    {
      public WaterBallPool pool;
      public ParticleImpactSplashPool psImpactSplashPool;

      public override void Infuse(WaterBall instance)
      {
        base.Infuse(instance);
        instance.DepsInject(pool, psImpactSplashPool);
      }
    }

    protected override void Blueprint() { }

    protected override WaterBall CreateInstance()
    {
      return prefab.Instantiate<WaterBall>();
    }
  }
}