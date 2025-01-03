using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class TrainingPVEBattle : PVEBattle
    {
        public TrainingPVEBattle(BattleType type, eDungeonMode mode, int id) : base(type, eDungeonMode.LocalFrame, id)
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
                var equipsInfo = BattlePlayer.GetEquips(current,false);
                var sideEquipInfo = BattlePlayer.GetSideEquips(current,false);
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
#endif
        }

        #region 玩家修改自身属性相关 目前是单人模式 所有没有同步数据 如果后期要组队 需要使用帧同步同步消息
        private TrainingPveFrame mainFrame = null;
        private int[] addBuffIdArr = new int[4] { 0, 0, 0, 0 };     //记录人物属性的四个Buff
        private int monsterCount = 0;
        private List<BeActor> monsterList = new List<BeActor>();    //记录召唤的所有怪物

        //初始化
        public void Init()
        {
            mainFrame = ClientSystemManager.instance.OpenFrame<TrainingPveFrame>(FrameLayer.Middle) as TrainingPveFrame;
            if (mainFrame != null)
            {
                mainFrame.InitUserData(this);
            }
        }

        protected override void _onUpdate(int delta)
        {
            base._onUpdate(delta);
            CheckMonsterExist();
        }

        //重置自己的技能 道具 宠物的技能 道具的CD时间
        public void ResetAllCD()
        {
            BeActor actor = GetMainActor();
            if (actor == null)
                return;
            actor.ResetSkillCoolDown();
            BeActor pet = actor.pet;
            if (pet != null)
                pet.ResetSkillCoolDown();
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
            if (battleUI != null)
            {
                battleUI.ResetChangeEquipCD();
                battleUI.ResetChangeWeaponCD();
            }
            actor.TriggerEventNew(BeEventType.onTrainingPveResetSkillCD, new EventParam());
            //添加一个回蓝BUFF
            actor.buffController.TryAddBuff(811042,1);
        }

        //召唤怪物
        public BeActor SummonMonster()
        {
#if !LOGIC_SERVER
            if (GetMonsterCount() >= 5)
            {
                SystemNotifyManager.SysNotifyTextAnimation("召唤数量已达上限，无法召唤", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                return null;
            }
            BeActor actor = GetMainActor();
            if (actor == null)
                return null;
            if (mainFrame == null)
                return null;
            TrainingPveMonsterData data = mainFrame.GetSummonMonsterData();
            int monsterId = data.monsterId;
            BeScene scene = actor.CurrentBeScene;
            if (scene == null)
                return null;
            var monster = scene.SummonMonster(data.monsterId + data.level * 100, actor.GetPosition(), 1);
            if (monster == null)
                return null;
            monster.m_pkGeActor.goFootInfo.CustomActive(false);
            if (data.isBati)
                monster.buffController.TryAddBuff(1, int.MaxValue);
            if (data.abnormalId != 0)
            {
                monster.delayCaller.DelayCall(1050, () => 
                {
                    if(monster != null && !monster.IsDead())
                        monster.buffController.TryAddBuff(data.abnormalId, int.MaxValue, 60);
                });
            }
            monsterList.Add(monster);
            return monster;
#else
            return null;
#endif
        }

        //实时改变玩家属性
        public void ChangeMainActorBuff(int index, int attType)
        {
#if !LOGIC_SERVER
            BeActor actor = GetMainActor();
            if (actor == null)
                return;
            if (mainFrame == null)
                return;
            int addBuffId = 0;
            if(addBuffIdArr[attType] != 0)
                actor.buffController.RemoveBuff(addBuffIdArr[attType]);
            if (index == 0)
                return;
            switch(attType)
            {
                case 0:
                    addBuffId = mainFrame.critBuffIdArr[index-1];
                    break;
                case 1:
                    addBuffId = mainFrame.staAddBuffIdArr[index - 1];
                    break;
                case 2:
                    addBuffId = mainFrame.attAddBuffIdArr[index - 1];
                    break;
                case 3:
                    addBuffId = mainFrame.speedAddBuffIdArr[index - 1];
                    break;
            } 
            if(addBuffId != 0)
            {
                actor.buffController.TryAddBuff(addBuffId, int.MaxValue);
                addBuffIdArr[attType] = addBuffId;
            }
#endif
        }

        //修改玩家命中概率
        public void ChangeActorHitRate(bool value)
        {
#if !LOGIC_SERVER
            BeActor actor = GetMainActor();
            if (actor == null)
                return;
            if (value)
                actor.buffController.TryAddBuff(811041, int.MaxValue);
            else
                actor.buffController.RemoveBuff(811041);
#endif
        }

        //修改玩家是否绝对破招
        public void ChangeActorBroken(bool value)
        {
#if !LOGIC_SERVER
            BeActor actor = GetMainActor();
            if (actor == null)
                return;
            actor.absoluteBroken = value;
#endif
        }

        //获取主玩家 不能用于验证服务器
        private BeActor GetMainActor()
        {
#if !LOGIC_SERVER
            if (BattleMain.instance == null
                || BattleMain.instance.GetPlayerManager() == null
                || BattleMain.instance.GetPlayerManager().GetMainPlayer() == null)
                return null; 
            BeActor actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
            return actor;
#else
            return null;
#endif
        }

        //回到城镇
        public void ReturnToTown()
        {
#if !LOGIC_SERVER
            if (RecordServer.GetInstance().IsReplayRecord())
                RecordServer.GetInstance().EndRecord("DoBack");
            BeUtility.ResetCamera();
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
#endif
        }

        //获取怪物数量
        private int GetMonsterCount()
        {
            int count = 0;
            if (monsterList == null)
                return count;
            for(int i = 0; i < monsterList.Count; i++)
            {
                if (monsterList[i] == null || monsterList[i].IsDead())
                    continue;
                count++;
            }
            return count;
        }

        //清除所有怪物
        public void DeleteAllMonster()
        {
            if (monsterList == null)
                return;
            for(int i = 0; i < monsterList.Count; i++)
            {
                if (monsterList[i] == null || monsterList[i].IsDead())
                    continue;
                monsterList[i].DoDead();
            }

            PauseTiming();
        }

        /// <summary>
        /// 检测怪物存活
        /// </summary>
        private void CheckMonsterExist()
        {
            if (monsterList == null)
                return;
            for(int i=0;i< monsterList.Count; i++)
            {
                if (!monsterList[i].IsDead())
                    return;
            }

            PauseTiming();
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        private void PauseTiming()
        {
            BeActor actor = GetMainActor();
            if (actor == null)
                return;
            if (actor.skillDamageManager == null)
                return;
            actor.skillDamageManager.SetTimingFlag(false);
        }

        #endregion
    }
}