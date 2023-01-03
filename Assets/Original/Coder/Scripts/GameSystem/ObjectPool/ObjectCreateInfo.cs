namespace Assembly.GameSystem.ObjectPool
{
  public abstract class ObjectCreateInfo<T> : IInfuser<T>
    where T : DiBehavior, IPoolCollectable
  {
    public TransformUsage transformUsage = new TransformUsage { };
    public TransformInfo transformInfo = new TransformInfo { };
    public virtual void Infuse(T instance)
    {
      transformInfo.Infuse(instance.transform, transformUsage);
    }
  }
}