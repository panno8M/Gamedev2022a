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
            Global.PlayerRespawn.Return();
            _player.controlMethod = PlayerAct.ControlMethod.IgnoreAnyInput;
          });
    }
  }
}
