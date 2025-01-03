using UnityEngine;
using UnityEngine.Rendering;
using Tenmove.Runtime.Client;

namespace Tenmove.Runtime.Unity
{
    public class RadialBlur : PostprocessEffectRenderer<RadialBlurSetting>
    {
        public Vector2 Center { get; set; }
        public float BlurFactor { get; set; }
        public float LerpFactor { get; set; }
        public float DisappearSpeed { get; set; }
        public override bool NeedSwapRT { get { return false; } }
        public override bool NeedFinalPass { get { return true; } }
        public override int BlitToLayerTimes { get { return 0; } }
        public override bool CanBlitToScreen { get { return true; } }
       

        private Material m_Material;
        private RenderTexture m_BlurTexture;

        protected override void OnInit()
        {
            layer.UpdateCallback += OnUpdate;
            layer.RenderTextureSizeChangedCallback += OnRenderTextureSizeChanged;

            m_Material = PostprocessUtilities.GetMaterial("Hidden/Postprocessing/RadialBlur");
            m_BlurTexture = new RenderTexture(layer.RenderTextureWidth / 2, layer.RenderTextureHeight / 2, 0, RenderTextureFormat.Default);
            m_BlurTexture.name = "RadialBlur";

            Center = new Vector2(0.5f, 0.5f);
        }

        //TODO 整合到FinalPass中
        public override void Render(CommandBuffer cmd, bool renderToScreen = false)
        {
            cmd.BeginSample("Postprocess-RadialBlur");

//             cmd.GetTemporaryRT(ShaderManager.RadialBlurID, layer.ScreenWidth / 2, layer.ScreenHeight / 2, 0, 
//                                 FilterMode.Bilinear, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            cmd.BlitFullscreenTriangle(layer.SourceRT, m_BlurTexture, m_Material, 0, true);

            layer.FinalPassMaterial.EnableKeyword("_RadialBlur");
            layer.FinalPassMaterial.SetTexture("_BlurTex", m_BlurTexture);
            layer.FinalPassMaterial.SetVector("_BlurCenter", Center);
            layer.FinalPassMaterial.SetFloat("_LerpFactor", LerpFactor);
            m_Material.SetVector("_BlurCenter", Center);
            m_Material.SetFloat("_BlurFactor", BlurFactor);


            //cmd.ReleaseTemporaryRT(ShaderManager.RadialBlurID);

            cmd.EndSample("Postprocess-RadialBlur");
        }

        private void OnUpdate(float deltaTime)
        {
            if (LerpFactor > 0)
            {
                LerpFactor -= deltaTime * DisappearSpeed;

                layer.FinalPassMaterial.SetVector("_BlurCenter", Center);
                layer.FinalPassMaterial.SetFloat("_LerpFactor", LerpFactor);
                if(m_Material != null)
                {
                    m_Material.SetVector("_BlurCenter", Center);
                    m_Material.SetFloat("_BlurFactor", BlurFactor);
                }
            }
            else
            {
                layer.DeactiveEffect(PostProcessType.RadialBlur);
                layer.UpdateCommandBuffer(false);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            layer.UpdateCallback -= OnUpdate;
            layer.RenderTextureSizeChangedCallback -= OnRenderTextureSizeChanged;

            PostprocessUtilities.DestroyObject(m_Material);
            if(m_BlurTexture != null)
            {
                m_BlurTexture.Release();
                PostprocessUtilities.DestroyObject(m_BlurTexture);
            }
        }

        public override void SetSettings(PostprocessEffectSettings _settings)
        {
            base.SetSettings(_settings);

            if (m_Settings == null)
                m_Settings = GetDefaultSettings();

            if(m_Settings != null)
            {
                BlurFactor = m_Settings.BlurFactor;
                LerpFactor = m_Settings.LerpFactor;
                DisappearSpeed = m_Settings.DisppearSpeed;
            }
        }

        private void OnRenderTextureSizeChanged(int rtWidth, int rtHeight)
        {
            if(m_BlurTexture != null)
            {
                PostprocessUtilities.DestroyObject(m_BlurTexture);
                m_BlurTexture = new RenderTexture(rtWidth / 2, rtHeight / 2, 0, RenderTextureFormat.Default);
                m_BlurTexture.name = "RadialBlur";
            }
        }

        public override bool CheckQualityAndSupport()
        {
            if (GeGraphicSetting.instance.GetGraphicLevel() >= (int)GraphicLevel.MIDDLE)
                return true;
            else
                return false;
            // return false;
        }
    }
}

