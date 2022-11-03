using System;
using UnityEngine;

using UniRx;
using UniRx.Triggers;


public class Bomb : MonoBehaviour
{

    [SerializeField] ParticleSystem IgnitionBurn;
    [SerializeField] ParticleSystem BombExplosion;
    [SerializeField] Damagable damagable;
    [SerializeField] MeshRenderer meshRenderer;
    public float WaitExplode;
    public float WaitDestroy;

    void Start()
    {   
    
        var slowupdate = this
            .UpdateAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(.5f))
            .Share();

        damagable.OnBroken
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => IgnitionBurn.Play())
            .AddTo(this);;

        damagable.OnBroken
            .Delay(TimeSpan.FromSeconds(WaitExplode))
            .Subscribe(_ => Explode())
            .AddTo(this);

        damagable.OnBroken
            .Delay(TimeSpan.FromSeconds(WaitDestroy))
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
        
    }

    void Explode(){
        damagable.enabled = false;
        meshRenderer.enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        BombExplosion.Play();
    }
    
}
