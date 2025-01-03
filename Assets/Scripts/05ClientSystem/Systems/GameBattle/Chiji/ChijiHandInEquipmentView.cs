using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class ChijiHandInEquipmentView : MonoBehaviour
    {
        ChijiNpcData m_NpcData = new ChijiNpcData();

        [SerializeField]private ComUIListScript mEquipItemListScripts;
        [SerializeField]private Button mCloseBtn;
        [SerializeField]private Button mSubmitBtn;
        [SerializeField]private int mMaxHandInNumber = 5;//最大上交数量

        public static List<ulong> mHandInEquipmentList = new List<ulong>();
        private List<ItemData> mAllEquipmentList = new List<ItemData>();
        public static int hasSelectNumber = 0;
        private void Awake()
        {
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveAllListeners();
                mCloseBtn.onClick.AddListener(OnCloseBtnClick);
            }

            if (mSubmitBtn != null)
            {
                mSubmitBtn.onClick.RemoveAllListeners();
                mSubmitBtn.onClick.AddListener(OnSubmitBtnClick);
            }

            InitEquipItemListScripts();
        }

        private void OnDestroy()
        {
            if (mHandInEquipmentList != null)
            {
                mHandInEquipmentList.Clear();
            }

            if (mAllEquipmentList != null)
            {
                mAllEquipmentList.Clear();
            }

            if (m_NpcData != null)
            {
                m_NpcData.guid = 0;
                m_NpcData.npcTableId = 0;
            }

            UnInitEquipItemListScripts();
        }

        public void InitView(ChijiNpcData NpcData)
        {
            m_NpcData = NpcData;

            if (mHandInEquipmentList == null)
            {
                mHandInEquipmentList = new List<ulong>();
            }
            else
            {
                mHandInEquipmentList.Clear();
            }

            if (mAllEquipmentList == null)
            {
                mAllEquipmentList = new List<ItemData>();
            }
            else
            {
                mAllEquipmentList.Clear();
            }

            LoadAllEquipment();
        }

        private void LoadAllEquipment()
        {
            var allEquipment = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            for (int i = 0; i < allEquipment.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(allEquipment[i]);
                if (itemData == null)
                {
                    continue;
                }

                mAllEquipmentList.Add(itemData);
            }

            if (mEquipItemListScripts != null)
                mEquipItemListScripts.SetElementAmount(mAllEquipmentList.Count);
        }

        private void InitEquipItemListScripts()
        {
            if (mEquipItemListScripts != null)
            {
                mEquipItemListScripts.Initialize();
                mEquipItemListScripts.onBindItem += OnBindItemDelegate;
                mEquipItemListScripts.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitEquipItemListScripts()
        {
            if (mEquipItemListScripts != null)
            {
                mEquipItemListScripts.onBindItem -= OnBindItemDelegate;
                mEquipItemListScripts.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private ChijiHandInEquipmentItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ChijiHandInEquipmentItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var equipmentItem = item.gameObjectBindScript as ChijiHandInEquipmentItem;
            if (equipmentItem != null && item.m_index >= 0 && item.m_index < mAllEquipmentList.Count)
            {
                equipmentItem.OnItemVisiable(mAllEquipmentList[item.m_index], OnEquipItemClick);
            }
        }

        /// <summary>
        /// toggle回调
        /// </summary>
        /// <param name="guid">道具GUID</param>
        /// <param name="isAdd">是否添加</param>
        private void OnEquipItemClick(ulong guid, bool isAdd)
        {
            if (isAdd)
            {
                mHandInEquipmentList.Add(guid);
            }
            else
            {
                mHandInEquipmentList.Remove(guid);
            }

            hasSelectNumber = mHandInEquipmentList.Count;
        }

        private void OnCloseBtnClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<ChijiHandInEquipmentFrame>();
        }

        private void OnSubmitBtnClick()
        {
            if (mHandInEquipmentList.Count < mMaxHandInNumber)
            {
                SystemNotifyManager.SysNotifyTextAnimation(string.Format("选择的装备不足{0}个，不可提交!", mMaxHandInNumber));
                return;
            }

            string mContent = string.Format("是否确定上交选中的{0}件装备?", mMaxHandInNumber);
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, _Subitmit);
        }

        private  void _Subitmit()
        {
            ChijiDataManager.GetInstance().SendNpcTradeReq((uint)m_NpcData.npcTableId, m_NpcData.guid, mHandInEquipmentList);
            ClientSystemManager.GetInstance().CloseFrame<ChijiNpcDialogFrame>();

            OnCloseBtnClick();
        }
    }
}