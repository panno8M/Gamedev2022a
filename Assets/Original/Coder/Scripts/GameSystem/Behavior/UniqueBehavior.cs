namespace Assembly.GameSystem
{
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