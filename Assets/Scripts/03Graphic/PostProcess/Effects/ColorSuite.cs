using Tenmove.Runtime.Client;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tenmove.Runtime.Unity
{
    public enum ColorSuiteMode
    {
        Default = 0,
        External
    }

    public enum DitherMode
    {
        Off,
        Ordered,
        Triangular
    }

    public class ColorSuite : PostprocessEffectSettings
    {
        public ColorSuiteMode mode = ColorSuiteMode.External;
        public Texture2D Lut;
        [Range(-1, 1)]
        public float _colorTemp = 0.0f;
        [Range(-1, 1)]
        public float _colorTint = 0.0f;
        public bool _toneMapping = false;
        public float _exposure = 1.0f;
        [Range(0, 2)]
        public float _saturation = 1.0f;
        public AnimationCurve _rCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve _gCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve _bCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve _cCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public DitherMode _ditherMode = DitherMode.Off;

        public override PostProcessType EffectType
        {
            get
            {
                return PostProcessType.ColrSuite;
            }
        }
    }

    public class ColorSuiteRenderer : PostprocessEffectRenderer<ColorSuite>
    {
        public override bool NeedSwapRT { get { return false; } }
        public override bool NeedFinalPass { get { return true; } }
        public override int BlitToLayerTimes { get { return 0; } }
        public override bool CanBlitToScreen { get { return true; } }

        public Material m_Material;
        Texture2D _lutTexture;

        protected override void OnInit()
        {
            m_Material = PostprocessUtilities.GetMaterial("Hidden/Postprocessing/ColorSuite");

            _lutTexture = new Texture2D(512, 1, TextureFormat.ARGB32, false, true);
            _lutTexture.hideFlags = HideFlags.DontSave;
            _lutTexture.wrapMode = TextureWrapMode.Clamp;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            if(m_Material != null)
                PostprocessUtilities.DestroyObject(m_Material);
            if(_lutTexture != null)
                PostprocessUtilities.DestroyObject(_lutTexture);
        }

        #region Local Functions

        // RGBM encoding.
        static Color EncodeRGBM(float r, float g, float b)
        {
            var a = Mathf.Max(Mathf.Max(r, g), Mathf.Max(b, 1e-6f));
            a = Mathf.Ceil(a * 255) / 255;
            return new Color(r / a, g / a, b / a, a);
        }

        // An analytical model of chromaticity of the standard illuminant, by Judd et al.
        // http://en.wikipedia.org/wiki/Standard_illuminant#Illuminant_series_D
        // Slightly modifed to adjust it with the D65 white point (x=0.31271, y=0.32902).
        static float StandardIlluminantY(float x)
        {
            return 2.87f * x - 3.0f * x * x - 0.27509507f;
        }

        // CIE xy chromaticity to CAT02 LMS.
        // http://en.wikipedia.org/wiki/LMS_color_space#CAT02
        static Vector3 CIExyToLMS(float x, float y)
        {
            var Y = 1.0f;
            var X = Y * x / y;
            var Z = Y * (1.0f - x - y) / y;

            var L = 0.7328f * X + 0.4296f * Y - 0.1624f * Z;
            var M = -0.7036f * X + 1.6975f * Y + 0.0061f * Z;
            var S = 0.0030f * X + 0.0136f * Y + 0.9834f * Z;

            return new Vector3(L, M, S);
        }

        #endregion

        #region Private Methods

        // Update the LUT texture.
        void UpdateLUT()
        {
            for (var x = 0; x < _lutTexture.width; x++)
            {
                var u = 1.0f / (_lutTexture.width - 1) * x;
                var r = m_Settings._cCurve.Evaluate(m_Settings._rCurve.Evaluate(u));
                var g = m_Settings._cCurve.Evaluate(m_Settings._gCurve.Evaluate(u));
                var b = m_Settings._cCurve.Evaluate(m_Settings._bCurve.Evaluate(u));
                _lutTexture.SetPixel(x, 0, EncodeRGBM(r, g, b));
            }
            _lutTexture.Apply();
        }

        // Calculate the color balance coefficients.
        Vector3 CalculateColorBalance()
        {
            // Get the CIE xy chromaticity of the reference white point.
            // Note: 0.31271 = x value on the D65 white point
            var x = 0.31271f - m_Settings._colorTemp * (m_Settings._colorTemp < 0.0f ? 0.1f : 0.05f);
            var y = StandardIlluminantY(x) + m_Settings._colorTint * 0.05f;

            // Calculate the coefficients in the LMS space.
            var w1 = new Vector3(0.949237f, 1.03542f, 1.08728f); // D65 white point
            var w2 = CIExyToLMS(x, y);
            return new Vector3(w1.x / w2.x, w1.y / w2.y, w1.z / w2.z);
        }

        #endregion

        public override void Render(CommandBuffer cmd, bool renderToScreen = false)
        {
            cmd.BeginSample("Postprocess-ColorSuite");

            if (m_Settings == null)
                m_Settings = GetDefaultSettings();

            if (m_Settings.mode == ColorSuiteMode.External)
            {
                if(m_Settings.Lut != null)
                {
                    layer.FinalPassMaterial.EnableKeyword("COLORSUITE_EXTERNAL");
                    layer.FinalPassMaterial.SetTexture("_LutTexture", m_Settings.Lut);
                }
            }
            else
            {
                layer.FinalPassMaterial.DisableKeyword("COLORSUITE_EXTERNAL");

                var linear = QualitySettings.activeColorSpace == ColorSpace.Linear;

                UpdateLUT();

                if (linear)
                    m_Material.EnableKeyword("COLORSPACE_LINEAR");
                else
                    m_Material.DisableKeyword("COLORSPACE_LINEAR");

                if (m_Settings._colorTemp != 0.0f || m_Settings._colorTint != 0.0f)
                {
                    m_Material.EnableKeyword("BALANCING_ON");
                    m_Material.SetVector("_Balance", CalculateColorBalance());
                }
                else
                    m_Material.DisableKeyword("BALANCING_ON");

                if (m_Settings._toneMapping && linear)
                {
                    m_Material.EnableKeyword("TONEMAPPING_ON");
                    m_Material.SetFloat("_Exposure", m_Settings._exposure);
                }
                else
                    m_Material.DisableKeyword("TONEMAPPING_ON");

                m_Material.SetTexture("_Curves", _lutTexture);
                m_Material.SetFloat("_Saturation", m_Settings._saturation);

                if (m_Settings._ditherMode == DitherMode.Ordered)
                {
                    m_Material.EnableKeyword("DITHER_ORDERED");
                    m_Material.DisableKeyword("DITHER_TRIANGULAR");
                }
                else if (m_Settings._ditherMode == DitherMode.Triangular)
                {
                    m_Material.DisableKeyword("DITHER_ORDERED");
                    m_Material.EnableKeyword("DITHER_TRIANGULAR");
                }
                else
                {
                    m_Material.DisableKeyword("DITHER_ORDERED");
                    m_Material.DisableKeyword("DITHER_TRIANGULAR");
                }

                cmd.BlitFullscreenTriangle(layer.SourceRT, layer.TargetRT, m_Material, -1, false);
            }

            cmd.EndSample("Postprocess-ColorSuite");
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
