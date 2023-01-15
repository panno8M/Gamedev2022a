using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
namespace Assembly.Components.Pools
{
  public class SignalLinePool : GameObjectPool<SignalLineParticle>
  {
    protected override SignalLineParticle CreateInstance()
    {
      return prefab.Instantiate<SignalLineParticle>();
    }
  }
}