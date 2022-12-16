using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;

namespace Assembly.Components.Actors
{
  public class PlayerBreath : ActorBehavior<PlayerAct>
  {
    [SerializeField] PlayerFlameReceptor _flameReceptor;
    [SerializeField] ParticleSystem psFlameBreath;

    public EzLerp exhalingProgress = new EzLerp(3, EzLerp.Mode.Decrease);

    protected override void Blueprint()
    {
      this.FixedUpdateAsObservable()
          .Subscribe(_ =>
          {
            if (exhalingProgress.isIncreasing)
            {
              psFlameBreath.transform.LookAt(Global.Control.MousePosStage.Value);
              _flameReceptor.flameQuantity = 1 - exhalingProgress.UpdFactor();
              if (exhalingProgress.PeekFactor() == 1)
              {
                exhalingProgress.SetAsDecrease();
              }
            }
          }).AddTo(this);

      exhalingProgress.OnModeChanged
          .Subscribe(mode =>
          {
            if (mode == EzLerp.Mode.Increase)
            {
              psFlameBreath.Play();
            }
            else
            {
              _flameReceptor.flameQuantity = 0;
              psFlameBreath.Stop();
            }
          }).AddTo(this);

      Global.Control.BreathPress
          .Where(_ => _flameReceptor.flameQuantity != 0)
          .Where(_ => !_actor.holder.hasItem)
          .Subscribe(_ => exhalingProgress.SetAsIncrease())
          .AddTo(this);

      _actor.holder.RequestHold
          .Subscribe(_ => exhalingProgress.SetAsDecrease())
          .AddTo(this);
    }
  }
}