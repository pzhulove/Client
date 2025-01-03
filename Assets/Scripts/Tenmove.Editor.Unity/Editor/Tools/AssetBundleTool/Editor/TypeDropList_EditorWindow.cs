using System.Collections.Generic;
using UnityEditor;
namespace AssetBundleTool
{
    public class TypeDropList_EditorWindow<T> where T : class
    {
        public string m_TypeName;

        private string[] m_TypeNamesList;
        private int m_SelectTypeNameIndex;

        public TypeDropList_EditorWindow()
        {
            m_TypeName = null;
            m_TypeNamesList = null;
            m_SelectTypeNameIndex = -1;
        }

        public void Init(string typePropFieldName)
        {
            m_TypeName = typePropFieldName;
            if (string.IsNullOrEmpty(m_TypeName))
                Refresh();
        }
        public bool Draw(string displayLabel = "")
        {
            bool m_change = false;
            if (string.IsNullOrEmpty(displayLabel))
                displayLabel = string.Format("{0}:", Tenmove.Runtime.Utility.Text.GetNameWithType<T>(""));
            int selectIndex = EditorGUILayout.Popup(displayLabel, m_SelectTypeNameIndex, m_TypeNamesList);
            if (selectIndex != m_SelectTypeNameIndex)
            {
                m_change = true;
                m_SelectTypeNameIndex = selectIndex;
                m_TypeName = (selectIndex < 0 ? null : m_TypeNamesList[selectIndex]);
            }
            return m_change;
        }

        public void Refresh()
        {
            List<string> typeNameList = new List<string>();
            typeNameList.AddRange(Tenmove.Runtime.Utility.Assembly.GetTypeNamesOf(typeof(T)));
            m_TypeNamesList = typeNameList.ToArray();

            if (!string.IsNullOrEmpty(m_TypeName))
            {
                m_SelectTypeNameIndex = typeNameList.IndexOf(m_TypeName);
                if (m_SelectTypeNameIndex < 0)
                {
                    m_SelectTypeNameIndex = -1;
                    m_TypeName = null;
                }
            }
            else
            {
                if (m_TypeNamesList.Length > 0)
                {
                    m_SelectTypeNameIndex = 0;
                    m_TypeName = m_TypeNamesList[m_SelectTypeNameIndex];
                }
            }
        }
    }
}
