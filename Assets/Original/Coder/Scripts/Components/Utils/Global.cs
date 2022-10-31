using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static Control Control => Control.Instance;
    public static Player Player => Player.Instance;
}

public abstract class UniqueBehaviour<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance ?? (instance = (T)FindObjectOfType(typeof(T)));
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }
}
