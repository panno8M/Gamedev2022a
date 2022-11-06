using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Holdable : MonoBehaviour {
    [SerializeField] Interactable _interactable;
    Rigidbody rb;
    void Awake() {
        rb = GetComponent<Rigidbody>();
        var playerInteract = _interactable.OnInteracted
            .Where(interactor => interactor.gameObject.CompareTag("Player"));

        playerInteract
            .Subscribe(interactor => {
                if (interactor.HoldingItem.Value == rb) {
                    interactor.Release();
                }
                else {
                    interactor.Hold(rb);
                }
            });
    }
}
