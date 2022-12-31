using UnityEngine;
using Zenject;
using Assembly.Components.Pools;

namespace Assembly.Components.Installers
{
  public class PoolInstaller : MonoInstaller
  {
    [SerializeField] PlayerPool playerPool;
    [SerializeField] WaterBallPool waterBallPool;
    [SerializeField] BombPool bombPool;
    [SerializeField] HostileDronePool hostileDronePool;
    [SerializeField] ParticleExplosionPool particleExplosionPool;
    [SerializeField] ParticleImpactSplashPool particleImpactSplashPool;
    public override void InstallBindings()
    {
      Container.Bind<PlayerPool>()
        .FromComponentInNewPrefab(playerPool)
        .AsSingle();

      Container.Bind<WaterBallPool>()
        .FromComponentInNewPrefab(waterBallPool)
        .AsSingle();

      Container.Bind<BombPool>()
        .FromComponentInNewPrefab(bombPool)
        .AsSingle();

      Container.Bind<HostileDronePool>()
        .FromComponentInNewPrefab(hostileDronePool)
        .AsSingle();

      Container.Bind<ParticleExplosionPool>()
        .FromComponentInNewPrefab(particleExplosionPool)
        .AsSingle();

      Container.Bind<ParticleImpactSplashPool>()
        .FromComponentInNewPrefab(particleImpactSplashPool)
        .AsSingle();
    }
  }
}