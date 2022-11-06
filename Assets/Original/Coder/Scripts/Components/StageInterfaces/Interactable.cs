using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour {
    public ReactiveProperty<Interactor> Interactor = new ReactiveProperty<Interactor>();
    public bool isActive = true;

    Subject<Interactor> _onInteracted = new Subject<Interactor>();
    public IObservable<Interactor> OnInteracted => _onInteracted;

    void Awake() {
        OnInteracted
            .Subscribe(interactor => interactor.React(this));
    }

    public void Interact(Interactor interactor) {
        if (isActive && interactor) {
            Interactor.Value = interactor;
            _onInteracted.OnNext(interactor);
        }
    }
}
