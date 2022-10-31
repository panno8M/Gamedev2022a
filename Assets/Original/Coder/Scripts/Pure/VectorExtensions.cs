using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3SwizzleExtension
{
    public static Vector3 _y_(this float v)   {
        return new Vector3(0,v,0);            }
    public static Vector3 _Y_(this float v)   {
        return new Vector3(0,-v,0);           }

    public static Vector3 x_y(this Vector2 v) {
        return new Vector3(v.x,0,v.y);        }

    public static Vector3 _y_(this Vector3 v) {
        return new Vector3(0,v.y,0);          }

    public static Vector3 Xyz(this Vector3 v) {
        return new Vector3(-v.x,v.y,v.z);     }
}
