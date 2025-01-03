using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace GameClient
{
    [ExecuteAlways]
    public class AdventureTeamPassBlessExpPoolBind : MonoBehaviour
    {
        private const string EFFUI_EXP_POOL_IDLE_RES_PATH = "Effects/UI/Prefab/EffUI_yongbingtuan/Prefab/EffUI_yongbingtuan_daiji";
        private const string EFFUI_EXP_POOL_RISE_UP_RES_PATH = "Effects/UI/Prefab/EffUI_yongbingtuan/Prefab/EffUI_yongbingtuan_yaojizengzhang";
        private const string EFFUI_EXP_POOL_FILL_UP_RES_PATH = "Effects/UI/Prefab/EffUI_yongbingtuan/Prefab/EffUI_yongbingtuan_manpin";
        private const string EFFUI_EXP_FULL_FLYING_RES_PATH = "Effects/UI/Prefab/EffUI_yongbingtuan/Prefab/EffUI_yongbingtuan_feixing";

        [SerializeField]
        [Header("特效挂载根节点")]
        private GameObject mEffectsRoot;
        [Space(5)]

        [SerializeField]
        [Header("空-液体高度 - 只用于显示")]
        [Range(0f, 1f)]
        private float mEmptyPoolLiquidHeight = 0.1f;
        [SerializeField]
        [Header("满-液体高度 - 只用于显示")]
        [Range(0f, 1f)]
        private float mFullPoolLiquidHeight = 0.67f;
        [SerializeField]
        [Header("半-液体高度")]
        [Range(0f, 1f)]
        private float mHalfPoolLiquidHeight = 0.41f;
        [SerializeField]
        [Header("液体高度百分比")]
        private Text mPoolLiquidPercentText = null;
        //[SerializeField]
        //[Header("液体上涨速度/每百分之一 = 上涨1%需要的时间")]
        //private float mPoolLiquidVelocity = 5f;
        [SerializeField]
        [Header("液体每次上涨一定量的时间")]
        private float mPoolLiquidRiseupTime = 2f;
        [SerializeField]
        [Header("半瓶 - 液体上涨高度曲线")]
        private AnimationCurve mHalfPoolLiguidAnimCurve;

        [SerializeField]
        [Header("液体每次下降一定量的时间")]
        private float mPoolLiquidGrowdownTime = 1f;

        [Space(10)]

        [Header("经验飞行轨迹 主要依赖于终点配置 见每个终点")]
        [SerializeField]
        [Header("经验飞行起点")]
        private GameObject flyStartPoint;
        [SerializeField]
        [Header("经验飞行缓动曲线")]
        private AnimationCurve flyEaseCurveType;
        [SerializeField]
        [Header("经验飞行轨迹点数目，不包括起点，但包括终点")]
        private int flyPathPointCount = 20;
        [Header("经验飞行时间")]
        [SerializeField]
        private float flyDuration = 1f;
        [Header("经验飞行轨迹弧度")]
        [SerializeField]
        private float flyPathRadian = 1f;
        [Header("经验飞行间隔时间，每次飞行前后的时间间隔")]
        [SerializeField]
        private float flyDelayTime = 0.5f;

        [Space(5)]
        [Header("### 特效本身相关 ###")]
        [SerializeField]
        [Header("液体经验涨满动画时长")]
        private float mEffectExpFullDuration = 3f;
        [SerializeField]
        [Header("液体经验上涨动画时长")]
        private float mEffectExpRiseupDuration = 4f;


        private bool bExpPoolIdleEffectInited = false;
        private bool bExpPoolRiseupEffectInited = false;
        private bool bExpPoolFillupEffectInited = false;
        private bool bExpPoolFullFlyingEffectInited = false;

        private GameObject mExpPoolIdle = null;
        private GameObject mExpPoolRiseup = null;
        private GameObject mExpPoolFillup = null;
        private GameObject mExpPoolFullFlying = null;

        private MeshRenderer mExpPoolLiquidRen = null;
        private Material mExpPoolLiquidMat = null;

        //经验飞行相关
        private int totalFlyPathPointCount = 0;
        private Vector3[] totalflyPathPostions = null;
        private bool isFlying = false;      
        private Tweener flyTween = null;
        private GameObject flyTarget = null;
        

        //经验涨
        private UnityEngine.Coroutine waitToRiseupExp = null;
        

        //特效本身
        private UnityEngine.Coroutine waitToPlayExpFullEffect = null;
        private UnityEngine.Coroutine waitToPlayExpRiseupEffect = null;


        [HideInInspector]
        public Action ExpFlyToTargetHandler;
        [HideInInspector]
        public Action ExpRiseupToFullHandler;
        [HideInInspector]
        public Action ExpRiseupStartHandler;
        [HideInInspector]
        public Action ExpRiseupEndHandler;


        private string tr_exp_percent_format = "";

        void Awake()
        {
            _InitView();
        }

        void OnDestroy()
        {
            bExpPoolIdleEffectInited = false;
            bExpPoolRiseupEffectInited = false;
            bExpPoolFillupEffectInited = false;
            bExpPoolFullFlyingEffectInited = false;

            mExpPoolIdle = null;
            mExpPoolRiseup = null;
            mExpPoolFillup = null;
            mExpPoolFullFlying = null;

            mExpPoolLiquidRen = null;
            mExpPoolLiquidMat = null;

            totalFlyPathPointCount = 0;
            totalflyPathPostions = null;
            isFlying = false;
            if (flyTween != null)
            {
                flyTween.Kill();
            }
            flyTween = null;
            flyTarget = null;

            if (waitToRiseupExp != null)
            {
                StopCoroutine(waitToRiseupExp);
                waitToRiseupExp = null;
            }

            _RemoveAllDelegateHandler(ExpRiseupToFullHandler);
            _RemoveAllDelegateHandler(ExpRiseupStartHandler);
            _RemoveAllDelegateHandler(ExpFlyToTargetHandler);
            _RemoveAllDelegateHandler(ExpRiseupEndHandler);

            tr_exp_percent_format = "";


            if (waitToPlayExpFullEffect != null)
            {
                StopCoroutine(waitToPlayExpFullEffect);
                waitToPlayExpFullEffect = null;
            }
            if (waitToPlayExpRiseupEffect != null)
            {
                StopCoroutine(waitToPlayExpRiseupEffect);
                waitToPlayExpRiseupEffect = null;
            }
        }

        private void _RemoveAllDelegateHandler(System.Action handler)
        {
            if (handler != null)
            {
                var dels = handler.GetInvocationList();
                if (dels != null)
                {
                    for (int i = 0; i < dels.Length; i++)
                    {
                        handler -= dels[i] as System.Action;
                    }
                }
            }
            handler = null;
        }

        private void _InitView()
        {
            if (mHalfPoolLiguidAnimCurve != null && mHalfPoolLiguidAnimCurve.length > 0)
            {
                mEmptyPoolLiquidHeight = mHalfPoolLiguidAnimCurve[0].value;
                mFullPoolLiquidHeight = mHalfPoolLiguidAnimCurve[mHalfPoolLiguidAnimCurve.length - 1].value;
            }

            totalFlyPathPointCount = flyPathPointCount + 1; //加上起点
            totalflyPathPostions = new Vector3[totalFlyPathPointCount];

            tr_exp_percent_format = TR.Value("adventure_team_pass_bless_get_exp_percent");
        }

        private GameObject _LoadEffectByResPath(string effectPath)
        {
            GameObject effectGo = null;
            if (string.IsNullOrEmpty(effectPath))
            {
                return effectGo;
            }
            effectGo = AssetLoader.GetInstance().LoadResAsGameObject(effectPath);
            return effectGo;
        }

        public void InitExpPoolIdleEffect()
        {
            if (bExpPoolIdleEffectInited)
                return;
            mExpPoolIdle = _LoadEffectByResPath(EFFUI_EXP_POOL_IDLE_RES_PATH);            
            Utility.AttachTo(mExpPoolIdle, mEffectsRoot);

            if (mExpPoolIdle != null && mExpPoolIdle.transform.childCount > 0)
            {
                var firstChild = mExpPoolIdle.transform.GetChild(0);
                if (firstChild != null)
                {
                    mExpPoolLiquidRen = firstChild.GetComponent<MeshRenderer>();
                    if (mExpPoolLiquidRen != null)
                    {
                        mExpPoolLiquidMat = mExpPoolLiquidRen.material;
                    }

                    //初始化
                    _ControlExpHeight(mEmptyPoolLiquidHeight);
                }
            }

            //避免特效的显示问题 处理后再显示出来
            mExpPoolIdle.CustomActive(true);

            bExpPoolIdleEffectInited = true;
        }
        public void StartExpRiseupToHeight(int startPercent, int endPercent, bool fakePlayAnim = false)
        {
            float startHeight = _GetHeightByPercent(startPercent / 100f);
            float endHeight = _GetHeightByPercent(endPercent / 100f);
            float currHeight = startHeight;
            float delta = endHeight - startHeight;
            delta = delta >= 0f ? delta : -delta;
            if (delta <= 0.001f)
            {
                //接近相等
                _ControlExpHeight(endHeight);
                _SetPercentText(endPercent.ToString());
            }
            else
            {
                _ControlExpHeight(startHeight);
                _SetPercentText(startPercent.ToString());
                if (waitToRiseupExp != null)
                {
                    StopCoroutine(waitToRiseupExp);
                }

                //增涨的回调
                if (endPercent > startPercent)
                {
                    if (ExpRiseupStartHandler != null)
                    {
                        ExpRiseupStartHandler();
                    }
                }

                waitToRiseupExp = StartCoroutine(_WaitToRiseupExp(startPercent, endPercent));

                //如果不是假动画 需要考虑动画完成的回调
                if (fakePlayAnim == false)
                {
                    //这里是真正的结束
                    if (ExpRiseupEndHandler != null)
                    {
                        ExpRiseupEndHandler();
                    }
                }
            }
        }

        IEnumerator _WaitToRiseupExp(int startPercent, int endPercent)
        {
            int tempStart = startPercent;
            int tempEnd = endPercent;
            int deltaPercent = tempEnd - tempStart;

            bool bAddup = true;
            if (deltaPercent < 0)
            {
                bAddup = false;
            }

            int tempPercent = tempStart;
            float ftempPercent = 0f;


            //float duration = 1f / mPoolLiquidVelocity;
            deltaPercent = deltaPercent > 0 ? deltaPercent : -deltaPercent;

            if (deltaPercent == 0)
            {
                yield break;
            }

            float duration = 0f;
            if (bAddup)
            {
                duration = mPoolLiquidRiseupTime / deltaPercent;   
            }
            else
            {
                duration = mPoolLiquidGrowdownTime / deltaPercent;   
            }

            while (deltaPercent != 0 && tempPercent <= 100 && tempPercent >= 0)
            {
                if (bAddup)
                {
                    tempPercent++;
                    if (tempPercent == 100)
                    {
                        if (ExpRiseupToFullHandler != null)
                        {
                            ExpRiseupToFullHandler();
                        }
                    }
                    else if(tempPercent > 100)
                    {
                        yield break;
                    }
                }
                else
                {
                    tempPercent--;
                    if (tempPercent < 0)
                    {
                        yield break;
                    }
                }
                _SetPercentText(tempPercent.ToString());
                ftempPercent = tempPercent / 100f;
                _ControlExpHeight(_GetHeightByPercent(ftempPercent));
                deltaPercent = tempEnd - tempPercent;
                yield return Yielders.GetWaitForSeconds(duration);
            }
        }

        private float _GetHeightByPercent(float percent)
        {
            if (percent > 1f)
            {
                percent = 1f;
            }
            else if (percent < 0f)
            {
                percent = 0f;
            }
            if (mHalfPoolLiguidAnimCurve != null)
            {
                //Logger.LogError("_GetHeightByPercent : " + mHalfPoolLiguidAnimCurve.Evaluate(percent));
                return mHalfPoolLiguidAnimCurve.Evaluate(percent);
            }
            return 0f;
        }


        private void _ControlExpHeight(float height)
        {
            if (height < mEmptyPoolLiquidHeight)
            {
                height = mEmptyPoolLiquidHeight;
            }
            if (height > mFullPoolLiquidHeight)
            {
                height = mFullPoolLiquidHeight;
            }
            if (mExpPoolLiquidMat != null)
            {
                mExpPoolLiquidMat.SetFloat("_Quantity", height);
            }
        }

        private void _SetPercentText(string percent)
        {
            if (mPoolLiquidPercentText)
            {
                mPoolLiquidPercentText.text = string.Format(tr_exp_percent_format, percent);
            }
        }

        public void InitExpPoolRiseupEffect()
        {
            if (bExpPoolRiseupEffectInited)
                return;
            mExpPoolRiseup = _LoadEffectByResPath(EFFUI_EXP_POOL_RISE_UP_RES_PATH);
            mExpPoolRiseup.CustomActive(false);
            Utility.AttachTo(mExpPoolRiseup, mEffectsRoot);
            bExpPoolRiseupEffectInited = true;
        }

        public void SetExpPoolRiseupShow(bool bShow)
        {
            if (mExpPoolRiseup)
            {
                if (!bShow)
                {
                    mExpPoolRiseup.CustomActive(false);
                }
                else
                {
                    if (waitToPlayExpRiseupEffect != null)
                    {
                        StopCoroutine(waitToPlayExpRiseupEffect);
                    }
                    waitToPlayExpRiseupEffect = StartCoroutine(_WaitToPlayExpRiseupEffect());
                }
            }
        }

        IEnumerator _WaitToPlayExpRiseupEffect()
        {
            mExpPoolRiseup.CustomActive(false);
            mExpPoolRiseup.CustomActive(true);
            yield return Yielders.GetWaitForSeconds(mEffectExpRiseupDuration);
            mExpPoolRiseup.CustomActive(false);
        }

        public void InitExpPoolFillupEffect()
        {
            if (bExpPoolFillupEffectInited)
                return;
            mExpPoolFillup = _LoadEffectByResPath(EFFUI_EXP_POOL_FILL_UP_RES_PATH);
            mExpPoolFillup.CustomActive(false);
            Utility.AttachTo(mExpPoolFillup, mEffectsRoot);
            bExpPoolFillupEffectInited = true;
        }

        public void SetExpPoolFillupShow(bool bShow)
        {
            if (mExpPoolFillup)
            {
                if (!bShow)
                {
                    mExpPoolFillup.CustomActive(false);
                }
                else
                {
                    if (waitToPlayExpFullEffect != null)
                    {
                        StopCoroutine(waitToPlayExpFullEffect);
                    }
                    waitToPlayExpFullEffect = StartCoroutine(_WaitToPlayExpFullEffect());
                }
            }
        }

        IEnumerator _WaitToPlayExpFullEffect()
        {
            mExpPoolFillup.CustomActive(false);
            mExpPoolFillup.CustomActive(true);
            yield return Yielders.GetWaitForSeconds(mEffectExpFullDuration);
            mExpPoolFillup.CustomActive(false);
        }

        public void InitExpPoolFullFlyingEffect()
        {
            if (bExpPoolFullFlyingEffectInited)
                return;
            mExpPoolFullFlying = _LoadEffectByResPath(EFFUI_EXP_FULL_FLYING_RES_PATH);
            mExpPoolFullFlying.CustomActive(false);
            Utility.AttachTo(mExpPoolFullFlying, mEffectsRoot);
            bExpPoolFullFlyingEffectInited = true;
        }

        public void StartFullExpFlyingToTarget(GameObject flyTarget)
        {
            if (mExpPoolFullFlying == null || flyStartPoint == null || flyTarget == null)
            {
                return;
            }
            this.flyTarget = flyTarget;

            Vector3 startPos = flyStartPoint.transform.position;
            Vector3 endPos = flyTarget.transform.position;
            //Debug.LogError("111 startPos " + startPos);
            //Debug.LogError("111 endPos " + endPos);
            
            //if (flyTween != null)
            //{
            //    flyTween.Kill();
            //}
            //直线飞行
            //flyTween = mExpPoolFullFlying.transform.DOMove(endPos, flyDuration)
            //    .SetEase(flyEaseCurveType)
            //    .OnStart(() =>
            //    {
            //        isFlying = true;
            //        mExpPoolFullFlying.CustomActive(true);
            //    })
            //    .OnComplete(() =>
            //    {
            //        mExpPoolFullFlying.CustomActive(false);
            //        isFlying = false;
            //    });

            //重新计算抛物线轨迹点
            //遍历所有的点 包括起点和终点
            for (int i = 0; i < totalFlyPathPointCount; i++)
            {
                Vector3 p = SampleParabola(startPos, endPos, flyPathRadian, i / (float)flyPathPointCount);
                //Debug.LogError("222 " + p);
                if (totalflyPathPostions != null && totalflyPathPostions.Length >= totalFlyPathPointCount)
                {
                    totalflyPathPostions[i] = p;
                }
            }

            if (totalflyPathPostions != null && totalflyPathPostions.Length > 0)
            {
                if (flyTween != null)
                {
                    flyTween.Kill();
                }
                //重置位置到起点
                mExpPoolFullFlying.transform.position = startPos;

                flyTween = mExpPoolFullFlying.transform.DOPath(totalflyPathPostions, flyDuration,
                    PathType.CatmullRom, PathMode.Sidescroller2D, 5, Color.red)
                    .SetEase(flyEaseCurveType)
                    .SetDelay(flyDelayTime)
                    .OnStart(_OnFlyToTargetStart)
                    .OnComplete(_OnFlyToTargetCompete);                    
            }
        }

        private void _OnFlyToTargetStart()
        {
            isFlying = true;
            mExpPoolFullFlying.CustomActive(true);
        }
        private void _OnFlyToTargetCompete()
        {
            mExpPoolFullFlying.CustomActive(false);
            isFlying = false;
            if (ExpFlyToTargetHandler != null)
            {
                ExpFlyToTargetHandler();
            }
        }


        #region TOOLS

        void OnDrawGizmos()
        {
            if (flyStartPoint == null || flyTarget == null)
            {
                return;
            }
            Vector3 a = flyStartPoint.transform.position;
            Vector3 b = flyTarget.transform.position;
            //Draw the parabola by sample a few times
            Gizmos.color = Color.red;
            Gizmos.DrawLine(a, b);
            Vector3 lastP = a;
            for (float i = 0; i < flyPathPointCount + 1; i++)
            {
                if (flyPathPointCount == 0)
                {
                    return;
                }

                Vector3 p = SampleParabola(a, b, flyPathRadian, i / flyPathPointCount);
                Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
                Gizmos.DrawLine(lastP, p);
                lastP = p;
            }
        }


        /// <summary>
        /// Get position from a parabola defined by start and end, height, and time
        /// </summary>
        /// <param name='start'>
        /// The start point of the parabola
        /// </param>
        /// <param name='end'>
        /// The end point of the parabola
        /// </param>
        /// <param name='height'>
        /// The height of the parabola at its maximum
        /// </param>
        /// <param name='t'>
        /// Normalized time (0->1)
        /// </param>S
        Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
        {
            float parabolicT = t * 2 - 1;
            if (Mathf.Abs(start.y - end.y) < 0.1f)
            {
                //start and end are roughly level, pretend they are - simpler solution with less steps
                Vector3 travelDirection = end - start;
                Vector3 result = start + t * travelDirection;
                result.y += (-parabolicT * parabolicT + 1) * height;
                return result;
            }
            else
            {
                //start and end are not level, gets more complicated
                Vector3 travelDirection = end - start;
                Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
                Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
                Vector3 up = Vector3.Cross(right, travelDirection);
                if (end.y > start.y)
                    up = -up;
                Vector3 result = start + t * travelDirection;
                result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
                return result;
            }
        }

        public Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        public Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector2.Lerp(start, end, t);

            return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
        }

        #endregion
    }
}