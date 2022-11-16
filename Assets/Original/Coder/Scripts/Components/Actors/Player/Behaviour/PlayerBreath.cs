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
    [SerializeField] float msecOverheatCooldown = 3000;


    void Awake()
    {
      this.FixedUpdateAsObservable()
          .Where(_ => Global.Control.BreathInput.Value)
          .Subscribe(_ =>
          {
            psFlameBreath.transform.LookAt(Global.Control.MousePosStage.Value);
          }).AddTo(this);

      _player.OnBreathStart
          .Subscribe(_ => psFlameBreath.Play()).AddTo(this);
      _player.OnBreathStop
          .Subscribe(_ => psFlameBreath.Stop()).AddTo(this);

      _player.OnBreathStart
          .Subscribe(_ => _player.flapCtl.OverrideLimit(0)).AddTo(this);
      _player.OnBreathStop
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