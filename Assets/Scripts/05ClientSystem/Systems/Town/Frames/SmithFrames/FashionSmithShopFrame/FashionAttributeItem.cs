using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class FashionAttributeItem : CachedSelectedObject<FashionAttributeItemData, FashionAttributeItem>
    {
        Text name;
        GameObject goGraphic;
        GameObject goCheckMark;
        GameObject goCurAttribute;
        GameObject goLines;

        public bool IsAssigned
        {
            get
            {
                return Value.item == Value.selected;
            }
        }

        public override void Initialize()
        {
            name = Utility.FindComponent<Text>(goLocal, "Name");
            goGraphic = Utility.FindChild(goLocal, "Mark");
            goCheckMark = Utility.FindChild(goLocal, "CheckMark");
            goCurAttribute = Utility.FindChild(goLocal, "CurAttribute");
            goLines = Utility.FindChild(goLocal, "Lines");
        }

        public override void UnInitialize()
        {

        }

        public override void OnUpdate()
        {
            if (true == IsAssigned)
            {
                goGraphic.CustomActive(false);
                goCurAttribute.CustomActive(true);
            }
            else
            {
                goGraphic.CustomActive(true);
                goCurAttribute.CustomActive(false);
            }

            name.text = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(Value.item.ID,"fashion_attribute_color_white"," ");
            goLines.CustomActive(!Value.bLast);
        }

        public override void OnDisplayChanged(bool bShow)
        {
            goCheckMark.CustomActive(bShow);
        }
    }
}