using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace GameClient
{
    class NpcInteractionItem : CachedObject
    {
        protected GameObject goLocal;
        public GameObject goPrefab;
        public GameObject goParent;
        public NpcInteractionData data;
        public ClientFrame frame;

        Image image;
        Button btnClick;

        Image icon = null;
        Text type = null;

        public override void OnCreate(object[] param)
        {
            goParent = param[0] as GameObject;
            goPrefab = param[1] as GameObject;
            data = param[2] as NpcInteractionData;
            frame = param[3] as ClientFrame;

            if (goLocal == null)
            {
                goLocal = GameObject.Instantiate(goPrefab) as GameObject;
                Utility.AttachTo(goLocal, goParent);
                image = goLocal.GetComponent<Image>();
                btnClick = goLocal.GetComponent<Button>();
                btnClick.onClick.RemoveAllListeners();
                btnClick.onClick.AddListener(() =>
                {
                    if (null != data.onClickFunction)
                    {
                        data.onClickFunction.Invoke();
                        //if(frame != null)
                        //{
                        //    frame.Close(true);
                        //}
                    }
                });

                var bind = goLocal.GetComponent<ComCommonBind>();
                if(bind != null)
                {
                    icon = bind.GetCom<Image>("icon");
                    type = bind.GetCom<Text>("type");
                }
            }
            Enable();
            SetAsLastSibling();
            OnUpdate();
        }

        public override void OnDestroy()
        {
            goLocal = null;
            data = null;
            goPrefab = null;
            goParent = null;
            image = null;
            if(btnClick != null)
            {
                btnClick.onClick.RemoveAllListeners();
                btnClick = null;
            }
            frame = null;
        }

        public override void Enable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(true);
            }
        }

        public override void Disable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(false);
            }
        }

        public override void SetAsLastSibling()
        {
            if (goLocal != null)
            {
                goLocal.transform.SetAsLastSibling();
            }
        }

        public override void OnRefresh(object[] param)
        {
            if (param != null && param.Length > 0)
            {
                data = param[0] as NpcInteractionData;
            }
            OnUpdate();
        }

        public override void OnRecycle()
        {
            Disable();
        }

        public override void OnDecycle(object[] param)
        {
            OnCreate(param);
        }

        string GetInteractionName()
        {
            if(data == null)
            {
                return "";
            }

            switch(data.eNpcInteractionType)
            {
                case NpcInteractionType.NIT_DIALOG:
                    return TR.Value("npc_interaction_dialog");              
                case NpcInteractionType.NIT_FUNCTION:
                    return TR.Value("npc_interaction_shop");             
                case NpcInteractionType.NIT_MISSION:
                    return TR.Value("npc_interaction_task");              
                case NpcInteractionType.NIT_Attack_City_Monster:
                    return TR.Value("npc_interaction_attack_city");         
            }

            return "";
        }

        void OnUpdate()
        {
            // image.sprite = AssetLoader.instance.LoadRes(data.icon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref image, data.icon);
            icon.SafeSetImage(data.icon,true);
            type.SafeSetText(data.name);
        }
    }
}