using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamRewardPreviewFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComUIListScript mComUIList = null;

        protected override void _bindExUI()
        {
            mComUIList = mBind.GetCom<ComUIListScript>("ComUIList");
        }

        protected override void _unbindExUI()
        {
            mComUIList = null;
        }
        #endregion

        private List<ItemData> items = new List<ItemData>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamRewardPreviewFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitUIList();
            InitData();
            OnSetElmentCount();
        }

        protected override void _OnCloseFrame()
        {
            UnInitUIList();
            items.Clear();
        }

        private void InitUIList()
        {
            if (mComUIList != null)
            {
                mComUIList.Initialize();
                mComUIList.onBindItem += OnBindItemDelegate;
                mComUIList.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitUIList()
        {
            if (mComUIList != null)
            {
                mComUIList.onBindItem -= OnBindItemDelegate;
                mComUIList.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private ComCommonBind OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var comBind = item.gameObjectBindScript as ComCommonBind;
            if (comBind != null && item.m_index >= 0 && item.m_index < items.Count)
            {
                int index = item.m_index;
                UpdateItemInfo(comBind, index);
            }
        }

        private void UpdateItemInfo(ComCommonBind comBind, int index)
        {
            if (comBind == null)
            {
                return;
            }

            if (index < 0 || index >= items.Count)
            {
                return;
            }

            ItemData itemData = items[index];
            if (itemData == null)
            {
                return;
            }

            Image backGround = comBind.GetCom<Image>("backgroud");
            Image icon = comBind.GetCom<Image>("Icon");
            Text count = comBind.GetCom<Text>("Count");
            //Text name = comBind.GetCom<Text>("name");
            Button iconBtn = comBind.GetCom<Button>("Iconbtn");

            if (backGround != null)
            {
                ETCImageLoader.LoadSprite(ref backGround, itemData.GetQualityInfo().Background);
            }

            if (icon != null)
            {
                ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
            }

            if (count != null)
            {
                count.text = itemData.Count.ToString();
            }

//             if (name != null)
//             {
//                 name.text = itemData.GetColorName();
//             }

            if (iconBtn != null)
            {
                iconBtn.onClick.RemoveAllListeners();
                iconBtn.onClick.AddListener(()=> { ItemTipManager.GetInstance().ShowTip(itemData); });
            }
        }

        private void InitData()
        {
            if (items == null)
            {
                items = new List<ItemData>();
            }

            items.Clear();

            var enumerator = TableManager.GetInstance().GetTable<TeamRewardTable>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var table = enumerator.Current.Value as TeamRewardTable;

                if (table == null)
                {
                    continue;
                }

                //类型为结算经验过滤掉
                if (table.Type == 1)
                {
                    continue;
                }

                ItemData itemData = ItemDataManager.CreateItemDataFromTable(table.Reward);
                if (itemData == null)
                {
                    continue;
                }

                itemData.Count = table.Num;

                items.Add(itemData);
            }

            items.Sort((x, y) => 
            {
                if (x.Quality != y.Quality)
                {
                    return y.Quality - x.Quality;
                }

                return x.TableID - y.TableID;
            });
        }

        private void OnSetElmentCount()
        {
            if (mComUIList != null)
            {
                mComUIList.SetElementAmount(items.Count);
            }
        }
    }
}

