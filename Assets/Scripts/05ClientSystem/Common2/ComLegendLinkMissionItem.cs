using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using System;

namespace GameClient
{
    class ComLegendLinkMissionItem : MonoBehaviour
    {
        public Text title;
        public Text itemName;
        public GameObject goItemParent;
        public GameObject goMaterialPrefab;
        public GameObject goMaterialParent;
        public Text unLockHint;
        public UIGray comGray;
        public StateController comStateController;
        public Text missionDesc;
        //public Text receivedLabel;
        public GameObject ownedFlag;
        ComItem comItem;
        MissionManager.SingleMissionInfo missionValue = null;
        private int legendId = -1;
        List<ComLegendMaterialItem> materials = new List<ComLegendMaterialItem>();

        string key_locked = "locked";
        string key_finished = "finished";
        string key_acquired = "acquired";
        string key_normal = "normal";
        string key_unfinished = "un_finished";

        #region CDTime
        private bool isInCD = false;
        private const float CDTime = 2.0f;
        private float cdFinishedTime = 0.0f;
        private float cdLeftTime = 0.0f;
        public Text cdLeftTimeLabel = null;
        public GameObject cdLeftTimeGameObject = null;
        public Button receivedButton = null;

        private ItemData mItemData = null;
        #endregion

        public void SetItems(List<ComLegendMaterialItemData> datas)
        {
            if(null != goMaterialPrefab)
            {
                goMaterialPrefab.CustomActive(false);

                for(int i = materials.Count; i < datas.Count; ++i)
                {
                    GameObject goCurrent = GameObject.Instantiate(goMaterialPrefab);
                    Utility.AttachTo(goCurrent, goMaterialParent);
                    goCurrent.CustomActive(true);
                    materials.Add(goCurrent.GetComponent<ComLegendMaterialItem>());
                }

                for(int i = 0;i < materials.Count; ++i)
                {
                    materials[i].gameObject.CustomActive(i < datas.Count);
                    if(i < datas.Count)
                    {
                        materials[i].SetItemData(datas[i]);
                    }
                }
            }
        }

        bool _CheckHasFinishAllPreMissions(ProtoTable.MissionTable missionItem)
        {
            if(null != missionItem)
            {
                for(int i = 0; i < missionItem.PreIDs.Count; ++i)
                {
                    var missionTableItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(missionItem.PreIDs[i]);
                    if(null == missionTableItem || missionTableItem.MissionOnOff == 0)
                    {
                        continue;
                    }

                    if(missionItem == missionTableItem)
                    {
                        continue;
                    }

                    var missionValue = MissionManager.GetInstance().GetMission((uint)missionItem.PreIDs[i]);
                    if(null == missionValue ||
                        missionValue.status != (int)Protocol.TaskStatus.TASK_OVER)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        void _SetMainItemData()
        {
            if(null != missionDesc && null != missionValue && null != missionValue.missionItem)
            {
                missionDesc.text = missionValue.missionItem.TaskInitText;
            }

            var awards = MissionManager.GetInstance().GetMissionAwards(missionValue.missionItem.ID);
            if (null != awards && awards.Count > 0)
            {
                if(missionValue.missionItem.MinPlayerLv > PlayerBaseData.GetInstance().Level)
                {
                    unLockHint.text = TR.Value("legend_need_lv", missionValue.missionItem.MinPlayerLv);
                }
                else //if (!_CheckHasFinishAllPreMissions(missionValue.missionItem))
                {
                    unLockHint.text = missionValue.missionItem.PreIDsConditionDesc;
                }

                var itemData = ItemDataManager.CreateItemDataFromTable(awards[0].ID);
                if (null != itemData)
                {
                    itemData.Count = awards[0].Num;
                }

                if (null == comItem)
                {
                    comItem = ComItemManager.Create(goItemParent);
                }

                if (null != comItem)
                {
                    comItem.Setup(itemData, (GameObject obj, ItemData item) => { ItemTipManager.GetInstance().ShowTip(itemData); });
                }

                if (null != itemData)
                {
                    title.text = itemData.GetColorName();
                    mItemData = itemData;
                }
               
            }
        }

        void _SetStaus()
        {
            switch((Protocol.TaskStatus)missionValue.status)
            {
                case Protocol.TaskStatus.TASK_INIT:
                    comStateController.Key = key_locked;
                    comGray.enabled = true;
                    break;
                case Protocol.TaskStatus.TASK_UNFINISH:
                    comStateController.Key = key_unfinished;
                    comGray.enabled = true;
                    break;
                case Protocol.TaskStatus.TASK_FINISHED:
                    comStateController.Key = key_finished;
                    comGray.enabled = false;
                    break;
                case Protocol.TaskStatus.TASK_OVER:
                    comStateController.Key = key_acquired;
                    comGray.enabled = false;
                    break;
                default:
                    comStateController.Key = key_normal;
                    comGray.enabled = false;
                    break;
            }
        }

        public void SetMissionData(MissionManager.SingleMissionInfo missionValue, int curLegendId = -1)
        {
            this.missionValue = missionValue;
            this.legendId = curLegendId;
            if (null != missionValue)
            {
                //if (null != this.title)
                //{
                //    this.title.text = missionValue.missionItem.TaskName;

                //}

                _SetMainItemData();

                List<ComLegendMaterialItemData> materialDatas = new List<ComLegendMaterialItemData>();
                var materials = MissionManager.GetInstance().GetMissionMaterials(missionValue.missionItem.ID);
                if(null != materials)
                {
                    for (int i = 0; i < materials.Count; ++i)
                    {
                        MissionManager.ParseObject parseObject = MissionManager.Parse(missionValue.missionItem.MissionMaterialsKeyValue[i], (MissionManager.MaterialMatchInfo matchInfo) =>
                         {
                             return MissionManager._TokenMaterials(missionValue.missionItem.ID,materials[i],matchInfo);
                         });

                        materialDatas.Add(new ComLegendMaterialItemData
                        {
                            itemData = materials[i],
                            parseObject = parseObject,
                        });
                    }
                }
                SetItems(materialDatas);

                _SetStaus();
                UpdateOwnedFlag();
            }
        }

        private void UpdateOwnedFlag()
        {
            if (ownedFlag == null)
            {
                return;
            }

            if (missionValue.submitCount >= 1)
            {
                ownedFlag.CustomActive(true);
            }
            else
            {
                ownedFlag.CustomActive(false);
            }
        }

        public void OnSubmitMission()
        {
            if (mItemData == null)
            {
                Logger.LogError("mItemData is null");
                return;
            }
            var tipContent = string.Format(TR.Value("LengendceRoad_Equip_Exchange"), mItemData.GetColorName());
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("LengendceRoad_Equip_Exchange_Cancel"),
                RightButtonText = TR.Value("LengendceRoad_Equip_Exchange_OK"),
                OnRightButtonClickCallBack = OnSureClick,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);


        }

        private void OnSureClick()
        {
         
            if (missionValue == null)
                return;

            if (missionValue.status != (int)TaskStatus.TASK_FINISHED)
                return;

            if (missionValue.submitCount <= 0)
            {
                OnSendSubmitMissionRequest();
            }
            else
            {
                SystemNotifyManager.SystemNotify(1293, OnSendSubmitMissionRequest);
            }
        }

        private void OnSendSubmitMissionRequest()
        {
            SetCdContent();
            MissionManager.GetInstance().sendCmdSubmitTask(missionValue.taskID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
        }

        private void SetCdContent()
        {
            isInCD = true;
            cdFinishedTime = Time.realtimeSinceStartup + CDTime;
            cdLeftTimeGameObject.CustomActive(true);
        }

        private void Update()
        {
            if (isInCD == true)
            {
                cdLeftTime = cdFinishedTime - Time.realtimeSinceStartup;
                if (cdLeftTime > 0.0f)
                {
                    cdLeftTimeLabel.text = string.Format("{0}", (int)cdLeftTime + 1);
                    cdLeftTimeGameObject.CustomActive(true);
                    receivedButton.CustomActive(false);
                }
                else
                {
                    isInCD = false;
                    cdLeftTime = 0.0f;
                    cdLeftTimeGameObject.CustomActive(false);
                    _SetStaus();
                }
            }
        }        
    }
}