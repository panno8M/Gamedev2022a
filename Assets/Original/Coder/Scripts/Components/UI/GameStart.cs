using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;

namespace Assembly.Components.UI
{
  public class GameStart : MonoBehaviour
  {
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] EzLerp titleFadeProgress = new EzLerp(1);
    SimpleFader fader;
    [Zenject.Inject]
    public void DepsInject(SimpleFader fader)
    {
      this.fader = fader;
    }
    void Start()
    {
      fader.progress.secDuration = 2;
      fader.progress.SetFactor1();
      titleFadeProgress.SetFactor1();
      titleFadeProgress.SetAsIncrease();

      this.OnTriggerExitAsObservable()
        .Subscribe(_ =>
        {
          titleFadeProgress.SetAsDecrease();
        });
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