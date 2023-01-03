namespace Assembly.GameSystem.ObjectPool
{
  public interface IInfuser<T>
  {
    void Infuse(T instance);
  }
}