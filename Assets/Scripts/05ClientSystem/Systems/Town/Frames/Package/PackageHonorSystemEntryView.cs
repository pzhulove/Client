using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
    
    //更新红点红点和名字
    public class PackageHonorSystemEntryView : MonoBehaviour
    {

        private int _honorSystemLevel = -1;

        [Space(5)]
        [HeaderAttribute("EntryController")]
        [Space(5)]
        [SerializeField] private ComButtonWithCd entryButtonWithCd;
        [SerializeField] private GameObject redPointFlag;

        [Space(5)]
        [HeaderAttribute("HonorSystemLevel")]
        [Space(5)]
        [SerializeField] private Image leftImage;
        [SerializeField] private Image rightImage;
        [SerializeField] private Text honorSystemEntryNameLabel;

        private void Awake()
        {
            if (entryButtonWithCd != null)
            {
                entryButtonWithCd.ResetButtonListener();
                entryButtonWithCd.SetButtonListener(OnEntryButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (entryButtonWithCd != null)
                entryButtonWithCd.ResetButtonListener();
            _honorSystemLevel = -1;
        }

        private void OnEnable()
        {
            UpdateEntryController();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveHonorSystemRedPointUpdateMessage,
                OnReceiveHonorSystemRedPointUpdateMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveHonorSystemRedPointUpdateMessage,
                OnReceiveHonorSystemRedPointUpdateMessage);
        }

        private void OnReceiveHonorSystemRedPointUpdateMessage(UIEvent uiEvent)
        {
            UpdateRedPointFlag();
        }

        private void OnEntryButtonClicked()
        {
            //打开界面
            HonorSystemUtility.OnOpenHonorSystemFrame();

            //如果当前显示小红点，发送小红点更新的消息
            if (HonorSystemDataManager.GetInstance().IsShowRedPointFlag == true)
            {
                HonorSystemDataManager.GetInstance().IsShowRedPointFlag = false;

                //发送红点更新的消息
                HonorSystemUtility.SendHonorSystemRedPointUpdateMessage();
            }
        }

        public void UpdateEntryController()
        {
            if (entryButtonWithCd != null)
                entryButtonWithCd.Reset();

            UpdateRedPointFlag();

            UpdateHonorSystemLevelInfo();
        }

        private void UpdateRedPointFlag()
        {
            var isShowRedPoint = HonorSystemUtility.IsShowHonorSystemRedPoint();
            CommonUtility.UpdateGameObjectVisible(redPointFlag, isShowRedPoint);
        }

        //更新荣誉等级的图片和名字
        private void UpdateHonorSystemLevelInfo()
        {
            //如果等级相同，直接返回
            if (_honorSystemLevel == (int)HonorSystemDataManager.GetInstance().PlayerHonorLevel)
                return;

            _honorSystemLevel = (int)HonorSystemDataManager.GetInstance().PlayerHonorLevel;

            var currentLevel = _honorSystemLevel;
            if (currentLevel == 0)
                currentLevel = HonorSystemDataManager.DefaultHonorSystemLevel;

            var levelTable = TableManager.GetInstance().GetTableItem<HonorLevelTable>(currentLevel);
            if (levelTable == null)
                return;

            if (leftImage != null)
                ETCImageLoader.LoadSprite(ref leftImage, levelTable.TitleFlag);
            if (rightImage != null)
                ETCImageLoader.LoadSprite(ref rightImage, levelTable.TitleFlag);

            if (honorSystemEntryNameLabel != null)
            {
                var honorLevelTitleNameStr = HonorSystemUtility.GetTitleNameByTitleId(levelTable.Title);
                honorSystemEntryNameLabel.text = honorLevelTitleNameStr;
            }
        }
    }
}