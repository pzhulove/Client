using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    public class ComItemList : MonoBehaviour
    {

        public int mWeidth = 100;
        public int mHeight = 100;

        public int mMaxCount = 5;

        public bool mCanSell = false;
        public bool mCanUse = false;
        public bool mShowTips = true;

        public bool mUseCustomCountFormat = false;
        public bool mUseOnCountUpdate     = true;

        public bool mIsAutoAdjustSize = false;

        private HorizontalLayoutGroup mHorizontalLayoutGroup = null;
        private VerticalLayoutGroup mVerticalLayoutGroup = null;

        private List<ComItem> mCachedItems = new List<ComItem>();
        private List<Component> mCachedComs = new List<Component>();
        private List<GameObject> mCachedFlags = new List<GameObject>();

        public enum eItemType
        {
            /// <summary>
            /// 自定义数量
            /// <summary>
            Custom,

            /// <summary>
            /// 从背包获取数量
            /// <summary>
            Package,

        }

        public enum eItemExtraFlag
        {
            /// <summary>
            /// 正常的显示
            /// <summary>
            Normal,

            /// <summary>
            /// 额外的奖励，需要有标记
            /// <summary>
            ExtraReward
        }

        [System.Serializable]
        public class Items
        {
            public eItemType      type;
            public eItemExtraFlag flag = eItemExtraFlag.Normal;
            public uint           count;
            public int            id;
            public int            strenthLevel;
            public EEquipType     equipType;
        }

        public Items[] mItemDatas = new Items[0];

        void Awake()
        {
            _getLayoutGroup();
            _loadItems();
            _bindUIEvent();
        }

        void OnDestroy()
        {
            _unloadItems();
            _unbindUIEvent();
        }

        private void _bindUIEvent()
        {
            if (mUseOnCountUpdate)
            {

                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _updatePackageCount);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, _updatePackageCount);
            }
        }

        private void _unbindUIEvent()
        {
            if (mUseOnCountUpdate)
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _updatePackageCount);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, _updatePackageCount);
            }
        }

        private void _updatePackageCount(UIEvent ui)
        {
            if(mItemDatas == null)
            {
                return;
            }

            if(mCachedItems == null)
            {
                return;
            }

            for (int i = 0; i < mItemDatas.Length; ++i)
            {
                if (null != mItemDatas[i] && mItemDatas[i].type == eItemType.Package)
                {
                    int id = mItemDatas[i].id;
                    ComItem item = mCachedItems.Find(x=>
                    {
                        return (x != null 
                        && x.ItemData != null 
                        && id == x.ItemData.TableID);
                    });

                    if (null != item)
                    {
                        ItemData dropData = ItemDataManager.CreateItemDataFromTable(id);
                        if(dropData == null)
                        {
                            continue;
                        }

                        dropData.Count = ItemDataManager.GetInstance().GetOwnedItemCount(id, false);

                        if (mShowTips)
                        {
                            item.Setup(dropData, (go, data) =>
                            {
                                ItemTipManager.GetInstance().ShowTip(data);
                            });
                        }
                        else
                        {
                            item.Setup(dropData, null);
                        }
                    }
                }
            }
        }

        private void _loadItems()
        {
            _unloadItems();

            //加载前自动调整List箱子的Size
            _autoAdjustSize();

            for (int i = 0; i < mItemDatas.Length; ++i)
            {
                ComItem item = ComItemManager.Create(this.gameObject);

                ItemData dropData = ItemDataManager.CreateItemDataFromTable(mItemDatas[i].id);
                if (null != dropData)
                {
                    dropData.CanSell = mCanSell;
                    dropData.StrengthenLevel = mItemDatas[i].strenthLevel;
                    dropData.EquipType = mItemDatas[i].equipType;

                    if (mItemDatas[i].type == eItemType.Custom)
                    {
                        dropData.Count = (int)mItemDatas[i].count;
                    }
                    else
                    {
                        dropData.Count = ItemDataManager.GetInstance().GetOwnedItemCount(mItemDatas[i].id, false);
                    }

                    if (mUseCustomCountFormat)
                    {
                        item.SetCountFormatter(x=>
                        {
                            return string.Format("{0}", x.ItemData.Count);
                        });
                    }

                    item.SetActive(true);

                    if (mShowTips)
                    {
                        item.Setup(dropData, (go, data) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(data);
                        });
                    }
                    else
                    {
                        item.Setup(dropData, null);
                    }

                    if (mItemDatas[i].flag == eItemExtraFlag.ExtraReward)
                    {
                        GameObject teamFlag = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Chapter/Normal/ChapterDropItemTeamFlag");
                        Utility.AttachTo(teamFlag, item.gameObject);
                        mCachedFlags.Add(teamFlag);
                    }
                }

                GameObject itemObject = item.gameObject;

                LayoutElement ly = itemObject.AddComponent<LayoutElement>();

                ly.preferredWidth  = mWeidth;
                ly.preferredHeight = mHeight;

                mCachedItems.Add(item);
                mCachedComs.Add(ly);
            }
        }

        private void _unloadItems()
        {
            for (int i = 0; i < mCachedComs.Count; ++i)
            {
                GameObject.Destroy(mCachedComs[i]);
                mCachedComs[i] = null;
            }
            mCachedComs.Clear();

            for (int i = 0; i < mCachedItems.Count; ++i)
            {
                ComItemManager.Destroy(mCachedItems[i]);
                mCachedItems[i] = null;
            }
            mCachedItems.Clear();

            for (int i = 0; i < mCachedFlags.Count; ++i)
            {
                GameObject.Destroy(mCachedFlags[i]);
                mCachedFlags[i] = null;
            }
            mCachedFlags.Clear();
        }

        public void SetItems(Items[] list)
        {
            if (null != list)
            {
                mItemDatas = list;
                _loadItems();
            }
        }

        public void SetItems(IList<int> list)
        {
            mItemDatas = new Items[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                mItemDatas[i] = new Items();
                mItemDatas[i].id = list[i];
                mItemDatas[i].type = eItemType.Custom;
                mItemDatas[i].flag = eItemExtraFlag.Normal;
                mItemDatas[i].count = 0;
            }
            _loadItems();
        }

        public void AddItems(Items[] list)
        {
            List<Items> cur = new List<Items>(mItemDatas);

            for (int i = 0; i < list.Length; ++i)
            {
                cur.Add(list[i]);
            }

            mItemDatas = cur.ToArray();

            _loadItems();
        }

        private void _getLayoutGroup()
        {
            mHorizontalLayoutGroup = transform.GetComponent<HorizontalLayoutGroup>();
            mVerticalLayoutGroup = transform.GetComponent<VerticalLayoutGroup>();
        }

        /// <summary>
        /// 自动调整List箱子的Size
        /// </summary>
        private void _autoAdjustSize()
        {
            if(mIsAutoAdjustSize)
            {
                RectTransform rts = transform as RectTransform;
                int number = mItemDatas.Length;
                //水平List
                if (mHorizontalLayoutGroup != null)
                {
                    float oneItemWidth = mWeidth + ((mHorizontalLayoutGroup.spacing > 0) ? mHorizontalLayoutGroup.spacing : 0);
                    rts.sizeDelta = new Vector2(number * oneItemWidth, rts.sizeDelta.y);
                }
                else if(mVerticalLayoutGroup != null) //竖直
                {
                    float oneItemHeight = mHeight + ((mVerticalLayoutGroup.spacing > 0) ? mVerticalLayoutGroup.spacing : 0);
                    rts.sizeDelta = new Vector2(rts.sizeDelta.x, number * oneItemHeight);
                }
            }
        }

    }
}
