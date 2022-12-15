using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.StageGimmicks
{
  public class BombPool : GameObjectPool<Bomb>
  {
    protected override Bomb CreateInstance()
    {
      return prefab.Instantiate<Bomb>();
    }
    protected override void InfuseInfoOnSpawn(Bomb newObj, ObjectCreateInfo info)
    {
      Transform t = info.userData as Transform;
      if (newObj.transform.parent != t)
      {
        newObj.transform.SetParent(t);
      }
      newObj.transform.localPosition = Vector3.zero;
      newObj.transform.localRotation = Quaternion.identity;
      newObj.transform.localScale = Vector3.one;
    }
    protected override void Blueprint()
    {
    }
  }
}