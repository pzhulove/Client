

using System.IO;

namespace Tenmove.Runtime
{
    public interface IDataVerifier<T>
    {
        float Progress
        {
            get;
        }

        bool BeginVerify(Stream dataStream,int cacheSize, float timeSlice);
        bool EndVerify();

        T GetVerifySum();
    }
}