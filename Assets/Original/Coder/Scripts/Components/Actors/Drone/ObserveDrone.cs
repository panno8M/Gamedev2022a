using UniRx;

namespace Assembly.Components.Actors
{

  public class ObserveDrone : DroneAct
  {
    protected override void Subscribe()
    {
      base.Subscribe();
      aim.Target
        .Where(_ => AlarmMgr.Instance)
        .Subscribe(_ =>
        {
          if (aim.target) AlarmMgr.Instance.ActivateAlarm();
          else AlarmMgr.Instance.DisarmAlarm();
        }).AddTo(this);
    }

    public override void Despawn()
    {
      throw new System.NotImplementedException();
    }
  }
}