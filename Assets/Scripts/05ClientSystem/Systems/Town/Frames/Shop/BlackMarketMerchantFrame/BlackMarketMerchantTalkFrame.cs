using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class BlackMarketMerchantTalkFrame : ClientFrame
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
            return "UIFlatten/Prefabs/Shop/BlackMarketMerchantFrame/BlackMarketMerchantTalkFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            var table = TableManager.GetInstance().GetTableItem<BlackMarketTable>((int)BlackMarketMerchantDataManager.BlackMarketType);
            if (table != null)
            {
                var npcTable = TableManager.GetInstance().GetTableItem<NpcTable>(table.NpcID);
                if (npcTable != null)
                {
                    ETCImageLoader.LoadSprite(ref mNpcPic, npcTable.NpcBody);

                    mNpcName.text = npcTable.NpcName;
                }
            }

            mTalk.text = TR.Value("BlackMarketMerchantTalk");
            mFunctionUnlockDes.text = TR.Value("BlackMarketMerchantFunctionUnLockDes");
        }

        protected sealed override void _OnCloseFrame()
        {
            
        }
    }
}

