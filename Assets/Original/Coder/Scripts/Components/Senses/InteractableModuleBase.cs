using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.Senses
{
  [RequireComponent(typeof(Interactable))]
  public abstract class InteractableModuleBase: MonoBehaviour
  {
    [SerializeField] Interactable _interactable;
    void Reset()
    {
      SetDefaultComponent();
    }

    void SetDefaultComponent()
    {
      _interactable = GetComponent<Interactable>();
    }
  }
}
