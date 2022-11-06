using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Interactable : MonoBehaviour {
    Subject<Interactor> _onInteracted = new Subject<Interactor>();
    public IObservable<Interactor> OnInteracted => _onInteracted;

    public void Interact(Interactor interactor) {
        if (interactor) {
            _onInteracted.OnNext(interactor);
        }
    }

    void Awake() {
        OnInteracted.Subscribe(interactor => Debug.Log("INTERACTED BY " + interactor.gameObject.name));
    }
}
