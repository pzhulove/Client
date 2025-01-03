using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameClient
{
    public enum TitleType
    {
        TT_NONE,
        TT_INALYSUCCESS,//镶嵌成功
        TT_REPLACEMENTSUCCESS,//置换成功
    }
    class SarahResultFrame : ClientFrame
    {
        public class SarahResultFrameData
        {
            public bool bMerge;
            public ItemData itemData;
            public int iCardTableID;
            public int iHoleIndex;
            public int iTitleType;
        }
        SarahResultFrameData m_kData;

        #region var
        GameObject m_goParent;
        ComItemNew m_kComItem;
        [UIControl("common_black/itemname")]
        Text m_kName;
        [UIControl("ScrollView/Viewport/content/Text")]
        Text m_kAttributes;
        [UIControl("BGRoot/TitleLeft")]
        Image m_kTitle;
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/SarahResultFrame";
        }

        protected override void _OnOpenFrame()
        {
            m_goParent = Utility.FindChild(frame, "common_black/ItemParent");
            m_kComItem = CreateComItemNew(m_goParent);
            
			AudioManager.instance.PlaySound(12);

            SetData(userData as SarahResultFrameData);
        }

        protected override void _OnCloseFrame()
        {
            m_kData = null;
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

        public void SetData(SarahResultFrameData data)
        {
            m_kData = data;

            //m_kHint.text = data.bMerge ? TR.Value("enchant_merge_ok") : TR.Value("enchant_addmagic_ok");

            m_kComItem.Setup(m_kData.itemData, OnItemClicked);

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
                m_kAttributes.text = BeadCardManager.GetInstance().GetCondition((int)data.itemData.TableID);
                m_kAttributes.text += "\n";
                m_kAttributes.text += BeadCardManager.GetInstance().GetAttributesDesc((int)data.itemData.TableID);
            }
            else
            {
                int iBeadTableId = 0;
                int iBeadRandomBuffID = 0;

                for (int i = 0; i < m_kData.itemData.PreciousBeadMountHole.Length; i++)
                {
                    if (m_kData.itemData.PreciousBeadMountHole[i] == null)
                    {
                        continue;
                    }

                    if (m_kData.itemData.PreciousBeadMountHole[i].index != m_kData.iHoleIndex)
                    {
                        continue;
                    }

                    iBeadTableId = m_kData.itemData.PreciousBeadMountHole[i].preciousBeadId;
                    iBeadRandomBuffID = m_kData.itemData.PreciousBeadMountHole[i].randomBuffId;
                }

                m_kAttributes.text = string.Format("<color={0}>宝珠属性:</color>", "#0FCF6Aff");

                if (iBeadRandomBuffID > 0)
                {
                    m_kAttributes.text += BeadCardManager.GetInstance().GetAttributesDesc(iBeadTableId) + "\n" +
                       string.Format("附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(iBeadRandomBuffID));
                }
                else
                {
                    m_kAttributes.text += BeadCardManager.GetInstance().GetAttributesDesc(iBeadTableId);
                }
                
                if (data.iTitleType == (int)TitleType.TT_INALYSUCCESS)
                {
                    ETCImageLoader.LoadSprite(ref m_kTitle, TR.Value("Item_Mosaic_success_xiangqian"));
                }
                else
                {
                    //m_kTitle.text = TR.Value("Replacement_Success");
                }
            }
        }

        void OnItemClicked(GameObject obj, IItemDataModel item)
        {
            ItemTipManager.GetInstance().ShowTip(item as ItemData);
        }
    }
}