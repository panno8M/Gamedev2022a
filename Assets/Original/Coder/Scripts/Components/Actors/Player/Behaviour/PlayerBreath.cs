using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Actors
{
  public class PlayerBreath : ActorBehavior<PlayerAct>
  {
    [SerializeField] ParticleSystem psFlameBreath;

    ReactiveProperty<bool> _IsExhaling = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> IsExhaling => _IsExhaling;


    [SerializeField] float _msecExhalableLimit = 3000;
    float _msecExhaling;
    float _msecCooldown;
    public float msecExhalableLimit => _msecExhalableLimit;
    public float msecExhaling => _msecExhaling;
    public float msecCooldown => _msecCooldown;
    public bool isCoolingDown => msecCooldown != 0;

    protected override void OnInit()
    {
      this.FixedUpdateAsObservable()
          .TimeInterval()
          .Where(_ => IsExhaling.Value)
          .Subscribe(delta =>
          {
            psFlameBreath.transform.LookAt(Global.Control.MousePosStage.Value);
            _msecExhaling += delta.Interval.Milliseconds;
          }).AddTo(this);

      this.FixedUpdateAsObservable()
          .FrameTimeInterval()
          .Where(_ => isCoolingDown)
          .Subscribe(delta =>
          {
            _msecCooldown -= delta.Interval.Milliseconds;

            if (msecCooldown <= 0)
            {
              _msecCooldown = 0;
              RemoveOveruseLimitation();
            }
          }).AddTo(this);

      IsExhaling
          .Subscribe(b =>
          {
            if (b)
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
          .Where(_ => !_actor.interactor.holder.hasItem)
          .Where(_ => !isCoolingDown)
          .Subscribe(_ => _IsExhaling.Value = true)
          .AddTo(this);

      Observable
          .Merge(
              Global.Control.BreathRelease,
              _actor.interactor.holder.RequestHold
                  .Where(_ => Global.Control.BreathInput.Value)
                  .AsUnitObservable(),
              this.FixedUpdateAsObservable()
                  .Where(_ => msecExhaling > msecExhalableLimit))
          .Where(_ => !isCoolingDown)
          .Subscribe(_ => _IsExhaling.Value = false)
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
      _msecCooldown = msecExhaling;
      _msecExhaling = 0;
    }
  }
}