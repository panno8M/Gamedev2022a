using UnityEngine;

namespace Assembly.Components
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Interactor : MonoBehaviour
  {
    SafetyTrigger _trigger;

    public void Awake()
    {
      _trigger = GetComponent<SafetyTrigger>();
    }

    public void Interact()
    {
      if (FindInteractableInAround(out Interactable interactable))
      { interactable.Attempt(); }
    }

    bool FindInteractableInAround(out Interactable result)
    {
      for (int i = 0; i < _trigger.others.Count; i++)
      {
        SafetyTrigger trigger = _trigger.others[i];
        if (result = trigger?.GetComponent<Interactable>())
        { return true; }
      }
      result = null;
      return false;
    }
  }
}