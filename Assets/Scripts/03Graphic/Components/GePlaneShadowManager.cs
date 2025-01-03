using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class GePlaneShadowManager : Singleton<GePlaneShadowManager>
{
    public class GePlaneShadowSetting
    {
        public Color m_ShadowColor;
        public Vector4 m_ShadowPlane;
        public float m_AttenuatePow;
    }

    protected class GePlaneShadowDesc
    {
        public GePlaneShadowDesc(GameObject go,SkinnedMeshRenderer[] asmr,Material mat,Vector3 min,Vector3 max, Vector4 plane)
        {
            m_PlaneShadowObj = go;
            m_ShadowObjRenderer = asmr;
            m_PlaneShadowMat = mat;
            m_BBoxMin = min;
            m_BBoxMax = max;
            m_Plane = plane;
        }

        public GameObject m_PlaneShadowObj;
        public SkinnedMeshRenderer[] m_ShadowObjRenderer = null;
        public Material m_PlaneShadowMat;
        public Vector3 m_BBoxMin = new Vector3();
        public Vector3 m_BBoxMax = new Vector3();
        public Vector4 m_Plane = new Vector4(0, 1, 0, 0.03f);
    }

    public override void Init()
    {
        m_PlaneShadowShader = AssetShaderLoader.Find("Hidden/HeroGo/PlaneShadow");
        if (null != m_PlaneShadowShader)
        {
            if (null == m_PlaneShadowMaterial)
                m_PlaneShadowMaterial = new Material(m_PlaneShadowShader);

            GameObject mainLightNode = GameObject.Find("Environment/Directional light").gameObject;
            if(null != mainLightNode)
            {
                Light mainLight = mainLightNode.GetComponent<Light>();
                if (null != mainLight)
                    mainLight.shadows = LightShadows.None;
            }

            m_RenderCommand.name = "Plane Shadow";
            //Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, m_RenderCommand);
            Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, m_RenderCommand);

            m_ShadowSetting.m_AttenuatePow = 1.0f;
            m_ShadowSetting.m_ShadowColor = new Color(0.10f,0.12f,0.11f,1.0f);
            m_ShadowSetting.m_ShadowPlane = new Vector4(0,1,0,0);
        }
    }

    public override void UnInit()
    {
        for (int i = 0; i < m_PlaneShadowDescList.Count; ++i)
        {
            m_PlaneShadowDescList[i].m_PlaneShadowObj = null;
            if (null != m_PlaneShadowDescList[i].m_PlaneShadowMat)
            {
                UnityEngine.Object.Destroy(m_PlaneShadowDescList[i].m_PlaneShadowMat);
                m_PlaneShadowDescList[i].m_PlaneShadowMat = null;
            }
        }

        m_PlaneShadowDescList.Clear();
        m_ListDirtyCount = 0;

        if (m_PlaneShadowMaterial)
            UnityEngine.Object.Destroy(m_PlaneShadowMaterial);
    }

    public void Update()
    {
        if(m_ListDirtyCount > 5)
            m_PlaneShadowDescList.RemoveAll(
                s =>
                {
                    if (null == s.m_PlaneShadowObj)
                    {
                        if(null != s.m_PlaneShadowMat)
                        {
                            UnityEngine.Object.Destroy(s.m_PlaneShadowMat);
                            s.m_PlaneShadowMat = null;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                );

        m_RenderCommand.Clear();

        for (int i = 0; i < m_PlaneShadowDescList.Count; ++i)
        {
            GePlaneShadowDesc curShadowDesc = m_PlaneShadowDescList[i];

            if(null == curShadowDesc.m_PlaneShadowObj)
                continue;

            if (!curShadowDesc.m_PlaneShadowObj.activeInHierarchy)
                continue;

            Vector3 pos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            SkinnedMeshRenderer[] amr = curShadowDesc.m_ShadowObjRenderer;
            float boundY = curShadowDesc.m_BBoxMax.y + 0.01f;
            for (int j = 0; j < amr.Length; ++j)
            {
                if(null == amr[j] || null == amr[j].sharedMesh)
                    continue;

                if (null != curShadowDesc.m_PlaneShadowMat)
                {
                    curShadowDesc.m_PlaneShadowMat.SetVector("_ShadowLightDir", Global.Settings.shadowLightDir);
                    curShadowDesc.m_PlaneShadowMat.SetVector("_ShadowPlane", curShadowDesc.m_Plane);

                    curShadowDesc.m_PlaneShadowMat.SetFloat("_ShadowInvLen", 1.0f / (boundY + curShadowDesc.m_PlaneShadowObj.transform.position.y));
                    pos = curShadowDesc.m_PlaneShadowObj.transform.position;
                    pos.y = curShadowDesc.m_Plane.w;
                    curShadowDesc.m_PlaneShadowMat.SetVector("_WorldRefPos",pos );

                    for(int k = 0; k < amr[j].sharedMesh.subMeshCount ; ++ k)
                        m_RenderCommand.DrawRenderer(amr[j], curShadowDesc.m_PlaneShadowMat, k);
                }
            }
        }
    }

    public void SetShadowSetting(GePlaneShadowSetting setting)
    {
        m_ShadowSetting.m_AttenuatePow = setting.m_AttenuatePow;
        m_ShadowSetting.m_ShadowColor = setting.m_ShadowColor;
        m_ShadowSetting.m_ShadowPlane = setting.m_ShadowPlane;

        if (null != m_PlaneShadowMaterial)
        {
            m_PlaneShadowMaterial.SetColor("_ShadowPlaneColor", m_ShadowSetting.m_ShadowColor);
            m_PlaneShadowMaterial.SetFloat("_ShadowFadePow", m_ShadowSetting.m_AttenuatePow);
            m_PlaneShadowMaterial.SetVector("_ShadowPlane", m_ShadowSetting.m_ShadowPlane);
        }
    }

    public void AddShadowObject(GameObject[] go,Vector4 plane)
    {
        Vector3 min= new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max= new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int j = 0; j < go.Length; ++j)
        {
            if(null == go[j])
                continue;

            MeshRenderer[] amr = go[j].GetComponentsInChildren<MeshRenderer>();
            if(null != amr)
            {
                for (int i = 0; i < amr.Length; ++i)
                {
                    if(null != amr[i])
                    {
                        max = Vector3.Max(max, amr[i].bounds.max);
                        min = Vector3.Min(min, amr[i].bounds.min);
                    }
                }
            }

            SkinnedMeshRenderer[] asmr = go[j].GetComponentsInChildren<SkinnedMeshRenderer>();
            if (null != asmr)
            {
                for (int i = 0; i < asmr.Length; ++i)
                {
                    if (null != asmr[i])
                    {
                        max = Vector3.Max(max, asmr[i].bounds.max);
                        min = Vector3.Min(min, asmr[i].bounds.min);
                    }
                }
            }
        }

        for (int j = 0; j < go.Length; ++ j)
        {
            if (null == go[j])
                continue;

            int Nullidx = -1;
            for (int i = 0; i < m_PlaneShadowDescList.Count; ++i)
            {
                GePlaneShadowDesc curShadowDesc = m_PlaneShadowDescList[i];
                if (null == curShadowDesc.m_PlaneShadowObj)
                {
                    Nullidx = i;
                    ++m_ListDirtyCount;
                    break;
                }

                if (curShadowDesc.m_PlaneShadowObj == go[j])
                    break;
            }

            Material newMaterial = null;
            if (-1 != Nullidx)
            {
                GePlaneShadowDesc newShadowDesc = null;
                newShadowDesc = m_PlaneShadowDescList[Nullidx];
                newShadowDesc.m_PlaneShadowObj = go[j];
                newShadowDesc.m_ShadowObjRenderer = go[j].GetComponentsInChildren<SkinnedMeshRenderer>();
                newMaterial = newShadowDesc.m_PlaneShadowMat;
                newShadowDesc.m_BBoxMax = max;
                newShadowDesc.m_BBoxMin = min;
                newShadowDesc.m_Plane = plane;

                --m_ListDirtyCount; 
            }
            else
            {
                newMaterial = new Material(m_PlaneShadowMaterial);
                m_PlaneShadowDescList.Add(new GePlaneShadowDesc(go[j], go[j].GetComponentsInChildren<SkinnedMeshRenderer>(), newMaterial,min, max,plane));
            }
        }
    }

    public void RemoveShadowObject(GameObject[] go)
    {
        if (null == go)
            return;

        for (int j = 0; j < go.Length; ++j)
        {
            for (int i = 0; i < m_PlaneShadowDescList.Count; ++i)
            {
                if (m_PlaneShadowDescList[i].m_PlaneShadowObj == go[j])
                {
                    ++m_ListDirtyCount;
                    m_PlaneShadowDescList[i].m_PlaneShadowObj = null;
                    m_PlaneShadowDescList[i].m_ShadowObjRenderer = null;
                    break;
                }
            }
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < m_PlaneShadowDescList.Count; ++i)
        {
            ++m_ListDirtyCount;
            m_PlaneShadowDescList[i].m_PlaneShadowObj = null;
            m_PlaneShadowDescList[i].m_ShadowObjRenderer = null;
        }
    }

    protected List<GePlaneShadowDesc> m_PlaneShadowDescList = new List<GePlaneShadowDesc>();
    protected Shader m_PlaneShadowShader;
    protected Material m_PlaneShadowMaterial;
    protected GePlaneShadowSetting m_ShadowSetting = new GePlaneShadowSetting();
    protected CommandBuffer m_RenderCommand = new CommandBuffer();
    protected int m_ListDirtyCount = 0;
}
