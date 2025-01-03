using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SendGiftItem : MonoBehaviour
    {
        [SerializeField]
        private Image mHeadImage;
        [SerializeField]
        private Text mNameTxt;
        [SerializeField]
        private Text mStateTxt;
        [SerializeField]
        private Text mSendMeTxt;
        [SerializeField]
        private Text mMySendTxt;
        [SerializeField]
        private Text mBeSendTxt;
        [SerializeField]
        private Toggle mSelectToggle;
        
        private FriendPresentInfo mData;
        private Action<FriendPresentInfo> mSelectAction = null;
        private void Start()
        {
            mSelectToggle.SafeAddOnValueChangedListener(_OnSelect);
        }
        private void OnDestroy()
        {
            mSelectToggle.SafeRemoveOnValueChangedListener(_OnSelect);
        }


        public void SendData(FriendPresentInfo data,Action<FriendPresentInfo> selectAction,FriendPresentInfo curSeletData)
        {
            mNameTxt.SafeSetText(data.friendname);
            ShowHeadIcon(data);
            mStateTxt.SafeSetText(GetStateDes(data.isOnline));
            mMySendTxt.SafeSetText(string.Format("{0}/{1}",data.sendTimes, data.sendLimit));
            mSendMeTxt.SafeSetText(string.Format("{0}/{1}", data.beSendedTimes, data.beSendedLimit));
            mBeSendTxt.SafeSetText(string.Format("{0}/{1}", data.sendedTotalTimes, data.sendedTotalLimit));
            mData = data;
            mSelectAction = selectAction;
            if (curSeletData != null)
            {
                if (curSeletData.friendId == data.friendId)
                {
                    _SetSelected(true);
                }
                else
                {
                    _SetSelected(false);
                }
            }
        }

        private void ShowHeadIcon(FriendPresentInfo data)
        {
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(data.friendOcc);
            if (null != jobItem)
            {
                ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                if (resData != null)
                {
                    ETCImageLoader.LoadSprite(ref mHeadImage, resData.IconPath);
                    mHeadImage.SetNativeSize();
                }
            }
           
        }
        private void _OnSelect(bool isOn)
        {
           if(isOn)
            {
                if(mSelectAction!=null)
                {
                    _SetAllowSwitchOff(false);
                    mSelectAction(mData);
                }
            }
        }
        private void _SetSelected(bool isOn)
        {
            if(mSelectToggle!=null)
            {
                mSelectToggle.isOn = isOn;
            }
        }

        private void _SetAllowSwitchOff(bool isCan)
        {
            if (mSelectToggle!=null&&mSelectToggle.group != null)
            {
                mSelectToggle.group.allowSwitchOff = isCan;
            }
        }

        public void OnRecycle()
        {
            _SetAllowSwitchOff(true);
            _SetSelected(false);
            _SetAllowSwitchOff(false);
            mData = null;
            mSelectAction = null;
           
        }
        private string GetStateDes(byte state)
        {
            string str = string.Empty;
            if(state==0)//离线
            {
                str = TR.Value("OffOnLine_State");
            }else
            {
                str = TR.Value("OnLine_State");
            }
            return str;
        }
    }
}
