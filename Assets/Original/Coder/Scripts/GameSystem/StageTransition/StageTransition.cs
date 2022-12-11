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

    public bool CheckAlreadySceneLoaded(string sceneName){

        bool alreadyLoaded = false;

        for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++){
            string loadedSceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name;
            if(loadedSceneName == sceneName){
                alreadyLoaded = true;
            }
        }

        return alreadyLoaded;
    }
}
