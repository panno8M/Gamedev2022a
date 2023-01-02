using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem.PathNetwork;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Actors
{
  public abstract class DroneHatch<T> : PathNode
    where T : DroneAct
  {
    protected IObjectPool<T> pool;

    public T drone;

    public enum LifetimeKind { Scene, Alarm }
    [SerializeField] ReactiveProperty<LifetimeKind> _Lifetime = new ReactiveProperty<LifetimeKind>();
    public IObservable<LifetimeKind> Lifetime => _Lifetime;
    public LifetimeKind lifetime
    {
      get => _Lifetime.Value;
      set => _Lifetime.Value = value;
    }
    protected AlarmMgr alarmMgr;

    protected void Subscribe()
    {
      Lifetime.Where(lt => lt == LifetimeKind.Scene)
        .Subscribe(_ => Launch());

      alarmMgr.IsOnAlert
        .Where(_ => lifetime == LifetimeKind.Alarm)
        .Subscribe(b =>
        {
          if (b) { Launch(); }
          else { Collect(); }
        });
    }

    protected abstract void Launch();
    protected void Collect()
    {
      if (!drone) { return; }
      drone.launcher.Collect();
      drone.despawnable.Despawn();
      drone = null;
    }
  }
}