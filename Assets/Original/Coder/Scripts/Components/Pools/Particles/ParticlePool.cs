using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public abstract class ParticlePool : GameObjectPool<PoolManagedParticle>
  {
    protected override PoolManagedParticle CreateInstance()
    {
      return prefab.Instantiate<PoolManagedParticle>();
    }
    protected override void InfuseInfoOnSpawn(PoolManagedParticle newObj, ObjectCreateInfo info)
    {
      newObj.transform.position = info.position;
    }
    protected override void Blueprint()
    {
    }
  }
}