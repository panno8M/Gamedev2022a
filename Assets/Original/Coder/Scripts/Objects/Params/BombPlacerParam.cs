using System;
using UnityEngine;

namespace Assembly.Params
{
  [CreateAssetMenu(fileName = "BombPlacer", menuName = "Params/BombPlacer")]
  public class BombPlacerParam : ScriptableObject
  {
    [Tooltip("消失した爆弾がリポップするまでの時間(秒)")]
    [SerializeField] float secTimeToRespawnFromDespawn = 5;

    public TimeSpan timeToRespawnFromDespawn => TimeSpan.FromSeconds(secTimeToRespawnFromDespawn);
  }
}
