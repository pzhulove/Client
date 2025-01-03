using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    [ExecuteAlways][RequireComponent(typeof(Image))]
    class SpriteAniRender : MonoBehaviour
    {
        public string orgPath;
        public string orgName;
        public int count;
        public int looptimes;
        public int playinterval;
        int iIndex;
        int iInterval;
        Image image;
        float fScale = 1.0f;
        public SpeedString speedString = new SpeedString(256);

        [System.NonSerialized]
        private List<Sprite> m_AnimSpirteList = new List<Sprite>();

        public void SetEnable(bool bEnable)
        {
            if (image != null)
            {
                if(bEnable)
                {
                    Sprite sprite = _LoadSprite(0);
                    image.rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x * 100.0f * fScale, sprite.bounds.size.y * 100.0f * fScale);
                    image.sprite = sprite;
                }
                else
                {
                    image.sprite = null;
                }
                image.enabled = bEnable;
            }
        }
        
        public void Reset(string orgPath,string orgName,int count,float fScale)
        {
            m_AnimSpirteList.Clear();
            this.orgPath = orgPath;
            this.orgName = orgName;
            this.count = count;
            iIndex = 0;
            image = GetComponent<Image>();
            if(image.enabled)
            {
                Sprite sprite = _LoadSprite(0);
                image.rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x * 100.0f * fScale, sprite.bounds.size.y * 100.0f * fScale);
            }
        }
        // Use this for initialization
        void Start()
        {
            iIndex = 0;
            iInterval = 0;
            image = GetComponent<Image>();
            if(count < 1)
            {
                count = 1;
            }
            if(playinterval < 1)
            {
                playinterval = 1;
            }
            if(image.enabled)
            {
                Sprite sprite = _LoadSprite(count - 1);
            }
        }

        void OnDestroy()
        {
            m_AnimSpirteList.Clear();
        }

        // Update is called once per frame
        void Update()
        {
            if(null == image || !image.enabled)
            {
                return;
            }

            if(iInterval >= playinterval)
            {
                iInterval = 0;
                _PlayOneTimes();
            }
            ++iInterval;
        }

        int _GetNumCount(int iValue)
        {
            iValue = iValue > 0 ? iValue : -iValue;
            int iNum = 0;
            if(iValue == 0)
            {
                iNum = 1;
            }
            else
            {
                while (iValue > 0)
                {
                    ++iNum;
                    iValue /= 10;
                }
            }
            return iNum;
        }

        Sprite _LoadSprite(int iIndex)
        {
            if (m_AnimSpirteList.Count <= iIndex)
            {
                for (int i = m_AnimSpirteList.Count,icnt = iIndex +1;i<icnt;++i)
                {
                    speedString.Clear();
                    speedString.Append(orgPath);
                    speedString.Append(orgName);

                    int iNumCount = _GetNumCount(i);

                    for (int j = 0; j < 5 - iNumCount;++j)
                    {
                        speedString.Append(0);
                    }
                    speedString.Append(i);

                    speedString.Append(".png:");
                    speedString.Append(orgName);

                    for (int j = 0; j < 5 - iNumCount; ++j)
                    {
                        speedString.Append(0);
                    }
                    speedString.Append(i);

                    var cur = speedString.GetBaseStringWithEndTrim();
                    m_AnimSpirteList.Add(AssetLoader.instance.LoadRes(cur, typeof(Sprite)).obj as Sprite);
                }
            }

            return m_AnimSpirteList[iIndex];
        }

        void _PlayOneTimes()
        {
            image.sprite = _LoadSprite(iIndex);
            iIndex = (iIndex + 1) % count;
        }
    }
}