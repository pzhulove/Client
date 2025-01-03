using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //荣誉系统助手
    public static class HonorSystemUtility
    {
        //荣誉保护卡使用界面
        public static void OnOpenHonorSystemProtectCardFrame()
        {
            OnCloseHonorSystemProtectCardFrame();

            ClientSystemManager.GetInstance().OpenFrame<HonorSystemProtectCardFrame>(FrameLayer.Middle);
        }

        public static void OnCloseHonorSystemProtectCardFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<HonorSystemProtectCardFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<HonorSystemProtectCardFrame>();
        }

        //荣誉预览界面
        public static void OnOpenHonorSystemPreviewFrame()
        {
            OnCloseHonorSystemPreviewFrame();

            ClientSystemManager.GetInstance().OpenFrame<HonorSystemPreviewFrame>(FrameLayer.Middle);
        }

        public static void OnCloseHonorSystemPreviewFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<HonorSystemPreviewFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<HonorSystemPreviewFrame>();
        }


        public static void SendHonorSystemRedPointUpdateMessage()
        {
            //更新荣誉入口按钮的红点
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveHonorSystemRedPointUpdateMessage);

            //更新主界面背包上的红点
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);
        }

        //是否展示荣耀系统的小红点
        public static bool IsShowHonorSystemRedPoint()
        {
            return HonorSystemDataManager.GetInstance().IsShowRedPointFlag;
        }

        //是否展示荣耀系统
        public static bool IsShowHonorSystem()
        {
            //没有解锁
            if (IsHonorSystemUnLock() == false)
                return false;

            //吃鸡场景中
            if (CommonUtility.IsInGameBattleScene() == true)
                return false;

            return true;
        }

        //荣誉系统是否解锁
        public static bool IsHonorSystemUnLock()
        {
            if (PlayerBaseData.GetInstance().Level >= HonorSystemDataManager.HonorSystemUnLockLevel)
                return true;
            return false;
        }

        //打开荣誉系统
        public static void OnOpenHonorSystemFrame()
        {
            OnCloseHonorSystemFrame();
            ClientSystemManager.GetInstance().OpenFrame<HonorSystemFrame>(FrameLayer.Middle);
        }

        //关闭荣誉系统
        public static void OnCloseHonorSystemFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<HonorSystemFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<HonorSystemFrame>();
        }


        #region CreateHistoryHonorDataModel

        //荣誉信息
        public static PlayerHistoryHonorInfo CreatePlayerHistoryHonorInfoByDateType(HONOR_DATE_TYPE dateType)
        {
            PlayerHistoryHonorInfo playerHistoryHonorInfo = new PlayerHistoryHonorInfo();

            playerHistoryHonorInfo.HonorDateType = dateType;
            playerHistoryHonorInfo.HonorTotalNumber = 0;
            
            return playerHistoryHonorInfo;
        }
        
        public static void AddPvpNumberStatisticsInPlayerHistoryHonorInfo(PlayerHistoryHonorInfo playerHistoryHonorInfo,
            HonorPlayerTable honorPlayerTable)
        {
            if (playerHistoryHonorInfo == null)
                return;

            if (honorPlayerTable == null)
                return;

            if (playerHistoryHonorInfo.PvpNumberStatisticsList == null)
                playerHistoryHonorInfo.PvpNumberStatisticsList = new List<PvpNumberStatistics>();

            var pvpNumberStatistics = CreatePvpNumberStatisticsByHonorPlayerTable(honorPlayerTable);
            if (pvpNumberStatistics != null)
                playerHistoryHonorInfo.PvpNumberStatisticsList.Add(pvpNumberStatistics);
        }

        //更新模型的新标志
        public static void UpdatePlayerNewFlagInHistoryInfo(PlayerHistoryHonorInfo playerHistoryHonorInfo,
            HistoryHonorInfo historyHonorInfo)
        {
            if (playerHistoryHonorInfo == null)
                return;

            if (playerHistoryHonorInfo.PvpNumberStatisticsList == null
                || playerHistoryHonorInfo.PvpNumberStatisticsList.Count <= 0)
                return;

            for (var i = 0; i < playerHistoryHonorInfo.PvpNumberStatisticsList.Count; i++)
            {
                var pvpNumberStatistics = playerHistoryHonorInfo.PvpNumberStatisticsList[i];

                if(pvpNumberStatistics == null)
                    continue;

                //默认为false
                pvpNumberStatistics.IsNewFlag = false;

                if (pvpNumberStatistics.PvpCount == 1)
                {
                    //当前次数是1，判断是否显示新的标志
                    if (historyHonorInfo == null
                        || historyHonorInfo.pvpStatisticsList == null
                        || historyHonorInfo.pvpStatisticsList.Length <= 0)
                    {
                        pvpNumberStatistics.IsNewFlag = true;
                        continue;
                    }

                    bool isFind = false;
                    int count = 0;
                    for (var j = 0; j < historyHonorInfo.pvpStatisticsList.Length; j++)
                    {
                        var pvpStatistics = historyHonorInfo.pvpStatisticsList[j];
                        if (pvpStatistics != null
                            && pvpStatistics.pvpType == pvpNumberStatistics.PvpType)
                        {
                            isFind = true;
                            count = (int)pvpStatistics.pvpCnt;
                            break;
                        }
                    }

                    //没有找到活动，或者上一次的活动次数为0；
                    if (isFind == false
                        || count == 0)
                    {
                        pvpNumberStatistics.IsNewFlag = true;
                    }
                }
                
            }
        }

        //
        public static void UpdatePlayerHistoryInfo(PlayerHistoryHonorInfo playerHistoryHonorInfo,
            HistoryHonorInfo historyHonorInfo)
        {
            if (playerHistoryHonorInfo == null || historyHonorInfo == null)
                return;

            if (playerHistoryHonorInfo.HonorDateType != (HONOR_DATE_TYPE) historyHonorInfo.dateType)
                return;
            //荣誉总的数值
            playerHistoryHonorInfo.HonorTotalNumber = historyHonorInfo.totalHonor;

            if (playerHistoryHonorInfo.PvpNumberStatisticsList == null ||
                playerHistoryHonorInfo.PvpNumberStatisticsList.Count <= 0)
                return;

            if (historyHonorInfo.pvpStatisticsList == null || historyHonorInfo.pvpStatisticsList.Length <= 0)
                return;

            //活动次数赋值
            for (var i = 0; i < historyHonorInfo.pvpStatisticsList.Length; i++)
            {
                var pvpStatistics = historyHonorInfo.pvpStatisticsList[i];
                if(pvpStatistics == null)
                    continue;

                for (var j = 0; j < playerHistoryHonorInfo.PvpNumberStatisticsList.Count; j++)
                {
                    var pvpNumberStatistics = playerHistoryHonorInfo.PvpNumberStatisticsList[j];
                    if(pvpNumberStatistics == null)
                        continue;

                    if(pvpNumberStatistics.PvpType != pvpStatistics.pvpType)
                        continue;
                    //服务器同步数据赋值
                    pvpNumberStatistics.PvpCount = pvpStatistics.pvpCnt;
                    break;
                }
                
            }

        }

        //是否在周几展示
        public static bool IsPvpShowInWeekDay(int weekDay, HonorPlayerTable honorPlayerTable)
        {
            if (weekDay <= 0 || weekDay > 7)
                return false;

            if (honorPlayerTable == null)
                return false;

            var dayList = CommonUtility.GetNumberListBySplitString(honorPlayerTable.OpenWeekDay);
            if (dayList == null || dayList.Count <= 0)
                return false;

            for (var i = 0; i < dayList.Count; i++)
            {
                //包含当前的天（周一-周日）
                if (weekDay == dayList[i])
                    return true;
            }

            return false;
        }

       

        private static PvpNumberStatistics CreatePvpNumberStatisticsByHonorPlayerTable(
            HonorPlayerTable honorPlayerTable)
        {
            if (honorPlayerTable == null)
                return null;

            PvpNumberStatistics pvpNumberStatistics = new PvpNumberStatistics();
            pvpNumberStatistics.PvpType = (uint)honorPlayerTable.ID;
            pvpNumberStatistics.PvpCount = 0;

            pvpNumberStatistics.PvpName = honorPlayerTable.name;
            pvpNumberStatistics.PvpIconPath = honorPlayerTable.HornorPlayIcon;
            pvpNumberStatistics.PvpSort = honorPlayerTable.Sort;

            return pvpNumberStatistics;
        }

       
        //得到荣誉登记表
        public static HonorLevelTable GetHonorLevelTableByLevel(int honorLevel)
        {
            HonorLevelTable honorLevelTable = TableManager.GetInstance().GetTableItem<HonorLevelTable>(honorLevel);

            return honorLevelTable;
        }

        #endregion

        //根据日期类型得到活动的列表
        public static List<PvpNumberStatistics> GetPvpNumberStaticsListByDateType(HONOR_DATE_TYPE honorDateType)
        {
            var playerHistoryHonorInfoList = HonorSystemDataManager.GetInstance().PlayerHistoryHonorInfoList;
            if (playerHistoryHonorInfoList == null || playerHistoryHonorInfoList.Count <= 0)
                return null;

            for (var i = 0; i < playerHistoryHonorInfoList.Count; i++)
            {
                var curPlayerHistoryHonorInfo = playerHistoryHonorInfoList[i];
                if(curPlayerHistoryHonorInfo == null)
                    continue;

                if (curPlayerHistoryHonorInfo.HonorDateType == honorDateType)
                    return curPlayerHistoryHonorInfo.PvpNumberStatisticsList;
            }

            return null;
        }

        //根据日期类型得到荣誉数据
        public static PlayerHistoryHonorInfo GetPlayerHistoryHonorInfoByDateType(HONOR_DATE_TYPE honorDateType)
        {
            var playerHistoryHonorInfoList = HonorSystemDataManager.GetInstance().PlayerHistoryHonorInfoList;
            if (playerHistoryHonorInfoList == null || playerHistoryHonorInfoList.Count <= 0)
                return null;

            for (var i = 0; i < playerHistoryHonorInfoList.Count; i++)
            {
                var curPlayerHistoryHonorInfo = playerHistoryHonorInfoList[i];
                if (curPlayerHistoryHonorInfo == null)
                    continue;

                if (curPlayerHistoryHonorInfo.HonorDateType == honorDateType)
                    return curPlayerHistoryHonorInfo;
            }

            return null;
        }

        #region HonorSystemPreviewLevelDataModel

        private static PreviewLevelItemDataModel CreatePreviewLevelItemDataModel(HonorLevelTable honorLevelTable)
        {
            PreviewLevelItemDataModel previewLevelItemDataModel = new PreviewLevelItemDataModel();

            if (honorLevelTable.ID == HonorSystemDataManager.DefaultHonorSystemLevel)
            {
                previewLevelItemDataModel.HonorSystemLevel = 0;
            }
            else
            {
                previewLevelItemDataModel.HonorSystemLevel = honorLevelTable.ID;
            }

            previewLevelItemDataModel.HonorLevelName = honorLevelTable.name;
            previewLevelItemDataModel.HonorSystemTotalNeedExpValue = honorLevelTable.NeedExp;

            previewLevelItemDataModel.HonorLevelFlagPath = honorLevelTable.TitleFlag;

            previewLevelItemDataModel.TitleId = honorLevelTable.Title;

            previewLevelItemDataModel.ShopDiscountList.Clear();
            for (var i = 0; i < honorLevelTable.ShopDiscount.Count; i++)
            {
                var shopDiscountStr = honorLevelTable.ShopDiscount[i];
                if (string.IsNullOrEmpty(shopDiscountStr) == false)
                    previewLevelItemDataModel.ShopDiscountList.Add(shopDiscountStr);
            }
            
            //当前等级解锁的商店道具
            previewLevelItemDataModel.UnLockShopItemList.Clear();
            if (string.IsNullOrEmpty(honorLevelTable.UnlockItem) == false)
            {
                var numberValueList = CommonUtility.GetNumberListBySplitString(honorLevelTable.UnlockItem);
                if (numberValueList != null && numberValueList.Count > 0)
                {
                    numberValueList.Sort();
                    previewLevelItemDataModel.UnLockShopItemList.AddRange(numberValueList.ToList());
                }
            }


            return previewLevelItemDataModel;
        }

        public static List<PreviewLevelItemDataModel> GetPreviewLevelItemDataModelList()
        {
            List<PreviewLevelItemDataModel> levelItemDataModelList = new List<PreviewLevelItemDataModel>();

            //0级
            var zeroLevelTable = TableManager.GetInstance()
                .GetTableItem<HonorLevelTable>(HonorSystemDataManager.DefaultHonorSystemLevel);
            if (zeroLevelTable != null)
            {
                var previewLevelDataModel = CreatePreviewLevelItemDataModel(zeroLevelTable);
                levelItemDataModelList.Add(previewLevelDataModel);
            }

            //正常等级
            var levelItemTables = TableManager.GetInstance().GetTable<HonorLevelTable>();
            if (levelItemTables != null)
            {
                var iter = levelItemTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var honorLevelTable = iter.Current.Value as HonorLevelTable;
                    if(honorLevelTable == null)
                        continue;

                    //0级，已经添加
                    if(honorLevelTable.ID == HonorSystemDataManager.DefaultHonorSystemLevel)
                        continue;

                    var previewLevelDataModel = CreatePreviewLevelItemDataModel(honorLevelTable);
                    levelItemDataModelList.Add(previewLevelDataModel);
                }
            }

            //按照等级排序
            if (levelItemDataModelList.Count > 1)
            {
                levelItemDataModelList.Sort((x, y) =>
                    x.HonorSystemLevel.CompareTo(y.HonorSystemLevel)
                );
            }

            //特殊处理，升级经验和解锁的商店道具
            var levelItemCount = levelItemDataModelList.Count;
            for (var i = 1; i < levelItemCount; i++)
            {
                var curLevelItemDataModel = levelItemDataModelList[i];
                var preLevelItemDataModel = levelItemDataModelList[i - 1];
                if(curLevelItemDataModel == null
                   || preLevelItemDataModel == null)
                    continue;

                curLevelItemDataModel.HonorSystemNeedExpValue = curLevelItemDataModel.HonorSystemTotalNeedExpValue
                                                                - preLevelItemDataModel.HonorSystemTotalNeedExpValue;

                if (preLevelItemDataModel.UnLockShopItemList != null &&
                    preLevelItemDataModel.UnLockShopItemList.Count > 0)
                    curLevelItemDataModel.UnLockShopItemList.AddRange(preLevelItemDataModel.UnLockShopItemList
                        .ToList());
            }

            return levelItemDataModelList;
        }


        #endregion

        #region ProtectCardItem

        public static ItemData GetProtectCardItemData(int itemTableId)
        {
            return null;
        }

        #endregion

        //得到荣誉等级对应的头衔的名字
        public static string GetTitleNameByTitleId(int titleId)
        {

            //头衔Id不存在
            if (titleId <= 0)
                return "";

            //头衔的Table不存在
            var newTitleTable = TableManager.GetInstance().GetTableItem<NewTitleTable>(titleId);
            if (newTitleTable == null)
                return "";

            //头衔类型是固定的
            if (newTitleTable.Type == 1)
            {
                return newTitleTable.name;
            }
            else
            {
                //需要拼接当前的职业：小职业+头衔名称
                var titleNameStr = newTitleTable.name;
                var professionName = CommonUtility.GetPlayerProfessionName();

                var finalTitleName = string.Format(titleNameStr, professionName);
                return finalTitleName;
            }
        }

        //得到当前荣誉登记对应的头衔图标
        public static string GetTitleIconPathByTitleId(int titleId)
        {
            //头衔的Table不存在
            var newTitleTable = TableManager.GetInstance().GetTableItem<NewTitleTable>(titleId);
            if (newTitleTable == null)
            {
                return "";
            }
            
            return newTitleTable.path;
        }

        //得到商店打折的字符串
        public static string GetShopDiscountValue(List<string> shopDiscountList)
        {
            if (shopDiscountList == null || shopDiscountList.Count <= 0)
                return "";

            var finalShopDiscountStr = "";

            for (var i = 0; i < shopDiscountList.Count; i++)
            {
                var shopCountStr = shopDiscountList[i];
                if(shopCountStr == null)
                    continue;

                var shopCountArray = CommonUtility.GetNumberListBySplitStringWithLine(shopCountStr);
                if(shopCountArray == null || shopCountArray.Count != 2)
                    continue;

                var shopId = shopCountArray[0];
                var discountValue = shopCountArray[1];
                if(shopId <= 0)
                    continue;

                if(discountValue <= 0 || discountValue >= 100)
                    continue;

                var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
                if(shopTable == null)
                    continue;

                if (i != 0)
                    finalShopDiscountStr += TR.Value("Common_Format_Split_Flag");

                var shopName = shopTable.ShopName;
                var discountStr = ShopNewUtility.GetDiscountStr(discountValue);
                var curDiscountStr = TR.Value("Honor_System_Shop_Item_Discount_Value",
                    shopName,
                    discountStr);

                finalShopDiscountStr += curDiscountStr;
            }

            return finalShopDiscountStr;
        }

        #region HonorExpValue

        //得到Pk的周荣誉
        public static string GetThisWeekHonorExpStrInPk()
        {
            if (HonorSystemDataManager.GetInstance().PkHonorExpMaxValue == 0)
            {
                //获得Pk荣誉的上限
                var pkHonorMaxValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                    (int) SystemValueTable.eType3.SVT_PK_SEASON_1V1_HONOR_MAX);
                if (pkHonorMaxValueTable != null)
                    HonorSystemDataManager.GetInstance().PkHonorExpMaxValue = pkHonorMaxValueTable.Value;
            }

            var maxValue = HonorSystemDataManager.GetInstance().PkHonorExpMaxValue;
            //当前数值
            var currentValue = CountDataManager.GetInstance()
                .GetCount(HonorSystemDataManager.GetInstance().PkHonorExpCounterStr);

            var finalStr = TR.Value("Common_Two_Number_Format_One",
                currentValue, maxValue);

            return finalStr;
        }

        //得到吃鸡的周荣耀
        public static string GetThisWeekHonorExpStrInChiJi()
        {
            //吃鸡本周荣誉的最大值
            if (HonorSystemDataManager.GetInstance().ChiJiHonorExpMaxValue == 0)
            {
                var chiJiHonorMaxValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                    (int)SystemValueTable.eType3.SVT_PK_CHIJI_HONOR_MAX);
                if (chiJiHonorMaxValueTable != null)
                    HonorSystemDataManager.GetInstance().ChiJiHonorExpMaxValue = chiJiHonorMaxValueTable.Value;
            }

            var maxValue = HonorSystemDataManager.GetInstance().ChiJiHonorExpMaxValue;
            //吃鸡本周荣誉的当前数值
            var currentValue = CountDataManager.GetInstance()
                .GetCount(HonorSystemDataManager.GetInstance().ChiJiHonorExpCounterStr);

            var finalStr = TR.Value("Common_Two_Number_Format_One",
                currentValue,
                maxValue);

            return finalStr;
        }

        #endregion

    }

}