using UnityEngine;

namespace Assembly.Components
{
  public class Interactable : MonoBehaviour
  {
    IInteractable[] interactables;

    void Start()
    { interactables = GetComponents<IInteractable>(); }

    public void Attempt()
    {
      for (int i = 0; i < interactables.Length; i++)
      { interactables[i].OnInteract(); }
    }
  }
}

