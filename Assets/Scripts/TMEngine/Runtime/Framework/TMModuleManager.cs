
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    /// <summary>
    /// 游戏框架入口。
    /// </summary>
    public static class ModuleManager
    {
        private const string FrameworkVersion = "1.0.0";

        // 按照Priority从大到小排序，Priority大的先Update
        private static readonly LinkedList<BaseModule> sm_GameFrameworkModules = new LinkedList<BaseModule>();

        /// <summary>
        /// 获取游戏框架版本号。
        /// </summary>
        public static string Version
        {
            get
            {
                return FrameworkVersion;
            }
        }
        
        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (BaseModule module in sm_GameFrameworkModules)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理所有游戏框架模块。
        /// </summary>
        public static void Shutdown()
        {
            for (LinkedListNode<BaseModule> current = sm_GameFrameworkModules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            sm_GameFrameworkModules.Clear();
            //ReferencePool.ClearAll();
            //Log.SetLogHelper(null);
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        public static T GetModule<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                Debugger.AssertFailed(string.Format("You must get module by interface, but '{0}' is not.", interfaceType.FullName));
                return null;
            }

            if (!interfaceType.FullName.StartsWith("Tenmove."))
            {
                Debugger.AssertFailed(string.Format("You must get a Game Framework module, but '{0}' is not.", interfaceType.FullName));
                return null;
            }

            string moduleName = string.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(3));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                Debugger.AssertFailed(string.Format("Can not find BaseFramework module type '{0}'.", moduleName));
                return null;
            }

            return _GetModule(moduleType) as T;
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要获取的游戏框架模块类型。</param>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        private static BaseModule _GetModule(Type moduleType)
        {
            foreach (BaseModule module in sm_GameFrameworkModules)
            {
                if (module.GetType() == moduleType)
                {
                    return module;
                }
            }

            return _CreateModule(moduleType);
        }

        /// <summary>
        /// 创建游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要创建的游戏框架模块类型。</param>
        /// <returns>要创建的游戏框架模块。</returns>
        private static BaseModule _CreateModule(Type moduleType)
        {
            BaseModule module = (BaseModule)Utility.Assembly.CreateInstance(moduleType);
            if (module == null)
            {
                Debugger.AssertFailed(string.Format("Can not create module '{0}'.", module.GetType().FullName));
                return null;
            }

            LinkedListNode<BaseModule> current = sm_GameFrameworkModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                sm_GameFrameworkModules.AddBefore(current, module);
            }
            else
            {
                sm_GameFrameworkModules.AddLast(module);
            }

            return module;
        }
    }
}

