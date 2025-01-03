
using System;

namespace Tenmove.Runtime
{
    /// <summary>
    /// 对象回收站(可回收对象)
    /// </summary>
    public interface ITMRecycleBin
    {
        /// <summary>
        /// 设置指定类型的可回收对象保留个数
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="reserveCount">保留个数</param>
        /// <returns></returns>
        void SetReserveCountOfType(Type type, int reserveCount);

        /// <summary>
        /// 获取指定类型的可回收对象保留个数
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns>指定类型的对象保留个数</returns>
        int GetResserveCountOfType(Type type);

        /// <summary>
        /// 申请对象
        /// </summary>
        /// <returns>返回申请的对象</returns>
        T Acquire<T>() where T: Recyclable,new ();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">要被回收的对象</param>
        void Recycle<T>(T obj) where T : Recyclable, new();

        /// <summary>
        /// 清理所有指定类型的对象
        /// </summary>
        /// <param name="type">要清理对象的类型</param>
        void ClearAllObjectOfType<T>() where T : Recyclable, new();

        /// <summary>
        /// 清洗池子
        /// </summary>
        /// <param name="clearAll">是否清理所有对象，如果是则清理所有，如果为否则保留Reserve所指定的对象个数</param>
        void Purge(bool clearAll);        
    }
}

