using System;

namespace Senses.Pain
{
  [Serializable]
  public struct DamageUnit
  {
    public DamageKind kind;
    public int scale;
    public DamageUnit(DamageKind kind, int scale)
    {
      this.kind = kind;
      this.scale = scale;
    }
    public DamageUnit(DamageKind kind)
    {
      this.kind = kind;
      this.scale = 1;
    }

    public static DamageUnit None = new DamageUnit(DamageKind.None);
    public static DamageUnit Fire = new DamageUnit(DamageKind.Fire);
    public static DamageUnit Water = new DamageUnit(DamageKind.Water);
    public static DamageUnit Explosion = new DamageUnit(DamageKind.Explosion);
  }
}
