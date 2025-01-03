using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Scripts.UI;
using System;

namespace GameClient
{
    class NewMessageNoticeScript
    {
        public Text title;
        public Text message;
        public Text forwardText;
    }
    class NewMessageNoticeFrame : ClientFrame
    {
        private ComCommonBind mComBind;
        private ComUIListScript  mListScript;

        private UInt64 mDataVersion  = UInt64.MaxValue;
        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            var newMsgNoticeMgr = NewMessageNoticeManager.GetInstance();
            if(mDataVersion != newMsgNoticeMgr.DataVersion)
            {
                newMsgNoticeMgr.MarkAllRead();
                mDataVersion = newMsgNoticeMgr.DataVersion;
                mListScript.SetElementAmount(newMsgNoticeMgr.mNewMessageNoticeCount);
            }
        }

        [UIEventHandle("Title/Button")]
        protected void OnClose()
        {
            Close();
        }

        protected override void _OnOpenFrame()
        {
            mComBind = frame.GetComponent<ComCommonBind>();
            mListScript = mComBind.GetCom<ComUIListScript>("ListControl");
            var Button = mComBind.GetCom<Button>("Clear");
	        if(Button)
            {
                Button.onClick.AddListener(()=> {NewMessageNoticeManager.GetInstance().ClearNewMessageNotice();});
            }
            
            mListScript.onBindItem = obj => {
                NewMessageNoticeScript script = new NewMessageNoticeScript();
                ComUIListElementScript elem = obj.GetComponent<ComUIListElementScript>();
                var mElemBind = obj.GetComponent<ComCommonBind>();
                	script.title = mElemBind.GetCom<Text>("Title");
	                script.message = mElemBind.GetCom<Text>("Message");
	                var forwardButton = mElemBind.GetCom<Button>("Forward");
                    forwardButton.onClick.AddListener(()=>{OnForward(elem);});
	                script.forwardText = mElemBind.GetCom<Text>("ForwardText");
                return script;
            };

            mListScript.onItemVisiable = elem =>
            {
                var newMsgNoticeMgr = NewMessageNoticeManager.GetInstance();
                var itemData = newMsgNoticeMgr.GetNewMessageNoticeByIndex(elem.m_index);
                if(itemData != null)
                {
                    var bindScript = elem.gameObjectBindScript as NewMessageNoticeScript;
                    bindScript.title.text = itemData.mTitle;
                    bindScript.message.text = itemData.mMessage;
                }
            };

            mListScript.Initialize();
            //mListScript.SetElementAmount(10);
        }

        protected void OnForward(ComUIListElementScript item)
        {
            var newMsgNoticeMgr = NewMessageNoticeManager.GetInstance();
            var itemData = newMsgNoticeMgr.GetNewMessageNoticeByIndex(item.m_index);

            if(itemData != null && itemData.mForwardAction != null)
            {
                itemData.mForwardAction(itemData);
            }

            Close();
        }
        protected override void _OnCloseFrame()
        {
            mDataVersion  = UInt64.MaxValue;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/NewMessageNotice";
        }
    }
}
