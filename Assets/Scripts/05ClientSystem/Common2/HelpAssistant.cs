using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTable;

namespace GameClient
{
    [RequireComponent(typeof(Button))]
    class HelpAssistant : MonoBehaviour
    {
        Button button = null;
        public HelpFrameContentTable.eHelpType eType = HelpFrameContentTable.eHelpType.HT_MISSION;
        // Use this for initialization
        void Start()
        {
            button = GetComponent<Button>();
            if(button != null)
            {
                button.onClick.AddListener(_OnClickHelp);
            }
        }

        void _OnClickHelp()
        {
            ClientSystemManager.GetInstance().OpenFrame<HelpFrame>(GameClient.FrameLayer.Middle,eType);
        }

        void OnDestroy()
        {
            if(button != null)
            {
                button.onClick.RemoveAllListeners();
                button = null;
            }
        }
    }
}