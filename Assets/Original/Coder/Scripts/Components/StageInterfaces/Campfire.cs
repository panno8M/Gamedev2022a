using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Campfire : MonoBehaviour
{
    [SerializeField] Damagable damagable;
    [SerializeField] ParticleSystem psBurnUp;

    void Start() {
        damagable.OnBroken
            .Subscribe(_ => {
                psBurnUp.Play();
                Global.PlayerRespawn.activeSpawnPoint.position = transform.position;
            });
        
    }
}
