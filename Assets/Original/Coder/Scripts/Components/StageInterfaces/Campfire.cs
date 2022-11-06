using UnityEngine;
using UniRx;

using DamageTraits.UnityBridge;

public class Campfire : MonoBehaviour
{
    [SerializeField] DamagableWrapper damagable;
    [SerializeField] ParticleSystem psBurnUp;

    void Start() {
        damagable.OnBroken
            .Subscribe(_ => {
                psBurnUp.Play();
                Global.PlayerRespawn.activeSpawnPoint.position = transform.position;
            });
        
    }
}
