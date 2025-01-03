/***********************************************
				EasyTouch Controls
	Copyright © 2016 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class ETCJoystick : ETCBase,IPointerEnterHandler,IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler {
		
	#region Unity Events
	[System.Serializable] public class OnMoveStartHandler : UnityEvent{}
	[System.Serializable] public class OnMoveSpeedHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveEndHandler : UnityEvent{ }

	[System.Serializable] public class OnTouchStartHandler : UnityEvent{}
	[System.Serializable] public class OnTouchUpHandler : UnityEvent{ }

	[System.Serializable] public class OnDownUpHandler : UnityEvent{ }
	[System.Serializable] public class OnDownDownHandler : UnityEvent{ }
	[System.Serializable] public class OnDownLeftHandler : UnityEvent{ }
	[System.Serializable] public class OnDownRightHandler : UnityEvent{ }

	[System.Serializable] public class OnPressUpHandler : UnityEvent{ }
	[System.Serializable] public class OnPressDownHandler : UnityEvent{ }
	[System.Serializable] public class OnPressLeftHandler : UnityEvent{ }
	[System.Serializable] public class OnPressRightHandler : UnityEvent{ }


	[SerializeField] public OnMoveStartHandler onMoveStart;
	[SerializeField] public OnMoveHandler onMove;
	[SerializeField] public OnMoveSpeedHandler onMoveSpeed;
	[SerializeField] public OnMoveEndHandler onMoveEnd;

	[SerializeField] public OnTouchStartHandler onTouchStart;
	[SerializeField] public OnTouchUpHandler onTouchUp;

	[SerializeField] public OnDownUpHandler OnDownUp;
	[SerializeField] public OnDownDownHandler OnDownDown;
	[SerializeField] public OnDownLeftHandler OnDownLeft;
	[SerializeField] public OnDownRightHandler OnDownRight;

	[SerializeField] public OnDownUpHandler OnPressUp;
	[SerializeField] public OnDownDownHandler OnPressDown;
	[SerializeField] public OnDownLeftHandler OnPressLeft;
	[SerializeField] public OnDownRightHandler OnPressRight;
	#endregion

	#region Enumeration
	public enum JoystickArea { UserDefined,FullScreen, Left,Right,Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight};
	public enum JoystickType {Dynamic, Static};
	public enum RadiusBase {Width, Height, UserDefined};
	#endregion

	#region Members

	#region Public members
	public JoystickType joystickType;
	public bool allowJoystickOverTouchPad;
	public RadiusBase radiusBase;
	public float radiusBaseValue;
	public ETCAxis axisX;
	public ETCAxis axisY;
	public RectTransform thumb;
    public RectTransform direction;
    public JoystickArea joystickArea;
	public RectTransform userArea;
    public GameObject dirObj;
    public bool isTurnAndMove = false;
	public float tmSpeed = 10;
	public float tmAdditionnalRotation = 0;
	public AnimationCurve tmMoveCurve;
	public bool tmLockInJump = false;
	private Vector3 tmLastMove;

    public int maxOffsetX = -200;
    public int maxOffsetY = -100;
    public int initMaxOffsetX = -200;
    public int initMaxOffsetY = -100;

    public RectTransform directionNew;

    #endregion

    #region Private members
    private Vector2 thumbPosition;
	private bool isDynamicActif;
    private bool needUpdateDynamic;
    
	private Vector2 tmpAxis;
	private Vector2 OldTmpAxis;
	private bool isOnTouch;


	#endregion

	#region Joystick behavior option
	[SerializeField]
	private bool isNoReturnThumb;
	public bool IsNoReturnThumb {
		get {
			return isNoReturnThumb;
		}
		set {
			isNoReturnThumb = value;
		}
	}	

	private Vector2 noReturnPosition;
	private Vector2 noReturnOffset;
	
	private Vector2 initJoyPosition;

	[SerializeField]
	private bool isNoOffsetThumb;
	public bool IsNoOffsetThumb {
		get {
			return isNoOffsetThumb;
		}
		set {
			isNoOffsetThumb = value;
		}
	}
	#endregion

	#region Inspector


	#endregion

	#endregion
	
	#region Constructor
	public ETCJoystick(){
		joystickType = JoystickType.Static;
		allowJoystickOverTouchPad = false;
		radiusBase = RadiusBase.Width;

		axisX = new ETCAxis("Horizontal");
		axisY = new ETCAxis("Vertical");
	
		_visible = true;
		_activated = true;

		joystickArea = JoystickArea.FullScreen;

		isDynamicActif = false;
		isOnDrag = false;
		isOnTouch = false;

		axisX.unityAxis = "Horizontal";
		axisY.unityAxis = "Vertical";

		enableKeySimulation = true;

		isNoReturnThumb = false;

		showPSInspector = false;
		showAxesInspector = false;
		showEventInspector = false;
		showSpriteInspector = false;
	}
	#endregion

	#region Monobehaviours Callback
	
	void ResetJoyStick()
	{
		cachedRectTransform.anchoredPosition = initJoyPosition;//new Vector3(-756,-282);
		thumbPosition = Vector2.zero;
	}
	
	public void SetInitPosition(Vector3 pos)
    {
		initJoyPosition = pos;
	}

	public void ChangeMaxOffset(int offsetX, int offsetY)
	{
		maxOffsetX = initMaxOffsetX + offsetX;
		maxOffsetY = initMaxOffsetY + offsetY;
	}
	
	private void OnValidate()
	{
		initMaxOffsetX = maxOffsetX;
		initMaxOffsetY = maxOffsetY;
	}

	protected override void Awake (){

		base.Awake ();

		if (joystickType == JoystickType.Dynamic){
			this.rectTransform().anchorMin = new Vector2(0.5f,0.5f);
			this.rectTransform().anchorMax = new Vector2(0.5f,0.5f);
            // comment the code line
			//this.rectTransform().SetAsLastSibling();
			//visible = true;
		}

		if (allowSimulationStandalone && enableKeySimulation && !Application.isEditor /*&& joystickType!=JoystickType.Dynamic*/){
			SetVisible(visibleOnStandalone);
		}

        directionNew = Utility.FindComponent<RectTransform>(this.gameObject, "Direction");

        //initJoyPosition = new Vector2(500,100);//cachedRectTransform.anchoredPosition;
    }

	public override void Start(){
	
		axisX.InitAxis();
		axisY.InitAxis();

		if (enableCamera){
			InitCameraLookAt();
		}

		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;

		noReturnPosition = thumb.position;

		pointId = -1;

		if (joystickType == JoystickType.Dynamic){
			//visible = false;
		}

		base.Start();

		// Init Camera position
		if (enableCamera && cameraMode == CameraMode.SmoothFollow){
			if (cameraTransform && cameraLookAt){
				cameraTransform.position = cameraLookAt.TransformPoint( new Vector3(0,followHeight,-followDistance));
				cameraTransform.LookAt( cameraLookAt);
			}
		}

		if (enableCamera && cameraMode == CameraMode.Follow){
			if (cameraTransform && cameraLookAt){
				cameraTransform.position = cameraLookAt.position + followOffset;
				cameraTransform.LookAt( cameraLookAt.position);
			}
		}
		
		initJoyPosition =  cachedRectTransform.anchoredPosition;
		//UnityEngine.Debug.LogWarningFormat("cachedRectTransform {0} {1}",initJoyPosition.x,initJoyPosition.y);
	}


	public override void Update (){

		base.Update ();

        #region dynamic joystick
        /*
		if (joystickType == JoystickType.Dynamic && needUpdateDynamic && _activated){
			Vector2 localPosition = Vector2.zero;
			Vector2 screenPosition = Vector2.zero;
			
			if (isTouchOverJoystickArea(ref localPosition, ref screenPosition)){
				
				GameObject overGO = GetFirstUIElement( screenPosition);
				
				if (overGO == null || (allowJoystickOverTouchPad && overGO.GetComponent<ETCTouchPad>()) || (overGO != null && overGO.GetComponent<ETCArea>() ) ) {

					Logger.LogErrorFormat("Update cachedRectTransform.anchoredPosition:localPosition:{0} screenPosition:{1}", localPosition, screenPosition);
					cachedRectTransform.anchoredPosition = localPosition;
					needUpdateDynamic = false;
				}
			}
		}*/

        #endregion

        UpdateDirectionNew();
    }

    void UpdateDirectionNew()
    {
        if (directionNew == null)
        {
            return;
        }

        float l = Mathf.Sqrt(thumbPosition.x * thumbPosition.x + thumbPosition.y * thumbPosition.y);
        float redians = Mathf.Acos(thumbPosition.x / l);
        short nDegree = (short)(Mathf.Rad2Deg * redians);
        if (thumbPosition.y < -0f)
        {
            nDegree = (short)(360 - nDegree);
        }

        directionNew.localEulerAngles = new Vector3(0, 0, nDegree - 90);
        directionNew.gameObject.CustomActive(isOnTouch);
    }

    public override void LateUpdate (){

		if (enableCamera && !cameraLookAt ){
			InitCameraLookAt();
		}
		base.LateUpdate ();

	}

	private void InitCameraLookAt(){

		if (cameraTargetMode == CameraTargetMode.FromDirectActionAxisX){
			cameraLookAt = axisX.directTransform;
		}
		else if (cameraTargetMode == CameraTargetMode.FromDirectActionAxisY){
			cameraLookAt = axisY.directTransform;
			if (isTurnAndMove){
				cameraLookAt = axisX.directTransform;
			}
		}
		else if (cameraTargetMode == CameraTargetMode.LinkOnTag){
			GameObject tmpobj = GameObject.FindGameObjectWithTag(camTargetTag);
			if (tmpobj){
				cameraLookAt = tmpobj.transform;
			}
		}

		if (cameraLookAt)
			cameraLookAtCC = cameraLookAt.GetComponent<CharacterController>();
	}

	protected override void UpdateControlState (){
	
		UpdateJoystick();
	}

	#endregion
	
	#region UI Callback
	public void OnPointerEnter(PointerEventData eventData){

	

	}

	public void OnPointerDown(PointerEventData eventData){
		//Logger.LogErrorFormat("joystick OnPointerDown");
		if (/*joystickType == JoystickType.Dynamic &&*/ !isDynamicActif && _activated &&  pointId==-1){
			eventData.pointerDrag = gameObject;
			eventData.pointerPress = gameObject;

			isDynamicActif = true;
			pointId = eventData.pointerId;
			needUpdateDynamic = true;
			//Update();
		}

	/*	if (joystickType == JoystickType.Dynamic &&  !eventData.eligibleForClick){
			OnPointerUp( eventData );
		}
*/		
		onTouchStart.Invoke();
		pointId = eventData.pointerId;
		isOnDrag = true;
		if (isNoOffsetThumb){
			OnDrag( eventData);
		}
	}

	public void OnBeginDrag(PointerEventData eventData){
	}


    /// <summary>
    /// get screen position
    /// </summary>
    /// <returns></returns>
    private Vector2 _getThumbScreenPosition()
    {
        if (cachedCamera != null)
        {
            return RectTransformUtility.WorldToScreenPoint(cachedCamera, cachedRectTransform.position);
        }

        return (Vector2)cachedRectTransform.position;
    }

    private Vector3 _ScreenToWorldPoint(Vector3 pos)
    {
        if (cachedCamera != null)
        {
            return cachedCamera.ScreenToWorldPoint(pos);
        }

        return pos;
    }

    void UpdateThumb()
    {
        float radius = GetRadius();

        thumbPosition.x = Mathf.FloorToInt(thumbPosition.x);
        thumbPosition.y = Mathf.FloorToInt(thumbPosition.y);

        if (!axisX.enable)
        {
            thumbPosition.x = 0;
        }

        if (!axisY.enable)
        {
            thumbPosition.y = 0;
        }

        if (thumbPosition.magnitude > radius)
        {
            if (!isNoReturnThumb)
            {
                thumbPosition = thumbPosition.normalized * radius;
            }
            else
            {
                thumbPosition = thumbPosition.normalized * radius;
            }
        }
        thumb.anchoredPosition = thumbPosition;
    }

    void UpdateDirection()
    {
        float l = Mathf.Sqrt(thumbPosition.x * thumbPosition.x + thumbPosition.y * thumbPosition.y);
        float redians = Mathf.Acos(thumbPosition.x / l);
        short nDegree = (short)(Mathf.Rad2Deg * redians);
        if (thumbPosition.y < -0f)
        {
            nDegree = (short)(360 - nDegree);
        }
        if (GameClient.SettingManager.instance.GetJoystickDir() == InputManager.JoystickDir.EIGHT_DIR)
        {
            direction.localEulerAngles = new Vector3(0, 0, GetDirection(nDegree - 90) * 45);//箭头的方向是旋转了90度的

            direction.anchoredPosition = GetDirPos(nDegree);
        }
        else
        {
            direction.localEulerAngles = new Vector3(0, 0, nDegree - 90);

            direction.anchoredPosition = thumbPosition.normalized * 73;
        }
    }

    /// <summary>
    /// 获取方向
    /// </summary>
    /// <param name="degree"></param>
    /// <returns></returns>
    private int GetDirection(int degree)
    {
        degree = degree < 0 ? 360 + degree : degree;
        BeAIManager.MoveDir2 dir = InputManager.GetDir8(degree);
        return (int)dir;
    }

    /// <summary>
    /// 为了减少计算，直接算出答案
    /// </summary>
    /// <param name="degree"></param>
    /// <returns></returns>
    private Vector2 GetDirPos(int degree)
    {
        BeAIManager.MoveDir2 dir = (BeAIManager.MoveDir2)GetDirection(degree);

        switch (dir)
        {

            case BeAIManager.MoveDir2.RIGHT:
                return new Vector2(73, 0);
            case BeAIManager.MoveDir2.RIGHT_TOP:
                return new Vector2(51.6f, 51.6f);
            case BeAIManager.MoveDir2.TOP:
                return new Vector2(0, 73);
            case BeAIManager.MoveDir2.LEFT_TOP:
                return new Vector2(-51.6f, 51.6f);
            case BeAIManager.MoveDir2.LEFT:
                return new Vector2(-73, 0);
            case BeAIManager.MoveDir2.LEFT_DOWN:
                return new Vector2(-51.6f, -51.6f);
            case BeAIManager.MoveDir2.DOWN:
                return new Vector2(0, -73);
            case BeAIManager.MoveDir2.RIGHT_DOWN:
                return new Vector2(51.6f, -51.6f);
            case BeAIManager.MoveDir2.COUNT:
                break;
            default:
                break;
        }
        return Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData){
		
		if (pointId == eventData.pointerId && isOnDrag){

		//	Logger.LogErrorFormat("joystick OnDrag");

			isOnTouch = true;
            isDynamicActif = true;
			float radius =  GetRadius();

            // TODO fix the ScreenSpace - Camera bug!!!!!!!!!!!!!!!!!!!

		//	Logger.LogErrorFormat("position:({0},{1}) pressPosition:({2},{3})", eventData.position.x, eventData.position.y, eventData.pressPosition.x, eventData.pressPosition.y);

            if (!isNoReturnThumb){
				thumbPosition =  (eventData.position - eventData.pressPosition) / cachedRootCanvas.rectTransform().localScale.x;
			}
			else{
				thumbPosition = ((eventData.position - noReturnPosition) /cachedRootCanvas.rectTransform().localScale.x) + noReturnOffset;
			}

			/*
            if (isNoOffsetThumb){
                thumbPosition = (_ScreenToWorldPoint(eventData.position) - cachedRectTransform.position) / cachedRootCanvas.rectTransform().localScale.x;
			}
			*/
			
			float r = radius;
			
			if (joystickType == JoystickType.Dynamic && _activated){
				Vector2 planepos = cachedRectTransform.anchoredPosition;
				Vector2 tpos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),eventData.position,cachedRootCanvas.worldCamera,out tpos);

				float deltax = Mathf.Clamp(tpos.x - planepos.x,-r,r);
				float deltay = Mathf.Clamp(tpos.y - planepos.y,-r,r);
				planepos.x =   tpos.x  - deltax;
				planepos.y =   tpos.y  - deltay;

                if (planepos.x > maxOffsetX)
                    planepos.x = maxOffsetX;
                if (planepos.y > maxOffsetY)
                    planepos.y = maxOffsetY;

                cachedRectTransform.anchoredPosition = planepos;
                //Logger.LogErrorFormat("Pos: {0},{1}", planepos.x, planepos.y);
                thumbPosition = tpos - planepos;//(eventData.position - planepos) / cachedRootCanvas.rectTransform().localScale.x;
			}
			else if (joystickType == JoystickType.Static && _activated)
			{
				Vector2 planepos = cachedRectTransform.anchoredPosition;
				Vector2 tpos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),eventData.position,cachedRootCanvas.worldCamera,out tpos);


				float deltax = Mathf.Clamp(tpos.x - planepos.x,-r,r);
				float deltay = Mathf.Clamp(tpos.y - planepos.y,-r,r);

				thumbPosition.x += deltax;
				thumbPosition.y += deltay;
			}
			
			UpdateThumb();
          //  UpdateDirection();
            UpdateJoystick();
        }


	}

	public void OnPointerUp (PointerEventData eventData)
    {
	//	Logger.LogErrorFormat("joystick OnPointerUp");
		if (pointId == eventData.pointerId)
        {
			OnUp();
		}
        //else
        //{
        //    UnityEngine.Debug.LogFormat("pointId not equal the eventDataId {0} {1}", pointId, eventData.pointerId);
        //}
	}

	private void OnUp(bool real=true){
	
		if(isDynamicActif == false && joystickType == JoystickType.Dynamic)
		{
			return;
		}
		
		isOnDrag = false;
		isOnTouch = false;
		
		if (isNoReturnThumb){
			noReturnPosition = thumb.position;
			noReturnOffset = thumbPosition;
		}
		
		if (!isNoReturnThumb){
			thumbPosition =  Vector2.zero;
			thumb.anchoredPosition = Vector2.zero;
			
			axisX.axisState = ETCAxis.AxisState.None;
			axisY.axisState = ETCAxis.AxisState.None;
		}
		
		if (!axisX.isEnertia && !axisY.isEnertia){
			axisX.ResetAxis();
			axisY.ResetAxis();
			tmpAxis = Vector2.zero;
			OldTmpAxis = Vector2.zero;
			if (real){
				onMoveEnd.Invoke();
			}
		}
		
		if (joystickType == JoystickType.Dynamic){
			real = isDynamicActif;
			visible = true;
			isDynamicActif = false;
			ResetJoyStick();
		}

		pointId=-1;

		if (real){
			onTouchUp.Invoke();
		}
	}
	#endregion

	#region Joystick Update
	protected override void DoActionBeforeEndOfFrame (){
		axisX.DoGravity();
		axisY.DoGravity();
	}

    private Vector2 m_ThumbXLocalPosition = Vector2.zero;
    private Vector2 m_ThumbYLocalPosition = Vector2.zero;

    private void UpdateJoystick(){

		#region Unity axes
		if (enableKeySimulation && !isOnTouch && _activated && _visible ){

			float x = Input.GetAxis(axisX.unityAxis);
			float y= Input.GetAxis(axisY.unityAxis);

			if (!isNoReturnThumb){
				thumb.localPosition = Vector2.zero;
			}

			isOnDrag = false;

			if (x!=0)
            {
				isOnDrag = true;
                m_ThumbXLocalPosition.x = GetRadius() * x;
                m_ThumbXLocalPosition.y = thumb.localPosition.y;

                thumb.localPosition = m_ThumbXLocalPosition;
			}

			if (y!=0)
            {
				isOnDrag = true;
                m_ThumbYLocalPosition.x = thumb.localPosition.x;
                m_ThumbYLocalPosition.y = GetRadius() * y;
                thumb.localPosition = m_ThumbYLocalPosition;
			}

			thumbPosition = thumb.localPosition;
		}
		#endregion

		// Computejoystick value
		OldTmpAxis.x = axisX.axisValue;
		OldTmpAxis.y = axisY.axisValue;

		tmpAxis = thumbPosition / GetRadius();

		axisX.UpdateAxis( tmpAxis.x,isOnDrag, ETCBase.ControlType.Joystick,true);
		axisY.UpdateAxis( tmpAxis.y,isOnDrag, ETCBase.ControlType.Joystick,true);

		#region Move event
		if ((axisX.axisValue!=0 ||  axisY.axisValue!=0 ) && OldTmpAxis == Vector2.zero){
			onMoveStart.Invoke();
		}
		if (axisX.axisValue!=0 ||  axisY.axisValue!=0 ){

			if (!isTurnAndMove){
				// X axis
				if( axisX.actionOn == ETCAxis.ActionOn.Down && (axisX.axisState == ETCAxis.AxisState.DownLeft || axisX.axisState == ETCAxis.AxisState.DownRight)){
					axisX.DoDirectAction();
				}
				else if (axisX.actionOn == ETCAxis.ActionOn.Press){
					axisX.DoDirectAction();
				}

				// Y axis
				if( axisY.actionOn == ETCAxis.ActionOn.Down && (axisY.axisState == ETCAxis.AxisState.DownUp || axisY.axisState == ETCAxis.AxisState.DownDown)){
					axisY.DoDirectAction();
				}
				else if (axisY.actionOn == ETCAxis.ActionOn.Press){
					axisY.DoDirectAction();
				}
			}
			else{
				DoTurnAndMove();
			}
			onMove.Invoke( new Vector2(axisX.axisValue,axisY.axisValue));
			onMoveSpeed.Invoke( new Vector2(axisX.axisSpeedValue,axisY.axisSpeedValue));
		}
		else if (axisX.axisValue==0 &&  axisY.axisValue==0  && OldTmpAxis!=Vector2.zero) {
			onMoveEnd.Invoke();
		}		

		if (!isTurnAndMove){
			if (axisX.axisValue==0 &&  axisX.directCharacterController ){
				if (!axisX.directCharacterController.isGrounded && axisX.isLockinJump)
					axisX.DoDirectAction();
			} 

			if (axisY.axisValue==0 &&  axisY.directCharacterController ){
				if (!axisY.directCharacterController.isGrounded && axisY.isLockinJump)
					axisY.DoDirectAction();
			}
		}
		else{
			if ((axisX.axisValue==0 && axisY.axisValue==0) &&  axisX.directCharacterController ){
				if (!axisX.directCharacterController.isGrounded && tmLockInJump)
					DoTurnAndMove();
			}
		}

		#endregion



		#region Down & press event
		float coef =1;
		if (axisX.invertedAxis) coef = -1;
		if (Mathf.Abs(OldTmpAxis.x)< axisX.axisThreshold && Mathf.Abs(axisX.axisValue)>=axisX.axisThreshold){

			if (axisX.axisValue*coef >0){
				axisX.axisState = ETCAxis.AxisState.DownRight;
				OnDownRight.Invoke();
			}
			else if (axisX.axisValue*coef<0){
				axisX.axisState = ETCAxis.AxisState.DownLeft;
				OnDownLeft.Invoke();
			}
			else{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisX.axisState!= ETCAxis.AxisState.None) {
			if (axisX.axisValue*coef>0){
				axisX.axisState = ETCAxis.AxisState.PressRight;
				OnPressRight.Invoke();
			}
			else if (axisX.axisValue*coef<0){
				axisX.axisState = ETCAxis.AxisState.PressLeft;
				OnPressLeft.Invoke();
			}
			else{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}

		coef =1;
		if (axisY.invertedAxis) coef = -1;
		if (Mathf.Abs(OldTmpAxis.y)< axisY.axisThreshold && Mathf.Abs(axisY.axisValue)>=axisY.axisThreshold  ){
			
			if (axisY.axisValue*coef>0){
				axisY.axisState = ETCAxis.AxisState.DownUp;
				OnDownUp.Invoke();
			}
			else if (axisY.axisValue*coef<0){
				axisY.axisState = ETCAxis.AxisState.DownDown;
				OnDownDown.Invoke();
			}
			else{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisY.axisState!= ETCAxis.AxisState.None) {
			if (axisY.axisValue*coef>0){
				axisY.axisState = ETCAxis.AxisState.PressUp;
				OnPressUp.Invoke();
			}
			else if (axisY.axisValue*coef<0){
				axisY.axisState = ETCAxis.AxisState.PressDown;
				OnPressDown.Invoke();
			}
			else{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}
		#endregion

	}		
	#endregion

	#region Touch manager
	private bool isTouchOverJoystickArea(ref Vector2 localPosition, ref Vector2 screenPosition){
		
		bool touchOverArea = false;
		bool doTest = false;
		screenPosition = Vector2.zero;
		
		int count = GetTouchCount();
		int i=0;
		while (i<count && !touchOverArea){
			#if ((UNITY_ANDROID || UNITY_IOS || UNITY_WINRT || UNITY_BLACKBERRY) && !UNITY_EDITOR) 
			if (Input.GetTouch(i).phase == TouchPhase.Began){
				screenPosition = Input.GetTouch(i).position;
				doTest = true;
			}
			#else
			if (Input.GetMouseButtonDown(0)){
				screenPosition = Input.mousePosition;
				doTest = true;

			}
            #endif

            screenPosition = _ScreenToWorldPoint(screenPosition);
			
			if (doTest && isScreenPointOverArea(screenPosition, ref localPosition) ){
				touchOverArea = true;
			}
			
			i++;
		}
		
		return touchOverArea;
	}
	
	private bool isScreenPointOverArea(Vector2 screenPosition, ref Vector2 localPosition){
		
		bool returnValue = false;
		
		if (joystickArea != JoystickArea.UserDefined){
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),screenPosition,null,out localPosition)){
				
				switch (joystickArea){
				case JoystickArea.Left:
					if (localPosition.x<0){
						returnValue=true;
					}
					break;
					
				case JoystickArea.Right:
					if (localPosition.x>0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.FullScreen:
					returnValue = true;
					break;
					
				case JoystickArea.TopLeft:
					if (localPosition.y>0 && localPosition.x<0){
						returnValue = true;
					}
					break;
				case JoystickArea.Top:
					if (localPosition.y>0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.TopRight:
					if (localPosition.y>0 && localPosition.x>0){
						returnValue=true;
					}
					break;
					
				case JoystickArea.BottomLeft:
					if (localPosition.y<0 && localPosition.x<0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.Bottom:
					if (localPosition.y<0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.BottomRight:
					if (localPosition.y<0 && localPosition.x>0){
						returnValue = true;
					}
					break;
				}
			}
		}
		else{
			if (RectTransformUtility.RectangleContainsScreenPoint( userArea, screenPosition, cachedRootCanvas.worldCamera )){
				RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),screenPosition,cachedRootCanvas.worldCamera,out localPosition);
				returnValue = true;
			}
		}
		
		return returnValue;
		
	}
	
	private int GetTouchCount(){
		#if ((UNITY_ANDROID || UNITY_IOS || UNITY_WINRT || UNITY_BLACKBERRY) && !UNITY_EDITOR) 
		return Input.touchCount;
		#else
		if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)){
			return 1;
		}
		else{
			return 0;
		}
		#endif
	}
	#endregion

	#region Other private method
	public float GetRadius(){

		float radius =0;
		
		switch (radiusBase){
		case RadiusBase.Width:
			radius = cachedRectTransform.sizeDelta.x * 0.5f;
			break;
		case RadiusBase.Height:
			radius = cachedRectTransform.sizeDelta.y * 0.5f;
			break;
		case RadiusBase.UserDefined:
			radius = radiusBaseValue;
			break;
		}
		
		return radius;
	}

	protected override void SetActivated (){
	
		GetComponent<CanvasGroup>().blocksRaycasts = _activated;

		if (!_activated){
			OnUp(false);
		}
	}

	protected override void SetVisible (bool visible=true){

        //Debug.LogWarningFormat("SetVisible is {0}",visible);
		bool localVisible = _visible;
		if (!visible){
			localVisible = visible;
		}

		var go = GetComponent<Image>();
		if (go != null)
			go.enabled = localVisible;

		if (thumb != null)
		{
			go = thumb.GetComponent<Image>();
			if (go != null)
				go.enabled = localVisible;
		}

		var go2 = GetComponent<CanvasGroup>();
		if (go2 != null)
			go2.blocksRaycasts = _activated;

		/*
		GetComponent<Image>().enabled = localVisible;
		thumb.GetComponent<Image>().enabled = localVisible;
		GetComponent<CanvasGroup>().blocksRaycasts = _activated;
		*/
	}
	#endregion


	private void DoTurnAndMove(){

		float angle =Mathf.Atan2( axisX.axisValue,axisY.axisValue ) * Mathf.Rad2Deg;
		float speed = tmMoveCurve.Evaluate( new Vector2(axisX.axisValue,axisY.axisValue).magnitude) * tmSpeed;

		if (axisX.directTransform != null){

			axisX.directTransform.rotation = Quaternion.Euler(new Vector3(0,  angle + tmAdditionnalRotation,0));

			if (axisX.directCharacterController != null){
				if (axisX.directCharacterController.isGrounded || !tmLockInJump){
					Vector3 move = axisX.directCharacterController.transform.TransformDirection(Vector3.forward) *  speed;
					axisX.directCharacterController.Move(move* Time.deltaTime);
					tmLastMove = move;
				}
				else{
					axisX.directCharacterController.Move(tmLastMove* Time.deltaTime);
				}
			}
			else{
				axisX.directTransform.Translate(Vector3.forward *  speed * Time.deltaTime,Space.Self);
			}
		}

	}

	public void InitCurve(){
		axisX.InitDeadCurve();
		axisY.InitDeadCurve();
		InitTurnMoveCurve();
	}

	public void InitTurnMoveCurve(){
		tmMoveCurve = AnimationCurve.EaseInOut(0,0,1,1);
		tmMoveCurve.postWrapMode = WrapMode.PingPong;
		tmMoveCurve.preWrapMode = WrapMode.PingPong;
	}
}

