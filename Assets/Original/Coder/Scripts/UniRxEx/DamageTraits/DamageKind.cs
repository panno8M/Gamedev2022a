using System;

namespace UniRx.Ex.DamageTraits {
    [Flags]
    public enum DamageKind {
        None      = 0,
        Fire      = 1 << 0,
        Water     = 1 << 1,
        Explosion = 1 << 2,
        All = Fire|Water|Explosion
    }
}
