using System;
using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime.Client;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tenmove.Runtime.Unity
{
    [Serializable]
    public abstract class PostprocessEffectSettings : ScriptableObject
    {
        public abstract PostProcessType EffectType { get; }
    }

    public abstract class PostprocessEffectRenderer
    {
        /// <summary>
        /// 如果一个效果计算的结果不会输出到当前的TargetRT上，那这个值就为false，否则为true
        /// </summary>
        public abstract bool NeedSwapRT { get; }
        /// <summary>
        /// 是否需要FinalPass
        /// </summary>
        public abstract bool NeedFinalPass { get; }
        /// <summary>
        /// Blit到PostprocessLayer的RT上的次数
        /// </summary>
        public abstract int BlitToLayerTimes { get; }

        /// <summary>
        /// 能不能直接从RT Blit到屏幕，RT大小可能和屏幕大小不一致，所以xBR不能Blit到屏幕，要先Blit到另一个RT再Blit到屏幕，
        /// </summary>
        public abstract bool CanBlitToScreen { get; }

        protected PostprocessLayer layer;


        bool inited;
        public void Init(PostprocessLayer layer)
        {
            if (inited)
                return;

            inited = true;
            this.layer = layer;
            OnInit();
        }

        public void Release()
        {
            if (!inited)
                return;

            inited = false;
            OnRelease();
        }

        protected virtual void OnInit() { }

        protected virtual void OnRelease() { }

        public abstract void Render(CommandBuffer cmd, bool renderToScreen = false);

        public virtual void OnPreRender() { }

        public virtual void OnPreNotRender() { }

        public virtual void SetSettings(PostprocessEffectSettings settings) { }

        public abstract bool CheckQualityAndSupport();

        public virtual bool CanRendering() { return true; }
    }

    public abstract class PostprocessEffectRenderer<T> : PostprocessEffectRenderer
            where T : PostprocessEffectSettings
    {
        protected T m_Settings;
        protected T m_DefaultSettings;

        public override void SetSettings(PostprocessEffectSettings _settings)
        {
            m_Settings = (T)_settings;
        }

        public T GetSettings()
        {
            return m_Settings;
        }

        protected T GetDefaultSettings()
        {
            if (m_DefaultSettings == null)
                m_DefaultSettings = ScriptableObject.CreateInstance<T>();
            return m_DefaultSettings;
        }

        protected override void OnRelease()
        {
            m_Settings = null;
        }
    }
}
