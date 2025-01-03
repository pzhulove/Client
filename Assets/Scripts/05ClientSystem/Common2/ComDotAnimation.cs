using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
    public class ComDotAnimation : MonoBehaviour
    {
        public AnimationCurve curve;
        public int frequencySession;
        public int MaxFrame;

        int frameInterval = 1;
        int iIndex = 0;
        int frameTotal = 0;
        int curFrequency = 0;
        public List<GameObject> children = null;
        GameObject goCurrent = null;
        bool bStart = false;

        // Use this for initialization
        void Start()
        {
            Play();
        }

        public void Play()
        {
            iIndex = 0;
            frameTotal = 0;
            MaxFrame = IntMath.Max(MaxFrame, 1);
            curFrequency = 0;
            frameInterval = _GetFrameInterval();
            bStart = true;
            goCurrent.CustomActive(false);
            goCurrent = null;
        }

        int _GetFrameInterval()
        {
            float evalute = curve.Evaluate(curFrequency * 1.0f / frequencySession);
            frameInterval = Mathf.FloorToInt(evalute * MaxFrame);
            frameInterval = IntMath.Max(frameInterval, 1);
            //Logger.LogErrorFormat("frameInterval = {0} curFrequency = {1} frequencySession = {2} evalute = {3}", frameInterval, curFrequency, frequencySession, evalute);
            return frameInterval;
        }

        // Update is called once per frame
        void Update()
        {
            if(!bStart)
            {
                return;
            }

            if(frameTotal == 0)
            {
                _PlayNext();

                curFrequency += 1;
                frameInterval = _GetFrameInterval();

                if (curFrequency >= frequencySession)
                {
                    bStart = false;
                }
            }
            frameTotal = (frameTotal + 1) % frameInterval;

        }

        void _PlayNext()
        {
            if (children == null)
            {
                return;
            }

            goCurrent.CustomActive(false);
            goCurrent = children[iIndex];
            goCurrent.CustomActive(true);
            iIndex = (1 + iIndex) % children.Count;
        }
    }
}