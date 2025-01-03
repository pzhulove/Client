using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class ComFriendBackBuffBonusInfo : MonoBehaviour
    {
        public Image buffIcon;
        public Text buffDes;
        
        public void OnItemVisible(BackBuffBonusInfo backBuffBonusInfo)
        {
            if (backBuffBonusInfo == null)
            {
                return;
            }

            if (buffIcon != null)
            {
                ETCImageLoader.LoadSprite(ref buffIcon, backBuffBonusInfo.IconPath);
            }

            if (buffDes != null)
            {
                buffDes.text = backBuffBonusInfo.Name;
            }
        }
    }
}