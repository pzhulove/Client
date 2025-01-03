using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    public class PackagePreciousConfirmFrameParam
    {
        public int Mode; //PackageNewFrame.EItemsShowMode
        public IList<ItemData> ItemList;
        public IList<ItemData> ItemPreciousList;
        public Action CallBack;
    }

    // 珍品类装备分解、出售二次确认界面
    public class PackagePreciousConfirmFrame : ClientFrame
    {
        private PackagePreciousConfirmView mView;
        PackagePreciousConfirmFrameParam mParam;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/PackagePreciousConfirmFrame";
        }

        protected override void _OnOpenFrame()
        {
            var data = userData as PackagePreciousConfirmFrameParam;
            if (data == null || frame == null)
            {
                Close();
            }
            else
            {
                mParam = data;
                mView = frame.GetComponent<PackagePreciousConfirmView>();
                if (mView != null)
                {
                    string tip = TR.Value("package_precious_decompose_confirm_tip", data.ItemPreciousList.Count);
                    string check = TR.Value("package_precious_decopose_check");
                    string confirm = TR.Value("package_precious_decopose_confirm");
                    if ((PackageNewFrame.EItemsShowMode)data.Mode == PackageNewFrame.EItemsShowMode.QuickSell)
                    {
                        tip = TR.Value("package_precious_sell_confirm_tip", data.ItemPreciousList.Count);
                        check = TR.Value("package_precious_sell_check");
                        confirm = TR.Value("package_precious_sell_confirm");
                    }
                    mView.Init(data.ItemList, TR.Value("package_precious_confirm_title"), tip, check, confirm, _OnConfirmClick, _OnCloseClick);
                }
            }
        }

        private void _OnCloseClick()
        {
            Close();
        }

        private void _OnConfirmClick()
        {
            if (mParam != null)
            {
                ulong[] guids = new ulong[mParam.ItemList.Count];
                for (int i = 0; i < guids.Length; ++i)
                {
                    guids[i] = mParam.ItemList[i].GUID;
                }
                if (mParam.Mode == (int)PackageNewFrame.EItemsShowMode.Decompose)
                {
                    ItemDataManager.GetInstance().SendDecomposeItem(guids);
                }
                else
                {
                    ItemDataManager.GetInstance().SendSellItem(guids);
                }
                mParam.CallBack?.Invoke();
            }
            Close();
        }

        protected override void _bindExUI()
        {
            //itemsRoot = mBind.GetGameObject("itemsRoot");
            //itemTemplate = mBind.GetGameObject("itemTemplate");
            //noItemTips = mBind.GetGameObject("noItemTips");
        }

        protected override void _unbindExUI()
        {

        }

    }
}
