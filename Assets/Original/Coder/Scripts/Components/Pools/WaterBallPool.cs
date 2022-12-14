using Assembly.Components.Effects;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public class WaterBallPool : GameObjectPool<WaterBall>
  {
    protected override void Blueprint() { }

    protected override WaterBall CreateInstance()
    {
      return prefab.Instantiate<WaterBall>();
    }
  }
}