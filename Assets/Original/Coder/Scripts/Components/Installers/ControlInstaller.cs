using UnityEngine;
using Zenject;
using Assembly.GameSystem.Input;

namespace Assembly.Components.Installers
{
  public class ControlInstaller : MonoInstaller
  {
    [SerializeField] InputControl inputControl;
    public override void InstallBindings()
    {
      Container.Bind<InputControl>()
        .FromComponentInNewPrefab(inputControl)
        .AsSingle();
    }
  }
}