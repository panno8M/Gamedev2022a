using UnityEngine;
using Assembly.Components.Input;
using Assembly.Components.Actors;
using Assembly.Components.Actors.Player.Pure;

namespace Assembly.Components
{
  public static class Global
  {
    public static InputControl Control => InputControl.Instance;
    public static PlayerAct Player => PlayerAct.Instance;
    public static PlayerRespawnMgr PlayerRespawn => PlayerRespawnMgr.Instance;
    public static GameTime GameTime => GameTime.Instance;

    public static Camctl Cameraman => Camctl.Instance;
  }
  public abstract class UniqueBehaviour<T> : MonoBehaviour
      where T : MonoBehaviour
  {
    protected static T instance;
    public static T Instance => instance ?? (instance = (T)FindObjectOfType(typeof(T)));
  }

}