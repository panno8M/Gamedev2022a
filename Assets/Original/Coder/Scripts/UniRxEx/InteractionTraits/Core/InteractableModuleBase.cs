using System;
using UnityEngine;

namespace UniRx.Ex.InteractionTraits.Core
{
  [RequireComponent(typeof(Interactable))]
  public abstract class InteractableModuleBase : MonoBehaviour
  {
    [SerializeField] Interactable _interactable;

    [SerializeField] ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(true);
    public bool isActive => _isActive.Value;

    public void Activate() { _isActive.Value = true; }
    public void Disactivate() { _isActive.Value = false; }

    protected IObservable<Unit> OnActivated => _isActive.Where(x => x).AsUnitObservable();
    protected IObservable<Unit> OnDisactivated => _isActive.Where(x => !x).AsUnitObservable();

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
