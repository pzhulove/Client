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
    public class DemoBattle : BaseBattle
    {
        public DemoBattle(BattleType type, eDungeonMode mode, int id) : base(type, eDungeonMode.LocalFrame, id)
        {
            NeedPreload = false;
            NeedCreateInput = false;
            NeedPlaySound = false;
            NeedShowHitFloat = false;
            NeedSendMsg = false;
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
					//petData = PlayerBaseData.GetInstance().GetPetData(true);
                    actor = main.CreateCharacter(
                        true,
                        current.occupation,
                        60,//current.level,
                        (int)ProtoTable.UnitTable.eCamp.C_HERO + current.seat,
						new Dictionary<int, int>(),//SkillDataManager.GetInstance().GetSkillInfo(BattleMain.IsModePvP(BattleMain.battleType)),
                        null,//PlayerBaseData.GetInstance().GetEquipedEquipments(),
                        null, 0, "",
                        strengthenLevel,
                        null,//PlayerBaseData.GetInstance().GetRankBuff(),
                        null,//petData,
                        null,//PlayerBaseData.GetInstance().GetSideEquipments(),
                        null,
                        false,
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

				}

                actor.InitChangeEquipData(current.equips, current.equipScheme);
                actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(current);

				players[i].playerActor = actor;

				//临时代码 设置位置，朝向，移除时间
				actor.SetPosition(VInt3.zero, true);
				actor.SetFace(i != 0);
				actor.m_iRemoveTime = 1000 * 120;

				if (current.accid == ClientApplication.playerinfo.accid)
				{
					actor.isLocalActor = true;
                    // actor.m_pkGeActor.AddTittleComponent(PlayerBaseData.GetInstance().GetTitleID(), current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_PLAYER);
                    // actor.m_pkGeActor.CreateInfoBar(current.name, PlayerInfoColor.PK_PLAYER, current.level);
					// actor.m_pkGeActor.CreatePKHPBar(CPKHPBar.PKBarType.Left, current.name, PlayerInfoColor.PK_PLAYER);

				}
				else
                {
                    // actor.m_pkGeActor.AddTittleComponent(0, current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_OTHER_PLAYER);
                    // actor.m_pkGeActor.CreateInfoBar(current.name, PlayerInfoColor.PK_OTHER_PLAYER, current.level);
					// actor.m_pkGeActor.CreatePKHPBar(CPKHPBar.PKBarType.Right, current.name, PlayerInfoColor.PK_OTHER_PLAYER);
					// actor.m_pkGeActor.CreateFootIndicator();

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
                actor.InitAutoFight();
                actor.pauseAI = false;
                actor.SetDefaultAIRun(false);
                actor.skillController.SetNeedCost(false);

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
    }
}
