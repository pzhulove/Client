using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ScriptBinderTestFrame : ClientFrame
    {
        protected override void _OnOpenFrame()
        {
            _InitScriptBinder();
        }

        protected override void _OnCloseFrame()
        {

        }

        public static void CommandOpen(object argv = null)
        {
            ClientSystemManager.GetInstance().CloseFrame<ScriptBinderTestFrame>();
            ClientSystemManager.GetInstance().OpenFrame<ScriptBinderTestFrame>(FrameLayer.Middle,argv);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FunctionFrame/FunctionFrame";
        }

        [UIControl("",typeof(ComScriptBinder))]
        ComScriptBinder mScriptBinder = null;

        void _InitScriptBinder()
        {
            if (null != mScriptBinder)
            {
                var linkParse = mScriptBinder.GetScript<LinkParse>((int)ComScriptLabel.Label_FunctionFrame_Button_Link);
                if(null != linkParse)
                {
                    linkParse.SetText(@"I am a cao ni ma !!!");
                }
                mScriptBinder.SetText((int)ComScriptLabel.Label_FunctionFrame_Button_Merge, @"what a fucking day !!!");
                mScriptBinder.SetImage((int)ComScriptLabel.Label_FunctionFrame_Button_Fuck,0);
            }
        }
    }
}