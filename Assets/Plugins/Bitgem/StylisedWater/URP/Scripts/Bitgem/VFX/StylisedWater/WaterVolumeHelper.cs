#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Bitgem.VFX.StylisedWater
{
    public class WaterVolumeHelper : MonoBehaviour
    {
        #region Shader IDs

        static int idWaveFrequency = Shader.PropertyToID("_WaveFrequency");
        static int idWaveScale = Shader.PropertyToID("_WaveScale");
        static int idWaveSpeed = Shader.PropertyToID("_WaveSpeed");

        #endregion

        #region Private static fields

        private static WaterVolumeHelper instance;

        #endregion

        #region Public fields

        public WaterVolumeBase WaterVolume;

        #endregion

        #region Public static properties

        public static WaterVolumeHelper Instance => instance;

        #endregion

        #region Public methods

        public bool GetHeight(Vector3 _position, out float height)
        {
            height = 0;

            // ensure a water volume
            if (!WaterVolume)
            {
                return false;
            }

            // ensure a material
            var renderer = WaterVolume.gameObject.GetComponent<MeshRenderer>();
            if (!renderer || !renderer.sharedMaterial)
            {
                return false;
            }

            // replicate the shader logic, using parameters pulled from the specific material, to return the height at the specified position
            if (WaterVolume.GetHeight(_position, out height))
            {
                var _WaveFrequency = renderer.sharedMaterial.GetFloat(idWaveFrequency);
                var _WaveScale = renderer.sharedMaterial.GetFloat(idWaveScale);
                var _WaveSpeed = renderer.sharedMaterial.GetFloat(idWaveSpeed);
                var time = Time.time * _WaveSpeed;
                var shaderOffset = (Mathf.Sin(_position.x * _WaveFrequency + time) + Mathf.Cos(_position.z * _WaveFrequency + time)) * _WaveScale;
                height += shaderOffset;
                return true;
            }

            return false;
        }

        #endregion

        #region MonoBehaviour events

        void Awake()
        {
            instance = this;
        }
        void OnDestroy()
        {
            instance = null;
        }

        #endregion
    }
}