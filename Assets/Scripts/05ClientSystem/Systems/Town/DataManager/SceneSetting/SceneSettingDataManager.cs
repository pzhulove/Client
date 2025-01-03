using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;


namespace GameClient
{
    //通用的Key，value数据处理器
    public class SceneSettingDataManager : DataManager<SceneSettingDataManager>
    {
        
        public override void Initialize()
        {
            BindNetEvents();
        }

        public override void Clear()
        {
            ClearData();
            UnBindNetEvents();
        }

        private void ClearData()
        {
        }


        private void BindNetEvents()
        {
            //key value 设置的返回
            NetProcess.AddMsgHandler(SceneSetStorageNameRes.MsgID, OnReceiveSceneShortcutKeySetRes);
            //key value 设置的同步
            NetProcess.AddMsgHandler(SceneStorageNameSync.MsgID, OnReceiveSceneShortcutKeySetSync);

        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneSetStorageNameRes.MsgID, OnReceiveSceneShortcutKeySetRes);
            NetProcess.RemoveMsgHandler(SceneStorageNameSync.MsgID, OnReceiveSceneShortcutKeySetSync);
        }


        #region ShortcutKey

        //设置Key Value请求
        public void OnSendSceneShortcutKeySetReq(int type, string value, StorageType storageType = StorageType.RoleStorage)
        {
            int storageTypeTemp = type - 1;
            if (storageType == StorageType.AccountStorage)
            {
                storageTypeTemp = type + (int)StorageNameType.ACCOUNT_STORAGE_NAME_BEGIN - 1;
            }

            SceneSetStorageNameReq sceneShortcutKeyReq = new SceneSetStorageNameReq()
            {
                storageType = (ushort)storageTypeTemp,
                storageName = value,
            };

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneShortcutKeyReq);
        }

        //key value 设置的返回
        public void OnReceiveSceneShortcutKeySetRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneSetStorageNameRes sceneShortcutKeySetRes = new SceneSetStorageNameRes();
            sceneShortcutKeySetRes.decode(msgData.bytes);

            //修改不成功
            if (sceneShortcutKeySetRes.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)sceneShortcutKeySetRes.result);
                return;
            }

            //仓库的名字（1-9）
            if (sceneShortcutKeySetRes.storageType >= (int)StorageNameType.STORAGE_NAME_BEGIN && sceneShortcutKeySetRes.storageType <= (int)StorageNameType.STORAGE_NAME_END)
            {
                int key = (int)sceneShortcutKeySetRes.storageType + 1;
                //缓存名字
                StorageDataManager.GetInstance().UpdateRoleStorageSetNameByRoleStorageIndex(key,
                    sceneShortcutKeySetRes.storageName);
                //发送消息
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveStorageChangeNameMessage,
                    key);

                CommonUtility.OnCloseCommonSetContentFrame();
            }
            else if (sceneShortcutKeySetRes.storageType >= (int)StorageNameType.ACCOUNT_STORAGE_NAME_BEGIN && sceneShortcutKeySetRes.storageType <= (int)StorageNameType.ACCOUNT_STORAGE_NAME_END)
            {
                int key = (int)sceneShortcutKeySetRes.storageType - (int)StorageNameType.ACCOUNT_STORAGE_NAME_BEGIN + 1;
                //缓存名字
                StorageDataManager.GetInstance().UpdateAccountStorageSetNameByRoleStorageIndex(key,
                    sceneShortcutKeySetRes.storageName);

                //发送消息
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveStorageChangeNameMessage,
                    key);

                CommonUtility.OnCloseCommonSetContentFrame();
            }

        }

        //key value 同步
        public void OnReceiveSceneShortcutKeySetSync(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneStorageNameSync sceneShortcutKeySetSync = new SceneStorageNameSync();
            sceneShortcutKeySetSync.decode(msgData.bytes);

            if (sceneShortcutKeySetSync.infos == null
               || sceneShortcutKeySetSync.infos.Length <= 0)
                return;

            for (var i = 0; i < sceneShortcutKeySetSync.infos.Length; i++)
            {
                var curInfo = sceneShortcutKeySetSync.infos[i];
                if (curInfo == null)
                    continue;

                //仓库名字
                if (curInfo.type >= (int)StorageNameType.STORAGE_NAME_BEGIN && curInfo.type <= (int)StorageNameType.STORAGE_NAME_END)
                {
                    StorageDataManager.GetInstance().UpdateRoleStorageSetNameByRoleStorageIndex(
                         curInfo.type + 1,
                        curInfo.name);
                }
                else if(curInfo.type >= (int)StorageNameType.ACCOUNT_STORAGE_NAME_BEGIN && curInfo.type <= (int)StorageNameType.ACCOUNT_STORAGE_NAME_END)
                {
                    StorageDataManager.GetInstance().UpdateAccountStorageSetNameByRoleStorageIndex(
                         curInfo.type - (int)StorageNameType.ACCOUNT_STORAGE_NAME_BEGIN + 1,
                        curInfo.name);
                }

            }
        }
        
        #endregion
        
    }
}
