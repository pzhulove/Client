using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tenmove.Runtime.Unity
{
    /// <summary>
    /// Renderer和Setting的集合，控制后处理的开启关闭和资源的创建删除
    /// </summary>
    public class PostprocessBundle
    {
        // Profile或者代码控制的开关
        bool m_Active;
        // 设置界面控制的开关
        bool m_ExternalSwitch = true;
        // 默认开关和设置，如果没有通过其他方式开启这个后处理就使用默认的,
        // 比如一个Profile中没有开启xBR，但是xBR的defaultSwitch为True，也会开启xBR
        bool m_DefaultSwitch;
        PostprocessEffectSettings m_DefaultSetting;

        PostprocessEffectSettings m_Settings;
        PostprocessEffectRenderer m_Renderer;

        public PostprocessBundle(PostprocessEffectRenderer postprocessEffectRenderer)
        {
            m_Renderer = postprocessEffectRenderer;
        }

        public void SetDefaultSetting(bool defaultSwitch, PostprocessEffectSettings defaultSetting = null)
        {
            m_DefaultSwitch = defaultSwitch;
            m_DefaultSetting = defaultSetting;
        }

        public void SetActive(bool active)
        {
            m_Active = active;
        }
        public void SetExternalSwitch(bool externalSwitch)
        {
            m_ExternalSwitch = externalSwitch;
        }
        public bool GetExternalSwitch()
        {
            return m_ExternalSwitch;
        }

        public bool CanInit()
        {
            return m_Renderer.CheckQualityAndSupport() && m_ExternalSwitch && (m_Active || m_DefaultSwitch);
        }

        public bool CanRendering()
        {
            return m_Renderer.CanRendering();
        }

        public void Init(PostprocessLayer layer)
        {
            m_Renderer.Init(layer);
        }

        public void Release()
        {
            m_Renderer.Release();
        }

        public void SetOverrideSettings(PostprocessEffectSettings settings)
        {
            m_Settings = settings;
        }

        public void Render(CommandBuffer cmd, bool renderToScreen = false)
        {
            if (m_Settings == null)
                m_Renderer.SetSettings(m_DefaultSetting);
            else
                m_Renderer.SetSettings(m_Settings);
            m_Renderer.Render(cmd, renderToScreen);
        }

        public PostprocessEffectRenderer GetRenderer()
        {
            return m_Renderer;
        }

        public T CastRenderer<T>() where T : PostprocessEffectRenderer
        {
            return (T)m_Renderer;
        }
    }
}
