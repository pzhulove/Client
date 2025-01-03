using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;
///////删除linq
using ProtoTable;

namespace GameClient
{
    public class BeadCardManager : DataManager<BeadCardManager>
    {
        Dictionary<int, Dictionary<int, Dictionary<int, List<BeadPickItemModel>>>> mRemovJewelsTableDic = new Dictionary<int, Dictionary<int, Dictionary<int, List<BeadPickItemModel>>>>();//宝珠摘除表数据,外面int表示宝珠品级，外面第二个int表示宝珠等级，最里面int表示宝珠类型（是普通宝珠还是多彩宝珠）

        Dictionary<int, BeadRandomBuff> mBeadRandomBuffDic = new Dictionary<int, BeadRandomBuff>();

        Dictionary<int, Dictionary<int, Dictionary<int, ReplacejewelsTable>>> mReplaceJewelsTableDic = new Dictionary<int, Dictionary<int, Dictionary<int, ReplacejewelsTable>>>();//宝珠置换表数据,外面int表示宝珠品级，外面第二个int表示宝珠等级，最里面int表示宝珠类型（是普通宝珠还是多彩宝珠）

        public bool TreasureConvertTip = false;
        public sealed override void Clear()
        {
            UnRegisterNetHandler();
            mRemovJewelsTableDic.Clear();
            mBeadRandomBuffDic.Clear();
            mReplaceJewelsTableDic.Clear();
            TreasureConvertTip = false;
        }

        public sealed override void Initialize()
        {
            RegisterNetHandler();
            InitRemovJewelsTable();
            InitBeadRandomBuff();
            InitReplaceJewelsTable();
        }

       
        /// <summary>
        /// 得到宝珠摘除需要消耗的道具数据
        /// </summary>
        /// <returns></returns>
        public List<BeadPickItemModel> GetBeadExpendItemModel(int cololLevel,int beadLevel,int beadType)
        {
            if (mRemovJewelsTableDic.ContainsKey(cololLevel))
            {
               if (mRemovJewelsTableDic[cololLevel].ContainsKey(beadLevel))
               {
                    if (mRemovJewelsTableDic[cololLevel][beadLevel].ContainsKey(beadType))
                    {
                        return mRemovJewelsTableDic[cololLevel][beadLevel][beadType];
                    }
               }
            }

            return null;
        }
        /// <summary>
        /// 初始化宝珠摘除表格数据
        /// </summary>
        void InitRemovJewelsTable()
        {
            mRemovJewelsTableDic.Clear();

            var mTable = TableManager.GetInstance().GetTable<RemovejewelsTable>().GetEnumerator();
            while (mTable.MoveNext())
            {
                RemovejewelsTable item = mTable.Current.Value as RemovejewelsTable;
                if (item == null)
                {
                    continue;
                }

                if (mRemovJewelsTableDic.ContainsKey(item.Colour) == false)
                {
                    mRemovJewelsTableDic.Add(item.Colour,new Dictionary<int, Dictionary<int, List<BeadPickItemModel>>>());
                }

                if (mRemovJewelsTableDic[item.Colour].ContainsKey(item.Grades) == false)
                {
                    mRemovJewelsTableDic[item.Colour].Add(item.Grades, new Dictionary<int, List<BeadPickItemModel>>());
                }

                if (mRemovJewelsTableDic[item.Colour][item.Grades].ContainsKey(item.BeadType) == false)
                {
                    mRemovJewelsTableDic[item.Colour][item.Grades].Add(item.BeadType, new List<BeadPickItemModel>());
                }

                BeadPickItemModel model = new BeadPickItemModel(item.Material1,item.Num1,item.Success1,item.PickNum);
                mRemovJewelsTableDic[item.Colour][item.Grades][item.BeadType].Add(model);
            }
        }
        
        void InitBeadRandomBuff()
        {
            mBeadRandomBuffDic.Clear();

            var mBeadRandomBuffs = TableManager.GetInstance().GetTable<BeadRandomBuff>().GetEnumerator();
            while (mBeadRandomBuffs.MoveNext())
            {
                var mTable = mBeadRandomBuffs.Current.Value as BeadRandomBuff;
                if (mTable == null)
                {
                    continue;
                }

                if (mBeadRandomBuffDic.ContainsKey(mTable.BuffinfoID) == false)
                {
                    mBeadRandomBuffDic.Add(mTable.BuffinfoID, mTable);
                }
            }
        }

        /// <summary>
        /// 初始化宝珠置换表
        /// </summary>
        void InitReplaceJewelsTable()
        {
            mReplaceJewelsTableDic.Clear();

            var mTable = TableManager.GetInstance().GetTable<ReplacejewelsTable>().GetEnumerator();
            while (mTable.MoveNext())
            {
                ReplacejewelsTable item = mTable.Current.Value as ReplacejewelsTable;
                if (item == null)
                {
                    continue;
                }

                if (mReplaceJewelsTableDic.ContainsKey(item.Colour) == false)
                {
                    mReplaceJewelsTableDic.Add(item.Colour, new Dictionary<int, Dictionary<int, ReplacejewelsTable>>());
                }

                if (mReplaceJewelsTableDic[item.Colour].ContainsKey(item.Grades) == false)
                {
                    mReplaceJewelsTableDic[item.Colour].Add(item.Grades, new Dictionary<int, ReplacejewelsTable>());
                }

                if (mReplaceJewelsTableDic[item.Colour][item.Grades].ContainsKey(item.BeadType) == false)
                {
                    mReplaceJewelsTableDic[item.Colour][item.Grades].Add(item.BeadType, item);
                }
                
            }
        }


        /// <summary>
        /// 根据宝珠品质、等级、类型得到宝珠置换表数据
        /// </summary>
        /// <returns></returns>
        public ReplacejewelsTable GetBeadReplaceJewelsTableData(int cololLevel, int beadLevel, int beadType)
        {
            if (mReplaceJewelsTableDic.ContainsKey(cololLevel))
            {
                if (mReplaceJewelsTableDic[cololLevel].ContainsKey(beadLevel))
                {
                    if (mReplaceJewelsTableDic[cololLevel][beadLevel].ContainsKey(beadType))
                    {
                        return mReplaceJewelsTableDic[cololLevel][beadLevel][beadType];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据宝珠升级随机属性表的属性ID得到表格数据
        /// </summary>
        /// <param name="buffInfoID"></param>
        /// <returns></returns>
        public BeadRandomBuff GetBeadRandomBuffData(int buffInfoID)
        {
           BeadRandomBuff data = null;
           if (mBeadRandomBuffDic.ContainsKey(buffInfoID))
           {
                data = mBeadRandomBuffDic[buffInfoID];
           }

            return data;
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneAddPreciousBeadRet.MsgID, OnRecvSceneAddPreciousBeadRet);
            NetProcess.AddMsgHandler(SceneMountPreciousBeadRet.MsgID, OnSceneMountPreciousBeadRet);
            NetProcess.AddMsgHandler(SceneExtirpePreciousBeadRet.MsgID, OnSceneExtirpePreciousBeadRet);
            NetProcess.AddMsgHandler(SceneUpgradePreciousbeadRes.MsgID, OnSceneUpgradePreciousbeadRes);
            NetProcess.AddMsgHandler(SceneReplacePreciousBeadRet.MsgID, OnSceneReplacePreciousBeadRet);
            NetProcess.AddMsgHandler(SceneBeadConvertRes.MsgID, OnSceneBeadConvertRes);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneAddPreciousBeadRet.MsgID, OnRecvSceneAddPreciousBeadRet);
            NetProcess.RemoveMsgHandler(SceneMountPreciousBeadRet.MsgID, OnSceneMountPreciousBeadRet);
            NetProcess.RemoveMsgHandler(SceneExtirpePreciousBeadRet.MsgID, OnSceneExtirpePreciousBeadRet);
            NetProcess.RemoveMsgHandler(SceneUpgradePreciousbeadRes.MsgID, OnSceneUpgradePreciousbeadRes);
            NetProcess.RemoveMsgHandler(SceneReplacePreciousBeadRet.MsgID, OnSceneReplacePreciousBeadRet);
            NetProcess.RemoveMsgHandler(SceneBeadConvertRes.MsgID, OnSceneBeadConvertRes);
        }

        public string GetCondition(int iBeadCard)
        {
            var kStringBuilder = StringBuilderCache.Acquire();
            kStringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_condition_color"));
            //kStringBuilder.Append(TR.Value("enchantments_part"));
            //kStringBuilder.Append("(");

            var beatItem = TableManager.GetInstance().GetTableItem<ProtoTable.BeadTable>(iBeadCard);
            if (beatItem != null)
            {
                for (int i = 0; i < beatItem.Parts.Count; ++i)
                {
                    var describe = Utility.GetEnumDescription((EEquipWearSlotType)beatItem.Parts[i]);
                    kStringBuilder.Append(TR.Value(describe));
                    if (i != beatItem.Parts.Count - 1)
                    {
                        kStringBuilder.Append("、");
                    }
                }
            }

            //kStringBuilder.Append(")");
            kStringBuilder.Append("</color>");
            string ret = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);

            return ret;
        }

        public string GetAttributesDesc(int iID,bool isSubstitutionBox = false)
        {
            string ret = "";
            var beadItem = TableManager.GetInstance().GetTableItem<ProtoTable.BeadTable>(iID);
            if (beadItem == null)
            {
                return ret;
            }

            if (beadItem.BuffInfoIDPve.Count <= 0 && beadItem.BuffInfoIDPvp.Count <= 0)
            {
                ret = EnchantmentsCardManager.GetInstance().MagicCardAndBeadCardGetAttributesDesc(beadItem.PropType, beadItem.PropValue, beadItem.BuffInfoIDPve, beadItem.SkillAttributes, ret, beadItem.ProminentAtt == 1, isSubstitutionBox);
            }
            else
            {
                //判断Pve和Pvp两个数组值是否相同
                bool IsValueSame = false;

                for (int i = 0; i < beadItem.BuffInfoIDPve.Count; i++)
                {
                    if (beadItem.BuffInfoIDPve[i] == beadItem.BuffInfoIDPvp[i])
                    {
                        IsValueSame = true;
                    }
                    else
                    {
                        IsValueSame = false;
                    }

                    break;
                }

                if (IsValueSame)
                {
                    ret = EnchantmentsCardManager.GetInstance().MagicCardAndBeadCardGetAttributesDesc(beadItem.PropType, beadItem.PropValue, beadItem.BuffInfoIDPve, beadItem.SkillAttributes, ret, beadItem.ProminentAtt == 1, isSubstitutionBox);
                }
                else
                {
                    ret = string.Format("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), beadItem.Instruction);
                }
            }
           
            return ret;
        }

        public string GetBeadRandomAttributesDesc(int iBuffId)
        {
            string str = "";

            var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(iBuffId);
            if (null != bufferitem)
            {
                if (bufferitem.Description.Count > 0)
                {
                    str = string.Format("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), bufferitem.Description[0]);
                }
            }

            return str;
        }

        /// <summary>
        /// 宝珠摘取剩余次数
        /// </summary>
        /// <returns></returns>
        public string GetBeadPickRemainNumber(int TableID,int BeadPickNumber)
        {
            string strDes = "";
            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(TableID);

            if (mBeadTable != null)
            {
                var mBeadPickItemLists = BeadCardManager.GetInstance().GetBeadExpendItemModel(mBeadTable.Color, mBeadTable.Level, mBeadTable.BeadType);
                if (mBeadPickItemLists != null)
                {
                    for (int i = 0; i < mBeadPickItemLists.Count; i++)
                    {
                        var beadPickItem = mBeadPickItemLists[i];

                        var mExpendItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(beadPickItem.mExpendItemID);
                        if (mExpendItemData == null)
                        {
                            continue;
                        }

                        if (mExpendItemData.SubType != (int)ItemTable.eSubType.ST_PEARL_HAMMER)
                        {
                            continue;
                        }

                        if (beadPickItem.mBeadPickTotleNumber <= 0)
                        {
                            continue;
                        }

                        strDes = TR.Value("BeadPickRemainNumber", beadPickItem.mBeadPickTotleNumber - BeadPickNumber);
                    }
                }
            }

            return strDes;
        }

        /// <summary>
        /// 宝珠置换剩余次数
        /// </summary>
        /// <returns></returns>
        public string GetBeadReplaceRemainNumber(int TableID, int BeadReplaceNumber)
        {
            string strDes = "";
            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(TableID);

            if (mBeadTable != null)
            {
                ReplacejewelsTable mReplacejewelsTable = GetBeadReplaceJewelsTableData(mBeadTable.Color, mBeadTable.Level, mBeadTable.BeadType);
                if (mReplacejewelsTable != null)
                {
                    if (mReplacejewelsTable.ReplaceNum == -1)
                    {
                        strDes = TR.Value("ReplaceNumberDesc","不限");
                    }
                    else if (mReplacejewelsTable.ReplaceNum > 0)
                    {
                        //剩余置换次数
                        int mReplaceRemainNuber = mReplacejewelsTable.ReplaceNum - BeadReplaceNumber;
                        strDes = TR.Value("ReplaceNumberDesc", mReplaceRemainNuber);
                    }
                }
            }

            return strDes;
        }

        /// <summary>
        /// 找到本职业属性ID显示绿色字体排在第一列
        /// </summary>
        /// <param name="iBuffIDs"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool FindProminentAttID(IList<int> iBuffIDs, ref int buffId)
        {
            bool isFind = false;
            for (int i = 0; i < iBuffIDs.Count; i++)
            {
                var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(iBuffIDs[i]);
                if (bufferitem == null)
                {
                    continue;
                }


                if (bufferitem.SkillID.Count <= 0)
                {
                    continue;
                }

                for (int j = 0; j < bufferitem.SkillID.Count; j++)
                {
                    var skillTable = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(bufferitem.SkillID[j]);
                    if (skillTable == null)
                    {
                        continue;
                    }

                    if (skillTable.JobID.Length <= 0)
                    {
                        continue;
                    }

                    if (skillTable.JobID.Contains(PlayerBaseData.GetInstance().JobTableID))
                    {
                        isFind = true;
                        buffId = iBuffIDs[i];
                        break;
                    }
                }

                //找到了跳出循环
                if (isFind)
                {
                    break;
                }
            }

            return isFind;
        }

        #region netmsg
        public void SendAddSarah(ulong cardid, ulong itemid)
        {
            SceneAddPreciousBeadReq kCmd = new SceneAddPreciousBeadReq();
            var leftItem = ItemDataManager.GetInstance().GetItem(cardid);
            var rightItem = ItemDataManager.GetInstance().GetItem(itemid);
            if (leftItem != null && rightItem != null)
            {
                kCmd.preciousBeadUid = leftItem.GUID;
                kCmd.itemUid = rightItem.GUID;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
            }
        }

        //[MessageHandle(SceneAddPreciousBeadRet.MsgID)]
        void OnRecvSceneAddPreciousBeadRet(MsgDATA msgData)
        {
            SceneAddPreciousBeadRet kRet = new SceneAddPreciousBeadRet();
            kRet.decode(msgData.bytes);
            if (kRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.code);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAddSarahSuccess);

                SarahResultFrame.SarahResultFrameData data = new SarahResultFrame.SarahResultFrameData();
                data.bMerge = false;
                data.iCardTableID = (int)kRet.preciousBeadId;
                data.itemData = ItemDataManager.GetInstance().GetItem(kRet.itemUid);

                if (ClientSystemManager.GetInstance().IsFrameOpen<SarahResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<SarahResultFrame>();
                }
                ClientFrame.OpenTargetFrame<SarahResultFrame>(FrameLayer.Middle, data);
            }
        }
        #endregion

#region   宝珠镶嵌New

        public void SedndSceneMountPreciousBeadReq(ulong beadid, ulong itemid,byte holeIndex)
        {
            SceneMountPreciousBeadReq req = new SceneMountPreciousBeadReq();
            var beadItem = ItemDataManager.GetInstance().GetItem(beadid);
            var item = ItemDataManager.GetInstance().GetItem(itemid);
            if (beadItem != null && item != null)
            {
                req.preciousBeadUid = beadItem.GUID;
                req.itemUid = item.GUID;
                req.holeIndex = holeIndex;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
        }

        private void OnSceneMountPreciousBeadRet(MsgDATA msgData)
        {
            SceneMountPreciousBeadRet kRet = new SceneMountPreciousBeadRet();
            kRet.decode(msgData.bytes);
            if (kRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.code);
            }
            else
            {
                SarahResultFrame.SarahResultFrameData data = new SarahResultFrame.SarahResultFrameData();
                data.bMerge = false;
                data.iCardTableID = (int)kRet.preciousBeadId;
                data.itemData = ItemDataManager.GetInstance().GetItem(kRet.itemUid);
                data.iHoleIndex = kRet.holeIndex;
                data.iTitleType = (int)TitleType.TT_INALYSUCCESS;

                if (ClientSystemManager.GetInstance().IsFrameOpen<SarahResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<SarahResultFrame>();
                }

                ClientFrame.OpenTargetFrame<SarahResultFrame>(FrameLayer.Middle, data);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAddSarahSuccess);
        }

        public void SendSceneExtirpePreciousBeadReq(byte holeIndex,ulong itemUid,uint pestleId)
        {
            SceneExtirpePreciousBeadReq req = new SceneExtirpePreciousBeadReq();
            req.holeIndex = holeIndex;
            req.itemUid = itemUid;
            req.pestleId = pestleId;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        void OnSceneExtirpePreciousBeadRet(MsgDATA msgData)
        {
            SceneExtirpePreciousBeadRet kRet = new SceneExtirpePreciousBeadRet();
            kRet.decode(msgData.bytes);

            if (kRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.code);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("BeadPickSuccess"));
                if (ClientSystemManager.GetInstance().IsFrameOpen<BeadPickFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<BeadPickFrame>();
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BeadPickSuccess);
        }
        #endregion

        #region 宝珠升级
        public void SendSceneUpgradePreciousbeadReq(BeadItemModel model, int mExpendBeadID)
        {
            SceneUpgradePreciousbeadReq req = new SceneUpgradePreciousbeadReq();
            if (model.mountedType == (int)UpgradePrecType.Mounted)
            {
                req.mountedType = (byte)model.mountedType;
                req.equipGuid = model.equipItemData.GUID;
                req.eqPrecHoleIndex = (byte)model.eqPrecHoleIndex;
                req.precId = (uint)model.beadItemData.TableID;
            }
            else
            {
                req.mountedType = (byte)model.mountedType;
                req.precGuid = model.beadItemData.GUID;
            }

            req.consumePrecId = (uint)mExpendBeadID;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        void OnSceneUpgradePreciousbeadRes(MsgDATA msgData)
        {
            SceneUpgradePreciousbeadRes kRet = new SceneUpgradePreciousbeadRes();
            kRet.decode(msgData.bytes);

            if (kRet.retCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.retCode);
            }
            else
            {
                BeadUpgradeResultData mData = new BeadUpgradeResultData((int)kRet.mountedType,kRet.equipGuid,(int)kRet.precId,(int)kRet.addBuffId,kRet.newPrebeadUid);

                if (ClientSystemManager.GetInstance().IsFrameOpen<BeadUpgradeResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<BeadUpgradeResultFrame>();
                }

                ClientSystemManager.GetInstance().OpenFrame<BeadUpgradeResultFrame>(FrameLayer.Middle, mData);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnBeadUpgradeSuccess, mData);
            }
        }
        #endregion

#region 宝珠完美置换
        public void OnSendSceneReplacePreciousBeadReq(byte holeIndex, ulong itemUid, ulong beadId)
        {
            SceneReplacePreciousBeadReq req = new SceneReplacePreciousBeadReq();
            req.holeIndex = holeIndex;
            req.itemUid = itemUid;
            req.preciousBeadUid = beadId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void OnSceneReplacePreciousBeadRet(MsgDATA msgData)
        {
            SceneReplacePreciousBeadRet kRet = new SceneReplacePreciousBeadRet();
            kRet.decode(msgData.bytes);

            if (kRet.retCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.retCode);
            }
            else
            {
                SarahResultFrame.SarahResultFrameData data = new SarahResultFrame.SarahResultFrameData();
                data.bMerge = false;
                data.iCardTableID = (int)kRet.preciousBeadId;
                data.itemData = ItemDataManager.GetInstance().GetItem(kRet.itemUid);
                data.iHoleIndex = kRet.holeIndex;
                data.iTitleType = (int)TitleType.TT_REPLACEMENTSUCCESS;

                if (ClientSystemManager.GetInstance().IsFrameOpen<SarahResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<SarahResultFrame>();
                }

                if (ClientSystemManager.GetInstance().IsFrameOpen<BeadPerfectReplacementFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<BeadPerfectReplacementFrame>();
                }

                ClientFrame.OpenTargetFrame<SarahResultFrame>(FrameLayer.Middle, data);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAddSarahSuccess);
            }
        }
        #endregion

        #region 宝珠转换
        /// <summary>
        /// 宝珠转换请求
        /// </summary>
        /// <param name="treasureGuid">转换宝珠的guid</param>
        /// <param name="materialGuid">0代表消耗金币，guid表示消耗的宝珠</param>
        public void OnSceneBeadConvertReq(ulong treasureGuid,ulong materialGuid)
        {
            SceneBeadConvertReq req = new SceneBeadConvertReq();
            req.beadGuid = treasureGuid;
            req.materialGuid = materialGuid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }


        private void OnSceneBeadConvertRes(MsgDATA msg)
        {
            SceneBeadConvertRes res = new SceneBeadConvertRes();
            res.decode(msg.bytes);

            if (res.retCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureConversionSuccessed);
            }
        }

        public bool CheckTreasureConvertActivityOpon()
        {
            ActivityInfo activityInfo = ActiveManager.GetInstance().GetActivityInfo(TR.Value("TreasureConvert_activity_name"));
            if (activityInfo == null)
            {
                return false;
            }
            
            if (activityInfo.level > PlayerBaseData.GetInstance().Level)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}

