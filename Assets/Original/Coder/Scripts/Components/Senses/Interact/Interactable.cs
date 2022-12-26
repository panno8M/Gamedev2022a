using System.Collections;
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.StageGimmicks{

    public class Interactable : MonoBehaviour
    {
        public SafetyTrigger safetyTrigger;
        Subject<Interactable> _OnInteractRecieve = new Subject<Interactable>();
        IObservable<Interactable> OnInteractRecieve => _OnInteractRecieve;
        public Interactable a;

        void Start()
        {
            safetyTrigger = GetComponent<SafetyTrigger>();
        }


    }
}

