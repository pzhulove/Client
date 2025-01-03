using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ChijiBackPackDisplayNode : MonoBehaviour
    {
        [SerializeField]
        private StateController mStateControl;
        private string normal = "normal";
        private string chiji = "chiji";

        private void Awake()
        {
            var current = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (current != null)
            {
                if (mStateControl != null)
                {
                    mStateControl.Key = chiji;
                }
            }
            else
            {
                if (mStateControl != null)
                {
                    mStateControl.Key = normal;
                }
            }
        }

        private void OnDestroy()
        {
            mStateControl = null;
        }
    }
}
