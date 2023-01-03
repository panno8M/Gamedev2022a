using System;
using UnityEngine;

namespace Assembly.Components.StageGimmicks
{
  [Serializable]
  public class LaunchSchedule
  {
    public enum LifetimeKind { Scene, Alarm }
    public enum DelayMode { Absolute, Relative }
    [SerializeField] LifetimeKind _lifetime;
    public float msecDelay;
    public DelayMode delayMode;

    public LifetimeKind lifetime
    {
      get => _lifetime;
      set => _lifetime = value;
    }
  }
}