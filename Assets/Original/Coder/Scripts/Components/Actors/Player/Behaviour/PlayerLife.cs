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
            _player.Damagable.Break();
          }).AddTo(this);

      _player.Damagable.OnBroken
          .Subscribe(_ =>
          {
            _player.interactor.Forget();
            _player.controlMethod = PlayerAct.ControlMethod.IgnoreAnyInput;
          });

      _player.Damagable.OnBroken
        .Delay(System.TimeSpan.FromMilliseconds(1000))
        .Subscribe(_ => Global.PlayerRespawn.Return());

      _player.Damagable.OnBroken
        .Delay(System.TimeSpan.FromMilliseconds(3000))
        .Subscribe(_ => Global.PlayerRespawn.Rent());

      Global.PlayerRespawn.OnSpawn
        .Subscribe(instance =>
        {
          instance.Damagable.Repair();
          instance.controlMethod = PlayerAct.ControlMethod.ActiveAll;
          instance.transform.position = Global.PlayerRespawn.activeSpawnPoint.position;
        });
    }
  }
}
