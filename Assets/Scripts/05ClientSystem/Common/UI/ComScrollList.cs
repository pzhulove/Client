using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{
    public abstract class ScrollItem
    {
        int m_nDataID;
        public int GetDataID()
        {
            return m_nDataID;
        }

        public void Setup(int a_nDataID)
        {
            m_nDataID = a_nDataID;
            _Refresh(a_nDataID);
        }

        public abstract ScrollItem Clone();
        public abstract void Destroy();
        public abstract void SetAsFirstSibling();
        public abstract void SetAsLastSibling();
        public abstract void SetActive(bool a_bActive);
        public abstract bool IsActive();
        //public abstract Vector2 GetSize();
        public abstract Vector3 GetPosInContent();
        protected abstract void _Refresh(int a_nDataID);
    }

    public class ComScrollList : ScrollRect
    {
        public enum EStartFouce
        {
            Top,
            Bottom,
        }

        public int mainScrollItemCount = 5;
        public int dynamicMainScrollItemCount = 2;
        public int subScrollItemCount = 4;
        public EStartFouce startFouce = EStartFouce.Top;

        enum EState
        {
            Invalid,
            LoadingUp,
            Normal,
            FouceBottom,
            LoadingDown,
            PrepareLoadingDown,
            PrepareLoadingUp,
        }
        EState m_eState = EState.Invalid;
        ScrollItem m_template = null;
        int m_nMinDataID = -1;
        int m_nMaxDataID = -1;
        List<ScrollItem> m_arrScrollItems = new List<ScrollItem>();
        bool m_bDragging = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_bDragging = true;
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData data)
        {
            if (m_eState == EState.Normal)
            {
                if (verticalNormalizedPosition <= 0.0f && data.delta.y > 0)
                {
                    if (m_arrScrollItems.Count > 0)
                    {
                        int nCount = dynamicMainScrollItemCount * subScrollItemCount;
                        int nCurrentMaxDataID = 0;
                        for (int i = m_arrScrollItems.Count - 1; i >= 0; --i)
                        {
                            if (m_arrScrollItems[i].IsActive())
                            {
                                nCurrentMaxDataID = m_arrScrollItems[i].GetDataID();
                                break;
                            }
                        }

                        if (nCurrentMaxDataID + nCount > m_nMaxDataID)
                        {
                            nCount = m_nMaxDataID - nCurrentMaxDataID;
                        }

                        if (nCount > 0)
                        {
                            m_eState = EState.PrepareLoadingDown;
                        }
                        else
                        {
                            m_eState = EState.FouceBottom;
                        }
                    }
                    else
                    {
                        m_eState = EState.FouceBottom;
                    }
                }
                else if (verticalNormalizedPosition >= 1.0f && data.delta.y < 0)
                {
                    if (m_arrScrollItems.Count > 0)
                    {
                        int nCount = dynamicMainScrollItemCount * subScrollItemCount;
                        int nCurrentMinDataID = 0;
                        for (int i = 0; i < m_arrScrollItems.Count; ++i)
                        {
                            if (m_arrScrollItems[i].IsActive())
                            {
                                nCurrentMinDataID = m_arrScrollItems[i].GetDataID();
                                break;
                            }
                        }
                        if (nCurrentMinDataID - nCount < m_nMinDataID)
                        {
                            nCount = nCurrentMinDataID - m_nMinDataID;
                        }

                        if (nCount > 0)
                        {
                            m_eState = EState.PrepareLoadingUp;
                        }
                    }
                }
            }
            else if (m_eState == EState.FouceBottom)
            {
                if (verticalNormalizedPosition > 0.0f && data.delta.y < 0)
                {
                    m_eState = EState.Normal;
                }
            }

            base.OnDrag(data);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            m_bDragging = false;
            if (m_eState == EState.PrepareLoadingDown)
            {
                if (m_arrScrollItems.Count > 0)
                {
                    int nCount = dynamicMainScrollItemCount * subScrollItemCount;
                    int nCurrentMaxDataID = 0;
                    for (int i = m_arrScrollItems.Count - 1; i >= 0; --i)
                    {
                        if (m_arrScrollItems[i].IsActive())
                        {
                            nCurrentMaxDataID = m_arrScrollItems[i].GetDataID();
                            break;
                        }
                    }

                    if (nCurrentMaxDataID + nCount > m_nMaxDataID)
                    {
                        nCount = m_nMaxDataID - nCurrentMaxDataID;
                    }

                    if (nCount > 0)
                    {
                        m_eState = EState.LoadingDown;
                        StartCoroutine(_LoadScrollItemsDownDynamic(nCurrentMaxDataID, nCount));
                    }
                }
            }
            else if (m_eState == EState.PrepareLoadingUp)
            {
                if (m_arrScrollItems.Count > 0)
                {
                    int nCount = dynamicMainScrollItemCount * subScrollItemCount;
                    int nCurrentMinDataID = 0;
                    for (int i = 0; i < m_arrScrollItems.Count; ++i)
                    {
                        if (m_arrScrollItems[i].IsActive())
                        {
                            nCurrentMinDataID = m_arrScrollItems[i].GetDataID();
                            break;
                        }
                    }
                    if (nCurrentMinDataID - nCount < m_nMinDataID)
                    {
                        nCount = nCurrentMinDataID - m_nMinDataID;
                    }

                    if (nCount > 0)
                    {
                        m_eState = EState.LoadingUp;
                        StartCoroutine(_LoadScrollItemsUpDynamic(nCurrentMinDataID, nCount));
                    }
                }
            }

            base.OnEndDrag(eventData);
        }

        public void SetTemplate(ScrollItem a_template)
        {
            if (m_template != a_template)
            {
                if (m_template != null)
                {
                    m_template.Destroy();
                    m_template = null;
                }
                ClearScrollItems(true);

                m_template = a_template;
                m_template.SetActive(false);
            }
        }

        public void SetDataRange(int a_nMinDataID, int a_nMaxDataID)
        {
            m_nMinDataID = a_nMinDataID;
            m_nMaxDataID = a_nMaxDataID;

            _TryForceBottom();
        }

        void _TryForceBottom()
        {
            if (m_eState == EState.FouceBottom)
            {
                int nCurrentMaxDataID = 0;
                for (int i = m_arrScrollItems.Count - 1; i >= 0; --i)
                {
                    if (m_arrScrollItems[i].IsActive())
                    {
                        nCurrentMaxDataID = m_arrScrollItems[i].GetDataID();
                        break;
                    }
                }

                int nCount = m_nMaxDataID - nCurrentMaxDataID;
                if (nCount > 0)
                {
                    m_eState = EState.LoadingDown;
                    StartCoroutine(_LoadScrollItemsDownDynamic(nCurrentMaxDataID, nCount, true));
                }
            }
        }

        public void Clear()
        {
            m_template = null;
            m_nMinDataID = -1;
            m_nMaxDataID = -2;
            ClearScrollItems(true);
        }

        public void ClearScrollItems(bool a_bDestroy = false)
        {
            m_eState = EState.Invalid;
            for (int i = 0; i < m_arrScrollItems.Count; ++i)
            {
                m_arrScrollItems[i].Destroy();
            }
            m_arrScrollItems.Clear();
            StopAllCoroutines();
        }

        public void InitScrollItems()
        {
            StartCoroutine(_InitScrollItems());
        }

        IEnumerator _InitScrollItems()
        {
            if (m_eState == EState.Invalid)
            {
                if (dynamicMainScrollItemCount <= 0)
                {
                    yield break;
                }

                if (subScrollItemCount <= 0)
                {
                    yield break;
                }

                if (m_template == null)
                {
                    yield break;
                }

                m_eState = EState.Normal;

                if (startFouce == EStartFouce.Top)
                {
                    int nCount = dynamicMainScrollItemCount * subScrollItemCount;
                    int nCurrentDataID = m_nMinDataID;
                    if (nCurrentDataID + nCount - 1 > m_nMaxDataID)
                    {
                        nCount = m_nMaxDataID - nCurrentDataID + 1;
                    }
                    int nRealCount = nCount;
                    if (nCount % subScrollItemCount != 0)
                    {
                        nCount += (subScrollItemCount - nCount % subScrollItemCount);
                    }

                    if (nCount <= 0)
                    {
                        yield break;
                    }

                    for (int i = 0; i < m_arrScrollItems.Count; ++i)
                    {
                        m_arrScrollItems[i].SetActive(false);
                    }
                    yield return Yielders.EndOfFrame;

                    for (int i = 0; i < nCount; ++i)
                    {
                        ScrollItem scrollItem;
                        if (i < m_arrScrollItems.Count)
                        {
                            scrollItem = m_arrScrollItems[i];
                        }
                        else
                        {
                            scrollItem = m_template.Clone();
                            scrollItem.SetActive(false);
                            scrollItem.SetAsLastSibling();
                            m_arrScrollItems.Add(scrollItem);
                        }
                        scrollItem.Setup(nCurrentDataID);
                        nCurrentDataID++;

                        yield return Yielders.EndOfFrame;
                    }

                    for (int i = 0; i < m_arrScrollItems.Count && i < nRealCount; ++i)
                    {
                        m_arrScrollItems[i].SetActive(true);
                    }

                    verticalNormalizedPosition = 1.0f;
                }
                else
                {
                    int nCount = dynamicMainScrollItemCount * subScrollItemCount;
                    int nCurrentDataID = m_nMaxDataID;
                    if (nCurrentDataID - nCount + 1 < m_nMinDataID)
                    {
                        nCount = nCurrentDataID - m_nMinDataID + 1;
                    }
                    int nRealCount = nCount;
                    if (nCount % subScrollItemCount != 0)
                    {
                        nCount += (subScrollItemCount - nCount % subScrollItemCount);
                    }

                    if (nCount <= 0)
                    {
                        yield break;
                    }

                    for (int i = 0; i < m_arrScrollItems.Count; ++i)
                    {
                        m_arrScrollItems[i].SetActive(false);
                    }
                    yield return Yielders.EndOfFrame;

                    int nIndex = m_arrScrollItems.Count - 1;
                    for (int i = 0; i < nCount; ++i)
                    {
                        ScrollItem scrollItem;
                        if (nIndex >= 0)
                        {
                            scrollItem = m_arrScrollItems[nIndex];
                            nIndex--;
                        }
                        else
                        {
                            scrollItem = m_template.Clone();
                            scrollItem.SetActive(false);
                            scrollItem.SetAsFirstSibling();
                            m_arrScrollItems.Insert(0, scrollItem);
                        }

                        scrollItem.Setup(nCurrentDataID);
                        nCurrentDataID--;

                        yield return Yielders.EndOfFrame;
                    }

                    for (int i = 0; i < m_arrScrollItems.Count && i < nRealCount; ++i)
                    {
                        m_arrScrollItems[i].SetActive(true);
                    }

                    verticalNormalizedPosition = 0.0f;

                    m_eState = EState.FouceBottom;
                }
            }
        }

        IEnumerator _LoadScrollItemsDownDynamic(int a_nCurrentMaxDataID, int a_nCount, bool a_bForceBottom = false)
        {
            if (m_eState == EState.LoadingDown)
            {
                ScrollItem scrollItem;

                int nMainScrollItemCount = a_nCount / subScrollItemCount;
                if (a_nCount % subScrollItemCount != 0)
                {
                    nMainScrollItemCount++;
                }

                // 多帧创建、调整元素
                {
                    for (int i = 0; i < nMainScrollItemCount; ++i)
                    {
                        if (m_arrScrollItems.Count / subScrollItemCount >= mainScrollItemCount)
                        {
                            for (int j = 0; j < subScrollItemCount; ++j)
                            {
                                scrollItem = m_arrScrollItems[0];
                                m_arrScrollItems.RemoveAt(0);
                                m_arrScrollItems.Add(scrollItem);
                                scrollItem.SetActive(false);
                                scrollItem.SetAsLastSibling();
                            }
                            //yield return Yielders.EndOfFrame;
                        }
                        else
                        {
                            for (int j = 0; j < subScrollItemCount; ++j)
                            {
                                scrollItem = m_template.Clone();
                                m_arrScrollItems.Add(scrollItem);
                                scrollItem.SetActive(false);
                                scrollItem.SetAsLastSibling();

                                if (j % 2 == 0)
                                {
                                    yield return Yielders.EndOfFrame;
                                }
                            }
                        }

                    }
                    yield return Yielders.EndOfFrame;
                }
                
                // 多帧刷新
                {
                    int nStartIndex = m_arrScrollItems.Count / subScrollItemCount - nMainScrollItemCount;
                    int nDataID = a_nCurrentMaxDataID;
                    int nCount = 0;
                    for (int i = 0; i < nMainScrollItemCount; ++i)
                    {
                        for (int j = 0; j < subScrollItemCount; ++j)
                        {
                            scrollItem = m_arrScrollItems[(nStartIndex + i) * subScrollItemCount + j];
                            nDataID++;
                            nCount++;
                            if (nCount <= a_nCount)
                            {
                                scrollItem.Setup(nDataID);

                                if (j % 2 == 0)
                                {
                                    yield return Yielders.EndOfFrame;
                                }
                            }
                        }
                    }
                    yield return Yielders.EndOfFrame;
                }

                // 单帧统一显示
                {

                    int nStartIndex = m_arrScrollItems.Count / subScrollItemCount - nMainScrollItemCount;
                    int nCount = 0;
                    for (int i = 0; i < nMainScrollItemCount; ++i)
                    {
                        for (int j = 0; j < subScrollItemCount; ++j)
                        {
                            scrollItem = m_arrScrollItems[(nStartIndex + i) * subScrollItemCount + j];
                            nCount++;
                            if (nCount <= a_nCount)
                            {
                                scrollItem.SetActive(true);
                            }
                        }
                    }


//                     LayoutRebuilder.ForceRebuildLayoutImmediate(content);
// 
//                     ScrollItem forceScrollItem = _GetScrollItem(a_nCurrentMaxDataID);
//                     if (forceScrollItem != null)
//                     {
//                         float fOffset = content.rect.height - Mathf.Abs(forceScrollItem.GetPosInContent().y);
//                         verticalNormalizedPosition = 1.0f - fOffset / (content.rect.height - viewport.rect.height);
//                     }
//                     else
//                     {
//                         verticalNormalizedPosition = 1.0f;
//                     }

                    yield return Yielders.EndOfFrame;
                }

                if (a_bForceBottom)
                {
                    m_eState = EState.FouceBottom;
                }
                else
                {
                    m_eState = EState.Normal;
                }
            }
        }

        IEnumerator _LoadScrollItemsUpDynamic(int a_nCurrentMinDataID, int a_nCount)
        {
            if (m_eState == EState.LoadingUp)
            {
                ScrollItem scrollItem;

                int nMainScrollItemCount = a_nCount / subScrollItemCount;
                if (a_nCount % subScrollItemCount != 0)
                {
                    nMainScrollItemCount++;
                }

                // 多帧创建、调整元素
                {
                    for (int i = 0; i < nMainScrollItemCount; ++i)
                    {
                        if (m_arrScrollItems.Count / subScrollItemCount >= mainScrollItemCount)
                        {
                            for (int j = 0; j < subScrollItemCount; ++j)
                            {
                                scrollItem = m_arrScrollItems[m_arrScrollItems.Count - 1];
                                m_arrScrollItems.RemoveAt(m_arrScrollItems.Count - 1);
                                m_arrScrollItems.Insert(0, scrollItem);
                                scrollItem.SetActive(false);
                                scrollItem.SetAsFirstSibling();
                            }
                            //yield return Yielders.EndOfFrame;
                        }
                        else
                        {
                            for (int j = 0; j < subScrollItemCount; ++j)
                            {
                                scrollItem = m_template.Clone();
                                m_arrScrollItems.Insert(0, scrollItem);
                                scrollItem.SetActive(false);
                                scrollItem.SetAsFirstSibling();

                                if (j % 2 == 0)
                                {
                                    yield return Yielders.EndOfFrame;
                                }
                            }
                        }

                    }
                    yield return Yielders.EndOfFrame;
                }

                // 多帧刷新
                {
                    int nDataID = a_nCurrentMinDataID;
                    int nCount = 0;
                    for (int i = nMainScrollItemCount - 1; i >= 0; --i)
                    {
                        for (int j = subScrollItemCount - 1; j >= 0; --j)
                        {
                            scrollItem = m_arrScrollItems[i * subScrollItemCount + j];
                            nDataID--;
                            nCount++;
                            if (nCount <= a_nCount)
                            {
                                scrollItem.Setup(nDataID);

                                if (j % 2 == 0)
                                {
                                    yield return Yielders.EndOfFrame;
                                }
                            }
                        }
                    }
                    yield return Yielders.EndOfFrame;
                }

                // 单帧统一显示
                {
                    float fOldPos = 0.0f;
                    ScrollItem forceScrollItem = _GetScrollItem(a_nCurrentMinDataID);
                    if (forceScrollItem != null)
                    {
                        fOldPos = forceScrollItem.GetPosInContent().y;
                    }
                    
                    int nCount = 0;
                    for (int i = nMainScrollItemCount - 1; i >= 0; --i)
                    {
                        for (int j = subScrollItemCount - 1; j >= 0; --j)
                        {
                            scrollItem = m_arrScrollItems[i * subScrollItemCount + j];
                            nCount++;
                            if (nCount <= a_nCount)
                            {
                                scrollItem.SetActive(true);
                            }
                        }
                    }
                    LayoutRebuilder.ForceRebuildLayoutImmediate(content);

                    if (forceScrollItem != null)
                    {
                        float fOffset = Mathf.Abs(forceScrollItem.GetPosInContent().y - fOldPos);
                        verticalNormalizedPosition = 1.0f - fOffset / (content.rect.height - viewport.rect.height);
                    }
                    else
                    {
                        verticalNormalizedPosition = 0.0f;
                    }

                    yield return Yielders.EndOfFrame;
                }

                m_eState = EState.Normal;
            }
        }

        ScrollItem _GetScrollItem(int a_nID)
        {
            for (int i = 0; i < m_arrScrollItems.Count; ++i)
            {
                if (m_arrScrollItems[i].GetDataID() == a_nID)
                {
                    return m_arrScrollItems[i];
                }
            }
            return null;
        }
    }
}
