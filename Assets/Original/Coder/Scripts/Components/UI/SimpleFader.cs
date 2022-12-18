using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;
using Assembly.GameSystem;

namespace Assembly.Components.UI
{
  public class SimpleFader : UniqueBehaviour<SimpleFader>
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
  }
}