using ProtoTable;
using UnityEngine;

/// <summary>
/// 使用药品
/// </summary>
namespace GameClient
{
    public class BattleUIDrug : BattleUIBase
    {
        #region ExtraUIBind
        private ComDrugTipsBar mComDrugTipsBar = null;
        private GameObject mDragObj = null;
        protected override void _bindExUI()
        {
            mComDrugTipsBar = mBind.GetCom<ComDrugTipsBar>("ComDrugTipsBar");
            mDragObj = mBind.GetGameObject("DragObj");
        }

        protected override void _unbindExUI()
        {
            mComDrugTipsBar = null;
            mDragObj = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIDrug";
        }

        protected override void OnStart()
        {
            base.OnStart();

            _setDungeonItem();

            _BindGet_localPlayerHPValueChanged();
            _BindGet_localPlayerMPValueChanged();

            var data = new InputSettingItem();
            var alpha = 1.0f;
            var canvasGroup = mDragObj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                alpha = canvasGroup.alpha;
            }
            data.SetData(mDragObj.transform.localPosition, mDragObj.transform.localScale, alpha);
            InputSettingBattleManager.instance.InitOriginData_BattleUIDrug(data);
            
            var currInputSettingBattleProgram = InputSettingBattleManager.instance.GetCurrInputSettingBattleProgram();
            if (currInputSettingBattleProgram != null)
            {
                SetInputSettingData(mDragObj.transform, currInputSettingBattleProgram.mBattleUIDrug);
            }
        }
        
        public GameObject GetDragObj()
        {
            return mDragObj;
        }
        
        public GameObject GetNeedShowObj()
        {
            if(mComDrugTipsBar != null)
                return mComDrugTipsBar.gameObject;
            return null;
        }

        private bool _BindGet_localPlayerHPValueChanged()
        {
            if (BattleMain.instance == null) return false;
            var _localPlayer = BattleMain.instance.GetLocalPlayer();
            if (null == _localPlayer) return false;
            if (!BattlePlayer.IsDataValidBattlePlayer(_localPlayer)) return false;
            if (null == _localPlayer.playerActor) return false;

            _localPlayer.playerActor.RegisterEventNew(BeEventType.onHPChange, (args3) =>
            {
                if (null == _localPlayer)
                {
                    return;
                }

                var actor = _localPlayer.playerActor;

                if (null == mComDrugTipsBar)
                {
                    return;
                }

                if (mComDrugTipsBar.gameObject.activeSelf)
                {
                    GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.HpChanged);
                    mComDrugTipsBar.UseHpDrug(true);
                }
            });

            return true;
        }


        /// <summary>
        /// 吃药是否显示
        /// </summary>
        /// <returns></returns>
        public bool IsDrugVisible()
        {
            if (mComDrugTipsBar != null && mComDrugTipsBar.gameObject != null)
                return mComDrugTipsBar.gameObject.activeSelf;
            return false;
        }

        public void UseDrug()
        {

            //if (PlayerBaseData.GetInstance().VipLevel < 1)
            //	return;

            //if (mComDrugTipsBar != null)
            //	mComDrugTipsBar.UseDefaultDrug();
        }

        public void SetDrugVisible(bool flag)
        {
            if (mComDrugTipsBar != null)
                mComDrugTipsBar.CustomActive(flag);
        }



        private bool _BindGet_localPlayerMPValueChanged()
        {
            if (BattleMain.instance == null) return false;
            var _localPlayer = BattleMain.instance.GetLocalPlayer();
            if (null == _localPlayer) return false;
            if (!BattlePlayer.IsDataValidBattlePlayer(_localPlayer)) return false;
            if (null == _localPlayer.playerActor) return false;

            _localPlayer.playerActor.RegisterEventNew(BeEventType.onMPChange, args3 =>
           //_localPlayer.playerActor.RegisterEvent(BeEventType.onMPChange, (object[] args3) =>
            {
                if (null == _localPlayer)
                {
                    return;
                }
                var actor = _localPlayer.playerActor;
                if (null == mComDrugTipsBar)
                {
                    return;
                }
                if (mComDrugTipsBar.gameObject.activeSelf)
                {
                    mComDrugTipsBar.UseMPDrug(_localPlayer.isAutoFight);
                }
            });

            return true;
        }

        private void _setDungeonItem()
        {
            if (mComDrugTipsBar == null)
                return;
            mComDrugTipsBar.Init();

            switch (BattleMain.battleType)
            {
                case BattleType.Single:
                case BattleType.MutiPlayer:
                case BattleType.GuildPVP:
                case BattleType.MoneyRewardsPVP:
                case BattleType.Training:
                case BattleType.DeadTown:
                case BattleType.PVP3V3Battle:
                case BattleType.ScufflePVP:
                case BattleType.TrainingPVE:
                case BattleType.ChijiPVP:
                case BattleType.FinalTestBattle:
                    mComDrugTipsBar.SetRootActive(false);
                    break;
                case BattleType.InputSetting:
                case BattleType.Dungeon:
                case BattleType.GuildPVE:
                case BattleType.ChampionMatch:
                    mComDrugTipsBar.SetRootActive(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.BattleDrugs));
                    break;
            }
        }
    }
}
