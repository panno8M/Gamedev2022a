using UnityEngine;
using UniRx;
using Assembly.GameSystem.Damage;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.StageGimmicks
{
  public class CheckPoint : MonoBehaviour, ISpawnSpot
  {
    public Vector3 spawnPosition => transform.position;
    public void OnActivate()
    {
    }
    public void OnDeactivate()
    {
    }
  }
}