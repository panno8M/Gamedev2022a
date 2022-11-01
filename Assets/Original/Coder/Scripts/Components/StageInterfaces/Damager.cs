using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Damager : MonoBehaviour {
    ParticleSystem ps;
    List<ParticleCollisionEvent> ev;
    void Start() {
        ps = GetComponent<ParticleSystem>();
        ev = new List<ParticleCollisionEvent>();

        this.OnTriggerStayAsObservable()
            .Where(other => other.gameObject.layer == (int)Layer.Damagable)
            .Subscribe(other => other.GetComponent<Damagable>().AddDamage.OnNext(1));

        this.OnParticleCollisionAsObservable()
            .Where(other => other.layer == (int)Layer.Damagable)
            .ThrottleFirst(TimeSpan.FromSeconds(.2f))
            .Subscribe(other => {
                int num = ps.GetCollisionEvents(other, ev);
                if (num != 0) {
                    other.GetComponent<Damagable>().AddDamage.OnNext(1);
                }
            }).AddTo(this);
    }
}
