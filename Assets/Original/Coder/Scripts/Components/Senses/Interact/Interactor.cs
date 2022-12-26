using UnityEngine;

namespace Assembly.Components
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Interactor : MonoBehaviour
  {
    [SerializeField] SafetyTrigger safetyTrigger;

    public void Interact()
    {
      if (FindInteractableInAround(out Interactable interactable))
      { interactable.Attempt(); }
    }

    bool FindInteractableInAround(out Interactable result)
    {
      for (int i = 0; i < safetyTrigger.triggers.Count; i++)
      {
        SafetyTrigger trigger = safetyTrigger.triggers[i];
        if (result = trigger?.GetComponent<Interactable>())
        { return true; }
      }
      result = null;
      return false;
    }
  }
}