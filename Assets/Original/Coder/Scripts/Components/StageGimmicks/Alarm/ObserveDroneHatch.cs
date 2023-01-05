using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public class ObserveDroneHatch : DroneHatch<ObserveDrone>
  {
    [Zenject.Inject]
    public void DepsInject(
      ObserveDronePool observeDronePool,
      AlarmMgr alarmMgr,
      Rollback rollback,
      ParticleExplosionPool psExplosionPool)
    {
      base.DepsInject(
        pool: observeDronePool,
        alarmMgr: alarmMgr,
        rollback: rollback);
      _droneCI.psExplosionPool = psExplosionPool;
      _droneCI.alarmMgr = alarmMgr;
    }

    ObserveDrone.CreateInfo _droneCI = new ObserveDrone.CreateInfo
    {
      transformUsage = new TransformUsage { },
      transformInfo = new TransformInfo { },
    };

    void Start()
    {
      _droneCI.baseNode = this;

      Subscribe();
    }
    protected override ObserveDrone Launch()
    {
      _droneCI.transformInfo.position = transform.position;
      ObserveDrone result = pool.Spawn(_droneCI);
      result.launcher.Launch();
      return result;
    }
  }
}