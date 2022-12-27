using UnityEngine;
using Assembly.Components.Effects;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public class WaterBallPool : GameObjectPool<WaterBall>
  {
    protected override void Blueprint()
    {
    }

    protected override WaterBall CreateInstance()
    {
      return prefab.Instantiate<WaterBall>();
    }
    protected override void InfuseInfoOnSpawn(WaterBall newObj, ObjectCreateInfo info)
    {
      if (!info.offset) { return; }
      newObj.transform.position = info.offset.position;
      newObj.transform.rotation = info.offset.rotation;
    }
  }
}