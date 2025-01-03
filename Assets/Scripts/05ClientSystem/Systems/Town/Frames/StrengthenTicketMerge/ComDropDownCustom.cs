using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComDropDownCustom : MonoBehaviour
    {
        
        #region Model Params

        public delegate void OnDropDownSelectIndex(int index);
        public OnDropDownSelectIndex onSelectIndex;

        public delegate void OnDisabledHandle();
        public OnDisabledHandle onDisabledHandle;

        private bool bExpand = false;
        public bool BExpand { get { return bExpand; } }

        private bool bEnabled = true;
        public bool BEnabled { get { return bEnabled; } }

        private List<ComDropDownItemCustom> m_ChildItems;
        private bool bInited = false;
        private int currIndexCount = 0;

        #endregion
        
        #region View Params

        [SerializeField]
        private Button m_ShowBtn;
        [SerializeField]
        private Text m_ShowText;
        [SerializeField]
        private GameObject m_ChildRoot;
        [SerializeField]
        private ComDropDownItemCustom m_ChildItemTemplete;
        [SerializeField]
        private GameObject m_ExtendDesc;
        [SerializeField]
        private ToggleGroup m_ChildToggleGroup;

        [SerializeField]
        [Header("初始index")]
        private int m_PreIndex = 1;
        [SerializeField]
        [Header("index顺序相反")]
        private bool b_IndexReverse = false;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Start () 
        {
            if (m_ShowBtn)
            {
                m_ShowBtn.onClick.AddListener(_OnSelectBtnClick);
            }
        }
        
        //Unity life cycle
        void OnDestroy () 
        {
            if (m_ShowBtn)
            {
                m_ShowBtn.onClick.RemoveListener(_OnSelectBtnClick);
            }
            Clear();
        }

        void _OnSelectBtnClick()
        {
            //不可用时
            if (!bEnabled)
            {
                if (onDisabledHandle != null)
                {
                    onDisabledHandle();
                }
                return;
            }
            if (m_ChildRoot == null)
            {
                return;
            }
            if (m_ChildRoot.activeSelf)
            {
                _SetChildExpand(false);
            }
            else
            {
                _SetChildExpand(true);
            }
        }
        
        void _SetChildExpand(bool expand)
        {
            bExpand = expand;
            m_ChildRoot.CustomActive(expand);
            m_ExtendDesc.CustomActive(!expand);
        }

        void _SetShowText(string text)
        {
            if (m_ShowText)
            {
                m_ShowText.text = text;
            }
        }

        #endregion
        
        #region  PUBLIC METHODS

        public void Init(List<string> showTextList)
        {
            if (bInited)
            {
                return;
            }
            if (showTextList == null || showTextList.Count == 0
                || m_ChildItemTemplete == null )
            {
                return;
            }
            List<string> tempTextList = new List<string>();
            tempTextList.AddRange(showTextList);
            if (m_ChildItems == null)
            {
                m_ChildItems = new List<ComDropDownItemCustom>();
            }
            if (tempTextList.Count > m_ChildItems.Count)
            {
                int newCount = tempTextList.Count - m_ChildItems.Count;
                for (int i = 0; i < newCount; i++)
                {
                    GameObject go = GameObject.Instantiate(m_ChildItemTemplete.gameObject);
                    if (go == null) continue;
                    Utility.AttachTo(go, m_ChildRoot);
                    go.transform.SetAsLastSibling();                   
                    ComDropDownItemCustom item = go.GetComponent<ComDropDownItemCustom>();
                    if (item == null || item.GetToggle() == null) continue;
                    item.GetToggle().group = m_ChildToggleGroup;
                    item.SetToggleOff();
                    m_ChildItems.Add(item);
                }
            }
            else if (tempTextList.Count < m_ChildItems.Count)
            {
                for (int i = tempTextList.Count; i < m_ChildItems.Count; i++)
                {
                    if (m_ChildItems.Contains(m_ChildItems[i]))
                    {
                        m_ChildItems[i].CustomActive(false);
                    }
                }
            }
            //
            currIndexCount = tempTextList.Count;
            if (b_IndexReverse)
            {
                tempTextList.Reverse();
                for (int i = tempTextList.Count - 1; i >= 0; i--)
                {
                    if (i >= m_ChildItems.Count) continue;
                    var child = m_ChildItems[i];
                    if (child == null) continue;
                    child.Init(tempTextList.Count - i, tempTextList[i]);
                    child.onToggleClick = (isOn, index) =>
                    {
                        if (isOn)
                        {
                            _SetChildExpand(false);
                            _SetShowText(child.GetToggleDesc());
                            if (onSelectIndex != null)
                            {
                                onSelectIndex(index);
                            }
                        }
                    };
                    child.gameObject.CustomActive(true);
                }
            }
            else
            {
                for (int i = 0; i < tempTextList.Count; i++)
                {
                    if (i >= m_ChildItems.Count) continue;
                    var child = m_ChildItems[i];
                    if (child == null) continue;
                    child.Init(m_PreIndex + i, tempTextList[i]);
                    child.onToggleClick = (isOn, index) =>
                    {
                        if (isOn)
                        {
                            _SetChildExpand(false);
                            _SetShowText(child.GetToggleDesc());
                            if (onSelectIndex != null)
                            {
                                onSelectIndex(index);
                            }
                        }
                    };
                    child.gameObject.CustomActive(true);
                }
            }
            _SetChildExpand(false);
            m_ChildItemTemplete.CustomActive(false);
            bInited = true;
        }

        public void Clear()
        {
            onSelectIndex = null;
            onDisabledHandle = null;

            bExpand = false;
            m_ChildRoot.CustomActive(false);

            if (m_ChildItems != null)
            {
                for (int i = 0; i < m_ChildItems.Count; i++)
                {
                    var child = m_ChildItems[i];
                    if (child == null)
                        continue;
                    child.Clear();
                }
            }
            bInited = false;
            currIndexCount = 0;
        }

        public void ResetFirstIndex()
        {
            if (m_ChildItems == null || m_ChildItems.Count == 0)
            {
                return;
            }
            if (b_IndexReverse)
            {
                if (m_ChildItems.Count >= currIndexCount)
                {
                    m_ChildItems[currIndexCount - 1].SetToggleOn();
                    _SetShowText(m_ChildItems[currIndexCount - 1].GetToggleDesc());
                }
            }
            else
            {
                m_ChildItems[0].SetToggleOn();
                _SetShowText(m_ChildItems[0].GetToggleDesc());
            }
        }

        public int GetFirstIndex()
        {
            return m_PreIndex;
        }

        public void SetEnable(bool enabled)
        {
            this.bEnabled = enabled;
        }

        #endregion
    }
}