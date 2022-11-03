using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[Flags]
public enum DamageKind {
    Any       = 0,
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

    public static DamageUnit None = new DamageUnit(DamageKind.Any);
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
        public int TotalDamageFire;
        public int TotalDamageWater;
        public int TotalDamageExplosion;
    }
    [SerializeField] float dmgCoolDownDur = 3;
    [SerializeField] bool allowDamage = true;
    [SerializeField] DamageKind allowDamageSource = DamageKind.All;
    [SerializeField] Inspector inspector;
    public Subject<DamageUnit> _addDamage = new Subject<DamageUnit>();
    public IObserver<DamageUnit> AddDamage => _addDamage;
    public IObservable<DamageUnit> OnDamage;
    public ReadOnlyReactiveProperty<int> TotalDamage;

    void Awake() {
        OnDamage = _addDamage
            .Where(_ => allowDamage)
            .ThrottleFirst(TimeSpan.FromSeconds(dmgCoolDownDur))
            .Share();
        TotalDamage = OnDamage
            .Where(dmg => (dmg.kind & allowDamageSource) > 0)
            .Select(dmg => dmg.scale)
            .Scan((o,n)=>o+n)
            .ToReadOnlyReactiveProperty();

        OnDamage
            .Where(dmg => dmg.kind.HasFlag(DamageKind.Fire))
            .Select(dmg => dmg.scale)
            .Scan((o,n) => o+n)
            .Subscribe(x => inspector.TotalDamageFire = x);
        OnDamage
            .Where(dmg => dmg.kind.HasFlag(DamageKind.Water))
            .Select(dmg => dmg.scale)
            .Scan((o,n) => o+n)
            .Subscribe(x => inspector.TotalDamageWater = x);
        OnDamage
            .Where(dmg => dmg.kind.HasFlag(DamageKind.Explosion))
            .Select(dmg => dmg.scale)
            .Scan((o,n) => o+n)
            .Subscribe(x => inspector.TotalDamageExplosion = x);
    }
}
