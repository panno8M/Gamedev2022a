using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.Message;
using Utilities;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem.Input;

namespace Assembly.Components.StageGimmicks{
    public class ClearElevator : MonoBehaviour, IMessageListener
    {
        bool isPowered;
        bool isClear;

        InputControl control;
        AudioSource audioSource;
        public AudioClip elevatorSE;
        [SerializeField] GameObject clearText;
        StageTransitionMaster stageTransitionMaster;

        Actors.Player.PlayerAct playerAct;
        UI.SimpleFader fader;
        [Zenject.Inject]
        public void DepsInject(UI.SimpleFader fader, InputControl control, Actors.Player.PlayerAct playerAct, StageTransitionMaster stageTransitionMaster)
        {
            this.fader = fader;
            this.control = control;
            this.playerAct = playerAct;
            this.stageTransitionMaster = stageTransitionMaster;
        }

        public void Powered(MixFactor powerGain)
        {
            if(powerGain.PeekFactor() == 0){
                isPowered = false;
            }else{
                if(powerGain.PeekFactor() == 1){
                   isPowered = true;
                }
            }
        }

        public void ReceiveSignal(MixFactor message)
        {
            if(isPowered && message.PeekFactor() == 1){
                ClearSequence().Forget();
            }
        }

        void Start()
        {
            control.PauseInput.Where(x => isClear == true).Subscribe(_ =>
            {
                stageTransitionMaster.Transition();
            }).AddTo(this);

            audioSource = GetComponent<AudioSource>();
        }

        async UniTask ClearSequence()
        {   
            playerAct.ctl.ControlMethod.Value = Actors.Player.PlayerController.Methods.IgnoreAll;
            await UniTask.Delay(500);

            fader.Fade(.5f).Forget();

            await UniTask.Delay(1000);

            audioSource.PlayOneShot(elevatorSE);

            await UniTask.Delay(1500);

            clearText.SetActive(true);
            isClear = true;

        }
    }
}