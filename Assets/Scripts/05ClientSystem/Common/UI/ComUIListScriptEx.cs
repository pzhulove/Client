
using UnityEngine;

namespace Scripts.UI
{
    public class ComUIListScriptEx :ComUIListScript
    {

        protected override ComUIListElementScript CreateElement(int index,
            ref stRect rect)
        {
            ComUIListElementScript item = null;
            if (this.m_unUsedElementScripts.Count > 0)
            {
                bool isFind = false;
                for (var i = 0; i < this.m_unUsedElementScripts.Count; i++)
                {
                    if (this.m_unUsedElementScripts[i] != null
                        && this.m_unUsedElementScripts[i].m_index == index)
                    {
                        item = this.m_unUsedElementScripts[i];
                        this.m_unUsedElementScripts.RemoveAt(i);
                        isFind = true;
                        break;
                    }
                }
                //没有找到
                if (isFind == false)
                {
                    item = this.m_unUsedElementScripts[0];
                    this.m_unUsedElementScripts.RemoveAt(0);
                }
            }
            else if (this.m_elementTemplate != null)
            {
                GameObject root = InstantiateElement(this.m_elementTemplate);
                root.transform.SetParent(this.m_content.transform);
                root.transform.localScale = Vector3.one;
                item = root.GetComponent<ComUIListElementScript>();
            }
            if (item != null)
            {
                if (onBindItem != null && item.gameObjectBindScript == null)
                {
                    item.gameObjectBindScript = onBindItem(item.gameObject);
                }

                item.Initialize();
                item.Enable(this, index, this.m_elementName, ref rect, 
                    this.IsSelectedIndex(index));
                this.m_elementScripts.Add(item);

                if (onItemVisiable != null)
                {
                    onItemVisiable(item);
                }
            }
            return item;
        }

        //将Index对应的元素，显示在第一个位置；
        //如果当前的位置元素为最后几个，即：则当前的位置的元素显示在界面内，并且最后一个Item同时显示在界面内，
        //最后一个Item后面还有空间，则位置调整为最后一个元素在界面的最后位置
        public void MoveItemToFirstPosition(int index)
        {
            //如果是第一个元素，默认不用跳转
            if (index < 0 || index >= this.m_elementAmount)
                return;

            var isItemCoverScrollRect = IsItemCoverScrollRect();
            if (isItemCoverScrollRect == false)
                return;

            var currentItemRect = !this.m_useOptimized
                ? this.m_elementScripts[index].m_rect
                : this.m_elementsRect[index];

            var lastItemRect = !this.m_useOptimized
                ? this.m_elementScripts[this.m_elementAmount - 1].m_rect
                : this.m_elementsRect[this.m_elementAmount - 1];

            switch (this.m_listType)
            {
                case enUIListType.Horizontal:
                case enUIListType.HorizontalGrid:
                    //水平方向
                    var currentMovePositionX = -(currentItemRect.m_left - m_elementSpacing.x / 2.0f);
                    var lastItemPositionX = -(lastItemRect.m_right + this.m_elementSpacing.x / 2.0f);
                    //跳转过头,即：如果index位置的元素显示在第一个，最后一个元素显示出来，最后元素后面还有剩余空间
                    //则限制元素的显示，最后一个元素放置在最后的位置
                    if (currentMovePositionX - lastItemPositionX < this.m_scrollAreaSize.x)
                    {
                        this.m_contentRectTransform.anchoredPosition = new Vector2(
                            lastItemPositionX + this.m_scrollAreaSize.x,
                            this.m_contentRectTransform.anchoredPosition.y);
                    }
                    else
                    {
                        //index位置的元素显示在第一个
                        this.m_contentRectTransform.anchoredPosition = new Vector2(currentMovePositionX,
                            this.m_contentRectTransform.anchoredPosition.y);
                    }
                    break;
                case enUIListType.Vertical:
                case enUIListType.VerticalGrid:
                    //竖直方向
                    var currentMovePositionY = -(currentItemRect.m_top + m_elementSpacing.y / 2.0f);
                    var lastItemPositionY = -(lastItemRect.m_bottom - this.m_elementSpacing.y / 2.0f);
                    //跳转过头,即：如果index位置的元素显示在第一个，最后一个元素显示出来，最后元素后面还有剩余空间
                    //则限制元素的显示，最后一个元素放置在最后的位置
                    if (lastItemPositionY - currentMovePositionY < this.m_scrollAreaSize.y)
                    {
                        this.m_contentRectTransform.anchoredPosition = new Vector2(
                            this.m_contentRectTransform.anchoredPosition.x,
                            lastItemPositionY - this.m_scrollAreaSize.y);
                    }
                    else
                    {
                        //index位置的元素显示在第一个
                        this.m_contentRectTransform.anchoredPosition = new Vector2(
                            this.m_contentRectTransform.anchoredPosition.x,
                            currentMovePositionY);
                    }
                    break;
            }
        }

        //将Index对应的元素，显示在中间位置；
        //如果当前位置的元素为前几个元素，则第0个元素在最前面；
        //如果当前元素为后几个元素，则最后一个元素显示在最后面；
        //否则：当前元素显示在中间位置
        public void MoveItemToMiddlePosition(int index)
        {
            if (index < 0 || index >= this.m_elementAmount)
                return;

            var isItemCoverScrollRect = IsItemCoverScrollRect();
            if (isItemCoverScrollRect == false)
                return;

            var currentItemRect = !this.m_useOptimized
               ? this.m_elementScripts[index].m_rect
               : this.m_elementsRect[index];

            var lastItemRect = !this.m_useOptimized
                ? this.m_elementScripts[this.m_elementAmount - 1].m_rect
                : this.m_elementsRect[this.m_elementAmount - 1];

            switch (this.m_listType)
            {
                case enUIListType.Horizontal:
                case enUIListType.HorizontalGrid:
                    //水平方向
                    var currentMovePositionX = (this.m_scrollAreaSize.x * 0.5f - currentItemRect.m_center.x);
                    //前几个元素
                    if (currentMovePositionX >= 0)
                    {
                        currentMovePositionX = 0;
                    }
                    else if (currentMovePositionX + lastItemRect.m_right < this.m_scrollAreaSize.x)
                    {
                        //最后几个元素
                        currentMovePositionX = this.m_scrollAreaSize.x - lastItemRect.m_right;
                    }

                    //设置位置
                    this.m_contentRectTransform.anchoredPosition = new Vector2(
                        currentMovePositionX,
                        this.m_contentRectTransform.anchoredPosition.y);

                    break;
                case enUIListType.Vertical:
                case enUIListType.VerticalGrid:
                    //竖直方向
                    var currentMovePositionY = (-this.m_scrollAreaSize.y * 0.5f - currentItemRect.m_center.y);
                    //前几个元素
                    if (currentMovePositionY <= 0)
                    {
                        currentMovePositionY = 0;
                    }
                    else if (currentMovePositionY + lastItemRect.m_bottom > -this.m_scrollAreaSize.y)
                    {
                        //最后几个元素
                        currentMovePositionY = -this.m_scrollAreaSize.y - lastItemRect.m_bottom;
                    }

                    //设置位置
                    this.m_contentRectTransform.anchoredPosition = new Vector2(
                        this.m_contentRectTransform.anchoredPosition.x,
                        currentMovePositionY);

                    break;
            }
        }

        //判断Item的数量是否可以超过ScrollRect的大小
        private bool IsItemCoverScrollRect()
        {
            bool isItemCoverFlag = false;
            switch (m_listType)
            {
                case enUIListType.Horizontal:
                case enUIListType.HorizontalGrid:
                    if (this.m_contentSize.x > this.m_scrollAreaSize.x)
                        isItemCoverFlag = true;
                    break;
                case enUIListType.Vertical:
                case enUIListType.VerticalGrid:
                    if (this.m_contentSize.y > this.m_scrollAreaSize.y)
                        isItemCoverFlag = true;
                    break;
            }

            return isItemCoverFlag;
        }

        public void ResetComUiListScriptEx()
        {
            //暂停移动
            if(m_scrollRect != null)
                m_scrollRect.StopMovement();

            //位置重置
            ResetContentPosition();
        }

    }
}
