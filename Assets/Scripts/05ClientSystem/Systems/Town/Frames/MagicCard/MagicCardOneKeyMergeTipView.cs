using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class MagicCardOneKeyMergeTipView : MonoBehaviour
    {

        private MagicCardMergeData _magicCardMergeData = null;
        private Action _magicCardMergeAction = null;


        [Space(15)]
        [HeaderAttribute("WhiteFlag")]
        [Space(5)]
        [SerializeField] private Image whiteFlag;
        [SerializeField] private Button whiteButton;

        [Space(15)]
        [HeaderAttribute("BlueFlag")]
        [Space(5)]
        [SerializeField] private Image blueFlag;
        [SerializeField] private Button blueButton;

        [Space(15)]
        [HeaderAttribute("GoldFlag")]
        [Space(5)]
        [SerializeField] private GeUISwitchButton goldButton;

        [Space(15)]
        [HeaderAttribute("BindItemFlag")]
        [Space(5)]
        [SerializeField] private GeUISwitchButton noBindItemButton;


        [Space(10)]
        [HeaderAttribute("Button")]
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button mergeButton;

        [SerializeField] private ComUIListScript enchantmentCardUIListScript;
        [SerializeField] private Text goldNumber;

        private List<ItemData> enchantmentCardItemDataList = new List<ItemData>();
        private void Awake()
        {
            InitEnchantmentCardUIListScript();
            BindEvents();
        }

        private void OnDestroy()
        {
            UnInitEnchantmentCardUIListScript();
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (whiteButton != null)
            {
                whiteButton.onClick.RemoveAllListeners();
                whiteButton.onClick.AddListener(OnWhiteButtonClick);
            }

            if (blueButton != null)
            {
                blueButton.onClick.RemoveAllListeners();
                blueButton.onClick.AddListener(OnBlueButtonClick);
            }

            if (goldButton != null)
            {
                goldButton.onValueChanged.RemoveAllListeners();
                goldButton.onValueChanged.AddListener(OnGoldButtonClick);
            }

            if (noBindItemButton != null)
            {
                noBindItemButton.onValueChanged.RemoveAllListeners();
                noBindItemButton.onValueChanged.AddListener(OnNoBindItemButtonClick);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCloseFrame);
            }

            if (mergeButton != null)
            {
                mergeButton.onClick.RemoveAllListeners();
                mergeButton.onClick.AddListener(OnMergeClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOneKeyMergeSuccess, OnOneKeyMergeSucceed);
        }

        private void UnBindEvents()
        {

            if (whiteButton != null)
                whiteButton.onClick.RemoveAllListeners();

            if (blueButton != null)
                blueButton.onClick.RemoveAllListeners();

            if (goldButton != null)
                goldButton.onValueChanged.RemoveAllListeners();

            if(noBindItemButton != null)
                noBindItemButton.onValueChanged.RemoveAllListeners();

            if(cancelButton != null)
                cancelButton.onClick.RemoveAllListeners();

            if(mergeButton != null)
                mergeButton.onClick.RemoveAllListeners();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOneKeyMergeSuccess, OnOneKeyMergeSucceed);
        }

        private void ClearData()
        {
            _magicCardMergeAction = null;
            _magicCardMergeData = null;

            if (enchantmentCardItemDataList != null)
                enchantmentCardItemDataList.Clear();
        }

        public void InitData(MagicCardMergeData magicCardMergeData)
        {

            _magicCardMergeData = magicCardMergeData;
            _magicCardMergeAction = magicCardMergeData.MagicCardMergeAction;

            MagicCardMergeDataManager.GetInstance().ResetOneKeyMergeSetting();

            InitView();
        }

        private void InitView()
        {
            UpdateWhiteFlag();
            UpdateBlueFlag();

            OnSetElementAmount();
        }

        #region UIListScript

        private void InitEnchantmentCardUIListScript()
        {
            if(enchantmentCardUIListScript != null)
            {
                enchantmentCardUIListScript.Initialize();
                enchantmentCardUIListScript.onBindItem += OnBindItemDelegate;
                enchantmentCardUIListScript.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitEnchantmentCardUIListScript()
        {
            if (enchantmentCardUIListScript != null)
            {
                enchantmentCardUIListScript.onBindItem -= OnBindItemDelegate;
                enchantmentCardUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private CommonNewItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonNewItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var commonNewItem = item.gameObjectBindScript as CommonNewItem;
            if(commonNewItem != null && item.m_index >= 0 && item.m_index < enchantmentCardItemDataList.Count)
            {
                ItemData itemData = enchantmentCardItemDataList[item.m_index];
                commonNewItem.InitItem(itemData.TableID, itemData.Count);
            }
        }

        private void OnSetElementAmount()
        {
            enchantmentCardItemDataList = MagicCardMergeDataManager.GetInstance().GetEnchantmentCardMergeItemDataList();

            if (enchantmentCardUIListScript != null)
                enchantmentCardUIListScript.SetElementAmount(enchantmentCardItemDataList.Count);

            UpdateGoldNumber();
        }
        #endregion

        private void UpdateWhiteFlag()
        {
            if(whiteFlag != null)
                whiteFlag.CustomActive(MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseWhiteCard);
        }

        private void UpdateBlueFlag()
        {
            if (blueFlag != null)
                blueFlag.CustomActive(MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseBlueCard);
        }
        
        private void OnWhiteButtonClick()
        {
            MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseWhiteCard
                = !MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseWhiteCard;
            UpdateWhiteFlag();

            OnSetElementAmount();
        }

        private void OnBlueButtonClick()
        {
            MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseBlueCard
                = !MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseBlueCard;
            UpdateBlueFlag();

            OnSetElementAmount();
        }

        private void OnGoldButtonClick(bool value)
        {
            MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUserGold = value;
        }

        private void OnNoBindItemButtonClick(bool value)
        {
            MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseNoBindItem = value;

            OnSetElementAmount();
        }

        private void UpdateGoldNumber()
        {
            int whiteMagicCardNumber = 0;
            int whiteMergeCost = 0;
            int blueMagicCardNumber = 0;
            int blueMergeCost = 0;

            MagicCardMergeUtility.GetMagicCardOneKeyMergeInfo(ref whiteMagicCardNumber,
                ref blueMagicCardNumber,
                ref whiteMergeCost,
                ref blueMergeCost);

            //得到最多合成次数
            var maxMergeTimes = MagicCardMergeUtility.GetMaxMergeTimes();

            //可以合成的次数
            var whiteMergeNumber = whiteMagicCardNumber / 2;
            var blueMergeNumber = blueMagicCardNumber / 2;

            //对合成次数进行处理，如果可以合成的次数超过最多合成次数
            if (whiteMergeNumber + blueMergeNumber > maxMergeTimes)
            {
                //只能合成白色
                if (whiteMergeNumber >= maxMergeTimes)
                {
                    whiteMergeNumber = maxMergeTimes;
                    blueMergeNumber = 0;
                }
                else
                {
                    blueMergeNumber = maxMergeTimes - whiteMergeNumber;
                }
            }

            //总共消耗的金币数量
            var totalMergeNeedMoneyNumber = (ulong)(whiteMergeNumber * whiteMergeCost + blueMergeNumber * blueMergeCost);

            if (goldNumber != null)
            {
                goldNumber.text = totalMergeNeedMoneyNumber.ToString();
            }
        }

        private void OnOneKeyMergeSucceed(UIEvent uiEvent)
        {
            OnSetElementAmount();
        }

        private void OnCloseFrame()
        {
            MagicCardMergeUtility.OnCloseMagicCardOneKeyMergeTipFrame();
        }

        private void OnMergeClick()
        {
            if (_magicCardMergeAction != null)
                _magicCardMergeAction();
        }
    }
}