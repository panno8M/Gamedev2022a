using System;
using UniRx;

namespace Senses.Pain
{
  [Serializable]
  public class Damagable : IDamagable
  {
    public Damagable(DamagableParam param)
    {
      _param = param;

      _affect = new Subject<DamageUnit>();
      _repair = new Subject<Unit>();
      _totalDamage = new ReactiveProperty<int>();

      _onAffected = _affect
        .Where(dmg => !_param.damageDisabled)
        .ThrottleFirst(param.coolDownToNextDamage);

      _onAffected
        .Where(dmg => dmg.kind.MatchAny(_param.allowDamageSource))
        .Subscribe(dmg => _totalDamage.Value += dmg.scale);
      _repair.Subscribe(_ => _totalDamage.Value = 0);

      _onBroken = TotalDamage
        .Where(_ => isBroken)
        .AsUnitObservable();
    }

    DamagableParam _param;

    Subject<DamageUnit> _affect;
    Subject<Unit> _repair;

    IObservable<DamageUnit> _onAffected;
    ReactiveProperty<int> _totalDamage;
    IObservable<Unit> _onBroken;

    public IObservable<Unit> OnRepaired => _repair;
    public IObservable<DamageUnit> OnAffected => _onAffected;
    public IObservable<int> TotalDamage => _totalDamage;
    public int totalDamage => _totalDamage.Value;
    public IObservable<Unit> OnBroken => _onBroken;

    public bool isBroken => 0 < _param.stamina && _param.stamina <= _totalDamage.Value;

    public void Affect(DamageUnit du) { _affect.OnNext(du); }
    public void Repair() { _repair.OnNext(Unit.Default); }

    public void Break()
    {
      _totalDamage.Value = _param.stamina;
    }
  }
}
