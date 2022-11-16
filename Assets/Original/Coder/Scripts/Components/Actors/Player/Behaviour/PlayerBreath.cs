using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Actors.Player
{
  public class PlayerBreath : MonoBehaviour
  {
    [SerializeField] PlayerAct _player;
    [SerializeField] ParticleSystem psFlameBreath;

    ReactiveProperty<bool> _IsExhaling = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> IsExhaling => _IsExhaling;
    public IObservable<bool> OnBreathStart => _IsExhaling.Where(x => x);
    public IObservable<bool> OnBreathStop => _IsExhaling.Where(x => !x);


    [SerializeField] float _msecExhalableLimit = 3000;
    [SerializeField] float _msecExhaling;
    [SerializeField] float _msecCooldown;
    public float msecExhalableLimit => _msecExhalableLimit;
    public float msecExhaling => _msecExhaling;
    public float msecCooldown => _msecCooldown;
    public bool isCoolingDown => msecCooldown != 0;

    void Awake()
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
          .Where(_ => !_player.interactor.holder.HoldingItem.Value)
          .Where(_ => !isCoolingDown)
          .Subscribe(_ => _IsExhaling.Value = true)
          .AddTo(this);

      Global.Control.BreathRelease
          .Where(_ => !isCoolingDown)
          .Subscribe(_ => _IsExhaling.Value = false)
          .AddTo(this);
      _player.interactor.holder.RequestHold
          .Where(_ => Global.Control.BreathInput.Value)
          .Where(_ => !isCoolingDown)
          .Subscribe(_ => _IsExhaling.Value = false)
          .AddTo(this);
      this.FixedUpdateAsObservable()
          .Where(_ => msecExhaling > msecExhalableLimit)
          .Where(_ => !isCoolingDown)
          .Subscribe(_ => _IsExhaling.Value = false)
          .AddTo(this);
    }


    void SetOveruseLimitation()
    {
      _player.flapCtl.TightenLimit(0);
    }

    void RemoveOveruseLimitation()
    {
      _player.flapCtl.ResetLimit();
    }


    void CooldownStart()
    {
      psFlameBreath.Stop();
      _msecCooldown = msecExhaling;
      _msecExhaling = 0;
    }


    void Reset()
    {
      SetDefaultComponent();
    }

    void SetDefaultComponent()
    {
      _player = GetComponent<PlayerAct>();
    }
  }
}