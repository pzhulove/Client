using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    [ExecuteAlways]
    public class StateController : MonoBehaviour
    {
        [System.Serializable]
        public class VisibleData
        {
            public bool bVisible;
            public GameObject gameObject;
            public void Apply()
            {
                if (null != gameObject)
                {
                    gameObject.CustomActive(bVisible);
                }
            }
        }

        [System.Serializable]
        public class StateVisible
        {
            public List<VisibleData> elements = new List<VisibleData>();
            public int count;
            public bool expand;
            public void Apply()
            {
                for (int i = 0; i < count && i < elements.Count; ++i)
                {
                    elements[i].Apply();
                }
            }
        }

        [System.Serializable]
        public class ImageSpriteData
        {
            public Sprite imageData;
            public Image image;
            public void Apply()
            {
                if (null != image)
                {
                    image.sprite = imageData;
                }
            }
        }
        [System.Serializable]
        public class StateImageSprite
        {
            public List<ImageSpriteData> elements = new List<ImageSpriteData>();
            public int count;
            public bool expand;
            public void Apply()
            {
                for (int i = 0; i < count && i < elements.Count; ++i)
                {
                    elements[i].Apply();
                }
            }
        }


        [System.Serializable]
        public class GraphicColorData
        {
            public Color color;
            public Graphic graphic;
            public void Apply()
            {
                if (null != graphic)
                {
                    graphic.color = color;
                }
            }
        }
        [System.Serializable]
        public class StateGraphicColor
        {
            public List<GraphicColorData> elements = new List<GraphicColorData>();
            public int count;
            public bool expand;
            public void Apply()
            {
                for (int i = 0; i < count && i < elements.Count; ++i)
                {
                    elements[i].Apply();
                }
            }
        }

        [System.Serializable]
        public class CanvasGroupData
        {
            public float alpha;
            public bool isRaycast;
            public CanvasGroup canvasGroup;
            public void Apply()
            {
                if (null != canvasGroup)
                {
                    canvasGroup.alpha = alpha;
                    canvasGroup.blocksRaycasts = isRaycast;
                }
            }
        }
        [System.Serializable]
        public class StateCanvasGroup
        {
            public List<CanvasGroupData> elements = new List<CanvasGroupData>();
            public int count;
            public bool expand;
            public void Apply()
            {
                for (int i = 0; i < count && i < elements.Count; ++i)
                {
                    elements[i].Apply();
                }
            }
        }

        [System.Serializable]
        public class UIPrefabWrapperData
        {
            public bool isLoad;
            public UIPrefabWrapper uiPrefabWrapper;
            public void Apply()
            {
                if (null != uiPrefabWrapper && isLoad)
                {
                    uiPrefabWrapper.Load();
                }
            }
        }
        [System.Serializable]
        public class StateUIPrefabWrapper
        {
            public List<UIPrefabWrapperData> elements = new List<UIPrefabWrapperData>();
            public int count;
            public bool expand;
            public void Apply()
            {
                for (int i = 0; i < count && i < elements.Count; ++i)
                {
                    elements[i].Apply();
                }
            }
        }

        [System.Serializable]
        public class State
        {
            public StateVisible instanceVisible = new StateVisible();
            public StateImageSprite instanceImageSprite = new StateImageSprite();
            public StateCanvasGroup instanceCanvasGroup = new StateCanvasGroup();
            public StateGraphicColor instanceGraphicColor = new StateGraphicColor();
            public StateUIPrefabWrapper instanceUIPrefabWrapper = new StateUIPrefabWrapper();
            public UnityEvent onEvent;

            public string key;
            public string desc;
            public bool showInEditor = true;
        }

        public List<State> elements = new List<State>();
        public int count;
        public bool expand;

        public string defKey;
        bool bDirty = false;

        void Start()
        {
            //这句代码不要注释，注释掉在手机包会有问题
           _ChangeStatus();
            bDirty = false;
        }

        public void _ChangeStatus()
        {
            for(int i = 0; i < elements.Count && i < count; ++i)
            {
                if(!string.IsNullOrEmpty(defKey) && defKey.Equals(elements[i].key))
                {
                    if(null != elements[i].instanceVisible)
                    {
                        elements[i].instanceVisible.Apply();
                    }
                    if (null != elements[i].instanceImageSprite)
                    {
                        elements[i].instanceImageSprite.Apply();
                    }
                    if (null != elements[i].instanceCanvasGroup)
                    {
                        elements[i].instanceCanvasGroup.Apply();
                    }
                    if (null != elements[i].instanceGraphicColor)
                    {
                        elements[i].instanceGraphicColor.Apply();
                    }
                    if (null != elements[i].instanceUIPrefabWrapper)
                    {
                        elements[i].instanceUIPrefabWrapper.Apply();
                    }

                    if (null != elements[i].onEvent)
                    {
                        elements[i].onEvent.Invoke();
                    }
                }
            }
        }

        public string Key
        {
            get
            {
                return defKey;
            }

            set
            {
                defKey = value;
                bDirty = true;
//#if UNITY_EDITOR
                _ChangeStatus();
                bDirty = false;
//#endif
            }
        }

        void Update()
        {
            if(bDirty)
            {
                _ChangeStatus();
                bDirty = false;
            }
        }
    }
}