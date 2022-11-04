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
        public int TotalDamageFire;
        public int TotalDamageWater;
        public int TotalDamageExplosion;
    }
    [SerializeField] int stamina = -1;
    [SerializeField] float dmgCoolDownDur = 3;
    [SerializeField] bool allowDamage = true;
    [SerializeField] DamageKind allowDamageSource = DamageKind.All;
    [SerializeField] Inspector inspector;
    public Subject<DamageUnit> _addDamage = new Subject<DamageUnit>();
    public IObservable<DamageUnit> _onDamage;
    public ReadOnlyReactiveProperty<int> _totalDamage;
    public IObservable<Unit> _onBroken;

    public IObserver<DamageUnit> AddDamage => _addDamage;
    public IObservable<DamageUnit> OnDamage => _onDamage ??
        (_onDamage = _addDamage
            .Where(_ => allowDamage)
            .ThrottleFirst(TimeSpan.FromSeconds(dmgCoolDownDur))
            .Share());
    public ReadOnlyReactiveProperty<int> TotalDamage => _totalDamage ??
        (_totalDamage = OnDamage
            .Where(dmg => (dmg.kind & allowDamageSource) > 0)
            .Select(dmg => dmg.scale)
            .Scan((o,n)=>o+n)
            .ToReadOnlyReactiveProperty());
    public IObservable<Unit> OnBroken => _onBroken ??
        (_onBroken = TotalDamage
            .Where(scl => scl == stamina)
            .AsUnitObservable());

    void Awake() {
        OnBroken.Subscribe(_ => _addDamage.OnCompleted());

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
