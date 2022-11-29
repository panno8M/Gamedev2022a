using UnityEngine;
using UniRx;
using Senses;

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
            Global.PlayerRespawn.activeSpawnPoint.position = transform.position;
          });
    }
  }
}