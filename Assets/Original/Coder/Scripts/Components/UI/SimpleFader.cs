using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Utilities;
using Assembly.GameSystem;

namespace Assembly.Components.UI
{
  public class SimpleFader : DiBehavior
  {
    public EzLerp progress = new EzLerp(1);
    public CanvasGroup canvasGroup;

    protected override void Blueprint()
    {
      throw new System.NotImplementedException();
    }

    void Start()
    {
      Observable.EveryUpdate()
        .Subscribe(_ =>
        {
          if (progress.needsCalc)
          {
            canvasGroup.alpha = progress.UpdFactor();
          }
        }).AddTo(this);
    }

    public async UniTask Rollback()
    {
      progress.secDuration = 1f;
      progress.SetAsIncrease();

      await UniTask.Delay(2000);

      progress.SetAsDecrease();

      await UniTask.Delay(1000);

      progress.secDuration = .3f;
      progress.SetAsIncrease();

      await UniTask.Delay(300);

      progress.SetAsDecrease();

      await UniTask.Delay(300);
    }

    public async UniTask Fade()
    {
      progress.secDuration = 0.5f;
      progress.SetAsIncrease();

      await UniTask.Delay(1500);

      progress.SetAsDecrease();
    }
  }
}