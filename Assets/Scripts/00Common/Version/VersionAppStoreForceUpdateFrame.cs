using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    public class VersionAppStoreForceUpdateFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "Base/UI/Prefabs/AppStoreForceUpdateFrame";
        }

#region ExtraUIBind
        private Button mUpdate = null;

        protected override void _bindExUI()
        {
            mUpdate = mBind.GetCom<Button>("update");
            mUpdate.onClick.AddListener(_onUpdateButtonClick);
        }

        protected override void _unbindExUI()
        {
            mUpdate.onClick.RemoveListener(_onUpdateButtonClick);
            mUpdate = null;
        }
#endregion   

#region Callback
        private void _onUpdateButtonClick()
        {
            /* put your code in here */
            Application.OpenURL(mUrl);
        }
#endregion

        private string mUrl = string.Empty; 

        protected override void _OnOpenFrame()
        {
            mUrl = (string)userData;

            Logger.LogErrorFormat("[打开AppStore 强制更新界面] {0}", mUrl);
        }

        protected override void _OnCloseFrame()
        {

        }
    }
}

