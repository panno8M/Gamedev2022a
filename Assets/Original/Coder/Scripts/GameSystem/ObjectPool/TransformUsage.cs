namespace Assembly.GameSystem.ObjectPool
{
  public enum eopSpawnSpace { Parent, Global }
  public enum eopReferenceUsage { Discard, Local, Global }

  public class TransformUsage
  {
    public eopSpawnSpace spawnSpace = eopSpawnSpace.Parent;
    public eopReferenceUsage referenceUsage = eopReferenceUsage.Discard;
  }
}