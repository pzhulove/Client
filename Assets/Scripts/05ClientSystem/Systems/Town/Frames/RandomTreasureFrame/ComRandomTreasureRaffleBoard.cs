using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System;

namespace GameClient
{
    class ComRandomTreasureRaffleGridStep
    {
       public int stepIndex = 0;                          //动画帧序号
       public Vector2 anchorVec2 = new Vector2();         //格子Rect位置
       public float stepInterval = 0f;                    //动画移动间隔
    }

    [Serializable]
    public class ComRandomTresureRaffleGetItem
    {
        public GameObject getItemGo;
        public RectTransform getItemRect;
        public float getItemTweenDuration = 0f;
        public float getItemTweenDelay = 0f;
        public Ease getItemPosTweenEase = Ease.Unset;
        public Vector2 getItemTargetPos;
        public Vector2 getItemInitPos;

        //add
        public List<DOTweenAnimation> getItemIconTweens;        
    }

    public class ComRandomTreasureRaffleBoard : MonoBehaviour
    {
        #region Model Params

        private UnityAction raffleAnimStart;
        private UnityAction raffleAnimEnd;

        private bool bInited = false;
        
        #endregion
        
        #region View Params
        //格子
        public int mGridX = 5;
        public int mGridY = 5;
        public float mGridSize = 100f;
        public float mGridSpace = 20f;
        public GameObject mGridContent = null;
        List<Vector2> mGridPosList = new List<Vector2>();

        //格子选择遮罩
        public GameObject mGridSelect = null;
        private RectTransform mGridSelectRect = null;
        private Vector2 mSelectGridPosVec2 = new Vector2();

        private float gridContentSizeX = 0f;
        public float GridContentSizeX {
            get { return gridContentSizeX; }
        }
        private float gridContentSizeY = 0f;
        public float GridContentSizeY {
            get { return gridContentSizeX; }
        }

        //获得展示格子
        public List<ComRandomTresureRaffleGetItem> mGridGetItems = null;
        private ComItem mMainGetItem = null;
        private ItemData mMainGetItemData = null;
        private List<ItemSimpleData> mTotalGetItemDatas = new List<ItemSimpleData>();
        private Sequence mGetItemTweenQuene = null;


        //格子换置动画参数
        [Header("变速动画")]
        [SerializeField]
        private AnimationCurve mAnimCurve = null;
        [Header("这个速度用于增加动画时长，越大则动画越慢")]
        [SerializeField]
        private float mAnimSlowSpeed = 1f;
        [Header("快结束的慢速点数量固定值")]
        [SerializeField]
        private int mAnimStepCount_Final = 0;
        [Header("快结束的慢速点数量，固定值<=0，则使用随机值，该值为随机的最小值")]
        [SerializeField]
        private uint mAnimStepCount_Final_Min = 2;
        [Header("快结束的慢速点数量，固定值<=0，则使用随机值，该值为随机的最大值")]
        [SerializeField]
        private uint mAnimStepCount_Final_Max = 5;
        [Header("一开始的匀速点数量占出去最后慢速点后的占比")]
        [SerializeField]
        private float mAnimStepCount_Constant_Scale = 0.5f;
        private int mAnimStepCount_Constant = 0;
        [Header("不同动画间的间隔时间")]
        [SerializeField]
        private float mTwoAnimInterval = 0.5f;
        private float mAnimTotalTime_Change = 0f;
        private Keyframe mAnimCurveFirstKey = new Keyframe();
        private Keyframe mAnimCurveLastKey = new Keyframe();

        //格子换置动画变参
        private Dictionary<int, ComRandomTreasureRaffleGridStep> mStepGridDic = new Dictionary<int, ComRandomTreasureRaffleGridStep>();
        private Coroutine mWaitAnimCor = null;
        private bool gridSelectAnimPlaying = false;
        public bool GridSelectAnimPlaying
        {
            get { return gridSelectAnimPlaying; }
        }

        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake()
        {
            gridContentSizeX = mGridX * mGridSize + (mGridX - 1) * mGridSpace;
            gridContentSizeY = mGridY * mGridSize + (mGridY - 1) * mGridSpace;
        }
        
        //Unity life cycle
        //void Start () 
        //{
        //}
        
        //Unity life cycle
        //void Update () 
        //{

        //}
        
        //Unity life cycle
        void OnDestroy () 
        {
            _ResetStepAnim();
            _ResetMoveAnim();

            raffleAnimStart = null;
            raffleAnimEnd = null;

            mGridSelectRect = null;
            mSelectGridPosVec2 = new Vector2();

            if (mGridPosList != null)
            {
                mGridPosList.Clear();
            }

            if (mGridGetItems != null)
            {
                for (int i = 0; i < mGridGetItems.Count; i++)
                {
                    var getItem = mGridGetItems[i];
                    if (getItem == null)
                    {
                        continue;
                    }
                    getItem.getItemTargetPos = new Vector2();
                    getItem.getItemInitPos = new Vector2();
                    getItem.getItemRect = null;
                    if (getItem.getItemIconTweens == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < getItem.getItemIconTweens.Count; j++)
                    {
                        var tween = getItem.getItemIconTweens[j];
                        if (tween == null)
                        {
                            continue;
                        }
                        tween.DOPause();
                        tween.DOKill();
                    }
                    getItem.getItemIconTweens = null;
                }
            }
            if (mMainGetItem != null)
            {
                ComItemManager.Destroy(mMainGetItem);
                mMainGetItem = null;
            }
            mMainGetItemData = null;
            if (mTotalGetItemDatas != null)
            {
                mTotalGetItemDatas.Clear();
            }
            if (mGetItemTweenQuene != null)
            {
                mGetItemTweenQuene.Pause();
                mGetItemTweenQuene.Kill();
                mGetItemTweenQuene = null;
            }

            bInited = false;
        }

        void _ResetStepAnim()
        {
            //注意重置位置和显示的顺序
            if (mGridSelectRect)
            {
                mGridSelectRect.anchoredPosition = mSelectGridPosVec2;
            }
            if (mGridSelect)
            {
                mGridSelect.CustomActive(false);
            }
            if (mStepGridDic != null)
            {
                mStepGridDic.Clear();
            }
            if (mWaitAnimCor != null)
            {
                StopCoroutine(mWaitAnimCor);
                mWaitAnimCor = null;
            }

            gridSelectAnimPlaying = false;
        }

        void _RestartStepAnim()
        {
            _ResetStepAnim();
            if (mGridSelect)
            {
                mGridSelect.CustomActive(true);
            }
        }

        void _ResetMoveAnim()
        {
            if (mGridGetItems != null)
            {
                for (int i = 0; i < mGridGetItems.Count; i++)
                {
                    var getItem = mGridGetItems[i];
                    if (getItem == null)
                    {
                        continue;
                    }
                    getItem.getItemGo.CustomActive(false);
                    getItem.getItemRect.anchoredPosition = getItem.getItemInitPos;
                }
            }
        }

        void _RestartMoveAnim()
        {
            if (mGridGetItems != null)
            {
                for (int i = 0; i < mGridGetItems.Count; i++)
                {
                    var getItem = mGridGetItems[i];
                    if (getItem == null)
                    {
                        continue;
                    }
                    getItem.getItemRect.anchoredPosition = getItem.getItemInitPos;
                    if (getItem.getItemIconTweens == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < getItem.getItemIconTweens.Count; j++)
                    {
                        var tween = getItem.getItemIconTweens[j];
                        if (tween == null)
                        {
                            continue;
                        }
                        tween.DORestart();
                    }
                    getItem.getItemGo.CustomActive(true);
                }
            }
        }

        /// <summary>
        /// 计算随机的抽奖点 总步数
        /// </summary>
        /// <param name="gridIndex"> 格子数 从0开始 因为就是道具id列表里的</param>
        /// 但是返回值 是从1开始的 正常情况 最小一格
        private int _CalAnimStepCount(int gridIndex)
        {
            if (mGridPosList == null)
            {
                return 0;
            }
            if (gridIndex > mGridPosList.Count)
            {
                Logger.LogErrorFormat("[ComRandomTreasureRaffleBoard] - _CalRanAnimStepCount outParam gridIndex:{0} > totalGridCount:{1}", gridIndex, mGridPosList.Count);
                return 0;
            }
            int totalStepCount = mGridPosList.Count + gridIndex + _InitFirstAnimStepIndex() ;
            return totalStepCount;
        }

        private int _InitFirstAnimStepIndex()
        {
            return 1;
        }

        private int _InitOneBoardGridCount()
        {
            if (mGridPosList == null)
            {
                return 0;
            }
            return mGridPosList.Count;
        }

        private void _ExcuteStepAnimEnd()
        {
            _ResetStepAnim();

            bool getItemDataNeedNotifyRecord = true;
            if (mMainGetItemData != null)
            {
                getItemDataNeedNotifyRecord = RandomTreasureDataManager.GetInstance().CheckGetItemDataNeedNotifyRecord(mMainGetItemData);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureRaffleEnd, true, getItemDataNeedNotifyRecord);

            //if (this.mMainGetItemData != null)
            //{
            //    RandomTreasureDataManager.GetInstance().SystemNotifyOnGetItem(this.mMainGetItemData);
            //}

            if (this.mTotalGetItemDatas != null)
            {
                for (int i = 0; i < mTotalGetItemDatas.Count; i++)
                {
                    var getItemData = mTotalGetItemDatas[i];
                    if (getItemData == null)
                    {
                        continue;
                    }
                    RandomTreasureDataManager.GetInstance().SystemNotifyOnGetItem(getItemData);
                }
            }
        }

        private void _ExcuteStepAnimStart()
        {
            _RestartStepAnim();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureRaffleStart);
        }

        private void _ExcuteMoveAnimEnd()
        {
            _ResetMoveAnim();
        }

        private void _ExcuteMoveAnimStart()
        {
            _RestartMoveAnim();
        }

        /// <summary>
        /// 触发所有动画结束
        /// </summary>
        private void _ExcuteAllAnimEnd()
        {
            _ExcuteStepAnimEnd();
            _ExcuteMoveAnimEnd();
        }

        private void _InitGridStepInfo(int stepIndex, int firstStepIndex, float stepInterval)
        {
            if (mGridPosList == null || mStepGridDic == null)
            {
                return;
            }

            Vector2 gridVec2 = new Vector2();
            int tempIndexToPos = stepIndex - firstStepIndex;
            if (tempIndexToPos >= mGridPosList.Count)
            {
                //Logger.LogError("StartSelectAnim step tempIndexToPos 111 : " + tempIndexToPos);
                tempIndexToPos = tempIndexToPos % mGridPosList.Count;
            }
            //Logger.LogError("StartSelectAnim step tempIndexToPos 222 : " + tempIndexToPos);
            gridVec2 = mGridPosList[tempIndexToPos];

            ComRandomTreasureRaffleGridStep grid = null;
            if (mStepGridDic.ContainsKey(stepIndex))
            {
                grid = mStepGridDic[stepIndex];
                if (grid == null)
                {
                    return;
                }
                grid.stepInterval = stepInterval;
                grid.stepIndex = stepIndex;
                grid.anchorVec2 = gridVec2;
            }
            else
            {
                grid = new ComRandomTreasureRaffleGridStep();
                grid.stepInterval = stepInterval;
                grid.stepIndex = stepIndex;
                grid.anchorVec2 = gridVec2;
                mStepGridDic.Add(stepIndex, grid);
            }
        }

        private void _ResetRaffleBoardData(int raffleItemId, int raffleItemCount, List<ItemSimpleData> otherShowItems)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(raffleItemId);
            itemData.Count = raffleItemCount;
            this.mMainGetItemData = itemData;
            ItemSimpleData raffleSItem = new ItemSimpleData();
            raffleSItem.ItemID = raffleItemId;
            raffleSItem.Count = raffleItemCount;
            raffleSItem.Name = itemData.Name;
            this.mTotalGetItemDatas = new List<ItemSimpleData>();
            this.mTotalGetItemDatas.Add(raffleSItem);
            this.mTotalGetItemDatas.AddRange(otherShowItems);
        }
		
		private void _ResetRandomFinalStepCount()
		{
		    //设置一个随机值
            mAnimStepCount_Final = UnityEngine.Random.Range((int)mAnimStepCount_Final_Min, (int)mAnimStepCount_Final_Max);
		}

        #endregion
        
        #region  PUBLIC METHODS

        public void InitView()
        {
            if (bInited)
            {
                return;
            }
            if (mGridPosList != null)
            {
                mGridPosList.Clear();
                //坐标系 左上角为原点 和 Grid ComUIList 排版一致 ！！！
                float offsetZero = mGridSize * 0.5f;
                Vector2 localPos = new Vector2();
                for (int y = 1; y <= mGridY; y++)
                {
                    for (int x = 1; x <= mGridX; x++)
                    {
                        localPos.y = -((y - 1) * mGridSize + offsetZero + (y - 1) * mGridSpace);
                        localPos.x = (x - 1) * mGridSize + offsetZero + (x - 1) * mGridSpace;
                        //Logger.LogError("RaffleBoardPos : "+localPos);
                        mGridPosList.Add(localPos);
                    }
                }
            }

            if (mAnimCurve != null && mAnimCurve.length > 0)
            {
                mAnimCurveFirstKey = mAnimCurve[0];
                mAnimCurveLastKey = mAnimCurve[mAnimCurve.length - 1];
                mAnimTotalTime_Change = mAnimCurveLastKey.time;
            }

            //默认最少动画步数
            int defaultStepCount = _CalAnimStepCount(0);
            
			//设一个默认值
            if (mAnimStepCount_Constant_Scale > 1 || mAnimStepCount_Constant_Scale <= 0)
            {
                mAnimStepCount_Constant_Scale = 0.5f;
            }
            //设一个默认值
            if (mAnimStepCount_Constant <= 0)
            {
                mAnimStepCount_Constant = (int)(defaultStepCount * mAnimStepCount_Constant_Scale);
            }
            //设置一个随机值
            if (mAnimStepCount_Final <= 0)
            {
                mAnimStepCount_Final = UnityEngine.Random.Range((int)mAnimStepCount_Final_Min, (int)mAnimStepCount_Final_Max);
            }

            if (mGridSelect)
            {
                mGridSelectRect = mGridSelect.GetComponent<RectTransform>();
                if (mGridSelectRect)
                    mSelectGridPosVec2 = mGridSelectRect.anchoredPosition;
            }

            //if (mGridSelectItemGos)
            //{
            //    mSelectItem = ComItemManager.Create(mGridSelectItemGos);
            //    mTweenToPack = mGridSelectItemGos.GetComponent<DOTweenAnimation>();
            //    if (mTweenToPack != null && mTweenToPack.onComplete != null)
            //    {
            //        mTweenToPack.onComplete.AddListener(_ExcuteAllAnimEnd);
            //    }
            //    mGridSelectItemRect = mGridSelectItemGos.GetComponent<RectTransform>();
            //    if (mGridSelectItemRect)
            //    {
            //        mGridSelectItemGoPosVec2 = mGridSelectItemRect.anchoredPosition;
            //    }
            //    mGridSelectItemGos.CustomActive(false);
            //}

            if (mGridGetItems != null)
            {
                for (int i = 0; i < mGridGetItems.Count; i++)
                {
                    var getItem = mGridGetItems[i];
                    if (getItem == null)
                    {
                        continue;
                    }
                    getItem.getItemInitPos = getItem.getItemRect.anchoredPosition;
                    getItem.getItemGo.CustomActive(false);
                }
            }

            if (mGetItemTweenQuene != null)
            {
                mGetItemTweenQuene.Kill();
            }
            mGetItemTweenQuene = DOTween.Sequence();

            _ResetMoveAnim();
            _ResetStepAnim();

            bInited = true;
        }

        public void StartSelectAnim(int raffleIndex, int raffleItemId, int raffleItemCount, List<ItemSimpleData> otherShowItems, bool bSkipAnim = false)
        {
			_ResetRandomFinalStepCount();
            _ResetRaffleBoardData(raffleItemId, raffleItemCount, otherShowItems);
            _ExcuteStepAnimStart();
            _ExcuteMoveAnimEnd();

            if (mAnimCurve == null)
            {
                _ExcuteStepAnimEnd();
                return;
            }
            int totalStep = _CalAnimStepCount(raffleIndex);
            //从1开始计步
            int firstStepIndex = _InitFirstAnimStepIndex();

            //动画规则：除去最后几步，剩余动画步数中，一开始匀速动画占比，然后进行减速动画
            mAnimStepCount_Constant = (int)((totalStep - mAnimStepCount_Final) * mAnimStepCount_Constant_Scale);
            if (totalStep <= 0)
            {
                return;
            }
            if (mAnimStepCount_Constant + mAnimStepCount_Final >= totalStep)
            {
                return;
            }
            float intervalTimeNor = mAnimCurveFirstKey.value;
            float intervalTimeFinal = mAnimCurveLastKey.value;
            float intervalTimeChange = mAnimTotalTime_Change / (totalStep - mAnimStepCount_Constant - mAnimStepCount_Final);
            for (int i = firstStepIndex; i <= totalStep; i++)
            {
                float stepInterval = 0f;
                if (i <= mAnimStepCount_Constant)
                {
                    stepInterval = intervalTimeNor * mAnimSlowSpeed;
                    Logger.LogProcessFormat("Nor !!! StartSelectAnim stepIndex : {0} ,  stepInterval : {1} ", i, stepInterval);
                }
                else if (i > totalStep - mAnimStepCount_Final)
                {
                    stepInterval = intervalTimeFinal * mAnimSlowSpeed;
                    Logger.LogProcessFormat("Reduce !!! StartSelectAnim stepIndex : {0} ,  stepInterval : {1} ", i, stepInterval);
                }
                else
                {
                    float intervalTimer = intervalTimeChange * ( i - mAnimStepCount_Constant );
                    stepInterval = mAnimCurve.Evaluate(intervalTimer / mAnimTotalTime_Change) * mAnimSlowSpeed;
                    Logger.LogProcessFormat("Changed !!! StartSelectAnim stepIndex : {0} ,  stepInterval : {1} ", i, stepInterval);
                }
                _InitGridStepInfo(i, firstStepIndex, stepInterval);
            }

            if (mWaitAnimCor != null)
            {
                StopCoroutine(mWaitAnimCor);              
            }
            mWaitAnimCor = StartCoroutine(_WaitToMoveSelectGrid(bSkipAnim == true));
        }

        IEnumerator _WaitToMoveSelectGrid(bool bDirectSelect = false)
        {
            //首先播放抽奖选择动画
            if (mStepGridDic == null)
            {
                _ExcuteStepAnimEnd();
                yield break;
            }

            //动画开始 ！！！
            gridSelectAnimPlaying = true;

            if (!bDirectSelect)
            {
                int mCurrStepIndex = _InitFirstAnimStepIndex();
                while (mStepGridDic.ContainsKey(mCurrStepIndex))
                {                   
                    var gridStep = mStepGridDic[mCurrStepIndex];
                    if (gridStep == null)
                    {
                        _ExcuteStepAnimEnd();
                        yield break;
                    }
                    if (gridStep.stepIndex != mCurrStepIndex)
                    {
                        _ExcuteStepAnimEnd();
                        yield break;
                    }
                    float currStepInterval = gridStep.stepInterval;
                    if (currStepInterval <= 0)
                    {
                        _ExcuteStepAnimEnd();
                        yield break;
                    }
                    yield return new WaitForSecondsRealtime(currStepInterval);
                    if (mGridSelectRect)
                    {
                        mGridSelectRect.anchoredPosition = gridStep.anchorVec2;
                    }
                    mCurrStepIndex++;
                    yield return null;
                }

                //延迟播放获取奖励的动画
                yield return new WaitForSecondsRealtime(mTwoAnimInterval);
            }
            else
            {
                int totalSelectAnimStepCount = mStepGridDic.Count;
                if (!mStepGridDic.ContainsKey(totalSelectAnimStepCount))
                {
                    Logger.LogErrorFormat("[ComRandomTreasureRaffleBoard] - _WaitToMoveSelectGrid skipSelectAnim is true, but mStepGridDic not contain the key {0}", totalSelectAnimStepCount);
                }
                var lastGridStep = mStepGridDic[totalSelectAnimStepCount];
                if (lastGridStep != null)
                {
                    if (mGridSelectRect)
                    {
                        mGridSelectRect.anchoredPosition = lastGridStep.anchorVec2;
                    }
                }
            }

            //if (mSelectItem != null && mGridSelectItemGos)
            //{
            //    if(this.mItemData == null)
            //    {
            //        Logger.LogError("[ComRandomTreasureRaffleBoard] - _WaitToMoveSelectGrid mItemData is null");
            //        yield break;
            //    }
            //    mSelectItem.Setup(this.mItemData, null);
            //    _ExcuteMoveAnimStart();
            //    if (mTweenToPack != null)
            //    {
            //        mTweenToPack.DORestart();
            //    }
            //    else
            //    {
            //        _ExcuteStepAnimEnd();
            //    }
            //}
            if (mTotalGetItemDatas == null)
            {
                _ExcuteStepAnimEnd();
                yield break;
            }
            if(mGridGetItems == null)   
            {
                _ExcuteStepAnimEnd();
                yield break;
            }
            if (mGridGetItems.Count != mTotalGetItemDatas.Count)
            {
                Logger.LogError("[ComRandomTreasureRaffleBoard] - _WaitToMoveSelectGrid : mGridGetItems.Length != mTotalGetItemDatas.Count");
                _ExcuteStepAnimEnd();
                yield break;
            }
            else
            {
                if (mGetItemTweenQuene == null)
                {
                    _ExcuteStepAnimEnd();
                    yield break;
                }
                for (int i = 0; i < mGridGetItems.Count; i++)
                {
                    var getItem = mGridGetItems[i];
                    if (getItem == null || getItem.getItemGo == null || getItem.getItemRect == null)
                    {
                        _ExcuteStepAnimEnd();
                        yield break;
                    }
                    var getItemData = mTotalGetItemDatas[i];
                    if (getItemData == null)
                    {
                        _ExcuteStepAnimEnd();
                        yield break;
                    }
                    RandomTreasureInfo info = getItem.getItemGo.GetComponent<RandomTreasureInfo>();
                    if (info == null)
                    {
                        _ExcuteStepAnimEnd();
                        yield break;
                    }
                    info.SetInfoTitleImg(ItemDataManager.GetInstance().GetOwnedItemIconPath(getItemData.ItemID));
                    //开始DOTween动画队列
                    if (i == mGridGetItems.Count - 1)
                    {
                        mGetItemTweenQuene.Insert(i, getItem.getItemRect.DOAnchorPos(getItem.getItemTargetPos, getItem.getItemTweenDuration)
                                .SetDelay(getItem.getItemTweenDelay)
                                .SetEase(getItem.getItemPosTweenEase)
                                .OnComplete(_ExcuteAllAnimEnd))
                                .SetAutoKill();
                    }
                    else
                    {
                        mGetItemTweenQuene.Insert(i, getItem.getItemRect.DOAnchorPos(getItem.getItemTargetPos, getItem.getItemTweenDuration)
                            .SetDelay(getItem.getItemTweenDelay)
                            .SetEase(getItem.getItemPosTweenEase))
                            .SetAutoKill();
                    }
                }
                _ExcuteMoveAnimStart();
                mGetItemTweenQuene.Restart();
            }
            yield return null;
        }

        #endregion
    }
}