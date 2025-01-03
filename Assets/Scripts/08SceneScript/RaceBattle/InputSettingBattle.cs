using UnityEngine;
using System.Collections.Generic;
using ProtoTable;

namespace GameClient
{
    public class InputSettingBattle : PVEBattle
    {
        InputSettingBattleFrame mainFrame;

        public InputSettingBattle(BattleType type, eDungeonMode mode, int id) : base(type, eDungeonMode.LocalFrame, id)
        {
            
        }

        protected override void _createPlayers()
        {
            var main = mDungeonManager.GetBeScene();
            var players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                var currentBattlePlayer = players[i];
                var current = players[i].playerInfo;

                var skillInfo = BattlePlayer.GetSkillInfo(current);
                var equipsInfo = BattlePlayer.GetEquips(current, false);
                var sideEquipInfo = BattlePlayer.GetSideEquips(current, false);
                var strengthenLevel = 0;
                if (i == 0)
                    strengthenLevel = PlayerBaseData.GetInstance().GetWeaponStrengthenLevel();
                else
                    strengthenLevel = BattlePlayer.GetWeaponStrengthenLevel(current);

                var avatorData = new Dictionary<int, string>();
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
                    petData = PlayerBaseData.GetInstance().GetPetData(false);
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
                    InitAutoFight(actor);
                    //初始化伤害统计数据
                    actor.skillDamageManager.InitData(main);

                }

                actor.InitTrainingPveBattleData();
                actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(current);

                players[i].playerActor = actor;

                //临时代码 设置位置，朝向，移除时间
                VInt3 pos = new VInt3(i == 0 ? -3 * (int)IntMath.kIntDen : 3 * (int)IntMath.kIntDen, 0, 0);
                actor.SetPosition(pos, true);
                actor.SetFace(i != 0);
                actor.m_iRemoveTime = 1000 * 120;

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
                actor.UseActorData();

                if (current.accid == ClientApplication.playerinfo.accid)
                {
                    if (petData != null)
                        actor.SetPetData(petData);
                    actor.CreateFollowMonster();
                }
            }

#if !LOGIC_SERVER
            mDungeonManager.GetGeScene().AttachCameraTo(mDungeonPlayers.GetMainPlayer().playerActor.m_pkGeActor);

            mainFrame = ClientSystemManager.instance.OpenFrame<InputSettingBattleFrame>(FrameLayer.Middle) as InputSettingBattleFrame;
#endif
        }

    }
}