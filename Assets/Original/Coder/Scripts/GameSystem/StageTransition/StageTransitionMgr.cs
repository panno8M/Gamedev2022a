using UnityEngine;

public class StageTransitionMgr : MonoBehaviour
{
    StageTransition stageTransition = new StageTransition();
    public string nextStageName;
    public AsyncOperation nextScene;
    [SerializeField] bool doStartToLoadNextStage;

    void Start()
    {
        if(!stageTransition.CheckAlreadySceneLoaded(nextStageName) && doStartToLoadNextStage){
            nextScene = stageTransition.Load(nextStageName);
        }
    }

}
