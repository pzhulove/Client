using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    [RequireComponent(typeof(Button))]
    public class CommonHelpNewAssistant : MonoBehaviour
    {
        Button button = null;

        public int HelpId = 0;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(OnHelpButtonClick);
        }

        private void OnDestroy()
        {
            if(button != null)
                button.onClick.RemoveAllListeners();
        }

        private void OnHelpButtonClick()
        {
            if (HelpId <= 0)
            {
                Logger.LogError("HelpId is not more than zero");
                return;
            }

            if(ClientSystemManager.GetInstance().IsFrameOpen<CommonHelpNewFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<CommonHelpNewFrame>();
            ClientSystemManager.GetInstance().OpenFrame<CommonHelpNewFrame>(FrameLayer.Middle, HelpId);
        }
    }
}