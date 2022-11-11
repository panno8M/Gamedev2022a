using UnityEngine;
using Assembly.Components.Input;
using Assembly.Components.Actors;
using Assembly.Components.Actors.Pool;

namespace Assembly.Components
{
  public static class Global
  {
    public static InputControl Control => InputControl.Instance;
    public static Player Player => Player.Instance;
    public static PlayerRespawnMgr PlayerRespawn => PlayerRespawnMgr.Instance;

    public static Camctl Cameraman => Camctl.Instance;
  }
  public abstract class UniqueBehaviour<T> : MonoBehaviour
      where T : MonoBehaviour
  {
    protected static T instance;
    public static T Instance => instance ?? (instance = (T)FindObjectOfType(typeof(T)));
  }

}