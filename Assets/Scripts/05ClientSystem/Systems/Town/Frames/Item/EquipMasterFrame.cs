using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using EEquipPart = ProtoTable.ItemTable.eSubType;

namespace GameClient
{
    class EquipMasterFrame : ClientFrame
    {
        [UIControl("Content/Title/Text")]
        Text m_labTitle;

        [UIControl("Content/Desc/Desc1")]
        Text m_labMasterDesc1;

        [UIControl("Content/Desc/Desc2")]
        Text m_labMasterDesc2;

        class Part
        {
            bool m_bActive;
            Image m_imgBG1;
            UIGray m_comIcon;
            GameObject m_objLine;
            Text m_labName;

            public EEquipPart part;

            public Part(EEquipPart a_part, GameObject a_frame)
            {
                part = a_part;

                m_imgBG1 = Utility.GetComponetInChild<Image>(a_frame, string.Format("Content/State/Part{0}/BG1", (int)a_part));
                m_comIcon = Utility.GetComponetInChild<UIGray>(a_frame, string.Format("Content/State/Part{0}/Icon", (int)a_part));
                m_objLine = Utility.FindGameObject(a_frame, string.Format("Content/State/Part{0}/Line", (int)a_part));
                m_bActive = true;
                SetActive(false);

                m_labName = Utility.GetComponetInChild<Text>(a_frame, string.Format("Content/State/Part{0}/Name", (int)a_part));
                EEquipWearSlotType slotType = (EEquipWearSlotType)((int)a_part);
                m_labName.text = TR.Value(slotType.GetDescription());
            }

            public void SetActive(bool a_bActive)
            {
                if (m_bActive != a_bActive)
                {
                    m_bActive = a_bActive;
                    if (m_imgBG1 != null)
                    {
                        if (m_bActive)
                        {
                            // m_imgBG1.sprite = AssetLoader.instance.LoadRes("UIPacked/p-Armor.png:Armor_icon_jt", typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref m_imgBG1, "UIPacked/p-Armor.png:Armor_icon_jt");
                            m_imgBG1.SetNativeSize();
                        }
                        else
                        {
                            // m_imgBG1.sprite = AssetLoader.instance.LoadRes("UIPacked/p-Armor.png:Armor_icon_N", typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref m_imgBG1, "UIPacked/p-Armor.png:Armor_icon_N");
                            m_imgBG1.SetNativeSize();
                        }
                    }
                    if (m_comIcon != null)
                    {
                        m_comIcon.enabled = !m_bActive;
                    }
                    if (m_objLine != null)
                    {
                        m_objLine.SetActive(m_bActive);
                    }
                }
            }
        }

        List<Part> m_arrParts = new List<Part>();

        List<Text> m_arrValues = new List<Text>();

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/EquipMaster/EquipMaster";
        }

        protected override void _OnOpenFrame()
        {
            _InitUI();

           //// EquipMasterDataManager.JobMasterDesc jonMasterDesc = EquipMasterDataManager.GetInstance().GetJobMasterDesc(PlayerBaseData.GetInstance().JobTableID);
           // m_labTitle.text = jonMasterDesc.title;
           // m_labMasterDesc1.text = jonMasterDesc.effectDesc;
           // m_labMasterDesc2.text = jonMasterDesc.attrDesc;

            EquipProp totalEquipProp = new EquipProp();
            List<ulong> itemGUIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < itemGUIDs.Count; ++i)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(itemGUIDs[i]);
                if (item == null) continue;
                EquipProp equipProp = EquipMasterDataManager.GetInstance().GetEquipMasterProp(PlayerBaseData.GetInstance().JobTableID, item);
                if (equipProp != null)
                {
                    Part part = _GetPart((EEquipPart)item.SubType);
                    if (part != null)
                    {
                        part.SetActive(true);
                    }

                    totalEquipProp += equipProp;
                }
            }

            List<string> propDescs = totalEquipProp.GetPropsFormatStr();
            for (int i = 0; i < propDescs.Count; ++i)
            {
                if (i >= 0 && i < m_arrValues.Count)
                {
                    m_arrValues[i].text = propDescs[i];
                }
                else
                {
                    break;
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
        }

        void _InitUI()
        {
            m_arrParts.Add(new Part(EEquipPart.HEAD, frame));
            m_arrParts.Add(new Part(EEquipPart.CHEST, frame));
            m_arrParts.Add(new Part(EEquipPart.BELT, frame));
            m_arrParts.Add(new Part(EEquipPart.LEG, frame));
            m_arrParts.Add(new Part(EEquipPart.BOOT, frame));

            for (int i = 0; i < 6; ++i)
            {
                Text labValue = Utility.GetComponetInChild<Text>(frame, string.Format("Content/Attr/BG/Value{0}", i));
                if (labValue != null)
                {
                    labValue.text = "";
                    m_arrValues.Add(labValue);
                }
            }
        }

        void _ClearUI()
        {
            m_arrParts.Clear();
            m_arrValues.Clear();
        }

        Part _GetPart(EEquipPart a_part)
        {
            for (int i = 0; i < m_arrParts.Count; ++i)
            {
                if (m_arrParts[i].part == a_part)
                {
                    return m_arrParts[i];
                }
            }
            return null;
        }

        [UIEventHandle("Black")]
        void _OnBackCLicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
