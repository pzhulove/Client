using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BattleBuffTipsFrame : ClientFrame
    {
        #region ExtraUIBind
        private Button mCloseButton;
        private RectTransform mContent;
        private BuffTipsScrollRectHandle scrollRectHandle;
        private GameObject mTipsParent;
        private VerticalLayoutGroup mTipsParentVertical;

        protected override void _bindExUI()
        {
            mCloseButton = mBind.GetCom<Button>("CloseBtn");
            if(mCloseButton != null)
            {
                mCloseButton.onClick.AddListener(_onCloseBtnClick);
            }
            mContent = mBind.GetCom<RectTransform>("Content");
            scrollRectHandle = mBind.GetCom<BuffTipsScrollRectHandle>("ScrollRectHandle");
            mTipsParent = mBind.GetGameObject("TipsParent");
            if(mTipsParent!= null)
            {
                mTipsParentVertical = mTipsParent.GetComponent<VerticalLayoutGroup>();
            }
        }

        protected override void _unbindExUI()
        {
            if(mCloseButton != null)
            {
                mCloseButton.onClick.AddListener(_onCloseBtnClick);
            }
            mCloseButton = null;
            mContent = null;
            scrollRectHandle = null;
            mTipsParent = null;
            mTipsParentVertical = null;
        }
        #endregion
        
        private DungeonBuffDisplayFrame buffDisplayFrame;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleBuffTips";
        }

        protected sealed override void _OnLoadPrefabFinish()
        {
            if (null == mComClienFrame)
            {
                mComClienFrame = frame.AddComponent<ComClientFrame>();
            }
            mComClienFrame.SetGroupTag("system");
        }

        protected override void _OnOpenFrame()
        {
            object[] dataArr = (object[])userData;
            if(dataArr== null || dataArr.Length != 2)
            {
                return;
            }
            buffDisplayFrame = dataArr[0] as DungeonBuffDisplayFrame;
            if(buffDisplayFrame == null)
            {
                return;
            }
            var position = (Vector3)dataArr[1];
            mContent.position = position;

            mTipsComBindList.Clear();
        }
        
        private int buffCount = -1;
        protected override void _OnUpdate(float timeElapsed)
        {
            //这部分更新不应太频繁 分隔开
            if (buffDisplayFrame.IsInited)
            {
                if (buffCount != buffDisplayFrame.GetBuffTipsCount())
                {
                    buffCount = buffDisplayFrame.GetBuffTipsCount();
                    _ResetTipsRect();
                    if(mTipsParentVertical != null)
                    {
                        mTipsParentVertical.enabled = false;
                    }
                }
                _UpdateAllTips(buffCount);
                if (mTipsParentVertical != null && !mTipsParentVertical.enabled) 
                {
                    mTipsParentVertical.enabled = true;
                }
                if (buffDisplayFrame.GetValidBuffCount() == 0)
                {
                    Close();
                }
            }
            if (scrollRectHandle != null)
            {
                if (scrollRectHandle.mPointerUpFlag)
                {
                    if (scrollRectHandle.mPointerDownFlag && !scrollRectHandle.mBeginDragFlag)
                    {
                        scrollRectHandle.ResetFlag();
                        Close();
                    }
                    else
                    {
                        scrollRectHandle.ResetFlag();
                    }
                }
            }
        }
        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            if(mTipsComBindList != null)
            {
                for (int i = 0; i < mTipsComBindList.Count; ++i)
                {
                    DestroyTip(mTipsComBindList[i]);
                }
            }
            buffDisplayFrame.CloseBuffTipListUpdate();
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        List<ComCommonBind> mTipsComBindList = new List<ComCommonBind>();

        private void _UpdateAllTips(int count)
        {
            if(mTipsComBindList == null)
            {
                return;
            }
            
            int sub = count - mTipsComBindList.Count;
            if (sub >= 0)
            {
                for (int i = 0; i < mTipsComBindList.Count; ++i)
                {
                    var combind = mTipsComBindList[i];
                    _SetComBindGameObjectActive(true, combind);
                }
                if (sub > 0)//缺少则补充
                {
                    for (int i = 0; i < sub; ++i)
                    {
                        var tip = CreateTips(mTipsParent);
                        mTipsComBindList.Add(tip);
                    }
                }
            }
            else //有多余
            {
                for(int i = 0; i < mTipsComBindList.Count; ++i)
                {
                    var combind = mTipsComBindList[i];
                    if (i < count)
                    {
                        _SetComBindGameObjectActive(true, combind);
                    }
                    else
                    {
                        _SetComBindGameObjectActive(false, combind);
                    }
                }
            }

            for(int i =0; i < mTipsComBindList.Count; ++i)
            {
                _UpdateOneTipByIndex(i);
            }
        }

        private void _SetComBindGameObjectActive(bool active,ComCommonBind bind)
        {
            if (bind != null && bind.gameObject.activeSelf != active)
            {
                bind.gameObject.SetActive(active);
            }
        }

        private void _UpdateOneTipByIndex(int index)
        {
            ComCommonBind mBind = mTipsComBindList[index];
            if (mBind == null)
            {
                return;
            }
            var infoTip = buffDisplayFrame.GetBuffTipsByIndex(index);
            if (infoTip == null)
            {
                return;
            }
            Image icon = mBind.GetCom<Image>("Icon");
            Text name = mBind.GetCom<Text>("Name");
            Text leftTime = mBind.GetCom<Text>("LeftTime");
            Text info = mBind.GetCom<Text>("Info");
            if (icon != null)
            {
                ETCImageLoader.LoadSprite(ref icon, infoTip.IconPath);
            }
            if (name != null)
            {
                name.text = infoTip.Name;
            }
            if (leftTime != null)
            {
                leftTime.text = infoTip.LeftTime;
            }
            if(info != null)
            {
                if (!string.IsNullOrEmpty(infoTip.Detail))
                {
                    info.text = infoTip.Detail;
                    if (!info.enabled)
                    {
                        info.enabled = true;
                    }
                }
                else
                {
                    info.enabled = false;
                }
            }
        }

        private void _ResetTipsRect()
        {
            if(mTipsComBindList != null)
            {
                for(int i =0; i < mTipsComBindList.Count; ++i)
                {
                    var tip = mTipsComBindList[i];
                    if(tip != null)
                    {
                        Text info = tip.GetCom<Text>("Info");
                        if(info != null)
                        {
                            info.text = string.Empty;
                        }
                    }
                }
            }
        }

        private void _onCloseBtnClick()
        {
            Close();
        }

        public static ComCommonBind CreateTips(GameObject parent)
        {
            if (parent == null)
            {
                Logger.LogError("BattleBuffTipsFrame Create function param parent is null!");
                return null;
            }

            GameObject item = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/BattleUI/BattleBuffTip", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            if (item != null)
            {
                ComCommonBind comTip = item.GetComponent<ComCommonBind>();
                if (comTip != null)
                {
                    Utility.AttachTo(comTip.gameObject, parent);
                    return comTip;
                }
            }
            return null;
        }

        public static void DestroyTip(ComCommonBind comTip)
        {
            if (comTip != null && comTip.gameObject != null)
            {
                Text info = comTip.GetCom<Text>("Info");
                if(info != null)
                {
                    info.text = string.Empty;
                }
                CGameObjectPool.instance.RecycleGameObject(comTip.gameObject);
            }
        }
    }
}