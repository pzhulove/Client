using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

namespace GameClient
{
    class ComExpandControlRedPointBinder : MonoBehaviour
    {      
        public GameObject ParentObj = null;
        public Image ParentRedPoint = null;
        public Text ParentRedPointText = null;

        // 红点对子节点没有顺序要求
        public List<GameObject> ChildrenObj = null; 
        public List<Image> ChildrenRedPoint = null;

        static public string[] stringTbl = null;

        void OnDestroy()
        {
            ParentObj = null;
            ParentRedPoint = null;
            ParentRedPointText = null;
            ChildrenObj = null;
            ChildrenRedPoint = null;
        }

        // Use this for initialization
        void Start()
        {
            if(ParentObj == null || ParentRedPoint == null || ParentRedPointText == null || 
               ChildrenObj == null || ChildrenRedPoint == null)
            {
                return;
            }
        }

        void Update()
        {
            if (ParentObj == null || ParentRedPoint == null || ParentRedPointText == null)
            {
                return;
            }
           
            bool bShowParent = false;
            if (ChildrenObj != null)
            {
                for (int i = 0; i < ChildrenObj.Count; i++)
                {
                    if(ChildrenObj[i] == null)
                    {
                        continue;
                    }

                    if (ChildrenObj[i].activeSelf)
                    {
                        bShowParent = true;
                        break;
                    }
                }
            }
            ParentObj.CustomActive(bShowParent);

            if (ChildrenRedPoint != null)
            {
                int iNum = 0;
                for (int i = 0; i < ChildrenRedPoint.Count; i++)
                {
                    if(ChildrenRedPoint[i] == null)
                    {
                        continue;
                    }

                    var redpointGo = ChildrenRedPoint[i].gameObject;
                    if (redpointGo == null || 
                        redpointGo.transform.parent == null || 
                        redpointGo.transform.parent.gameObject == null)
                        continue;
                    if (redpointGo.transform.parent.gameObject.activeSelf &&
                        redpointGo.activeSelf)
                    {
                        iNum++;
                    }
                }
                if (iNum > 0)
                {
                    _PrepareStringTbl(iNum + 1);
                    ParentRedPointText.text = stringTbl[iNum];

                    ParentRedPoint.gameObject.CustomActive(true);
                }
                else
                {
                    ParentRedPoint.gameObject.CustomActive(false);
                }
            }
        }

        protected void _PrepareStringTbl(int num)
        {
            if(null == stringTbl)
            {
                stringTbl = new string[num];
                for(int i = 0,icnt = stringTbl.Length;i<icnt;++i)
                    stringTbl[i] = i.ToString();

                return;
            }

            if(stringTbl.Length < num)
            {
                string[] newTbl = new string[num];
                for (int i = 0, icnt = stringTbl.Length; i < icnt; ++i)
                    newTbl[i] = stringTbl[i];

                for (int i = stringTbl.Length, icnt = newTbl.Length; i < icnt; ++i)
                    newTbl[i] = i.ToString();

                stringTbl = newTbl;
            }
        }
    }

}