using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Holdable : MonoBehaviour {
    [SerializeField] Interactable _interactable;
    Rigidbody rb;
    void Awake() {
        rb = GetComponent<Rigidbody>();
        _interactable.OnInteracted
            .Where(interactor => interactor.gameObject.CompareTag("Player"))
            .Subscribe(interactor => interactor.Hold(rb));
    }
}
