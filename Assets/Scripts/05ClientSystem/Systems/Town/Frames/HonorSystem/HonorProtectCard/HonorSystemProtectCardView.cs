using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemProtectCardView : MonoBehaviour
    {

        private float _intervalTime = 0.0f;
        private uint _finishTimeStamp = 0;

        private ItemData _normalProtectCardItemData;
        private ItemData _highProtectCardItemData;

        [Space(10)]
        [HeaderAttribute("Title")]
        [Space(10)]
        [SerializeField] private Text timeTitleLabel;
        [SerializeField] private Text timeCounterDownLabel;
        [SerializeField] private Text introductionLabel;

        [Space(10)] [HeaderAttribute("ProtectCardItem")] [Space(10)]
        [SerializeField] private HonorSystemProtectCardItem normalProtectCardItem;

        [SerializeField] private HonorSystemProtectCardItem highProtectCardItem;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
        }

        private void UnBindEvents()
        {
        }

        private void ClearData()
        {
            _intervalTime = 0.0f;
            _finishTimeStamp = 0;
            _normalProtectCardItemData = null;
            _highProtectCardItemData = null;
        }

        public void InitView()
        {
            InitProtectCardData();

            InitBaseView();
            UpdateTimeCountDownLabel();

            InitProtectCardItem();
        }

        private void InitProtectCardData()
        {
            _intervalTime = 0.0f;
            _finishTimeStamp = HonorSystemDataManager.GetInstance().FinishTimeStamp;

            GetProtectCardItemData();
        }

        private void  GetProtectCardItemData()
        {
            var items = ItemDataManager.GetInstance().GetItemsByPackageType(
                HonorSystemDataManager.ProtectCardItemPackageType);

            if (items == null)
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                var itemGuid = items[i];
                var itemData = ItemDataManager.GetInstance().GetItem(itemGuid);
                if(itemData == null)
                    continue;

                if (itemData.TableID == HonorSystemDataManager.NormalHonorProtectCardId)
                {
                    _normalProtectCardItemData = itemData;
                }
                else if (itemData.TableID == HonorSystemDataManager.HighHonorProtectCardId)
                {
                    _highProtectCardItemData = itemData;
                }
            }
        }

        private void InitBaseView()
        {
            if (timeTitleLabel != null)
            {
                timeTitleLabel.text = TR.Value("Honor_System_Count_Down_Time_Title_Label");
            }

            if (introductionLabel != null)
            {
                introductionLabel.text = TR.Value("Honor_System_Protect_Card_Introduction_Label");
            }
        }

        private void InitProtectCardItem()
        {
            if (normalProtectCardItem != null)
            {
                normalProtectCardItem.InitItem(HonorSystemDataManager.NormalHonorProtectCardId,
                    _normalProtectCardItemData);
            }

            if (highProtectCardItem != null)
            {
                highProtectCardItem.InitItem(HonorSystemDataManager.HighHonorProtectCardId,
                    _highProtectCardItemData);
            }
        }

        

        private void OnCloseFrame()
        {
            HonorSystemUtility.OnCloseHonorSystemProtectCardFrame();
        }

        #region Update
        private void Update()
        {
            if (_finishTimeStamp < TimeManager.GetInstance().GetServerTime())
                return;

            _intervalTime += Time.deltaTime;
            if (_intervalTime >= 1.0f)
            {
                _intervalTime = 0.0f;
                UpdateTimeCountDownLabel();
            }
        }

        private void UpdateTimeCountDownLabel()
        {
            if (timeCounterDownLabel == null)
                return;

            var endTime = _finishTimeStamp;
            var currentTime = TimeManager.GetInstance().GetServerTime();

            var timeCountDownStr = CountDownTimeUtility.GetCountDownTimeByDayHourMinuteSecondFormat(
                endTime,
                currentTime);

            timeCounterDownLabel.text = timeCountDownStr;
        }
        #endregion

    }
}