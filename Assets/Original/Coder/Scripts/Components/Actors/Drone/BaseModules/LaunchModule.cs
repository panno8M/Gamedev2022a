using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem;
using Assembly.GameSystem.PathNetwork;


namespace Assembly.Components.Actors
{
  public abstract class LaunchModule : DiBehavior
  {
    [SerializeField] protected DroneAct _actor;

    public abstract UniTask Launch();
    public abstract UniTask Collect();
  }
}