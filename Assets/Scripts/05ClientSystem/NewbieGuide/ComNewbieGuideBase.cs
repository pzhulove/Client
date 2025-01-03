using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public enum NewbieGuideComType
{
    NULL = -2,
    USER_DEFINE = -1,
	BUTTON = 0,
	ETC_BUTTON,
	ETC_JOYSTICK,
	MOVE_2_POS,
	PAUSE_BATTLE,
	RESUME_BATTLE,
	SYSTEM_BUTTON,
	TALK_DIALOG,
	TOGGLE,
	WAIT,
    INTRODUCTION,
    INTRODUCTION2,
    COVER,
    PASS_THROUGH,
    SHOW_IMAGE,
    STRESS,
    OPEN_EYES,
    NEWICON_UNLOCK,
    BATTLEDRUGDRAG,
    PLAY_EFFECT,
}

public enum TextTipType
{
    TextTipType_One,
    TextTipType_Two,
	TextTipType_Three,
}

[LoggerModel("NewbieGuide")]
public class ComNewbieGuideBase : MonoBehaviour
{
    // 枚举定义
    protected enum eNewbieGuideBaseState
    {
        None,
        Init,
        Guide,
        Complete,
    }

    public enum GuideState
    {
        Normal,
        Wait,
        Exception,
    }

    public enum eNewbieGuideAnchor
    {
        Top,
        Buttom,
        Left,
        Right,
        TopLeft,
        TopRight,
        ButtomLeft,
        ButtomRight,
        Center,
    }

    // 资源路径
    private const string kButtonTipsPath = "UIFlatten/Prefabs/NewbieGuide/ButtonTips";
    private const string kDragTipsPath = "UIFlatten/Prefabs/NewbieGuide/DragTips";
    private const string kTouchTipsPath = "UIFlatten/Prefabs/NewbieGuide/TouchTips";
    private const string kIntroductionTipsPath = "UIFlatten/Prefabs/NewbieGuide/IntroductionTips";
    private const string kIntroductionTipsPath2 = "UIFlatten/Prefabs/NewbieGuide/IntroductionTips2";
    private const string kPassThroughTipsPath = "UIFlatten/Prefabs/NewbieGuide/PassThroughTips";
    public  const string kWaitTipsPath = "UIFlatten/Prefabs/NewbieGuide/WaitTips";
    private const string kCoverPath = "UIFlatten/Prefabs/NewbieGuide/Cover";
    private const string kShowImagePath = "UIFlatten/Prefabs/NewbieGuide/ShowImage";
    private const string kTextTipsOnePath = "UIFlatten/Prefabs/NewbieGuide/TextTips";
    private const string kTextTipsTwoPath = "UIFlatten/Prefabs/NewbieGuide/TextTipsTwo";
    private const string kTextTipsThreePath = "UIFlatten/Prefabs/NewbieGuide/TextTipsThree";
    private const string kFingerTipsPath = "";
    private const string kLoopTipsPath = "";
    private const string kStressPath = "UIFlatten/Prefabs/NewbieGuide/Stress";
    private const string kOpenEyesPath = "UIFlatten/Prefabs/NewbieGuide/guidetownstart";
    private const string kBattleDrugDrag = "UIFlatten/Prefabs/NewbieGuide/BattleDrugDragTips";

    // 管理器
    protected ComNewbieGuideControl mGuideControl;
    protected eNewbieGuideBaseState mBaseState;

    // 控制变量
    public int mSortOrder = 0;
    public bool mIsButton = false;
    public bool mIsFinger = false;
    private float mTimeIntreval = 0.0f;

    // 数据
    protected List<GameObject> mCachedObject = new List<GameObject>();
    private List<GameObject> mTipsObject = new List<GameObject>();
    private List<GameObject> mRootObject = new List<GameObject>();
    private List<Component> mComponents = new List<Component>();

    // 外部参数
    public string mFrameType;
    public string mComRoot;
    public eNewbieGuideAnchor mAnchor;
    public string mHighLightPointPath;
    public string mTextTips;
    public TextTipType mTextTipType;
    public Vector3 mLocalPos;
    public bool mSendSaveBoot = false;
    public bool mTryPauseBattle = false;
    public bool mTryResumeBattle = false;
    public bool mTryPauseBattleAI = false;
    public float mProtectTime = 0.0f;

    public ComNewbieGuideBase()
    {
        mBaseState = eNewbieGuideBaseState.None;
    }

    public void ClearData()
    {
		bool needDisable = GameClient.SwitchFunctionUtility.IsOpen(14);

        mHighLightPointPath = "";
        mProtectTime = 0.0f;
        mTimeIntreval = 0.0f;
        mTryResumeBattle = false;
        mBaseState = eNewbieGuideBaseState.None;

        if (mTryPauseBattle)
        {
            _tryResumeBattle();
        }
        else if (mTryPauseBattleAI)
        {
            _tryResumeBattleEnemyAI();
        }

        for (int i = 0; i < mComponents.Count; ++i)
        {
            if (mComponents[i] != null)
            {
				if (needDisable)
				{
					var canvas = mComponents[i] as Canvas;
					if (canvas != null)
					{
						canvas.enabled = false;
					}
				}
					
                GameObject.Destroy(mComponents[i]);
            }

            mComponents[i] = null;
        }
        mComponents.Clear();

        for (int i = 0; i < mCachedObject.Count; i++)
        {
            GameObject go = mCachedObject[i];

            if(go != null)
            {
                Button[] buts = go.GetComponentsInChildren<Button>();
                for (int j = 0, jcnt = buts.Length; j < jcnt; ++j)
                {
                    if (null != buts[j])
                        buts[j].onClick.RemoveAllListeners();
                }

                Toggle[] togs = go.GetComponentsInChildren<Toggle>();
                for (int j = 0, jcnt = togs.Length; j < jcnt; ++j)
                {
                    if (null != togs[j])
                        togs[j].onValueChanged.RemoveAllListeners();
                }


				if (needDisable)
				{
					go.CustomActive(false);
					GameObject.Destroy(go, 0.1f);
				}
				else
                	GameObject.Destroy(go);
            }

            mCachedObject[i] = null;
        }
        mCachedObject.Clear();

        mTipsObject.Clear();
        mRootObject.Clear();

        GameObject.Destroy(this);
    }

    public virtual void StartInit(params object[] args)
    {
        Logger.LogProcessFormat("base start init");

        mBaseState = eNewbieGuideBaseState.Init;
        mFrameType = "";
        mComRoot = "";
        mAnchor = eNewbieGuideAnchor.TopLeft;
        mTextTips = "";
        mTextTipType = TextTipType.TextTipType_One;
        mLocalPos = new Vector3();
        mProtectTime = 0.0f;
        mHighLightPointPath = "";
    }

    protected virtual GuideState _init()
    {
        return GuideState.Normal;
    }

    public void BaseComplete()
    {
        mBaseState = eNewbieGuideBaseState.Complete;
    }

    public void SetShow(bool bShow)
    {
        if(bShow)
        {
            for(int i = 0; i < mCachedObject.Count; i++)
            {
                if(mCachedObject[i] == null)
                {
                    continue;
                }

                mCachedObject[i].SetActive(true);
            }

            for (int i = 0; i < mTipsObject.Count; i++)
            {
                if (mTipsObject[i] == null)
                {
                    continue;
                }

                mTipsObject[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < mCachedObject.Count; i++)
            {
                if (mCachedObject[i] == null)
                {
                    continue;
                }

                mCachedObject[i].SetActive(false);
            }

            for (int i = 0; i < mTipsObject.Count; i++)
            {
                if (mTipsObject[i] == null)
                {
                    continue;
                }

                mTipsObject[i].SetActive(false);
            }
        }
    }

    protected void _complete(Button ClickBtn)
    {
        if (mGuideControl != null)
        {
            if(mTimeIntreval < mProtectTime)
            {
                return;
            }

            if(ClickBtn != null)
            {
                ClickBtn.enabled = false;
                ClickBtn.interactable = false;
            }        

            Logger.LogProcessFormat("base complete");
            mGuideControl.ControlComplete();
        }
        else
        {
            if(ClickBtn != null)
            {
                Logger.LogErrorFormat("task base control is nil in [_complete] with button, ClickBtn Name = {0}", ClickBtn.name);
            }
            else
            {
                Logger.LogError("task base control is nil in [_complete] with button");
            }     
        }
    }

    protected void _complete()
    {
        if (mGuideControl != null)
        {
            if (mTimeIntreval < mProtectTime)
            {
                return;
            }

            Logger.LogProcessFormat("base complete");
            mGuideControl.ControlComplete();
        }
        else
        {
            Logger.LogError("task base control is nil in [_complete]");
        }
    }

    protected void _complete(bool bEnd)
    {
        if (mGuideControl != null)
        {
            if (mTimeIntreval < mProtectTime)
            {
                return;
            }

            Logger.LogProcessFormat("base complete");
            mGuideControl.ControlComplete();
        }
        else
        {
            Logger.LogErrorFormat("task base control is nil, bEnd = {0}", bEnd);
        }
    }

    private void _wait()
    {
        if (mGuideControl != null)
        {
            Logger.LogProcessFormat("base skip");
            mGuideControl.ControlWait();
        }
        else
        {
            Logger.LogError("task base control is nil in [_wait]");
        }
    }

    protected void _exception()
    {
        if (mGuideControl != null)
        {
            Logger.LogProcessFormat("base exception");
            mGuideControl.ControlException();
        }
        else
        {
            Logger.LogError("task base control is nil in [_exception]");
        }
    }

    void Update()
    {
        mTimeIntreval += Time.deltaTime;

        if (mBaseState == eNewbieGuideBaseState.Init)
        {
            GuideState GuideComInitState = _init();

            if (GuideComInitState == GuideState.Normal)
            {
//                 if (mTryPauseBattle)
//                 {
//                     _tryPauseBattle();
//                 }
//                 else if (mTryPauseBattleAI)
//                 {
//                     _tryPauseBattleEnemyAI();
//                 }

                mBaseState = eNewbieGuideBaseState.Guide;
            }
            else if(GuideComInitState == GuideState.Wait)
            {
                _wait();
                mBaseState = eNewbieGuideBaseState.Guide;
            }
            else
            {
                _exception();
                mBaseState = eNewbieGuideBaseState.Complete;
            }
        }
        else if (mBaseState == eNewbieGuideBaseState.Guide)
        {
            _update();

            if(mTipsObject != null && mRootObject != null)
            {
                for (int i = 0; i < mTipsObject.Count && i < mRootObject.Count; ++i)
                {
                    if (mTipsObject[i] != null && mRootObject[i] != null)
                    {
                        _updateTipPosition(mTipsObject[i], mRootObject[i]);
                    }
                }
            }
        }
        else if (mBaseState == eNewbieGuideBaseState.Complete)
        {
            ClearData();
        }
    }

    private void OnDestroy()
    {
        Logger.LogProcessFormat("base destroy");
    }

    protected virtual void _update()
    {
    }

    private void _save()
    {
        if (mGuideControl != null)
        {
            Logger.LogProcessFormat("base save");
            mGuideControl.ControlSave();
        }
        else
        {
            Logger.LogError("task base control is nil in [_save]");
        }
    }

    public void SetTaskBaseNewbieGuideControl(ComNewbieGuideControl taskBase)
    {
        mGuideControl = taskBase;
    }

    public void AddCanvasCom(GameObject obj, bool r = false)
    {
        if(obj == null)
        {
            return;
        }

        obj.layer = 5;

        Canvas cam = obj.GetComponent<Canvas>();
        if (null == cam)
        {
            cam = obj.AddComponent<Canvas>();
            cam.overrideSorting = true;
            cam.sortingOrder = 805;

            mComponents.Add(cam);
        }
        else
        {
            cam.overrideSorting = true;
            cam.sortingOrder = 805;
        }

        if (r)
        {
            for (int i = 0; i < obj.transform.childCount; ++i)
            {
                AddCanvasCom(obj.transform.GetChild(i).gameObject, r);
            }
        }
    }

    public void AddCanvasComEx(GameObject obj, bool r = false,int iLayer = 805)
    {
        if(obj == null)
        {
            return;
        }

        obj.layer = 5;

        Canvas cam = obj.GetComponent<Canvas>();
        if (null == cam)
        {
            cam = obj.AddComponent<Canvas>();
            cam.overrideSorting = true;
            cam.sortingOrder = iLayer;

            mComponents.Add(cam);
        }
        else
        {
            cam.overrideSorting = true;
            cam.sortingOrder = iLayer;
        }

        if (r)
        {
            for (int i = 0; i < obj.transform.childCount; ++i)
            {
                AddCanvasComEx(obj.transform.GetChild(i).gameObject, r,iLayer + 1);
            }
        }
    }

    public void _clickEffect()
    {
        for(int i = 0; i < mCachedObject.Count; i++)
        {
            GameObject obj = mCachedObject[i];
            if(obj == null)
            {
                continue;
            }

            if(obj.name == "NewbieButtonType")
            {
                GeUIEffectParticle[] effects = obj.GetComponentsInChildren<GeUIEffectParticle>();

                for(int j = 0; j < effects.Length; j++)
                {
                    if(effects[j].gameObject == null)
                    {
                        continue;
                    }

                    if(effects[j].name == "UIEffectParticle")
                    {
                        effects[j].RestartEmit();

                        break;
                    }
                }

                break;
            }
        }
    }

    public void AddToCachedObject(GameObject go)
    {
        mCachedObject.Add(go);
    }

    public GameObject AddButtonTips(GameObject buttonRoot, UnityAction action, GameObject fromRoot = null, float time = 1.0f)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kButtonTipsPath);
        go.name = "NewbieButtonType";
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Button");

        if(button == null)
        {
            Logger.LogError("Add button is null when [AddButtonTips]");
            return null;
        }

        var buttonCom = button.GetComponent<Button>();
        
        if(buttonCom == null)
        {
            Logger.LogError("Add buttonCom is null when [AddButtonTips]");
            return null;
        }

        buttonCom.onClick.AddListener(() => { if (action != null) action.Invoke(); _complete(buttonCom); });
        //buttonCom.onClick.AddListener(action);

        var HGbutton = Utility.FindGameObject(go, "HGButton");
        if(HGbutton == null)
        {
            Logger.LogError("Add HGbutton is null when [AddButtonTips]");
            return null;
        }

        HGbutton.gameObject.SetActive(false);

        var clickobj = Utility.FindGameObject(go, "click");
        if(clickobj == null)
        {
            Logger.LogError("Add clickobj is null when [AddButtonTips]");
            return null;
        }

        var clickCom = clickobj.GetComponent<Button>();
        clickCom.onClick.AddListener(_clickEffect);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        AddCanvasCom(buttonRoot);

        mTipsObject.Add(button);
        mRootObject.Add(buttonRoot);

        var tips = Utility.FindGameObject(button, "Finger");

        if(tips != null)
        {
            RectTransform rec = tips.GetComponent<RectTransform>();
            _updateFingerDirection(rec, mAnchor);

            if (fromRoot != null)
            {
                DOTween.To(
                    () => button.transform.localPosition,
                    r => tips.transform.localPosition = r,
                    fromRoot.transform.localPosition,
                    time).SetEase(Ease.OutQuad).From();
            }
        }

        return button;
    }

    public GameObject AddHGButtonTips(GameObject buttonRoot, UnityAction<bool> action, GameObject fromRoot = null, float time = 1.0f)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kButtonTipsPath);
        go.name = "NewbieButtonType";
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "HGButton");
        var buttonCom = button.GetComponent<HGButton>();
        buttonCom.onUpDown.AddListener(_complete);
        buttonCom.onUpDown.AddListener(action);

        var btn = Utility.FindGameObject(go, "Button");
        btn.gameObject.SetActive(false);

        var clickobj = Utility.FindGameObject(go, "click");
        var clickCom = clickobj.GetComponent<Button>();
        clickCom.onClick.AddListener(_clickEffect);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        AddCanvasCom(buttonRoot);

        mTipsObject.Add(button);
        mRootObject.Add(buttonRoot);

        var tips = Utility.FindGameObject(button, "Finger");

        if (tips != null)
        {
            RectTransform rec = tips.GetComponent<RectTransform>();
            _updateFingerDirection(rec, mAnchor);

            if (fromRoot != null)
            {
                DOTween.To(
                    () => button.transform.localPosition,
                    r => tips.transform.localPosition = r,
                    fromRoot.transform.localPosition,
                    time).SetEase(Ease.OutQuad).From();
            }
        }

        return button;
    }

    public GameObject AddBattleDrugDrag(GameObject buttonRoot, UnityAction action, GameObject fromRoot = null, float time = 1.0f)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kBattleDrugDrag);
        go.name = "NewbieDrugDragType";
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Button");

        if (button == null)
        {
            Logger.LogError("Add button is null when [AddButtonTips]");
            return null;
        }

        var buttonCom = button.GetComponent<Button>();

        if (buttonCom == null)
        {
            Logger.LogError("Add buttonCom is null when [AddButtonTips]");
            return null;
        }

        buttonCom.onClick.AddListener(() => { if (action != null) action.Invoke(); _complete(buttonCom); });
        //buttonCom.onClick.AddListener(action);

        var clickobj = Utility.FindGameObject(go, "click");
        if (clickobj == null)
        {
            Logger.LogError("Add clickobj is null when [AddButtonTips]");
            return null;
        }

        var clickCom = clickobj.GetComponent<Button>();
        clickCom.onClick.AddListener(_clickEffect);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        AddCanvasCom(buttonRoot);

        mTipsObject.Add(button);
        mRootObject.Add(buttonRoot);

        var tips = Utility.FindGameObject(button, "Finger");

        if (tips != null)
        {
            RectTransform rec = tips.GetComponent<RectTransform>();
            _updateFingerDirection(rec, mAnchor);

            if (fromRoot != null)
            {
                DOTween.To(
                    () => button.transform.localPosition,
                    r => tips.transform.localPosition = r,
                    fromRoot.transform.localPosition,
                    time).SetEase(Ease.OutQuad).From();
            }
        }

        return button;
    }
   
    public GameObject AddButtonTipNoAutoComplete(GameObject buttonRoot, UnityAction action, GameObject fromRoot = null, float time = 1.0f)
    {
        var go = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/NewbieGuide/ButtonTipsDrag");
        go.name = "NewbieButtonType";
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Button");
        //var buttonCom = button.GetComponent<Button>();
       // buttonCom.onClick.AddListener(action);

        var clickobj = Utility.FindGameObject(go, "click");
        var clickCom = clickobj.GetComponent<Button>();
        clickCom.onClick.AddListener(_clickEffect);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        
        //AddCanvasCom(buttonRoot);

        mTipsObject.Add(button);
        mRootObject.Add(buttonRoot);
        
        var tips = Utility.FindGameObject(button, "Finger");

        if(tips != null)
        {
            RectTransform rec = tips.GetComponent<RectTransform>();
            _updateFingerDirection(rec, mAnchor);

            if (fromRoot != null)
            {
                DOTween.To(
                    () => button.transform.localPosition,
                    r => tips.transform.localPosition = r,
                    fromRoot.transform.localPosition,
                    time).SetEase(Ease.OutQuad).From();
            }
        }

        return go;
    }

    public GameObject AddIntroductionTips(GameObject buttonRoot)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kIntroductionTipsPath);
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Button");
        var buttonCom = button.GetComponent<Button>();
        buttonCom.onClick.AddListener(() => { _complete(buttonCom); });

        AddCanvasCom(buttonRoot);

        var Pos = Utility.FindGameObject(go, "Pos");

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        mTipsObject.Add(Pos);
        mRootObject.Add(buttonRoot);

        return Pos;
    }

    public void AddIntroductionTips2()
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kIntroductionTipsPath2);
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Button");
        var buttonCom = button.GetComponent<Button>();
        buttonCom.onClick.AddListener(() => { _complete(buttonCom); });

        var content = Utility.FindGameObject(go, "middle/content");
        var contentText = content.GetComponent<Text>();
        contentText.text = mTextTips;

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();
    }

    public GameObject AddPassThroughTips(GameObject buttonRoot, GameObject ShowAreaRoot, UnityAction action)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kPassThroughTipsPath);
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Button");
        var buttonCom = button.GetComponent<Button>();
        buttonCom.onClick.AddListener(() => { _complete(buttonCom); });
        buttonCom.onClick.AddListener(action);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        //mTipsObject.Add(button);
        //mRootObject.Add(buttonRoot);

        var ShowArea = Utility.FindGameObject(go, "ShowArea");

        if (ShowAreaRoot != null)
        {
            mTipsObject.Add(ShowArea);
            mRootObject.Add(ShowAreaRoot);
        }
        else
        {
            ShowArea.SetActive(false);
        }

        return ShowArea;
    }

    public bool AddShowImage(string ShowImagePath)
    {
        Sprite spr = AssetLoader.instance.LoadRes(ShowImagePath, typeof(Sprite)).obj as Sprite;
        if(spr == null)
        {
            return false;
        }

        var go = AssetLoader.instance.LoadResAsGameObject(kShowImagePath);
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Button");
        var buttonCom = button.GetComponent<Button>();
        buttonCom.onClick.AddListener(() => { _complete(buttonCom); });

        var imgObj = Utility.FindGameObject(go, "pos");
        Image img = imgObj.GetComponent<Image>();
        // img.sprite = spr;
        ETCImageLoader.LoadSprite(ref img, ShowImagePath);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        return true;
    }

    public bool AddStress(GameObject SourceObj)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kStressPath);
        mCachedObject.Add(go);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        AddCanvasCom(SourceObj);
        mRootObject.Add(SourceObj);

        var pos = Utility.FindGameObject(go, "pos");
        mTipsObject.Add(pos);

        return true;
    }

    public bool AddOpenEyes()
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kOpenEyesPath);
        mCachedObject.Add(go);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        return true;
    }

    public GameObject AddWaitTips(GameObject buttonRoot, bool mbPathThrough)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kWaitTipsPath);
        mCachedObject.Add(go);

        if(buttonRoot != null)
        {
            AddCanvasCom(buttonRoot);
        }

        var Pos = Utility.FindGameObject(go, "Pos");

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        if(mbPathThrough)
        {
            var cover = Utility.FindGameObject(go, "cover");
            cover.GetComponentInChildren<Image>().raycastTarget = false;
        }

        mTipsObject.Add(Pos);

        if(buttonRoot != null)
        {
            mRootObject.Add(buttonRoot);
        }
       
        return Pos;
    }

    public void AddWaitTips()
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kWaitTipsPath);
        mCachedObject.Add(go);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        GameObject Pos = Utility.FindGameObject(go, "Pos");
        Pos.SetActive(false);

        GameObject cover = Utility.FindGameObject(go, "cover");
        cover.GetComponent<Image>().color = new Color(1,1,1,0);
    }

    public GameObject AddCover()
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kCoverPath);
        mCachedObject.Add(go);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        return go;
    }

    public bool AddNewIconUnlock(string LoadResFile, string TargetRootPath, string IconPath, string IconName)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(LoadResFile);
        if(go == null)
        {
            return false;
        }

        mCachedObject.Add(go);

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        if(TargetRootPath != "")
        {
            GameObject TargetPosobj = Utility.FindGameObject(TargetRootPath);
            if (TargetPosobj != null)
            {
                GameObject bgObj = Utility.FindGameObject(go, "bg");
                if (bgObj != null)
                {
                    RectTransform child = bgObj.GetComponent<RectTransform>();
                    RectTransform target = TargetPosobj.GetComponent<RectTransform>();

                    Vector3 oriPos = target.position;

                    Tweener doTweener = DOTween.To(() => child.position, r => { child.position = r; }, oriPos, 1.0f);
                    doTweener.SetDelay(1.0f);
                    doTweener.SetEase(Ease.Linear);
                }
            }
        }

        if (IconPath != "")
        {
            GameObject Icon = Utility.FindGameObject(go, "bg/Icon");
            if (Icon == null)
            {
                return false;
            }

            //Sprite sprite = AssetLoader.instance.LoadRes(IconPath, typeof(Sprite)).obj as Sprite;
            //if (sprite == null)
            //{
            //    return false;
            //}

            //var IconCom = Icon.GetComponent<Image>();
            //IconCom.sprite = sprite;
            Image IconCom = Icon.GetComponent<Image>();
            ETCImageLoader.LoadSprite(ref IconCom, IconPath);
        }

        if(IconName != "")
        {
            GameObject Name = Utility.FindGameObject(go, "bg/Name");
            if (Name == null)
            {
                return false;
            }

            var NameCom = Name.GetComponent<Text>();
            NameCom.text = IconName;
        }

        return true;
    }

    public bool AddEffect(string LoadResFile)
    {
        GameObject NewEffect = AssetLoader.instance.LoadResAsGameObject(LoadResFile);
        if (NewEffect == null)
        {
            return false;
        }

        mCachedObject.Add(NewEffect);

        Utility.AttachTo(NewEffect, GameClient.ClientSystemManager.instance.TopLayer);
        NewEffect.transform.SetAsLastSibling();

        return true;
    }

    public GameObject AddTouchedTips(GameObject buttonRoot, UnityAction action, UnityAction up)
    {
        // NewbieGuideButtonFrame
        var go = AssetLoader.instance.LoadResAsGameObject(kTouchTipsPath);
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Drag");
        var buttonCom = button.GetComponent<ETCTouchPad>();
        buttonCom.onTouchStart.AddListener(action);
        buttonCom.onTouchUp.AddListener(up);
        buttonCom.onTouchUp.AddListener(() => { _complete(buttonCom); });

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        mTipsObject.Add(button);
        mRootObject.Add(buttonRoot);

        return button;
    }

    public GameObject AddDragTips(GameObject root, UnityAction<Vector2> move, UnityAction up,  eNewbieGuideAnchor anchor)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kDragTipsPath);
        mCachedObject.Add(go);

        var button = Utility.FindGameObject(go, "Drag");
        var buttonCom = button.GetComponent<ETCTouchPad>();

        var buttonImage = button.GetComponent<Image>();
        buttonImage.enabled = false;

        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        //var tips = Utility.FindGameObject(button, "Finger");
        //DOTween.To(
        //    () => Vector3.zero,
        //	r => tips.transform.localPosition = r,
        //	new Vector3(200, 0, 0),
        //	1).SetEase(Ease.OutQuad).SetLoops(-1);

        //mTipsObject.Add(button);
        //mRootObject.Add(root);

        _updateTipPosition(button, root);

        return button;
    }

    public GameObject AddTextTips(GameObject root, eNewbieGuideAnchor anchor, string content, TextTipType eTextTipType = TextTipType.TextTipType_One, Vector3 localPos = new Vector3())
    {
        if (root == null)
        {
            Logger.LogError("root is nil");
            return null;
        }

        if(string.IsNullOrEmpty(content))
        {
            return null;
        }

        GameObject go = null;
        if(eTextTipType == TextTipType.TextTipType_One)
        {
            go = AssetLoader.instance.LoadResAsGameObject(kTextTipsOnePath);
        }
		else if(eTextTipType == TextTipType.TextTipType_Two)
		{
			go = AssetLoader.instance.LoadResAsGameObject(kTextTipsTwoPath);
		}
		else if(eTextTipType == TextTipType.TextTipType_Three)
		{
			go = AssetLoader.instance.LoadResAsGameObject(kTextTipsThreePath);
		}
        else
        {
            go = AssetLoader.instance.LoadResAsGameObject(kTextTipsOnePath);
        }

        if(go == null)
        {
            return null;
        }

        _updateAnchor(go.GetComponent<RectTransform>(), anchor, localPos);

        var text = Utility.FindGameObject(go, "Text");
        var textCom = text.GetComponent<Text>();
        textCom.text = content;

        Utility.AttachTo(go, root);
   
        mCachedObject.Add(go);

        return go;
    }

    public void AddLoopTips(GameObject root, eNewbieGuideAnchor anchor)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(kLoopTipsPath);
        mCachedObject.Add(go);
    }

    private void _updateTipPosition(GameObject tips, GameObject root)
    {
        var targetRect = root.GetComponent<RectTransform>();
        var tipsRect = tips.GetComponent<RectTransform>();

        tipsRect.position = targetRect.position;
        tipsRect.pivot = targetRect.pivot;

        tipsRect.anchorMin = new Vector2(0.5f, 0.5f);
        tipsRect.anchorMax = new Vector2(0.5f, 0.5f);

        var rect = targetRect.rect;
        tipsRect.sizeDelta = new Vector2(rect.width, rect.height);       
    }

    private void _tryPauseBattle()
    {
//        var main = BattleMain.instance.Main;
//        if (main != null)
//        {
//            main.state = BeSceneState.onPause;
//        }
    }

    private void _tryResumeBattle()
    {
//        var main = BattleMain.instance.Main;
//        if (main != null)
//        {
//            main.state = BeSceneState.onReady;
//        }
    }

    private void _tryResumeBattleEnemyAI()
    {
        var main = BattleMain.instance.Main;
        if (main != null)
        {
            main.isTickAI = true;
        }
    }

    private void _tryPauseBattleEnemyAI()
    {
        var main = BattleMain.instance.Main;
        if (main != null)
        {
            main.isTickAI = false;
        }
    }

    public void AddFingerTips()
    {
    }

    public bool AddDialog(int id, UnityAction action)
    {
        var task = new GameClient.TaskDialogFrame.OnDialogOver();
        task.AddListener(_complete);

        if (action != null)
        {
            task.AddListener(action);
        }

        GameClient.MissionManager.GetInstance().CloseAllDialog();

        GameClient. IClientFrame fra = GameClient.MissionManager.GetInstance().CreateDialogFrame(id, 0, task);

        if (fra == null)
        {
            return false;
        }

        return true;
    }

    public void RemoveButtonTips()
    {
    }

    public void RemoveFingerTips()
    {
    }

    private void _updateAnchor(RectTransform rect, eNewbieGuideAnchor anchor, Vector3 localPos = new Vector3())
    {
        switch (anchor)
        {
            case eNewbieGuideAnchor.Buttom:
                {
                    rect.anchorMin = new Vector2(0.5f, 0.0f);
                    rect.anchorMax = new Vector2(0.5f, 0.0f);
                    rect.pivot = new Vector2(0.5f, 1.0f);

                    break;
                }
            case eNewbieGuideAnchor.Top:
                {
                    rect.anchorMin = new Vector2(0.5f, 1.0f);
                    rect.anchorMax = new Vector2(0.5f, 1.0f);
                    rect.pivot = new Vector2(0.5f, 0.0f);

                    break;
                }
            case eNewbieGuideAnchor.Left:
                {
                    rect.anchorMin = new Vector2(0.0f, 0.5f);
                    rect.anchorMax = new Vector2(0.0f, 0.5f);
                    rect.pivot = new Vector2(1.0f, 0.5f);

                    break;
                }
            case eNewbieGuideAnchor.Right:
                {
                    rect.anchorMin = new Vector2(1.0f, 0.5f);
                    rect.anchorMax = new Vector2(1.0f, 0.5f);
                    rect.pivot = new Vector2(0.0f, 0.5f);

                    break;
                }
            case eNewbieGuideAnchor.TopLeft:
                {
                    rect.anchorMin = new Vector2(0.0f, 1.0f);
                    rect.anchorMax = new Vector2(0.0f, 1.0f);
                    rect.pivot = new Vector2(1.0f, 0.0f);

                    break;
                }
            case eNewbieGuideAnchor.TopRight:
                {
                    rect.anchorMin = new Vector2(1.0f, 1.0f);
                    rect.anchorMax = new Vector2(1.0f, 1.0f);
                    rect.pivot = new Vector2(0.0f, 0.0f);

                    break;
                }
            case eNewbieGuideAnchor.ButtomLeft:
                {
                    rect.anchorMin = new Vector2(0.0f, 0.0f);
                    rect.anchorMax = new Vector2(0.0f, 0.0f);
                    rect.pivot = new Vector2(1.0f, 1.0f);

                    break;
                }
            case eNewbieGuideAnchor.ButtomRight:
                {
                    rect.anchorMin = new Vector2(1.0f, 0.0f);
                    rect.anchorMax = new Vector2(1.0f, 0.0f);
                    rect.pivot = new Vector2(0.0f, 1.0f);

                    break;
                }
            case eNewbieGuideAnchor.Center:
                {
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.0f);

                    break;
                }
        }

        rect.localPosition = localPos;
    }

    private void _updateFingerDirection(RectTransform rect, eNewbieGuideAnchor anchor)
    {
        switch (anchor)
        {
            case eNewbieGuideAnchor.Buttom:
                {
                    Quaternion formal = rect.localRotation;
                    formal.z = -40f;

                    rect.localRotation = formal;

                    break;
                }
            case eNewbieGuideAnchor.Top:
                {
                    Quaternion formal = rect.localRotation;
                    formal.z = 140f;

                    rect.localRotation = formal;

                    break;
                }
            case eNewbieGuideAnchor.Left:
                {
                    Quaternion formal = rect.localRotation;
                    formal.z = 122f;

                    rect.localRotation = formal;

                    break;
                }
            case eNewbieGuideAnchor.Right:
                {
                    Quaternion formal = rect.localRotation;
                    formal.z = 55f;

                    rect.localRotation = formal;

                    break;
                }
            case eNewbieGuideAnchor.TopLeft:
                {
                    rect.localScale = new Vector3(-1, -1, 1);
                    rect.localRotation = Quaternion.Euler(0, 0, 0);

                    break;
                }
            case eNewbieGuideAnchor.TopRight:
                {
                    rect.localScale = new Vector3(1, -1, 1);
                    rect.localRotation = Quaternion.Euler(0, 0, 0);

                    break;
                }
            case eNewbieGuideAnchor.ButtomLeft:
                {
                    rect.localScale = new Vector3(-1, 1, 1);         
                    rect.localRotation = Quaternion.Euler(0, 0, 0);

                    break;
                }
            case eNewbieGuideAnchor.ButtomRight:
                {
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.localRotation = Quaternion.Euler(0, 0, 0);

                    break;
                }
            case eNewbieGuideAnchor.Center:
                {
                    Quaternion formal = rect.localRotation;
                    formal.z = 45f;

                    rect.localRotation = formal;

                    break;
                }
        }
    }
}
