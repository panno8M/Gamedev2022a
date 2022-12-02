using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.Actors
{
  public abstract class ActorBehavior<Actor> : MonoBehaviour
    where Actor : MonoBehaviour
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
      OnInit();
    }
    void Reset()
    {
      SetActor();
      OnResetInEditor();
    }

    protected abstract void OnInit();
    protected virtual void OnResetInEditor(){}
  }
}