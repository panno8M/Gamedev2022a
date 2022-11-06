using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DamageTraits;


[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Damagable : MonoBehaviour
{
    [Serializable]
    struct Inspector {
        public bool IsBroken;
        public int TotalDamageAllowed;
        public int TotalDamageFire;
        public int TotalDamageWater;
        public int TotalDamageExplosion;
    }
    [SerializeField] int stamina;
    [SerializeField] float dmgCoolDownDur;
    [SerializeField] bool disableDamage;
    [SerializeField] DamageKind allowDamageSource = DamageKind.All;
    [SerializeField] Inspector inspector;

    Subject<DamageUnit> _affect = new Subject<DamageUnit>();
    Subject<Unit> _repair = new Subject<Unit>();

    IObservable<DamageUnit> _onAffected;
    ReadOnlyReactiveProperty<int> _totalDamage;
    IObservable<Unit> _onBroken;

    public IObserver<DamageUnit> Affect => _affect;
    public IObserver<Unit> Repair => _repair;

    public IObservable<Unit> OnRepaired => _repair;
    public IObservable<DamageUnit> OnAffected => _onAffected ??
        (_onAffected = _affect
            .Where(dmg => !disableDamage && dmg.scale > 0)
            .ThrottleFirst(TimeSpan.FromSeconds(dmgCoolDownDur)));
    public ReadOnlyReactiveProperty<int> TotalDamage => _totalDamage ??
        (_totalDamage = Observable.Merge(
            OnRepaired.Select(_ => 0),
            OnAffected
            .Where(dmg => (dmg.kind & allowDamageSource) > 0)
            .Select(dmg => dmg.scale)
            .Scan((o,n)=>o+n))
         .ToReadOnlyReactiveProperty());
    public IObservable<Unit> OnBroken => _onBroken ??
        (_onBroken = TotalDamage
            .Where(_ => stamina > 0)
            .Where(total => total >= stamina)
            .AsUnitObservable());

    void Awake() {
        Inspect();
    }

    void Inspect() {
        OnBroken
            .Subscribe(x => inspector.IsBroken = true);
        TotalDamage
            .Subscribe(x => inspector.TotalDamageAllowed = x);
        OnAffected
            .Where(dmg => dmg.kind.HasFlag(DamageKind.Fire))
            .Select(dmg => dmg.scale)
            .Scan((o,n) => o+n)
            .Subscribe(x => inspector.TotalDamageFire = x);
        OnAffected
            .Where(dmg => dmg.kind.HasFlag(DamageKind.Water))
            .Select(dmg => dmg.scale)
            .Scan((o,n) => o+n)
            .Subscribe(x => inspector.TotalDamageWater = x);
        OnAffected
            .Where(dmg => dmg.kind.HasFlag(DamageKind.Explosion))
            .Select(dmg => dmg.scale)
            .Scan((o,n) => o+n)
            .Subscribe(x => inspector.TotalDamageExplosion = x);

        OnRepaired.Subscribe(_ => inspector = new Inspector());
    }
}
