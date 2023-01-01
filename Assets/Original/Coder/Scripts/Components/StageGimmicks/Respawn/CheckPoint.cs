using UnityEngine;
using UniRx;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.StageGimmicks
{
  public class CheckPoint : MonoBehaviour, ISpawnSpot, ITransactionDispatcher
  {
    PlayerAct player;
    Rollback rollback;
    [Zenject.Inject]
    public void DepsInject(PlayerAct player, Rollback rollback)
    {
      this.player = player;
      this.rollback = rollback;
    }

    [SerializeField] bool activateOnAwake;
    public Vector3 spawnPosition => transform.position;

    void Awake()
    {
      if (activateOnAwake)
      {
        player.rebirth.activeSpot = this;
        rollback.UpdateTransaction(this);
      }
    }

    public void OnActivate()
    {
    }
    public void OnDeactivate()
    {
    }
  }
}