using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    public enum StatueType
    {
        TownStatue = 0,
        ViceTownStatue,
    }

    class GuildStatueFrame : ClientFrame
    {
        StatueType eStatueType = StatueType.TownStatue;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildStatueFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                eStatueType = (StatueType)userData;
            }

            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            eStatueType = StatueType.TownStatue;
        }

        void InitInterface()
        {
            if(eStatueType == StatueType.TownStatue)
            {
                mTitle.text = "城主雕像";

                mName.CustomActive(true);
                mViceName.CustomActive(false);

                mMemList.CustomActive(true);
                mViceMemList.CustomActive(false);
            }
            else if(eStatueType == StatueType.ViceTownStatue)
            {
                mTitle.text = "副城主雕像";

                mName.CustomActive(false);
                mViceName.CustomActive(true);

                mMemList.CustomActive(false);
                mViceMemList.CustomActive(true);
            }
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private Text mTitle = null;
        private GameObject mName = null;
        private GameObject mViceName = null;
        private GameObject mMemList = null;
        private GameObject mViceMemList = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mTitle = mBind.GetCom<Text>("Title");
            mName = mBind.GetGameObject("Name");
            mViceName = mBind.GetGameObject("ViceName");
            mMemList = mBind.GetGameObject("MemList");
            mViceMemList = mBind.GetGameObject("ViceMemList");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mTitle = null;
            mName = null;
            mViceName = null;
            mMemList = null;
            mViceMemList = null;
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
