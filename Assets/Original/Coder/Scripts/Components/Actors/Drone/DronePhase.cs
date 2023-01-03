using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{
  [Flags]
  public enum DronePhase
  {
    Sleep = 0,
    Standby = 1 << 0,

    Patrol = 1 << 1,
    Hostile = 1 << 2,
    Attention = 1 << 3,

    Dead = 1 << 4,
  }
  [Serializable]
  public class DronePhaseUnit
  {
    [SerializeField] DronePhase _previous;
    [SerializeField] DronePhase _current;
    Subject<DronePhaseUnit> _OnChanged = new Subject<DronePhaseUnit>();
    public DronePhase previous => _previous;
    public DronePhase current
    {
      get => _current;
      set
      {
        if (_current == value) { return; }
        _previous = _current;
        _current = value;
        _OnChanged.OnNext(this);
      }
    }
    public IObservable<DronePhaseUnit> OnChanged => _OnChanged;
    public IObservable<DronePhaseUnit> OnEnter(DronePhase phase)
    {
      return OnChanged.Where(x => phase.HasFlag(x.current));
    }
    public IObservable<DronePhaseUnit> OnExit(DronePhase phase)
    {
      return OnChanged.Where(x => phase.HasFlag(x.previous));
    }
    public void ActivateSwitch(DronePhase cond, params Behaviour[] targets)
    {
      OnChanged
        .Subscribe(phase =>
        {
          bool b = (phase.current & cond) != 0;
          for (int i = 0; i < targets.Length; i++)
            if (targets[i])
            { targets[i].enabled = b; }
        });
    }

    public void ShiftSleep() => current = DronePhase.Sleep;
    public void ShiftStandby() => current = DronePhase.Standby;
    public void ShiftPatrol() => current = DronePhase.Patrol;
    public void ShiftHostile() => current = DronePhase.Hostile;
    public void ShiftAttention() => current = DronePhase.Attention;
    public void ShiftDead() => current = DronePhase.Dead;

    public bool isSleep => current == DronePhase.Sleep;
    public bool isStandby => current == DronePhase.Standby;
    public bool isPatrol => current == DronePhase.Patrol;
    public bool isHostile => current == DronePhase.Hostile;
    public bool isAttension => current == DronePhase.Attention;
    public bool isDead => current == DronePhase.Dead;
  }
  public static class DronePhaseUnitExtensions
  {
    public static bool IsSleep<T>(this DronePhaseUnit phase, T t) => phase.isSleep;
    public static bool IsStandby<T>(this DronePhaseUnit phase, T t) => phase.isStandby;
    public static bool IsPatrol<T>(this DronePhaseUnit phase, T t) => phase.isPatrol;
    public static bool IsHostile<T>(this DronePhaseUnit phase, T t) => phase.isHostile;
    public static bool IsAttension<T>(this DronePhaseUnit phase, T t) => phase.isAttension;
    public static bool IsDead<T>(this DronePhaseUnit phase, T t) => phase.isDead;

    public static void ShiftStandby<T>(this DronePhaseUnit phase, T t) => phase.ShiftStandby();
  }
}