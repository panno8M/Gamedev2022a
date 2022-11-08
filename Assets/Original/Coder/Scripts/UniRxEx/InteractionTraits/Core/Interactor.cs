using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;

namespace UniRx.Ex.InteractionTraits.Core
{
  [RequireComponent(typeof(Collider))]
  public class Interactor : MonoBehaviour
  {
    // TODO: AiSight等と統合できないか？
    List<InteractableModuleBase> _interactables = new List<InteractableModuleBase>();
    public List<InteractableModuleBase> interactables => _interactables;

    Subject<Unit> _OnForget = new Subject<Unit>();
    public IObservable<Unit> OnForget => _OnForget;

    public void Forget() {
      _OnForget.OnNext(Unit.Default);
      _interactables.Clear();
    }

    void Awake()
    {
      GetComponent<Collider>().isTrigger = true;
      this.OnTriggerEnterAsObservable()
          .Subscribe(other =>
          {
            _interactables.Add(other.GetComponent<InteractableModuleBase>());
          });
      this.OnTriggerExitAsObservable()
          .Subscribe(other =>
          {
            _interactables.Remove(other.GetComponent<InteractableModuleBase>());
          });

    }

  }
}