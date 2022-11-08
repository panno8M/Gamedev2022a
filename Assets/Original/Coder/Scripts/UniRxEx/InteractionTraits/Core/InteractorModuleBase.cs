using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Ex.InteractionTraits.Core
{
  [RequireComponent(typeof(Interactor))]
  public abstract class InteractorModuleBase : MonoBehaviour
  {
    [SerializeField] Interactor _interactor;
    public Interactor interactor => _interactor;

    protected List<Interactable> _interactables => _interactor.interactables;

    public abstract void Interact();

    void Reset()
    {
      SetDefaultComponent();
    }

    void SetDefaultComponent()
    {
      _interactor = GetComponent<Interactor>();
    }
  }
}