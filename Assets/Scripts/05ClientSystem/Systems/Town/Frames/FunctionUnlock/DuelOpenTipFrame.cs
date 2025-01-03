using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum OpenType
    {
        none,
        changejobbefore,
        changejobbeafter,
    }
    class DuelOpenTipFrame : ClientFrame
    {
       
        OpenType openType = OpenType.none;
        int jobType = 0;
        string changejobbeforePath = "UIFlatten/Image/Background/UI_Beijing_Wanfa.png:UI_Beijing_Wanfa";
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FunctionUnlock/NextOpenShow_1";
        }

        protected override void _OnOpenFrame()
        {
            openType = (OpenType)userData;

            jobType = PlayerBaseData.GetInstance().JobTableID / 10 * 10;

            ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(jobType);

            if (openType==OpenType.changejobbefore)
            {
                if (jobTable != null)
                {
                    mBorder.CustomActive(false);
                    ETCImageLoader.LoadSprite(ref mImgBeijing, jobTable.ChangJobTaskPrompt);
                }
            }
            else if (openType == OpenType.changejobbeafter)
            {
                mBorder.CustomActive(true);
                ETCImageLoader.LoadSprite(ref mImgBeijing, changejobbeforePath);
            }
        }

        protected override void _OnCloseFrame()
        {
            openType = OpenType.none;
            jobType = 0;
        }

        #region ExtraUIBind
        private Image mImgBeijing = null;
        private GameObject mBorder = null;
        private Button mBtClose = null;

        protected override void _bindExUI()
        {
            mImgBeijing = mBind.GetCom<Image>("ImgBeijing");
            mBorder = mBind.GetGameObject("Border");
            mBtClose = mBind.GetCom<Button>("BtClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mImgBeijing = null;
            mBorder = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
