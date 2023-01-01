using UnityEngine;
using Zenject;

namespace Assembly.Components.Installers
{
  public class RollbackInstaller : MonoInstaller
  {
    [SerializeField] Rollback from;
    public override void InstallBindings()
    {
      Container.Bind<Rollback>()
        .FromComponentOn(from.gameObject)
        .AsTransient();
    }
  }
}