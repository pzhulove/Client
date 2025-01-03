using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using ProtoTable;

namespace GameClient
{
public class PayRewardItem : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Base Info 

    protected const string effect_obj_1 = "";
    private List<GameObject> m_Effect_objs = new List<GameObject>();

    private Text m_ItemName;
    private Text m_Count;
    private Image m_Icon;
    private GameObject m_RootObj;
    private GameObject m_IconRoot;
    private ComItem m_ComItem;

    private ItemData m_ItemData;

    private bool m_bComItemUsed = false;
    private bool m_bComItemHideCount = false;
    private string m_ItemCountTextFormat = "X{0}";
    private string m_ComItemCountTextFormat = "{0}.{1}万元";

    public void Initialize(ClientFrame dependFrame, ItemData data = null, bool bUsedComItem = true, bool bComItemHideCount=true)
    {
        if (m_ItemName == null)
        {
            m_ItemName = Utility.GetComponetInChild<Text>(this.gameObject, "Desc");
        }
        if (m_Count == null)
        {
            m_Count = Utility.GetComponetInChild<Text>(this.gameObject, "Bg/count");
        }
        if (m_Icon == null)
        {
            m_Icon = Utility.GetComponetInChild<Image>(this.gameObject, "Bg/icon");
                if (bUsedComItem && m_Icon)
                {
                    m_Icon.enabled = false;
                }
        }
        if (m_RootObj == null)
        {
            m_RootObj = Utility.FindChild(this.gameObject, "Bg");
        }
        if (m_Icon != null && m_IconRoot == null)
        {
            m_IconRoot = m_Icon.gameObject;
        }
        if (m_ComItem == null && dependFrame != null && bUsedComItem)
        {
            m_ComItem = dependFrame.CreateComItem(m_IconRoot);
        }

        this.m_ItemData = data;
        m_bComItemHideCount = bComItemHideCount;
        m_bComItemUsed = bUsedComItem;
        m_ItemCountTextFormat = TR.Value("vip_month_card_first_buy_reward_item_count_format");
        m_ComItemCountTextFormat = TR.Value("vip_month_card_first_buy_first_comitem_count_format");
    }

    public void Clear()
    {
        m_ItemName = null;
        m_Count = null;
        m_Icon = null;
        if (m_Effect_objs != null)
        {
            for (int i = 0; i < m_Effect_objs.Count; i++)
            {
                m_Effect_objs[i].CustomActive(false);
            }
            m_Effect_objs.Clear();
        }
        onPayItemClick = null;
        m_ComItem = null;
        m_IconRoot = null;
        m_RootObj = null;

        m_ItemData = null;
        m_bComItemUsed = false;
        m_bComItemHideCount = false;
        m_ItemCountTextFormat = "";
        m_ComItemCountTextFormat = "";
    }

    public void RefreshView(bool bShowName=true, bool bShowCount=true)
    {
        if (m_ItemData == null)
        {
            return;
        }
        if (bShowName)
        {
            SetItemName(m_ItemData.GetColorName());
        }
        else
        {
            SetItemName("");
        }
        if (bShowCount)
        {
            SetItemCount(string.Format(m_ItemCountTextFormat, m_ItemData.Count.ToString()));
        }
        else
        {
            SetItemCount("");
        }

        //注意 刷新这块通用Item 要放在最后 控制是否显示通用Item的Count
        if (m_bComItemUsed)
        {
            RefreshComItem(m_bComItemHideCount);
        }
    }

    public void EnableItemEffect()
    {

    }

    public void SetItemIcon(string spritePath)
    {
        if (m_Icon)
        {
            //Logger.LogErrorFormat("PayRewardItem Icon path is "+spritePath);
            ETCImageLoader.LoadSprite(ref m_Icon, spritePath);
        }
    }

    public void SetItemName(string name)
    {
        if (m_ItemName)
        {
            m_ItemName.text = name;
        }
    }

    public void SetItemCount(string count)
    {
        if (m_Count)
        {
            m_Count.text = count;
        }
    }

    void RefreshComItem(bool bItemHideCount)
    {
        if (m_ComItem != null)
        {
            m_ComItem.Reset();
            if (bItemHideCount)
            {
                m_ItemData.Count = 0;
            }
            else
            {
                int count = m_ItemData.Count;
                int numCount = Mathf.Abs(count).ToString().Length;
                if (numCount > 4)
                {
                    int pow = (int)Mathf.Pow(10, 4);
                    int pow2 = (int)Mathf.Pow(10, 3);
                    int fCount = (int)(count / pow);
                    int sCount = (count % pow) / pow2;

                    string formatCountStr  ="";

                    if (sCount == 0)
                    {
                        formatCountStr = string.Format("{0}", fCount);
                    }
                    else
                    {
                        formatCountStr = string.Format("{0}.{1}", fCount, sCount);
                    }
                    m_ComItem.SetCountFormatter((var) =>
                    {
                        return string.Format(m_ComItemCountTextFormat, formatCountStr);
                    });
                }
                else
                {
                    //重置数目显示状态
                    m_ComItem.SetCountFormatter(null);
                }
            }
            m_ComItem.Setup(m_ItemData, (GameObject obj, ItemData item1) => 
            {
                List<TipFuncButon> funcs = new List<TipFuncButon>();
                TipFuncButon tempFunc = null;

                var dataItem = TableManager.GetInstance().GetTableItem<ItemTable>(item1.TableID);
                if (dataItem != null)
                {
                    if (dataItem.Type == ItemTable.eType.EXPENDABLE && dataItem.SubType == ItemTable.eSubType.GiftPackage && dataItem.ThirdType == ItemTable.eThirdType.HaloGift)
                    {
                        tempFunc = new TipFuncButonSpecial();
                        tempFunc.text = TR.Value("tip_preview");
                        tempFunc.callback = OnPreviewItem;

                        funcs.Add(tempFunc);
                    }
                }
                //添加埋点
                Utility.DoStartFrameOperation("SecondPayFrame", string.Format("ItemID/{0}", item1.TableID));
               ItemTipManager.GetInstance().ShowTip(item1, funcs);
            });
        }
    }

        void OnPreviewItem(ItemData item, object data)
        {
            if(item != null)
            {
                ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, item.TableID);
            }
        }

    #endregion


        #region Callback

        public delegate void OnPayItemClick();

    public OnPayItemClick onPayItemClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onPayItemClick != null)
        {
            onPayItemClick();
        }
    }

    /// <summary>
    /// 需要实现down and up 接口 才能保证click 能正常触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    #endregion
}
}