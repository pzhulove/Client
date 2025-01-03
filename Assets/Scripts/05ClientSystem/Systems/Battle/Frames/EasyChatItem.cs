using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EasyChatItem : MonoBehaviour
    {
        [SerializeField] private Button mEasyChatBtn;
        [SerializeField] private Text mEasyChatText;

        private int easyChatIndex;

        public void Init(int index)
        {
            easyChatIndex = index;
            InitUI();
        }

        public void Recycle()
        {
            easyChatIndex = -1;
            if(mEasyChatBtn != null)
            {
                mEasyChatBtn.onClick.RemoveListener(SendEasyChatMsg);
            }
        }

        private void InitUI()
        {
            if(mEasyChatBtn != null)
            {
                mEasyChatBtn.onClick.AddListener(SendEasyChatMsg);
            }
            if(mEasyChatText != null)
            {
                mEasyChatText.text = BattleEasyChatDataManager.GetInstance().GetEasyChatStringByIndex(easyChatIndex);
            }
        }

        private void SendEasyChatMsg()
        {
            BattleEasyChatDataManager.GetInstance().SendEasyChatTipsByIndex(easyChatIndex);
            if (ClientSystemManager.instance.IsFrameOpen<DungeonTeamChatFrame>())
            {
                ClientSystemManager.instance.CloseFrame<DungeonTeamChatFrame>();
            }
        }
    }
}

