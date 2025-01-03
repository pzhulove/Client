using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using GameClient;
using System.ComponentModel;
using UnityEngine.UI;
using HeroInput;
using UnityEngine.EventSystems;

public partial class InputManager
{
    #region Enum
    public enum ButtonMode
    {
        [Description("普通")]
        NORMAL,
        [Description("普通-8")]
        NORMALEIGHT,
    }
    #endregion

	public enum PressDir
	{
		NONE = -1,
		TOP = 0,
		DOWN = 1,
		LEFT = 2,
		RIGHT = 3
	}

    public enum EtcSkillMode
    {
        EIGHT = 0,
        NORMAL
    }

	public enum JoystickMode
	{
		DYNAMIC = 0,	//非固定摇杆
		STATIC = 1		//固定摇杆
	}

    public enum JoystickDir
    {
        MORE_DIR = 0,
        EIGHT_DIR = 1,
    }

    public enum RunAttackMode
    {
        NORMAL,
        QTE
    }

    public enum CameraShockMode
    {
        OPEN,
        CLOSE
    }
    //滑动设置
    public enum SlideSetting
    {
        NORMAL,
        SLIDE
    }

    //驱魔普攻
    public enum PaladinAttack
    {
        OPEN,
        CLOSE,
    }

    static string[] cEctSkillResPath = { "UIFlatten/Prefabs/ETCInput/ETCButtonsModeNormalEight", "UIFlatten/Prefabs/ETCInput/ETCButtonsModeNormalNew" };
    static string[] cEctSkillRootName = { "ETCButtonsModeNormalEight(Clone)", "ETCButtonsModeNormalNew(Clone)" };

    static public EtcSkillMode sEctSkillMode = EtcSkillMode.NORMAL;

    static public string GetEtcSkillRoot()
    {
        return cEctSkillRootName[(int)sEctSkillMode];
    }

    static public string GetEtcSkillResPath()
    {
        return cEctSkillResPath[(int)sEctSkillMode];
    }

    public static InputManager instance;

    #region Let't Go
    public void ForceJoyStickUp()
    {
        if (null == joystick)
        {
            return;
        }

        //UnityEngine.Debug.LogFormat("ExecuteEvents.Execute board cast");

        ExecuteEvents.Execute<IPointerDownHandler>(joystick.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute<IPointerUpHandler>(joystick.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
    }
    #endregion


    #region Property
    private int mPid = 0;

    public int mUseKeyCallBack = 0;

    private float mJoyX = 0.0f;
    private float mJoyY = 0.0f;

    public ButtonMode currentMode = Global.Settings.buttonType;

    #endregion

    #region Load Skill Slot Map

    public int GetJobID()
    {
        return PlayerBaseData.GetInstance().JobTableID;
    }

    private void _clearSlot()
    {
        mButtonMap.Clear();
        mDictSkillID.Clear();
    }


     public void ReloadButtons(int pid, BeActor actor, Dictionary<int, int> skillSlotDic,bool hideJump=false)
    {
        UnloadActorConfig();
        controllActor = actor;

        _clearSlot();

        _LoadSkillButton(pid, skillSlotDic);

        //mServerSlotMap = new Dictionary<int, int>(slotmap);

        
        LoadActorConfig(actor);

        Update(1);

        if (hideJump)
            HiddenJump();
        else
            ShowJump();
    }

    #endregion

    #region EasyTouch

    #region Button & Joystick UI
    private ETCJoystick mJoystick;
    public ETCJoystick joystick
    {
        get
        {
            /*            if (mJoystick == null)
                        {
                            Logger.LogError("joystick is nil");
                        }*/
            return mJoystick;
        }
    }

    private GameObject mObJoyStick = null;
	bool pressOne = false;
	int doublePressCheckDuration = 500;
	int doubleCheckAcc = int.MaxValue;
	bool doublePress = false;

	public JoystickMode joystickMode = JoystickMode.DYNAMIC;

    public Queue skillOnClickQue = new Queue();

    public GameObject GetJoyStick()
    {
        return mObJoyStick;
    }
    
	private void _loadJoystickUI(JoystickMode mode=JoystickMode.DYNAMIC)
    {
		joystickMode = mode;

        string prefabName = mode==JoystickMode.STATIC?"ETCJoystickStatic(Clone)":"ETCJoystick(Clone)";
        GameObject obj = Utility.FindGameObject(ClientSystemManager.instance.MiddleLayer, prefabName, false);
        if (obj != null)
        {
            Logger.LogProcessFormat("[InputManager] ReCreateJoyStick");
            GameObject.Destroy(obj);
        }

		string path = mode==JoystickMode.STATIC?"UIFlatten/Prefabs/ETCInput/ETCJoystickStatic":"UIFlatten/Prefabs/ETCInput/ETCJoystick";

		mObJoyStick = AssetLoader.instance.LoadResAsGameObject(path);


        if (mObJoyStick == null)
        {
            Logger.LogError("[InputManager] Can not find [UIFlatten/Prefabs/ETCInput/ETCJoystick] Prefabs,Please Check!");
            return;
        }

        //mObJoyStick.name = "ETCJoystick";
        Utility.AttachTo(mObJoyStick, GameClient.ClientSystemManager.instance.MiddleLayer);
        mObJoyStick.transform.SetAsLastSibling();
        mJoystick = mObJoyStick.GetComponent<ETCJoystick>();
        mJoystick.CustomActive(true);
        //add joystick callback
        if (mJoystick == null)
        {
            Logger.LogError("[InputManager] ETCJoystick Not Find,Please Check [UIFlatten/Prefabs/ETCInput/ETCJoystick] Prefabs!");
        }
        else
        {
            var joystickScale = BattleInputSetting.GetFloat(BattleInputSetting.JoystickScale, 1f);
            mJoystick.transform.localScale = Vector3.one * joystickScale;

            var offsetX = BattleInputSetting.GetFloat(BattleInputSetting.JoystickOffsetX, 0);
            var offsetY = BattleInputSetting.GetFloat(BattleInputSetting.JoystickOffsetY, 0);
            var joystickOffset = new Vector3(offsetX, offsetY, 0);

            var pos = mJoystick.transform.position;
            mJoystick.transform.position = pos + joystickOffset;

            mJoystick.onMove.AddListener(_onJoystickMove);
            mJoystick.onTouchUp.AddListener(_onJoystickMoveEnd);

			mJoystick.onTouchStart.AddListener(()=>{
				CheckDoublePress();
			});

            var com = mObJoyStick.GetComponent<ComScaleScripts>();
            if (com != null)
            {
                com.SetBaseScale(joystickScale);
            }
        }

    }

	void CheckDoublePress()
	{
		if (pressOne)
		{
			if (doubleCheckAcc <= Global.Settings.doublePressCheckDuration)
			{
				_onDoublePress();
				StartCheckDoublePress();
			}
		}
		else {
			StartCheckDoublePress();
		}
	}

	void StartCheckDoublePress()
	{
		doubleCheckAcc = 0;
		pressOne = true;
		//doublePress = false;
		//Logger.LogErrorFormat("StartCheckDoublePress doublePress = false;");
	}

	void EndCheckDoublePress()
	{
		doubleCheckAcc = int.MaxValue;
		pressOne = false;
		doublePress = false;
		//Logger.LogErrorFormat("EndCheckDoublePress doublePress = false;");
	}

	void UpdateCheckDoublePress(int delta)
	{
		if (!pressOne)
			return;

		doubleCheckAcc += delta;
		if (doubleCheckAcc > Global.Settings.doublePressCheckDuration)
		{
			if (!doublePress)
				EndCheckDoublePress();
		}
	}

	void _onDoublePress()
	{
		//Logger.LogErrorFormat("double press");

		doublePress = true;
	}

    private void _unloadJoystickUI()
    {
        if (mObJoyStick != null)
        {
            GameObject.Destroy(mObJoyStick);
            mObJoyStick = null;
        }

        if (null != joystick)
        {
            joystick.onMove.RemoveListener(_onJoystickMove);
            joystick.onTouchUp.RemoveListener(_onJoystickMoveEnd);

            mJoystick = null;
        }
    }

    #endregion
    private IBeEventHandle mPassDoorHandle = null;        //监听过门  

    #region Button Init

    private Dictionary<int, ETCButton> mDictSkillID = new Dictionary<int, ETCButton>();

	public static void _setButtonImageByPath(ETCButton button, string path, float scale=1.0f)
	{
		var pic = AssetLoader.instance.LoadRes(path, typeof(Sprite), false);
		if (pic != null)
		{
            // button.SetFgImage((Sprite)pic.obj);
            button.SetFgImage(path, false);
            button.SetFgImageScale(scale);
		}
			
	}

    public void  SetEnable(bool enable)
    {
        isLock = !enable;

        if (IsJoyStickLoaded)
        {
            mJoystick.activated = enable;
        }

        if (IsSkillButtonLoaded)
        {
            var iter = mButtonMap.GetEnumerator();
            while (iter.MoveNext())
            {
                ETCButton button = iter.Current.Value;
                if (null != button)
                {
                    button.activated = enable;
                    button.SetDark(!enable);
                }
            }
        }
    }

    private GameObject _addOneSliderButtonUI()
    {
        var sliderButtonUnit = AssetLoader.instance.LoadRes("UIFlatten/Prefabs/ETCInput/ETCButtonSliderUnit").obj as GameObject;
        Utility.AttachTo(sliderButtonUnit, mObETCButtons);
        return sliderButtonUnit;
    }
    #endregion

    public void LoadSkillButton(BeActor actor)
    {
        if (actor == null)
        {
            Logger.LogError("[InputManager] LoadSkillButton Must have a Actor!");
            return;
        }
        _setActorControl(actor);
        _LoadSkillButton(actor.professionID,null);
        mPassDoorHandle = actor.RegisterEventNew(BeEventType.onPassedDoor, _onPassedDoor);
    }

    public void LoadInputSettingBattleProgram()
    {
        if (mJoystick == null)
        {
            Logger.LogError("[InputManager] ETCJoystick Not Find!");
            return;
        }
        if (mObETCButtons == null)
        {
            Logger.LogErrorFormat("没有加载到技能按钮界面");
            return;
        }
        
        InputSettingBattleItemList data = new InputSettingBattleItemList();
        var alpha = 1.0f;
        var canvasGroup = mJoystick.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            alpha = canvasGroup.alpha;
        }
        
        data.mJoystick.SetData(
            mJoystick.transform.localPosition,
            mJoystick.transform.localScale,
            alpha
            );

        alpha = 1.0f;
        canvasGroup = mObETCButtons.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            alpha = canvasGroup.alpha;
        }
        
        data.mETCButtons.SetData(
            mObETCButtons.transform.localPosition,
            mObETCButtons.transform.localScale,
            alpha
        );

        foreach (var button in ButtonSlotMap.Values)
        {
            var position = button.transform.localPosition;
            var localScale = button.transform.localScale;
            alpha = 1.0f;
            canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                alpha = canvasGroup.alpha;
            }
            var item1 = new InputSettingItem(position,localScale,alpha);
            data.ETCButtonlistAdd(item1);
        }
        InputSettingBattleManager.instance.InitOriginData(data);
        
        var currInputSettingBattleProgram = InputSettingBattleManager.instance.GetCurrInputSettingBattleProgram();
        if (currInputSettingBattleProgram != null)
        {
            SetInputSettingData(mJoystick.transform, currInputSettingBattleProgram.mJoystick);
            
            mJoystick.SetInitPosition(mJoystick.rectTransform()
                    .anchoredPosition);
            mJoystick.ChangeMaxOffset(
                    (int) (currInputSettingBattleProgram.mJoystick.position.x -
                           InputSettingBattleManager.instance.GetInputSettingBattleItemProgramOrigin().mJoystick.position.x),
                    (int) (currInputSettingBattleProgram.mJoystick.position.y -
                           InputSettingBattleManager.instance.GetInputSettingBattleItemProgramOrigin().mJoystick.position.y));
            var com = mJoystick.GetComponent<ComScaleScripts>();
            if (com != null)
            {
                com.SetBaseScale(currInputSettingBattleProgram.mJoystick.scale.x);
            }

            for (int i = 0; i < currInputSettingBattleProgram.mETCButtonlist.Count; i++)
            {
                if (i < mObETCButtonRoot.transform.childCount)
                {
                    var transform = mObETCButtonRoot.transform.GetChild(i);
                    SetInputSettingData(transform, currInputSettingBattleProgram.mETCButtonlist[i]);
                }
            }
            
            SetInputSettingData(mObETCButtons.transform, currInputSettingBattleProgram.mETCButtons);
        }
        ResetETCEffectTrans();
    }
    
    public void SetInputSettingData(Transform obj, InputSettingItem item)
    {
        obj.localPosition = item.position;
        obj.localScale = item.scale;
        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = obj.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = item.alpha;
    }
    
    private void _onPassedDoor(BeEvent.BeEventParam args)
    {
        return;
        var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        if (null != actor && null != actor.playerActor)
        {
            PassDoorCommand cmd = new PassDoorCommand
            {
                seat = actor.playerInfo.seat
            };
            FrameSync.instance.FireFrameCommand(cmd,true);
        }
       
    }

    public ETCButton GetETCButton(int skillid)
    {
        if (mDictSkillID.ContainsKey(skillid))
        {
            return mDictSkillID[skillid];
        }

        return null;
    }

    public ETCButton GetSpecialETCButton(SpecialSkillID sp)
    {
        int slot = 0;
        switch (sp)
        {
            case SpecialSkillID.JUMP:
                slot = 2;
                break;
            case SpecialSkillID.JUMP_BACK:
                slot = 3;
                break;
            case SpecialSkillID.NORMAL_ATTACK:
                slot = 1;
                break;
        }

        if (mButtonMap.ContainsKey(slot))
            return mButtonMap[slot];
        else
            return null;
    }

	public void LoadJoystick(JoystickMode mode=JoystickMode.DYNAMIC)
    {
        _loadJoystickUI(mode);
    }


    public void SetVisible(bool flag)
    {
        if (mObJoyStick != null)
            mObJoyStick.SetActive(flag);
        if (mObETCButtons != null)
            mObETCButtons.SetActive(flag);
    
    }

    public void SetVisible(bool stickShow,bool buttonShow)
    {
        if (mObJoyStick != null)
            mObJoyStick.SetActive(stickShow);
        if (mObETCButtons != null)
            mObETCButtons.SetActive(buttonShow);
    
    }

    #endregion

    #region JoyStick MoveCallback -- for Town

    private UnityEngine.Events.UnityAction<Vector2> mMoveCallback;
    public void SetJoyStickMoveCallback(UnityEngine.Events.UnityAction<Vector2> callback)
    {
        mUseKeyCallBack = 1;
        mMoveCallback += callback;
    }

    public void ReleaseJoyStickMoveCallback(UnityEngine.Events.UnityAction<Vector2> callback)
    {
        mJoyX = 0;
        mJoyY = 0;
        mUseKeyCallBack = 0;
        mMoveCallback -= callback;
    }

    public Action JoysticMoveCallBack = null;

    private UnityEngine.Events.UnityAction mMoveEndCallback;
    public void SetJoyStickMoveEndCallback(UnityEngine.Events.UnityAction callback)
    {
        mMoveEndCallback += callback;
    }

    public void ReleaseJoyStickMoveEndCallback(UnityEngine.Events.UnityAction callback)
    {
        mMoveEndCallback -= callback;
    }

    #endregion

    #region JoyStick MoveCallback -- for InputManager

    private void _onJoystickMoveImp(Vector2 move, bool force=false)
    {
        mJoyX = move.x;
        mJoyY = move.y;

        if (mMoveCallback != null)
        {
            mMoveCallback(move);
        }
        if (JoysticMoveCallBack != null)
        {
            JoysticMoveCallBack();
        }

       

        if (!force)
            _updateJoystick(0);
    }

    private void _onJoystickMove(Vector2 move)
    {
        /*
       if (isLock)
            return;
            */
        ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;


        _onJoystickMoveImp(move, systemTown != null);

    }
  
    private void _onJoystickMoveEnd()
    {
        /*
        if (isLock)
            return;
            */
        mJoyX = 0;
        mJoyY = 0;

        if (mMoveEndCallback != null)
        {
            mMoveEndCallback();
        }
    }

    #endregion

    #region Unload
    void _clearJoystickState()
    {
        mJoyX = 0;
        mJoyY = 0;
        mUseKeyCallBack = 0;

        mMoveCallback = null;
        mMoveEndCallback = null;
    }

    private void _clearCmd()
    {
        GameClient.StopFrameCommand cmd = new GameClient.StopFrameCommand();
        FrameSync.instance.FireFrameCommand(cmd);
        FrameSync.instance.bInMoveMode = false;

        FrameSync.instance.ResetMove();
    }


    public void Unload()
    {
        _clearJoystickState();
        _clearSlot();
        _setActorControl(null);
        _clearCmd();
        _UnLoadSkillButtonUI();
        //_unloadDirectionJoystick();
        _unloadJoystickUI();
        _ClearSkillJoystick();
        if (mPassDoorHandle != null)
        {
            mPassDoorHandle.Remove();
            mPassDoorHandle = null;
        }
    }
    #endregion

    #region Actor

    private BeActor mControllActor = null;
    private bool mIsLock = false;
    private IBeEventHandle mOnStateChangeHandler = null;

    //用于Loading，过门等锁住
    static private bool mIsForceLock = false;

    static public bool isForceLock
    {
        set
        {
            mIsForceLock = value;
        }
        get
        {
            return mIsForceLock;
        }
    }

    public bool isLock
    {
        set
        {
            mIsLock = value;
        }
        get
        {
            return mIsLock || /*(controllActor == null) || */ (IsInit == false) || isForceLock;
        }
    }

    public bool IsJoyStickLoaded
    {
        get
        {
            return (mObJoyStick != null);
        }
    }

    public bool IsSkillButtonLoaded
    {
        get
        {
            return (mObETCButtons != null);
        }
    }

    public bool IsInit
    {
        get
        {
            return IsJoyStickLoaded || IsSkillButtonLoaded;
        }
    }


    public BeActor controllActor
    {
        get
        {
            return mControllActor;
        }

        private set
        {
            _setActorControl(value);
        }
    }

    public void UnloadActorConfig()
    {
        if (controllActor != null)
        {
            //controllActor.attackButtonState = ButtonState.RELEASE;
            controllActor.SetAttackButtonState(ButtonState.RELEASE);
            controllActor = null;
            if (mOnStateChangeHandler != null)
                mOnStateChangeHandler.Remove();
        }
    }

    public void LoadActorConfig(BeActor actor)
    {
        controllActor = actor;

        mOnStateChangeHandler = controllActor.RegisterEventNew(BeEventType.onStateChange, (param) =>
        {
            for(int i= 0; i < _skillSlotList.Count; i++)
            {
                var data = _skillSlotList[i];
                int skillId = data.skillId;
                int slot = data.slot;
                if (skillId > 0 && mButtonMap.ContainsKey(slot))
                {
                    var skill = controllActor.GetSkill(skillId);
                    if (skill != null && !skill.isCooldown)
                    {
                        var bCanUse = controllActor.CanUseSkill(skillId);

                        mButtonMap[slot].SetDark(!bCanUse, 0.5f);
                    }
                }
            }
        });
    }

    private void OnActorStateChange(GameClient.BeEvent.BeEventParam param)
    {
        if (isLock)
            return;

        for(int i = 0; i < _skillSlotList.Count; i++)
        {
            var data = _skillSlotList[i];
            int skillId = data.skillId;
            int slot = data.slot;
            
            if (skillId > 0 && mButtonMap.ContainsKey(slot))
            {
                var skill = controllActor.GetSkill(skillId);
                if (skill != null && !skill.isCooldown)
                {
                    var bCanUse = controllActor.CanUseSkill(skillId);

                    mButtonMap[slot].SetDark(!bCanUse, 0.5f);
                }
            }
        }
    }

    private void _setActorControl(BeActor actor)
    {
        if (mControllActor == actor)
        {
            return;
        }

        if (mControllActor != null)
        {
            //mControllActor.RemoveEvent(BeEventType.onStateChange, OnActorStateChange);
            mControllActor.SetAttackButtonState(ButtonState.RELEASE);
            mOnStateChangeHandler.Remove();
            mOnStateChangeHandler = null;
        }

        mControllActor = actor;

        if (mControllActor != null)
        {
            mOnStateChangeHandler = mControllActor.RegisterEventNew(BeEventType.onStateChange, OnActorStateChange);
        }

    }

    #endregion

    #region Update

    private void _updateCD()
    {
        if(controllActor == null || IsSkillButtonLoaded == false || isAttackButtonOnly || _skillSlotList == null)
        {
            return;
        }

        for(int i=0;i< _skillSlotList.Count; i++)
        {
            var data = _skillSlotList[i];

            int skillSlot = data.slot;
            int skillId = data.skillId;
            if (mButtonMap.TryGetValue(skillSlot, out var button))
            {
                var skill = controllActor.GetSkill(skillId);
                if (skill == null)
                {
                    button.StopFakeCoolDown();
                    //Logger.LogErrorFormat("on cast skill error : {0}", item.Value);
                    continue;
                }

                if (skill.IsDisplayCDing)
                {
                    button.UpdateFakeCoolDown(skill.DisplayCD, (int)skill.DisplayFullCD, skill.isBuffSkill, controllActor);
                    button.SetDark(true, 0.5f);
                }
                else if (skill.ForceShowButtonImage)
                {
                    button.StopFakeCoolDown(skill.isBuffSkill);
                    button.SetDark(false);
                }
                else
                {
                    button.StopFakeCoolDown(skill.isBuffSkill);
                    if (!controllActor.CanUseSkill(skillId))
                    {
                        button.SetDark(true, 0.5f);
                    }
                    else
                    {
                        button.SetDark(false);
                    }
                }
            }
            else
            {
                //Logger.LogFormat("skill not in buttonmap with slot id : {0}", item.Key);
            }
        }

    }

    public void InitState()
    {
        _updateCD();
    }

    public void Update(int deltaTime)
    {
        _updateCD();
        if (isLock)
            return;
        _updateJoystick(deltaTime);

    }

    public void SingleUpdate(int deltaTime)
    {
        if (isLock)
            return;

        _updateJoystick(deltaTime);
    }

    public const short DegreeDiv = 15;

    private Vector2 m_MoveVector = Vector2.zero;
    void _updateJoystick(int deltaTime)
    {
       if (isLock)
            return;

		UpdateCheckDoublePress(deltaTime);

        float mx = Input.GetAxisRaw("Horizontal");
        float my = Input.GetAxisRaw("Vertical");

		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
		{
			CheckDoublePress();
		}

        bool bInMoving = false;
        float numx = mJoyX;
        float numy = mJoyY;
        short nDegree = 0;

        if (Mathf.Abs(mx) > 0.00001f || Mathf.Abs(my) > 0.00001f)
        {
            numx = mx;
            numy = my;
            bInMoving = true;
        }

        if (Mathf.Abs(numx) > 0.00001f || Mathf.Abs(numy) > 0.00001f)
        {
            float l = Mathf.Sqrt(numx * numx + numy * numy);
            float redians = Mathf.Acos(numx / l);
            nDegree = (short)(Mathf.Rad2Deg * redians);
            if (numy < -0f)
            {
                nDegree = (short)(360 - nDegree);
            }
            nDegree /= DegreeDiv;
            bInMoving = true;
        }

        bool runmode = false;

        if (bInMoving)
        {
			if (doublePress)
			{
				if (controllActor != null && controllActor.hasDoublePress)
					runmode = true;
				doublePress = false;
			}
			if (controllActor != null && !controllActor.hasDoublePress)
				runmode = true;

            if (SettingManager.GetInstance().GetJoystickDir() == JoystickDir.EIGHT_DIR&&SwitchFunctionUtility.IsOpen(29))
            {
                MoveIn8Dir(ref nDegree);
            }

            GameClient.MoveFrameCommand cmd = new GameClient.MoveFrameCommand
            {
                degree = nDegree,
                run = runmode
            };

            if (runmode != FrameSync.instance.bInRunMode
                || nDegree != FrameSync.instance.nDegree
                || false == FrameSync.instance.bInMoveMode)
            {
                FrameSync.instance.FireFrameCommand(cmd);

                FrameSync.instance.bInRunMode = runmode;
                FrameSync.instance.nDegree = nDegree;
            }

            FrameSync.instance.bInMoveMode = true;

            if (mUseKeyCallBack == 1)
            {
                m_MoveVector.x = mx;
                m_MoveVector.y = my;
                _onJoystickMoveImp(m_MoveVector, true);
            }
        }
        else
        {
            if (FrameSync.instance.bInMoveMode == true)
            {
                if (mUseKeyCallBack == 1)
                    _onJoystickMoveEnd();
                FrameSync.instance.bInMoveMode = false;
            }

			FireStopCommand();
            
        }
    }

    private void MoveIn8Dir(ref short degree)
    {
        BeAIManager.MoveDir2 dir = GetDir8(degree * DegreeDiv);
        degree = (short)((int)dir * 45 / DegreeDiv);
    }

    public bool isAttackButtonOnly = false;

    /// <summary>
    /// 除了这个按钮其他都隐藏 函数内部实现不是太好 暂时不改
    /// </summary>
    /// <param name="index">隐藏的按钮序号</param>
    public void SetButtonStateActive(int index)
    {
        int slot = index + 1;

        isAttackButtonOnly = true;

        var enumerator = mButtonMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            stateDic[current.Key] = current.Value.gameObject.activeSelf;

            current.Value.SetSkillBtnVisible(current.Key == slot);
            current.Value.SetDark(current.Key != slot);
        }
    }

    Dictionary<int, bool> stateDic = new Dictionary<int, bool>();

    /// <summary>
    /// 显示全部按钮1
    /// </summary>
    public void ResetButtonState()
    {
        EtcButtonStopCoolDown();
        isAttackButtonOnly = false;

        var enumerator = mButtonMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            if (!stateDic.ContainsKey(current.Key)) continue;
            current.Value.SetSkillBtnVisible(stateDic[current.Key]);
        }
    }

    //如果按钮在所属技能不在CD状态 则按钮停止CD(因为按钮的CD只是表现)
    protected void EtcButtonStopCoolDown()
    {
#if !LOGIC_SERVER
        for(int i = 0; i < _skillSlotList.Count; i++)
        {
            var data = _skillSlotList[i];
            int skillId = data.skillId;
            int slot = data.slot;
            
            if (mButtonMap.ContainsKey(slot))
            {
                var skill = controllActor.GetSkill(skillId);
                if (skill != null && !skill.isCooldown && null != skill.button)
                {
                    skill.button.StopCoolDown2();
                    mButtonMap[slot].RemoveEffect(ETCButton.eEffectType.onCDFinish);
                    mButtonMap[slot].RemoveEffect(ETCButton.eEffectType.onCDFinishBuff);
                }
            }
        }
#endif
    }

    public void FireStopCommand()
	{
		if (mControllActor != null && mControllActor.IsInMoveDirection())
		{
            IFrameCommand cmd = FrameCommandFactory.CreateCommand((uint)Protocol.FrameCommandID.Stop);
            FrameSync.instance.FireFrameCommand(cmd);
			FrameSync.instance.bInMoveMode = false;
		}
	}

    /// <summary>
    /// 记录技能按钮按下与抬起信息
    /// </summary>
    private void SaveETCBtnClick(int skillId,int up)
    {
#if MG_TEST_EXTENT && !LOGIC_SERVER
        uint curFrame = 0;
        if (FrameSync.instance != null)
        {
            curFrame = FrameSync.instance.curFrame;
        }

        string str = string.Format("curFrame:{0} skillId:{1} Up:{2} curTime:{3}", curFrame, skillId, up, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"));
        if (skillOnClickQue.Count >= 6)
        {
            skillOnClickQue.Dequeue();
            skillOnClickQue.Enqueue(str);
        }
        else
        {
            skillOnClickQue.Enqueue(str);
        }
#endif
    }

    public delegate void OnSkillCommand(int skillid,int value);
    public static  OnSkillCommand onSkillCommandCallBack = null;
    public static bool needJoystickOnTouch = false;
    public static bool isProperDir = false;
    public void  CreateSkillFrameCommand(int skillID, SkillFrameCommand.SkillFrameData data = default)
    {
#if !UNITY_EDITOR
        if (needJoystickOnTouch)
        {
            if (!joystick.isOnDrag)
            {
                SystemNotifyManager.SysDungeonSkillTip("要先按指向拖住方向键，再释放技能", 100);
                return;
            }
            if (!isProperDir)
            {
                SystemNotifyManager.SysDungeonSkillTip("方向不对", 100);
                return;
            }
        }
#endif
        var value = SkillFrameCommand.Assemble(data);
        SkillFrameCommand cmd = new SkillFrameCommand
        {
            frame = 0,
            skillSolt = (uint)skillID,

            skillSlotUp = value
        };
        FrameSync.instance.FireFrameCommand(cmd);

            if (onSkillCommandCallBack != null)
        {
            onSkillCommandCallBack(skillID, (int) value);
        }
    }

    public static void CreateStopSkillFrameCommand(int skillID)
    {
        StopSkillCommand cmd = new StopSkillCommand
        {
            frame = 0,
            skillID = skillID
        };

        FrameSync.instance.FireFrameCommand(cmd);
    }

    public static void CreateSkillDoattackFrameCommand(int skillID, int bulletNum, int pid)
    {
        DoAttackCommand cmd = new DoAttackCommand
        {
            skillID = skillID,
            bulletCount = bulletNum,
            pid = pid
        };

        FrameSync.instance.FireFrameCommand(cmd);
    }

    public static void CreateSkillSynSightFrameCommand(int skillID, int x,int z)
    {
        DoSyncSightCommand cmd = new DoSyncSightCommand
        {
            skillID = skillID,
            x = x,
            z = z,
        };

        FrameSync.instance.FireFrameCommand(cmd);
    }

    public static void CreateBossPhaseChange(int phaseIndex)
    {
        BossPhaseChange cmd = new BossPhaseChange
        {
            phase = phaseIndex,
        };

        FrameSync.instance.FireFrameCommand(cmd, true);
        //Logger.LogError(string.Format("BOSS阶段上报 阶段:{0}", phaseIndex));
    }

#endregion

    public enum SkillBtnState
    {
        NORMAL,
        PRESS,
        UP,
    }

    public void EnableSkillButton(bool flag)
    {
        for(int i = 0; i < _skillSlotList.Count; i++)
        {
            var data = _skillSlotList[i];
            int slot = data.slot;
            int skillId = data.skillId;
            
            if (mButtonMap.ContainsKey(slot))
            {
                if (slot == 3 && !flag)                             //显示技能摇杆时 不屏蔽后跳技能
                    continue;
                var skill = controllActor.GetSkill(skillId);
                if (skill != null)
                {
                    if (skill.skillState.IsRunning() && !flag)
                        continue;
                    mButtonMap[slot].activated = flag;
                }
            }
        }
    }

	public static InputManager.PressDir GetDir(int degree)
	{
		InputManager.PressDir dir = InputManager.PressDir.NONE;

		if (degree > 0)
		{
			int tmp = degree;
			if (tmp >= 45 && tmp < 135)
				dir = InputManager.PressDir.TOP;
			else if (tmp >= 135 && tmp < 225)
				dir = InputManager.PressDir.LEFT;
			else if (tmp >= 225 && tmp < 315)
				dir = InputManager.PressDir.DOWN;
			else
				dir = InputManager.PressDir.RIGHT;
		}

		return dir;
	}
    public static InputManager.PressDir GetForwardBack(int degree)
    {
        InputManager.PressDir dir = InputManager.PressDir.NONE;

        if (degree >= 0)
        {
            int tmp = degree;
            if (tmp >= 90 && tmp < 270)
                dir = InputManager.PressDir.RIGHT;
            else 
                dir = InputManager.PressDir.LEFT;
        }

        return dir;
    }
    
	public static BeAIManager.MoveDir2 GetDir8(int degree)
	{
		if (degree > 337 || degree <= 22.5f)
			return BeAIManager.MoveDir2.RIGHT;
		else if (degree > 22.5f && degree <= 67.5f)
			return BeAIManager.MoveDir2.RIGHT_TOP;
		else if (degree > 67.5f && degree <= 112.5f)
			return BeAIManager.MoveDir2.TOP;
		else if (degree > 112.5f && degree <= 157.5f)
			return BeAIManager.MoveDir2.LEFT_TOP;
		else if (degree > 157.5f && degree <= 202.5f)
			return BeAIManager.MoveDir2.LEFT;
		else if (degree > 202.5f && degree <= 247.5f)
			return BeAIManager.MoveDir2.LEFT_DOWN;
		else if (degree > 247.5f && degree <= 292.5f)
			return BeAIManager.MoveDir2.DOWN;
		else if (degree > 292.5f && degree <= 337.5f)
			return BeAIManager.MoveDir2.RIGHT_DOWN;

		return BeAIManager.MoveDir2.COUNT;
	}

    public static BeAIManager.MoveDir MoveDir2ToMoveDir(BeAIManager.MoveDir2 dir)
    {
        BeAIManager.MoveDir ret = BeAIManager.MoveDir.TOP;
        if (dir == BeAIManager.MoveDir2.TOP)
            ret = BeAIManager.MoveDir.TOP;
        if (dir == BeAIManager.MoveDir2.DOWN)
            ret = BeAIManager.MoveDir.DOWN;
        if (dir == BeAIManager.MoveDir2.LEFT)
            ret = BeAIManager.MoveDir.LEFT;
        if (dir == BeAIManager.MoveDir2.RIGHT)
            ret = BeAIManager.MoveDir.RIGHT;
        if (dir == BeAIManager.MoveDir2.RIGHT_TOP)
            ret = BeAIManager.MoveDir.RIGHT_TOP;
        if (dir == BeAIManager.MoveDir2.LEFT_TOP)
            ret = BeAIManager.MoveDir.LEFT_TOP;
        if (dir == BeAIManager.MoveDir2.LEFT_DOWN)
            ret = BeAIManager.MoveDir.LEFT_DOWN;
        if (dir == BeAIManager.MoveDir2.RIGHT_DOWN)
            ret = BeAIManager.MoveDir.RIGHT_DOWN;
        return ret;
    }
}
