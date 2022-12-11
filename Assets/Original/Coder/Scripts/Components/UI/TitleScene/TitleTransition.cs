using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

namespace Assembly.Components.Actors.Player.Pure{

    public class TitleTransition : MonoBehaviour
    {   
        
        StageTransition stageTransition =  new StageTransition();
        AsyncOperation nextScene;

        public StageTransitionMgr stageTransitionMgr;

        void Start()
        {
            if(!stageTransition.CheckAlreadySceneLoaded(stageTransitionMgr.nextStageName)){
                nextScene = stageTransition.Load(stageTransitionMgr.nextStageName);
            }

            Global.Control.Interact.Subscribe(_ => stageTransition.Transition(nextScene));
        }
    }

}
