using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.Actors
{
  public class PlayerLife : ActorBehavior<PlayerAct>
  {
    [SerializeField] DamagableComponent _damagable;
    public IDamagable damagable => _damagable;

    Subject<Unit> _OnDead = new Subject<Unit>();
    Subject<Unit> _OnRevived = new Subject<Unit>();

    public IObservable<Unit> OnDead => _OnDead;
    public IObservable<Unit> OnRevived => _OnRevived;

    protected override void OnAssemble()
    {
      damagable.Repair();
    }

    protected override void Blueprint()
    {
      _actor.ctl.Respawn
        .Subscribe(_ =>
        {
          damagable.Break();
        }).AddTo(this);

      damagable.OnBroken
          .Subscribe(_ =>
          {
            _actor.ctl.enabled = false;
            _actor.hand.holder.Forget();
            _OnDead.OnNext(Unit.Default);

            Observable.Timer(TimeSpan.FromMilliseconds(1000))
              .Subscribe(_ => Global.PlayerPool.Despawn())
              .AddTo(this);
            Observable.Timer(TimeSpan.FromMilliseconds(3000))
              .Subscribe(_ => Global.PlayerPool.Spawn())
              .AddTo(this);
          }).AddTo(this);

      damagable.OnRepaired.Subscribe(_ =>
          {
            _OnRevived.OnNext(Unit.Default);
          }).AddTo(this);
    }
  }
}
