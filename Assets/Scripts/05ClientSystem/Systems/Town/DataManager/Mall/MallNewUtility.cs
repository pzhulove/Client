using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //商城助手类
    public static class MallNewUtility
    {

        ////主页签的数据(道具，材料，功能，兑换，药品，金币)
        //public static List<MallNewPropertyMallTabData> GetPropertyMallTabDataModelList()
        //{
        //    List<MallNewPropertyMallTabData> propertyMallTabDataModelList = new List<MallNewPropertyMallTabData>();

        //    //道具
        //    var curTabDataModel = new MallNewPropertyMallTabData
        //    {
        //        Index = 0,
        //        PropertyMallTabType = MallNewPropertyMallTabType.Item,
        //        MallTypeTableId = 17,
        //        SortId = 0,
        //    };
        //    propertyMallTabDataModelList.Add(curTabDataModel);

        //    //功能
        //    curTabDataModel = new MallNewPropertyMallTabData
        //    {
        //        Index = 1,
        //        PropertyMallTabType = MallNewPropertyMallTabType.Function,
        //        MallTypeTableId = 18,
        //        SortId = 2,
        //    };
        //    propertyMallTabDataModelList.Add(curTabDataModel);

        //    //兑换
        //    curTabDataModel = new MallNewPropertyMallTabData
        //    {
        //        Index = 2,
        //        PropertyMallTabType = MallNewPropertyMallTabType.Exchange,
        //        MallTypeTableId = 19,
        //        SortId = 3,
        //    };
        //    propertyMallTabDataModelList.Add(curTabDataModel);

        //    //药品
        //    curTabDataModel = new MallNewPropertyMallTabData
        //    {
        //        Index = 3,
        //        PropertyMallTabType = MallNewPropertyMallTabType.Medicine,
        //        MallTypeTableId = 20,
        //        SortId = 4,
        //    };
        //    propertyMallTabDataModelList.Add(curTabDataModel);

        //    //金币
        //    curTabDataModel = new MallNewPropertyMallTabData
        //    {
        //        Index = 4,
        //        PropertyMallTabType = MallNewPropertyMallTabType.Gold,
        //        MallTypeTableId = 5,
        //        SortId = 5,
        //    };
        //    propertyMallTabDataModelList.Add(curTabDataModel);

        //    //材料
        //    curTabDataModel = new MallNewPropertyMallTabData
        //    {
        //        Index = 5,
        //        PropertyMallTabType = MallNewPropertyMallTabType.Material,
        //        MallTypeTableId = 14,
        //        SortId = 1,
        //    };
        //    propertyMallTabDataModelList.Add(curTabDataModel);

        //    //按照sortId，从小到大进行排序
        //    propertyMallTabDataModelList.Sort((x, y) => x.SortId.CompareTo(y.SortId));

        //    return propertyMallTabDataModelList;
        //}

        /// <summary>
        /// 积分商城页签（道具）
        /// </summary>
        /// <returns></returns>
        public static List<MallNewIntergralMallTabData> GetIntergralMallTabDataModelList()
        {
            List<MallNewIntergralMallTabData> intergralMallTabModelList = new List<MallNewIntergralMallTabData>();

            var tabDataModel = new MallNewIntergralMallTabData
            {
                index = 0,
                mallTypeTableId = 22
            };

            intergralMallTabModelList.Add(tabDataModel);

            return intergralMallTabModelList;
        }

        /// <summary>
        /// 得到积分商城积分上限
        /// </summary>
        /// <returns></returns>
        public static int GetIntergralMallTicketUpper()
        {
            var system = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_MALL_INTEGRAL_NUM);
            if (system == null)
            {
                return 0;
            }

            return system.Value;
        }

        /// <summary>
        /// 点券转化积分系数
        /// </summary>
        /// <returns></returns>
        public static int GetMallTicketConvertIntergalRate()
        {
            var system = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_TICKET_CONVERT_MALL_INTEGRAL_RATE);
            if (system == null)
            {
                return 0;
            }

            return system.Value;
        }

        /// <summary>
        /// 得到点券转化积分数量
        /// </summary>
        /// <param name="ticket">点券数量</param>
        /// <returns></returns>
        public static int GetTicketConvertIntergalNumnber(int ticket)
        {
            return ticket * GetMallTicketConvertIntergalRate();
        }

        /// <summary>
        /// 购买道具积分商城积分弹框
        /// </summary>
        /// <param name="onToggleClick"></param>
        /// <param name="onRureClick"></param>
        public static void CommonIntergralMallPopupWindow(string content, OnCommonMsgBoxToggleClick onToggleClick, Action onRureClick)
        {
            CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
            comconMsgBoxOkCancelParamData.ContentLabel = content;
            comconMsgBoxOkCancelParamData.IsShowNotify = true;
            comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = onToggleClick;
            comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
            comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure");
            comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = onRureClick;

            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(comconMsgBoxOkCancelParamData);
        }
        
        public static void ItemMallIntergralMallScoreIsEqual(bool value)
        {
            MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual = value;
        }

        public static void ItemMallIntergralMallScoreIsExceed(bool value)
        {
            MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed = value;
        }
    }
}