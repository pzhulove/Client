using UnityEngine;
using System.Collections.Generic;

public class GeSimpleShadowManager : Singleton<GeSimpleShadowManager>
{

    protected class GeSimpleShadowDesc
    {
        public GeSimpleShadowDesc(GameObject go,GameObject shadowGo,Vector4 plane)
        {
            m_PlaneShadowObj = go;
            m_Plane = plane;
            m_PlaneGO = shadowGo;
        }

        public GameObject m_PlaneShadowObj;
        public GameObject m_PlaneGO;
        public Vector4 m_Plane = new Vector4(0, 1, 0, 0.03f);
    }

    public sealed override void Init()
    {
    }

    public sealed override void UnInit()
    {
    }

    public void Update()
    {
        for (int i = 0; i < m_SimpleShadowDescList.Count; ++i)
        {
            GeSimpleShadowDesc curShadowDesc = m_SimpleShadowDescList[i];

            if (curShadowDesc == null) continue;
            if (null == curShadowDesc.m_PlaneShadowObj)
                continue;

            if (!curShadowDesc.m_PlaneShadowObj.activeInHierarchy)
                continue;

            if (null == curShadowDesc.m_PlaneGO)
                continue;

            Vector3 pos = curShadowDesc.m_PlaneGO.transform.position;
            pos.y = 0.05f + curShadowDesc.m_Plane.w;
            curShadowDesc.m_PlaneGO.transform.position = pos;
			curShadowDesc.m_PlaneGO.transform.rotation = Quaternion.identity;
        }
    }

	public void AddShadowObject(GameObject[] go, Vector4 plane, Vector3 scale,Transform parent = null)
    {	
        if (null != go && go.Length > 0)
        {
            GameObject newGo = go[0];
            if (null == newGo)
                return;

            string name = newGo.name;
            //if (name.Contains("Cube") || name.Contains("Eff"))
            //    return;

            Renderer meshR = null;
            for (int i = 0,icnt = go.Length;i<icnt;++i)
            {
                meshR = go[i].GetComponentInChildren<Renderer>();
                //if (null != meshR)
                //{
                //    Vector3 temp = meshR.bounds.extents;
                //    scale = temp.x * temp.z > scale ? temp.x * temp.z : scale;
                //}
            }
            if (null == meshR)
                return;

            //Renderer meshR = newGo.GetComponentInChildren<Renderer>();
            //if (null != meshR)
            //{
            //    Vector3 temp = meshR.bounds.extents;
            //    scale = temp.x * temp.z * 5;
            //}
            //else
            //    return;
            int Nullidx = -1;
            for (int i = 0; i < m_SimpleShadowDescList.Count; ++i)
            {
                GeSimpleShadowDesc curShadowDesc = m_SimpleShadowDescList[i];
                if (null == curShadowDesc.m_PlaneShadowObj)
                {
                    Nullidx = i;
                    break;
                }

                if (curShadowDesc.m_PlaneShadowObj == newGo)
                    break;
            }

            if (-1 != Nullidx)
            {
                GeSimpleShadowDesc newShadowDesc = null;

                if (m_SimpleShadowDescList[Nullidx] != null)
                {
                    newShadowDesc = m_SimpleShadowDescList[Nullidx];

                    newShadowDesc.m_PlaneShadowObj = newGo;
                    if (null == newShadowDesc.m_PlaneGO)
                        newShadowDesc.m_PlaneGO = _CreateShadowRes();

                    if (null != newShadowDesc.m_PlaneGO)
                    {
                        newShadowDesc.m_PlaneGO.SetActive(true);
                        newShadowDesc.m_PlaneGO.transform.localPosition = Vector3.zero;
                        Vector3 pos = newGo.transform.position;
                        pos.y = 0.05f + plane.w;
                        newShadowDesc.m_PlaneGO.transform.position = pos;
                        newShadowDesc.m_PlaneGO.transform.localScale = scale;


                        newShadowDesc.m_PlaneGO.transform.SetParent(parent == null?newShadowDesc.m_PlaneShadowObj.transform: parent, true);

                        //new Vector3(scale, scale, scale);
                        newShadowDesc.m_PlaneGO.transform.rotation = Quaternion.identity;
                    }

                    newShadowDesc.m_Plane = plane;
                }
            }
            else
            {
                GameObject shadowGo = _CreateShadowRes();
                if (shadowGo!=null)
                {
                    shadowGo.transform.localPosition = Vector3.zero;
                    Vector3 pos = newGo.transform.position;
                    pos.y = 0.05f + plane.w;
                    shadowGo.transform.position = pos;


                    shadowGo.transform.SetParent(parent == null ? newGo.transform : parent, true);
                    var localScale = shadowGo.transform.localScale;
                    shadowGo.transform.localScale = scale;//new Vector3(localScale.x*scale.x, localScale.y*scale.y, localScale.z*scale.z);

                    m_SimpleShadowDescList.Add(new GeSimpleShadowDesc(newGo, shadowGo, plane));
                    shadowGo.transform.rotation = Quaternion.identity;
                }
            }
        }
    }

    GameObject _CreateShadowRes()
    {
        return CGameObjectPool.instance.GetGameObject("Actor/Other_ShadowPlane/p_ShadowPlane",enResourceType.BattleScene,(uint)GameObjectPoolFlag.ReserveLast);
    }

    //获取影子的预制体
    public GameObject GetShadowObj(GameObject go)
    {
        if (GetShadow(go) == null)
            return null;
        return GetShadow(go).m_PlaneGO;
    }

    protected GeSimpleShadowDesc GetShadow(GameObject go)
    {
        if (go == null)
            return null;
        GeSimpleShadowDesc shadowDesc = null;
        for (int i = 0; i < m_SimpleShadowDescList.Count; ++i)
        {
            GeSimpleShadowDesc curShadowDesc = m_SimpleShadowDescList[i];
            if (curShadowDesc.m_PlaneShadowObj == go)
            {
                if (null != curShadowDesc)
                {
                    shadowDesc = curShadowDesc;
                    break;
                }
            }
        }
        return shadowDesc;
    }

    public void RemoveShadowObject(GameObject[] go)
    {
		if (go == null)
			return;
		
        for (int j = 0; j < go.Length; ++j)
        {
            for (int i = 0; i < m_SimpleShadowDescList.Count; ++i)
            {
                GeSimpleShadowDesc curShadowDesc = m_SimpleShadowDescList[i];

                if (curShadowDesc.m_PlaneShadowObj == go[j])
                {
                    ++m_ListDirtyCount;
                    curShadowDesc.m_PlaneShadowObj = null;
                    if (null != curShadowDesc.m_PlaneGO)
                    {
                        CGameObjectPool.instance.RecycleGameObject(curShadowDesc.m_PlaneGO);
                        curShadowDesc.m_PlaneGO = null;
                    }
                    break;
                }
            }
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < m_SimpleShadowDescList.Count; ++i)
        {
            GeSimpleShadowDesc curShadowDesc = m_SimpleShadowDescList[i];
            if (null != curShadowDesc.m_PlaneGO)
            {
                curShadowDesc.m_PlaneGO.transform.SetParent(null);
                //curShadowDesc.m_PlaneGO.transform.localScale = Vector3.one;
                CGameObjectPool.instance.RecycleGameObject(curShadowDesc.m_PlaneGO);
                curShadowDesc.m_PlaneGO = null;
                curShadowDesc.m_PlaneShadowObj = null;
            }
        }

        m_SimpleShadowDescList.Clear();
    }

    protected List<GeSimpleShadowDesc> m_SimpleShadowDescList = new List<GeSimpleShadowDesc>();
    protected int m_ListDirtyCount = 0;
}
