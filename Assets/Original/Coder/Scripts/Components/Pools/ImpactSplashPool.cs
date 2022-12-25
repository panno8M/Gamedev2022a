using System;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Pools
{
  public class ImpactSplashPool : GameObjectPool<PoolManagedParticle>
  {
    protected override void Blueprint()
    {
    }
    protected override PoolManagedParticle CreateInstance()
    {
      return prefab.Instantiate<PoolManagedParticle>();
    }
    protected override void InfuseInfoOnSpawn(PoolManagedParticle newObj, ObjectCreateInfo info)
    {
      newObj.transform.position = info.position;
    }
  }
}