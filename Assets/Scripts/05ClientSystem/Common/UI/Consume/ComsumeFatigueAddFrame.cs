using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTable;

namespace GameClient
{
    public class ComsumeFatigueAddFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/ComsumeFatigueAdd";
        }

        public static void CommandOpen(object argv)
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<ComsumeFatigueAddFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<ComsumeFatigueAddFrame>();
            }
        }

#region ExtraUIBind
        private Button mAdd0 = null;
        private Button mAdd1 = null;
        private Button mAdd2 = null;
        private GameObject[] mNocountroots = new GameObject[3];
        private ComItemList[] mDrugs = new ComItemList[3];
        private GameObject[] mCountroots = new GameObject[3];

        protected override void _bindExUI()
        {
            mAdd0 = mBind.GetCom<Button>("add0");
            mAdd0.onClick.AddListener(_onAdd0ButtonClick);
            mAdd1 = mBind.GetCom<Button>("add1");
            mAdd1.onClick.AddListener(_onAdd1ButtonClick);
            mAdd2 = mBind.GetCom<Button>("add2");
            mAdd2.onClick.AddListener(_onAdd2ButtonClick);
            mNocountroots[0] = mBind.GetGameObject("nocountroot0");
            mNocountroots[1] = mBind.GetGameObject("nocountroot1");
            mNocountroots[2] = mBind.GetGameObject("nocountroot2");
            mDrugs[0] = mBind.GetCom<ComItemList>("drug0");
            mDrugs[1] = mBind.GetCom<ComItemList>("drug1");
            mDrugs[2] = mBind.GetCom<ComItemList>("drug2");
            mCountroots[0] = mBind.GetGameObject("countroot0");
            mCountroots[1] = mBind.GetGameObject("countroot1");
            mCountroots[2] = mBind.GetGameObject("countroot2");
        }

        protected override void _unbindExUI()
        {
            mAdd0.onClick.RemoveListener(_onAdd0ButtonClick);
            mAdd0 = null;
            mAdd1.onClick.RemoveListener(_onAdd1ButtonClick);
            mAdd1 = null;
            mAdd2.onClick.RemoveListener(_onAdd2ButtonClick);
            mAdd2 = null;
            mNocountroots[0] = null;
            mNocountroots[1] = null;
            mNocountroots[2] = null;
            mDrugs[0] = null;
            mDrugs[1] = null;
            mDrugs[2] = null;
            mCountroots[0] = null;
            mCountroots[1] = null;
            mCountroots[2] = null;
        }
#endregion    

#region Callback
        private void _onAdd0ButtonClick()
        {
            /* put your code in here */
            _useItem(0);
        }
        private void _onAdd1ButtonClick()
        {
            /* put your code in here */
            _useItem(1);
        }
        private void _onAdd2ButtonClick()
        {
            /* put your code in here */
            _useItem(2);
        }
#endregion

        protected override void _OnOpenFrame()
        {
            _updateCount(null);
            _bindEvent();
        }

        protected override void _OnCloseFrame()
        {
            _unbindEvent();
        }

        private void _bindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _updateCount);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, _updateCount);

        }

        private void _unbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _updateCount);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, _updateCount);
        }

        private void _updateCount(UIEvent ui)
        {
            for (int i = 0; i < mDrugs.Length; ++i)
            {
                ComItemList list = mDrugs[i];
                if (null != list && list.mItemDatas.Length > 0)
                {
                    int id = list.mItemDatas[0].id;

                    int count = ItemDataManager.GetInstance().GetOwnedItemCount(id);
                    if (count <= 0)
                    {
                        mNocountroots[i].SetActive(true);
                        mCountroots[i].SetActive(false);
                    }
                    else 
                    {
                        mNocountroots[i].SetActive(false);
                        mCountroots[i].SetActive(true);
                    }

                }
            }
        }

        protected void _useItem(int idx)
        {
            if (idx >= 0 && idx < mDrugs.Length)
            {
                ComItemList list = mDrugs[idx];
                if (null != list && list.mItemDatas.Length > 0)
                {
                    int id = list.mItemDatas[0].id;

                    ItemData data = ItemDataManager.GetInstance().GetItemByTableID(id);
                    if (null != data && data.Count > 0)
                    {
                        ItemDataManager.GetInstance().UseItem(data);
                    }
                    else 
                    {
                        ItemComeLink.OnLink(id, 0, false, () =>
                        {
                            frameMgr.CloseFrame(this, true);
                        });

                    }
                }
            }
        }

    }
}
