using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class MoneyRewardsEnterFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MoneyRewards/MoneyRewardsEnterFrame";
        }

        [UIControl("PlayerCount", typeof(Text))]
        Text playerCount;

        [UIControl("MoneyIcon_0", typeof(Image))]
        Image icon_0;

        [UIControl("FixedDesc_0", typeof(Text))]
        Text fixedDesc0;

        [UIControl("FixedDesc_1", typeof(Text))]
        Text fixedDesc1;

        [UIControl("MoneyCount", typeof(Text))]
        Text moneyCount;

        [UIControl("poolsMoney", typeof(ComRollNumber))]
        ComRollNumber poolsMoney;

        [UIControl("", typeof(StateController))]
        StateController comState;

        [UIControl("", typeof(ComMoneyRewardsEnterSetting))]
        ComMoneyRewardsEnterSetting comSetting;

        public static void CommandOpen(object argv)
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<MoneyRewardsEnterFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsEnterFrame>();
            }
        }
        public static void OpenLinkFrame(string strParam)
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<MoneyRewardsEnterFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsEnterFrame>();
            }
        }

        protected override void _OnOpenFrame()
        {
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });
            _AddButton("BtnEnter", _OnClickEnter);

            if(null != icon_0)
            {
                ETCImageLoader.LoadSprite(ref icon_0, MoneyRewardsDataManager.GetInstance().MoneyIcon);
            }

            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsPoolsMoneyChanged, _OnMoneyRewardsPoolsMoneyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsPlayerCountChanged, _OnMoneyRewardsPlayerCountChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsAwardListChanged, _OnOnMoneyRewardsAwardListChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsSelfResultChanged, _OnMoneyRewardsSelfResultChanged);

            if(null != comSetting)
            {
                comSetting.UpdateHint();
            }

            _UpdateStage();
            _UpdateMoneyCount();
            _UpdatePlayerCount();
            _UpdatePoolMoneys();
            _UpdateDesc();
            _UpdateChampAward();
        }

        void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _UpdateMoneyCount();
        }

        void _OnMoneyRewardsPoolsMoneyChanged(UIEvent uiEvent)
        {
            _UpdatePoolMoneys();
        }

        void _OnMoneyRewardsPlayerCountChanged(UIEvent uiEvent)
        {
            _UpdatePlayerCount();
        }

        void _OnMoneyRewardsStatusChanged(UIEvent uiEvent)
        {
            _UpdateStage();
        }

        void _OnOnMoneyRewardsAwardListChanged(UIEvent uiEvent)
        {
            _UpdateChampAward();
        }

        void _OnMoneyRewardsSelfResultChanged(UIEvent uiEvent)
        {
            _UpdateChampAward();
        }

        void _UpdateMoneyCount()
        {
            if (null != moneyCount)
            {
                if (MoneyRewardsDataManager.GetInstance().IsMoneyEnough)
                {
                    moneyCount.text = TR.Value("money_rewards_cost_money_enable", MoneyRewardsDataManager.GetInstance().MoneyCount);
                }
                else
                {
                    moneyCount.text = TR.Value("money_rewards_cost_money_disable", MoneyRewardsDataManager.GetInstance().MoneyCount);
                }
            }
        }

        void _UpdatePlayerCount()
        {
            if(null != playerCount)
            {
                playerCount.text = MoneyRewardsDataManager.GetInstance().playerCount.ToString();
            }
        }

        void _UpdatePoolMoneys()
        {
            if(null != poolsMoney)
            {
                poolsMoney.RollValue = MoneyRewardsDataManager.GetInstance().moneysInPool;
            }
        }

        void _UpdateDesc()
        {
            if(null != fixedDesc0)
            {
                fixedDesc0.text = TR.Value("money_rewards_fixed_desc0");
            }
            if (null != fixedDesc1)
            {
                fixedDesc1.text = TR.Value("money_rewards_fixed_desc1");
            }
        }

        void _UpdateChampAward()
        {
            if(null != comSetting)
            {
                comSetting.UpdateChampAwards();
            }
        }

        void _UpdateStage()
        {
            if(null != comState)
            {
                bool bOpened = MoneyRewardsDataManager.GetInstance().isOpen;
                if(bOpened)
                {
                    if(!MoneyRewardsDataManager.GetInstance().bHasParty)
                    {
                        if(MoneyRewardsDataManager.GetInstance().Status == PremiumLeagueStatus.PLS_ENROLL)
                        {
                            comState.Key = "addparty";
                        }
                        else
                        {
                            comState.Key = "timeover";
                        }
                    }
                    else
                    {
                        comState.Key = "enter";
                    }
                }
                else
                {
                    comState.Key = "closed";
                }
            }
        }

        void _OnClickEnter()
        {
            if (MoneyRewardsDataManager.GetInstance().needLevel > PlayerBaseData.GetInstance().Level)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_add_need_lv", MoneyRewardsDataManager.GetInstance().needLevel));
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }

            if (!MoneyRewardsDataManager.GetInstance().bHasParty)
            {
                if (MoneyRewardsDataManager.GetInstance().Status != PremiumLeagueStatus.PLS_ENROLL)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_addparty_stage_error"), ProtoTable.CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                    return;
                }

                if (!MoneyRewardsDataManager.GetInstance().IsMoneyEnough)
                {
                    ItemComeLink.OnLink(MoneyRewardsDataManager.GetInstance().MoneyID, MoneyRewardsDataManager.GetInstance().MoneyCount);
                    return;
                }

                SystemNotifyManager.SystemNotify(7023, () =>
                 {
                     MoneyRewardsDataManager.GetInstance().SendAddParty(_onAddPartyOK);
                 }, 
                 null,
                new object[] { MoneyRewardsDataManager.GetInstance().MoneyCount, MoneyRewardsDataManager.GetInstance().MoneyName }
                );
                return;
            }

            ActivityDungeonDataManager.GetInstance().mIsLimitActivityRedPoint = false;
            
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshLimitTimeState);
            _onAddParty();
        }

        void _onAddPartyOK()
        {
            _UpdateStage();
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc13"));
        }

        void _onAddParty()
        {
            MoneyRewardsDataManager.GetInstance().GotoPvpFight();
            frameMgr.CloseFrame(this);
        }

        protected override void _OnCloseFrame()
        {
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsPoolsMoneyChanged, _OnMoneyRewardsPoolsMoneyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsPlayerCountChanged, _OnMoneyRewardsPlayerCountChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsAwardListChanged, _OnOnMoneyRewardsAwardListChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsSelfResultChanged, _OnMoneyRewardsSelfResultChanged);
        }
    }
}