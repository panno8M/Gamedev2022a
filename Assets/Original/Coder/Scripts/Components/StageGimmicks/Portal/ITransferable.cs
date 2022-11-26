namespace Assembly.Components.StageGimmicks
{
  public interface ITransferable
  {
    void OnPortalEnter(Portal portal);
    void OnPortalExit(Portal portal);

    void StartTransfer(Portal portal);
    void ProcessTransfer(Portal portal);
    void CompleteTransfer(Portal portal);
  }
}