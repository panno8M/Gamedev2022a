using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Actors;

namespace Assembly.Components.Pools
{
  public class ObserveDronePool : GameObjectPool<ObserveDrone>
  {
    protected override void Blueprint()
    {
    }
    protected override ObserveDrone CreateInstance()
    {
      return prefab.Instantiate<ObserveDrone>();
    }
  }
}