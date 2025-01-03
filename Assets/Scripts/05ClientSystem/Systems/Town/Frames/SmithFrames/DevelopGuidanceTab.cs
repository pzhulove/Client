using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using System;

namespace GameClient
{
    class TabData
    {
        public ProtoTable.GuidanceMainTable mainItem;
    }

    class DevelopGuidanceTab : CachedSelectedObject<TabData, DevelopGuidanceTab>
    {
        Text Label;
        Text CheckLabel;
        GameObject goCheckMark;

        public override void Initialize()
        {
            Label = Utility.FindComponent<Text>(goLocal,"Label");
            CheckLabel = Utility.FindComponent<Text>(goLocal, "CheckMark/Label");
            goCheckMark = Utility.FindChild(goLocal, "CheckMark");
        }

        public override void UnInitialize()
        {

        }

        public override void OnUpdate()
        {
            if(Value != null && Value.mainItem != null)
            {
                Label.text = CheckLabel.text = Value.mainItem.Desc;
            }
        }

        public override void OnDisplayChanged(bool bShow)
        {
            goCheckMark.CustomActive(bShow);
        }
    }
}