using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ComDotController : MonoBehaviour
    {
        [SerializeField]
        private GameObject dotRoot;
        [SerializeField]
        private GameObject dotParent;
        [SerializeField]
        private List<GameObject> dotsParent = new List<GameObject>();

        private List<GameObject> dots = new List<GameObject>();

        private int m_MaxPageNum = 0;

        void OnDestroy()
        {
            if (dotsParent != null)
            {
                dotsParent.Clear();
            }
            if (dots != null)
            {
                dots.Clear();
            }

            m_MaxPageNum = 0;
        }

        public void InitDots(int iMaxPage, bool needMoreThanTwo = true)
        {
            m_MaxPageNum = iMaxPage;

            if (dotParent == null || dotRoot == null)
            {
                return;
            }
            for (int i = 0; i < iMaxPage; i++)
            {
                GameObject dotGo = null;
                if (i < dotsParent.Count)
                {
                    dotGo = dotsParent[i];
                }
                else
                {
                    dotGo = GameObject.Instantiate(dotParent);
                    Utility.AttachTo(dotGo, dotRoot);
                    dotsParent.Add(dotGo);
                }
            }

            if (dots != null)
            {
                dots.Clear();
            }

            for (int i = 0; i < dotsParent.Count; i++)
            {
                GameObject dotGo = dotsParent[i];
                if (dotGo)
                {
                    GameObject dotChild = dotGo.transform.GetChild(0).gameObject;
                    if (dotChild)
                    {
                        dots.Add(dotChild);
                    }
                }
            }

            if (needMoreThanTwo)
            {
                this.gameObject.CustomActive(iMaxPage >= 2);
            }
        }

        public void SetDots(int iPage,int iMaxPage = 0)
        {
            if (iMaxPage <= 0)
            {
                iMaxPage = m_MaxPageNum;
            }

            if (dotsParent == null || dots == null)
            {
                return;
            }
            if (dotsParent.Count != dots.Count)
            {
                return;
            }
            
            int iIndex = iPage - 1;
            for(int i = 0; i < dotsParent.Count; ++i)
            {
                dotsParent[i].CustomActive(i < iMaxPage);
            }

            for (int i = 0; i < dots.Count; ++i)
            {
                dots[i].CustomActive(i == iIndex);
            }
        }
    }
}