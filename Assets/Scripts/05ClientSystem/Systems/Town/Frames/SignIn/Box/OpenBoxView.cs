using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using DG.Tweening;
using Random = System.Random;

namespace GameClient
{

    public class OpenBoxView : MonoBehaviour
    {

        private BoxDataModel _boxDataModel;

        private GameObject _boxEffectGo;

        [Space(10)] [HeaderAttribute("Box")] [Space(10)] 
        [SerializeField] private GameObject boxRoot;

        [SerializeField] private GameObject boxEffectRoot;
        [SerializeField] private Slider boxOpenProgress;
        [SerializeField] private Text boxRandomItemName;


        [Space(10)] [HeaderAttribute("Effect")] [Space(10)]
        [SerializeField] private Toggle effectToggle;
        [SerializeField] private Animator effectAnimator;
        [SerializeField] private GameObject effectRoot;

        [Space(10)] [HeaderAttribute("Card")] [Space(10)]
        [SerializeField] private GameObject cardRoot;
        [SerializeField] private Animator cardAnimator;
        [SerializeField] private RectTransform cardRtf;
        [SerializeField] private ComCardEffect comCardEffect;

        [Space(10)]
        [HeaderAttribute("Item")]
        [Space(10)]
        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text ItemName;
        [SerializeField] private Image backImage;
        [SerializeField] private Image frontImage;

        [Space(10)] [HeaderAttribute("Button")] [Space(10)]
        [SerializeField] private Button okButton;

        //用于一定时间内刷新一次按钮
        [Space(20)]
        [HeaderAttribute("TimeRefresh")]
        [Space(10)]
        [SerializeField] private CommonTimeRefreshControl commonTimeRefreshControl;

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
                okButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (commonTimeRefreshControl != null)
                commonTimeRefreshControl.SetInvokeAction(OnUpdateOkButton);
        }

        private void UnBindEvents()
        {
            if(okButton != null)
                okButton.onClick.RemoveAllListeners();

            if (commonTimeRefreshControl != null)
                commonTimeRefreshControl.ResetInvokeAction();
        }

        private void ClearData()
        {
            if (_boxEffectGo != null)
                GameObject.Destroy(_boxEffectGo);

            _boxDataModel = null;
        }


        public void InitView(BoxDataModel boxDataModel)
        {
            _boxDataModel = boxDataModel;

            if (_boxDataModel == null)
                return;

            if (string.IsNullOrEmpty(_boxDataModel.BoxModelPath) == true)
                return;

            if (_boxDataModel.CommonNewItemDataModelList == null
                || _boxDataModel.CommonNewItemDataModelList.Count <= 0)
                return;

            if (_boxDataModel.AwardItemData == null)
                return;

            StartCoroutine(OpenBoxEffect());
        }

        private IEnumerator OpenBoxEffect()
        {
            yield return Yielders.EndOfFrame;

            _boxEffectGo = AssetLoader.GetInstance().LoadRes(_boxDataModel.BoxModelPath).obj as GameObject;
            if (_boxEffectGo != null)
            {
                _boxEffectGo.transform.SetParent(boxEffectRoot.transform, false);
                _boxEffectGo.SetActive(true);
            }

            #region OpenBox
            DOTweenAnimation animRandom = Utility.GetComponetInChild<DOTweenAnimation>(_boxEffectGo, 
                "p_Pot02/Bone007/Dummy001");

            Image imageRandomBg = Utility.GetComponetInChild<Image>(_boxEffectGo, "p_Pot02/Bone007/Dummy001/BG");
            if(imageRandomBg != null)
                imageRandomBg.gameObject.SetActive(false);
            Image imageRandomIcon = Utility.GetComponetInChild<Image>(_boxEffectGo, "p_Pot02/Bone007/Dummy001/Icon");
            if(imageRandomIcon != null)
                imageRandomIcon.gameObject.SetActive(false);

            animRandom.onStepComplete.RemoveAllListeners();
            animRandom.onStepComplete.AddListener(OnAnimatorStepComplete);

            animRandom.onComplete.RemoveAllListeners();
            animRandom.onComplete.AddListener(OnAnimatorComplete);
            
            yield return Yielders.GetWaitForSeconds(0.1f);
            AudioManager.instance.PlaySound(21);

            //boxOpenProgress.gameObject.CustomActive(true);
            //boxRandomItemName.gameObject.CustomActive(true);

            animRandom.tween.Restart();

            float maxTime = 2.2f;
            boxOpenProgress.value = 0.0f;
            float startTime = Time.time;
            float elapsed = 0.0f;
            while (elapsed < maxTime)
            {

                elapsed = Time.time - startTime;
                boxOpenProgress.value = elapsed / maxTime;

                yield return Yielders.EndOfFrame;
            }
            boxOpenProgress.value = 1.0f;


            boxOpenProgress.gameObject.SetActive(false);
            boxRandomItemName.gameObject.SetActive(false);
            animRandom.gameObject.SetActive(false);

            boxRoot.CustomActive(false);
            yield return Yielders.EndOfFrame;

            #endregion

            yield return PlayCardEffect();

            UpdateOkButton(true);
        }

        #region CardAnimator
        private IEnumerator PlayCardEffect()
        {
            SetAwardItemInfo();

            effectToggle.isOn = false;
            cardRoot.CustomActive(true);
            effectRoot.CustomActive(true);

            #region CardBigger
            effectAnimator.enabled = true;
            while (effectToggle.isOn == false)
            {
                yield return Yielders.EndOfFrame;
            }
            effectAnimator.enabled = false;
            #endregion

            #region CardOpen
            comCardEffect.bFinished = false;
            if (cardAnimator != null)
                cardAnimator.enabled = true;

            while (true)
            {
                if (comCardEffect.bFinished == false)
                {
                    yield return Yielders.GetWaitForSeconds(0.1f);
                }
                else
                {
                    if (cardAnimator != null)
                        cardAnimator.enabled = false;

                    break;
                }
            }
            #endregion
        }

        private void SetAwardItemInfo()
        {
            var rewardItemTable =
                TableManager.GetInstance().GetTableItem<ItemTable>(_boxDataModel.AwardItemData.ItemId);
            if (rewardItemTable != null)
            {
                //默认的卡牌路径
                string strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Huise";
                //如果奖励是大奖，根据道具的品质决定卡牌的颜色
                if (_boxDataModel.IsSpecialAward == true)
                {
                    switch (rewardItemTable.Color)
                    {
                        case ProtoTable.ItemTable.eColor.GREEN:
                        {
                            strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Lvse";
                            break;
                        }
                        case ProtoTable.ItemTable.eColor.PINK:
                        {
                            strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Fense";
                            break;
                        }
                        case ProtoTable.ItemTable.eColor.YELLOW:
                        {
                            strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Jinse";
                            break;
                        }
                        default:
                        {
                            strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Huise";
                            break;
                        }
                    }
                }
                //设置卡牌背景
                ETCImageLoader.LoadSprite(ref backImage, strCardBack);
            }

            // TODO 重置动画
            cardRtf.localRotation = Quaternion.identity;
            cardRtf.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            for (int i = 0; i < cardRtf.childCount; ++i)
            {
                Transform temp = cardRtf.GetChild(i);
                temp.localRotation = Quaternion.identity;
                temp.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            if (itemRoot != null)
            {
                var commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                if (commonNewItem == null)
                    commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);
                commonNewItem.InitItem(_boxDataModel.AwardItemData.ItemId,
                    _boxDataModel.AwardItemData.ItemCount);
            }

            if (ItemName != null)
                ItemName.text = _boxDataModel.AwardItemData.GetItemColorName();
        }
        #endregion

        #region RandomName
        private void OnAnimatorStepComplete()
        {
            var randomIndex = UnityEngine.Random.Range(0, _boxDataModel.CommonNewItemDataModelList.Count - 1);
            var itemDataModel = _boxDataModel.CommonNewItemDataModelList[randomIndex];
            if (itemDataModel != null)
                SetBoxRandomItemName(itemDataModel.GetItemColorName());
        }

        private void OnAnimatorComplete()
        {
            var itemDataModel = _boxDataModel.CommonNewItemDataModelList[0];
            if (itemDataModel != null)
                SetBoxRandomItemName(itemDataModel.GetItemColorName());
        }

        private void SetBoxRandomItemName(string colorName)
        {
            if (boxRandomItemName != null)
                boxRandomItemName.text = colorName;
        }
        #endregion

        private void OnCloseButtonClicked()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnBoxOpenFinished);

            ClientSystemManager.GetInstance().CloseFrame<OpenBoxFrame>();
        }

        private void UpdateOkButton(bool flag)
        {
            if (okButton != null)
                okButton.gameObject.CustomActive(flag);
        }

        //一定时间内，如果Ok按钮一直没有出来，自动刷新
        private void OnUpdateOkButton()
        {
            UpdateOkButton(true);
            if(commonTimeRefreshControl != null)
                commonTimeRefreshControl.ResetInvokeAction();
        }


    }
}
