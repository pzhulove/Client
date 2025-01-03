using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace GameClient
{
    /// <summary>
    /// 团队副本UI界面
    /// </summary>
    public class TeamDungeonBattleFrame : ClientFrame
    {
        #region ExtraUIBind
        private GameObject mLiuxingqipao = null;
        private Slider mSlider_LiuxingComplete = null;
        private Text mText_CompleteNum = null;
        private Image mImage_LiuXingTimeReduce = null;
        private Image mImage_LiuxingAttackCD = null;
        private Text mText_CDTime = null;
        private ComButtonEx mBtn_Attack = null;
        private GameObject mBtn_AttackEffect = null;
        private GameObject mXianshijisha = null;
        private Text mText_KillCount = null;
        private Text mText_KillTime = null;
        private GameObject mTeamBossEnergyBar = null;
        private Slider mEnergyValue = null;
        private GameObject mEnergyStat = null;
        private GameObject mEnergyFull = null;
        private GameObject mPurpleEffect = null;
        private GameObject mBlueEffect = null;
        private GameObject mYellowEffect = null;
        private GameObject mFullEffect = null;
        private GameObject mKillCompleteGO = null;
        private GameObject mKillTimeGO = null;
        private RectTransform mTunshiGo = null;
        private Text mTunshiText = null;

        protected override void _bindExUI()
        {
            mLiuxingqipao = mBind.GetGameObject("Liuxingqipao");
            mSlider_LiuxingComplete = mBind.GetCom<Slider>("Slider_LiuxingComplete");
            mText_CompleteNum = mBind.GetCom<Text>("Text_CompleteNum");
            mImage_LiuXingTimeReduce = mBind.GetCom<Image>("Image_LiuXingTimeReduce");
            mImage_LiuxingAttackCD = mBind.GetCom<Image>("Image_LiuxingAttackCD");
            mText_CDTime = mBind.GetCom<Text>("Text_CDTime");
            mBtn_Attack = mBind.GetCom<ComButtonEx>("Btn_Attack");
            if (null != mBtn_Attack)
            {
                mBtn_Attack.onClick.AddListener(_onBtn_AttackButtonClick);
            }
            mBtn_AttackEffect = mBind.GetGameObject("Btn_AttackEffect");
            mXianshijisha = mBind.GetGameObject("Xianshijisha");
            mText_KillCount = mBind.GetCom<Text>("Text_KillCount");
            mText_KillTime = mBind.GetCom<Text>("Text_KillTime");
            mTeamBossEnergyBar = mBind.GetGameObject("TeamBossEnergyBar");
            mEnergyValue = mBind.GetCom<Slider>("EnergyValue");
            mEnergyStat = mBind.GetGameObject("energyStat");
            mEnergyFull = mBind.GetGameObject("energyFull");
            mPurpleEffect = mBind.GetGameObject("purpleEffect");
            mBlueEffect = mBind.GetGameObject("blueEffect");
            mYellowEffect = mBind.GetGameObject("yellowEffect");
            mFullEffect = mBind.GetGameObject("fullEffect");
            mKillCompleteGO = mBind.GetGameObject("KillCompleteGO");
            mKillTimeGO = mBind.GetGameObject("KillTimeGO");
            mTunshiGo = mBind.GetCom<RectTransform>("TunshiGo");
            mTunshiText = mBind.GetCom<Text>("TunshiText");
        }

        protected override void _unbindExUI()
        {
            mLiuxingqipao = null;
            mSlider_LiuxingComplete = null;
            mText_CompleteNum = null;
            mImage_LiuXingTimeReduce = null;
            mImage_LiuxingAttackCD = null;
            mText_CDTime = null;
            if (null != mBtn_Attack)
            {
                mBtn_Attack.onClick.RemoveListener(_onBtn_AttackButtonClick);
            }
            mBtn_Attack = null;
            mBtn_AttackEffect = null;
            mXianshijisha = null;
            mText_KillCount = null;
            mText_KillTime = null;
            mTeamBossEnergyBar = null;
            mEnergyValue = null;
            mEnergyStat = null;
            mEnergyFull = null;
            mPurpleEffect = null;
            mBlueEffect = null;
            mYellowEffect = null;
            mFullEffect = null;
            mKillCompleteGO = null;
            mKillTimeGO = null;
            mTunshiGo = null;
            mTunshiText = null;
        }
        #endregion

        #region Callback
        #endregion

        private int mLastBossEnergyLevel = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/TeamDungeonBattleFrame";
        }
        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            mLastBossEnergyLevel = 0;
        }
        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            base._OnUpdate(timeElapsed);
            UpdateLiuXingReduceBar(timeElapsed);
            UpdateLiuXingAttackCD(timeElapsed);
        }

        #region 击杀计数 非限时
        public void SetNoTimeLimitKillNum(int curNum,int totalNum)
        {
            //Init UI
            if (!mXianshijisha.activeSelf)
            {
                mXianshijisha.CustomActive(true);
            }
            if (mKillTimeGO.activeSelf)
            {
                mKillTimeGO.SetActive(false);
            }
            if (!mKillCompleteGO.activeSelf)
            {
                mKillCompleteGO.SetActive(true);
            }
            mText_KillCount.text = string.Format("{0}/{1}", curNum, totalNum);
        }
        #endregion

        #region 限时击杀
        /// <summary>
        /// 设置击杀数量
        /// </summary>
        public void SetKillNum(int curNum, int totalNum)
        {
            mText_KillCount.text = string.Format("{0}/{1}", curNum, totalNum);
            mXianshijisha.CustomActive(true);
        }

        /// <summary>
        /// 剩余时间
        /// </summary>
        /// <param name="time"></param>
        public void SetKillTime(int remainTime)
        {
            float timeSpend = remainTime / 1000;
            int hour = (int)timeSpend / 3600;
            int minute = ((int)timeSpend - hour * 3600) / 60;
            int second = (int)timeSpend - hour * 3600 - minute * 60;

            if (mText_KillTime != null)
            {
                mText_KillTime.text = string.Format("{0:D2}:{1:D2}", minute, second);
            }

            if (mXianshijisha != null)
            {
                mXianshijisha.CustomActive(true);
            }
        }
        #endregion

        #region 流星气泡
        private Mechanism2064.Del liuXingAttackDel;
        private float curTotalTime = 0;
        private float curTime = 0;
        private float reduceTime = 0;
        private float delayTime = 0.3f;
        private float curDelayTime = 0;

        private float cdTime = 0.7f;
        private float curCdTime = 0;

        /// <summary>
        /// 初始化流星气泡
        /// </summary>
        /// <param name="totalTime"></param>
        public void InitLiuXingData(int totalTime)
        {
            curTotalTime = totalTime;
            curTime = totalTime;
            mSlider_LiuxingComplete.value = 1;
            mLiuxingqipao.CustomActive(true);
        }
        
        /// <summary>
        /// 设置时间的进度条
        /// </summary>
        public void RefreshLiuXingTime(int time)
        {
            curTime -= time;
            mSlider_LiuxingComplete.value = curTime / curTotalTime;
            mLiuxingqipao.CustomActive(true);
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        public void AttackFial(int time)
        {
            curDelayTime = delayTime;
            reduceTime += time;
            RefreshLiuXingTime(time);
        }

        /// <summary>
        /// 设置完成数量
        /// </summary>
        public void SetLiuXingCompleteNum(int curNum,int TotalNum)
        {
            mText_CompleteNum.text = string.Format("{0}/{1}", curNum, TotalNum);
            mLiuxingqipao.CustomActive(true);
        }

        /// <summary>
        /// 显示关卡玩家操作按钮
        /// </summary>
        public void SetLiuXingAttackBtn(Mechanism2064.Del del)
        {
            liuXingAttackDel = del;
            curCdTime = 2.0f;
            mBtn_Attack.CustomActive(true);
            mLiuxingqipao.CustomActive(true);
        }

        private void _onBtn_AttackButtonClick()
        {
            var effect = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/EffectUI/EffUI_dianjifankui01");
            if (effect != null)
            {
                Utility.AttachTo(effect, mBtn_AttackEffect);
            }
            curCdTime = cdTime;
            mBtn_AttackEffect.CustomActive(true);
            if (liuXingAttackDel != null)
            {
                liuXingAttackDel();
            }
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public void PlayAttackResultEffect(bool isSuccess)
        {
            string path = "Effects/UI/Prefab/EffUI_tuanben/Prefab/EffUI_tuanben_wanchengdu_zengjia";
            if (!isSuccess)
            {
                path = "Effects/UI/Prefab/EffUI_tuanben/Prefab/EffUI_tuanben_wanchengdu_jianshao";
            }
            var effect = AssetLoader.instance.LoadResAsGameObject(path);
            if (effect != null)
            {
                Utility.AttachTo(effect, mSlider_LiuxingComplete.gameObject);
                effect.transform.localPosition = new Vector3(0, -6, 0);
            }
        }

        /// <summary>
        /// 刷新淤血条
        /// </summary>
        private void UpdateLiuXingReduceBar(float timeDeleta)
        {
            if (reduceTime <= 0)
            {
                mImage_LiuXingTimeReduce.fillAmount = curTime / curTotalTime;
                return;
            }

            if (curDelayTime <= 0)
            {
                float reduceDelta = timeDeleta * 10000;
                reduceTime -= reduceDelta;
                mImage_LiuXingTimeReduce.fillAmount -= reduceDelta / curTotalTime;
            }
            else
            {
                curDelayTime -= timeDeleta;
            }
        }

        /// <summary>
        /// 刷新按钮CD
        /// </summary>
        /// <param name="deltaTime"></param>
        private void UpdateLiuXingAttackCD(float deltaTime)
        {
            if(curCdTime <= 0)
            {
                mImage_LiuxingAttackCD.CustomActive(false);
                mText_CDTime.CustomActive(false);
                mBtn_Attack.interactable = true;
            }
            else
            {
                curCdTime -= deltaTime;
                mImage_LiuxingAttackCD.fillAmount = curCdTime / cdTime;
                mText_CDTime.text = curCdTime.ToString("0.0");
                mImage_LiuxingAttackCD.CustomActive(true);
                mText_CDTime.CustomActive(true);
                mBtn_Attack.interactable = false;
            }
        }
        #endregion


        private void RefreshBossEnergyUIEffect(int level)
        {
            if(level == 0)
            {
                if (mPurpleEffect != null)
                {
                    mPurpleEffect.CustomActive(false);
                }
                if (mYellowEffect != null)
                {
                    mYellowEffect.CustomActive(false);
                }
                if (mBlueEffect != null)
                {
                    mBlueEffect.CustomActive(false);
                }
            }
            else if (level == 1)
            {
                if(mPurpleEffect != null)
                {
                    mPurpleEffect.CustomActive(false);
                }
                if (mYellowEffect != null)
                {
                    mYellowEffect.CustomActive(false);
                }
                if (mBlueEffect != null)
                {
                    mBlueEffect.CustomActive(true);
                }
            }
            else if(level == 2)
            {
                if (mPurpleEffect != null)
                {
                    mPurpleEffect.CustomActive(false);
                }
                if (mYellowEffect != null)
                {
                    mYellowEffect.CustomActive(true);
                }
                if (mBlueEffect != null)
                {
                    mBlueEffect.CustomActive(false);
                }
            }
            else if(level == 3)
            {
                if (mPurpleEffect != null)
                {
                    mPurpleEffect.CustomActive(true);
                }
                if (mYellowEffect != null)
                {
                    mYellowEffect.CustomActive(false);
                }
                if (mBlueEffect != null)
                {
                    mBlueEffect.CustomActive(false);
                }
            }
        }
        public void SetBossEnergyBarActive(bool isActive)
        {
            if(mTeamBossEnergyBar != null)
            {
                mTeamBossEnergyBar.CustomActive(isActive);
            }
            if (mEnergyFull != null)
            {
                mEnergyFull.CustomActive(!isActive);
            }
            if(mEnergyStat != null)
            {
                mEnergyStat.CustomActive(isActive);
            }
            if(isActive)
            {
                RefreshBossEnergyUIEffect(mLastBossEnergyLevel);
            }
            
        }
        public void  SetBossEnergyValue(float value,int level)
        {
            if(value <= 0 || value > 99.0f)
            {
                if(mEnergyFull != null && !mEnergyFull.activeInHierarchy)
                {
                    mEnergyFull.CustomActive(true);
                    if (mFullEffect != null)
                    {
                        mFullEffect.CustomActive(true);
                    }
                    mLastBossEnergyLevel = 0;
                    RefreshBossEnergyUIEffect(mLastBossEnergyLevel);
                }
                if(mEnergyStat != null && mEnergyStat.activeInHierarchy)
                {
                    mEnergyStat.CustomActive(false);
                }
            }
            else
            {
                if (mEnergyFull != null && mEnergyFull.activeInHierarchy)
                {
                    mEnergyFull.CustomActive(false);
                    if(mFullEffect != null)
                    {
                        mFullEffect.CustomActive(false);
                    }
                }
                if (mEnergyStat != null)
                {
                    if(!mEnergyStat.activeInHierarchy)
                        mEnergyStat.CustomActive(true);

                    if(mEnergyValue != null)
                    {
                        mEnergyValue.value = value;
                    }
                }
                if(level != mLastBossEnergyLevel)
                {
                    RefreshBossEnergyUIEffect(level);
                    mLastBossEnergyLevel = level;
                }
            }
        }

        #region 章鱼吞噬机制
        /// <summary>
        /// 完成数量
        /// </summary>
        public void ShowCompelteCount(int curCount,int totalCount)
        {
            mTunshiGo.gameObject.CustomActive(true);
            mTunshiText.text = string.Format("{0}/{1}", curCount, totalCount);
        }
        #endregion
    }
}
