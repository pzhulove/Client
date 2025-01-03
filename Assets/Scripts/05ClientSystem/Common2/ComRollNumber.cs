using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ComRollNumber : MonoBehaviour
    {
        public ComScrollNumber[] scrollItems = new ComScrollNumber[0];
        bool bRoll = false;
        bool bDirty = false;
        int iValue = 0;
        int iTargetValue = 0;
        public int iEditorValue = 0;
        public void SetEditorValue()
        {
            RollValue = iEditorValue;
        }
        public int RollValue
        {
            get
            {
                return iValue;
            }
            set
            {
                if(bRoll)
                {
                    iTargetValue = value;
                    bDirty = true;
                }
                else
                {
                    int iOrgV = iValue;
                    iValue = value;
                    bDirty = false;
                    setValues(ref orgValues, iOrgV);
                    setValues(ref curValues, iValue);
                    StartRoll();
                }
            }
        }
        int[] orgValues = new int[0];
        int[] curValues = new int[0];
        void setValues(ref int[] Values,int iValue)
        {
            if (Values.Length < scrollItems.Length)
            {
                Values = new int[scrollItems.Length];
            }
            for (int i = 0; i < Values.Length; ++i)
            {
                Values[i] = 0;
            }

            for (int i = Values.Length - 1; i >= 0 && iValue > 0; --i)
            {
                Values[Values.Length - 1 - i] = iValue % 10;
                iValue /= 10;
            }
        }
        int refCount = 0;
        public void StartRoll()
        {
            if(bRoll)
            {
                return;
            }
            bRoll = true;
            refCount = scrollItems.Length;
            for(int i = 0; i < scrollItems.Length; ++i)
            {
                scrollItems[i].Run(orgValues[i], curValues[i]);
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        void Awake()
        {
            for (int i = 0; i < scrollItems.Length; ++i)
            {
                scrollItems[i].onActionEnd.AddListener(_OnActionEnd);
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < scrollItems.Length; ++i)
            {
                if(null != scrollItems[i])
                {
                    scrollItems[i].onActionEnd.RemoveListener(_OnActionEnd);
                }
            }
        }

        void _OnActionEnd()
        {
            refCount -= 1;
            if (refCount == 0)
            {
                bRoll = false;
                if(bDirty)
                {
                    bDirty = false;
                    RollValue = iTargetValue;
                }
            }
        }
    }
}