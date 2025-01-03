using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{
    enum ShopGoodDataType
    {
        none,
        Nomal,//正常
        LuckCharm,//好运符
        ReComend,//推荐
    }
    class WeaponLeaseShopView : MonoBehaviour
    {
        [SerializeField]
        private Button mCloseBtn;
        [SerializeField]
        private GameObject mWeaponLeaseGoodsParent;
        [SerializeField]
        private GameObject mWeaponLeaseGoodsPrefab;
        [SerializeField]
        private ComUIListScript mWeaponLeaseItemComUIListScript;

        WeaponLeaseShopFrame frame;
        WeaponLeaseItem.OnClickLease mOnClickLease;
        ShopData shopData;
        List<GoodsData> mShopGoodDatas =new List<GoodsData>();

        public void InitView(WeaponLeaseShopFrame frame, ShopData shopData,WeaponLeaseItem.OnClickLease onClickLease)
        {
            this.frame = frame;
            mOnClickLease = onClickLease;
            this.shopData = shopData;
            InitShopGoodDatas();
            BindUIEvent();
            InitWeaponLeaseItemComUIListScript();

            mCloseBtn.onClick.RemoveAllListeners();
            mCloseBtn.onClick.AddListener(() => 
            {
                this.frame.Close();
            });
        }

        void InitShopGoodDatas()
        {
            List<GoodsData> mLuckCharmGoods = GetGoodDataList(ShopGoodDataType.LuckCharm);
            List<GoodsData> mReComendGoodDatas = GetGoodDataList(ShopGoodDataType.ReComend);
            List<GoodsData> mNormalGoodDatas = GetGoodDataList(ShopGoodDataType.Nomal);
            mReComendGoodDatas.Sort(SortGoodsData);
            mNormalGoodDatas.Sort(SortGoodsData);
            mShopGoodDatas.AddRange(mLuckCharmGoods);
            mShopGoodDatas.AddRange(mReComendGoodDatas);
            mShopGoodDatas.AddRange(mNormalGoodDatas);
        }

        List<GoodsData> GetGoodDataList(ShopGoodDataType type)
        {
            List<GoodsData> mDatas = new List<GoodsData>();
            for (int i = 0; i < shopData.Goods.Count; i++)
            {
                var mGoodData = shopData.Goods[i];
                if (type == ShopGoodDataType.LuckCharm)
                {
                    if (mGoodData.ItemData.Type != ItemTable.eType.INCOME)
                    {
                        continue;
                    }

                    if (mGoodData.ItemData.SubType != (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
                    {
                        continue;
                    }

                    mDatas.Add(mGoodData);
                }
                else if (type == ShopGoodDataType.Nomal)
                {
                    if (mGoodData.ItemData.Type == ItemTable.eType.INCOME)
                    {
                        continue;
                    }

                    if (mGoodData.ItemData.SubType == (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
                    {
                        continue;
                    }

                    if (ShopDataManager.GetInstance().WeaponLeaseIsRecommendOccu(mGoodData.ItemData.TableID))
                    {
                        continue;
                    }

                    mDatas.Add(mGoodData);
                }
                else
                {
                    if (ShopDataManager.GetInstance().WeaponLeaseIsRecommendOccu(mGoodData.ItemData.TableID))
                    {
                        mDatas.Add(mGoodData);
                    }
                }
            }

            return mDatas;
        }

        int SortGoodsData(GoodsData x,GoodsData y)
        {
            if (x.ItemData.TableID != y.ItemData.TableID)
            {
                return x.ItemData.TableID - y.ItemData.TableID;
            }
            return x.ItemData.FixTimeLeft - y.ItemData.FixTimeLeft < 0 ? -1 : 1;
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChange;
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChange;
        }

        void InitWeaponLeaseItemComUIListScript()
        {
            mWeaponLeaseItemComUIListScript.Initialize();
            mWeaponLeaseItemComUIListScript.onBindItem += _OnBindItemDelegate;
            mWeaponLeaseItemComUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            SetElementAmount();
        }

        WeaponLeaseItem _OnBindItemDelegate(GameObject itemObject)
        {
            WeaponLeaseItem item = itemObject.GetComponent<WeaponLeaseItem>();
            return item;
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as WeaponLeaseItem;
            if (current != null && item.m_index >= 0 && item.m_index < shopData.Goods.Count)
            {
                current.UpdateWeaponLeaseItem(frame, mShopGoodDatas[item.m_index], mOnClickLease);
            }
        }
      
        void _RereshAllGoods(UIEvent uiEvent)
        {
            SetElementAmount();
        }

        void _OnMoneyChange(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            if (eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_GOLD || 
                eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_WEAPON_LEASE_TICKET)
            {
                SetElementAmount();
            }
        }

        void SetElementAmount()
        {
            mWeaponLeaseItemComUIListScript.SetElementAmount(mShopGoodDatas.Count);
        }
        public void UnInitView()
        {
            if (mWeaponLeaseItemComUIListScript != null)
            {
                mWeaponLeaseItemComUIListScript.onBindItem -= _OnBindItemDelegate;
                mWeaponLeaseItemComUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                mWeaponLeaseItemComUIListScript = null;
            }
            frame = null;
            mOnClickLease = null;
            shopData = null;
            UnBindUIEvent();
            mShopGoodDatas .Clear();
        }
    }
}

