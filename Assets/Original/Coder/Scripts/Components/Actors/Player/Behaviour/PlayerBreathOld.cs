using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;

namespace Assembly.Components.Actors.Player
{
  public class PlayerBreathOld: ActorBehavior<PlayerAct>
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
              if (exhalingProgress.UpdFactor() == 1)
              {
                exhalingProgress.SetAsDecrease();
              }
            }
            else
            {
              if (exhalingProgress.UpdFactor() == 0)
              {
                RemoveOveruseLimitation();
              }
            }
          }).AddTo(this);

      exhalingProgress.OnModeChanged
          .Subscribe(mode =>
          {
            if (mode == EzLerp.Mode.Increase)
            {
              psFlameBreath.Play();
              SetOveruseLimitation();
            }
            else
            {
              CooldownStart();
            }
          }).AddTo(this);

      Global.Control.BreathPress
          .Where(_ => !_actor.hand.holder.hasItem)
          .Where(_ => exhalingProgress.isDecreasing)
          .Subscribe(_ => exhalingProgress.SetAsIncrease())
          .AddTo(this);

      _actor.hand.holder.RequestHold
          .Where(_ => Global.Control.BreathInput.Value)
          .AsUnitObservable()
          .Merge(Global.Control.BreathRelease)
          .Where(_ => exhalingProgress.isIncreasing)
          .Subscribe(_ => exhalingProgress.SetAsDecrease())
          .AddTo(this);
    }


    void SetOveruseLimitation()
    {
      _actor.wings.TightenLimit(0);
      _actor.behavior.SetAsKnackered();
    }

    void RemoveOveruseLimitation()
    {
      _actor.wings.ResetLimit();
      _actor.behavior.SetAsNormal();
    }


    void CooldownStart()
    {
      psFlameBreath.Stop();
    }
  }
}