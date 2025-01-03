using UnityEngine;
using System.Collections;

namespace GameClient
{
    [System.Serializable]
    public class ContinueJump
    {
        public AnimationCurve curve;
        public float length = 0.60f;
        public float fMax = 1.0f;
    }

    public class ScalerJump : MonoBehaviour
    {
        public ContinueJump normalJump = new ContinueJump();
        public ContinueJump[] continueJump = new ContinueJump[0];
        public delegate void OnJumpEnd(int iIndex);

        public OnJumpEnd onJumpEnd;
        bool bStart = false;
        bool bContinue = false;
        float timeBeg = 0.0f;
        int iStarIndex = 0;
        int jumpIndex = 0;
        int jumpLength = 1;

        public void StartJumpNormal(int jumpIndex)
        {
            if(bStart)
            {
                return;
            }
            bStart = true;
            bContinue = false;
            timeBeg = Time.time;
            transform.localScale = Vector3.one;
            iStarIndex = 0;
            this.jumpIndex = jumpIndex;
            jumpLength = 1;
        }

        public void StartJumpContinue(int jumpIndex,int jumpLength)
        {
            if (bStart)
            {
                return;
            }
            bStart = true;
            bContinue = true;
            timeBeg = Time.time;
            transform.localScale = Vector3.one;
            iStarIndex = 0;
            this.jumpIndex = jumpIndex;
            this.jumpLength = jumpLength;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!bStart)
            {
                return;
            }

            ContinueJump current = null;
            if (!bContinue)
            {
                current = normalJump;
            }
            else
            {
                if(iStarIndex >= 0 && iStarIndex < continueJump.Length && iStarIndex < jumpLength)
                {
                    current = continueJump[iStarIndex];
                }
            }

            if (null == current || Time.time >= timeBeg + current.length)
            {
                if(current != null && bContinue)
                {
                    timeBeg = Time.time;
                    iStarIndex += 1;
                }
                else
                {
                    transform.localScale = Vector3.one;
                    bStart = false;
                }

                if (null != onJumpEnd && null != current)
                {
                    if(bContinue)
                    {
                        onJumpEnd(jumpIndex + iStarIndex);
                    }
                    else
                    {
                        onJumpEnd(jumpIndex);
                    }
                }
            }
            else
            {
                float value = (Time.time - timeBeg) / current.length;
                value = Mathf.Clamp01(value);
                transform.localScale = current.curve.Evaluate(value) * current.fMax * Vector3.one;
            }
        }
    }
}