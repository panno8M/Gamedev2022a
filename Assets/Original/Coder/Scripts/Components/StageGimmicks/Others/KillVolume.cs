using UnityEngine;
using UniRx;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  [RequireComponent(typeof(BoxCollider))]
  public class KillVolume : MonoBehaviour
  {
    SafetyTrigger _trigger;
    void Start()
    {
      _trigger = GetComponent<SafetyTrigger>();

      _trigger.OnEnter
        .Select(ohter => ohter.GetComponent<IDamagable>())
        .Where(damagable => damagable != null)
        .Subscribe(damagable =>
        {
          damagable.Break();
        });
    }
  }
}