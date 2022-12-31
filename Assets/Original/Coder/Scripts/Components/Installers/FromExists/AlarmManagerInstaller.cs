using UnityEngine;
using Zenject;

namespace Assembly.Components.Installers
{
  public class AlarmManagerInstaller : MonoInstaller
  {
    [SerializeField] GameObject alarmManager;
    public override void InstallBindings()
    {
      Container.Bind<AlarmMgr>()
        .FromComponentOn(alarmManager)
        .AsTransient();
    }
  }
}