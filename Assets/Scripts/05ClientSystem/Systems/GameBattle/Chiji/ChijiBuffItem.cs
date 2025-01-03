using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChijiBuffItem : MonoBehaviour
    {
        [SerializeField]private Image mBuffIcon;
        [SerializeField]private Text mBuffTime;
        [SerializeField]private Button mTipsBtn;

        private float timer = 0;
        private int time = 0;
        private bool isUpdate = false;
        private BeFightBuff mBeFightBuff;
        public void OnItemVisiable(BeFightBuff beFightBuff)
        {
            if (beFightBuff == null)
            {
                return;
            }

            mBeFightBuff = beFightBuff;
            isUpdate = true;
            time = (int)mBeFightBuff.LeftTime;
            SetBuffIcon();
            UpdateTime(time);

            if (mTipsBtn != null)
            {
                mTipsBtn.onClick.RemoveAllListeners();
                mTipsBtn.onClick.AddListener(OnTipsButtonClick);
            }
        }

        private void OnTipsButtonClick()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChijiBuffTipsFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChijiBuffTipsFrame>();

                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<ChijiBuffTipsFrame>(FrameLayer.Middle,mBeFightBuff.BuffID);
        }

        private void SetBuffIcon()
        {
            if (mBuffIcon != null)
            {
                if(mBeFightBuff != null)
                {
                    if(mBeFightBuff.Table != null)
                    {
                        if(mBeFightBuff.Table.Icon != null)
                        {
                            if(mBeFightBuff.Table.Icon != "" && mBeFightBuff.Table.Icon != "-")
                            {
                                ETCImageLoader.LoadSprite(ref mBuffIcon, mBeFightBuff.Table.Icon);
                            }
                            else
                            {
                                Logger.LogErrorFormat("吃鸡buff icon显示报错,Icon路径错误,buff id = {0}", mBeFightBuff.BuffID);
                            }
                        }
                        else
                        {
                            Logger.LogErrorFormat("吃鸡buff icon显示报错,Icon = null, buff id = {0}", mBeFightBuff.BuffID);
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("吃鸡buff icon显示报错,mBeFightBuff.Table == null, buff id = {0}", mBeFightBuff.BuffID);
                    }
                }
                else
                {
                    Logger.LogErrorFormat("吃鸡buff icon显示报错,mBeFightBuff == null");
                }
            }
        }

        private void UpdateTime(int time)
        {
            if (mBuffTime != null)
            {
                mBuffTime.text = string.Format("{0}秒", time);
            }
        }
        
        private void Update()
        {
            if (mBeFightBuff != null)
            {
                if (time > 0)
                {
                    isUpdate = true;
                }

                if (isUpdate)
                {
                    timer += Time.deltaTime;
                    if (timer >= 1.0f)
                    {
                        time -=  1;
                        UpdateTime(time);
                        timer = 0.0f;
                    }

                    if (time <= 0)
                    {
                        isUpdate = false;
                    }
                }
            } 
        }

        private void OnDestroy()
        {
            time = 0;
            timer = 0;
            isUpdate = false;
            mBeFightBuff = null;
        }
    }
}

