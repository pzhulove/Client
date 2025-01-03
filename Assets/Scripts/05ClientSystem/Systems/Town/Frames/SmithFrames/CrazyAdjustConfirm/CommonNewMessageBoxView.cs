using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class CommonNewMessageBoxView : MonoBehaviour
    {
        #region MODEL PARAMS
        static  MsgBoxNewdata data = null;

        static MsgBoxNewdata Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        bool IsReverse
        {
            get
            {
                return _HasFlag(MsgBoxDataType.MBDT_REVERSE);
            }
        }

        bool _HasFlag(MsgBoxDataType eFlag)
        {
            return (eFlag & Value.flags) == eFlag;
        }

        private Button tempCancelBtn;
        private Button tempOKBtn;
        #endregion

        #region VIEW PARAMS
        [SerializeField] private GameObject mTitleLabel;
        [SerializeField] private Text mContentLabel;
        [SerializeField] private ComUIListScript mToggleRoot;
        [SerializeField] private Button mCancelBtn;
        [SerializeField] private Button mOkBtn;
        [SerializeField] private Button mCloseBtn;
        [SerializeField] private ToggleGroup mGroup;
        #endregion


        #region PIRVATE METHODS

        private void Awake()
        {
            bStopCloseFrame = false;
            _BindUIEvents();
            _InitTR();
            _InitToggleScrollListBind();
        }

        private void OnDestroy()
        {
            bStopCloseFrame = false;
            _UnBindUIEvents();
        }

        protected virtual void _bindEvents()
        {

        }

        protected virtual void _unBindEvente()
        {

        }

        protected virtual void _BindUIEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.StopCloseCommonNewMessageBoxView,_OnStopCloseFrame);
        }

        protected virtual void _UnBindUIEvents()
        {
            if (tempCancelBtn != null)
            {
                tempCancelBtn.onClick.RemoveListener(_CloseFrame);
            }
            if (tempOKBtn != null)
            {
                tempOKBtn.onClick.RemoveListener(_TryCloseFrame);
            }
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveListener(_CloseFrame);
            }
            if(Value.ToggleListEvent == null)
            {
                if (Value.OnOK != null && tempOKBtn != null)
                {
                    tempOKBtn.onClick.RemoveListener(Value.OnOK);
                }
                if (Value.OnCancel != null && tempCancelBtn != null)
                {
                    tempCancelBtn.onClick.RemoveListener(Value.OnCancel);
                }
            }
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.StopCloseCommonNewMessageBoxView, _OnStopCloseFrame);
        }

        protected virtual void _InitTR()
        {
        }

        protected virtual void _ClearTR()
        {
        }

        protected virtual void _InitBaseData()
        {
            if (mTitleLabel)
            {
                //Text titleText = mTitleLabel.GetComponentInChildren<Text>();
                Text titleText = Utility.GetComponetInChild<Text>(mTitleLabel, "Title/titletext");
                if (titleText && string.IsNullOrEmpty(Value.title))
                {
                    titleText.text = Value.title;
                }
                else
                {
                    mTitleLabel.CustomActive(false);
                }
            }

            if (mContentLabel)
            {
                mContentLabel.text = Value.content;
            }

            //Text mOkBtnText = mOkBtn.GetComponentInChildren<Text>();
            //Text mCancelBtnText = mCancelBtn.GetComponentInChildren<Text>();
            Text mOkBtnText = Utility.GetComponetInChild<Text>(mOkBtn.gameObject, "Text");
            Text mCancelBtnText = Utility.GetComponetInChild<Text>(mCancelBtn.gameObject, "Text");

            if (IsReverse)
            {
                tempCancelBtn = mOkBtn;
                tempOKBtn = mCancelBtn;
                if (mOkBtnText)
                {
                    mOkBtnText.text = Value.cancel;
                }
                if (mCancelBtnText)
                {
                    mCancelBtnText.text = Value.ok;
                }
            }
            else
            {
                tempCancelBtn = mCancelBtn;
                tempOKBtn = mOkBtn;
                if (mOkBtnText)
                {
                    mOkBtnText.text = Value.ok;
                }
                if (mCancelBtnText)
                {
                    mCancelBtnText.text = Value.cancel;
                }
            }

            
            if (Value.ToggleListEvent != null)
            {
                if (Value.ToggleListEvent.Count > 0 && mToggleRoot != null)
                {
                    mToggleRoot.SetElementAmount(Value.ToggleListEvent.Count);
                }
            }
            else
            {
                if (Value.OnOK != null && tempOKBtn != null)
                {
                    tempOKBtn.onClick.AddListener(Value.OnOK);
                }
                if (Value.OnCancel != null && tempCancelBtn != null)
                {
                    tempCancelBtn.onClick.AddListener(Value.OnCancel);
                }
            }
            if (tempCancelBtn != null)
            {
                tempCancelBtn.onClick.AddListener(_CloseFrame);
            }
            if (tempOKBtn != null)
            {
                tempOKBtn.onClick.AddListener(_TryCloseFrame);
            }
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.AddListener(_CloseFrame);
            }
        }
        bool bStopCloseFrame = false;
        private void _TryCloseFrame()
        {
            if(bStopCloseFrame)
            {
                bStopCloseFrame = false;
                return;
            }
            _CloseFrame();
        }

        public static void _CloseFrame()
        {
            if(Value == null)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen("CommonNewMessageBox" + Value.iID.ToString())) 
            {
                ClientSystemManager.GetInstance().CloseFrame("CommonNewMessageBox" + Value.iID.ToString());
            }
        }
        #endregion

        #region UI
        private void _OnCloseButtonClickCallBack()
        {
            _CloseFrame();
        }
        private void _OnStopCloseFrame(UIEvent uiEvent)
        {
            bStopCloseFrame = true;
        }

        void _InitToggleScrollListBind()
        {
            mToggleRoot.Initialize();

            mToggleRoot.onItemVisiable = (item) =>
            {
                if (mGroup != null && item != null && item.m_index >= 0)
                {
                    mGroup.allowSwitchOff = true;
                    NewMessageBoxToggleUnit mItem = item.GetComponent<NewMessageBoxToggleUnit>();
                    if (mItem != null)
                    {
                        Button tempBtn;
                        if (IsReverse)
                        {
                            tempBtn = mCancelBtn;
                        }
                        else
                        {
                            tempBtn = mOkBtn;
                        }
                        mItem.InitBaseData(Value.ToggleListEvent[item.m_index], tempBtn);
                        mItem.UpdateItemInfo();
                    }
                    if(item.m_index == 0)
                    {
                        Toggle tempToggle = item.GetComponent<Toggle>();
                        if (tempToggle != null)
                        {
                            tempToggle.isOn = true;
                        }
                    }
                    mGroup.allowSwitchOff = false;
                }

            };

            mToggleRoot.OnItemRecycle = (item) =>
            {
                if (mGroup != null && item != null)
                {
                    mGroup.allowSwitchOff = true;
                    NewMessageBoxToggleUnit mItem = item.GetComponent<NewMessageBoxToggleUnit>();
                    if (mItem != null)
                    {
                        mItem.OnItemRecycle();
                    }
                    mGroup.allowSwitchOff = false;
                }
            };
        }
        #endregion

        #region PUBLIC METHODS
        public void InitData(MsgBoxNewdata data)
        {
            Value = data;
            _InitBaseData();
        }

        public void Clear()
        {
            _ClearTR();
        }
        #endregion
    }
}