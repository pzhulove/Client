using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 金币关卡
    /// </summary>
    public class BattleUIGoldRush : BattleUIBase
    {
        #region ExtraUIBind

        private UINumber mTimeLeft = null;
        private TextEx mChestGold = null;
        private GameObject mGoldTrailRoot = null;
        private RectTransform mGoldBag = null;
        private DOTweenAnimation mChestGoldDT = null;

        protected override void _bindExUI()
        {
            mTimeLeft = mBind.GetCom<UINumber>("timeLeft");
            mChestGold = mBind.GetCom<TextEx>("chestGold");
            mGoldTrailRoot = mBind.GetGameObject("goldTrailRoot");
            mGoldBag = mBind.GetCom<RectTransform>("goldBag");
            mChestGoldDT = mBind.GetCom<DOTweenAnimation>("chestGoldDT");
        }

        protected override void _unbindExUI()
        {
            mTimeLeft = null;
            mChestGold = null;
            mGoldTrailRoot = null;
            mGoldBag = null;
            mChestGoldDT = null;
        }

        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIGoldRush";
        }

        private GoldRushBattle mBattle;
        private uint mEndTime;
        private int mLastGold;
        private int mCurGold;
        private int mLastCountTime;
        private GameObject mWaveTipGo;
        private Text mWaveTip;
        private float mLastUpdateGoldTime;
        private bool mGoldUpdateFlag;
        private int mGoldUpdateTimes;

        private GameObject mGoldEff2;
        private readonly string goldEffPath1 = "_NEW_RESOURCES/Effects/Monster/Huodongben/Baoxiang/Prefab/Skill_HD_Baoxiang_tuowei";
        private readonly string goldEffPath2 = "_NEW_RESOURCES/Effects/Monster/Huodongben/Baoxiang/Prefab/Skill_HD_Baoxiang_jiemian";
        private readonly string goldEffPath3 = "_NEW_RESOURCES/Effects/Monster/Huodongben/Baoxiang/Prefab/Skill_UI_HD_Baoxiang_wenzi";

        protected override void OnEnter()
        {
            base.OnEnter();

            mBattle = BattleMain.instance.GetBattle() as GoldRushBattle;
            mCurGold = mLastGold = 0;
            mLastCountTime = -1;
            mLastUpdateGoldTime = 0;
            mGoldUpdateFlag = false;
            mChestGold.text = "0";
        }

        protected override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void OnExit()
        {
            if (mGoldEff2 != null)
                GameObject.Destroy(mGoldEff2);
        }

        protected override void OnUpdate(float timeElapsed)
        {
            if (mBattle == null)
                return;
            int curTime = mBattle.CurTime;
            if (curTime > mLastCountTime)
            {
                mLastCountTime = curTime;
                //count down
                int formatTime = 0;
                if (mBattle.EndTime > curTime)
                {
                    var intervalTime = mBattle.EndTime - curTime;
                    var minute = intervalTime / 60;
                    var second = intervalTime - (minute * 60);
                    formatTime = (int) minute * 100 + (int) second;
                }
                mTimeLeft.Value = formatTime;
                //wave tip
                if (mWaveTip != null)
                {
                    var secLeft = Math.Max(0, mBattle.NextMonsterWaveTime - curTime);
                    mWaveTip.text = string.Format(mBattle.GoldRushData.waveTipContent, secLeft);
                }
            }
            //update gold
            UpdateGoldAnim(timeElapsed);
        }

        private void StartGoldAnim()
        {
            mLastUpdateGoldTime = 0;
            mGoldUpdateTimes = 0;
            mGoldUpdateFlag = true;
        }
        
        private void UpdateGoldAnim(float timeElapsed)
        {
            if (!mGoldUpdateFlag)
                return;
            mLastUpdateGoldTime += timeElapsed;
            if (mLastUpdateGoldTime >= 0.15f * (mGoldUpdateTimes + 1))
            {
                mChestGoldDT.DORestart();
                mGoldUpdateTimes++;
                mLastGold = Mathf.CeilToInt(Mathf.Lerp(mLastGold, mCurGold, mLastUpdateGoldTime));
                mChestGold.text = string.Format("{0}", mLastGold);
                if (mLastGold >= mCurGold)
                {
                    mGoldUpdateFlag = false;
                }
            }
        }
        

        public void ShowWaveTip(bool bActive)
        {
            if (bActive)
            {
                string tipText = string.Format(mBattle.GoldRushData.waveTipContent, mBattle.GoldRushData.waveTipTime);
                mWaveTipGo = SystemNotifyManager.SysDungeonSkillTip(tipText, mBattle.GoldRushData.waveTipTime);
                ComCommonBind bind = mWaveTipGo.GetComponent<ComCommonBind>();
                if (bind != null)
                {
                    mWaveTip = bind.GetCom<Text>("txtTip");
                    if (mWaveTip != null)
                        mWaveTip.text = tipText;
                }
            }
            else if (mWaveTipGo != null)
            {
                GameObject.Destroy(mWaveTipGo);
                mWaveTipGo = null;
                mWaveTip = null;
            }
        }
        
        public void AddGold(int num, Vector3 pos)
        {
            var battleCamera = BattleMain.instance.Main.currentGeScene.GetCamera();
            var camera = ClientSystemManager.GetInstance().UICamera;
            var screenPos = battleCamera.WorldToScreenPoint(pos);
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mGoldTrailRoot.rectTransform(), screenPos, camera, out localPos);
            
            var effgo = AssetLoader.instance.LoadResAsGameObject(goldEffPath1);
            Utility.AttachTo(effgo, mGoldTrailRoot);
            effgo.transform.localPosition = localPos;
            var comp = effgo.AddComponent<TweenPosArc>();
            comp.targetPosition = mGoldBag.gameObject;
            comp.conmat = 30f;
            comp.NeedFindTarget = false;
            int addNum = num;
            comp.onFinish.AddListener(() =>
            {
                mCurGold += addNum;
                StartGoldAnim();
                
                if (effgo != null)
                {
                    if (mGoldEff2 == null)
                    {
                        mGoldEff2 = AssetLoader.instance.LoadResAsGameObject(goldEffPath2);
                        Utility.AttachTo(mGoldEff2, mGoldTrailRoot);
                        mGoldEff2.transform.position = mGoldBag.transform.position;
                    }
                    else
                    {
                        mGoldEff2.CustomActive(false);
                        mGoldEff2.CustomActive(true);
                    }
                    
                    GameObject.Destroy(effgo);
                }
            });
        }
    }
}