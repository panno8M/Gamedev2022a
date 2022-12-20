using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Utilities;

public class AlarmMgr : UniqueBehaviour<AlarmMgr>
{
  [SerializeField] EzLerp alarmProgress = new EzLerp(10);
  [SerializeField] ReactiveProperty<bool> _IsOnAlert = new ReactiveProperty<bool>();
  public IObservable<bool> IsOnAlert => _IsOnAlert;
  public bool isOnAlert
  {
    get { return _IsOnAlert.Value; }
    private set { _IsOnAlert.Value = value; }
  }
  public void ActivateAlarm()
  {
    alarmProgress.SetFactor1();
    alarmProgress.SetAsIncrease();
  }
  public void DisarmAlarm()
  {
    alarmProgress.SetAsDecrease();
  }

  void Start() { Initialize(); }

  protected override void Blueprint()
  {
    this.FixedUpdateAsObservable()
      .Subscribe(_ => isOnAlert = alarmProgress.UpdFactor() != 0);
  }
}
