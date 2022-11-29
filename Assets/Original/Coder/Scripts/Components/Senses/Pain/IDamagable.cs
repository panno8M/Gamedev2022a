using System;
using UniRx;

namespace Senses.Pain
{
  public interface IDamagable
  {
    void Affect(DamageUnit du);
    void Repair();
    void Break();

    IObservable<Unit> OnRepaired { get; }
    IObservable<DamageUnit> OnAffected { get; }
    IObservable<int> TotalDamage { get; }
    int totalDamage { get; }
    IObservable<Unit> OnBroken { get; }

    bool isBroken { get; }
    int stamina { get; }
  }
}