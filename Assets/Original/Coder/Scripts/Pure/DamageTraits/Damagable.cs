using System;
using UniRx;

namespace DamageTraits {
    [Serializable]
    public struct DamagableParams {
        public int stamina;
        public float secCoolDownToNextDamage;
        public DamageKind allowDamageSource;
        public bool damageDisabled;
        public TimeSpan coolDownToNextDamage => TimeSpan.FromSeconds(secCoolDownToNextDamage);
    }

    [Serializable]
    public class Damagable {
        public Damagable(DamagableParams param) {
            _param = param;

            _affect = new Subject<DamageUnit>();
            _repair = new Subject<Unit>();

            _onAffected = _affect
                .Where(dmg => !_param.damageDisabled && dmg.scale > 0)
                .ThrottleFirst(param.coolDownToNextDamage);

            _totalDamage = Observable
                .Merge(
                    OnRepaired.Select(_ => -_totalDamage.Value),
                    OnAffected
                    .Where(dmg => (dmg.kind & _param.allowDamageSource) > 0)
                    .Select(dmg => dmg.scale))
                .Scan((o,n)=>o+n)
                .ToReadOnlyReactiveProperty();

            _onBroken = TotalDamage
                .Where(_ => _param.stamina > 0)
                .Where(total => total >= _param.stamina)
                .AsUnitObservable();
        }

        DamagableParams _param;

        Subject<DamageUnit> _affect;
        Subject<Unit> _repair;

        IObservable<DamageUnit> _onAffected;
        ReadOnlyReactiveProperty<int> _totalDamage;
        IObservable<Unit> _onBroken;

        public IObservable<Unit> OnRepaired => _repair;
        public IObservable<DamageUnit> OnAffected => _onAffected;
        public ReadOnlyReactiveProperty<int> TotalDamage => _totalDamage;
        public IObservable<Unit> OnBroken => _onBroken;

        public void Affect(DamageUnit du) { _affect.OnNext(du); }
        public void Repair() { _repair.OnNext(Unit.Default); }
    }
}
