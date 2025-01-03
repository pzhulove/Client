using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    public class ContentItem
    {
        public string unlockTitle;
        public string iconPath;
        public string name;
        public string desc;
        public List<string[]> rewards;
        
        public void Clear()
        {
            unlockTitle = "";
            iconPath = "";
            name = "";
            desc = "";
            rewards.Clear();
        }
    }

    class NextOpenShowFrame : ClientFrame
    {
        List<int> itemList = new List<int>();
        List<ContentItem> itemDatas = new List<ContentItem>();
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FunctionUnlock/NextOpenShow";
        }

        protected sealed override void _OnOpenFrame()
        {
            InitData();
            InitInterface();
        }

        void InitData()
        {
            var temp = PlayerBaseData.GetInstance().NextUnlockFunc;
            if (temp == null)
            {
                return;
            }
            
            itemList = new List<int>();
            for (int i = 0; i < temp.Count; i++)
            {
                // Debug.Log(temp[i].ToString());
                itemList.Add(temp[i]);
            }
            
            if (itemList == null)
            {
                return;   
            }
            
            //item数据整理.
            for (int i = 0; i < itemList.Count; i++)
            {
                var tableData = TableManager.GetInstance().GetTableItem<FunctionUnLock>(itemList[i]);
                if (tableData == null)
                {
                    return;
                }
                
                ContentItem itemData = new ContentItem();
                itemData.unlockTitle = string.Format(TR.Value("ComLinkUnlockHint"), tableData.FinishLevel);
                itemData.iconPath = tableData.IconPath;
                itemData.name = tableData.Name;
                if (tableData.Explanation != "" && tableData.Explanation != "-")
                    itemData.desc = tableData.Explanation;
                if (!string.IsNullOrEmpty(tableData.Award))
                {
                    string reward = tableData.Award;
                    var rewardsArr = reward.Split('|');
                    if (rewardsArr.Length > 0)
                    {
                        //奖励暂时只放一个,后面有添加再说
                        itemData.rewards = new List<string[]>();
                        if (!string.IsNullOrEmpty(rewardsArr[0]))
                        {
                            var itemAward = rewardsArr[0].Split('_');
                            if (itemAward != null && itemAward.Length == 3)
                            {
                                itemData.rewards.Add(itemAward);
                            }   
                        } 
                    }
                }
                itemDatas.Add(itemData);
            }
            
            
            
        }

        void InitInterface()
        {
            InitListView();
            RefreshListView();
        }


        void InitListView()
        {
            if(mComUIList != null)
            {
                mComUIList.Initialize();
                SetScrollPos();
                mComUIList.OnItemUpdate += RefreshItem;
                mComUIList.onItemVisiable += RefreshItem;
            }
        }
        
        
        void RefreshListView()
        {
            if (mComUIList != null && itemList != null && itemList.Count > 0)
            {
                mComUIList.SetElementAmount(itemList.Count);
            }
        }

        void UnBindListView()
        {
            if (mComUIList != null)
            {
                mComUIList.OnItemUpdate -= RefreshItem;
                mComUIList.onItemVisiable -= RefreshItem;
            }
        }

        void RefreshItem(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (itemList == null)
            {
                return;
            }

            if (item.m_index >= itemList.Count)
            {
                return;
            }
            
            //set item
            ComCommonBind comBind = item.GetComponent<ComCommonBind>();
            if (comBind == null)
            {
                return;
            }

            if (itemDatas == null)
            {
                return;
            }

            var itemData = itemDatas[item.m_index];

            var unlockTitle = comBind.GetCom<TextEx>("UnlockTitle");
            var icon = comBind.GetCom<ImageEx>("Icon");
            var name = comBind.GetCom<TextEx>("Name");
            var desc = comBind.GetCom<TextEx>("Desc");
            var comItemNew = comBind.GetCom<ComItemNew>("ComItemNew");
            var tips = comBind.GetGameObject("Tips");
            
            unlockTitle.SafeSetText(itemData.unlockTitle);
            icon.SafeSetImage(itemData.iconPath);
            name.SafeSetText(itemData.name);
            desc.SafeSetText(itemData.desc);
            if (comItemNew != null)
            {
                if (itemData.rewards.Count>0)
                {
                    var rewardsData = itemData.rewards[0];
                    if (rewardsData != null && rewardsData.Length == 3)
                    {
                        var thisItem = ItemDataManager.CreateItemDataFromTable(int.Parse(rewardsData[1]));
                        if (thisItem != null)
                        {
                            comItemNew.Setup(thisItem,null,true);
                            if (!string.IsNullOrEmpty(rewardsData[2]))
                            {
                                comItemNew.SetCount(rewardsData[2]);   
                            }
                            comItemNew.CustomActive(true);
                            tips.CustomActive(true);
                            return;
                        }    
                    }     
                }
                   
            }
            comItemNew.CustomActive(false);
            tips.CustomActive(false);
        }


        void SetScrollPos()
        {
            int num = itemList.Count;
            float leftSpace = 92;
            if (num > 0 && num < 4)
            {
                leftSpace = (4 - num) * 236;
            }

            if (mComUIList != null)
            {
                mComUIList.m_elementPadding = new Vector2(leftSpace,0);
            }
        }

        void OnClose()
        {
            ClearData();
            UnBindListView();
            Close();
        }

        void ClearData()
        {
            itemList.Clear();
            itemDatas.Clear();
        }
        

        #region ExtraUIBind
        private ComUIListScript mComUIList = null;
        private ButtonEx mClose = null;

        protected override void _bindExUI()
        {
            mComUIList = mBind.GetCom<ComUIListScript>("ComUIList");
            mClose = mBind.GetCom<ButtonEx>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mComUIList = null;
        }
        #endregion   
        
        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            OnClose();
        }
        #endregion
        
    }
}
