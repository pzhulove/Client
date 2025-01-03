using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;
using ProtoTable;
using Protocol;
using Network;
using System.Diagnostics;

namespace GameClient
{
    public interface ICommonConsume
    {
        void OnChange();

        void OnAdd();

        EUIEventID[] WatchEvents();

        ulong GetCount();

        ulong GetSumCount();
    }

    class BaseConsum
    {
        public ClientFrameBinder comFrameBinder = null;
        public void OnCloseLinkFrame()
        {
            if(comFrameBinder != null)
            {
                comFrameBinder.CloseFrame(true);
            }
        }
        public BaseConsum(ClientFrameBinder comFrameBinder)
        {
            this.comFrameBinder = comFrameBinder;
        }
        private ulong mSumCnt;
        public ulong sumCnt
        {
            get { return mSumCnt; }
            protected set { mSumCnt = value; }
        }

        private ulong mCnt;
        public ulong cnt 
        {
            get { return mCnt; }
            protected set { mCnt = value; }
        }

        protected EUIEventID[] mEvents = new EUIEventID[0];

        public EUIEventID[] WatchEvents()
        {
            return mEvents;
        }

        public ulong GetCount()
        {
            return cnt;
        }

        public ulong GetSumCount()
        {
            return sumCnt;
        }

        [Conditional("UNITY_EDITOR")]
        protected void _send_gm(string msg)
        {
            var req = new SceneChat
            {
                channel = 1,
                targetId = 0,
                word = string.Format("!!{0}", msg)
            };

            //CUDLR.Console.Log("send : gm " + req.word);

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }
    }

    class ItemConsume : BaseConsum, ICommonConsume
    {
        protected int mItemID;
        public ItemConsume(int id,ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mItemID = id;
            mEvents = new EUIEventID[]
            {
                EUIEventID.ItemTakeSuccess,
                EUIEventID.ItemCountChanged,
            };
        }

        public void OnChange()
        {
            cnt = (ulong)ItemDataManager.GetInstance().GetOwnedItemCount(mItemID, false);;
        }

        public void OnAdd()
        {
            ItemComeLink.OnLink(mItemID,0,false, OnCloseLinkFrame, false,true);
        }
    }

    class GoldConsume : BaseConsum, ICommonConsume
    {
        public GoldConsume(ClientFrameBinder comFrameBinder):base(comFrameBinder)
        {
            mEvents = new EUIEventID[]
            {
                EUIEventID.GoldChanged
            };
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().Gold;
        }
        public void OnAdd()
        {
            _send_gm("addgold num=1000");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.GOLD),0, false, OnCloseLinkFrame, false, true);
        }
    }

    class BindGoldConsume : BaseConsum, ICommonConsume
    {
        public BindGoldConsume(ClientFrameBinder comFrameBinder):base(comFrameBinder)
        {
            mEvents = new EUIEventID[]
            {
                EUIEventID.BindGoldChanged
            };
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().BindGold;
        }

        public void OnAdd()
        {
            _send_gm("addbindgold num=1000");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD),0, false, OnCloseLinkFrame, false, true);
        }
    }

    class GuildContributionConsume : BaseConsum, ICommonConsume
    {
        public GuildContributionConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[]
            {
                EUIEventID.PlayerDataGuildUpdated
            };
        }

        public void OnChange()
        {
			cnt = (ulong)PlayerBaseData.GetInstance().guildContribution;
        }

        public void OnAdd()
        {
            _send_gm("addfund num= 10000");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.GuildContri),0, false, OnCloseLinkFrame, false, true);
        }
    }

    class GoldJarScoreConsume : BaseConsum, ICommonConsume
    {
        public GoldJarScoreConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[]
            {
                EUIEventID.GoldJarScoreChanged
            };
        }

        public void OnChange()
        {
            cnt = (ulong)PlayerBaseData.GetInstance().GoldJarScore;
        }

        public void OnAdd()
        {
            _send_gm("addgoldjarpoint num=10000");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.GoldJarPoint), 0, false, OnCloseLinkFrame, false, true);
        }
    }

    class MagicJarScoreConsume : BaseConsum, ICommonConsume
    {
        public MagicJarScoreConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[]
            {
                EUIEventID.MagicJarScoreChanged
            };
        }

        public void OnChange()
        {
            cnt = (ulong)PlayerBaseData.GetInstance().MagicJarScore;
        }

        public void OnAdd()
        {
            _send_gm("addmagjarpoint num=10000");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.MagicJarPoint), 0, false, OnCloseLinkFrame, false, true);
        }
    }

    class ExpConsume : BaseConsum, ICommonConsume
    {
        public ExpConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[]{ EUIEventID.ExpChanged };
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().Exp;
        }

        public void OnAdd()
        {
            _send_gm("addexp num=1000");
        }
    }

    class PointConsume : BaseConsum, ICommonConsume
    {
        public PointConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] {EUIEventID.TicketChanged};
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().Ticket;
        }

        public void OnAdd()
        {
            _send_gm("addpoint num=1000");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT),0, false, OnCloseLinkFrame, false, true);
        }
    }


    class BindPointConsume : BaseConsum, ICommonConsume
    {
        public BindPointConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] {EUIEventID.BindTicketChanged};
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().BindTicket;
        }
        public void OnAdd()
        {
            _send_gm("addbindpoint num=1000");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT),0, false, OnCloseLinkFrame, false, true);
        }
    }

    class ResurrectionCcurrencyComsume : BaseConsum, ICommonConsume
    {
        public ResurrectionCcurrencyComsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] {EUIEventID.AliveCoinChanged};
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().AliveCoin;
        }

        public void OnAdd()
        {
            _send_gm("additem id=600000006 num=10");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.ResurrectionCcurrency), 0,false, OnCloseLinkFrame, false, true);
        }
    }

    class WarriorsoulConsume : BaseConsum, ICommonConsume
    {
        public WarriorsoulConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] {EUIEventID.WarriorSoulChanged};
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().WarriorSoul;
        }
        public void OnAdd()
        {
            _send_gm("addwarriorsoul num=100");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.WARRIOR_SOUL),0, false, OnCloseLinkFrame, false, true);
        }
    }

    class DuelcoinConsume : BaseConsum, ICommonConsume
    {
        public DuelcoinConsume(ClientFrameBinder comFrameBinder) : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] {EUIEventID.PkCoinChanged};
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().uiPkCoin;
        }

        public void OnAdd()
        {
            _send_gm("addpkcoin num=100");
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.DUEL_COIN),0, false, OnCloseLinkFrame, false, true);
        }
    }

    class BaseActivityConsume : BaseConsum, ICommonConsume
    {
        protected IActivityConsumeData mData;

        public BaseActivityConsume(int id,ClientFrameBinder comFrameBinder = null) : base(comFrameBinder)
        {
            mData   = new NormalActivityConsumeData(id);
            mEvents = new EUIEventID[] {EUIEventID.OnCountValueChange};
        }

        public void OnChange()
        {
            cnt    = (ulong)mData.GetLeftCount();
            sumCnt = (ulong)mData.GetSumCount();
        }

        public void OnAdd()
        {
            if (mData.IsCanBuyCount())
            {
                int costId       = mData.GetCostItemID();
                int costCount    = mData.GetCostItemCount();
                byte subType     = mData.GetCostItemType();
                int buyTimes     = (int)mData.GetHasBuyCount();
                int leftBuyTimes = (int)mData.GetLeftBuyCount();

                GameFrameWork.instance.StartCoroutine(_quickBuyTimes(costId, costCount, leftBuyTimes, subType));
            }
            else 
            {
                SystemNotifyManager.SystemNotify(8200);
            }
        }

        private IEnumerator _quickBuyTimes(int id, int count, int leftcount, byte type)
        {
            QuickBuyTimesFrame frame = ClientSystemManager.instance.OpenFrame<QuickBuyTimesFrame>() as QuickBuyTimesFrame;;
            if (frame != null)
            {
                frame.SetLeftCount(leftcount);
                frame.SetCostItem(id, count);

                while (frame.state == QuickBuyTimesFrame.eState.None)
                {
                    yield return Yielders.EndOfFrame;
                }

                if (frame.state == QuickBuyTimesFrame.eState.Success)
                {
                    yield return _buy(type);
                }
            }
        }

        private IEnumerator _buy(byte subType)
        {
            MessageEvents msg = new MessageEvents();
            SceneDungeonBuyTimesRes res = new SceneDungeonBuyTimesRes();
            SceneDungeonBuyTimesReq req = new SceneDungeonBuyTimesReq
            {
                subType = subType
            };

            yield return MessageUtility.Wait(ServerType.GATE_SERVER, msg, req, res, true);

            if (msg.IsAllMessageReceived())
            {
                if (res.result != 0)
                {
                    SystemNotifyManager.SystemNotify((int)res.result);
                }
            }
        }
    }

    class DeadTowerCountConsume : BaseActivityConsume
    {
        public DeadTowerCountConsume(int id) : base(id)
        {
            mData = new DeadTowerActivityConsumeData(id);
            mEvents = new EUIEventID[] {EUIEventID.OnCountValueChange};
        }

        public new void OnAdd()
        {
            _send_gm("reset");
        }
    }

    class FinalTestCountConsume : BaseActivityConsume
    {
        public FinalTestCountConsume(int id) : base(id)
        {
            mData = new FinalTestActivityConsumeData(id);
            mEvents = new EUIEventID[] { EUIEventID.OnCountValueChange };
        }        
    }
    ///深渊
    class HellTiketConsume : BaseConsum, ICommonConsume
    {
        public HellTiketConsume(ClientFrameBinder comFrameBinder) :base(comFrameBinder)
        {
            mEvents = new EUIEventID[] {EUIEventID.OnCountValueChange};
        }

        public void OnChange()
        {
        }
        public void OnAdd()
        {
            _send_gm("reset");
        }
    }


    /// 体力
    class FatigueConsume : BaseConsum, ICommonConsume
    {
        public FatigueConsume(ClientFrameBinder comFrameBinder):base(comFrameBinder)
        {
            mEvents = new EUIEventID[]{EUIEventID.FatigueChanged};
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().fatigue;
        }

        public void OnAdd()
        {
            _send_gm("addfatigue num=10");
            ClientSystemManager.instance.OpenFrame<ComsumeFatigueAddFrame>();
        }
    }

    //佣兵勋章
    class BlessCrystalConsume : BaseConsum, ICommonConsume
    {
        public BlessCrystalConsume(ClientFrameBinder comFrameBinder):base(comFrameBinder)
        {
            mEvents = new EUIEventID[] { EUIEventID.OnAdventureTeamBlessCrystalCountChanged };
        }

        public void OnChange()
        {
            cnt = AdventureTeamDataManager.GetInstance().GetAdventureTeamBlessCrystalCount();
        }

        public void OnAdd()
        {
            AdventureTeamInformationFrame.OpenTabFrame(AdventureTeamMainTabType.BaseInformation);
        }
    }

	//佣兵赏金
    class BountyConsume : BaseConsum, ICommonConsume
    {
        public BountyConsume(ClientFrameBinder comFrameBinder):base(comFrameBinder)
        {
            mEvents = new EUIEventID[] { EUIEventID.OnAdventureTeamBountyCountChanged };
        }

        public void OnChange()
        {
            cnt = AdventureTeamDataManager.GetInstance().GetAdventureTeamBountyCount();
        }

        public void OnAdd()
        {
            var bountyItem = AdventureTeamDataManager.GetInstance().BountyModel;
            if (bountyItem == null)
            {
                return;
            }
            ItemComeLink.OnLink((int)bountyItem.itemTableId, 0, false, OnCloseLinkFrame, false, true);

            //AdventureTeamInformationFrame.OpenTabFrame(AdventureTeamMainTabType.CharacterExpedition);
        }
    }
    
    //成长药剂
    class PassBlessConsume : BaseConsum, ICommonConsume
    {
        public PassBlessConsume(ClientFrameBinder comFrameBinder)
            : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] { EUIEventID.OnAdventureTeamBountyCountChanged };
        }

        public void OnChange()
        {
            cnt = AdventureTeamDataManager.GetInstance().GetAdventureTeamPassBlessCount();
        }

        public void OnAdd()
        {
            AdventureTeamInformationFrame.OpenTabFrame(AdventureTeamMainTabType.PassingBless);
        }
    }

    /// <summary>
    /// 招募硬币
    /// </summary>
    class WarriorRecruitConsume : BaseConsum, ICommonConsume
    {
        public WarriorRecruitConsume(ClientFrameBinder comFrameBinder)
            : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] { EUIEventID.AccountSpecialItemUpdate };
        }

        public void OnChange()
        {
            cnt = AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_COUNTER_HIRE_COIN);
        }

        public void OnAdd()
        {
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.ST_HIRE_COIN), 0, false, OnCloseLinkFrame, false, true);
        }
    }

    /// <summary>
    /// 商城积分
    /// </summary>
    class MallPointConsume : BaseConsum, ICommonConsume
    {
        public MallPointConsume(ClientFrameBinder comFrameBinder)
            : base(comFrameBinder)
        {
            mEvents = new EUIEventID[] { EUIEventID.PlayerMallPointUpdate };
        }

        public void OnChange()
        {
            cnt = PlayerBaseData.GetInstance().IntergralMallTicket;
        }

        public void OnAdd()
        {
            ItemComeLink.OnLink(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.ST_MALL_POINT), 0, false, OnCloseLinkFrame, false, true);
        }
    }

    class ConsumeFactory
    {
        public static ICommonConsume CreateByItemID(int a_nItemID, ClientFrameBinder comFrameBinder)
        {
            ItemTable table = TableManager.instance.GetTableItem<ItemTable>(a_nItemID);
            if (null != table)
            {
                switch (table.SubType)
                {
                    case ItemTable.eSubType.GOLD:
                        return new GoldConsume(comFrameBinder);
                    case ItemTable.eSubType.BindGOLD:
                        return new BindGoldConsume(comFrameBinder);
                    case ItemTable.eSubType.POINT:
                        return new PointConsume(comFrameBinder);
                    case ItemTable.eSubType.BindPOINT:
                        return new BindPointConsume(comFrameBinder);
                    case ItemTable.eSubType.EXP:
                        return new ExpConsume(comFrameBinder);
                    case ItemTable.eSubType.ResurrectionCcurrency:
                        return new ResurrectionCcurrencyComsume(comFrameBinder);
                    case ItemTable.eSubType.WARRIOR_SOUL:
                        return new WarriorsoulConsume(comFrameBinder);
                    case ItemTable.eSubType.DUEL_COIN:
                        return new DuelcoinConsume(comFrameBinder);
                    case ItemTable.eSubType.GuildContri:
                        return new GuildContributionConsume(comFrameBinder);
                    case ItemTable.eSubType.GoldJarPoint:
                        return new GoldJarScoreConsume(comFrameBinder);
                    case ItemTable.eSubType.MagicJarPoint:
                        return new MagicJarScoreConsume(comFrameBinder);
                    case ItemTable.eSubType.ST_BLESS_CRYSTAL_VALUE:
                        return new BlessCrystalConsume(comFrameBinder);
                    case ItemTable.eSubType.ST_GOLD_REWARD_VALUE:
                        return new BountyConsume(comFrameBinder);
                    case ItemTable.eSubType.ST_INHERIT_BLESS_VALUE:
                        return new PassBlessConsume(comFrameBinder);
                    case ItemTable.eSubType.ST_HIRE_COIN:
                        return new WarriorRecruitConsume(comFrameBinder);
                    case ItemTable.eSubType.ST_MALL_POINT:
                        return new MallPointConsume(comFrameBinder);
                }

                return new ItemConsume(a_nItemID, comFrameBinder);
            }

            
            return null;
        }


        public static ICommonConsume CreateByCountType(ComCommonConsume.eCountType type, object arg = null,ClientFrameBinder comFrameBinder = null)
        {
            switch (type)
            {
                case ComCommonConsume.eCountType.Fatigue:
                    return new FatigueConsume(comFrameBinder);
                case ComCommonConsume.eCountType.HellTiketCount:
                    return new HellTiketConsume(comFrameBinder);
                case ComCommonConsume.eCountType.NorthCount:
                    return new BaseActivityConsume((int)arg, comFrameBinder);
                case ComCommonConsume.eCountType.MouCount:
                    return new BaseActivityConsume((int)arg, comFrameBinder);
                case ComCommonConsume.eCountType.DeadTower:
                    return new DeadTowerCountConsume((int)arg);
                case ComCommonConsume.eCountType.Final_Test:
                    return new FinalTestCountConsume((int)arg);
            }

            return null;
        }
    }
}

