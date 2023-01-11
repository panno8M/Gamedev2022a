using UnityEngine;

namespace Assembly.Params
{
  [CreateAssetMenu(fileName = "Blink", menuName = "Params/Blink")]
  public class BlinkParam : ScriptableObject
  {
    public bool useBlink;
    public Range secondsToWait = new Range
    { min = Mathf.Infinity, max = Mathf.Infinity };
    public Range secondsToBlink;

    [System.Serializable]
    public struct Range
    {
      public float min;
      public float max;

      public float PickRandomSeconds() => UnityEngine.Random.Range(min, max);
      public int PickRandomMilliSeconds() => (int)(PickRandomSeconds() * 1000);
    }
  }
}
