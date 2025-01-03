using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    class StrengthenSpecialConfirmFrameData
    {
        public ItemData itemData;
        public ItemData equipData;
    };

    class StrengthenSpecialConfirmFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/StrengthenSpecialConfirm";
        }

        StrengthenSpecialConfirmFrameData m_kData = null;
        Text title;
        ComItemNew comItem;
        Text name;
        Button btnOk;
        GameObject goParent;
        GameObject goPrefab;
        Text desc;
        protected override void _OnOpenFrame()
        {
            m_kData = userData as StrengthenSpecialConfirmFrameData;
            title = Utility.FindComponent<Text>(frame, "Text");
            btnOk = Utility.FindComponent<Button>(frame, "ok");
            goParent = Utility.FindChild(frame, "middle/body/Viewport/content");
            goPrefab = Utility.FindChild(goParent, "prefabs");
            goPrefab.CustomActive(true);
            comItem = CreateComItemNew(Utility.FindChild(goPrefab, "itemParent"));
            comItem.Setup(null, null);
            name = Utility.FindComponent<Text>(goPrefab, "name");
            desc = Utility.FindComponent<Text>(frame, "image/hintUsed");

            _SetData();
        }

        protected override void _OnCloseFrame()
        {
            m_kData = null;
            title = null;
            comItem.Setup(null,null);
            name = null;
            if(btnOk != null)
            {
                btnOk.onClick.RemoveAllListeners();
                btnOk = null;
            }
        }

        void OnItemClicked(GameObject obj, IItemDataModel item)
        {

        }

        void _SetData()
        {
            if(m_kData != null && m_kData.itemData != null && m_kData.equipData != null)
            {
                var mItemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)m_kData.itemData.TableID);
                if (mItemTable == null)
                {
                    return;
                }

                var item = TableManager.GetInstance().GetTableItem<ProtoTable.StrengthenTicketTable>(mItemTable.StrenTicketDataIndex);
                if(item != null)
                {
                    if (m_kData.equipData.EquipType == EEquipType.ET_COMMON)
                    {
                        title.text = string.Format(TR.Value("strengthen_sp_con_title"), float.Parse(item.desc) * 100, item.Level);
                        desc.text = TR.Value("strength_desc");
                    }
                    else if(m_kData.equipData.EquipType == EEquipType.ET_REDMARK)
                    {
                        title.text = string.Format(TR.Value("growth_sp_con_title"), float.Parse(item.desc) * 100, item.Level);
                        desc.text = TR.Value("growth_desc");
                    }
                }

                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)m_kData.itemData.TableID);
                comItem.Setup(itemData, OnItemClicked);

                if (itemData != null)
                {
                    name.text = itemData.GetColorName();
                }
            }
        }

        [UIEventHandle("ok")]
        void OnClickOk()
        {
            btnOk.enabled = false;
            //StrengthenDataManager.GetInstance().StrengthenItem(m_kData.equipData, false, m_kData.itemData.GUID);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSpecailStrenghthenStart);
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("close/image")]
        void OnClickClose()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSpecailStrenghthenCanceled);
            frameMgr.CloseFrame(this);
        }
    }
}