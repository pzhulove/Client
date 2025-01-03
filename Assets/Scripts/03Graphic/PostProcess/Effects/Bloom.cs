using Tenmove.Runtime.Client;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tenmove.Runtime.Unity
{
    public class Bloom : PostprocessEffectSettings
    {
        [Tooltip("计算的BloomTex叠加到原图时的强度")]
        public float intensity = 0.8f;

        [Tooltip("提取明亮的部分的阈值(Gamma Space)")]
        public float threshold = 1.0f;

        [Range(0, 1), Tooltip("使提取的亮度有一个平滑的过度，值为0是平滑的，值为1是hard")]
        public float softThreshold = 0.5f;

        [Range(2, 10), Tooltip("模糊迭代次数")]
        public int iteration = 8;

        [Range(0.05f, 0.95f), Tooltip("光晕扩散的范围")]
        public float m_Scatter = 0.5f;

        [Tooltip("Bloom的颜色")]
        public Color color = Color.white;

        public override PostProcessType EffectType
        {
            get
            {
                return PostProcessType.Bloom;
            }
        }
    }

    public class BloomRenderer : PostprocessEffectRenderer<Bloom>
    {
        public override bool NeedSwapRT { get { return false; } }
        public override bool NeedFinalPass { get { return true; } }
        public override int BlitToLayerTimes { get { return 0; } }
        public override bool CanBlitToScreen { get { return true; } }

        private int reference = 0;

        protected Material m_Material;

        // 最大迭代次数
        const int k_MaxPyramidSize = 16;

        enum Pass
        {
            Prefilter,
            BlurH,
            BlurV,
            UpSample
        }
        struct Level
        {
            public int up;
            public int down;
        }
        Level[] m_Pyramid;

        protected override void OnInit()
        {
            m_Material = PostprocessUtilities.GetMaterial("Hidden/Postprocessing/Bloom");

            m_Pyramid = new Level[k_MaxPyramidSize];
            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                m_Pyramid[i] = new Level
                {
                    down = Shader.PropertyToID("Bloom_MipDown" + i),
                    up = Shader.PropertyToID("Bloom_MipUp" + i)
                };
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            if (m_Material != null)
            {
                PostprocessUtilities.DestroyObject(m_Material);
                m_Material = null;
            }
        }

        public override void Render(CommandBuffer cmd, bool renderToScreen = false)
        {
            cmd.BeginSample("Postprocess-Bloom");

            if (m_Settings == null)
                m_Settings = GetDefaultSettings();

            Shader.EnableKeyword("BLOOM");
            m_Material.SetFloat("_Scatter", m_Settings.m_Scatter);

            bool supportRGB111110 = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float);
            RenderTextureFormat rtFormat = supportRGB111110 ? RenderTextureFormat.RGB111110Float : RenderTextureFormat.Default;
            if (supportRGB111110)
                cmd.DisableShaderKeyword("_USE_RGBM");
            else
                cmd.EnableShaderKeyword("_USE_RGBM");


            // 第一次DownSample直接降到四分之一
            int width = Mathf.FloorToInt(layer.RenderTextureWidth / 4);
            int height = Mathf.FloorToInt(layer.RenderTextureHeight / 4);

            // Prefilter
            float lthresh = Mathf.GammaToLinearSpace(m_Settings.threshold);
            float knee = lthresh * m_Settings.softThreshold + 1e-5f;
            Vector4 threshold = new Vector4(lthresh, lthresh - knee, knee * 2f, 0.25f / knee);
            m_Material.SetVector("_Threshold", threshold);
            cmd.GetTemporaryRT(m_Pyramid[0].down, width, height, 0, FilterMode.Bilinear, rtFormat);
            // Pyramid[0]没有用到
            //cmd.GetTemporaryRT(m_Pyramid[0].up, width, height, 0, FilterMode.Bilinear, rtFormat);
            cmd.BlitFullscreenTriangle(layer.SourceRT, m_Pyramid[0].down, m_Material, (int)Pass.Prefilter, false);

            // DownSample
            // TODO: 好像多分配了一个纹理
            var lastDown = m_Pyramid[0].down;
            for (int i = 1;i < m_Settings.iteration;i++)
            {
                width = Mathf.Max(width / 2, 1);
                height = Mathf.Max(height / 2, 1);

                int mipDown = m_Pyramid[i].down;
                int mipUp = m_Pyramid[i].up;

                cmd.GetTemporaryRT(mipDown, width, height, 0, FilterMode.Bilinear, rtFormat);
                cmd.GetTemporaryRT(mipUp,   width, height, 0, FilterMode.Bilinear, rtFormat);

                // 两个Pass的高斯模糊，第一个Pass 2xdownsample + 采样9次， 第二个Pass利用GPU的双线性插值只需采样5次
                cmd.BlitFullscreenTriangle(lastDown, mipUp, m_Material, (int)Pass.BlurH, false);
                cmd.BlitFullscreenTriangle(mipUp, mipDown, m_Material, (int)Pass.BlurV, false);

                lastDown = mipDown;
            }

            // Upsample
            int lastUp = m_Pyramid[m_Settings.iteration - 1].down;
            for (int i = m_Settings.iteration - 2; i >= 1; i--)
            {
                int mipDown = m_Pyramid[i].down;
                int mipUp = m_Pyramid[i].up;
                cmd.SetGlobalTexture("_DownTex", mipDown);
                cmd.BlitFullscreenTriangle(lastUp, mipUp, m_Material, (int)Pass.UpSample, false);
                lastUp = mipUp;
            }

            // Setup PostprocessCombine
            cmd.SetGlobalTexture("_BloomTex", lastUp);
            layer.FinalPassMaterial.SetColor("_Bloom_Color", m_Settings.color);
            layer.FinalPassMaterial.SetVector("_Bloom_Settings", new Vector4(0, m_Settings.intensity, 0, 0));

            // 不释放lastUp，lastUp要在后处理的最后一个Pass中使用，后处理渲染完成后释放lastUp
            for (int i = 0; i < m_Settings.iteration; i++)
            {
                if (m_Pyramid[i].down != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].down);
                if (m_Pyramid[i].up != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].up);
            }

            layer.BloomBufferNameID = lastUp;

            cmd.EndSample("Postprocess-Bloom");
        }

        public override bool CheckQualityAndSupport()
        {
            if(GeGraphicSetting.instance.GetGraphicLevel() <= (int)GraphicLevel.MIDDLE
                && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                return true;
            else
                return false;
        }

        public override bool CanRendering()
        {
            return true;

            //if (reference > 0)
            //    return true;
            //else
            //    return false;
        }

        /*
        ---------------------------------------------
            引用计数
        ---------------------------------------------
        */
        public void IncreaseRef()
        {
            reference++;
        }
        public void DecreaseRef()
        {
            reference--;
            reference = Mathf.Max(0, reference);
        }
    }
}
