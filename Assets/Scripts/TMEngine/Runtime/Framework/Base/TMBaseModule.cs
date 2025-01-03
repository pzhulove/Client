
namespace Tenmove.Runtime
{
    /*
     * 引擎Module
     * 1：EventManager       priority = 100
     * 2：ObjectPoolManager  priority = 90
     * 3：ResourceManager    priority = 70
     */
    /// <summary>
    /// 游戏框架模块抽象类。
    /// </summary>
    public abstract class BaseModule
    {
        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal abstract int Priority
        {
            get;
        }

        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        internal abstract void Shutdown();
    }
}

