using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ProtoTable;

namespace GameClient
{
    /// <summary>
    /// 战斗基础UI
    /// </summary>
    public class BattleUICommon : BattleUIBase
    {
        #region ExtraUIBind
        private GameObject mBlindMask = null;
        private ComboCount mCombo = null;
        private GameObject mArrowLeft = null;
        private GameObject mArrowRight = null;
        private GameObject mArrowLeftGo = null;
        private GameObject mArrowRightGo = null;
        private GameObject mObjFlashWhite = null;
        private DOTweenAnimation mFlashWhiteTween = null;

        protected override void _bindExUI()
        {
            mBlindMask = mBind.GetGameObject("BlindMask");
            mCombo = mBind.GetCom<ComboCount>("Combo");
            mArrowLeft = mBind.GetGameObject("ArrowLeft");
            mArrowRight = mBind.GetGameObject("ArrowRight");
            mArrowLeftGo = mBind.GetGameObject("ArrowLeftGo");
            mArrowRightGo = mBind.GetGameObject("ArrowRightGo");
            mObjFlashWhite = mBind.GetGameObject("ObjFlashWhite");
            mFlashWhiteTween = mBind.GetCom<DOTweenAnimation>("FlashWhiteTween");
        }

        protected override void _unbindExUI()
        {
            mBlindMask = null;
            mCombo = null;
            mArrowLeft = null;
            mArrowRight = null;
            mArrowLeftGo = null;
            mArrowRightGo = null;
            mObjFlashWhite = null;
            mFlashWhiteTween = null;
        }
        #endregion

        public ShowHitComponent comShowHit = new ShowHitComponent();
        public DebugBattleStatisCompnent comDebugBattleStatis = new DebugBattleStatisCompnent();

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUICommon";
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            comShowHit.RefreshGraphicSetting();

            if (mArrowLeft != null)
            {
                mArrowLeft.SetActive(false);
                mArrowRight.SetActive(false);
            }

            if (mBlindMask != null)
                SetBlindMask(false);
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (Global.Settings.showBattleInfoPanel)
                comDebugBattleStatis._loadBattleStatisticsUI();
        }

        protected override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void OnUpdate(float timeElapsed)
        {
            base.OnUpdate(timeElapsed);

            int deltaTime = (int)(timeElapsed * GlobalLogic.VALUE_1000);
            comShowHit.Update(deltaTime);
        }

        protected override void OnExit()
        {
            base.OnExit();
            
            mCombo = null;
            ClientSystemManager.GetInstance().Clear3DUIRoot();
            comShowHit.Clear();
            comShowHit = null;
        }

        public void FeedCombo(int count)
        {
            if (mCombo == null) return;
            mCombo.Feed(count);
        }

        public void ResetCombo()
        {
            if (mCombo == null) return;
            mCombo.StopFeed();
        }

        public void SwitchCombo(bool bActive)
        {
            mCombo.CustomActive(bActive);
        }

        public void SetBlindMask(bool visible)
        {
            if (mBlindMask == null) return;
            if (mBlindMask.activeSelf != visible)
            {
                mBlindMask.SetActive(visible);
            }
        }

        public void SetBlindMaskPosition(Vector2 pos)
        {
            if (mBlindMask != null)
            {
                RectTransform rt = mBlindMask.transform as RectTransform;
                rt.anchorMin = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
                rt.anchorMax = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
            }
        }

        public void ShowArrow(bool show = true, float angle = 0, bool right = true, bool withGo = false)
        {
            if (mArrowLeft != null)
            {
                if (withGo)
                {
                    _DisplayArraw(mArrowLeftGo, show);
                    _DisplayArraw(mArrowRightGo, show);
                    //mArrowLeftGo.SetActive(show);
                    //mArrowRightGo.SetActive(show);
                }
                else
                {
                    _DisplayArraw(mArrowLeft, show);
                    _DisplayArraw(mArrowRight, show);
                    //mArrowLeft.SetActive(show);
                    //mArrowRight.SetActive(show);
                }

                if (show)
                {
                    RectTransform rt = null;

                    if (right)
                    {
                        rt = mArrowRight.transform.transform as RectTransform;
                        if (withGo)
                        {
                            //mArrowLeftGo.SetActive(false);
                            _DisplayArraw(mArrowLeftGo, false);
                        }
                        else
                        {
                            //mArrowLeft.SetActive(false);
                            _DisplayArraw(mArrowLeft, false);
                        }
                    }
                    else
                    {
                        rt = mArrowLeft.transform.transform as RectTransform;
                        if (withGo)
                        {
                            //mArrowRightGo.SetActive(false);
                            _DisplayArraw(mArrowRightGo, false);
                        }
                        else
                        {
                            //mArrowRight.SetActive(false);
                            _DisplayArraw(mArrowRight, false);
                        }
                    }
                    float percent = Mathf.Clamp(angle + 0.17f, 0, 1f);
                    if (right)
                    {
                        rt.anchorMin = new Vector2(1f, percent);
                        rt.anchorMax = new Vector2(1f, percent);
                    }
                    else
                    {
                        rt.anchorMin = new Vector2(0f, percent);
                        rt.anchorMax = new Vector2(0f, percent);
                    }
                }
            }
        }

        void _DisplayArraw(GameObject go, bool isShow, bool useActive = false)
        {
            if (useActive)
                go.SetActive(isShow);
            else
            {
                if (!go.activeSelf)
                    go.SetActive(true);

                RectTransform rcTrans = go.transform as RectTransform;
                rcTrans.localScale = isShow ? new Vector3(0.8f, 0.8f, 0.8f) : new Vector3(0f, 0f, 0f);
            }
        }

        public void ShowFlashWhite()
        {
            if (mFlashWhiteTween != null)
            {
                mFlashWhiteTween.DORestart();
                mFlashWhiteTween.DOPlay();
            }
            mObjFlashWhite.CustomActive(true);
        }
    }
}