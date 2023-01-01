namespace Assembly.GameSystem.ObjectPool
{
  public interface IPoolCollectable
  {
    void Assemble();
    void Disassemble();

    IDespawnable despawnable { set; }
  }
}