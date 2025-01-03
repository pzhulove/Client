using System;
using Tenmove.Runtime.Client;

namespace Tenmove.Runtime.Unity
{
    [Serializable]
    public class RadialBlurSetting : PostprocessEffectSettings
    {
        public float BlurFactor = 0.01f;
        public float LerpFactor = 4f;
        public float DisppearSpeed = 0.7f;

        public override PostProcessType EffectType
        {
            get
            {
                return PostProcessType.RadialBlur;
            }
        }
    }
}

