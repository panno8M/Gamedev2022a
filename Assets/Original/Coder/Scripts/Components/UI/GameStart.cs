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

      fader.progress.secDuration = 2;
      fader.progress.SetFactor1();
      titleFadeProgress.SetFactor1();
      titleFadeProgress.SetAsIncrease();

      trigger.OnEnter
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