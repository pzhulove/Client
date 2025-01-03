using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using Network;

namespace GameClient
{
    class TAPItemSelectFrameData
    {
        public RelationData relation;
        public List<ItemData> datas = new List<ItemData>();
    }

    class TAPDonateFrame : ClientFrame
    {
        public static void Open(RelationData relation)
        {
            if(null == relation)
            {
                return;
            }

            if(!ClientSystemManager.GetInstance().IsFrameOpen<TAPDonateFrame>())
            {
                TAPItemSelectFrameData data = new TAPItemSelectFrameData();
                data.relation = relation;
                data.datas.Clear();
                var itemIds = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
                if(null != itemIds)
                {
                    for(int i = 0; i < itemIds.Count; ++i)
                    {
                        var itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                        if(null == itemData)
                        {
                            continue;
                        }

                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemData.TableID);
                        if (null == item)
                        {
                            continue;
                        }

                        if(item.CanMasterGive == 0)
                        {
                            continue;
                        }

                        if (!(itemData.Quality > ProtoTable.ItemTable.eColor.CL_NONE && itemData.Quality <= ProtoTable.ItemTable.eColor.PURPLE))
                        {
                            continue;
                        }

                        if(relation.level < itemData.LevelLimit)
                        {
                            continue;
                        }

                        bool bHasChangeJob = relation.occu % 10 != 0;
                        bool bOccuFit = false;
                        for(int j = 0; j < item.Occu.Count; ++j)
                        {
                            if(item.Occu[j] < 0)
                            {
                                if(bHasChangeJob)
                                {
                                    bOccuFit = true;
                                    break;
                                }
                            }
                            else if(item.Occu[j] == 0)
                            {
                                bOccuFit = true;
                                break;
                            }
                            else if(item.Occu[j] == relation.occu ||
                                item.Occu[j] == relation.occu / 10 * 10)
                            {
                                bOccuFit = true;
                                break;
                            }
                        }
                        if(!bOccuFit)
                        {
                            continue;
                        }

                        if (itemData.BindAttr == ProtoTable.ItemTable.eOwner.NOTBIND ||
                            itemData.Packing)
                        {
                            data.datas.Add(itemData);
                        }
                    }
                }

                ClientSystemManager.GetInstance().OpenFrame<TAPDonateFrame>(FrameLayer.Middle,data);
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAPSystem/TAPDonateframe";
        }
        [UIControl("ScrollView", typeof(ComUIListScript))]
        ComUIListScript comDonateList;
        [UIControl("ScrollView/Hint", typeof(Text))]
        Text donateHint;
        TAPItemSelectFrameData data = null;
        protected override void _OnOpenFrame()
        {
            data = userData as TAPItemSelectFrameData;
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });
            _AddButton("BtnApply", _Donate);

            if(null != comDonateList)
            {
                comDonateList.Initialize();
                comDonateList.onBindItem = (GameObject go) =>
                {
                    return go.GetComponent<ComTapDonate>();
                };
                comDonateList.onItemVisiable = (ComUIListElementScript item) =>
                {
                    if (null != item && item.m_index >= 0 && item.m_index < data.datas.Count)
                    {
                        var script = item.gameObjectBindScript as ComTapDonate;
                        if (null != script)
                        {
                            script.OnItemVisible(data.datas[item.m_index]);
                        }
                    }
                };
            }

            _UpdateItems();
        }

        void _UpdateItems()
        {
            if(null != comDonateList)
            {
                var tempDonateList = data.datas;
                tempDonateList.RemoveAll(value => value.BEquipIsInsetBead);
                comDonateList.SetElementAmount(tempDonateList.Count);
                donateHint.CustomActive(tempDonateList.Count <= 0);
            }
        }

        protected override void _OnCloseFrame()
        {
            data = null;
            if (null != comDonateList)
            {
                comDonateList.onBindItem = null;
                comDonateList.onItemVisiable = null;
                comDonateList = null;
            }
            ComTapDonate.Clear();
        }

        void _Donate()
        {
            if(ComTapDonate.SelectedItems.Count <= 0)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("donate_needs_item"));
                return;
            }
            MasterGiveEquip kSend = new MasterGiveEquip();
            kSend.discipleId = data.relation.uid;
            kSend.itemUids = ComTapDonate.GetSelectedItems();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER,kSend);
            ComTapDonate.DelecteAllItems();
            //Logger.LogErrorFormat("donate {0} {1}", kSend.discipleId, kSend.itemUids[0]);
            frameMgr.CloseFrame(this);
        }
    }
}
