using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    [Serializable]
    public class MallNewMainTabDataModel
    {
        // public int index;
        public MallNewType mainTabType;
        public string mainTabName;
        public GameObject contentRoot;
        // public bool isHaveEffect;
        public List<int> titleItemList = new List<int>();
    }

    public class MallNewView : MonoBehaviour
    {

        [Space(5)] [HeaderAttribute("MainTabDataModelList")] 
        [SerializeField] private List<MallNewMainTabDataModel> mainTabsDataModelList = new List<MallNewMainTabDataModel>();

        [Space(15)]
        [SerializeField] private ComUIListScript mainTabsList = null;


        [SerializeField] private List<ComCommonConsume> titleConsumeList;
        [SerializeField] private GameObject mObjMallRoot;
        [SerializeField] private List<int> mRechargeCoinList;

        //初始化
        public void InitView()
        {
            SpecialIOSInit();
            BindUiEventSystem();
            if (mainTabsList != null)
            {
                mainTabsList.SetElementAmount(mainTabsDataModelList.Count);
            }
            if ((int)MallNewType.ReChargeMall == MallNewFrame.DefaultMainTabIndex)
            {
                OnClickRecharge();
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        // 苹果商城特殊处理 可能不开启限时商城
        private void SpecialIOSInit()
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
            {
                if (mainTabsDataModelList == null)
                {
                    return;
                }
                MallNewMainTabDataModel limitTimeModel = null;
                int modelCount = mainTabsDataModelList.Count;
                for (int i = 0; i < modelCount; i++)
                {
                    var model = mainTabsDataModelList[i];
                    if (model == null)
                    {
                        continue;
                    }
                    if (model.mainTabType == MallNewType.LimitTimeMall)
                    {
                        limitTimeModel = model;
                        break;
                    }
                }
                if (limitTimeModel != null)
                {
                    mainTabsDataModelList.Remove(limitTimeModel);
                }
            }
#endif
        }

        // 绑定事件
        private void BindUiEventSystem()
        {
            if (mainTabsList != null)
            {
                mainTabsList.Initialize();
                mainTabsList.onItemVisiable += OnMainTabItemVisible;
            }
        }

        // 解除绑定事件
        private void UnBindUiEventSystem()
        {
            if (mainTabsList != null)
            {
                mainTabsList.onItemVisiable -= OnMainTabItemVisible;
            }
        }

        // 左侧页签显示
        private void OnMainTabItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            var tabItem = item.GetComponent<MallNewMainTabItem>();
            if(tabItem == null)
                return;

            if (mainTabsDataModelList != null
                && item.m_index >= 0 && item.m_index < mainTabsDataModelList.Count)
            {
                var mainTabDataModel = mainTabsDataModelList[item.m_index];
                if (mainTabDataModel != null)
                {
                    //默认选中的触发事件
                    tabItem.Init(mainTabDataModel, OnMallTabClick, (int)mainTabDataModel.mainTabType == MallNewFrame.DefaultMainTabIndex);
                }
            }
        }

        // 点击页签事件
        private void OnMallTabClick(MallNewMainTabDataModel mainTabData)
        {
            //修改title的货币显示 积分商城和其他商城显示不一样
            _SetTitleCoin(mainTabData.titleItemList);
            //隐藏充值页面
            mObjMallRoot.CustomActive(true);
        }

        //设置标题货币
        private void _SetTitleCoin(List<int> itemIdList)
        {
            if (null  == itemIdList)
                return;
            int index = 0;
            for(; index < itemIdList.Count; ++index)
            {
                if (titleConsumeList.Count <= index)
                {
                    Logger.LogError("商城预制体title的显示道具预制体不足");
                    return;
                }
                titleConsumeList[index].CustomActive(true);
                //道具相等时不需要初始化
                if (titleConsumeList[index].mItemID != itemIdList[index])
                    titleConsumeList[index].SetData(ComCommonConsume.eType.Item, ComCommonConsume.eCountType.Fatigue, itemIdList[index]);
            }
            //多余的不显示
            for(; index < titleConsumeList.Count; ++index)
            {
                titleConsumeList[index].CustomActive(false);
            }
        }

        //点击充值
        public void OnClickRecharge()
        {
            mObjMallRoot.CustomActive(false);
            _SetTitleCoin(mRechargeCoinList);
            if (null != MallNewMainTabItem.selectItem)
                MallNewMainTabItem.selectItem.SetOffToggle();
            //充值类型，直接打开重置界面
            MallNewFrame.OpenMallPayFrame();
        }

        //关闭商城
        public void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<MallNewFrame>();
        }

    }
}
