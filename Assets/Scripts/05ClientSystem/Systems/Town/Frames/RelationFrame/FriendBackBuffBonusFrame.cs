using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{
    /// <summary>
    /// 回归Buff加成信息
    /// </summary>
    public class BackBuffBonusInfo
    {
        public string IconPath; //buffIcon路径
        public string Name;  //buff加成描述
    }

    class FriendBackBuffBonusFrame : ClientFrame
    {
        const int friendBackActivityID = 1209;
        List<BackBuffBonusInfo> backBuffBonusList = new List<BackBuffBonusInfo>();

        private RelationData value;

		#region ExtraUIBind
		private Button mClose = null;
		private ComUIListScript mBuffBonusComUIList = null;
		
		protected sealed override void _bindExUI()
		{
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
			mBuffBonusComUIList = mBind.GetCom<ComUIListScript>("BuffBonus");
		}
		
		protected sealed override void _unbindExUI()
		{
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
			mBuffBonusComUIList = null;
		}
		#endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                value = userData as RelationData;
            }
            InitFriendBackBuffBonusComUIList();
            InitBackBuffBonusInfo();
        }

        protected sealed override void _OnCloseFrame()
        {
            backBuffBonusList.Clear();
            value = null;
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RelationFrame/FriendBackBuffBonusFrame";
        }
        
        void InitFriendBackBuffBonusComUIList()
        {
            mBuffBonusComUIList.Initialize();
            mBuffBonusComUIList.onBindItem = (GameObject go) => 
            {
                return go.GetComponent<ComFriendBackBuffBonusInfo>();
            };
            mBuffBonusComUIList.onItemVisiable = (ComUIListElementScript item) => 
            {
                if (item != null)
                {
                    var current = item.gameObjectBindScript as ComFriendBackBuffBonusInfo;
                    if (current != null)
                    {
                        if (item.m_index >= 0 && item.m_index < backBuffBonusList.Count)
                        {
                            current.OnItemVisible(backBuffBonusList[item.m_index]);
                        }
                    }
                }
            };
        }

        void InitBackBuffBonusInfo()
        {
            backBuffBonusList.Clear();
            var table = TableManager.GetInstance().GetTableItem<OpActivityTable>(friendBackActivityID);
            if (table != null)
            {
                if (table.Param2.Count > 0)
                {
                    for (int i = 0; i < table.Param2.Count; i++)
                    {
                        int buffId = table.Param2[i];
                        var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
                        if (buffTable != null)
                        {
                            BackBuffBonusInfo buffBonusInfo = new BackBuffBonusInfo();
                            buffBonusInfo.IconPath = buffTable.Icon;
                            buffBonusInfo.Name = buffTable.Name;
                            backBuffBonusList.Add(buffBonusInfo);
                        }
                    }
                }
            }

            //如果该玩家穿戴了周年庆称号，显示称号属性信息
            if (value != null && value.returnYearTitle != 0)
            {
                var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(1201092);
                if (buffTable != null)
                {
                    BackBuffBonusInfo buffBonusInfo = new BackBuffBonusInfo();
                    buffBonusInfo.IconPath = buffTable.Icon;
                    buffBonusInfo.Name = buffTable.Name;
                    backBuffBonusList.Add(buffBonusInfo);
                }
            }

            if (backBuffBonusList.Count > 0)
            {
                mBuffBonusComUIList.SetElementAmount(backBuffBonusList.Count);
            }
        }
    }
}
