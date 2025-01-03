using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using Network;

namespace GameClient
{
    public enum StrengthenTicketMergeBuffType
    {
        None = 0,
        MaterialDiscount,
    }

    public class StrengthenTicketMergeItemData
    {
        public ItemSimpleData tempItemData;
        public int rawItemCount;
    }

    //下面是合成数据结构
    public class StrengthenTicketMergeMaterial
    {
        public int mergeTableId;
        public List<StrengthenTicketMergeItemData> needMaterialDatas = new List<StrengthenTicketMergeItemData>();
        public StrengthenTicketMergeMaterial()
        {
            Clear();
        }

        public void Clear()
        {
            if (needMaterialDatas != null)
            {
                needMaterialDatas.Clear();
            }
        }
    }

    public class StrengthenTicketMaterialMergeModel
    {
        public int mergeTableId;
        public string name;
        public int strengthenLevel;
        public bool isBind;
        public int increaseLevel;
        public int previewMinPercent;
        public int previewMaxPercent;
        public int displayItemTableId;
        public bool bDisplay;                                                  //是否需要展示
        public StrengthenTicketMergeMaterial needMaterialModel = new StrengthenTicketMergeMaterial();

        public StrengthenTicketMaterialMergeModel()
        {
            Clear();
        }

        public void Clear()
        {
            if (needMaterialModel != null)
            {
                needMaterialModel.Clear();
            }
        }
    }

    /// <summary>
    /// 材料合成强化券 一大类的增幅
    /// </summary>
    public class StrengthenTicketMaterialMergeIncreaseLevel
    {
        public int displayItemTableId;
        public string name;
        public int strengthenLevel;
        public bool isBind;
        public List<int> mergeTableIds = new List<int>();
        public List<int> increaseLevels = new List<int>();

        public StrengthenTicketMaterialMergeIncreaseLevel()
        {
            Clear();
        }

        public void Clear()
        {
            if (mergeTableIds != null)
            {
                mergeTableIds.Clear();
            }
            if (increaseLevels != null)
            {
                increaseLevels.Clear();
            }
        }
    }

    //下面是融合数据结构
    public class StrengthenTicketFuseSpecialMaterial
    {
        public ItemSimpleData fuseNeedItemData = new ItemSimpleData();
        public int limitStrengthenLevelMin;                 //融合材料最小可用范围
        public int limitStrengthenLevelMax;                 //融合材料最大可用范围

        public StrengthenTicketFuseSpecialMaterial()
        {
            Clear();
        }
        public void Clear()
        {
            fuseNeedItemData = new ItemSimpleData();
        }
    }

    public class StrengthenTicketFuseItemData
    {
        public ItemData ticketItemData = null;
        public int fuseValue;
        public bool canFuse;
        public float sProbabilityConvert;                   //强化概率 (取表格数据 并/1000)                
        public int sLevel;                                  //强化等级
        public int bNotBindInt;                                  //是否绑定

        //private int _fuseReadyCount;
        //public int fuseReadyCount                           //准备融合数量，不选择默认为0
        //{
        //    get {
        //        if (_fuseReadyCount < 0) 
        //            _fuseReadyCount = 0;
        //        return _fuseReadyCount; 
        //    }
        //    set { _fuseReadyCount = value; }
        //}                   

        public StrengthenTicketFuseItemData ()
	    {
            Clear();
	    }

        public void Clear()
        {
             ticketItemData = null;
             //_fuseReadyCount = 0;
        }
    }

    public class StrengthenTicketMaterialFuseModel
    {
        public List<StrengthenTicketFuseSpecialMaterial> materialModels = new List<StrengthenTicketFuseSpecialMaterial>(); //表示能参与强化的材料
        public List<StrengthenTicketFuseItemData> ticketModels = new List<StrengthenTicketFuseItemData>();

        public int predictMinLevel;
        public int predictMaxLevel;
        public int perdictMinPercent;                           //预计最小产出属性百分比 (*100)
        public int perdictMaxPercent;

        public StrengthenTicketMaterialFuseModel ()
	    {
            Clear();
	    }

        public void Clear()
        {
            if (materialModels != null)
            {
                for (int i = 0; i < materialModels.Count; i++)
                {
                    if (materialModels[i] != null)
                        materialModels[i].Clear();                    
                }
                materialModels.Clear();
            }
            if(ticketModels != null)
            {
                for (int i = 0; i < ticketModels.Count; i++)
                {
                    if(ticketModels[i] != null)
                        ticketModels[i].Clear();
                }
                ticketModels.Clear();
            }
        }
    }

    public class StrengthenTicketMergeDataManager : DataManager<StrengthenTicketMergeDataManager>
    {        
        #region Model Params

        public const int OP_ACTIVITY_TASKID_TO_ACTID = 1000;

        //BUFF祈福活动 可能有多个活动一起下发  只存开着的活动
        private Dictionary<uint, OpActivityData> openedBuffPrayActivityDatas = new Dictionary<uint, OpActivityData>();
        //全部活动任务状态
        private Dictionary<uint, OpActTask> totalActivityTasks = new Dictionary<uint, OpActTask>();

        private bool bFuncOpen;
        public bool BFuncOpen {
            //get { return bFuncOpen && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.StrengthenTicketMerge); }
            get { return bFuncOpen; }
        }
        public bool BSyntheticFrameTip = false;//材料合成是否弹框提示玩家
        public bool PrayActivityIsFinish = true;//祈福活动是否结束
        private int fuseLimitLevel_Max; //强化券融合限制强化券等级 上线 应该 <=
        public int FuseLimitLevel_Max {
            get { return fuseLimitLevel_Max; }        
        }

        private int fuseLimitLevel_Min; //强化券融合限制强化券等级 下线 应该 >
        public int FuseLimitLevel_Min
        {
            get { return fuseLimitLevel_Min; }
        }

        private int ticketFuseReadyCapacity = 2;//强化券融合准备容量
        public int TicketFuseReadyCapacity{
            get { return ticketFuseReadyCapacity; }
        }       

        //合成
        private List<StrengthenTicketMaterialMergeModel> m_MaterialMergeTicketModels = new List<StrengthenTicketMaterialMergeModel>();
        //展示用
        private List<StrengthenTicketMaterialMergeModel> m_DisplayMaterialMergeTicketModels = new List<StrengthenTicketMaterialMergeModel>();
        //选择增幅
        private List<StrengthenTicketMaterialMergeIncreaseLevel> m_MaterialMergeIncreaseLevelModels = new List<StrengthenTicketMaterialMergeIncreaseLevel>();


        //材料合成
        private StrengthenTicketMaterialMergeModel currSelectMaterialMergeModel = new StrengthenTicketMaterialMergeModel();
        public StrengthenTicketMaterialMergeModel CurrSelectMaterialMergeModel { 
            get {
				if(currSelectMaterialMergeModel == null)
				{
					return new StrengthenTicketMaterialMergeModel();
				}
                return currSelectMaterialMergeModel; 
            }
        }

        //券融合
        private List<StrengthenTicketFuseSpecialMaterial> m_FuseMaterialModels = new List<StrengthenTicketFuseSpecialMaterial>();
        private StrengthenTicketMaterialFuseModel tempMaterialFuseModel = new StrengthenTicketMaterialFuseModel();
        public StrengthenTicketMaterialFuseModel TempMaterialFuseModel {
            get
            {
                if (tempMaterialFuseModel == null)
                {
                    return new StrengthenTicketMaterialFuseModel();
                }
                return tempMaterialFuseModel;
            }
        }

        /// <summary>
        ///         
        ///融合计算产出概率区间  区间公式中用到的修正值 ( min( m / Mathf.Pow(4,b-a) + n , 1 ) ) / (b+1)的修正值
        ///其中 m是券A的概率  a是券A的等级 同n和b 对应于券B  
        ///同时 卷B等级＞卷A等级（若相等时，强化成功概率B大于A）
        ///
        /// 强化等级 key   |||    修正值  Value
        /// </summary>
        private Dictionary<int, int> m_FuseProbabilityIntervalCorrectValueDic = new Dictionary<int, int>();

        //UI
        private string tr_notice_fuse_limit_num = "准备融合强化券已经足够";
        private string tr_notice_merge_material_not_enough = "合成材料不足";
        private string tr_notice_dropdown_percent_format = "{0}%";
        private string tr_notice_cost_bind_ticket_tip = "";


        //使用绑定券时通知
        private bool bCostBindTicketNotifyEnable = false;

        private OpActivityData mStrengthTickActivityData = null;

        #endregion
               
        #region PRIVATE METHODS

        public override void Initialize()
        {
            _BindEvent();
            _InitLocalTicketMergeData();
            _InitLocalMaterialMergeIncreaseLevelModels();
            _InitLocalTicketFuseData();
            _InitSystemValue();
            _LoadTR();
        }

        public override void Clear()
        {
            _UnBindEvent();
            _ClearData();
        }

        private void _ClearData()
        {
            if (openedBuffPrayActivityDatas != null)
            {
                openedBuffPrayActivityDatas.Clear();
            }
            if (totalActivityTasks != null)
            {
                totalActivityTasks.Clear();
            }

            //默认关
            bFuncOpen = false;
            PrayActivityIsFinish = true;
            BSyntheticFrameTip = false;

            if (m_MaterialMergeTicketModels != null)
            {               
                m_MaterialMergeTicketModels.Clear();
            }
            if (m_DisplayMaterialMergeTicketModels != null)
            {
                m_DisplayMaterialMergeTicketModels.Clear();
            }
            if (currSelectMaterialMergeModel != null)
            {
                currSelectMaterialMergeModel.Clear();
            }
            if (m_MaterialMergeIncreaseLevelModels != null)
            {
                m_MaterialMergeIncreaseLevelModels.Clear();
            }

            if (m_FuseMaterialModels != null)
            {
                m_FuseMaterialModels.Clear();
            }
            if (tempMaterialFuseModel != null)
            {
                tempMaterialFuseModel.Clear();
            }

            if (m_FuseProbabilityIntervalCorrectValueDic != null)
            {
                m_FuseProbabilityIntervalCorrectValueDic.Clear();
            }

            tr_notice_fuse_limit_num = "";
            tr_notice_merge_material_not_enough = "";
            tr_notice_dropdown_percent_format= "";
            tr_notice_cost_bind_ticket_tip = "";

            bCostBindTicketNotifyEnable = false;

            mStrengthTickActivityData = null;
         
        }

        private void _LoadTR()
        {
            tr_notice_fuse_limit_num = TR.Value("strengthen_merge_fuse_limit_num");
            tr_notice_merge_material_not_enough = TR.Value("strengthen_merge_material_not_enough");
            tr_notice_dropdown_percent_format = TR.Value("strengthen_merge_dropdown_percent_format");
            tr_notice_cost_bind_ticket_tip = TR.Value("strengthen_merge_cost_bind_ticket_tip");
        }

        private void _BindEvent()
        {
            NetProcess.AddMsgHandler(SyncOpActivityDatas.MsgID, _OnSyncActivities);
            NetProcess.AddMsgHandler(SyncOpActivityStateChange.MsgID, _OnSyncActivityStateChange);
            NetProcess.AddMsgHandler(SyncOpActivityTaskChange.MsgID, _OnSyncActivityTaskChange);
            NetProcess.AddMsgHandler(SyncOpActivityTasks.MsgID, _OnSyncActivityTasks);

            NetProcess.AddMsgHandler(SceneStrengthenTicketSynthesisRes.MsgID, _OnStrengthenTicketMergeRet);
            NetProcess.AddMsgHandler(SceneStrengthenTicketFuseRes.MsgID, _OnStrengthenTicketFuseRet);

            NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, _OnMallBuyRes);

            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(ServiceType.SERVICE_STRENGTHEN_TICKET_MERGE, _OnServerSwitchFunc);
        }

        private void _UnBindEvent()
        {
            NetProcess.RemoveMsgHandler(SyncOpActivityDatas.MsgID, _OnSyncActivities);
            NetProcess.RemoveMsgHandler(SyncOpActivityStateChange.MsgID, _OnSyncActivityStateChange);
            NetProcess.RemoveMsgHandler(SyncOpActivityTaskChange.MsgID, _OnSyncActivityTaskChange);
            NetProcess.RemoveMsgHandler(SyncOpActivityTasks.MsgID, _OnSyncActivityTasks);

            NetProcess.RemoveMsgHandler(SceneStrengthenTicketSynthesisRes.MsgID, _OnStrengthenTicketMergeRet);
            NetProcess.RemoveMsgHandler(SceneStrengthenTicketFuseRes.MsgID, _OnStrengthenTicketFuseRet);

            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, _OnMallBuyRes);

            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(ServiceType.SERVICE_STRENGTHEN_TICKET_MERGE, _OnServerSwitchFunc);
        }

        private void _InitLocalTicketMergeData()
        {
            if (m_MaterialMergeTicketModels == null)
            {
                m_MaterialMergeTicketModels = new List<StrengthenTicketMaterialMergeModel>();
            }
            else
            {
                m_MaterialMergeTicketModels.Clear();
            }
            if (m_DisplayMaterialMergeTicketModels == null)
            {
                m_DisplayMaterialMergeTicketModels = new List<StrengthenTicketMaterialMergeModel>();
            }
            else 
            {
                m_DisplayMaterialMergeTicketModels.Clear();
            }
            var mergeTicketTable = TableManager.GetInstance().GetTable<StrengthenTicketSynthesisTable>();
            if (mergeTicketTable == null)
            {
                _DebugDataManagerLoggger("_InitLocalTicketMergeData", "can not find table : StrengthenTicketSynthesisTable");
                return;
            }
            var enumerator = mergeTicketTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current.Value as StrengthenTicketSynthesisTable;
                if(item == null)
                {
                    continue;
                }
                StrengthenTicketMaterialMergeModel mergeModel = new StrengthenTicketMaterialMergeModel();
                mergeModel.mergeTableId = item.ID;
                mergeModel.name = item.Name;
                mergeModel.strengthenLevel = item.StrengthenLv;
                mergeModel.isBind = item.Binding == 1 ? true : false;
                mergeModel.increaseLevel = (int)(item.Amplitude * 0.001);
                mergeModel.previewMinPercent = (int)(item.Lower * 0.1);
                mergeModel.previewMaxPercent = (int)(item.Upper * 0.1);
                mergeModel.displayItemTableId = item.DisplayItemIndex;
                mergeModel.bDisplay = item.DisplayItemIndex > 0 ? true : false;
                var materialList = item.CostMaterial;
                if (materialList == null)
                {
                    continue;
                }
                if(mergeModel.needMaterialModel == null)
                {
                    mergeModel.needMaterialModel = new StrengthenTicketMergeMaterial();
                }else
                {
                    mergeModel.needMaterialModel.Clear();
                }
                for (int i = 0; i < materialList.Count; i++)
                {
                    var materialDesc = materialList[i];
                    if (string.IsNullOrEmpty(materialDesc))
                    {
                        continue;
                    }
                    var materialItemDatas = mergeModel.needMaterialModel.needMaterialDatas;
                    if (materialItemDatas == null)
                    {
                        materialItemDatas = new List<StrengthenTicketMergeItemData>();
                    }
                    var mergeItemData = new StrengthenTicketMergeItemData();
                    ItemSimpleData sItemData = Utility.GetItemSimpleDataFromTableWithIdCount(materialDesc);
                    if (sItemData == null)
                    {
                        _DebugDataManagerLoggger("_InitLocalTicketMergeData", "get item simple data from id and count failed : "+materialDesc);
                        continue;
                    }
                    mergeItemData.tempItemData = sItemData;
                    mergeItemData.rawItemCount = sItemData.Count;
                    materialItemDatas.Add(mergeItemData);
                }

                m_MaterialMergeTicketModels.Add(mergeModel);
                //是否需要展示
                if (mergeModel.bDisplay)
                {
                    m_DisplayMaterialMergeTicketModels.Add(mergeModel);
                }
            }
        }
      
        private void _InitLocalMaterialMergeIncreaseLevelModels()
        {
            if (m_MaterialMergeIncreaseLevelModels == null)
            {
                m_MaterialMergeIncreaseLevelModels = new List<StrengthenTicketMaterialMergeIncreaseLevel>();
            }
            else
            {
                m_MaterialMergeIncreaseLevelModels.Clear();
            }
            if (m_MaterialMergeTicketModels == null || m_MaterialMergeTicketModels.Count == 0 ||
                m_DisplayMaterialMergeTicketModels == null || m_DisplayMaterialMergeTicketModels.Count == 0)
            {
                return;
            }
            for (int i = 0; i < m_DisplayMaterialMergeTicketModels.Count; i++)
            {
                var displayModel = m_DisplayMaterialMergeTicketModels[i];
                if (displayModel == null || displayModel.bDisplay == false)
                    continue;
                var levelModel = new StrengthenTicketMaterialMergeIncreaseLevel();
                levelModel.displayItemTableId = displayModel.displayItemTableId;
                for (int j = 0; j < m_MaterialMergeTicketModels.Count; j++)
                {
                    var model = m_MaterialMergeTicketModels[j];
                    if (model == null)
                        continue;
                    if (model.strengthenLevel == displayModel.strengthenLevel &&
                        model.isBind == displayModel.isBind)
                    {
                        levelModel.name = model.name;
                        levelModel.isBind = model.isBind;
                        levelModel.strengthenLevel = model.strengthenLevel;
                        if (levelModel.mergeTableIds == null)
                        {
                            levelModel.mergeTableIds = new List<int>();
                        }
                        levelModel.mergeTableIds.Add(model.mergeTableId);
                        if (levelModel.increaseLevels == null)
                        {
                            levelModel.increaseLevels = new List<int>();
                        }
                        levelModel.increaseLevels.Add(model.increaseLevel * 100);
                    }
                }
                m_MaterialMergeIncreaseLevelModels.Add(levelModel);
            }
        }

        private void _RefreshMaterialMergeTicketModelsByNet()
        {
            if (openedBuffPrayActivityDatas == null || openedBuffPrayActivityDatas.Count == 0)
            {
                return;
            }
            if (totalActivityTasks == null || totalActivityTasks.Count == 0)
            {
                return;
            }
            var enumerator = openedBuffPrayActivityDatas.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var buffPrayActData = enumerator.Current as OpActivityData;
                if (buffPrayActData == null) continue;
                if (buffPrayActData.state != (int)OpActivityState.OAS_IN ||
                    buffPrayActData.tmpType != (int)OpActivityTmpType.OAT_DAILY_BUFF)
                {
                    continue;
                }
                OpActTaskData[] buffPrayActTasks = buffPrayActData.tasks;
                if (buffPrayActTasks == null)
                {
                    continue;
                }
                for (int j = 0; j < buffPrayActTasks.Length; j++)
                {
                    var taskData = buffPrayActTasks[j];
                    if (taskData == null) continue;
                    if (totalActivityTasks.ContainsKey(taskData.dataid) == false)
                    {
                        continue;
                    }
                    uint[] variables = taskData.variables;
                    if (variables == null || variables.Length == 0) continue;
                    if (variables[0] != (uint)StrengthenTicketMergeBuffType.MaterialDiscount)
                    {
                        continue;
                    }
                    OpActTask task = totalActivityTasks[taskData.dataid];
                    if (task == null) continue;
                    if (task.state == (int)OpActTaskState.OATS_OVER)
                    {
                        _RefreshLocalMaterialTicketMergeMat(false);
                    }
                    else if (task.state == (int)OpActTaskState.OATS_FINISHED)
                    {
                       if(IsHaveLeftPrayTimer())
                        {
                            OpTaskReward[] taskRewards = taskData.rewards;
                            if (taskRewards == null) continue;
                            for (int k = 0; k < taskRewards.Length; k++)
                            {
                                var reward = taskRewards[k];
                                if (reward == null) continue;
                                _RefreshLocalMaterialTicketMergeMat(true, (int)reward.id, (int)reward.num);
                            }
                        }
                        else
                        {
                            _RefreshLocalMaterialTicketMergeMat(false);
                        }
                      
                    }             
                }
            }
        }

        private void _RefreshLocalMaterialTicketMergeMat(bool bDiscount, int itemId = 0, int discountPercent = 0)
        {
            if (m_MaterialMergeTicketModels == null )//|| m_DisplayMaterialMergeTicketModels == null)
            {
                return;
            }
            if (bDiscount == false)
            {
                _ResetLocalMaterialMergeTicketModels();                
                return;
            }
            for (int i = 0; i < m_MaterialMergeTicketModels.Count; i++)
            {
                var mergeTicketModel = m_MaterialMergeTicketModels[i];
                if (mergeTicketModel == null) continue;
                var mergeMaterial = mergeTicketModel.needMaterialModel;
                if (mergeMaterial == null) continue;
                List<StrengthenTicketMergeItemData> sDatas = mergeMaterial.needMaterialDatas;
                if (sDatas == null) continue;
                for (int j = 0; j < sDatas.Count; j++)
                {
                    var mergeItemData = sDatas[j];
                    if (mergeItemData == null)
                    {
                        continue;
                    }
                    ItemSimpleData sData = mergeItemData.tempItemData;
                    if (sData != null && sData.ItemID == itemId)
                    {
                        int totalCount = mergeItemData.rawItemCount;
                        sData.Count = (totalCount * (100 - discountPercent)) / 100;
                        //_DebugDataManagerLoggger("_RefreshLocalMaterialTicketMergeMat", 
                        //    string.Format("itemid {0}, old count {1}, new count {2}, percent {3}",
                        //    itemId, currCount, sData.Count, 100 - discountPercent));
                    }
                }
            }
            //for (int i = 0; i < m_DisplayMaterialMergeTicketModels.Count; i++)
            //{
            //    var mergeTicketModel = m_DisplayMaterialMergeTicketModels[i];
            //    if (mergeTicketModel == null) continue;
            //    var mergeMaterial = mergeTicketModel.needMaterialModel;
            //    if (mergeMaterial == null) continue;
            //    List<ItemSimpleData> sDatas = mergeMaterial.needMaterialDatas;
            //    if (sDatas == null) continue;
            //    for (int j = 0; j < sDatas.Count; j++)
            //    {
            //        ItemSimpleData sData = sDatas[j];
            //        if (sData.ItemID == itemId)
            //        {
            //            int currCount = sData.Count;
            //            sData.Count = (currCount * (100 - discountPercent)) / 100;
            //        }
            //    }
            //}
        }

        private void _ResetLocalMaterialMergeTicketModels()
        {
            if (m_MaterialMergeTicketModels == null)
            {
                m_MaterialMergeTicketModels = new List<StrengthenTicketMaterialMergeModel>();
            }
            else
            {
                m_MaterialMergeTicketModels.Clear();
            }
            var mergeTicketTable = TableManager.GetInstance().GetTable<StrengthenTicketSynthesisTable>();
            if (mergeTicketTable == null)
            {
                _DebugDataManagerLoggger("_ResetLocalMaterialMergeModels", "can not find table : StrengthenTicketSynthesisTable");
                return;
            }
            var enumerator = mergeTicketTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current.Value as StrengthenTicketSynthesisTable;
                if (item == null)
                {
                    continue;
                }
                StrengthenTicketMaterialMergeModel mergeModel = new StrengthenTicketMaterialMergeModel();
                mergeModel.mergeTableId = item.ID;
                mergeModel.name = item.Name;
                mergeModel.strengthenLevel = item.StrengthenLv;
                mergeModel.isBind = item.Binding == 1 ? true : false;
                mergeModel.increaseLevel = (int)(item.Amplitude * 0.001);
                mergeModel.previewMinPercent = (int)(item.Lower * 0.1);
                mergeModel.previewMaxPercent = (int)(item.Upper * 0.1);
                mergeModel.displayItemTableId = item.DisplayItemIndex;
                mergeModel.bDisplay = item.DisplayItemIndex > 0 ? true : false;
                var materialList = item.CostMaterial;
                if (materialList == null)
                {
                    continue;
                }
                if (mergeModel.needMaterialModel == null)
                {
                    mergeModel.needMaterialModel = new StrengthenTicketMergeMaterial();
                }
                else
                {
                    mergeModel.needMaterialModel.Clear();
                }
                for (int i = 0; i < materialList.Count; i++)
                {
                    var materialDesc = materialList[i];
                    if (string.IsNullOrEmpty(materialDesc))
                    {
                        continue;
                    }
                    var materialItemDatas = mergeModel.needMaterialModel.needMaterialDatas;
                    if (materialItemDatas == null)
                    {
                        materialItemDatas = new List<StrengthenTicketMergeItemData>();
                    }
                    var mergeItemData = new StrengthenTicketMergeItemData();
                    ItemSimpleData sItemData = Utility.GetItemSimpleDataFromTableWithIdCount(materialDesc);
                    if (sItemData == null)
                    {
                        _DebugDataManagerLoggger("_InitLocalTicketMergeData", "get item simple data from id and count failed : " + materialDesc);
                        continue;
                    }
                    mergeItemData.tempItemData = sItemData;
                    mergeItemData.rawItemCount = sItemData.Count;
                    materialItemDatas.Add(mergeItemData);
                }
                //初始化数量
                for (int i = 0; i < currSelectMaterialMergeModel.needMaterialModel.needMaterialDatas.Count; i++)
                {
                    currSelectMaterialMergeModel.needMaterialModel.needMaterialDatas[i].tempItemData.Count =
                         currSelectMaterialMergeModel.needMaterialModel.needMaterialDatas[i].rawItemCount;
                }
                m_MaterialMergeTicketModels.Add(mergeModel);
            }
        }


        private void _InitLocalTicketFuseData()
        {
            if (m_FuseMaterialModels == null)
            {
                m_FuseMaterialModels = new List<StrengthenTicketFuseSpecialMaterial>();
            }
            else
            {
                m_FuseMaterialModels.Clear();
            }
            var mergeTicketTable = TableManager.GetInstance().GetTable<StrenTicketFuseMaterialTable>();
            if (mergeTicketTable == null)
            {
                _DebugDataManagerLoggger("_InitLocalTicketFuseData", "can not find table : StrenTicketFuseMaterialTable");
                return;
            }
            var enumerator = mergeTicketTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current.Value as StrenTicketFuseMaterialTable;
                if (item == null)
                {
                    continue;
                }
                StrengthenTicketFuseSpecialMaterial fuseModel = new StrengthenTicketFuseSpecialMaterial();
                fuseModel.fuseNeedItemData = Utility.GetItemSimpleDataFromTableWithIdCount(item.Material);
                string limitLevel = item.PickCondOfStrenLv;
                if (string.IsNullOrEmpty(limitLevel))
                {
                    continue;
                }
                string[] levels = limitLevel.Split('_');
                if (levels == null || levels.Length != 2)
                {
                    continue;
                }
                int levelMin, levelMax;
                if (int.TryParse(levels[0], out levelMin))
                {
                    fuseModel.limitStrengthenLevelMin = levelMin;
                }
                if (int.TryParse(levels[1], out levelMax))
                {
                    fuseModel.limitStrengthenLevelMax = levelMax;
                }
                m_FuseMaterialModels.Add(fuseModel);
            }
            //添加融合材料到缓存数据中  不能在这里加！！！ 材料不一定满足强化要求
            //if (tempMaterialFuseModel != null)
            //{
            //    tempMaterialFuseModel.materialModels = m_FuseMaterialModels;
            //}
        }

        private void _InitSystemValue()
        {
            //系统数值表 
            var sysTable_1 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_STRENGTHEN_TICKET_FUSE_STREN_LEVEL_LIMIT);
            fuseLimitLevel_Max = sysTable_1.Value;
            var sysTable_2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_STRENGTHEN_TICKET_FUSE_STREN_LEVEL_LOWER_LIMIT);
            fuseLimitLevel_Min = sysTable_2.Value;

            //预制体预设值
            StrengthenTicketMergeFrame frame = ClientSystemManager.GetInstance().GetFrame(typeof(StrengthenTicketMergeFrame)) as StrengthenTicketMergeFrame;
            if (frame != null)
            {
                ticketFuseReadyCapacity = frame.GetFuseTicketCount();
            }

            var fuseTables = TableManager.GetInstance().GetTable<StrengthenTicketFuseTable>();
            if (fuseTables != null && m_FuseProbabilityIntervalCorrectValueDic != null)
            {
                 var enumerator = fuseTables.GetEnumerator();
                 while (enumerator.MoveNext())
                 {
                     var fuseTable = enumerator.Current.Value as StrengthenTicketFuseTable;
                     if (m_FuseProbabilityIntervalCorrectValueDic.ContainsKey(fuseTable.StrengthenLv))
                     {
                         m_FuseProbabilityIntervalCorrectValueDic[fuseTable.StrengthenLv] = fuseTable.FixM;
                     }
                     else
                     {
                         m_FuseProbabilityIntervalCorrectValueDic.Add(fuseTable.StrengthenLv, fuseTable.FixM);
                     }
                 }
            }
        }

        #region NET EVENT CALLBACK

        void _OnSyncActivities(MsgDATA data)
        {
            var resp = new SyncOpActivityDatas();
            resp.decode(data.bytes);
            OpActivityData[] actData = resp.datas;
            if (actData == null)
            {
                return;
            }
            for (var i = 0; i < actData.Length; ++i)
            {
                if (actData[i].tmpType == (int)OpActivityTmpType.OAT_STRENGTHEN_TICKET_SYNTHESIS)
                {
                    mStrengthTickActivityData = actData[i];
                    bFuncOpen = (OpActivityState)actData[i].state == OpActivityState.OAS_IN ? true : false;
                    if (bFuncOpen)
                    {
                        break;
                    }
                }
               
            }
            if (!bFuncOpen)
            {
                CloseStrengthenTicketMergeFrame();
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeStateUpdate);

            for (var i = 0; i < actData.Length; ++i)
            {
                if (actData[i].tmpType == (int)OpActivityTmpType.OAT_DAILY_BUFF)
                {
                    if (actData[i].state == (int)OpActivityState.OAS_IN)
                    {
                        //直接用 [] 进行存储
                        openedBuffPrayActivityDatas[actData[i].dataId] = actData[i];
                        _RefreshMaterialMergeTicketModelsByNet();
                    }
                    if (PrayActivityIsFinish)//只要有一个活动开了
                    {
                        PrayActivityIsFinish = (OpActivityState)actData[i].state == OpActivityState.OAS_END ? true : false;
                    }

                }
            }
        }

        void _OnSyncActivityStateChange(MsgDATA data)
        {
            var resp = new SyncOpActivityStateChange();
            resp.decode(data.bytes);
            OpActivityData actData = resp.data;
            if (actData == null)
            {
                return;
            }
            if (actData.tmpType == (int)OpActivityTmpType.OAT_STRENGTHEN_TICKET_SYNTHESIS)
            {
                bFuncOpen = (OpActivityState)actData.state == OpActivityState.OAS_IN ? true : false;
            }else if(actData.tmpType==(int)OpActivityTmpType.OAT_DAILY_BUFF)
            {
                PrayActivityIsFinish = (OpActivityState)actData.state == OpActivityState.OAS_END ? true : false;
            }
            if (!bFuncOpen)
            {
                CloseStrengthenTicketMergeFrame();
            }
          


            if (actData.tmpType == (int)OpActivityTmpType.OAT_DAILY_BUFF)
            {
                if (actData.state == (int)OpActivityState.OAS_END)
                {
                    if (openedBuffPrayActivityDatas != null && openedBuffPrayActivityDatas.ContainsKey(actData.dataId))
                    {
                        //直接用 [] 进行修改
                        openedBuffPrayActivityDatas.Remove(actData.dataId);
                    }
                    _RefreshMaterialMergeTicketModelsByNet();
                }
                else if (actData.state == (int)OpActivityState.OAS_IN)
                {
                    if (openedBuffPrayActivityDatas != null)
                    {
                        //直接用 [] 进行存储
                        openedBuffPrayActivityDatas[actData.dataId] = actData;
                    }
                    _RefreshMaterialMergeTicketModelsByNet();
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeStateUpdate);
        }

        private void _OnSyncActivityTasks(MsgDATA msg)
        {
            int pos = 0;
            SyncOpActivityTasks data = new SyncOpActivityTasks();
            data.decode(msg.bytes, ref pos);
            OpActTask[] tasks = data.tasks;
            if (tasks == null || tasks.Length == 0)
            {
                return;
            }
            if (totalActivityTasks == null)
            {
                return;
            }
            for (int i = 0; i < tasks.Length; ++i)
            {
                var task = tasks[i];
                if (task == null) continue;
                totalActivityTasks[task.dataId] = task;
                if (openedBuffPrayActivityDatas != null &&　
                    openedBuffPrayActivityDatas.ContainsKey(task.dataId / OP_ACTIVITY_TASKID_TO_ACTID))
                {
                    _RefreshMaterialMergeTicketModelsByNet();
                }
            }
        }

        private void _OnSyncActivityTaskChange(MsgDATA msg)
        {
            int pos = 0;
            SyncOpActivityTaskChange data = new SyncOpActivityTaskChange();
            data.decode(msg.bytes, ref pos);
            
            if (data.tasks == null || data.tasks.Length == 0)
            {
                return;
            }
            if (totalActivityTasks == null)
            {
                return;
            }
            for (int i = 0; i < data.tasks.Length; ++i)
            {
                var task = data.tasks[i];
                if (task == null) continue;
                totalActivityTasks[task.dataId] = task;
                if (openedBuffPrayActivityDatas != null && 
                    openedBuffPrayActivityDatas.ContainsKey(task.dataId / OP_ACTIVITY_TASKID_TO_ACTID))
                {
                    _RefreshMaterialMergeTicketModelsByNet();
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketFreshView);
            
        }

        void _OnStrengthenTicketMergeRet(MsgDATA data)
        {
            var mergeRet = new SceneStrengthenTicketSynthesisRes();
            mergeRet.decode(data.bytes);
            if (mergeRet.ret != (uint)ProtoErrorCode.SUCCESS)
            {
                _DebugDataManagerLoggger("_OnStrengthenTicketMergeRet", "merge error !!! code :"+mergeRet.ret);
                SystemNotifyManager.SystemNotify((int)mergeRet.ret);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeFailed);
            }
            else
            {
                _DebugDataManagerLoggger("_OnStrengthenTicketMergeRet", "merge success !!!");
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeSuccess);
            }
        }

        void _OnStrengthenTicketFuseRet(MsgDATA data)
        {
            var fuseRet = new SceneStrengthenTicketFuseRes();
            fuseRet.decode(data.bytes);
            if (fuseRet.ret != (uint)ProtoErrorCode.SUCCESS)
            {
                _DebugDataManagerLoggger("_OnStrengthenTicketFuseRet", "fuse error !!! code :" + fuseRet.ret);
                SystemNotifyManager.SystemNotify((int)fuseRet.ret);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketFuseFailed);
            }
            else
            {
                _DebugDataManagerLoggger("_OnStrengthenTicketFuseRet", "fuse success !!!");
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketFuseSuccess);
            }
        }

        void _OnMallBuyRes(MsgDATA data)
        {
            WorldMallBuyRet mallBuyRes = new WorldMallBuyRet();
            mallBuyRes.decode(data.bytes);

            if (mallBuyRes.code != (uint)ProtoErrorCode.SUCCESS)
            {
                //统一在ShopNewDataManager处理
                // SystemNotifyManager.SystemNotify((int)mallBuyRes.code);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMallBuySuccess);
            }
        }

        private void _OnServerSwitchFunc(ServerSceneFuncSwitch funcSwitch)
        {
            bFuncOpen = bFuncOpen && funcSwitch.sIsOpen;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeStateUpdate);
        }


        #endregion

        #region  TEST
        private void _DebugDataManagerLoggger(string methodName, string errorLog)
        {
#if UNITY_EDITOR
            if (Global.Settings.isDebug)
            {
                Logger.LogErrorFormat("[StrengthenTicketMergeDataManager] - {0}, error: {1}", methodName, errorLog);
            }
#endif
        }
        #endregion

        #endregion

        #region  PUBLIC METHODS

        public List<StrengthenTicketMaterialMergeModel> GetMaterialMergeTicketModels()
        {
            return m_MaterialMergeTicketModels;
        }

        public List<StrengthenTicketMaterialMergeModel> GetDisplayMaterialMergeTicketModels()
        {
            return m_DisplayMaterialMergeTicketModels;
        }

        public List<StrengthenTicketMaterialMergeIncreaseLevel> GetMaterialMergeIncreaseLevelModels()
        {
            return m_MaterialMergeIncreaseLevelModels;
        }

        //获取当前开放的buff活动信息
        public OpActivityData GetCurBuffPrayActivityData()
        {
            if (null == openedBuffPrayActivityDatas)
                return null;
            foreach (OpActivityData data in openedBuffPrayActivityDatas.Values)
            {   
                if (data.state == (int)OpActivityState.OAS_IN)
                    return data;
            }
            return null;
        }

        /// <summary>
        /// 获取材料合成 增幅列表 
        /// </summary>
        /// <param name="model"> 一类材料合成强化券数据 （可以是total model 中的 或者 display model中的） </param>
        /// <returns></returns>
        public List<string> GetMaterialMergeIncreaseLevelDescList(StrengthenTicketMaterialMergeModel model)
        {
            if (model == null || m_MaterialMergeIncreaseLevelModels == null)
            {
                return null;
            }
            List<string> levelDescList = new List<string>();
            for (int i = 0; i < m_MaterialMergeIncreaseLevelModels.Count; i++)
            {
                var levelModel = m_MaterialMergeIncreaseLevelModels[i];
                if (levelModel == null) continue;
                if (levelModel.strengthenLevel == model.strengthenLevel &&
                    levelModel.isBind == model.isBind)
                {
                    List<int> levelDescs = levelModel.increaseLevels;
                    if (levelDescs == null) continue;
                    for (int j = 0; j < levelDescs.Count; j++)
                    {
                        levelDescList.Add(string.Format(tr_notice_dropdown_percent_format, levelDescs[j].ToString()));
                    }
                    break;
                }
            }
            return levelDescList;
        }

        public StrengthenTicketMaterialMergeModel GetMaterialMergeStrengthenTicketTableId(StrengthenTicketMaterialMergeModel mergeModel, int index)
        {
            if (m_MaterialMergeTicketModels == null || mergeModel == null)
            {
                return null;
            }
            for (int i = 0; i < m_MaterialMergeTicketModels.Count; i++)
            {
                var model = m_MaterialMergeTicketModels[i];
                if (model == null)
                {
                    continue;
                }
                if (model.strengthenLevel == mergeModel.strengthenLevel && model.isBind == mergeModel.isBind && model.increaseLevel == index)
                {
                    //缓存下来
                    currSelectMaterialMergeModel = model;
                    return model;
                }
            }
            return null;
        }

        public void ReqMaterialMergeStrengthenTicket()
        {
            if (currSelectMaterialMergeModel == null)
            {
                return;
            }
            if (currSelectMaterialMergeModel.mergeTableId == 0)
            {
                _DebugDataManagerLoggger("ReqMaterialMergeStrengthenTicket", "curr temp model mergeTableId is 0");
                return;
            }
            SceneStrengthenTicketSynthesisReq msg = new SceneStrengthenTicketSynthesisReq();
            msg.synthesisPlan = (uint)(currSelectMaterialMergeModel.mergeTableId);
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
        }

        public void ReqFuseStrengthenTicket(ulong aGUID, ulong bGUID)
        {
            SceneStrengthenTicketFuseReq msg = new SceneStrengthenTicketFuseReq();
            msg.pickTicketA = aGUID;
            msg.pickTicketB = bGUID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
        }


        public void ReqFastMallBuy(int itemId)
        {
            WorldGetMallItemByItemIdReq msg = new WorldGetMallItemByItemIdReq();
            msg.itemId = (uint)itemId;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGetMallItemByItemIdRes>(msgRet =>
            {
                var mallItemInfo = msgRet.mallItem;
                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
            }, false);
        }


        #region ITEM DATA

        /// <summary>
        /// 获取 指定 tableID 的拥有的强化券数量
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public int GetOwnStrengthenTicketCount(int tableId)
        {
            int count = ItemDataManager.GetInstance().GetOwnedItemCount(tableId, false);
            return count;
        }

        /// <summary>
        /// 是否拥有强化券
        /// </summary>
        /// <returns></returns>
        public bool HasStrengthenTicket()
        {
            List<ulong> ticketsGUIDs = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Material, ItemTable.eSubType.Coupon);
            if (ticketsGUIDs != null && ticketsGUIDs.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取拥有的强化券GUID列表
        /// </summary>
        /// <returns></returns>
        public List<ulong> GetOwnStrengthenTicketItemDataGUIDs()
        {
            List<ulong> ticketsGUIDs = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Material, ItemTable.eSubType.Coupon);
            return ticketsGUIDs;
        }

        /// <summary>
        /// 清理材料合成 选择数据
        /// </summary>
        public void ClearCurrSelectMaterialMergeModel()
        {
            //只移除引用   材料合成数据常驻不变  券融合数据变化
            //只清理引用 不要清引用的对象
            currSelectMaterialMergeModel = new StrengthenTicketMaterialMergeModel();
            //错了
            //if (currSelectMaterialMergeModel != null)
            //{
            //    currSelectMaterialMergeModel.Clear();
            //}
        }

        /// <summary>
        /// 清理融合准备 数据
        /// </summary>
        public void ClearTempMaterialFuseModel()
        {
            //！！！注意注意 引用间接被清掉了 ！！！
            //if (tempMaterialFuseModel != null)
            //{
            //    tempMaterialFuseModel.Clear();
            //}

            //只清理引用 不要清引用的对象
            if (tempMaterialFuseModel == null)
            {
                return;
            }
            if (tempMaterialFuseModel.materialModels != null)
            {
                tempMaterialFuseModel.materialModels.Clear();
            }
            if (tempMaterialFuseModel.ticketModels != null)
            {
                tempMaterialFuseModel.ticketModels.Clear();
            }
        }

        /// <summary>
        /// 获取拥有的强化券 道具数据列表
        /// 
        /// 从强化等级 低到高 排序 
        /// </summary>
        /// <returns></returns>
        public List<StrengthenTicketFuseItemData> GetStrengthenTicketFuseItemDatas(bool bReverse = false)
        {
            //注意 重新调用时 数据存储堆栈 会发生改变 
            List<StrengthenTicketFuseItemData> tempStrengthenTickets = new List<StrengthenTicketFuseItemData>();

            List<ulong> ticketGUIDs = GetOwnStrengthenTicketItemDataGUIDs();
            if (ticketGUIDs == null || ticketGUIDs.Count == 0)
            {
                return tempStrengthenTickets;
            }
            for (int i = 0; i < ticketGUIDs.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(ticketGUIDs[i]);
                if (itemData == null || tempStrengthenTickets == null)
                {
                    continue;
                }
                int tableId = itemData.TableID;
                ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(tableId);
                if(itemTable == null)
                {
                    continue;
                }
                int sTableId = itemTable.StrenTicketDataIndex;
                StrengthenTicketTable sTable = TableManager.GetInstance().GetTableItem<StrengthenTicketTable>(sTableId);
                if(sTable == null)
                {
                    continue;
                }
                StrengthenTicketFuseItemData sModel = new StrengthenTicketFuseItemData();
                sModel.ticketItemData = itemData;
                sModel.fuseValue = sTable.FuseValue;
                sModel.canFuse = (sTable.Compound == 1) && (sTable.Level < fuseLimitLevel_Max) &&　(sTable.Level > fuseLimitLevel_Min);                      //强化券能否融合条件 ！！！
                if (sModel.canFuse == false)
                {
                    //5，6，7，13强化券不显示
                    continue;
                }
                sModel.sProbabilityConvert = sTable.Probility * 0.001f;
                sModel.sLevel = sTable.Level;
                sModel.bNotBindInt = itemData.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND ? 0 : 1;

                tempStrengthenTickets.Add(sModel);
            }
            if(bReverse)
            {
                 // tempStrengthenTickets.Sort((x, y) =>  -x.fuseValue.CompareTo(y.fuseValue));
                tempStrengthenTickets.Sort((x,y) =>   - x.sLevel.CompareTo(y.sLevel) * 100
                                                      - x.sProbabilityConvert.CompareTo(y.sProbabilityConvert) * 20
                                                      - x.bNotBindInt.CompareTo(y.bNotBindInt) * 1);
            }else
            {
                 //tempStrengthenTickets.Sort((x, y) =>  x.fuseValue.CompareTo(y.fuseValue));
                tempStrengthenTickets.Sort((x, y) =>  + x.sLevel.CompareTo(y.sLevel) * 100
                                                      + x.sProbabilityConvert.CompareTo(y.sProbabilityConvert) * 20
                                                      + x.bNotBindInt.CompareTo(y.bNotBindInt) * 1);
            }
            return tempStrengthenTickets;
        }

        /// <summary>
        /// 判断 材料合成强化券时 材料是否足够
        /// </summary>
        /// <param name="materialMergeModel"></param>
        public bool CheckMaterialMergeIsEnough()
        {
            if (currSelectMaterialMergeModel == null)
            {
                return false;
            }
            if (currSelectMaterialMergeModel.needMaterialModel == null)
            {
                return false;
            }
            if (currSelectMaterialMergeModel.needMaterialModel.needMaterialDatas == null)
            {
                return false;
            }

            for (int i = 0; i < currSelectMaterialMergeModel.needMaterialModel.needMaterialDatas.Count; i++)
            {
                var data = currSelectMaterialMergeModel.needMaterialModel.needMaterialDatas[i];
                if (data == null || data.tempItemData == null)
                {
                    continue;
                }
                if (CheckItemCountEnough(data.tempItemData.ItemID, data.tempItemData.Count) == false)
                {
                    return false;
                }
            }
            return true;
        }
        public bool CheckMaterialMergeIsEnough(StrengthenTicketMaterialMergeModel model)
        {
            if (model == null)
            {
                return false;
            }
            if (model.needMaterialModel == null)
            {
                return false;
            }
            if (model.needMaterialModel.needMaterialDatas == null)
            {
                return false;
            }

            for (int i = 0; i < model.needMaterialModel.needMaterialDatas.Count; i++)
            {
                var data = model.needMaterialModel.needMaterialDatas[i];
                if (data == null || data.tempItemData == null)
                {
                    continue;
                }
                if (CheckItemCountEnough(data.tempItemData.ItemID, data.tempItemData.Count) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断 是不是 金币
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public bool CheckIsCoin(int tableId)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(tableId);
            if (itemData == null)
            {
                return false;
            }
            if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.BindGOLD ||
                itemData.SubType == (int)ProtoTable.ItemTable.eSubType.GOLD)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断 单个材料 是否够
        /// 
        /// 注意不是判断 ItemData 的 Count（堆叠数量）
        /// 
        /// 不是总数 ： 总数 = 相同道具的堆叠数量
        /// </summary>
        /// <param name="needCount"></param>
        /// <returns></returns>
        public bool CheckItemCountEnough(int tableId, int needCount)
        {
            int ownCount = ItemDataManager.GetInstance().GetOwnedItemCount(tableId, true);
            if (ownCount >= needCount)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试获取 消耗的金币ID 通用是否绑定
        /// </summary>
        /// <param name="isBind"></param>
        /// <returns></returns>
        public int TryGetCoinIdByBindType(bool isBind)
        {
            int coinId = 0;
            if (isBind)
            {
                coinId = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
            }
            else
            {
                coinId = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.GOLD);
            }
            return coinId;
        }

        /// <summary>
        /// 获取 材料合成中 需要材料中的 金币道具数据（id + needCount）
        /// </summary>
        /// <param name="mergeModel"></param>
        /// <returns></returns>
        public ItemSimpleData TryGetFirstCoinItemDataInMaterials()
        {
            ItemSimpleData sData = new ItemSimpleData();
            if (currSelectMaterialMergeModel == null || currSelectMaterialMergeModel.needMaterialModel == null)
            {
                return sData;
            }
            var materials = currSelectMaterialMergeModel.needMaterialModel.needMaterialDatas;
            if (materials == null)
            {
                return sData;
            }
            for (int i = 0; i < materials.Count; i++)
            {
                var mergeItemData = materials[i];
                if (mergeItemData == null || mergeItemData.tempItemData == null)
                    continue;
                bool isCoin = CheckIsCoin(mergeItemData.tempItemData.ItemID);
                if (isCoin)
                {
                    sData.ItemID = mergeItemData.tempItemData.ItemID;
                    sData.Count = mergeItemData.tempItemData.Count;
                    break;
                }
            }
            return sData;
        }

        /// <summary>
        /// 券融合 是否可以添加券 准备阶段
        /// 
        /// 注意 比较数量 用GUID
        /// </summary>
        /// <param name="fuseItemData"></param>
        /// <returns></returns>
        public bool CheckFuseTicketCanAdd(StrengthenTicketFuseItemData fuseItemData)
        {
            if (ticketFuseReadyCapacity <= 0)
            {
                _DebugDataManagerLoggger("CheckFuseTicketCanAdd", "please check merge frame: fuse capacity less zero");
                return false;
            }
            if (fuseItemData == null || fuseItemData.ticketItemData == null)
            {
                return false;
            }
            if (tempMaterialFuseModel == null)
            {
               return false;
            }
            var ticketModels = tempMaterialFuseModel.ticketModels;
            if (ticketModels == null || ticketModels.Count >= ticketFuseReadyCapacity)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_notice_fuse_limit_num);
                return false;
            }
            int needItemID = fuseItemData.ticketItemData.TableID;
            int readyItemCount = 0;//已经准备的相同道具数量(总数)
            int readySelectItemCount = 0;//已经准备的相同道具 当前选择堆的数量 (堆数)
            for (int i = 0; i < ticketModels.Count; i++)
            {
                var ticket = ticketModels[i];
                if (ticket == null || ticket.ticketItemData == null)
                {
                    continue;
                }
                if (ticket.ticketItemData.TableID.Equals(needItemID))
                {
                    readyItemCount++;//全部同类堆中可用数量
                }
                if (ticket.ticketItemData.GUID.Equals(fuseItemData.ticketItemData.GUID))
                {
                    readySelectItemCount++;//堆中可选择数量
                }
                //引用是否相等
                //if (ticket == fuseItemData)
                //{
                //    readySelectItemCount++;
                //}
            }
            // readyItemCount + 1 已经准备的 和 打算放到准备的 相同道具数量 
            // 判断道具是否足够 需要判断总数 和 当前堆数  是否够
            if (CheckItemCountEnough(needItemID, readyItemCount + 1) == false ||
                fuseItemData.ticketItemData.Count < (readySelectItemCount + 1))
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_notice_merge_material_not_enough);
                return false;
            }

            ticketModels.Add(fuseItemData);
            //fuseItemData.fuseReadyCount++;
            return true;
        }

        /// <summary>
        /// 券融合 是否可以移除添加的券 准备阶段
        /// </summary>
        /// <param name="fuseItemData"></param>
        /// <returns></returns>
        public bool CheckFuseTicketCanRemove(StrengthenTicketFuseItemData fuseItemData)
        {
            if (fuseItemData == null || fuseItemData.ticketItemData == null)
            {
                return false;
            }
            if (tempMaterialFuseModel == null)
            {
                return false;
            }
            var ticketModels = tempMaterialFuseModel.ticketModels;
            if (ticketModels == null || ticketModels.Count <= 0)
            {
                return false;
            }
            if (ticketModels.Contains(fuseItemData) == false)
            {
                return false;
            }
            //移除引用
            ticketModels.Remove(fuseItemData);
            //fuseItemData.fuseReadyCount--;
            return true;
        }
        
        /// <summary>
        /// 检查 券融合 指定券数据是否在准备区
        /// </summary>
        /// <param name="fuseItemData"></param>
        /// <returns></returns>
        public bool CheckFuseTicketOnReady(StrengthenTicketFuseItemData fuseItemData)
        {
            if (fuseItemData == null || fuseItemData.ticketItemData == null)
            {
                return false;
            }
            if (tempMaterialFuseModel == null)
            {
                return false;
            }
            var ticketModels = tempMaterialFuseModel.ticketModels;
            if (ticketModels == null || ticketModels.Count <= 0)
            {
                return false;
            }
            if (ticketModels.Contains(fuseItemData) == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取 券融合时  指定券数据有多少在准备区
        /// 
        /// 注意 比较数量 用GUID
        /// </summary>
        /// <param name="fuseItemData"></param>
        /// <returns></returns>
        public int CheckFuseTicketNumOnReadyByTableId(StrengthenTicketFuseItemData fuseItemData)
        {
            int ticketNumOnReady = 0;
            if (fuseItemData == null || fuseItemData.ticketItemData == null)
            {
                return ticketNumOnReady;
            }
            if (tempMaterialFuseModel == null)
            {
                return ticketNumOnReady;
            }
            var ticketModels = tempMaterialFuseModel.ticketModels;
            if (ticketModels == null || ticketModels.Count <= 0)
            {
                return ticketNumOnReady;
            }
            for (int i = 0; i < ticketModels.Count; i++)
            {
                var ticket = ticketModels[i];
                if (ticket == null || ticket.ticketItemData == null)
                {
                    continue;
                }
                if (ticket.ticketItemData.GUID.Equals(fuseItemData.ticketItemData.GUID))
                {
                    ticketNumOnReady++;
                }
            }
            return ticketNumOnReady;
        }

        /// <summary>
        /// 尝试添加 券合成的可用材料 
        /// </summary>
        public void TryAddFuseMaterialCanUse()
        {
            if (m_FuseMaterialModels == null)
            {
                return;
            }
            if (tempMaterialFuseModel == null)
            {
                return;
            }
            var ticketModels = tempMaterialFuseModel.ticketModels;
            if (ticketModels == null || ticketModels.Count == 0)
            {
                return;
            }
            int tempStrengthenLevelMajor = 0; //缓存最大的准备中的强化券强化等级
            for (int i = 0; i < ticketModels.Count; i++)
            {
                var stItemData = ticketModels[i];
                if (stItemData == null)
                {
                    continue;
                }                
                if (tempStrengthenLevelMajor < stItemData.sLevel)
                    tempStrengthenLevelMajor = stItemData.sLevel;
            }
            for (int i = 0; i < m_FuseMaterialModels.Count; i++)
            {
                var matModel = m_FuseMaterialModels[i];
                if (matModel == null)
                {
                    continue;
                }
                if (matModel.limitStrengthenLevelMin <= tempStrengthenLevelMajor &&
                    matModel.limitStrengthenLevelMax >= tempStrengthenLevelMajor)
                {
                    if (tempMaterialFuseModel.materialModels == null)
                    {
                        tempMaterialFuseModel.materialModels = new List<StrengthenTicketFuseSpecialMaterial>();
                    }
                    else
                    {
                        tempMaterialFuseModel.materialModels.Clear();
                    }
                    tempMaterialFuseModel.materialModels.Add(matModel);
                }
            }
        }

       /// <summary>
       /// 计算券融合 产出概率区间 （只适合两个强化券融合）
       /// 
        /// 计算时都保证参数值 是 原公式的1000倍
       /// ( min( m / Mathf.Pow(4,b-a) + n , 1 ) ) / (b+1)的修正值
       /// </summary>
        public bool TryCalculateFuseOutputProbabilityInterval()
        {
            //券融合时 检查准备融合的券的数量是否达到两个
            if (tempMaterialFuseModel == null)
            {
                return false;
            }
            var ticketModels = tempMaterialFuseModel.ticketModels;
            if (ticketModels == null)
            {
                return false;
            }
            if (ticketModels.Count != ticketFuseReadyCapacity)
            {
                //_DebugDataManagerLoggger("TryCalculateFuseOutputProbabilityInterval", 
                //    string.Format("count not equal : ready tickets count {0}, tickets fuse need count {1}", ticketModels.Count, ticketFuseReadyCapacity));
                return false;
            }
            if (ticketModels.Count != 2)
            {
                //_DebugDataManagerLoggger("TryCalculateFuseOutputProbabilityInterval", "count not equal : ready tickets count != 2");
                return false;
            }

            if (m_FuseProbabilityIntervalCorrectValueDic == null)
            {
                _DebugDataManagerLoggger("TryCalculateFuseOutputProbabilityInterval", "tickets fuse fix value dictionary is null");
                return false;
            }
            StrengthenTicketFuseItemData aTicket = null, bTicket = null;
            aTicket = ticketModels[0];
            bTicket = ticketModels[1];
            if (aTicket == null || bTicket == null)
            {
                return false;
            }
            bool needExchange = false;
            if (aTicket.sLevel > bTicket.sLevel)
            {
                needExchange = true;
            }
            else if (aTicket.sLevel == bTicket.sLevel)
            {
                if (aTicket.sProbabilityConvert > bTicket.sProbabilityConvert)
                {
                    needExchange = true;
                }
            }
            if (needExchange)
            {
                StrengthenTicketFuseItemData tempTicket = null;
                tempTicket = bTicket;
                bTicket = aTicket;
                aTicket = tempTicket;
            }
            float aSLevel = 1f;
            float bSLevel = 1f;
            float aSProbability = 0.01f;
            float bSProbability = 0.01f;
            aSLevel = aTicket.sLevel;
            bSLevel = bTicket.sLevel;
            aSProbability = aTicket.sProbabilityConvert;
            bSProbability = bTicket.sProbabilityConvert;
            //bLevel+1 对应的 融合修正值 （取表格数据 并/10）
            float fuseFixValueConvert = 1f;
            if (m_FuseProbabilityIntervalCorrectValueDic != null && m_FuseProbabilityIntervalCorrectValueDic.ContainsKey(bTicket.sLevel + 1))
            {
                fuseFixValueConvert = m_FuseProbabilityIntervalCorrectValueDic[bTicket.sLevel + 1] * 0.1f;
            }
            float r1 = Mathf.Min((aSProbability / Mathf.Pow(4f, (bSLevel - aSLevel)) + bSProbability), 1f) / fuseFixValueConvert;           
            int resultHundred = Mathf.CeilToInt(r1 * 100); //向上取整
            //_DebugDataManagerLoggger("TryCalculateFuseOutputProbabilityInterval", "percent : " + r1 * 100 + ", resultHundred : " + resultHundred);
            //set
            tempMaterialFuseModel.perdictMinPercent = (int)(aSProbability * 100);
            tempMaterialFuseModel.perdictMaxPercent = resultHundred;
            tempMaterialFuseModel.predictMinLevel = aTicket.sLevel;
            tempMaterialFuseModel.predictMaxLevel = bTicket.sLevel + 1;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketFuseCalPercent);
            return true;
        }

        //计算券融合的结果
        public StrengthenTicketMaterialFuseModel CalculateMixRes(StrengthenTicketFuseItemData aTicket, StrengthenTicketFuseItemData bTicket)
        {
            //券融合时 检查准备融合的券的数量是否达到两个
            if (aTicket == null || bTicket == null)
                return null;
            if (m_FuseProbabilityIntervalCorrectValueDic == null)
            {
                _DebugDataManagerLoggger("TryCalculateFuseOutputProbabilityInterval", "tickets fuse fix value dictionary is null");
                return null;
            }
            if (aTicket == null || bTicket == null)
            {
                return null;
            }
            StrengthenTicketMaterialFuseModel model = new StrengthenTicketMaterialFuseModel();
            bool needExchange = false;
            if (aTicket.sLevel > bTicket.sLevel)
            {
                needExchange = true;
            }
            else if (aTicket.sLevel == bTicket.sLevel)
            {
                if (aTicket.sProbabilityConvert > bTicket.sProbabilityConvert)
                {
                    needExchange = true;
                }
            }
            if (needExchange)
            {
                StrengthenTicketFuseItemData tempTicket = null;
                tempTicket = bTicket;
                bTicket = aTicket;
                aTicket = tempTicket;
            }
            float aSLevel = 1f;
            float bSLevel = 1f;
            float aSProbability = 0.01f;
            float bSProbability = 0.01f;
            aSLevel = aTicket.sLevel;
            bSLevel = bTicket.sLevel;
            aSProbability = aTicket.sProbabilityConvert;
            bSProbability = bTicket.sProbabilityConvert;
            //bLevel+1 对应的 融合修正值 （取表格数据 并/10）
            float fuseFixValueConvert = 1f;
            if (m_FuseProbabilityIntervalCorrectValueDic != null && m_FuseProbabilityIntervalCorrectValueDic.ContainsKey(bTicket.sLevel + 1))
            {
                fuseFixValueConvert = m_FuseProbabilityIntervalCorrectValueDic[bTicket.sLevel + 1] * 0.1f;
            }
            float r1 = Mathf.Min((aSProbability / Mathf.Pow(4f, (bSLevel - aSLevel)) + bSProbability), 1f) / fuseFixValueConvert;           
            int resultHundred = Mathf.CeilToInt(r1 * 100); //向上取整
            model.perdictMinPercent = (int)(aSProbability * 100);
            model.perdictMaxPercent = resultHundred;
            model.predictMinLevel = aTicket.sLevel;
            model.predictMaxLevel = bTicket.sLevel + 1;
            return model;
        }

        /// <summary>
        /// 检查 券融合 的全部材料是否足够
        /// </summary>
        /// <param name="onReqFuseHandle"></param>
        /// <returns></returns>
        public bool CheckMaterialFuseIsEnough(System.Action<ulong, ulong> onReqFuseHandle = null, System.Action onReqCancelHandle = null)
        {
            if (tempMaterialFuseModel == null)
            {
                return false;
            }
            if (tempMaterialFuseModel.materialModels == null || tempMaterialFuseModel.ticketModels == null)
            {
                return false;
            }

            for (int i = 0; i < tempMaterialFuseModel.materialModels.Count; i++)
            {
                var matModel = tempMaterialFuseModel.materialModels[i];
                if (matModel == null || matModel.fuseNeedItemData == null)
                {
                    return false;
                }
                if (!CheckItemCountEnough(matModel.fuseNeedItemData.ItemID, matModel.fuseNeedItemData.Count))
                {
                    return false;
                }
            }
            if (tempMaterialFuseModel.ticketModels.Count != ticketFuseReadyCapacity ||
                ticketFuseReadyCapacity != 2)
            {
                return false;
            }
            var aTicketData = tempMaterialFuseModel.ticketModels[0];
            var bTicketData = tempMaterialFuseModel.ticketModels[1];
            if (aTicketData == null || aTicketData.ticketItemData == null ||
                bTicketData == null || bTicketData.ticketItemData == null)
            {
                return false;
            }

            if (onReqFuseHandle != null)
            {
                if (!bCostBindTicketNotifyEnable)
                {
                    if (aTicketData.bNotBindInt != 1 || bTicketData.bNotBindInt != 1)
                    {
                        ComCostNotifyData data = new ComCostNotifyData();
                        data.strContent = tr_notice_cost_bind_ticket_tip;
                        data.delSetNotify = SetCostBindTicketNotifyEnable;
                        data.delGetNotify = GetCostBindTicketNotifyEnable;
                        data.delOnOkCallback = () =>
                        {
                            onReqFuseHandle(aTicketData.ticketItemData.GUID, bTicketData.ticketItemData.GUID);
                        };
                        data.delOnCancelCallback = () =>
                        {
                            if (onReqCancelHandle != null)
                            {
                                onReqCancelHandle();
                            }
                        };
                        OpenComCostNotifyFrame(data);
                    }
                    else
                    {
                        onReqFuseHandle(aTicketData.ticketItemData.GUID, bTicketData.ticketItemData.GUID);
                    }
                }
                else
                {
                    onReqFuseHandle(aTicketData.ticketItemData.GUID, bTicketData.ticketItemData.GUID);
                }
            }
          
            return true;
        }

        //检查能不能融合
        public bool checkEnableMix(StrengthenTicketFuseItemData aTicketData, StrengthenTicketFuseItemData bTicketData, System.Action<ulong, ulong> onReqFuseHandle = null, System.Action onReqCancelHandle = null)
        {
            if (null == aTicketData || null == bTicketData)
                return false;
            if (onReqFuseHandle != null)
            {
                if (!bCostBindTicketNotifyEnable)
                {
                    if (aTicketData.bNotBindInt != 1 || bTicketData.bNotBindInt != 1)
                    {
                        ComCostNotifyData data = new ComCostNotifyData();
                        data.strContent = tr_notice_cost_bind_ticket_tip;
                        data.delSetNotify = SetCostBindTicketNotifyEnable;
                        data.delGetNotify = GetCostBindTicketNotifyEnable;
                        data.delOnOkCallback = () =>
                        {
                            onReqFuseHandle(aTicketData.ticketItemData.GUID, bTicketData.ticketItemData.GUID);
                        };
                        data.delOnCancelCallback = () =>
                        {
                            if (onReqCancelHandle != null)
                            {
                                onReqCancelHandle();
                            }
                        };
                        OpenComCostNotifyFrame(data);
                    }
                    else
                    {
                        onReqFuseHandle(aTicketData.ticketItemData.GUID, bTicketData.ticketItemData.GUID);
                    }
                }
                else
                {
                    onReqFuseHandle(aTicketData.ticketItemData.GUID, bTicketData.ticketItemData.GUID);
                }
            }
          
            return true;
        }

        #endregion

        public void OpenStrengthenTicketMergeFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenTicketMergeFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<StrengthenTicketMergeFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<StrengthenTicketMergeFrame>(FrameLayer.Middle);
        }

        public void CloseStrengthenTicketMergeFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenTicketMergeFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<StrengthenTicketMergeFrame>();
            }
        }

        public bool GetCostBindTicketNotifyEnable()
        {
            return bCostBindTicketNotifyEnable;
        }

        public void SetCostBindTicketNotifyEnable(bool notify)
        {
            bCostBindTicketNotifyEnable = notify;
        }

        public void OpenComCostNotifyFrame(ComCostNotifyData notifyData)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ComCostNotifyFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ComCostNotifyFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<ComCostNotifyFrame>(FrameLayer.Middle, notifyData);
        }

        #endregion
        #region 祈福活动的数据
        /// <summary>
        /// 得到已经祈福的次数
        /// </summary>
        /// <returns></returns>
        private  uint GetHavePrayedTimer()
        {
            uint haveNum = 0;
            Protocol.OpActTaskData taskData = GetPrayTaskData();
            if (taskData != null)
            {
                Protocol.OpActTask task = ActivityDataManager.GetInstance().GetLimitTimeTaskData(taskData.dataid);
                if (task != null && task.parms != null && task.parms.Length > 0)
                {
                    for (int k = 0; k < task.parms.Length; k++)
                    {
                        if (task.parms[k].key.Equals("times_var"))
                        {
                            haveNum = task.parms[k].value;
                            break;
                        }
                    }
                }
            }
            return haveNum;
        }

        /// <summary>
        /// 得到剩余的赐福次数
        /// </summary>
        /// <returns></returns>
        public uint GetLeftPrayeTimer()
        {
            uint reslut = 0;
            if(GetTotalPrayTimer() <=GetHavePrayedTimer())
            {
                reslut = 0;
            }
            else
            {
                reslut = GetTotalPrayTimer() -GetHavePrayedTimer();
            }
            return reslut;
        }

        /// <summary>
        /// 得到总的祈福的次数
        /// </summary>
        /// <returns></returns>
        public uint GetTotalPrayTimer()
        {
            uint totalNum = 0;
            Protocol.OpActTaskData taskData = GetPrayTaskData();
            if(taskData!=null)
            {
                if (taskData.variables2 != null && taskData.variables2.Length > 0)
                {
                    totalNum = taskData.variables2[0];
                }
            }
            return totalNum;
        }

        /// <summary>
        /// 是否有剩余的祈福次数
        /// </summary>
        /// <returns></returns>
        public bool IsHaveLeftPrayTimer()
        {
            return GetLeftPrayeTimer()>0;
        }

        /// <summary>
        /// 得到祈福的活动
        /// </summary>
        /// <returns></returns>
        private Protocol.OpActTaskData GetPrayTaskData()
        {
            var activityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.BUFF_PRAY_ACTIVITY);
            Protocol.OpActTaskData taskData = null;
            if (activityData == null) return null;
            bool isFind = false;
            if (activityData.tasks != null && activityData.tasks.Length > 0)
            {
                for (int i = 0; i < activityData.tasks.Length; i++)
                {
                    if (activityData.tasks[i].variables != null && activityData.tasks[i].variables.Length > 0)
                    {
                        for (int j = 0; j < activityData.tasks[i].variables.Length; j++)
                        {
                            if (activityData.tasks[i].variables[j] == 1)
                            {
                                taskData = activityData.tasks[i];
                                isFind = true;
                                break;
                            }
                        }
                    }
                    if (isFind)
                    {
                        break;
                    }
                }

            }
            return taskData;
        }
        /// <summary>
        /// 得到祈祷Buff描述的提示
        /// </summary>
        /// <returns></returns>
        public string GetPrayTaskDes()
        {
            if(mStrengthTickActivityData!=null)
            {
                return mStrengthTickActivityData.ruleDesc + TR.Value("strengthen_merge_left_time");
            }
            return "";
        }
       
        #endregion

    }
}