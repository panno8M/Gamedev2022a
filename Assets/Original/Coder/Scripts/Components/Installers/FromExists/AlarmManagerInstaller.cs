using UnityEngine;
using Zenject;
using Assembly.Components.StageGimmicks;

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