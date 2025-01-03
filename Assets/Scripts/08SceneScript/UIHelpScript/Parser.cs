using UnityEngine;
using System.Collections;
using System;
using Protocol;
using Network;
using GameClient;
using ProtoTable;

namespace Parser
{
    public struct ParserReturn
    {
        public string content;
        public string color;
        public UInt32 iId;
    }
    public interface IParser
    {
        ParserReturn OnParse(string value);
    }

    public class ParserParams
    {
        public Int32 Params0 = 0;
        public Int32 Params1 = 0;
    }

    public class ItemParser : IParser
    {
        public ParserReturn OnParse(string value)
        {
            ParserReturn ret;
            ret.color = "#FFFFFF";
            ret.iId = 0;
            ret.content = "";

            string temp = value;
            temp = temp.TrimStart('[');
            temp = temp.TrimEnd(']');

            Int32 iItemID = Int32.Parse(temp);
            ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iItemID);
            if (item == null)
            {
                return ret;
            }

            ret.color = item.Color.ToString().ToLower();
            ret.content = item.Name;
            ret.iId = (UInt32)iItemID;

            return ret;
        }

        public static void OnItemLink(ulong guid, Int32 iItemID, uint queryPlayerType = 0, uint zoneId = 0)
        {
            if(0 != guid)
            {
                WorldChatLinkDataReq kSend = new WorldChatLinkDataReq();
                kSend.type = (byte)'I';
                kSend.uid = guid;
                kSend.queryType = queryPlayerType;
                kSend.zoneId = zoneId;
                NetManager.Instance().SendCommand<WorldChatLinkDataReq>(ServerType.GATE_SERVER, kSend);
            }
            else
            {
                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(iItemID);
                if(null != itemData)
                {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                }
                else
                {
                    Logger.LogErrorFormat("itemData create failed with id = {0}", iItemID);
                }
            }
        }

        public static string GetItemColor(ProtoTable.ItemTable item)
        {
            string color = "white";

            if(item != null)
            {
                switch (item.Color)
                {
                    case ProtoTable.ItemTable.eColor.WHITE:
                        color = "white";
                        break;
                    case ProtoTable.ItemTable.eColor.BLUE:
                        color = "#00C0FF";
                        break;
                    case ProtoTable.ItemTable.eColor.PURPLE:
                        color = "#C000FF";
                        break;
                    case ProtoTable.ItemTable.eColor.GREEN:
                        color = "#00FF00";
                        break;
                    case ProtoTable.ItemTable.eColor.PINK:
                        //color = "#FF00C0";
                        //color = 5, color2 = 3 表示玫红色
                        if (item.Color2 == 3)
                        {
                            color = "#FF0058";
                        }
                        else
                        {
                            color = "#FF00C0";
                        }
                        break;
                    case ProtoTable.ItemTable.eColor.YELLOW:
                        color = "#FFC000";
                        break;
                    default:
                        color = "white";
                        break;
                }
            }

            return color;
        }

        public static SpriteAssetColor GetAssetColor(ProtoTable.ItemTable item)
        {
            SpriteAssetColor eSpriteAssetColor = SpriteAssetColor.SAC_WHITE;
            if(item != null)
            {
                switch (item.Color)
                {
                    case ProtoTable.ItemTable.eColor.WHITE:
                        eSpriteAssetColor = SpriteAssetColor.SAC_WHITE;
                        break;
                    case ProtoTable.ItemTable.eColor.BLUE:
                        eSpriteAssetColor = SpriteAssetColor.SAC_BLUE;
                        break;
                    case ProtoTable.ItemTable.eColor.PURPLE:
                        eSpriteAssetColor = SpriteAssetColor.SAC_PURPLE;
                        break;
                    case ProtoTable.ItemTable.eColor.GREEN:
                        eSpriteAssetColor = SpriteAssetColor.SAC_GREEN;
                        break;
                    case ProtoTable.ItemTable.eColor.PINK:
                        eSpriteAssetColor = SpriteAssetColor.SAC_PINK;
                        //color = 5 color2 = 3 红
                        if (item.Color2 == 3)
                        {
                            eSpriteAssetColor = SpriteAssetColor.SAC_PINK_RED;
                        }
                        break;
                    case ProtoTable.ItemTable.eColor.YELLOW:
                        eSpriteAssetColor = SpriteAssetColor.SAC_ORANGE;
                        break;
                    default:
                        eSpriteAssetColor = SpriteAssetColor.SAC_COUNT;
                        break;
                }
            }
            return eSpriteAssetColor;
        }
    }

    public class MonsterParser : IParser
    {
        public ParserReturn OnParse(string value)
        {
            ParserReturn ret;
            ret.color = "#ffffff";
            ret.iId = 0;
            ret.content = "";

            string temp = value;
            temp = temp.TrimStart('[');
            temp = temp.TrimEnd(']');

            Int32 iItemID = Int32.Parse(temp);
            ProtoTable.UnitTable item = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(iItemID);
            if (item == null)
            {
                return ret;
            }

            ret.color = "#179fcb";
            ret.content = item.Name;
            ret.iId = (UInt32)iItemID;

            return ret;
        }
    }

    class SceneJump
    {
        public OnLinkOk onLinkOk = null;

        public void OnMoveStateChanged(BeTownPlayerMain.EMoveState ePre, BeTownPlayerMain.EMoveState eCur)
        {

        }

        public void OnMoveSuccess()
        {
            BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
            BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
            BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);

            if (onLinkOk != null)
            {
                onLinkOk.Invoke();
                onLinkOk = null;
            }
        }

        public void OnAutoMoveFail()
        {
            BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
            BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
            BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);
            onLinkOk = null;
        }
    }

    public class DungeonParser : IParser
    {
        public ParserReturn OnParse(string value)
        {
            ParserReturn ret;
            ret.color = "#ffffff";
            ret.iId = 0;
            ret.content = "";

            string temp = value;
            temp = temp.TrimStart('[');
            temp = temp.TrimEnd(']');

            Int32 iItemID = Int32.Parse(temp);
            ProtoTable.DungeonTable item = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTable>(iItemID);
            if (item == null)
            {
                return ret;
            }

            ret.color = "#77e0d7";
            ret.content = item.Name;
            ret.iId = (UInt32)iItemID;

            return ret;
        }

        public static void OnClickLink(Int32 iDungenID, Int32 iTaskID)
        {
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            ProtoTable.DungeonTable dungenItem = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTable>(iDungenID);
            if(clientSystem != null && dungenItem != null)
            {
                GameClient.MissionManager.GetInstance().OnExecuteDungenTrace(iTaskID);
            }
        }

        class DungenTrace
        {
            public Int32 iDungenID = 0;
            public OnLinkOk onLinkOk = null;

            public void OnMoveStateChanged(BeTownPlayerMain.EMoveState ePre, BeTownPlayerMain.EMoveState eCur)
            {

            }

            public void OnMoveSuccess()
            {
                BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
                BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
                BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);
                OnTriggerDungen();
            }

            public void OnAutoMoveFail()
            {
                BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
                BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
                BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);
                onLinkOk = null;
            }

            void OnTriggerDungen()
            {
                ProtoTable.DungeonTable dungeonItem = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTable>(iDungenID);
                if (dungeonItem != null)
				{  
					ChapterSelectFrame.SetDungeonID(iDungenID);
                }
                if(onLinkOk != null)
                {
                    onLinkOk.Invoke();
                    onLinkOk = null;
                }
            }
        }

        public static bool OnClickLink(Int32 iDungenID, OnLinkOk onLinkOk = null)
        {
            ClientSystemTown clientSystem = ClientSystem.GetTargetSystem<ClientSystemTown>();
            if (clientSystem != null)
            {
                ProtoTable.DungeonTable dungeon = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTable>(iDungenID);
                if(dungeon != null)
                {
                    BeTownPlayerMain.CommandStopAutoMove();
                    DungenTrace dungenTrace = new DungenTrace();
                    dungenTrace.onLinkOk = onLinkOk;
                    dungenTrace.iDungenID = iDungenID;
                    BeTownPlayerMain.OnMoveStateChanged.AddListener(dungenTrace.OnMoveStateChanged);
                    BeTownPlayerMain.OnAutoMoveSuccess.AddListener(dungenTrace.OnMoveSuccess);
                    BeTownPlayerMain.OnAutoMoveFail.AddListener(dungenTrace.OnAutoMoveFail);
                    clientSystem.MainPlayer.CommandMoveToDungeon(iDungenID);
                    return true;
                }
            }
            return false;
        }
    }

    public class MissionParser : IParser
    {
        public ParserReturn OnParse(string value)
        {
            ParserReturn ret;
            ret.color = "#FFFFFF";
            ret.iId = 0;
            ret.content = "";

            string temp = value;
            temp = temp.TrimStart('[');
            temp = temp.TrimEnd(']');

            Int32 iItemID = Int32.Parse(temp);
            ProtoTable.MissionTable item = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iItemID);
            if (item == null)
            {
                return ret;
            }

            ret.color = "blue";
            ret.content = item.TaskName;
            ret.iId = (UInt32)iItemID;

            return ret;
        }
    }

    public class NpcParser : IParser
    {
        public ParserReturn OnParse(string value)
        {
            ParserReturn ret;
            ret.color = "#ffffff";
            ret.iId = 0;
            ret.content = "";

            string temp = value;
            temp = temp.TrimStart('[');
            temp = temp.TrimEnd(']');

            Int32 iItemID = Int32.Parse(temp);
            ProtoTable.NpcTable item = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iItemID);
            if (item == null)
            {
                return ret;
            }

            ret.color = TR.Value("parse_color_npc");
            ret.content = item.NpcName;
            ret.iId = (UInt32)iItemID;

            return ret;
        }

        public static void OnClickLink(UInt64 guid,
            Int32 npcId,
            ESceneActorType eSceneActorType = ESceneActorType.Npc,
            OnReached onReached = null,
            UnityEngine.Events.UnityAction onFailed = null)
        {
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(npcId);
            if (clientSystem != null && npcItem != null)
            {
                GameClient.TaskTrace.NpcTrace taskTrace = new GameClient.TaskTrace.NpcTrace();
                taskTrace.iNpcID = npcId;
                taskTrace.iTaskID = 0;
                taskTrace.onReached = onReached;
                taskTrace.bNeedDialog = false;
                taskTrace.onFailed = onFailed;

                BeTownPlayerMain.CommandStopAutoMove();
                GameClient.BeTownPlayerMain.OnMoveStateChanged.AddListener(taskTrace.OnMoveStateChanged);
                GameClient.BeTownPlayerMain.OnAutoMoveSuccess.AddListener(taskTrace.OnMoveSuccess);
                GameClient.BeTownPlayerMain.OnAutoMoveFail.AddListener(taskTrace.OnAutoMoveFail);

                if (clientSystem.MainPlayer != null)
                {
                    clientSystem.MainPlayer.CommandAutoMoveToSceneActor(guid,eSceneActorType);
                }
                else
                {
                    Logger.LogErrorFormat("The ClientSystem MainPlayer is not exist and guid is {0}, npcId is {1}", 
                        guid, npcId);
                }
            }
        }

        /// <summary>
        /// 通过npcId，寻路到npcId，并打开相应的界面
        /// 是对OnClickLink的一个简单封装
        /// </summary>
        /// <param name="npcId"></param>
        public static void OnClickLinkByNpcId(Int32 npcId)
        {
            OnClickLink(npcId,
                () =>
                {
                    TaskNpcAccess.OnClickFunctionNpc(npcId);
                });
        }

        public static void OnClickLink(Int32 iID, OnReached onReached = null)
        {
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iID);
            if (clientSystem != null && npcItem != null)
            {
                GameClient.TaskTrace.NpcTrace taskTrace = new GameClient.TaskTrace.NpcTrace();
                taskTrace.iNpcID = iID;
                taskTrace.iTaskID = 0;
                taskTrace.onReached = onReached;
                taskTrace.bNeedDialog = false;

                BeTownPlayerMain.CommandStopAutoMove();
                GameClient.BeTownPlayerMain.OnMoveStateChanged.AddListener(taskTrace.OnMoveStateChanged);
                GameClient.BeTownPlayerMain.OnAutoMoveSuccess.AddListener(taskTrace.OnMoveSuccess);
                GameClient.BeTownPlayerMain.OnAutoMoveFail.AddListener(taskTrace.OnAutoMoveFail);

                if (clientSystem.MainPlayer != null)
                {
                    clientSystem.MainPlayer.CommandAutoMoveToSceneActor(iID);
                }
                else
                {
                    Logger.LogErrorFormat("MainPlayer is not exist,where do you want to go ? iNpcID = {0}", iID);
                }
            }
        }

        public static void OnClickLink(Int32 iID, Int32 iTaskID,bool bNeedDialog = true,
                        UnityEngine.Events.UnityAction onSuccessed = null,
            UnityEngine.Events.UnityAction onFailed = null)
        {
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iID);
            if (clientSystem != null && npcItem != null)
            {
                GameClient.TaskTrace.NpcTrace taskTrace = new GameClient.TaskTrace.NpcTrace();
                if(taskTrace == null) // 申请内存失败了
                {
                    Logger.LogErrorFormat("Parser.NpcParser.OnClickLink: alloc memory failed!!! iNpcID = {0} iTaskID = {1}", iID, iTaskID);
                    return;
                }
                taskTrace.iNpcID = iID;
                taskTrace.iTaskID = iTaskID;
                taskTrace.bNeedDialog = bNeedDialog;
                taskTrace.onSucceed = onSuccessed;
                taskTrace.onFailed = onFailed;

                BeTownPlayerMain.CommandStopAutoMove();
                GameClient.BeTownPlayerMain.OnMoveStateChanged.AddListener(taskTrace.OnMoveStateChanged);
                GameClient.BeTownPlayerMain.OnAutoMoveSuccess.AddListener(taskTrace.OnMoveSuccess);
                GameClient.BeTownPlayerMain.OnAutoMoveFail.AddListener(taskTrace.OnAutoMoveFail);

                if(clientSystem.MainPlayer != null)
                {
                    clientSystem.MainPlayer.CommandAutoMoveToSceneActor(iID);
                    return;
                }
                else
                {
                    Logger.LogErrorFormat("MainPlayer is not exist,where do you want to go ? iNpcID = {0} iTaskID = {1}", iID, iTaskID);
                }
            }

            if(onFailed != null)
            {
                onFailed.Invoke();
            }
        }
    }

    public class Common
    {
        public static void NameParse(ulong guid,byte job, string name)
        {
            //自身
            if (guid == PlayerBaseData.GetInstance().RoleID)
                return;

            var chatData = ChatUtility.GetChatDataBySendGuid(guid);
            var isChatDataFrameDifferentServer = ChatUtility.IsChatDataFromDifferentServer(chatData);
            //跨服聊天（团本相关)
            if (isChatDataFrameDifferentServer == true)
            {
                OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(guid,
                    (uint) QueryPlayerType.QPT_TEAM_COPY,
                    (uint) chatData.zoneId);
            }
            else
            {
                OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(guid);
            }
        }

        public static void RetinueParse(ulong guid,int iTableID)
        {
            // 随从系统早已废弃不用了
//             var myownRetinueData = RetinueManager.GetInstance().GetRetinueDataByUID(guid);
//             if(myownRetinueData == null)
//             {
//                 WorldChatLinkDataReq kSend = new WorldChatLinkDataReq();
//                 kSend.type = (byte)'R';
//                 kSend.uid = guid;
//                 NetManager.Instance().SendCommand<WorldChatLinkDataReq>(ServerType.GATE_SERVER, kSend);
//             }
//             else
//             {
//                 RetinueLinkFrame.Open(myownRetinueData);
//             }
        }
    }
}