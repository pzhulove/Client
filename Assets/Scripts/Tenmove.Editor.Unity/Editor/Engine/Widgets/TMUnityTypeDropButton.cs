using Tenmove.Runtime;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Internal;

namespace Tenmove.Editor.Unity.Widgets
{
    public sealed class UnityTypeDropButton
    {
        private GenericMenu m_DropDownList;
        private string[] m_TypeNamesList;
        private Type[] m_TypeList;
        private Function<Type> m_OnSelected;

        public UnityTypeDropButton()
        {
            m_TypeNamesList = null;
            m_TypeList = null;
            m_OnSelected = null;
            m_DropDownList = null;
        }

        public void Init(Type[] typeList, Function<Type> onSelected)
        {
            if (null != typeList && typeList.Length > 0)
            {
                m_OnSelected = onSelected;
                _Refresh(typeList);
            }
            else
                Debugger.LogWarning("Parameter 'typeList' can not be null or empty!");
        }

        public void Draw(string displayText,GUIStyle style = null)
        {
            if (GUILayout.Button(displayText,null == style ? "button" : style))
                m_DropDownList.ShowAsContext();           
        }

        private void _Refresh(Type[] typeList)
        {
            m_TypeList = typeList;
            if (null != m_TypeList)
            {
                m_DropDownList = new GenericMenu();
                m_TypeNamesList = new string[m_TypeList.Length];
                for (int i = 0, icnt = m_TypeList.Length; i < icnt; ++i)
                {
                    m_TypeNamesList[i] = m_TypeList[i].Name;
                    GUIContent content = new GUIContent(m_TypeNamesList[i]);
                    m_DropDownList.AddItem(content,false, _OnSelectedInternal, m_TypeList[i]);
                }
            }
        }

        private void _OnSelectedInternal(object target)
        {
            if (null != m_OnSelected)
                m_OnSelected(target as Type);
            else
                Debugger.LogWarning("OnSelected is not bind any callback!");
        }
    }
}
