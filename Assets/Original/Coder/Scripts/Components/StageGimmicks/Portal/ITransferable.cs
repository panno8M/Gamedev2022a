namespace Assembly.Components.StageGimmicks
{
  public interface ITransferable
  {
    void OnPortalEnter(Portal portal);
    void OnPortalExit(Portal portal);
    void Transfer(Portal portal);
    void OnCompleteTransfer(Portal portal);
  }
}