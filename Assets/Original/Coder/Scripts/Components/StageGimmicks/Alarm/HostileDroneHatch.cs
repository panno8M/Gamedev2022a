using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public class HostileDroneHatch : DroneHatch<HostileDrone>
  {
    [Zenject.Inject]
    public void DepsInject(
      HostileDronePool hostileDronePool,
      AlarmMgr alarmMgr,
      Rollback rollback,
      WaterBallPool waterBallPool,
      ParticleExplosionPool psExplosionPool,
      ParticleImpactSplashPool psImpactSplashPool)
    {
      base.DepsInject(
        pool: hostileDronePool,
        alarmMgr: alarmMgr,
        rollback: rollback);
      _droneCI.waterBallPool = waterBallPool;
      _droneCI.psExplosionPool = psExplosionPool;
      _droneCI.psImpactSplashPool = psImpactSplashPool;
    }

    HostileDrone.CreateInfo _droneCI = new HostileDrone.CreateInfo
    {
      transformUsage = new TransformUsage { },
      transformInfo = new TransformInfo { },
    };

    void Start()
    {
      _droneCI.baseNode = this;

      Subscribe();
    }
    protected override HostileDrone Launch()
    {
      _droneCI.transformInfo.position = transform.position;
      HostileDrone result = pool.Spawn(_droneCI);
      result.launcher.Launch();
      return result;
    }
  }
}