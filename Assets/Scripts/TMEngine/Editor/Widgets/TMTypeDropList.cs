using System.Collections.Generic;
using UnityEditor;

namespace Tenmove.Editor.Unity.Widgets
{
    public sealed class TypeDropList<T> where T : class
    {
        private SerializedProperty m_TypeName;

        private string[] m_TypeNamesList;
        private int m_SelectTypeNameIndex;

        public TypeDropList()
        {
            m_TypeName = null;
            m_TypeNamesList = null;
            m_SelectTypeNameIndex = -1;
        }

        public void Init(SerializedObject serializedObject,string typePropFieldName)
        {
            m_TypeName = serializedObject.FindProperty(typePropFieldName);
            if (string.IsNullOrEmpty(m_TypeName.stringValue))
                Refresh();
        }

        public void Draw()
        {
            string displayName = Tenmove.Runtime.Utility.Text.GetNameWithType<T>("");
            int selectIndex = EditorGUILayout.Popup(string.Format("{0}:", displayName), m_SelectTypeNameIndex, m_TypeNamesList);
            if(selectIndex != m_SelectTypeNameIndex)
            {
                m_SelectTypeNameIndex = selectIndex;
                m_TypeName.stringValue = (selectIndex < 0 ? null : m_TypeNamesList[selectIndex]);
            }
        }

        public void Refresh()
        {
            List<string> typeNameList = new List<string>();
            typeNameList.AddRange(Tenmove.Runtime.Utility.Assembly.GetTypeNamesOf(typeof(T)));
            m_TypeNamesList = typeNameList.ToArray();
            
            if(!string.IsNullOrEmpty(m_TypeName.stringValue))
            {
                m_SelectTypeNameIndex = typeNameList.IndexOf(m_TypeName.stringValue);
                if(m_SelectTypeNameIndex < 0)
                {
                    m_SelectTypeNameIndex = -1;
                    m_TypeName.stringValue = null;
                }
            }
            else
            {
                if(m_TypeNamesList.Length > 0)
                {
                    m_SelectTypeNameIndex = 0;
                    m_TypeName.stringValue = m_TypeNamesList[m_SelectTypeNameIndex];
                }
            }
        }
    }
}
