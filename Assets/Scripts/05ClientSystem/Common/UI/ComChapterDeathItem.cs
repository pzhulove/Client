using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;
using UnityEngine.Events;
using ProtoTable;

public class ComChapterDeathItem : MonoBehaviour
{
    const string TeXiaoQuan = "Effects/Scene_effects/EffectUI/EffUI_quan";
    const string TeXiaoGuang = "Effects/Scene_effects/EffectUI/EffUI_xiangzi";
    const string TeXiaoQuanPath = "EffUI_quan(Clone)";
    const string TeXiaoGuangPath = "EffUI_item_cheng(Clone)";

    public enum eState
    {
        /// <summary>
        /// 未解锁
        /// </summary>
        Lock,

        /// <summary>
        /// 解锁，未通关
        /// </summary>
        Unlock,

        /// <summary>
        /// 通关奖励未领取
        /// </summary>
        Pass,
        
    }

    public delegate void ChapterDeadItemAction(eState st);

    public ComCommonBind mBind;

#region ExtraUIBind
    private GameObject mImage = null;
    private CanvasGroup mStageLineCanvas;
    private CanvasGroup mLastStageCanvas;
    private Text mText = null;
    //private UIGray mGray = null;
    private ComItemList mItemlist = null;
    private Button mButton = null;
    private UIGray mDropItemGray = null;
    private GameObject mFg = null;
    private Image mBoxState = null;
    private GameObject mTeXiao = null;
    private GameObject mTeXiaoQuan = null;
    private Text mLimitLevel = null;

    protected void _bindExUI()
    {
        mImage = mBind.GetGameObject("image");

        mStageLineCanvas = mBind.GetCom<CanvasGroup>("StageLine");
        mLastStageCanvas = mBind.GetCom<CanvasGroup>("Stage2");
        mText = mBind.GetCom<Text>("text");
        //mGray = mBind.GetCom<UIGray>("gray");
        mItemlist = mBind.GetCom<ComItemList>("itemlist");
        mButton = mBind.GetCom<Button>("button");
        mDropItemGray = mBind.GetCom<UIGray>("gray");
        mFg = mBind.GetGameObject("Fg");
        mBoxState = mBind.GetCom<Image>("boxState");
        mTeXiao = mBind.GetGameObject("TeXiao");
        mTeXiaoQuan = mBind.GetGameObject("TeXiaoQuan");
        mLimitLevel = mBind.GetCom<Text>("LimitLevel");
    }

    protected void _unbindExUI()
    {
        mImage = null;
        mStageLineCanvas = null;
        mLastStageCanvas = null;
        mText = null;
        //mGray = null;
        mItemlist = null;
        mButton.onClick.RemoveAllListeners();
        mButton = null;
        mDropItemGray = null;
        mFg = null;
        mBoxState = null;
        mTeXiao = null;
        mTeXiaoQuan = null;
        mLimitLevel = null;
    }
#endregion

    void Awake()
    {
        _bindExUI();
    }

    void OnDestroy()
    {
        _unbindExUI();
    }

    private eState mState = eState.Lock;
    private int mMask = 0;
    private int mIndex = 0;

    private void _setStageLineArrow(int index)
    {
        if (index == 0)
        {
            mStageLineCanvas.CustomActive(false);
            return;
        }

        mStageLineCanvas.CustomActive(true);
        if (index % 2 == 0)
        {
            mStageLineCanvas.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            mStageLineCanvas.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void _setLastStageShow(int index)
    {
        if (index == ChapterUtility.kDeadTowerLevelCount - 1)
        {
            mLastStageCanvas.CustomActive(false);
        }
        else
        {
            mLastStageCanvas.CustomActive(true);
        }
    }

    private void _setLevel(int start, int end)
    {
        if (null != mText)
        {
            mText.text = string.Format("{0}-{1}层", start, end);
        }
    }

    private void _setLevelLimit(int level)
    {
        mLimitLevel.gameObject.CustomActive(false);
        if (null != mLimitLevel)
        {
            mLimitLevel.gameObject.CustomActive(false);
        }
        var DeathTowerAwardTableData = TableManager.GetInstance().GetTableItem<DeathTowerAwardTable>(level);
        if(null == DeathTowerAwardTableData)
        {
            Logger.LogErrorFormat("DeathTowerAwardTableData is null");
            return;
        }
        int LimitLevel = DeathTowerAwardTableData.LimitLevel;
        if(0 == LimitLevel)
        {
            Logger.LogErrorFormat("LimitLevel is null");
            return;
        }
        if(0 != LimitLevel && LimitLevel > PlayerBaseData.GetInstance().Level)
        {
            if (null != mLimitLevel)
            {
                mLimitLevel.gameObject.CustomActive(true);
                mLimitLevel.text = string.Format("{0}级开启", LimitLevel);
            }
        }
    }

    public void SetIndex(int i)
    {
        mIndex = i;

        _setStageLineArrow(i);
        _setLastStageShow(i);
        _setLevel(i * 5 + 1, i * 5 + 5);
        _setLevelLimit(i * 5+1);
    }

    public void UpdateLimitLevel(int i)
    {
        _setLevelLimit(i * 5 + 1);
    }
    public void SetMask(int mask)
    {
        mMask = mask;
        mFg.SetActive(_isGetReward());
        mDropItemGray.enabled = _isGetReward();
    }

    private bool _isGetReward()
    {
        return (mMask & (1 << mIndex)) > 0;
    }

    public void SetClick(ChapterDeadItemAction cb)
    {
        if (null != mButton)
        {
            mButton.onClick.AddListener(() =>
            {
                if (mState == eState.Pass)
                {
                    if (!_isGetReward())
                    {
                        if (null != cb)
                        {
                            cb(mState);
                        }
                    }
                }
                else 
                {
                    if (null != cb)
                    {
                        cb(mState);
                    }
                }
            });
        }
    }

    public void SetSelect(bool isSelect)
    {
        mImage.SetActive(isSelect);
        if(!isSelect)
        {
            GameObject TeXiaoQuanGO = null;
            var Geeffect2 = mTeXiaoQuan.GetComponentInChildren<GeEffectProxy>();
            if (Geeffect2 != null)
            {
                TeXiaoQuanGO = Geeffect2.gameObject;
            }
            if (TeXiaoQuanGO != null)
            {
                GameObject.DestroyImmediate(TeXiaoQuanGO);
            }
        }
        else if(_isGetReward())
        {
            GameObject item1 = AssetLoader.instance.LoadResAsGameObject(TeXiaoQuan);
            item1 = AssetLoader.instance.LoadResAsGameObject(TeXiaoQuan);
            if (item1 != null)
            {
                Utility.AttachTo(item1, mTeXiaoQuan);
            }
        }
    }

    public void SetItems(GameClient.ComItemList.Items[] list)
    {
        mItemlist.SetItems(list);
    }

    public void SetState(eState state)
    {
        mState = state;

        // mBoxState.sprite = _isGetReward() ? mBind.GetSprite("open") : mBind.GetSprite("close");
        if(_isGetReward())
        {
            mBind.GetSprite("open", ref mBoxState);
        }
        else
        {
            mBind.GetSprite("close", ref mBoxState);
        }

        mDropItemGray.SetEnable(mDropItemGray.enabled);

        if(!_isGetReward())
        {
            var ImageRect = mBoxState.gameObject.GetComponent<RectTransform>();
            ImageRect.localScale = new Vector3(1.8f, 1.8f, 1);
        }
        else
        {
            var ImageRect = mBoxState.gameObject.GetComponent<RectTransform>();
            ImageRect.localScale = new Vector3(1, 1, 1);
        }
        mDropItemGray.enabled = false;
        switch (state)
        {
            case eState.Lock:
                mDropItemGray.enabled = true;
                break;
            case eState.Pass:


                GameObject TeXiaoGuangGO = null;
                var Geeffect = mTeXiao.GetComponentInChildren<GeEffectProxy>();
                if(Geeffect!=null)
                {
                    TeXiaoGuangGO = Geeffect.gameObject;
                }

                
                if (TeXiaoGuangGO != null)
                {
                    GameObject.DestroyImmediate(TeXiaoGuangGO);
                }
                


                if (_isGetReward())
                {
                }
                else
                {

                    GameObject item2 = AssetLoader.instance.LoadResAsGameObject(TeXiaoGuang);
                    if (item2 != null)
                    {
                        Utility.AttachTo(item2, mTeXiao);
                    }
                }
                break;
            case eState.Unlock:
                break;
        }
    }
}

