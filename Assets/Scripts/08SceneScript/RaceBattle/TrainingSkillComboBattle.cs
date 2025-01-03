using Network;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
namespace GameClient
{
    public class TrainingSkillComboBattle : BaseBattle
    {

        public Action<int> UseSkillCallBack;
        public TrainingSkillComboBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
        }

        protected override void _createPlayers()
        {
            var main = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var players = mDungeonPlayers.GetAllPlayers();
            _prepareDebugData();
            for (int i = 0; i < players.Count; ++i)
            {
                var currentBattlePlayer = players[i];
                var current = players[i].playerInfo;
                BeActor actor = null;

                actor = main.CreateCharacter(
                       true,
                       current.occupation,
                       current.level,
                       (int)ProtoTable.UnitTable.eCamp.C_HERO + current.seat,
                       BattlePlayer.GetSkillInfo(current),
                       BattlePlayer.GetEquips(current,false),
                       null,
                       0,
                       null,
                       0,
                       null,
                       null,
                       null,
                       PlayerBaseData.GetInstance().GetAvatar(),
                       false,
                       false
                   );

                actor.RegisterEventNew(BeEventType.onCastSkill, args =>
                {
                    int skillId = args.m_Int;

                    _onPlayerUseSkill(skillId);

                });
                actor.RegisterEventNew(BeEventType.onHitOther, args =>
                //actor.RegisterEvent(BeEventType.onHitOther, args =>
                {
                    var skillID = args.m_Int2;
                    var id = args.m_Int3;
                    //if (args.Length >= 5)
                    //    id = (int)args[4];
                    _onPlayerHitOther(skillID, id);
                });


                players[i].playerActor = actor;

                actor.InitChangeEquipData(current.equips, current.equipScheme);
                actor.SetPosition(mDungeonData.GetBirthPosition(), true);
                actor.m_iRemoveTime = Int32.MaxValue;

                if (current.accid == ClientApplication.playerinfo.accid)
                {
                    actor.isLocalActor = true;
					actor.m_pkGeActor.AddTittleComponent((int)PlayerBaseData.GetInstance().iTittle, current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_PLAYER);
                    actor.m_pkGeActor.CreateInfoBar(current.name, PlayerInfoColor.PK_PLAYER, current.level);

                }

                if (null != actor.m_pkGeActor)
                {
                    actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
                }

                actor.isMainActor = true;
                actor.UseProtect();
                actor.UseActorData();
                actor.UseAdjustBalance();
            }

#if !LOGIC_SERVER
            mDungeonManager.GetGeScene().AttachCameraTo(mDungeonPlayers.GetMainPlayer().playerActor.m_pkGeActor);
#endif
        }
        public void RecreatePlayer()
        {
            if (mDungeonManager == null) return;
            var main = mDungeonManager.GetBeScene();
            main.ClearAllEntity();
            _createEntitys();
            mInputManager.SetVisible(true);
            mInputManager.ReloadButtons(mDungeonPlayers.GetMainPlayer().playerActor.professionID, mDungeonPlayers.GetMainPlayer().playerActor, GetSlot());
        }
        protected sealed override void _onSceneStart()
        {
            mDungeonManager.GetBeScene().isUseBossShow = false;
            mDungeonManager.GetBeScene().SetTraceDoor(false);
        }

        protected override void _onStart()
        {
            // _hiddenDungeonMap(true);
        }

        protected override void _createDoors()
        {
            // keep empty
        }

        protected override void _onDoorStateChange(BeEvent.BeEventParam args)
        {
            // keep empty
        }

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
            // keep empty
        }

        protected override void _onEnd()
        {
#if !LOGIC_SERVER
            base._onEnd();
            SkillComboControl.instance.SetEndBattle();
#endif
        }

        protected void _onPlayerUseSkill(int skill)
        {
            if (UseSkillCallBack != null)
            {
                UseSkillCallBack(skill);
            }
        }
        public Action<int, int> playerHitCallBack;
        protected virtual void _onPlayerHitOther(int skillID, int id)
        {
            if (playerHitCallBack != null)
                playerHitCallBack(skillID, id);
        }

        public void EndInstitueTrain()
        {
            if (hasSend) return;
            hasSend = true;
            GameFrameWork.instance.StartCoroutine(_sendDungeonRaceEndReqIter());
        }

        bool hasSend = false;
        private IEnumerator _sendDungeonRaceEndReqIter()
        {
            yield return _sendDungeonReportDataIter();

            var msgEvent = new MessageEvents();
            var res = new SceneDungeonRaceEndRes();
            var req = _getDungeonRaceEndReq();

            yield return _sendMsgWithResend(ServerType.GATE_SERVER, msgEvent, req, res);

            if (msgEvent.IsAllMessageReceived())
            {
                ClientSystemManager.instance.OpenFrame<InstituteFinishFrame>(FrameLayer.Middle, dungeonManager);
            }

        }

        protected IEnumerator _sendDungeonReportDataIter()
        {
            if (GetMode() == eDungeonMode.LocalFrame)
            {
                if (mDungeonManager == null)
                    yield break;
                if (mDungeonManager.GetDungeonDataManager() == null)
                    yield break;

                mDungeonManager.GetDungeonDataManager().PushFinalFrameData();

                mDungeonManager.GetDungeonDataManager().SendWorldDungeonReportFrame();

                yield return null;

                while (!mDungeonManager.GetDungeonDataManager().IsAllReportDataServerRecived())
                {
                    yield return null;
                }

                mDungeonManager.GetDungeonDataManager().UnlockUpdateCheck();
            }
        }

        protected SceneDungeonRaceEndReq _getDungeonRaceEndReq()
        {
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            var msg = new SceneDungeonRaceEndReq
            {
                beHitCount = (ushort)mDungeonStatistics.HitCount(mainPlayer.playerInfo.seat),
                usedTime = (uint)mDungeonStatistics.AllFightTime(true),
                score = (byte)mDungeonStatistics.FinalDungeonScore(),
                maxDamage = mDungeonStatistics.GetAllMaxHurtDamage(),
                skillId = mDungeonStatistics.GetAllMaxHurtSkillID(),
                param = mDungeonStatistics.GetAllMaxHurtID(),
                totalDamage = mDungeonStatistics.GetAllHurtDamage(),
                lastFrame = mDungeonManager.GetDungeonDataManager().GetFinalFrameDataIndex()
            };
            msg.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", msg.score, msg.beHitCount, msg.usedTime));
            return msg;
        }

        protected override void _onPostStart()
        {
#if !LOGIC_SERVER
            if (mInputManager != null && mDungeonPlayers != null && dungeonManager != null)
            {
                ClientSystemManager.instance.OpenFrame<InstituteBattleFrame>();
                mInputManager.ReloadButtons(mDungeonPlayers.GetMainPlayer().playerActor.professionID, mDungeonPlayers.GetMainPlayer().playerActor, GetSlot());
                BeDungeon data = dungeonManager as BeDungeon;
                if(data!=null&& data.GetDungeonDataManager()!=null)
                   SkillComboControl.instance.Init(data, data.GetDungeonDataManager().id.dungeonID);
            }
#endif
        }

        public Dictionary<int, int> GetSlot()
        {
            Dictionary<int, int> skillSlot = new Dictionary<int, int>();
            if (teachData == null) return skillSlot;

            if (BattleMain.instance == null) return skillSlot;
            if (BattleMain.instance.GetLocalPlayer() == null) return skillSlot;
            if (BattleMain.instance.GetLocalPlayer().playerActor == null) return skillSlot;
            if (BattleMain.instance.GetLocalPlayer().playerActor.GetEntityData() == null) return skillSlot;

            skillSlot.Add(1, BattleMain.instance.GetLocalPlayer().playerActor.GetEntityData().normalAttackID);
            skillSlot.Add(2, -1);
            skillSlot.Add(3, -1);

            for (int i = 0; i < teachData.datas.Length; i++)
            {
                ComboData data = teachData.datas[i];
                if (data.skillSlot == -1) continue;
                BeSkill skill = BattleMain.instance.GetLocalPlayer().playerActor.GetSkill(data.skillID);
                if (skill != null)
                {
                    if (skill.comboSkillSourceID != 0)
                    {
                        skillSlot[data.skillSlot] = skill.comboSkillSourceID;
                    }
                    else
                    {
                        skillSlot[data.skillSlot] = data.skillID;
                    }
                }


            }


            return skillSlot;
        }

        public void ModifyPlayerInfo(RacePlayerInfo info)
        {

            InstituteTable data = TableManager.instance.GetDataByDungeonID(info.occupation, dungeonManager.GetDungeonDataManager().id.dungeonID);
            if (data != null)
            {
                info.equips = new RaceEquip[data.EquipmentID.Count];
                for (int i = 0; i < data.EquipmentID.Count; i++)
                {
                    info.equips[i] = new RaceEquip();
                    info.equips[i].id = (uint)data.EquipmentID[i];
                    if (i == 0)
                    {
                        info.equips[i].phyAtk = 1000;
                        info.equips[i].magAtk = 1000;
                        info.equips[i].strengthen = 15;
                    }
                }
            }

            info.raceItems = new RaceItem[]
            {
                new RaceItem{id=300000105, num=ushort.MaxValue},
                new RaceItem{id=300000106, num=ushort.MaxValue}
            };
        }
        private void _prepareDebugData()
        {
            LoadComboData();
            RacePlayerInfo info = null;
            for (int i = 0; i < BattleDataManager.GetInstance().PlayerInfo.Length; i++)
            {
                RacePlayerInfo raceInfo = BattleDataManager.GetInstance().PlayerInfo[i];
                if (raceInfo.accid == ClientApplication.playerinfo.accid)
                {
                    info = raceInfo;
                }
            }
            if (info != null)
            {
                ModifyPlayerInfo(info);
                info.skills = GetRceSkillInfo(info.occupation);
            }

        }

        protected override void _createMonsters()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();
            var monsterCreatedCount = mBeScene.CreateMonsterList(monsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition());
        }

        private RaceSkillInfo[] GetRceSkillInfo(int jobID)
        {
            List<RaceSkillInfo> skillList = new List<RaceSkillInfo>();
            if (teachData == null) return null;


            for (int j = 0; j < teachData.datas.Length; j++)
            {
                ComboData data = teachData.datas[j];
                RaceSkillInfo skillInfo = skillList.Find(x => { return x.id == data.skillID; });
                if (skillInfo != null) continue;
                skillList.Add(new RaceSkillInfo
                {
                    id = (ushort)data.skillID,
                    level = (byte)data.skillLevel,
                    slot = (byte)data.skillSlot
                });
            }


            return skillList.ToArray();
        }

        private void LoadComboData()
        {
            teachData = TableManager.instance.GetComboData(dungeonManager.GetDungeonDataManager().id.dungeonID);
        }

        public ComboTeachData teachData;

        public bool IsLastCombo(int id)
        {

            if (teachData == null) return false;
            int max = 0;
            for (int i = 0; i < teachData.datas.Length; i++)
            {
                if (teachData.datas[i].skillGroupID >= max)
                {
                    max = teachData.datas[i].skillGroupID;
                }
            }
            if (id == max)
                return true;

            return false;
        }

        public ComboData GetComboData(int index)
        {
            if (teachData == null) return null;
            return teachData.datas[index];
        }
    }


}
