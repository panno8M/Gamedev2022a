using UnityEngine;
using UnityEngine.SceneManagement;

public class StageTransition
{   
    public AsyncOperation Load(string nextSceneName){
        AsyncOperation nextScene = SceneManager.LoadSceneAsync(nextSceneName);
        nextScene.allowSceneActivation = false;

        return nextScene;
    }

    public void Transition(AsyncOperation nextScene){
        nextScene.allowSceneActivation = true;
    }
}
