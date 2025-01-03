using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Network;
using ProtoTable;
using System;

using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace GameClient
{
    public class DungeonUtility 
    {
        /// <summary>
        /// 复活类型
        /// </summary>
        public enum eDungeonRebornType
        {
            /// <summary>
            /// 非法复活类型
            /// </summary>
            None,

            /// <summary>
            /// VIP每日免费复活
            /// </summary>
            VipFreeReborn,

            /// <summary>
            /// 正常消耗复活币复活
            /// </summary>
            NormalReborn,

            /// <summary>
            /// 快速购买复活
            /// </summary>
            QuickBuyReborn,

            /// <summary>
            /// 没钱快速购买复活
            /// </summary>
            NoCostItem2Reborn,

            /// <summary>
            /// 地下城复活上限
            /// </summary>
            NoCount2Reborn,
        }

        /// <summary>
        /// 是否有VIP的权益
        /// </summary>
        private static bool _hasGotVipRight()
        {
            return GetVipRebornSumCount() > 0;
        }

        public static int GetVipRebornSumCount()
        {
            float num = Utility.GetCurVipLevelPrivilegeData(VipPrivilegeTable.eType.FREE_REVIVE);
            if (num <= 0.0f)
            {
                return 0;
            }

            return (int)num;
        }

        public static int GetVipRebornLeftCount()
        {
            int num = GetVipRebornSumCount();

            if (num > 0)
            {
                return num - CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_VIP_FREEREBORN_USENUM);
            }

            return -1;
        }

        private static bool _isRebornLimit(int dungeonID)
        {
            // TODO 复活次数限制
            return false;
        }

        private static bool _isVipFreeReborn()
        {
            return GetVipRebornLeftCount() > 0;
        }

        /// <summary>
        /// 获得当前玩家的地下城的复活类型
        ///
        /// <param name="dungeonID"> 地下城ID </param>
        /// <param name="isRebornSelf"> 是否是复活自己 </param>
        /// </summary>
        public static eDungeonRebornType GetDungeonRebornType(int dungeonID, bool isRebornSelf)
        {
            if (null == BattleMain.instance)
            {
                return eDungeonRebornType.None;
            }

            eDungeonRebornType type = eDungeonRebornType.None;

            if (_isRebornLimit(dungeonID))
            {
                type = eDungeonRebornType.NoCount2Reborn;
            }
            else 
            {
                if (_isVipFreeReborn() && isRebornSelf)
                {
                    type = eDungeonRebornType.VipFreeReborn;
                }
                else 
                {
                    BattlePlayer localPlayer = BattleMain.instance.GetLocalPlayer();
                    if (localPlayer.CanUseItem(Battle.DungeonItem.eType.RebornCoin, 1))
                    {
                        type = eDungeonRebornType.NormalReborn;
                    }
                    else 
                    {
                        if (Utility.CanQuickBuyItem(ProtoTable.ItemTable.eSubType.ResurrectionCcurrency))
                        {
                            type = eDungeonRebornType.QuickBuyReborn;
                        }
                        else 
                        {
                            type = eDungeonRebornType.NoCostItem2Reborn;
                        }
                    }
                }
            }

            return type;
        }


        /// <summary>
        ///
        /// 是否能够复活
        /// 
        /// <param name="dungeonID"> 地下城ID </param>
        /// <param name="isRebornSelf"> 是否是复活自己 </param>
        /// </summary>
        public static bool CanReborn(int dungeonID, bool isRebornSelf)
        {
            if (BattleMain.instance == null) return false;
            if (BattleMain.instance.GetDungeonManager() != null && BattleMain.instance.GetDungeonManager().IsFinishFight()) return false;
            eDungeonRebornType type = GetDungeonRebornType(dungeonID, isRebornSelf);

            switch (type)
            {
                case eDungeonRebornType.None:
                case eDungeonRebornType.NoCount2Reborn:
                case eDungeonRebornType.NoCostItem2Reborn:
                    return false;
            }

            return true;
        }

        private static void _rebornCommand(byte who, byte target)
        {
            if (!BattleMain.IsModeMultiplayer(BattleMain.mode))
            {
                RebornFrameCommand cmd = new RebornFrameCommand();
                cmd.seat               = who;
                cmd.targetSeat         = target;
                FrameSync.instance.FireFrameCommand(cmd);
            }
        }

        private static uint _getRebornReciveID(byte seat)
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(seat);

            if (null != player)
            {
                return (uint)(seat * 100) + (uint)player.statistics.data.deadCount;
            }

            return FrameSync.instance.curFrame * 10 + seat;
        }

        /// <summary>
        /// 复活
        ///
        /// <param name="who">谁</param>
        /// <param name="target">目标</param>
        /// <param name="cnt">需要消耗的复活币的数量</param>
        /// </summary>
        public static IEnumerator Reborn(byte who, byte target, uint cnt, bool isVip = false)
        {
            SceneDungeonReviveReq req = new SceneDungeonReviveReq();
            SceneDungeonReviveRes res = new SceneDungeonReviveRes();
            MessageEvents events      = new MessageEvents();

			BattlePlayer whopay 	  = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(who);
            BattlePlayer player       = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(target);
            if (null == player || null == whopay)
            {
                // TODO the player not exist
                yield break;
            }

            req.reviveCoinNum         = cnt;
            req.targetId              = player.playerInfo.roleId;
            req.reviveId              = _getRebornReciveID(target);

            yield return MessageUtility.WaitWithResend<SceneDungeonReviveReq, SceneDungeonReviveRes>(ServerType.GATE_SERVER, events, req, res);

            if (events.IsAllMessageReceived())
            {
                if (res.result == 0)
                {
                    if (!isVip)
                    {
                        whopay.UseItem(Battle.DungeonItem.eType.RebornCoin, (ushort)cnt);
                    }

                    // TODO reborn success
                    _rebornCommand(who, target);

                }
                else 
                {
                    // TODO reborn Failed
                    SystemNotifyManager.SystemNotify((int)res.result);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, target);
                }
            }
            else 
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, target);
                // TODO reborn Failed
            }
        }


        /// <summary>
        /// 快速复活的流程
        ///
        /// <param name="who">谁</param>
        /// <param name="target">目标</param>
        /// <param name="id">快速购买表中ID</param>
        /// </summary>
        public static IEnumerator QuickBuyReborn(byte who, byte target, int id)
        {
            QuickBuyFrame quickBuyFrame = QuickBuyFrame.Open(QuickBuyFrame.eQuickBuyType.FullScreen);

            quickBuyFrame.SetQuickBuyItem(id, 1);
            quickBuyFrame.SetRebornPlayerSeat(target);

            yield return Yielders.EndOfFrame;

            while (!QuickBuyFrame.IsOpen(QuickBuyFrame.eQuickBuyType.FullScreen))
            {
                yield return Yielders.EndOfFrame;
            }

            while (quickBuyFrame.state == QuickBuyFrame.eState.None)
            {
                yield return Yielders.EndOfFrame;
            }

            if (quickBuyFrame.state == QuickBuyFrame.eState.Success)
            {
                MessageEvents events = new MessageEvents();

                SceneQuickBuyReq req = new SceneQuickBuyReq();
                SceneQuickBuyRes res = new SceneQuickBuyRes();                

                BattlePlayer targetPlayer = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(target);

                req.type   = (byte)QuickBuyTargetType.QUICK_BUY_REVIVE;
                req.param1 = targetPlayer.playerInfo.roleId;
                req.param2 = _getRebornReciveID(target);

                yield return MessageUtility.WaitWithResend<SceneQuickBuyReq, SceneQuickBuyRes>(ServerType.GATE_SERVER, events, req, res);

                if (events.IsAllMessageReceived())
                {
                    if (0 != res.result)
                    {
                        SystemNotifyManager.SystemNotify((int)res.result);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, target);
                    }
                    else
                    {
                        _rebornCommand(who, target);
                    }
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, target);
                }
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, target);
            }

        }

        public static void StartRebornProcess(byte who, byte target, int dungeonID)
        {
            bool isSelfReborn = (who == target);

            eDungeonRebornType type = GetDungeonRebornType(dungeonID, isSelfReborn);

            Logger.LogProcessFormat("[StartRebornProcess] 当前复活的类型 {0}, 在 {1}, {2} 复活了 {3}", type, dungeonID, who, target);

            switch (type)
            {
                case eDungeonRebornType.NormalReborn:
                    {
                        GameFrameWork.instance.StartCoroutine(Reborn(who, target, 1));
                    }
                    break;
                case eDungeonRebornType.VipFreeReborn:
                    {
                        GameFrameWork.instance.StartCoroutine(Reborn(who, target, 1, true));
                    }
                    break;
                case eDungeonRebornType.QuickBuyReborn:
                    {
                        int id = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.ResurrectionCcurrency);

                        var mQuickRebornCoroutine = GameFrameWork.instance.StartCoroutine(QuickBuyReborn(who, target, id));

                        var mQuickRebornFrame = ClientSystemManager.instance.GetFrame(QuickBuyFrame._getFrameName(QuickBuyFrame.eQuickBuyType.FullScreen)) as QuickBuyFrame;
                        if (mQuickRebornFrame != null)
                        {
                            mQuickRebornFrame.SetRebornCoroutine(mQuickRebornCoroutine);
                        }
                    }
                    break;
                //case eDungeonRebornType.NoCount2Reborn:
                //    // TODO tip
                //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, who, target);
                //    break;
                //case eDungeonRebornType.NoCostItem2Reborn:
                //    // TODO tip
                //    SystemNotifyManager.SystemNotify(1098);
                //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, who, target);
                //    break;
                //case eDungeonRebornType.None:
                //    // TODO tip
                //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornFail, who, target);
                //    break;
            }
        }

        private static UInt32 _getBattleEncryptKey()
        {
            UInt32 key1 = BattleDataManager.GetInstance().BattleInfo.key1;
            UInt32 key2 = BattleDataManager.GetInstance().BattleInfo.key2;
            UInt32 key3 = BattleDataManager.GetInstance().BattleInfo.key3;
            UInt32 key4 = BattleDataManager.GetInstance().BattleInfo.key4;

            Logger.LogProcessFormat("[加密] 加密key1 {0}", key1);
            Logger.LogProcessFormat("[加密] 加密key2 {0}", key2);
            Logger.LogProcessFormat("[加密] 加密key3 {0}", key3);
            Logger.LogProcessFormat("[加密] 加密key4 {0}", key4);

            return (key1 & key4) ^ (key3 | (key4 << 16)) | key2;
        }

        //private static MD5 mBattleEncryptMD5Hash = MD5.Create();

        private static ManualMD5.MD5 mManualMD5 = new ManualMD5.MD5(); 

        public static byte[] GetBattleEncryptMD5(string msg)
        {
            string finalString      = string.Format("{0}###{1}", msg, _getBattleEncryptKey());

            Logger.LogProcessFormat("[加密] 加密字符串 {0}", finalString);

            byte[] finalStringBytes = System.Text.Encoding.UTF8.GetBytes(finalString);

            if (null == finalStringBytes || finalStringBytes.Length <= 0)
            {
                Logger.LogProcessFormat("[加密] 转换成bytes出错 {0}", finalString);
                return new byte[16];
            }

            mManualMD5.ValueAsByte = finalStringBytes;
            byte[] md5Data = mManualMD5.FingerBytes;

            if (null == md5Data)
            {
                return new byte[16];
            }

            Logger.LogProcessFormat("[加密] MD5 数据长度 {0}", md5Data.Length);

            return md5Data;
        }

        public static byte[] GetMD5(string str)
        {
            byte[] finalStringBytes = System.Text.Encoding.UTF8.GetBytes(str);

            return GetMD5(finalStringBytes);
        }

        public static string GetMD5Str(byte[] bytes)
        {
            mManualMD5.ValueAsByte = bytes;
            return mManualMD5.FingerPrint;
        }

        public static byte[] GetMD5(byte[] bytes)
        {
            if (null == bytes)
            {
                return new byte[16];
            }

            mManualMD5.ValueAsByte = bytes;
            byte[] md5Data = mManualMD5.FingerBytes;

            if (null == md5Data)
            {
                return new byte[16];
            }

            return md5Data;
        }

        private static string _getFilePath(string path)
        {
            return Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(path + "_bytes", Utility.kRawDataExtension)).ToLower();
        }

        public static ISceneData LoadSceneData(string path)
        {
            ISceneData data = null;
#if USE_FB
            string dungeonPath = _getFilePath(path);
            if (File.Exists(dungeonPath))
            {
                byte[] dataBytes = System.IO.File.ReadAllBytes(dungeonPath);
                if (null != dataBytes)
                {
                    FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(dataBytes);
                    FBSceneData.DSceneData fbdata = FBSceneData.DSceneData.GetRootAsDSceneData(buffer);
                    data = new BattleSceneData(fbdata);
                }
            }
#else
            data = AssetLoader.instance.LoadRes(path, typeof(DSceneData)).obj as DSceneData;
#endif
            return data;
        }

        public static IDungeonData LoadDungeonData(string path)
        {
            IDungeonData data = null;
#if USE_FB
            string dungeonPath = _getFilePath(path);

            if (File.Exists(dungeonPath))
            {
                byte[] dataBytes = System.IO.File.ReadAllBytes(dungeonPath);
                if (null != dataBytes)
                {
                    FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(dataBytes);
                    FBDungeonData.DDungeonData fbdata = FBDungeonData.DDungeonData.GetRootAsDDungeonData(buffer);
                    data = new BattleDungeonData(fbdata);
                }
            }
#else
            data = AssetLoader.instance.LoadRes(path, typeof(DDungeonData)).obj as DDungeonData;
#endif
            return data;
        }

        public static string GetSceneTransportExtraDataPath(string path)
        {
            return path.Replace("Scene/", "Data/ExTransportData/");
        }

        public static ITransportDoorExtraData LoadSceneTransportExtraData(string path)
        {
            ITransportDoorExtraData data = null;

            path = GetSceneTransportExtraDataPath(path);
#if USE_FB
            string dungeonPath = _getFilePath(path);

            if(File.Exists(dungeonPath))
            {
                byte[] dataBytes = System.IO.File.ReadAllBytes(dungeonPath);
                if (null != dataBytes)
                {
                    FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(dataBytes);
                    FBTransportDoorExtraData.DTransportDoorExtraData fbdata = FBTransportDoorExtraData.DTransportDoorExtraData.GetRootAsDTransportDoorExtraData(buffer);

                    data = new SceneTransportDoorExtraData(fbdata);
                }
            }
#else
            data = AssetLoader.instance.LoadRes(path, typeof(DTransportDoorExtraData)).obj as DTransportDoorExtraData;
#endif
            return data;
        }

        public static ISceneRegionInfoData CreateSceneRegionInfoData(int id, Vector3 pos)
        {
            return new DRegionInfo() { resid = id, position = pos };
        }

        /// <summary>
        /// 获得地下城每日基础挑战次数
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public static int GetDungeonDailyBaseTimes(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return 0;

            var dungeonTimesTable =
                TableManager.GetInstance().GetTableItem<DungeonTimesTable>((int) dungeonTable.SubType);

            if (dungeonTimesTable == null)
                return 0;

            ////每日最大次数，由基础次数和购买的次数决定
            //int dailyMaxTime = dungeonTimesTable.BaseTimes;
            //dailyMaxTime = dailyMaxTime + CountDataManager.GetInstance().GetCount(dungeonTimesTable.BuyTimesCounter);

            //每日基础挑战次数，只使用地下城次数表中，相关的基础次数
            var dailyBaseTimes = dungeonTimesTable.BaseTimes;
            if (dungeonTable.SubType == DungeonTable.eSubType.S_DEVILDDOM)
            {
                if (ActivityDataManager.GetInstance().GettAnniverTaskIsFinish(EAnniverBuffPrayType.XuKongChallengeNumAdd))
                {
                    dailyBaseTimes += ActivityDataManager.GetInstance().GetAnniverTaskValue(EAnniverBuffPrayType.XuKongChallengeNumAdd);
                }
            }
            return dailyBaseTimes;
        }

        //获得地下城每日最大挑战次数
        public static int GetDungeonDailyMaxTimes(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return 0;

            var dungeonTimesTable =
                TableManager.GetInstance().GetTableItem<DungeonTimesTable>((int)dungeonTable.SubType);

            if (dungeonTimesTable == null)
                return 0;

            //每日最大次数，由基础次数和购买的次数决定
            var dailyMaxTime = dungeonTimesTable.BaseTimes;
            dailyMaxTime = dailyMaxTime + CountDataManager.GetInstance().GetCount(dungeonTimesTable.BuyTimesCounter);
            if (dungeonTable.SubType == DungeonTable.eSubType.S_DEVILDDOM)
            {
                if (ActivityDataManager.GetInstance().GettAnniverTaskIsFinish(EAnniverBuffPrayType.XuKongChallengeNumAdd))
                {
                    dailyMaxTime += ActivityDataManager.GetInstance().GetAnniverTaskValue(EAnniverBuffPrayType.XuKongChallengeNumAdd);
                }
            }
            return dailyMaxTime;
        }
        //获得地下城今日已经挑战的次数
        public static int GetDungeonDailyFinishedTimes(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return 0;

            var dungeonTimesTable =
                TableManager.GetInstance().GetTableItem<DungeonTimesTable>((int)dungeonTable.SubType);

            if (dungeonTimesTable == null)
                return 0;

            return CountDataManager.GetInstance().GetCount(dungeonTimesTable.UsedTimesCounter);
        }

        //获得地下城今日剩余的挑战次数
        //最大次数 - 完成次数
        public static int GetDungeonDailyLeftTimes(int dungeonId)
        {
            var dungeonDailyMaxTimes = GetDungeonDailyMaxTimes(dungeonId);
            if (dungeonDailyMaxTimes <= 0)
                return 0;

            var dungeonDailyFinishTimes = GetDungeonDailyFinishedTimes(dungeonId);

            var dungeonDailyLeftTimes = dungeonDailyMaxTimes - dungeonDailyFinishTimes;

            if (dungeonDailyLeftTimes <= 0)
                return 0;

            return dungeonDailyLeftTimes;
        }

        //获得地下城本周剩余的挑战次数
        //最大次数 - 完成次数
        public static int GetDungeonWeekLeftTimes(int dungeonId)
        {
            var dungeonWeekMaxTimes = GetDungeonWeekMaxTimes(dungeonId);
            if (dungeonWeekMaxTimes <= 0)
                return 0;

            var dungeonWeekFinishTimes = GetDungeonWeekFinishedTimes(dungeonId);

            var dungeonWeekLeftTimes = dungeonWeekMaxTimes - dungeonWeekFinishTimes;

            if (dungeonWeekLeftTimes <= 0)
                return 0;

            return dungeonWeekLeftTimes;
        }

        //获得地下城本周已经挑战的次数
        public static int GetDungeonWeekFinishedTimes(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return 0;

            var dungeonTimesTable =
                TableManager.GetInstance().GetTableItem<DungeonTimesTable>((int)dungeonTable.SubType);

            if (dungeonTimesTable == null)
                return 0;

            return CountDataManager.GetInstance().GetCount(dungeonTimesTable.WeekUsedTimesCounter);
        }

        //获得地下城每周最大挑战次数
        public static int GetDungeonWeekMaxTimes(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return 0;

            var dungeonTimesTable =
                TableManager.GetInstance().GetTableItem<DungeonTimesTable>((int)dungeonTable.SubType);

            if (dungeonTimesTable == null)
                return 0;

            //每周最大次数，由基础次数和购买的次数决定
            var dailyMaxTime = dungeonTimesTable.WeekTimesLimit;
            dailyMaxTime = dailyMaxTime + CountDataManager.GetInstance().GetCount(dungeonTimesTable.BuyTimesCounter);

            return dailyMaxTime;
        }


        public static string GetDungeonRebornNumber(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return "0";

            if (dungeonTable.RebornCount < 0)
                return "10000";
            return dungeonTable.RebornCount.ToString();
        }

        public static string GetDungeonRebornValue(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return "0";

            if (dungeonTable.RebornCount < 0)
                return "不限";
            return dungeonTable.RebornCount.ToString();
        }

        //获得地下城的抗魔值：
        public static int GetDungeonResistMagicValueById(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return 0;

            return dungeonTable.ResistMagic;
        }

        //在进入游戏的时候，判断是否显示抗魔值不足的提示
        public static bool IsShowDungeonResistMagicValueTip(int dungeonId, ref string content)
        {
            bool isShow = false;

            var dungeonResistMagicValue = DungeonUtility.GetDungeonResistMagicValueById(dungeonId);
            //关卡不存在抗魔值，不显示
            if (dungeonResistMagicValue <= 0)
            {
                isShow = false;
            }
            else
            {
                //不存在队伍
                if (TeamDataManager.GetInstance().HasTeam() == false)
                {
                    var ownerResistMagicValue = DungeonUtility.GetDungeonMainPlayerResistMagicValue();
                    //自己抗魔值不足
                    if (dungeonResistMagicValue > ownerResistMagicValue)
                    {
                        content = TR.Value("resist_magic_owner_not_enough_tip");
                        string lessStr =
                            GetMagicValueNotEnoughEffectStr(ownerResistMagicValue, dungeonResistMagicValue);
                        content += string.Format(TR.Value("resist_magic_owner_not_enough_value"), lessStr);
                        content += TR.Value("resist_magic_level_enter");
                        isShow = true;          //显示提示
                    }
                }
                else
                {
                    //存在队伍
                    var teamData = TeamDataManager.GetInstance().GetMyTeam();
                    if (teamData != null)
                    {
                        content = TR.Value("resist_magic_team_member_not_enough_tip");
                        for (var i = 0; i < teamData.members.Length; i++)
                        {
                            var curMemberData = teamData.members[i];
                            if(curMemberData == null || curMemberData.id <= 0)
                                continue;

                            var teamMemberResistMagicValue = (int)curMemberData.resistMagicValue;
                            if (dungeonResistMagicValue > teamMemberResistMagicValue)
                            {
                                isShow = true;
                                string lessStr = GetMagicValueNotEnoughEffectStr(teamMemberResistMagicValue,
                                    dungeonResistMagicValue);
                                content += string.Format(TR.Value("resist_magic_team_member_not_enough_value"),
                                    curMemberData.name, lessStr);
                            }
                        }

                        if (isShow == false)
                        {
                            content = string.Empty;
                        }
                        else
                        {
                            content += TR.Value("resist_magic_level_enter");
                        }
                    }
                }
            }
            return isShow;
        }

        public static string GetMagicValueNotEnoughEffectStr(int ownerResistMagicValue, int dungeonResistMagicValue)
        {
            //return "-20%";
            var floatValue = (((float) ownerResistMagicValue / (float) dungeonResistMagicValue) - 1) * 100;
            //抗魔值的显示范围在-70% ~~ 20 % 之间
            var intValue = (int) floatValue;
            if (intValue > 20)
            {
                intValue = 20;
            }
            else if (intValue < -70)
            {
                intValue = -70;
            }

            return string.Format("{0}%", intValue);
        }


        public static void ShowResistMagicValueTips(int dungeonResistMagicValue)
        {

            var teamData = TeamDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
                return;

            for (var i = 0; i < teamData.members.Length; i++)
            {
                var curMemberData = teamData.members[i];
                if(curMemberData == null ||curMemberData.id <= 0)
                    continue;

                var curResistMagicValue = teamData.members[i].resistMagicValue;
                if (dungeonResistMagicValue > curResistMagicValue)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("resist_magic_less_tip"));
                    return;
                }
            }
            return;
        }

        //获得主角的抗魔值
        public static int GetDungeonMainPlayerResistMagicValue()
        {
            if (PlayerBaseData.GetInstance() == null)
                return 0;

            if (PlayerBaseData.GetInstance().ResistMagicValue <= 0)
                return 0;

            return PlayerBaseData.GetInstance().ResistMagicValue;
        }

        //获取地下城限制复活次数
        public static int GetDungeonRebornCount(int dungeonId)
        {
            var dungeonTableData = TableManager.instance.GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTableData == null)
                return -1;
            return dungeonTableData.RebornCount;
        }

        #region HellDungeonInfo

        //地下城是否会限时深渊
        public static bool IsLimitTimeHellDungeon(int dungeonId)
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (mDungeonTable == null)
            {
                return false;
            }

            //活动类型，限时深渊
            if (mDungeonTable.Type == DungeonTable.eType.L_ACTIVITY)
            {
                if (mDungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL || mDungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL)
                    return true;
            }

            return false;
        }

        //地下城是否是免限时示深渊
        public static bool IsLimitTimeFreeHellDungeon(int dungeonId)
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);

            if (mDungeonTable == null)
                return false;

            if (mDungeonTable.Type == DungeonTable.eType.L_ACTIVITY)
            {
                if (mDungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL)
                    return true;
            }

            return false;
        }

        //地下城是否是周常深渊
        public static bool IsWeekHellDungeon(int dungeonId)
        {

            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (mDungeonTable == null)
            {
                return false;
            }

            if (mDungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL)
                return true;
            
            return false;
        }

        //是否为周常活动的前置关卡
        public static bool IsWeekHellPreDungeon(int dungeonId)
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (mDungeonTable == null)
            {
                return false;
            }

            if (mDungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER)
                return true;

            return false;
        }

        //是否为周常深渊的入口地下城
        public static bool IsWeekHellEntryDungeon(int dungeonId)
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (mDungeonTable == null)
            {
                return false;
            }

            if (mDungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
                return true;

            return false;
        }
		
		//组队地下城对应的地下城ID是否为周常深渊
        public static bool IsWeekHellTeamDungeon(int teamDungeonId)
        {
            var teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(teamDungeonId);
            if (teamDungeonTable == null)
                return false;

            if (teamDungeonTable.DungeonID <= 0)
                return false;

            return IsWeekHellEntryDungeon(teamDungeonTable.DungeonID);
        }


        //地下城前置任务的状态
        public static WeekHellPreTaskState GetWeekHellPreTaskState(int dungeonId)
        {
            ////test 没有领取
            ////return WeekHellPreTaskState.UnReceived;

            ////test 正在进行中
            //return WeekHellPreTaskState.IsProcessing;

            ////test 已经完成
            //return WeekHellPreTaskState.IsFinished;

            //任务不存在，返回
            var preTaskId = GetWeekHellPreTaskId(dungeonId);
            if (preTaskId <= 0)
                return WeekHellPreTaskState.None;

            MissionManager.SingleMissionInfo singleMissionInfo = null;
            MissionManager.GetInstance().taskGroup.TryGetValue((uint)preTaskId, out singleMissionInfo);

            //服务器没有同步任务的相关数据：任务已经完成，并且提交了
            if (singleMissionInfo == null)
                return WeekHellPreTaskState.IsFinished;

            switch (singleMissionInfo.status)
            {
                case (int)TaskStatus.TASK_INIT:
                    return WeekHellPreTaskState.UnReceived;
                case (int)TaskStatus.TASK_UNFINISH:
                case (int)TaskStatus.TASK_FAILED:
                    return WeekHellPreTaskState.IsProcessing;
            }
            return WeekHellPreTaskState.IsFinished;
        }

        //地下城前置任务的ID
        public static int GetWeekHellPreTaskId(int dungeonId)
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return 0;

            if (dungeonTable.PreTaskID <= 0)
                return 0;

            return dungeonTable.PreTaskID;
        }

        //得到前置任务的地下城ID
        public static int GetWeekHellPreTaskDungeonId(int dungeonId)
        {
            int preTaskId = GetWeekHellPreTaskId(dungeonId);
            if (preTaskId <= 0)
                return dungeonId;

            var taskTable = TableManager.GetInstance().GetTableItem<MissionTable>(preTaskId);
            if (taskTable == null || taskTable.MapID <= 0)
                return dungeonId;

            return taskTable.MapID;
        }

        //只是针对于周常深渊,周常深渊的前置关卡，周常深渊的入口
        public static bool IsWeekHellDungeonCanAgain(int dungeonId)
        {

            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return false;

            //周常深渊的前置关卡，判断剧情故事是否完成，如果完成，则不能再进行一次，（前置关卡只能进行一次)
            if (dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER)
            {
                //任务完成，不能进行；任务没有完成，则可以进行
                var isStoryTaskFinished = IsDungeonStoryTaskFinished(dungeonTable.storyTaskID);
                return isStoryTaskFinished != true;
            }
            else if (dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL)
            {
                //判断周常深渊的入口地下城次数是否大于0.每周的次数小于0， 每日的次数小于0
                var weekHellEntryDungeonId = dungeonTable.ownerEntryId;
                if (GetDungeonWeekLeftTimes(weekHellEntryDungeonId) <= 0)
                    return false;
                if (GetDungeonDailyLeftTimes(weekHellEntryDungeonId) <= 0)
                    return false;
                return true;

            }
            else if (dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
            {
                //判断周常深渊的入口地下城次数是否大于0.每周的次数小于0， 每日的次数小于0
                if (GetDungeonWeekLeftTimes(dungeonId) <= 0)
                    return false;
                if (GetDungeonDailyLeftTimes(dungeonId) <= 0)
                    return false;
                return true;
            }

            return true;
        }

        public static bool IsDungeonStoryTaskFinished(int storyTaskId)
        {
            MissionManager.SingleMissionInfo singleMissionInfo = null;
            MissionManager.GetInstance().taskGroup.TryGetValue((uint)storyTaskId, out singleMissionInfo);

            //服务器没有同步任务的相关数据：任务已经完成，并且提交了
            if (singleMissionInfo == null)
                return true;

            switch (singleMissionInfo.status)
            {
                case (int)TaskStatus.TASK_INIT:
                    return false;
                case (int)TaskStatus.TASK_UNFINISH:
                case (int)TaskStatus.TASK_FAILED:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 为暑假活动提供根据地下城ID获得地下城模型类型
        /// </summary>
        /// <param name="dungeonID"></param>
        /// <returns></returns>
        public static DungeonModelTable.eType GetDugeonModleTypeById(int dungeonID)
        {
            DungeonModelTable.eType type = DungeonModelTable.eType.Type_None;
            DungeonTable table= TableManager.GetInstance().GetTableItem <DungeonTable>(dungeonID);
            if(table!=null)
            {
                if (table.SubType == DungeonTable.eSubType.S_HELL_ENTRY)//深渊
                {
                    type = DungeonModelTable.eType.DeepModel;
                } else if (table.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY)//混沌 周常深渊
                {
                    type = DungeonModelTable.eType.WeekHellModel;
                }else if(table.SubType==DungeonTable.eSubType.S_DEVILDDOM)// 异界 虚空
                {
                    type = DungeonModelTable.eType.Type_None;
                }
            }
            return type;
        }

        #endregion

    }
}
