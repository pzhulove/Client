using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    public class DailyTodoFrame : ClientFrame
    {
        #region MODEL PARAMS

        public static readonly string OPEN_LINK_INFO = "<type=framename value=GameClient.DailyTodoFrame>";

        private List<ComDailyTodoFunctionItem> tempFunctionItemList = new List<ComDailyTodoFunctionItem>();

        private UnityEngine.Coroutine waitToRefreshFuncViewOnAnimEnd;

        #endregion

        #region VIEW PARAMS

        #endregion

        public static void OpenLinkFrame()
        {
            try
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<DailyTodoFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<DailyTodoFrame>();
                }
                ClientSystemManager.GetInstance().OpenFrame<DailyTodoFrame>(FrameLayer.Middle);
            }
            catch (System.Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public static void CloseFrame()
        {
            try
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<DailyTodoFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<DailyTodoFrame>();
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        #region PRIVATE METHODS

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/DailyTodo/DailyTodoFrame";
        }

        protected override void _OnOpenFrame()
        {
            _BindUIEvent();

            DailyTodoDataManager.GetInstance().UpdateDailyTodoActivityList();

            _InitView();

            DailyTodoDataManager.GetInstance().ReqDailyTodoFunctionState();
        }

        protected override void _OnCloseFrame()
        {
            _UnInitView();
            _UnBindUIEvent();

            if (tempFunctionItemList != null)
            {
                tempFunctionItemList.Clear();
            }
            DailyTodoDataManager.GetInstance().ClearTempShowDailyTodoData();
        }      

        private void _InitView()
        {
            _InitActivityView();
            _InitFunctionView();
        }

        private void _UnInitView()
        {
            _UnInitActvityView();
            _UnInitFunctionView();
        }

        private void _InitActivityView()
        {
            if (mActivityCarousel != null)
            {
                mActivityCarousel.onBindItem += _OnActivityItemBindView;
                mActivityCarousel.onItemCreate += _OnActivityItemCreate;
                mActivityCarousel.onIndexChange += _OnActivityItemIndexChange;
            }

            _RefreshActivityItemView();
        }

        private void _UnInitActvityView()
        {
            if (mActivityCarousel != null)
            {
                mActivityCarousel.onBindItem -= _OnActivityItemBindView;
                mActivityCarousel.onItemCreate -= _OnActivityItemCreate;
                mActivityCarousel.onIndexChange -= _OnActivityItemIndexChange;
            }
        }

        private void _RefreshActivityItemView()
        {
            var tempActvityList = DailyTodoDataManager.GetInstance().GetShowDailyTodoActivityList();
            if (tempActvityList == null)
            {
                return;
            }
            if (mActivityCarousel != null)
            {
                mActivityCarousel.SetCellAmount(tempActvityList.Count);
            }
            if (mScrollDots != null)
            {
                mScrollDots.InitDots(tempActvityList.Count);
                if (mActivityCarousel != null)
                {
                    mScrollDots.SetDots(mActivityCarousel.CurrentIndex + 1);
                }
            }
        }

        private ComDailyTodoActivityItem _OnActivityItemBindView(GameObject go)
        {
            if (null == go || go.transform.childCount <= 0)
            {
                return null;
            }
            var firstChild = go.transform.GetChild(0);
            if (null == firstChild)
            {
                return null;
            }
            return firstChild.GetComponent<ComDailyTodoActivityItem>();
        }

        private void _OnActivityItemCreate(ComCarouselCell item)
        {
            if (null == item)
            {
                return;
            }
            var tempActvityList = DailyTodoDataManager.GetInstance().GetShowDailyTodoActivityList();
            if (null == tempActvityList || tempActvityList.Count <= 0)
            {
                return;
            }
            int i_Index = item.Index;
            var comActivityItem = item.BindScript as ComDailyTodoActivityItem;
            if (i_Index >= 0 && i_Index < tempActvityList.Count)
            {
                var activityModel = tempActvityList[i_Index];
                if (activityModel == null)
                {
                    return;
                }
                if (comActivityItem != null)
                {
                    comActivityItem.RefreshView(activityModel);
                }
            }
        }

        private void _OnActivityItemIndexChange(int index)
        {
            if (mScrollDots != null)
            {
                mScrollDots.SetDots(index + 1);
            }
        }

        private void _InitFunctionView()
        {
            if (mFuncScorllView != null)
            {
                if (mFuncScorllView.IsInitialised())
                {
                    return;
                }
                mFuncScorllView.Initialize();
                mFuncScorllView.onBindItem += _OnFunctionBindItem;
                mFuncScorllView.onItemVisiable += _OnFunctionVisable;
                mFuncScorllView.OnItemRecycle += _OnFunctionItemRecycle;
            }

            _RefreshFunctionItemView(false);
        }        

        private void _UnInitFunctionView()
        {
            if (mFuncScorllView != null)
            {
                mFuncScorllView.onBindItem -= _OnFunctionBindItem;
                mFuncScorllView.onItemVisiable -= _OnFunctionVisable;
                mFuncScorllView.OnItemRecycle -= _OnFunctionItemRecycle;
                mFuncScorllView.UnInitialize();
            }

            if (waitToRefreshFuncViewOnAnimEnd != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToRefreshFuncViewOnAnimEnd);
                waitToRefreshFuncViewOnAnimEnd = null;
            }
        }

        private void _RefreshFunctionItemView(bool needPlayAnim = true)
        {
            var tempFunctionList = DailyTodoDataManager.GetInstance().GetShowDailyTodoFunctionListByCount();
            if (tempFunctionList != null && mFuncScorllView != null)
            {
                mFuncScorllView.SetElementAmount(tempFunctionList.Count);
            }
            if (needPlayAnim)
            {
                _TryInvokeNearlyFunctionItemPlayAnim();
            }
        }

        private ComDailyTodoFunctionItem _OnFunctionBindItem(GameObject go)
        {
            if (null == go)
            {
                return null;
            }

            return go.GetComponent<ComDailyTodoFunctionItem>();
        }

        private void _OnFunctionVisable(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var tempFunctionList = DailyTodoDataManager.GetInstance().GetShowDailyTodoFunctionListByCount();
            int i_Index = item.m_index;
            ComDailyTodoFunctionItem funcItem = item.gameObjectBindScript as ComDailyTodoFunctionItem;
            if (i_Index >= 0 && i_Index < tempFunctionList.Count)
            {
                if (funcItem != null)
                {
                    funcItem.RefreshView(tempFunctionList[i_Index]);

                    if (tempFunctionItemList != null && !tempFunctionItemList.Contains(funcItem))
                    {
                        tempFunctionItemList.Add(funcItem);
                    }
                }
            }
        }

        private void _OnFunctionItemRecycle(ComUIListElementScript item)
        {
            if (null == item)
            {
                return;
            }
            ComDailyTodoFunctionItem funcItem = item.gameObjectBindScript as ComDailyTodoFunctionItem;
            if (null != funcItem)
            {
                funcItem.Recycle();

                if (tempFunctionItemList != null && tempFunctionItemList.Contains(funcItem))
                {
                    tempFunctionItemList.Remove(funcItem);
                }
            }
        }

        /// <summary>
        /// 尝试触发最近的item播放动画
        /// </summary>
        /// <returns></returns>
        private bool _TryInvokeNearlyFunctionItemPlayAnim()
        {
            if (null == tempFunctionItemList)
            {
                return false;
            }
            for (int i = 0; i < tempFunctionItemList.Count; i++)
            {
                var item = tempFunctionItemList[i];
                if (null == item)
                    continue;
                bool bStart = item.TryPlayAnim();
                if (bStart)
                {
                    return true;
                }
            }
            return false;
        }

        #region EVENT

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDailyTodoFuncStateUpdate, _OnDailyTodoFuncStateUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDailyTodoFuncPlayAnimEnd, _OnDailyTodoFuncPlayAnimEnd);
        }
        
        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDailyTodoFuncStateUpdate, _OnDailyTodoFuncStateUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDailyTodoFuncPlayAnimEnd, _OnDailyTodoFuncPlayAnimEnd);
        }

        private void _OnDailyTodoFuncStateUpdate(UIEvent uiEvent)
        {
            _RefreshFunctionItemView();
        }

        private void _OnDailyTodoFuncPlayAnimEnd(UIEvent uiEvent)
        {
            if (null == uiEvent || null == uiEvent.Param1)
            {
                return;
            }

            var extraAnimParam = uiEvent.Param1 as ComDailyTodoFunctionExtraAnimParam;
            if (null == extraAnimParam)
            {
                return;
            }

            //当刷新下一个最近item不需要播放动画时
            //刷新整个界面
            if (_TryInvokeNearlyFunctionItemPlayAnim() == false)
            {
                if (waitToRefreshFuncViewOnAnimEnd != null)
                {
                    GameFrameWork.instance.StopCoroutine(waitToRefreshFuncViewOnAnimEnd);
                }
                waitToRefreshFuncViewOnAnimEnd = GameFrameWork.instance.StartCoroutine(_WaitToRefreshFuncViewOnAnimEnd(extraAnimParam.finishTagEndWaitingTime));
            }
        }

        IEnumerator _WaitToRefreshFuncViewOnAnimEnd(float waitTime)
        {
            yield return Yielders.GetWaitForSeconds(waitTime);
            _RefreshFunctionItemView();
        }

        #endregion

		#region ExtraUIBind
		private Button mBtnClose = null;
		private ComCarouselView mActivityCarousel = null;
		private ComUIListScript mFuncScorllView = null;
		private ComDotController mScrollDots = null;
		
		protected override void _bindExUI()
		{
			mBtnClose = mBind.GetCom<Button>("BtnClose");
			if (null != mBtnClose)
			{
				mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
			}
			mActivityCarousel = mBind.GetCom<ComCarouselView>("ActivityCarousel");
			mFuncScorllView = mBind.GetCom<ComUIListScript>("FuncScorllView");
			mScrollDots = mBind.GetCom<ComDotController>("ScrollDots");
		}
		
		protected override void _unbindExUI()
		{
			if (null != mBtnClose)
			{
				mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
			}
			mBtnClose = null;
			mActivityCarousel = null;
			mFuncScorllView = null;
			mScrollDots = null;
		}
		#endregion

        #region Callback
        private void _onBtnCloseButtonClick()
        {
            this.Close();
        }
        #endregion

        #endregion
    }
}
