using UnityEngine;
using Assembly.GameSystem;

namespace Assembly.Params
{
  [CreateAssetMenu(fileName = "Sight", menuName = "Params/Sight")]
  public class SightParam : ScriptableObject
  {
    public float angle = 18;
    public Layer obstacleLayer = Layer.Stage;

  }
}
