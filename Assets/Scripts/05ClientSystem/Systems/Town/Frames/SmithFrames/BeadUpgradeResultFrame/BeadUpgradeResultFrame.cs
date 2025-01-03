using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class BeadUpgradeResultData
    {
        public int mountedType;
        public ulong equipGuid;
        public int mBeadID;
        public int mBuffID;
        public ulong mBeadGUID;
        public BeadUpgradeResultData(int mountedType, ulong equipGuid,int mBeadID,int mBuffID,ulong mBeadGUID)
        {
            this.mountedType = mountedType;
            if (this.mountedType == (int)UpgradePrecType.Mounted)
            {
                this.equipGuid = equipGuid;
                this.mBeadID = mBeadID;
            }
            else
            {
                this.mBeadGUID = mBeadGUID;
            }
            this.mBuffID = mBuffID;
        }
    }

    class BeadUpgradeResultFrame : ClientFrame
    {
		#region ExtraUIBind
		private BeadUpgradeResultView mBeadUpgradeResultView = null;
		
		protected sealed override void _bindExUI()
		{
			mBeadUpgradeResultView = mBind.GetCom<BeadUpgradeResultView>("BeadUpgradeResultView");
		}
		
		protected sealed override void _unbindExUI()
		{
			mBeadUpgradeResultView = null;
		}
		#endregion
        BeadUpgradeResultData mData = null;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/BeadUpgradeResultFrame/BeadUpgradeResultFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mData = (BeadUpgradeResultData)userData; 
            if (mData != null)
            {
                mBeadUpgradeResultView.InitView(this,mData);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            mData = null;
        }
    }
}
