using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Assembly.GameSystem
{
  public static class GameTime
  {
    static bool _paused;
    public static bool paused => _paused;
    public static void Pause(bool b = true)
    {
      timeScale = b ? 0 : timeScaleOVerride;
      _paused = b;

    }
    static float timeScaleOVerride;
    public static float timeScale
    {
      get => timeScaleOVerride;
      set
      {
        timeScaleOVerride = value;
        Time.timeScale = value;
      }
    }

    public static async UniTask HitStop(TimeSpan duration)
    {
      timeScale = 0;
      await UniTask.Delay(duration.Milliseconds, true);
      timeScale = 1;
    }
  }
}