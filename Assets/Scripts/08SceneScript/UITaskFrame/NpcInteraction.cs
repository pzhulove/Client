using UnityEngine;
using System;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine.UI;

namespace GameClient
{
    public enum NpcInteractionType
    {
        NIT_DIALOG = 0,
        NIT_FUNCTION,
        NIT_MISSION,
        NIT_Attack_City_Monster,
    }

    public class NpcInteractionData
    {
        public delegate void OnClickFunction();
        public string name;
        public string icon;
        public OnClickFunction onClickFunction;
        public NpcInteractionType eNpcInteractionType = NpcInteractionType.NIT_DIALOG;
        public int iParam0;
    }

    public class NpcInteraction : MonoBehaviour
    {
        public static float fOutMaxDistance = 2.00f;
        static float fInMaxDistance = 1.90f;
        private Int32 _iNpcId;
        private UInt64 _npcGuid;           //怪物NPC的guid是由服务器决定，不为0;
        private NpcTable _npcItem;
        List<NpcInteractionData> datas = new List<NpcInteractionData>();
        List<NpcInteractionData> dialogSecondDatas = new List<NpcInteractionData>();
        //Image imgIcon;
        LayoutElement layoutElementTrace;
        GameObject goTraceEff;
        Button btnMissionTrace;
        UIGray gray;
        bool bInitialized = false;
        bool bCreated = false;
        GameObject gExchangeShop;
        Image iExchangeShopImage;

        void OnDisable()
        {
            Clear();
        }

        // Use this for initialization
        public void Initialize(Int32 iNpcId,UInt64 guid = 0)
        {
            Clear();
            this.bInitialized = true;
            this._iNpcId = iNpcId;
            _npcGuid = guid;
            _npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(iNpcId);
            if(!bCreated)
            {
                layoutElementTrace = Utility.FindComponent<LayoutElement>(gameObject, "MissionTrace");
                btnMissionTrace = Utility.FindComponent<Button>(gameObject, "MissionTrace");
                gray = btnMissionTrace.gameObject.GetComponent<UIGray>();
                if (gray == null)
                {
                    gray = btnMissionTrace.gameObject.AddComponent<UIGray>();
                }
                bCreated = true;
            }

            btnMissionTrace.CustomActive(true);
            btnMissionTrace.onClick.RemoveAllListeners();
            gray.enabled = false;

            _TryAddFunctionListener();
            _TryAddDialogListener();
            _TryAddMissionListener();
            _TryExchangeShopIsShow();
            TryAddAttackCityMonsterListener();
            _TryAddTAPListener();
            _TryAddBlackMarketMerchanListener();
            _TryAddChijiShopListener();
            _TryAddAnniversaryParyListener();

            NpcRelationMissionManager.GetInstance().onNpcRelationMissionChanged += OnNpcRelationMissionChanged;
        }

        /// <summary>
        /// 兑换商店是否显示
        /// </summary>
        void _TryExchangeShopIsShow()
        {
            ComCommonBind mBind = gameObject.GetComponent<ComCommonBind>();
            if (null == mBind)
            {
                return;
            }

            gExchangeShop = mBind.GetGameObject("ExchangeShop");

            gExchangeShop.CustomActive(false);

            ComCommonBind eShopRootBind = gExchangeShop.GetComponent<ComCommonBind>();
            if (eShopRootBind == null)
            {
                return;
            }
            iExchangeShopImage = eShopRootBind.GetCom<Image>("Image");

            if (_npcItem != null && _npcItem.ExchangeShopData != "")
            {
                string[] exchangeShopDatas = _npcItem.ExchangeShopData.Split('|');
                bool isFlag = int.Parse(exchangeShopDatas[0]) == 1;
                string iconPath = exchangeShopDatas[1];
                ETCImageLoader.LoadSprite(ref iExchangeShopImage, iconPath);
                gExchangeShop.CustomActive(btnMissionTrace.enabled != true && isFlag);
            }
        }

        void OnNpcRelationMissionChanged(int iNpcId)
        {
            if (this._iNpcId == iNpcId)
            {
                _TryAddMissionListener();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NpcRelationMissionChanged, this._iNpcId);
            }
        }

        void _TryAddFunctionListener()
        {
            if (_npcItem != null && 
                ((_npcItem.Function > ProtoTable.NpcTable.eFunction.none && _npcItem.Function < ProtoTable.NpcTable.eFunction.clicknpc) || _npcItem.Function == NpcTable.eFunction.guildDungeonActivityChest || _npcItem.Function == NpcTable.eFunction.guildGuardStatue))
            {
                NpcInteractionData data = new NpcInteractionData();
                data.icon = _npcItem.FunctionIcon;
                data.eNpcInteractionType = NpcInteractionType.NIT_FUNCTION;
                data.iParam0 = _npcItem.ID;
                data.name = _npcItem.NpcName;
                data.onClickFunction = () =>
                {
                    GameClient.TaskNpcAccess.OnClickFunctionNpc(data.iParam0);
                };
                datas.Add(data);
            }
        }

        private void _TryAddTAPListener()
        {
            if (_npcItem == null)
                return;
            
            if (_npcItem.Function != NpcTable.eFunction.TAPGraduation)
            {
                return;
            }
            var data = new NpcInteractionData
            {
                icon = _npcItem.FunctionIcon,
                eNpcInteractionType = NpcInteractionType.NIT_FUNCTION,
                iParam0 = _npcItem.ID,
                name = TR.Value("npc_interaction_master"),
                onClickFunction = () =>
                {
                    GameClient.TaskNpcAccess.OnClickFunctionNpc(_npcItem.ID);
                    //RelationData selectRelationData = TAPNewDataManager.GetInstance()._GetCurSelectPupil();
                    //if (selectRelationData != null)
                    //{
                    //    ClientSystemManager.GetInstance().OpenFrame<TAPSubmitGraduationFrame>(FrameLayer.Middle, selectRelationData);
                    //}
                },
            };
            datas.Add(data);
        }

        /// <summary>
        /// 添加黑市商人监听
        /// </summary>
        private void _TryAddBlackMarketMerchanListener()
        {
            if (_npcItem == null)
                return;

            //类型不等于黑市商人 retun
            if (_npcItem.Function != NpcTable.eFunction.BlackMarketMerchan)
            {
                return;
            }

            var data = new NpcInteractionData
            {
                icon = _npcItem.FunctionIcon,
                eNpcInteractionType = NpcInteractionType.NIT_FUNCTION,
                iParam0 = _npcItem.ID,
                name = TR.Value("npc_interaction_shop"),
                onClickFunction = () =>
                {
                    GameClient.TaskNpcAccess.OnClickFunctionNpc(_npcItem.ID);
                },
            };
            datas.Add(data);
        }

        private void _TryAddChijiShopListener()
        {
            if (_npcItem == null)
            {
                return;
            }

            if (_npcItem.Function != NpcTable.eFunction.Chiji)
            {
                return;
            }

            //             ClientSystemGameBattle Chiji = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            //             if (Chiji == null)
            //             {
            //                 return;
            //             }

            //             CitySceneTable tableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(Chiji.CurrentSceneID);
            //             if(tableData == null)
            //             {
            //                 return;
            //             }

            //             if(tableData.SceneSubType != CitySceneTable.eSceneSubType.Battle)
            //             {
            //                 return;
            //             }

            var data = new NpcInteractionData
            {
                icon = _npcItem.FunctionIcon,
                eNpcInteractionType = NpcInteractionType.NIT_FUNCTION,
                iParam0 = _npcItem.ID,
                name = TR.Value("npc_interaction_shop"),
                onClickFunction = () =>
                {
                    TaskNpcAccess.OnClickFunctionNpc(_npcItem.ID, _npcGuid);
                },
            };
            datas.Add(data);
        }

        private void TryAddAttackCityMonsterListener()
        {
            if(_npcItem == null)
                return;

            //NPC的GUID为0，则不是由服务器同步的攻城怪物，则不显示
            if (_npcGuid <= 0)
                return;
            
            //攻城怪物
            if (_npcItem.Function != NpcTable.eFunction.attackCityMonster)
                return;

            var data = new NpcInteractionData
            {
                icon = _npcItem.FunctionIcon,
                eNpcInteractionType = NpcInteractionType.NIT_Attack_City_Monster,
                iParam0 = _npcItem.ID,
                name = TR.Value("npc_interaction_attack_city"),
                onClickFunction = () =>
                {
                    AttackCityMonsterDataManager.GetInstance().SetOpenTalkFrameType(EOpenTalkFrameType.Normal);
                    AttackCityMonsterDataManager.GetInstance().ShowAttackCityMonsterDialogFrame(_npcGuid);
                },
            };
            datas.Add(data);
        }

        void _TryAddDialogListener()
        {
            if (_npcItem != null && _npcItem.Function == ProtoTable.NpcTable.eFunction.none)
            {
                if (_npcItem.DialogShowType == NpcTable.eDialogShowType.Direct)
                {
                    if (_npcItem.NpcTalk.Length > 0 && string.IsNullOrEmpty(_npcItem.NpcTalk[0]) == false)
                    {
                        Int32 dialogID = Int32.Parse(_npcItem.NpcTalk[0]);
                        ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(dialogID);
                        if (talkItem != null)
                        {
                            NpcInteractionData data = new NpcInteractionData();
                            data.icon = _npcItem.FunctionIcon;
                            data.eNpcInteractionType = NpcInteractionType.NIT_DIALOG;
                            data.iParam0 = dialogID;
                            data.name = TR.Value("npc_interaction_dialog");
                            data.onClickFunction = () =>
                            {
                                if (true)
                                {
                                    ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                                    if (current != null)
                                    {
                                        current.PlayNpcSound(_iNpcId, NpcVoiceComponent.SoundEffectType.SET_Start);
                                    }
                                }
                                var dialogCallback = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                                {
                                    ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                                    if (current != null)
                                    {
                                        current.PlayNpcSound(_iNpcId, NpcVoiceComponent.SoundEffectType.SET_End);
                                    }
                                });
                                GameClient.MissionManager.GetInstance().CreateDialogFrame(data.iParam0, 0, dialogCallback);
                            };
                            datas.Add(data);
                        }
                    }
                }
                else if (_npcItem.DialogShowType == NpcTable.eDialogShowType.SecondaryInterface)
                {
                    dialogSecondDatas.Clear();
                    for (int i = 0; i < _npcItem.NpcTalk.Length; i++)
                    {
                        if (string.IsNullOrEmpty(_npcItem.NpcTalk[i]) == false)
                        {
                            Int32 dialogID = Int32.Parse(_npcItem.NpcTalk[i]);
                            ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(dialogID);
                            if (talkItem != null)
                            {
                                NpcInteractionData dataSecond = new NpcInteractionData();
                                dataSecond.icon = _npcItem.FunctionIcon;
                                dataSecond.eNpcInteractionType = NpcInteractionType.NIT_DIALOG;
                                dataSecond.iParam0 = dialogID;
                                dataSecond.name = talkItem.TalkAbbreviation;
                                dataSecond.onClickFunction = () =>
                                {
                                    if (true)
                                    {
                                        ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                                        if (current != null)
                                        {
                                            current.PlayNpcSound(_iNpcId, NpcVoiceComponent.SoundEffectType.SET_Start);
                                        }
                                    }
                                    var dialogCallback = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                                    {
                                        ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                                        if (current != null)
                                        {
                                            current.PlayNpcSound(_iNpcId, NpcVoiceComponent.SoundEffectType.SET_End);
                                        }
                                    });
                                    GameClient.MissionManager.GetInstance().CreateDialogFrame(dataSecond.iParam0, 0, dialogCallback);
                                };
                                dialogSecondDatas.Add(dataSecond);
                            }
                        }
                    }

                    NpcInteractionData data = new NpcInteractionData();
                    data.icon = _npcItem.FunctionIcon;
                    data.eNpcInteractionType = NpcInteractionType.NIT_DIALOG;
                    //data.iParam0 = dialogID;
                    data.name = TR.Value("npc_interaction_dialog");
                    data.onClickFunction = () =>
                    {
                        if (true)
                        {
                            ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                            if (current != null)
                            {
                                current.PlayNpcSound(_iNpcId, NpcVoiceComponent.SoundEffectType.SET_Start);
                            }
                        }
                        var dialogCallback = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                        {
                            ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                            if (current != null)
                            {
                                current.PlayNpcSound(_iNpcId, NpcVoiceComponent.SoundEffectType.SET_End);
                            }
                        });
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OpenDialogSecondView, dialogSecondDatas);
                    };
                    datas.Add(data);

                    
                }
            }
        }
        /// <summary>
        /// 周年庆派对活动
        /// </summary>
        private void _TryAddAnniversaryParyListener()
        {
            if (_npcItem == null)
            {
                return;
            }

            if (_npcItem.Function != NpcTable.eFunction.AnniersaryParty)
            {
                return;
            }
           
                var data = new NpcInteractionData
                {
                    icon = _npcItem.FunctionIcon,
                    eNpcInteractionType = NpcInteractionType.NIT_FUNCTION,
                    iParam0 = _npcItem.ID,
                    name = TR.Value("npc_interaction_activity"),
                    onClickFunction = () =>
                    {
                        TaskNpcAccess.OnClickFunctionNpc(_npcItem.ID);
                    },
                };
                datas.Add(data);
            
        }
        string _GetMissionInteractionIcon(MissionManager.SingleMissionInfo value)
        {
            return "UI/Image/NewPacked/MainUIIcon.png:UI_Zhujiemian_Npc_Renwu";
        }

        void _TryAddMissionListener()
        {
            datas.RemoveAll(x =>
            {
                return x.eNpcInteractionType == NpcInteractionType.NIT_MISSION;
            });
            DeActiveMissionTrace();
            btnMissionTrace.onClick.RemoveAllListeners();

            var values = NpcRelationMissionManager.GetInstance().GetNpcRelationMissions(_iNpcId);
            if(values != null && values.Count > 0)
            {
                for (int i = 0; i < values.Count; ++i)
                {
                    NpcInteractionData data = new NpcInteractionData();
                    data.icon = _GetMissionInteractionIcon(values[i]);
                    data.eNpcInteractionType = NpcInteractionType.NIT_MISSION;
                    data.iParam0 = values[i].missionItem.ID;
                    data.name = TR.Value("npc_interaction_task");
                    data.onClickFunction = () =>
                    {
                        GameClient.MissionManager.GetInstance().AutoTraceTask(data.iParam0,null,null,true);
                    };
                    int iMissionId = values[i].missionItem.ID;
                    btnMissionTrace.onClick.AddListener(() =>
                    {
                        GameClient.MissionManager.GetInstance().AutoTraceTask(iMissionId,null, null, true);
                    });
                    datas.Add(data);
                    _UpdateMissionIcon((uint)values[i].missionItem.ID);
                    break;
                }
            }

            _TryExchangeShopIsShow();
        }

        void _UpdateMissionIcon(uint iTaskId)
        {
            int traceEffectId = 1036;//has accepted
            // gray.enabled = true;
            bool bGray = true;
            var missionValue = MissionManager.GetInstance().GetMission((uint)iTaskId);
            if(missionValue != null)
            {
                if (missionValue.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    traceEffectId = 1038;//has completed
                    // gray.enabled = false;
                    bGray = false;
                }
            
                if (missionValue.status == (int)Protocol.TaskStatus.TASK_INIT)
                {
                    traceEffectId = 1037;//has not accepted yet
                    // gray.enabled = false;
                    bGray = false;
                }
            }
            
            //ETCImageLoader.LoadSprite(ref imgIcon, path);
            ActiveMissionTrace(traceEffectId);
            gray.enabled = bGray;
        }

        private void ActiveMissionTrace(int effectId)
        {
            if (goTraceEff != null)
            {
                GameObject.Destroy(goTraceEff);
            }
            var tableData = TableManager.GetInstance().GetTableItem<EffectInfoTable>(effectId);
            goTraceEff = AssetLoader.instance.LoadResAsGameObject(tableData.Path);
            Utility.AttachTo(goTraceEff, btnMissionTrace.gameObject);

            goTraceEff.CustomActive(true);
            layoutElementTrace.enabled = true;
            btnMissionTrace.enabled = true;
        }

        private void DeActiveMissionTrace()
        {
            if (goTraceEff != null)
            {
                GameObject.Destroy(goTraceEff);
                goTraceEff = null;
            }
            layoutElementTrace.enabled = false;
            btnMissionTrace.enabled = false;
        }
        

        public void Tick()
        {
            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown ||
                ClientSystemManager.GetInstance().CurrentSystem is ClientSystemGameBattle)
            {
                //对话交互类型的NPC
                ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                ClientSystemGameBattle currentBattle = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                if (current == null && currentBattle == null)
                {
                    return;
                }

                // hack by wuduanduan
                // 解决宠物龙和边上NPC的交互框重叠的问题
                // FIXIT FIXIT FIXIT FIXIT
                if (2037 == _iNpcId)
                {
                    return ;
                }

                GameObject goActor = null;
                if(currentBattle != null)
                {
                    var npc = GetChijiNpc(currentBattle);
                    if (npc == null) return;
                    goActor = npc.GraphicActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
                }
                else
                {
                    //todo 
                    //根据_npcItem的类型决定获得Npc还是攻城怪物
                    var townNpc = GetBeTownNpc(current);
                    if (townNpc == null)
                        return;
                    goActor = townNpc.GraphicActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
                }

                if (null == _npcItem || _npcItem.Function == ProtoTable.NpcTable.eFunction.invalid ||
                    _npcItem.SubType == ProtoTable.NpcTable.eSubType.TownViceOwner_1 ||
                    _npcItem.SubType == ProtoTable.NpcTable.eSubType.TownViceOwner_2 ||
                    _npcItem.SubType == ProtoTable.NpcTable.eSubType.GuildGuard2 ||
                    _npcItem.SubType == ProtoTable.NpcTable.eSubType.GuildGuard3)
                {
                    return;
                }

                Vector3 kDistance = Vector3.zero;
                if (currentBattle != null)
                {
                    kDistance = goActor.transform.position - currentBattle.MainPlayer.ActorData.MoveData.Position;
                }
                else
                {
                    kDistance = goActor.transform.position - current.MainPlayer.ActorData.MoveData.Position;
                }

                kDistance.y = 0.0f;
                var fDistance = Mathf.Sqrt(kDistance.sqrMagnitude);

                if (fDistance > fInMaxDistance)
                {
                    NpcInterfaceFrame.TryCloseFrame(_iNpcId, _npcGuid);
                }
                else
                {
                    NpcInterfaceFrame.OpenFrame(_iNpcId, datas, _npcGuid);
                }
            }
        }

        private BeNPC GetChijiNpc(ClientSystemGameBattle systemTown)
        {
            if (_npcItem == null)
                return null;
            BeNPC curTownNpc = null;
            curTownNpc = systemTown.GetNpcByGuid(_iNpcId, _npcGuid);
            return curTownNpc;
        }
        /// <summary>
        /// 根据npcItem的类型获得BeTownNpc
        /// </summary>
        /// <param name="systemTown"></param>
        /// <returns></returns>
        private BeTownNPC GetBeTownNpc(ClientSystemTown systemTown)
        {
            if (_npcItem == null)
                return null;

            BeTownNPC curTownNpc = null;
            if (_npcGuid > 0 && _npcItem.Function == NpcTable.eFunction.attackCityMonster)
            {
                curTownNpc = systemTown.GetTownAttackCityMonsterByGuid(_npcGuid);
            }
            else if (_npcItem.Function == NpcTable.eFunction.BlackMarketMerchan)
            {
                curTownNpc = systemTown.GetBlackMarketMerchanNpcById(_iNpcId);
            }
            else
            {
                curTownNpc = systemTown.GetTownNpcByNpcId(_iNpcId);
            }

            return curTownNpc;
        }

        void OnDestroy()
        {
            Clear();
            //imgIcon = null;
            btnMissionTrace = null;
            gray = null;
            bCreated = false;
            bInitialized = false;
            gExchangeShop = null;
            iExchangeShopImage = null;
        }

        public void Clear()
        {

            NpcInterfaceFrame.TryCloseFrame(_iNpcId, _npcGuid);

            _iNpcId = 0;
            _npcGuid = 0;
            _npcItem = null;

            // if(null != imgIcon)
            // {
            //     imgIcon.CustomActive(false);
            // }

            if (null != goTraceEff)
            {
                GameObject.Destroy(goTraceEff);
                goTraceEff = null;
            }

            if(null != gray)
            {
                gray.enabled = false;
            }
            NpcRelationMissionManager.GetInstance().onNpcRelationMissionChanged -= OnNpcRelationMissionChanged;
            if (btnMissionTrace != null)
            {
                btnMissionTrace.onClick.RemoveAllListeners();
            }

            foreach (var data in datas)
            {
                data.onClickFunction = null;
            }
            datas.Clear();
        }
    }
}
