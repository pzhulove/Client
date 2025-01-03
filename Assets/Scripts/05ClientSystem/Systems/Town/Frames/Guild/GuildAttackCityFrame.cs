using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using Protocol;
using Network;
using System;

namespace GameClient
{
    class GuildAttackCityFrame : ClientFrame
    {
        int TerritoryID = 0;
        int AddedMoneyNum = 0;

        DelayCallUnitHandle m_repeatCallLeftTime;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildAttackCityFrame";
        }

        protected override void _OnOpenFrame()
        { 
            if(userData != null)
            {
                TerritoryID = (int)userData;
            }

            var AddedMoney = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_BATTLE_CHALLENGE_ITEM_NUM);
            if (AddedMoney != null)
            {
                AddedMoneyNum = AddedMoney.Value;
            }

            RegisterUIEvent();
            SendAttackGuildReq();
        }

        protected override void _OnCloseFrame()
        {
            UnRegisterUIEvent();
            ClearData();
        }

        void ClearData()
        {
            TerritoryID = 0;
            AddedMoneyNum = 0;

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallLeftTime);
        }

        void RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildAttackCityInfoUpdate, UpdateInterface);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, OnUpdateGuildBattleState);
        }

        void UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildAttackCityInfoUpdate, UpdateInterface);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, OnUpdateGuildBattleState);
        }

        void OnUpdateGuildBattleState(UIEvent iEvent)
        {
            UpdateInterface(iEvent);
        }

        void UpdateInterface(UIEvent iEvent)
        {
            GuildAttackCityData data = GuildDataManager.GetInstance().GetAttackCityData();
            if(data == null)
            {
                return;
            }

            if (data.enrollGuildId <= 0)
            {
                mName.text = "当前尚未有公会进行宣战";
                mLevel.text ="0" ;
                mGuildLeader.text = "无";
            }
            else
            {
                mName.text = string.Format("【{0}】", data.enrollGuildName);
                mLevel.text = data.enrollGuildLevel.ToString();
                mGuildLeader.text = data.enrollGuildleaderName;
            }

            mBidPrice.text = data.itemNum.ToString();
            mButtonPrice.text = (data.itemNum + AddedMoneyNum).ToString();

            UpdateSignUp();

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallLeftTime);

            m_repeatCallLeftTime = ClientSystemManager.GetInstance().delayCaller.RepeatCall(1000, () =>
            {
                _UpdateLeftTime();
            }, 9999999, true);
        }

        void UpdateSignUp()
        {
            if (!CheckCanChallenge())
            {
                mSignUpGray.enabled = true;
                mSignUp.interactable = false;
            }
            else
            {
                mSignUpGray.enabled = false;
                mSignUp.interactable = true;
            }
        }

        void _UpdateLeftTime()
        {
            if (GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Signup)
            {
                uint nTimeLeft = GuildDataManager.GetInstance().GetGuildBattleStateEndTime() - TimeManager.GetInstance().GetServerTime();

                if (nTimeLeft > 0)
                {
                    mLeftTime.text = Function.GetLeftTime((int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime(), (int)TimeManager.GetInstance().GetServerTime(), ShowtimeType.OnlineGift);
                    mLeftTime.gameObject.CustomActive(true);
                }
                else
                {
                    mLeftTime.text = "已结束";
                }
            }
            else
            {
                mLeftTime.text = "已结束";
            }
        }

        bool CheckCanChallenge(bool bNeedTip = false)
        {
            GuildAttackCityData data = GuildDataManager.GetInstance().GetAttackCityData();
            if (data == null)
            {
                return false;
            }

            if (GuildDataManager.GetInstance().GetGuildBattleType() != GuildBattleType.GBT_CHALLENGE)
            {
                if(bNeedTip)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("当前非攻城战日期,无法发起挑战");
                }
                
                return false;
            }

            if (GuildDataManager.GetInstance().GetGuildBattleState() != EGuildBattleState.Signup)
            {
                if(bNeedTip)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("攻城战报名阶段已结束");
                }
               
                return false;
            }

            if (!GuildDataManager.GetInstance().HasPermission(EGuildPermission.StartGuildAttackCity))
            {
                if(bNeedTip)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你当前没有权限发起挑战!");
                }
                
                return false;
            }

            if(data.info.terrId <= 0)
            {
                if(bNeedTip)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("该领地未被占领,无法发起挑战");
                }

                return false;
            }

            if (GuildDataManager.GetInstance().GetManorOwner(TerritoryID) == GuildDataManager.GetInstance().GetMyGuildName())
            {
                if (bNeedTip)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你所在公会已经占领该领地，无需发起挑战!");
                }

                return false;
            }

            return true;
        }

        void SendAttackGuildReq()
        {
            WorldGuildChallengeInfoReq req = new WorldGuildChallengeInfoReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendAttackCityReq()
        {
            GuildAttackCityData data = GuildDataManager.GetInstance().GetAttackCityData();
            if(data == null)
            {
                return;
            }

            WorldGuildChallengeReq req = new WorldGuildChallengeReq();

            req.terrId = (byte)TerritoryID;
            req.itemId = data.itemId;
            req.itemNum = (uint)AddedMoneyNum + data.itemNum;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private Text mTitle = null;
        private Text mName = null;
        private Text mLevel = null;
        private Text mGuildLeader = null;
        private Text mBidPrice = null;
        private Text mButtonPrice = null;
        private Button mSignUp = null;
        private UIGray mSignUpGray = null;
        private Text mLeftTime = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mTitle = mBind.GetCom<Text>("Title");
            mName = mBind.GetCom<Text>("Name");
            mLevel = mBind.GetCom<Text>("Level");
            mGuildLeader = mBind.GetCom<Text>("GuildLeader");
            mBidPrice = mBind.GetCom<Text>("BidPrice");
            mButtonPrice = mBind.GetCom<Text>("ButtonPrice");
            mSignUp = mBind.GetCom<Button>("SignUp");
            mSignUp.onClick.AddListener(_onSignUpButtonClick);
            mSignUpGray = mBind.GetCom<UIGray>("SignUpGray");
            mLeftTime = mBind.GetCom<Text>("LeftTime");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mTitle = null;
            mName = null;
            mLevel = null;
            mGuildLeader = null;
            mBidPrice = null;
            mButtonPrice = null;
            mSignUp.onClick.RemoveListener(_onSignUpButtonClick);
            mSignUp = null;
            mSignUpGray = null;
            mLeftTime = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        private void _onSignUpButtonClick()
        {
            if(!CheckCanChallenge(true))
            {
                return;
            }

            GuildAttackCityData data = GuildDataManager.GetInstance().GetAttackCityData();
            if (data == null)
            {
                return;
            }

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = (int)data.itemId;
            costInfo.nCount = AddedMoneyNum + (int)data.itemNum;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                SendAttackCityReq();
            });         
        }
        #endregion
    }
}

