namespace Assembly.GameSystem.ObjectPool
{
  public interface IObjectPool<T>
  {
    T Spawn(IInfuser<T> info);
    void Despawn(T instance);
  }
}