using UnityEngine;
using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components.Actors
{
  public abstract class ActorBehavior<Actor> : DiBehavior
    where Actor : ActorCore<Actor>
  {
    [SerializeField] protected Actor _actor;

    protected virtual void SetActor()
    {
      if (!_actor)
      {
        _actor = GetComponent<Actor>()
                ?? transform.parent.GetComponent<Actor>();
      }
    }
    protected void Start()
    {
      SetActor();
      _actor.OnAssembleObservable.Subscribe(_ => OnAssemble());
      Blueprint();
    }
    protected void Reset()
    {
      SetActor();
      OnResetInEditor();
    }

    protected virtual void OnResetInEditor(){}
    protected virtual void OnAssemble(){}
  }
}