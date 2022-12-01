using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Senses.Pain;

namespace Assembly.Components.Actors.Player
{
  public class PlayerLife : MonoBehaviour
  {
    [SerializeField] PlayerAct _player;
    [SerializeField] int _dmgAmountOverTime = 1;
    [SerializeField] float _msDmgInterval = 300;
    void Awake()
    {
      Global.Control.Respawn
          .Where(_ => _player.isControlAccepting)
          .Subscribe(_ =>
          {
            _player.damagable.Break();
          }).AddTo(this);

      _player.damagable.OnBroken
          .Subscribe(_ =>
          {
            _player.interactor.Forget();
            _player.controlMethod.Value = PlayerAct.ControlMethod.IgnoreAnyInput;

            Observable.Timer(TimeSpan.FromMilliseconds(1000))
              .Subscribe(_ => Global.PlayerRespawn.Return())
              .AddTo(this);
            Observable.Timer(TimeSpan.FromMilliseconds(3000))
              .Subscribe(_ => Global.PlayerRespawn.Rent())
              .AddTo(this);
          }).AddTo(this);

      Global.PlayerRespawn.OnSpawn
        .Subscribe(instance =>
        {
          instance.InitializeCondition();
        }).AddTo(this);

      this.FixedUpdateAsObservable()
        .ThrottleFirst(TimeSpan.FromMilliseconds(_msDmgInterval))
        .Subscribe(_ =>
        {
          _player.damagable.Affect(new DamageUnit(DamageKind.Water, _dmgAmountOverTime));
        }).AddTo(this);
    }
  }
}
