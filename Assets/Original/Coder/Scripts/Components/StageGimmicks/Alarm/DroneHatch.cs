#if UNITY_EDITOR
// #define DEBUG_DRONE_HATCH
#endif

using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem.PathNetwork;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public abstract class DroneHatch<T> : PathNode
    where T : DroneAct
  {
#if DEBUG_DRONE_HATCH
    [Header("[Debug Inspector]\ndon't forget to turn symbol DEBUG_DRONE_HATCH off.")]
    byte __headerTarget__;
    void Log(string text)
      => Debug.Log($"{text} ({name})");
    string SignColor(LaunchSchedule.LifetimeKind kind)
      => (kind == LaunchSchedule.LifetimeKind.Scene
          ? "#0FF" :
         kind == LaunchSchedule.LifetimeKind.Alarm
          ? "#F0F" :
         "#FFF");
#endif

    protected IObjectPool<T> pool;
    protected AlarmMgr alarmMgr;
    protected Rollback rollback;
    protected void DepsInject(
      IObjectPool<T> pool,
      AlarmMgr alarmMgr,
      Rollback rollback)
    {
      this.pool = pool;
      this.alarmMgr = alarmMgr;
      this.rollback = rollback;
    }


    [SerializeField]
    protected LaunchSchedule[] schedule;
#if DEBUG_DRONE_HATCH
    [SerializeField]
#endif
    T[] drone;

    protected void Subscribe()
    {
      drone = new T[schedule.Length];
      LaunchAll(LaunchSchedule.LifetimeKind.Scene);

#if DEBUG_DRONE_HATCH
      Log($"subscribe rollback sequence to {rollback}");
#endif
      rollback.OnPreflight.Subscribe(_ =>
        { CollectAll(LaunchSchedule.LifetimeKind.Scene); });
      rollback.OnExecute.Subscribe(_ =>
        { LaunchAll(LaunchSchedule.LifetimeKind.Scene); });

      alarmMgr.IsOnAlert
        .Subscribe(b =>
        {
          if (b)
          { LaunchAll(LaunchSchedule.LifetimeKind.Alarm); }
          else
          { CollectAll(LaunchSchedule.LifetimeKind.Alarm); }
        });
    }
    protected abstract T Launch();
    public void Launch(int index, float msecDelay)
    {
      if (msecDelay == 0)
      { drone[index] = Launch(); }
      else
      {
        Observable.Timer(TimeSpan.FromMilliseconds(msecDelay))
          .Subscribe(_ => drone[index] = Launch())
          .AddTo(this);
      }
    }
    public void Collect(int index)
    {
      if (!drone[index]) { return; }
      drone[index].launcher.Collect();
      drone[index].despawnable.Despawn();
      drone[index] = null;
    }

    void LaunchAll(LaunchSchedule.LifetimeKind lifetimeFilter)
    {
#if DEBUG_DRONE_HATCH
      int count = 0;
#endif
      float msecDelay = 0;
      for (int i = 0; i < schedule.Length; i++)
      {
        if (schedule[i].lifetime != lifetimeFilter) { continue; }
        switch (schedule[i].delayMode)
        {
          case LaunchSchedule.DelayMode.Absolute:
            msecDelay = schedule[i].msecDelay;
            break;
          case LaunchSchedule.DelayMode.Relative:
            msecDelay += schedule[i].msecDelay;
            break;
        }
        Launch(i, msecDelay);
#if DEBUG_DRONE_HATCH
        count++;
#endif
      }
#if DEBUG_DRONE_HATCH
      if (count != 0)
        Log($"<color={SignColor(lifetimeFilter)}>>></color> launch {count} drones from {lifetimeFilter}");
#endif
    }
    void CollectAll(LaunchSchedule.LifetimeKind lifetimeFilter)
    {
#if DEBUG_DRONE_HATCH
      int count = 0;
#endif
      for (int i = 0; i < schedule.Length; i++)
      {
        if (schedule[i].lifetime != lifetimeFilter) { continue; }
        Collect(i);
#if DEBUG_DRONE_HATCH
        count++;
#endif
      }
#if DEBUG_DRONE_HATCH
      if (count != 0)
        Log($"<color={SignColor(lifetimeFilter)}><<</color> collect {count} drones from {lifetimeFilter}");
#endif
    }
  }
}