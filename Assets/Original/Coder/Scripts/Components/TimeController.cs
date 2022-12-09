using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.Input;

public class TimeController : MonoBehaviour
{
  void Start()
  {
    InputControl.Instance.Pause.Subscribe(_ =>
    {
      GameTime.Pause(!GameTime.paused);
    }).AddTo(this);
  }
}
