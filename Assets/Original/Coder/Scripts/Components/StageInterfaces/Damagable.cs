using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[Flags]
public enum DamageKind {
    None      = 0,
    Fire      = 1 << 0,
    Water     = 1 << 1,
    Explosion = 1 << 2,
    All = Fire|Water|Explosion
}
[Serializable]
public struct DamageUnit {
    public DamageKind kind;
    public int scale;
    public DamageUnit(DamageKind kind, int scale) {
        this.kind = kind;
        this.scale = scale;
    }
    public DamageUnit(DamageKind kind){
        this.kind = kind;
        this.scale = 1;
    }

    public static DamageUnit None = new DamageUnit(DamageKind.None);
    public static DamageUnit Fire = new DamageUnit(DamageKind.Fire);
    public static DamageUnit Water = new DamageUnit(DamageKind.Water);
    public static DamageUnit Explosion = new DamageUnit(DamageKind.Explosion);
}

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
    [SerializeField] int stamina = -1;
    [SerializeField] float dmgCoolDownDur = 3;
    [SerializeField] bool allowDamage = true;
    [SerializeField] DamageKind allowDamageSource = DamageKind.All;
    [SerializeField] Inspector inspector;
    Subject<DamageUnit> _affect = new Subject<DamageUnit>();
    IObservable<DamageUnit> _onDamage;
    IObservable<DamageUnit> _onHeal;
    IObservable<DamageUnit> _onAffected;
    ReadOnlyReactiveProperty<int> _totalDamage;
    IObservable<Unit> _onBroken;

    public IObserver<DamageUnit> Affect => _affect;
    public IObservable<DamageUnit> OnDamage => _onDamage ??
        (_onDamage = _affect
            .Where(dmg => allowDamage && dmg.scale > 0)
            .ThrottleFirst(TimeSpan.FromSeconds(dmgCoolDownDur)));
    public IObservable<DamageUnit> OnHeal => _onHeal ??
        (_onDamage = _affect
            .Where(dmg => allowDamage && dmg.scale < 0));
    public IObservable<DamageUnit> OnAffected => _onAffected ??
        (_onAffected = Observable.Merge(OnDamage, OnHeal)
            .Share());
    public ReadOnlyReactiveProperty<int> TotalDamage => _totalDamage ??
        (_totalDamage = OnAffected
            .Where(dmg => (dmg.kind & allowDamageSource) > 0)
            .Select(dmg => dmg.scale)
            .Scan((o,n)=>o+n)
            .ToReadOnlyReactiveProperty());
    public IObservable<Unit> OnBroken => _onBroken ??
        (_onBroken = TotalDamage
            .Where(scl => scl == stamina)
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
    }

    public void Fix() {
        Affect.OnNext(new DamageUnit(allowDamageSource, -TotalDamage.Value));
        inspector = new Inspector();
    }
}
