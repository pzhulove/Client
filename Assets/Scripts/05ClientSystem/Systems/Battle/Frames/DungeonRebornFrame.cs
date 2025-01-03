using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using Protocol;
using Network;
using System.Diagnostics;
using ProtoTable;

namespace GameClient
{
    public class DungeonRebornFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComCountScript m_countScirpt = null;
        private Image m_dianji = null;
        private Text m_viptips = null;
        private Text mLeftCoinCount = null;
        private GameObject mRebornRoot = null;
        private GameObject mDungeonReborn = null;
        private Text mLeftRebornCoinCount = null;
        private Text mLeftRebornCount = null;
        private GameObject mLeftCountRoot = null;
        private GameObject mCannotRebornRoot = null;
        private RectTransform mGiveupRebornRoot = null;
        private Button mGiveupRebornBtn = null;
        private Button mBtnOk = null;
        private GameObject mVipDescRoot = null;
        private GameObject mNormalDescRoot = null;
        private GameObject mVipOkButtonRoot = null;
        private GameObject mNormalOkButtonRoot = null;
        private Text mNotVipLevel = null;
        private Text mNotVipCount = null;
        private Text mIsVipLevel = null;
        private Text mIsVipLeft = null;
        private Text mIsVipSum = null;
        private Button mBtnCancel = null;
        private GameObject mOkButtonRoot = null;
        private GameObject mCancelButtonRoot = null;

        protected override void _bindExUI()
        {
            m_countScirpt = mBind.GetCom<ComCountScript>("_countScirpt");
            m_dianji = mBind.GetCom<Image>("_dianji");
            m_viptips = mBind.GetCom<Text>("_viptips");
            mLeftCoinCount = mBind.GetCom<Text>("LeftCoinCount");
            mRebornRoot = mBind.GetGameObject("RebornRoot");
            mDungeonReborn = mBind.GetGameObject("DungeonReborn");
            mLeftRebornCoinCount = mBind.GetCom<Text>("LeftRebornCoinCount");
            mLeftRebornCount = mBind.GetCom<Text>("LeftRebornCount");
            mLeftCountRoot = mBind.GetGameObject("LeftCountRoot");
            mCannotRebornRoot = mBind.GetGameObject("CannotRebornRoot");
            mGiveupRebornRoot = mBind.GetCom<RectTransform>("GiveupRebornRoot");
            mGiveupRebornBtn = mBind.GetCom<Button>("GiveupRebornBtn");
            if (null != mGiveupRebornBtn)
            {
                mGiveupRebornBtn.onClick.AddListener(_onGiveupRebornBtnButtonClick);
            }
            mBtnOk = mBind.GetCom<Button>("btnOk");
            if (null != mBtnOk)
            {
                mBtnOk.onClick.AddListener(_onBtnOkButtonClick);
            }
            mVipDescRoot = mBind.GetGameObject("vipDescRoot");
            mNormalDescRoot = mBind.GetGameObject("normalDescRoot");
            mVipOkButtonRoot = mBind.GetGameObject("vipOkButtonRoot");
            mNormalOkButtonRoot = mBind.GetGameObject("normalOkButtonRoot");
            mNotVipLevel = mBind.GetCom<Text>("notVipLevel");
            mNotVipCount = mBind.GetCom<Text>("notVipCount");
            mIsVipLevel = mBind.GetCom<Text>("isVipLevel");
            mIsVipLeft = mBind.GetCom<Text>("isVipLeft");
            mIsVipSum = mBind.GetCom<Text>("isVipSum");
            mBtnCancel = mBind.GetCom<Button>("btnCancel");
            if (null != mBtnCancel)
            {
                mBtnCancel.onClick.AddListener(_onBtnCancelButtonClick);
            }
            mOkButtonRoot = mBind.GetGameObject("okButtonRoot");
            mCancelButtonRoot = mBind.GetGameObject("cancelButtonRoot");
        }

        protected override void _unbindExUI()
        {
            m_countScirpt = null;
            m_dianji = null;
            m_viptips = null;
            mLeftCoinCount = null;
            mRebornRoot = null;
            mDungeonReborn = null;
            mLeftRebornCoinCount = null;
            mLeftRebornCount = null;
            mLeftCountRoot = null;
            mCannotRebornRoot = null;
            mGiveupRebornRoot = null;
            if (null != mGiveupRebornBtn)
            {
                mGiveupRebornBtn.onClick.RemoveListener(_onGiveupRebornBtnButtonClick);
            }
            mGiveupRebornBtn = null;
            if (null != mBtnOk)
            {
                mBtnOk.onClick.RemoveListener(_onBtnOkButtonClick);
            }
            mBtnOk = null;
            mVipDescRoot = null;
            mNormalDescRoot = null;
            mVipOkButtonRoot = null;
            mNormalOkButtonRoot = null;
            mNotVipLevel = null;
            mNotVipCount = null;
            mIsVipLevel = null;
            mIsVipLeft = null;
            mIsVipSum = null;
            if (null != mBtnCancel)
            {
                mBtnCancel.onClick.RemoveListener(_onBtnCancelButtonClick);
            }
            mBtnCancel = null;
            mOkButtonRoot = null;
            mCancelButtonRoot = null;
        }
        #endregion

        #region Callback
        private void _onBtnOkButtonClick()
        {
            _onButtonReborn();
        }
        private void _onBtnCancelButtonClick()
        {
            m_countScirpt.StopCount();
            frame.CustomActive(false);
        }

        private void _onGiveupRebornBtnButtonClick()
        {
            if (BattleMain.instance == null || BattleMain.instance.GetPlayerManager() == null)
                return;
            //单机模式下直接返回城镇
            if(BattleMain.instance.GetPlayerManager().GetPlayerCount() == 1)
                m_countScirpt.RebornFail();
            ClientSystemManager.instance.CloseFrame(this);
        }
        #endregion

        public static bool isLocal;

        private const int COUNT_TIME = 9;

        public enum eState
        {
            None,

            /// <summary>
            /// 用户复活
            /// </summary>
            Reborn,

            /// <summary>
            /// 用户取消复活
            /// </summary>
            Cancel,

            /// <summary>
            /// 无法复活
            /// </summary>
            NoWay,
        }

        public static eState sState = eState.None;


        #region Override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Reborn/DungeonReborn";
        }

        protected override bool _isLoadFromPool()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
            sState = eState.None;

            _Initialize();
            _updateCancelButtonState();
            _TryStartCountDown();
            _RegisterUIEvent();

            _updateOkButtonState();
            _updateLeftCount();
            UpdateDungeonRebornCount();

#if DEBUG_SETTING
            if (Global.Settings.IsTestTeam())
            {
                ClientSystemManager.GetInstance().delayCaller.DelayCall(2000, () =>
                {
                    if (ClientSystemManager.GetInstance().IsFrameOpen<DungeonRebornFrame>())
                        _onButtonReborn();
                });
            }
#endif
        }

        protected override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
        }
        #endregion

        #region GetRebornType

        /// <summary>
        /// 是否有VIP的权益
        /// </summary>
        private bool _hasGotVipRight()
        {
            return DungeonUtility.GetVipRebornSumCount() > 0;// _getVipRebornSumCount() > 0;
        }

        private bool _isVipFreeReborn()
        {
            return DungeonUtility.GetVipRebornLeftCount() > 0;
        }
        #endregion

        void _updateLeftCount()
        {
            /// todo 不对
            mLeftCoinCount.text = PlayerBaseData.GetInstance().AliveCoin.ToString();
        }

        void UpdateDungeonRebornCount()
        {
            int actorDungeonCount = GetLocalActorRebornCount();
            int dungeonRebornCount = GetDungeonRebornLimit();
            var battle = BattleMain.instance.GetBattle();

            bool isBattleRebornCountOk = true;
            if (IsBattleRebornLimit())
            {
                isBattleRebornCountOk = GetBattleLeftRebornCount() > 0;
            }
            bool isLimited = false;
            if (isBattleRebornCountOk)
            {
                if (dungeonRebornCount < 0)
                    return;
                isLimited = actorDungeonCount >= dungeonRebornCount;
            }
            else
            {
                isLimited = true;
            }

            mCancelButtonRoot.CustomActive(false);

            if (isLimited)
            {
                mCannotRebornRoot.CustomActive(true);
                mDungeonReborn.CustomActive(false);
                mRebornRoot.CustomActive(false);
                mOkButtonRoot.CustomActive(false);
                mVipDescRoot.CustomActive(false);
                mGiveupRebornRoot.gameObject.CustomActive(true);
            }
            else
            {
                mRebornRoot.CustomActive(false);
                mDungeonReborn.CustomActive(true);
                mLeftRebornCoinCount.text = PlayerBaseData.GetInstance().AliveCoin.ToString();
                mLeftRebornCount.text = string.Format(TR.Value("chapter_revive_tips"), dungeonRebornCount - actorDungeonCount);
            }
        }

        private void _updateOkButtonState()
        {
            mOkButtonRoot.SetActive(_canReborn());
        }

        private void _updateCancelButtonState()
        {
            if (!CheckRebornCount())
                return;
            mCancelButtonRoot.SetActive(!BattleMain.instance.GetPlayerManager().IsAllPlayerDead());
        }

        void _Initialize()
        {
            m_countScirpt.StopCount();


            mVipDescRoot.SetActive(false);
            mVipOkButtonRoot.SetActive(false);
            mNormalDescRoot.SetActive(false);
            mNormalOkButtonRoot.SetActive(false);

            if (_hasGotVipRight())
            {
                mVipDescRoot.SetActive(true);
                mIsVipLevel.text = PlayerBaseData.GetInstance().VipLevel.ToString();
                mIsVipLeft.text = DungeonUtility.GetVipRebornLeftCount().ToString();// _getVipRebornLeftCount().ToString();
                mIsVipSum.text = DungeonUtility.GetVipRebornSumCount().ToString(); //_getVipRebornSumCount().ToString();
            }
            else
            {
                KeyValuePair<int, float> kv = Utility.GetFirstValidVipLevelPrivilegeData(VipPrivilegeTable.eType.FREE_REVIVE);

                if (kv.Value > 0.0f)
                {
                    mNormalDescRoot.SetActive(true);

                    mNotVipLevel.text = kv.Key.ToString();
                    mNotVipCount.text = ((int)kv.Value).ToString();
                }
            }

            if (_isVipFreeReborn())
            {
                mVipOkButtonRoot.SetActive(true);
            }
            else
            {
                mNormalOkButtonRoot.SetActive(true);
            }
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattlePlayerDead, _OnBattlePlayerDead);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRebornFail, _onBattlePlayerRebornFail);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onBattlePlayerRebornSuccess);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattlePlayerDead, _OnBattlePlayerDead);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonRebornFail, _onBattlePlayerRebornFail);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onBattlePlayerRebornSuccess);
        }

        void _OnBattlePlayerDead(UIEvent uiEvent)
        {
            _updateCancelButtonState();
            _TryStartCountDown();
        }

        //有队友成功复活
        void _onBattlePlayerRebornSuccess(UIEvent ui)
        {
            byte target = (byte)ui.Param1;

            m_countScirpt.StopCount();

            byte mainPlayerSeat = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerInfo.seat;

            sState = eState.Reborn;

            if (target == mainPlayerSeat)
            {
                ClientSystemManager.instance.CloseFrame(this);
            }
            else
            {
                _updateCancelButtonState();
            }

        }

        //有玩家复活失败
        void _onBattlePlayerRebornFail(UIEvent ui)
        {
            if (!CheckRebornCount())
                return;
            byte target = (byte)ui.Param1;

            m_countScirpt.ResumeCount();
        }

        //所有队员都死了，开启战败倒计时
        void _TryStartCountDown()
        {
            if (m_countScirpt.State == ComCountScript.CountState.Count)
            {
                return;
            }

            if (BattleMain.instance.GetPlayerManager().IsAllPlayerDead())
            {
                frame.CustomActive(true);
                m_countScirpt.StartCount(() => { _BackToTown(); }, COUNT_TIME);
            }
        }

        void _BackToTown()
        {
            sState = eState.Cancel;
        }

        bool _canReborn()
        {
            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            var battle = BattleMain.instance.GetBattle();
            if (battle != null && !battle.CanReborn()) return false;
            return DungeonUtility.CanReborn(id, true);
        }

        void _onButtonReborn()
        {
            if (_canReborn())
            {
                if (m_countScirpt == null)
                    return;
                int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
                byte mainPlayerSeat = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerInfo.seat;
                DungeonUtility.eDungeonRebornType type = DungeonUtility.GetDungeonRebornType(id, true);
                if (DungeonUtility.eDungeonRebornType.QuickBuyReborn != type)
                {
                    m_countScirpt.PauseCount();
                }
                DungeonUtility.StartRebornProcess(mainPlayerSeat, mainPlayerSeat, id);
            }
            else
            {
                SystemNotifyManager.SystemNotify(1098);
            }
        }

        //检查复活次数
        protected bool CheckRebornCount()
        {
            int actorDungeonCount = GetLocalActorRebornCount();
            int dungeonRebornCount = GetDungeonRebornLimit();
            return actorDungeonCount < dungeonRebornCount;
        }

        //获取自己的复活次数
        protected int GetLocalActorRebornCount()
        {
            if (BattleMain.instance == null
                   || BattleMain.instance.GetPlayerManager() == null 
                   || BattleMain.instance.GetPlayerManager().GetMainPlayer() == null)
                return 0;
            BeActor actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
            if (actor == null)
                return 0;
            return actor.dungeonRebornCount;
        }

        //获取地下城限制复活次数
        protected int GetDungeonRebornLimit()
        {
            if (BattleMain.instance == null
                   || BattleMain.instance.GetDungeonManager() == null
                   || BattleMain.instance.GetDungeonManager().GetDungeonDataManager() == null)
                return 0;
            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            return DungeonUtility.GetDungeonRebornCount(id);
        }
        protected bool IsBattleRebornLimit()
        {
            if (BattleMain.instance == null
                  || BattleMain.instance.GetBattle() == null)
                return false;
            return BattleMain.instance.GetBattle().IsRebornCountLimit();
        }
        protected int GetBattleLeftRebornCount()
        {
            if (BattleMain.instance == null
                   || BattleMain.instance.GetBattle() == null)
                return 0;
            var baseBattle = BattleMain.instance.GetBattle();
            if (!baseBattle.IsRebornCountLimit()) return 0;
            return baseBattle.GetLeftRebornCount();
        }

    }
}
