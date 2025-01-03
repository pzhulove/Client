using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;

namespace GameClient
{
    class FashionAvatarSeleteFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionAvatarSeleteFrame";
        }

        [UIControl("middle/ItemListView", typeof(ComUIListScript))]
        ComUIListScript comFashionListScript;
        [UIControl("middle/ActorShow", typeof(ComModelBinder))]
        ComModelBinder comModelBinder;
        ItemData mBindValue = null;

        void _InitializeFashions()
        {
            if (null != comFashionListScript)
            {
                comFashionListScript.Initialize();
                comFashionListScript.onBindItem = (GameObject go) =>
                {
                    if (null != go)
                    {
                        return go.GetComponent<ComGridItem>();
                    }
                    return null;
                };
                comFashionListScript.onItemVisiable = (ComUIListElementScript item) =>
                {
                    if (null != item && null != mDatas && item.m_index >= 0 && item.m_index < 80)
                    {
                        var script = item.gameObjectBindScript as ComGridItem;
                        if (null != script)
                        {
                            ItemData value = item.m_index >= mDatas.Count ? null : mDatas[item.m_index];
                            script.OnItemVisible(value);
                        }
                    }
                };
                comFashionListScript.onItemSelected = (ComUIListElementScript item) =>
                {
                    if (null != item)
                    {
                        ComGridItem script = item.gameObjectBindScript as ComGridItem;
                        if (null != script)
                        {
                            if(null != comModelBinder)
                            {
                                mBindValue = script.Value;
                                comModelBinder.SetFashion(script.Value);
                            }
                        }
                    }
                };
                comFashionListScript.onItemChageDisplay = (ComUIListElementScript item, bool bSelect) =>
                {
                    if (null != item)
                    {
                        ComGridItem script = item.gameObjectBindScript as ComGridItem;
                        if (null != script)
                        {
                            script.OnItemChangeDisplay(bSelect);
                        }
                    }
                };
            }
        }

        void _UnInitializeFashions()
        {
            if(null != comFashionListScript)
            {
                comFashionListScript.onBindItem = null;
                comFashionListScript.onItemVisiable = null;
                comFashionListScript.onItemSelected = null;
                comFashionListScript.onItemChageDisplay = null;
                comFashionListScript = null;
            }
        }

        List<ItemData> mDatas = null;

        void _UpdateFashions()
        {
            if(null != comFashionListScript)
            {
                comFashionListScript.SetElementAmount(80);
            }
        }

        protected override void _OnOpenFrame()
        {
            mDatas = userData as List<ItemData>;
            _AddButton("middle/btOK", () => 
            {
                if (null != mBindValue)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNormalFashionSelected, mBindValue);
                    mBindValue = null;
                }
                frameMgr.CloseFrame(this);
            });
            _AddButton("middle/btRefresh", () => 
            {
                _ResetFashions();
                if(null != comFashionListScript)
                {
                    comFashionListScript.SelectElement(-1);
                }
                mBindValue = null;
            });
            _AddButton("middle/btClose", () => { frameMgr.CloseFrame(this); });

            _InitializeFashions();
            _UpdateFashions();

            _InitAvatar();
            _ResetFashions();
        }

        void _InitAvatar()
        {
            if (null != comModelBinder)
            {
                comModelBinder.LoadAvatar(PlayerBaseData.GetInstance().JobTableID);
                comModelBinder.LoadWeapon();
            }
        }

        void _ResetFashions()
        {
            if(null != comModelBinder)
            {
                var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                List<ItemData> datas = new List<ItemData>();
                for (int i = 0; i < uids.Count; ++i)
                {
                    ItemData data = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (null != data)
                    {
                        datas.Add(data);
                    }
                }
                comModelBinder.SetFashions(datas);
            }
        }

        protected override void _OnCloseFrame()
        {
            mDatas = null;
            _UnInitializeFashions();
        }
    }
}