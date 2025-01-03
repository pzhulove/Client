using DataModel;
using Protocol;
using ProtoTable;
using Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        public interface IActivityTreasureLotteryActivityViewBase : IDisposable
        {
            void UpdateData();
        }

        /// <summary>
        /// 夺宝活动分页view的基类
        /// 绑定数据管理器，初始化scroll rect
        /// 绑定ComUIListScript的事件
        /// </summary>
        /// <typeparam name="T">scroll rect里面的item的脚本类</typeparam>
        public abstract class ActivityTreasureLotteryActivityViewBase<T> : MonoBehaviour, IActivityTreasureLotteryActivityViewBase where T: IActivityTreasureLotteryModelBase
        {
            public int SelectId { get { return mSelectId; } }

            //当前选中的id;
            protected int mSelectId = 0;
            protected int mSelectLotteryId = -1;

            protected ComUIListScript mScrollList = null;
            protected IActivityTreasureLotteryDataMananger mDataManager;
            protected IActivityTreasureLotteryItem mSelectedItem;
            public void Dispose()
            {
                UnBindEvents();
                gameObject.CustomActive(false);
                mSelectId = 0;
                mSelectLotteryId = -1;
                if(mSelectedItem != null)
                {
                    mSelectedItem = null;
                }
                OnDispose();
            }
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="dataManager">数据管理器</param>
            /// <param name="itemPrefabPath">item的prefab路径</param>
            /// <param name="scrollList">滚动控件</param>
            public void Init(IActivityTreasureLotteryDataMananger dataManager, string itemPrefabPath, ComUIListScript scrollList)
            {
                mScrollList = scrollList;
                mDataManager = dataManager;
                BindEvents();
                if (mScrollList != null)
                {
                    mScrollList.InitialLizeWithExternalElement(itemPrefabPath);
                    mScrollList.SetElementAmount(0);
                    mScrollList.ResetContentPosition();

	                //因为3个分页用的同一个scroll list, 而scroll list内部的selectedId是同一个变量
	                //如果在其他分页选中了id为x的item,切到下一个分页时刚好mSelectId也是x,scroll list内部是不处理的
	                //所以先select -1 重置一下
	                mScrollList.SelectElement(-1);
                }
                OnInit();
                UpdateData();
            }

            public void UpdateWnd(IActivityTreasureLotteryDataMananger dataManager, ComUIListScript scrollList)
            {
                mScrollList = scrollList;
                mDataManager = dataManager;
                if (mScrollList != null)
                {
                    //因为3个分页用的同一个scroll list, 而scroll list内部的selectedId是同一个变量
                    //如果在其他分页选中了id为x的item,切到下一个分页时刚好mSelectId也是x,scroll list内部是不处理的
                    //所以先select -1 重置一下
                    mScrollList.SelectElement(-1);
                }
                OnInit();
                UpdateData();
            }

            public virtual void UpdateData()
            {
                if (mSelectLotteryId != -1 && mDataManager != null)
                {
                    mSelectId = mDataManager.GetModelIndexByLotteryId<T>(mSelectLotteryId);
                }

	            if (mScrollList != null)
	            {
					if (mDataManager != null)
                    {
	                    int count = mDataManager.GetModelAmount<T>();
                        mScrollList.UpdateElementAmount(count);
	                    if (count > 0)
	                    {
		                    mScrollList.SelectElement(mSelectId, true);
	                    }
                    }

				}
				gameObject.CustomActive(true);
            }

            protected virtual void OnInit() { }

            protected virtual void OnDispose() { }

            /// <summary>
            /// 刷新界面数据
            /// </summary>
            /// <param name="data">IActivityTreasureLotteryModelBase，数据模型</param>
            protected abstract void OnSelectItem(T data);

            protected virtual void BindEvents()
            {
                if (mScrollList != null)
                {
                    mScrollList.onItemSelected += OnItemSelected;
                    mScrollList.onItemVisiable += OnItemVisiable;
	                mScrollList.OnItemUpdate += OnItemVisiable;
					mScrollList.onItemChageDisplay += OnItemChangeDisplayDelegate;
                }
			}

            protected virtual void UnBindEvents()
            {
                if (mScrollList != null)
                {
                    mScrollList.onItemSelected -= OnItemSelected;
                    mScrollList.onItemVisiable -= OnItemVisiable;
	                mScrollList.OnItemUpdate -= OnItemVisiable;
	                mScrollList.onItemChageDisplay -= OnItemChangeDisplayDelegate;

                }
			}

	        protected virtual void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
	        {
		        if (item == null)
		        {
			        return;
		        }

		        if (mDataManager != null)
		        {
			        IActivityTreasureLotteryItem lotteryItem = item.GetComponent<IActivityTreasureLotteryItem>();
			        if (lotteryItem != null)
			        {
				        lotteryItem.OnSelect(bSelected);
			        }
		        }
	        }

			protected virtual void OnItemVisiable(ComUIListElementScript item)
            {
                if (item == null)
                {
                    return;
                }

                if (mDataManager != null)
                {
                    IActivityTreasureLotteryItem lotteryItem = item.GetComponent<IActivityTreasureLotteryItem>();
                    T model = mDataManager.GetModel<T>(item.m_index);
                    if (lotteryItem != null)
                    {
                        lotteryItem.Init(model, mSelectId == item.m_index);
                    }
                }
            }

			protected virtual void OnItemSelected(ComUIListElementScript item)
            {
                if (item == null)
                {
                    return;
                }

                if (mSelectedItem != null && mSelectedItem != item.GetComponent<IActivityTreasureLotteryItem>())
                {
                    mSelectedItem.OnSelect(false);
                }
                mSelectId = item.m_index;
                if (mDataManager != null)
                {
                    OnSelectItem(mDataManager.GetModel<T>(item.m_index));
                    mSelectedItem = item.GetComponent<IActivityTreasureLotteryItem>();
                    if (mSelectedItem != null)
                    {
                        mSelectedItem.OnSelect(true);
                    }
                }
            }
        }
    }
}