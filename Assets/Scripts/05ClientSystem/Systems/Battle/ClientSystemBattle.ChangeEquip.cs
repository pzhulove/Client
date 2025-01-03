//using DG.Tweening;

//namespace GameClient
//{
//    /// <summary>
//    /// 切换装备用
//    /// </summary>
//    public partial class ClientSystemBattle
//    {
//        protected string[] m_IconPath = new string[] { "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_Zhandou_Huangzhuang_Text_03",
//            "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_Zhandou_Huangzhuang_Text_01" };

//        /// <summary>
//        /// 初始化切换装备
//        /// </summary>
//        protected void InitSwitchEquip()
//        {
//            if (!CanSwitchEquip())
//                return;
//            mSwitchEquips.gameObject.CustomActive(true);
//        }

//        /// <summary>
//        /// 初始化切换装备按钮图标
//        /// </summary>
//        public void InitSwitchEquipIcon(int index)
//        {
//            ChangeEquipIcon(index);
//        }

//        /// <summary>
//        /// 隐藏切换装备按钮
//        /// </summary>
//        public void SetSwitchEquipBtnState(bool flag)
//        {
//            if (!CanSwitchEquip())
//                return;
//            if (mSwitchEquips == null)
//                return;
//            mSwitchEquips.gameObject.CustomActive(flag);
//        }

//        /// <summary>
//        /// 切换装备
//        /// </summary>
//        private void _onSwitchEquipsBtnButtonClick()
//        {
//            BeActor actor = GetLocalActor();
//            if (actor == null)
//                return;
//            if (actor.IsCastingSkill())
//            {
//                SystemNotifyManager.SysNotifyFloatingEffect("技能施放动作期间无法切换装备");
//                return;
//            }
//            mSwitchEquipsBtn.transform.parent.DOScale(1.3f, 0.1f).OnComplete(() =>
//            {
//                mSwitchEquipsBtn.transform.parent.DOScale(1.2f, 0.1f);
//            });

//            if (mSwitchEquipCD != null && mSwitchEquipCD.surplusTime > 0)
//            {
//                SystemNotifyManager.SysNotifyFloatingEffect("切换装备冷却中");
//                return;
//            }

//            if (actor.isSpecialMonster)
//            {
//                SystemNotifyManager.SysNotifyFloatingEffect("当前状态不可切换");
//                return;
//            }

//            if (!CanSwitchEquip())
//                return;

//            //切换装备CD
//            StartSwitchEquipCD();

//            //目前只有两个方案进行切换
//            int newEquipIndex = actor.GetCurrentSchemeIndex() == 0 ? 1 : 0;

//            ChangeEquipIcon(newEquipIndex);

//            ChangeWeaponCommand cmd = new ChangeWeaponCommand();
//            cmd.equipIndex = newEquipIndex + 1;
//            FrameSync.instance.FireFrameCommand(cmd);
//        }

//        /// <summary>
//        /// 切换装备按钮Icon改变
//        /// </summary>
//        public void ChangeEquipIcon(int pathIndex)
//        {
//            if (mSwitchEquipIcon == null)
//                return;
//            string path = m_IconPath[pathIndex];
//            ETCImageLoader.LoadSprite(ref mSwitchEquipIcon, path);
//        }

//        /// <summary>
//        /// 开始切换装备CD
//        /// </summary>
//        protected void StartSwitchEquipCD()
//        {
//            BeActor actor = GetLocalActor();
//            if (actor == null)
//                return;
//            float surpusTime = 3;
//            float reduceCD = 0;
//            Mechanism81 mechanism = actor.GetMechanism(5072) as Mechanism81;
//            if (mechanism != null)
//                reduceCD = mechanism.changeWeaponCD.f;

//            Mechanism81 weaponMechanism = actor.GetMechanism(158) as Mechanism81;
//            if (weaponMechanism != null)
//                reduceCD += weaponMechanism.changeWeaponCD.f;

//            if (mechanism != null && mSwitchEquipCD != null)
//            {
//                mSwitchEquipCD.StartCD(surpusTime * (1 + reduceCD));
//            }
//            else
//            {
//                mSwitchEquipCD.StartCD(surpusTime);
//            }
//        }

//        //重置切换武器CD
//        public void ResetChangeEquipCD()
//        {
//            mSwitchEquipCD.ResetCD();
//        }

//        /// <summary>
//        /// 设置本地玩家
//        /// </summary>
//        protected BeActor GetLocalActor()
//        {
//            if (BattleMain.instance == null)
//                return null;
//            if (BattleMain.instance.GetLocalPlayer() == null)
//                return null;
//            return BattleMain.instance.GetLocalPlayer().playerActor;
//        }

//        /// <summary>
//        /// 是否支持装备切换
//        /// </summary>
//        protected bool CanSwitchEquip()
//        {
//            if (mSwitchEquips == null || ReplayServer.GetInstance().IsReplay())
//                return false;
//            var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//            if (player != null && !player.isFighting && BattleMain.battleType == BattleType.PVP3V3Battle)
//                return false;
//            //Pvp模式不支持切换装备
//            if (BattleMain.IsModePvP(BattleMain.battleType))
//                return false;
//            //技能连招不支持装备切换
//            if (BattleMain.battleType == BattleType.TrainingSkillCombo)
//                return false;
//            BeActor actor = GetLocalActor();
//            if (actor == null)
//                return false;
//            int schemeCount = actor.GetSchemeCount();
//            if (schemeCount > 1)
//                return true;
//            return false;
//        }
//    }
//}