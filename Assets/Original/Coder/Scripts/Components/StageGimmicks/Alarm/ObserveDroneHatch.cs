using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
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
    protected override void Launch()
    {
      if (drone) { return; }
      _droneCI.transformInfo.position = transform.position;
      drone = pool.Spawn(_droneCI);
      drone.launcher.Launch();
    }
  }
}