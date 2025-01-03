using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    [System.Serializable]
    public class ComDailyTodoFunctionExtraAnimParam
    {
        [Header("动画结束后的等待时间，用于刷新界面")]
        public float finishTagEndWaitingTime;
    }

    public class ComDailyTodoFunctionItem : MonoBehaviour
    {
        #region MODEL PARAMS

        private DailyTodoFunction tempModel;

        private bool isEndAnim = false;
        public bool IsEndAnim {
            get {
                return isEndAnim;
            }
        }

        private Sequence mFinishTagAnimQueue;

        #endregion

        #region VIEW PARAMS

        [SerializeField]
        private Image backgroundImg;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text recommendText;
        [SerializeField]
        private Text descText;
        [SerializeField]
        private Button gotoBtn;
        [SerializeField]
        private Button gotoBtn2;

        [SerializeField]
        private Image blackMask;

        [SerializeField]
        private Image finishTagImg;
        [SerializeField]
        private RectTransform finishTagRect;
        [SerializeField]
        private AnimationCurve finishTagAnimCurve;
        [SerializeField]
        private float finishTagAnimDuration;
        [SerializeField]
        private float finishTagAnimDelay;
        [SerializeField]
        private float finishTagScaleAnimFromValue;
        [SerializeField]
        private Ease finishTagFromFadeZeroEase;
        [SerializeField]
        private float finishTagFromFadeZeroDuration;
        [SerializeField]
        private float finishTagFromFadeZeroDelay;
        [SerializeField]
        [Header("动画额外参数")]
        private ComDailyTodoFunctionExtraAnimParam extraAnimParam;

        #endregion

        #region PRIVATE METHODS
        private void Awake()
        {
            if (gotoBtn)
            {
                gotoBtn.onClick.AddListener(_OnGotoBtnClick);
            }
            if (gotoBtn2)
            {
                gotoBtn2.onClick.AddListener(_OnGotoBtnClick);
            }
        }

        private void OnDestroy()
        {
            if (gotoBtn)
            {
                gotoBtn.onClick.RemoveListener(_OnGotoBtnClick);
            }
            if (gotoBtn2)
            {
                gotoBtn2.onClick.RemoveListener(_OnGotoBtnClick);
            }

            _ClearView();
        }

        private void _ClearView()
        {
            tempModel = null;

            if (mFinishTagAnimQueue != null)
            {
                mFinishTagAnimQueue.Pause();
                mFinishTagAnimQueue.Kill();
                mFinishTagAnimQueue = null;
                isEndAnim = false;
            }
        }

        private void _OnGotoBtnClick()
        {
            if (tempModel != null && tempModel.gotoHandler != null)
            {
                tempModel.gotoHandler(tempModel);
            }
        }

        private void _SetName(string name)
        {
            if (nameText)
            {
                nameText.text = name;
            }
        }

        private void _SetBackground(string imgPath)
        {
            if (!string.IsNullOrEmpty(imgPath) && backgroundImg)
            {
                //重置色调
                backgroundImg.color = Color.white;

                bool bEnable = ETCImageLoader.LoadSprite(ref backgroundImg, imgPath);
                if (!bEnable)
                {
                    Logger.LogErrorFormat("[ComDailyTodoActivityItem] - SetBackground can not load img : {0}", imgPath);
                }
            }
        }

        private void _SetChacterDesc(string desc)
        {
            if (descText)
            {
                descText.text = desc;
            }
        }

        private void _SetRecommendDesc(string desc)
        {
            if (recommendText)
            {
                recommendText.text = desc;
            }
        }

        private void _SetFunctionState(DailyTodoFuncState dailyTodoFuncState)
        {
            if (dailyTodoFuncState == DailyTodoFuncState.End)
            {
                _SetGoToFieldStatus(true, false, true);
            }
            else if (dailyTodoFuncState == DailyTodoFuncState.Start)
            {
                _SetGoToFieldStatus(false, true, false);
            }
            else
            {
                _SetGoToFieldStatus(false, false, false);
            }
        }

        private void _OnFinishTagPlayTweenStart()
        {
            isEndAnim = false;
            _SetGoToFieldStatus(true, false, false);
        }

        private void _OnFinishTagPlayTweenEnd()
        {
            isEndAnim = true;
            _SetGoToFieldStatus(true, false, true);

            if (this.tempModel != null)
            {
                this.tempModel.RecommendState = DailyTodoFuncState.End;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnDailyTodoFuncPlayAnimEnd, extraAnimParam);
        }


        private void _SetGoToFieldStatus(bool enableFinishTag, bool enableBtn, bool enableBlackMask)
        {
            if (finishTagImg)
            {
                finishTagImg.enabled = enableFinishTag;
            }
            if (gotoBtn)
            {
                gotoBtn.interactable = enableBtn;
            }
            if (blackMask)
            {
                blackMask.enabled = enableBlackMask;
            }
        }

        #endregion

        #region  PUBLIC METHODS

        public void RefreshView(DailyTodoFunction model)
        {
            if (null == model)
            {
                return;
            }
            this.tempModel = model;

            _SetBackground(model.backgroundImgPath);

            _SetName(model.name);

            _SetChacterDesc(model.characterDesc);

            _SetRecommendDesc(model.dayRecommendDesc);

            _SetFunctionState(model.RecommendState);
        }

        public void Recycle()
        {
            _ClearView();
        }

        public bool TryPlayAnim()
        {
            if (this.tempModel != null && this.tempModel.RecommendState == DailyTodoFuncState.Finishing)
            {
                if (finishTagRect && finishTagImg)
                {
                    if (mFinishTagAnimQueue == null)
                    {
                        mFinishTagAnimQueue = DOTween.Sequence();
                        mFinishTagAnimQueue.Append(finishTagRect.DOScale(finishTagScaleAnimFromValue, finishTagAnimDuration)
                            .SetDelay(finishTagAnimDelay)
                            .SetEase(finishTagAnimCurve)
                            .From()
                            .OnStart(_OnFinishTagPlayTweenStart)
                            .OnComplete(_OnFinishTagPlayTweenEnd));
                        mFinishTagAnimQueue.Join(finishTagImg.DOColor(new Color(1f, 1f, 1f, 0f), finishTagFromFadeZeroDuration)
                            .SetDelay(finishTagFromFadeZeroDelay)
                            .SetEase(finishTagFromFadeZeroEase)
                            .From());
                    }
                    if (mFinishTagAnimQueue.IsPlaying())
                    {
                        mFinishTagAnimQueue.Pause();
                    }
                    mFinishTagAnimQueue.Restart();
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
