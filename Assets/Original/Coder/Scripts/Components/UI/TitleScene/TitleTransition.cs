using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

namespace Assembly.Components.Actors.Player.Pure{

    public class TitleTransition : MonoBehaviour
    {   
        [SerializeField] string firstStageName;
        StageTransition stageTransition =  new StageTransition();
        AsyncOperation nextScene;

        void Start()
        {
            nextScene = stageTransition.Load(firstStageName);
            Global.Control.Interact.Subscribe(_ => stageTransition.Transition(nextScene));
        }
    }

}
