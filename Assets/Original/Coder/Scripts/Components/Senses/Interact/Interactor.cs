using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Assembly.Components
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Interactor : MonoBehaviour
  {
    SafetyTrigger _trigger;
    [SerializeField] List<Interactable> _accessibles = new List<Interactable>();

    public void Awake()
    {
      _trigger = GetComponent<SafetyTrigger>();
      _trigger.OnEnter.Subscribe(AddToAccessibles);
      _trigger.OnExit.Subscribe(RemoveFromAccessibles);

      void AddToAccessibles(SafetyTrigger trigger)
      {
        Interactable interactable = trigger.GetComponent<Interactable>();
        if (interactable) { _accessibles.Add(interactable); }
      }
      void RemoveFromAccessibles(SafetyTrigger trigger)
      {
        Interactable interactable = trigger.GetComponent<Interactable>();
        if (interactable) { _accessibles.Remove(interactable); }
      }
    }

    public void Interact()
    {
      if (FindInteractableInAround(out Interactable interactable))
      { interactable.Attempt(); }
    }

    public bool FindInteractableInAround(out Interactable result)
    {
      result = _accessibles.Count == 0 ? null : _accessibles[0];
      return _accessibles.Count != 0;
    }
  }
}