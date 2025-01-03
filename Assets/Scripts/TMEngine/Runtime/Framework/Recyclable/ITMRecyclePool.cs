
namespace Tenmove.Runtime
{
    /// <summary>
    /// 可回收对象池（频繁创建和销毁的实例）
    /// </summary>
    public interface ITMRecyclePool<T> where T : Recyclable, new()
    {
        /// <summary>
        /// 保留对象数
        /// </summary>
        int ReserveCount
        {
            get;
        }

        /// <summary>
        /// 未使用的对象个数
        /// </summary>
        int UnusedObjectCount
        {
            get;
        }

        /// <summary>
        /// 正在被使用的对象个数
        /// </summary>
        int UsingObjectCount
        {
            get;
        }

        /// <summary>
        /// 请求对象的次数
        /// </summary>
        int AcquireCount
        {
            get;
        }

        /// <summary>
        /// 回收对象的次数
        /// </summary>
        int RecycleCount
        {
            get;
        }

        /// <summary>
        /// 创建的对象个数
        /// </summary>
        int CreateCount
        {
            get;
        }

        /// <summary>
        /// 彻底释放对象的个数
        /// </summary>
        int ReleaseCount
        {
            get;
        }

        /// <summary>
        /// 设置池子的预保留对象数量
        /// </summary>
        /// <param name="reserveCount">预保留对象数量</param>
        void SetReserveCount(int reserveCount);

        /// <summary>
        /// 设置池子的预保留对象数量
        /// </summary>
        /// <param name="reserveCount">预保留对象数量</param>
        void SetExpireTime(float expireTime);

        /// <summary>
        /// 设置池子的预保留对象数量
        /// </summary>
        /// <param name="reserveCount">预保留对象数量</param>
        void SetPriority(int priority);

        /// <summary>
        /// 申请对象
        /// </summary>
        /// <returns>返回申请的对象</returns>
        T Acquire();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">要被回收的对象</param>
        void Recycle(T obj);

        /// <summary>
        /// 清洗池子
        /// </summary>
        /// <param name="clearAll">是否清理所有对象，如果是则清理所有，如果为否则保留Reserve所指定的对象个数</param>
        void PurgePool(bool clearAll);
    }
}

