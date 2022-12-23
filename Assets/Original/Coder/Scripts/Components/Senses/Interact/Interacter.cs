using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Assembly.Components.StageGimmicks{
    public class Interacter : MonoBehaviour
    {
        public SafetyTrigger safetyTrigger;

        void Start()
        {
            safetyTrigger = GetComponent<SafetyTrigger>();
            Global.Control.Interact.Where(_ => JudgeInteracterOnTrigger()).Subscribe(_ => Interact());
        }

        public void Interact(){
            if(JudgeInteracterOnTrigger()){
                IInteractable[] II = safetyTrigger.triggers[0].gameObject.GetComponentsInChildren<IInteractable>();
                foreach (IInteractable i in II) {
                    i.OnInteract();
                }
            }
        }

        public bool JudgeInteracterOnTrigger(){
            bool onInteracter = false;
            if(safetyTrigger.triggers != null){
                for(int i =0; i < safetyTrigger.triggers.Count; i++){
                    if(safetyTrigger.triggers[i] == safetyTrigger.triggers[i].gameObject.GetComponent<Interactable>().safetyTrigger){
                        onInteracter = true;
                    };
                }
            }
            return onInteracter;
        }
    }
}