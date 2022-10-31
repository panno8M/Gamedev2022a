using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Damagable : MonoBehaviour
{
    [SerializeField] float dmgCoolDownDur = 3;
    public Subject<int> AddDamage = new Subject<int>();
    public IObservable<int> OnDamage;

    void Awake() {
        OnDamage = AddDamage
            .ThrottleFirst(TimeSpan.FromSeconds(dmgCoolDownDur))
            .Share();
    }
}
