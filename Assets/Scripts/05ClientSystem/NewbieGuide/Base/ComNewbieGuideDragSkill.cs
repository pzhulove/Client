using UnityEngine.UI;
using GameClient;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ComNewbieGuideDragSkill : ComNewbieGuideBase
{
    #region vitrual
    public sealed override void StartInit(params object[] args)
    {
        base.StartInit(args);
        if (args != null)
        {
            guideDrag1 = new stButtonTipsGuideArg();
            int start = 0;
            guideDrag1.DecodeFromArgs(args, ref start);
            guideDrag2.DecodeFromArgs(args, ref start);

            int remainLength = args.Length - start;

            if (remainLength >= 1)
            {
                if ((eNewbieGuideAgrsName)args[start] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }

            if (remainLength >= 2)
            {
                if ((eNewbieGuideAgrsName)args[start + 1] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }

            if (remainLength >= 3)
            {
                if (args[8] as string != "")
                {
                    mHighLightPointPath = args[start + 2] as string;
                }
            }
        }
    }

    const string bgPath = "UIFlatten/Prefabs/NewbieGuide/BgMask";
    const string dragFingerPath = "UIFlatten/Prefabs/NewbieGuide/DragFinger";

    GameObject LoadGuidePrefab(string path,int canvasOrder = 805)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(path);
        go.name = "NewbieButtonType";
        mCachedObject.Add(go);
        Utility.AttachTo(go, GameClient.ClientSystemManager.instance.TopLayer);
        go.transform.SetAsLastSibling();

        if(go.GetComponent<GraphicRaycaster>() == null)
        {
            go.AddComponent<GraphicRaycaster>();
        }
       
        if (canvasOrder > 0)
        {
            AddCanvasComEx(go,false,canvasOrder);
        }

        return go;
    }

    private void SyncPosition(GameObject tips, GameObject root)
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

    DG.Tweening.Tween dragTween;
    IEnumerator TweenShow(RectTransform dragRect,RectTransform startRect,RectTransform endRect,float waittime,float loopDurarion)
    {
        yield return Yielders.GetWaitForSeconds(waittime);
        dragRect.gameObject.CustomActive(true);
        dragRect.position = startRect.position;

        dragTween = DOTween.To(
                    () => dragRect.position,
                    pos => dragRect.position = pos,
                    endRect.position,
                    loopDurarion).SetEase(Ease.OutQuad).SetLoops(-1);

        if(dragTween != null)
        {
            dragTween.target = dragRect.gameObject;
        }

        yield break;
    }
    protected sealed override GuideState _init()
    {
        IGameBind dragBind = _getBind(guideDrag1);
        IGameBind dropBind = _getBind(guideDrag2);
     
        if (dragBind == null || dropBind == null)
        {
            Logger.LogError("SkillGuide Error, 这里严格意义上讲不算报错，但要打印出来统计");
            return GuideState.Wait;
        }

        var dragRoot = dragBind.GetComponent<RectTransform>(guideDrag1.MComRoot);
        if(dragRoot == null)
        {
            Logger.LogError("SkillGuide Error, dragRoot == null");
            return GuideState.Exception;
        }
        AddCanvasCom(dragRoot.gameObject);
        var g1 = dragRoot.gameObject.AddComponent<GraphicRaycaster>();

        var dropRoot = dropBind.GetComponent<RectTransform>(guideDrag2.MComRoot);
        if(dropRoot == null)
        {
            Logger.LogError("SkillGuide Error, dropRoot == null");
            return GuideState.Exception;
        }
        AddCanvasCom(dropRoot.gameObject);
        var g2 = dropRoot.gameObject.AddComponent<GraphicRaycaster>();
        
        LoadGuidePrefab(bgPath, 800);

        var dragFinger = LoadGuidePrefab(dragFingerPath, 810);
        if(dragFinger == null)
        {
            Logger.LogError("SkillGuide Error, dragFinger == null");
            return GuideState.Exception;
        }
        dragFinger.CustomActive(false);

        var dragRect = dragFinger.transform.rectTransform();
        if(dragRect == null)
        {
            Logger.LogError("SkillGuide Error, dragRect == null");
            return GuideState.Exception;
        }
        
        StartCoroutine(TweenShow(dragRect,dragRoot,dropRoot,1.0f,2.0f));

        var dragTextTip = AddTextTips(
            AddButtonTipNoAutoComplete(dragRoot.gameObject,()=>{}),
            guideDrag1);

        var dropTextTip = AddTextTips(
            AddButtonTipNoAutoComplete(dropRoot.gameObject,()=>{}), 
            guideDrag2);

        if(dragTextTip == null || dropTextTip == null)
        {
            Logger.LogError("SkillGuide Error, dragTextTip == null or dropTextTip == null");
            return GuideState.Exception;
        }
        dropTextTip.CustomActive(false);

        DragGuideCom dragCom = null;
        DropGuideCom dropCom = null;

        var dragSkill = dragRoot.GetComponentInChildren<Drag_Me>();
        var dropSkill = dropRoot.GetComponentInChildren<Drop_Me>();

        if(dragSkill == null || dropSkill == null)
        {
            var dragDrug = dragRoot.GetComponentInChildren<ComDrag>();
            var dropDrug = dropRoot.GetComponentInChildren<ComDrag>();

            dragCom = dragDrug.gameObject.AddComponent<DragGuideCom>() as DragGuideCom;
            dropCom = dropDrug.gameObject.AddComponent<DropGuideCom>() as DropGuideCom;
        }
        else
        {
            dragCom = dragSkill.gameObject.AddComponent<DragGuideCom>() as DragGuideCom;
            dropCom = dropSkill.gameObject.AddComponent<DropGuideCom>() as DropGuideCom;
        }

        if(dragCom == null || dropCom == null)
        {
            Logger.LogError("SkillGuide Error, dragCom == null or dropCom == null");
            return GuideState.Exception;
        }

        dragCom.eventOnBeginDrag = data=>{
            step = 1;
            dropTextTip.CustomActive(true);
            dragTextTip.CustomActive(false);
        };
       
        dragCom.eventOnEndDrag = data=>{
            if(step == 1)
            {
                dropTextTip.CustomActive(false);
                dragTextTip.CustomActive(true);
            }
        };

        dropCom.eventOnDrop = data=>{

            if(dragTween != null)
            {
                DOTween.Kill(dragTween.target);
                dragTween = null;
            }
            _complete();
            Destroy(g1);
            Destroy(g2);
            step = 2;
        };

        return GuideState.Normal;
    }
    #endregion

    struct stButtonTipsGuideArg
    {
        string mFrameType;
        string mComRoot;
        string mTextTips;
        eNewbieGuideAnchor mAnchor;
        TextTipType mTextTipType;
        Vector3 mLocalPos;

        public string MFrameType { get { return mFrameType; } }
        public string MComRoot { get { return mComRoot; } }
        public string MTextTips { get { return mTextTips; } }
        public eNewbieGuideAnchor MAnchor { get { return mAnchor; } }
        public TextTipType MTextTipType { get { return mTextTipType; } }
        public Vector3 MLocalPos { get { return mLocalPos; } }

        public void DecodeFromArgs(object[] args, ref int start)
        {
            int index = start;
            int remainLength = args.Length - start;

            if (remainLength >= 2)
            {
                mFrameType = args[index] as string;
                mComRoot = args[index + 1] as string;
                index += 2;
            }

            if (remainLength >= 3)
            {
                mTextTips = args[index] as string;
                index++;
            }

            if (remainLength >= 4)
            {
                mAnchor = (eNewbieGuideAnchor)args[index];
                index++;
            }

            if (remainLength >= 5)
            {
                mTextTipType = (TextTipType)args[index];
                index++;
            }

            if (remainLength >= 6)
            {
                mLocalPos = (Vector3)args[index];
                index++;
            }

            start = index;
        }
    }


    stButtonTipsGuideArg guideDrag1;
    stButtonTipsGuideArg guideDrag2;

    IGameBind _getBind(stButtonTipsGuideArg arg)
    {
        IGameBind ibind = null;
        if (arg.MFrameType == "ClientSystemBattle")
        {
            ibind = ClientSystemManager.instance.CurrentSystem as IGameBind;
        }
        else
        {
            var frameManager = ClientSystemManager.instance as IClientFrameManager;

            if (frameManager.IsFrameOpen(arg.MFrameType))
            {
                ibind = frameManager.GetFrame(arg.MFrameType) as IGameBind;
            }
        }

        return ibind;
    }
    
    int step = 0;

    static public GameObject sCurrentDrapObject;

    GameObject AddTextTips(GameObject parent,stButtonTipsGuideArg tipsArg)
    {
        return AddTextTips(Utility.FindGameObject(parent, "Button"), tipsArg.MAnchor, tipsArg.MTextTips, tipsArg.MTextTipType, tipsArg.MLocalPos);
    }
}
