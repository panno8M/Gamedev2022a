using UnityEngine;
using Zenject;

namespace Assembly.Components.Installers
{
  public class FaderInstaller : MonoInstaller
  {
    [SerializeField] GameObject fader;
    public override void InstallBindings()
    {
      Container.Bind<UI.SimpleFader>()
        .FromComponentOn(fader)
        .AsSingle();
    }
  }
}