#if UNITY_EDITOR
// #define DEBUG_ALARM
#endif

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
#if DEBUG_ALARM
    [Header("[Debug Inspector]\ndon't forget to turn symbol DEBUG_ALARM off.")]
    byte __headerTarget__;
#endif

#if DEBUG_ALARM
    [SerializeField]
#endif
    Rollback rollback;
    LightShift lightShift;
    [Zenject.Inject]
    public void DepsInject(Rollback rollback, LightShift lightShift)
    {
      this.rollback = rollback;
      this.lightShift = lightShift;
    }
    [SerializeField] AudioSource alarmSpeaker;
    [SerializeField] EzLerp alarmProgress = new EzLerp(10);
    [SerializeField] ReactiveProperty<bool> _IsOnAlert = new ReactiveProperty<bool>();
    public IObservable<bool> IsOnAlert => _IsOnAlert;
    public bool isOnAlert
    {
      get => _IsOnAlert.Value;
      private set
      {
        _IsOnAlert.Value = value;
#if DEBUG_ALARM
        Debug.Log($"isOnAlert: {isOnAlert} ({name})");
#endif
      }
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
      rollback.OnPreflight
        .Subscribe(_ =>
        {
          alarmProgress.SetFactor0();
          alarmProgress.SetAsDecrease();
        });
    }
  }
}