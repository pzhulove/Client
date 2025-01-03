using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonReplaceView : MonoBehaviour
    {
        [SerializeField] private Text titleName;
        [SerializeField] private Text content;
        [SerializeField] private Button replaceBtn;
        [SerializeField] private CommonReplaceItem oldItem;
        [SerializeField] private CommonReplaceItem newItem;

        private void Awake()
        {
            if(replaceBtn != null)
            {
                replaceBtn.onClick.RemoveAllListeners();
                replaceBtn.onClick.AddListener(OnReplaceBtnClick);
            }
        }

        private void OnDestroy()
        {
            if (replaceBtn != null)
            {
                replaceBtn.onClick.RemoveListener(OnReplaceBtnClick);
            }

            commonReplaceData = null;
        }

        private CommonReplaceData commonReplaceData;
        public void InitView(CommonReplaceData commonReplaceData)
        {
            if (commonReplaceData == null)
                return;

            this.commonReplaceData = commonReplaceData;
            InitContent();

            if (oldItem != null)
                oldItem.InitItem(commonReplaceData, false);

            if (newItem != null)
                newItem.InitItem(commonReplaceData, true);
        }

        private void InitContent()
        {
            string name = string.Empty;
            string desc = string.Empty;
            switch (commonReplaceData.commonReplaceType)
            {
                case CommonReplaceType.CRT_BEAD:
                    name = TR.Value("common_replace_bead_title_name");
                    desc = TR.Value("common_replace_bead_content");
                    break;
                case CommonReplaceType.CRT_MAGICCARD:
                    name = TR.Value("common_replace_magic_card_title_name");
                    desc = TR.Value("common_replace_magic_card_content");
                    break;
                case CommonReplaceType.CRT_INSCRIPTIONMOSAIC:
                    name = TR.Value("common_replace_inscription_title_name");
                    desc = TR.Value("common_replace_inscription_content");
                    break;
            }

            if (titleName != null)
                titleName.text = name;

            if (content != null)
                content.text = desc;
        }
        
        private void OnReplaceBtnClick()
        {
            if (commonReplaceData == null)
                return;

            if (commonReplaceData.callBack == null)
                return;

            commonReplaceData.callBack();

            ClientSystemManager.GetInstance().CloseFrame<CommonReplaceFrame>();
        }
    }
}