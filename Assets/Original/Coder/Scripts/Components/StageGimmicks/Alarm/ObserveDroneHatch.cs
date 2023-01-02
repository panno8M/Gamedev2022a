using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public class ObserveDroneHatch : DroneHatch<ObserveDrone>
  {
    [Zenject.Inject]
    public void DepsInject(
      AlarmMgr alarmMgr,
      ObserveDronePool observeDronePool,
      ParticleExplosionPool psExplosionPool)
    {
      this.alarmMgr = alarmMgr;
      this.pool = observeDronePool;
      _droneCI.psExplosionPool = psExplosionPool;
      _droneCI.alarmMgr = alarmMgr;
    }

    ObserveDronePool.CreateInfo _droneCI = new ObserveDronePool.CreateInfo
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