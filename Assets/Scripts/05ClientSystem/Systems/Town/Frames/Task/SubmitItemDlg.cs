using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.Events;
using Protocol;
using ProtoTable;
using Network;
using Scripts.UI;


namespace GameClient
{
    public class SubmitItemDlg : ClientFrame
    {
        protected UInt32 param1;
        protected UInt32 param2;
        protected UInt32 param3;

        protected List<ulong> itemList = new List<ulong>();
        protected int itemSelectIndex = 0;

        protected ItemData materialItem;

        ComUIListScript submitEquip = null;

        protected List<ulong> GetItemToShow(int iMissionID)
        {
            if (param1 == 0)
            {
                List<ulong> items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
                if (items.Count > 0)
                {
                    items = items.FindAll((id) =>
                    {
                        var itemData = ItemDataManager.GetInstance().GetItem(id);
                        if (itemData == null) return false;
                        if(itemData.bLocked)
                        {
                            return false;
                        }
                        UInt32 curQuality = (UInt32)itemData.Quality;

                        if (itemData.EquipType == EEquipType.ET_REDMARK)
                        {
                            return false;
                        }

                        if(itemData.LevelLimit < param3)
                        {
                            return false;
                        }

                        if(curQuality < param2)
                        {
                            return false;
                        }

                        //在未穿戴的装备方案中，不添加
                        if (itemData.IsItemInUnUsedEquipPlan == true)
                            return false;
						
						bool ret = false;

						if (param2 == (int)ProtoTable.ItemTable.eColor.WHITE)
						{
							if (curQuality >= (int)ProtoTable.ItemTable.eColor.WHITE && curQuality <= (int)ProtoTable.ItemTable.eColor.BLUE)
								ret = true;
						}
						else{
							if (curQuality == param2)
								ret = true;
						}

						ret = ret && itemData.StrengthenLevel<8;
								
						return ret;
                    });

                    items.Sort((left, right) =>
                    {
                        var leftItem = ItemDataManager.GetInstance().GetItem(left);
                        var rightItem = ItemDataManager.GetInstance().GetItem(right);
                        if (rightItem == null || leftItem == null) return 0;
                        if (rightItem.Quality == leftItem.Quality)
                        {
                            if(rightItem.LevelLimit == leftItem.LevelLimit)
                            {
                                return 0;
                            }
                            else if(rightItem.LevelLimit > leftItem.LevelLimit)
                            {
                                return -1;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                        else  if (rightItem.Quality > leftItem.Quality)
                        {
                             return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    });
                }

                return items;
            }
            else if (param1 == 1)
            {
                List<ulong> items = new List<ulong>();
                var curItem = ItemDataManager.GetInstance().GetItemByTableID((int)param2);
                if (curItem != null)
                {
                    //在未启用的装备方案中,不添加
                    if (curItem.IsItemInUnUsedEquipPlan == true)
                    {
                        //不添加
                    }
                    else
                    {
                        items.Add(curItem.GUID);
                    }
                }
                return items;
            }
            else
            {
                Logger.LogErrorFormat("[上交物品任务]，任务<{0}>,MissionParam1 错{1}，目前只支持（0装备，1材料)", iMissionID, param1);
                return null;
            }
        }

        protected override void _OnOpenFrame()
        {
            int iMissionID = (int)userData;
            ProtoTable.MissionTable table = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iMissionID);

            if (table != null)
            {
                string[] list = table.MissionParam.Split(',');

                if (list.Length != 3)
                {
                    Logger.LogErrorFormat("[上交物品任务]，任务<{0}>,{1}参数个数填错 {2} != 3", iMissionID, table.TaskName, list.Length);
                }

                try
                {
                    param1 = UInt32.Parse(list[0]);
                    param2 = UInt32.Parse(list[1]);
                    param3 = UInt32.Parse(list[2]);
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("[上交物品任务]，任务<{0}>,{1} 参数转换错误", iMissionID, table.TaskName, e.ToString());
                }

            }
            else
            {
                Logger.LogErrorFormat("[上交物品任务]，任务<{0}> ID错误，查不到数据", iMissionID);
            }

            itemList = GetItemToShow(iMissionID);
            if(submitEquip != null)
            {
                ItemData itemDataFirst = null;

                submitEquip.Initialize();
                submitEquip.onBindItem = (go) =>
                {
                    return go;
                };

                submitEquip.onItemVisiable = (go) =>
                {
                    int i = go.m_index;
                    ComCommonBind bind = go.GetComponent<ComCommonBind>();
                    if(bind == null)
                    {
                        return;
                    }
                        itemImage[i] = bind.GetCom<Image>("Item");
                        itemSelectImage[i] = bind.GetCom<Image>("Image");
                        itemNum[i] = bind.GetCom<Text>("Num");
                    if(itemImage[i] == null || itemSelectImage[i] == null || itemNum[i] == null)
                    {
                        return;
                    }

                    if (itemList.Count > 0)
                    {
                        if (i < itemList.Count)
                        {
                            var itemID = (ulong)itemList[i];
                            ComItem cur = itemImage[i].GetComponentInChildren<ComItem>();
                            if (cur == null)
                            {
                                cur = CreateComItem(itemImage[i].gameObject);
                            }                   
                            cur.gameObject.transform.SetAsFirstSibling();
                            ItemData itemTemp = null;

                            if (param1 == 0)
                            {
                                itemNum[i].text = "1/1";
                                ItemData itemData = ItemDataManager.GetInstance().GetItem(itemID);
                                if (itemData != null)
                                {
                                    itemTemp = ItemDataManager.CreateItemDataFromTable((int)itemData.TableID);
                                    itemTemp.StrengthenLevel = itemData.StrengthenLevel;
                                    itemTemp.Count = 1;
                                }
                            }
                            else if (param1 == 1)
                            {
                                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)param2);
                                itemTemp = ItemDataManager.CreateItemDataFromTable((int)param2);
                                itemTemp.Count = 1;
                                itemNum[i].text = string.Format("{0}/{1}", iHasCount, param3);
                            }
                            int localIndex = i;
                            cur.Setup(itemTemp, (obj, item) => { OnItemClicked(go.gameObject, item, localIndex); });

                            itemSelectImage[i].enabled = (itemSelectIndex == i);

                            if (i == 0)
                            {
                                itemDataFirst = itemTemp;
                            }

                        }
                    }
                    else
                    {
                        if(go.m_index == 0)
                        {
                            ItemData itemTemp = null;

                            ComItem cur = itemImage[0].GetComponentInChildren<ComItem>();
                            if (cur == null)
                            {
                                cur = CreateComItem(itemImage[0].gameObject);
                            }
                            cur.gameObject.transform.SetAsFirstSibling();

                            if (param1 == 0)
                            {
                                //itemNum[0].text = "0/1";
                                //itemTemp = ItemDataManager.GetInstance().GetItem((ulong)itemID);
                            }
                            else if (param1 == 1)
                            {
                                itemTemp = ItemDataManager.CreateItemDataFromTable((int)param2);
                                itemTemp.Count = 1;
                                itemNum[0].text = string.Format("0/{0}", param3);
                                cur.Setup(itemTemp, null);
                            }
                            //cur.Setup(itemTemp,null);
                        }
                    }
                };

                int count = itemList.Count > 3 ? itemList.Count : 3;
                itemImage = new Image[count];
                itemNum = new Text[count];
                itemSelectImage = new Image[count];

                submitEquip.SetElementAmount(count);

                OnItemClicked(null, itemDataFirst, 0);
            }           
        }

        void OnItemClicked(GameObject obj, ItemData item, int index)
        {
            int idx = index;
            if(obj != null)
            {
                ComUIListElementScript comUIListElementScript = obj.GetComponent<ComUIListElementScript>();
                if(comUIListElementScript != null)
                {
                    idx = comUIListElementScript.m_index;
                }
            }

            SetSelectItem(idx);
            textShow.text = GetTextShow(item);
        }

        string GetTextShow(ItemData item)
        {
            if(item == null)
            {
                return "";
            }

			var qualityInfo = item.GetQualityInfo();
			string equipName = string.Format("<color={0}>{1}</color>", qualityInfo.ColStr, item.Name);

			return string.Format("当前选择：{0}", equipName);
        }

        void Reset()
        {
            itemList = new List<ulong>();
            itemSelectIndex = 0;
        }
        protected override void _OnCloseFrame()
        {
            Reset();
        }

        protected override void _bindExUI()
        {
            submitEquip = mBind.GetCom<ComUIListScript>("submitEquip");
        }

        protected override void _unbindExUI()
        {
            submitEquip = null;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/SubmitItem";
        }


        [UIEventHandle("Submit")]
        void OnClickOK()
        {
            ulong itemID = GetSelectItem();

            if (itemID > 0)
            {
                if(param1 == 1)
                {
                    var count = ItemDataManager.GetInstance().GetOwnedItemCount((int)param2);
                    if(count < param3)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Task_Submit_Tip_Content"));
                        return;
                    }
                }
                
				ItemData itemData = ItemDataManager.GetInstance().GetItem(itemID);
				if (itemData != null && itemData.StrengthenLevel > 0)
				{
					var qualityInfo = itemData.GetQualityInfo();
					string equipName = string.Format("<color={0}>{1}</color>", qualityInfo.ColStr, itemData.Name);
					string content = string.Format(TR.Value("mission_submit_strengthLevel"), "+"+itemData.StrengthenLevel+" "+equipName);
					SystemNotifyManager.SysNotifyMsgBoxCancelOk(content, null, ()=>{
						_Submit();			
					});
				}
				else {
					_Submit();	
				}
            }
        }

		void _Submit()
		{
			ulong itemID = GetSelectItem();
			SceneSetTaskItemReq kCmd = new SceneSetTaskItemReq();
			kCmd.taskId = (uint)(int)userData;
			kCmd.itemIds = new ulong[] { (ulong)itemID };
			NetManager.Instance().SendCommand<SceneSetTaskItemReq>(ServerType.GATE_SERVER, kCmd);

			frameMgr.CloseFrame(this);
		}

        [UIEventHandle("Cancle")]
        void OnCancle()
        {
            frameMgr.CloseFrame(this);
        }


        ulong GetSelectItem()
        {
            if (itemList.Count <= 0)
            {
                return 0;
            }

            if (itemSelectIndex < 0 || itemSelectIndex >= itemList.Count)
            {
                return 0;
            }

            return itemList[itemSelectIndex];
        }

        void SetSelectItem(int index)
        {
            if (index < 0 || index >= itemList.Count)
            {
                return;
            }

            for (int i = 0; i < itemSelectImage.Length; ++i)
            {               
                if (itemSelectImage[i] != null)
                {
                    itemSelectImage[i].enabled = false;
                }
            }
            if(itemSelectImage[index] != null)
            {
                itemSelectImage[index].enabled = true;
            }            

            itemSelectIndex = index;
        }



        protected Image[] itemImage = new Image[0];
        protected Image[] itemSelectImage = new Image[0];
        protected Text[] itemNum = new Text[0];

        [UIObject("ItemGroupBg/Text")]
        protected GameObject textShowNoItem;

        [UIControl("ItemGroupBg/Text")]
        protected Text textShow;

    }

}
