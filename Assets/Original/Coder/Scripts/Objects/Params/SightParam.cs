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

    [Tooltip("Allow Watching On Exit Sight Area\n一度ターゲットしたオブジェクトがコライダで指定したエリア外へ出た時もターゲットし続ける")]
    public bool allowWatchingOnExitSightArea;


    [Header("For Optimization")]
    public int frameSkips = 3;
  }
}
