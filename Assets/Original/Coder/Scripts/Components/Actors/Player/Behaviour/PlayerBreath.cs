using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;

namespace Assembly.Components.Actors.Player
{
  public class PlayerBreath : ActorBehavior<PlayerAct>
  {
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
              _actor.flame.flameQuantity = 1 - exhalingProgress.UpdFactor();
              if (exhalingProgress.PeekFactor() == 1)
              {
                exhalingProgress.SetAsDecrease();
              }
            }
          }).AddTo(this);

      exhalingProgress.OnModeChanged
          .Where(mode => mode == EzLerp.Mode.Increase)
          .Subscribe(mode =>
          {
            psFlameBreath.Play();
          }).AddTo(this);
      exhalingProgress.OnModeChanged
          .Where(mode => mode == EzLerp.Mode.Decrease)
          .Subscribe(mode =>
          {
            exhalingProgress.SetFactor0();
            _actor.flame.flameQuantity = 0;
            psFlameBreath.Stop();
          }).AddTo(this);

      Global.Control.BreathPress
          .Where(_ => _actor.flame.flameQuantity != 0)
          .Where(_ => !_actor.hand.holder.hasItem)
          .Subscribe(_ => exhalingProgress.SetAsIncrease())
          .AddTo(this);

      _actor.hand.holder.RequestHold
          .Subscribe(_ => exhalingProgress.SetAsDecrease())
          .AddTo(this);
    }
  }
}