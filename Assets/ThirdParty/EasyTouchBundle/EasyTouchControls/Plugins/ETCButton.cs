/***********************************************
				EasyTouch Controls
	Copyright © 2016 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class ETCButton : ETCBase, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler {

    //UIGray componentGray = null;

    #region Unity Events
    [System.Serializable] public class OnDownHandler : UnityEvent { }
    [System.Serializable] public class OnPressedHandler : UnityEvent { }
    [System.Serializable] public class OnPressedValueandler : UnityEvent<float> { }
    [System.Serializable] public class OnUPHandler : UnityEvent { }
    [System.Serializable] public class OnCoolDownStartHandler : UnityEvent { }
    [System.Serializable] public class OnCoolDownEndHandler : UnityEvent { }
    [System.Serializable] public class OnCoolDownStepHandler : UnityEvent<float> { }

    [System.Serializable] public class OnDragOutUpHandler : UnityEvent { }
    [System.Serializable] public class OnDragOutDownHandler : UnityEvent { }

    [SerializeField] public OnDownHandler onDown;
    [SerializeField] public OnPressedHandler onPressed;
    [SerializeField] public OnPressedValueandler onPressedValue;
    [SerializeField] public OnUPHandler onUp;

    [SerializeField] public OnCoolDownStartHandler onCoolDownStart;
    [SerializeField] public OnCoolDownEndHandler onCoolDownEnd;
    [SerializeField] public OnCoolDownStepHandler onCoolDownStep;

    [SerializeField] public OnDragOutUpHandler onDragOutUp;
    [SerializeField] public OnDragOutDownHandler onDragOutDown;
    #endregion

    #region Members

    #region Public members
    public ETCAxis axis;

    public Sprite normalSprite;
    public Color normalColor;
    public Color normalDisableColor;

    public Sprite pressedSprite;
    public Color pressedColor;

    public Sprite coolDownSprite;
    public Color coolDownColor;
    public Image coolDownImage;
    public RectTransform coolDownRect;

    public Sprite fgSprite;
    public Color fgColor;
    public Color fgCoolDownColor;
    public Text fgCoolDownText;
    public Image fgImage;
    public RectTransform fgRect;

    private Image imgBuffBg;
    private Image imgBuff;
    private GameObject goBuff;

    private Image darkImage;

    //private BeBuff buff;
    private int buffPID;
    private bool isBuffCD = false;

    private GameObject _inputEffectRoot = null;
    public GameObject InPutEffectRoot { set { _inputEffectRoot = value; } }
    public GameObject EffectRoot { get { return _effectRoot; } }
    
    private GameObject _effectRoot;

    #endregion

    #region Private members
    private Image cachedImage; 
	private bool isOnPress;
	private GameObject previousDargObject;
	private bool isOnTouch;

	#endregion

	#endregion

	#region Constructor
	public ETCButton(){

		axis = new ETCAxis( "Button");
		_visible = true;
		_activated = true;
		isOnTouch = false;


        enableKeySimulation = true;

		axis.unityAxis = "Jump";
		showPSInspector = true; 
		showSpriteInspector = false;
		showBehaviourInspector = false;
		showEventInspector = false;
	}
	#endregion

	#region Monobehaviour Callback
	protected override void Awake (){
		base.Awake ();

		cachedImage = GetComponent<UnityEngine.UI.Image>();

        fgCoolDownText.enabled = false;
    }

    public override void Start(){
		axis.InitAxis();
		base.Start();
		isOnPress = false;

		if (allowSimulationStandalone && enableKeySimulation && !Application.isEditor){
			SetVisible(visibleOnStandalone);
		}

		//var obj = Utility.FindGameObject(gameObject, "buff_bg", false);
		//if (obj != null)
		//{
		//	imgBuffBg = obj.GetComponent<Image>();
		//	goBuffBg = obj;
		//	goBuffBg.SetActive(false);
		//}

		var obj = Utility.FindGameObject(gameObject, "buff", false);
		if (obj != null)
		{
			imgBuff = obj.GetComponent<Image>();
			goBuff = obj;
			goBuff.SetActive(false);
		}

        var darkObj = Utility.FindGameObject(gameObject, "Image_Mask", false);
        if (darkObj != null)
        {
            darkImage = darkObj.GetComponent<Image>();
            darkObj.SetActive(true);
            mDarkImageColor = darkImage.color;
            SetDark(false);
        }
    }
	
    public override void OnDestroy()
    {
        normalSprite = null;

        pressedSprite = null;
        coolDownSprite = null;
        coolDownImage = null;
        coolDownRect = null;

        fgSprite = null;
        fgCoolDownText = null;
        fgImage = null;
        fgRect = null;

        imgBuffBg = null;
        imgBuff = null;


        //goBuffBg = null;
        goBuff = null;

        //buff = null;
        buffPID = 0;
        
        cachedImage = null;
        previousDargObject = null;

        base.OnDestroy();
    }

	protected override void UpdateControlState (){
		UpdateButton();
        //UpdateCoolDown();
	}

	protected override void DoActionBeforeEndOfFrame (){
		axis.DoGravity();
	}
    #endregion

    private bool bDark = true;
    private Color mDarkImageColor=Color.white;
    private Color _cdImageColor = new Color(1, 1, 1, 0.5f);
    public void SetDark(bool bActive, float alpha=1.0f)
    {
        if (darkImage == null) return;
        darkImage.color = bActive ? mDarkImageColor : Color.clear;
  //      if (componentGray == null && fgImage != null)
  //      {
  //          componentGray = fgImage.gameObject.AddComponent<UIGray>();
  //      }

        //      if(componentGray == null)
        //      {
        //          return;
        //      }

        //if (componentGray.enabled == bActive)
        //	return;

        //      componentGray.enabled = bActive;
    }

    #region cooldown
    private float fCoolTime = 0.0f;
    private float fCoolTimeRate = 0.0f;
    private bool bCoolDown = false;
        
    public void StartCoolDown(float time)
    {
        fCoolTime = time;
        fCoolTimeRate = 0.0f;
        bCoolDown = true;


        onCoolDownStart.Invoke();
    }

    public void StopCoolDown2()
    {
        bCoolDown = false;
    }

    public void StopCoolDown()
    {
        fCoolTimeRate = 0.99f;
        bCoolDown = true;
    }

    public void Clear()
    {

    }

    //public void SetFgImage(Sprite sp)
    //{
    //    fgImage.sprite = sp;
    //}
    public void SetFgImage(string spritePath, bool isMustExist = true)
    {
        // fgImage.sprite = sp;
        ETCImageLoader.LoadSprite(ref fgImage, spritePath, isMustExist);
    }

    public void SetFgImageScale(float scale=1.0f)
	{
		if (fgImage != null)
		{
			fgImage.transform.localScale = new Vector3(scale, scale, scale);
		}
	}

    private int mTotoalFakeCDTime = 0;
    public void UpdateFakeCoolDown(int totoalTime, int sumTime,bool isBuffSkill, BeActor actor)
    {
		UpdateBuffCD(actor);

        mTotoalFakeCDTime = sumTime;

        if (totoalTime >= mTotoalFakeCDTime)
        {
            StopFakeCoolDown(isBuffSkill);
        }
        else
        {
            //on start cooldown
            if (!bCoolDown)
            {
                bCoolDown = true;
                SetDark(true);
            }
				
            var image = coolDownImage;

            if (image == null)
            {
                image = cachedImage;
            }

            //fgImage.color = fgCoolDownColor;
            if (coolDownImage != null)
                coolDownImage.color = _cdImageColor;
                //coolDownImage.color = coolDownColor;
            if (image != null)
            	image.fillAmount =  1 - (totoalTime * 1.0f / mTotoalFakeCDTime);

			if (fgCoolDownText != null)
			{
				fgCoolDownText.enabled = true;
                float rate = (mTotoalFakeCDTime - totoalTime) / 1000.0f;
                RefreshCDText(rate);
            }
        }
    }


    private float m_LastRate = 0.0f;
    private string[] m_CDMap = new string[] { "0.0", "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9" };
    private Dictionary<int, string> m_CDTextDic = new Dictionary<int, string>();
    /// <summary>
    /// 刷新技能按钮CD文本
    /// </summary>
    /// <param name="rate"></param>
    private void RefreshCDText(float rate)
    {
        //Debug模式下显示 外网环境下不会勾选Debug
        if (Global.Settings.isDebug)
        {
            fgCoolDownText.text = rate.ToString("f1");
            return;
        }

        if(rate > 1)
        {
            if(Mathf.Abs(m_LastRate - rate) >= 1)
            {
                int key = (int)(rate);
                if (m_CDTextDic.ContainsKey(key))
                {
                    fgCoolDownText.text = m_CDTextDic[key];
                }
                else
                {
                    string str = key.ToString();
                    m_CDTextDic.Add(key,str);
                    fgCoolDownText.text = str;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            if(Mathf.Abs(m_LastRate - rate) >= 0.1f)
            {
                int index = (int)(rate * 10);
                if (index < 10)
                {
                    fgCoolDownText.text = m_CDMap[index];
                }
            }
            else
            {
                return;
            }
        }
        m_LastRate = rate;
    }

    private float _getEffectSize()
    {
        Rect rect = normalSprite.rect;
        float ratio = rect.width / rect.height;

        float size = 0;
        if (ratio >= 1)
        {
            size = this.rectTransform().rect.width;
        }
        else
        {
            size = this.rectTransform().rect.height;
        }

        return size;
    }

    public enum eEffectType
    {
        /// <summary>
        /// 点击特效
        /// </summary>
        onClick,
        /// <summary>
        /// CD完成特效(普通技能)
        /// </summary>
        onCDFinish,
        /// <summary>
        /// CD完成特效(Buff技能)
        /// </summary>
        onCDFinishBuff,
        /// <summary>
        /// 连续点击提示
        /// </summary>
        onContinue,
        /// <summary>
        /// 中断技能提示
        /// </summary>
        onBreak,
        /// <summary>
        /// 节能蓄力提示
        /// </summary>
        onCharge,
		/// 
		onSkillBuff,
		onSummonAccompy,
        /// <summary>
        /// 滑动操作提示
        /// </summary>
        onSlide,
        /// <summary>
        /// 新技能特效
        /// </summary>
        onNew,
    }

    private class CacheObject
    {
        public CacheObject(eEffectType t, GameObject go)
        {
            this.gameObject = go;
            this.type = t;
        }

        public GameObject gameObject { get; private set; }

        public eEffectType type { get; private set; }
    }

    private List<CacheObject> mCacheEffectObject = new List<CacheObject>();

    private const string _skillButtonEffectRootPath = "UIFlatten/Prefabs/ETCInput/SkillButtonEffectRoot";

    private void _loadEffect(eEffectType type, string path, bool needscale)
    {
		for(int i=0; i<mCacheEffectObject.Count; ++i)
		{
			if (mCacheEffectObject[i].type == type)
				return;
		}

        var obj = CGameObjectPool.instance.GetGameObject(path, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
        if (_effectRoot == null)
            _effectRoot = CGameObjectPool.instance.GetGameObject(_skillButtonEffectRootPath, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);

        if (_inputEffectRoot != null && _effectRoot != null)
        {
            Utility.AttachTo(obj, _effectRoot);
            Utility.AttachTo(_effectRoot, _inputEffectRoot);
            _effectRoot.transform.localPosition = gameObject.transform.localPosition;
            _effectRoot.transform.localScale = gameObject.transform.localScale;
            _effectRoot.name = gameObject.name;
        }
        else
            Utility.AttachTo(obj, gameObject);

        mCacheEffectObject.Add(new CacheObject(type, obj));

        if (needscale)
        {
            var size = _getEffectSize();
            var newScale = Vector3.one / 300 * size;
            //obj.transform.localScale = newScale;
        }
    }


	public void StartBuffCD(BeBuff buff)
	{
		if (isBuffCD)
			return;
		//goBuffBg.SetActive(true);
		goBuff.SetActive(true);

		imgBuff.fillAmount = 1.0f;

		//this.buff = buff;
        if (buff != null)
            this.buffPID = buff.PID;
		isBuffCD = true;
	}

	public void UpdateBuffCD(BeActor actor)
	{
        BeBuff buff = null;
        if (actor != null)
            buff = actor.buffController.GetBuffByPID(buffPID);
        
		if (isBuffCD && buff != null)
		{
			imgBuff.fillAmount = 1.0f - buff.GetProcess();
			if (imgBuff.fillAmount <= 0f)
			{
				StopBuffCD();
			}
		}
	}

	public void StopBuffCD()
	{
		if (!isBuffCD)
			return;

		//goBuffBg.SetActive(false);
		goBuff.SetActive(false);

		//buff = null;
		isBuffCD = false;
        buffPID = 0;
	}

    public static string skEffectOnClick = "UI/UIEffects/Skill_UI_Icon/Prefab/Skill_UI_Dianjifankui01";
    public static string skEffectOnCDFinish = "UI/UIEffects/Skill_UI_Icon/Prefab/Skill_UI_CD_End";
    public static string skEffectOnCDFinishBuff = "UI/UIEffects/Skill_UI_Icon/Prefab/Skill_UI_CD_End_buff";
		
	public void AddEffect(eEffectType type, bool isBuff=false)
    {

		if ((type == eEffectType.onBreak || type == eEffectType.onContinue) && isBuff)
		{
			type = eEffectType.onSkillBuff;
		}
        switch (type)
        {
            case eEffectType.onClick:
                _loadEffect(type, skEffectOnClick, true);
                break;
            case eEffectType.onCDFinish:
                _loadEffect(type, skEffectOnCDFinish, true);
                break;
            case eEffectType.onCDFinishBuff:
                _loadEffect(type, skEffectOnCDFinishBuff, true);
                break;
            case eEffectType.onContinue:
                _loadEffect(type, "Effects/Scene_effects/EffectUI/EffUI_Autoskill_hong_guo", true);
                break;
            case eEffectType.onBreak:
                _loadEffect(type, "Effects/Scene_effects/EffectUI/EffUI_Autoskill_hong_guo", true);
                break;
            case eEffectType.onCharge:
                _loadEffect(type, "UIFlatten/Prefabs/BattleUI/DungeonButtonStateCharge", false);
                break;
            case eEffectType.onSkillBuff:
                _loadEffect(type, "Effects/Scene_effects/EffectUI/EffUI_Autoskill_chixu", false);
                break;
            case eEffectType.onSummonAccompy:
                _loadEffect(type, "Effects/Scene_effects/EffectUI/EffUI_Autoskill_chixu_guo", false);
                break;
            case eEffectType.onSlide:
                _loadEffect(type, "UIFlatten/Prefabs/BattleUI/DungeonButtonStateSlide", false);
                break;
            case eEffectType.onNew:
                _loadEffect(type, "UIFlatten/Prefabs/Skill/SkillNewEffect", true);
                break;
            default:
                break;
        }
    }

    public void RemoveEffect(eEffectType type, bool isBuff=false)
    {
		if ((type == eEffectType.onBreak || type == eEffectType.onContinue) && isBuff)
		{
			type = eEffectType.onSkillBuff;
		}

		bool needRemove = false;
		for(int i=0; i<mCacheEffectObject.Count; ++i)
		{
			if (mCacheEffectObject[i].type == type)
			{
				needRemove = true;
				break;
			}
		}

		if (needRemove)
		{
			mCacheEffectObject.RemoveAll(x =>
				{
					if (x.type == type)
					{
						CGameObjectPool.instance.RecycleGameObject(x.gameObject);
						return true;
					}
					else
					{
						return false;
					}
				});
		}
    }

    public void StopFakeCoolDown(bool isBuffSkill = false)
    {
        if (null != fgImage)
            fgImage.color = fgColor;
        if (null != coolDownImage)
            coolDownImage.color = Color.clear;
        if (null != fgCoolDownText)
            fgCoolDownText.enabled = false;

        if (bCoolDown)
        {
            bCoolDown = false;
            eEffectType effectType = isBuffSkill ? eEffectType.onCDFinishBuff : eEffectType.onCDFinish;
            RemoveEffect(effectType);
            AddEffect(effectType);

            if (null != onCoolDownEnd)
                onCoolDownEnd.Invoke();

            SetDark(false);
        }
    }

    private void UpdateCoolDown()
    {
        var image = coolDownImage;

        if (image == null)
        {
            image = cachedImage;
        }

        if (bCoolDown)
        {
            fCoolTimeRate += ((int)(Time.deltaTime * 1000) / 1000f / fCoolTime);
            if (fCoolTimeRate >= 1.0f)
            {
                onCoolDownStep.Invoke(1.0f);
                // TODO
                // FIX IT
                image.fillAmount = 0f;

                onCoolDownEnd.Invoke();

                fgImage.color = fgColor;
                coolDownImage.color = Color.clear;

                fCoolTime = 0.0f;
                bCoolDown = false;
            }
            else
            {
                fgImage.color = fgCoolDownColor;
                coolDownImage.color = coolDownColor;

                image.fillAmount = 1 - fCoolTimeRate;
                onCoolDownStep.Invoke(fCoolTimeRate);
            }
        }
        else
        {
            fgImage.color = fgColor;
            coolDownImage.color = Color.clear;
        }
    }

    #endregion

    #region UI Callback
    public void OnPointerEnter(PointerEventData eventData){

		if (isSwipeIn && !isOnTouch){

			if (eventData.pointerDrag != null){
				if (eventData.pointerDrag.GetComponent<ETCBase>() && eventData.pointerDrag!= gameObject){
					previousDargObject=eventData.pointerDrag;
					//ExecuteEvents.Execute<IPointerUpHandler> (previousDargObject, eventData, ExecuteEvents.pointerUpHandler);
				}
			}

			eventData.pointerDrag = gameObject;
			eventData.pointerPress = gameObject;
			OnPointerDown( eventData );
		}
	}

	public void OnPointerDown(PointerEventData eventData){

		if (_activated && !isOnTouch){
			pointId = eventData.pointerId;

			axis.ResetAxis();
			axis.axisState = ETCAxis.AxisState.Down;

			isOnPress = false;
			isOnTouch = true;

            RemoveEffect(eEffectType.onClick);
            AddEffect(eEffectType.onClick);
            RemoveEffect(eEffectType.onNew);

			onDown.Invoke();
			ApllyState();
			axis.UpdateButton();
            if (ComDrugTipsBar.instance != null)
            {
                ComDrugTipsBar.instance.SetDrugColumnStat();
            }
		}
	}

	public void OnPointerUp(PointerEventData eventData){
		if (pointId == eventData.pointerId){

            if(!_activated)
            {
                return;
            }

            if (axis.axisState == ETCAxis.AxisState.PressDown
                || axis.axisState == ETCAxis.AxisState.PressUp )
            {
                axis.axisState = ETCAxis.AxisState.None;
                ApllyState();
                return;
            }

			isOnPress = false;
			isOnTouch = false;
			axis.axisState = ETCAxis.AxisState.Up;
			axis.axisValue = 0;
			onUp.Invoke();
			ApllyState();

			if (previousDargObject){
				ExecuteEvents.Execute<IPointerUpHandler> (previousDargObject, eventData, ExecuteEvents.pointerUpHandler);
				previousDargObject = null;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData){
		if (pointId == eventData.pointerId){
			if (axis.axisState == ETCAxis.AxisState.Press && !isSwipeOut){
                OnPointerUp(eventData);
            }
        }
	}
	#endregion

	#region Button Update
	private void UpdateButton(){

		if (axis.axisState == ETCAxis.AxisState.Down){

			isOnPress = true;
			axis.axisState = ETCAxis.AxisState.Press;
		}

		if (isOnPress){
			axis.UpdateButton();
			onPressed.Invoke();
			onPressedValue.Invoke( axis.axisValue);

		}

		if (axis.axisState == ETCAxis.AxisState.Up){
			isOnPress = false;
			axis.axisState = ETCAxis.AxisState.None;
		}


		if (enableKeySimulation && _activated && _visible && !isOnTouch){

			if (Input.GetButton( axis.unityAxis)&& axis.axisState ==ETCAxis.AxisState.None ){	
				axis.ResetAxis();
				onDown.Invoke();
				axis.axisState = ETCAxis.AxisState.Down;
			}

			if (!Input.GetButton(axis.unityAxis )&& axis.axisState == ETCAxis.AxisState.Press){
				axis.axisState = ETCAxis.AxisState.Up;
				axis.axisValue = 0;
				
				onUp.Invoke();
			}

			axis.UpdateButton();
			ApllyState();
		}
	}	
	#endregion

	#region Private Method
	protected override void SetVisible (bool forceUnvisible=false){
		bool localVisible = _visible;
		if (!visible){
			localVisible = visible;
		}
		GetComponent<Image>().enabled = localVisible;
	}

    private void ApllyState()
    {
        Color color;
        if (bDark) {
            color = normalColor;
        } else {
            color = normalDisableColor;
        }

        if (cachedImage)
        {
            switch (axis.axisState)
            {
                case ETCAxis.AxisState.Down:
                case ETCAxis.AxisState.Press:
                    cachedImage.sprite = pressedSprite;
                    cachedImage.color = pressedColor;
                    break;
                default:
                    cachedImage.sprite = normalSprite;
                    cachedImage.color = color;
                    break;
            }
        }
    }

	protected override void SetActivated(){

		if (!_activated){
			isOnPress = false;
			isOnTouch = false;
			axis.axisState = ETCAxis.AxisState.None;
			axis.axisValue = 0;
        }
        ApllyState();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (pointId == eventData.pointerId)
        {
        }
    }

    private float mDragOutLen = 120.0f;
    public float dragOutLen
    {
        get
        {
            return mDragOutLen;
        }

        set
        {
            mDragOutLen = value;
        }
    }


    public enum DragDivMode
    {
        UpDown,
        ToCorner,
    }

    public DragDivMode dragMode = DragDivMode.ToCorner;

    public void OnDrag(PointerEventData eventData)
    {
        if (isSwipeOut && pointId == eventData.pointerId)
        {
            if (axis.axisState == ETCAxis.AxisState.Press)
            {
                // 按钮中心位置
                Vector2 centPos = cachedRectTransform.position;

                // 拖拽向量
                Vector2 dir = (eventData.position - centPos) / cachedRootCanvas.rectTransform().localScale.x; ;

                // 划分按钮范围向量
                Vector2 divDir = Vector2.down;

                switch(dragMode)
                {
                    case DragDivMode.ToCorner:
                        divDir = new Vector2(anchorOffet.x, -anchorOffet.y);
                        break;
                    case DragDivMode.UpDown:
                        divDir = Vector2.down;
                        break;
                    default:
                        Logger.LogErrorFormat("undefine the drag mode {0}", dragMode);
                        break;
                }

                // 拖拽长度
                float len = dir.magnitude;

//                 if (len > dragOutLen)
//                 {
//                     /**
//                      * there are some question about why we should call on point up
//                      * please check the drag mode is good to run
//                      */
//                     OnPointerUp(eventData);
// 
//                     float up = Vector2.Dot(divDir.normalized, dir.normalized);
// 
//                     if (up < 0)
//                     {
//                         cachedImage.color = Color.red;
//                         onDragOutUp.Invoke();
//                         axis.axisState = ETCAxis.AxisState.PressUp;
//                     }
//                     else
//                     {
//                         cachedImage.color = Color.green;
//                         onDragOutDown.Invoke();
//                         axis.axisState = ETCAxis.AxisState.PressDown;
//                     }
// 
//                 }
            }
        }
    }

    /// <summary>
    /// 设置技能按钮显示与隐藏(包含特效)
    /// </summary>
    public void SetSkillBtnVisible(bool isShow)
    {
        gameObject.CustomActive(isShow);
        if (_effectRoot != null)
            _effectRoot.CustomActive(isShow);
    }
    #endregion
}
