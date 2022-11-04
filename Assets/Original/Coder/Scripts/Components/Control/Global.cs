using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global {
    public static InputControl Control => InputControl.Instance;
    public static Player Player => Player.Instance;
}

public abstract class UniqueBehaviour<T> : MonoBehaviour
    where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance => instance ?? (instance = (T)FindObjectOfType(typeof(T)));
}
