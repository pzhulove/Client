using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    [ExecuteAlways]
    public class UINumber : MonoBehaviour
    {
        public string preFixed;
        public List<Image> imgPools = new List<Image>();
        bool bInitialize = false;
        public int iValue = 0;
        
        public bool bIntDigitsNum2 = true;//路径资源数字位数2位或1位
        public bool bKeepZero = false;

        public int Value
        {
            get
            {
                return iValue;
            }
            set
            {
                iValue = value;
                m_bDirty = true;
            }
        }

        bool m_bDirty = true;

        void Start()
        {
            m_bDirty = true;
        }

        void Update()
        {
#if UNITY_EDITOR
            m_bDirty = true;
#endif
            if (!m_bDirty)
            {
                return;
            }
            m_bDirty = false;

            int iCurValue = iValue;
            for (int i = 0; i < imgPools.Count; ++i)
            {
                imgPools[imgPools.Count - i - 1].CustomActive(i == 0 || iCurValue > 0 || bKeepZero);
                // imgPools[imgPools.Count - i - 1].sprite = AssetLoader.instance.LoadRes(string.Format("{0}{1:D2}", preFixed, iCurValue % 10),typeof(Sprite)).obj as Sprite;
                Image img = imgPools[imgPools.Count - i - 1];
                string format = bIntDigitsNum2 ? "{0}{1:D2}" : "{0}{1:D1}";
                ETCImageLoader.LoadSprite(ref img, string.Format(format, preFixed, iCurValue % 10));
                iCurValue /= 10;
            }
        }

        void OnDestroy()
        {
            imgPools.Clear();
        }
    }
}