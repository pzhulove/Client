using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoTable;

namespace GameClient
{
    public class ChangeOccuBattle : PVEBattle
    {
        public int MainPlayerOccuID
        {
            get; private set;
        }

        public int mOccuId = 0;
        public List<int> mAllOccus = new List<int>();

        BeActor MainPlayerActor
        {
            get
            {
                var player = mDungeonPlayers.GetMainPlayer();
                if (player != null)
                {
                    return player.playerActor;
                }
                return null;
            }
        }

        public ChangeOccuBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
            
        }

        protected override void _onStart()
        {
            base._onStart();

            MainPlayerOccuID = mDungeonPlayers.GetMainPlayer().playerInfo.occupation;
        }

        protected override void _createPlayers()
        {
            base._createPlayers();
            InitOccus();
            //CreatePlayer();
        }

        protected override void _createMonsters()
        {
            //base._createMonsters();
            CreateMonster();
        }

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
            ClientSystemManager.GetInstance().delayCaller.DelayCall(100, () =>
            {
                CreateMonster();
            });
        }
        
        protected sealed override void _onSceneStart()
        {
            ClientSystemManager.instance.OpenFrame<ChangeOccuBattleFrame>(FrameLayer.High);
        }
        
        public override void OnAfterSeedInited()
        {
            if (dungeonManager != null && dungeonManager.GetDungeonDataManager() != null)
            {
                dungeonManager.GetDungeonDataManager().OnInitClientDungeonData();
            }
        }

        void InitOccus()
        {
            if (mAllOccus.Count > 0)
            {
                return;
            }

            var battlePlayer = mDungeonPlayers.GetMainPlayer();
            if (battlePlayer != null)
            {
                var notOpen = new List<int>();
                mAllOccus.Clear();
                var occuDatas = TableManager.GetInstance().GetTable<JobTable>().GetEnumerator();
                while (occuDatas.MoveNext())
                {
                    var occuData = occuDatas.Current.Value as JobTable;
                    var occuId = occuData.ID;
                    if (occuId / 10 == battlePlayer.playerInfo.occupation / 10 && occuId % 10 != 0)
                    {
                        if (occuData.Open == 1)
                        {
                            mAllOccus.Add(occuId);
                        }
                        else
                        {
                            notOpen.Add(occuId);
                        }
                    }
                }
                if (notOpen.Count > 0)
                {
                    mAllOccus.AddRange(notOpen);
                }
                
                //mOccuId = battlePlayer.playerInfo.occupation;
                mOccuId = 0;
            }
        }
        
        
        
        void CreateMonster()
        {
            mDungeonManager.GetBeScene().ClearAllMonsters();
            
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();
            for (int i = 0; i < monsters.Count; ++i)
            {
                monsters[i].removed = false;
            }
            var monsterCreatedCount = mBeScene.CreateMonsterList(monsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition());
            this.thisRoomMonsterCreatedCount = monsterCreatedCount;
            
            var list = mBeScene.GetFullEntities();
            for (int i = 0; i < list.Count; ++i)
            {
                var item = list[i] as BeActor;
                if (item != null)
                {
                    if (item.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY && !item.isMainActor)
                    {
                        item.StartAI(null);
                    }
                }
            }
            /*BeActor monster = mDungeonManager.GetBeScene().CreateMonster(9800014, VInt3.zero);
            if (monster != null)
            {
                monster.SetFace(true);
                ClientSystemManager.GetInstance().delayCaller.DelayCall(500, () =>
                {
                    monster.StartAI(null);
                });
            }*/
        }

        void CreatePlayer()
        {
            mDungeonManager.GetBeScene().ClearAllCharacter();
            
            var battlePlayer = mDungeonPlayers.GetMainPlayer();
            if (battlePlayer != null)
            {
                var playerInfo = battlePlayer.playerInfo;
                if (playerInfo != null)
                {
                    if (battlePlayer.playerActor != null)
                    {
                        battlePlayer.playerActor.RemoveAllMechanism();
                        battlePlayer.playerActor.ClearEvent();
                        battlePlayer.playerActor.OnRemove(true);
                    }

                    var mBeScene = mDungeonManager.GetBeScene();
                    var skillInfo = GetSkillInfo(mOccuId);
                    var actor = mBeScene.CreateCharacter(true, mOccuId, playerInfo.level, 0, skillInfo, GetConfigEquips(mOccuId));
                    if (actor != null)
                    {
                        battlePlayer.playerActor = actor;
                        actor.SetIsDead(false);
                        actor.SetPosition(mDungeonManager.GetDungeonDataManager().GetBirthPosition(), true);
                        actor.SetFace(false);
                        actor.m_iRemoveTime = int.MaxValue;

                        actor.professionID = mOccuId;

                        mDungeonManager.GetGeScene().AttachCameraTo(actor.m_pkGeActor);

                        actor.RegisterEventNew(BeEventType.onHPChange, (args) =>
                        {
                            if (actor.GetEntityData().GetHPRate().single < 0.5f)
                            {
                                actor.SetIsDead(false);
                                actor.GetEntityData().SetHP(actor.GetEntityData().GetMaxHP());
                            }
                        });
                        actor.RegisterEventNew(BeEventType.onMPChange, (args) =>
                        {
                            if (actor.GetEntityData().GetMPRate().single < 0.5f)
                            {
                                actor.GetEntityData().SetMP(actor.GetEntityData().GetMaxMP());
                            }
                        });
                    }
                }
            }
        }

        Dictionary<int, int> GetSkillInfo(int Occu)
        {
            var dic = new Dictionary<int, int>();
            var jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(Occu);
            if (jobTable != null)
            {
                for (int i = 0; i < jobTable.ChangeOccuSkillsLength; ++i)
                {
                    dic.Add(jobTable.ChangeOccuSkills[i], 1);
                }
            }
            
            return dic;
        }

        List<ItemProperty> GetConfigEquips(int Occu)
        {
            var equips = new List<ItemProperty>();
            
            var jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(Occu);
            if (jobTable != null)
            {
                for (int i = 0; i < jobTable.ChangeOccuEquipsLength; i++)
                {
                    Protocol.RaceEquip raceEquip = new Protocol.RaceEquip();
                    raceEquip.id = (uint) jobTable.ChangeOccuEquips[i];
                    var item = BattlePlayer.GetEquip(raceEquip, false);
                    if (item != null)
                    {
                        equips.Add(item);
                    }
                }
            }

            return equips;
        }

        public void ChangeOccuId(int occuId)
        {
            if (mOccuId != occuId)
            {
                var battlePlayer = mDungeonPlayers.GetMainPlayer();
                if (battlePlayer != null)
                {
                    battlePlayer.playerInfo.occupation = (byte)occuId;
                    mOccuId = occuId;
                }
                ClientSystemManager.GetInstance().delayCaller.DelayCall(100, () =>
                {
                    CreatePlayer();
                    CreateMonster();
                    if (mInputManager == null)
                    {
                        _createInput();
                    }
                    else
                    {
                        _reLoadSkillButton(); 
                    }
                    
                });
            }
        }

        public void PauseChangeOccuFight()
        {
            mDungeonManager.GetBeScene().ClearAllEntity();
            mDungeonManager.PauseFight();
        }
        
        public void ResumeChangeOccuFight()
        {
            mDungeonManager.ResumeFight();
        }

        public void Exit()
        {
            BeUtility.ResetCamera();
            ChangeJobType param = ChangeJobSelectFrame.changeType;
            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(ChangeJobSelectFrame), param));
            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemTown>();
        }

        public void RefreshCD()
        {
            var actor = MainPlayerActor;
            if (actor != null)
            {
                actor.ResetSkillCoolDown();
            }
        }
    }
}
