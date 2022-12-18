using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

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
    Transform t = info.userData as Transform;
    if (!t) { return; }
    newObj.transform.position = t.position;
    newObj.transform.rotation = t.rotation;
  }
}
