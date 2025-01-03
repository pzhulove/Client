using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scripts.UI;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void CommonBriefTabToggleOnClick(CommonBriefTabData tabData);

    [Serializable]
    public class CommonBriefTabData
    {
        public int id; // 自定义数据，方便从组件内部获取某个具体页签数据
        public string tabName;
    }

    public class CommonBriefTabToggleGroup : MonoBehaviour
    {
        [Header("编辑页签列表")]
        [SerializeField] private List<CommonBriefTabData> commonBriefTabDatas = new List<CommonBriefTabData>();

        [SerializeField] private ComUIListScript listScript;

        /// <summary>
        /// 保存所有tabitem key：索引 value：tabItem
        /// </summary>
        private Dictionary<int, CommonBriefTabToggleItem> tabItems = new Dictionary<int, CommonBriefTabToggleItem>();
        private Dictionary<int, bool> dicRedPoint = new Dictionary<int, bool>();

        public CommonBriefTabToggleOnClick commonTabToggleOnClick;
        private int SelectedTabId = -1;

        private void Awake()
        {
            tabItems.Clear();
            dicRedPoint.Clear();
            InitListScript();
        }

        private void OnDestroy()
        {
            UnInitListScript();

            commonTabToggleOnClick = null;
            SelectedTabId = -1;
            tabItems.Clear();
            dicRedPoint.Clear();
        }

        public void InitComTab(CommonBriefTabToggleOnClick callback, int defaultSelectedId = -1, List<CommonBriefTabData> TabListDatas = null/*动态加载数据时使用,编辑器内静态编辑数据不需要传该参数*/)
        {
            // 设置默认页签索引时先绑定CommonTabBriefToggleOnClick事件
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
                commonBriefTabDatas = TabListDatas;

                foreach (var data in commonBriefTabDatas)
                {
                    dicRedPoint.SafeAdd(data.id, false);
                }
            }

            if (defaultSelectedId != -1)
            {
                SelectedTabId = defaultSelectedId;
            }
            else
            {
                if(commonBriefTabDatas != null && commonBriefTabDatas.Count > 0)
                {
                    SelectedTabId = commonBriefTabDatas[0].id;
                }
                else
                {
                    Logger.LogError("通用页签组件初始化失败，没有页签数据");
                }
            }

            if (listScript != null && commonBriefTabDatas != null && commonBriefTabDatas.Count > 0)
            {
                listScript.SetElementAmount(commonBriefTabDatas.Count);

                int index = GetIndexById(SelectedTabId);
                if (index < 0)
                {
                    index = 0;
                }
                listScript.SelectElement(index);
            }
        }

        /// <summary>
        /// 列表中所有tab都取消选中
        /// </summary>
        public void UnSelectList()
        {
            if (listScript == null)
            {
                return;
            }

            listScript.SelectElement(-1);
            SelectedTabId = -1;
        }

        public int GetIndexById(int id)
        {
            if (commonBriefTabDatas == null)
            {
                return -1;
            }

            for (int i = 0; i < commonBriefTabDatas.Count; i++)
            {
                if (commonBriefTabDatas[i] == null)
                {
                    continue;
                }

                if (commonBriefTabDatas[i].id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        public List<CommonBriefTabData> GetTabDatas()
        {
            List<CommonBriefTabData> tempCommonBriefTabDatas = new List<CommonBriefTabData>();
            if (commonBriefTabDatas == null)
            {
                return tempCommonBriefTabDatas;
            }

            foreach (var v in commonBriefTabDatas)
            {
                tempCommonBriefTabDatas.Add(v);
            }

            return tempCommonBriefTabDatas;
        }

        private void InitListScript()
        {
            if (listScript != null)
            {
                listScript.Initialize();

                listScript.onBindItem += OnBindItemDelegate;
                listScript.onItemVisiable += OnItemVisiableDelegate;
                listScript.OnItemUpdate += OnItemVisiableDelegate;
                listScript.onItemSelected += _OnItemSelect;
                listScript.onItemChageDisplay += _OnItemChangeDisplay;
            }
        }

        private void UnInitListScript()
        {
            if (listScript != null)
            {
                listScript.onBindItem -= OnBindItemDelegate;
                listScript.onItemVisiable -= OnItemVisiableDelegate;
                listScript.OnItemUpdate -= OnItemVisiableDelegate;
                listScript.onItemSelected -= _OnItemSelect;
                listScript.onItemChageDisplay -= _OnItemChangeDisplay;
            }
        }

        private CommonBriefTabToggleItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonBriefTabToggleItem>();
        }

        public virtual void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            CommonBriefTabToggleItem comTabItem = item.gameObjectBindScript as CommonBriefTabToggleItem;
            if(comTabItem != null && item.m_index >= 0 && item.m_index < commonBriefTabDatas.Count)
            {
                CommonBriefTabData tabData = commonBriefTabDatas[item.m_index];

                bool isShowRedPoint = false;
                dicRedPoint.TryGetValue(tabData.id, out isShowRedPoint);
                comTabItem.Init(tabData, tabData.id == SelectedTabId, item.m_index, commonBriefTabDatas.Count, isShowRedPoint);

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

        private void _OnItemSelect(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            CommonBriefTabData tabData = commonBriefTabDatas[item.m_index];
            if (tabData != null)
            {
                SelectedTabId = tabData.id;
                commonTabToggleOnClick?.Invoke(tabData);
            }
        }

        private void _OnItemChangeDisplay(ComUIListElementScript item, bool isSelect)
        {
            if (item == null)
            {
                return;
            }

            CommonBriefTabToggleItem comTabItem = item.gameObjectBindScript as CommonBriefTabToggleItem;
            if (comTabItem == null)
            {
                return;
            }

            comTabItem.SelectToggle(isSelect);
        }

        /// <summary>
        /// 根据自定义id获取Toggle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Toggle GetToggle(int id)
        {
            if (listScript == null)
            {
                return null;
            }

            int index = GetIndexById(id);
            ComUIListElementScript tabToggle = listScript.GetElemenet(index);
            if (tabToggle == null)
            {
                return null;
            }

            Toggle tog = tabToggle.GetComponent<Toggle>();

            return tog;
        }

        public void OnSetRedPoint(int id, bool isFlag)
        {
            if (dicRedPoint != null && dicRedPoint.ContainsKey(id))
            {
                dicRedPoint[id] = isFlag;
            }

            if (listScript == null)
            {
                return;
            }

            listScript.UpdateElement();
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
            foreach (var data in commonBriefTabDatas)
            {
                if (data.id == id)
                {
                    SelectedTabId = id;
                    if (listScript != null)
                    {
                        listScript.SelectElement(GetIndexById(id));
                    }
                    break;
                }
            }
        }

        public void SelectTabIndex(int index)
        {
            if (index >= 0 && index <= commonBriefTabDatas.Count)
            {
                SelectedTabId = commonBriefTabDatas[index].id;
                if (listScript != null)
                {
                    listScript.SelectElement(index);
                }
            }
        }
    }
}