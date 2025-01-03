using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using ProtoTable;

/// <summary>
/// Battle类
/// </summary>
namespace GameClient
{
    public class NewbieGuideBattle : BaseNewbieGuideBattle
    {
        public NewbieGuideBattle(BattleType type, eDungeonMode mode, int id) : base(type, eDungeonMode.Test, id)
        {
        }

        /// <summary>
        /// 减CD系数 0.0 到 1.0
        /// </summary>
        /// <returns></returns>
        protected sealed override float _getSkillCDReduceRate()
        {
            NewBieGuideJobData data = TableManager.instance.GetTableItem<NewBieGuideJobData>(PlayerBaseData.GetInstance().PreChangeJobTableID);
            if (data != null)
            {
                return data.CDReduceRate/1000.0f;
            }
            return 0;          
        }

        /// <summary>
        /// 技能等级
        /// </summary>
        /// <returns></returns>
        protected sealed override int _getSkillLevel()
        {
            NewBieGuideJobData data = TableManager.instance.GetTableItem<NewBieGuideJobData>(PlayerBaseData.GetInstance().PreChangeJobTableID);
            if (data != null)
            {
                return data.SkillLevel;
            }
            return 1;
        }


        protected sealed override IEnumerator _startGuide()
        {
            return new ProcessUnit()
                .Append(_Guide1())
                .Append(_Guide2())
                .Append(_Guide3())
                .Sequence();
        }

        protected sealed override void _onSceneStart()
        {
            mDungeonManager.GetBeScene().isUseBossShow = false;
            mDungeonManager.GetBeScene().SetTraceDoor(false);
        }


		public static IList<int> GetNewbieGuidePlayerSkills()
		{
            // IList<int> skilllist = null;

            int dataID = PlayerBaseData.GetInstance().PreChangeJobTableID;
            if (dataID == 0)
                dataID = PlayerBaseData.GetInstance().JobTableID;

            NewBieGuideJobData data = TableManager.instance.GetTableItem<NewBieGuideJobData>(dataID);
            if (data != null)
            {
                return data.SkillList;
            }
            return null;
        }

        //初始化玩家数据[Newbie]
        static public void InitNewbieGuidePlayerInfo(ref RacePlayerInfo info)
        {
            if (PlayerBaseData.GetInstance().PreChangeJobTableID > 0)
                info.occupation = (byte)PlayerBaseData.GetInstance().PreChangeJobTableID;
            NewBieGuideJobData data = TableManager.instance.GetTableItem<NewBieGuideJobData>(PlayerBaseData.GetInstance().PreChangeJobTableID);
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


        //第二个房间连招[Newbie]
        private IEnumerator _Guide2ComboTips()
        {
			/*
            if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.SwordMan)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                               new ComboSkillUnit(1512, 1.0f),
                            // new ComboSkillUnit(1500, 0.3f),
                           //  new ComboSkillUnit(1501, 0.3f),
                           //  new ComboSkillUnit(1503, 0.5f),
                             //new ComboSkillUnit(1503, 0.8f),
                             new ComboSkillUnit(1509, 0.9f),
                             new ComboSkillUnit(1604, 2.0f),
                             new ComboSkillUnit(1606, 0.5f),
                        });

            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.Gungirl)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                            new ComboSkillUnit(2504, 0.5f),
                            new ComboSkillUnit(2507, 1.3f),
                            new ComboSkillUnit(2517, 2.5f),
                            new ComboSkillUnit(2512, 0.4f),
                            // new ComboSkillUnit(2501, 0.7f),
                            // new ComboSkillUnit(2512, 0.5f), 
                        });
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.MagicMan)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                            // new ComboSkillUnit(1204, 1.0f),
                            // new ComboSkillUnit(1011, 1.0f),
                            // new ComboSkillUnit(1101, 1.0f),
                            // new ComboSkillUnit(1010, 1.0f),
                            new ComboSkillUnit(2011, 1.0f),
                        });
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.GunMan)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                            new ComboSkillUnit(1006, 1.0f),
                            new ComboSkillUnit(1013, 1.5f),
                            new ComboSkillUnit(1011, 2.4f),
                            new ComboSkillUnit(1101, 0.5f),

                            // new ComboSkillUnit(2011, 1.0f),
                        });
            }*/

            return null;
        }

        //高锦连招时间调下
        private IEnumerator _Guide3ComboTips()
        {
			/*
            if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.SwordMan)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                               new ComboSkillUnit(1608, 2.5f),
                        });

            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.Gungirl)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                            new ComboSkillUnit(2518, 0.5f),
                        });
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.MagicMan)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                            new ComboSkillUnit(2011, 1.0f),
                        });
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.GunMan)
            {
                return _addComboSkillTips("UIPacked/screenshots-2.png:screenshots-2",
                        new ComboSkillUnit[]
                        {
                            new ComboSkillUnit(1106, 1.0f),
                        });
            }*/

            return null;
        }

        //第二关引导，连招怪物ID[Newbie]  
        private int _Guide2GetMonsterComboID()
        {
            return 2150111;
        }

        //第二关引导，连招怪物创建位置[Newbie]
        private float _Guide2GetMonsterComboPosition()
        {
			/*
            if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.SwordMan)
            {
                return 2.4f;
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.Gungirl)
            {
                return 1.0f;
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.MagicMan)
            {
                return 2.7f;
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.GunMan)
            {
                return 2.8f;
            }
*/
            return 2.7f;
        }

        //怪物血量，Boss血量
        private int _GuideGetMonsterHP(int room, bool isBoss)
        {
            if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.SwordMan)
            {
                switch (room)
                {
                    case 1:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 10000;
                            }
                            //小怪血量
                            else
                            {
                                return 5000;
                            }
                        }
                        break;
                    case 2:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 10000;
                            }
                            //小怪血量
                            else
                            {
                                return 10000;
                            }
                        }
                        break;
                    case 3:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 40000;
                            }
                            //小怪血量
                            else
                            {
                                return 40000;
                            }
                        }
                        break;
                }
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.Gungirl)
            {
                switch (room)
                {
                    case 1:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 5000;
                            }
                            //小怪血量
                            else
                            {
                                return 5000;
                            }
                        }
                        break;
                    case 2:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 14000;
                            }
                            //小怪血量
                            else
                            {
                                return 14000;
                            }
                        }
                        break;
                    case 3:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 80000;
                            }
                            //小怪血量
                            else
                            {
                                return 40000;
                            }
                        }
                        break;
                }
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.MagicMan)
            {
                switch (room)
                {
                    case 1:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 5000;
                            }
                            //小怪血量
                            else
                            {
                                return 5000;
                            }
                        }
                        break;
                    case 2:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 15000;
                            }
                            //小怪血量
                            else
                            {
                                return 10000;
                            }
                        }
                        break;
                    case 3:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 90000;
                            }
                            //小怪血量
                            else
                            {
                                return 50000;
                            }
                        }
                        break;
                }
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.GunMan)
            {
                switch (room)
                {
                    case 1:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 5000;
                            }
                            //小怪血量
                            else
                            {
                                return 5000;
                            }
                        }
                        break;
                    case 2:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 90000;
                            }
                            //小怪血量
                            else
                            {
                                return 30000;
                            }
                        }
                        break;
                    case 3:
                        {
                            //boss血量
                            if (isBoss)
                            {
                                return 70000;
                            }
                            //小怪血量
                            else
                            {
                                return 40000;
                            }
                        }
                        break;
                }
            }
			else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.Gedoujia)//格斗家
			{
				switch (room)
				{
				case 1:
					{
						//boss血量
						if (isBoss)
						{
							return 5000;
						}
						//小怪血量
						else
						{
							return 5000;
						}
					}
					break;
				case 2:
					{
						//boss血量
						if (isBoss)
						{
							return 90000;
						}
						//小怪血量
						else
						{
							return 30000;
						}
					}
					break;
				case 3:
					{
						//boss血量
						if (isBoss)
						{
							return 70000;
						}
						//小怪血量
						else
						{
							return 40000;
						}
					}
					break;
				}
			}

            return 10000;
        }

        private IEnumerator _Guide2InitSkill()
        {
            return _addExSkill(new int[] { });
        }

        //第二关引导,连招技能[Newbie]
        private IEnumerator _Guide2AddExSkill()
        {
            NewBieGuideJobData data = TableManager.instance.GetTableItem<NewBieGuideJobData>(PlayerBaseData.GetInstance().PreChangeJobTableID);
            
            if (data != null)
            {
                int[] skillID = new int[data.SecondComboSkill.Count];
                for (int i = 0; i < data.SecondComboSkill.Count; i++)
                {
                    skillID[i] = data.SecondComboSkill[i];
                }
                return _addExSkill(skillID, 2);
            }          

            return _addExSkill(new int[] { });
        }

        //第三关引导，连招技能[Newbie]

        private IEnumerator _Guide3AddExSkill(int step)
        {

            NewBieGuideJobData data = TableManager.instance.GetTableItem<NewBieGuideJobData>(PlayerBaseData.GetInstance().PreChangeJobTableID);

            int[] skilllist = new int[] { };
            if (data != null)
            {
                skilllist = new int[data.ThirdComboSkill.Count];
                for (int i = 0; i < data.ThirdComboSkill.Count; i++)
                {
                    skilllist[i] = data.ThirdComboSkill[i];
                }
            }


            if (step == 0)
            {
                _initExSkill(skilllist);
                _hideExSkill(skilllist, 3, 4);
                yield break;
            }
            else if (step == 1)
            {
                yield return _addExSkillEx(skilllist, 3, 4);
                yield break;
            }

            yield break;
        }

        //第三关引导，最后技能[Newbie]
        private IEnumerator _Guide3UseFinalSkill()
        {
            if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.SwordMan)
            {
                return _useFinalSkill(1604, 0, ActorOccupation.SwordMan);
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.Gungirl)
            {
                return _useFinalSkill(2517, 0, ActorOccupation.Gungirl);
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.MagicMan)
            {
                new ProcessUnit()
                    .Append(_useFinalSkill(2008, 0, ActorOccupation.MagicMan))
                    .Append(_createAPC(9080031, 1, 5324))
                    .Sequence();
            }
            else if (PlayerBaseData.GetInstance().JobTableID == (int)ActorOccupation.GunMan)
            {
                return _useFinalSkill(2517, 0, ActorOccupation.GunMan);
            }

            return _useFinalSkill(1505, 0, ActorOccupation.SwordMan);
        }

        //第三关引导，Combo压屏[Newbie]

        private IEnumerator _Guide2SummonMonster()
        {

            BeActor mainPlayer = _GuideGetMainPlayer();

            if (mainPlayer != null)
            {
                mainPlayer.ModifyMoveDirection(true, 0);
                mainPlayer.ChangeRunMode(true);
            }

            yield return Yielders.GetWaitForSeconds(0.2f);

            if (mainPlayer != null)
            {
                mainPlayer.ModifyMoveDirection(false);
            }

            yield return Yielders.GetWaitForSeconds(0.5f);

            var pos = mainPlayer.GetPosition();
            var monster = mDungeonManager.GetBeScene().SummonMonster(
                _Guide2GetMonsterComboID(),
                new VInt3(pos.x + VInt.Float2VIntValue(_Guide2GetMonsterComboPosition()), pos.y, pos.z), 1);
            monster.buffController.TryAddBuff(31, 2000);
            monster.aiManager.Stop();
            mDungeonManager.GetGeScene().CreateEffect(1016, monster.GetPosition().vec3);

            yield return Yielders.GetWaitForSeconds(2.0f);
        }

        //private IEnumerator _GuideCreateMonster()
        protected IEnumerator _waitForPlayerMove(float t1 = 0.5f, float t2 = 0.5f)
        {
            FreazeMonsters();

            BeActor mainPlayer = _GuideGetMainPlayer();

            if (mainPlayer != null)
            {
                mainPlayer.ModifyMoveDirection(true, 0);
                mainPlayer.ChangeRunMode(true);
            }

            yield return Yielders.GetWaitForSeconds(t1);

            if (mainPlayer != null)
            {
                mainPlayer.ModifyMoveDirection(false);
            }

            yield return Yielders.GetWaitForSeconds(t2);
        }

        protected IEnumerator _mainPlayerTalk(string text)
        {
            var main = _GuideGetMainPlayer();

            if (main != null)
            {
                main.m_pkGeActor.ShowHeadDialog(text);
            }

            yield break;
        }

        protected void _mainPlayerTalkEx(string text)
        {
            var main = _GuideGetMainPlayer();

            if (main != null)
            {
                main.m_pkGeActor.ShowHeadDialog(text);
            }
        }

        private IEnumerator _GuideShowNextRoom(float delays)
        {
            _DoStateNewbieGuideFunc("_GuideShowNextRoom");
            Log("_GuideShowNextRoom delays {0} ", delays);
            InvokeMethod.Invoke(delays, () =>
            {
                ClientSystemManager.instance.OpenFrame<NewbieGuideNextRoom>(FrameLayer.Middle);
            });
            yield break;
        }


        protected override void ChangeActorAttribute(BeActor actor)
        {
            base.ChangeActorAttribute(actor);
            if (actor.isLocalActor)
            {               
                BeEntityData data = actor.GetEntityData();
                if (data != null)
                {
                    NewBieGuideJobData tableData = TableManager.instance.GetTableItem<NewBieGuideJobData>(PlayerBaseData.GetInstance().PreChangeJobTableID);
                    if (tableData != null)
                    {
                        data.SetAttributeValue(AttributeType.attackSpeed, tableData.AttackSpped,true);
                        data.SetAttributeValue(AttributeType.spellSpeed, tableData.SpellSpeed,true);
                        data.SetAttributeValue(AttributeType.moveSpeed, tableData.WalkSpeed,true);
                    }
                }
            }
        }



        private BeActor _GetBoss()
        {
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            BeActor boss = null;
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                if (current != null && current.IsMonster() && current.IsBoss())
                {
                    boss = current;
                }
            }

            return boss;
        }


        private IEnumerator _GuideMonsterSpeach(int monsterid, string text, bool isMonster = true)
        {
            var current = _getEntityByID(monsterid, isMonster);

            if (current != null)
            {
                current.m_pkGeActor.ShowHeadDialog(text);
            }

            yield break;
        }

        private void _GuideMonsterSpeachEx(int monsterid, string text, bool isMonster = true)
        {
            //var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            //for (int i = 0; i < entityList.Count; ++i)
            //{
            var current = _getEntityByID(monsterid, isMonster);

            if (current != null)
            {
                current.m_pkGeActor.ShowHeadDialog(text);
            }
        }

        private IEnumerator _GuideMonsterUseSkill(int monsterid, int skillid, bool isMonster = true)
        {
            Log("_GuideMonsterUseSkill id {0} skill {1}", monsterid, skillid);

            var current = _getEntityByID(monsterid, isMonster);

            if (current != null)
            {
                current.buffController.RemoveBuff(31);
				current.aiManager.StopCurrentCommand();
                current.UseSkill(skillid, true);
                //UnityEngine.Debug.LogFormat("[使用技能] {0} 使用了 {1}", monsterid, skillid);
            }

            yield break;
        }


        private IEnumerator _GuideMainPlayerUseSkill(int skillid)
        {
            Log("_GuideMainPlayerUseSkill  skill {0}", skillid);
            var mainPlayers = mDungeonPlayers.GetMainPlayer();
            if (mainPlayers != null)
            {
                mainPlayers.playerActor.UseSkill(skillid, true);
                //UnityEngine.Debug.LogFormat("[使用技能] {0} 使用了 {1}", monsterid, skillid);
            }

            yield break;
        }

        private IEnumerator _GuideResetMainPlayerState()
        {
            if (mDungeonPlayers == null) yield break;
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            if (mainPlayer != null && mainPlayer.playerActor != null)
            {
                mainPlayer.playerActor.Reset();
            }
            yield break;
        }

        private IEnumerator _GuideResetMonster(int monsterid, bool isMonster = true)
        {
            var current = _getEntityByID(monsterid, isMonster);
            if (current != null)
            {
                current.Reset();
            }
            yield break;
        }

        private IEnumerator _GuideEnableInputManager(bool bEnable)
        {
            Log("_GuideEnableInputManager {0}", bEnable);
            if (mInputManager == null) yield break;
            mInputManager.SetEnable(bEnable);
            yield break;
        }

        private IEnumerator _GuideVisiableInputManager(bool bJoyShow, bool bButtonShow)
        {
            Log("_GuideVisiableInputManager Joy{0} Button{1}", bJoyShow, bButtonShow);
            if (mInputManager == null) yield break;
            mInputManager.SetVisible(bJoyShow, bButtonShow);
            yield break;
        }

        private IEnumerator _GuideHackPlayerHPMP(int hp, int mp)
        {
            var player = _GuideGetMainPlayer();
            if (player != null)
            {
                var data = player.GetEntityData().battleData;
                data.maxHp = hp;
                data.hp = hp;
                data.maxMp = mp;
                data.mp = mp;

                player.m_pkGeActor.ResetHPBar();
            }

            yield break;
        }

        private IEnumerator _GuideHackMonsterHP(int hp, bool bSetBoss = false)
        {
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                if (current != null && current.IsMonster() && (bSetBoss ? current.IsBoss() : true))
                {
                    var data = current.GetEntityData().battleData;
                    data.maxHp = hp;
                    data.hp = hp;
                    current.m_pkGeActor.ResetHPBar();
                }
            }

            yield break;
        }

        /// <summary>
        /// [Newbie] 命中，暴击修正
        /// </summary>
        private IEnumerator _GuideHackMainPlayerDex()
        {
            BeActor mainPlayer = _GuideGetMainPlayer();

            if (mainPlayer != null)
            {
                var data = mainPlayer.GetEntityData().battleData;
                mainPlayer.GetEntityData().battleData.dex = 2000;
                data.ciriticalAttack = 0;
                data.ciriticalMagicAttack = 0;
            }

            //string str = string.Format("[GuideBattle]Start");
            //GameStatisticManager.GetInstance().DoStatistic(str);
            yield break;
        }


        BeActor _GuideGetMainPlayer()
        {
            if (mDungeonManager == null) return null;
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            BeActor mainPlayer = null;

            //FindMainPlayer
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i];
                if (current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO)
                {
                    mainPlayer = current as BeActor;
                    break;
                }
            }

            return mainPlayer;
        }

        private IEnumerator _GuideLockPlayerMove(bool bLock)
        {
            Log("_GuideLockPlayerMove bLock {0} ", bLock);
            BeActor mainPlayer = _GuideGetMainPlayer();
            if (mainPlayer == null) yield break;
            //mainPlayer.stateController.SetAbilityEnable(BeAbilityType.MOVE, !bLock);

            mainPlayer.bLockMove = bLock;
            mainPlayer.ResetMoveCmd();
            mainPlayer.SetAttackButtonState(ButtonState.RELEASE);

            yield break;
        }

        private IEnumerator _Guide2Begin()

        {
            yield return _waitForState(BeSceneState.onReady);
            _DoStateNewbieGuideFunc("_Guide2Begin");
            yield return _GuideLockPlayerMove(true);
            if(fingerDoubleMove != null)
                GameObject.Destroy(fingerDoubleMove);
            fingerDoubleMove = null;
            if (mInputManager == null) yield break;
            mInputManager.SetVisible(false);
            FreazeMonsters();
            ClientSystemManager.instance.CloseFrame<NewbieGuideNextRoom>();
            yield return _HideGuideTips(0.0f);
            yield break;
        }

        private IEnumerator _GuideUnFreaseMonsters(float delays)
        {
            InvokeMethod.Invoke(delays, () => { UnFreazeMonsters(); });
            yield break;
        }

        private IEnumerator _ShowGuideTips(string text, float showtime, bool bLeft = true)
        {
            InvokeMethod.Invoke(showtime, () =>
            {
                ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
                NewbieGuidTipsFrame tips = ClientSystemManager.instance.OpenFrame<NewbieGuidTipsFrame>(FrameLayer.Top, bLeft) as NewbieGuidTipsFrame;
                tips.SetTipsText(text);
            });
            yield break;
        }

        private IEnumerator _ShowGuideTipsEx3(string text, float delaytime, float showtime, bool bLeft = true)
        {
            InvokeMethod.Invoke(delaytime, () =>
            {
                ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
                NewbieGuidTipsFrame tips = ClientSystemManager.instance.OpenFrame<NewbieGuidTipsFrame>(FrameLayer.Top, bLeft) as NewbieGuidTipsFrame;
                tips.SetTipsText(text);
            });

            InvokeMethod.Invoke(delaytime + showtime, () =>
           {
               ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
           });
            yield break;
        }

        private IEnumerator _WaitForUnitEvent(int id, bool isMonster, BeEventType e)
        {
            Log("_WaitForUnitEvent id {0} isMonster {1} BeEventType {2}", id, isMonster, e);

            var current = _getEntityByID(id, isMonster);

            if (current != null)
            {
                bool bWaited = false;
                current.RegisterEventNew(e, (args) => { bWaited = true; });

                while (bWaited == false)
                {
                    yield return Yielders.EndOfFrame;
                }
            }
            else
            {
                Logger.LogError("[_WaitForUnitEvent] 找不到 id" + id);
                yield break;
            }
        }

        private IEnumerator _ShowGuideTipsEx(string text, float showtime, float speed, float delay, bool bLeft = true)
        {
            Log("_ShowGuideTipsEx text {0} ", text);
            InvokeMethod.Invoke(showtime, () =>
            {
                _DoStateNewbieGuideFunc("_ShowGuideTipsEx" + text);
                ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
                NewbieGuidTipsFrame tips = ClientSystemManager.instance.OpenFrame<NewbieGuidTipsFrame>(FrameLayer.Top, bLeft) as NewbieGuidTipsFrame;
                //tips.SetTipsText(text);
                tips.SetTipsTextEx(text, speed, delay);
            });
            yield break;
        }

        private IEnumerator _ShowGuideTipsEx2(string text, float showtime, bool bLeft = true)
        {
            Log("_ShowGuideTipsEx2 text {0} ", text);
            InvokeMethod.Invoke(showtime, () =>
            {
                ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
                NewbieGuidTipsFrame tips = ClientSystemManager.instance.OpenFrame<NewbieGuidTipsFrame>(FrameLayer.Top, bLeft) as NewbieGuidTipsFrame;
                //tips.SetTipsText(text);
                tips.SetTipsTextEx(text, 0.05f, 1.5f);
            });
            yield break;
        }

        private IEnumerator _HideGuideTips(float showtime)
        {
            Log("_HideGuideTips  showtime {0}", showtime);
            InvokeMethod.Invoke(showtime, () =>
            {
                Log("_HideGuideTips");
                ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
            });
            yield break;
        }

        /// <summary>
        /// 正确版本显示引导文字
        /// </summary>
        private IEnumerator _rightShowGuideTip(string text, float showTime, bool bLeft = true)
        {
            ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
            NewbieGuidTipsFrame tips = ClientSystemManager.instance.OpenFrame<NewbieGuidTipsFrame>(FrameLayer.Top, bLeft) as NewbieGuidTipsFrame;
            tips.SetTipsText(text);

            yield return Yielders.GetWaitForSeconds(showTime);

            ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
        }

        /*
         tips.SetTipsText("好招！请继续使用技能清除怪物吧！");
                    InvokeMethod.Invoke(3.0f,()=>{
                        ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
                    });
        */

        private IEnumerator _GuildeHackMainPlayerCDandCost(int cd, int mp)
        {
            var mainPlayer = _GuideGetMainPlayer();

            if (mainPlayer != null)
            {
                var data = mainPlayer.GetEntityData().battleData;
                data.cdReduceRate = cd;
                data.mpCostReduceRate = mp;
            }

            yield break;
        }

        private BeActor _getEntityByID(int id = -1, bool isMonster = true)
        {
            if (mDungeonManager == null) return null;
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                bool flag = false;

                if (isMonster)
                {
                    if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        BeEntity player = mDungeonPlayers.GetMainPlayer().playerActor;
                        if (current != player)
                        {
                            flag = true;
                        }
                    }
                }

                if (flag)
                {
                    return current;
                }
            }

            //UnityEngine.Debug.LogErrorFormat("[找] 找不到怪物 {0} 是否是怪物 {1}", id, isMonster);

            return null;
        }

        private IEnumerator GetupMonster(int id = -1, bool isMonster = true)
        {
            BeActor current = _getEntityByID(id, isMonster);

            if (null != current)
            {
                current.buffController.TryAddBuff(31, -1);
                current.hasAI = false;
                current.Reset();

                //current.m_pkGeActor.GetAnimation().PlayAction("Anim_Xiadun", 1.0f, true);
                current.m_pkGeActor.ChangeAction("Anim_Xiadun", 1.0f, true, true, true);
            }

            yield break;
        }

        private IEnumerator WeakMonster(int id = -1, bool isMonster = true)
        {
            BeActor current = _getEntityByID(id, isMonster);

            if (null != current)
            {
                current.buffController.TryAddBuff(31, -1);
                current.hasAI = false;
                current.Reset();

                //current.m_pkGeActor
                current.PlayAction(ActionType.ActionType_DEAD);
            }

            yield break;
        }

        private IEnumerator DeadMonster(int id, bool isMonster = true)
        {
            Log("dead monster id:{0}", id);
            BeActor current = _getEntityByID(id, isMonster);

            if (null != current)
            {
                current.DoDead();
            }

            yield break;
        }

        private IEnumerator SetMonsterBlock(int id, bool block = true, bool isMonster = true)
        {
            Log("SetMonsterBlock id {0} block {1} isMonster {2}", id, block, isMonster);
            BeActor current = _getEntityByID(id, isMonster);
            if (null != current)
            {
                //current.stateController.SetState(BeStateType.BLOCK, )
                current.stateController.SetAbilityEnable(BeAbilityType.BLOCK, block);
            }
            yield break;
        }

        private IEnumerator SetMonsterZhenWudi(int id, bool zhenwudi = true, bool isMonster = true)
        {

            BeActor current = _getEntityByID(id, isMonster);
            if (null != current)
            {
                //current.stateController.SetState(BeStateType.BLOCK, )
                current.stateController.SetAbilityEnable(BeAbilityType.FALLGROUND, zhenwudi);
                current.stateController.SetAbilityEnable(BeAbilityType.FLOAT, zhenwudi);
                current.stateController.SetAbilityEnable(BeAbilityType.BEGRAB, zhenwudi);
                current.stateController.SetAbilityEnable(BeAbilityType.BEBREAK, zhenwudi);
            }
            yield break;
        }

        private IEnumerator SetMainplayerZhenwudi(bool zhenwudi)
        {

            BeActor current = _GuideGetMainPlayer();
            if (null != current)
            {
                //current.stateController.SetState(BeStateType.BLOCK, )
                current.stateController.SetAbilityEnable(BeAbilityType.FALLGROUND, zhenwudi);
                current.stateController.SetAbilityEnable(BeAbilityType.FLOAT, zhenwudi);
                current.stateController.SetAbilityEnable(BeAbilityType.BEGRAB, zhenwudi);
                current.stateController.SetAbilityEnable(BeAbilityType.BEBREAK, zhenwudi);
            }
            yield break;
        }

        private IEnumerator SetMonsterBatiShow(int id, bool batiShow, bool isMonster)
        {
            BeActor current = _getEntityByID(id, isMonster);
            if (null != current)
            {
                if (batiShow)
                {
                    current.m_pkGeActor.ChangeSurface("霸体", 0);
                }
                else
                {
                    //current.m_pkGeActor.ChangeSurface("",0);
                    current.m_pkGeActor.RemoveSurface(uint.MaxValue);
                }
            }
            yield break;
        }

        private IEnumerator WaitMonsterHPLow(float percent, int id, bool isMonster)
        {
            BeActor current = _getEntityByID(id, isMonster);
            if (null != current)
            {
                while (current != null && current.GetEntityData().GetHPRate().single > percent)
                {
                    yield return Yielders.EndOfFrame;
                }
            }
            yield break;
        }
        private IEnumerator MonsterTalk(string text, int id = -1, bool isMonster = true)
        {
            _DoStateNewbieGuideFunc("MonsterTalk " + text);
            Log("MonsterTalk Text {0} id {1} isMonster {2}", text, id, isMonster);
            BeActor current = _getEntityByID(id, isMonster);

            if (null != current)
            {
                current.m_pkGeActor.ShowHeadDialog(text);
                //current.PlayAction(ActionType.ActionType_Getup);
            }

            yield break;
        }

        private IEnumerator FreazeMonsters(int id = -1, bool isMonster = true)
        {
            if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
            {
                var entityList = mDungeonManager.GetBeScene().GetFullEntities();

                if(entityList != null)
                {
                    for (int i = 0; i < entityList.Count; ++i)
                    {
                        var current = entityList[i] as BeActor;
                       
                        if (current == null)
                        {
                            continue;
                        }

                        BeEntityData entityData = current.GetEntityData();
                        if(entityData == null)
                        {
                            continue;
                        }

                        bool flag = false;

                        if (isMonster)
                        {
                            if (current != null && current.IsMonster() && (id == -1 ? true : id == entityData.monsterID))
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            if (current != null && current.GetCamp() == (int)UnitTable.eCamp.C_HERO && (id == -1 ? true : id == entityData.monsterID) && mDungeonPlayers != null && mDungeonPlayers.GetMainPlayer() != null)
                            {
                                BeEntity player = mDungeonPlayers.GetMainPlayer().playerActor;
                                if (current != player)
                                {
                                    flag = true;
                                }
                            }
                        }

                        if (current != null && flag)
                        {
                            Log("FreazeMonsters {0} isMonster {1}", id, isMonster);
                            if(current.buffController != null)
                            {
                                current.buffController.TryAddBuff(31, -1);
                            }
                            
                            current.hasAI = false;
                            current.Reset();
                        }
                    }
                }

                yield break;
            }
        }

        private IEnumerator AddMonstersBuff(int id, bool isMonster, int bufferid, int time = -1)
        {
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                bool flag = false;

                if (isMonster)
                {
                    if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        BeEntity player = mDungeonPlayers.GetMainPlayer().playerActor;
                        if (current != player)
                        {
                            flag = true;
                        }
                    }
                }

                if (flag)
                {
                    current.buffController.TryAddBuff(bufferid, time);
                }
            }
            yield break;
        }

        private IEnumerator RemoveMonstersBuff(int id, bool isMonster, int bufferid)
        {
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                bool flag = false;

                if (isMonster)
                {
                    if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        BeEntity player = mDungeonPlayers.GetMainPlayer().playerActor;
                        if (current != player)
                        {
                            flag = true;
                        }
                    }
                }

                if (flag)
                {
                    current.buffController.RemoveBuff(bufferid);
                }
            }
            yield break;
        }

        private IEnumerator SetMainPlayerFaceMonster(int mid)
        {
            BeActor mainPlayer = _GuideGetMainPlayer();
            var monster = _getEntityByID(mid, true);
            if (mainPlayer != null && monster != null)
            {
                mainPlayer.SetFace(monster.GetPosition().x < mainPlayer.GetPosition().x, true);

				float jumpBackDistance = 1.6f;
				var pos = mainPlayer.GetPosition();
				pos.z = 0;
				if (mainPlayer.GetFace())
					pos.x += VInt.Float2VIntValue(jumpBackDistance);
				else 
					pos.x -= VInt.Float2VIntValue(jumpBackDistance);

				if (mainPlayer.CurrentBeScene.IsInBlockPlayer(pos))
				{
					mainPlayer.SetFace(!mainPlayer.GetFace(), true);
				}
            }

            yield break;
        }

        private IEnumerator SetKillMask(int id = -1, bool isMonster = true, bool bAdd = true)
        {
            if (mDungeonManager == null) yield break;
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                bool flag = false;

                if (isMonster)
                {
                    if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        BeEntity player = mDungeonPlayers.GetMainPlayer().playerActor;
                        if (current != player)
                        {
                            flag = true;
                        }
                    }
                }

                if (flag)
                {
                    current.m_pkGeActor.AddKillMark();
                }
            }

            yield break;
        }


        private IEnumerator UnFreazeMonsters(int id = -1, bool isMonster = true)
        {
            if (mDungeonManager == null) yield break;
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                bool flag = false;

                if (isMonster)
                {
                    if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        BeEntity player = mDungeonPlayers.GetMainPlayer().playerActor;
                        if (current != player)
                        {
                            flag = true;
                        }
                    }
                }

                if (flag)
                {
                    Log("UnFreazeMonsters {0} isMonster {1}", id, isMonster);
                    current.buffController.RemoveBuff(31);
                    current.hasAI = true;
                    current.Reset();
                    current.StartAI(null);
                }
            }

            yield break;
        }

        private IEnumerator _Parallel(float delay, UnityAction ation)
        {
            InvokeMethod.Invoke(delay, ation);
            yield break;
        }

        private IEnumerator _addBuffer2MainPlayer(int bufferid)
        {
            BeActor mainPlayer = _GuideGetMainPlayer();
            if (mainPlayer != null)
            {
                mainPlayer.buffController.TryAddBuff(bufferid, -1);
            }
            yield break;
        }

        BossController bossShow = null;
        BeActor boss = null;
        private IEnumerator _Guide3Begin()
        {
            yield return _waitForState(BeSceneState.onReady);
            mInputManager.SetVisible(false);
            boss = _GetBoss();
            boss.m_pkGeActor.SetHeadInfoVisible(false);
            boss.m_pkGeActor.SetFootIndicatorVisible(false);
            //var root = boss.m_pkGeActor.GetEntityNode(GeActorEx.GeEntityNodeType.Root);
            //bossShow = root.GetComponentInChildren<BossController>();
            //bossShow.SetUp();
            ClientSystemManager.instance.CloseFrame<NewbieGuideNextRoom>();
            yield return _HideGuideTips(0.0f);
            yield break;
        }

        Vector3 moveInPostion = new Vector3(14.3f, 0, 6.4f);
        float moveInRadius = 1.5f;

        /// <summary>
        /// 在场景中播放特效
        /// </summary>
        private IEnumerator _playEffectInScene(string effectpath, float time, Vec3 pos, bool isLoop = false, bool isFaceLeft = false)
        {
            GeEffectEx moveInEffect = mDungeonManager.GetGeScene().CreateEffect(effectpath, time, pos, 1, 1, isLoop, isFaceLeft);
            yield return _waitForTime(time);

            if (null != moveInEffect)
            {
                mDungeonManager.GetGeScene().DestroyEffect(moveInEffect);
            }
        }

        //private IEnumerator _GuideUseSkill(int index, float timeLen)
        //{
        //    var list = mInputManager.SkillButtons;
        //    var jumpback = LoadUIEffect(list[index], buttonTips);
        //    InvokeMethod.Invoke(timeLen, () => { GameObject.Destroy(jumpback); });
        //    yield break;
        //}

        /// <summary>
        /// 引导使用后跳技能
        /// </summary>
        private IEnumerator _GuideJumpback()
        {
            _DoStateNewbieGuideFunc("_GuideJumpback Guide2 - Begin");

            if (mInputManager.ButtonSlotMap == null) yield break;
            if (!mInputManager.ButtonSlotMap.ContainsKey(3)) yield break;

            var jumpBackBtn = mInputManager.ButtonSlotMap[3];

            jumpBackBtn.SetSkillBtnVisible(true);

            yield return SetMainPlayerFaceMonster(guide2_huonv);

            //yield return Yielders.GetWaitForSeconds(0.4f);
            var jumpback = LoadUIEffect(jumpBackBtn.gameObject, buttonTips);
            yield return _GuideShowSkill(jumpBackBtn.gameObject, 0.3f);
            yield return _ShowGuideTipsEx2("快使用后跳可以快速位移，躲避敌人技能", 0.5f);
            bool bInput = false;

            if (mDungeonManager == null)
            {
                yield break;
            }

            mDungeonManager.PauseFight();
            InputManager.onSkillCommandCallBack = (id, value) =>
            {
                if (id == (int)SpecialSkillID.JUMP_BACK && CanResumeFightJumpBack(bInput))
                {
                    bInput = true;
                    if (mDungeonManager != null)
                    {
                        mDungeonManager.ResumeFight();
                    }
                }
            };

            

            while (!bInput)
            {
                yield return Yielders.EndOfFrame;
            }

            if (null != jumpback)
            {
                GameObject.Destroy(jumpback);
                jumpback = null;
            }

            InputManager.onSkillCommandCallBack = null;
            ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
            
            

            _DoStateNewbieGuideFunc("_GuideJumpback Guide2 - End");
        }

        //接收后跳按钮回调时能否响应
        private bool CanResumeFightJumpBack(bool input)
        {
            if (!BeClientSwitch.FunctionIsOpen(ClientSwitchType.NewbieGuideJumpBack))
                return true;
            if (!input)
                return true;
            return false;
        }


        GameObject temp;
        private IEnumerator _GuideShowJoystickAndSkillButton()
        {

            //隐藏普
            _DoStateNewbieGuideFunc("MoveJoyStik Guide1");
            yield return Yielders.GetWaitForSeconds(0.3f);
            yield return _ShowGuideTips("操作左下方移动按钮，移动到指定位置", 0.0f);
            yield return Yielders.GetWaitForSeconds(0.2f);
            yield return _GuideEnableInputManager(true);
            if (mInputManager.ButtonSlotMap == null) yield break;

            ETCButton attackBtn = null;
            if (mInputManager.ButtonSlotMap.ContainsKey(1))
                attackBtn = mInputManager.ButtonSlotMap[1];
            if(attackBtn != null)
                attackBtn.transform.localScale = Vector3.zero;
            mInputManager.SetVisible(true, false);

            GameObject joystick = mInputManager.GetJoyStick();
            GameObject fingerMove = LoadUIEffect(joystick, fingerPath);

            var moveInEffect = mDungeonManager.GetGeScene().CreateEffect(12, new Vec3(moveInPostion.x, moveInPostion.z, moveInPostion.y));
            while (FrameSync.instance.bInMoveMode == false)
            {
                yield return Yielders.EndOfFrame;
            }
            _DoStateNewbieGuideFunc("MoveJoyStik Guide1 - move");
            yield return _HideGuideTips(0.0f);
            yield return _GuideWaitForPlayerMoveIn(moveInPostion, moveInRadius);
            yield return _GuideLockPlayerMove(true);
            GameObject.Destroy(fingerMove);
            fingerMove = null;

            _DoStateNewbieGuideFunc("MoveJoyStik Guide1 - moveOver");
            yield return SetKillMask(6330021);
            if (mDungeonManager != null)
            {
                mDungeonManager.GetGeScene().CreateEffect(1030, new Vec3(moveInPostion.x, moveInPostion.z, moveInPostion.y), false, 0, 2.0f);
                mDungeonManager.GetGeScene().DestroyEffect(moveInEffect);
            }
            yield return _HideGuideTips(0.0f);
            yield return UnFreazeMonsters();
            if(mInputManager != null)
                mInputManager.SetVisible(true, true);
            yield return _ShowGuideTips("点击右下角攻击按钮，击败敌人！", 0.0f);
            if (attackBtn != null)
            {
                temp = LoadUIEffect(attackBtn.gameObject, buttonTips);
                yield return _GuideShowSkill(attackBtn.gameObject, 0.3f);
                bool bInput = false;
                InputManager.onSkillCommandCallBack = (id, value) =>
                {
                    bInput = true;
                };
                yield return Yielders.GetWaitForSeconds(0.5f);
                yield return _GuideLockPlayerMove(false);
                while (bInput == false)
                {
                    yield return Yielders.EndOfFrame;
                }
                ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
                _DoStateNewbieGuideFunc("MoveJoyStik Guide1 - FightBegin");
                UnFreazeMonsters();
            }
            yield break;
        }

        GameObject fingerDoubleMove = null;
        private IEnumerator _GuideShowDoubleMove()
        {
            GameObject joystick = mInputManager.GetJoyStick();
            fingerDoubleMove = LoadUIEffect(joystick, fingerDoublePath, false);

            while (FrameSync.instance.bInMoveMode == false)
            {
                yield return Yielders.EndOfFrame;
            }
            yield break;

        }
        private IEnumerator _GuideWaitForPlayerMoveIn(Vector3 position, float raduis)
        {
            BeActor actor = _GuideGetMainPlayer();
            if (actor == null) yield break;
            bool bMoveIn = false;

            while (!bMoveIn)
            {
                Vector3 playerPosition = actor.m_pkGeActor.GetPosition();
                var dis = playerPosition - position;
                bMoveIn = dis.magnitude < raduis;
                yield return Yielders.EndOfFrame;
            }

            yield break;
        }


        private IEnumerator _Guide1MonsterMoveForward(float dis, float time)
        {
            {
                var entityList = mDungeonManager.GetBeScene().GetFullEntities();
                for (int i = 0; i < entityList.Count; ++i)
                {
                    var current = entityList[i] as BeActor;
                    if (current != null && current.IsMonster())
                    {
                        var pos = current.GetPosition();
                        pos.x += VInt.Float2VIntValue(dis);
                        current.buffController.RemoveBuff(31);
                        //current.aiManager.Stop();
                        current.aiManager.ExecuteCommand(new BeAISimpleWalkCommand(current, pos, VInt.Float2VIntValue(0.1f)));
                    }
                }
            }

            yield return Yielders.GetWaitForSeconds(time);

            {
                var entityList = mDungeonManager.GetBeScene().GetFullEntities();
                for (int i = 0; i < entityList.Count; ++i)
                {
                    var current = entityList[i] as BeActor;
                    if (current != null && current.IsMonster())
                    {
                        current.Reset();
                    }
                }
            }
        }

        private IEnumerator _entityMoveForward(BeActor current, float dis, float time)
        {
            if (null != current)
            {
                var pos = current.GetPosition();
                pos.x += VInt.Float2VIntValue(dis);
                current.buffController.RemoveBuff(31);
                //current.aiManager.Stop();
                //current.aiManager.ExecuteCommand(new BeAISimpleWalkCommand(current, pos, 0.1f));
                current.SetController(new BeActorMoveController(pos, 0.1f));
                current.ChangeRunMode(true);
                current.ModifyMoveDirection(false);
            }

            yield return Yielders.GetWaitForSeconds(time);

            if (null != current)
            {
                current.ChangeRunMode(false);
                current.ModifyMoveDirection(false);
                current.Reset();
            }
        }

        private IEnumerator _mainPlayerMoveForward(float dis, float time)
        {
            Log("_mainPlayerMoveForward dis {0} time {1}", dis, time);
            if (mDungeonPlayers == null) yield break;
            var current = mDungeonPlayers.GetMainPlayer().playerActor;
            yield return _entityMoveForward(current, dis, time);
        }

        private IEnumerator _monsterMoveForward(int id, float dis, float time, bool isMonster = true)
        {
            Log("_monsterMoveForward id{0} dis {1} time {2} isMonster {3}", id, dis, time, isMonster);
            BeActor current = _getEntityByID(id, isMonster);
            yield return _entityMoveForward(current, dis, time);
        }

        private IEnumerator _createSummonMonster(int id, Vec3 pos, bool isEnemy, int attack = 200, int level = 20, int hp = 5000, int mp = 5000)
        {
            Log("_createSummonMonster id{0} pos {1} isEnemy {2} attack {3} level{4} hp{5} mp{6}", id, pos, isEnemy, attack, level, hp, mp);
            BeEntity entity = mDungeonManager.GetBeScene().SummonMonster(
                    id,
                    new VInt3(pos),
                    (int)(isEnemy ? ProtoTable.UnitTable.eCamp.C_ENEMY : ProtoTable.UnitTable.eCamp.C_HERO),
                    null,
                    false,
                    level,
                    false
                    );

            if (null != entity)
            {
                entity.GetEntityData().battleData.attack = attack;
                //entity.GetEntityData().battleData.defence = attack;
                entity.GetEntityData().battleData.ciriticalAttack = 0;
                entity.GetEntityData().battleData.ciriticalMagicAttack = 0;
                entity.GetEntityData().battleData.hp = hp;
                entity.GetEntityData().battleData.maxHp = hp;
                entity.GetEntityData().battleData.mp = mp;
                entity.GetEntityData().battleData.maxMp = mp;

                entity.m_pkGeActor.ResetHPBar();
            }

            yield return Yielders.EndOfFrame;
        }
        private IEnumerator _waitForMonsterCastSkill(int id, int iSkillID)
        {
            bool bCast = false;
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;
                if (id == current.GetEntityData().monsterID)
                {
                    current.RegisterEventNew(BeEventType.onCastSkill, args =>
                        {
                            if (args != null)
                            {
                                int skillid = args.m_Int;

                                if (skillid == iSkillID)
                                {
                                    bCast = true;
                                }
                            }
                        }
                    );
                }
            }

            while (bCast == false)
            {
                yield return Yielders.EndOfFrame;
            }

            yield break;
        }

        private IEnumerator _waitActorFirstHurt(int id, bool isMonster)
        {
            BeActor current = _getEntityByID(id, isMonster);
            bool flag = false;
            IBeEventHandle h = null;

            if (null != current)
            {
                h = current.RegisterEventNew(BeEventType.onHurt, (args) =>
                //h = current.RegisterEvent(BeEventType.onHurt, (args) =>
                {
                    flag = true;
                });
            }

            while (!flag)
            {
                yield return Yielders.EndOfFrame;
            }

            if (null != current)
            {
                //current.RemoveEvent(h);
                h.Remove();
                h = null;
            }
        }

        private IEnumerator _setMonsterFaceLeft(int id, bool isRight, bool isMonster)
        {
            BeActor current = _getEntityByID(id, isMonster);
            if (current != null)
                current.SetFace(isRight, true, true);
            yield break;
        }


        private IEnumerator _GuideDoorOpen(TransportDoorType type, bool bOpen)
        {
            if (mDungeonManager == null) yield break;
            mDungeonManager.GetBeScene().SetTransportDoorEnable(type, bOpen);
            yield break;
        }

        private const int kAllUnitID = -1;

        private IEnumerator _Guide1()
        {
            return new ProcessUnit()
                          .Append(_DoStateNewbieGuideBegin())
                          .Append(_DoStateNewbieGuideIter("_Guide1Begin"))
                          .Append(_DoStateNewbieGuideIter("Comic1Begin"))
                          .Append(_waitFrameClose(typeof(NewbieGuideComic1)))
                          .Append(_DoStateNewbieGuideIter("Comic1End"))
                          .Append(FreazeMonsters(-1, true))
                          .Append(_GuideEnableInputManager(false))
                          .Append(_GuideVisiableInputManager(false, false))
                          .Append(_ShowComicFrame(true))
                          .Append(new ProcessUnit()
                              .Append(new ProcessUnit()
                                   .Append(_GuideLockPlayerMove(true))
                                   .Append(_mainPlayerMoveForward(1.4f, 0.5f))
                              .Sequence())
                              .Append(new ProcessUnit()
                                  .Append(_moveCameraTo(2.2f, 0.8f))
                              .Sequence())
                          .Parallel())

                          .Append(_waitForDialog(14001))
                          .Append(_resetCamera(0.5f))
                          //锁定玩家操作
                          .Append(_setPlayerFrameCommand(true))
                          .Append(_waitForTime(1.0f))

                          .Append(FreazeMonsters(-1, false))
                          .Append(_waitForState(BeSceneState.onFight))
                          // /* * * * * * * * * * * * * * * * * * * * * * * * * * 
                          //  *  引导玩家普通攻击怪物，杀死怪物                  
                          //  * * * * * * * * * * * * * * * * * * * * * * * * * * */
                          .Append(_ShowComicFrame(false))
                          .Append(_GuideLockPlayerMove(false))
                          .Append(_GuideShowJoystickAndSkillButton())
                          .Append(_GuideEnableInputManager(true))
                          .Append(_Parallel(0.0f, () =>
                          {
                              if(temp != null)
                                GameObject.Destroy(temp);
                              temp = null;
                          }))
                          //埋点
                          //.Append(_WaitForUnitEvent(6330021,true,BeEventType.onDead))
                          .Append(_waitForState(BeSceneState.onClear))
                          .Append(_GuideLockPlayerMove(true))
                          .Append(_GuideResetMainPlayerState())
                          .Append(_GuideVisiableInputManager(false,false))
                          .Append(_ShowComicFrame(true))
                          .Append(_ShowGuideTipsEx("快向右移动，通关传送门到下一个房间吧", 0.0f, -1, 1.0f, false))
                          //.Append(_GuideVisiableInputManager(true, true))
                          .Append(_waitForTime(2.0f))
                          .Append(_ShowComicFrame(false))
                          .Append(_GuideVisiableInputManager(true,true))
                          .Append(_GuideLockPlayerMove(false))
                          .Append(_GuideShowNextRoom(0.0f))
                          .Append(_GuideDoorOpen(TransportDoorType.Right, true))
                          .Sequence();


        }

        int GuideGetMonsterCount(int id, bool isMonster)
        {
            int iCount = 0;
            var entityList = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < entityList.Count; ++i)
            {
                var current = entityList[i] as BeActor;

                if (isMonster)
                {
                    if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        iCount++;
                    }
                }
                else
                {
                    if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                    {
                        BeEntity player = mDungeonPlayers.GetMainPlayer().playerActor;
                        if (current != player)
                        {
                            iCount++;
                        }
                    }
                }
            }

            return iCount;
        }


        IEnumerator _GuideWaitMonsterNumZero(int id, bool isMonster)
        {
            while (GuideGetMonsterCount(id, isMonster) > 0)
            {
                yield return Yielders.EndOfFrame;
            }

            yield break;
        }

        int mOriginCameraMode = -1;

        IEnumerator _CamreShock(float time, float speed, float x, float y)
        {
            if (mDungeonManager == null) yield break;
            mOriginCameraMode = mDungeonManager.GetGeScene().GetCamera().GetPlayerShockEffectMode();
            mDungeonManager.GetGeScene().GetCamera().PlayShockEffect(time, speed, x, y, 2);
            yield break;
        }

        IEnumerator _resetCameraShockMode()
        {
            if (mDungeonManager == null) yield break;
            if (-1 != mOriginCameraMode)
            {
                mDungeonManager.GetGeScene().GetCamera().SetPlayerShockEffectMode(mOriginCameraMode);
            }
            yield break;
        }

        IEnumerator _ShowComicFrame(bool bShow)
        {
            if (bShow)
            {
                ClientSystemManager.instance.OpenFrame<ComicFrame>(FrameLayer.Middle);
            }
            else
            {
                ClientSystemManager.instance.CloseFrame<ComicFrame>();
            }

            yield break;
        }

        IEnumerator _GuideTimeScale(float scale)
        {
            Time.timeScale = scale;
            yield break;
        }

        IEnumerator _GuideTips(string tips)
        {
            NewbieGuideBattleTipsFrame frame = ClientSystemManager.GetInstance().OpenFrame<NewbieGuideBattleTipsFrame>(FrameLayer.Top) as NewbieGuideBattleTipsFrame;
            frame.SetTipsText(tips);
            yield break;
        }

        IEnumerator _GuideTipsClose()
        {
            ClientSystemManager.GetInstance().CloseFrame<NewbieGuideBattleTipsFrame>();
            yield break;
        }

        IEnumerator _GuideJumpShow(bool bShow)
        {
            if (mInputManager == null) yield break;
            if (bShow)
            {
                mInputManager.ShowJump();
            }
            else
            {
                mInputManager.HiddenJump();
            }

            yield break;
        }


        const int guide2_longren = 6320021;
        const int guide2_huonv = 6300021;
        const int guide2_allmonster = -1;

        const int guide2_majia = 6290021;

        const int newbieSkillID = 5528;
        List<int> newbieSkillList = new List<int>{newbieSkillID};
        private IEnumerator SetMonsterSkillsEnable(int id, List<int> skillIDs, bool flag)
        {
            BeActor current = _getEntityByID(id, true);
           
            if(current != null)
            {
                current.aiManager.SetSkillsEnable(skillIDs,flag);
            }
            yield break;
        }

        private IEnumerator MonsterSkillTalk(string skillSpeech,int id,bool isMonster)
        {
            BeActor current = _getEntityByID(id, isMonster);
            if(current != null)
            {
                current.m_pkGeActor.ShowHeadDialog(skillSpeech, false, false, false, true);
            }

            yield break;
        }

        int chongjibo_skillid = 9999;

        private IEnumerator _Guide2()
        {
            return new ProcessUnit()
                .Append(_Guide2Begin())
                .Append(_waitForTime(0.2f))
                .Append(FreazeMonsters(guide2_allmonster, true))
                // .Append(_GuideShowJoystickAndSkillButton())
                .Append(_mainPlayerMoveForward(4.5f, 1.0f))
                .Append(_GuideEnableInputManager(false))
                .Append(_moveCameraTo(1.5f, 0.5f))
                .Append(_ShowComicFrame(true))
                .Append(_waitForTime(0.5f))
                .Append(_waitForDialog(14011))
                // .Append(SetMonsterSkillsEnable(guide2_huonv,newbieSkillList,false))
                .Append(_ShowComicFrame(false))

                // .Append(SetKillMask(guide2_huonv))
                .Append(_waitForTime(0.5f))
                .Append(_GuideMonsterUseSkill(guide2_huonv, 7255 /* 陨石 */))

                .Append(_waitForTime(1.55f))

                // .Append(_GuideTimeScale(0.3f))
                // .Append(_GuideResetMainPlayerState())

                // .Append(_setPlayerFrameCommand(true))
				.Append(_GuideLockPlayerMove(true))
                .Append(_GuideEnableInputManager(true))
                .Append(_GuideVisiableInputManager(true, true))

                .Append(_GuideJumpback())
                // .Append(_GuideTimeScale(1.0f))

				.Append(_GuideJumpShow(true))
                .Append(_GuideEnableInputManager(false))

                .Append(_waitForTime(0.5f))

                .Append(FreazeMonsters(guide2_allmonster, true))
                .Append(_GuideLockPlayerMove(true))
                .Append(_resetCamera(0.3f))
                
                // .Append(_waitForTime(0.3f))


                // .Append(_GuideTimeScale(1.0f))
                
//系统自动后跳
//莉亚告诉玩家后跳的作用
//给予技能，自由战斗
                .Append(_rightShowGuideTip("熟练的使用后跳会让你更飘逸，再教你一些攻击技能去战斗吧！", 1f))

                .Append(_Guide2AddExSkill())
				.Append(_GuideJumpShow(true))
                // .Append(_rightShowGuideTip("要随时注意脚下，经常使用后跳，躲避怪物技能", 2.0f))
               // .Append(_waitForTime(0.8f))

                .Append(_GuideLockPlayerMove(false))
                .Append(UnFreazeMonsters(guide2_allmonster, true))
                .Append(_GuideEnableInputManager(true))


                // .Append(_GuideVisiableInputManager(true, true))

               // //检测火女血量 50%
               //  .Append(WaitMonsterHPLow(0.5f, guide2_huonv,  true))
               //  .Append(FreazeMonsters(guide2_huonv,true))
                
               //  //释放冲击波
               //  .Append(_GuideMonsterUseSkill(guide2_huonv, chongjibo_skillid))
               //  .Append(MonsterSkillTalk("可恶！！",guide2_huonv,true))
               //  .Append(_waitForTime(1.5f))
               //  .Append(_GuideEnableInputManager(false))
               //  .Append(_GuideLockPlayerMove(true))
               //  .Append(_GuideResetMainPlayerState())
               //  .Append(SetMonsterSkillsEnable(guide2_huonv,newbieSkillList,true))
               //  .Append(_waitForTime(0.5f))

               //  //释放陨石
               //  .Append(_GuideMonsterUseSkill(guide2_huonv, 5528 /* 陨石 */))
               //  .Append(MonsterSkillTalk("让你尝尝我的厉害！",guide2_huonv,true))
               //  .Append(_waitForTime(2.0f))
               //  .Append(_GuideResetMainPlayerState())

               //  .Append(_setPlayerFrameCommand(true))
               //  .Append(_GuideJumpback())
               //  .Append(_GuideJumpShow(true))
               //  .Append(_GuideTimeScale(1.0f))
               //  .Append(_waitForTime(1.0f))
               //  .Append(_ShowComicFrame(true))
               //  .Append(_waitForDialog(14041))

               // .Append(_rightShowGuideTip("要随时注意脚下，经常使用后跳，躲避怪物技能", 2.0f))
               // .Append(_ShowComicFrame(false))
                // .Append(UnFreazeMonsters(guide2_huonv, true))
                // .Append(_GuideVisiableInputManager(true, true))


                // .Append(_HideGuideTips(0))
                // .Append(_waitForTime(0.2f))
                // .Append(UnFreazeMonsters(guide2_allmonster, true))
                // .Append(_GuideJumpShow(true))
                // .Append(_GuideLockPlayerMove(false))
                // .Append(_waitForTime(0.2f))
                // .Append(RemoveMonstersBuff(guide2_huonv, true, 2))
                // .Append(UnFreazeMonsters(guide2_huonv, true))
                // .Append(_WaitForUnitEvent(guide2_huonv, true, BeEventType.onDead))
                // .Append(_waitForTime(1.0f))
                // .Append(DeadMonster(guide2_majia, true))
                // .Append(_GuideLockPlayerMove(true))
                .Append(_GuideResetMainPlayerState())
                .Append(_waitForState(BeSceneState.onClear))
                .Append(_ShowGuideTipsEx("继续跑吧，追兵好像越来越近了！", 0.0f, -1, 1.5f, false))
                .Append(_waitForTime(0.5f))
                .Append(_GuideLockPlayerMove(false))
                .Append(_GuideDoorOpen(TransportDoorType.Right, true))
                .Append(_GuideShowNextRoom(0.0f))
                .Sequence();
        }

        private IEnumerator Guide3Begin()
        {

            //  .Append(_createSummonMonster(apc1, birthPosition1, false))
            yield return _waitForState(BeSceneState.onReady);
            _DoStateNewbieGuideFunc("Guide3Begin");
            if(fingerDoubleMove != null)
                GameObject.Destroy(fingerDoubleMove);
            fingerDoubleMove = null;
            if (mDungeonManager == null) yield break;
            mDungeonManager.GetBeScene().SetTraceDoor(false);
            if(mInputManager != null)
                mInputManager.SetVisible(false);
            yield return FreazeMonsters(-1, true);
            ClientSystemManager.instance.CloseFrame<NewbieGuideNextRoom>();
            yield return _HideGuideTips(0.0f);
            yield break;
        }
        private IEnumerator _Guide3()
        {

            return new ProcessUnit()
            .Append(Guide3Begin())

            .Append(_mainPlayerMoveForward(2.0f, 1.0f))

            .Append(_HideGuideTips(0))
            .Append(_Guide3AddExSkill(0))
            .Append(_GuideJumpShow(true))
            .Append(_ShowComicFrame(true))

            .Append(_GuideEnableInputManager(false))
            .Append(_GuideResetMainPlayerState())

            //.Append (_waitForTime (0.2f))
            .Append(_CamreShock(1.5f, 100, 0.03f, 0.01f))
            .Append(_waitForTime(1.0f))
            .Append(_resetCameraShockMode())
            
            .Append(_moveCameraTo(4.5f, 1.5f))
            .Append(_moveCameraTo(-2.5f, 0.7f))



            .Append(_mainPlayerMoveForward(2f, 1.0f))

            //给予玩家技能

            .Append(_waitForDialog(14021))

           

            .Append(_waitForTime(0.5f))
		   
		    //.Append(_rightShowGuideTip("让敌人尝尝新技能的滋味吧！", 1.5f))
            //.Append (_rightShowGuideTip ("为了自由,让敌人尝尝新技能吧!!", 1.7f))
            // .Append(_GuideTips("为了自由,让敌人尝尝新技能吧!!!!"))

            .Append(_ShowComicFrame(false))
            .Append(_resetCamera(0.5f))

            .Append(_GuideVisiableInputManager(true, true))

            .Append(_HideGuideTips(0))
            .Append(_Guide3AddExSkill(1))

            .Append(_waitForTime(0.2f))
			.Append(_ShowGuideTipsEx("让敌人尝尝新技能的滋味吧！", 0.0f, -1, 1.5f, false))
            .Append(_GuideEnableInputManager(true))
            .Append(UnFreazeMonsters(-1, true))

            // .Append(UnFreazeMonsters(9000031))
            .Append(_waitForTime(0.5f))

             //.Append(_GuideUseSkill(8 , 3f))




             //判断开门
             .Append(_waitForState(BeSceneState.onFinish))
             .Append(_CamreShock(1.5f, 100, 0.03f, 0.01f))
            .Append(_waitForTime(1.5f))
            .Append(_resetCameraShockMode())




                //              .Append(SetMonsterBlock(8890021, true,true))
                //              .Append(SetMonsterBlock(8980031, true,true))
                //              .Append(SetMonsterBlock(6290021, true,true))

                // .Append (
                //      new ProcessUnit ()

                //          .Append (new ProcessUnit ()
                //              .Append (_monsterMoveForward (8890021, 4f, 2.0f, true))
                //              .Sequence ())

                //          .Append (new ProcessUnit ()
                //              .Append (_monsterMoveForward (8980031, 4f, 2.0f, true))
                //              .Sequence ())

                //          .Append (new ProcessUnit ()
                //              .Append (_monsterMoveForward (6290021, 4f, 2.0f, true))
                //              .Sequence ())

                //      .Parallel ())

                //             .Append (_waitForTime (1.0f))


                // .Append(_mainPlayerMoveForward(7f, 2f))


                // .Append(_mainPlayerMoveForward(20f, 3.5f))

                // .Append(_playEffectInScene("Effects/Scene_effects/Xinshou/Eff_yingdao_dimian", 2.0f, effectPostion))

                /* * * * * * * * * * * * * * * * * * * * * * * * * * 
                 * 传送进入room4，在黑屏时候切漫画               
                 * * * * * * * * * * * * * * * * * * * * * * * * * */
                .Append(_ShowComicFrame(true))
                .Append(_waitForDialog(14031))
                .Append(_mainPlayerMoveForward(7f, 1.5f))
                .Append(_ShowComicFrame(false))

                // .Append(_waitFrameClose(typeof(NewbieGuideFinalShow)))

                .Append(_Parallel(0.0f, () => {/*GameStatisticManager.GetInstance().DoStatistic("[GuideBattle]Comic02Start");*/}))

                //.Append(_DoStateNewbieGuideIter("Comic2Begin"))
                //.Append(_waitFrameClose(typeof(NewbieGuideComic2)))
                //.Append(_DoStateNewbieGuideIter("Comic2End"))
                .Append(_Parallel(0.0f, () => {/*GameStatisticManager.GetInstance().DoStatistic("[GuideBattle]Comic02End");*/}))

                .Append(_returnToTown())
                .Sequence();

        }
    }
}
