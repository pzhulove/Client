using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameClient;
using Protocol;

class MoneyBinder : MonoBehaviour
{
    public PlayerBaseData.MoneyBinderType m_eMoneyBinderType = PlayerBaseData.MoneyBinderType.MBT_OTHER;
    public Text text = null;
    public Text moneyName = null;
    public Image image = null;
    public int iLinkItemID = 0;
    public MoneyShowType eMoneyShowType = MoneyShowType.MST_NORMAL;
    public enum MoneyShowType
    {
        MST_NORMAL = 0,
        MST_MONEY_NAME = 1,
    }

    public static MoneyBinder Create(GameObject goLocal, Image image,Text text, Text name,int iTable, MoneyShowType eMoneyShowType)
    {
        ItemData data = GameClient.ItemDataManager.CreateItemDataFromTable(iTable);
        if(data != null)
        {
            MoneyBinder comMoney = goLocal.GetComponent<MoneyBinder>();
            if (comMoney == null)
            {
                comMoney = goLocal.AddComponent<MoneyBinder>();
            }

            if(comMoney != null)
            {
                comMoney.m_eMoneyBinderType = PlayerBaseData.MoneyBinderType.MBT_OTHER;
                comMoney.iLinkItemID = iTable;
                comMoney.image = image;
                comMoney.text = text;
                comMoney.moneyName = name;
                comMoney.eMoneyShowType = eMoneyShowType;
                comMoney.Init();
                comMoney.Bind();
                return comMoney;
            }
        }
        return null;
    }

    public int LinkItemID
    {
        get
        {
            return iLinkItemID;
        }
        set
        {
            iLinkItemID = value;
            Init();
        }
    }

    // Use this for initialization
    void Start ()
    {
        Init();
        Bind();
    }

    void Bind()
    {
        ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
        ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
        PlayerBaseData.GetInstance().onMoneyChanged += this.OnMoneyChanged;
    }

    void OnAddNewItem(List<Item> items)
    {
        for(int i = 0; i < items.Count; ++i)
        {
            var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
            if (itemData != null && itemData.TableID == iLinkItemID)
            {
                Init();
                break;
            }
        }
    }

    void OnRemoveItem(ItemData data)
    {
        if(data != null && data.TableID == iLinkItemID)
        {
            Init();
        }
    }

    void OnUpdateItem(List<Item> items)
    {
        for (int i = 0; i < items.Count; ++i)
        {
            var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
            if(itemData != null && itemData.TableID == iLinkItemID)
            {
                Init();
                break;
            }
        }
    }

    void OnDestroy()
    {
        ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
        ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
        PlayerBaseData.GetInstance().onMoneyChanged -= this.OnMoneyChanged;
        text = null;
        image = null;
    }

    void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
    {
        if(eTarget == m_eMoneyBinderType || m_eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_OTHER)
        {
            Init();
        }
    }

    void Init()
    {
        if(text != null)
        {
            if(PlayerBaseData.MoneyBinderType.MBT_OTHER != m_eMoneyBinderType)
            {
                switch (m_eMoneyBinderType)
                {
                    case PlayerBaseData.MoneyBinderType.MBT_GOLD:
                        {
                            text.text = PlayerBaseData.GetInstance().Gold.ToString();
                            iLinkItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.GOLD);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_BIND_GOLD:
                        {
                            text.text = PlayerBaseData.GetInstance().BindGold.ToString();
                            iLinkItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindGOLD);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_POINT:
                        {
                            text.text = PlayerBaseData.GetInstance().Ticket.ToString();
                            iLinkItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.POINT);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_BIND_POINT:
                        {
                            text.text = PlayerBaseData.GetInstance().BindTicket.ToString();
                            iLinkItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_ALIVE_COIN:
                        {
                            text.text = PlayerBaseData.GetInstance().AliveCoin.ToString();
                            iLinkItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.ResurrectionCcurrency);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_WARRIOR_SOUL:
                        {
                            text.text = PlayerBaseData.GetInstance().WarriorSoul.ToString();
                            iLinkItemID = 0;// ItemDataManager.GetInstance().GetTableIDByType(ProtoTable.ItemTable.eSubType.DUEL_COIN);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_FIGHT_COIN:
                        {
                            text.text = PlayerBaseData.GetInstance().uiPkCoin.ToString();
                            iLinkItemID = 0;// ItemDataManager.GetInstance().GetTableIDByType(ProtoTable.ItemTable.eSubType.DUEL_COIN);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_PKMONSETR_COIN:
                        {
                            text.text = PlayerBaseData.GetInstance().uiPkCoin.ToString();
                            iLinkItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.DUEL_COIN);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_GUILD_CONTRIBUTION:
                        {
                            text.text = PlayerBaseData.GetInstance().guildContribution.ToString();
                            iLinkItemID = 0;// ItemDataManager.GetInstance().GetTableIDByType(ProtoTable.ItemTable.eSubType.DUEL_COIN);
                        }
                        break;
                    case PlayerBaseData.MoneyBinderType.MBT_GoodTeacher_Value:
                        {
                            text.text = PlayerBaseData.GetInstance().GoodTeacherValue.ToString();
                            iLinkItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.ST_MASTER_GOODTEACH_VALUE);
                        }
                       
                        break;
                }

                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iLinkItemID);
                if (item != null)
                {
                    if (image != null)
                    {
                        // image.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref image, item.Icon);
                        image.CustomActive(eMoneyShowType == MoneyShowType.MST_NORMAL);
                    }

                    if(moneyName != null)
                    {
                        moneyName.text = item.Name;
                        moneyName.CustomActive(eMoneyShowType == MoneyShowType.MST_MONEY_NAME);
                    }
                }
            }
            else
            {
                int iNum = ItemDataManager.GetInstance().GetOwnedItemCount(iLinkItemID, false);
                text.text = iNum.ToString();
                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iLinkItemID);
                if (item != null)
                {
                    if (image != null)
                    {
                        // image.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref image, item.Icon);
                        image.CustomActive(eMoneyShowType == MoneyShowType.MST_NORMAL);
                    }

                    if (moneyName != null)
                    {
                        moneyName.text = item.Name;
                        moneyName.CustomActive(eMoneyShowType == MoneyShowType.MST_MONEY_NAME);
                    }
                }
            }
        }
    }
}