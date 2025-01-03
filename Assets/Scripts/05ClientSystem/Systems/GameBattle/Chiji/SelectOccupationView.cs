using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using System;

namespace GameClient
{
    public class SelectOccupationView : MonoBehaviour,IDisposable
    {
        [SerializeField]private ComUIListScript mRolesUIList;
        [SerializeField]private Text mTimer;
        [SerializeField]private string mTimerDesc="{0}s";
        [SerializeField]private Slider mSlider;
        [Space(5)]
        [Header("倒计时长")]
        [SerializeField]private int mDuration = 15;
        [SerializeField]private int fSliderTime = 15;

        private List<int> mJobsList;
        private OnSelectJobClick mOnSelectJobClick;
        private bool isInitialize = false;
        private float timer = 0;
        private float sliderTimer = 0;
        private void Awake()
        {
            if (mRolesUIList != null)
            {
                mRolesUIList.Initialize();
                mRolesUIList.onBindItem += OnBindItemDelegate;
                mRolesUIList.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void Update()
        {
            if (isInitialize)
            {
                timer += Time.deltaTime;
                if (timer >= 1.0f)
                {
                    mDuration = mDuration - (int)timer;
                    UpdateTimer();
                    timer = 0.0f;
                }

                sliderTimer += Time.deltaTime;

                if (mSlider != null)
                {
                    mSlider.value = 1.0f - sliderTimer / fSliderTime;
                }

                if (mDuration <= 0)
                {
                    if (mOnSelectJobClick != null)
                    {
                        if (mJobsList.Count > 0)
                        {
                            mOnSelectJobClick.Invoke(mJobsList[0]);
                        }
                    }
                    isInitialize = false;
                }
            }
        }

        private RoleItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<RoleItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var mRoleItem = item.gameObjectBindScript as RoleItem;
            if (mRoleItem == null) return;

            if (item.m_index >= 0 && item.m_index < mJobsList.Count)
            {
                mRoleItem.OnItemVisiable(mJobsList[item.m_index], mOnSelectJobClick, item.m_index);
            }
        }

        public void InitView(List<int> jobList, OnSelectJobClick onSelectJobClick)
        {
            isInitialize = true;
            mJobsList = new List<int>();
            mJobsList = jobList;
            mOnSelectJobClick = onSelectJobClick;

            UpdateTimer();
            mRolesUIList.SetElementAmount(mJobsList.Count);
        }

        private void UpdateTimer()
        {
            if (mTimer != null)
            {
                mTimer.text = string.Format(mTimerDesc, mDuration);
            }
        }

        public void Dispose()
        {
            if (mRolesUIList != null)
            {
                mRolesUIList.onBindItem -= OnBindItemDelegate;
                mRolesUIList.onItemVisiable -= OnItemVisiableDelegate;
            }
            mRolesUIList = null;
            mJobsList.Clear();
            mOnSelectJobClick = null;
            isInitialize = false;
            timer = 0;
            sliderTimer = 0;
    }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

