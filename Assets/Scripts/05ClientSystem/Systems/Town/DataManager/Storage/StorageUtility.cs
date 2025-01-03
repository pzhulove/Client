using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoTable;

namespace GameClient
{
    public static class StorageUtility
    {

        //打开界面
        public static void OnOpenStorageUnLockTipFrame(StorageUnLockCostDataModel storageUnLockCostDataModel, EPackageType ePackageType)
        {
            var costItemId = storageUnLockCostDataModel.CostItemId;
            var costItemNumber = storageUnLockCostDataModel.CostItemNumber;

            var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(costItemId);
            if (costItemTable == null)
                return;

            var tipContentStr = TR.Value("storage_unlock_cost_format", costItemNumber,
                costItemTable.Name);

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo()
            {
                nMoneyID = costItemId,
                nCount = costItemNumber,
            };

            EPackageType ePackageTypeTemp = ePackageType;
            CommonUtility.OnShowCommonMsgBox(tipContentStr,
                () => {OnCheckMoneyIsEnough(costInfo, ePackageTypeTemp);},
                TR.Value("common_data_sure_2"));
        }

        //检测消耗品是否足够
        public static void OnCheckMoneyIsEnough(CostItemManager.CostInfo costInfo, EPackageType ePackageType)
        {
            if (costInfo == null)
                return;

            //判断消耗品是否足够，如果足够，直接发送消息
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo,
                () => 
                {
                    if (ePackageType == EPackageType.RoleStorage)
                    {
                        StorageDataManager.GetInstance().OnSendSceneUnlockRoleStorageReq();
                    }
                    else if (ePackageType == EPackageType.Storage)
                    {
                        StorageDataManager.GetInstance().OnSendSceneUnlockAccountStorageReq();
                    }
                });
        }

        

        //得到解锁解锁仓库消耗的道具和数量
        public static StorageUnLockCostDataModel GetStorageUnLockCostDataModel(int totalOwnerNumber, EPackageType ePackageType)
        {
            int nextLockIndex = totalOwnerNumber + 1;
            if (nextLockIndex > StorageDataManager.RoleStorageTotalNumber)
            {
                return null;
            }

            ItemData itemData = null;
            if (ePackageType == EPackageType.Storage)
            {
                itemData = StorageDataManager.GetInstance().GetAccountExpandConsumeByIndex(nextLockIndex);
            }
            else if (ePackageType == EPackageType.RoleStorage)
            {
                itemData = StorageDataManager.GetInstance().GetRoleExpandConsumeByIndex(nextLockIndex);
            }

            //int nextLockIndex = totalOwnerNumber + 1;
            //if (nextLockIndex > StorageDataManager.RoleStorageTotalNumber)
            //    return null;

            //SystemValueTable.eType3 costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_2_COSTITEMID;
            //SystemValueTable.eType3 costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_2_COSTNUM;

            //if (nextLockIndex == 2)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_2_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_2_COSTNUM;

            //}
            //else if (nextLockIndex == 3)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_3_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_3_COSTNUM;
            //}
            //else if (nextLockIndex == 4)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_4_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_4_COSTNUM;
            //}
            //else if (nextLockIndex == 5)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_5_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_5_COSTNUM;
            //}
            //else if (nextLockIndex == 6)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_6_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_6_COSTNUM;
            //}
            //else if (nextLockIndex == 7)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_7_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_7_COSTNUM;
            //}
            //else if (nextLockIndex == 8)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_8_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_8_COSTNUM;
            //}
            //else if (nextLockIndex == 9)
            //{
            //    costItemIdType3 = SystemValueTable.eType3.SVT_WAREHOUSE_9_COSTITEMID;
            //    costItemNumberType3 = SystemValueTable.eType3.SVT_WAREHOUSE_9_COSTNUM;
            //}

            //var costItemIdTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int) costItemIdType3);
            //var costItemNumberTable =
            //    TableManager.GetInstance().GetTableItem<SystemValueTable>((int) costItemNumberType3);

            //if (costItemIdTable == null || costItemNumberTable == null)
            //    return null;

            if (itemData == null)
            {
                return null;
            }

            StorageUnLockCostDataModel storageUnLockCostDataModel = new StorageUnLockCostDataModel()
            {
                CostItemId = itemData.TableID,
                CostItemNumber = itemData.Count,
            };

            return storageUnLockCostDataModel;
        }

        //得到仓库的名字
        public static string GetStorageNameByStorageIndex(int index, EPackageType packageType = EPackageType.RoleStorage)
        {
            string setNameStr = string.Empty;
            if (packageType == EPackageType.RoleStorage)
            {
                setNameStr = StorageDataManager.GetInstance().GetRoleStorageSetNameByRoleStorageIndex(index);
            }
            else if(packageType == EPackageType.Storage)
            {
                setNameStr = StorageDataManager.GetInstance().GetAccountStorageSetNameByRoleStorageIndex(index);
            }

            //名字设置过
            if (string.IsNullOrEmpty(setNameStr) == false)
                return setNameStr;

            //名字没有设置过，得到默认的名字

            var defaultNameStr = GetStorageDefaultNameByStorageIndex(index);
            return defaultNameStr;
        }

        public static string GetStorageDefaultNameByStorageIndex(int index)
        {
            if (index == 1)
            {
                return TR.Value("storage_free_store");
            }
            else
            {
                var defaultNameStr = TR.Value("storage_store_format", index);
                return defaultNameStr;
            }
        }

        public static string GetEnlargeStorageSizeCostDesc(CostItemManager.CostInfo costInfo)
        {
            var costMoneyDescStr = CostItemManager.GetInstance().GetCostMoneiesDesc(costInfo);

            if (string.IsNullOrEmpty(costMoneyDescStr) == false)
            {
                return TR.Value("storage_unlock_grids", costMoneyDescStr);
            }
            return "";
        }

        //得到账号仓库中某一页的道具GuidList
        private static List<ulong> GetAccountStorageItemGuidListByStorageIndex(int storageIndex)
        {
            var allItemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Storage);
            List<ulong> itemGuidList = new List<ulong>();

            var currentStorageIndex = storageIndex;
            if (currentStorageIndex <= 0)
            {
                currentStorageIndex = 1;
            }
            else if (currentStorageIndex > StorageDataManager.AccountStorageTotalNumber)
            {
                currentStorageIndex = StorageDataManager.AccountStorageTotalNumber;
            }

            //0-29；30-59；60-89
            var minGridIndex = GetRoleStorageItemGridMinGridIndex(currentStorageIndex);
            var maxGridIndex = GetRoleStorageItemGridMaxGridIndex(currentStorageIndex);

            for (var i = 0; i < allItemGuidList.Count; i++)
            {
                var curItemGuid = allItemGuidList[i];
                var curItemData = ItemDataManager.GetInstance().GetItem(curItemGuid);
                if (curItemData == null)
                    continue;

                if (curItemData.GridIndex < minGridIndex)
                    continue;

                if (curItemData.GridIndex > maxGridIndex)
                    continue;

                itemGuidList.Add(curItemGuid);
            }

            return itemGuidList;
        }

        //得到角色仓库中某一页的道具GuidList
        private static List<ulong> GetRoleStorageItemGuidListByStorageIndex(int storageIndex)
        {
            var allItemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.RoleStorage);
            List<ulong> itemGuidList = new List<ulong>();

            var currentStorageIndex = storageIndex;
            if (currentStorageIndex <= 0)
            {
                currentStorageIndex = 1;
            }
            else if (currentStorageIndex > StorageDataManager.RoleStorageTotalNumber)
            {
                currentStorageIndex = StorageDataManager.RoleStorageTotalNumber;
            }

            //0-29；30-59；60-89
            var minGridIndex = GetRoleStorageItemGridMinGridIndex(currentStorageIndex);
            var maxGridIndex = GetRoleStorageItemGridMaxGridIndex(currentStorageIndex);
            
            for (var i = 0; i < allItemGuidList.Count; i++)
            {
                var curItemGuid = allItemGuidList[i];
                var curItemData = ItemDataManager.GetInstance().GetItem(curItemGuid);
                if (curItemData == null)
                    continue;

                if (curItemData.GridIndex < minGridIndex)
                    continue;

                if (curItemData.GridIndex > maxGridIndex)
                    continue;

                itemGuidList.Add(curItemGuid);
            }

            return itemGuidList;
        }

        //得到数据模型
        public static List<StorageItemDataModel> GetStorageItemDataModelList(StorageType storageType,
            int storageIndex = 0)
        {
            List<StorageItemDataModel> storageItemDataModelList = new List<StorageItemDataModel>();

            int totalNumber = 0;
            List<ulong> itemGuidList = null;

            if (storageType == StorageType.AccountStorage)
            {
                totalNumber = PlayerBaseData.GetInstance().AccountStorageSize;
                itemGuidList = GetAccountStorageItemGuidListByStorageIndex(storageIndex);
            }
            else if (storageType == StorageType.RoleStorage)
            {
                totalNumber = PlayerBaseData.GetInstance().RoleStorageSize;
                itemGuidList = GetRoleStorageItemGuidListByStorageIndex(storageIndex);
            }

            for (var i = 0; i < totalNumber; i++)
            {
                var storageItemDataModel = new StorageItemDataModel();
                storageItemDataModelList.Add(storageItemDataModel);

                if (itemGuidList == null)
                    continue;

                if (itemGuidList.Count <= 0)
                    continue;

                if (i >= 0 && i < itemGuidList.Count)
                {
                    var itemGuid = itemGuidList[i];
                    storageItemDataModel.ItemGuid = itemGuid;
                }
            }

            return storageItemDataModelList;
        }

        //显示几个几个
        public static int GetNeedShowItemNumber(int totalNumber)
        {
            //1-2个，展示三个
            if (totalNumber < 3)
                return 3;

            //3,4,5,展示6个
            if (totalNumber < 6)
                return 6;

            //6, 6+ 展示9个
            return 9;
        }

        //显示几行
        public static int GetNeedShowLineNumber(int totalNumber)
        {
            if (totalNumber < 3)
                return 1;

            if (totalNumber < 6)
                return 2;

            return 3;
        }


        //得到仓库中，某个道具存储的数量
        public static int GetStorageItemCount(int tableId, EPackageType packageType)
        {
            if (packageType != EPackageType.Storage && packageType != EPackageType.RoleStorage)
                return 0;

            //var GetItemsByPackageType
            var itemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(packageType);
            if (itemGuidList == null || itemGuidList.Count <= 0)
                return 0;

            var storageItemCount = 0;

            for (var i = 0; i < itemGuidList.Count; i++)
            {
                var itemGuid = itemGuidList[i];
                var itemData = ItemDataManager.GetInstance().GetItem(itemGuid);

                if(itemData == null)
                    continue;

                if(itemData.TableID != tableId)
                    continue;

                storageItemCount += itemData.Count;
            }

            return storageItemCount;
        }

        //最小边界
        public static int GetRoleStorageItemGridMinGridIndex(int pageIndex)
        {
            var minIndex = (pageIndex - 1) * StorageDataManager.RoleStorageMaxGridInOnePage;
            return minIndex;
        }

        //最大边界
        public static int GetRoleStorageItemGridMaxGridIndex(int pageIndex)
        {
            var maxIndex = pageIndex * StorageDataManager.RoleStorageMaxGridInOnePage - 1;
            return maxIndex;
        }
        
        //对角色仓库中对应页数的道具进行排序,并缓存
        //首先分别按照顺序取出非本页的道具和本页的道具；对本页的道具进行排序，之后对数据进行合并和缓存
        public static void ResortRoleStorageItemGuidByGridIndex(int minGridIndex, int maxGridIndex)
        {
            //得到角色背包的数据
            var itemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.RoleStorage);
            if (itemGuidList == null || itemGuidList.Count <= 1)
                return;

            List<ulong> baseItemGuidList = new List<ulong>();
            List<ulong> curPageItemGuidList = new List<ulong>();

            for (var i = 0; i < itemGuidList.Count; i++)
            {
                var itemGuid = itemGuidList[i];
                var item = ItemDataManager.GetInstance().GetItem(itemGuid);
                if(item == null)
                    continue;

                //不是当前页的道具
                if (item.GridIndex < minGridIndex || item.GridIndex > maxGridIndex)
                {
                    baseItemGuidList.Add(itemGuid);
                }
                else
                {
                    //当前页的道具
                    curPageItemGuidList.Add(itemGuid);
                }
            }

            //将当前页的道具进行排序
            ItemDataUtility.ArrangeItemGuidList(curPageItemGuidList);

            //排序号的道具添加到对应的List中
            baseItemGuidList.AddRange(curPageItemGuidList);

            //更新角色仓库的数据
            ItemDataManager.GetInstance().UpdateItemGuidListByPackageType(EPackageType.RoleStorage, baseItemGuidList);
        }

        //道具的基础价格
        public static ulong GetBasePriceByItemData(ItemData itemData)
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemData.TableID);
            if (itemTable == null)
            {
                return 0;
            }

            var priceRate = GetEquipStrengthLvAdditionalPriceRate(itemData.StrengthenLevel);
            var basePriceValue = GetBasePrice(itemTable.RecommendPrice, priceRate);

            return basePriceValue;
        }

        private static ulong GetBasePrice(int recommendPrice, int iStrengthRate)
        {
            return (ulong)(recommendPrice * (100 + iStrengthRate) / 100.0f);
        }


        private static int GetEquipStrengthLvAdditionalPriceRate(int iStrengthLv)
        {
            if (iStrengthLv < 10)
            {
                return 0;
            }
            if (iStrengthLv == 10)
            {
                return 10;
            }
            else if (iStrengthLv == 11)
            {
                return 30;
            }
            else if (iStrengthLv == 12)
            {
                return 60;
            }
            else if (iStrengthLv == 13)
            {
                return 100;
            }
            else if (iStrengthLv == 14)
            {
                return 150;
            }
            else if (iStrengthLv == 15)
            {
                return 200;
            }
            else if (iStrengthLv == 16)
            {
                return 300;
            }
            else if (iStrengthLv == 17)
            {
                return 400;
            }
            else if (iStrengthLv == 18)
            {
                return 500;
            }
            else if (iStrengthLv == 19)
            {
                return 600;
            }
            else if (iStrengthLv == 20)
            {
                return 700;
            }
            else
            {
                return 800;
            }
        }

    }
}
