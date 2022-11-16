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
    [SerializeField] float msecExhalableLimit = 3000;

    ReactiveProperty<bool> _IsExhaling = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> IsExhaling => _IsExhaling;
    public IObservable<bool> OnBreathStart => _IsExhaling.Where(x => x);
    public IObservable<bool> OnBreathStop => _IsExhaling.Where(x => !x);

    [SerializeField] float msecExhaling;
    float msecNeedsRest => msecExhaling;

    void Awake()
    {
      this.FixedUpdateAsObservable()
          .Where(_ => IsExhaling.Value)
          .TimeInterval()
          .Subscribe(delta =>
          {
            psFlameBreath.transform.LookAt(Global.Control.MousePosStage.Value);
            msecExhaling += delta.Interval.Milliseconds;
          }).AddTo(this);

      IsExhaling
          .Subscribe(b =>
          {
            if (b)
            {
              psFlameBreath.Play();
              _player.flapCtl.TightenLimit(0);
            }
            else
            {
              psFlameBreath.Stop();
              _player.flapCtl.ResetLimit();
              msecExhaling = 0;

            }
          }).AddTo(this);

      Global.Control.BreathPress
          .Where(_ => !_player.interactor.holder.HoldingItem.Value)
          .Subscribe(_ => _IsExhaling.Value = true)
          .AddTo(this);

      Global.Control.BreathRelease
          .Subscribe(_ => _IsExhaling.Value = false)
          .AddTo(this);
      _player.interactor.holder.RequestHold
          .Where(_ => Global.Control.BreathInput.Value)
          .Subscribe(_ => _IsExhaling.Value = false)
          .AddTo(this);
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