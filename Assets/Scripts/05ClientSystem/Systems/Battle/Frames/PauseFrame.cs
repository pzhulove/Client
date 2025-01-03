using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class PauseFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Pause/DungeonPauseVIP";
        }
        private bool IsDrugVisible()
        {
            var battleUI= BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
            if (battleUI != null)
            {
                return battleUI.IsDrugVisible();
            }
            return false;
        }
        protected override bool _isLoadFromPool()
        {
            return true;
        }

#region ExtraUIBind
        private Button mCancel = null;
        private Button mOk = null;
        private Button reporter = null;
        private Text info = null;

        private Toggle mDrugTog;
        private Toggle mSysTog;
        private GameObject mDrugRoot;
        private GameObject mSysRoot;
        private PauseSystemSetView mSystemSetView;

        protected override void _bindExUI()
        {
            mCancel = mBind.GetCom<Button>("cancel");
            mCancel.onClick.AddListener(_onCancelButtonClick);
            mOk = mBind.GetCom<Button>("ok");
            mOk.onClick.AddListener(_onOkButtonClick);
            reporter = mBind.GetCom<Button>("reporter");
            info = mBind.GetCom<Text>("info");

            mDrugTog = mBind.GetCom<Toggle>("DrugSettingTab");
            mSysTog = mBind.GetCom<Toggle>("SystemSettingTab");
            mDrugRoot = mBind.GetGameObject("DrugSetting");
            mSysRoot = mBind.GetGameObject("SystemBattleSetting");
            mSystemSetView = mBind.GetCom<PauseSystemSetView>("systemSet");
            if (null != mSystemSetView)
                mSystemSetView.OnInit();
            if (mDrugTog)
            {
                mDrugTog.onValueChanged.AddListener(_onDrugToggleValueChange);
            }
            if (mSysTog)
            {
                mSysTog.onValueChanged.AddListener(_onSysToggleValueChange);
            }
#if MG_TEST
           if(reporter != null)
           {
                reporter.onClick.AddListener(_onReporterButtonClick);
           }
#endif

#if MG_TEST || MG_TEST2 || MGSPTIYAN
            if (info)
            {
                info.verticalOverflow = VerticalWrapMode.Overflow;
                info.raycastTarget = false;
                info.supportRichText = true;
                info.text = string.Format("暂停中（组队模式无效）\n剩余内存:{0},app占用内存:{1}", PluginManager.GetInstance().GetAvailMemory(), PluginManager.GetInstance().GetCurrentProcessMemory());
            }
#endif
        }

        protected override void _unbindExUI()
        {
            mCancel.onClick.RemoveListener(_onCancelButtonClick);
            mCancel = null;
            mOk.onClick.RemoveListener(_onOkButtonClick);
            mOk = null;
            info = null;
#if MG_TEST
            if (reporter != null)
            {
                reporter.onClick.RemoveListener(_onReporterButtonClick);
            }
#endif
            reporter = null;

            mDrugTog = null;
            mSysTog = null;
            mDrugRoot = null;
            mSysRoot = null;

            if (mDrugTog)
            {
                mDrugTog.onValueChanged.RemoveListener(_onDrugToggleValueChange);
            }
            if (mSysTog)
            {
                mSysTog.onValueChanged.RemoveListener(_onSysToggleValueChange);
            }
            if (null != mSystemSetView)
            {
                mSystemSetView.OnUnInit();
            }
        }
#endregion

#region Callback
        private void _onCancelButtonClick()
        {
            /* put your code in here */
            OnClickClose();

        }
        private void _onReporterButtonClick()
        {
#if MG_TEST
            VideoInfo info = new VideoInfo();
            info.isInBattle = true;
            ClientSystemManager.instance.OpenFrame<PKReporterFrame>(FrameLayer.Middle, info);
#endif
        }
        private void _onOkButtonClick()
        {
            /* put your code in here */
            OnClickBack();

        }

        private void _onDrugToggleValueChange(bool flag)
        {
            if (mDrugRoot) 
            {
                mDrugRoot.CustomActive(flag);
            }
        }

        private void  _onSysToggleValueChange(bool flag)
        {
            if (mSysRoot)
            {
                mSysRoot.CustomActive(flag);
            }
        }
#endregion

        private GameObject battlePotionSet;
        private GameObject vipsetting;

        private GameObject systemSetting;
        protected override void _OnOpenFrame()
        {
#if !MG_TEST
            if (reporter != null)
            {
                reporter.gameObject.CustomActive(false);
            }
#endif
            //if (mDrugRoot != null)
            //{
            //    battlePotionSet = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/SettingPanel/BattlePotionSet");
            //    Utility.AttachTo(battlePotionSet, mDrugRoot);
            //}
            InitToggleUI();
        }

        protected override void _OnCloseFrame()
        {
            if (battlePotionSet != null)
            {
                GameObject.Destroy(battlePotionSet);
            }
            if (vipsetting != null)
            {
                GameObject.Destroy(vipsetting);
            }
            if(systemSetting != null)
            {
                GameObject.Destroy(systemSetting);
            }
            base._OnCloseFrame();
        }

        private void InitToggleUI()
        {
            if (!IsDrugVisible())
            {
                mDrugRoot.CustomActive(false);
                mSysTog.isOn = true;
            }
            else
            {
                mDrugTog.isOn = true;
            }
        }

        public void OnClickClose()
        {

            if (!BattleMain.IsModeMultiplayer(BattleMain.mode))
                BattleMain.instance.GetDungeonManager().ResumeFight();

            ClientSystemManager.instance.CloseFrame<PauseFrame>();
        }

        public void OnClickBack()
        {
            TeamDataManager.GetInstance().NotPopUpTeamList = true;
			if (NeedPromotLeave())
			{
                if (BeUtility.CheckDungeonIsLimitTimeHell())
                {
                    SystemNotifyManager.SystemNotify(9974,
                   () =>
                   {//ok
                       DoBack();
                   });
                    return;
                }
                if (IsInDifferentWorld())
                {
                    SystemNotifyManager.SystemNotify(9935,
                    () =>
                    {//ok
                        DoBack();
                    });
                }
                else
                {
                    SystemNotifyManager.SystemNotify(3125,
                    () =>
                    {//ok
                        DoBack();
                    });
                }

                
			}
			else
			{
				DoBack();
			}
        }

		bool NeedPromotLeave()
		{
			var data = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table;
			if (data != null)
			{
				return null != TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTimesTable>((int)data.SubType);
			}

			return false;
		}
        //是否处于异界关卡中
        bool IsInDifferentWorld()
        {
            var data = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table;
            if(data.SubType == ProtoTable.DungeonTable.eSubType.S_DEVILDDOM)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void DoBack()
		{
#if !LOGIC_SERVER
            SaveBattleResult();
            if (RecordServer.GetInstance().IsReplayRecord())
			{
/*				if (RecordServer.GetInstance().IsRecord())
				{
					RecordServer.GetInstance().RecordProcess("[PAUSE FRAME]on Click back");
				}	*/
				RecordServer.GetInstance().EndRecord("DoBack");
			}
            TMBattleAssetLoadRecord.instance.SaveInfoToFile();
            BeUtility.ResetCamera();
#endif
			//BattleMain.instance.RewardReq();
			NewbieGuideManager.instance.CleanWeakGuide();
			ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
		}

        /// <summary>
        /// 存储战斗结果
        /// </summary>
        private void SaveBattleResult()
        {
#if !LOGIC_SERVER
            if (BattleMain.instance == null)
                return;
            var baseBattle = BattleMain.instance.GetBattle() as BaseBattle;
            if (baseBattle == null)
                return;
            baseBattle.PveBattleResult = BattleResult.Success;
#endif
        }
    }
}
