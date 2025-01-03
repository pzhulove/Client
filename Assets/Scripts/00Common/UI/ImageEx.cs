using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    /// <summary>
    /// Image is a textured element in the UI hierarchy.
    /// </summary>

    [AddComponentMenu("UI/ImageEx", 11)]
    public class ImageEx : Image
    {

        [FormerlySerializedAs("mIsEnableWhiteImage")]
        [SerializeField]
        private bool m_IsEnableWhiteImage;

        public bool IsEnableWhiteImage
        {
            get
            {
                return m_IsEnableWhiteImage;
            }
            set
            {
                m_IsEnableWhiteImage = value;
            }
        }
        #region Atlas
        /*
        [SerializeField]
        public string m_SpriteName;
        public string OriginSpriteName()
        {
            if (string.IsNullOrEmpty(m_SpriteName))
                return "";

            return m_SpriteName.Substring(m_SpriteName.IndexOf(':') + 1);
        }

        public string OriginTexName()
        {
            if (string.IsNullOrEmpty(m_SpriteName))
                return "";

            return m_SpriteName.Substring(0, m_SpriteName.IndexOf(':'));
        }

        /// <summary>
        /// 序列化之前，将sprite置为空，避免打AB时引入散图。但该函数在编辑器中选中该控件时
        /// 会一直调用，为了编辑器显示正确，将sprite设置给overrideSprite。
        /// </summary>
        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            if(sprite != null && !string.IsNullOrEmpty(m_SpriteName))
            {
                overrideSprite = sprite;
                sprite = null;
            }
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
        }        */
#if UNITY_EDITOR
        public ImageEx()
        {
            raycastTarget = false;
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            //raycastTarget = false;

//             // 运行状态下，而是是Mobile或者设置了loadUIFromAtlas才替换
//             //if (UnityEngine.Application.isPlaying && (UnityEngine.Application.isMobilePlatform || Global.Settings.loadUIFromAtlas))
//             if (!string.IsNullOrEmpty(m_SpriteName))
//             {
//                 // 加载合图
//                 if((UnityEngine.Application.isMobilePlatform || (Global.Settings.loadUIFromAtlas && UnityEngine.Application.isPlaying)))
//                 {
//                     //sprite = AssetLoader.LoadSprite("XXFSprite/sprite/monster/goblin/event/goblin.img/0");
// 
//                     // 判断该图是否在合图中，在就从合图中加载，否则加载散图。
// 
//                     sprite = AssetLoader.LoadSprite(OriginTexName());
//                 }
//                 else
//                 {
//                     sprite = AssetLoader.LoadSprite(OriginTexName());
//                 }
//             }
        }
        #endregion

        /// <summary>
        /// 重载OnPopulateMesh，支持在SpriteAtlas还未异步加载完成前隐藏Image
        /// </summary>
        /// <param name="toFill"></param>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if ((overrideSprite != null && overrideSprite.texture == null)
                || (overrideSprite == null && !m_IsEnableWhiteImage && color == Color.white) || color.a == 0)
            {
                toFill.Clear();
            }
            else
            {
                base.OnPopulateMesh(toFill);
            }
        }
    }
}
