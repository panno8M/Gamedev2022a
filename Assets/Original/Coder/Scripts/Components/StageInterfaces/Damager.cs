using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DamageTraits;

public class Damager : MonoBehaviour {
    [SerializeField] LayerMask lmDamagable = new Layers(Layer.Damagable);
    [SerializeField] DamageUnit _damageUnit;
    ParticleSystem ps;
    List<ParticleCollisionEvent> ev;
    void Start() {
        ps = GetComponent<ParticleSystem>();
        ev = new List<ParticleCollisionEvent>();

        this.OnTriggerStayAsObservable()
            .Where(other => (lmDamagable & 1<<other.gameObject.layer) != 0)
            .Subscribe(other => other.GetComponent<Damagable>().Affect.OnNext(_damageUnit));

        this.OnParticleCollisionAsObservable()
            .Where(other => (lmDamagable & 1<<other.layer) != 0)
            .ThrottleFirst(TimeSpan.FromSeconds(.2f))
            .Subscribe(other => {
                int num = ps.GetCollisionEvents(other, ev);
                if (num != 0) {
                    other.GetComponent<Damagable>().Affect.OnNext(_damageUnit);
                }
            }).AddTo(this);
    }
}
