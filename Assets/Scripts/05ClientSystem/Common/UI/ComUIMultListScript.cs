using UnityEngine.EventSystems;

namespace Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Serialization;
    using Tenmove.Runtime;

    public class ComUIMultListScript : ComUIListScript
    {
	    public delegate int          OnSelectPrefabDelegate(int index);

		[HideInInspector]
        public OnSelectPrefabDelegate OnSelectPrefab;

		[HideInInspector]
        public Func<int, string> OnBindPrefabPath;

        [SerializeField]
        private List<string> listPrefabs = new List<string>();
        [SerializeField]
        private List<Vector2> listElementSize = new List<Vector2>();

        protected override string GetElementPrefabPath(int index)
        {
            if (OnBindPrefabPath != null)
                return OnBindPrefabPath(index);

            if (OnSelectPrefab == null || listPrefabs.Count == 0)
            {
                return m_externalElementPrefabPath;
            }
            int type = OnSelectPrefab(index);
            type = Mathf.Clamp(type, 0, listPrefabs.Count - 1);
            return listPrefabs[type];
        }

        public Vector2 GetElementSize(int index)
        {
            if (OnSelectPrefab == null || listElementSize.Count == 0)
            {
                return m_elementDefaultSize;
            }
            int type = OnSelectPrefab(index);
            type = Mathf.Clamp(type, 0, listElementSize.Count - 1);
            return listElementSize[type];
        }
        protected override ComUIListElementScript CreateElement(int index, ref stRect rect)
        {
            ComUIListElementScript item = null;
            string prefabPath = GetElementPrefabPath(index);
            for (int i = 0; i < m_unUsedElementScripts.Count; i++)
            {
                var titem = this.m_unUsedElementScripts[i];
                if (titem.prefabPath == prefabPath)
                {
                    this.m_unUsedElementScripts.RemoveAt(i);
                    item = titem;
                    break;
                }
            }
            if (item == null)
            {
                GameObject content = (GameObject)AssetLoader.instance.LoadResAsGameObject(prefabPath);
                content.transform.SetParent(this.m_content.transform, false);
                content.transform.localScale = Vector3.one;
                item = content.GetComponent<ComUIListElementScript>();
            }
            if(item != null)
            {
                if (onBindItem != null && item.gameObjectBindScript == null)
                {
                    item.gameObjectBindScript = onBindItem(item.gameObject);
                }

                item.prefabPath = prefabPath;
                item.Initialize();
                item.Enable(this, index, this.m_elementName, ref rect, this.IsSelectedIndex(index));
                this.m_elementScripts.Add(item);

                if (onItemVisiable != null)
                {
                    onItemVisiable(item);
                }
            }
            return item;
        }
        protected override stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect rect;
            rect = new stRect();
            //计算元素的位置和宽、高
            var elementsSize = m_elementsSize != null && index >= 0 && index < this.m_elementsSize.Count ? m_elementsSize[index] : GetElementSize(index);
            rect.m_width = (int)elementsSize.x;
            rect.m_height = (int)elementsSize.y;
            rect.m_left = (int)offset.x;
            rect.m_top = (int)offset.y;
            rect.m_right = rect.m_left + rect.m_width;
            rect.m_bottom = rect.m_top - rect.m_height;
            rect.m_center = new Vector2(rect.m_left + (rect.m_width * 0.5f), rect.m_top - (rect.m_height * 0.5f));

            //如果元素的宽高超过content的宽高，则设置content的宽高
            if (rect.m_right > contentSize.x)
            {
                contentSize.x = rect.m_right;
            }
            if (-rect.m_bottom > contentSize.y)
            {
                contentSize.y = -rect.m_bottom;
            }
            switch (this.m_listType)
            {
                case enUIListType.Vertical:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    return rect;

                case enUIListType.Horizontal:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    return rect;

                case enUIListType.VerticalGrid:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    if ((offset.x + rect.m_width) > this.m_scrollAreaSize.x)
                    {
                        offset.x = m_elementPadding.x;
                        offset.y -= rect.m_height + this.m_elementSpacing.y;
                    }
                    return rect;

                case enUIListType.HorizontalGrid:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    if ((-offset.y + rect.m_height) > this.m_scrollAreaSize.y)
                    {
                        offset.y = m_elementPadding.y;
                        offset.x += rect.m_width + this.m_elementSpacing.x;
                    }
                    return rect;
            }
            return rect;
        }

        
    }
}

