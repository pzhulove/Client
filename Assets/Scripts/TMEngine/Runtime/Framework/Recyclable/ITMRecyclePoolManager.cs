
namespace Tenmove.Runtime
{
    public delegate Recyclable CreateRecyclable();

    public interface ITMRecyclePoolManager
    {
        RecyclePoolBase CreateRecyclePool<T>(CreateRecyclable createAction) where T : Recyclable, new();

        void DestroyRecyclePool(RecyclePoolBase objPoolBase);
    }
}