using Network;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SettingNoteNameFrame:ClientFrame
    {
        RelationData relationData = null;
        int mNoteNameMaxCount = 8;
        #region ExtraUIBind
        private Button mOk = null;
        private Text mCount = null;
        private InputField mInputField = null;
        private Button mClose = null;

        protected sealed override void _bindExUI()
        {
            mOk = mBind.GetCom<Button>("Ok");
            if (null != mOk)
            {
                mOk.onClick.AddListener(_onOkButtonClick);
            }
            mCount = mBind.GetCom<Text>("Count");
            mInputField = mBind.GetCom<InputField>("InputField");
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected sealed override void _unbindExUI()
        {
            if (null != mOk)
            {
                mOk.onClick.RemoveListener(_onOkButtonClick);
            }
            mOk = null;
            mCount = null;
            mInputField = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onOkButtonClick()
        {
            if (relationData != null)
            {
                //如果备注名称不为空
                if (relationData.remark != null && relationData.remark != "")
                {
                    if (mInputField.text != relationData.remark)
                    {
                        SendWorldSetPlayerRemarkReq();
                    }
                }
                else
                {
                    if (mInputField.text != relationData.name)
                    {
                        SendWorldSetPlayerRemarkReq();
                    }
                }
               
            }

            _onCloseButtonClick();
        }

        void SendWorldSetPlayerRemarkReq()
        {
            WorldSetPlayerRemarkReq req = new WorldSetPlayerRemarkReq();
            req.roleId = relationData.uid;
            req.remark = mInputField.text;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RelationFrame/SettingNoteName";
        }

        protected sealed override void _OnOpenFrame()
        {
            relationData = userData as RelationData;

            int.TryParse(TR.Value("m_notename_maxcount"), out mNoteNameMaxCount);

            if (relationData.remark != null && relationData.remark != "")
            {
                mInputField.text = relationData.remark;
            }
            else
            {
                mInputField.text = relationData.name;
            }

            mCount.text = string.Format("{0}/{1}", mInputField.text.Length, mNoteNameMaxCount);

            //mCount.text = string.Format("{0}/{1}", 0, mNoteNameMaxCount);
            mInputField.onValueChanged.AddListener((string a_strValue) =>
            {
                mCount.text = string.Format("{0}/{1}", a_strValue.Length, mNoteNameMaxCount);
            });
        }

        protected sealed override void _OnCloseFrame()
        {
            
        }
    }
}

