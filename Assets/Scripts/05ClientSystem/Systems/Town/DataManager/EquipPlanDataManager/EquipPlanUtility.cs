using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //装备方案助手
    public static class EquipPlanUtility
    {

        //装备方案功能是否开着
        //如果打开（返回true）,表示可以切换装备；如果关闭（返回false),表示不可以切换装备
        public static bool IsEquipPlanOpenedByServer()
        {
            //装备方案开关是否打开
            var isServiceTypeSwitchOpen = ServerSceneFuncSwitchManager.GetInstance()
                .IsServiceTypeSwitchOpen(ServiceType.SERVICE_EQUIP_SCHEME);

            return isServiceTypeSwitchOpen;
        }

        //装备方案是否解锁
        public static bool IsShowEquipPlanFunction()
        {
            //数据没有同步
            if (EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId <= 0)
                return false;

            //等级没有达到
            if (PlayerBaseData.GetInstance().Level < EquipPlanDataManager.GetInstance().GetEquipPlanUnLockLevel())
                return false;

            return true;
        }

        //更新EquipPlanDataModel
        public static void UpdateEquipPlanDataModel(EquipPlanDataModel equipPlanDataModel,
            EquipSchemeInfo equipSchemeInfo)
        {
            if (equipPlanDataModel == null || equipSchemeInfo == null)
                return;

            equipPlanDataModel.Guid = equipSchemeInfo.guid;
            equipPlanDataModel.EquipPlanType = (EquipSchemeType) equipSchemeInfo.type;
            equipPlanDataModel.EquipPlanId = equipSchemeInfo.id;
            equipPlanDataModel.IsWear = equipSchemeInfo.weared == 1 ? true : false;
            
            equipPlanDataModel.EquipItemGuidList.Clear();
            if (equipSchemeInfo.equips != null && equipSchemeInfo.equips.Length > 0)
                equipPlanDataModel.EquipItemGuidList = equipSchemeInfo.equips.ToList();

            //从小到大排序
            if (equipPlanDataModel.EquipItemGuidList != null && equipPlanDataModel.EquipItemGuidList.Count > 0)
                equipPlanDataModel.EquipItemGuidList.Sort();
        }

        //创建EquipPlanDataModel
        public static EquipPlanDataModel CreateEquipPlanDataModel(EquipSchemeInfo schemeInfo)
        {
            if (schemeInfo == null)
                return null;

            EquipPlanDataModel equipPlanDataModel = new EquipPlanDataModel();

            UpdateEquipPlanDataModel(equipPlanDataModel, schemeInfo);
            
            return equipPlanDataModel;
        }

        //判断Item上是否显示未启用方案的Icon
        public static bool IsNeedShowUnUsedEquipPlanFlag(ItemData itemData, ref int equipPlanId)
        {
            if (itemData == null)
                return false;

            if (itemData.GUID == 0)
                return false;

            if (itemData.TableData == null)
                return false;

            var isNeedShowUnUsedEquipPlanFlag = itemData.IsItemInUnUsedEquipPlan;
            //在未启用的装备方案中，赋值装备方案ID
            if (isNeedShowUnUsedEquipPlanFlag == true)
                equipPlanId = EquipPlanDataManager.GetInstance().UnSelectedEquipPlanId;

            return isNeedShowUnUsedEquipPlanFlag;
        }

        //装备方案中是否包含道具的类型
        public static bool IsEquipPlanOwnerItemType(ItemTable itemTable)
        {
            if (itemTable == null)
                return false;

            //装备武器不在装备方案中
            if (itemTable.Type == ItemTable.eType.EQUIP
                && itemTable.SubType == ItemTable.eSubType.WEAPON)
                return false;

            //非装备且非称号，不显示
            if (itemTable.Type != ItemTable.eType.EQUIP
                && itemTable.SubType != ItemTable.eSubType.TITLE)
                return false;

            return true;
        }

        //判断道具是否在未启用的方案中
        public static bool IsItemInUnUsedEquipPlanByItemData(ItemData itemData)
        {
            if (itemData == null)
                return false;

            if (itemData.GUID <= 0)
                return false;

            if (itemData.TableData == null)
                return false;

            //穿戴上的装备返回false
            if (itemData.PackageType == EPackageType.WearEquip)
                return false;

            //装备方案中不包括这种类型
            var isEquipPlanOwnerItemType = IsEquipPlanOwnerItemType(itemData.TableData);
            if (isEquipPlanOwnerItemType == false)
                return false;
            
            return IsItemInUnUsedEquipPlanByItemGuid(itemData.GUID);
        }

        //判断背包中的道具是否在未启用的装备方案中
        private static bool IsItemInUnUsedEquipPlanByItemGuid(ulong guid)
        {
            if (guid == 0)
                return false;

            //装备方案没有解锁，不存在
            if (IsShowEquipPlanFunction() == false)
                return false;

            //未启用的装备方案
            var unSelectedEquipPlanItemGuidList = EquipPlanDataManager.GetInstance()
                .UnSelectedEquipPlanItemGuidList;
            
            if (unSelectedEquipPlanItemGuidList == null
                || unSelectedEquipPlanItemGuidList.Count <= 0)
                return false;

            var isFind = CommonUtility.FindInListByBinarySearch(unSelectedEquipPlanItemGuidList,
                guid);

            return isFind;
        }

        //判断某种装备方案的穿戴装备是否都在背包中
        public static bool IsAllEquipInPackageByEquipPlanId(int equipPlanId)
        {
            //方案的装备列表
            List<ulong> equipItemGuidList = GetEquipItemGuidListByEquipPlanId(equipPlanId);
            //装备方案列表中不存在道具
            if (equipItemGuidList == null || equipItemGuidList.Count <= 0)
                return true;

            //检测每一个道具是否都在背包中（穿在身上，装备包裹和称号包裹)
            for (var i = 0; i < equipItemGuidList.Count; i++)
            {
                var equipItemGuid = equipItemGuidList[i];
                if (equipItemGuid <= 0)
                    continue;

                //道具已经不存在，返回false
                var itemData = ItemDataManager.GetInstance().GetItem(equipItemGuid);
                if (itemData == null)
                    return false;

                //道具没有穿在身上，不在装备包裹，不在称号包裹，返回true
                if (itemData.PackageType != EPackageType.WearEquip
                    && itemData.PackageType != EPackageType.Equip
                    && itemData.PackageType != EPackageType.Title
                    && itemData.PackageType != EPackageType.Sinan)
                    return false;
            }

            return true;
        }

        //得到某种装备方案的装备列表
        public static List<ulong> GetEquipItemGuidListByEquipPlanId(int equipPlanId)
        {
            var equipPlanDataModelList = EquipPlanDataManager.GetInstance().EquipPlanDataModelList;
            if (equipPlanDataModelList == null || equipPlanDataModelList.Count <= 0)
                return null;

            for (var i = 0; i < equipPlanDataModelList.Count; i++)
            {
                var equipPlanDataModel = equipPlanDataModelList[i];
                if (equipPlanDataModel == null)
                    continue;

                if (equipPlanDataModel.EquipPlanId == equipPlanId)
                    return equipPlanDataModel.EquipItemGuidList;
            }

            return null;
        }

        //切换装备方案的操作
        public static void OnSwitchEquipPlanAction()
        {
            //如果装备方案切换的功能已经关闭，弹出飘窗
            if (IsEquipPlanOpenedByServer() == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Equip_Plan_Is_Already_Closed"));
                return;
            }

            //如果当前是第一套，
            if (EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId == 1)
            {
                //首次启用第二套
                if (EquipPlanDataManager.GetInstance().IsAlreadySwitchEquipPlan == false)
                {
                    OpenSwitchEquipPlanTipFrameByFirstTime();
                    return;
                }
            }

            //需要切换的装备方案Id
            var switchToEquipPlanId = GetSwitchToEquipPlanId();

            var isAllEquipItemInPackage = IsAllEquipInPackageByEquipPlanId(switchToEquipPlanId);
            //打开界面
            OpenSwitchEquipPlanTipFrameByCommonState(
                EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId,
                switchToEquipPlanId,
                isAllEquipItemInPackage);

        }

        //首次切换的提示界面
        private static void OpenSwitchEquipPlanTipFrameByFirstTime()
        {
            //首次启用方案2
            var currentEquipPlanId = EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId;
            var switchToEquipPlanId = GetSwitchToEquipPlanId();

            var currentEquipPlanIdStr = GetEquipPlanIdStr(currentEquipPlanId);
            var switchToEquipPlanIdStr = GetEquipPlanIdStr(switchToEquipPlanId);

            var tipContent = TR.Value("Equip_Plan_First_Use_Plan_Two_Content",
                switchToEquipPlanIdStr,
                currentEquipPlanIdStr);

            CommonMsgBoxOkCancelNewParamData paramData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("Equip_Plan_Not_Sync_Text"),
                RightButtonText = TR.Value("Equip_Plan_Sync_Text"),
                OnLeftButtonClickCallBack = EquipPlanDataManager.GetInstance().OnSwitchEquipPlanByCommonAction,
                OnRightButtonClickCallBack = EquipPlanDataManager.GetInstance().OnSwitchEquipPlanWithSyncFirstEquipPlan,
            };

            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(paramData);
        }

        //正常切换页面的提示
        private static void OpenSwitchEquipPlanTipFrameByCommonState(int currentEquipPlanId,
            int switchToEquipPlanId,
            bool isAllEquipItemInPackage)
        {
            var tipContent = "";
            var currentEquipPlanIdStr = GetEquipPlanIdStr(currentEquipPlanId);
            var switchToEquipPlanIdStr = GetEquipPlanIdStr(switchToEquipPlanId);

            //存在不在背包中的装备
            if (isAllEquipItemInPackage == false)
            {
                tipContent = TR.Value("Equip_Plan_Change_Equip_Plan_WithSpecial_Item_Content",
                    currentEquipPlanIdStr,
                    switchToEquipPlanIdStr,
                    switchToEquipPlanIdStr);
            }
            else
            {
                tipContent = TR.Value("Equip_Plan_Change_Equip_Plan_Common_Content",
                    currentEquipPlanIdStr,
                    switchToEquipPlanIdStr);
            }

            CommonMsgBoxOkCancelNewParamData paramData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure_2"),
                OnRightButtonClickCallBack = EquipPlanDataManager.GetInstance()
                    .OnSwitchEquipPlanByCommonAction,
            };

            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(paramData);
        }

        //装备方案对应的字符串
        public static string GetEquipPlanIdStr(int equipPlanId)
        {
            if (equipPlanId == 2)
            {
                return TR.Value("Equip_Plan_Format_Two_Text");
            }
            else
            {
                return TR.Value("Equip_Plan_Format_One_Text");
            }
        }

        //得到要切换的装备方案Id
        public static int GetSwitchToEquipPlanId()
        {
            if (EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId == 1)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        //只获得某种装备方案中的装备（排除称号）GUID的列表
        //返回null，表示该装备方案中的装备列表不存在
        public static List<ulong> OnlyGetEquipItemGuidListInEquipPlanByEquipPlanId(int equipPlanId)
        {
            //装备方案功能没有打开
            if (IsEquipPlanOpenedByServer() == false)
                return null;

            //装备方案中的所有装备（包括称号）
            var allEquipItemGuidList = GetEquipItemGuidListByEquipPlanId(equipPlanId);
            //装备方案不存在
            if (allEquipItemGuidList == null || allEquipItemGuidList.Count <= 0)
                return null;

            List<ulong> onlyEquipItemGuidList = new List<ulong>();

            for (var i = 0; i < allEquipItemGuidList.Count; i++)
            {
                var equipItemGuid = allEquipItemGuidList[i];
                if(equipItemGuid <= 0)
                    continue;

                var itemData = ItemDataManager.GetInstance().GetItem(equipItemGuid);
                if(itemData == null)
                    continue;

                if(itemData.TableData == null)
                    continue;

                //称号,排除
                if(itemData.TableData.SubType == ItemTable.eSubType.TITLE)
                    continue;

                //添加
                onlyEquipItemGuidList.Add(equipItemGuid);
            }

            return onlyEquipItemGuidList;
        }

        //只获得某种装备方案中称号的GUID
        //返回0，表示该装备方案中的称号不存在
        public static ulong OnlyGetTitleItemGuidInEquipPlanByEquipPlanId(int equipPlanId)
        {

            //装备方案没有打开
            if (IsEquipPlanOpenedByServer() == false)
                return 0;

            //装备方案中的所有装备（包括称号）
            var allEquipItemGuidList = GetEquipItemGuidListByEquipPlanId(equipPlanId);
            //装备方案不存在
            if (allEquipItemGuidList == null || allEquipItemGuidList.Count <= 0)
                return 0;

            for (var i = 0; i < allEquipItemGuidList.Count; i++)
            {
                var equipItemGuid = allEquipItemGuidList[i];
                if (equipItemGuid <= 0)
                    continue;

                var itemData = ItemDataManager.GetInstance().GetItem(equipItemGuid);
                if (itemData == null)
                    continue;

                if (itemData.TableData == null)
                    continue;

                //称号,返回
                if (itemData.TableData.SubType == ItemTable.eSubType.TITLE)
                    return equipItemGuid;
            }

            return 0;
        }

        //两种装备方案中是否拥有不同的道具,用于判断飘字
        public static bool IsEquipPlanOwnerDifferentItem()
        {

            var unSelectedItemList = EquipPlanDataManager.GetInstance().UnSelectedEquipPlanItemGuidList;
            var currentSelectedItemList = EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanItemGuidList;

            //未启用的装备方案不存在
            if (unSelectedItemList == null || unSelectedItemList.Count <= 0)
            {
                if (currentSelectedItemList == null)
                    return false;
                else
                {
                    if (currentSelectedItemList.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                //未启用的装备方案存在道具
                //当前装备方案不存在
                if (currentSelectedItemList == null || currentSelectedItemList.Count <= 0)
                    return true;
            }

            var unSelectedItemNumber = unSelectedItemList.Count;
            var currentSelectedItemNumber = currentSelectedItemList.Count;

            //都没有道具
            if (unSelectedItemNumber == 0 && currentSelectedItemNumber == 0)
            {
                return false;
            }
            else
            {
                //道具数量有一个为0
                if ((unSelectedItemNumber == 0 && currentSelectedItemNumber != 0)
                    || (unSelectedItemNumber != 0 && currentSelectedItemNumber == 0))
                {
                    return true;
                }
                else
                {
                    //道具数量都不为0，且数量不同
                    if (unSelectedItemNumber != currentSelectedItemNumber)
                    {
                        return true;
                    }
                    else
                    {
                        //道具数量相同
                        bool isFindDifferentItem = false;
                        for (var i = 0; i < unSelectedItemNumber; i++)
                        {
                            if (i >= currentSelectedItemNumber)
                                break;

                            if (unSelectedItemList[i] != currentSelectedItemList[i])
                            {
                                isFindDifferentItem = true;
                                break;
                            }
                        }

                        return isFindDifferentItem;
                    }
                }
            }
        }

        //得到未穿戴装备方案的数据
        public static List<ulong> GetUnSelectedEquipPlanItemGuidList()
        {
            //装备方案的开关没有打开
            if (IsEquipPlanOpenedByServer() == false)
                return null;

            return EquipPlanDataManager.GetInstance().UnSelectedEquipPlanItemGuidList;
        }
    }
}