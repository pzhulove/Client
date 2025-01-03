using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    public class ShowArtifactJarDiscountFrame : ClientFrame
    {
        private Image num = null;
        private Button btnExit = null;
        private Text count = null;
        private GameObject EffUI_moguandazhe = null;
        private GameObject EffUI_moguandazhe_chixu = null;


        public const string numberStr = "UI/Image/Packed/p_UI_Moguanpaidui.png:UI_Moguanpaidui_Number_0{0}";

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ArtifactJar/ShowArtifactJarDiscountFrame";
        }      

        protected override void _OnOpenFrame()
        {           
            UpdateDiscount();

            InvokeMethod.Invoke(1.0f, ShowEffect1);
            InvokeMethod.Invoke(2.5f, ShowEffect2);
        }

        protected override void _OnCloseFrame()
        {
            InvokeMethod.RemoveInvokeCall(ShowEffect1);
            InvokeMethod.RemoveInvokeCall(ShowEffect2);
        }

        protected override void _bindExUI()
        {
            num = mBind.GetCom<Image>("num");
            btnExit = mBind.GetCom<Button>("btnExit");
            btnExit.SafeAddOnClickListener(() =>
            {
                frameMgr.CloseFrame(this);

                frameMgr.OpenFrame<ArtifactFrame>(FrameLayer.Middle);
            });

            count = mBind.GetCom<Text>("count");

            EffUI_moguandazhe = mBind.GetGameObject("EffUI_moguandazhe");
            EffUI_moguandazhe_chixu = mBind.GetGameObject("EffUI_moguandazhe_chixu");
        }

        protected override void _unbindExUI()
        {
            num = null;
            btnExit = null;
            EffUI_moguandazhe = null;
            EffUI_moguandazhe_chixu = null;
        }

        private void UpdateDiscount()
        {
            num.SafeSetImage(string.Format(numberStr, ActivityJarFrame.GetArtifactJarDiscountRate()));
            count.SafeSetText(TR.Value("artifact_jar_discount_num", ActivityJarFrame.GetArtifactJarDiscountTimes()));
        }
        
        private void ShowEffect1()
        {
            EffUI_moguandazhe.CustomActive(true);
        }
        private void ShowEffect2()
        {
            EffUI_moguandazhe_chixu.CustomActive(true);
        }
    }
}
