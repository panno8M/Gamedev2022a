using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.ObjectPool;

public class WaterBallPool : GameObjectPool<WaterBall>
{
  protected override WaterBall CreateInstance()
  {
    return GameObject.Instantiate(prefab).GetComponent<WaterBall>();
  }
  protected override void InfuseInfoOnSpawn(WaterBall newObj, ObjectCreateInfo info)
  {
    Transform t = info.userData as Transform;
    if (!t) { return; }
    newObj.transform.position = t.position;
    newObj.transform.rotation = t.rotation;
  }
}
