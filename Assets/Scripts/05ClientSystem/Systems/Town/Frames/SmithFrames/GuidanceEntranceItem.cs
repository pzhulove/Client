using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;

namespace GameClient
{
    class GuidanceEntrance : CachedNormalObject<EntranceData>
    {
        ComGuidanceItem comGuidanceItem;
        ClientFrameBinder frameBinder;
        public override void Initialize()
        {
            frameBinder = goLocal.GetComponentInParent<ClientFrameBinder>();
            comGuidanceItem = goLocal.GetComponent<ComGuidanceItem>();
            comGuidanceItem.button.onClick.RemoveListener(OnClickItem);
            comGuidanceItem.button.onClick.AddListener(OnClickItem);
        }

        public override void UnInitialize()
        {
            if (comGuidanceItem.button != null)
            {
                comGuidanceItem.button.onClick.RemoveListener(OnClickItem);
            }
        }

        void OnClickItem()
        {
            if(Value != null && Value.entranceItem != null &&
                !string.IsNullOrEmpty(Value.entranceItem.LinkInfo))
            {
                if(frameBinder != null)
                {
                    frameBinder.CloseFrame();
                }
                ActiveManager.GetInstance().OnClickLinkInfo(Value.entranceItem.LinkInfo);
            }
        }

        public override void OnUpdate()
        {
            if(Value != null)
            {
                comGuidanceItem.Name.text = Value.entranceItem.Name;
                // comGuidanceItem.Icon.sprite = AssetLoader.instance.LoadRes(Value.entranceItem.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref comGuidanceItem.Icon, Value.entranceItem.Icon);
            }
        }
    }
}