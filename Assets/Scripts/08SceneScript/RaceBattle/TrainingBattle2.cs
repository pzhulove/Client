using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using DG.Tweening;

namespace GameClient
{
    public class TrainingBatte2 : BaseBattle
    {
        public TrainingBatte2(BattleType type, eDungeonMode mode, int id) : base(type, eDungeonMode.LocalFrame, id)
        {
        }

		void CreateMainPlayer()
		{
			
		}

		public void RecreatePlayers()
		{
			var main = mDungeonManager.GetBeScene();
            bool flag = BeClientSwitch.FunctionIsOpen(ClientSwitchType.TrainRemoveMechanism);
            main.ClearAllEntity(flag);

            ClientSystemManager.GetInstance().delayCaller.DelayCall(100, ()=>{
				_createPlayers();

				_reLoadSkillButton();
                _ResetSyncPlayerData();
            });
		}

        protected override void _createPlayers()
        {
			var main = mDungeonManager.GetBeScene();
			var players = mDungeonPlayers.GetAllPlayers();
			for (int i = 0; i < players.Count; ++i)
			{
				var currentBattlePlayer = players[i];
				var current = players[i].playerInfo;

				var skillInfo     = BattlePlayer.GetSkillInfo(current);
				var equipsInfo    = BattlePlayer.GetEquips(current,true);
                var sideEquipInfo    = BattlePlayer.GetSideEquips(current,true);
				var strengthenLevel = 0;
				if (i == 0)
					strengthenLevel = PlayerBaseData.GetInstance().GetWeaponStrengthenLevel();
				else 
					strengthenLevel = BattlePlayer.GetWeaponStrengthenLevel(current);

                var avatorData =  new Dictionary<int, string>();
                if (i == 0)
                    avatorData = PlayerBaseData.GetInstance().GetAvatar();
                else
                    avatorData = BattlePlayer.GetAvatar(current);

                bool isShowFashionWeapon = false;
                if (i == 0)
                    isShowFashionWeapon = PlayerBaseData.GetInstance().avatar.isShoWeapon == 1 ? true : false;
                else
                    isShowFashionWeapon = current.avatar.isShoWeapon == 1 ? true : false;

                BeActor actor = null;
				PetData petData = null;
				//我方
				if (i == 0)
				{
					petData = PlayerBaseData.GetInstance().GetPetData(true);
                    actor = main.CreateCharacter(
                        true,
                        current.occupation,
                        current.level,
                        (int)ProtoTable.UnitTable.eCamp.C_HERO + current.seat,
						SkillDataManager.GetInstance().GetSkillInfo(BattleMain.IsModePvP(BattleMain.battleType)),
                        PlayerBaseData.GetInstance().GetEquipedEquipments(),
                        null, 0, "",
                        strengthenLevel,
                        PlayerBaseData.GetInstance().GetRankBuff(),
                        petData,
                        PlayerBaseData.GetInstance().GetSideEquipments(),
                        avatorData,
                        isShowFashionWeapon,
                        false
                    );
						
				}
				//敌方
				else {
					actor = main.CreateCharacter(
						false,
						current.occupation,
						current.level,
						(int)ProtoTable.UnitTable.eCamp.C_HERO + current.seat,
						skillInfo,
						equipsInfo,
						null, 0, "",
						strengthenLevel,
                        null,
                        null,
                        sideEquipInfo,
                        avatorData,
                        isShowFashionWeapon,
                        true
                    );


                    if (current.robotAIType > 0)
					{
						//BattlePlayer.DebugPrint(current);
						InitAIForRobot(actor, (int)current.robotAIType, current.robotHard);
					}
				}

                actor.InitChangeEquipData(current.equips, current.equipScheme);
                actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(current);

				players[i].playerActor = actor;

				//临时代码 设置位置，朝向，移除时间
				VInt3 pos = new VInt3(i == 0 ? -3 * (int)IntMath.kIntDen: 3* (int)IntMath.kIntDen, 0, 0);
				actor.SetPosition(pos, true);
				actor.SetFace(i != 0);
				actor.m_iRemoveTime = 1000 * 120;

				if (current.accid == ClientApplication.playerinfo.accid)
				{
					actor.isLocalActor = true;
                    actor.m_pkGeActor.AddTittleComponent(PlayerBaseData.GetInstance().GetTitleID(), current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_PLAYER);
                    actor.m_pkGeActor.CreateInfoBar(current.name, PlayerInfoColor.PK_PLAYER, current.level);
					actor.m_pkGeActor.CreatePKHPBar(CPKHPBar.PKBarType.Left, current.name, PlayerInfoColor.PK_PLAYER);

				}
				else
                {
                    actor.m_pkGeActor.AddTittleComponent(0, current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_OTHER_PLAYER);
                    actor.m_pkGeActor.CreateInfoBar(current.name, PlayerInfoColor.PK_OTHER_PLAYER, current.level);
					actor.m_pkGeActor.CreatePKHPBar(CPKHPBar.PKBarType.Right, current.name, PlayerInfoColor.PK_OTHER_PLAYER);
					actor.m_pkGeActor.CreateFootIndicator();

					actor.RegisterEventNew(BeEventType.onHPChange, (args)=>{
						if (actor.GetEntityData().GetHP() <= 100)
						{
							actor.SetIsDead(false);
							actor.GetEntityData().SetHP(actor.GetEntityData().GetMaxHP());
							actor.m_pkGeActor.ResetHPBar();
						}
					});
				}

				actor.GetEntityData().AdjustHPForPvP(current.level, current.level, 1, 1);

                if (null != actor.m_pkGeActor)
                {
                    actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
                }

                actor.isMainActor = true;
				actor.UseProtect();
                actor.UseActorData();
                actor.UseAdjustBalance();

				if (current.accid == ClientApplication.playerinfo.accid)
				{
					if (petData != null)
						actor.SetPetData (petData);
                    actor.CreateFollowMonster();
                }
			}

#if !LOGIC_SERVER
			mDungeonManager.GetGeScene().AttachCameraTo(mDungeonPlayers.GetMainPlayer().playerActor.m_pkGeActor);
#endif
        }


		void InitAIForRobot(BeActor actor, int aiLevel, int matchNum = 0)
		{
            var tables = TableManager.GetInstance().GetTable<ProtoTable.AIConfigTable>();
            ProtoTable.AIConfigTable aiData = null;
            foreach (var item in tables)
            {
                var data = item.Value as ProtoTable.AIConfigTable;
                if (data.JobID == actor.GetEntityData().professtion && data.AIType == aiLevel)
                {
                    aiData = data;
                }
            }
            actor.isPkRobot = true;
            if (aiData != null)
            {
                actor.InitAutoFight(
                    aiData.AIActionPath,
                    aiData.AIDestinationSelectPath,
                    aiData.AIEventPath,
                    aiData.AIAttackDelay,
                    aiData.AIDestinationChangeTerm,
                    aiData.AIThinkTargetTerm,
                    aiData.AIKeepDistance
                );
				actor.delayCaller.DelayCall(2000, ()=>{
					actor.pauseAI = false;
				});
            }
            else
            {
                Logger.LogErrorFormat("找不到机器人AI难度:{0}", aiLevel);
            }

            
		}

        //练习模式下重置双击跑 前冲攻击等帧同步设置
        void _ResetSyncPlayerData()
        {
            if (!BattleMain.IsModeTrain(BattleMain.battleType))
                return;
            var players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                BeActor actor = players[i].playerActor;
                if (!actor.isLocalActor)
                    continue;
                actor.hasDoublePress = Global.Settings.hasDoubleRun;
                actor.hasRunAttackConfig = SettingManager.GetInstance().GetRunAttackMode() == InputManager.RunAttackMode.QTE;
                string key = BattleMain.IsModePvP(GetBattleType()) ? SettingManager.STR_CHASER_PVP: SettingManager.STR_CHASER_PVE;
                byte chaserSetting = (byte) SettingManager.GetInstance().GetChaserSetting(key);
                actor.chaserSwitch = chaserSetting;
            }
        }
    }
}
