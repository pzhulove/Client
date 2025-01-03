using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    [System.Serializable]
    public class BaseWebViewTitleInfo
    {
        public BaseWebViewType type;
        public string titleName;
    }

    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class ComBaseWebViewTitle : MonoBehaviour
    {
        [SerializeField]
        private BaseWebViewTitleInfo[] kindTitleInfos = null;

        private Text titleText;

        private void Awake()
        {
            titleText = this.GetComponent<Text>();
        }  

        private void OnDestroy()
        {
            kindTitleInfos = null;
        }

        public void SetTitleByType(BaseWebViewType type)
        {
            if (titleText == null)
            {
                return;
            }
            if (type == BaseWebViewType.None ||
                type == BaseWebViewType.Count)
            {
                return;
            }
            if (kindTitleInfos == null || kindTitleInfos.Length <= 0)
            {
                return;
            }
            for (int i = 0; i < kindTitleInfos.Length; i++)
            {
                var info = kindTitleInfos[i];
                if (info == null)
                    continue;
                if (info.type == type)
                {
                    titleText.text = info.titleName;
                    return;
                }
            }
        }
    }
}
