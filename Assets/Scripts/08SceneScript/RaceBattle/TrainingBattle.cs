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
    public class TrainingBatte : BaseBattle
    {
        public TrainingBatte(BattleType type, eDungeonMode mode, int id) : base(type, eDungeonMode.LocalFrame, 0)
        {
            // TODO 这里做一个很不好的代码，记得改掉
            if (Global.Settings.startSystem == GameClient.EClientSystem.Battle)
            {
                NeedPreload = false;
                mDungeonManager.GetDungeonDataManager().asset.GetAreaConnectList(0).SetSceneAreaPath(Global.Settings.scenePath);
                mDungeonManager.GetDungeonDataManager().asset.GetAreaConnectList(0).SetSceneData(DungeonUtility.LoadSceneData(Global.Settings.scenePath));
            }
        }

#if UNITY_EDITOR
        protected override void _createDoors()
        {
            if (Global.Settings.startSystem != GameClient.EClientSystem.Battle)
            {
                return;
            }

            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var doors = mDungeonData.CurrentDoors(true);

            var chanceDoorType = mBeScene.GetChanceDoorType();

            for (int i = 0; i < doors.Count; ++i)
            {
                if (null != doors[i].door)
                {
                    /*					if (!BattleMain.instance.HasBattleFlag(BattleFlag.NPC_PROTECT) || 
                                            (BattleMain.instance.HasBattleFlag(BattleFlag.NPC_PROTECT) && doors[i].doorType == chanceDoorType))*/
                    mBeScene.AddTransportDoor(doors[i].door.GetRegionInfo(), null, null, doors[i].isconnectwithboss, doors[i].isvisited, doors[i].doorType, doors[i].isEggDoor, doors[i].materialPath);
                }
            }
        }
#endif

        protected override void _createPlayers()
        {
            var main          = mDungeonManager.GetBeScene();
            var current       = mDungeonPlayers.GetMainPlayer().playerInfo;

            var skillInfo     = BattlePlayer.GetSkillInfo(current);
            var equipsInfo    = BattlePlayer.GetEquips(current,true);
            var sideEquipInfo = BattlePlayer.GetSideEquips(current,true);

            if (Global.Settings.startSystem == GameClient.EClientSystem.Battle)
            {
                skillInfo = TableManager.GetInstance().GetSkillInfoByPid(current.occupation);
            }
            else
            {
                skillInfo = SkillDataManager.GetInstance().GetSkillInfo(BattleMain.IsModePvP(BattleMain.battleType));
            }
            var avatarData = BattlePlayer.GetAvatar(current);
            bool isShowFashionWeapon = current.avatar.isShoWeapon == 1 ? true : false;
            bool isAIRobot = current.robotAIType > 0 ? true : false;

            var actor = main.CreateCharacter(
					true,
                    current.occupation,
                    current.level,
                    (int)ProtoTable.UnitTable.eCamp.C_HERO + current.seat,
                    skillInfo,
                    equipsInfo,
                    null, 0, "",
                    0,
                    null,
                    null,
                    sideEquipInfo,
                    avatarData,
                    isShowFashionWeapon,
                    isAIRobot
                    );

            actor.InitChangeEquipData(current.equips, current.equipScheme);
            actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(current);
            actor.SetPosition(mDungeonManager.GetDungeonDataManager().GetBirthPosition());

            
            actor.isLocalActor = true;
			current.name = "测试玩家";
            actor.m_pkGeActor.AddTittleComponent(PlayerBaseData.GetInstance().GetTitleID(), current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_PLAYER);
            actor.m_pkGeActor.CreateInfoBar(current.name , PlayerInfoColor.PK_PLAYER, current.level);
			
            actor.isMainActor = true;
			actor.UseProtect();
            actor.UseActorData();

            if (null != actor.m_pkGeActor)
            {
                actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
            }

            mDungeonPlayers.GetMainPlayer().playerActor = actor;
#if !LOGIC_SERVER
            mDungeonManager.GetGeScene().AttachCameraTo(actor.m_pkGeActor);
#endif
            //main.playerActor = actor;
        }
    }
}
