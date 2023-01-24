using UnityEngine;
using Assembly.GameSystem;

[RequireComponent(typeof(Collider))]
public class StageLoadVolume : MonoBehaviour
{
  [Zenject.Inject]
  public void Construct(StageTransitionMaster master)
  {
    this.master = master;
  }
  StageTransitionMaster master;


  void OnTriggerEnter(Collider other)
  {
    if (!other.CompareTag(Tag.Player.GetName())) { return; }
    master.Load();
  }
}