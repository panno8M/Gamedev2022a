using UnityEngine;
using Assembly.GameSystem.Message;
using UniRx;


namespace Assembly.Components.StageGimmicks{
[RequireComponent(typeof(SafetyTrigger))]
    public class Lever : MonoBehaviour,IInteractable
    {   
        public BoolReactiveProperty _targetSwitch;
        [SerializeField] MessageDispatcher _OnSwitch = new MessageDispatcher();
        Interactable interactable;

        void Start()
        {
            interactable = GetComponent<Interactable>();
            
            _targetSwitch.Where(_ => _targetSwitch.Value == true).Subscribe(_ => _OnSwitch.message.intensity.SetFactor1());
            _targetSwitch.Where(_ => _targetSwitch.Value == false).Subscribe(_ => _OnSwitch.message.intensity.SetFactor0());
            
        }

        private void SwitchOn_Off(){

            if(_targetSwitch.Value){
                _targetSwitch.Value = false;
            }else{
                _targetSwitch.Value = true;
            }

            _OnSwitch.Dispatch();
        
        }
        
        public void OnInteract(){
            SwitchOn_Off();
        }

    }
}