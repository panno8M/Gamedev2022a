using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors.Player
{
  public class PlayerLife : MonoBehaviour
  {
    [SerializeField] PlayerAct _player;
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
          }).AddTo(this);

      _player.damagable.OnBroken
        .Delay(TimeSpan.FromMilliseconds(1000))
        .Subscribe(_ => Global.PlayerRespawn.Return())
        .AddTo(this);

      _player.damagable.OnBroken
        .Delay(TimeSpan.FromMilliseconds(3000))
        .Subscribe(_ => Global.PlayerRespawn.Rent())
        .AddTo(this);

      Global.PlayerRespawn.OnSpawn
        .Subscribe(instance =>
        {
            instance.InitializeCondition();
        }).AddTo(this);
    }
  }
}
