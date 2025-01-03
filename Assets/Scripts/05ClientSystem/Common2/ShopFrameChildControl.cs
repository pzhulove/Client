using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class ShopFrameChildControl : MonoBehaviour
    {
        public List<GameObject> mainVisible = new List<GameObject>();
        public List<GameObject> childVisible = new List<GameObject>();
        public List<GameObject> guildVisible = new List<GameObject>();

        public void SetMode(ShopFrame.ShopFrameMode eMode)
        {
            List<GameObject> targets = null;
            if (eMode == ShopFrame.ShopFrameMode.SFM_CHILD_FRAME)
            {
                targets = childVisible;
            }
            else if(eMode == ShopFrame.ShopFrameMode.SFM_MAIN_FRAME)
            {
                targets = mainVisible;
            }
            else if(eMode == ShopFrame.ShopFrameMode.SFM_GUILD_CHILD_FRAME)
            {
                targets = guildVisible;
            }

            for(int i = 0; i < targets.Count; ++i)
            {
                if(targets[i] != null)
                {
                    targets[i].CustomActive(true);
                }
            }
        }
    };
}