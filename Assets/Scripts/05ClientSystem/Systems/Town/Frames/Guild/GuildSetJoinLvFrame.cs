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
    public class GuildSetJoinLvFrame : ClientFrame
    {
        public class GuildJovLvItem
        {
            public GuildSetJoinLvItem GuildSetJoinLvItem;
            public RectTransform RectTransform;
            public string ValueLv;
        }


        private string mTargetLv;
        private Button mBtnClose;

        private Button mBtnOK;

        private ComUIListScript mUIList;

        private RectTransform mTargetPosRect;

        private Dictionary<int, GuildJovLvItem> mAllItemDic = new Dictionary<int, GuildJovLvItem>();

        private RectTransform mContent;

        private int mMinLv = 18;

        private int mMaxLv = 60;

        private int mMinPosY = -129;

        private int mMaxPosY = 3658;

        private List<string> mLvs = new List<string>();
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildSetJoinLv";
        }

        protected override void _OnOpenFrame()
        {
            _Init();
        }

        protected override void _OnCloseFrame()
        {
            _ClearData();
        }

        protected override void _bindExUI()
        {
            mBtnClose = mBind.GetCom<Button>("BtnClose");
            mBtnClose.SafeAddOnClickListener(_OnCloseBtnClick);

            mBtnOK = mBind.GetCom<Button>("btOK");
            mBtnOK.SafeAddOnClickListener(_OnOkBtnClick);

            mUIList = mBind.GetCom<ComUIListScript>("LvlList");

            mTargetPosRect = mBind.GetCom<RectTransform>("TargetPos");

            mContent = mBind.GetCom<RectTransform>("Content");
        }

        protected override void _unbindExUI()
        {
            mBtnClose.SafeRemoveOnClickListener(_OnCloseBtnClick);
            mBtnClose = null;

            mBtnOK.SafeRemoveOnClickListener(_OnOkBtnClick);
            mBtnOK = null;

            mUIList = null;

            mTargetPosRect = null;

            mContent = null;
        }        
      
        private void _Init()
        {
            mMaxLv = GuildDataManager.GetJoinGuildMaxLevel();
            mMinLv = GuildDataManager.GetJoinGuildMinLv();
            for (int i = mMinLv; i <= mMaxLv; i++)
            {
                mLvs.Add(i.ToString());
            }


            if (mUIList == null) return;
            if(!mUIList.IsInitialised())
            {
                mUIList.Initialize();
                mUIList.onItemVisiable = _OnItemVisiable;
                mUIList.OnItemRecycle = _OnItemRecycle;
                mUIList.OnScrollViewDrag = _OnScrollViewDrag;
            }
            mUIList.SetElementAmount(mLvs.Count);
          
        }
        

        private void _OnScrollViewDrag(Vector2 changeValue)
        {
            SetTarget();
        }
        
        
        private void _OnItemVisiable(ComUIListElementScript item)
        {
            if (item == null|| mLvs==null) return;
            if (item.m_index < 0 || item.m_index >= mLvs.Count) return;
            GuildSetJoinLvItem guildSetJoinLvItem = item.GetComponent<GuildSetJoinLvItem>();
            if (guildSetJoinLvItem == null) return;
            guildSetJoinLvItem.Init(mLvs[item.m_index]);

            RectTransform rectTransform = guildSetJoinLvItem.GetRectTransform();
            GuildJovLvItem guildJovLvItem = new GuildJovLvItem();
            guildJovLvItem.RectTransform = rectTransform;
            guildJovLvItem.GuildSetJoinLvItem = guildSetJoinLvItem;
            guildJovLvItem.ValueLv = mLvs[item.m_index];
            if (!mAllItemDic.ContainsKey(item.m_index))
            {
                mAllItemDic.Add(item.m_index, guildJovLvItem);
            }
            else
            {
                mAllItemDic[item.m_index] = guildJovLvItem;
            }
        }

        private void _OnItemRecycle(ComUIListElementScript item)
        {

            if (item == null || mLvs == null) return;
            if (item.m_index < 0 || item.m_index >= mLvs.Count) return;
            GuildSetJoinLvItem guildSetJoinLvItem = item.GetComponent<GuildSetJoinLvItem>();
            if (guildSetJoinLvItem == null) return;
            guildSetJoinLvItem.SetH(false);
            if (mAllItemDic.ContainsKey(item.m_index))
            {
                mAllItemDic.Remove(item.m_index);
            }
        }

        private void SetTarget()
        {
            if (mTargetPosRect == null||mContent==null) return;
            var iter = mAllItemDic.GetEnumerator();
            GuildJovLvItem guildJovLvItem = null;
            while (iter.MoveNext())
            {
                var value = iter.Current.Value;
                if (value == null) continue;
                value.GuildSetJoinLvItem?.SetH(false);
                if (mTargetPosRect.ContainsRect(value.RectTransform))
                {
                    guildJovLvItem = value;
                }
            }
            if(guildJovLvItem != null)
            {
                guildJovLvItem.GuildSetJoinLvItem?.SetH(true);
                mTargetLv = guildJovLvItem.ValueLv;
            }
            if(mContent.anchoredPosition.y>mMaxPosY)
            {
                mContent.anchoredPosition = new Vector2(mContent.anchoredPosition.x, mMaxPosY);
            }
            else if (mContent.anchoredPosition.y <mMinPosY)
            {
                mContent.anchoredPosition = new Vector2(mContent.anchoredPosition.x, mMinPosY);
            }
        }
        private void _OnOkBtnClick()
        {
            GuildDataManager.GetInstance().SendWorldGuildSetJoinLevelReq(Utility.ToUInt(mTargetLv));
            frameMgr.CloseFrame(this);
        }

        private void _OnCloseBtnClick()
        {
            frameMgr.CloseFrame(this);
        }


        private void _ClearData()
        {
            mAllItemDic.Clear();
            mMaxLv = 60;
        }
        
    }
}
