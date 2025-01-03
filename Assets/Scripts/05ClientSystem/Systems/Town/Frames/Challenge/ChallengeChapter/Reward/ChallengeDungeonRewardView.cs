using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class ChallengeDungeonRewardView : MonoBehaviour
    {

        private ChallengeDungeonRewardDataModel _rewardDataModel;

        [Space(20)]
        [HeaderAttribute("Title")]
        [Space(10)]
        [SerializeField] private Text titleLabel;


        [Space(20)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private Button okButton;
        [SerializeField] private UIGray okButtonGray;

        [Space(20)]
        [HeaderAttribute("content")]
        [Space(10)]
        [SerializeField] private Text contentLabel;
        [SerializeField] private ComUIListScript rewardItemList;
        [SerializeField] private GameObject rewardItemListRoot;
        [SerializeField] private GameObject oneItemPos;             //一个Item的位置
        [SerializeField] private GameObject twoItemPos;          // 两个Item的位置
        [SerializeField] private GameObject moreItemPos;            //不小于三个Item的位置
        
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

            if (okButton != null)
            {
                okButton.onClick.RemoveAllListeners();
                okButton.onClick.AddListener(OnOkButtonClick);
            }

            if (rewardItemList != null)
            {
                rewardItemList.Initialize();
                rewardItemList.onItemVisiable += OnRewardItemVisible;
                rewardItemList.OnItemRecycle += OnRewardItemRecycle;
            }

        }

        private void UnBindEvents()
        {
            if(okButton != null)
                okButton.onClick.RemoveAllListeners();

            if (rewardItemList != null)
            {
                rewardItemList.onItemVisiable -= OnRewardItemVisible;
                rewardItemList.OnItemRecycle -= OnRewardItemRecycle;
            }
        }

        private void ClearData()
        {
            _rewardDataModel = null;
        }

        public void InitRewardView(ChallengeDungeonRewardDataModel rewardDataModel)
        {
            _rewardDataModel = rewardDataModel;

            if (_rewardDataModel == null)
            {
                Logger.LogErrorFormat("RewardDataModel is null");
                return;
            }

            InitContent();

        }

        private void InitContent()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("challenge_pass_reward_title");

            if (contentLabel != null)
                contentLabel.text = string.Format(TR.Value("challenge_pass_number_to_get_reward"),
                    _rewardDataModel.TotalNumber);

            var isCanReceiveReward = _rewardDataModel.ChallengeNumber >= _rewardDataModel.TotalNumber;

            if (okButton != null)
                okButton.enabled = isCanReceiveReward;

            if (okButtonGray != null)
                okButtonGray.enabled = !isCanReceiveReward;

            InitRewardItemList();
        }

        private void InitRewardItemList()
        {
            if (rewardItemList == null)
                return;

            //设置List的数量
            var rewardItemCount = 0;
            if (_rewardDataModel.AwardItemDataList != null)
                rewardItemCount = _rewardDataModel.AwardItemDataList.Count;
            rewardItemList.SetElementAmount(rewardItemCount);

            if (rewardItemListRoot != null)
            {
                var rewardItemListRootPos = rewardItemListRoot.gameObject.transform.localPosition;
                if (rewardItemCount <= 1)
                {
                    if (oneItemPos != null)
                        rewardItemListRoot.gameObject.transform.localPosition = new Vector3(
                            oneItemPos.transform.localPosition.x,
                            rewardItemListRootPos.x, rewardItemListRootPos.y);
                }
                else if (rewardItemCount == 2)
                {
                    if (twoItemPos != null)
                        rewardItemListRoot.gameObject.transform.localPosition = new Vector3(
                            twoItemPos.transform.localPosition.x,
                            rewardItemListRootPos.x, rewardItemListRootPos.y);
                }
                else
                {
                    if (moreItemPos != null)
                        rewardItemListRoot.gameObject.transform.localPosition = new Vector3(
                            moreItemPos.transform.localPosition.x,
                            rewardItemListRootPos.x, rewardItemListRootPos.y);
                }

            }
        }

        private void OnOkButtonClick()
        {
            if (_rewardDataModel == null)
                return;

            Logger.LogErrorFormat("OnOkButtonClick and cur number is {0}, totalNumber is {1}",
                _rewardDataModel.ChallengeNumber, _rewardDataModel.TotalNumber);
        }

        private void OnRewardItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_rewardDataModel == null)
                return;

            if (_rewardDataModel.AwardItemDataList == null || _rewardDataModel.AwardItemDataList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _rewardDataModel.AwardItemDataList.Count)
                return;

            var awardItemData = _rewardDataModel.AwardItemDataList[item.m_index];
            var dungeonRewardItem = item.GetComponent<ChallengeDungeonRewardItem>();

            if (awardItemData != null && dungeonRewardItem != null)
                dungeonRewardItem.InitItem(awardItemData);


        }

        private void OnRewardItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var dungeonRewardItem = item.GetComponent<ChallengeDungeonRewardItem>();
            if(dungeonRewardItem != null)
                dungeonRewardItem.OnItemRecycle();
        }




    }
}
