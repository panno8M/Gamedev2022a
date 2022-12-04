using UnityEngine;
using UniRx;

namespace Assembly.Components
{
  public class GameTime : UniqueBehaviour<GameTime>
  {
    bool _paused;
    public bool paused => _paused;
    void Start()
    {
      Global.Control.Pause.Subscribe(_ =>
      {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
      }).AddTo(this);
    }
  }
}