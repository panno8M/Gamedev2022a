using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Utilities;
using Assembly.GameSystem;

namespace Assembly.Components.UI
{
  public class SimpleFader : DiBehavior
  {
    [SerializeField] EzLerp progress = new EzLerp(2);
    public CanvasGroup canvasGroup;

    void Start() => Initialize();

    protected override void Blueprint()
    {
      progress.SetFactor1();

      Observable.EveryUpdate()
        .Subscribe(_ =>
        {
          if (progress.needsCalc)
          {
            canvasGroup.alpha = progress.UpdFactor();
          }
        }).AddTo(this);
    }

    public async UniTask Fade(float secDuration)
    {
      progress.secDuration = secDuration;
      progress.SetAsIncrease();
      await UniTask.Delay(TimeSpan.FromSeconds(secDuration));
    }
    public async UniTask Unfade(float secDuration)
    {
      progress.secDuration = secDuration;
      progress.SetAsDecrease();
      await UniTask.Delay(TimeSpan.FromSeconds(secDuration));
    }

    public async UniTask Rollback()
    {
      await Fade(1);
      await UniTask.Delay(1000);
      await Fade(.3f);
      await Unfade(.3f);
    }

    public async UniTask Transition(float secFade, float secWait)
    {
      await Fade(secFade);
      await UniTask.Delay(TimeSpan.FromSeconds(secWait));
      await Unfade(secFade);
    }
  }
}