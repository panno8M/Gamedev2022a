using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Actors;

namespace Assembly.Components.Pools
{
  public class HostileDronePool : GameObjectPool<HostileDrone>
  {
    public class CreateInfo : ObjectCreateInfo<HostileDrone>
    {
      public DroneHatch hatch;
      public override void Infuse(HostileDrone instance)
      {
        base.Infuse(instance);
        instance.launcher.baseNode = hatch;
      }
    }

    protected override void Blueprint()
    {
    }
    protected override HostileDrone CreateInstance()
    {
      return prefab.Instantiate<HostileDrone>();
    }
  }
}