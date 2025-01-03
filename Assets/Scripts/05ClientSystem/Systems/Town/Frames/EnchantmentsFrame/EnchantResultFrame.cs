using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 装备清除、转换、激活数据
    /// </summary>
    public class EquipmentClearChangeActivationResultData
    {
        public StrengthenGrowthType mStrengthenGrowthType;
        public ItemData mEquipItemData;
    }

    /// <summary>
    /// 材料合成返回数据
    /// </summary>
    public class MaterialsSynthesisResultData
    {
        public int itemId;
        public int itemNumber;
    }

    class EnchantResultFrame : ClientFrame
    {
        public class EnchantResultFrameData
        {
            public bool bMerge;
            public ItemData itemData;
            public int iCardTableID;
            public int iCardLevel;
        }
        EnchantResultFrameData m_kData;

        #region ExtraUIBind
        private GameObject mImageEx9 = null;
        private GameObject mImageEx10 = null;
        private GameObject mHint = null;

        protected override void _bindExUI()
        {
            mImageEx9 = mBind.GetGameObject("ImageEx9");
            mImageEx10 = mBind.GetGameObject("ImageEx10");
            mHint = mBind.GetGameObject("Hint");
        }

        protected override void _unbindExUI()
        {
            mImageEx9 = null;
            mImageEx10 = null;
            mHint = null;
        }
        #endregion

        #region var
        GameObject m_goParent;
        ComItemNew m_kComItem;
        [UIControl("FontRoot/itemname")]
        Text m_kName;
        [UIControl("ScrollView/Viewport/content/Text")]
        Text m_kAttributes;
        [UIControl("BGRoot/TitleLeft")]
        Image m_kTitleLeft;
        [UIControl("BGRoot/TitleRight")]
        Image m_kTitleRight;
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/EnchantResultFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            m_goParent = Utility.FindChild(frame, "common_black/ItemParent");
            m_kComItem = CreateComItemNew(m_goParent);
            
            if (userData is EnchantResultFrameData)
            {
                AudioManager.instance.PlaySound(12);

                SetData(userData as EnchantResultFrameData);
            }
			else if (userData is EquipmentClearChangeActivationResultData)
			{
                EquipmentClearChangeActivationResultData data = userData as EquipmentClearChangeActivationResultData;
                string mTitlePath = string.Empty;
                string attrDesc = string.Empty;
                if (data.mStrengthenGrowthType == StrengthenGrowthType.SGT_Clear)
                {
                    mTitlePath = TR.Value("Item_clear_success_qingchu");
                }
                else if (data.mStrengthenGrowthType == StrengthenGrowthType.SGT_Activate)
                {
                    mTitlePath = TR.Value("Item_activation_success_jihuo");
                    attrDesc = TR.Value("growth_attr_des", EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(data.mEquipItemData.GrowthAttrType), data.mEquipItemData.GrowthAttrNum);
                }
                else if (data.mStrengthenGrowthType == StrengthenGrowthType.SGT_Change)
                {
                    mTitlePath = TR.Value("Item_changed_successed_zhuanhua");
                    attrDesc = TR.Value("growth_attr_des", EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(data.mEquipItemData.GrowthAttrType), data.mEquipItemData.GrowthAttrNum);
                }

                ETCImageLoader.LoadSprite(ref m_kTitleLeft, mTitlePath);

                m_kComItem.Setup(data.mEquipItemData, Utility.OnItemClicked);

                if (data.mEquipItemData != null)
                {
                    m_kName.text = data.mEquipItemData.GetColorName();
                }

                m_kAttributes.text = attrDesc;

                if(attrDesc != string.Empty)
                {
                    _HideGameObject();
                }
            }
            else if (userData is MaterialsSynthesisResultData)
            {
                MaterialsSynthesisResultData data = userData as MaterialsSynthesisResultData;
                ETCImageLoader.LoadSprite(ref m_kTitleLeft, TR.Value("Item_merge_successed_hecheng"));
                m_kAttributes.text = string.Empty;
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.itemId);
                if (itemData != null)
                {
                    itemData.Count = data.itemNumber;
                }
                m_kComItem.Setup(itemData, Utility.OnItemClicked);
                m_kName.text = itemData.GetColorName();
                _HideGameObject();
            }
        }

        protected override void _OnCloseFrame()
        {
            m_kData = null;
        }

        private void _HideGameObject()
        {
            mImageEx9.CustomActive(false);
            mImageEx10.CustomActive(false);
            mHint.CustomActive(false);
        }

        [UIEventHandle("OK")]
        void OnFunction()
        {
            frameMgr.CloseFrame(this);
        }

        void _OnClickCard(GameObject obj, ItemData item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
        }

        void _OnClickItem(GameObject obj, ItemData item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
        }

        public void SetData(EnchantResultFrameData data)
        {
            m_kData = data;

            //m_kHint.text = data.bMerge ? TR.Value("enchant_merge_ok") : TR.Value("enchant_addmagic_ok");

            m_kComItem.Setup(m_kData.itemData, Utility.OnItemClicked);

            if(m_kData.itemData != null)
            {
                m_kName.text = m_kData.itemData.GetColorName();
            }
            else
            {
                m_kName.text = "none";
            }

            if(m_kData.bMerge)
            {
                if (m_kData.iCardLevel == 99)
                {
                    List<ulong> itemIds = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Bxy, ProtoTable.ItemTable.eSubType.ST_BXY_EQUIP);
                    ItemData itemData = null;
                    for (int i = 0; i < itemIds.Count; i++)
                    {
                        if(itemIds[i] == Global.bxyMergeGuid)
                        {
                            itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                            break;
                        }
                    }
                    if (itemData == null)
                    {
                        Logger.LogError("itemData is null");
                    } 
                    else 
                    {
                        List<string> texts = itemData.GetComplexAttrDescs();
                        string text = "";
                        for (int i = 0; i < texts.Count; i++)
                        {
                            text = text + texts[i] + "\n";
                        }                     
                        m_kAttributes.text = text;
                    }
                    Global.bxyMergeGuid = 0;
                }
                else if (m_kData.iCardLevel == 100)
                {
                    List<ulong> itemIds = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Sinan, ProtoTable.ItemTable.eSubType.ST_SINAN);
                    ItemData itemData = null;
                    for (int i = 0; i < itemIds.Count; i++)
                    {
                        if(itemIds[i] == Global.sinanRongheGuid)
                        {
                            itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                            break;
                        }
                    }
                    if (itemData == null)
                    {
                        Logger.LogError("itemData is null");
                    } 
                    else 
                    {
                        List<string> texts = itemData.GetSinanBuffDescs();
                        string text = "";
                        for (int i = 0; i < texts.Count; i++)
                        {
                            text = text + texts[i] + "\n";
                        }                     
                        m_kAttributes.text = text;
                    }
                    Global.sinanRongheGuid = 0;
                }
                else if (m_kData.iCardLevel == 101)
                {
                    m_kAttributes.text = "";
                    ETCImageLoader.LoadSprite(ref m_kTitleLeft, "UI/Image/NewPacked/Duanye_Common.png:Duanye_Txt_Chenggong_Zhuanyi");
                    return;
                }
                else
                {
                    m_kAttributes.text = EnchantmentsCardManager.GetInstance().GetCondition((int)data.itemData.TableID);
                    m_kAttributes.text += "\n";
                    m_kAttributes.text += EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc((int)data.itemData.TableID,data.iCardLevel);
                }
                ETCImageLoader.LoadSprite(ref m_kTitleLeft, TR.Value("Item_merge_successed_hecheng"));
            }
            else
            {
                m_kAttributes.text = string.Format("<color={0}>附魔属性:</color>", "#0FCF6Aff");
                m_kAttributes.text += EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(m_kData.iCardTableID,m_kData.iCardLevel);
                ETCImageLoader.LoadSprite(ref m_kTitleLeft, TR.Value("Item_magic_successed_fumo"));
            }
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
        }
    }
}