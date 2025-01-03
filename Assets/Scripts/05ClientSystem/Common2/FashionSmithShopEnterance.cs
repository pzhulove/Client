using UnityEngine;
using System.Collections;

namespace GameClient
{
    class FashionSmithShopEnterance : MonoBehaviour
    {
        public void OnClickOpenFashionSmithShopFrame()
        {
            FashionSmithShopFrame.OpenLinkFrame("1_0");
        }
    }
}