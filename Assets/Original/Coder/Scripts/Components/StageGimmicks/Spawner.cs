using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(Collider))]
  public class Spawner : MonoBehaviour
  {
    public GameObject prefSpawnee;
    Collider _collider;
    Collider _Collider => _collider ?? (_collider = GetComponent<Collider>());
    Color areaColor = new Color(1f, 0f, 0f, .3f);
    Color spawnerColor = Color.yellow;

    public List<GameObject> spawnees = new List<GameObject>();

    void OnDrawGizmos()
    {
      Gizmos.color = areaColor;
      if (_Collider as BoxCollider)
      {
        var col = _collider as BoxCollider;
        Gizmos.DrawCube(transform.position + col.center, col.size);
      }
      Gizmos.color = spawnerColor;
      Gizmos.DrawSphere(transform.position, 0.3f);
    }

    void Start()
    {
      Spawn();
    }

    public GameObject Spawn()
    {
      var res = Instantiate(prefSpawnee, transform.position, transform.rotation);
      spawnees.Add(res);
      return res;
    }
  }
}