using UniRx;
using Assembly.GameSystem.PathNetwork;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{
  public class DroneHatch : PathNode
  {
    AlarmMgr alarmMgr;
    HostileDronePool hostileDronePool;
    [Zenject.Inject]
    public void DepsInject(
      AlarmMgr alarmMgr,
      HostileDronePool hostileDronePool,
      WaterBallPool waterBallPool,
      ParticleExplosionPool psExplosionPool,
      ParticleImpactSplashPool psImpactSplashPool)
    {
      this.alarmMgr = alarmMgr;
      this.hostileDronePool = hostileDronePool;
      _droneCI.waterBallPool = waterBallPool;
      _droneCI.psExplosionPool = psExplosionPool;
      _droneCI.psImpactSplashPool = psImpactSplashPool;
    }

    HostileDronePool.CreateInfo _droneCI = new HostileDronePool.CreateInfo
    {
      transformUsage = new TransformUsage { },
      transformInfo = new TransformInfo { },
    };

    public HostileDrone drone;

    void Start()
    {
      _droneCI.baseNode = this;
      alarmMgr.IsOnAlert
        .Subscribe(b =>
        {
          if (b)
          {
            if (drone) { return; }
            _droneCI.transformInfo.position = transform.position;
            drone = hostileDronePool.Spawn(_droneCI);
            drone.launcher.Launch();
          }
          else
          {
            if (!drone) { return; }
            drone.launcher.Collect();
            drone.despawnable.Despawn();
            drone = null;
          }
        });
    }
  }
}