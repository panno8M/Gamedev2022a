using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Ex.InteractionTraits.Core
{
  [RequireComponent(typeof(Collider))]
  public class Interactable : MonoBehaviour
  {
    public ReactiveProperty<Interactor> Interactor = new ReactiveProperty<Interactor>();
    public bool isActive = true;

    Subject<Interactor> _onInteracted = new Subject<Interactor>();
    public IObservable<Interactor> OnInteracted => _onInteracted;

    [SerializeField] HoldableModule _holdable;
    public HoldableModule holdable => _holdable;

    public void Interact(Interactor interactor)
    {
      if (isActive && interactor)
      {
        Interactor.Value = interactor;
        _onInteracted.OnNext(interactor);
      }
    }
    void Reset()
    {
      SetDefaultComponent();
    }

    void SetDefaultComponent()
    {
      _holdable = GetComponent<HoldableModule>();
    }
  }
}