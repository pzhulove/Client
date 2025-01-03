using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using ProtoTable;

[ExecuteAlways]
public class ComChapterDungeonUnit : MonoBehaviour
{

    public enum eMissionType
    {
        None = -1,
        Main = 0,
        Other = 1,
    }

    public enum eState
    {
        /// <summary>
        /// 未解锁
        /// </summary>
        Locked,
        /// <summary>
        /// 已解锁，未通关
        /// </summary>
        Unlock,
        /// <summary>
        /// 已通关
        /// </summary>
        Passed,
        /// <summary>
        /// 已通关，未解锁，剧情关卡
        /// </summary>
        LockPassed,

    }

    public enum eDungeonType
    {
        Normal = 0,
        Prestory,
    }

    public RectTransform thumbRoot = null;
    public ComCommonBind mBind;

    [SerializeField] private float mSelectScale = 1.2f;
    [SerializeField] private Transform mTrRoot = null;
    [SerializeField] private Color mAppropriateColor;
    [SerializeField] private Color mLowColor;
    [SerializeField] private Color mHighColor;
    [SerializeField] private Text mTextLevelDesc;
    [SerializeField] private ImageEx mImageUnitType;
    [SerializeField] private CanvasGroup[] mUnitGos;
    [SerializeField] private Transform[] mIconParentTr;
    [SerializeField] private UIGray[] mGrayLocks;
    [SerializeField] private Color mLockIconColor;
    [SerializeField] private Text mTextElite;


    #region ExtraUIBind
    private Text mName = null;
    private Text mLevel = null;
    private GameObject mLockRoot = null;
    private Text mTypeName = null;
    private GameObject mCheckRoot = null;
    private GameObject mNormalRoot = null;
    private ComChapterDungeonUnit mDungeonUnit = null;
    private GameObject mMissionFlag = null;
    private Toggle mSelect = null;
    private Image mBackground = null;
    private Image mCharactorIcon = null;
    private RectTransform mSourcePosition = null;
    private RectTransform mTargetPosition = null;
    private RectTransform mTargetRightPosition = null;
    private TriangleGraph mAngleGraph = null;
    private GameObject mOpenEffect = null;
    private GameObject mEffectRoot = null;
    private GameObject challengeFlag = null;
    private GameObject mSpecialUnlockTipRoot = null;
    private Text mUnlockTipText = null;
    private GameClient.HelpAssistant mHelp = null;
    private GameObject mLvR = null;
    private GameObject mEliteBg = null;
    private GameObject mEffUI = null;
    private Button mPreview = null;
    private Slider mUnlockProcess = null;
    private Text mUnlockValue = null;

    protected void _bindExUI()
    {
        mName = mBind.GetCom<Text>("Name");
        mLevel = mBind.GetCom<Text>("Level");
        mLockRoot = mBind.GetGameObject("LockRoot");
        mTypeName = mBind.GetCom<Text>("TypeName");
        mCheckRoot = mBind.GetGameObject("CheckRoot");
        mNormalRoot = mBind.GetGameObject("NormalRoot");
        mDungeonUnit = mBind.GetCom<ComChapterDungeonUnit>("DungeonUnit");
        mMissionFlag = mBind.GetGameObject("MissionFlag");
        mSelect = mBind.GetCom<Toggle>("Select");
        if (mSelect != null)
        {
            mSelect.onValueChanged.AddListener(_onSelectToggleValueChange);
        }
        mBackground = mBind.GetCom<Image>("Background");
        mCharactorIcon = mBind.GetCom<Image>("CharactorIcon");
        mSourcePosition = mBind.GetCom<RectTransform>("sourcePosition");
        mTargetPosition = mBind.GetCom<RectTransform>("targetPosition");
        mTargetRightPosition = mBind.GetCom<RectTransform>("targetRightPosition");
        mAngleGraph = mBind.GetCom<TriangleGraph>("angleGraph");
        thumbRoot = mBind.GetCom<RectTransform>("thumbRoot");
        mOpenEffect = mBind.GetGameObject("openEffect");
        mEffectRoot = mBind.GetGameObject("effectRoot");
        challengeFlag = mBind.GetGameObject("challengeFlag");
        mSpecialUnlockTipRoot = mBind.GetGameObject("specialUnlockTipRoot");
        mUnlockTipText = mBind.GetCom<Text>("unlockTipText");
        mHelp = mBind.GetCom<GameClient.HelpAssistant>("Help");
        mLvR = mBind.GetGameObject("LvR");
        mEliteBg = mBind.GetGameObject("eliteBg");
        mEffUI = mBind.GetGameObject("effUI");
        mPreview = mBind.GetCom<Button>("preview");
        mPreview.SafeSetOnClickListener(() =>
        {
            GameClient.ClientSystemManager.GetInstance().OpenFrame<GameClient.EliteDungeonPreviewFrame>();
        });
        mUnlockProcess = mBind.GetCom<Slider>("unlockProcess");
        mUnlockValue = mBind.GetCom<Text>("unlockValue");
    }

    protected void _unbindExUI()
    {
        mName = null;
        mLevel = null;
        mLockRoot = null;
        mTypeName = null;
        mCheckRoot = null;
        mNormalRoot = null;
        mDungeonUnit = null;
        mMissionFlag = null;
        if (mSelect != null)
        {
            mSelect.onValueChanged.RemoveListener(_onSelectToggleValueChange);
        }
        mSelect = null;
        mBackground = null;
        mCharactorIcon = null;
        mSourcePosition = null;
        mTargetPosition = null;
        mTargetRightPosition = null;
        mAngleGraph = null;
        thumbRoot = null;
        mOpenEffect = null;
        mEffectRoot = null;
        challengeFlag = null;
        mSpecialUnlockTipRoot = null;
        mUnlockTipText = null;
        mHelp = null;
        mLvR = null;
        mEliteBg = null;
        mEffUI = null;
        mPreview = null;
        mUnlockProcess = null;
        mUnlockValue = null;
    }
    #endregion

    #region Callback
    private void _onSelectToggleValueChange(bool changed)
    {
        /* put your code in here */
        mCheckRoot.SetActive(changed);
        mNormalRoot.SetActive(!changed);
        mOpenEffect.SetActive(changed);

        if (mTrRoot != null)
        {
            Vector2 scale = changed ? Vector2.one * mSelectScale : Vector2.one;
            mTrRoot.localScale = scale;
        }
    }
    #endregion

    public void Awake()
    {
        dungeonID = 0;
        _bindExUI();
    }

    public void OnDestroy()
    {
        dungeonID = 0;
        _unbindExUI();
    }

    public eMissionType mType;

    public eState mState;

    public eDungeonType mDungeonType = eDungeonType.Normal;
    int dungeonID = 0;

    private bool mIsLock;
    public bool isLock
    {
        get
        {
            return mState == eState.Locked;
        }
    }
    bool bIsLoadEffect = false;

    //public void SetLock(bool isLock)
    //{
    //    mIsLock = isLock;
    //    mLock.gameObject.SetActive(isLock);/
    //    mUnLock.gameObject.SetActive(!isLock);
    //    mNameRoot.gameObject.SetActive(!isLock);
    //}
    //
    //
    //
    public void SetDungeonType(eDungeonType type)
    {
        mDungeonType = eDungeonType.Normal;
    }

    //public void SetBackgroud(Sprite sp)
    //{
    //    mBackground.sprite = sp;
    //}
    public void SetBackgroud(string spritePath)
    {
        // mBackground.sprite = sp;
        if (null == spritePath)
        {
            mBackground.sprite = null;
        }
        else
        {
            ETCImageLoader.LoadSprite(ref mBackground, spritePath);
        }
    }

    //public void SetCharactorSprite(Sprite sp)
    //{
    //    mCharactorIcon.sprite = sp;
    //    mCharactorIcon.SetNativeSize();
    //}
    public void SetCharactorSprite(string spritePath)
    {
        // mCharactorIcon.sprite = sp;
        if (null == spritePath || spritePath.Length <= 0)
        {
            mCharactorIcon.sprite = null;
        }
        else
        {
            ETCImageLoader.LoadSprite(ref mCharactorIcon, spritePath);
        }

        RectTransform rect = mCharactorIcon.rectTransform;

        float originHight = rect.sizeDelta.y;

        mCharactorIcon.SetNativeSize();
        mCharactorIcon.CustomActive(null != mCharactorIcon.sprite);

        float newHight = rect.sizeDelta.y;

        rect.sizeDelta = rect.sizeDelta * (originHight / newHight);
        rect.localScale = Vector3.one;
    }

    public void SetState(eState state)
    {
        bool isLock = false;
        //Sprite sprite = mUnLock.sprite;

        //Sprite[] sprites = mStateSprite;

        //switch (mDungeonType)
        //{
        //    case eDungeonType.Normal:
        //        sprites = mStateSprite;
        //        break;
        //    case eDungeonType.Prestory:
        //        sprites = mDungeonTypeSprite;
        //        break;
        //}

        mState = state;

        mSelect.interactable = true;

        switch (state)
        {
            case eState.Locked:
                isLock = true;
                mSelect.interactable = false;
                //sprite = sprites[0];
                break;
            case eState.Unlock:
                break;
            case eState.Passed:
                //sprite = sprites[1];
                break;
            case eState.LockPassed:
                //mSelect.interactable = false;
                //gameObject.GetComponent<Toggle>().interactable = false;
                //sprite = sprites[1];
                break;
        }


        //mUnLock.sprite = sprite;
        //mUnLock.SetNativeSize();

        //var ur = mUnLock.GetComponent<RectTransform>();
        //var lr = mLock.GetComponent<RectTransform>();

        //lr.offsetMax = ur.offsetMax;
        //lr.offsetMin = ur.offsetMin;

        mLockRoot.SetActive(isLock);
        _SetGrayLock(isLock);
        //mLock.gameObject.SetActive(isLock);
        //mUnLock.gameObject.SetActive(!isLock);
        //mNameRoot.gameObject.SetActive(mDungeonType == eDungeonType.Normal && !isLock);
        mEffUI.CustomActive(mState != eState.Locked);
        if (GameClient.TeamUtility.IsEliteDungeonID(dungeonID))
        {

            mPreview.CustomActive(mState == eState.Locked);
            mUnlockProcess.CustomActive(mState == eState.Locked);
            List<int> ids = GameClient.EliteDungeonPreviewFrame.GetCurrentChapterNormalDungeonIDs();
            int count = 0;
            if (ids != null && ids.Count > 0)
            {
                count = ids.Count;
            }
            mUnlockValue.SafeSetText(string.Format("{0}/{1}", GameClient.EliteDungeonPreviewFrame.GetSSSDungeonNum(), count));
            float percent = 1.0f;
            if (count > 0)
            {
                percent = ((float)GameClient.EliteDungeonPreviewFrame.GetSSSDungeonNum()) / ((float)count);
            }
            mUnlockProcess.SafeSetValue(percent);

            // 特殊规则 锁住状态不显示帮助
            if (state == eState.Locked)
            {
                SetHelpType(HelpFrameContentTable.eHelpType.HT_NONE);
            }
            else
            {
                SetHelpType(HelpFrameContentTable.eHelpType.HT_ELITE_DUNGEON);
            }
        }
        else
        {
            mPreview.CustomActive(false);
            mUnlockProcess.CustomActive(false);
            SetHelpType(HelpFrameContentTable.eHelpType.HT_NONE);
        }
    }

    private void _SetGrayLock(bool isGray)
    {
        if (mCharactorIcon != null)
        {
            mCharactorIcon.color = isGray ? mLockIconColor : Color.white;
        }

        if (mGrayLocks != null)
        {
            foreach (var v in mGrayLocks)
            {
                if (v == null)
                {
                    continue;
                }

                v.SetEnable(isGray);
            }
        }
    }

    public void SetType(eMissionType type)
    {
        if (mTypeName != null)
            mTypeName.text = "";

        //if (type != mType)
        {
            mType = type;

            if(mMissionFlag == null)
            {
                return;
            }
            mMissionFlag.SetActive(false);

            //mIcon.enabled = true;

            switch (mType)
            {
                case eMissionType.Main:
                    mMissionFlag.SetActive(true);
                    //mTypeName.text = "主线";
                    //mIcon.sprite = mMissionTypeSprite[0];
                    break;
                case eMissionType.Other:
                    mMissionFlag.SetActive(true);
                    //mTypeName.text = "支线";
                    //mIcon.sprite = mMissionTypeSprite[1];
                    break;
                default:
                    //mTypeName.text = "";
                    //mIcon.enabled = false;
                    break;
            }
        }
    }

    public void SetDungeonID(int id)
    {
        dungeonID = id;
    }
    public void SetName(string name, string level)
    {
        if (mName != null) mName.text = name;
        if (mLevel != null) mLevel.text = level;
        mTextElite.SafeSetText(name);
        _UpdateLevelColor(level);
    }

    private void _UpdateLevelColor(string level)
    {
        string[] strs = level.Split('-');
        if (strs != null && strs.Length == 2)
        {
            int curLevel = GameClient.PlayerBaseData.GetInstance().Level;
            int minLevel = 0;
            int maxLevel = 0;
            int.TryParse(strs[0], out minLevel);
            int.TryParse(strs[1], out maxLevel);
            if (curLevel < minLevel)
            {
                mLevel.SafeSetColor(mHighColor);
                mTextLevelDesc.SafeSetColor(mHighColor);
            }
            else if (curLevel >= minLevel && curLevel <= maxLevel)
            {
                mLevel.SafeSetColor(mAppropriateColor);
                mTextLevelDesc.SafeSetColor(mAppropriateColor);
            }
            else if (curLevel > maxLevel)
            {
                mLevel.SafeSetColor(mLowColor);
                mTextLevelDesc.SafeSetColor(mLowColor);
            }
        }
    }

    public void SetIsChallenging(bool value)
    {
        challengeFlag.CustomActive(value);
    }

    public void SetHelpType(HelpFrameContentTable.eHelpType helpType)
    {
        if (mHelp != null)
        {
            mHelp.eType = helpType;
            mHelp.CustomActive(helpType != HelpFrameContentTable.eHelpType.HT_NONE);
        }
    }
    const string eliteEffUIPath = "Effects/UI/Prefab/EffUI_juqingguanka/Prefab/Eff_UI_juqingguanka_Jingyingdixiacheng";
    public void ShowEliteBg(bool value)
    {
        mEliteBg.CustomActive(value);
        if (mEffUI != null)
        {
            RectTransform rectTransform = mEffUI.GetComponent<RectTransform>();
            for (int i = 0; i < rectTransform.childCount; ++i)
            {
                GameObject.Destroy(rectTransform.GetChild(i).gameObject);
            }
            if (value)
            {
                GameObject ObjEffect = AssetLoader.GetInstance().LoadRes(eliteEffUIPath).obj as GameObject;
                if (ObjEffect != null)
                {
                    ObjEffect.transform.SetParent(rectTransform, false);
                    ObjEffect.SetActive(true);
                }
            }
            mEffUI.CustomActive(mState != eState.Locked);
        }
    }

    public void UpdateUnityType(ChapterDungeonUnitType chapterDungeonUnitType, Vector3 iconPos)
    {
        if (chapterDungeonUnitType == ChapterDungeonUnitType.None)
        {
            return;
        }

        if (mUnitGos == null)
        {
            return;
        }

        int index = (int)chapterDungeonUnitType;
        if (mUnitGos.Length <= index)
        {
            return;
        }

        mTextElite.CustomActive(index == (int)ChapterDungeonUnitType.Elite);
        mName.CustomActive(index != (int)ChapterDungeonUnitType.Elite);

        mUnitGos[index].CustomActive(true);

        if (mIconParentTr != null && mIconParentTr.Length > index && mCharactorIcon != null)
        {
            mCharactorIcon.transform.SetParent(mIconParentTr[index], false);
            mCharactorIcon.transform.localPosition = iconPos;
        }
    }

    // 设置额外的解锁提示文字
    // 目前只有精英地下城有额外的解锁提示
    public void SetExtarLockTipText(string tipText)
    {
        mUnlockTipText.SafeSetText(tipText);
        mSpecialUnlockTipRoot.CustomActive(!string.IsNullOrEmpty(tipText));
    }

    // 设置是否显示关卡等级限制
    public void ShowDungeonLvLimit(bool bShow)
    {
        mLvR.CustomActive(bShow);
    }
    public void SetEffect(string effectPath)
    {
        if (bIsLoadEffect)
        {
            return;
        }

        if (mEffectRoot != null)
        {
            GameObject YiJieEffectObj = AssetLoader.instance.LoadResAsGameObject(effectPath);

            if (YiJieEffectObj != null)
            {
                Utility.AttachTo(YiJieEffectObj, mEffectRoot);

                bIsLoadEffect = true;
            }
        }
    }

    [Conditional("UNITY_EDITOR")]
    public void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            // Display the explosion radius when selected
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gameObject.transform.position, 20);
            Gizmos.color = Color.white;
        }
    }
}
