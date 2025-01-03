using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class LegendMissionItemNew : MonoBehaviour
    {
        private MissionManager.SingleMissionInfo _singleMissionInfo;
        private UInt32 _taskId = 0;

        private List<ComLegendMaterialItem> materials = new List<ComLegendMaterialItem>();
        private ComItem _comItem;

        [SerializeField] private GameObject goItemParent;        //奖励
        [SerializeField] private GameObject goMaterialPrefab;
        [SerializeField] private GameObject goMaterialParent;

        [SerializeField] private Text titleText;

        [SerializeField] private Button exchangeButton;     //任务完成，可以兑换

        [SerializeField] private GameObject unfinish;       //任务没有完成不可领取

        [SerializeField] private GameObject taskOver;       //任务结束，兑换完成
        
        private void Start()
        {
            if (exchangeButton != null)
            {
                exchangeButton.onClick.RemoveAllListeners();
                exchangeButton.onClick.AddListener(OnExChangeButtonClick);
            }
        }

        private void OnDestroy()
        {
            if (exchangeButton != null)
            {
                exchangeButton.onClick.RemoveAllListeners();
            }

            if (null != _comItem)
            {
                ComItemManager.Destroy(_comItem);
                _comItem = null;
            }
        }

        public void Init(MissionManager.SingleMissionInfo singleMissionInfo)
        {
            _singleMissionInfo = singleMissionInfo;
            _taskId = singleMissionInfo.taskID;
        }

        //需要更新一下数据
        private void OnEnable()
        {
            BindEvents();
            
            OnUpdateData();
        }

        private void OnDisable()
        {
            UnBindEvents();
        }

        private void BindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MissionSync, OnMissionSync);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MissionUpdated, OnMissionUpdate);
        }

        private void UnBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MissionSync, OnMissionSync);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MissionUpdated, OnMissionUpdate);
        }

        //消息更新
        private void OnMissionUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }

            var taskId = (uint) uiEvent.Param1;
            if (taskId != _taskId)
            {
                return;
            }

            OnUpdateData();
        }

        //消息同步
        private void OnMissionSync(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }

            var taskId = (uint)uiEvent.Param1;
            if (taskId != _taskId)
            {
                return;
            }

            OnUpdateData();
        }


        //OnEnable 和 获得同步数据的时候，对数据进行更新
        //根据_taskId从任务管理器中获得相应的任务数据
        private void OnUpdateData()
        {
            _singleMissionInfo = MissionManager.GetInstance().GetMission(_taskId);
            if(_singleMissionInfo == null)
            {
                return;
            }
            //更新标题
            UpdateMissionTitle();
            //更新材料内容
            UpdateMissionItemContent();
            //更新按钮
            UpdateMissionStatus();
        }

        //更新标题
        private void UpdateMissionTitle()
        {
            if(_singleMissionInfo.missionItem == null)
                return;

            var maxAcceptCount = _singleMissionInfo.missionItem.MaxSubmitCount;
            var cursubmitCount = _singleMissionInfo.submitCount;
            titleText.text = string.Format(_singleMissionInfo.missionItem.TaskInitText,
                cursubmitCount, maxAcceptCount);
        }

        //更新任务状态
        private void UpdateMissionStatus()
        {
            if (_singleMissionInfo.status == (int)TaskStatus.TASK_FINISHED)
            {
                unfinish.CustomActive(false);
                taskOver.CustomActive(false);
                exchangeButton.CustomActive(true);
            }
            else if (_singleMissionInfo.status == (int)TaskStatus.TASK_OVER)
            {
                unfinish.CustomActive(false);
                taskOver.CustomActive(true);
                exchangeButton.CustomActive(false);
            }
            else
            {
                //未完成
                unfinish.CustomActive(true);
                taskOver.CustomActive(false);
                exchangeButton.CustomActive(false);
            }
        }

        //更新任务内容
        private void UpdateMissionItemContent()
        {
            if (_singleMissionInfo.missionItem == null)
            {
                return;
            }

            SetMissionAwardItem();

            var materialDatas = new List<ComLegendMaterialItemData>();
            var missionMaterialList = MissionManager.GetInstance().GetMissionMaterials(_singleMissionInfo.missionItem.ID);
            if (null != missionMaterialList)
            {
                for (var i = 0; i < missionMaterialList.Count; ++i)
                {
                    var parseObject = MissionManager.Parse(_singleMissionInfo.missionItem.MissionMaterialsKeyValue[i], (MissionManager.MaterialMatchInfo matchInfo) =>
                    {
                        return MissionManager._TokenMaterials(_singleMissionInfo.missionItem.ID, missionMaterialList[i], matchInfo);
                    });

                    materialDatas.Add(new ComLegendMaterialItemData
                    {
                        itemData = missionMaterialList[i],
                        parseObject = parseObject,
                    });
                }
            }
            SetMissionMaterialItems(materialDatas);
        }

        private void SetMissionAwardItem()
        {
            var awards = MissionManager.GetInstance().GetMissionAwards(_singleMissionInfo.missionItem.ID);
            if (null != awards && awards.Count > 0)
            {
                var itemData = ItemDataManager.CreateItemDataFromTable(awards[0].ID);
                if (null != itemData)
                {
                    itemData.Count = awards[0].Num;
                }

                if(goItemParent == null)
                    return;

                if (null == _comItem)
                {
                    _comItem = ComItemManager.Create(goItemParent);
                }

                if (null != _comItem)
                {
                    _comItem.Setup(itemData, (GameObject obj, ItemData item) => { ItemTipManager.GetInstance().ShowTip(itemData); });
                }

                //if (null != itemData)
                //{
                //    itemName.text = itemData.GetColorName();
                //}
            }
        }

        public void SetMissionMaterialItems(List<ComLegendMaterialItemData> datas)
        {
            if (null != goMaterialPrefab && null != goMaterialParent)
            {
                goMaterialPrefab.CustomActive(false);

                for (var i = materials.Count; i < datas.Count; ++i)
                {
                    var goCurrent = GameObject.Instantiate(goMaterialPrefab);
                    Utility.AttachTo(goCurrent, goMaterialParent);
                    goCurrent.CustomActive(true);
                    materials.Add(goCurrent.GetComponent<ComLegendMaterialItem>());
                }

                for (var i = 0; i < materials.Count; ++i)
                {
                    materials[i].gameObject.CustomActive(i < datas.Count);
                    if (i < datas.Count)
                    {
                        materials[i].SetItemData(datas[i]);
                    }
                }
            }
        }

        private void OnExChangeButtonClick()
        {
            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            //发送兑换请求
            MissionManager.GetInstance().sendCmdSubmitTask(_taskId, TaskSubmitType.TASK_SUBMIT_AUTO, 0);
        }
    }
}