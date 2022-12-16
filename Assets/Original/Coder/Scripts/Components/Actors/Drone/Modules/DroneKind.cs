using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.Components.StageGimmicks;

public class DroneKind : MonoBehaviour
{
    public enum DRONEKIND{
        WaterEmitter,
        Observe
    }

    public DRONEKIND _droneType;
}

