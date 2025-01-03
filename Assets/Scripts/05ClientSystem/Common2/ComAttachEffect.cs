using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ComAttachEffect : MonoBehaviour
    {
        public string effectPath = "";
        private GameObject effectObj = null;
        Button btn = null;
        
        /// <summary>
        /// 返回一下特效,如果是批量的,自己存起来再统一清一下.
        /// </summary>
        /// <returns></returns>
        public GameObject AddEffect()
        {
            btn = gameObject.GetComponent<Button>();
            if (btn != null)
            { 
                btn.onClick.AddListener(OnBtnClick);
                // 挂载特效
                if (!string.IsNullOrEmpty(effectPath))
                {
                    effectObj = AssetLoader.instance.LoadResAsGameObject(effectPath);
                    if (effectObj != null)
                    {
                        Utility.AttachTo(effectObj, btn.gameObject);
                        return effectObj;
                    }  
                }
            }
            return null;
        }

        void OnBtnClick()
        {
            GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            // 删除特效
            if (effectObj != null)
            {
                Destroy(effectObj);
            }

            effectObj = null;
            btn.onClick.RemoveListener(OnBtnClick);
        }
        
        
    }
}
