using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

//|---------------------------------------------------|
//|this code is created by generator do not edit it!!!|
//|---------------------------------------------------|

namespace GameClient
{
    class TestFrame : ClientFrame
    {
        ///<summary>
        ///module tag = SSJ
        ///</summary>
        GameObject itemRoot;
        Text name;
        ///<summary>
        ///module tag = WF
        ///</summary>
        Button btnFuck;
        public void Create(GameObject frame)
        {
            itemRoot = Utility.FindChild(frame, "ItemParent");
            name = Utility.FindComponent<Text>(frame, "ItemParent/Prefabs/Name");
            btnFuck = Utility.FindComponent<Button>(frame, "ItemParent/Prefabs/Button");
        }
        public void Destroy()
        {
            itemRoot = null;
            name = null;
            btnFuck.onClick.RemoveAllListeners();
            btnFuck = null;
        }
        public override string GetPrefabPath()
        {
            return "UI/Prefabs/ActiveFrame/TestFrame";
        }
        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            Create(frame);
        }
        protected override void _OnCloseFrame()
        {
            Destroy();
            base._OnCloseFrame();
        }
        protected class ItemParent
        {
            public Text name;
            public Button btnClick;
            public Text btnText;
            string strParentPath = "ItemParent";
            string strPrefabPath = "Prefabs";

            public GameObject GetParent(GameObject frame)
            {
                return Utility.FindChild(frame, strParentPath);
            }

            public GameObject GetPrefab(GameObject parent)
            {
                return Utility.FindChild(parent, strPrefabPath);
            }

            public void Create(GameObject goLocal)
            {
                if (goLocal == null)
                {
                    return;
                }

                name = Utility.FindComponent<Text>(goLocal, "Name");
                btnClick = Utility.FindComponent<Button>(goLocal, "Button");
                btnText = Utility.FindComponent<Text>(goLocal, "Button/Text");
            }

            public void Destroy(GameObject goLocal)
            {
                if (goLocal == null)
                {
                    return;
                }

                name = null;
                btnClick.onClick.RemoveAllListeners();
                btnClick = null;
                btnText = null;

                strParentPath = null;
                strPrefabPath = null;
            }
        }
    }
}