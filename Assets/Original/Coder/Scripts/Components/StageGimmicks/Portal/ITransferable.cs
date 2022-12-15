using System.Threading;
using Cysharp.Threading.Tasks;

namespace Assembly.Components.StageGimmicks
{
  public interface ITransferable
  {
    Portal closestPortal {get; set;}

    bool Handshake(Portal portal);

    UniTask StartTransfer(Portal portal, CancellationToken token);
    UniTask ProcessTransfer(Portal portal, CancellationToken token);
    UniTask CompleteTransfer(Portal portal, CancellationToken token);
  }
}