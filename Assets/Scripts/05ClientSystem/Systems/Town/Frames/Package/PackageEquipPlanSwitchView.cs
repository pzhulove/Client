
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class PackageEquipPlanSwitchView : MonoBehaviour
    {

        [Space(10)]
        [HeaderAttribute("EquipPlanButton")]
        [Space(10)]
        [SerializeField] private Text equipPlanButtonText;
        [SerializeField] private Button equipPlanButton;

        [Space(20)]
        [HeaderAttribute("CountDownRoot")]
        [Space(10)]
        [SerializeField] private GameObject countDownRoot;
        [SerializeField] private Image countDownImage;
        [SerializeField] private Text countDownTimeLabel;

        private void Awake()
        {
            if (equipPlanButton != null)
            {
                equipPlanButton.onClick.RemoveAllListeners();
                equipPlanButton.onClick.AddListener(OnEquipPlanButtonClick);
            }
        }

        private void OnDestroy()
        {
            if (equipPlanButton != null)
                equipPlanButton.onClick.RemoveAllListeners();

            //时间戳重置
            EquipPlanDataManager.GetInstance().EquipPlanSwitchCountDownLeftTime = 0;
            EquipPlanDataManager.GetInstance().ResetUpdateCountDownTimeAction();
        }

        private void OnEnable()
        {
            OnEnableEquipPlanController();

            //换装数据同步之后的更新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveEquipPlanSwitchMessage,
                OnReceiveEquipPlanSwitchMessage);

            EquipPlanDataManager.GetInstance().SetUpdateCountDownTimeAction(UpdateEquipPlanSwitchState);
        }

        private void OnDisable()
        {

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveEquipPlanSwitchMessage,
                OnReceiveEquipPlanSwitchMessage);

            EquipPlanDataManager.GetInstance().ResetUpdateCountDownTimeAction();
        }

        private void OnEnableEquipPlanController()
        {
            UpdateEquipPlanButtonText();

            UpdateEquipPlanSwitchState();
        }

        private void OnReceiveEquipPlanSwitchMessage(UIEvent uiEvent)
        {
            UpdateEquipPlanButtonText();
        }

        //更新按钮的名字
        public void UpdateEquipPlanButtonText()
        {
            if (equipPlanButtonText == null)
                return;

            var equipPlanIdStr = EquipPlanUtility.GetEquipPlanIdStr(
                EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId);
            var contentFormatStr = TR.Value("Equip_Plan_Format_String", equipPlanIdStr);

            equipPlanButtonText.text = contentFormatStr;
        }

        //更新按钮的CD时间
        public void UpdateEquipPlanSwitchState()
        {

            var countDownLeftTime = EquipPlanDataManager.GetInstance().EquipPlanSwitchCountDownLeftTime;

            //不存在倒计时
            if (countDownLeftTime <= 0)
            {
                CommonUtility.UpdateGameObjectVisible(countDownRoot, false);
                return;
            }

            CommonUtility.UpdateGameObjectVisible(countDownRoot, true);

            //数字
            var timeValue = Mathf.CeilToInt(countDownLeftTime);
            if (countDownTimeLabel != null)
                countDownTimeLabel.text = timeValue.ToString();

            //图片
            if (countDownImage != null)
            {
                var fillAmountValue = countDownLeftTime / EquipPlanDataManager.EquipPlanSwitchCountDownInterval;

                if (fillAmountValue < 0)
                {
                    fillAmountValue = 0;
                }
                else if (fillAmountValue > 1)
                {
                    fillAmountValue = 1;
                }

                countDownImage.fillAmount = fillAmountValue;
            }
        }

        private void OnEquipPlanButtonClick()
        {
            EquipPlanUtility.OnSwitchEquipPlanAction();
        }


    }
}
