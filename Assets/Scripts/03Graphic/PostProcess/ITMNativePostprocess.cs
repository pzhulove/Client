using Tenmove.Runtime.Client;

namespace Tenmove.Runtime
{
    public interface ITMNativePostprocess
    {
        void SetRenderTarget(object renderTarget);
        void SetPostProcess<TParam>(TParam param) where TParam : PostProcessParam;
    }
}

