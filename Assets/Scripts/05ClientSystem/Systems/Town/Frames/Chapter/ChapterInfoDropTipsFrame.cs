using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;


namespace GameClient
{
    public sealed class ChapterInfoDropTipsFrame : ClientFrame
    {
        public static void ShowTips(List<int[]> items)
        {
            if (!ClientSystemManager.instance.IsFrameOpen<ChapterInfoDropTipsFrame>())
            {
                ChapterInfoDropTipsFrame frame =  ClientSystemManager.instance.OpenFrame<ChapterInfoDropTipsFrame>() as ChapterInfoDropTipsFrame;
                frame._setItems(items);
            }
        }

        private void _setItems(List<int[]> items)
        {
            mItems = items;
            _addUnit();
        }

#region ExtraUIBind
        private Button mClose = null;
        private GameObject mRoot = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mRoot = mBind.GetGameObject("root");
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mRoot = null;
        }
#endregion   

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _onClose();
        }
#endregion

        private void _addUnit()
        {
            _clearCache();

            for (int i = 0; i < mItems.Count; ++i)
            {
                GameObject line = new GameObject("line");
                Utility.AttachTo(line, mRoot);

                HorizontalLayoutGroup group = line.AddComponent<HorizontalLayoutGroup>();
                group.spacing = 60;
                group.childForceExpandWidth = false;
                group.childForceExpandHeight = false;

                ComChapterInfoDrop drop = line.AddComponent<ComChapterInfoDrop>();


                drop.SetDropList(new List<int>(mItems[i]),-1);

                mCache.Add(line);
            }
        }

        private void _clearCache()
        {
            //for (int i = 0; i < mCache.Count; ++i)
            //{
            //    GameObject.Destroy(mCache[i]);
            //}
            mCache.Clear();
        }

        private List<int[]> mItems = new List<int[]>();
        private List<GameObject> mCache = new List<GameObject>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Normal/ChapterInfoDropTips";
        }

        protected override void _OnOpenFrame()
        {
            _addUnit();
        }

        protected override void _OnCloseFrame()
        {
            _clearCache();

            mItems.Clear();
        }

        private void _onClose()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }
    }
}
