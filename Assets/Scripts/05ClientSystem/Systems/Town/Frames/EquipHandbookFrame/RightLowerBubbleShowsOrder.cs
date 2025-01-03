using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
   
    public class RightLowerBubbleShowsOrder : MonoBehaviour
    {
       
        float timer = 0f;

        bool mIsPlayAnimtion = true;

        bool mIsUpdata = false;

        List<BubbleShowType> mBubbleType = new List<BubbleShowType>();
      
        void OnDestroy()
        {
            timer = 0;
            mBubbleType.Clear();
            mIsUpdata = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RightLowerBubbleStopAnimation, mRightLowerBubbleStopAnimation);
        }

        void Start()
        {
            mIsPlayAnimtion = true;
            mIsUpdata = false;
            timer = 0;
            mBubbleType.Clear();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RightLowerBubbleStopAnimation, mRightLowerBubbleStopAnimation);
        }

        void mRightLowerBubbleStopAnimation(UIEvent iEvent)
        {
            BubbleShowType type = (BubbleShowType)iEvent.Param1;

            if (mBubbleType.Contains(type))
            {
                mBubbleType.Remove(type);
                timer = 0;
                mIsPlayAnimtion = true;
            }
        }

        public void AddAnimation(BubbleShowType type)
        {
            if(!mBubbleType.Contains(type))
            {
                mBubbleType.Add(type);
                mIsUpdata = true;
                mIsPlayAnimtion = true;
                timer = 0;
            }
        }
        
        public void Update()
        {
            if (mIsUpdata)
            {
                timer += Time.deltaTime;

                if (timer > 0.2f)
                {
                    if (mIsPlayAnimtion)
                    {
                        if (mBubbleType.Count != 0)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RightLowerBubblePlayAnimation, mBubbleType[0]);
                            mIsPlayAnimtion = false;
                        }
                        else
                        {
                            mIsPlayAnimtion = false;
                        }
                    }

                    if (timer > 10f)
                    {
                        if (mBubbleType.Count != 0)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RightLowerBubbleStopAnimation, mBubbleType[0]);
                        }
                        else
                        {
                            mIsUpdata = false;
                        }
                    }

                }
            }
        }

    }
}
