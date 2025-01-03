using UnityEngine;
using System.Collections.Generic;

public class GeSceneObjManager : Singleton<GeSceneObjManager>
{
    protected readonly string ALPHA_PARAM_NAME = "_AlphaFactor";
    protected readonly float ALPHA_DELTA_SPEED = 0.05f;
    protected readonly float ALPHA_MIN_VALUE = 0.3f;

    protected class GeOcclusionMatDesc
    {
        public float m_OriginAlpha = 1.0f;
        public float m_CurAlpha = 1.0f;
        public Material m_OriginMaterial = null;
    }

    protected class GeOcclusionMeshDesc
    {
        public GeOcclusionMatDesc[] m_MatDescArray = null;
        public Renderer m_MeshRender = null;
    }

    protected class GeOcclusionObjDesc
    {
        public GeOcclusionMeshDesc[] m_RenderArray = null;
        public float m_AABBXMax = 0;
        public float m_AABBXMin = 0;
        public GameObject m_Object = null;
        public bool m_Fading = false;
        public bool m_Occluding = false;
    }

    protected List<GeOcclusionObjDesc> m_OcclusionDescList = new List<GeOcclusionObjDesc>();
    protected GeCamera m_Camera = null;

    protected GameObject m_FocusObj = null;

    public override void Init()
    {
    }

    public override void UnInit()
    {
    }

    public bool SetRefCamera(GeCamera mainCamera)
    {
        if (null == mainCamera)
            return false;

        m_Camera = mainCamera;

        return true;
    }

    public void SetFocusEntity(GameObject goFocus)
    {
        m_FocusObj = goFocus;
    }

    public void AddOccludeObject(GameObject[] obj)
    {
        for (int i = 0; i < obj.Length; ++i)
        {
            GameObject curObj = obj[i];
            if (null == curObj)
                continue;

            List<Renderer> renderList = new List<Renderer>();
            renderList.AddRange(curObj.GetComponentsInChildren<MeshRenderer>());
            renderList.AddRange(curObj.GetComponentsInChildren<SkinnedMeshRenderer>());

            if(renderList.Count > 0)
            {
                /// 替换掉之前的信息
                RemoveOccludeObject(curObj);

                GeOcclusionObjDesc newOccObjDesc = new GeOcclusionObjDesc();
                newOccObjDesc.m_RenderArray = new GeOcclusionMeshDesc[renderList.Count];
                newOccObjDesc.m_Object = curObj;

                newOccObjDesc.m_AABBXMax = float.MinValue;
                newOccObjDesc.m_AABBXMin = float.MaxValue;

                for (int j = 0; j < newOccObjDesc.m_RenderArray.Length; ++ j)
                {
                    GeOcclusionMeshDesc newOccMeshDesc = new GeOcclusionMeshDesc();

                    newOccMeshDesc.m_MeshRender = renderList[j];
                    newOccMeshDesc.m_MatDescArray = new GeOcclusionMatDesc[renderList[j].materials.Length];

                    for (int k = 0; k < renderList[j].materials.Length; ++k)
                    {
                        GeOcclusionMatDesc newOccMatDesc = new GeOcclusionMatDesc();
                        newOccMatDesc.m_OriginMaterial = renderList[j].materials[k];

                        if (newOccMatDesc.m_OriginMaterial.HasProperty(ALPHA_PARAM_NAME))
                            newOccMatDesc.m_OriginAlpha = newOccMatDesc.m_OriginMaterial.GetFloat(ALPHA_PARAM_NAME);

                        newOccMatDesc.m_CurAlpha = newOccMatDesc.m_OriginAlpha;

                        newOccMeshDesc.m_MatDescArray[k] = newOccMatDesc;
                    }

                    newOccObjDesc.m_AABBXMin = Mathf.Min(newOccMeshDesc.m_MeshRender.bounds.min.x, newOccObjDesc.m_AABBXMin);
                    newOccObjDesc.m_AABBXMax = Mathf.Max(newOccMeshDesc.m_MeshRender.bounds.max.x, newOccObjDesc.m_AABBXMax);

                    newOccObjDesc.m_RenderArray[j] = newOccMeshDesc;
                }

                m_OcclusionDescList.Add(newOccObjDesc);
            }
        }
    }

    public void RemoveOccludeObject(GameObject obj)
    {
        m_OcclusionDescList.RemoveAll(
            f =>
            {
                if (null == f.m_Object)
                    return true;

                if (obj == f.m_Object)
                {
                    if (null != f.m_RenderArray)
                    {
                        for (int j = 0; j < f.m_RenderArray.Length; ++j)
                        {
                            GeOcclusionMeshDesc omd = f.m_RenderArray[j];
                            GeOcclusionMatDesc[] omat = omd.m_MatDescArray;

                            for (int k = 0; k < omat.Length; ++k)
                            {
                                if (omat[k].m_OriginMaterial.HasProperty(ALPHA_PARAM_NAME))
                                    omat[k].m_OriginMaterial.SetFloat(ALPHA_PARAM_NAME, omd.m_MatDescArray[k].m_OriginAlpha);
                            }

                            omd.m_MatDescArray = null;
                            omd.m_MeshRender = null;
                        }
                    }

                    return true;
                }
                else
                    return false;
            });
    }

    public void ClearAll()
    {
        m_OcclusionDescList.RemoveAll(
            f =>
            {
                if (null == f.m_Object)
                    return true;

                if (null != f.m_RenderArray)
                {
                    for (int j = 0; j < f.m_RenderArray.Length; ++j)
                    {
                        GeOcclusionMeshDesc omd = f.m_RenderArray[j];
                        GeOcclusionMatDesc[] omat = omd.m_MatDescArray;

                        for (int k = 0; k < omat.Length; ++k)
                        {
                            if (omat[k].m_OriginMaterial.HasProperty(ALPHA_PARAM_NAME))
                                omat[k].m_OriginMaterial.SetFloat(ALPHA_PARAM_NAME, omd.m_MatDescArray[k].m_OriginAlpha);
                        }

                        omd.m_MatDescArray = null;
                        omd.m_MeshRender = null;
                    }
                }

                return true;
            });
    }

    public void Update()
    {
        if (null == m_FocusObj)
            return;

        Vector3 pos = m_FocusObj.transform.position;

        if (null != m_Camera)
        {
            float refZ = m_Camera.position.z - pos.z;

            for(int i = 0; i < m_OcclusionDescList.Count; ++ i)
            {
                GeOcclusionObjDesc curObjDesc = m_OcclusionDescList[i];

                if(null == curObjDesc || null == curObjDesc.m_Object) continue;

                float curDeltaX = curObjDesc.m_Object.transform.position.x - pos.x;
                bool bNewState = curObjDesc.m_AABBXMin < curDeltaX && curDeltaX < curObjDesc.m_AABBXMax && (m_Camera.position.z - curObjDesc.m_Object.transform.position.z > refZ);

                if(curObjDesc.m_Fading)
                {
                    bool fadingAll = false;
                    if (null != curObjDesc.m_RenderArray)
                    {
                        for (int j = 0; j < curObjDesc.m_RenderArray.Length; ++j)
                        {
                            GeOcclusionMeshDesc omd = curObjDesc.m_RenderArray[j];
                            if (null == omd)
                                continue;

                            GeOcclusionMatDesc[] omat = omd.m_MatDescArray;
                            if (null == omat)
                                continue;

                            for (int k = 0; k < omat.Length; ++k)
                            {
                                if (null == omat[k])
                                    continue;

                                bool fading = true;
                                omat[k].m_CurAlpha += curObjDesc.m_Occluding ? (-ALPHA_DELTA_SPEED) : (ALPHA_DELTA_SPEED);

                                if (omat[k].m_CurAlpha < omat[k].m_OriginAlpha * ALPHA_MIN_VALUE)
                                {
                                    omat[k].m_CurAlpha = omat[k].m_OriginAlpha * ALPHA_MIN_VALUE;
                                    fading = false;
                                }

                                if (omat[k].m_CurAlpha > omat[k].m_OriginAlpha)
                                {
                                    omat[k].m_CurAlpha = omat[k].m_OriginAlpha;
                                    fading = false;
                                }
                                
                                if (omat[k].m_OriginMaterial.HasProperty(ALPHA_PARAM_NAME))
                                    omat[k].m_OriginMaterial.SetFloat(ALPHA_PARAM_NAME, omat[k].m_CurAlpha);

                                fadingAll |= fading;
                            }
                        }
                    }

                    curObjDesc.m_Fading = fadingAll;
                }
                else
                    curObjDesc.m_Fading = bNewState ^ curObjDesc.m_Occluding;

                curObjDesc.m_Occluding = bNewState;
            }
        }
    }


}
