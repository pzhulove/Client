using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    public class DungeonHellTipsFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Hell/DungeonHellTips";
        }

#region ExtraUIBind
        private GameObject mDamnDescRoot = null;
        private GameObject mNormalDescRoot = null;
        private GameObject mHardTitleRoot = null;
        private GameObject mDamnHardTitleRoot = null;

        protected override void _bindExUI()
        {
            mDamnDescRoot = mBind.GetGameObject("damnDescRoot");
            mNormalDescRoot = mBind.GetGameObject("normalDescRoot");
            mHardTitleRoot = mBind.GetGameObject("hardTitleRoot");
            mDamnHardTitleRoot = mBind.GetGameObject("damnHardTitleRoot");
        }

        protected override void _unbindExUI()
        {
            mDamnDescRoot = null;
            mNormalDescRoot = null;
            mHardTitleRoot = null;
            mDamnHardTitleRoot = null;
        }
#endregion   



        public void SetHellType(Protocol.DungeonHellMode mode)
        {
            mDamnDescRoot.SetActive(false);
            mNormalDescRoot.SetActive(false);
            mHardTitleRoot.SetActive(false);
            mDamnHardTitleRoot.SetActive(false);

            switch (mode)
            {
                case Protocol.DungeonHellMode.Null:
                case Protocol.DungeonHellMode.Normal:
                    mNormalDescRoot.SetActive(true);
                    mHardTitleRoot.SetActive(true);
                    break;
                case Protocol.DungeonHellMode.Hard:
                    mDamnDescRoot.SetActive(true);
                    mDamnHardTitleRoot.SetActive(true);
                    break;
            }
        }
    }
}
