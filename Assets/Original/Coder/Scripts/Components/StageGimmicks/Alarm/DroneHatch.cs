using System;
using UniRx;
using Assembly.GameSystem.PathNetwork;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public abstract class DroneHatch<T> : PathNode
    where T : DroneAct
  {
    protected IObjectPool<T> pool;

    protected AlarmMgr alarmMgr;

    [UnityEngine.SerializeField]
    protected LaunchSchedule[] schedule;
    T[] drone;

    protected void Subscribe()
    {
      drone = new T[schedule.Length];
      LaunchAll(LaunchSchedule.LifetimeKind.Scene);

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
          .Subscribe(_ => drone[index] = Launch());
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
      }
    }
    void CollectAll(LaunchSchedule.LifetimeKind lifetimeFilter)
    {
      for (int i = 0; i < schedule.Length; i++)
      {
        if (schedule[i].lifetime != lifetimeFilter) { continue; }
        Collect(i);
      }
    }
  }
}