using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;


namespace GameClient
{
    [LoggerModel("Chapter")]
    public class CommonBoardFrame : ClientFrame
    {
        protected ComCommonBoard mCommonBoard;

        protected override void _OnLoadPrefabFinish()
        {
            mCommonBoard = frame.GetComponent<ComCommonBoard>();
            if (null != mCommonBoard)
            {
                mCommonBoard.OnClose(() => { frameMgr.CloseFrame(this); });
                mCommonBoard.OnBack(() => { frameMgr.CloseFrame(this); });
            }
            else
            {
                Logger.LogError("missinng ComCommonBoard");
            }
        }

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/Chapter/CommonBoard";
        }
    }

    public class ChapterCommonBoardFrame : CommonBoardFrame
    {
        private const string                kCommonTipsPath = "UI/Prefabs/Chapter/CommonCostTips";

        protected override void _OnLoadPrefabFinish()
        {
            base._OnLoadPrefabFinish();

            _loadData();

            _loadBg();
            _loadLeftPanel();
            _loadRightPanel();
        }

        private void _onTownSceneChange(UIEvent ui)
        {
            if (ui.EventParams.CurrentSceneID != ChapterSelectFrame.sSceneID)
            {
                frameMgr.CloseFrame(this);
            }
        }

        protected override void _OnOpenFrame()
        {
        }

        protected override void _OnCloseFrame()
        {
        }


        protected virtual void _loadData()
        {
        }

        protected virtual void _loadBg()
        {
        }

        protected virtual void _loadLeftPanel()
        {
        }

        protected virtual void _loadRightPanel()
        {
        }
    }
}
