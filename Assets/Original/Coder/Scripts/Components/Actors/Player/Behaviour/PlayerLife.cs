using System;
using UniRx;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Actors
{
  public class PlayerLife : ActorBehavior<PlayerAct>
  {
    protected override void OnAssemble()
    {
      _actor.damagable.Repair();
    }

    protected override void Blueprint()
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
              .Subscribe(_ => Global.PlayerPool.Despawn())
              .AddTo(this);
            Observable.Timer(TimeSpan.FromMilliseconds(3000))
              .Subscribe(_ => Global.PlayerPool.Spawn(ObjectCreateInfo.None))
              .AddTo(this);
          }).AddTo(this);
    }
  }
}
