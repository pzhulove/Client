using UnityEngine;
using UnityEngine.UI;
using System;

public class CachedObjectBehavior : MonoBehaviour
{
    public enum CreateType
    {
        CT_USE_COPY,
        CT_USE_CURRENT = 1,
        CT_COUNT,
    }
    public CreateType eCreateType = CreateType.CT_USE_COPY;
    public GameObject goParent;
    public GameObject goPrefab;
    public bool IsOpenCreate = true;
    public UIBinder[] uiBinders = null;

    public class UIBinderAttribute :Attribute
    {
        public UIBinderAttribute(Type type)
        {
            this.type = type;
        }
        Type type;

        public Type Type
        {
            get
            {
                return type;
            }
        }
    }

    [Serializable]
    public class UIBinder
    {
        public GameObject goLocal;
        public string varName;
        public enum BinderType
        {
            [UIBinderAttribute(typeof(Text))]
            BT_TEXT = 0,
            [UIBinderAttribute(typeof(Button))]
            BT_BUTTON,
            [UIBinderAttribute(typeof(Toggle))]
            BT_TOGGLE,
            [UIBinderAttribute(typeof(GameClient.NewSuperLinkText))]
            BT_SUPERLINKTEXT,
            [UIBinderAttribute(typeof(Image))]
            BT_IMAGE,
            [UIBinderAttribute(typeof(GameObject))]
            BT_GAMEOBJECT,

            BT_INVALID,
            BT_COUNT = BT_INVALID,
        }
        public BinderType eBinderType = BinderType.BT_INVALID;
    }

    bool bCreate = false;

    public void Create()
    {
        if(!bCreate)
        {
            bCreate = true;
        }
    }
}