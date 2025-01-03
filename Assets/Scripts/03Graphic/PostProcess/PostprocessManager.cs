using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    /// <summary>
    /// 后处理管理器
    /// </summary>
    public class PostprocessManager
    {
        private static Dictionary<Camera, PostprocessLayer> m_PostprocessLayers;

        public static void RegisterLayer(Camera _camera, PostprocessLayer _layer)
        {
            if(m_PostprocessLayers == null)
            {
                m_PostprocessLayers = new Dictionary<Camera, PostprocessLayer>();
            }

            PostprocessLayer oldLayer;
            if (m_PostprocessLayers.TryGetValue(_camera, out oldLayer))
            {
                if (oldLayer == _layer)
                    return;

                m_PostprocessLayers[_camera] = _layer;
            }
            else
            {
                m_PostprocessLayers.Add(_camera, _layer);
            }

            UpdateLayerSetting(_layer, (GraphicLevel)GeGraphicSetting.instance.GetGraphicLevel());
        }


        public static void UnRegisterLayer(Camera _camera)
        {
            if (m_PostprocessLayers != null)
            {
                m_PostprocessLayers.Remove(_camera);
            }
        }

        public static void IncreaseBloomRef()
        {
            var layer = m_PostprocessLayers.GetEnumerator();
            while (layer.MoveNext())
            {
                if (layer.Current.Value != null)
                    layer.Current.Value.IncreaseBloomRef();
            }
        }

        public static void DecreaseBloomRef()
        {
            var layer = m_PostprocessLayers.GetEnumerator();
            while (layer.MoveNext())
            {
                if (layer.Current.Value != null)
                    layer.Current.Value.DecreaseBloomRef();
            }
        }

        public static void IncreaseDistortionRef()
        {
            var layer = m_PostprocessLayers.GetEnumerator();
            while (layer.MoveNext())
            {
                if (layer.Current.Value != null)
                    layer.Current.Value.IncreaseDistortionRef();
            }
        }

        public static void DecreaseDistortionRef()
        {
            var layer = m_PostprocessLayers.GetEnumerator();
            while (layer.MoveNext())
            {
                if (layer.Current.Value != null)
                    layer.Current.Value.DecreaseDistortionRef();
            }
        }

        public static void SetProcessQuality(GraphicLevel postProcessQuality)
        {
            bool bHasInvalidCamera = false;
            if (m_PostprocessLayers != null)
            {
                var layer = m_PostprocessLayers.GetEnumerator();
                while (layer.MoveNext())
                {
                    if (layer.Current.Value != null)
                    {
                        UpdateLayerSetting(layer.Current.Value, postProcessQuality);
                    }
                    else
                    {
                        bHasInvalidCamera = true;
                    }
                }
            }
        }

        private static void UpdateLayerSetting(PostprocessLayer _layer, GraphicLevel postProcessQuality)
        {
            _layer.OnQualityChanged(postProcessQuality);
        }
    }
}
