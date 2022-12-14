using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.Input;

public class TimeController : MonoBehaviour
{
  InputControl control;
  [Zenject.Inject]
  public void DepsInject(InputControl control)
  {
    this.control = control;
  }

  void Start()
  {
    control.PauseInput.Where(x => x).Subscribe(_ =>
    {
      GameTime.Pause(!GameTime.paused);
    }).AddTo(this);
  }
}
