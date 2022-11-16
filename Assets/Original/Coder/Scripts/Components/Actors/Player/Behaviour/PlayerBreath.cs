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

    IObservable<Unit> _onBreathStart;
    IObservable<Unit> _onBreathStop;
    public IObservable<Unit> OnBreathStart => _onBreathStart ??
        (_onBreathStart = Global.Control.BreathPress
            .Where(_ => !_player.interactor.holder.HoldingItem.Value));

    public IObservable<Unit> OnBreathStop => _onBreathStop ??
        (_onBreathStop = Observable.Merge(
            Global.Control.BreathRelease,
            _player.interactor.holder.RequestHold.AsUnitObservable()
                .Where(_ => Global.Control.BreathInput.Value)));


    void Awake()
    {
      this.FixedUpdateAsObservable()
          .Where(_ => Global.Control.BreathInput.Value)
          .Subscribe(_ =>
          {
            psFlameBreath.transform.LookAt(Global.Control.MousePosStage.Value);
          }).AddTo(this);

      OnBreathStart
          .Subscribe(_ => psFlameBreath.Play()).AddTo(this);
      OnBreathStop
          .Subscribe(_ => psFlameBreath.Stop()).AddTo(this);

      OnBreathStart
          .Subscribe(_ => _player.flapCtl.OverrideLimit(0)).AddTo(this);
      OnBreathStop
          .Subscribe(_ => _player.flapCtl.ResetLimit()).AddTo(this);
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