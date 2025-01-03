using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System;
using ProtoTable;

namespace GameClient
{
    public class BeTownPlayerData : BeBaseActorData
    {
        public bool bDirty { get; set; }

        string m_strName;
        public override string Name
        {
            get { return m_strName; }
            set { m_strName = value; bDirty = true; }
        }

        int m_nPKRank = 0;
        public int pkRank
        {
            get { return m_nPKRank; }
            set { m_nPKRank = value; bDirty = true; }
        }

        int m_nPKStar = 0;
        public int pkStar
        {
            get { return m_nPKStar; }
            set { m_nPKStar = value; bDirty = true; }
        }

        public int JobID { get; set; }

        public int State { get; set; }

        public ushort RoleLv { get; set; }
        public bool bRoleLvDirty { get; set; }
        public bool bAwakeDirty { get; set; }

        //public uint pkPoints { get; set; }

        public uint tittle { get; set; }

        public byte GuildPost { get; set; }
        public string GuildName { get; set; }

		public int vip;

        public PlayerAvatar avatorInfo = null;

        public int petID;

        public int ZoneID;
		public string AdventureTeamName { get; set; }
        public PlayerWearedTitleInfo WearedTitleInfo { get; set; }

        public int GuildEmblemLv { get; set; }
    }

    class BeTownPlayer : BeBaseActor
    {
        protected struct MoveCommand
        {
            public MoveCommand(Vector3 pos, Vector3 dir)
            {
                currPosition = pos;
                targetDirection = dir;
            }

            public Vector3 currPosition;
            public Vector3 targetDirection;
        }
        protected LinkedList<MoveCommand> moveCommands = new LinkedList<MoveCommand>();
        protected MoveCommand currMoveCommand;
        DelayCallUnitHandle m_delayCallStopMove;

        protected BeTownPet m_townPet = null;
        GameObject attachModel = null;                  //附加在玩家身上的模型
        protected  IBeTownActionPlay _beTownActionPlay;

        //public void CreateFollowPet()
        //{
        //    return;//
        //    DestroyFollowPet();

        //    BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;

        //    var petItem = TableManager.GetInstance().GetTableItem<ProtoTable.FollowPetTable>((int)townPlayerData.iFollowTableID);
        //    if (petItem != null)
        //    {
        //        BeFollowPetData data = new BeFollowPetData();
        //        data.playerData = townPlayerData;
        //        data.petItem = petItem;
        //        data.kPosition = ActorData.MoveData.Position;
        //        data.kTargetPosition = data.kPosition + new Vector3(ActorData.MoveData.FaceRight ? -1.0f : 1.0f, 0.0f, 0.0f);
        //        data.bFightOut = true;

        //        _systemTown.RemoveFollowPet(data);

        //        curFollowPet = new BeMainFollowPet(data, _systemTown, this);
        //        curFollowPet.InitGeActor(_geScene);
        //    }
        //}
        //void DestroyFollowPet()
        //{
        //    if (curFollowPet != null)
        //    {
        //        _systemTown.AddFollowPet(curFollowPet.ActorData as BeFollowPetData);

        //        curFollowPet.Dispose();
        //        curFollowPet = null;
        //    }
        //}

        public BeTownPlayer(BeTownPlayerData data, ClientSystemTown systemTown)
            : base(data, systemTown)
        {
            if (data.State == (int)Protocol.SceneObjectStatus.SOS_WALK)
            {
                AddMoveCommand(data.MoveData.Position, data.MoveData.TargetDirection, data.MoveData.FaceRight);
            }

            CreatePet(data.petID);
            CreateFollow();
            //ClientSystemTown.AddToAsyncLoadPetList(this, data.petID);
        }

        public override void Dispose()
        {
            DestroyPet();
            DestroyFollow();
            if(_beTownActionPlay!=null)
                _beTownActionPlay.DeInit();
            base.Dispose();
        }

        public override void InitGeActor(GeSceneEx geScene)
        {
            _geScene = geScene;
            if (geScene == null)
            {
                return;
            }

            if (_geActor == null)
            {
                BeTownPlayerData townData = ActorData as BeTownPlayerData;
                var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(townData.JobID);
                if (jobData == null)
                {
                    Logger.LogErrorFormat(" 职业表 没有ID为 {0} 的条目 playerInfo {1} {2} {3}", townData.JobID,townData.Name, townData.ZoneID, townData.GUID);
                    return;
                }

                _geActor = geScene.CreateActor(jobData.Mode,0,0,false,false,false);
                //_geActor.SetScale(Global.Settings.charScale);
                //_geActor.SetScale(Global.Settings.charScale * 1.15f);
                if (_geActor != null)
                {

                    PlayerInfoColor color = PlayerInfoColor.TOWN_OTHER_PLAYER;
                    if ((this as BeTownPlayerMain) != null)
                        color = PlayerInfoColor.TOWN_PLAYER;

                    if (
#if DEBUG_SETTING
                    Global.Settings.testFashionEquip ||
#endif
                    DebugSettings.instance.EnableTestFashionEquip)
                    {
#if DEBUG_REPORT_ROOT
					townData.tittle = (uint)(130193049 + UnityEngine.Random.Range(0,12));
					#endif
				}
                _geActor.AddTittleComponent((System.Int32)townData.tittle, townData.Name, townData.GuildPost, townData.GuildName, townData.RoleLv, townData.pkRank, color, townData.AdventureTeamName,townData.WearedTitleInfo,townData.GuildEmblemLv);

                _geActor.CreateInfoBar(townData.Name, townData.NameColor, townData.RoleLv);
                _geActor.AddSimpleShadow(Vector3.one,_geActor.GetResName());

                    GameObject objActor = _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                    if (objActor != null)
                    {
                        //objActor = objActor.transform.GetChild(0).gameObject;
                        var boxCollider = objActor.GetComponent<BoxCollider>();
                        if (boxCollider == null)
                        {
                            boxCollider = objActor.AddComponent<BoxCollider>();
                            boxCollider.center = new Vector3(0.0f, 0.850f, 0.0f);
                            boxCollider.size = new Vector3(0.60f, 1.80f, 0.86f);
                            boxCollider.enabled = true;
                        }
                        if (boxCollider != null)
                        {
                            RaycastObject comRaycastObject = objActor.GetComponent<RaycastObject>();
                            if (comRaycastObject == null)
                            {
                                comRaycastObject = objActor.AddComponent<RaycastObject>();
                            }
                            if (comRaycastObject != null)
                            {
                                comRaycastObject.Initialize(townData.JobID, RaycastObject.RaycastObjectType.ROT_TOWNPLAYER, townData);
                            }
                        }
                    }
                }
                else
                {
                    Logger.LogErrorFormat("TownPlayer createActor {0} failed", jobData.Mode);
                }
                ActorData.SetAniInfos(jobData.IdleAniName, jobData.WalkAniName, jobData.RunAniName, jobData.DeadAniName);
                //EquipWeapon();
				if (townData.avatorInfo != null)
				{
					if (Global.Settings.testFashionEquip || DebugSettings.instance.EnableTestFashionEquip)
					{
						#if DEBUG_REPORT_ROOT
						var testEquips = TestGetEquipFashions(townData.JobID);
						PlayerBaseData.GetInstance().AvatarEquipFromItems(null, /*townData.avatorInfo.equipItemIds*/testEquips, townData.JobID, (int)(townData.avatorInfo.weaponStrengthen), _geActor);
						#endif
				    }
				    else
				        PlayerBaseData.GetInstance().AvatarEquipFromItems(null,
				            townData.avatorInfo.equipItemIds,
				            townData.JobID,
				            (int) (townData.avatorInfo.weaponStrengthen),
				            _geActor,
				            false,
				            townData.avatorInfo.isShoWeapon);

				}

                /*
                if (m_townPet != null)
                {
                    m_townPet.InitGeActor(geScene);
                }*/
                CreateFollow();
                InitActionPlay(jobData.Mode);
                if (townData.petID > 0)
                {
                    CreatePet(townData.petID);
                    //ClientSystemTown.AddToAsyncLoadPetList(this, townData.petID);
                }
                else
                {
                    if (Global.Settings.testFashionEquip || DebugSettings.instance.EnableTestFashionEquip)
                    {

#if DEBUG_REPORT_ROOT
                        int randomPetID = UnityEngine.Random.Range(101, 113);
                        //ClientSystemTown.AddToAsyncLoadPetList(this, randomPetID);
                        CreatePet(randomPetID);

#endif
                    }
                }
            }

            base.InitGeActor(geScene);
        }

        /// <summary>
        /// 初始化城镇玩家技能配置文件
        /// </summary>
        protected void InitActionPlay(int resId)
        {
            if (resId == 0) return;
            var resData = TableManager.instance.GetTableItem<ProtoTable.ResTable>(resId);
            if (resData == null) return;
            string actionPath = resData.ActionConfigPath2;
            if (string.IsNullOrEmpty(actionPath) || actionPath.Equals("-")) return;
            _beTownActionPlay = new BeTownPlayerActionPlay();
            _beTownActionPlay.Init(_geActor, actionPath);
        }

        public void RemoveGeActor()
        {
            if (null != _geScene)
            {
                if (null != _geActor)
                {
                    _geScene.DestroyActor(_geActor);
                    _geActor = null;
                    DestroyFollow();
                    DestroyPet();
                }
            }
            else
            {
                if(null != _geActor)
                    Logger.LogError("Town player with error data!");
            }
        }

		static UInt32[] _fashionIDs = {
			530002001, 
			530003001,
			530003007,
			530003012,
			530003017,
			530003022,
			530003027,
			530003032,
		};

        static UInt32[] _wingIDs =
        {
            530005006,
            530005007,
            530005008,
            530005009,
            530005010,
            530005011,
            530005017,
            530005018,
        };

        private UInt32[] TestGetEquipFashions(int jobID)
		{
			int rand = UnityEngine.Random.Range(0,8);
			UInt32[] equips = new UInt32[6];
			for(int i=0; i<5; ++i)
				equips[i] = (UInt32)(_fashionIDs[rand]+(jobID-jobID%10)*100000 + i);

            equips[5] = (UInt32)(_wingIDs[rand] + (jobID - jobID % 10) * 100000);

            return equips;
		}

        public sealed override void UpdateGeActor(float deltaTime)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;

            if (_geActor != null)
            {
                if (townPlayerData.bRoleLvDirty)
                {
                    _geActor.UpdateInfoBarLevel(townPlayerData.RoleLv);
                    townPlayerData.bRoleLvDirty = false;
                }

                if (townPlayerData.bDirty)
                {
                    _geActor.UpdatePkRank(townPlayerData.pkRank, townPlayerData.pkStar);
                    _geActor.UpdateName(townPlayerData.Name);
                    townPlayerData.bDirty = false;
                }

                if(townPlayerData.bAwakeDirty)
                {
#if !LOGIC_SERVER
                    _geActor.PlayAwakeEffect();
#endif
					townPlayerData.bAwakeDirty = false;
                }

                if (_beTownActionPlay != null)
                {
                    _beTownActionPlay.Update(deltaTime);
                }

                base.UpdateGeActor(deltaTime);
            }
        }

        public override void UpdateMove(float timeElapsed)
        {
            _TryDoMoveCommand();

            // TODO !!这里的逻辑存疑
            //if (m_delayCallStopMove != null && m_delayCallStopMove.CanRemove() == true)
            //{
            //    m_delayCallStopMove = null;
            //}

            base.UpdateMove(timeElapsed);
        }

        public override void CommandStopMove()
        {
            base.CommandStopMove();
            if (currMoveCommand.targetDirection.x != 0.0f || currMoveCommand.targetDirection.z != 0.0f)
            {
                currMoveCommand.targetDirection = new Vector3(0.0f, 0.0f, 0.0f);

                DoActionWalk();

                if (!m_delayCallStopMove.IsValid())
                {
                    m_delayCallStopMove = delayCaller.DelayCall(2000, () =>
                    {
                        Logger.LogProcessFormat("town player auto stop move, because not receive stop cmd!");
                        CommandStopMove();
                    });
                }
            }

            //Logger.LogFormat("targetDirection:({0},{1})", currMoveCommand.targetDirection.x, currMoveCommand.targetDirection.z);
        }

        public sealed override void DoActionIdle()
        {
            if (_beTownActionPlay != null)
            {
                _beTownActionPlay.PlayAction("Idle");
            }
            else
            {
                base.DoActionIdle();
            }
            

            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            if (IsFemaleGunner(townPlayerData.JobID))
            {
                ActorData.SetAttachmentVisible(Global.WEAPON_ATTACH_NAME, true);
            }
            ActorData.PlayAttachmentAction(Global.WING_ATTACH_NAME, "Anim_Idle01");
        }

        public sealed override void DoActionWalk()
        {
            if (_beTownActionPlay != null)
            {
                string actionName = Global.Settings.townPlayerRun ?"Run" : "Walk";
                _beTownActionPlay.PlayAction(actionName);
            }
            else
            {
                base.DoActionWalk();
            }

            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            if (IsFemaleGunner(townPlayerData.JobID))
            {
                // ActorData.SetAttachmentVisible(Global.WEAPON_ATTACH_NAME, false);
                ActorData.SetAttachmentVisible(Global.WEAPON_ATTACH_NAME, true);
            }
            ActorData.PlayAttachmentAction(Global.WING_ATTACH_NAME, "Anim_Walk");
        }

        public void AddMoveCommand(Vector3 pos, Vector3 dir, bool faceRight)
        {
            delayCaller.StopItem(m_delayCallStopMove);
            
            //Logger.LogFormat("rece move dir:({0},{1}) pos:({2},{3})", dir.x, dir.z, pos.x, pos.z);

            // 缓存移动命令
            if (moveCommands.Count > 0)
            {
                MoveCommand moveCommand = moveCommands.Last.Value;
                if (moveCommand.currPosition == pos)
                {
                    moveCommands.RemoveLast();
                }
            }

            moveCommands.AddLast(new MoveCommand(pos, dir));

            if (moveCommands.Count > 10)
            {
                moveCommands.RemoveFirst();
            }
        }

        protected void _TryDoMoveCommand()
        {
            if (ActorData.MoveData.MoveType == EActorMoveType.Invalid)
            {
                if (moveCommands.Count > 0)
                {
                    currMoveCommand = moveCommands.First.Value;
                    CommandDirectMoveTo(currMoveCommand.currPosition);
                    if (currMoveCommand.targetDirection.x > 0)
                    {
                        ActorData.MoveData.FaceRight = true;
                    }
                    else if (currMoveCommand.targetDirection.x < 0)
                    {
                        ActorData.MoveData.FaceRight = false;
                    }
                    moveCommands.RemoveFirst();
                }
            }
        }

        public void SetPlayerJobTableID(int JobTableID)
        {       
            if(ActorData != null)
            {
                BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
                townPlayerData.JobID = JobTableID;
            }
        }

        public void SetPlayerRoleLv(ushort RoleLv)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;

            townPlayerData.RoleLv = RoleLv;
            townPlayerData.bRoleLvDirty = true;
            if (GraphicActor != null)
            {
                GraphicActor.OnLevelChanged((System.Int32)RoleLv);
                GraphicActor.UpdateLevel(RoleLv);
            }
        }

        public void SetPlayerAwakeState(bool bAwake)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.bAwakeDirty = bAwake;
        }

        public void SetPlayerName(string a_strName)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.Name = a_strName;
        }

        public void SetPlayerPKRank(int a_nPKRank, int a_nStar)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.pkRank = a_nPKRank;
            townPlayerData.pkStar = a_nStar;
        }

        public void SetPlayerTittle(uint tittle)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.tittle = tittle;
            if(GraphicActor != null)
            {
                GraphicActor.OnTittleChanged((System.Int32)tittle);
            }
        }

        public void SetPlayerGuildName(string name)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.GuildName = name;
            if (GraphicActor != null)
            {
                GraphicActor.OnGuildNameChanged(name);
            }
        }

        public void SetPlayerGuildDuty(byte duty)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.GuildPost = duty;
            if (GraphicActor != null)
            {
                GraphicActor.OnGuildPostChanged(duty);
            }
        }

        public void SetPlayerZoneID(int iZoneId)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.ZoneID = iZoneId;
        }

        public int GetPlayerZoneID()
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            if(townPlayerData != null)
            {
                return townPlayerData.ZoneID;
            }

            return 0;
        }

        public void SetAdventureTeamName(string name)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.AdventureTeamName = name;
            if (GraphicActor != null)
            {
                GraphicActor.UpdateAdventTeamName(name);
            }
        }

        public void SetTitleName(PlayerWearedTitleInfo wearedTitleInfo)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            //townPlayerData.TitleName = name;
            townPlayerData.WearedTitleInfo = wearedTitleInfo;
            if (GraphicActor != null)
            {
                GraphicActor.UpdateTitleName(wearedTitleInfo);
            }
        }

        public void SetGuildEmblemLv(int guildEmblemLv)
        {
            BeTownPlayerData townPlayerData = ActorData as BeTownPlayerData;
            townPlayerData.GuildEmblemLv = guildEmblemLv;
            if (GraphicActor != null)
            {
                GraphicActor.UpdateGuidLv(guildEmblemLv);
            }
        }

        /*
		public void EquipFasion(EFashionWearSlotType slotType, bool equip=true, int fashionID = 0,bool highPriority = false)
		{
            PlayerBaseData.GetInstance().AvatarEquipPart(null, slotType, fashionID, _geActor, highPriority);
			if (slotType == EFashionWearSlotType.Halo)
			{
				PlayerBaseData.GetInstance().AvatarEquipWing(null, fashionID, _geActor);
			}
		}

		public void EquipWeapon(int weaponID=0, int strengthenLevel=0,bool highPriority = false)
		{
			BeTownPlayerData townData = ActorData as BeTownPlayerData;
			PlayerBaseData.GetInstance().AvatarEquipWeapon(null, townData.JobID, weaponID, _geActor,highPriority);
			ShowEquipStrengthenEffect(strengthenLevel);
		}
        */

		public void ShowEquipStrengthenEffect(int strengthenLevel=0)
		{
			if (strengthenLevel < 0)
				return;

			PlayerBaseData.GetInstance().AvatarShowWeaponStrengthen(null, strengthenLevel, _geActor);
		}

		public bool IsFemaleGunner(int jobID)
		{
			return 
				jobID >= 50 && jobID <= 54 || 
				jobID >= 20 && jobID <= 24;
		}

        public virtual void CreatePet(int a_nPetID)
        {
            //return;

            DestroyPet();

            if (a_nPetID > 0)
            {
                BeTownPetData petData = new BeTownPetData
                {
                    PetID = a_nPetID
                };
                petData.MoveData.PosLogicToGraph = ActorData.MoveData.PosLogicToGraph;
                m_townPet = new BeTownPet(petData, _systemTown,true);
                m_townPet.SetOwner(this);
                m_townPet.SetDialogEnable(false);

                if (_geActor != null)
                {
                    m_townPet.InitGeActor(_geScene);
                }
            }
        }

        public void DestroyPet()
        {
            if (m_townPet != null)
            {
                m_townPet.Dispose();
                m_townPet = null;
            }
        }

        //创建城镇跟随怪物
        public void CreateFollow()
        {
            DestroyFollow();
            if (_geActor == null)
                return;
            BeTownPlayerData townData = ActorData as BeTownPlayerData;
            if (townData == null)
                return;
            string resPath = BeUtility.GetAttachModelPath(townData.JobID);
            if (resPath == null)
                return;
            GameObject attachNode = _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
            attachModel = Utility.AddModelToActor(resPath, _geActor, attachNode);
        }

        public void DestroyFollow()
        {
            if (attachModel != null)
            {
                GameObject.Destroy(attachModel);
                attachModel = null;
            }
        }

        public void Init3v3RoomPlayerJiaoDiGuangQuan()
        {
            if (_geActor == null)
            {
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.Pk3v3)
            {
                return;
            }

            RoomInfo roominfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roominfo == null || roominfo.roomSlotInfos == null)
            {
                //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");

                CreateFootIndicator(FootEffectType.DEFUALT);

                return;
            }

            bool bFind = false;

            for (int i = 0; i < roominfo.roomSlotInfos.Length; i++)
            {
                if (roominfo.roomSlotInfos[i].playerId == ActorData.GUID)
                {
                    if (roominfo.roomSlotInfos[i].group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_RED)
                    {
                        //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_hong");
                        CreateFootIndicator(FootEffectType.RED);
                    }
                    else
                    {
                        //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_lan");
                        CreateFootIndicator(FootEffectType.BULE);
                    }

                    bFind = true;

                    break;
                }
            }

            if (!bFind)
            {
                //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");
                CreateFootIndicator(FootEffectType.DEFUALT);
            }
        }

        public void Update3v3RoomPlayerJiaoDiGuangQuan(byte group)
        {
            if (_geActor == null)
            {
                return;
            }

            if (group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_RED)
            {
                CreateFootIndicator(FootEffectType.RED);
               // _geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_hong");
            }
            else
            {
                CreateFootIndicator(FootEffectType.BULE);
                //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_lan");
            }
        }

        public enum FootEffectType
        {
            DEFUALT = 0,
            RED,
            BULE,
        }

        public void CreateFootIndicator(FootEffectType type = FootEffectType.DEFUALT)
        {
            var prefix = "";
            var effectName = "";
            if (type == FootEffectType.DEFUALT)
                effectName = "Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo";
            else if (type == FootEffectType.RED)
            {
                prefix = "red";
                effectName = "Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_hong";
            }
                
            else if (type == FootEffectType.BULE)
            {
                prefix = "blue";
                effectName = "Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_lan";
            }
                

           
            var attachment = _geActor.CreateAttachment(prefix + Global.FOOT_INDICATOR_ATTACH_NAME, effectName, Global.ATTACH_NAME_ORIGIN,false, false, false);
            if (attachment != null && _geActor.GetAttachment(Global.HALO_ATTACH_NAME) != null && type == FootEffectType.DEFUALT)
            {
                attachment.SetVisible(false);
            }
            else
                attachment.SetVisible(true);
        }
    }
}
