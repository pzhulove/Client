using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TM/角色武器强化特效组件")]
public class GeAttachPhaseProxy : MonoBehaviour
{
    // 用来设置粒子特效的Shape，这两个Renderer名字要求包含“Weapon” 
    [SerializeField] private MeshFilter m_WeaponMeshFilter;
    [SerializeField] private MeshRenderer m_WeaponMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer m_WeaponSkinnedMeshRenderer;

    // 根据强化等级设置Renderer的材质，不包括Tag为“EffectModel”的Renderer！！！ 
    [SerializeField] private List<Renderer> m_Renderers = new List<Renderer>();

    // 武器的Glow，需要根据等级改变颜色
    [SerializeField] private List<GameObject> m_EffGlowRoots = new List<GameObject>();
    [SerializeField] private List<Renderer> m_EffGlowRenderers = new List<Renderer>();

    private GePhaseStageEffects m_CurPhaseStageEffects;
    private GePhaseStageDesc m_CurPhaseStageDesc;

    protected class PhaseMatSurfDesc
    {
        public PhaseMatSurfDesc(Material[] origMaterials, Renderer renderer)
        {
            m_Renderer = renderer;
            m_OriginMaterials = origMaterials;
        }

        public Renderer m_Renderer;
        public Material[] m_OriginMaterials;
    }
    private List<PhaseMatSurfDesc> m_PhaseMatSurfDescList = new List<PhaseMatSurfDesc>();

    public void ChangePhase(string phaseEffect, int phaseIdx, bool forceAddtive, int layer)
    {
        if (string.IsNullOrEmpty(phaseEffect) || phaseIdx <= 0)
            return;

        GePhaseStageDesc curPhaseStageDesc = GePhaseEffect.instance.CreatePhaseEffect(phaseEffect, phaseIdx - 1);
        MaterialPropertyBlock block = GePhaseEffect.instance.MatPropertyBlock;

        m_CurPhaseStageDesc = curPhaseStageDesc;

        if (curPhaseStageDesc != null)
        {
            // 设置Glow特效
            if (curPhaseStageDesc.m_Glow)
            {
                if (m_EffGlowRoots != null)
                {
                    for (int i = 0; i < m_EffGlowRoots.Count; ++i)
                    {
                        if (m_EffGlowRoots[i] != null)
                            m_EffGlowRoots[i].SetActive(true);
                    }
                }

                curPhaseStageDesc.m_GlowColor.a = 0.5f;
                if (m_EffGlowRenderers != null)
                {
                    for (int i = 0; i < m_EffGlowRenderers.Count; ++i)
                    {
                        Renderer effGlowRenderer = m_EffGlowRenderers[i];
                        if (effGlowRenderer == null)
                            continue;
                        // 设置Glow特效的Layer 
                        effGlowRenderer.gameObject.layer = layer;
                        // 设置Glow特效的颜色
                        if (block != null)
                        {
                            effGlowRenderer.GetPropertyBlock(block);
                            block.SetColor("_TintColor", curPhaseStageDesc.m_GlowColor);
                            effGlowRenderer.SetPropertyBlock(block);
                        }
                    }
                }

            }
            else
            {
                if (m_EffGlowRoots != null)
                {
                    for (int i = 0; i < m_EffGlowRoots.Count; ++i)
                    {
                        if (m_EffGlowRoots[i] != null)
                            m_EffGlowRoots[i].SetActive(false);
                    }
                }
            }


            // 创建这个强化等级配置的特效
            GePhaseStageEffects phaseStageEffects = curPhaseStageDesc.GetEffects();
            m_CurPhaseStageEffects = phaseStageEffects;

            if (phaseStageEffects != null)
            {
                phaseStageEffects.m_EffectRoot.transform.SetParent(transform, false);

                // 设置特效Renderer的Layer
                if(phaseStageEffects.m_Renderers != null)
                {
                    for(int i = 0;i < phaseStageEffects.m_Renderers.Length;++i)
                    {
                        phaseStageEffects.m_Renderers[i].gameObject.layer = layer;
                    }
                }

                // 设置特效ParticleSystem的Shape
                if(phaseStageEffects.m_Particles != null)
                {
                    for(int i = 0;i < phaseStageEffects.m_Particles.Length;++i)
                    {
                        ParticleSystem.ShapeModule shape = phaseStageEffects.m_Particles[i].shape;
                        if (shape.enabled && ParticleSystemShapeType.Mesh == shape.shapeType)
                            shape.mesh = m_WeaponMeshFilter.mesh;
                        else
                        {
                            if (shape.enabled && null != m_WeaponMeshRenderer)
                            {
                                shape.meshRenderer = m_WeaponMeshRenderer;
                                shape.shapeType = ParticleSystemShapeType.MeshRenderer;
                            }
                            if (shape.enabled && null != m_WeaponSkinnedMeshRenderer)
                            {
                                shape.skinnedMeshRenderer = m_WeaponSkinnedMeshRenderer;
                                shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                            }
                        }
                    }
                }
            }


            // 设置武器的材质为这个强化等级配置的材质
            Material phaseMat = curPhaseStageDesc.m_Material;
            if (null != phaseMat)
            {
                if(m_Renderers != null)
                {
                    for(int i = 0;i < m_Renderers.Count;++i)
                    {
                        Renderer renderer = m_Renderers[i];
                        if (null == renderer) continue;

                        Material[] materials = renderer.materials;
                        if(block != null)
                        {
                            for(int j = 0;j < materials.Length;++j)
                            {
                                Material material = materials[j];
                                if (null == material) continue;

                                renderer.GetPropertyBlock(block, j);

                                if (material.HasProperty("_MainTex"))
                                {
                                    if (material.GetTexture("_MainTex") != null)
                                    {
                                        block.SetTexture("_MainTex", material.GetTexture("_MainTex"));
                                    }
                                }
                                    
                                if (material.HasProperty("_BumpMap"))
                                {
                                    if (material.GetTexture("_BumpMap") != null)
                                    {
                                        block.SetTexture("_BumpMap", material.GetTexture("_BumpMap"));
                                    }
                                }  

                                if (material.HasProperty("_TintMap"))
                                {
                                    if (material.GetTexture("_TintMap") != null)
                                    {
                                        block.SetTexture("_TintMap", material.GetTexture("_TintMap"));
                                    }
                                }   

                                if (material.HasProperty("_Modify_Color"))
                                {
                                    block.SetColor("_Modify_Color", material.GetColor("_Modify_Color"));
                                }
                                    
                                if (material.HasProperty("_AmbientIntensity"))
                                {
                                    block.SetFloat("_AmbientIntensity", material.GetFloat("_AmbientIntensity"));
                                }
                                    

                                if (material.HasProperty("_LightIntensity"))
                                {
                                    block.SetFloat("_LightIntensity", material.GetFloat("_LightIntensity"));
                                }

                                renderer.SetPropertyBlock(block, j);
                            }
                        }

                        renderer.material = phaseMat;
                        // 记录原始材质
                        m_PhaseMatSurfDescList.Add(new PhaseMatSurfDesc(materials, renderer));
                    }
                }
            }
        }
        else
        {
            ClearPhase();
        }
    }

    public void ClearPhase()
    {
        // DeActive Glow特效
        if (m_EffGlowRoots != null)
        {
            if (m_EffGlowRoots != null)
            {
                for (int i = 0; i < m_EffGlowRoots.Count; ++i)
                {
                    if (m_EffGlowRoots[i] != null)
                        m_EffGlowRoots[i].SetActive(false);
                }
            }
        }

        // 回收或销毁特效Prefab
        if(m_CurPhaseStageEffects != null)
        {
            if (m_CurPhaseStageDesc != null)
                m_CurPhaseStageDesc.RecycleEffects(m_CurPhaseStageEffects);
            else
                GameObject.Destroy(m_CurPhaseStageEffects.m_EffectRoot);
        }
        m_CurPhaseStageDesc = null;
        m_CurPhaseStageEffects = null;

        // 恢复原始材质
        m_PhaseMatSurfDescList.RemoveAll(
            e =>
            {
                if (null == e.m_Renderer)
                    return true;

                e.m_Renderer.materials = e.m_OriginMaterials;

                return true;
            });
    }


#if UNITY_EDITOR
    public void InitInEditor()
    {
        // Clear
        m_WeaponMeshFilter = null;
        m_WeaponMeshRenderer = null;
        m_WeaponSkinnedMeshRenderer = null;
        m_Renderers.Clear();
        m_EffGlowRoots.Clear();
        m_EffGlowRenderers.Clear();


        // 用来设置粒子特效的Shape，这两个Renderer名字要求包含“Weapon” 
        m_WeaponMeshFilter = GetComponentInChildren<MeshFilter>();

        m_WeaponMeshRenderer = GetComponentInChildren<MeshRenderer>();
        if (null != m_WeaponMeshRenderer)
        {
            if (!m_WeaponMeshRenderer.name.Contains("weapon", System.StringComparison.OrdinalIgnoreCase))
                m_WeaponMeshRenderer = null;
        }

        m_WeaponSkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (null != m_WeaponSkinnedMeshRenderer)
        {
            if (!m_WeaponSkinnedMeshRenderer.name.Contains("weapon", System.StringComparison.OrdinalIgnoreCase))
                m_WeaponSkinnedMeshRenderer = null;
        }


        // 根据强化等级设置Renderer的材质，不包括Tag为“EffectModel”的Renderer！！！
        GetComponentsInChildren<Renderer>(m_Renderers);
        for(int i = 0;i < m_Renderers.Count;++i)
        {
            if(!(m_Renderers[i] is MeshRenderer || m_Renderers[i] is SkinnedMeshRenderer) 
                || m_Renderers[i].gameObject.CompareTag("EffectModel"))
            {
                m_Renderers.RemoveAt(i);
                i--;
            }
        }


        // 武器的Glow特效
        Transform[] children = GetComponentsInChildren<Transform>(true);
        for (int i = 0, icnt = children.Length; i < icnt; ++i)
        {
            Transform curChild = children[i];
            if (curChild.name.TrimEnd() == "Eff_Glow")
            {
                m_EffGlowRoots.Add(curChild.gameObject);
                m_EffGlowRenderers.AddRange(curChild.GetComponentsInChildren<Renderer>(true));
            }
        }
    }
#endif
}
