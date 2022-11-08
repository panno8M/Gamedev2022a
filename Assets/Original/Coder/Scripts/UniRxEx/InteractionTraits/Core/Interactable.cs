using System;
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

    public void Interact(Interactor interactor)
    {
      if (isActive && interactor)
      {
        Interactor.Value = interactor;
        _onInteracted.OnNext(interactor);
      }
    }
  }
}