using System;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using ProtoTable;
namespace GameClient
{
    public sealed class BeFighterData : BeBaseActorData
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

        public int awaken;

        public string AdventureTeamName { get; set; }

        public PlayerWearedTitleInfo WearedTitleInfo { get; set; }

        public int GuildEmblemLv { get; set; }
    }
    public class BeFighter : BeBaseFighter
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
        bool m_isDead = false;
        protected ushort m_grassId = 0;
        protected BePet m_townPet = null;
        public ushort GrassId { get { return m_grassId; } }
        public bool IsDead { get { return m_isDead; } }

        GameObject attachModel = null;                  //附加在玩家身上的模型
        public BeFighter(BeFighterData data, ClientSystemGameBattle systemTown)
            : base(data, systemTown)
        {
            if (data.State == (int)Protocol.SceneObjectStatus.SOS_WALK)
            {
                AddMoveCommand(data.MoveData.Position, data.MoveData.TargetDirection, data.MoveData.FaceRight);
            }
        }
        public GeActorEx GraphicActor
        {
            get
            {
                return _geActor;
            }
        }
        public override void Dispose()
        {
            DestroyPet();
            DestroyFollow();
            base.Dispose();
            //Logger.LogErrorFormat("objId {0} is Dispose", this.ActorData.GUID);
        }
        public void SetDead()
        {
            //Logger.LogErrorFormat("objId {0} is Dead", this.ActorData.GUID);
            if (!m_isDead)
            {
                m_isDead = true;
                ResetMoveCommand();
                DoActionDead();
            }
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
                BeFighterData townData = ActorData as BeFighterData;
                var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(townData.JobID);
                if (jobData == null)
                {
                    Logger.LogErrorFormat(" 职业表 没有ID为 {0} 的条目", townData.JobID);
                    return;
                }
                //Logger.LogErrorFormat("CreateActor GUID {0} jobID {1}", townData.GUID, jobData.Mode);
                _geActor = geScene.CreateActor(jobData.Mode, 0, 0, false, false, false);
#if !LOGIC_SERVER
                _geActor.mPostLoadInfoBar = OnPostLoadInfoBar;
#endif
                _geActor.CreateInfoBar(townData.Name, townData.NameColor, townData.RoleLv);
                _geActor.AddSimpleShadow(Vector3.one);
                _geActor.SetProfessionId(townData.JobID);

                PlayerInfoColor color = PlayerInfoColor.TOWN_OTHER_PLAYER;
                if ((this as BeFighterMain) != null)
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
                //Logger.LogErrorFormat("[AddTittleComponent] iTittleID {0} name {1} guildduty {2} bangName {3} iRoleLv {4} pkRank {5} adventTeamName {6}",
                //                townData.tittle,
                //                townData.Name,
                //                townData.GuildPost,
                //                townData.GuildName,
                //                townData.RoleLv,
                //                townData.pkRank,
                //                townData.AdventureTeamName);
            ///    _geActor.AddTittleComponent((System.Int32)townData.tittle, townData.Name, townData.GuildPost, townData.GuildName, townData.RoleLv, townData.pkRank, color, townData.AdventureTeamName);
            //    Logger.LogErrorFormat("GUID {0} Name {1}", townData.GUID, townData.Name);
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

                ActorData.SetAniInfos(jobData.IdleAniName, jobData.WalkAniName, jobData.RunAniName,jobData.DeadAniName);
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
                            (int)(townData.avatorInfo.weaponStrengthen),
                            _geActor,
                            false,
                            townData.avatorInfo.isShoWeapon);

                }
                CreateFollow();
              
                if (townData.petID > 0)
                {
                    CreatePet(townData.petID);
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

        public void RemoveGeActor()
        {
            if (null != _geScene)
            {
                if (null != _geActor)
                {
                    _geScene.DestroyActor(_geActor);
                    DestroyFollow();
                    DestroyPet();
                    _geActor = null;
                }
            }
            else
            {
                if (null != _geActor)
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

        protected UInt32[] TestGetEquipFashions(int jobID)
        {
            int rand = UnityEngine.Random.Range(0, 8);
            UInt32[] equips = new UInt32[6];
            for (int i = 0; i < 5; ++i)
                equips[i] = (UInt32)(_fashionIDs[rand] + (jobID - jobID % 10) * 100000 + i);

            equips[5] = (UInt32)(_wingIDs[rand] + (jobID - jobID % 10) * 100000);

            return equips;
        }

        public sealed override void UpdateGeActor(float deltaTime)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;

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

                if (townPlayerData.bAwakeDirty)
                {
#if !LOGIC_SERVER
                    _geActor.PlayAwakeEffect();
#endif
                    townPlayerData.bAwakeDirty = false;
                }

                base.UpdateGeActor(deltaTime);
            }
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            _UpdateGrass();
            if (m_townPet != null)
            {
                m_townPet.Update(deltaTime);
            }
        }
        public override void UpdateMove(float timeElapsed)
        {
            _TryDoMoveCommand();
            base.UpdateMove(timeElapsed);
        }
        protected ushort _GetGrassId(float x, float y)
        {
            if (this._battle == null || _battle.LevelData == null)
            {
                return 0;
            }
            var gridSize = _battle.LevelData.GetGridSize();
            var logicXSize = _battle.LevelData.GetLogicXSize();
            var logicZSize = _battle.LevelData.GetLogicZSize();
            var gridX = (int)Math.Floor((x - logicXSize.x) / gridSize.x);
            var gridY = (int)Math.Floor((y - logicZSize.x) / gridSize.y);
            return _battle.LevelData.GetGrassId(gridX, gridY);
        }
        public override void SetGrassStat(GRASS_STATUS stat)
        {
            base.SetGrassStat(stat);
            if(m_townPet != null)
            {
                m_townPet.SetGrassStat(stat);
            }
        }
        bool m_lastInfoBarLoaded = false;
        private void OnPostLoadInfoBar(GeActorEx geActor)
        {
            if(this.GrassStatus == GRASS_STATUS.IN_GRASS)
            {
                if(geActor != null)
                    geActor.HideActor(true);    
            }
        }
        protected void _UpdateGrass()
        {
            if (_geActor == null || !_geActor.IsAvatarLoadFinished()) return;
            if (_battle == null) return;
            if (_battle.MainPlayer == null) return;
            var moveData = ActorData.MoveData;
            var curGrassId = _GetGrassId(moveData.Position.x, moveData.Position.z);
            if (curGrassId != m_grassId)
            {
                //移动后草丛id有变化
                m_grassId = curGrassId;
                if (m_grassId != 0)
                {
                    if (_battle.MainPlayer.ActorData.GUID != this.ActorData.GUID)
                    {
                        if (_battle.MainPlayer.GrassId != 0 && _battle.MainPlayer.GrassId == m_grassId)
                        {
                            //半透
                            SetGrassStat(GRASS_STATUS.SAME_WITH_MAINROLE_IN_GRASS);
                        }
                        else
                        {
                            //隐藏
                            SetGrassStat(GRASS_STATUS.IN_GRASS);
                        }
                    }
                    else
                    {
                        SetGrassStat(GRASS_STATUS.MAIN_ROLE_IN_GRASS);
                    }
                }
                else
                {
                    //现行
                    SetGrassStat(GRASS_STATUS.NONE);
                }
            }
            else if (_battle.MainPlayer.ActorData.GUID !=  ActorData.GUID && m_grassId != 0)
            {
                //主角移动后 
                if (_battle.MainPlayer.GrassId != m_grassId)
                {
                    SetGrassStat(GRASS_STATUS.IN_GRASS);
                }
                else
                {
                    SetGrassStat(GRASS_STATUS.MAIN_ROLE_IN_GRASS);
                }
            }
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
            base.DoActionIdle();

            BeFighterData townPlayerData = ActorData as BeFighterData;
            if (IsFemaleGunner(townPlayerData.JobID))
            {
                ActorData.SetAttachmentVisible(Global.WEAPON_ATTACH_NAME, true);
            }
            ActorData.PlayAttachmentAction(Global.WING_ATTACH_NAME, "Anim_Idle01");
        }

        public sealed override void DoActionWalk()
        {
            base.DoActionWalk();

            BeFighterData townPlayerData = ActorData as BeFighterData;
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

        public void SetPlayerRoleLv(ushort RoleLv)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;

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
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.bAwakeDirty = bAwake;
        }

        public void SetPlayerName(string a_strName)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.Name = a_strName;
        }

        public void SetPlayerPKRank(int a_nPKRank, int a_nStar)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.pkRank = a_nPKRank;
            townPlayerData.pkStar = a_nStar;
        }

        public void SetPlayerTittle(uint tittle)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.tittle = tittle;
            if (GraphicActor != null)
            {
                GraphicActor.OnTittleChanged((System.Int32)tittle);
            }
        }

        public void SetPlayerGuildName(string name)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.GuildName = name;
            if (GraphicActor != null)
            {
                GraphicActor.OnGuildNameChanged(name);
            }
        }

        public void SetPlayerGuildDuty(byte duty)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.GuildPost = duty;
            if (GraphicActor != null)
            {
                GraphicActor.OnGuildPostChanged(duty);
            }
        }

        public void SetPlayerZoneID(int iZoneId)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.ZoneID = iZoneId;
        }

        public GeEffectEx CreateAttackAreaEffect()
        {
            if (GraphicActor != null)
            {
                int effectInfoId = 4;
                return GraphicActor.CreateEffect(effectInfoId, new Vec3(0, 0, 0));
            }

            return null;
        }

        public int GetPlayerZoneID()
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            if (townPlayerData != null)
            {
                return townPlayerData.ZoneID;
            }

            return 0;
        }

        public string GetPlayerName()
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            if (townPlayerData != null)
            {
                return townPlayerData.Name;
            }

            return "";
        }

        public void SetAdventureTeamName(string name)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.AdventureTeamName = name;
            if (GraphicActor != null)
            {
                GraphicActor.UpdateAdventTeamName(name);
            }
        }

        public void SetTitleName(Protocol.PlayerWearedTitleInfo  playerWearedTitleInfo)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.WearedTitleInfo = playerWearedTitleInfo;
            if (GraphicActor != null)
            {
                GraphicActor.UpdateTitleName(playerWearedTitleInfo);
            }
        }

        public void SetGuildEmblemLv(int guildEmblemLv)
        {
            BeFighterData townPlayerData = ActorData as BeFighterData;
            townPlayerData.GuildEmblemLv = guildEmblemLv;
            if (GraphicActor != null)
            {
                GraphicActor.UpdateGuidLv(guildEmblemLv);
            }
        }

        public void ShowEquipStrengthenEffect(int strengthenLevel = 0)
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
                BePetData petData = new BePetData
                {
                    PetID = a_nPetID
                };
                petData.MoveData.PosLogicToGraph = ActorData.MoveData.PosLogicToGraph;
                m_townPet = new BePet(petData, _battle);
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
            BeFighterData townData = ActorData as BeFighterData;
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



            var attachment = _geActor.CreateAttachment(prefix + Global.FOOT_INDICATOR_ATTACH_NAME, effectName, Global.ATTACH_NAME_ORIGIN, false, false, false);
            if (attachment != null && _geActor.GetAttachment(Global.HALO_ATTACH_NAME) != null && type == FootEffectType.DEFUALT)
            {
                attachment.SetVisible(false);
            }
            else
                attachment.SetVisible(true);
        }

        public void CreateBullet(ulong targetId, int projectileId)
        {
            var data = new BeBattleProjectile.BeBulletData
            {
                attackId = this.ActorData.GUID,
                targetId = targetId,
                typeId = projectileId,
                damageValue = 5
            };
            var projectile = new BeBattleProjectile(data, _battle);
            projectile.ActorData.MoveData.PosLogicToGraph = this.ActorData.MoveData.PosLogicToGraph;
            projectile.ActorData.MoveData.PosServerToClient = this.ActorData.MoveData.PosServerToClient;
            projectile.InitGeActor(_geScene);
            _battle.Projectiles.AddFighter(projectile);
        }
    }
}
