using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Damagable : MonoBehaviour
{
    [Serializable]
    struct Inspector {
        public int totalDamage;
    }
    [SerializeField] float dmgCoolDownDur = 3;
    [SerializeField] Inspector inspector;
    public Subject<int> AddDamage = new Subject<int>();
    public ReadOnlyReactiveProperty<int> TotalDamage;
    public IObservable<int> OnDamage;

    void Awake() {
        OnDamage = AddDamage
            .ThrottleFirst(TimeSpan.FromSeconds(dmgCoolDownDur))
            .Share();
        TotalDamage = OnDamage
            .Scan((o,n) => n + o)
            .ToReadOnlyReactiveProperty<int>(0);

        TotalDamage
            .Subscribe(x => inspector.totalDamage = x);
    }
}
