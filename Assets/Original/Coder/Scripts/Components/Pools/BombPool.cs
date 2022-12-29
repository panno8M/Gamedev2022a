using UnityEngine;
using Assembly.Components.StageGimmicks;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public class BombPool : GameObjectPool<Bomb>
  {
    public class CreateInfo : ObjectCreateInfo<Bomb> { }

    protected override Bomb CreateInstance()
    {
      return prefab.Instantiate<Bomb>();
    }
    protected override void Blueprint()
    {
    }
  }
}