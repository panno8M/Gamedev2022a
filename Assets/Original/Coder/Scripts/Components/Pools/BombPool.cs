using Assembly.Components.Items;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public class BombPool : GameObjectPool<Bomb>
  {
    protected override Bomb CreateInstance()
    {
      return prefab.Instantiate<Bomb>();
    }
    protected override void Blueprint()
    {
    }
  }
}