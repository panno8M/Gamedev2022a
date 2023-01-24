using UnityEngine;
using UnityEngine.SceneManagement;

public class StageTransitionMaster : MonoBehaviour
{
  [SerializeField] string nextStageName;
  [SerializeField] bool LoadOnAwake = true;
  AsyncOperation loadNext;

  void Start() { if (LoadOnAwake) { Load(); } }

  public void Load()
  {
    if (loadNext != null) { return; }
    loadNext = SceneManager.LoadSceneAsync(nextStageName);
    if (loadNext == null) { return; }
    loadNext.allowSceneActivation = false;
  }

  public void Transition()
  {
    if (loadNext == null) { Load(); }
    if (loadNext == null) { return; }
    loadNext.allowSceneActivation = true;
  }
}