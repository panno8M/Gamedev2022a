#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Bitgem.VFX.StylisedWater
{
    public class WaterVolumeFloater : MonoBehaviour
    {
        #region Public fields

        public WaterVolumeHelper WaterVolumeHelper = null;

        #endregion

        #region MonoBehaviour events

        float __height;
        WaterVolumeHelper __instance;

        void Update()
        {
            if (!__instance)
            {
                __instance = WaterVolumeHelper ? WaterVolumeHelper : WaterVolumeHelper.Instance;
                if (!__instance)
                {
                    return;
                }
            }

            if (__instance.GetHeight(transform.position, out __height))
            {
                transform.position = new Vector3(transform.position.x, __height, transform.position.z);
            }

        }

        #endregion
    }
}