using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class BeadPickView : MonoBehaviour
    {
        [SerializeField]
        private GameObject mItemPos;
        [SerializeField]
        private Text mBeadName;
        [SerializeField]
        private Button mOkBtn;
        [SerializeField]
        private Button mCloseBtn;
        [SerializeField]
        private GameObject mPickItemRoot;
        [SerializeField]
        private ToggleGroup mGroup;
        [SerializeField]
        private string mPickItemPath = "UIFlatten/Prefabs/SmithShop/BeadPickFrame/BeadPickItem";
        BeadPickModel beadPickModel;
        UnityAction onOkBtnClick;
        UnityAction onCloseBtnClick;
        List<PickBeadExpendItem> mPickBeadExpendItemList = new List<PickBeadExpendItem>();
        ComItemNew comItem;

        void Start()
        {
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
        }

        public void Init(BeadPickModel model,UnityAction closeCallBack,UnityAction onOkClick)
        {
            if (model == null)
            {
                return;
            }

            beadPickModel = model;
            onCloseBtnClick = closeCallBack;
            onOkBtnClick = onOkClick;
            UpdateBeadItem();
            CreatPickItem();

            mCloseBtn.SafeAddOnClickListener(onCloseBtnClick);
            mOkBtn.SafeAddOnClickListener(onOkBtnClick);
        }

        void UpdateBeadItem()
        {
            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemPos);
            }

            ItemData mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(beadPickModel.mPrecBead.preciousBeadId);
            comItem.Setup(mItemData, (GameObject obj, IItemDataModel item) =>
            {
                if (item != null)
                {
                    mItemData.BeadPickNumber = beadPickModel.mPrecBead.pickNumber;
                    mItemData.BeadReplaceNumber = beadPickModel.mPrecBead.beadReplaceNumber;
                    ItemTipManager.GetInstance().ShowTip(item as ItemData);
                }
            });
            if (mBeadName != null)
            {
                mBeadName.text = mItemData.GetColorName();
            }
        }

        void CreatPickItem()
        {
            for (int i = 0; i < beadPickModel.mBeadPickItemList.Count; i++)
            {
                var mPickItem = beadPickModel.mBeadPickItemList[i];
                if (mPickItem == null)
                {
                    continue;
                }

                GameObject go = AssetLoader.instance.LoadResAsGameObject(mPickItemPath);
                if (go == null)
                {
                    Logger.LogError("加载活动预制体失败，路径:" + mPickItemPath);
                    return;
                }

                go.transform.SetParent(mPickItemRoot.transform, false);
                var item = go.GetComponent<PickBeadExpendItem>();
                if (item != null)
                {
                    item.Init(mPickItem,mGroup,beadPickModel);
                }
            }
        }

        void _OnUpdateItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var mItemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (mItemData == null)
                {
                    continue;
                }

                if (mItemData.GUID == beadPickModel.mEquipItemData.GUID)
                {
                    beadPickModel.mPrecBead.pickNumber = mItemData.PreciousBeadMountHole[beadPickModel.mPrecBead.index - 1].pickNumber;
                    break;
                }

            }

            UpdateBeadItem();
        }

        void OnDestroy()
        {
            beadPickModel = null;
            mCloseBtn.SafeRemoveOnClickListener(onCloseBtnClick);
            mOkBtn.SafeRemoveOnClickListener(onOkBtnClick);
            comItem = null;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
        }
    }
}

