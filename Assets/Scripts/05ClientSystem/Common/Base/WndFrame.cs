using System;
using System.Collections;
using System.Collections.Generic;
///////É¾³ýlinq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.Events;
using ProtoTable;

namespace GameClient
{
    public class WndFrame : ClientFrame
    {
        class CoinType
        {
            public ComCommonConsume.eType type;
            public UInt32 value;
        }

        #region ExtraUIBind
        private ComCommonConsume mCoin1 = null;
        private ComCommonConsume mCoin2 = null;
        private ComCommonConsume mCoin3 = null;
        private ComCommonConsume mCoin4 = null;
        private Button mHelp = null;
        private Button mClose = null;
        private Text mName = null;
        private RectTransform mContent = null;

        protected override void _bindExUI()
        {
            mCoin1 = mBind.GetCom<ComCommonConsume>("coin1");
            mCoin2 = mBind.GetCom<ComCommonConsume>("coin2");
            mCoin3 = mBind.GetCom<ComCommonConsume>("coin3");
            mCoin4 = mBind.GetCom<ComCommonConsume>("coin4");
            mHelp = mBind.GetCom<Button>("help");
            mClose = mBind.GetCom<Button>("close");
            mName = mBind.GetCom<Text>("name");
            mContent = mBind.GetCom<RectTransform>("content");
        }

        protected override void _unbindExUI()
        {
            mCoin1 = null;
            mCoin2 = null;
            mCoin3 = null;
            mCoin4 = null;
            mHelp = null;
            mClose = null;
            mName = null;
            mContent = null;
        }
        #endregion

        
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/ComWnd";
        }

        public virtual string GetContentPath()
        {
            return "";
        }

        public virtual string GetSubFrameName()
        {
            return "";
        }


        public virtual string GetTitle()
        {
            return "";
        }

        public string GetBindPath(string path)
        {
            return string.Format("Content/{0}(Clone)/{1}", GetSubFrameName(), path);
        }

        public void SetCoinType(byte idx, ComCommonConsume.eType type, ComCommonConsume.eCountType countType, int itemId)
        {
            switch(idx)
            {
                case 0:
                    {
                        mCoin1.SetData(type, countType, itemId);
                        break;
                    }
                case 1:
                    {
                        mCoin2.SetData(type, countType, itemId);
                        break;
                    }
                case 2:
                    {
                        mCoin3.SetData(type, countType, itemId);
                        break;
                    }
                case 3:
                    {
                        mCoin4.SetData(type, countType, itemId);
                        break;
                    }
                default:
                    break;
            }
        }

        protected override bool AttachContent()
        {
            content = AssetLoader.instance.LoadResAsGameObject(GetContentPath());
            content.transform.SetParent(mContent, false);

            _InitTitle();

            return true;
        }

        protected void _InitTitle()
        {
            mName.text = GetTitle();

        }
        
    }
}
