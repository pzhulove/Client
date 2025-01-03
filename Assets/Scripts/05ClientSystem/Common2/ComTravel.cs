using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
    public class ComTravel : MonoBehaviour
    {
        public delegate void OnItemVisisble(object script,object data);
        public delegate object OnItemCreate(GameObject go);
        public delegate void OnItemDestroy(object script);
        public delegate void OnTimeTrigger(float fTime);
        public delegate bool OnHitTest(object param,object data);
        public delegate void OnResult();
        public delegate void OnEnterHitFrame(object data);

        public OnItemVisisble onItemVisible;
        public OnItemCreate onItemCreate;
        public OnItemDestroy onItemDestroy;
        public OnTimeTrigger onTimeTrigger;
        public OnHitTest onHitTest;
        public OnResult onResult;
        public OnEnterHitFrame onEnterHitFrame;

        public GameObject goPrefab;
        public int padLeft,padX,iHitX,iHitMax;
        public float speedNormal = 240.0f;
        public float speedRunning = 1200.0f;

        public float curveTime = 5.0f;
        public float speedHigh = 100;
        public AnimationCurve curve;
        public float hitTime = 4.0f;

        Rect rectRoot;
        Rect rectPrefab;
        List<Block> templates = null;
        List<object> datas = null;
        int iHeadPos = 0;
        float nextPos = 0;

        public enum TravelMode
        {
            TMD_NORMAL,//常规段
            TMD_RUNNING,//高速奔跑段
            TMD_CURVE,//曲线减速段
            TMD_HIT,//命中段
            TMD_HITTING,//命中段
            TMD_HITTING_CACHED,//命中段
            TMD_READY_HIT_ADJUST,//命中末段
            TMD_READY_HIT_OVER,//命中后回调
            TMD_OVER,//命中后静止
        }
        TravelMode eTravelMode = TravelMode.TMD_NORMAL;

        float fStartCurveTime = 0.0f;
        bool bInitialize = false;
        public bool Initialized
        {
            get
            {
                return bInitialize;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        public void Initialize(List<object> datas,
            OnItemCreate onItemCreate,
            OnItemVisisble onItemVisible,
            OnItemDestroy onItemDestroy,
            OnHitTest onHitTest,
            OnEnterHitFrame onEnterHitFrame)
        {
            if(null == datas)
            {
                Logger.LogErrorFormat("Initialize datas is null !");
                return;
            }

            if(bInitialize)
            {
                return;
            }
            bInitialize = true;

            if (null != goPrefab)
            {
                rectRoot = getRect(gameObject);
                rectPrefab = getRect(goPrefab);
            }

            if (null == templates)
            {
                templates = GamePool.ListPool<Block>.Get();
            }

            this.datas = datas;
            this.onItemCreate = onItemCreate;
            this.onItemVisible = onItemVisible;
            this.onItemDestroy = onItemDestroy;
            iHeadPos = 0;
            nextPos = padLeft;
            this.onHitTest = onHitTest;
            this.onEnterHitFrame = onEnterHitFrame;
            createTemplates();
            setPos(nextPos);
        }

        public void SetTravelDatas(List<object> datas)
        {
            this.datas = datas;
            hit = null;

            for (int i = 0; i < templates.Count; ++i)
            {
                int iIndex = -1;
                if(this.datas != null && datas.Count > 0)
                {
                    iIndex = (iHeadPos + i) % datas.Count;
                }

                if(-1 != iIndex)
                {
                    if (null != templates[i] && null != onItemVisible)
                    {
                        onItemVisible(templates[i].bindScript, datas[iIndex]);
                    }
                }
            }
        }

        Rect getRect(GameObject go)
        {
            Rect ret = new Rect { x = 0, y = 0, width = 0, height = 0 };

            if(null != go)
            {
                var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(go.transform);
                return new Rect
                {
                    x = bounds.min.x,
                    y = bounds.min.y,
                    width = bounds.size.x,
                    height = bounds.size.y,
                };
            }

            return ret;
        }

        void createTemplates()
        {
            templates.Clear();
            if (null != goPrefab)
            {
                int iCnt = Mathf.FloorToInt(rectRoot.width / rectPrefab.width + 2);
                for(int i = 0; i < iCnt; ++i)
                {
                    GameObject current = GameObject.Instantiate(goPrefab);
                    if(null == current)
                    {
                        Logger.LogErrorFormat("create Template failed !");
                        continue;
                    }

                    current.CustomActive(true);
                    Utility.AttachTo(current, goPrefab.transform.parent.gameObject);
                    CanvasGroup canvasGroup = current.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 0.0f;
                    object bindObject = null;
                    if(null != onItemCreate)
                    {
                        bindObject = onItemCreate(current);
                    }

                    if(null != onItemVisible)
                    {
                        int iIndex = -1;
                        if(null != datas && datas.Count > 0)
                        {
                            iIndex = (iHeadPos + i) % datas.Count;
                        }
                        if(-1 != iIndex)
                        {
                            onItemVisible(bindObject, datas[iIndex]);
                        }
                    }

                    var local = new Block
                    {
                        goTarget = current,
                        canvasGroup = canvasGroup,
                        bindScript = bindObject,
                    };
                    local.isVisible = true;
                    templates.Add(local);
                }
                goPrefab.CustomActive(false);
            }
        }

        void setPos(float start)
        {
            if(null == templates)
            {
                Logger.LogErrorFormat("templates is null!");
                return;
            }
            nextPos = start;

            Vector3 prevPos;
            float fPos = start;
            for(int i = 0; i < templates.Count; ++i)
            {
                prevPos = templates[i].goTarget.transform.localPosition;

                templates[i].goTarget.transform.localPosition = new Vector3
                    (fPos,
                    templates[i].goTarget.transform.localPosition.y,
                    templates[i].goTarget.transform.localPosition.z);

                fPos += rectPrefab.width;
                fPos += padX;

                var currentItem = templates[i];
                currentItem.prevPos = prevPos.x;
                currentItem.curPos = fPos;
                currentItem.bDirty = false;
            }

            while(true)
            {
                bool bMark = false;
                if(templates.Count > 0)
                {
                    var currentItem = templates[0];
                    if (currentItem.isVisible)
                    {
                        if(!currentItem.bDirty)
                        {
                            if (currentItem.goTarget.transform.localPosition.x < -rectPrefab.width)
                            {
                                currentItem.isVisible = false;
                                templates.RemoveAt(0);
                                templates.Add(currentItem);
                                currentItem.bDirty = true;
                                bMark = true;
                                iHeadPos = (iHeadPos + 1) % datas.Count;
                                nextPos = (currentItem.goTarget.transform.localPosition.x + rectPrefab.width + padX);
                            }
                        }
                    }
                }
                if(!bMark)
                {
                    break;
                }
            }

            for (int i = 0; i < templates.Count; ++i)
            {
                var currentItem = templates[i];
                //enter in visible
                if (!currentItem.isVisible)
                {
                    if (currentItem.curPos <= rectRoot.width && currentItem.curPos >= -rectPrefab.width)
                    {
                        if (null != onItemVisible)
                        {
                            int iIndex = -1;
                            if (datas.Count > 0)
                            {
                                iIndex = (iHeadPos + i) % datas.Count;
                            }
                            if(-1 != iIndex)
                            {
                                onItemVisible(templates[i].bindScript, datas[iIndex]);
                            }
                        }
                        currentItem.isVisible = true;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(bInitialize)
            {
                for (int i = 0; i < templates.Count; ++i)
                {
                    int iIndex = (iHeadPos + i) % datas.Count;
                    if(templates[i].isVisible &&
                        templates[i].goTarget.transform.localPosition.x <= iHitX &&
                        iHitX <= templates[i].goTarget.transform.localPosition.x + rectPrefab.width)
                    {
                        if(null != onEnterHitFrame)
                        {
                            onEnterHitFrame(datas[iIndex]);
                        }
                    }
                }

                if (eTravelMode == TravelMode.TMD_HIT)
                {
                    int iIndex = -1;
                    iHitIndex = -1;
                    for (int i = 0; i < templates.Count && i < iHitMax; ++i)
                    {
                        iIndex = (iHeadPos + i) % datas.Count;
                        if (onHitTest(hit, datas[iIndex]))
                        {
                            if(templates[i].goTarget.transform.localPosition.x > iHitX)
                            {
                                iHitIndex = iIndex;
                                iIndex = i;
                                break;
                            }
                        }
                    }

                    if(-1 == iHitIndex)
                    {
                        setPos(nextPos - curve.Evaluate(1.0f) * speedHigh * Time.deltaTime);
                        //Logger.LogErrorFormat("travel hit !!!");
                    }
                    else
                    {
                        v1 = curve.Evaluate(1.0f) * speedHigh;
                        s1 = templates[iIndex].goTarget.transform.localPosition.x - iHitX;
                        t1 = 2.0f * s1 / v1;
                        a1 = v1 / t1;
                        if (t1 <= Time.deltaTime)
                        {
                            setPos(nextPos - s1);
                            eTravelMode = TravelMode.TMD_READY_HIT_ADJUST;
                            //Logger.LogErrorFormat("TravelMode.TMD_HIT ==> TravelMode.TMD_READY_HIT_ADJUST");
                            return;
                        }

                        float curPos = (v1 + (v1 - Time.deltaTime * a1)) * Time.deltaTime * 0.50f;
                        setPos(nextPos - curPos);
                        v1 = v1 - Time.deltaTime * a1;

                        eTravelMode = TravelMode.TMD_HITTING_CACHED;
                        //Logger.LogErrorFormat("TravelMode.TMD_HIT ==> TravelMode.TMD_HITTING_CACHED");
                    }
                }
                else if(eTravelMode == TravelMode.TMD_READY_HIT_OVER)
                {
                    if(null != onResult)
                    {
                        onResult();
                    }
                    eTravelMode = TravelMode.TMD_OVER;
                    s1 = 0.0f;
                    t1 = 0.0f;
                    a1 = 0.0f;
                    v1 = 0.0f;
                    //Logger.LogErrorFormat("TravelMode.TMD_OVER");
                }
                else if (eTravelMode == TravelMode.TMD_READY_HIT_ADJUST)
                {
                    Block find = null;
                    for(int i = 0; i < templates.Count; ++i)
                    {
                        if((iHeadPos + i) % datas.Count == iHitIndex)
                        {
                            find = templates[i];
                            break;
                        }
                    }

                    if(null == find)
                    {
                        eTravelMode = TravelMode.TMD_READY_HIT_OVER;
                        //Logger.LogErrorFormat("[TMD_READY_HIT_ADJUST] can not find turn to TravelMode.TMD_READY_HIT_OVER");
                        return;
                    }

                    s1 = find.goTarget.transform.localPosition.x - iHitX;
                    if(s1 != 0.0f)
                    {
                        setPos(nextPos - s1);
                        //Logger.LogErrorFormat("TravelMode.TMD_READY_HIT_ADJUST");
                    }
                    else
                    {
                        eTravelMode = TravelMode.TMD_READY_HIT_OVER;
                        //Logger.LogErrorFormat("TravelMode.TMD_READY_HIT_OVER");
                    }
                }
                else if(eTravelMode == TravelMode.TMD_HITTING_CACHED)
                {
                    Block find = null;
                    for (int i = 0; i < templates.Count; ++i)
                    {
                        if ((iHeadPos + i) % datas.Count == iHitIndex)
                        {
                            find = templates[i];
                            break;
                        }
                    }

                    if (null == find)
                    {
                        eTravelMode = TravelMode.TMD_READY_HIT_OVER;
                        //Logger.LogErrorFormat("[TMD_HITTING_CACHED] can not find turn to TravelMode.TMD_READY_HIT_OVER");
                        return;
                    }

                    s1 = find.goTarget.transform.localPosition.x - iHitX;
                    t1 = 2.0f * s1 / v1;
                    a1 = v1 / t1;
                    if(t1 <= Time.deltaTime)
                    {
                        setPos(nextPos - s1);
                        eTravelMode = TravelMode.TMD_READY_HIT_ADJUST;
                        //Logger.LogErrorFormat("TravelMode.TMD_READY_HIT_ADJUST");
                        return;
                    }

                    float curPos = (v1 + (v1 - Time.deltaTime * a1)) * Time.deltaTime * 0.50f;
                    setPos(nextPos - curPos);
                    v1 = v1 - Time.deltaTime * a1;

                    //Logger.LogErrorFormat("final step !!!");
                }
                else if (eTravelMode != TravelMode.TMD_HIT && eTravelMode != TravelMode.TMD_HITTING)
                {
                    setPos((nextPos - getSpeed() * Time.deltaTime));
                }
            }
        }

        float getSpeed()
        {
            if (eTravelMode == TravelMode.TMD_OVER)
            {
                return 0.0f;
            }

            if(eTravelMode == TravelMode.TMD_NORMAL)
            {
                return speedNormal;
            }

            if(eTravelMode == TravelMode.TMD_RUNNING)
            {
                return speedRunning;
            }

            if(eTravelMode == TravelMode.TMD_CURVE)
            {
                float delta = Time.time - fStartCurveTime;
                delta = Mathf.Min(delta, curveTime);
                float fCurveValue = curve.Evaluate(delta / curveTime);
                float curSpeed = fCurveValue * speedHigh;
                if (null != onTimeTrigger)
                {
                    onTimeTrigger(fCurveValue);
                }
                if (fStartCurveTime + curveTime <= Time.time)
                {
                    curSpeed = curve.Evaluate(1.0f) * speedHigh;
                    eTravelMode = TravelMode.TMD_HIT;
                    //Logger.LogErrorFormat("TravelMode.TMD_HIT");
                }
                return curSpeed;
            }

            Logger.LogErrorFormat("unexpected branch!!");
            return 0.0f;
        }

        float hitCacheSpeed = 120.0f;
        int iHitIndex = -1;
        //hit cached step
        float v1 = 0.0f;
        float a1 = 0.0f;
        float t1 = 0.0f;
        float s1 = 0.0f;

        public void StartCurve(OnTimeTrigger onTimeTrigger)
        {
            if(eTravelMode != TravelMode.TMD_RUNNING)
            {
                return;
            }
            eTravelMode = TravelMode.TMD_CURVE;

            this.onTimeTrigger = onTimeTrigger;
            fStartCurveTime = Time.time;
        }

        public void ChangeToNormal()
        {
            eTravelMode = TravelMode.TMD_NORMAL;
            fStartCurveTime = 0.0f;
            this.onTimeTrigger = null;
            onResult = null;
        }

        public void ChangedToRunning()
        {
            eTravelMode = TravelMode.TMD_RUNNING;
            fStartCurveTime = 0.0f;
            this.onTimeTrigger = null;
            onResult = null;
        }

        class Block
        {
            public GameObject goTarget;
            public object bindScript;
            public float prevPos;
            public float curPos;
            public bool isVisible
            {
                get
                {
                    return null != canvasGroup && canvasGroup.alpha >= 1.0f;
                }
                set
                {
                    canvasGroup.alpha = value ? 1.0f : 0.0f;
                }
            }
            public CanvasGroup canvasGroup;
            public bool bDirty = false;
        }

        void OnDestroy()
        {
            if(null != templates)
            {
                if(null != onItemDestroy)
                {
                    for (int i = 0; i < templates.Count; ++i)
                    {
                        if (null != templates[i])
                        {
                            onItemDestroy(templates[i].bindScript);
                        }
                    }
                }
                GamePool.ListPool<Block>.Release(templates);
                templates = null;
            }

            onItemCreate = null;
            onItemVisible = null;
            onItemDestroy = null;
            onHitTest = null;
            onEnterHitFrame = null;
        }

        object hit = null;
        public void Hit(object hit,OnResult onResult)
        {
            this.hit = null;
            this.onResult = null;

            if(null != onHitTest)
            {
                for (int i = 0; i < datas.Count; ++i)
                {
                    if(onHitTest(hit,datas[i]))
                    {
                        this.hit = hit;
                        this.onResult = onResult;
                        break;
                    }
                }
            }
        }
    }
}