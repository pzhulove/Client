using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;
using Network;
using System.Net;
using System.IO;
using System.Text;
using XUPorterJSON;
using Scripts.UI;

namespace GameClient
{
    public class RealNameRegContentFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Login/Publish/RealNameReg";
        }

#region ExtraUIBind
        // private Button mClose = null;
        private Button mAuth = null;

        private InputField mUserName = null;
        private InputField mIDS = null;

        protected override void _bindExUI()
        {
            // mClose = mBind.GetCom<Button>("close");
            // mClose.onClick.AddListener(_onCloseButtonClick);
            mAuth = mBind.GetCom<Button>("Auth");
            mAuth.onClick.AddListener(_onAuthButtonClick);
            mUserName = mBind.GetCom<InputField>("UserName");
            mIDS = mBind.GetCom<InputField>("IDS"); 
        }

        protected override void _unbindExUI()
        {
            // mClose.onClick.RemoveListener(_onCloseButtonClick);
            // mClose = null;
            mAuth.onClick.RemoveListener(_onAuthButtonClick);
            mAuth = null;
            mUserName = null;
            mIDS = null;
        }
       
#endregion    

#region Callback
        // private void _onCloseButtonClick()
        // {
        //     /* put your code in here */
        //     saveInfo2Local();
        // }


        private void _onAuthButtonClick()
        {
            /* put your code in here */
            string userName = mUserName.text.Trim();
            string ids = mIDS.text.Trim();
            if(userName.Equals(""))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请输入姓名!");
                return;
            }

            if(ids.Equals(""))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请输入身份证号!");
                return;
            }
            
            GameFrameWork.instance.StartCoroutine(_antiAddicition(userName, ids));
        }

#endregion

        private void saveInfo2Local()
        {
            PlayerLocalSetting.SetValue("RealNameReg", "true");     
            PlayerLocalSetting.SaveConfig();
            ClientSystemManager.instance.CloseFrame(this);
        }

        IEnumerator _antiAddicition(string username, string ids) {
            
            BaseWaitHttpRequest antiAddicitionWt = new BaseWaitHttpRequest();

            antiAddicitionWt.url = Global.ANTI_ADDICITION_ADDRESS + "?username=" + username.Trim() + "&ids=" + ids.Trim();

            yield return antiAddicitionWt;

            if (BaseWaitHttpRequest.eState.Success == antiAddicitionWt.GetResult())
            {
                string resText = antiAddicitionWt.GetResultString();
                if("true" == resText)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("实名认证成功!");
                    saveInfo2Local();
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("实名认证信息有误，请输入正确的实名信息!");
                }
                yield break;
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("实名认证信息有误，请输入正确的实名信息!");
                yield break;
            }
        }
    }
}
