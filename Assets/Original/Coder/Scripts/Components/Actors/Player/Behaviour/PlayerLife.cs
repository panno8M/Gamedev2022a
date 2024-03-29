using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.Actors.Player
{
  public class PlayerLife : ActorBehavior<PlayerAct>
  {
    UI.SimpleFader fader;
    [Zenject.Inject]
    public void DepsInject(UI.SimpleFader fader)
    {
      this.fader = fader;
    }
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
          .Subscribe(_ => DieSequence().Forget());

      damagable.OnRepaired.Subscribe(_ =>
          {
            _OnRevived.OnNext(Unit.Default);
          }).AddTo(this);
    }
    async UniTask DieSequence()
    {
      _actor.ctl.enabled = false;
      _actor.hand.holder.Forget();
      _OnDead.OnNext(Unit.Default);

      await UniTask.Delay(500);

      fader.Transition(.5f, 1f).Forget();

      await UniTask.Delay(500);

      _actor.rebirth.Despawn();

      await UniTask.Delay(1000);

      _actor.rebirth.Spawn();
    }
  }
}
