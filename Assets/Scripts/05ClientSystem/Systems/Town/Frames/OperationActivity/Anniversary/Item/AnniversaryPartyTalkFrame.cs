using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AnniversaryPartyTalkFrame : ClientFrame
    {

        #region ExtraUIBind
        private Image mNpcPic = null;
        private Text mTalk = null;
        private Text mFunctionUnlockDes = null;
        private Text mNpcName = null;
        private Button mClose = null;

        protected sealed override void _bindExUI()
        {
            mNpcPic = mBind.GetCom<Image>("NpcPic");
            mTalk = mBind.GetCom<Text>("Talk");
            mFunctionUnlockDes = mBind.GetCom<Text>("FunctionUnlockDes");
            mNpcName = mBind.GetCom<Text>("NpcName");
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected sealed override void _unbindExUI()
        {
            mNpcPic = null;
            mTalk = null;
            mFunctionUnlockDes = null;
            mNpcName = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/Anniversary/Item/AnniversaryPartyTalkFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            int npcID = (int)userData;
            var npcTable = TableManager.GetInstance().GetTableItem<NpcTable>(npcID);
            if (npcTable != null)
            {
                ETCImageLoader.LoadSprite(ref mNpcPic, npcTable.NpcBody);

                mNpcName.text = npcTable.NpcName;
            }

            mTalk.text = TR.Value("AnniversaryPartyTalk");
            mFunctionUnlockDes.text = TR.Value("AnniversaryPartyFunctionUnLockDes");
        }

        protected sealed override void _OnCloseFrame()
        {

        }
    }
}
