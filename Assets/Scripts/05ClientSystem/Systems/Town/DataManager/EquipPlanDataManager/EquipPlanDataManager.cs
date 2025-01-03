using System;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;


namespace GameClient
{

    //装备方案的数据模型
    public class EquipPlanDataModel
    {
        public ulong Guid;
        public EquipSchemeType EquipPlanType;
        public uint EquipPlanId;
        public bool IsWear;                         //是否穿戴
        public List<ulong> EquipItemGuidList = new List<ulong>();       //穿戴装备的列表
    }


    //装备方案数据管理器
    public class EquipPlanDataManager : DataManager<EquipPlanDataManager>
    {

        private int _equipPlanUnLockLevel = 0;           //装备方案解锁等级

        public int CurrentSelectedEquipPlanId = 0;              //当前的装备方案
        public bool IsAlreadySwitchEquipPlan = false;   //是否已经切换了装备方案，如果存在第二套方案的数据，说明已经切换过了

        //未启用装备方案的数据（只存在一种)
        public int UnSelectedEquipPlanId = 0;
        public List<ulong> UnSelectedEquipPlanItemGuidList = null;
        public List<ulong> CurrentSelectedEquipPlanItemGuidList = null;

        //装备方案的列表
        public List<EquipPlanDataModel> EquipPlanDataModelList = new List<EquipPlanDataModel>();

        private DisplayAttribute beforeDisplayAttribute = null;

        public const float EquipPlanSwitchCountDownInterval = 3;        //CountDownTime            
        public float EquipPlanSwitchCountDownLeftTime = 0;             //装备方案切换的时间戳
        private Action _updateCountDownTimeAction;

        #region Initialize
        public override void Initialize()
        {
            BindNetEvents();
        }

        public override void Clear()
        {
            UnBindNetEvents();
            ClearData();
        }

        private void ClearData()
        {
            ResetEquipPlanDataModel();
            _equipPlanUnLockLevel = 0;
            beforeDisplayAttribute = null;

            EquipPlanSwitchCountDownLeftTime = 0;
            ResetUpdateCountDownTimeAction();
        }

        private void BindNetEvents()
        {
            //装备方案同步
            NetProcess.AddMsgHandler(SceneEquipSchemeSync.MsgID, OnReceiveSceneEquipSchemeSync);

            //穿戴装备的返回
            NetProcess.AddMsgHandler(SceneEquipSchemeWearRes.MsgID, OnReceiveSceneEquipSchemeWearRes);
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneEquipSchemeSync.MsgID, OnReceiveSceneEquipSchemeSync);

            NetProcess.RemoveMsgHandler(SceneEquipSchemeWearRes.MsgID, OnReceiveSceneEquipSchemeWearRes);
        }

        #endregion

        //装备方案同步消息
        private void OnReceiveSceneEquipSchemeSync(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneEquipSchemeSync sceneEquipSchemeSync = new SceneEquipSchemeSync();
            sceneEquipSchemeSync.decode(msgData.bytes);

            if (sceneEquipSchemeSync.schemes == null || sceneEquipSchemeSync.schemes.Length <= 0)
                return;

            if (EquipPlanDataModelList == null)
                EquipPlanDataModelList = new List<EquipPlanDataModel>();

            //同步到本地
            for (var i = 0; i < sceneEquipSchemeSync.schemes.Length; i++)
            {
                var schemeInfo = sceneEquipSchemeSync.schemes[i];
                if(schemeInfo == null)
                    continue;

                //查找对象是否已经保存
                EquipPlanDataModel equipPlanDataModel = null;
                for (var j = 0; j < EquipPlanDataModelList.Count; j++)
                {
                    var curEquipPlanDataModel = EquipPlanDataModelList[j];
                    if(curEquipPlanDataModel == null)
                        continue;

                    //找到，赋值，结束
                    if (curEquipPlanDataModel.EquipPlanId == schemeInfo.id)
                    {
                        equipPlanDataModel = curEquipPlanDataModel;
                        break;
                    }
                }

                //保存到本地
                if (equipPlanDataModel == null)
                {
                    equipPlanDataModel = EquipPlanUtility.CreateEquipPlanDataModel(schemeInfo);
                    EquipPlanDataModelList.Add(equipPlanDataModel);
                }
                else
                {
                    EquipPlanUtility.UpdateEquipPlanDataModel(equipPlanDataModel, schemeInfo);
                }
            }

            //数据同步前的装备方案
            int preSelectedEquipPlanId = CurrentSelectedEquipPlanId;
            
            //相关数据更新
            IsAlreadySwitchEquipPlan = false;
            UnSelectedEquipPlanId = 0;
            UnSelectedEquipPlanItemGuidList = null;
            CurrentSelectedEquipPlanItemGuidList = null;
            
            for (var i = 0; i < EquipPlanDataModelList.Count; i++)
            {
                var curEquipPlanDataModel = EquipPlanDataModelList[i];
                if(curEquipPlanDataModel == null)
                    continue;

                //当前启用的装备方案
                if (curEquipPlanDataModel.IsWear == true)
                {
                    CurrentSelectedEquipPlanId = (int)curEquipPlanDataModel.EquipPlanId;
                    CurrentSelectedEquipPlanItemGuidList = curEquipPlanDataModel.EquipItemGuidList;
                }

                //第二套方案是否已经启用
                if (curEquipPlanDataModel.EquipPlanType == EquipSchemeType.EQST_EQUIP
                    && curEquipPlanDataModel.EquipPlanId == 2)
                    IsAlreadySwitchEquipPlan = true;

                //未启用方案的装备
                if (curEquipPlanDataModel.IsWear == false)
                {
                    UnSelectedEquipPlanId = (int) curEquipPlanDataModel.EquipPlanId;
                    UnSelectedEquipPlanItemGuidList = curEquipPlanDataModel.EquipItemGuidList;
                }
            }

            if (preSelectedEquipPlanId != CurrentSelectedEquipPlanId)
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveEquipPlanSwitchMessage);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveEquipPlanSyncMessage);
        }

        //发送穿戴消息的数据
        //isSync 0不同步，1同步
        public void OnSendSceneEquipSchemeWearReq(int equipPlanId, 
            bool isSync, 
            EquipSchemeType equipSchemeType = EquipSchemeType.EQST_EQUIP)
        {
            beforeDisplayAttribute = BeUtility.GetMainPlayerActorAttribute();

            SceneEquipSchemeWearReq sceneEquipSchemeWearReq = new SceneEquipSchemeWearReq();

            sceneEquipSchemeWearReq.id = (uint)equipPlanId;
            sceneEquipSchemeWearReq.isSync = isSync == true ? (byte)1 : (byte)0;

            sceneEquipSchemeWearReq.type = (byte)equipSchemeType;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneEquipSchemeWearReq);

            //切换装备，设置CD
            EquipPlanSwitchCountDownLeftTime = EquipPlanSwitchCountDownInterval;
            DoUpdateCountDownTimeAction();
        }

        //穿戴装备的返回
        private void OnReceiveSceneEquipSchemeWearRes(MsgDATA msgData)
        {
            if (msgData == null)
            {
                beforeDisplayAttribute = null;
                return;
            }
            
            SceneEquipSchemeWearRes sceneEquipSchemeWearRes = new SceneEquipSchemeWearRes();
            sceneEquipSchemeWearRes.decode(msgData.bytes);

            //方案替换不成功
            if (sceneEquipSchemeWearRes.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) sceneEquipSchemeWearRes.code);
                beforeDisplayAttribute = null;
                return;
            }

            //方案替换成功的飘字
            var firstEquipPlanIdStr = EquipPlanUtility.GetEquipPlanIdStr(1);
            var secondEquipPlanIdStr = EquipPlanUtility.GetEquipPlanIdStr(2);

            var contentStr = "";

            //穿戴第二套方案
            if (sceneEquipSchemeWearRes.id == 2)
            {
                //同步第一套方案
                if (sceneEquipSchemeWearRes.isSync == 1)
                {
                    contentStr = TR.Value("Equip_Plan_First_Used_Plan_Two_And_Sync_Tip",
                        secondEquipPlanIdStr,
                        firstEquipPlanIdStr);
                }
                else
                {
                    //直接启用第二套方案
                    contentStr = TR.Value("Equip_Plan_Used_Plan_Tip", secondEquipPlanIdStr);
                }
            }
            else if (sceneEquipSchemeWearRes.id == 1)
            {
                //穿戴第一套方案
                contentStr = TR.Value("Equip_Plan_Used_Plan_Tip", firstEquipPlanIdStr);
            }

            SystemNotifyManager.SysNotifyFloatingEffect(contentStr);

            ShowDisplayAttributeChangeFloatingEffect();


            //判断是否存在时效的装备
            DealWithEndTimeItemList(sceneEquipSchemeWearRes.overdueIds);

        }

        private void DealWithEndTimeItemList(ulong[] overDueIds)
        {
            if (overDueIds == null || overDueIds.Length <= 0)
                return;

            //是否存在标志位
            var isFindDeleteItem = false;

            for (var i = 0; i < overDueIds.Length; i++)
            {
                var curDeleteItemGuid = overDueIds[i];
                if (curDeleteItemGuid <= 0)
                    continue;

                var itemData = ItemDataManager.GetInstance().GetItem(curDeleteItemGuid);
                if (itemData == null)
                    continue;

                if (itemData.IsItemInUnUsedEquipPlan == false)
                    continue;

                //道具在装备方案中已经删除，道具的IsItemInUnUsedEquipPlan标志重置
                itemData.IsItemInUnUsedEquipPlan = false;
                isFindDeleteItem = true;
            }

            //存在装备方案中删除了道具导致标志重置的道具
            if (isFindDeleteItem == true)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveEquipPlanItemEndTimeMessage);
            }
        }

        //飘字的逻辑
        private void ShowDisplayAttributeChangeFloatingEffect()
        {
            bool isOwnerDifferentItem = EquipPlanUtility.IsEquipPlanOwnerDifferentItem();
            if (isOwnerDifferentItem == false)
            {
                beforeDisplayAttribute = null;
                return;
            }

            var afterDisplayAttribute = BeUtility.GetMainPlayerActorAttribute();
            var betterEquipmentDataList = ItemDataUtility.GetPlayerAttributeDisplayChangeList(beforeDisplayAttribute,
                afterDisplayAttribute);
            
            //飘字特效
            if(betterEquipmentDataList != null)
                ItemDataManager.GetInstance().PopUpChangedAttrbutes(betterEquipmentDataList);

            beforeDisplayAttribute = null;
            afterDisplayAttribute = null;
        }

        //重置相关数值
        private void ResetEquipPlanDataModel()
        {

            CurrentSelectedEquipPlanId = 0;

            UnSelectedEquipPlanId = 0;
            UnSelectedEquipPlanItemGuidList = null;
            CurrentSelectedEquipPlanItemGuidList = null;

            IsAlreadySwitchEquipPlan = false;

            if (EquipPlanDataModelList == null || EquipPlanDataModelList.Count <= 0)
                return;

            for (var i = 0; i < EquipPlanDataModelList.Count; i++)
            {
                var equipPlanDataModel = EquipPlanDataModelList[i];
                if(equipPlanDataModel == null)
                    continue;

                if(equipPlanDataModel.EquipItemGuidList != null && equipPlanDataModel.EquipItemGuidList.Count > 0)
                    equipPlanDataModel.EquipItemGuidList.Clear();
            }

            EquipPlanDataModelList.Clear();
        }

        //切换到第二套方案并同步第一套方案的数据
        public void OnSwitchEquipPlanWithSyncFirstEquipPlan()
        {
            var switchToEquipPlanId = EquipPlanUtility.GetSwitchToEquipPlanId();
            OnSendSceneEquipSchemeWearReq(switchToEquipPlanId,
                true);
        }

        //正常的操作，切换武器方案
        public void OnSwitchEquipPlanByCommonAction()
        {
            var switchToEquipPlanId = EquipPlanUtility.GetSwitchToEquipPlanId();
            OnSendSceneEquipSchemeWearReq(switchToEquipPlanId,
                false);
        }

        public int GetEquipPlanUnLockLevel()
        {
            if (_equipPlanUnLockLevel > 0)
                return _equipPlanUnLockLevel;

            var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                (int) SystemValueTable.eType3.SVT_EQUIP_PLAN_FUNCTION_UNLOCK_LV);

            //默认为50级解锁
            _equipPlanUnLockLevel = systemValueTable == null ? 50 : systemValueTable.Value;

            return _equipPlanUnLockLevel;
        }

        #region Update

        public void SetUpdateCountDownTimeAction(Action updateCountDownTimeAction)
        {
            _updateCountDownTimeAction = updateCountDownTimeAction;
        }

        public void ResetUpdateCountDownTimeAction()
        {
            _updateCountDownTimeAction = null;
        }

        private void DoUpdateCountDownTimeAction()
        {
            if (_updateCountDownTimeAction != null)
                _updateCountDownTimeAction();
        }

        public override void Update(float time)
        {
            if (EquipPlanSwitchCountDownLeftTime <= 0)
                return;

            EquipPlanSwitchCountDownLeftTime -= time;

            if (EquipPlanSwitchCountDownLeftTime <= 0)
                EquipPlanSwitchCountDownLeftTime = 0;

            DoUpdateCountDownTimeAction();
        }
        
        #endregion

    }
}
