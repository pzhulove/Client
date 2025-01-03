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
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 时装分解界面
    public class FashionEquipDecomposeFrame : ClientFrame
    {
        #region inner define

        public class DecomposeKey
        {
            public ItemTable.eSubType eSubType;
            public ItemTable.eThirdType eThirdType;
            public ItemTable.eColor eColor;

            public override bool Equals(object obj)
            {
                DecomposeKey other = obj as DecomposeKey;
                if(other == null)
                {
                    return false;
                }

                return other.eSubType == this.eSubType && other.eThirdType == this.eThirdType && other.eColor == this.eColor;
            }

            public override int GetHashCode()
            {
                return eSubType.GetHashCode() + eThirdType.GetHashCode() + eColor.GetHashCode();
            }
        }

        #endregion

        #region val  
        List<ulong> itemIDs = null;

        static Dictionary<DecomposeKey, List<int>> decomposeKey2InscriptionID = new Dictionary<DecomposeKey, List<int>>();
        static Dictionary<int, List<int>> fashionTableID2InscriptionID = new Dictionary<int, List<int>>();

        const int maxInscriptionIDNum = 5;

        #endregion

        #region ui bind
        Button Cancel = null;
        Button Confirm = null;
        GameObject itemsRoot = null;
        GameObject itemTemplate = null;
        GameObject noItemTips = null;
        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/FashionEquipDecompose";
        }

        protected override void _OnOpenFrame()
        {
            itemIDs = new List<ulong>();
            if(userData != null && userData is List<ulong>)
            {
                itemIDs = (List<ulong>)userData;
            }

            InitDecomposeTableData();

            BindUIEvent();

            UpdateUI();
        }

        protected override void _OnCloseFrame()
        {
            itemIDs = null;
            UnBindUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CloseFashionEquipDecompose);
        }

        protected override void _bindExUI()
        {
            Cancel = mBind.GetCom<Button>("Cancel");
            Cancel.SafeSetOnClickListener(() => 
            {                
                frameMgr.CloseFrame(this);
            });

            Confirm = mBind.GetCom<Button>("Confirm");
            Confirm.SafeSetOnClickListener(() => 
            {
                if(itemIDs != null)
                {
                    // 分解高级时装要进行提示
                    List<ulong> highValueEquipID = new List<ulong>();
                    if(highValueEquipID != null)
                    {
                        for(int i = 0;i < itemIDs.Count;i++)
                        {
                            ItemData itemData = ItemDataManager.GetInstance().GetItem(itemIDs[i]);
                            if(itemData == null)
                            {
                                continue;
                            }

                            //itemData.SuitID == 101139，天穹套装添加到确认提示里面
                            if (itemData.Quality > ItemTable.eColor.PURPLE || itemData.ThirdType == ItemTable.eThirdType.FASHION_FESTIVAL || itemData.SuitID == 101139)
                            {
                                highValueEquipID.Add(itemIDs[i]);
                            }
                        }

                        highValueEquipID.Sort((x, y) => 
                        {
                            var left = ItemDataManager.GetInstance().GetItem(x);
                            var right = ItemDataManager.GetInstance().GetItem(y);

                            if (left != null && right != null)
                            {
                                if (left.Quality != right.Quality)
                                {
                                    return right.Quality - left.Quality;
                                }

                                return left.TableID - right.TableID;
                            }

                            return -1;
                        });

                        ulong[] ids = itemIDs.ToArray();
                        if (highValueEquipID.Count == 0)
                        {
                            ItemDataManager.GetInstance().SendDecomposeItem(ids,true);
                        }
                        else 
                        {
                            var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_CONFIRT_COUNTDOWN_TIME);
                            int countDownTime = systemValue.Value;

                            SystemNotifyManager.SysNotifyMsgBoxCancelOk(GetMultiDecomposeTipDesc(highValueEquipID), "","",null, () =>
                            {
                                ItemDataManager.GetInstance().SendDecomposeItem(ids,true);
                            }, countDownTime, false,null,true);
                        }                       
                    }                                                      
                }                
            });

            itemsRoot = mBind.GetGameObject("itemsRoot");
            itemTemplate = mBind.GetGameObject("itemTemplate");
            noItemTips = mBind.GetGameObject("noItemTips");
        }

        protected override void _unbindExUI()
        {
            Cancel = null;
            Confirm = null;
            itemsRoot = null;
            itemTemplate = null;
            noItemTips = null;
        }

        #endregion

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SelectFashionEquipToDecompose, _OnSelectFashionEquipToDecompose);    
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SelectFashionEquipToDecompose, _OnSelectFashionEquipToDecompose);     
        }

        string GetMultiDecomposeTipDesc(List<ulong> guids)
        {
            if(guids == null)
            {
                return "";
            }

            if(guids.Count == 0)
            {
                return "";
            }

            if(guids.Count == 1)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[0]);
                if(itemData != null)
                {
                    return TR.Value("decompose_one_fashion_tips", itemData.GetQualityDesc(), itemData.GetColorName());
                }
            }
            else
            {
                string nameStr = "";
                for(int i = 0;i < guids.Count && i < 6;i++) // 最多只显示6个时装名字
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                    if(itemData == null)
                    {
                        continue;
                    }

                    nameStr += TR.Value("decompose_fashion_name", itemData.GetColorName());
                }

                return TR.Value("decompose_multiple_fashion_tips", nameStr, guids.Count);
            }

            return "";
        }

        void InitDecomposeTableData()
        {
            if (decomposeKey2InscriptionID != null && fashionTableID2InscriptionID != null)
            {
                if (decomposeKey2InscriptionID.Count == 0 || fashionTableID2InscriptionID.Count == 0)
                {
                    decomposeKey2InscriptionID.Clear();
                    fashionTableID2InscriptionID.Clear();

                    Dictionary<int, object> dicts = TableManager.instance.GetTable<FashionDecomposeTable>();
                    if (dicts != null)
                    {
                        var iter = dicts.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            FashionDecomposeTable adt = iter.Current.Value as FashionDecomposeTable;
                            if (adt == null)
                            {
                                continue;
                            }

                            DecomposeKey decomposeKey = new DecomposeKey()
                            {
                                eSubType = (ItemTable.eSubType)adt.SubType,
                                eThirdType = (ItemTable.eThirdType)adt.ThirdType,
                                eColor = (ItemTable.eColor)adt.Color,
                            };

                            List<int> itemIds = new List<int>();
                            itemIds.AddRange(adt.Text);

                            decomposeKey2InscriptionID.SafeAdd(decomposeKey, itemIds);

                            if (adt.FashionID > 0)
                            {
                                List<int> itemIds2 = new List<int>();
                                itemIds2.AddRange(adt.Text);
                                fashionTableID2InscriptionID.SafeAdd(adt.FashionID, itemIds2);
                            }
                        }
                    }
                }
            }
        }

        void UpdateUI()
        {
            bool hasNoItem = (itemIDs == null || itemIDs.Count == 0);
            Confirm.SafeSetGray(hasNoItem);
            noItemTips.CustomActive(hasNoItem);
            itemsRoot.CustomActive(!hasNoItem);

            UpdateDecomposeItems();            
        }

        List<int> CalcDecomposeInscriptionList()
        {
            if(decomposeKey2InscriptionID == null || fashionTableID2InscriptionID == null)
            {
                return null;
            }

            if(itemIDs == null)
            {
                return null;
            }

            List<int> ids = new List<int>();
            if(ids == null)
            {
                return null;
            }

            for(int i = 0;i < itemIDs.Count;i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(itemIDs[i]);
                if(itemData == null)
                {
                    continue;
                }

                if(fashionTableID2InscriptionID.ContainsKey(itemData.TableID))
                {
                    ids.AddRange(fashionTableID2InscriptionID[itemData.TableID]);
                    continue;
                }

                if(itemData.TableData == null)
                {
                    continue;
                }

                DecomposeKey decomposeKey = new DecomposeKey()
                {
                    eSubType = (ItemTable.eSubType)itemData.SubType,
                    eThirdType = (ItemTable.eThirdType)itemData.ThirdType,
                    eColor = (ItemTable.eColor)itemData.TableData.Color,
                };

                if(decomposeKey == null)
                {
                    continue;
                }

                if(decomposeKey2InscriptionID.ContainsKey(decomposeKey))
                {
                    ids.AddRange(decomposeKey2InscriptionID[decomposeKey]);
                }
            }

            // 这里根据品级排个序
            ids.Sort((a, b) => 
            {
                ItemData itemDataA = ItemDataManager.CreateItemDataFromTable(a);
                ItemData itemDataB = ItemDataManager.CreateItemDataFromTable(b);

                if(itemDataA != null && itemDataB != null)
                {
                    return itemDataB.Quality.CompareTo(itemDataA.Quality);
                }

                return 0;
            });

            return ids;
        }

        void UpdateDecomposeItems()
        {
            if (itemsRoot == null)
            {
                return;
            }

            if(itemTemplate == null)
            {
                return;
            }

            for (int i = 0; i < itemsRoot.transform.childCount; ++i)
            {
                GameObject go = itemsRoot.transform.GetChild(i).gameObject;
                GameObject.Destroy(go);
            }

            List<int> data = CalcDecomposeInscriptionList();
            if(data == null)
            {
                return;
            }

            Dictionary<int, int> itemID2Count = new Dictionary<int, int>();
            if(itemID2Count == null)
            {
                return;
            }

            for (int i = 0; i < data.Count; i++)
            {
                int itemID = data[i];
                int count = itemID2Count.SafeGetValue(itemID);
                count++;
                itemID2Count.SafeAdd(itemID, 1);

                if(itemID2Count.Count > maxInscriptionIDNum)
                {
                    break;
                }
            }

            foreach(var pair in itemID2Count)
            {
                int itemID = pair.Key;                

                GameObject goCurrent = GameObject.Instantiate(itemTemplate.gameObject);
                Utility.AttachTo(goCurrent, itemsRoot);
                goCurrent.CustomActive(true);

                ComCommonBind bind = goCurrent.GetComponent<ComCommonBind>();
                if (bind == null)
                {
                    continue;
                }

                ComItem comItem = bind.GetCom<ComItem>("Item");
                if (comItem == null)
                {
                    continue;
                }

                ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemID);
                if(itemData == null)
                {
                    continue;
                }
                itemData.Count = pair.Value;

                comItem.Setup(itemData, (go,item) => 
                {               
                    ItemTipManager.GetInstance().ShowTip(item);
                });

                Text name = bind.GetCom<Text>("name");
                name.SafeSetText(CommonUtility.GetItemColorName(itemData.TableData));
            }
        }

        #endregion

        #region ui event

        void _OnSelectFashionEquipToDecompose(UIEvent uiEvent)
        {
            if(uiEvent == null)
            {
                return;
            }

            if(itemIDs == null)
            {
                return;
            }

            ulong itemID = (ulong)uiEvent.Param1;
            bool bSelect = (bool)uiEvent.Param2;

            if(bSelect)
            {
                itemIDs.Add(itemID);
            }
            else
            {
                itemIDs.Remove(itemID);
            }

            UpdateUI();

            return;
        }      

        #endregion
    }
}
