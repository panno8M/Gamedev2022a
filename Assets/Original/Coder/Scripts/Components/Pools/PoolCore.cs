using Assembly.GameSystem;

namespace Assembly.Components.Pools
{
  public class PoolCore : UniqueBehaviour<PoolCore>
  {
    PlayerPool _player;
    public PlayerPool player => _player ?? (_player = GetComponent<PlayerPool>());

    BombPool _bomb;
    public BombPool bomb => _bomb ?? (_bomb = GetComponent<BombPool>());

    WaterBallPool _waterBall;
    public WaterBallPool waterBall => _waterBall ?? (_waterBall = GetComponent<WaterBallPool>());

    HostileDronePool _hostileDrone;
    public HostileDronePool hostileDrone => _hostileDrone ?? (_hostileDrone = GetComponent<HostileDronePool>());

    ParticleImpactSplashPool _psImpactSplash;
    public ParticleImpactSplashPool psImpactSplash => _psImpactSplash ?? (_psImpactSplash = GetComponent<ParticleImpactSplashPool>());

    ParticleExplosionPool _psExplosion;
    public ParticleExplosionPool psExplosion => _psExplosion ?? (_psExplosion = GetComponent<ParticleExplosionPool>());

    protected override void Blueprint()
    {
    }
  }
  public static class Pool
  {
    public static PlayerPool player => PoolCore.Instance.player;
    public static BombPool bomb => PoolCore.Instance.bomb;
    public static WaterBallPool waterBall => PoolCore.Instance.waterBall;
    public static HostileDronePool hostileDrone => PoolCore.Instance.hostileDrone;

    public static ParticleImpactSplashPool psImpactSplash => PoolCore.Instance.psImpactSplash;
    public static ParticleExplosionPool psExplosion => PoolCore.Instance.psExplosion;

  }
}