using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Ex.InteractionTraits.Core
{
  [RequireComponent(typeof(Interactor))]
  public abstract class InteractorModuleBase : MonoBehaviour
  {
    [SerializeField] Interactor _interactor;
    public Interactor interactor => _interactor;

    protected List<InteractableModuleBase> _interactables => _interactor.interactables;

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