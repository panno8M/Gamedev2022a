using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    [SerializeField] Transform target;
    void FixedUpdate()
    {
        transform.position = target.position;
    }
}
