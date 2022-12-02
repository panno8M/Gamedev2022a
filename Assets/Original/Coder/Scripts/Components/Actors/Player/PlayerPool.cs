using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{
  public class PlayerPool : ObjectPool<PlayerAct>
  {
    [SerializeField] GameObject _prefPlayer;
    public Transform activeSpawnPoint;
    PlayerAct _player;
    public PlayerAct player => _player ?? Spawn();

    protected override PlayerAct CreateInstance()
    {
      if (_player == null)
      {
        _player = (PlayerAct)FindObjectOfType(typeof(PlayerAct));

        if (_player == null)
        {
          var result = GameObject.Instantiate(_prefPlayer, activeSpawnPoint.position, Quaternion.identity);
          result.name = _prefPlayer.name;
          _player = result.GetComponent<PlayerAct>();
        }
      }

      return _player;
    }

    public void Despawn() { Despawn(_player); }

    protected override void OnInit()
    {
      Spawn();
    }
  }
}