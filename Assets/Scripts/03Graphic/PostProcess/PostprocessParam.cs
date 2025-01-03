namespace Tenmove.Runtime.Client
{
    public class PostProcessParam
    {
        public PostProcessType PostEffectType { protected set; get; }
    }

    public class RadialBlurParam : PostProcessParam
    {
        public RadialBlurParam()
        {
            PostEffectType = PostProcessType.RadialBlur;
            ConfigPath = "EngineRes/PostprocessData/RadialBlurSetting.asset";
        }

        public bool Active { set; get; }

        public VInt2 Center { set; get; }

        public string ConfigPath { set; get; }
    }
}
