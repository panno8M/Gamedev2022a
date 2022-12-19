using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Actors;

namespace Assembly.Components.Pools
{
  public class HostileDronePool : GameObjectPool<HostileDrone>
  {
    protected override void Blueprint()
    {
    }
    protected override HostileDrone CreateInstance()
    {
      throw new System.NotImplementedException();
    }
    protected override void InfuseInfoOnSpawn(HostileDrone newObj, ObjectCreateInfo info)
    {
      throw new System.NotImplementedException();
    }
  }
}