using System;
using UnityEngine;
using UniRx;
using Assembly.Components.Senses;

namespace Assembly.Components.StageGimmicks
{

  public class Bomb : MonoBehaviour
  {

    [SerializeField] ParticleSystem _psBurnUp;
    [SerializeField] ParticleSystem _psExplosion;
    [SerializeField] DamagableWrapper _damagable;
    [SerializeField] HoldableModule _holdable;
    [SerializeField] float secExplosionDelay = 4;

    Rigidbody _rb;
    MeshRenderer _mesh;

    void Start()
    {
      _rb = GetComponent<Rigidbody>();
      _mesh = GetComponent<MeshRenderer>();

      _damagable.OnBroken
          .Subscribe(_ =>
          {
            _holdable.ReleaseMe();
            _holdable.enabled = false;
          }).AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5))
          .Subscribe(_ => _psBurnUp.Play())
          .AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(secExplosionDelay))
          .Subscribe(_ => { Explode(); Destroy(gameObject, 1); })
          .AddTo(this);
    }

    void Explode()
    {
      _damagable.enabled = false;
      _mesh.enabled = false;
      GetComponent<Collider>().enabled = false;
      _psExplosion.Play();
    }
  }
}