using System;

namespace Senses.Pain
{
  [Flags]
  public enum DamageKind
  {
    None = 0,
    Fire = 1 << 0,
    Water = 1 << 1,
    Explosion = 1 << 2,
    Everything = Fire | Water | Explosion
  }

  public static class DamageKindExt
  {
    public static bool MatchAny(this DamageKind a, DamageKind b)
    {
      return (a & b) > 0;
    }
  }
}
