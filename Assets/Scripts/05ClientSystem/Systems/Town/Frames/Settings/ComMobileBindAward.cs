using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameClient;
using System.Text.RegularExpressions;

namespace Settings_old
{
    public class MobileBindButtonState
    {
        public const string UNENABLE_RECEIVE = "无法领取";
        public const string TO_RECEIVE = "领取";
        public const string RECEIVED = "已领取";
        public const string SEND_VERIFICATION_CODE = "发送验证码";
        public const string PUSH_VERIFY = "提交";
    }
    public class ComMobileBindAward : MonoBehaviour
    {
        public enum ReceiveBtnState
        {
            UnReceive,
            ToReceive,
            Received
        }

        public enum SendVerifyBtnState
        {
            Normal,
            Sending
        }

        public Transform[] awardItems;
        public Button receiveBtn;
        public Text receiveBtnText;
        public UIGray[] receiveGrayComs;

        public InputField mobileNumInput;
        public InputField verificationCodeInput;
        public Button sendVerifyBtn;
        public Text sendVerifyBtnText;
        public Button pushVerifyBtn;
        public int TotalCountDownNum = 60;

        private ReceiveBtnState receiveBtnState;

        private SendVerifyBtnState sendVerifyBtnState;
        private int countDownNum;
        
        //TODO
        //添加item数据

        void OnEnable()
        {
            countDownNum = TotalCountDownNum;
        }

        void Disable()
        {

            UnInitBtns();
        }

        void InitCom(ClientFrame currFrame)
        {
            InitBtns();
            InitAwardItems(currFrame);
        }

        void InitBtns()
        {
            if (receiveBtn)
            {
                receiveBtn.onClick.RemoveListener(OnReceiveBtnClick);
                receiveBtn.onClick.AddListener(OnReceiveBtnClick);
            }
            if (sendVerifyBtn)
            {
                sendVerifyBtn.onClick.RemoveListener(OnSendVerifyBtnClick);
                sendVerifyBtn.onClick.AddListener(OnSendVerifyBtnClick);
            }
            if (pushVerifyBtn)
            {
                pushVerifyBtn.onClick.RemoveListener(OnPushVerifyBtnClick);
                pushVerifyBtn.onClick.AddListener(OnPushVerifyBtnClick);
            }

            InitBtnsState();
        }

        void UnInitBtns()
        {
            if (receiveBtn)
            {
                receiveBtn.onClick.RemoveListener(OnReceiveBtnClick);
            }
            if (sendVerifyBtn)
            {
                sendVerifyBtn.onClick.RemoveListener(OnSendVerifyBtnClick);
            }
            if (pushVerifyBtn)
            {
                pushVerifyBtn.onClick.RemoveListener(OnPushVerifyBtnClick);
            }
        }

        void InitBtnsState()
        {
            SetReceiveBtnState(ReceiveBtnState.UnReceive);
            SetSendVerifyBtnState(SendVerifyBtnState.Normal);
        }

        void InitAwardItems(ClientFrame currFrame)
        {
            if(currFrame == null)
                return;
            if (awardItems != null)
            {
                for (int i = 0; i < awardItems.Length; i++)
                {
                    ComItem awardItem = currFrame.CreateComItem(awardItems[i].gameObject);
                    //var itemData = ItemDataManager.CreateItemDataFromTable();
                }
            }
        }

        void OnReceiveBtnClick()
        {

        }

        void OnSendVerifyBtnClick()
        {

        }

        void OnPushVerifyBtnClick()
        {

        }

        void SetReceiveBtnState(ReceiveBtnState state)
        {
            if (receiveBtnText == null)
                return;
            switch (state)
            {
                case ReceiveBtnState.UnReceive:
                    receiveBtnText.text = MobileBindButtonState.UNENABLE_RECEIVE;
                    break;
                case ReceiveBtnState.ToReceive:
                    receiveBtnText.text = MobileBindButtonState.TO_RECEIVE;
                    break;
                case ReceiveBtnState.Received:
                    receiveBtnText.text = MobileBindButtonState.RECEIVED;
                    break;
            }
        }

        void SetSendVerifyBtnState(SendVerifyBtnState state)
        {
            if (sendVerifyBtnText == null) return;
            switch (state)
            {
                case SendVerifyBtnState.Normal:
                    sendVerifyBtnText.text = MobileBindButtonState.SEND_VERIFICATION_CODE;
                    break;
                case SendVerifyBtnState.Sending:
                    sendVerifyBtnText.text = MobileBindButtonState.SEND_VERIFICATION_CODE + "("+countDownNum+")";
                    break;
            }
        }

        void StartSendVerifyCountDown()
        {
        }

        bool CheckIsRightMobileNum(string mNum)
        {
            Regex regex = new Regex(@"^[1]+[3,4,5,8]+\d{9}");
            return regex.IsMatch(mNum);
        }
    }
}
