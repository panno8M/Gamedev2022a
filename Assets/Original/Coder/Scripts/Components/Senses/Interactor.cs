using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Senses
{
  [RequireComponent(typeof(Collider))]
  public class Interactor : MonoBehaviour
  {
    // TODO: AiSight等と統合できないか？
    List<InteractableModuleBase> _interactables = new List<InteractableModuleBase>();
    public List<InteractableModuleBase> interactables => _interactables;

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