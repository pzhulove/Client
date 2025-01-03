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
    //附魔卡合成助手类
    public static class MagicCardMergeUtility
    {

        #region MagicCardMerge
        public static bool IsShowMagicCardMergeLevelTip()
        {
            if (MagicCardMergeDataManager.GetInstance().IsNotShowMagicCardMergeLevelTip == true)
                return false;
            else
                return true;
        }

        //是否显示两个附魔卡绑定类型不一致的提示
        public static bool IsShowMagicCardMergeOwnerDifferentTip(ItemData leftItem, ItemData rightItem)
        {
            if (leftItem == null || leftItem.TableData == null
                                 || rightItem == null || rightItem.TableData == null)
                return false;

            if (MagicCardMergeDataManager.GetInstance().IsNotShowMagicCardMergeOwnerDifferentTip == true)
                return false;

            //非绑定和绑定
            if (leftItem.TableData.Owner == ItemTable.eOwner.NOTBIND &&
                rightItem.TableData.Owner != ItemTable.eOwner.NOTBIND)
                return true;

            if (rightItem.TableData.Owner == ItemTable.eOwner.NOTBIND
                && leftItem.TableData.Owner != ItemTable.eOwner.NOTBIND)
                return true;

            return false;
        }

        public static void OnShowMagicCardMergeOwnerTip(Action onMergeClick, OnCommonMsgBoxToggleClick onLoginClick)
        {
            //显示提示
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("magic_card_merge_bind_tip"),
                IsShowNotify = true,
                OnCommonMsgBoxToggleClick = onLoginClick,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = onMergeClick,
            };

            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);

        }

        public static void OnShowMagicCardMergeLevelTip(Action onMergeClick, OnCommonMsgBoxToggleClick onLoginClick)
        {
            //显示提示
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("magic_card_merge_level_tip"),
                IsShowNotify = true,
                OnCommonMsgBoxToggleClick = onLoginClick,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure_merge"),
                OnRightButtonClickCallBack = onMergeClick,
            };

            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);

        }

        public static int GetMagicCardStrengthLevel(ItemData itemData)
        {
            if (itemData == null)
                return 0;

            if (itemData.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
            {
                if (itemData.mPrecEnchantmentCard == null)
                    return 0;
                if (itemData.mPrecEnchantmentCard.iEnchantmentCardLevel <= 0)
                    return 0;
                else
                {
                    return itemData.mPrecEnchantmentCard.iEnchantmentCardLevel;
                }
            }

            return 0;
        }

        #endregion

        #region MagicCardOneKeyMerge

        //打开关闭一键合成的结果按钮
        public static void OnOpenMagicCardOneKeyMergeResultFrame()
        {
            OnCloseMagicCardOneKeyMergeResultFrame();

            ClientSystemManager.GetInstance().OpenFrame<MagicCardOneKeyMergeResultFrame>(FrameLayer.Middle);
        }

        public static void OnCloseMagicCardOneKeyMergeResultFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<MagicCardOneKeyMergeResultFrame>() == true)
            {
                ClientSystemManager.GetInstance().CloseFrame<MagicCardOneKeyMergeResultFrame>();
            }
        }

        //打开一键合成的提示按钮
        public static void OnOpenMagicCardOneKeyMergeTipFrame(Action magicCardMergeAction)
        {

            OnCloseMagicCardOneKeyMergeTipFrame();

            MagicCardMergeData magicCardMergeData = new MagicCardMergeData();
            magicCardMergeData.MagicCardMergeAction = magicCardMergeAction;

            ClientSystemManager.GetInstance()
                .OpenFrame<MagicCardOneKeyMergeTipFrame>(FrameLayer.Middle, magicCardMergeData);

        }

        //一键合成附魔卡道具进行排序
        public static void SortMagicCardMergeResultData(List<ItemReward> itemRewardList)
        {
            if (itemRewardList == null || itemRewardList.Count <= 1)
                return;

            itemRewardList.Sort((x, y) =>
            {
                var itemDataX = TableManager.GetInstance().GetTableItem<ItemTable>((int)x.id);
                var itemDataY = TableManager.GetInstance().GetTableItem<ItemTable>((int)y.id);
                if (itemDataX == null || itemDataY == null)
                    return -1;

                //品质由高到底
                var a = -itemDataX.Color.CompareTo(itemDataY.Color);

                //品质相同，数量由多到少
                if (a == 0)
                    a = -x.num.CompareTo(y.num);

                //品质和数量都相同，按照ID的大小由小到大进行排序
                if (a == 0)
                    a = x.id.CompareTo(y.id);

                return a;
            });
        }


        public static ItemData GetMagicCardItem(ulong guid)
        {
            var itemData = ItemDataManager.GetInstance().GetItem(guid);
            //附魔卡等级≥1级，不会被选中，不会被批量合成
            if (itemData != null
                && itemData.Type == ItemTable.eType.EXPENDABLE
                && itemData.SubType == (int)ItemTable.eSubType.EnchantmentsCard
                && itemData.PackageType != EPackageType.Storage
                && itemData.PackageType != EPackageType.RoleStorage
                && GetMagicCardStrengthLevel(itemData) < 1)
            {
                return itemData;
            }
            return null;
        }

        //一键合成中白色和蓝色附魔卡的数量和价格
        public static void GetMagicCardOneKeyMergeInfo(ref int whiteMagicCardNumber,
            ref int blueMagicCardNumber,
            ref int whiteMagicCardCost,
            ref int blueMagicCardCost)
        {
            List<ItemData> whiteItemList = null;
            List<ItemData> blueItemList = null;

            if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseWhiteCard)
                whiteItemList = GetWhiteMagicCardItemList();

            if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseBlueCard)
                blueItemList = GetBlueMagicCardItemList();

            if (whiteItemList != null)
            {
                var bindWhiteItemCount = 0;
                var noBindWhiteItemCount = 0;
                var whiteItemCount = 0;
                var whiteItemCost = 0;
                for (var i = 0; i < whiteItemList.Count; i++)
                {
                    if (whiteItemList[i] != null)
                    {
                        //分别统计不同绑定方式的个数
                        if (whiteItemList[i].TableData != null)
                        {
                            if (whiteItemList[i].TableData.Owner == ItemTable.eOwner.NOTBIND)
                                noBindWhiteItemCount += whiteItemList[i].Count;
                            else
                                bindWhiteItemCount += whiteItemList[i].Count;
                        }
                        if (whiteItemCost == 0)
                        {
                            var magicItem = TableManager.GetInstance().GetTableItem<MagicCardTable>(whiteItemList[i].TableID);
                            if (null != magicItem)
                            {
                                whiteItemCost = magicItem.CostNum;
                            }
                        }
                    }
                }

                ////可以合成的白色数量(绑定和非绑定)
                //whiteItemCount = (noBindWhiteItemCount / 2) * 2 + (bindWhiteItemCount / 2) * 2;

                //可以合成的白色数量,
                //非绑定附魔卡不参与, 只有绑定
                if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseNoBindItem
                    == false)
                {
                    whiteItemCount = (bindWhiteItemCount / 2) * 2;
                }
                else
                {
                    ////非绑定附魔卡参与: 绑定与绑定的合成，非绑定和非绑定的合成。如果剩余1个绑定，1个非绑定，则直接合成
                    whiteItemCount = ((noBindWhiteItemCount + bindWhiteItemCount) / 2) * 2;
                }

                if (whiteItemCount >= 2)
                {
                    whiteMagicCardNumber = whiteItemCount;
                    whiteMagicCardCost = whiteItemCost;
                }
            }

            if (blueItemList != null)
            {
                var bindBlueItemCount = 0;
                var noBindBlueItemCount = 0;
                var blueItemCount = 0;
                var blueItemCost = 0;

                for (var i = 0; i < blueItemList.Count; i++)
                {
                    if (blueItemList[i] != null)
                    {
                        //分别统计不同绑定方式的个数
                        if (blueItemList[i].TableData != null)
                        {
                            if (blueItemList[i].TableData.Owner == ItemTable.eOwner.NOTBIND)
                                noBindBlueItemCount += blueItemList[i].Count;
                            else
                                bindBlueItemCount += blueItemList[i].Count;
                        }

                        if (blueItemCost == 0)
                        {
                            var magicItem = TableManager.GetInstance().GetTableItem<MagicCardTable>(
                                blueItemList[i].TableID);
                            if (null != magicItem)
                            {
                                blueItemCost = magicItem.CostNum;
                            }
                        }
                    }
                }

                ////可以合成的蓝色数量(绑定和非绑定)
                //blueItemCount = (noBindBlueItemCount / 2) * 2 + (bindBlueItemCount / 2) * 2;

                //可以合成的蓝色数量
                //非绑定附魔卡不参与，只有绑定
                if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseNoBindItem
                    == false)
                {
                    blueItemCount = (bindBlueItemCount / 2) * 2;
                }
                else
                {
                    ////非绑定附魔卡参与: 绑定与绑定的合成，非绑定和非绑定的合成。如果剩余1个绑定，1个非绑定，则直接合成
                    blueItemCount = ((bindBlueItemCount + noBindBlueItemCount) / 2) * 2;
                }

                if (blueItemCount >= 2)
                {
                    blueMagicCardNumber = blueItemCount;
                    blueMagicCardCost = blueItemCost;
                }
            }
        }

        //附魔卡一次最多合成的次数
        public static int GetMaxMergeTimes()
        {
            var maxMergeTimes = 0;     //一次最多合成次数
            var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                (int)SystemValueTable.eType2.SVT_MAGIC_COMP_ONES_MAXTIMES);
            if (systemValue != null)
                maxMergeTimes = systemValue.Value;

            if (maxMergeTimes <= 0)
                maxMergeTimes = 50;

            return maxMergeTimes;
        }

        //获得合成的详情（分别合成多少次，消耗多少金币)
        public static string GetMagicCardOneKeyMergeTipContent(int whiteMagicCardNumber,
            int whiteMergeCost,
            int blueMagicCardNumber,
            int blueMergeCost)
        {
            //得到最多合成次数
            var maxMergeTimes = GetMaxMergeTimes();

            var whiteStr = "白色";
            var blueStr = "蓝色";

            var result = "";

            //可以合成的次数
            var whiteMergeNumber = whiteMagicCardNumber / 2;
            var blueMergeNumber = blueMagicCardNumber / 2;

            //对合成次数进行处理，如果可以合成的次数超过最多合成次数
            if (whiteMergeNumber + blueMergeNumber > maxMergeTimes)
            {
                //只能合成白色
                if (whiteMergeNumber >= maxMergeTimes)
                {
                    whiteMergeNumber = maxMergeTimes;
                    blueMergeNumber = 0;
                }
                else
                {
                    blueMergeNumber = maxMergeTimes - whiteMergeNumber;
                }
            }

            //拥有金币或者绑金的数量
            var ownerMoneyNumber = PlayerBaseData.GetInstance().BindGold;
            if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUserGold == true)
                ownerMoneyNumber += PlayerBaseData.GetInstance().Gold;

            //总共消耗的金币数量
            var totalMergeNeedMoneyNumber = (ulong)(whiteMergeNumber * whiteMergeCost + blueMergeNumber * blueMergeCost);

            //金币或者绑金足够多
            if (ownerMoneyNumber >= totalMergeNeedMoneyNumber)
            {
                //绑定金币或者金币足够多的，可以合成两种品质所有的附魔卡
                var costBindGold = totalMergeNeedMoneyNumber > PlayerBaseData.GetInstance().BindGold
                    ? PlayerBaseData.GetInstance().BindGold
                    : totalMergeNeedMoneyNumber;
                //可能消耗金币的数量
                var costGold = totalMergeNeedMoneyNumber - costBindGold > 0
                    ? (totalMergeNeedMoneyNumber - costBindGold)
                    : 0;

                //使用金币
                if (costGold > 0)
                {
                    //白色和紫色存在
                    if (whiteMergeNumber > 0 && blueMergeNumber > 0)
                    {
                        result = string.Format(TR.Value("magic_card_merge_money_enough_four"),
                            whiteStr,
                            whiteMergeNumber * 2,
                            blueStr,
                            blueMergeNumber * 2,
                            costBindGold,
                            costGold);
                    }
                    else if (whiteMergeNumber > 0)
                    {
                        //只有白色
                        result = string.Format(TR.Value("magic_card_merge_money_enough_two"),
                            whiteStr,
                            whiteMergeNumber * 2,
                            costBindGold,
                            costGold);
                    }
                    else
                    {
                        //只有紫色
                        result = string.Format(TR.Value("magic_card_merge_money_enough_two"),
                            blueStr,
                            blueMergeNumber * 2,
                            costBindGold,
                            costGold);
                    }
                }
                else
                {
                    //不使用金币

                    if (whiteMergeNumber > 0 && blueMergeNumber > 0)
                    {
                        result = string.Format(TR.Value("magic_card_merge_money_enough_three"),
                            whiteStr,
                            whiteMergeNumber * 2,
                            blueStr,
                            blueMergeNumber * 2,
                            costBindGold);
                    }
                    else if (whiteMergeNumber > 0)
                    {
                        result = string.Format(TR.Value("magic_card_merge_money_enough_one"),
                            whiteStr,
                            whiteMergeNumber * 2,
                            costBindGold);
                    }
                    else
                    {
                        result = string.Format(TR.Value("magic_card_merge_money_enough_one"),
                            blueStr,
                            blueMergeNumber * 2,
                            costBindGold);
                    }
                }
            }
            else
            {
                //绑定金币或者金币不足，只能合成部分附魔卡，首先合成白色附魔卡；有剩余的时候再合成蓝色附魔卡

                //首先合成白色附魔卡
                if (whiteMergeNumber > 0)
                {
                    var whiteMergeNeedMoneyNumber = (ulong)(whiteMergeNumber * whiteMergeCost);
                    //金币只够合成白色的，可能白色全部合成完全但不足以合成一个蓝色附魔卡，或者只能合成部分白色的附魔卡
                    if (ownerMoneyNumber < whiteMergeNeedMoneyNumber + (ulong)blueMergeCost)
                    {
                        //白色消耗完金币，
                        var canMergeWhiteNumber = (ulong)whiteMergeNumber;
                        if (whiteMergeCost > 0)
                            canMergeWhiteNumber = ownerMoneyNumber / (ulong)whiteMergeCost;

                        if (canMergeWhiteNumber >= (ulong)whiteMergeNumber)
                            canMergeWhiteNumber = (ulong)whiteMergeNumber;
                        var canCostMoneyNumber = canMergeWhiteNumber * (ulong)whiteMergeCost;

                        //判断是否使用金币
                        ulong canCostGoldNumber = 0;
                        ulong canCostBindGoldNumber = 0;
                        GetCostBindGoldAndGoldInfo(canCostMoneyNumber,
                            out canCostBindGoldNumber,
                            out canCostGoldNumber);

                        //消耗金币
                        if (canCostGoldNumber > 0)
                        {
                            result = string.Format(TR.Value("magic_card_merge_money_less_two"),
                                canCostBindGoldNumber,
                                canCostGoldNumber,
                                whiteStr,
                                canMergeWhiteNumber * 2);
                        }
                        else
                        {
                            //不消耗金币
                            result = string.Format(TR.Value("magic_card_merge_money_less_one"),
                                canCostBindGoldNumber,
                                whiteStr,
                                canMergeWhiteNumber * 2);
                        }
                    }
                    else
                    {
                        //白色可以全部合成并且至少可以合成一个蓝色
                        var leftToMergeBlueMoneyNumber = ownerMoneyNumber - whiteMergeNeedMoneyNumber;

                        //计算出蓝色可以合成的数量，以及蓝色需要的金币
                        var canMergeBlueNumber = (ulong)blueMergeNumber;
                        if (blueMergeCost > 0)
                            canMergeBlueNumber = leftToMergeBlueMoneyNumber / (ulong)blueMergeCost;

                        if (canMergeBlueNumber >= (ulong)blueMergeNumber)
                            canMergeBlueNumber = (ulong)blueMergeNumber;

                        var blueMergeNeedMoneyNumber = canMergeBlueNumber * (ulong)blueMergeCost;

                        var needMoneyNumber = whiteMergeNeedMoneyNumber + blueMergeNeedMoneyNumber;

                        //判断是否使用金币
                        ulong canCostGoldNumber = 0;
                        ulong canCostBindGoldNumber = 0;
                        GetCostBindGoldAndGoldInfo(needMoneyNumber,
                            out canCostBindGoldNumber,
                            out canCostGoldNumber);

                        //消耗金币
                        if (canCostGoldNumber > 0)
                        {
                            result = string.Format(TR.Value("magic_card_merge_money_less_four"),
                                canCostBindGoldNumber,
                                canCostGoldNumber,
                                whiteStr,
                                whiteMergeNumber * 2,
                                blueStr,
                                canMergeBlueNumber * 2);
                        }
                        else
                        {
                            //不消耗金币
                            result = string.Format(TR.Value("magic_card_merge_money_less_three"),
                                canCostBindGoldNumber,
                                whiteStr,
                                whiteMergeNumber * 2,
                                blueStr,
                                canMergeBlueNumber * 2);
                        }
                    }
                }
                else
                {
                    //只有蓝色一种情况
                    var canMergeBlueNumber = (ulong)blueMergeNumber;
                    if (blueMergeCost > 0)
                        canMergeBlueNumber = ownerMoneyNumber / (ulong)blueMergeCost;

                    if (canMergeBlueNumber > (ulong)blueMergeNumber)
                        canMergeBlueNumber = (ulong)blueMergeNumber;

                    var canCostMoneyNumber = canMergeBlueNumber * (ulong)blueMergeCost;

                    //判断是否使用金币
                    ulong canCostGoldNumber = 0;
                    ulong canCostBindGoldNumber = 0;
                    GetCostBindGoldAndGoldInfo(canCostMoneyNumber, out canCostBindGoldNumber,
                        out canCostGoldNumber);

                    //消耗金币
                    if (canCostGoldNumber > 0)
                    {
                        result = string.Format(TR.Value("magic_card_merge_money_less_two"),
                            canCostBindGoldNumber,
                            canCostGoldNumber,
                            blueStr,
                            canMergeBlueNumber * 2);
                    }
                    else
                    {
                        //不消耗金币
                        result = string.Format(TR.Value("magic_card_merge_money_less_one"),
                            canCostBindGoldNumber,
                            blueStr,
                            canMergeBlueNumber * 2);
                    }
                }
            }

            return result;
        }

        //在金币和绑金不足的时候，获得绑金和金币分别消耗了多少。
        private static void GetCostBindGoldAndGoldInfo(ulong canCostMoneyNumber,
            out ulong canCostBindGoldNumber,
            out ulong canCostGoldNumber)
        {
            canCostBindGoldNumber = 0;
            canCostGoldNumber = 0;
            if (canCostMoneyNumber > PlayerBaseData.GetInstance().BindGold)
            {
                canCostBindGoldNumber = PlayerBaseData.GetInstance().BindGold;
                canCostGoldNumber = PlayerBaseData.GetInstance().Gold;
            }
            else
            {
                canCostBindGoldNumber = PlayerBaseData.GetInstance().BindGold;
                if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUserGold == true)
                    canCostGoldNumber = PlayerBaseData.GetInstance().Gold;
            }
        }
        

        //得到可以合成的白色附魔卡
        public static List<ItemData> GetWhiteMagicCardItemList()
        {
            return GetMagicCardItemListByQuality(ItemTable.eColor.WHITE);
        }

        public static List<ItemData> GetBlueMagicCardItemList()
        {

            return GetMagicCardItemListByQuality(ItemTable.eColor.BLUE);
        }

        public static List<ItemData> GetMagicCardItemListByQuality(ItemTable.eColor itemColor)
        {
            List<ItemData> itemList = new List<ItemData>();

            var itemIds = ItemDataManager.GetInstance().GetItemsByType(ItemTable.eType.EXPENDABLE);
            for (int i = 0; i < itemIds.Count; ++i)
            {
                var itemData = GetMagicCardItem(itemIds[i]);
                if (itemData != null)
                {
                    if(itemData.Quality == itemColor)
                        itemList.Add(itemData);
                }
            }
            return itemList;
        }


        //金币足够或者不足的提示
        public static void OnShowOneKeyMergeTipContent(string tipContent, Action onMergeAction)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = onMergeAction,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }


        public static void OnCloseMagicCardOneKeyMergeTipFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<MagicCardOneKeyMergeTipFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<MagicCardOneKeyMergeTipFrame>();
        }

        #endregion

    }
}