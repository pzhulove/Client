using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    public class GuildDungeonBossKillRankListFrame : ClientFrame
    {
        #region val
        DOTweenAnimation hideAnimation = null;
        DOTweenAnimation showAnimation = null;

        const int maxRankItemNum = 5;

        private const string rankImagePath = "UI/Image/Packed/p_UI_Gonghuifuben.png:UI_Gonghuifuben_Paiming_0{0}";
        #endregion

        #region ui bind
        private GameObject itemTemplate = null;
        private GameObject itemListParent = null;
        private GameObject zeroTips = null;
        private Button hide = null;
        private Button show = null;
        private GameObject root = null;
        private DOTweenAnimation rootAnimation = null;
        private Button help = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonBossKillRankList";
        }

        protected override void _OnOpenFrame()
        {           
            BindUIEvent();

            _UpdateRankList();
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            itemTemplate = mBind.GetGameObject("itemTemplate");
            itemListParent = mBind.GetGameObject("itemListParent");
            zeroTips = mBind.GetGameObject("zeroTips");

            hide = mBind.GetCom<Button>("hide");
            hide.SafeRemoveAllListener();
            hide.SafeAddOnClickListener(() => 
            {
                if(hideAnimation != null)
                {
                    hideAnimation.isActive = true;
                    if(hideAnimation.tween == null)
                    {
                        hideAnimation.CreateTween();
                    }

                    hideAnimation.DORestart();
                }              
            });

            show = mBind.GetCom<Button>("show");
            show.SafeRemoveAllListener();
            show.SafeAddOnClickListener(() => 
            {
                if (showAnimation != null)
                {
                    showAnimation.isActive = true;
                    if (showAnimation.tween == null)
                    {
                        showAnimation.CreateTween();
                    }

                    showAnimation.DORestart();
                }
            });

            root = mBind.GetGameObject("root");
            if(root != null)
            {
                DOTweenAnimation[] anis = root.GetComponents<DOTweenAnimation>();
                if(anis != null)
                {
                    for(int i = 0;i < anis.Length;i++)
                    {
                        if(anis[i].id == "hide")
                        {
                            hideAnimation = anis[i];
                        }
                        else if(anis[i].id == "show")
                        {
                            showAnimation = anis[i];
                        }
                    }
                }
            }

            rootAnimation = mBind.GetCom<DOTweenAnimation>("rootAnimation");
            help = mBind.GetCom<Button>("help");
            help.SafeRemoveAllListener();
            help.SafeAddOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildDungeonAuctionAwardShowFrame>();
            });
        }

        protected override void _unbindExUI()
        {
            itemTemplate = null;
            itemListParent = null;
            zeroTips = null;

            hide = null;
            show = null;

            root = null;
            rootAnimation = null;
            help = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
        }

        void _UpdateRankList()
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data == null)
            {
                return;
            }      

            if (itemListParent != null && itemTemplate != null)
            {
                for (int i = 0; i < itemListParent.transform.childCount; ++i)
                {
                    GameObject go = itemListParent.transform.GetChild(i).gameObject;
                    GameObject.Destroy(go);
                }

                int iRank = 0;
                for (int i = 0; i < data.guildDungeonClearGateInfos.Count && i < maxRankItemNum; i++)
                {
                    GuildDataManager.GuildDungeonClearGateInfo info = data.guildDungeonClearGateInfos[i];
                    if (info == null)
                    {
                        continue;
                    }

                    GameObject goCurrent = GameObject.Instantiate(itemTemplate.gameObject);
                    Utility.AttachTo(goCurrent, itemListParent);
                    goCurrent.CustomActive(true);

                    ComCommonBind bind = goCurrent.GetComponent<ComCommonBind>();
                    if (bind != null)
                    {                        
                        StaticUtility.SafeSetText(bind, "guild", info.guildName);
                        StaticUtility.SafeSetText(bind, "timeUse", Function.GetLastsTimeStr(info.spendTime));

                        if (iRank <= 2)
                        {
                            StaticUtility.SafeSetVisible<Image>(bind, "rankImage", true);
                            StaticUtility.SafeSetVisible<Text>(bind, "rank", false);
                            StaticUtility.SafeSetImage(bind, "rankImage", string.Format(rankImagePath, iRank + 1));                            
                        }
                        else
                        {
                            StaticUtility.SafeSetVisible<Image>(bind, "rankImage", false);
                            StaticUtility.SafeSetVisible<Text>(bind, "rank", true);
                            StaticUtility.SafeSetText(bind, "rank", (i + 1).ToString());
                        }

                        StaticUtility.SafeSetVisible<Image>(bind, "bg", (iRank % 2) != 0);
                        StaticUtility.SafeSetVisible<Image>(bind, "bg2", (iRank % 2) == 0);

                        iRank++;
                    }         
                }

                for(int i = System.Math.Min(maxRankItemNum,data.guildDungeonClearGateInfos.Count);i < maxRankItemNum;i++)
                {
                    GameObject goCurrent = GameObject.Instantiate(itemTemplate.gameObject);
                    Utility.AttachTo(goCurrent, itemListParent);
                    goCurrent.CustomActive(true);

                    ComCommonBind bind = goCurrent.GetComponent<ComCommonBind>();
                    if (bind != null)
                    {
                       
                        StaticUtility.SafeSetText(bind, "guild", "暂无");
                        StaticUtility.SafeSetText(bind, "timeUse", "暂无");

                        if (iRank <= 2)
                        {
                            StaticUtility.SafeSetVisible<Image>(bind, "rankImage", true);
                            StaticUtility.SafeSetVisible<Text>(bind, "rank", false);
                            StaticUtility.SafeSetImage(bind, "rankImage", string.Format(rankImagePath, iRank + 1));                            
                        }
                        else
                        {
                            StaticUtility.SafeSetVisible<Image>(bind, "rankImage", false);
                            StaticUtility.SafeSetVisible<Text>(bind, "rank", true);
                            StaticUtility.SafeSetText(bind, "rank", (i + 1).ToString());
                        }

                        StaticUtility.SafeSetVisible<Image>(bind, "bg", (iRank % 2) != 0);
                        StaticUtility.SafeSetVisible<Image>(bind, "bg2", (iRank % 2) == 0);

                        iRank++;
                    }
                }

                //zeroTips.CustomActive(data.guildDungeonClearGateInfos.Count == 0);
            }

            return;
        }

        #endregion

        #region ui event 

        void _OnUpdateActivityData(UIEvent uiEvent)
        {
            _UpdateRankList();
        }

        #endregion
    }
}
