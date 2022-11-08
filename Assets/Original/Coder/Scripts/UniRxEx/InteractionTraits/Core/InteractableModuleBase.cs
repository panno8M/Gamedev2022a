using UnityEngine;

namespace UniRx.Ex.InteractionTraits.Core
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
