using UnityEngine;
using UniRx;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.StageGimmicks
{
  public class Campfire : MonoBehaviour
  {
    [SerializeField] DamagableComponent damagable;
    [SerializeField] ParticleSystem psBurnUp;

    void Start()
    {
      damagable.OnBroken
          .Subscribe(_ =>
          {
            psBurnUp.Play();
            Global.PlayerPool.activeSpawnPoint.position = transform.position;
          });
    }
  }
}