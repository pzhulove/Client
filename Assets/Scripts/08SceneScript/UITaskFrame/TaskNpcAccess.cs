using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    public class TaskNpcAccess
    {
        /// <summary>
        /// 含有兑换商店NPCID
        /// </summary>
        static int[] iExchangeMallNPCID = new int[] { 2019, 2029, 2073, 2023 };

        public static void RemoveMissionListener(Int32 iNpcID,Int32 iMissionID)
        {
            /*
            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown)
            {
                ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                var townNpc = current.GetTownNpcByNpcId(iNpcID);
                if (townNpc != null && townNpc.GraphicActor != null)
                {
                    GameObject goRoot = townNpc.GraphicActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                    if (goRoot != null)
                    {
                        var head = goRoot.transform.Find("PlayerInfo_Head");
                        if (head != null)
                        {
                            NpcInteraction npcInteraction = head.gameObject.GetComponent<NpcInteraction>();
                            //npcInteraction.RemoveMissionListener(iMissionID);
                        }

                    }

                    return;
                }
            }
            */
        }

        public static void AddMissionListener(UInt32 iTaskID)
        {
            MissionManager.SingleMissionInfo retValue = null;
            if (!MissionManager.GetInstance().taskGroup.TryGetValue(iTaskID, out retValue))
            {
                return;
            }

            ProtoTable.MissionTable missionInfo = TableManager.instance.GetTableItem<ProtoTable.MissionTable>((int)retValue.taskID);
            if (missionInfo == null)
            {
                return;
            }

            Int32 iNpcID = 0;
            if (retValue.status == (int)Protocol.TaskStatus.TASK_INIT)
            {
                if (missionInfo.AcceptType == ProtoTable.MissionTable.eAcceptType.ACT_NPC)
                {
                    iNpcID = missionInfo.MissionTakeNpc;
                }
            }
            else if (retValue.status == (int)Protocol.TaskStatus.TASK_FINISHED)
            {
                if (missionInfo.FinishType == ProtoTable.MissionTable.eFinishType.FINISH_TYPE_NPC)
                {
                    iNpcID = missionInfo.MissionFinishNpc;
                }
            }

            ProtoTable.NpcTable npcItem = TableManager.instance.GetTableItem<ProtoTable.NpcTable>(iNpcID);
            if (npcItem == null)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown)
            {
                ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                current.AddMissionListenerForNpc(iNpcID, (int)iTaskID);
            }
        }

        public static void AddDialogListener(Int32 iNpcID)
        {
            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown)
            {
                ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                current.AddDialogListener(iNpcID);
            }
        }

        public static void OnClickFunctionNpc(Int32 iNpcID, UInt64 guid = 0, string strParam = "")
        {
            ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
            if(npcItem == null || npcItem.Function == ProtoTable.NpcTable.eFunction.none)
            {
                return;
            }

            if (npcItem.OpenLevel > PlayerBaseData.GetInstance().Level)
            {
                var dlgItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(npcItem.FunctionIntParam2);
                if(dlgItem != null)
                {
                    MissionManager.GetInstance().CloseAllDialog();
                    MissionManager.GetInstance().CreateDialogFrame(npcItem.FunctionIntParam2, 0, null);
                }
                return;
            }

            if (npcItem.Function == ProtoTable.NpcTable.eFunction.production)
            {
                DoErrorHint(iNpcID);
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.shopping)
            {
                if (npcItem.FunctionIntParam.Count == 1)
                {
                    ClientSystemManager.GetInstance().CloseFrame<ShopNewFrame>();
                    if (string.IsNullOrEmpty(strParam))
                    {
                        bool isFind = false;
                        for (int i = 0; i < iExchangeMallNPCID.Length; i++)
                        {
                            if (iExchangeMallNPCID[i] != npcItem.ID)
                            {
                                continue;
                            }
                            isFind = true;
                        }

                        if (isFind)
                        {
                            ShopNewDataManager.GetInstance()
                                .OpenShopNewFrame(24, npcItem.FunctionIntParam[0], 0, iNpcID);
                        }
                        else
                        {
                            ShopNewDataManager.GetInstance()
                                .OpenShopNewFrame(npcItem.FunctionIntParam[0], 0, 0, iNpcID);
                        }
                    }
                    else
                    {
                        var tokens = strParam.Split('|');
                        if (tokens.Length == 3)
                        {
                            int iShopID = npcItem.FunctionIntParam[0];
                            int iShopLinkID = int.Parse(tokens[1]);
                            int iShopTabID = int.Parse(tokens[2]);
                            ShopNewDataManager.GetInstance().OpenShopNewFrame(iShopID, 0, iShopTabID, iNpcID);
                        }
                        else
                        {
                            ShopNewDataManager.GetInstance()
                                .OpenShopNewFrame(npcItem.FunctionIntParam[0], 0, 0, iNpcID);
                        }
                    }
                }
                else
                {
                    Logger.LogWarningFormat("shop npc:{0} function param count error!!", iNpcID);
                }
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.strengthen)
            {
                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>();
                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle);
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.enchanting)
            {
                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>();
                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle);
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.store)
            {
                ClientSystemManager.GetInstance().CloseFrame<StorageGroupFrame>();
                ItemGroupData data = new ItemGroupData();
                data.isPackage = false;
                data.ePackageType = EPackageType.Equip;
                ClientSystemManager.GetInstance().OpenFrame<StorageGroupFrame>(FrameLayer.Middle, data);
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.mail)
            {
                ClientSystemManager.GetInstance().CloseFrame<MailNewFrame>();
                ClientSystemManager.GetInstance().OpenFrame<MailNewFrame>(FrameLayer.Middle);
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.Townstatue)
            {
                ClientSystemManager.GetInstance().CloseFrame<TownStatueTalkFrame>();
                ClientSystemManager.GetInstance().OpenFrame<TownStatueTalkFrame>(FrameLayer.Middle, (byte)npcItem.SubType);
            }
            else if(npcItem.Function == NpcTable.eFunction.guildGuardStatue)
            {
                ClientSystemManager.GetInstance().CloseFrame<GuildGuardStatueTalkFrame>();
                ClientSystemManager.GetInstance().OpenFrame<GuildGuardStatueTalkFrame>(FrameLayer.Middle, (byte)npcItem.SubType);
            }
            else if(npcItem.Function == ProtoTable.NpcTable.eFunction.guildDungeonActivityChest)
            {
                GuildDataManager.GetInstance().TryGetGuildDungeonActivityChestAward();
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.TAPGraduation)
            {
                var pupilDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                var teacherDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
                if(pupilDatas.Count + teacherDatas.Count > 0)
                {
                    ClientSystemManager.GetInstance().OpenFrame<TAPSubmitGraduationFrame>();
                }
                else
                {
                    openNormalTalk(npcItem);
                    
                }
            }
			else if (npcItem.Function == ProtoTable.NpcTable.eFunction.RandomTreasure)
            {
                ClientSystemManager.GetInstance().CloseFrame<RandomTreasureFrame>();
                ClientSystemManager.GetInstance().OpenFrame<RandomTreasureFrame>(FrameLayer.Middle);
            }
            else if (npcItem.Function == NpcTable.eFunction.BlackMarketMerchan)
            {
                if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.BlackMarket))
                {
                    ClientSystemManager.GetInstance().OpenFrame<BlackMarketMerchantTalkFrame>();
                    return;
                }

                if (ClientSystemManager.GetInstance().IsFrameOpen<BlackMarketMerchantFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<BlackMarketMerchantFrame>();
                }

                ClientSystemManager.GetInstance().OpenFrame<BlackMarketMerchantFrame>(FrameLayer.Middle);
            }
            else if(npcItem.Function == NpcTable.eFunction.Chiji)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<ChijiNpcDialogFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<ChijiNpcDialogFrame>();
                }

                ChijiNpcData npcdata = new ChijiNpcData();

                npcdata.npcTableId = iNpcID;
                npcdata.guid = guid;

                ClientSystemManager.GetInstance().OpenFrame<ChijiNpcDialogFrame>(FrameLayer.Middle, npcdata);
            }else if(npcItem.Function==NpcTable.eFunction.AnniersaryParty)
            {

                if (PlayerBaseData.GetInstance().Level>=20)
                {
                    ChapterSelectFrame.SetSceneID(6038);
                    ClientSystemManager.GetInstance().OpenFrame<ChapterSelectFrame>();
                }
                else
                {
                    if(!ClientSystemManager.GetInstance().IsFrameOpen<AnniversaryPartyTalkFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<AnniversaryPartyTalkFrame>(FrameLayer.Middle, iNpcID);
                    }
                }
            }
            else
            {
                DoErrorHint(iNpcID);
            }
        }

        //走npc聊天界面（需要先配置聊天相关的表格）
        private static void openNormalTalk(NpcTable npcItem)
        {
            if (npcItem.DialogShowType == NpcTable.eDialogShowType.Direct && npcItem.NpcTalk.Length > 0)
            {
                Int32 dialogID = Int32.Parse(npcItem.NpcTalk[0]);
                ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(dialogID);
                if (talkItem != null)
                {
                    var dialogCallback = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                    {
                        ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                        if (current != null)
                        {
                            current.PlayNpcSound(npcItem.ID, NpcVoiceComponent.SoundEffectType.SET_End);
                        }
                    });
                    GameClient.MissionManager.GetInstance().CreateDialogFrame(dialogID, 0, dialogCallback);
                }
            }
            else if (npcItem.DialogShowType == NpcTable.eDialogShowType.SecondaryInterface)
            {

            }
        }

        public static void OnClickNpc(BeTownNPCData townData)
        {
            if(townData == null)
            {
                return;
            }

            if (!CanClick())
            {
                return;
            }

            //Logger.LogErrorFormat("[npc]OnClickNpc NPC id = {0}", townData.NpcID);

            ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(townData.NpcID);
            if(npcItem == null)
            {
                return;
            }

            if (npcItem.Function == ProtoTable.NpcTable.eFunction.store)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<StorageGroupFrame>() == false)
                {
                    ItemGroupData data = new ItemGroupData();
                    data.isPackage = false;
                    data.ePackageType = EPackageType.Equip;
                    ClientSystemManager.GetInstance().OpenFrame<StorageGroupFrame>(FrameLayer.Middle, data);
                }
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.mail)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<MailNewFrame>() == false)
                {
                    ClientSystemManager.GetInstance().OpenFrame<MailNewFrame>(FrameLayer.Middle);
                }
            }
            else if (npcItem.Function == ProtoTable.NpcTable.eFunction.guildDungeonActivityChest)
            {
                GuildDataManager.GetInstance().TryGetGuildDungeonActivityChestAward();
            }
        }

        public static void DoErrorHint(Int32 iNpcID)
        {
            ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
            if (npcItem != null)
            {
                Logger.LogErrorFormat("[npc]请做 [{0} 系统] 的程序员在此加入功能接口! npcname = [{1}] id = {2}", Utility.GetNpcFunctionName(iNpcID), npcItem.NpcName, npcItem.ID);
            }
        }

        static float ms_fLastClickTime = 0.0f;
        static float ms_clickInterval = 0.50f;
        static bool CanClick()
        {
            if(ms_fLastClickTime + ms_clickInterval < Time.time)
            {
                ms_fLastClickTime = Time.time;
                return true;
            }
            return false;
        }

        public static void OnClickFightPlayer(BeFighterData beFighterData, CitySceneTable.eSceneType sceneType, Transform transform)
        {
            if (!CanClick())
            {
                return;
            }

            if (beFighterData == null)
            {
                return;
            }

            // 点击自己不处理
            if (beFighterData.GUID == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            if(sceneType == CitySceneTable.eSceneType.BATTLE)
            {
                if(!ChijiDataManager.GetInstance().IsReadyPk)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("你未开启挑战,请点击挑战按钮");
                    return;
                }

                if (ChijiDataManager.GetInstance().CurBattleStage < ChiJiTimeTable.eBattleStage.BS_START_PK)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("PK尚未开始,无法开启挑战");
                    return;
                }

                // 判断挑战范围
                ClientSystemGameBattle currentBattle = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                if(currentBattle != null)
                {
                    Vector3 kDistance = Vector3.zero;

                    kDistance = beFighterData.MoveData.Position - currentBattle.MainPlayer.ActorData.MoveData.Position;

                    kDistance.y = 0.0f;
                    var fDistance = Mathf.Sqrt(kDistance.sqrMagnitude);

                    if (fDistance > 5.4f)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation("超出攻击范围");
                        return;
                    }
                }

                // 死亡或掉线，尸体还存在的玩家无法挑战                   
                var otherDeadPlayers = ChijiDataManager.GetInstance().OtherDeadPlayers;
                for (int i = 0; i < otherDeadPlayers.Count; i++)
                {
                    if (otherDeadPlayers[i] == beFighterData.GUID)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation("玩家已被淘汰，不可挑战");
                        break;
                    }
                }

                if (currentBattle != null && currentBattle.MainPlayer != null)
                {
                    ChijiDataManager.GetInstance().SendBattlePkSomeOneReq(beFighterData.GUID, currentBattle.MainPlayer.GetPKDungeonID());
                }
                // 吃鸡菜单使用新界面
                //ClientSystemManager.GetInstance().OpenFrame<ChijiActorShowMenuFrame>(FrameLayer.Middle, data);
            }
            else
            {
                if(ChijiDataManager.GetInstance().IsMatching)
                {
                    SystemNotifyManager.SystemNotify(4200006);
                    return;
                }

                _AddChijiFunctionMenu(beFighterData, transform, null);
            }
        }

        public static void OnClickTownPlayer(BeTownPlayerData beTownPlayerData, Transform transform)
        {
            if(!CanClick())
            {
                return;
            }

            if (beTownPlayerData == null)
            {
                return;
            }

            // 点击自己不处理
            if (beTownPlayerData.GUID == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                _AddNormalFunctionMenu(beTownPlayerData, transform, systemTown);
            }
        }

        private static void _AddNormalFunctionMenu(BeTownPlayerData beTownPlayerData, Transform transform, ClientSystemTown systemTown)
        {
            MenuData menuData = new MenuData();
            menuData.kWorldPos = transform.position;
            menuData.name = beTownPlayerData.Name;
            menuData.items = new List<MenuItem>();
            menuData.level = (int)beTownPlayerData.RoleLv;
            menuData.vip = beTownPlayerData.vip;
            menuData.guildName = beTownPlayerData.GuildName;
            menuData.pkLevel = beTownPlayerData.pkRank;
            menuData.jobID = beTownPlayerData.JobID;
            menuData.ZoneID = beTownPlayerData.ZoneID;
            menuData.adventureTeamName = beTownPlayerData.AdventureTeamName;
            menuData.WearedTitleInfo = beTownPlayerData.WearedTitleInfo;
			menuData.guildLv = beTownPlayerData.GuildEmblemLv;
            menuData.GUID = beTownPlayerData.GUID;

            PlayerBaseData.GetInstance().CurrentMenuData = menuData;


            ulong guid = beTownPlayerData.GUID;
            var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(guid);
            bool isFirend = (relationData != null && relationData.IsFriend());

            menuData.items.Add(new MenuItem()
            {
                name = "查看信息",
                callback = () =>
                {
                    OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(guid);
                }
            });

            menuData.items.Add(new MenuItem()
            {
                name = "私密聊天",
                callback = () =>
                {
                    RelationData rd = relationData;
                    if (rd == null)
                    {
                        rd = new RelationData();
                        rd.level = (ushort)menuData.level;
                        rd.uid = guid;
                        rd.name = menuData.name;
                        rd.occu = (byte)menuData.jobID;
                        rd.vipLv = (byte)menuData.vip;
                    }
                    ChatManager.GetInstance().OpenPrivateChatFrame(rd);
                }
            });

            if (!isFirend)
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "添加好友",
                    callback = () =>
                    {
                        int friendLv = 0;
                        var FuncUnlockData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(31);
                        if (FuncUnlockData != null)
                        {
                            friendLv = FuncUnlockData.FinishLevel;
                        }

                        object[] args = new object[1];
                        args[0] = friendLv;
                        if (PlayerBaseData.GetInstance().Level < friendLv)
                        {
                            SystemNotifyManager.SystemNotify(1237, args);
                            return;
                        }
                        else if (menuData.level < friendLv)
                        {
                            SystemNotifyManager.SystemNotify(1236, args);
                            return;
                        }

                        RelationDataManager.GetInstance().AddFriendByID(guid);
                    }
                });
            }
            else
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "删除好友",
                    callback = () =>
                    {
                        RelationDataManager.GetInstance().DelFriend(guid);
                    }
                });
            }

            menuData.items.Add(new MenuItem()
            {
                name = "邀请组队",
                callback = () =>
                {
                    TeamDataManager.GetInstance().TeamInviteOtherPlayer(guid);
                }
            });

            menuData.items.Add(new MenuItem()
            {
                name = "申请入队",
                callback = () =>
                {
                    TeamDataManager.GetInstance().JoinOtherPlayerTeam(guid);
                }
            });

            if (!menuData.HasGuild() && GuildDataManager.GetInstance().myGuild != null)
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "邀请入会",
                    callback = () =>
                    {
                        GuildDataManager.GetInstance().InviteJoinGuild(guid);
                    }
                });
            }

            if (true)
            {
                RelationData rd = relationData;
                if (rd == null)
                {
                    rd = new RelationData();
                    rd.level = (ushort)menuData.level;
                    rd.uid = guid;
                    rd.name = menuData.name;
                    rd.occu = (byte)menuData.jobID;
                    rd.vipLv = (byte)menuData.vip;
                }

                if (RelationMenuFram._CheckGetPupil(rd))
                {
                    menuData.items.Add(new MenuItem
                    {
                        name = "收为弟子",
                        callback = () =>
                        {
                            RelationMenuFram._OnAskForPupil(rd);
                        },
                    });
                }

                if (RelationMenuFram._CheckGetTeacher(rd))
                {
                    menuData.items.Add(new MenuItem
                    {
                        name = "拜师",
                        callback = () =>
                        {
                            RelationMenuFram._OnAskForTeacher(rd);
                            TAPNewDataManager.GetInstance().AddQueryInfo(rd.uid);
                        },
                    });
                }

                if (rd.type == (byte)RelationType.RELATION_MASTER ||
                    rd.type == (byte)RelationType.RELATION_DISCIPLE)
                {
                    menuData.items.Add(new MenuItem
                    {
                        name = "解除师徒",
                        callback = () =>
                        {
                            RelationMenuFram._OnFireTeacher(rd);
                        },
                    });
                }
            }

            //if(isFirend)
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "加入黑名单",
                    callback = () =>
                    {
                        string msgCtx = String.Format("是否加入黑名单?");
                        SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx, () =>
                        {
                            RelationDataManager.GetInstance().AddBlackList(beTownPlayerData.GUID);
                        }, () => { return; });
                    }
                });
            }

            {
                menuData.items.Add(new MenuItem()
                {
                    name = "赠送物品",
                    callback = () =>
                    {
                        ClientSystemManager.GetInstance().OpenFrame<GiveGiftFrame>();
                    }
                });
            }


            bool bCanOpen = true;

            if (menuData.ZoneID != PlayerBaseData.GetInstance().ZoneID)
            {
                bCanOpen = false;
            }

            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS && scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossGuildBattle)
                    {
                        bCanOpen = false;
                    }
                }
            }

            if (bCanOpen)
            {
                if (Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
                {
                    return;
                }

                if (Pk2v2CrossDataManager.GetInstance().CheckPk2v2CrossScence())
                {
                    return;
                }
                if (Pk3v3DataManager.HasInPk3v3Room())
                {
                    return;
                }

                ActorShowMenu.Open(menuData);
            }
        }

        private static void _AddChijiFunctionMenu(BeFighterData beTownPlayerData, Transform transform, ClientSystemTown systemTown)
        {
            MenuData menuData = new MenuData();
            menuData.kWorldPos = transform.position;
            menuData.name = beTownPlayerData.Name;
            menuData.items = new List<MenuItem>();
            menuData.level = (int)beTownPlayerData.RoleLv;
            menuData.vip = beTownPlayerData.vip;
            menuData.guildName = beTownPlayerData.GuildName;
            menuData.pkLevel = beTownPlayerData.pkRank;
            menuData.jobID = beTownPlayerData.JobID;
            menuData.ZoneID = beTownPlayerData.ZoneID;
            menuData.adventureTeamName = beTownPlayerData.AdventureTeamName;
            menuData.WearedTitleInfo = beTownPlayerData.WearedTitleInfo;
            menuData.guildLv = beTownPlayerData.GuildEmblemLv;


            ulong guid = beTownPlayerData.GUID;
            var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(guid);
            bool isFirend = (relationData != null && relationData.IsFriend());

            menuData.items.Add(new MenuItem()
            {
                name = "查看信息",
                callback = () =>
                {
                    OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(guid);
                }
            });

            menuData.items.Add(new MenuItem()
            {
                name = "私密聊天",
                callback = () =>
                {
                    RelationData rd = relationData;
                    if (rd == null)
                    {
                        rd = new RelationData();
                        rd.level = (ushort)menuData.level;
                        rd.uid = guid;
                        rd.name = menuData.name;
                        rd.occu = (byte)menuData.jobID;
                        rd.vipLv = (byte)menuData.vip;
                    }
                    ChatManager.GetInstance().OpenPrivateChatFrame(rd);
                }
            });

            if (!isFirend)
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "添加好友",
                    callback = () =>
                    {
                        int friendLv = 0;
                        var FuncUnlockData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(31);
                        if (FuncUnlockData != null)
                        {
                            friendLv = FuncUnlockData.FinishLevel;
                        }

                        object[] args = new object[1];
                        args[0] = friendLv;
                        if (PlayerBaseData.GetInstance().Level < friendLv)
                        {
                            SystemNotifyManager.SystemNotify(1237, args);
                            return;
                        }
                        else if (menuData.level < friendLv)
                        {
                            SystemNotifyManager.SystemNotify(1236, args);
                            return;
                        }

                        RelationDataManager.GetInstance().AddFriendByID(guid);
                    }
                });
            }
            else
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "删除好友",
                    callback = () =>
                    {
                        RelationDataManager.GetInstance().DelFriend(guid);
                    }
                });
            }

//             menuData.items.Add(new MenuItem()
//             {
//                 name = "邀请组队",
//                 callback = () =>
//                 {
//                     TeamDataManager.GetInstance().TeamInviteOtherPlayer(guid);
//                 }
//             });
// 
//             menuData.items.Add(new MenuItem()
//             {
//                 name = "申请入队",
//                 callback = () =>
//                 {
//                     TeamDataManager.GetInstance().JoinOtherPlayerTeam(guid);
//                 }
//             });

            if (!menuData.HasGuild() && GuildDataManager.GetInstance().myGuild != null)
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "邀请入会",
                    callback = () =>
                    {
                        GuildDataManager.GetInstance().InviteJoinGuild(guid);
                    }
                });
            }

            if (true)
            {
                RelationData rd = relationData;
                if (rd == null)
                {
                    rd = new RelationData();
                    rd.level = (ushort)menuData.level;
                    rd.uid = guid;
                    rd.name = menuData.name;
                    rd.occu = (byte)menuData.jobID;
                    rd.vipLv = (byte)menuData.vip;
                }

                if (RelationMenuFram._CheckGetPupil(rd))
                {
                    menuData.items.Add(new MenuItem
                    {
                        name = "收为弟子",
                        callback = () =>
                        {
                            RelationMenuFram._OnAskForPupil(rd);
                        },
                    });
                }

                if (RelationMenuFram._CheckGetTeacher(rd))
                {
                    menuData.items.Add(new MenuItem
                    {
                        name = "拜师",
                        callback = () =>
                        {
                            RelationMenuFram._OnAskForTeacher(rd);
                            TAPNewDataManager.GetInstance().AddQueryInfo(rd.uid);
                        },
                    });
                }

                if (rd.type == (byte)RelationType.RELATION_MASTER ||
                    rd.type == (byte)RelationType.RELATION_DISCIPLE)
                {
                    menuData.items.Add(new MenuItem
                    {
                        name = "解除师徒",
                        callback = () =>
                        {
                            RelationMenuFram._OnFireTeacher(rd);
                        },
                    });
                }
            }

            //if(isFirend)
            {
                menuData.items.Add(new MenuItem()
                {
                    name = "加入黑名单",
                    callback = () =>
                    {
                        string msgCtx = String.Format("是否加入黑名单?");
                        SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx, () =>
                        {
                            RelationDataManager.GetInstance().AddBlackList(beTownPlayerData.GUID);
                        }, () => { return; });
                    }
                });
            }

            bool bCanOpen = true;

            if (menuData.ZoneID != PlayerBaseData.GetInstance().ZoneID)
            {
                bCanOpen = false;
            }

            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS && scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossGuildBattle)
                    {
                        bCanOpen = false;
                    }
                }
            }

            if (bCanOpen)
            {
                if (Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
                {
                    return;
                }

                if (Pk2v2CrossDataManager.GetInstance().CheckPk2v2CrossScence())
                {
                    return;
                }
                if (Pk3v3DataManager.HasInPk3v3Room())
                {
                    return;
                }

                ActorShowMenu.Open(menuData);
            }
        }

        public static void OnClickBlank()
        {
            if (!CanClick())
            {
                return;
            }
            ActorShowMenu.CloseMenu();
        }

        public static Vector3 WordToScenePoint(Vector3 wordPosition)
        {
            CanvasScaler canvasScaler = GameObject.Find("UIRoot").transform.Find("UI2DRoot").GetComponent<CanvasScaler>();

            float resolutionX = canvasScaler.referenceResolution.x;

            float resolutionY = canvasScaler.referenceResolution.y;

            float offect = (Screen.width / canvasScaler.referenceResolution.x) * (1 - canvasScaler.matchWidthOrHeight) + (Screen.height / canvasScaler.referenceResolution.y) * canvasScaler.matchWidthOrHeight;

            Vector2 a = RectTransformUtility.WorldToScreenPoint(Camera.main, wordPosition);
            return new Vector3(a.x / offect, a.y / offect, 0.0f);
        }
    }
}