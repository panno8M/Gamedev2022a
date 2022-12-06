using UnityEngine;

namespace Assembly.GameSystem
{
  public static class GameTime
  {
    static bool _paused;
    public static bool paused => _paused;
    public static void Pause(bool b = true)
    {
      Time.timeScale = b ? 0 : 1;
      _paused = b;

    }
  }
}