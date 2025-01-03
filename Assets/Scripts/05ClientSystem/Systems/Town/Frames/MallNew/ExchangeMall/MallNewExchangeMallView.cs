using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    //兑换商城View
    public class MallNewExchangeMallView : MallNewBaseView
    {
        private List<ShopTable> mExchangeMallShopTableList = new List<ShopTable>();

        [SerializeField] private ComUIListScript exchangeMallElementList = null;

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (exchangeMallElementList != null)
            {
                exchangeMallElementList.Initialize();
                exchangeMallElementList.onItemVisiable += OnExchangeMallItemVisible;
                exchangeMallElementList.onItemSelected += OnExchangeMallItemSelected;
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            if (exchangeMallElementList != null)
            {
                exchangeMallElementList.onItemVisiable -= OnExchangeMallItemVisible;
                exchangeMallElementList.onItemSelected -= OnExchangeMallItemSelected;
            }
        }

        //初始化
        public override void InitData(int index, int secondIndex = 0, int thirdIndex = 0)
        {
            InitExchangeMallData();
            InitExchangeMallView();
        }

        //初始化商城数据
        private void InitExchangeMallData()
        {
            if (null != mExchangeMallShopTableList)
                mExchangeMallShopTableList.Clear();
            var shopTables = TableManager.GetInstance().GetTable<ShopTable>();
            if (shopTables != null)
            {
                var iter = shopTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var shopTableItem = iter.Current.Value as ShopTable;
                    //添加需要展示的兑换商店
                    if (shopTableItem != null && shopTableItem.IsExchangeShopShow == 1)
                    {
                        //活动商店，并且不处在开启状态，直接跳出，不添加
                        if (ShopNewUtility.IsActivityShop(shopTableItem) && !ShopNewUtility.IsActivityShopInStartState(shopTableItem))
                        {
                            continue;
                        }
#if APPLE_STORE
                        //IOS屏蔽功能 积分魔罐商店
                        if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR) &&
                            shopTableItem.ShopKind == ShopTable.eShopKind.SK_Magic)
                        {
                            continue;
                        }
#endif
                        //非活动商店 或 活动商店并且处在开启的状态
                        mExchangeMallShopTableList.Add(shopTableItem);
                    }
                }
            }
            mExchangeMallShopTableList.Sort((x,y) => x.ExchangeShopOrder.CompareTo(y.ExchangeShopOrder));
        }

        //显示列表
        private void InitExchangeMallView()
        {
            if (exchangeMallElementList != null)
            {
                exchangeMallElementList.SetElementAmount(mExchangeMallShopTableList.Count);
                exchangeMallElementList.ResetContentPosition();
            }
        }

        private void OnExchangeMallItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(item.m_index < 0 || item.m_index >= mExchangeMallShopTableList.Count)
                return;

            var shopTable = mExchangeMallShopTableList[item.m_index];
            var shopElementItem = item.GetComponent<MallNewExchangeMallElementItem>();

            if (shopTable != null && shopElementItem != null)
            {
                shopElementItem.InitData(shopTable);
            }
        }

        private void OnExchangeMallItemSelected(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(item.m_index < 0 || item.m_index >= mExchangeMallShopTableList.Count)
                return;

            var shopTable = mExchangeMallShopTableList[item.m_index];
            var shopElementItem = item.GetComponent<MallNewExchangeMallElementItem>();

            if (shopTable != null && shopElementItem != null)
            {
                shopElementItem.OnMallButtonClicked();
            }
        }

    }
}