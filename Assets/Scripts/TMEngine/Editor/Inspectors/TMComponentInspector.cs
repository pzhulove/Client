﻿using UnityEditor;

namespace Tenmove.Editor.Unity
{
    /// <summary>
    /// 游戏框架 Inspector 抽象类。
    /// </summary>
    internal abstract class ComponentInspector : UnityEditor.Editor
    {
        private bool m_IsCompiling = false;

        /// <summary>
        /// 绘制事件。
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (m_IsCompiling && !EditorApplication.isCompiling)
            {
                m_IsCompiling = false;
                OnCompileComplete();
            }
            else if (!m_IsCompiling && EditorApplication.isCompiling)
            {
                m_IsCompiling = true;
                OnCompileStart();
            }
        }

        /// <summary>
        /// 编译开始事件。
        /// </summary>
        protected virtual void OnCompileStart()
        {

        }

        /// <summary>
        /// 编译完成事件。
        /// </summary>
        protected virtual void OnCompileComplete()
        {

        }
    }
}
