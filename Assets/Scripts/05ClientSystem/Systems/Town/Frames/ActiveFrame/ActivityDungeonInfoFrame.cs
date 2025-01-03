using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
///////É¾³ýlinq
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Collections;
using ProtoTable;

namespace GameClient
{
    public class ActivityDungeonInfoFrame : ClientFrame
    {
#region ExtraUIBind
        private Button mClose = null;
        private Text mName = null;
        private ComChapterInfoDrop mDrops = null;
        private Text mDesc = null;
        private Button mClosebutton = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mName = mBind.GetCom<Text>("name");
            mDrops = mBind.GetCom<ComChapterInfoDrop>("drops");
            mDesc = mBind.GetCom<Text>("desc");
            mClosebutton = mBind.GetCom<Button>("closebutton");
            mClosebutton.onClick.AddListener(_onClosebuttonButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mName = null;
            mDrops = null;
            mDesc = null;
            mClosebutton.onClick.RemoveListener(_onClosebuttonButtonClick);
            mClosebutton = null;
        }
#endregion   

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _onClose();
        }

        private void _onClosebuttonButtonClick()
        {
            /* put your code in here */
            _onClose();
        }
#endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/Dungeon/ActvityDungeonInfoFrame";
        }

        protected override void _OnOpenFrame()
        {
            int dungeonId = (int)userData;

            ActivityDungeonSub sub = ActivityDungeonDataManager.GetInstance().GetSubByDungeonID(dungeonId);

            mName.text = sub.name;
            mDesc.text = sub.table.PlayDescription;

            mDrops.SetDropList(sub.drops,dungeonId);
        }

        private void _onClose()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }
    }
}
