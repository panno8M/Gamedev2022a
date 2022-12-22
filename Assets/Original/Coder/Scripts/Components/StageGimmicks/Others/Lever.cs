using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Assembly.GameSystem.Message;
using UniRx;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.StageGimmicks{
[RequireComponent(typeof(SafetyTrigger))]
    public class Lever : MonoBehaviour
    {   
        SafetyTrigger safetyTrigger;
        public BoolReactiveProperty _targetSwitch;
        [SerializeField] MessageDispatcher _OnSwitch = new MessageDispatcher();


        // Start is called before the first frame update
        void Start()
        {
            safetyTrigger = GetComponent<SafetyTrigger>();
            
            _targetSwitch.Where(_ => _targetSwitch.Value == true).Subscribe(_ => _OnSwitch.message.intensity.SetFactor1());
            _targetSwitch.Where(_ => _targetSwitch.Value == false).Subscribe(_ => _OnSwitch.message.intensity.SetFactor0());
            Global.Control.InteractInput.Buffer(2).Subscribe(_ => SwitchOn_Off());
            
        }

        // Update is called once per frame
        void Update()
        {
            _OnSwitch.Dispatch();
        }

        private void SwitchOn_Off(){
            if(JudgeTriggerEnter()){
                if(_targetSwitch.Value){
                    _targetSwitch.Value = false;
                }else{
                    _targetSwitch.Value = true;
                }
            }
        }

        private bool JudgeTriggerEnter(){
            bool onPlayer = false;
            if(safetyTrigger.triggers != null){
                for(int i =0; i < safetyTrigger.triggers.Count; i++){
                    if(safetyTrigger.triggers[i].gameObject.GetComponent<PlayerAct>() != null){
                        onPlayer = true;
                    };
                }
            }
            return onPlayer;
        }
    }
}