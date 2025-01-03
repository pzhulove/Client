using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;

namespace GameClient
{
    public class CheckEmblemCanLvUp : MonoBehaviour
    {

        [SerializeField]
        GameObject goShow = null;

        // Use this for initialization
        void Start()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateGuildEmblemLvUpRedPoint, _OnUpdateShow);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateShow);

            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;

            _OnUpdateShow(null);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateGuildEmblemLvUpRedPoint, _OnUpdateShow);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateShow);

            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _OnUpdateShow(null);
        }

        void _OnAddNewItem(List<Item> items)
        {
            _OnUpdateShow(null);
        }

        void OnUpdateItem(List<Item> items)
        {
            _OnUpdateShow(null);
        }

        private void _OnUpdateShow(UIEvent uiEvent)
        {
            // 条件1 公会徽记入口要开启
            bool bCondition1 = (PlayerBaseData.GetInstance().Level >= GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit()) 
                && (GuildDataManager.GetInstance().myGuild != null && GuildDataManager.GetInstance().myGuild.nLevel >= GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit())
                && (GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.HONOUR) >= GuildDataManager.GetInstance().GetEmblemLvUpHonourLvLimit());

            // 条件2 荣耀殿堂等级要满足，消耗材料要满足
            int emblemLv = GuildDataManager.GetInstance().GetEmblemLv();
            int nHonourLv = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.HONOUR);
            int nNeedHonourLv = GuildDataManager.GetInstance().GetEmblemNeedHonourLv(emblemLv + 1);
            List<int> notEnoughItemIDs = null;
            bool bCondition2 = (nHonourLv >= nNeedHonourLv) && (GuildDataManager.GetInstance().IsCostEnoughToLvUpEmblem(ref notEnoughItemIDs));

            bool bShow = bCondition1 && bCondition2;
            goShow.CustomActive(bShow);
        }       
    }
}


