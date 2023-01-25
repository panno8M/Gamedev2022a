#if UNITY_EDITOR
// #define DEBUG_CHECKPOINT
#endif

using UnityEngine;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.StageGimmicks
{
  public class CheckPoint : MonoBehaviour, ISpawnSpot, ITransactionDispatcher
  {
#if DEBUG_CHECKPOINT
    [SerializeField]
#endif
    PlayerAct player;
#if DEBUG_CHECKPOINT
    [SerializeField]
#endif
    Rollback rollback;
    [Zenject.Inject]
    public void DepsInject(PlayerAct player, Rollback rollback)
    {
      this.player = player;
      this.rollback = rollback;
    }

    [SerializeField] bool activateOnAwake;
    public Vector3 spawnPosition => transform.position;

    void Start()
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