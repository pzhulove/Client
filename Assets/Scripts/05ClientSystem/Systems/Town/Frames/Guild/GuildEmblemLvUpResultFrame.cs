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
    public class GuildEmblemLvUpResultFrame : ClientFrame
    {
        #region val    

        #endregion

        #region ui bind
        private Button close = null;
        private Image icon = null;
        private Text name = null;
        private Image ImageTitle = null;
        private GuildEmblemAttrItem emblemLvNow = null;
        private Sprite mActiveSp = null;
        private Sprite mLvUpSp = null;

        float fCreateTime = 0.0f;
        const float fDelayTime = 2.0f;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildEmblemLvUpResult";
        }

        protected override void _OnOpenFrame()
        {
            fCreateTime = Time.realtimeSinceStartup;

            BindUIEvent();
            UpdateEmblemInfo();
        }

        protected override void _OnCloseFrame()
        {
            fCreateTime = 0.0f;

            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            close = mBind.GetCom<Button>("close");
            close.SafeRemoveAllListener();
            close.SafeAddOnClickListener(() => 
            {
                if(Time.realtimeSinceStartup - fCreateTime < fDelayTime)
                {
                    return;
                }

                frameMgr.CloseFrame(this);
            });

            icon = mBind.GetCom<Image>("icon");
            name = mBind.GetCom<Text>("name");
            ImageTitle = mBind.GetCom<ImageEx>("Title");
            emblemLvNow = mBind.GetCom<GuildEmblemAttrItem>("emblemLvNow");
        }

        protected override void _unbindExUI()
        {
            close = null;
            icon = null;
            name = null;
            ImageTitle = null;
            emblemLvNow = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
           
        }

        private string GetColorString(string text, string color)
        {
            return TR.Value("common_color_text", "#" + color, text);
        }

        void UpdateEmblemInfo()
        {
            int iEmblemLv = GuildDataManager.GetInstance().GetEmblemLv();
            GuildEmblemTable guildEmblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(iEmblemLv);
            if (guildEmblemTable == null)
            {
                return;
            }

            icon.SafeSetImage(guildEmblemTable.iconPath);
            name.SafeSetText(guildEmblemTable.name);

            if (ImageTitle != null)
            {
                if (iEmblemLv == 1)
                {
                    mBind.GetSprite("jihuo", ref ImageTitle);
                }
                else
                {
                    mBind.GetSprite("shengji", ref ImageTitle);
                }
            }

            if(emblemLvNow != null)
            {
                emblemLvNow.SetUp(GuildDataManager.GetInstance().GetEmblemLv());
            }
        }   

        #endregion

        #region ui event       

        #endregion
    }
}
