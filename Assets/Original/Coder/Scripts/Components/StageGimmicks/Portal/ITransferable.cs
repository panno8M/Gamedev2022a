using Cysharp.Threading.Tasks;

namespace Assembly.Components.StageGimmicks
{
  public interface ITransferable
  {
    Portal closestPortal {set;}

    bool Handshake(Portal portal);

    UniTask StartTransfer(Portal portal);
    UniTask ProcessTransfer(Portal portal);
    UniTask CompleteTransfer(Portal portal);
  }
}