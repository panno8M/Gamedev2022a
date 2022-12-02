using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{
  public abstract class ActorBehavior<Actor> : MonoBehaviour
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
    void Start()
    {
      SetActor();
      _actor.OnRebuildObservable.Subscribe(_ => OnRebuild());
      OnInit();
    }
    void Reset()
    {
      SetActor();
      OnResetInEditor();
    }

    protected abstract void OnInit();
    protected virtual void OnResetInEditor(){}
    protected virtual void OnRebuild(){}
  }
}