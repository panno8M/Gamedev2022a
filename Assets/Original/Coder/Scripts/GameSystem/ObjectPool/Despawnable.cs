using System;

namespace Assembly.GameSystem.ObjectPool
{
  public class Despawnable : IDespawnable
  {
    Action _Despawn;
    public Despawnable(Action despawn)
    {
      _Despawn = despawn;
    }
    public void Despawn() => _Despawn();
  }
}
