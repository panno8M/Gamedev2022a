using UnityEngine;
using Assembly.GameSystem;

namespace Assembly.Components.Actors
{
  public class ReactionModule : DiBehavior
  {
    [SerializeField] DroneReactionSet reactionSet;

    protected override void Blueprint()
    {
      reactionSet.Initialize();
    }
    public void Question()
    { reactionSet.kind = DroneReactionSet.Kind.Question; }
    public void Exclamation()
    { reactionSet.kind = DroneReactionSet.Kind.Exclamation; }
    public void GuruGuru()
    { reactionSet.kind = DroneReactionSet.Kind.GuruGuru; }
  }
}