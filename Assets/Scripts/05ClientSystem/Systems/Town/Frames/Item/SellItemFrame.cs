using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class SellItemFrame : ClientFrame
    {
        protected ItemData m_itemData;

        [UIControl("Price/Image")]
        Image m_imgIcon;

        [UIControl("NumberArea/InputField", typeof(InputField))]
        InputField m_editCount;

        [UIControl("Price/Text")]
        Text m_totalPrice;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/SellItem";
        }

        protected override void _OnOpenFrame()
        {
            if (userData != null)
            {
                m_itemData = (ItemData)userData;
            }
            
            if (m_itemData != null)
            {
                if(m_editCount != null)
                {
                    m_editCount.text = m_itemData.Count.ToString();
                    m_editCount.onValueChanged.RemoveAllListeners();
                    m_editCount.onValueChanged.AddListener((string value) => { _OnValueChanged(value); });
                }
                
                if(m_totalPrice != null)
                {
                    m_totalPrice.gameObject.SetActive(true);
                    int totalPrice = m_itemData.Count * m_itemData.Price;
                    m_totalPrice.text = totalPrice.ToString();
                }

                if(m_imgIcon != null)
                {
                    m_imgIcon.gameObject.SetActive(true);
                    string strIcon = ItemDataManager.GetInstance().GetCommonItemTableDataByID(m_itemData.PriceItemID).Icon;
                    // m_imgIcon.sprite = AssetLoader.GetInstance().LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref m_imgIcon, strIcon);
                }
                
            }
            else
            {
                if (m_editCount != null)
                {
                    m_editCount.text = "0";
                }
                if (m_totalPrice != null)
                {
                    m_totalPrice.gameObject.SetActive(false);
                }
                if (m_imgIcon != null)
                {
                    m_imgIcon.gameObject.SetActive(false);
                }
                Logger.LogError("sell item frame -> item data is null!");
            }
        }

        protected override void _OnCloseFrame()
        {
            m_itemData = null;
        }

        void _OnValueChanged(string value)
        {
            int count = 0;
            if (!int.TryParse(value, out count))
            {
                Logger.LogErrorFormat("value = {0}", value);
            }
            if (count < 0)
            {
                count = 0;
            }
            if (count > m_itemData.Count)
            {
                count = m_itemData.Count;
            }
            int totalPrice = count * m_itemData.Price;

            m_totalPrice.text = totalPrice.ToString();
            m_editCount.text = count.ToString();
        }

        [UIEventHandle("Title/closeicon")]
        void _OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("NumberArea/Increase")]
        void _OnIncreaseNumber()
        {
            int count = int.Parse(m_editCount.text);
            count++;
            if (count >= 1 && count <= m_itemData.Count)
            {
                //m_showCount.text = count.ToString();
                m_editCount.text = count.ToString();
            }
        }

        [UIEventHandle("NumberArea/Decrease")]
        void _OnDecreaseNumber()
        {
            int count = int.Parse(m_editCount.text);
            count--;
            if (count >= 0 && count <= m_itemData.Count)
            {
                m_editCount.text = count.ToString();
            }
        }

        [UIEventHandle("NumberArea/MaxNum")]
        void _OnMaxNumber()
        {
            m_editCount.text = m_itemData.Count.ToString();
        }

        [UIEventHandle("Sell")]
        void _OnSell()
        {
            if (m_itemData != null)
            {
                int count = int.Parse(m_editCount.text);
                if (count >= 1 && count <= m_itemData.Count)
                {
                    if (SecurityLockDataManager.GetInstance().CheckSecurityLock(() =>
                    {
                        return (m_itemData != null && m_itemData.Quality >= ProtoTable.ItemTable.eColor.PURPLE);
                    }))
                    {
                        return;
                    }

                    //增幅装备，增加二次弹框确认
                    if (m_itemData.EquipType == EEquipType.ET_REDMARK)
                    {
                        string mContent = TR.Value("growth_equip_desc", "确定要出售吗");
                        SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                        {
                            SellItem(count);
                        });
                    }
                    else
                    {
                        SellItem(count);
                    }
                }
            }
        }

        private void SellItem(int count)
        {
            ItemDataManager.GetInstance().SellItem(m_itemData, count);
            if (count == m_itemData.Count)
            {
                frameMgr.CloseFrame(this);
                ItemTipManager.GetInstance().CloseAll();
            }
            else
            {
                frameMgr.CloseFrame(this);
            }
        }
    }
}