using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public abstract class ParticlePool : GameObjectPool<PoolManagedParticle>
  {
    public class CreateInfo : ObjectCreateInfo<PoolManagedParticle> { }
    protected override PoolManagedParticle CreateInstance()
    {
      return prefab.Instantiate<PoolManagedParticle>();
    }
    protected override void Blueprint()
    {
    }
  }
}