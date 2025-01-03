using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class LegendTabData
    {
        public ProtoTable.LegendMainTable mainItem;
    }

    class LegendTab : CachedSelectedObject<LegendTabData,LegendTab>
    {
        Image BG;
        Image Title;
        GameObject goCheckMark;
        Vector3 hideScale = new Vector3(1.03f, 1.03f, 1.0f);
        Text hint;
        StateController comStateController;

        public override void Initialize()
        {
            BG = Utility.FindComponent<Image>(goLocal, "GrayParent/Image");
            Title = Utility.FindComponent<Image>(goLocal, "GrayParent/Title");
            goCheckMark = Utility.FindChild(goLocal, "GrayParent/CheckMark");
            hint = Utility.FindComponent<Text>(goLocal, "Hint");
            comStateController = goLocal.GetComponent<StateController>();
        }

        public override void UnInitialize()
        {

        }

        public override void OnUpdate()
        {
            if (Value != null && Value.mainItem != null)
            {
                // BG.sprite = AssetLoader.instance.LoadRes(Value.mainItem.Icons[0], typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref BG, Value.mainItem.Icons[0]);
                // Title.sprite = AssetLoader.instance.LoadRes(Value.mainItem.Icons[1], typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref Title, Value.mainItem.Icons[1]);
                Title.SetNativeSize();
                hint.text = TR.Value("legend_series_open_desc", Value.mainItem.UnLockLevel);
                comStateController.Key = "normal";
                if (null != comStateController)
                {
                    int status = Utility.GetLegendMainStatus(Value.mainItem);
                    switch(status)
                    {
                        case 0:
                            {
                                comStateController.Key = "normal";
                            }
                            break;
                        case 1:
                            {
                                comStateController.Key = "locked";
                            }
                            break;
                        case 2:
                            {
                                comStateController.Key = "finished";
                            }
                            break;
                    }
                }
            }
        }

        public override void OnDisplayChanged(bool bShow)
        {
            goCheckMark.CustomActive(bShow);

            if(!bShow)
            {
                goLocal.transform.localScale = Vector3.one;
            }
            else
            {
                goLocal.transform.localScale = hideScale;
            }
        }
    }
}