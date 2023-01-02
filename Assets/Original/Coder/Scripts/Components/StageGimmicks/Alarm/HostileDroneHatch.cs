using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public class HostileDroneHatch : DroneHatch<HostileDrone>
  {
    [Zenject.Inject]
    public void DepsInject(
      AlarmMgr alarmMgr,
      HostileDronePool hostileDronePool,
      WaterBallPool waterBallPool,
      ParticleExplosionPool psExplosionPool,
      ParticleImpactSplashPool psImpactSplashPool)
    {
      this.alarmMgr = alarmMgr;
      this.pool = hostileDronePool;
      _droneCI.waterBallPool = waterBallPool;
      _droneCI.psExplosionPool = psExplosionPool;
      _droneCI.psImpactSplashPool = psImpactSplashPool;
    }

    HostileDronePool.CreateInfo _droneCI = new HostileDronePool.CreateInfo
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