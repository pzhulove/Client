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
using Network;

namespace GameClient
{
    using UIItemData = AwardItemData;

    public class UltimateChallengeFloorInfoItem : MonoBehaviour
    {
        [SerializeField]
        Image lock1 = null;

        [SerializeField]
        Image lock2 = null;   

        [SerializeField]
        Text floornum = null;  

        [SerializeField]
        GameObject bg1 = null;

        [SerializeField]
        GameObject bg2 = null;

        [SerializeField]
        GameObject floorBg = null;

        [SerializeField]
        RectTransform effectRoot = null;

        [SerializeField]
        GameObject unopen = null;

        ulong tableID = 0;

        static string finishedEffPath = "Effects/UI/Prefab/EffUI_pata/Prefab/EffUI_pata_wancheng";
        static string notOpenEffPath = "Effects/UI/Prefab/EffUI_pata/Prefab/EffUI_pata_notopen";
        static string openEffPath = "Effects/UI/Prefab/EffUI_pata/Prefab/EffUI_pata_open";

        // Use this for initialization
        void Start()
        {
        }

        private void OnDestroy()
        {
            //auctionItemData = null;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().CloseAll();
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        void SetComItemData(ComItem comItem, UIItemData uIItemData)
        {
            if(comItem == null || uIItemData == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                itemData.Count = uIItemData.Num;
                comItem.Setup(itemData, ShowItemTip);
            }

            return;
        }

        string GetColorName(UIItemData uIItemData)
        {
            if(uIItemData == null)
            {
                return "";
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                return itemData.GetColorName();
            }

            return "";
        }
        
        void SetFloorEffect(string path)
        {
            if(effectRoot == null)
            {
                return;
            }

            for (int i = 0; i < effectRoot.childCount; ++i)
            {
                GameObject.Destroy(effectRoot.GetChild(i).gameObject);
            }

            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            GameObject ObjEffect = AssetLoader.GetInstance().LoadRes(path).obj as GameObject;
            if(ObjEffect == null)
            {
                return;
            }

            ObjEffect.transform.SetParent(effectRoot, false);
            ObjEffect.SetActive(true);
        }

        public void SetUp(object data)
        {
            if(data == null)
            {
                return;
            }

            if(!(data is ActivityDataManager.UltimateChallengeFloorData))
            {
                return;
            }

            ActivityDataManager.UltimateChallengeFloorData ultimateChallengeFloorData = data as ActivityDataManager.UltimateChallengeFloorData;

            SetFloorEffect("");
            if (ultimateChallengeFloorData.floor == 0) // 底座
            {
                bg1.CustomActive(false);
                bg2.CustomActive(true);
                floorBg.CustomActive(false);
                lock1.CustomActive(false);
                lock2.CustomActive(false);
                floornum.CustomActive(false);
                unopen.CustomActive(false);
                
                return;
            }            

            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());            
            int index = (int)dateTime.DayOfWeek;
            if(dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                index = 7;
            }
            int maxOpenFloorCount = index * ActivityDataManager.GetInstance().GetUltimateChallengeDungeonDailyOpenFloor();

            bg1.CustomActive(true);
            bg2.CustomActive(false);
            floorBg.CustomActive(true);
            floornum.CustomActive(true);
            unopen.CustomActive(true);

            if (ultimateChallengeFloorData.floor > maxOpenFloorCount) // 没开放
            {
                lock2.CustomActive(true);
                SetFloorEffect("");
                unopen.CustomActive(true);
            }
            else if (ultimateChallengeFloorData.floor == ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor()) // 当前正在打
            {
                lock2.CustomActive(false);
                SetFloorEffect(openEffPath);
                unopen.CustomActive(false);
            }
            else if (ultimateChallengeFloorData.floor < ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor()) // 已经通关
            {
                lock2.CustomActive(false);
                SetFloorEffect(finishedEffPath);
                unopen.CustomActive(false);
            }
            else // 没有通关
            {
                lock2.CustomActive(true);
                SetFloorEffect(notOpenEffPath);
                unopen.CustomActive(false);
            }

            UltimateChallengeDungeonTable ultimateChallengeDungeonTable = TableManager.GetInstance().GetTableItem<UltimateChallengeDungeonTable>(ultimateChallengeFloorData.tableID);
            lock1.CustomActive(ultimateChallengeDungeonTable != null && ultimateChallengeDungeonTable.IsDifficulty > 0);

            floornum.SafeSetText(ultimateChallengeFloorData.floor.ToString());           

            return;
        }
    }
}


