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
      Time.timeScale = b ? 0 : timeScaleOVerride;
      Debug.Log(b);
      _paused = b;

    }
    static float timeScaleOVerride = 1;
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
      Debug.Log("aaa");
      timeScale = 1;
    }
  }
}