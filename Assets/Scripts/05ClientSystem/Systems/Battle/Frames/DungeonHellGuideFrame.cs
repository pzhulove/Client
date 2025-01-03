using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    public class DungeonHellGuideFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Hell/DungeonHellGuide";
        }

#region ExtraUIBind
        private Text mDesc = null;
        private Text mCount = null;

        protected override void _bindExUI()
        {
            mDesc = mBind.GetCom<Text>("desc");
            mCount = mBind.GetCom<Text>("count");
        }

        protected override void _unbindExUI()
        {
            mDesc = null;
            mCount = null;
        }
#endregion   

        public void SetDescription(string desc)
        {
            mDesc.text = desc;
        }

        public void SetLeftCount(int cnt)
        {
            //mCount.text = cnt.ToString();
        }
    }
}
