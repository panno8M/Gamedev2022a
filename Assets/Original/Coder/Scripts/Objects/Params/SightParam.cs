using UnityEngine;
using Assembly.GameSystem;

namespace Assembly.Params
{
  [CreateAssetMenu(fileName = "Sight", menuName = "Params/Sight")]
  public class SightParam : ScriptableObject
  {
    [Header("Behavior")]
    public float angle = 15.5f;
    public Layer obstacleLayer = Layer.Stage;
    public bool noticeImmediately;
    [Tooltip("NoticeImmediatelyがtrueの場合無視される")]
    public float secondsToNotice = 1;


    [Header("For Optimization")]
    public int frameSkips = 3;
  }
}
