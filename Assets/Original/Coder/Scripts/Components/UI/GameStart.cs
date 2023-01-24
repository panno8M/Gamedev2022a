using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;

namespace Assembly.Components.UI
{
  public class GameStart : MonoBehaviour
  {
    SimpleFader fader;
    [Zenject.Inject]
    public void DepsInject(SimpleFader fader)
    {
      this.fader = fader;
    }

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] EzLerp titleFadeProgress = new EzLerp(1);
    SafetyTrigger trigger;

    void Start()
    {
      trigger = GetComponent<SafetyTrigger>();

      titleFadeProgress.SetFactor1();
      titleFadeProgress.SetAsIncrease();

      trigger.OnExit
        .Subscribe(titleFadeProgress.SetAsDecrease);
      this.UpdateAsObservable()
        .Where(titleFadeProgress.isNeedsCalc)
        .Select(titleFadeProgress.UpdFactor)
        .Subscribe(fac =>
        {
          canvasGroup.alpha = fac;
        });
    }
  }
}