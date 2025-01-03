using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinShopView : MonoBehaviour
    {
        [Space(5)]
        [HeaderAttribute("Title")]
        [SerializeField]
        private Text activityTime;
        [Space(5)]
        [HeaderAttribute("Middle")]
        [SerializeField]
        private GameObject shopListRoot;
        [Space(5)]
        [HeaderAttribute("Bottom")]
        [SerializeField]
        private Text nextTime;
        
        [SerializeField]
        private Image moneyIcon;
        [SerializeField]
        private Text moneyNum;
        [SerializeField]
        private Text clearTime;
        [SerializeField]
        private HorizontalLayoutGroup itemLayoutGroup;
        [Space(5)]
        [HeaderAttribute("Button")]
        [SerializeField]
        private Button getMoneyBtn;
        [SerializeField]
        private Button closeBtn;
        [SerializeField]
        private Button leftBtn;
        [SerializeField]
        private Button rightBtn;

        private const string shopItemPath = "UIFlatten/Prefabs/ShopNew/GoblinShopItem";
        private int curFirstIndex = 0;//此时展示第一个商品在总商品的位置
        private OpActivityData activityData;
        private AccountShopItemInfo[] shopData;
        private AccountShopQueryIndex accountShopQuery;
        List<GameObject> shopItemGOList = new List<GameObject>();
        private int specialItemId;
        public void InitShop(GoblinShopData goblinShopData)
        {
            activityData = ActivityDataManager.GetInstance().GetLimitTimeActivityData((uint)goblinShopData.activityId);
            shopData = AccountShopDataManager.GetInstance().GetAccountShopData(goblinShopData.accountShopItem);
            specialItemId = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_GNOME_COIN_DATA_ID).Value;
            accountShopQuery = goblinShopData.accountShopItem;
            if (activityData == null || shopData == null)
            {
                return;
            }
            if (shopData != null)
            {
                InitUI(goblinShopData);

                InitBtn();

                InitShopElementItemList();
            }
        }

        public void UpdateShopItem(AccountShopQueryIndex accountShopItem)
        {
            shopData = AccountShopDataManager.GetInstance().GetAccountShopData(accountShopItem);
            if (shopData == null)
                return;

            updateShopItem(curFirstIndex);
        }

        public void UpdateSpecialNum(int id)
        {
            if(id == specialItemId)
            {
                moneyNum.text = AccountShopDataManager.GetInstance().GetSpecialItemNum(specialItemId).ToString();
            }
        }
        void InitUI(GoblinShopData goblinShopData)
        {
            if (activityData != null)
            {
                activityTime.SafeSetText(Function.GetTimeWithoutYear((int)activityData.startTime, (int)activityData.endTime));

                if (activityData.parm2 != null && activityData.parm2.Length > 2)
                {
                    clearTime.text = string.Format("{0}清空", Function.GetDateTime((int)activityData.parm2[2], false));
                    clearTime.CustomActive(true);
                }
                else
                {
                    clearTime.CustomActive(false);
                }
            }

            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(specialItemId);
            if(itemTableData != null)
            {
                ETCImageLoader.LoadSprite(ref moneyIcon, itemTableData.Icon);
                moneyNum.text = AccountShopDataManager.GetInstance().GetSpecialItemNum(specialItemId).ToString();
            }
            int m_iNextTime = AccountShopDataManager.GetInstance().GetShopNextTime(goblinShopData.accountShopItem);
            if(m_iNextTime == 0)
            {
                nextTime.CustomActive(false);
            }
            else
            {
                nextTime.CustomActive(true);
                nextTime.text = Function.GetDateTime(m_iNextTime,false);
            }
        }

        void InitBtn()
        {
            leftBtn.onClick.RemoveAllListeners();
            leftBtn.onClick.AddListener(() =>
            {
                if(curFirstIndex > 0)
                {
                    curFirstIndex--;
                    updateShopItem(curFirstIndex);
                }
            });

            rightBtn.onClick.RemoveAllListeners();
            rightBtn.onClick.AddListener(() =>
            {
                if(curFirstIndex < shopData.Length - 5)
                {
                    curFirstIndex++;
                    updateShopItem(curFirstIndex);
                }
            });
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(() =>
            {
                ClientSystemManager.GetInstance().CloseFrame<GoblinShopFrame>();
            });
            getMoneyBtn.onClick.RemoveAllListeners();
            getMoneyBtn.onClick.AddListener(() =>
            {
                ClientSystemManager.GetInstance().OpenFrame<GoblinPreviewFrame>();
            });
        }

        void InitShopElementItemList()
        {
            if (shopData.Length <= 3)
            {
                itemLayoutGroup.spacing = 120;
            }
            else
            {
                itemLayoutGroup.spacing = 0;
            }
            shopItemGOList.Clear();
            for (int i = 0; i < 5; i++)
            {
                if(i >= shopData.Length)
                {
                    break;
                }
                GameObject shopItem = AssetLoader.instance.LoadResAsGameObject(shopItemPath);
                if(shopItem != null)
                {
                    Utility.AttachTo(shopItem, shopListRoot);
                }
                shopItemGOList.Add(shopItem);
            }
            updateShopItem(0);
        }

        void updateShopItem(int curIndex)
        {
            updateLeftAndRightBtn(curIndex);
            
            for(int i = curIndex;i< curIndex + 5; i++)
            {
                if(i >= shopData.Length)
                {
                    break;
                }
                if((i - curIndex) >= shopItemGOList.Count)
                {
                    break;
                }
                var goblinShopItem = shopItemGOList[i - curIndex].GetComponent<GoblinShopItem>();
                if(goblinShopItem != null)
                {
                    goblinShopItem.SetElementItem(shopData[i], accountShopQuery);
                }
            }
        }

        void updateLeftAndRightBtn(int curIndex)
        {
            leftBtn.CustomActive(true);
            rightBtn.CustomActive(true);
            if (shopData.Length <= 5)
            {
                leftBtn.CustomActive(false);
                rightBtn.CustomActive(false);
            }
            else
            {
                if (curIndex == 0)
                {
                    leftBtn.CustomActive(false);
                }
                if (curIndex == shopData.Length - 5)
                {
                    rightBtn.CustomActive(false);
                }
            }
        }
    }
}