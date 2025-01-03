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
    public class GuildEmblemAttrShowFrame : ClientFrame
    {
        #region val

        List<int> emblemLvs = null; 

        #endregion

        #region ui bind

        private Button Close = null;
        private ComUIListScript attrsShow = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildEmblemAttrShow";
        }

        protected override void _OnOpenFrame()
        {           
            BindUIEvent();

            emblemLvs = new List<int>();
            int iMaxLv = GuildDataManager.GetInstance().GetMaxEmblemLv();
            for(int i = 0;i < iMaxLv;i++)
            {
                emblemLvs.Add(i + 1);
            }

            UpdateEmblemAttrs();           
        }

        protected override void _OnCloseFrame()
        {          
            emblemLvs = null;
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            Close = mBind.GetCom<Button>("Close");   
            Close.SafeSetOnClickListener(() => 
            {
                frameMgr.CloseFrame(this);
            });

            attrsShow = mBind.GetCom<ComUIListScript>("attrsShow");
        }

        protected override void _unbindExUI()
        {            
            Close = null;
            attrsShow = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
            
        }        

        void UpdateEmblemAttrs()
        {
            if(attrsShow == null)
            {
                return;
            }

            if(emblemLvs == null)
            {
                return;
            }

            attrsShow.Initialize();
            attrsShow.onBindItem = (go) => 
            {
                if (null != go)
                {
                    return go.GetComponent<GuildEmblemAttrShowItem>();
                }

                return null;
            };
        
            attrsShow.onItemVisiable = (item) => 
            {
                GuildEmblemAttrShowItem attrShowItem = item.gameObjectBindScript as GuildEmblemAttrShowItem;
                if (null != attrShowItem && item.m_index < emblemLvs.Count)
                {
                    attrShowItem.SetUp(emblemLvs[item.m_index]);
                }
            };

            attrsShow.m_scrollRect.StopMovement();
            attrsShow.SetElementAmount(emblemLvs.Count);

            AdjustSrollBarValue(GuildDataManager.GetInstance().GetEmblemLv());
        }
        
        // 适配滚动条的值
        // 具体需求是这样的
        // 1、徽记预览界面，未激活徽记时，默认显示第一条徽记属性
        // 2、已激活徽记时，当前属性下面还有属性，且不止一屏，则将当前激活的徽记属性显示在当前屏幕第一个位置；当徽记属性显示在最后一屏，则不处理
        void AdjustSrollBarValue(int iEmblemLv)
        {
            if(attrsShow == null || attrsShow.m_scrollRect == null)
            {
                return;
            }
     
            int iCount = Math.Max(1,iEmblemLv);

            RectTransform rectTransform = attrsShow.GetComponent<RectTransform>();
            float fValue = (iCount - 1) * ((attrsShow.contentSize.y + attrsShow.m_elementSpacing.y)  / attrsShow.m_elementAmount) / (attrsShow.contentSize.y - rectTransform.rect.height);
      
            attrsShow.m_scrollRect.verticalNormalizedPosition = 1.0f - fValue;
        }

        #endregion

        #region ui event       

        #endregion
    }
}
