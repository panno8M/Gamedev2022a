using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class HurtWater : MonoBehaviour {
    ParticleSystem ps;
    List<ParticleCollisionEvent> ev;
    void Start() {
        ps = GetComponent<ParticleSystem>();
        ev = new List<ParticleCollisionEvent>();

        this.OnTriggerStay2DAsObservable()
            .Where(other => other.gameObject.layer == (int)Layer.Damagable)
            .Subscribe(other => other.GetComponent<Damagable>().AddDamage.OnNext(1));

        this.OnParticleCollisionAsObservable()
            .Where(other => other.layer == (int)Layer.Damagable)
            .Sample(TimeSpan.FromSeconds(.2f))
            .Subscribe(other => {
                int num = ps.GetCollisionEvents(other, ev);
                if (num != 0) {
                    other.GetComponent<Damagable>().AddDamage.OnNext(1);
                }
            }).AddTo(this);
    }
}
