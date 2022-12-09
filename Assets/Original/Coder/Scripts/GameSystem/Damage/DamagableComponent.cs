using System;
using UnityEngine;
using UniRx;

namespace Assembly.GameSystem.Damage
{
  [RequireComponent(typeof(Collider))]
  [RequireComponent(typeof(Rigidbody))]
  public class DamagableComponent : MonoBehaviour, IDamagable
  {
    Damagable _damagable;
    Damagable damagable => _damagable ?? (_damagable = new Damagable(param));

    [SerializeField] DamagableParam param;

    public IObservable<Unit> OnRepaired => damagable.OnRepaired;
    public IObservable<DamageUnit> OnAffected => damagable.OnAffected;
    public IObservable<int> TotalDamage => damagable.TotalDamage;
    public int totalDamage => damagable.totalDamage;
    public IObservable<Unit> OnBroken => damagable.OnBroken;
    public bool isBroken => damagable.isBroken;
    public int stamina => damagable.stamina;

    public void Affect(DamageUnit du)
    {
      if (isActiveAndEnabled) damagable.Affect(du);
    }
    public void Repair()
    {
      if (isActiveAndEnabled) damagable.Repair();
    }
    public void Break()
    {
      if (isActiveAndEnabled) damagable.Break();
    }

#if UNITY_EDITOR
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