using UnityEngine;
using System.Collections.Generic;
using ActivityLimitTime;
using UnityEngine.UI;
using DG.Tweening;
using ProtoTable;
using ActivityLimitTime;

namespace GameClient
{
    public enum ExpandButtonType
    {
        Welfare = 0,
        Jar,
        TreasureLotteryActivity,
        FirstRechargeActivity,

        TotalNum,
    }

    public enum RedPointImg
    {
        Welfare = 0,
        Jar,
        OnlineGift,
        TotalNum,
    }

    public enum HaveTwoLevelPermanent
    {
        ActivityLimittimeFrame =0,
        ActiveSevenDays,
        TotalNum,
    }

    public enum UnPermanentHaveTwoLevelErmanent
    {
        FirstRechargeActivity,
        TotalNum,
    }


    class ComTopButtonExpandController : MonoBehaviour
    {
        public bool bExpanding = false;

        // 伸展收缩对子节点在prefab里的摆放顺序没有要求,伸缩距离取决于List里元素的先后顺序
        
        public List<RectTransform> FirstBtnList = null;//第一排按钮的list
        public List<RectTransform> SecendBtnList = null;// 第二排按钮的list
        
        public List<RectTransform> UnPermanentBtnList = null; // 非常驻按钮的集合
        public List<RectTransform> HaveatwolevelermanentBtnList = null;  // 常驻按钮拥有二级层级的按钮
        public List<RectTransform> UnPermanentHaveTwoLevelErmanentBtnList = null;//非常驻按钮拥有二级节点的按钮
        
        public List<Image> RePointImg = null;   //魔罐、福利的红点特殊处理
        //public List<Image> RePointImage = null; //领取在线奖励、等级礼包的红点显示判断主按钮的红点显示的list
        
        public float OpenTopRightGoTime;
        public int ISevenActiveConfigID;

        public GameObject ParentIcon = null;
        public GameObject RePointGo = null;
        public GameObject LeftImg = null;
        public GameObject RightImg = null;
        public GameObject topRightGo = null;
        public GameObject topRight2Go = null;




        void OnDestroy()
        {
            FirstBtnList = null;
            SecendBtnList = null;
            UnPermanentBtnList = null;
            HaveatwolevelermanentBtnList = null;
            UnPermanentHaveTwoLevelErmanentBtnList = null;
            ParentIcon = null;
            RePointImg = null;
            RePointGo = null;
            LeftImg = null;
            RightImg = null;
            topRightGo = null;
            topRight2Go = null;

            bStartAnimation = false;
            Btnindex = 0;
            ActiveManager.GetInstance().onAddMainActivity -= _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= _OnUpdateMainActivity;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshActivityLimitTimeBtn, OnUpdateActivityLimitTimeBtn);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateBossActivityState, OnUpdataUnPermenentTwoLevelButton);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateUnlockFunc, OnUpdataUnPermenentTwoLevelButton);

            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }

        // Use this for initialization
        void Start()
        {
            ActiveManager.GetInstance().onAddMainActivity += _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += _OnUpdateMainActivity;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshActivityLimitTimeBtn, OnUpdateActivityLimitTimeBtn);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateBossActivityState, OnUpdataUnPermenentTwoLevelButton);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateUnlockFunc, OnUpdataUnPermenentTwoLevelButton);

            if (!CanGoOn())
            {
                return;
            }

            bExpanding = false;

            
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;

            //UpdateShowHideBtnState();
        }

        public void StartExpand(bool bExpand)
        {
            
            if (!CanGoOn())
            {
                return;
            }

            bExpanding = bExpand;

            CloseTopRightTopRight2GameObject();
            AnimationBefore();

            Invoke("OpenTopRightTopRight2GameObject", OpenTopRightGoTime);
            
            {
                StartMainUIIconAnimation(FirstBtnList,SecendBtnList);
            }
        }

        void AnimationBefore()
        {
            for (int i = 0; i < UnPermanentBtnList.Count; i++)
            {
                if (bExpanding)
                {
                    UnPermanentBtnList[i].gameObject.CustomActive(false);
                }
                else
                {
                    UpdateTopRightButtons();
                }
            }

            RefreshPermenentTwoLevelButton();

            MainButtonState();
        }

        public void UpdateTopRightState(bool IsExpand)
        {
            bExpanding = IsExpand;

            AnimationBefore();
        }

        void CloseTopRightTopRight2GameObject()
        {
            topRightGo.CustomActive(false);
            topRight2Go.CustomActive(false);
        }

        void OpenTopRightTopRight2GameObject()
        {
            topRightGo.CustomActive(true);
            topRight2Go.CustomActive(true);
        }
   

        public void  MainButtonState()
        {
            if (bExpanding)
            {
                LeftImg.CustomActive(bExpanding);
                RightImg.CustomActive(!bExpanding);
                RePointGo.CustomActive(IsRePointShow());
            }
            else
            {
                LeftImg.CustomActive(bExpanding);
                RightImg.CustomActive(!bExpanding);

                RePointGo.CustomActive(false);
            }

            UpdateShowHideBtnState();
        }
        

        bool CanGoOn()
        {
            if (FirstBtnList == null || ParentIcon == null|| SecendBtnList == null)
            {
                return false;
            }

            return true;
        }
        bool IsRePointShow()
        {
            int num = 0;

            if (bExpanding)
            {
             
                for (int i = 0; i < RePointImg.Count; i++)
                {
                    if (RePointImg[i].gameObject.activeSelf && IsShowRedPoint(i))
                    {
                        num++;
                    }
                }
            }

            if (num > 0)
            {
                return true;
            }

            return false;
        }

        public bool IsExpand()
        {
            return !bExpanding;
        }


        bool IsShowRedPoint(int Iindex)
        {
            if (Iindex==(int)RedPointImg.Welfare)
            {
                return Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Welfare);
            }
            else if (Iindex == (int)RedPointImg.Jar)
            {
                return Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Jar);
            }
            else if (Iindex == (int)RedPointImg.OnlineGift)
            {
                return Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.OnLineGift);
            }

            return true;
        }

        public bool IsShowUnPermenentButton(ExpandButtonType btnType)
        {
            //if(btnType == ExpandButtonType.ActivityJar)
            //{
            //    return _isActivityJarBtn();
            //}
            if(btnType == ExpandButtonType.Welfare)
            {
                return Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Welfare);
            }
            else if (btnType==ExpandButtonType.Jar)
            {
                return _IsMagicJarBtn();
            }
            else if (btnType == ExpandButtonType.FirstRechargeActivity)
            {
                return _isFirstRechargeActivityBtn();
            }
            else if (btnType == ExpandButtonType.TreasureLotteryActivity)
            {
                return _isTreasureLotteryActivityBtn();
            }

            return true;
        }

       

        public bool IsShowPermenentButton(HaveTwoLevelPermanent HTLP)
        {
            if (HTLP == HaveTwoLevelPermanent.ActivityLimittimeFrame)
            {
                return _isActivityLimittimeFrameBtn();
            }
            else if (HTLP == HaveTwoLevelPermanent.ActiveSevenDays)
            {
                return _isActiveSevenDaysBtn();
            }

            return true;
        }


        bool IsShowUnPermenentTwoLevelButton(UnPermanentHaveTwoLevelErmanent type)
        {
            if (type == UnPermanentHaveTwoLevelErmanent.FirstRechargeActivity)
            {
                return _isFirstRechargeActivityBtn();
            }


            return true;
        }

        bool _isActivityJarBtn()
        {
#if APPLE_STORE
            //IOS屏蔽功能 
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR))
            {
                return false;
            }
#endif     

            if (Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.ActivityJar) && JarDataManager.GetInstance().HasActivityJar())
            {
                return true;
            }
            return false;
        }

        bool _IsMagicJarBtn()
        {
            //return false; //暂时屏蔽 by chenhangjie
#if APPLE_STORE
            //IOS屏蔽功能 
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR))
            {
                return false;
            }
#endif     
            return Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Jar) || (Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.ActivityJar) && JarDataManager.GetInstance().HasActivityJar());
        }

        bool _isActivityLimittimeFrameBtn()
        {
#if APPLE_STORE
            //IOS屏蔽功能 
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_ACTIVITY))
            {
                return false;
            }
#endif

            return ActivityManager.GetInstance().IsHaveAnyActivity();
        }

        bool _isActiveSevenDaysBtn()
        {

#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SEVEN_AWARDS))
            {
                return false;
            }
#endif

            if (Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.ActivitySevenDays) && SevenDaysButtonIsShow())
            {
                return true;
            }

            return false;
        }
        bool _isFirstRechargeActivityBtn()
        {
            if (Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.FirstReChargeActivity) && (PayManager.GetInstance().HasFirstPay() || PayManager.GetInstance().HasConsumePay()))
            {
                return true;
            }
            return false;
        }

        bool _isTreasureLotteryActivityBtn()
        {
            var state = ActivityTreasureLotteryDataManager.GetInstance().GetState();
            //活动按钮
            if (state == ETreasureLotterState.Open || state == ETreasureLotterState.Prepare)
            {
                return true;
            }

            return false;
        }
        
        //刷新常驻拥有二级节点的按钮
        public void RefreshPermenentTwoLevelButton()
        {
            for (int i = 0; i < (int)HaveTwoLevelPermanent.TotalNum; i++)
            {
                HaveatwolevelermanentBtnList[i].gameObject.CustomActive(IsShowPermenentButton((HaveTwoLevelPermanent)i));
            }
        } 
        //刷新非常驻拥有二级节点的按钮
        void RefreshUnPermenentTwoLevelButton()
        {
            for (int i = 0; i < (int)UnPermanentHaveTwoLevelErmanent.TotalNum; i++)
            {
                UnPermanentHaveTwoLevelErmanentBtnList[i].gameObject.CustomActive(IsShowUnPermenentTwoLevelButton((UnPermanentHaveTwoLevelErmanent)i));
            }
        }

        //更新右上角所用非常驻按钮的显示
        public void UpdateTopRightButtons()
        {
            for(int i = 0; i < (int)ExpandButtonType.TotalNum; i++)
            {
                UnPermanentBtnList[i].CustomActive(IsShowUnPermenentButton((ExpandButtonType)i));
            }
        }
       
       
        bool SevenDaysButtonIsShow()
        {
            return SevendaysDataManager.GetInstance().IsSevenDaysActiveOpen();
            
        }

        public void UpdateShowHideBtnState()
        {
            if(topRightGo == null || topRight2Go == null)
            {
                return;
            }

            for(int i = 0;i < topRightGo.transform.childCount;i++)
            {
                if(topRightGo.transform.GetChild(i).gameObject != ParentIcon && topRightGo.transform.GetChild(i).gameObject.activeSelf)
                {
                    ParentIcon.CustomActive(true);
                    return;
                }
            }

            for (int i = 0; i < topRight2Go.transform.childCount; i++)
            {
                if (topRight2Go.transform.GetChild(i).gameObject.activeSelf)
                {
                    ParentIcon.CustomActive(true);
                    return;
                }
            }

            ParentIcon.CustomActive(false);
        }

        public float cMainUIIconHeight = 110f;
        public float cMainUIIconStartWidth = 60f;
        public float cMainUIIconEffectWidth = 180f;
        public float cMainUIIconTargetWidth = 110f;

        public float m_fSpeed = 0.08f;
        float timeflow = 0.0f;
        int Btnindex = 0;
        bool bStartAnimation = false;
        public float AlphaStart;
        public float AlphaEnd;

        void ResetMainUIIconTransform(List<RectTransform> btnList,float width)
        {
            if(btnList == null)
            {
                return;
            }

            for(int i = 0; i < btnList.Count; ++i)
            {
                var current = btnList[i];

                if(current)
                {
                    current.sizeDelta = new Vector2(width, cMainUIIconHeight);
                    var group = current.GetComponent<CanvasGroup>();
                    if(group)
                    {
                        group.alpha = AlphaStart;
                    }
                }
            }
        }


        List<RectTransform> currentMainList = new List<RectTransform>();
        List<RectTransform> currentMainList2 = new List<RectTransform>();

        void StartMainUIIconAnimation(List<RectTransform> btnFirstList, List<RectTransform> btnSecendList)
        {
            currentMainList.Clear();
            currentMainList2.Clear();
            for (int i = 0; i < btnFirstList.Count; ++i)
            {
                var current = btnFirstList[i];

                if (current && current.gameObject.activeSelf)
                {
                    currentMainList.Add(current);
                }
            }

            for (int i = 0; i < btnSecendList.Count; ++i)
            {
                var current = btnSecendList[i];

                if (current && current.gameObject.activeSelf)
                {
                    currentMainList2.Add(current);
                }
            }

            ResetMainUIIconTransform(currentMainList, cMainUIIconStartWidth);
            ResetMainUIIconTransform(currentMainList2, cMainUIIconStartWidth);
            bStartAnimation = true;
            Btnindex = 0;
            timeflow = 0;
        }
        
        public AnimationCurve mCurve;
        public bool bUseCurve = false;
        void Update()
        {
            if(bStartAnimation)
            {
                timeflow +=Time.deltaTime ;

                //if(timeflow >= intrval)
                {
                    int currentIndex = Btnindex;
                    int preIndex = Btnindex - 1;

                    float t = timeflow / m_fSpeed;

                    if(bUseCurve && mCurve != null)
                    {
                        t = mCurve.Evaluate(t);
                    }
                    else
                    {
                        t = Mathf.Sin(t * Mathf.PI / 2.0f);
                        t = Mathf.Sqrt(t);
                    }

                    if (currentIndex >= 0 && currentIndex < currentMainList.Count)
                    {
                        RectTransform child = currentMainList[currentIndex];
                        child.sizeDelta = new Vector2(Mathf.Lerp(cMainUIIconStartWidth,cMainUIIconEffectWidth, t), cMainUIIconHeight);
                        var group = child.GetComponent<CanvasGroup>();
                        if (group)
                        {
                            group.alpha = Mathf.Lerp(AlphaStart, AlphaEnd, t);
                        }
                    }

                    if (preIndex >= 0 && preIndex < currentMainList.Count)
                    {
                        RectTransform child = currentMainList[preIndex];
                        child.sizeDelta = new Vector2(Mathf.Lerp(cMainUIIconEffectWidth, cMainUIIconTargetWidth, t), cMainUIIconHeight);
                    }


                    if (currentIndex >= 0 && currentIndex < currentMainList2.Count)
                    {
                        RectTransform child = currentMainList2[currentIndex];
                        child.sizeDelta = new Vector2(Mathf.Lerp(cMainUIIconStartWidth, cMainUIIconEffectWidth, t), cMainUIIconHeight);

                        var group = child.GetComponent<CanvasGroup>();
                        if (group)
                        {
                            group.alpha = Mathf.Lerp(AlphaStart, AlphaEnd, t);
                        }
                    }

                    if (preIndex >= 0 && preIndex < currentMainList2.Count)
                    {
                        RectTransform child = currentMainList2[preIndex];
                        child.sizeDelta = new Vector2(Mathf.Lerp(cMainUIIconEffectWidth, cMainUIIconTargetWidth, t), cMainUIIconHeight);
                    }

                    if (timeflow >= m_fSpeed) 
                    {
                        Btnindex++;
                        timeflow = 0;

                        if (Btnindex > currentMainList.Count && Btnindex > currentMainList2.Count)
                        {
                            bStartAnimation = false;                   
                        }
                    }
                }
            }
        }
        //更新常驻拥有二级节点的按钮，七日活动
        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == ISevenActiveConfigID)
            {
                RefreshPermenentTwoLevelButton();
            }
        }
        //更新常驻拥有二级节点的按钮，七日活动
        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == ISevenActiveConfigID)
            {
                RefreshPermenentTwoLevelButton();
            }
        }
        //更新常驻拥有二级节点的按钮，七日活动
        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == ISevenActiveConfigID)
            {
                RefreshPermenentTwoLevelButton();
            }
        }
        //更新常驻拥有二级节点的按钮，限时活动
        void OnUpdateActivityLimitTimeBtn(UIEvent iEvent)
        {
            RefreshPermenentTwoLevelButton();
        }

        //更新非常驻拥有二级节点的按钮
        void OnUpdataUnPermenentTwoLevelButton(UIEvent iEvent)
        {
            RefreshUnPermenentTwoLevelButton();
            RefreshPermenentTwoLevelButton();
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            RefreshUnPermenentTwoLevelButton();
        }

    }
}