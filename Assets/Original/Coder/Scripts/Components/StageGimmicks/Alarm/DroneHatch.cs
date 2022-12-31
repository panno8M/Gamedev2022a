using UniRx;
using Assembly.GameSystem.PathNetwork;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{
  public class DroneHatch : PathNode
  {
    AlarmMgr alarmMgr;
    [Zenject.Inject]
    public void DepsInject(
      AlarmMgr alarmMgr,
      HostileDronePool pool,
      WaterBallPool waterBallPool,
      ParticleExplosionPool psExplosionPool,
      ParticleImpactSplashPool psImpactSplashPool)
    {
      this.alarmMgr = alarmMgr;
      _droneCI.pool = pool;
      _droneCI.waterBallPool = waterBallPool;
      _droneCI.psExplosionPool = psExplosionPool;
      _droneCI.psImpactSplashPool = psImpactSplashPool;
    }

    HostileDronePool.CreateInfo _droneCI = new HostileDronePool.CreateInfo
    {
      transformUsageInfo = new TransformUsageInfo { },
      transformInfo = new TransformInfo { },
    };

    public HostileDrone drone;

    void Start()
    {
      _droneCI.hatch = this;
      alarmMgr.IsOnAlert
        .Subscribe(b =>
        {
          if (b)
          {
            if (drone) { return; }
            _droneCI.transformInfo.position = transform.position;
            drone = _droneCI.pool.Spawn(_droneCI);
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