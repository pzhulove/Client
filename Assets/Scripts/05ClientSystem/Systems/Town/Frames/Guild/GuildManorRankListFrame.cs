using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // GuildManorRankList
    public class GuildManorRankListFrame : GameFrame
    {
        #region inner define

        #endregion

        #region val
        List<GuildBattleTerrScoreRank> rankComUIListDatas = new List<GuildBattleTerrScoreRank>();
        BaseSortList sortList = null;
        #endregion

        #region ui bind
        ComUIListScript rankComUIList = null;
        Text testTxt = null;
        Button testBtn = null;
        Image testImg = null;
        Slider testSlider = null;
        Toggle testToggle = null;
        GameObject testGo = null;
        private GuildManorRankListItem myGuildRank = null;
        private GameObject myGuildRankRoot = null;

        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildManorRankList";
        }

        protected override void OnOpenFrame()
        {
            sortList = userData as BaseSortList;

            InitRankComUIList();
            UpdateRankComUIList();

            UpdateMyGuildRank();
        }

        protected override void OnCloseFrame()
        {
            sortList = null;
        }

        protected override void _bindExUI()
        {
            rankComUIList = mBind.GetCom<ComUIListScript>("rankComUIList");
            testTxt = mBind.GetCom<Text>("testTxt");

            testBtn = mBind.GetCom<Button>("testBtn");
            testBtn.SafeSetOnClickListener(() => 
            {

            });

            testImg = mBind.GetCom<Image>("testImg");

            testSlider = mBind.GetCom<Slider>("testSlider");
            testSlider.SafeSetValueChangeListener((value) => 
            {

            });

            testToggle = mBind.GetCom<Toggle>("testToggle");
            testToggle.SafeSetOnValueChangedListener((value) => 
            {

            });

            testGo = mBind.GetGameObject("testGo");

            myGuildRank = mBind.GetCom<GuildManorRankListItem>("myGuildRank");
            myGuildRankRoot = mBind.GetGameObject("myGuildRankRoot");
        }

        protected override void _unbindExUI()
        {
            rankComUIList = null;

            testTxt = null;
            testBtn = null;
            testImg = null;
            testSlider = null;
            testToggle = null;
            testGo = null;
            myGuildRank = null;
            myGuildRankRoot = null;
        }

        protected override void OnBindUIEvent()
        {
            BindUIEvent(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
        }

        protected override void OnUnBindUIEvent()
        {
            UnBindUIEvent(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
        }

        public override bool IsNeedUpdate()
        {
            return false;//false means _OnUpdate is invalid
        }

        protected override void _OnUpdate(float timeElapsed)
        {

        }

        #endregion

        #region method

        void InitRankComUIList()
        {
            if(rankComUIList == null)
            {
                return;
            }

            rankComUIList.Initialize();
            rankComUIList.onBindItem = (go) => 
            {
                return go;
            };

            rankComUIList.onItemVisiable = (go) => 
            {
                if(go == null)
                {
                    return;
                }

                if(rankComUIListDatas == null)
                {
                    return;
                }

                ComUIListTemplateItem comUIListItem = go.GetComponent<ComUIListTemplateItem>();
                if(comUIListItem == null)
                {
                    return;
                }

                if(go.m_index >= 0 && go.m_index < rankComUIListDatas.Count)
                {
                    comUIListItem.SetUp(rankComUIListDatas[go.m_index]);
                }                
            };          
        }

        int GetMaxGuildBattleTerrScoreRank()
        {
            int rank = 0;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildBattleScoreRankRewardTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildBattleScoreRankRewardTable adt = iter.Current.Value as GuildBattleScoreRankRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }                  

                    if (adt.ID >= rank)
                    {
                        rank = adt.ID;                        
                    }
                }
            }

            return rank;       
        }

        void CalcRankComUIListDatas()
        {
            rankComUIListDatas = new List<GuildBattleTerrScoreRank>();
            if(rankComUIListDatas == null)
            {
                return;
            }

            if(sortList == null)
            {
                return;
            }

            if(sortList.entries == null)
            {
                return;
            }

            for(int i = 0;i < sortList.entries.Count;i++)
            {
                rankComUIListDatas.Add(sortList.entries[i] as GuildBattleTerrScoreRank);
            }

            int maxCount = GetMaxGuildBattleTerrScoreRank();
            for(int i = rankComUIListDatas.Count;i < maxCount;i++)
            {
                rankComUIListDatas.Add(new GuildBattleTerrScoreRank()
                {
                    ranking = (ushort)(i + 1),
                });
            }

            rankComUIListDatas.Sort((a, b) => 
            {
                if(a == null || b == null)
                {
                    return 0;
                }

                return a.ranking.CompareTo(b.ranking);
            });
        }

        void UpdateRankComUIList()
        {
            if(rankComUIList == null)
            {
                return;
            }

            CalcRankComUIListDatas();
            if(rankComUIListDatas == null)
            {
                return;
            }

            rankComUIList.SetElementAmount(rankComUIListDatas.Count);            
        }
        
        void UpdateMyGuildRank()
        {
            if(myGuildRank == null)
            {
                return;
            }

            if(sortList == null)
            {
                return;
            }

            var rank = sortList.selfEntry as GuildBattleTerrScoreRank;
            if(rank == null)
            {
                return;
            }

            myGuildRank.SetUp(rank);         
            myGuildRankRoot.CustomActive(sortList.entries != null && sortList.entries.Count > 0);
        }
        #endregion

        #region ui event

        void _OnOnCountValueChange(UIEvent uiEvent)
        {           
            return;
        }

        #endregion
    }
}
