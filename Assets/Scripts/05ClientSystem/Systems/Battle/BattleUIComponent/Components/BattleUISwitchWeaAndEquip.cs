using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 切换武器和装备UI组件
    /// </summary>
    public class BattleUISwitchWeaAndEquip : BattleUIBase
    {
        public BattleUISwitchWeaAndEquip() : base() { }

        #region ExtraUIBind
        private GameObject mSwitchWeapon = null;
        private Button mBtnChangeWeapon = null;
        private Image mWeaponBackImage = null;
        private Image mWeaponIconImage = null;
        private CDCoolDowm mCdCoolDowm = null;
        private GameObject mSwitchEquips = null;
        private Button mSwitchEquipsBtn = null;
        private CDCoolDowm mSwitchEquipCD = null;
        private Image mSwitchEquipIcon = null;
        private GameObject mDragObj = null;

        protected override void _bindExUI()
        {
            mSwitchWeapon = mBind.GetGameObject("SwitchWeapon");
            mBtnChangeWeapon = mBind.GetCom<Button>("BtnChangeWeapon");
            mBtnChangeWeapon.onClick.AddListener(_onBtnChangeWeaponButtonClick);
            mWeaponBackImage = mBind.GetCom<Image>("WeaponBackImage");
            mWeaponIconImage = mBind.GetCom<Image>("WeaponIconImage");
            mCdCoolDowm = mBind.GetCom<CDCoolDowm>("CdCoolDowm");
            mSwitchEquips = mBind.GetGameObject("SwitchEquips");
            mSwitchEquipsBtn = mBind.GetCom<Button>("SwitchEquipsBtn");
            mSwitchEquipsBtn.onClick.AddListener(_onSwitchEquipsBtnButtonClick);
            mSwitchEquipCD = mBind.GetCom<CDCoolDowm>("SwitchEquipCD");
            mSwitchEquipIcon = mBind.GetCom<Image>("SwitchEquipIcon");
            mDragObj = mBind.GetGameObject("DragObj");
        }

        protected override void _unbindExUI()
        {
            mSwitchWeapon = null;
            mBtnChangeWeapon.onClick.RemoveListener(_onBtnChangeWeaponButtonClick);
            mBtnChangeWeapon = null;
            mWeaponBackImage = null;
            mWeaponIconImage = null;
            mCdCoolDowm = null;
            mSwitchEquips = null;
            mSwitchEquipsBtn.onClick.RemoveListener(_onSwitchEquipsBtnButtonClick);
            mSwitchEquipsBtn = null;
            mSwitchEquipCD = null;
            mSwitchEquipIcon = null;
            mDragObj = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUISwitchWeaAndEquip";
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            InitWeaponChange();
            InitSwitchEquip();
            
            var data = new InputSettingItem();
            var alpha = 1.0f;
            var canvasGroup = mDragObj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                alpha = canvasGroup.alpha;
            }
            data.SetData(mDragObj.transform.localPosition, mDragObj.transform.localScale, alpha);
            InputSettingBattleManager.instance.InitOriginData_BattleUISwitchWeaAndEquip(data);
            
            var currInputSettingBattleProgram = InputSettingBattleManager.instance.GetCurrInputSettingBattleProgram();
            if (currInputSettingBattleProgram != null)
            {
                SetInputSettingData(mDragObj.transform, currInputSettingBattleProgram.mBattleUISwitchWeaAndEquip);
            }
        }

        public GameObject GetDragObj()
        {
            return mDragObj;
        }

        public GameObject GetNeedShowObj()
        {
            if (mSwitchWeapon != null)
                return mSwitchWeapon.gameObject;
            return null;
        }


        #region  切换装备
        protected string[] m_IconPath = new string[] { "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_Zhandou_Huangzhuang_Text_03",
            "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_Zhandou_Huangzhuang_Text_01" };

        /// <summary>
        /// 初始化切换装备
        /// </summary>
        protected void InitSwitchEquip()
        {
            if (!CanSwitchEquip())
                return;
            mSwitchEquips.gameObject.CustomActive(true);
        }

        /// <summary>
        /// 初始化切换装备按钮图标
        /// </summary>
        public void InitSwitchEquipIcon(int index)
        {
            ChangeEquipIcon(index);
        }

        /// <summary>
        /// 隐藏切换装备按钮
        /// </summary>
        public void SetSwitchEquipBtnState(bool flag)
        {
            if (!CanSwitchEquip())
                return;
            if (mSwitchEquips == null)
                return;
            mSwitchEquips.gameObject.CustomActive(flag);
        }

        /// <summary>
        /// 切换装备
        /// </summary>
        private void _onSwitchEquipsBtnButtonClick()
        {
            BeActor actor = GetLocalActor();
            if (actor == null)
                return;
            if (actor.IsCastingSkill())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("技能施放动作期间无法切换装备");
                return;
            }
            mSwitchEquipsBtn.transform.parent.DOScale(1.1f, 0.1f).OnComplete(() =>
            {
                mSwitchEquipsBtn.transform.parent.DOScale(1.0f, 0.1f);
            });

            if (mSwitchEquipCD != null && mSwitchEquipCD.surplusTime > 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("切换装备冷却中");
                return;
            }

            if (actor.isSpecialMonster)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("当前状态不可切换");
                return;
            }

            if (!CanSwitchEquip())
                return;

            //切换装备CD
            StartSwitchEquipCD();

            //目前只有两个方案进行切换
            int newEquipIndex = actor.GetCurrentSchemeIndex() == 0 ? 1 : 0;

            ChangeEquipIcon(newEquipIndex);

            ChangeWeaponCommand cmd = new ChangeWeaponCommand();
            cmd.equipIndex = newEquipIndex + 1;
            FrameSync.instance.FireFrameCommand(cmd);
        }

        /// <summary>
        /// 切换装备按钮Icon改变
        /// </summary>
        public void ChangeEquipIcon(int pathIndex)
        {
            if (mSwitchEquipIcon == null)
                return;
            string path = m_IconPath[pathIndex];
            ETCImageLoader.LoadSprite(ref mSwitchEquipIcon, path);
        }

        /// <summary>
        /// 开始切换装备CD
        /// </summary>
        protected void StartSwitchEquipCD()
        {
            BeActor actor = GetLocalActor();
            if (actor == null)
                return;
            float surpusTime = 3;
            float reduceCD = 0;
            Mechanism81 mechanism = actor.GetMechanism(5072) as Mechanism81;
            if (mechanism != null)
                reduceCD = mechanism.changeWeaponCD.f;

            Mechanism81 weaponMechanism = actor.GetMechanism(158) as Mechanism81;
            if (weaponMechanism != null)
                reduceCD += weaponMechanism.changeWeaponCD.f;

            if (mechanism != null && mSwitchEquipCD != null)
            {
                mSwitchEquipCD.StartCD(surpusTime * (1 + reduceCD));
            }
            else
            {
                mSwitchEquipCD.StartCD(surpusTime);
            }
        }

        //重置切换武器CD
        public void ResetChangeEquipCD()
        {
            mSwitchEquipCD.ResetCD();
        }

        /// <summary>
        /// 设置本地玩家
        /// </summary>
        protected BeActor GetLocalActor()
        {
            if (BattleMain.instance == null)
                return null;
            if (BattleMain.instance.GetLocalPlayer() == null)
                return null;
            return BattleMain.instance.GetLocalPlayer().playerActor;
        }

        /// <summary>
        /// 是否支持装备切换
        /// </summary>
        protected bool CanSwitchEquip()
        {
            if (mSwitchEquips == null || ReplayServer.GetInstance().IsReplay())
                return false;
            var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            if (player != null && !player.isFighting && BattleMain.battleType == BattleType.PVP3V3Battle)
                return false;
            //Pvp模式不支持切换装备
            if (BattleMain.IsModePvP(BattleMain.battleType))
                return false;
            //技能连招不支持装备切换
            if (BattleMain.battleType == BattleType.TrainingSkillCombo)
                return false;
            BeActor actor = GetLocalActor();
            if (actor == null)
                return false;
            int schemeCount = actor.GetSchemeCount();
            if (schemeCount > 1)
                return true;
            return false;
        }
        #endregion
        #region 切换武器

        public void InitWeaponChange()
        {
            if (mBtnChangeWeapon == null || ReplayServer.GetInstance().IsReplay())
                return;


            var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            if (player != null)
            {
                if (player.playerActor == null) return;

                if (player.playerActor.GetEntityData().CanChangeWeapon())
                {
                    if (BattleMain.battleType == BattleType.PVP3V3Battle)
                    {
                        mBtnChangeWeapon.transform.parent.gameObject.CustomActive(player.isFighting);
                    }
                    else
                    {
                        mBtnChangeWeapon.transform.parent.gameObject.CustomActive(true);
                    }
                    //bool isPVP = BattleMain.IsModePvP(BattleMain.battleType);
                    //mBtnChangeWeapon.transform.parent.rectTransform().anchoredPosition = isPVP ? new Vector2(-85, 427) : new Vector2(-121, 574);
                    ChangeWeaponIcon(player.playerActor.GetEntityData().GetBackupEquipItemID());
                }
            }
        }

        private void _onBtnChangeWeaponButtonClick()
        {
            try
            {
                if (ReplayServer.GetInstance().IsReplay())
                    return;

                mBtnChangeWeapon.transform.parent.DOScale(1.1f, 0.1f).OnComplete(() =>
                {
                    mBtnChangeWeapon.transform.parent.DOScale(1.0f, 0.1f);
                });
                BeActor actor = BattleMain.instance.GetLocalPlayer().playerActor;

                if (mCdCoolDowm != null && mCdCoolDowm.surplusTime > 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("切换武器冷却中");
                    return;
                }

                if (actor != null && actor.isSpecialMonster)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("当前状态不可切换");
                    return;
                }

                if (actor.IsCastingSkill())
                {
                    BeSkill skill = actor.GetCurrentSkill();
                    if (skill != null)
                    {
                        if (skill.canSwitchWeapon)
                        {
                            ChangeWeapon(0);
                        }
                        else
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect("该技能释放过程中无法更换武器");
                            //  actor.m_pkGeActor.CreateHeadText(HitTextType.SKILL_CANNOTUSE, "UI/Font/new_font/pic_incd.png");
                        }
                    }

                }
                else
                {
                    ChangeWeapon(0);
                }
                AudioManager.instance.PlaySound(102);

                GameStatisticManager.GetInstance().DoStartUIButton("BattleChangeWeapon");
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("_onBtnChangeWeaponButtonClick:{0}", e.Message);
            }
        }

        private void ChangeWeapon(int index)
        {
            var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            if (null != actor && null != actor.playerActor)
            {
                StartChangeWeaponCD(actor.playerActor);
                ChangeWeaponCommand cmd = new ChangeWeaponCommand();
                cmd.weaponIndex = index;

                FrameSync.instance.FireFrameCommand(cmd);
            }
        }

        public void StartChangeWeaponCD(BeActor actor)
        {
            float surpusTime = Global.Settings.switchWeaponTime;
            float reduceCD = 0;
            Mechanism81 mechanism = actor.GetMechanism(5072) as Mechanism81;
            if (mechanism != null)
                reduceCD = mechanism.changeWeaponCD.f;

            Mechanism81 weaponMechanism = actor.GetMechanism(158) as Mechanism81;
            if (weaponMechanism != null)
                reduceCD += weaponMechanism.changeWeaponCD.f;

            if (mechanism != null && mCdCoolDowm != null)
            {
                mCdCoolDowm.StartCD(surpusTime * (1 + reduceCD));
            }
            else
            {
                mCdCoolDowm.StartCD(Global.Settings.switchWeaponTime);
            }
        }

        public void ChangeWeaponIcon(int id)
        {
            if (mBtnChangeWeapon == null || id == 0)
                return;
            var data = ItemDataManager.GetInstance().GetItemByTableID(id);
            if (data == null)
                return;
            //ETCImageLoader.LoadSprite(ref mWeaponBackImage, data.GetQualityInfo().Background);
            ETCImageLoader.LoadSprite(ref mWeaponIconImage, data.Icon);
        }

        public void SetWeaponState(bool flag)
        {
            if (mBtnChangeWeapon == null || ReplayServer.GetInstance().IsReplay())
                return;
            SetSwitchEquipBtnState(flag);
            var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            if (player != null)
            {
                if (player.playerActor == null) return;
                if (flag)
                {
                    if (player.playerActor.GetEntityData().CanChangeWeapon())
                    {
                        if (BattleMain.battleType == BattleType.PVP3V3Battle)
                        {
                            mBtnChangeWeapon.transform.parent.gameObject.CustomActive(player.isFighting);
                        }
                        else
                        {
                            mBtnChangeWeapon.transform.parent.gameObject.CustomActive(true);
                        }
                    }

                }
                else
                {
                    mBtnChangeWeapon.transform.parent.gameObject.CustomActive(false);
                }
            }
        }

        //重置切换武器CD
        public void ResetChangeWeaponCD()
        {
            mCdCoolDowm.ResetCD();
        }
        #endregion
    }
}