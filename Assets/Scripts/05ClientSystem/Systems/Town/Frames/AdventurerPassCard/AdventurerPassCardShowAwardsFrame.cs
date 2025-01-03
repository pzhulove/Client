using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 购买王者版冒险者通行证大奖预览界面
    public class AdventurerPassCardShowAwardsFrame : ClientFrame
    {
        #region inner define

        #endregion

        #region val  
        List<AwardItemData> awardItemDataListNoraml = null;
        List<AwardItemData> awardItemDataListKing = null;
        #endregion

        #region ui bind
        private Button mBtBuy = null;
        private ComUIListScript mAwardShowNormal = null;
        private ComUIListScript mAwardShowKing = null;
        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventurerPassCard/AdventurerPassCardShowAwards";
        }

        protected override void _OnOpenFrame()
        {
            awardItemDataListNoraml = null;
            awardItemDataListKing = null;

            InitAwardItemsNormal();
            UpdateAwardItemsNormal();

            InitAwardItemsKing();
            UpdateAwardItemsKing();

            mBtBuy.SafeSetGray(AdventurerPassCardDataManager.GetInstance().GetPassCardType != AdventurerPassCardDataManager.PassCardType.Normal);

            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            awardItemDataListNoraml = null;
            awardItemDataListKing = null;

            UnBindUIEvent();      
        }

        protected override void _bindExUI()
        {
            mBtBuy = mBind.GetCom<Button>("btBuy");
            mBtBuy.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardBuyKingLevelFrame>();
                frameMgr.CloseFrame(this);
            });

            mAwardShowNormal = mBind.GetCom<ComUIListScript>("awardShowNormal");
            mAwardShowKing = mBind.GetCom<ComUIListScript>("awardShowKing");
        }

        protected override void _unbindExUI()
        {
            mBtBuy = null;
            mAwardShowNormal = null;
            mAwardShowKing = null;
        }

        #endregion

        #region method

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        void UpdateUI()
        {
            //_OnUpdateAventurePassStatus(null);
        }

        void InitAwardItemsNormal()
        {
            if (mAwardShowNormal == null)
            {
                return;
            }

            mAwardShowNormal.Initialize();

            mAwardShowNormal.onItemVisiable = (item) =>
            {
                if (item == null)
                {
                    return;
                }

                int iIndex = item.m_index;
                if (iIndex >= 0 && awardItemDataListNoraml != null && iIndex < awardItemDataListNoraml.Count)
                {
                    ItemData itemDetailData = ItemDataManager.GetInstance().CreateItem(awardItemDataListNoraml[iIndex].ID, awardItemDataListNoraml[iIndex].Num);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("AdventurerPassCardShowAwardsFrame Can not find item id in item table!!! Please Check item data id {0} !!!", awardItemDataListNoraml[iIndex].ID);
                        return;
                    }

                    ItemData itemData = itemDetailData;
                    if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
                    {
                        itemData.DeadTimestamp = itemData.TableData.ExpireTime;
                    }
                    ShowAwardItem script = item.GetComponent<ShowAwardItem>();
                    if (script != null)
                    {
                        script.OnInit(itemDetailData);
                    }
                }
            };
        }

        List<AwardItemData> GetAwardItems(string awardText)
        {
            if (awardText == null)
            {
                return null;
            }

            List<AwardItemData> itemInfos = new List<AwardItemData>();
            if (itemInfos == null)
            {
                return null;
            }

            string[] items = awardText.Split(new char[] { '|' });
            for (int j = 0; j < items.Length; j++)
            {
                string[] contents = items[j].Split(new char[] { '_' });
                if (contents.Length >= 2)
                {
                    int id = 0;
                    int.TryParse(contents[0], out id);

                    int num = 0;
                    int.TryParse(contents[1], out num);                   

                    AwardItemData itemInfo = new AwardItemData();
                    if (itemInfo != null)
                    {
                        itemInfo.ID = id;
                        itemInfo.Num = num;

                        itemInfos.Add(itemInfo);
                    }
                }
            }

            return itemInfos;
        }

        void CalAwardItemListNormal()
        {
            awardItemDataListNoraml = new List<AwardItemData>();
            if (awardItemDataListNoraml == null)
            {
                return;
            }      

            Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassBuyRewardTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    AdventurePassBuyRewardTable adt = iter.Current.Value as AdventurePassBuyRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.Season == AdventurerPassCardDataManager.GetInstance().SeasonID)
                    {
                        awardItemDataListNoraml.AddRange(GetAwardItems(adt.Normal));
                        break;
                    }
                }
            }

            return;
        }

        void UpdateAwardItemsNormal()
        {
            if (mAwardShowNormal == null)
            {
                return;
            }

            CalAwardItemListNormal();

            if (awardItemDataListNoraml != null)
            {
                mAwardShowNormal.SetElementAmount(awardItemDataListNoraml.Count);
            }
        }

        void InitAwardItemsKing()
        {
            if (mAwardShowKing == null)
            {
                return;
            }

            mAwardShowKing.Initialize();

            mAwardShowKing.onItemVisiable = (item) =>
            {
                if (item == null)
                {
                    return;
                }

                int iIndex = item.m_index;
                if (iIndex >= 0 && awardItemDataListKing != null && iIndex < awardItemDataListKing.Count)
                {
                    ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(awardItemDataListKing[iIndex].ID);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("AdventurerPassCardShowAwardsFrame Can not find item id in item table!!! Please Check item data id {0} !!!", awardItemDataListKing[iIndex].ID);
                        return;
                    }

                    ItemData itemData = itemDetailData;
                    if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
                    {
                        itemData.DeadTimestamp = itemData.TableData.ExpireTime;
                    }

                    itemDetailData.Count = awardItemDataListKing[iIndex].Num;
                    ShowAwardItem script = item.GetComponent<ShowAwardItem>();
                    if (script != null)
                    {
                        script.OnInit(itemDetailData);
                    }
                }
            };
        }

        void CalAwardItemListKing()
        {
            awardItemDataListKing = new List<AwardItemData>();
            if (awardItemDataListKing == null)
            {
                return;
            }

            Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassBuyRewardTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    AdventurePassBuyRewardTable adt = iter.Current.Value as AdventurePassBuyRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.Season == AdventurerPassCardDataManager.GetInstance().SeasonID)
                    {
                        awardItemDataListKing.AddRange(GetAwardItems(adt.High));
                        break;
                    }
                }
            }

            return;
        }

        void UpdateAwardItemsKing()
        {
            if (mAwardShowKing == null)
            {
                return;
            }

            CalAwardItemListKing();

            if (awardItemDataListKing != null)
            {
                mAwardShowKing.SetElementAmount(awardItemDataListKing.Count);
            }
        }

        #endregion

        #region ui event

        void _OnUpdateAventurePassStatus(UIEvent uiEvent)
        {
            return;
        }       

        #endregion
    }
}
