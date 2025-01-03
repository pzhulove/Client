using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;


namespace GameClient
{
    public class StorageDataManager : DataManager<StorageDataManager>
    {

        //仓库中当前选中的仓库类型
        public StorageType CurrentStorageType;

        //解锁数量的Count字符串
        public const string RoleStorageUnLockNumberStr = "role_storage_open_num";
        private int _roleStorageOwnerStorageNumber = 0;      //角色仓库拥有的仓库数量（解锁的数量 + 1个免费的数量）

        //角色仓库的索引从1-9
        public const int RoleStorageTotalNumber = 9;            //角色仓库的最大值
        public const int RoleStorageNameMaxNumber = 5;          //仓库名字的最大数值
        public const int RoleStorageMaxGridInOnePage = 30;    //角色仓库中一页的数量

        private int _roleStorageCurrentSelectedIndex = 0;     //角色仓库当前选择的索引

        public const string AccountStorageUnLockNumberStr = "account_storage_open_num";
        private int _accountStorageOwnerStorageNumber = 0;      //账号仓库拥有的仓库数量（解锁的数量 + 1个免费的数量）
        private int _accountStorageCurrentSelectedIndex = 0;     //账号仓库当前选择的索引

        //账号仓库的索引从1-9
        public const int AccountStorageTotalNumber = 9;            //账号仓库的最大值
        public const int AccountStorageNameMaxNumber = 5;          //仓库名字的最大数值
        public const int AccountStorageMaxGridInOnePage = 30;    //账号仓库中一页的数量
        //账号仓库的个性化名字(用户修改的名字)
        private Dictionary<int, string> _accountStorageSetNameDic = new Dictionary<int, string>();

        //角色仓库的个性化名字(用户修改的名字)
        private Dictionary<int, string> _roleStorageSetNameDic = new Dictionary<int, string>();

        private Dictionary<int, ItemData> _roleStorageExpandConsumeDic = new Dictionary<int, ItemData>();  //key 第几个仓库  value拓展花费
        private Dictionary<int, ItemData> _accountStorageExpandConsumeDic = new Dictionary<int, ItemData>();  //key 第几个仓库  value拓展花费


        public override void Initialize()
        {
            BindEvents();
        }

        public override void Clear()
        {
            ClearData();
            UnBindEvents();
        }

        private void ClearData()
        {
            _roleStorageCurrentSelectedIndex = 0;
            _roleStorageOwnerStorageNumber = 0;
            _roleStorageSetNameDic.Clear();

            _accountStorageCurrentSelectedIndex = 0;
            _accountStorageOwnerStorageNumber = 0;
            _accountStorageSetNameDic.Clear();

            CurrentStorageType = StorageType.RoleStorage;
        }


        private void BindEvents()
        {
            NetProcess.AddMsgHandler(SceneEnlargeStorageRet.MsgID, OnReceiveSceneUnLockStorageRes);
            NetProcess.AddMsgHandler(ScenePushStorageRet.MsgID, OnReceiveStoreItemRes);
            NetProcess.AddMsgHandler(SceneMoveItemRes.MsgID, OnReceiveSceneMoveItemRes);
            //NetProcess.AddMsgHandler(SceneEnlargeStorageRet.MsgID, OnReceiveSceneUnLockAccountStorageRes);
            
        }

        private void UnBindEvents()
        {
            NetProcess.RemoveMsgHandler(SceneEnlargeStorageRet.MsgID, OnReceiveSceneUnLockStorageRes);
            NetProcess.RemoveMsgHandler(ScenePushStorageRet.MsgID, OnReceiveStoreItemRes);
            NetProcess.RemoveMsgHandler(SceneMoveItemRes.MsgID, OnReceiveSceneMoveItemRes);
            //NetProcess.RemoveMsgHandler(SceneEnlargeStorageRet.MsgID, OnReceiveSceneUnLockAccountStorageRes);
        }

        private int _SortExpandTableList(PackageEnlargeTable first, PackageEnlargeTable second)
        {
            return first.GridSize - second.GridSize;
        }

        private ItemData _ParseConsume(PackageEnlargeTable tableData)
        {
            var strs = tableData.Consume.Split('_');
            if (strs.Length == 2)
            {
                int costId = 0;
                int costCount = 0;
                if (int.TryParse(strs[0], out costId) && int.TryParse(strs[1], out costCount))
                {
                    ItemData itemData = new ItemData(costId);
                    itemData.Count = costCount;

                    return itemData;
                }
            }

            return null;
        }

        #region StoreItem
        //将道具存入到仓库
        public void OnSendStoreItemReq(ItemData item, int count)
        {
            if (item == null)
                return;

            if (count <= 0)
                return;

            bool bCanStoreItem = false;
            
            //存储类型
            StorageType storageType = CurrentStorageType;

            //默认的账号仓库
            EPackageType packageType = EPackageType.Storage;
            //角色仓库
            if (storageType == StorageType.RoleStorage)
                packageType = EPackageType.RoleStorage;

            if (storageType == StorageType.AccountStorage)
            {
                if (item.HasTransfered
                    || item.BindAttr == ItemTable.eOwner.NOTBIND
                    || item.BindAttr == ItemTable.eOwner.ACCBIND
                    || (item.Packing && item.BindAttr == ItemTable.eOwner.ROLEBIND))
                {
                    bCanStoreItem = true;
                }

                packageType = EPackageType.Storage;
            }
            else if (storageType == StorageType.RoleStorage)
            {
                bCanStoreItem = true;
            }

            if (bCanStoreItem == false)
                return;

            //判断是否超过上限
            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int) item.TableID);
            if (itemTableData != null)
            {
                if (itemTableData.GetLimitNum != 0)
                {
                    var itemStorageCount = StorageUtility.GetStorageItemCount((int) item.TableID, packageType);

                    //存储超过上限
                    if (itemTableData.GetLimitNum < itemStorageCount + count)
                    {
                        SystemNotifyManager.SystemNotify(9104);
                        return;
                    }
                }
            }

            //账号仓库还是按照原来的协议
            if (storageType == StorageType.AccountStorage)
            {
                ScenePushStorage scenePushStorage = new ScenePushStorage();
                scenePushStorage.uid = item.GUID;
                scenePushStorage.targetPos.type = (byte)packageType;
                scenePushStorage.num = (ushort)count;
                //scenePushStorage.storageType = (byte)packageType;

                var accountStorageCurrentSelectedIndex = GetAccountStorageCurrentSelectedIndex();
                //第一页0；第二页30；第三页60；共9页
                var posIndex = (accountStorageCurrentSelectedIndex - 1) * AccountStorageMaxGridInOnePage;
                if (posIndex < 0)
                    posIndex = 0;

                scenePushStorage.targetPos = new ItemPos()
                {
                    type = (byte)packageType,
                    index = (uint)posIndex,
                };

                //发送存储的消息
                var netMgr = NetManager.Instance();
                if (netMgr != null)
                    netMgr.SendCommand(ServerType.GATE_SERVER, scenePushStorage);
            }
            else
            {
                //角色仓库使用新的协议
                ScenePushStorage sceneMoveItem = new ScenePushStorage();
                sceneMoveItem.uid = item.GUID;
                sceneMoveItem.num = (ushort) count;

                var roleStorageCurrentSelectedIndex = GetRoleStorageCurrentSelectedIndex();
                //第一页0；第二页30；第三页60；共9页
                var posIndex = (roleStorageCurrentSelectedIndex - 1) * RoleStorageMaxGridInOnePage;
                if (posIndex < 0)
                    posIndex = 0;

                sceneMoveItem.targetPos = new ItemPos()
                {
                    type = (byte) packageType,
                    index = (uint)posIndex,
                };

                var netMgr = NetManager.Instance();
                if (netMgr != null)
                    netMgr.SendCommand(ServerType.GATE_SERVER, sceneMoveItem);
            }
        }

        //存储成功（针对账号仓库）
        private void OnReceiveStoreItemRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            ScenePushStorageRet scenePushStorageRet = new ScenePushStorageRet();
            scenePushStorageRet.decode(msgData.bytes);

            //存储不成功
            if (scenePushStorageRet.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) scenePushStorageRet.code);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemStoreSuccess);
        }

        //角色仓库存储成功
        private void OnReceiveSceneMoveItemRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneMoveItemRes sceneMoveItemRes = new SceneMoveItemRes();
            sceneMoveItemRes.decode(msgData.bytes);

            if (sceneMoveItemRes.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) sceneMoveItemRes.code);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemStoreSuccess);
        }

        #endregion

        #region UnLockStorage
        //解锁仓库
        public void OnSendSceneUnlockRoleStorageReq()
        {
            SceneEnlargeStorage sceneUnLockRoleStorageReq = new SceneEnlargeStorage();
            sceneUnLockRoleStorageReq.itemEnlargeType = (byte)ItemEnlargeType.ITEM_ENLARGE_TYPE_STORAGE;
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneUnLockRoleStorageReq);
        }

        //解锁仓库返回
        private void OnReceiveSceneUnLockStorageRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneEnlargeStorageRet sceneUnlockRoleStorageRes = new SceneEnlargeStorageRet();
            sceneUnlockRoleStorageRes.decode(msgData.bytes);

            //解锁不成功
            if (sceneUnlockRoleStorageRes.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) sceneUnlockRoleStorageRes.code);
                return;
            }
            //总解锁数量，缓存到counter
            var unlockNumber = sceneUnlockRoleStorageRes.curOpenNum;
            if (sceneUnlockRoleStorageRes.itemEnlargeType == (byte)ItemEnlargeType.ITEM_ENLARGE_TYPE_STORAGE)
            {
                CountDataManager.GetInstance().SetCountWithoutUiEvent(RoleStorageUnLockNumberStr,
                    unlockNumber);

                //解锁成功，进行飘字,总拥有的数量
                var roleStorageOwnerNumber = GetRoleStorageOwnerStorageNumber();

                //解锁的最后一个
                var unlockStorageIndex = roleStorageOwnerNumber;
                //飘字
                var unlockStorageName = StorageUtility.GetStorageDefaultNameByStorageIndex(unlockStorageIndex);
                var tipStr = TR.Value("storage_unlock_name_format", unlockStorageName);
                SystemNotifyManager.SysNotifyFloatingEffect(tipStr);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveStorageUnlockMessage, roleStorageOwnerNumber,
                    unlockStorageIndex);
            }
            else if(sceneUnlockRoleStorageRes.itemEnlargeType == (byte)ItemEnlargeType.ITEM_ENLARGE_TYPE_ACCOUNT_STORAGE)
            {
                SetAccountStorageOwnerStorageNumber((int)unlockNumber);

                //解锁成功，进行飘字,总拥有的数量
                var accountStorageOwnerNumber = GetAccountStorageOwnerStorageNumber();

                //解锁的最后一个
                var unlockStorageIndex = accountStorageOwnerNumber;
                //飘字
                var unlockStorageName = StorageUtility.GetStorageDefaultNameByStorageIndex(unlockStorageIndex);
                var tipStr = TR.Value("storage_unlock_name_format", unlockStorageName);
                SystemNotifyManager.SysNotifyFloatingEffect(tipStr);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveStorageUnlockMessage, accountStorageOwnerNumber,
                    unlockStorageIndex);
            }
        }
        #endregion

        #region SetRoleStorageName

        //缓存设置的名字
        public void UpdateRoleStorageSetNameByRoleStorageIndex(int index, string name)
        {
            if (_roleStorageSetNameDic == null)
                _roleStorageSetNameDic = new Dictionary<int, string>();

            _roleStorageSetNameDic[index] = name;
        }

        //得到设置的名字
        public string GetRoleStorageSetNameByRoleStorageIndex(int index)
        {
            if (_roleStorageSetNameDic == null || _roleStorageSetNameDic.Count <= 0)
                return "";

            if (_roleStorageSetNameDic.ContainsKey(index) == true)
                return _roleStorageSetNameDic[index];

            return "";
        }
        #endregion



        #region RoleStorageOwnerNumber

        //角色仓库拥有的数量
        public int GetRoleStorageOwnerStorageNumber()
        {
            //解锁的数量
            var unLockRoleStorageNumber = CountDataManager.GetInstance().GetCount(RoleStorageUnLockNumberStr);
            ////todo test
            //unLockRoleStorageNumber = 5;
            ////todo test
            //总数量
            _roleStorageOwnerStorageNumber = unLockRoleStorageNumber + 1;

            if (_roleStorageOwnerStorageNumber <= 0)
                return 1;
            else if (_roleStorageOwnerStorageNumber >= RoleStorageTotalNumber)
            {
                return RoleStorageTotalNumber;
            }
            else
            {
                return _roleStorageOwnerStorageNumber;
            }
        }

        public ItemData GetRoleExpandConsumeByIndex(int index)
        {
            if (_roleStorageExpandConsumeDic == null || _roleStorageExpandConsumeDic.Count == 0)
            {
                List<PackageEnlargeTable> packageEnlargeTables = new List<PackageEnlargeTable>();
                var packageEnlargeTabledic = TableManager.GetInstance().GetTable<PackageEnlargeTable>();
                foreach (var v in packageEnlargeTabledic.Values)
                {
                    if (v == null)
                    {
                        continue;
                    }

                    PackageEnlargeTable packageEnlargeTable = v as PackageEnlargeTable;
                    if (packageEnlargeTable.ItemEnlargeType != PackageEnlargeTable.eItemEnlargeType.ITEM_ENLARGE_TYPE_STORAGE)
                    {
                        continue;
                    }

                    packageEnlargeTables.Add(packageEnlargeTable);
                }

                packageEnlargeTables.Sort(_SortExpandTableList);
                for (int i = 0; i < packageEnlargeTables.Count; i++)
                {
                    _roleStorageExpandConsumeDic.Add(i + 2, _ParseConsume(packageEnlargeTables[i]));
                }
            }

            if (_roleStorageExpandConsumeDic == null || _roleStorageExpandConsumeDic.Count == 0 || !_roleStorageExpandConsumeDic.ContainsKey(index))
            {
                return null;
            }

            return _roleStorageExpandConsumeDic[index];
        }

        /// <summary>
        /// 账号仓库解锁数量
        /// </summary>
        /// <param name="count"></param>
        public void SetAccountStorageOwnerStorageNumber(int count)
        {
            CountDataManager.GetInstance().SetCountWithoutUiEvent(AccountStorageUnLockNumberStr, (uint)count);
        }

        //账号仓库拥有的数量
        public int GetAccountStorageOwnerStorageNumber()
        {
            //解锁的数量
            var unLockAccountStorageNumber = CountDataManager.GetInstance().GetCount(AccountStorageUnLockNumberStr);
            ////todo test
            //unLockRoleStorageNumber = 5;
            ////todo test
            //总数量
            _accountStorageOwnerStorageNumber = unLockAccountStorageNumber + 1;

            if (_accountStorageOwnerStorageNumber <= 0)
                return 1;
            else if (_accountStorageOwnerStorageNumber >= AccountStorageTotalNumber)
            {
                return AccountStorageTotalNumber;
            }
            else
            {
                return _accountStorageOwnerStorageNumber;
            }
        }
        #endregion

        #region SelectedIndex

        //角色仓库中设置当前选中的页签
        public void SetRoleStorageCurrentSelectedIndex(int index)
        {
            _roleStorageCurrentSelectedIndex = index;
        }

        //角色仓库中得到当前选中的页签
        public int GetRoleStorageCurrentSelectedIndex()
        {
            //默认为1
            if (_roleStorageCurrentSelectedIndex <= 0)
                return 1;
            //返回设置的数值
            return _roleStorageCurrentSelectedIndex;
        }


        #endregion


        #region AccountStorage

        //解锁仓库
        public void OnSendSceneUnlockAccountStorageReq()
        {
            SceneEnlargeStorage sceneUnLockAccountStorageReq = new SceneEnlargeStorage();
            sceneUnLockAccountStorageReq.itemEnlargeType = (byte)ItemEnlargeType.ITEM_ENLARGE_TYPE_ACCOUNT_STORAGE;
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneUnLockAccountStorageReq);
        }

        //账号仓库中设置当前选中的页签
        public void SetAccountStorageCurrentSelectedIndex(int index)
        {
            _accountStorageCurrentSelectedIndex = index;
        }

        //账号仓库中得到当前选中的页签
        public int GetAccountStorageCurrentSelectedIndex()
        {
            //默认为1
            if (_accountStorageCurrentSelectedIndex <= 0)
                return 1;
            //返回设置的数值
            return _accountStorageCurrentSelectedIndex;
        }

        //缓存设置的名字
        public void UpdateAccountStorageSetNameByRoleStorageIndex(int index, string name)
        {
            if (_accountStorageSetNameDic == null)
                _accountStorageSetNameDic = new Dictionary<int, string>();

            if (_accountStorageSetNameDic.ContainsKey(index))
            {
                _accountStorageSetNameDic[index] = name;
            }
            else
            {
                _accountStorageSetNameDic.Add(index, name);
            }
        }

        //得到设置的名字
        public string GetAccountStorageSetNameByRoleStorageIndex(int index)
        {
            if (_accountStorageSetNameDic == null || _accountStorageSetNameDic.Count <= 0)
                return string.Empty;

            if (_accountStorageSetNameDic.ContainsKey(index) == true)
                return _accountStorageSetNameDic[index];

            return string.Empty;
        }

        public ItemData GetAccountExpandConsumeByIndex(int index)
        {
            if (_accountStorageExpandConsumeDic == null || _accountStorageExpandConsumeDic.Count == 0)
            {
                List<PackageEnlargeTable> packageEnlargeTables = new List<PackageEnlargeTable>();
                var packageEnlargeTabledic = TableManager.GetInstance().GetTable<PackageEnlargeTable>();
                foreach (var v in packageEnlargeTabledic.Values)
                {
                    if (v == null)
                    {
                        continue;
                    }

                    PackageEnlargeTable packageEnlargeTable = v as PackageEnlargeTable;
                    if (packageEnlargeTable.ItemEnlargeType != PackageEnlargeTable.eItemEnlargeType.ITEM_ENLARGE_TYPE_ACCOUNT_STORAGE)
                    {
                        continue;
                    }

                    packageEnlargeTables.Add(packageEnlargeTable);
                }

                packageEnlargeTables.Sort(_SortExpandTableList);
                for (int i = 0; i < packageEnlargeTables.Count; i++)
                {
                    _accountStorageExpandConsumeDic.Add(i + 2, _ParseConsume(packageEnlargeTables[i]));
                }
            }

            if (_accountStorageExpandConsumeDic == null || _accountStorageExpandConsumeDic.Count == 0 || !_accountStorageExpandConsumeDic.ContainsKey(index))
            {
                return null;
            }

            return _accountStorageExpandConsumeDic[index];
        }
        #endregion
    }
}
