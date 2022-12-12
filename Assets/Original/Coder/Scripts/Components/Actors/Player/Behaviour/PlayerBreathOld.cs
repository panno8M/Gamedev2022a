using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utilities;

namespace Assembly.Components.Actors
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
              if (exhalingProgress == 1)
              {
                exhalingProgress.SetAsDecrease();
              }
            }
            else
            {
              if (exhalingProgress == 0)
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
          .Where(_ => !_actor.holder.hasItem)
          .Where(_ => exhalingProgress.isDecreasing)
          .Subscribe(_ => exhalingProgress.SetAsIncrease())
          .AddTo(this);

      _actor.holder.RequestHold
          .Where(_ => Global.Control.BreathInput.Value)
          .AsUnitObservable()
          .Merge(Global.Control.BreathRelease)
          .Where(_ => exhalingProgress.isIncreasing)
          .Subscribe(_ => exhalingProgress.SetAsDecrease())
          .AddTo(this);
    }


    void SetOveruseLimitation()
    {
      _actor.flapCtl.TightenLimit(0);
      _actor.param.SetAsKnackered();
    }

    void RemoveOveruseLimitation()
    {
      _actor.flapCtl.ResetLimit();
      _actor.param.SetAsNormal();
    }


    void CooldownStart()
    {
      psFlameBreath.Stop();
    }
  }
}