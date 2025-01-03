using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime.Client;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tenmove.Runtime.Unity
{
    public class Distortion : PostprocessEffectRenderer
    {
        public override bool NeedSwapRT { get { return false; } }
        public override bool NeedFinalPass { get { return true; } }
        public override int BlitToLayerTimes { get { return 0; } }
        public override bool CanBlitToScreen { get { return true; } }

        private int reference;

        private Camera m_DistortCamera;
        private RenderTexture m_DistortRT;

        protected override void OnInit()
        {
            base.OnInit();

            layer.RenderTextureSizeChangedCallback += OnRendertextureSizeChanged;
            layer.OrthographicSizeChangedCallback += OnOrthographicSizeChanged;

            m_DistortRT = new RenderTexture(layer.RenderTextureWidth / 2, layer.RenderTextureHeight / 2, 0, RenderTextureFormat.Default);
            m_DistortRT.name = "DistortRT";

            GameObject distortCameraGO = new GameObject("PostDistortCamera");
            m_DistortCamera = distortCameraGO.AddComponent<Camera>();

            m_DistortCamera.CopyFrom(layer.PostprocessCamera);
            m_DistortCamera.depth = layer.PostprocessCamera.depth - 0.1f;
            //m_DistortCamera.cullingMask = 1 << (int)ENUM_OBJECTLAYER.PostDistort;
            m_DistortCamera.targetTexture = m_DistortRT;

            m_DistortCamera.transform.SetParent(layer.PostprocessCamera.transform);
            m_DistortCamera.transform.localPosition = Vector3.zero;
            m_DistortCamera.transform.localRotation = Quaternion.identity;
            m_DistortCamera.transform.localScale = Vector3.one;

            //场景编辑器下SceneCamera默认会画天空盒，会影响Mask图，生成Mask的相机要把这些设置一下
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                m_DistortCamera.clearFlags = CameraClearFlags.SolidColor;
                m_DistortCamera.backgroundColor = Color.black;
            }
#endif
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            layer.RenderTextureSizeChangedCallback -= OnRendertextureSizeChanged;
            layer.OrthographicSizeChangedCallback -= OnOrthographicSizeChanged;

            m_DistortCamera.targetTexture = null;
            PostprocessUtilities.DestroyObject(m_DistortCamera.gameObject);
            PostprocessUtilities.DestroyObject(m_DistortRT);
        }

        public override void OnPreRender()
        {
            if (m_DistortCamera != null)
                m_DistortCamera.enabled = true;
        }

        public override void OnPreNotRender()
        {
            if (m_DistortCamera != null)
                m_DistortCamera.enabled = false;
        }

        public override void Render(CommandBuffer cmd, bool renderToScreen = false)
        {
            Shader.EnableKeyword("_POST_DISTORTION");

            layer.FinalPassMaterial.SetTexture("_DistortionTex", m_DistortRT);
        }

        public override bool CheckQualityAndSupport()
        {
            if (GeGraphicSetting.instance.GetGraphicLevel() >= (int)GraphicLevel.NORMAL
                && SystemInfo.supportedRenderTargetCount > 2)
                return true;
            else
                return false;
            // return false;
        }

        public override bool CanRendering()
        {
            if (reference > 0)
                return true;
            else
                return false;
        }

        public void IncreaseRef()
        {
            reference++;
        }
        public void DecreaseRef()
        {
            reference--;
            reference = Mathf.Max(0, reference);
        }

        private void OnRendertextureSizeChanged(int rtWidth, int rtHeight)
        {
            if (m_DistortCamera != null && m_DistortRT != null)
            {
                m_DistortCamera.targetTexture = null;
                PostprocessUtilities.DestroyObject(m_DistortRT);

                m_DistortRT = new RenderTexture(rtWidth / 2, rtHeight / 2, 0, RenderTextureFormat.Default);
                layer.FinalPassMaterial.SetTexture("_DisortMaskTex", m_DistortRT);
                m_DistortCamera.targetTexture = m_DistortRT;
            }
        }

        private void OnOrthographicSizeChanged(float size)
        {
            if (m_DistortCamera != null)
                m_DistortCamera.orthographicSize = size;
        }
    }
}
