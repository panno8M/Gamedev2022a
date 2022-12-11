using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitToStageTransition : MonoBehaviour
{
    public enum REACTION_TYPE{
        Load,
        Transition
    }

    public REACTION_TYPE reaction_Type;
    public StageTransitionMgr stageTransitionMgr;
    StageTransition stageTransition = new StageTransition();

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            Debug.Log("HIT");
            switch(reaction_Type){
                case REACTION_TYPE.Load :
                    if(!stageTransition.CheckAlreadySceneLoaded(stageTransitionMgr.nextStageName)){
                        stageTransitionMgr.nextScene = stageTransition.Load(stageTransitionMgr.nextStageName);
                    }
                break;

                case REACTION_TYPE.Transition :
                    stageTransition.Transition(stageTransitionMgr.nextScene);
                break;
            }
        }
    }
}
