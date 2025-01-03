using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;
using Protocol;
using Network;

namespace GameClient
{
    class MoneyRewardsRankFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MoneyRewards/MoneyRewardsRankFrame";
        }

        public static void CommandOpen(object argv)
        {
            _RequestRanklist((int)SortListType.SORTLIST_PREMIUM_LEAGUE, 0, 20);
        }

        [UIControl("ScrollView", typeof(ComUIListScript))]
        ComUIListScript scripts;
        [UIControl("RankItemFixed", typeof(ComMoneyRewaradsRankItem))]
        ComMoneyRewaradsRankItem rankItemSelf;

        List<int> m_searched_list = new List<int>();

        protected override void _OnOpenFrame()
        {
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });

            if (null != scripts)
            {
                scripts.Initialize();
                scripts.onBindItem = (GameObject go) =>
                {
                    if (null != go)
                    {
                        return go.GetComponent<ComMoneyRewaradsRankItem>();
                    }
                    return null;
                };
                scripts.onItemVisiable = (ComUIListElementScript item) =>
                {
                    var datas = MoneyRewardsDataManager.GetInstance().RankItems;
                    var validCount = MoneyRewardsDataManager.GetInstance().getValidRankCount;
                    if (null != item && item.m_index >= 0 && item.m_index < datas.Length && item.m_index < validCount)
                    {
                        var script = item.gameObjectBindScript as ComMoneyRewaradsRankItem;
                        if (null != script)
                        {
                            script.OnItemVisible(datas[item.m_index]);

                            if(item.m_index == validCount - 1 && validCount < 100)
                            {
                                if(!m_searched_list.Contains(validCount))
                                {
                                    m_searched_list.Add(validCount);
                                    _RequestRanklist((int)SortListType.SORTLIST_PREMIUM_LEAGUE, validCount, 20);
                                }
                            }
                        }
                    }
                };
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsRankListChanged, _OnMoneyRewardsRankListChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsSelfResultChanged, _OnMoneyRewardsSelfResultChanged);

            _UpdateRankItems();
            _UpdateSelfRankItem();
        }

        public static void _RequestRanklist(int a_type, int a_startIndex, int a_count,bool bOpenRankFrame = true,UnityEngine.Events.UnityAction callback = null)
        {
            WorldSortListReq msg = new WorldSortListReq();
            msg.type = SortListDecoder.MakeSortListType((SortListType)a_type);
            msg.start = (ushort)a_startIndex;
            msg.num = (ushort)a_count;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait(WorldSortListRet.MsgID, msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }
                int pos = 0;
                BaseSortList arrRecods = SortListDecoder.Decode(msgRet.bytes, ref pos, msgRet.bytes.Length);
                for(int i = 0; i < arrRecods.entries.Count; ++i)
                {
                    var current = arrRecods.entries[i] as MoneyRewardsSortListEntry;
                    if (null != current)
                    {
                        if (current.ranking >= 1 && current.ranking <= MoneyRewardsDataManager.GetInstance().RankItems.Length)
                        {
                            if (null == MoneyRewardsDataManager.GetInstance().RankItems[current.ranking - 1])
                            {
                                MoneyRewardsDataManager.GetInstance().RankItems[current.ranking - 1] = new MoneyRewaradsRankItemData();
                            }
                            var assignData = MoneyRewardsDataManager.GetInstance().RankItems[current.ranking - 1];
                            if (null != assignData)
                            {
                                assignData.name = current.name;
                                assignData.rank = current.ranking;
                                assignData.score = (int)current.score;
                                assignData.maxScore = (int)current.maxScore;
                            }
                        }
                    }
                }

                if (true)
                {
                    var assignData = MoneyRewardsDataManager.GetInstance().RankItemSelf;
                    if (null != assignData)
                    {
                        assignData.name = PlayerBaseData.GetInstance().Name;
                        if (null != arrRecods.selfEntry)
                        {
                            assignData.rank = (arrRecods.selfEntry as MoneyRewardsSortListEntry).ranking;
                            MoneyRewardsDataManager.GetInstance().Rank = (arrRecods.selfEntry as MoneyRewardsSortListEntry).ranking;
                            assignData.score = (int)(arrRecods.selfEntry as MoneyRewardsSortListEntry).score;
                            MoneyRewardsDataManager.GetInstance().Score = (int)(arrRecods.selfEntry as MoneyRewardsSortListEntry).score;
                            assignData.maxScore = (int)(arrRecods.selfEntry as MoneyRewardsSortListEntry).maxScore;
                            MoneyRewardsDataManager.GetInstance().MaxScore = (int)(arrRecods.selfEntry as MoneyRewardsSortListEntry).maxScore;
                        }
                        else
                        {
                            assignData.rank = 999;
                            MoneyRewardsDataManager.GetInstance().Rank = 999;
                        }
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsSelfResultChanged);
                }

                if(bOpenRankFrame)
                {
                    if (ClientSystemManager.GetInstance().IsFrameOpen<MoneyRewardsRankFrame>())
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsRankListChanged);
                    }
                    else
                    {
                        ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsRankFrame>(FrameLayer.Middle, null);
                    }
                }

                if (null != callback)
                {
                    callback.Invoke();
                }
            });
        }

        void _UpdateRankItems()
        {
            if(null != scripts)
            {
                var datas = MoneyRewardsDataManager.GetInstance().RankItems;
                var validCount = MoneyRewardsDataManager.GetInstance().getValidRankCount;
                scripts.SetElementAmount(validCount);
            }
        }

        void _UpdateSelfRankItem()
        {
            if(null != rankItemSelf)
            {
                rankItemSelf.OnItemVisible(MoneyRewardsDataManager.GetInstance().RankItemSelf);
            }
        }

        protected override void _OnCloseFrame()
        {
            if (null != scripts)
            {
                scripts.onBindItem = null;
                scripts.onItemVisiable = null;
                scripts = null;
            }

            m_searched_list.Clear();
            MoneyRewardsDataManager.GetInstance().ClearRankItems();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsRankListChanged, _OnMoneyRewardsRankListChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsSelfResultChanged, _OnMoneyRewardsSelfResultChanged);
        }

        void _OnMoneyRewardsRankListChanged(UIEvent uiEvent)
        {
            _UpdateRankItems();
        }

        void _OnMoneyRewardsSelfResultChanged(UIEvent uiEvent)
        {
            _UpdateSelfRankItem();
        }
    }
}