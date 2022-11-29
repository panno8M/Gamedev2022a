using System;

namespace Senses.Pain
{
  [Serializable]
  public struct DamagableParam
  {
    public int stamina;
    public float secCoolDownToNextDamage;
    public DamageKind allowDamageSource;
    public bool damageDisabled;
    public TimeSpan coolDownToNextDamage => TimeSpan.FromSeconds(secCoolDownToNextDamage);
  }
}
