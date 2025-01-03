using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public enum ChapterDropItemType
    {
        None,
        Item,               //道具
        ItemCollection,     //道具集合
    }

    public class ChallengeChapterDropItem : MonoBehaviour
    {

        private int _dropItemId;        //道具ID
        private int _chapterId;         //对应地下城的ID
        private ChapterDropItemType _dropItemType;

        [Space(10)][HeaderAttribute("Item")]
        [SerializeField] private Image itemBackground;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Text itemQualityText;
        [SerializeField] private Button itemButton;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (itemButton != null)
            {
                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(OnItemClicked);
            }
        }

        private void UnBindEvents()
        {
            if(itemButton != null)
                itemButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _dropItemId = 0;
            _chapterId = 0;
            _dropItemType = ChapterDropItemType.None;
        }

        public void InitItem(int itemId, int chapterId = 0)
        {
            _dropItemId = itemId;
            _chapterId = chapterId;

            _dropItemType = GetDropItemType();
            if (_dropItemType == ChapterDropItemType.None)
            {
                Logger.LogErrorFormat("Cannot find dropItem Id is {0}", itemId);
                return;
            }

            InitContent();
        }

        private void InitContent()
        {
            switch (_dropItemType)
            {
                case ChapterDropItemType.Item:
                    InitDropItem();
                    break;
                case ChapterDropItemType.ItemCollection:
                    InitDropItemCollection();
                    break;
            }
        }

        private void InitDropItem()
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_dropItemId);
            if (itemTable != null)
            {
                ItemData.QualityInfo qualityInfo = ItemData.GetQualityInfo(itemTable.Color, itemTable.Color2);
                var itemNameStr = ChallengeUtility.GetColorString(qualityInfo.ColStr, itemTable.Name);
                var iconPath = itemTable.Icon;

                if (itemIcon != null)
                    ETCImageLoader.LoadSprite(ref itemIcon, iconPath);

                if (itemNameText != null)
                    itemNameText.text = itemNameStr;

                if (itemQualityText != null)
                    itemQualityText.text = "";

                UpdateItemBackground(itemTable.Color,itemTable.Color2);

            }
        }


        private void InitDropItemCollection()
        {
            ItemTable.eColor color = ItemTable.eColor.WHITE;
            int color2 = 0;

            ItemCollectionTable itemCollection = TableManager.instance.GetTableItem<ItemCollectionTable>(_dropItemId);
            if (null != itemCollection)
            {
                List<int> itemColorList = new List<int>(itemCollection.Color);
                itemColorList.Sort();

                if (itemIcon != null)
                    ETCImageLoader.LoadSprite(ref itemIcon, itemCollection.Icon);

                if (itemNameText != null)
                    itemNameText.text = itemCollection.Level;


                //品质
                ItemData.QualityInfo maxqi = null;
                ItemData.QualityInfo minqi = null;

                if (itemColorList.Count > 0)
                {
                    try
                    {
                        color2 = itemCollection.Color2;
                        maxqi = ItemData.GetQualityInfo((ItemTable.eColor)itemColorList[itemColorList.Count - 1], color2);
                        minqi = ItemData.GetQualityInfo((ItemTable.eColor)itemColorList[0], color2);
                        color = (ItemTable.eColor)itemColorList[itemColorList.Count - 1];
                    }
                    catch
                    {
                        maxqi = ItemData.GetQualityInfo(ItemTable.eColor.WHITE);
                        minqi = ItemData.GetQualityInfo(ItemTable.eColor.WHITE);
                        color = ItemTable.eColor.WHITE;
                        color2 = 0;
                    }

                    string itemQuality = "";

                    if (maxqi == minqi || (maxqi.Quality == minqi.Quality))
                    {
                        itemQuality = string.Format("{0}", ChallengeUtility.GetColorString(maxqi.ColStr, maxqi.Desc));
                    }
                    else
                    {
                        itemQuality = string.Format("{0}-{1}", ChallengeUtility.GetColorString(minqi.ColStr, minqi.Desc), 
                            ChallengeUtility.GetColorString(maxqi.ColStr, maxqi.Desc));
                    }

                    if (itemQualityText != null)
                        itemQualityText.text = itemQuality;

                }

                UpdateItemBackground(color, color2);
            }
        }

        private void UpdateItemBackground(ItemTable.eColor color,int color2)
        {
            var qualityInfo = ItemData.GetQualityInfo(color, color2);
            if (itemBackground != null)
                ETCImageLoader.LoadSprite(ref itemBackground, qualityInfo.Background);
        }

        private void OnItemClicked()
        {
            switch (_dropItemType)
            {
                case ChapterDropItemType.Item:
                    ShowItemTip();
                    break;
                case ChapterDropItemType.ItemCollection:
                    ShopItemCollectionTip();
                    break;
            }
        }

        private void ShowItemTip()
        {
            ItemData data = ItemDataManager.CreateItemDataFromTable(_dropItemId);
            if (null != data)
            {
                ItemTipManager.GetInstance().ShowTip(data, null);
            }
        }

        private void ShopItemCollectionTip()
        {
            var itemCollectionTable = TableManager.instance.GetTableItem<ItemCollectionTable>(_dropItemId);
            if (null != itemCollectionTable)
            {
                if (itemCollectionTable.TipsType == ItemCollectionTable.eTipsType.COLLECTION)
                {
                    List<int[]> tips = GamePool.ListPool<int[]>.Get();
                    for (var i = 0; i < itemCollectionTable.TipsContent.Count; ++i)
                    {
                        if (itemCollectionTable.TipsContent[i].valueType == UnionCellType.union_everyvalue)
                        {
                            if (itemCollectionTable.TipsContent[i].eValues.everyValues != null)
                            {
                                var tmpValues = itemCollectionTable.TipsContent[i].eValues.everyValues;
                                int[] tmpListInt = new int[tmpValues.Count];

                                for (var tempIndex = 0; tempIndex < tmpValues.Count; ++tempIndex)
                                {
                                    tmpListInt[tempIndex] = tmpValues[i];
                                }
                                tips.Add(tmpListInt);

                            }
                        }
                    }

                    if (tips.Count > 0)
                    {
                        ChapterInfoDropTipsFrame.ShowTips(tips);
                    }

                    GamePool.ListPool<int[]>.Release(tips);
                }
                else if (itemCollectionTable.TipsType == ItemCollectionTable.eTipsType.SINGLE)
                {
                    ChapterTempTipsFrame.Show(_dropItemId);
                }
            }
        }

        private ChapterDropItemType GetDropItemType()
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_dropItemId);
            if (itemTable != null)
                return ChapterDropItemType.Item;

            var itemCollectionTable = TableManager.GetInstance().GetTableItem<ItemCollectionTable>(_dropItemId);
            if (itemCollectionTable != null)
                return ChapterDropItemType.ItemCollection;

            return ChapterDropItemType.None;
        }
    }
}
