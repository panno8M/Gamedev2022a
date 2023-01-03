using Assembly.Components.Items;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public class BombPool : GameObjectPool<Bomb>
  {
    public class CreateInfo : ObjectCreateInfo<Bomb>
    {
      public ParticleExplosionPool psExplosionPool;
      public override void Infuse(Bomb instance)
      {
        base.Infuse(instance);
        instance.DepsInject(psExplosionPool);
      }
    }

    protected override Bomb CreateInstance()
    {
      return prefab.Instantiate<Bomb>();
    }
    protected override void Blueprint()
    {
    }
  }
}