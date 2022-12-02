using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Senses.Pain;
using Senses;

namespace Assembly.Components.Actors
{
  public class PlayerLife : ActorBehavior<PlayerAct>
  {
    [SerializeField] int _dmgAmountOverTime = 1;
    [SerializeField] float _msDmgInterval = 300;

    protected override void OnRebuild()
    {
      _actor.damagable.Repair();
    }

    protected override void OnInit()
    {
      Global.Control.Respawn
          .Where(_ => _actor.isControlAccepting)
          .Subscribe(_ =>
          {
            _actor.damagable.Break();
          }).AddTo(this);

      _actor.damagable.OnBroken
          .Subscribe(_ =>
          {
            _actor.interactor.Forget();
            _actor.ControlMethod.Value = PlayerAct.ControlMethods.IgnoreAnyInput;

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
          instance.Rebuild();
        }).AddTo(this);

      this.FixedUpdateAsObservable()
        .ThrottleFirst(TimeSpan.FromMilliseconds(_msDmgInterval))
        .Subscribe(_ =>
        {
          _actor.damagable.Affect(new DamageUnit(DamageKind.Water, _dmgAmountOverTime));
        }).AddTo(this);
    }
  }
}
