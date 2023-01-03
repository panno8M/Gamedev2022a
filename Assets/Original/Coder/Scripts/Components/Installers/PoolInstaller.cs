using UnityEngine;
using Zenject;
using Assembly.Components.Pools;

namespace Assembly.Components.Installers
{
  public class PoolInstaller : MonoInstaller
  {
    [SerializeField] WaterBallPool waterBallPool;
    [SerializeField] BombPool bombPool;
    [SerializeField] HostileDronePool hostileDronePool;
    [SerializeField] ObserveDronePool observeDronePool;
    [SerializeField] ParticleExplosionPool particleExplosionPool;
    [SerializeField] ParticleImpactSplashPool particleImpactSplashPool;
    public override void InstallBindings()
    {
      Container.Bind<WaterBallPool>()
        .FromComponentInNewPrefab(waterBallPool)
        .AsSingle();

      Container.Bind<BombPool>()
        .FromComponentInNewPrefab(bombPool)
        .AsSingle();

      Container.Bind<HostileDronePool>()
        .FromComponentInNewPrefab(hostileDronePool)
        .AsSingle();

      Container.Bind<ObserveDronePool>()
        .FromComponentInNewPrefab(observeDronePool)
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