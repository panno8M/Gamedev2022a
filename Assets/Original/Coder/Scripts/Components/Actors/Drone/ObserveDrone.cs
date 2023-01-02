using UniRx;
using Assembly.GameSystem.PathNetwork;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{

  public class ObserveDrone : DroneAct
  {
    AlarmMgr alarmMgr;
    [Zenject.Inject]
    public void DepsInject(
      AlarmMgr alarmMgr,
      ParticleExplosionPool psExplosionPool)
    {
      this.alarmMgr = alarmMgr;
      base.DepsInject(psExplosionPool);
    }
    protected override void Subscribe()
    {
      base.Subscribe();
      aim.Target
        .Where(_ => alarmMgr)
        .Select(target => target != null)
        .Subscribe(alarmMgr.SwitchAlarm)
        .AddTo(this);
    }
  }
}