using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scripts.UI;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void CommonTabToggleOnClick(CommonTabData tabData);

    [Serializable]
    public class CommonTabData
    {
        public int id; // 自定义数据，方便从组件内部获取某个具体页签数据
        public string tabName;
        public string normalIconPath;
        public string checkIconPath;
    }

    public class CommonTabToggleGroup : MonoBehaviour
    {
        [Header("编辑页签列表")]
        [SerializeField] private List<CommonTabData> commonTabDatas = new List<CommonTabData>();

        [SerializeField] private ComUIListScript listScript;

        /// <summary>
        /// 保存所有tabitem key：索引 value：tabItem
        /// </summary>
        private Dictionary<int, ICommonTabToggle> tabItems = new Dictionary<int, ICommonTabToggle>();

        public CommonTabToggleOnClick commonTabToggleOnClick;
        private int SelectedTabId = -1;

        private void Awake()
        {
            tabItems.Clear();
            InitListScript();
        }

        private void OnDestroy()
        {
            UnInitListScript();

            commonTabToggleOnClick = null;
            SelectedTabId = -1;
            tabItems.Clear();
        }

        public void InitComTab(CommonTabToggleOnClick callback, int defaultSelectedId = -1, List<CommonTabData> TabListDatas = null/*动态加载数据时使用,编辑器内静态编辑数据不需要传该参数*/)
        {
            // 设置默认页签索引时先绑定CommonTabToggleOnClick事件
            if (callback != null)
            {
                if(commonTabToggleOnClick != null)
                {
                    commonTabToggleOnClick = null;
                }

                commonTabToggleOnClick = callback;
            }

            if(TabListDatas != null)
            {
                commonTabDatas = TabListDatas;
            }

            if (defaultSelectedId != -1)
            {
                SelectedTabId = defaultSelectedId;
            }
            else
            {
                if(commonTabDatas != null && commonTabDatas.Count > 0)
                {
                    SelectedTabId = commonTabDatas[0].id;
                }
                else
                {
                    Logger.LogError("通用页签组件初始化失败，没有页签数据");
                }
            }

            if (listScript != null && commonTabDatas != null && commonTabDatas.Count > 0)
            {
                listScript.SetElementAmount(commonTabDatas.Count);
            }
        }

        public List<CommonTabData> GetTabDatas()
        {
            List<CommonTabData> tempCommonTabDatas = new List<CommonTabData>();
            if (commonTabDatas == null)
            {
                return tempCommonTabDatas;
            }

            foreach (var v in commonTabDatas)
            {
                tempCommonTabDatas.Add(v);
            }

            return tempCommonTabDatas;
        }

        private void InitListScript()
        {
            if (listScript != null)
            {
                listScript.Initialize();

                listScript.onBindItem += OnBindItemDelegate;
                listScript.onItemVisiable += OnItemVisiableDelegate;
                listScript.OnItemUpdate += OnItemVisiableDelegate;
            }
        }

        private void UnInitListScript()
        {
            if (listScript != null)
            {
                listScript.onBindItem -= OnBindItemDelegate;
                listScript.onItemVisiable -= OnItemVisiableDelegate;
                listScript.OnItemUpdate -= OnItemVisiableDelegate;
            }
        }

        private ICommonTabToggle OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ICommonTabToggle>();
        }

        public virtual void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            ICommonTabToggle comTabItem = item.gameObjectBindScript as ICommonTabToggle;
            if(comTabItem != null && item.m_index >= 0 && item.m_index < commonTabDatas.Count)
            {
                CommonTabData tabData = commonTabDatas[item.m_index];

                if (tabData.id == SelectedTabId)
                {
                    comTabItem.Init(tabData, commonTabToggleOnClick, true, item.m_index, commonTabDatas.Count);
                }
                else
                {
                    comTabItem.Init(tabData, commonTabToggleOnClick, false, item.m_index, commonTabDatas.Count);
                }

                if(tabItems.ContainsKey(tabData.id) == false)
                {
                   tabItems.Add(tabData.id, comTabItem);
                }
                else
                {
                   tabItems[tabData.id] = comTabItem;
                }
            }
        }

        /// <summary>
        /// 根据自定义id获取Toggle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Toggle GetToggle(int id)
        {
            Toggle tog = null;
            ICommonTabToggle tabToggle = null;
            if(tabItems.TryGetValue(id, out tabToggle))
            {
                tog = tabToggle.GetTog();
            }

            return tog;
        }

        public void OnSetRedPoint(int id, bool isFlag)
        {
            ICommonTabToggle tabToggle = null;
            if (tabItems.TryGetValue(id, out tabToggle))
            {
                tabToggle.OnSetRedPoint(isFlag);
            }
        }

        public void CustomActive(int id, bool isFlag)
        {
            Toggle tog = GetToggle(id);
            if (tog != null)
            {
                tog.CustomActive(isFlag);
            }
        }

        public void SelectTabId(int id)
        {
            foreach (var data in commonTabDatas)
            {
                if (data.id == id)
                {
                    SelectedTabId = id;
                    listScript.UpdateElement();
                    return;
                }
            }
        }

        public void SelectTabIndex(int index)
        {
            if (index >= 0 && index <= commonTabDatas.Count)
            {
                SelectedTabId = commonTabDatas[index].id;
                listScript.UpdateElement();
            }
        }
    }
}