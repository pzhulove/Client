using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class InputSettingBattleFrame : ClientFrame
    {
        private GameObject mObjSettingMode;
        private Button mButtonExit;
        private Button mButtonSetting;
        private Button mButtonSave;
        private Button mButtonSave2;
        private Button mButtonReset;
        private ComDragPad mDragPadJoystick;
        private ComDragPad mDragBattleUIDrug;
        private ComDragPad mDragBattleUISwitchWeaAndEquip;
        private GameObject mDragPadButtons;
        private List<ComDragPad> mListDragPadButton = new List<ComDragPad>();
        private ComDragPad mDragPadAllScaleMode1;
        private ComDragPad mDragPadAllScaleMode2;
        private ImageEx mImageControl;
        private GameObject mEditPanel = null;
        private Slider mButtonSizeSlider = null;
        private Slider mButtonTransparentSlider = null;
        private Toggle mButtonAllChange = null;
        private GameObject mSettingAllScale = null;
        private Toggle mAllScaleMode1 = null;
        private Toggle mAllScaleMode2 = null;
        private Dropdown mDropdown = null;
        private ComButtonEx mChangeName = null;

        private float MinButtonSize = 0.5f;
        private float MaxButtonSize = 1.5f;
        private float MinButtonTransparent = 0.3f;
        private float MaxButtonTransparent = 1.0f;
        
        /*private Vector3 mJoystickOriginPos = Vector3.zero;
        private Vector3 mButtonsOriginPos = Vector3.zero;
        private List<Vector3> mListButtonOriginPos = new List<Vector3>();
        
        private float mJoystickOriginTransparent = 1.0f;
        private float mButtonsOriginTransparent = 1.0f;
        private List<float> mListButtonOriginTransparent = new List<float>();
        
        private Vector3 mJoystickCurrPos = Vector3.zero;
        private Vector3 mButtonsCurrPos = Vector3.zero;
        private List<Vector3> mListButtonCurrPos = new List<Vector3>();
        
        private float mJoystickCurrTransparent = 1.0f;
        private float mButtonsCurrTransparent = 1.0f;
        private List<float> mListButtonCurrTransparent = new List<float>();*/

        private InputSettingBattleItemList mInputSettingBattleItemListOrigin = new InputSettingBattleItemList();
        private InputSettingBattleItemList mInputSettingBattleItemListCurr = new InputSettingBattleItemList();
        
        private List<bool> mListButtonVisible = new List<bool>();
        
        private ComDragPad mCurPad;
        //private ComScaleScripts mScaleScripts;

        InputSettingBattle battle;

        private bool isInitDragPad = false;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/InputSettingMain";
        }

        protected override void _bindExUI()
        {
            mObjSettingMode = mBind.GetGameObject("SettingMode");

            mButtonExit = mBind.GetCom<Button>("ButtonExit");
            if (null != mButtonExit)
            {
                mButtonExit.onClick.AddListener(_onButtonExitClick);
            }

            mButtonSetting = mBind.GetCom<Button>("ButtonSetting");
            if (null != mButtonSetting)
            {
                mButtonSetting.onClick.AddListener(_onButtonSettingClick);
            }

            mButtonSave = mBind.GetCom<Button>("ButtonSave");
            if (null != mButtonSave)
            {
                mButtonSave.onClick.AddListener(_onButtonSaveClick);
            }
            mButtonSave2 = mBind.GetCom<Button>("ButtonSave2");
            if (null != mButtonSave2)
            {
                mButtonSave2.onClick.AddListener(_onButtonSaveClick);
            }

            mButtonReset = mBind.GetCom<Button>("ButtonReset");
            if (null != mButtonReset)
            {
                mButtonReset.onClick.AddListener(_onButtonResetClick);
            }

            mDragPadJoystick = mBind.GetCom<ComDragPad>("DragPadJoystick");
            mDragBattleUIDrug = mBind.GetCom<ComDragPad>("DragBattleUIDrug"); 
            mDragBattleUISwitchWeaAndEquip = mBind.GetCom<ComDragPad>("DragBattleUISwitchWeaAndEquip"); 
            
            mListDragPadButton.Clear();
            mDragPadButtons = mBind.GetGameObject("DragPadButtons");
            if (null != mListDragPadButton)
            {
                for (int i = 0; i < mDragPadButtons.transform.childCount; i++)
                {
                    var com = mDragPadButtons.transform.GetChild(i).gameObject.GetComponent<ComDragPad>();
                    if (null != com)
                    {
                        mListDragPadButton.Add(com);
                    }
                }
            }

            mImageControl = mBind.GetCom<ImageEx>("SettingScale");

            mEditPanel = mBind.GetGameObject("EditPanel");

            mButtonSizeSlider = mBind.GetCom<Slider>("ButtonSizeSlider");
            if (mButtonSizeSlider != null)
            {
                mButtonSizeSlider.onValueChanged.AddListener(_onChangeButtonSize);
            }
            
            mButtonTransparentSlider = mBind.GetCom<Slider>("ButtonTransparentSlider");
            if (mButtonTransparentSlider != null)
            {
                mButtonTransparentSlider.onValueChanged.AddListener(_onChangeTransparentSize);
            }
            
            mButtonAllChange = mBind.GetCom<Toggle>("ButtonAllChange");
            if (mButtonAllChange != null)
            {
                mButtonAllChange.onValueChanged.AddListener(_onButtonAllChangeToggleValueChange);
            }
            
            mSettingAllScale = mBind.GetGameObject("SettingAllScale");
            mAllScaleMode1 = mBind.GetCom<Toggle>("AllScaleMode1");
            if (mAllScaleMode1 != null)
            {
                mAllScaleMode1.onValueChanged.AddListener(_onAllScaleMode1ToggleValueChange);
                mDragPadAllScaleMode1 = mAllScaleMode1.GetComponent<ComDragPad>();
            }

            mAllScaleMode2 = mBind.GetCom<Toggle>("AllScaleMode2");
            if (mAllScaleMode2 != null)
            {
                mAllScaleMode2.onValueChanged.AddListener(_onAllScaleMode2ToggleValueChange);
                mDragPadAllScaleMode2 = mAllScaleMode2.GetComponent<ComDragPad>();
            }
            
            mDropdown = mBind.GetCom<Dropdown>("Dropdown");
            if (mDropdown != null)
            {
                mDropdown.onValueChanged.AddListener(_onDropdownDropdownValueChange);
                SetDropDownData();
            }

            mChangeName = mBind.GetCom<ComButtonEx>("ChangeName");
            if (mChangeName != null)
            {
                mChangeName.onClick.AddListener(_onChangeNameButtonClick);
            }
            GameFrameWork.instance.StartCoroutine(Init());
            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.InputSettingBattleProNameChange, _OnInputSettingBattleProNameChange);
        }
        
        IEnumerator Init()
        {
            while(InputManager.instance == null || InputManager.instance.joystick == null || !InputSettingBattleManager.instance.IsInitSuccess())
                yield return null;
            if (!isInitDragPad)
            {
                SetEditMode(false);
                initComDragPad();
                
                if (InputSettingBattleManager.instance.GetInputSettingBattleProgramType() !=
                    InputSettingBattleProgramType.none)
                    mDropdown.value = (int) InputSettingBattleManager.instance.GetInputSettingBattleProgramType();
                else
                {
                    mDropdown.value = 0;
                }

                _onDropdownDropdownValueChange(mDropdown.value);
                isInitDragPad = true;
            }
        }

        protected override void _unbindExUI()
        {
            if (null != mButtonExit)
            {
                mButtonExit.onClick.RemoveAllListeners();
            }
            if (null != mButtonSetting)
            {
                mButtonSetting.onClick.RemoveAllListeners();
            }
            if (null != mButtonSave)
            {
                mButtonSave.onClick.RemoveAllListeners();
            }
            if (null != mButtonSave2)
            {
                mButtonSave2.onClick.RemoveAllListeners();
            }
            if (null != mButtonReset)
            {
                mButtonReset.onClick.RemoveAllListeners();
            }
            if (mButtonSizeSlider != null)
            {
                mButtonSizeSlider.onValueChanged.RemoveAllListeners();
            }
            if (mButtonTransparentSlider != null)
            {
                mButtonTransparentSlider.onValueChanged.RemoveAllListeners();
            }
            if (mButtonAllChange != null)
            {
                mButtonAllChange.onValueChanged.RemoveAllListeners();
            }
            if (mDropdown != null)
            {
                mDropdown.onValueChanged.RemoveAllListeners();
            }
            if (mChangeName != null)
            {
                mChangeName.onClick.RemoveAllListeners();
            }
            mObjSettingMode = null;
            mButtonExit = null;
            mButtonSetting = null;
            mButtonSave = null;
            mButtonSave2 = null;
            mButtonReset = null;
            mDragPadJoystick = null;
            mDragBattleUIDrug = null;
            mDragBattleUISwitchWeaAndEquip = null;
            mListDragPadButton.Clear();
            mDragPadButtons = null;
            mImageControl = null;
            mEditPanel = null;
            mButtonSizeSlider = null;
            mButtonTransparentSlider = null;
            mButtonAllChange = null;
            mDropdown = null;
            mChangeName = null;
            mInputSettingBattleItemListOrigin = null;
            mInputSettingBattleItemListCurr = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.InputSettingBattleProNameChange, _OnInputSettingBattleProNameChange);
        }

        void _OnInputSettingBattleProNameChange(UIEvent uiEvent)
        {
            SetDropDownData();
        }
        
        private void SetDropDownData()
        {
            mDropdown.options.Clear();
            for (int i = 0; i < (int)InputSettingBattleProgramType.Max; i++)
            {
                var temoData = new Dropdown.OptionData();
                var content = InputSettingBattleManager.instance.GetInputSettingBattleProgramName((InputSettingBattleProgramType)i);
                if(string.IsNullOrEmpty(content))
                    content = Utility.GetEnumDescription((InputSettingBattleProgramType)i);
                temoData.text = content;
                mDropdown.options.Add(temoData);
            }

            mDropdown.RefreshShownValue();
        }

        private void SetEditMode(bool flag)
        {
            mObjSettingMode.CustomActive(flag);
            mEditPanel.CustomActive(flag);
            mButtonReset.SafeSetGray(!flag);
            mButtonSave.CustomActive(flag);
            mButtonSave2.CustomActive(flag);
            mButtonSetting.CustomActive(!flag);
            mSettingAllScale.CustomActive(flag && mButtonAllChange.isOn);
            if (InputManager.instance != null)
            {
                InputManager.instance.ETCEffectRoot.CustomActive(!flag);
                InputManager.instance.ResetETCEffectTrans();
            }
        }

        private void _onDragCallBack(ComDragPad pad)
        {
            mCurPad = pad;
            mButtonSizeSlider.value = (mCurPad.dragTran.localScale.x - MinButtonSize) / (MaxButtonSize - MinButtonSize);
            var canvasGroup = mCurPad.dragTran.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = mCurPad.dragTran.gameObject.AddComponent<CanvasGroup>();
            }
            mButtonTransparentSlider.value = (canvasGroup.alpha - MinButtonTransparent) / (MaxButtonTransparent - MinButtonTransparent);
            
            ChangeInputSettingItemData(mCurPad,new EnumHelper<InputSettingItemChangeTypeFlag>(InputSettingItemChangeTypeFlag.position), mCurPad.dragTran.localPosition);
            
            if (null != mImageControl)
            {
                var comInputSelectImage = mImageControl.GetComponent<ComInputSelectImage>();
                if (comInputSelectImage != null)
                {
                    var sprite = comInputSelectImage.GetSprite(mCurPad.dragTran.name);
                    if (sprite != null)
                        mImageControl.sprite = sprite;
                }
                mImageControl.CustomActive(true);
                mImageControl.rectTransform.sizeDelta = pad.dragTran.localScale * pad.dragTran.sizeDelta;
                for (int i = 0; i < mListDragPadButton.Count; i++)
                {
                    if (i < mInputSettingBattleItemListCurr.mETCButtonlist.Count && mListDragPadButton[i] == pad)
                    {
                        mImageControl.rectTransform.sizeDelta = pad.dragTran.localScale * pad.dragTran.sizeDelta * mInputSettingBattleItemListCurr.mETCButtons.scale;
                        break;;
                    }
                }
                mImageControl.transform.position = pad.dragTran.position;
            }
        }
        
        private void _onDragCallBackAllChangeMode(ComDragPad pad)
        {
            mCurPad = pad;
            
            mButtonSizeSlider.value = (mCurPad.dragTran.localScale.x - MinButtonSize) / (MaxButtonSize - MinButtonSize);
            var canvasGroup = mCurPad.dragTran.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = mCurPad.dragTran.gameObject.AddComponent<CanvasGroup>();
            }
            mButtonTransparentSlider.value = (canvasGroup.alpha - MinButtonTransparent) / (MaxButtonTransparent - MinButtonTransparent);
            ChangeInputSettingItemData(mCurPad,new EnumHelper<InputSettingItemChangeTypeFlag>(InputSettingItemChangeTypeFlag.position), mCurPad.dragTran.localPosition);
            if (mCurPad == mDragPadAllScaleMode1)
            {
                mAllScaleMode1.isOn = true;
            }
            if (mCurPad == mDragPadAllScaleMode2)
            {
                mAllScaleMode2.isOn = true;
            }
        }

        private void _onButtonExitClick()
        {
            var data = InputSettingBattleManager.instance.GetInputSettingBattleProgram(
                (InputSettingBattleProgramType) mDropdown.value);
            if((data != null && !mInputSettingBattleItemListCurr.IsSameData(data)) || (data == null && !mInputSettingBattleItemListCurr.IsSameData(mInputSettingBattleItemListOrigin)))
                OpenSaveProgramSystemNotify();
            else
            {
                BeUtility.ResetCamera();
                ClientSystemManager.GetInstance().SwitchSystem<ClientSystemTown>();
            }
        }

        private void _onButtonSettingClick()
        {
            frame.transform.SetSiblingIndex(frame.transform.parent.childCount - 1);
            
            if (!isInitDragPad)
            {
                initComDragPad();
                isInitDragPad = true;
            }

            int index = 0;
            foreach (var button in InputManager.instance.ButtonSlotMap.Values)
            {
                mListButtonVisible[index] = button.gameObject.activeSelf;
                index++;
            }
            
            var battleUIDrug = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
            if (battleUIDrug != null && battleUIDrug.GetNeedShowObj() != null)
            {
                mListButtonVisible[index] = battleUIDrug.GetNeedShowObj().activeSelf;
                index++;
            }
            
            var battleUISwitchWeaAndEquip = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
            if (battleUISwitchWeaAndEquip != null && battleUISwitchWeaAndEquip.GetNeedShowObj() != null)
            {
                mListButtonVisible[index] = battleUISwitchWeaAndEquip.GetNeedShowObj().activeSelf;
                index++;
            }

            SetEditMode(true);
            SetInputEnable(false);
            SetCurrInputItemData();
            SetComDragPadSize();
            
            mImageControl.CustomActive(false);
            mButtonAllChange.isOn = false;
            for (int i = 0; i < mListDragPadButton.Count; i++)
            {
                if (mListDragPadButton[i].dragTran != null && mListDragPadButton[i].dragTran.name == "Btn_Attack")
                {
                    _onDragCallBack(mListDragPadButton[i]);
                    break;
                }
            }

        }

        private void _onButtonSaveClick()
        {
            OpenSaveProgramSystemNotify();
        }

        private void _onButtonResetClick()
        {
            /*mDragPadJoystick.SetPosition(mInputSettingBattleItemListOrigin.mJoystick.position);
            mDragPadJoystick.SetLocalScale(Vector3.one);
            //if (mScaleScripts != null)
            //{
            //    mScaleScripts.SetBaseScale(1);
            //}

            InputManager.instance.joystick.SetInitPosition(InputManager.instance.joystick.rectTransform().anchoredPosition);

            for (int i = 0; i < mListButtonOriginPos.Count; i++)
            {
                mListDragPadButton[i].SetPosition(mListButtonOriginPos[i]);
                mListDragPadButton[i].SetLocalScale(Vector3.one);
            }*/
            /*mInputSettingBattleItemListCurr.mJoystick.SetData(
                mInputSettingBattleItemListOrigin.mJoystick.position,
                mInputSettingBattleItemListOrigin.mJoystick.scale,
                mInputSettingBattleItemListOrigin.mJoystick.alpha
                );
            mInputSettingBattleItemListCurr.mETCButtons.SetData(
                mInputSettingBattleItemListOrigin.mETCButtons.position,
                mInputSettingBattleItemListOrigin.mETCButtons.scale,
                mInputSettingBattleItemListOrigin.mETCButtons.alpha
            );
            
            for (int i = 0; i < mInputSettingBattleItemListCurr.mETCButtonlist.Count; i++)
            {
                mInputSettingBattleItemListCurr.mETCButtonlist[i].SetData(
                    mInputSettingBattleItemListOrigin.mETCButtonlist[i].position,
                    mInputSettingBattleItemListOrigin.mETCButtonlist[i].scale,
                    mInputSettingBattleItemListOrigin.mETCButtonlist[i].alpha
                );
            }*/
            OpenResetProgramSystemNotify();
        }

        private void initComDragPad()
        {
            if (InputManager.instance != null)
            {
                mInputSettingBattleItemListOrigin =
                    InputSettingBattleManager.instance.GetInputSettingBattleItemProgramOrigin();
                mInputSettingBattleItemListCurr.SetData(mInputSettingBattleItemListOrigin);
                /*var alpha = 1.0f;
                var canvasGroup = InputManager.instance.joystick.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    alpha = canvasGroup.alpha;
                }
                
                mInputSettingBattleItemListOrigin.mJoystick.SetData(
                    InputManager.instance.joystick.transform.localPosition,
                    InputManager.instance.joystick.transform.localScale,
                    alpha
                    );
                mInputSettingBattleItemListCurr.mJoystick.SetData(
                    InputManager.instance.joystick.transform.localPosition,
                    InputManager.instance.joystick.transform.localScale,
                    alpha
                );
                
                alpha = 1.0f;
                canvasGroup = InputManager.instance.ETCButtons.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    alpha = canvasGroup.alpha;
                }
                
                mInputSettingBattleItemListOrigin.mETCButtons.SetData(
                    InputManager.instance.ETCButtons.transform.localPosition,
                    InputManager.instance.ETCButtons.transform.localScale,
                    alpha
                );
                mInputSettingBattleItemListCurr.mETCButtons.SetData(
                    InputManager.instance.ETCButtons.transform.localPosition,
                    InputManager.instance.ETCButtons.transform.localScale,
                    alpha
                );
                
                foreach (var button in InputManager.instance.ButtonSlotMap.Values)
                {
                    mListButtonVisible.Add(button.gameObject.activeSelf);
                    var position = button.transform.localPosition;
                    var localScale = button.transform.localScale;
                    alpha = 1.0f;
                    canvasGroup = button.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        alpha = canvasGroup.alpha;
                    }
                    var item1 = new InputSettingItem(position,localScale,alpha);
                    mInputSettingBattleItemListOrigin.ETCButtonlistAdd(item1);
                    var item2 = new InputSettingItem(position,localScale,alpha);
                    mInputSettingBattleItemListCurr.ETCButtonlistAdd(item2);
                }*/
                
                /*if (mButtonsOriginPos == Vector3.zero)
                {
                    var position = InputManager.instance.ETCButtons.transform.position;
                    mButtonsOriginPos = position;
                    mButtonsCurrPos = position;
                }

                if (mListButtonOriginPos.Count == 0)
                {
                    foreach (var button in InputManager.instance.ButtonSlotMap.Values)
                    {
                        var position = button.transform.position;
                        mListButtonOriginPos.Add(position);
                        mListButtonCurrPos.Add(position);
                        mListButtonVisible.Add(button.visible);
                    }
                }
                
                var canvasGroup1 = InputManager.instance.joystick.GetComponent<CanvasGroup>();
                if (canvasGroup1 != null)
                {
                    var alpha = canvasGroup1.alpha;
                    mJoystickOriginTransparent = alpha;
                    mJoystickCurrTransparent = alpha;
                }
                
                var canvasGroup2 = InputManager.instance.ETCButtons.GetComponent<CanvasGroup>();
                if (canvasGroup2 != null)
                {
                    var alpha = canvasGroup2.alpha;
                    mButtonsOriginTransparent = alpha;
                    mButtonsCurrTransparent = alpha;
                }
                
                if (mListButtonOriginTransparent.Count == 0)
                {
                    foreach (var button in InputManager.instance.ButtonSlotMap.Values)
                    {
                        var canvasGroup = button.GetComponent<CanvasGroup>();
                        if (canvasGroup != null)
                        {
                            mListButtonOriginTransparent.Add(canvasGroup.alpha);
                            mListButtonCurrTransparent.Add(canvasGroup.alpha);
                        }
                        else
                        {
                            mListButtonOriginTransparent.Add(1.0f);
                            mListButtonCurrTransparent.Add(1.0f);
                        }
                    }
                }*/

                if (null != mDragPadJoystick)
                {
                    mDragPadJoystick.SetCanDrag(true);
                    mDragPadJoystick.SetCallBack(_onDragCallBack); 
                    mDragPadJoystick.SetDragTransform(InputManager.instance.joystick.rectTransform());
                }
                
                var battleUIDrug = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
                if (battleUIDrug != null)
                {
                    if (null != mDragBattleUIDrug)
                    {
                        mDragBattleUIDrug.SetCanDrag(true);
                        mDragBattleUIDrug.SetCallBack(_onDragCallBack);
                        mDragBattleUIDrug.SetDragTransform(battleUIDrug.GetDragObj().rectTransform());
                    }
                }

                var battleUISwitchWeaAndEquip = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
                if (battleUISwitchWeaAndEquip != null)
                {
                    if (null != mDragBattleUISwitchWeaAndEquip)
                    {
                        mDragBattleUISwitchWeaAndEquip.SetCanDrag(true);
                        mDragBattleUISwitchWeaAndEquip.SetCallBack(_onDragCallBack);
                        mDragBattleUISwitchWeaAndEquip.SetDragTransform(battleUISwitchWeaAndEquip.GetDragObj()
                            .rectTransform());
                    }
                }

                int index = 0;
                foreach (var button in InputManager.instance.ButtonSlotMap.Values)
                {
                    if (index < mListDragPadButton.Count)
                    {
                        mListButtonVisible.Add(button.gameObject.activeSelf);
                        mListDragPadButton[index].SetCanDrag(true);
                        mListDragPadButton[index].SetCallBack(_onDragCallBack);
                        mListDragPadButton[index].SetDragTransform(button.rectTransform());
                        //mListDragPadButton[index].transform.position = button.transform.position;
                        //mListDragPadButton[index].rectTransform().sizeDelta = button.rectTransform().sizeDelta;
                        
                        index++;
                    }
                }
                
                if (battleUIDrug != null && battleUIDrug.GetNeedShowObj() != null)
                {
                    mListButtonVisible.Add(battleUIDrug.GetNeedShowObj().activeSelf);
                    index++;
                }
                
                if (battleUISwitchWeaAndEquip != null && battleUISwitchWeaAndEquip.GetNeedShowObj() != null)
                {
                    mListButtonVisible.Add(battleUISwitchWeaAndEquip.GetNeedShowObj().activeSelf);
                    index++;
                }

                //if (null != mDragPadJoystick)
                //{
                    //mDragPadJoystick.transform.position = InputManager.instance.joystick.transform.position;
                    
                    //mDragPadJoystick.rectTransform().sizeDelta = InputManager.instance.joystick.rectTransform().sizeDelta;

                    //mScaleScripts = InputManager.instance.joystick.GetComponent<ComScaleScripts>();
                //}
                
                mDragPadAllScaleMode1.SetDragTransform(InputManager.instance.joystick.rectTransform());
                mDragPadAllScaleMode1.SetCanDrag(true);
                mDragPadAllScaleMode1.SetCallBack(_onDragCallBackAllChangeMode);
                mDragPadAllScaleMode2.SetDragTransform(InputManager.instance.ETCButtons.transform.rectTransform());
                mDragPadAllScaleMode2.SetCanDrag(true);
                mDragPadAllScaleMode2.SetCallBack(_onDragCallBackAllChangeMode);
                
            }
        }

        private void SetComDragPadSize()
        {
            if (mDragPadJoystick.dragTran != null)
            {
                mDragPadJoystick.rectTransform().sizeDelta = mDragPadJoystick.dragTran.rectTransform().sizeDelta;
            }
            
            if (mDragBattleUIDrug.dragTran != null)
            {
                mDragBattleUIDrug.rectTransform().sizeDelta = mDragBattleUIDrug.dragTran.rectTransform().sizeDelta;
            }
            
            if (mDragBattleUISwitchWeaAndEquip.dragTran != null)
            {
                mDragBattleUISwitchWeaAndEquip.rectTransform().sizeDelta = mDragBattleUISwitchWeaAndEquip.dragTran.rectTransform().sizeDelta;
            }
            
            for (int i = 0; i < mListDragPadButton.Count; i++)
            {
                if (mListDragPadButton[i].dragTran != null && i < mInputSettingBattleItemListCurr.mETCButtonlist.Count)
                {
                    mListDragPadButton[i].rectTransform().sizeDelta = mListDragPadButton[i].dragTran.rectTransform().sizeDelta * mInputSettingBattleItemListCurr.mETCButtons.scale;
                }
            }
        }

        public void SetInputEnable(bool enable)
        {
            if (InputManager.instance != null)
            {
                int index = 0;
                foreach (var button in InputManager.instance.ButtonSlotMap.Values)
                {
                    if (index < mListButtonVisible.Count && index < mListDragPadButton.Count)
                    {
                        button.SetSkillBtnVisible(enable ? mListButtonVisible[index] : true);
                        button.activated = enable;
                        if (button.name == "Btn_Jump")
                        {
                            button.SetSkillBtnVisible(false);
                            button.activated = false;
                        }
                        //mListDragPadButton[index].transform.position = button.transform.position;
                        //mListDragPadButton[index].rectTransform().sizeDelta = button.rectTransform().sizeDelta;
                        index++;
                    }
                }
                
                var battleUIDrug = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
                if (battleUIDrug != null && battleUIDrug.GetNeedShowObj() != null)
                {
                    battleUIDrug.GetNeedShowObj().CustomActive(enable ? mListButtonVisible[index] : true);
                    index++;
                }
            
                var battleUISwitchWeaAndEquip = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
                if (battleUISwitchWeaAndEquip != null && battleUISwitchWeaAndEquip.GetNeedShowObj() != null)
                {
                    battleUISwitchWeaAndEquip.GetNeedShowObj().CustomActive(enable ? mListButtonVisible[index] : true);
                    index++;
                }
                
                if(InputManager.instance.joystick != null)
                    InputManager.instance.joystick.activated = enable;

                //if (null != mDragPadJoystick)
                //{
                //    mDragPadJoystick.transform.position = InputManager.instance.joystick.transform.position;
                //    mDragPadJoystick.rectTransform().sizeDelta = InputManager.instance.joystick.rectTransform().sizeDelta;
                //}
                
                //mCurPad.transform.localScale = mCurPad.dragTran.localScale;
                
                //mAllScaleMode1.transform.position = InputManager.instance.joystick.transform.position;
                //mAllScaleMode1.rectTransform().sizeDelta = InputManager.instance.joystick.rectTransform().sizeDelta + Vector2.one * 150;
            }
        }
        
        public void SetCurrInputItemData()
        {
            if (InputManager.instance != null)
            {
                /*int index = 0;
                foreach (var button in InputManager.instance.ButtonSlotMap.Values)
                {
                    if (index < mListButtonVisible.Count && index < mListDragPadButton.Count)
                    {
                        mListDragPadButton[index].transform.position = button.transform.position;
                        mListDragPadButton[index].rectTransform().sizeDelta = button.rectTransform().sizeDelta;
                        index++;
                    }
                }
                
                if (null != mDragPadJoystick)
                {
                    mDragPadJoystick.transform.position = InputManager.instance.joystick.transform.position;
                    mDragPadJoystick.rectTransform().sizeDelta = InputManager.instance.joystick.rectTransform().sizeDelta;
                }
                
                mCurPad.transform.localScale = mCurPad.dragTran.localScale;
                
                mAllScaleMode1.transform.position = InputManager.instance.joystick.transform.position;
                mAllScaleMode1.rectTransform().sizeDelta = InputManager.instance.joystick.rectTransform().sizeDelta + Vector2.one * 150;*/
                
                mDragPadJoystick.SetInputSettingData(mInputSettingBattleItemListCurr.mJoystick);
                mDragBattleUIDrug.SetInputSettingData(mInputSettingBattleItemListCurr.mBattleUIDrug);
                mDragBattleUISwitchWeaAndEquip.SetInputSettingData(mInputSettingBattleItemListCurr.mBattleUISwitchWeaAndEquip);
                
                if (InputManager.instance.joystick != null)
                {
                    InputManager.instance.joystick.SetInitPosition(InputManager.instance.joystick.rectTransform()
                        .anchoredPosition);
                    InputManager.instance.joystick.ChangeMaxOffset(
                        (int) (mInputSettingBattleItemListCurr.mJoystick.position.x -
                               mInputSettingBattleItemListOrigin.mJoystick.position.x),
                        (int) (mInputSettingBattleItemListCurr.mJoystick.position.y -
                               mInputSettingBattleItemListOrigin.mJoystick.position.y));
                    var com = InputManager.instance.joystick.GetComponent<ComScaleScripts>();
                    if (com != null)
                    {
                        com.SetBaseScale(mInputSettingBattleItemListCurr.mJoystick.scale.x);
                    }
                }

                for (int i = 0; i < mInputSettingBattleItemListCurr.mETCButtonlist.Count; i++)
                {
                    mListDragPadButton[i].SetInputSettingData(mInputSettingBattleItemListCurr.mETCButtonlist[i]);
                }
                
                mDragPadAllScaleMode1.SetInputSettingData(mInputSettingBattleItemListCurr.mJoystick);
                mDragPadAllScaleMode2.SetInputSettingData(mInputSettingBattleItemListCurr.mETCButtons);
            }
        }
        
        private void _onButtonAllChangeToggleValueChange(bool changed)
        {
            SetInputEnable(false);
            SetCurrInputItemData();
            SetComDragPadSize();
            mImageControl.CustomActive(false);
            
            if (mButtonAllChange.isOn)
            {
                mSettingAllScale.CustomActive(true);
                mDragPadJoystick.CustomActive(false);
                mDragBattleUIDrug.SetCanDrag(false);
                mDragBattleUISwitchWeaAndEquip.SetCanDrag(false);
                mDragPadButtons.CustomActive(false);
                SetAllScaleModeTrans();
                _onDragCallBackAllChangeMode(mDragPadAllScaleMode1);
                mAllScaleMode1.isOn = true;
            }
            else
            {
                mSettingAllScale.CustomActive(false);
                mDragPadJoystick.CustomActive(true);
                mDragBattleUIDrug.SetCanDrag(true);
                mDragBattleUISwitchWeaAndEquip.SetCanDrag(true);
                mDragPadButtons.CustomActive(true);
                for (int i = 0; i < mListDragPadButton.Count; i++)
                {
                    if (mListDragPadButton[i].dragTran != null && mListDragPadButton[i].dragTran.name == "Btn_Attack")
                    {
                        _onDragCallBack(mListDragPadButton[i]);
                        break;
                    }
                }
            }
        }
        
        private void SetAllScaleModeTrans()
        {
            mAllScaleMode1.transform.position = InputManager.instance.joystick.transform.position;
            mAllScaleMode1.rectTransform().sizeDelta = InputManager.instance.joystick.rectTransform().sizeDelta + Vector2.one * 100;
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            for (int i = 0; i < mListDragPadButton.Count; i++)
            {
                if (mListDragPadButton[i].dragTran != null)
                {
                    float _minX = mListDragPadButton[i].dragTran.rectTransform().position.x - mListDragPadButton[i].dragTran.rectTransform().sizeDelta.x/2 * mListDragPadButton[i].dragTran.rectTransform().lossyScale.x;
                    if (_minX < minX)
                        minX = _minX;
                    float _minY = mListDragPadButton[i].dragTran.rectTransform().position.y - mListDragPadButton[i].dragTran.rectTransform().sizeDelta.y/2 * mListDragPadButton[i].dragTran.rectTransform().lossyScale.x;
                    if (_minY < minY)
                        minY = _minY;
                    float _maxX = mListDragPadButton[i].dragTran.rectTransform().position.x + mListDragPadButton[i].dragTran.rectTransform().sizeDelta.x/2 * mListDragPadButton[i].dragTran.rectTransform().lossyScale.x;
                    if (_maxX > maxX)
                        maxX = _maxX;
                    float _maxY = mListDragPadButton[i].dragTran.rectTransform().position.y + mListDragPadButton[i].dragTran.rectTransform().sizeDelta.y/2 * mListDragPadButton[i].dragTran.rectTransform().lossyScale.x;
                    if (_maxY > maxY)
                        maxY = _maxY;
                }
            }
            mAllScaleMode2.transform.position = new Vector3((maxX+minX)/2,(maxY+minY)/2 ,0.0f);
            mAllScaleMode2.rectTransform().sizeDelta = new Vector2((maxX-minX),(maxY-minY)) / mAllScaleMode2.rectTransform().lossyScale;
        }
        
        private void _onAllScaleMode1ToggleValueChange(bool changed)
        {
            
        }
        
        private void _onAllScaleMode2ToggleValueChange(bool changed)
        {
            
        }
        
        void _onChangeButtonSize(float value)
        {
            if (null != mCurPad)
            {
                mCurPad.dragTran.localScale = ((MaxButtonSize - MinButtonSize) * value + MinButtonSize) * Vector3.one;
                mCurPad.transform.localScale = mCurPad.dragTran.localScale;
                ChangeInputSettingItemData(mCurPad,
                    new EnumHelper<InputSettingItemChangeTypeFlag>(InputSettingItemChangeTypeFlag.scale),
                    default, mCurPad.dragTran.localScale);
                
                if(mCurPad == mDragPadAllScaleMode2)
                    SetAllScaleModeTrans();
            }
            if(null != mImageControl && mImageControl.enabled)
            {
                mImageControl.rectTransform().sizeDelta = mCurPad.dragTran.localScale.x * mCurPad.dragTran.sizeDelta;
                for (int i = 0; i < mListDragPadButton.Count; i++)
                {
                    if (i < mInputSettingBattleItemListCurr.mETCButtonlist.Count && mListDragPadButton[i] == mCurPad)
                    {
                        mImageControl.rectTransform.sizeDelta = mCurPad.dragTran.localScale.x * mCurPad.dragTran.sizeDelta * mInputSettingBattleItemListCurr.mETCButtons.scale;
                        return;
                    }
                }
            }
        }
        
        void _onChangeTransparentSize(float value)
        {
            if (null != mCurPad)
            {
                var canvasGroup = mCurPad.dragTran.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = mCurPad.dragTran.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = ((MaxButtonTransparent - MinButtonTransparent) * value + MinButtonTransparent);
                ChangeInputSettingItemData(mCurPad,
                    new EnumHelper<InputSettingItemChangeTypeFlag>(InputSettingItemChangeTypeFlag.alpha),
                    default, default, canvasGroup.alpha);
            }
        }
        
        void ChangeInputSettingItemData(ComDragPad pad, EnumHelper<InputSettingItemChangeTypeFlag> flags = default, Vector3 position = default,Vector3 localScale = default, float alpha = 1.0f)
        {
            if (pad == mDragPadJoystick || pad == mDragPadAllScaleMode1)
            {
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.position))
                {
                    mInputSettingBattleItemListCurr.mJoystick.SetDataPosition(position);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.scale))
                {
                    mInputSettingBattleItemListCurr.mJoystick.SetDataScale(localScale);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.alpha))
                {
                    mInputSettingBattleItemListCurr.mJoystick.SetDataAlpha(alpha);
                }
                return;
            }
            if (pad == mDragBattleUIDrug)
            {
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.position))
                {
                    mInputSettingBattleItemListCurr.mBattleUIDrug.SetDataPosition(position);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.scale))
                {
                    mInputSettingBattleItemListCurr.mBattleUIDrug.SetDataScale(localScale);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.alpha))
                {
                    mInputSettingBattleItemListCurr.mBattleUIDrug.SetDataAlpha(alpha);
                }
                return;
            }
            if (pad == mDragBattleUISwitchWeaAndEquip)
            {
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.position))
                {
                    mInputSettingBattleItemListCurr.mBattleUISwitchWeaAndEquip.SetDataPosition(position);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.scale))
                {
                    mInputSettingBattleItemListCurr.mBattleUISwitchWeaAndEquip.SetDataScale(localScale);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.alpha))
                {
                    mInputSettingBattleItemListCurr.mBattleUISwitchWeaAndEquip.SetDataAlpha(alpha);
                }
                return;
            }
            if (pad == mDragPadAllScaleMode2)
            {
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.position))
                {
                    mInputSettingBattleItemListCurr.mETCButtons.SetDataPosition(position);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.scale))
                {
                    mInputSettingBattleItemListCurr.mETCButtons.SetDataScale(localScale);
                }
                if (flags.HasFlag(InputSettingItemChangeTypeFlag.alpha))
                {
                    mInputSettingBattleItemListCurr.mETCButtons.SetDataAlpha(alpha);
                }
                return;
            }

            for (int i = 0; i < mListDragPadButton.Count; i++)
            {
                if (i < mInputSettingBattleItemListCurr.mETCButtonlist.Count && mListDragPadButton[i] == pad)
                {
                    if (flags.HasFlag(InputSettingItemChangeTypeFlag.position))
                    {
                        mInputSettingBattleItemListCurr.mETCButtonlist[i].SetDataPosition(position);
                    }
                    if (flags.HasFlag(InputSettingItemChangeTypeFlag.scale))
                    {
                        mInputSettingBattleItemListCurr.mETCButtonlist[i].SetDataScale(localScale);
                    }
                    if (flags.HasFlag(InputSettingItemChangeTypeFlag.alpha))
                    {
                        mInputSettingBattleItemListCurr.mETCButtonlist[i].SetDataAlpha(alpha);
                    }
                    return;
                }
            }
        }

        private void _onDropdownDropdownValueChange(int index)
        {
            var data = InputSettingBattleManager.instance.GetInputSettingBattleProgram((InputSettingBattleProgramType)index);
            if (data != null)
            {
                mInputSettingBattleItemListCurr.SetData(data);
            }
            else
            {
                mInputSettingBattleItemListCurr.SetData(mInputSettingBattleItemListOrigin);
            }
            
            SetEditMode(false);
            SetInputEnable(true);
            SetCurrInputItemData();
            SetComDragPadSize();

            if (InputManager.instance != null)
            {
                InputManager.instance.ResetETCEffectTrans();
            }
            InputSettingBattleManager.instance.SaveInputSettingBattleProgramType((InputSettingBattleProgramType)index);
        }
        private void _onChangeNameButtonClick()
        {
            GuildCommonModifyData data = new GuildCommonModifyData();
            data.bHasCost = false;
            data.nMaxWords = 7;
            data.onOkClicked = (string a_strValue) =>
            {
                InputSettingBattleManager.instance.SaveInputSettingBattleItemProgramNameData((InputSettingBattleProgramType)mDropdown.value, a_strValue);
                ClientSystemManager.GetInstance().CloseFrame<GuildCommonModifyFrame>();
            };
            data.strTitle = TR.Value("inputting_setting_battle_changeName_title");
            data.strEmptyDesc = TR.Value("inputting_setting_battle_changeName_desc");
            data.strDefultContent = InputSettingBattleManager.instance.GetInputSettingBattleProgramName((InputSettingBattleProgramType)mDropdown.value);
            data.eMode = EGuildCommonModifyMode.Short;
            ClientSystemManager.GetInstance().OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
        }
        
        public void OpenSaveProgramSystemNotify()
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("inputting_setting_battle_save"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("inputting_setting_battle_btn_not_save"),
                RightButtonText = TR.Value("inputting_setting_battle_btn_save"),
                OnLeftButtonClickCallBack = NotSaveInputSettingBattleProgram,
                OnRightButtonClickCallBack = SaveInputSettingBattleProgram,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }
        
        private void NotSaveInputSettingBattleProgram()
        {
            var data = InputSettingBattleManager.instance.GetInputSettingBattleProgram(
                (InputSettingBattleProgramType) mDropdown.value);
            if(data != null)
            {
                mInputSettingBattleItemListCurr.SetData(data);
            }
            else
            {
                mInputSettingBattleItemListCurr.SetData(mInputSettingBattleItemListOrigin);
            }

            SetEditMode(false);
            SetInputEnable(true);
            SetCurrInputItemData();
            SetComDragPadSize();

            if (InputManager.instance != null)
            {
                InputManager.instance.ResetETCEffectTrans();
            }
        }
        
        private void SaveInputSettingBattleProgram()
        {
            SetEditMode(false);
            SetInputEnable(true);
            SetCurrInputItemData();
            SetComDragPadSize();

            if (InputManager.instance != null)
            {
                InputManager.instance.ResetETCEffectTrans();
            }
            
            InputSettingBattleManager.instance.SaveInputSettingBattleItemProgramData((InputSettingBattleProgramType)mDropdown.value, mInputSettingBattleItemListCurr);
            InputSettingBattleManager.instance.SaveInputSettingBattleProgramType((InputSettingBattleProgramType)mDropdown.value);
        }
        
        public void OpenResetProgramSystemNotify()
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("inputting_setting_battle_reset"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = ResetInputSettingBattleProgram,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }
        
        private void ResetInputSettingBattleProgram()
        {
            mInputSettingBattleItemListCurr.SetData(mInputSettingBattleItemListOrigin);

            SetCurrInputItemData();
            SetAllScaleModeTrans();
            SetComDragPadSize();
            SetCurrInputItemData();
            
            mImageControl.CustomActive(false);
            mButtonAllChange.isOn = false;
            for (int i = 0; i < mListDragPadButton.Count; i++)
            {
                if (mListDragPadButton[i].dragTran != null && mListDragPadButton[i].dragTran.name == "Btn_Attack")
                {
                    _onDragCallBack(mListDragPadButton[i]);
                    break;
                }
            }
        }
    }
}
