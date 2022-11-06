using System;
using UnityEngine;

using UniRx;
using UniRx.Triggers;


public class Bomb : MonoBehaviour
{

    [SerializeField] ParticleSystem _psBurnUp;
    [SerializeField] ParticleSystem _psExplosion;
    [SerializeField] Damagable _damagable;
    [SerializeField] Interactable _interactable;
    [SerializeField] float secExplosionDelay = 4;

    Rigidbody _rb;
    MeshRenderer _mesh;

    void Start() {
        _rb = GetComponent<Rigidbody>();
        _mesh = GetComponent<MeshRenderer>();

        _damagable.OnBroken
            .Subscribe(_ => _interactable.Disable())
            .AddTo(this);

        _damagable.OnBroken
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => _psBurnUp.Play())
            .AddTo(this);

        _damagable.OnBroken
            .Delay(TimeSpan.FromSeconds(secExplosionDelay))
            .Subscribe(_ => {Explode(); Destroy(gameObject, 1);})
            .AddTo(this);

        _interactable.OnInteracted
            .Where(interactor => interactor.gameObject.CompareTag("Player"))
            .Subscribe(interactor => {
                if (interactor.HoldingItem.Value == _rb) {
                    interactor.Release();
                }
                else {
                    interactor.Hold(_rb);
                }
            });
        
    }

    void Explode(){
        _damagable.enabled = false;
        _mesh.enabled = false;
        GetComponent<Collider>().enabled = false;
        _psExplosion.Play();
    }
    
}
