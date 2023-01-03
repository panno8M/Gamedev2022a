using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  public class AlarmMgr : DiBehavior
  {
    Rollback rollback;
    [Zenject.Inject]
    public void DepsInject(Rollback rollback)
    {
      this.rollback = rollback;
    }
    [SerializeField] EzLerp alarmProgress = new EzLerp(10);
    [SerializeField] ReactiveProperty<bool> _IsOnAlert = new ReactiveProperty<bool>();
    public IObservable<bool> IsOnAlert => _IsOnAlert;
    public bool isOnAlert
    {
      get { return _IsOnAlert.Value; }
      private set { _IsOnAlert.Value = value; }
    }
    public void ActivateAlarm() => SwitchAlarm(true);
    public void DisarmAlarm() => SwitchAlarm(false);
    public void SwitchAlarm(bool b)
    {
      if (b) alarmProgress.SetFactor1();
      alarmProgress.SetMode(b);
    }

    void Start() { Initialize(); }

    protected override void Blueprint()
    {
      this.FixedUpdateAsObservable()
        .Where(alarmProgress.isNeedsCalc)
        .Select(alarmProgress.UpdFactor)
        .Subscribe(fac =>
        {
          isOnAlert = fac != 0;
        });
      rollback.OnExecute
        .Subscribe(_ =>
        {
          alarmProgress.SetFactor0();
          alarmProgress.SetMode(increase: false);
        });
    }
  }
}