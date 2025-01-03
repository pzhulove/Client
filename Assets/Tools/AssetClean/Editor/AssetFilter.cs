using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    public class AssetFilter
    {
        public enum AssetMask
        {
            None = 0,
            Other,
            All
        };

        // 资源filter类型，三种：
        // 1: ShowAll为true，显示所有类型资源。
        // 2: Inclusive为ture，Types中为需要显示的资源类型列表。
        // 3: Inclusive为false，Types中为不需要显示的资源类型列表，其他的都要显示。
        public class AssetFilterResult
        {
            public bool ShowAll;
            public bool Inclusive;
            public List<Type> Types = new List<Type>();
        }

        public class AssetFilterType
        {
            public bool CheckOn;
            public string Name;
            public Type AType;
            public AssetMask Mask;

            public AssetFilterType(bool defaultValue, string name, Type type, AssetMask mask = AssetMask.None)
            {
                CheckOn = defaultValue;
                Name = name;
                AType = type;
                Mask = mask;

                if (type == null && mask == AssetMask.None)
                {
                    Debug.LogError("type == null && mask == AssetMask.None not Allowed~!");
                }
            }
        }

        // 如果有All或Other，则最后一个是All，倒数第二个是Other。
        private List<AssetFilterType> m_AssetTypeToggle = new List<AssetFilterType>();
        private AssetFilterResult m_FileterResult;
        private Action<AssetFilterResult> m_OnFilterChange;

        public AssetFilter(Action<AssetFilterResult> filterChange)
        {
            m_OnFilterChange = filterChange;
        }

        public void AddAssetFilterType(bool defaultValue, string name, Type type, AssetMask mask = AssetMask.None)
        {
            m_AssetTypeToggle.Add(new AssetFilterType(defaultValue, name, type, mask));
        }

        public void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            for (int i = 0; i < m_AssetTypeToggle.Count; ++i)
            {
                bool preValue = m_AssetTypeToggle[i].CheckOn;
                m_AssetTypeToggle[i].CheckOn = GUILayout.Toggle(m_AssetTypeToggle[i].CheckOn, m_AssetTypeToggle[i].Name);

                if (preValue != m_AssetTypeToggle[i].CheckOn)
                {
                    OnToggleChanged(m_AssetTypeToggle[i]);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void OnToggleChanged(AssetFilterType assetType)
        {
            if (assetType.CheckOn)
            {
                int size = m_AssetTypeToggle.Count;

                // 如果是All选中了，其他的都取消选中。否则若是其他的选中，取消All的选中。
                if (assetType.Mask == AssetMask.All)
                {
                    for (int i = 0; i < size - 1; ++i)
                    {
                        m_AssetTypeToggle[i].CheckOn = false;
                    }
                }
                else if (size > 0 && m_AssetTypeToggle[size - 1].Mask == AssetMask.All)
                {
                    m_AssetTypeToggle[size - 1].CheckOn = false;
                }
            }

            UpdateFilterResult();

            if (m_OnFilterChange != null)
            {
                m_OnFilterChange(m_FileterResult);
            }
        }

        private void UpdateFilterResult()
        {
            if (m_FileterResult == null)
            {
                m_FileterResult = new AssetFilterResult();
            }

            m_FileterResult.ShowAll = true;
            m_FileterResult.Inclusive = true;
            m_FileterResult.Types.Clear();

            int size = m_AssetTypeToggle.Count;
            int iTypeStart = size - 1;
            if (size == 0)
                return;

            // 如果All选中了，直接返回
            if (m_AssetTypeToggle[size - 1].Mask == AssetMask.All && m_AssetTypeToggle[size - 1].CheckOn)
            {
                return;
            }

            m_FileterResult.ShowAll = false;

            // 如果最后一个是Other且选中了，Inclusive设置为false
            if (m_AssetTypeToggle[size - 1].Mask == AssetMask.Other && m_AssetTypeToggle[size - 1].CheckOn)
            {
                m_FileterResult.Inclusive = false;
                iTypeStart = size - 2;
            }

            // 如果倒数第二个是Other且选中了，Inclusive设置为false
            if (size > 1)
            {
                if (m_AssetTypeToggle[size - 2].Mask == AssetMask.Other && m_AssetTypeToggle[size - 2].CheckOn)
                {
                    m_FileterResult.Inclusive = false;
                    iTypeStart = size - 3;
                }
            }


            for (int i = iTypeStart; i >= 0; --i)
            {
                // 如果Inclusive为true，则选中的添加到列表中。否则，未选中的添加到列表。
                if (m_FileterResult.Inclusive == m_AssetTypeToggle[i].CheckOn)
                {
                    m_FileterResult.Types.Add(m_AssetTypeToggle[i].AType);
                }
            }
        }

        public AssetFilterResult GetFilterResult()
        {
            if (m_FileterResult == null)
            {
                m_FileterResult = new AssetFilterResult();
                UpdateFilterResult();
            }

            return m_FileterResult;
        }
    }
}
