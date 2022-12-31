using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem.PathNetwork;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{
  public class DroneHatch : PathNode
  {
    HostileDronePool.CreateInfo _droneCI = new HostileDronePool.CreateInfo { };

    public HostileDrone drone;

    void Start()
    {
      _droneCI.hatch = this;
      AlarmMgr.Instance.IsOnAlert
        .Subscribe(b =>
        {
          if (b)
          {
            if (drone) { return; }
            _droneCI.position = transform.position;
            drone = HostileDrone.pool.Spawn(_droneCI);
            drone.launcher.Launch();
          }
          else
          {
            if (!drone) { return; }
            drone.launcher.Collect();
            drone.Despawn();
            drone = null;
          }
        });
    }
  }
}