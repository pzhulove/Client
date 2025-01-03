
namespace Tenmove.Runtime
{
    /// <summary>
    /// 可回收对象（频繁创建和销毁的实例）
    /// </summary>
    public abstract class Recyclable
    {
        private bool m_IsRecycled = false;

        public bool IsRecycled
        {
            get { return m_IsRecycled; }
        }

        public abstract bool IsValid
        {
            get;
        }

        public virtual void OnCreate()
        {
        }

        /// <summary>
        /// 对象再次使用处理
        /// </summary>
        public virtual void OnReuse()
        {
            m_IsRecycled = false;
        }

        /// <summary>
        /// 对象回收处理
        /// </summary>
        public virtual void OnRecycle()
        {
            m_IsRecycled = true;
        }

        /// <summary>
        /// 销毁处理
        /// </summary>
        public abstract void OnRelease();
    }
}

