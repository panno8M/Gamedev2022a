using System;
using UnityEngine;

namespace Assembly.Params
{
  [CreateAssetMenu(fileName = "Bomb", menuName = "Params/Bomb")]
  public class BombParam : ScriptableObject
  {
    [SerializeField] float secTimeToBurnUpFromBroken = 0.5f;
    [SerializeField] float secTimeToExplodeFromBroken = 4;
    public AnimationCurve exprodeColorCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public TimeSpan timeToBurnUpFromBroken => TimeSpan.FromSeconds(secTimeToBurnUpFromBroken);
    public TimeSpan timeToExplodeFromBroken => TimeSpan.FromSeconds(secTimeToExplodeFromBroken);
  }
}