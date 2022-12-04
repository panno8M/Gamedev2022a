using UnityEngine;
using Assembly.Components.Input;
using Assembly.Components.Actors;

namespace Assembly.Components
{
  public static class Global
  {
    public static InputControl Control => InputControl.Instance;
    public static PlayerAct Player => PlayerPool.player;
    public static PlayerPool PlayerPool => PlayerPool.Instance as PlayerPool;
    public static GameTime GameTime => GameTime.Instance;

    public static Camctl Cameraman => Camctl.Instance;
  }
  public abstract class UniqueBehaviour<T> : DiBehavior
      where T : DiBehavior
  {
    protected static T instance;
    public static T Instance => instance ?? (instance = (T)FindObjectOfType(typeof(T)));
    void OnDestroy()
    {
      if (instance) instance = null;
    }
  }

}