using System;
using UnityEngine;
using UniRx;

using UniRx.Ex.DamageTraits;

namespace Assembly.Components.Senses
{
  [RequireComponent(typeof(Collider))]
  [RequireComponent(typeof(Rigidbody))]
  public class DamagableWrapper : MonoBehaviour
  {
    Damagable _damagable;
    Damagable damagable => _damagable ?? (_damagable = new Damagable(param));

    [SerializeField] DamagableParams param;

    public IObservable<Unit> OnRepaired => damagable.OnRepaired;
    public IObservable<DamageUnit> OnAffected => damagable.OnAffected;
    public ReadOnlyReactiveProperty<int> TotalDamage => damagable.TotalDamage;
    public IObservable<Unit> OnBroken => damagable.OnBroken;

    public void Affect(DamageUnit du) { damagable.Affect(du); }
    public void Repair() { damagable.Repair(); }

#if DEBUG
    [Serializable]
    struct Inspector
    {
      public bool IsBroken;
      public int TotalDamageAllowed;
      public int TotalDamageFire;
      public int TotalDamageWater;
      public int TotalDamageExplosion;
    }
    [SerializeField] Inspector inspector;
    void Awake()
    {
      Inspect();
    }

    void Inspect()
    {
      OnBroken
          .Subscribe(x => inspector.IsBroken = true);
      TotalDamage
          .Subscribe(x => inspector.TotalDamageAllowed = x);
      OnAffected
          .Where(dmg => dmg.kind.HasFlag(DamageKind.Fire))
          .Select(dmg => dmg.scale)
          .Scan((o, n) => o + n)
          .Subscribe(x => inspector.TotalDamageFire = x);
      OnAffected
          .Where(dmg => dmg.kind.HasFlag(DamageKind.Water))
          .Select(dmg => dmg.scale)
          .Scan((o, n) => o + n)
          .Subscribe(x => inspector.TotalDamageWater = x);
      OnAffected
          .Where(dmg => dmg.kind.HasFlag(DamageKind.Explosion))
          .Select(dmg => dmg.scale)
          .Scan((o, n) => o + n)
          .Subscribe(x => inspector.TotalDamageExplosion = x);

      OnRepaired.Subscribe(_ => inspector = new Inspector());
    }
#endif
  }
}