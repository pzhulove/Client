using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;


namespace GameClient
{

    public class MagicCardOneKeyMergeResultView : MonoBehaviour
    {

        private SceneMagicCardCompOneKeyRes _oneKeyRes;
        private List<ItemReward> _itemRewardDataList = new List<ItemReward>();

        [Space(10)]
        [HeaderAttribute("Content")]
        [SerializeField] private Text title;
        [SerializeField] private ComUIListScriptEx magicCardItemList;

        [Space(10)]
        [HeaderAttribute("Text")]
        [SerializeField] private Text resultText;
        [SerializeField] private Button okButton;
        [SerializeField] private Button closeButton;
        
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
                okButton.onClick.AddListener(OkButtonClick);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OkButtonClick);
            }

            if (magicCardItemList != null)
            {
                magicCardItemList.Initialize();
                magicCardItemList.onItemVisiable += OnMagicCardItemVisible;
            }
        }

        private void UnBindEvents()
        {
            if(okButton != null)
                okButton.onClick.RemoveAllListeners();

            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (magicCardItemList != null)
            {
                magicCardItemList.onItemVisiable -= OnMagicCardItemVisible;
            }
        }

        private void ClearData()
        {
            _oneKeyRes = null;
            _itemRewardDataList.Clear();
        }

        public void InitData()
        {
            _oneKeyRes = MagicCardMergeDataManager.GetInstance().OneKeyMergeRes;
            if (_oneKeyRes == null)
            {
                Logger.LogErrorFormat("MagicCardOneKeyMergeResultView OneKeyMergeRes is null");
                return;
            }

            InitOneKeyMergeResultData();

            InitView();
        }

        private void InitOneKeyMergeResultData()
        {
            for (var i = 0; i < _oneKeyRes.items.Length; i++)
            {
                if (_oneKeyRes.items[i] != null)
                    _itemRewardDataList.Add(_oneKeyRes.items[i]);
            }

            //按照已经的规则进行排序
            MagicCardMergeUtility.SortMagicCardMergeResultData(_itemRewardDataList);
        }
        
        private void InitView()
        {
            InitCommonView();
            InitRewardItemList();
            InitResultText();
        }

        private void InitCommonView()
        {
            if (title != null)
            {
                title.text = TR.Value("magic_card_merge_result_title");
            }
        }

        private void InitRewardItemList()
        {
            var itemListCount = _itemRewardDataList.Count;
            if (magicCardItemList != null)
            {
                magicCardItemList.SetElementAmount(itemListCount);
            }
        }

        private void InitResultText()
        {
            if (resultText == null)
                return;

            var oneKeyMergeEndReason = (MagicCardCompOneKeyEndReason) _oneKeyRes.endReason;

            int mergeTime = (int) _oneKeyRes.compTimes;
            int costBindGolds = (int) _oneKeyRes.consumeBindGolds;
            int costGolds = (int) _oneKeyRes.comsumeGolds;

            //背包已经满了
            if (oneKeyMergeEndReason == MagicCardCompOneKeyEndReason.MCCER_ITEMPACK_FULL)
            {
                var resultStr = TR.Value("magic_card_merge_result_content_package_full_without_gold",
                    mergeTime, costBindGolds);
                if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUserGold == true)
                    resultStr = TR.Value("magic_card_merge_result_content_package_full_with_gold",
                        mergeTime,
                        costBindGolds,
                        costGolds);
                resultText.text = resultStr;
            }
            else
            {
                //其他情况
                var resultStr = TR.Value("magic_card_merge_result_content_without_gold",
                    mergeTime, costBindGolds);
                if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUserGold == true)
                    resultStr = TR.Value("magic_card_merge_result_content_with_gold",
                        mergeTime,
                        costBindGolds,
                        costGolds);
                resultText.text = resultStr;
            }
        }

        private void OnMagicCardItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (magicCardItemList == null)
                return;

            if (_itemRewardDataList == null)
                return;

            if (_itemRewardDataList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _itemRewardDataList.Count)
                return;

            var itemRewardData = _itemRewardDataList[item.m_index];
            var magicCardMergeItem = item.GetComponent<MagicCardMergeItem>();
            if(itemRewardData != null && magicCardMergeItem != null)
                magicCardMergeItem.InitItem(itemRewardData);
        }


        private void OkButtonClick()
        {
            OnCloseFrame();
        }

        private void OnCloseFrame()
        {
            MagicCardMergeDataManager.GetInstance().ResetOneKeyMergeRes();

            MagicCardMergeUtility.OnCloseMagicCardOneKeyMergeResultFrame();
        }

    }
}