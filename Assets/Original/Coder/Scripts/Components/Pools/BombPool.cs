using UnityEngine;
using Assembly.Components.StageGimmicks;
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
    protected override void InfuseInfoOnSpawn(Bomb newObj, ObjectCreateInfo info)
    {
      if (info.offset != null)
      {
        newObj.transform.SetParent(info.parent, false);
        newObj.transform.position = info.offset.position;
        newObj.transform.rotation = info.offset.rotation;
      }
      else
      {
        newObj.transform.SetParent(info.parent, true);
      }
    }
    protected override void Blueprint()
    {
    }
  }
}