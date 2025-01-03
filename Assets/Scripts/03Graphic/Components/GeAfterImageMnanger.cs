using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class GeAfterImageMnanger
{
    protected class GeSnapBakeObjDesc
    {
        public GameObject m_BakedObject = null;
        public Mesh m_Mesh = null;
        public Material[] m_BakedMaterials = null;
    }

    protected class GeSnapModelDesc
    {
        public float            m_TimePos = 0;
        public int              m_TimeLen = 0;
        public GameObject       m_OriginObj = null;
        public List<GeSnapBakeObjDesc> m_BakedObjDescList = new List<GeSnapBakeObjDesc>();
    }

    public bool Init()
    {
        m_Shader = AssetShaderLoader.Find("HeroGo/General/UnLit/MonoColor");
        if (null != m_Shader)
            return true;
        else
            return false;
    }

    public void Deinit()
    {
        m_SnapModelList.RemoveAll(
            f =>
            {
                for (int i = 0, icnt = f.m_BakedObjDescList.Count; i < icnt; ++i)
                {
                    UnityEngine.Object.Destroy(f.m_BakedObjDescList[i].m_BakedObject);
                    UnityEngine.Object.Destroy(f.m_BakedObjDescList[i].m_Mesh);
                    Material[] aMat = f.m_BakedObjDescList[i].m_BakedMaterials;

                    for (int j = 0, jcnt = aMat.Length; j < jcnt; ++j)
                        UnityEngine.Object.Destroy(aMat[j]);
                }

                return true;
            }
            );
    }
    
    public void CreateSnapshot(GameObject[] objList,Color32 color,int nTimeLen, string materialPath)
    {
        Material material = null;
        if(string.IsNullOrEmpty(materialPath))
        {
            material = new Material(m_Shader);
            if (material.HasProperty("_MonoColor"))
                material.SetColor("_MonoColor", color);
        }
        else
        {
            material = AssetLoader.instance.LoadRes(materialPath, typeof(Material)).obj as Material;
        }

        for (int o = 0; o < objList.Length; ++ o)
        {
            GameObject obj = objList[o];
            if (null != obj)
            {
                Animation anim = obj.GetComponentInChildren<Animation>();
                if (anim)
                    anim.Sample();

                if (null != material)
                {
                    GeSnapModelDesc snapModel = new GeSnapModelDesc();

                    SkinnedMeshRenderer[] asmr = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    for (int r = 0; r < asmr.Length; ++r)
                    {
                        if (null != asmr[r])
                        {
                            GeSnapBakeObjDesc newBakeObjDesc = new GeSnapBakeObjDesc();
                            Mesh bakeMesh = new Mesh();
                            bakeMesh.name = "snapped";
                            asmr[r].BakeMesh(bakeMesh); //烘焙到新生成的mesh

                            newBakeObjDesc.m_BakedMaterials = new Material[bakeMesh.subMeshCount];
                            GameObject go = new GameObject("SnapShot"); //生成新的GameObject
                            go.hideFlags = HideFlags.HideAndDontSave; //设置属性隐藏不保存

                            MeshFilter meshFilter = go.AddComponent<MeshFilter>(); //添加MeshFilter
                            meshFilter.mesh = bakeMesh; //将新添加的MeshFilter设置为mesh

                            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();//添加MeshRenderer
                            if(newBakeObjDesc.m_BakedMaterials.Length > 0)
                            {
                                for (int i = 0, icnt = newBakeObjDesc.m_BakedMaterials.Length; i < icnt; ++i)
                                    newBakeObjDesc.m_BakedMaterials[i] = new Material(material);
                                meshRenderer.materials = newBakeObjDesc.m_BakedMaterials;
                                meshRenderer.material = newBakeObjDesc.m_BakedMaterials[0];//将新添加的MeshRenderer的材质设置为获取到的材质
                            }

                            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                            meshRenderer.receiveShadows = false;

                            go.transform.position = asmr[r].gameObject.transform.position;
                            go.transform.rotation = asmr[r].gameObject.transform.rotation;
                            go.transform.localScale = asmr[r].gameObject.transform.localScale;


                            if (asmr[r].gameObject.transform.lossyScale.x < 0)
                            {
                                if (material.HasProperty("_CullMode"))
                                    material.SetFloat("_CullMode", (float)CullMode.Front);
                            }
                            else
                            {
                                if (material.HasProperty("_CullMode"))
                                    material.SetFloat("_CullMode", (float)CullMode.Back);
                            }

                            newBakeObjDesc.m_BakedObject = go;
                            newBakeObjDesc.m_Mesh = bakeMesh;
                            //snapModel.m_BakedObjList.Add(go);
                            snapModel.m_BakedObjDescList.Add(newBakeObjDesc);
                        }

                        snapModel.m_TimeLen = nTimeLen;
                        snapModel.m_OriginObj = obj;
                    }

                    m_SnapModelList.Add(snapModel);
                }
            }


        }
    }

    bool _IsOverLife(GeSnapModelDesc curSanpMdlDesc)
    {
        if (null == curSanpMdlDesc)
            return true;

        return curSanpMdlDesc.m_TimePos >= 1.0f;
    }

    public void Update(int deltaTime, GameObject obj)
    {
        bool isDirty = false;
        for (int i = 0; i < m_SnapModelList.Count; ++ i)
        {
            GeSnapModelDesc curSnapModel = m_SnapModelList[i];
            curSnapModel.m_TimePos += (float)deltaTime / (float)curSnapModel.m_TimeLen;

            if (_IsOverLife(curSnapModel))
                isDirty = true;

            for (int j = 0,jcnt = curSnapModel.m_BakedObjDescList.Count;j<jcnt;++j)
            {
                Material[] aMat = curSnapModel.m_BakedObjDescList[j].m_BakedMaterials;
                if (null == aMat) continue;

                for(int k = 0 ,kcnt = aMat.Length;k<kcnt;++k)
                {
                    Material curMat = aMat[k];
                    if (null == curMat) continue;

                    if (curMat.HasProperty("_Factor"))
                        curMat.SetFloat("_Factor", curSnapModel.m_TimePos);

                    if (curMat.HasProperty("_WorldRefPos"))
                        curMat.SetVector("_WorldRefPos", obj.transform.position);
                }
            }
        }

        if (isDirty)
            _ClearOverLifeSnapModel();
    }

    void _ClearOverLifeSnapModel()
    {
        m_SnapModelList.RemoveAll(
            f =>
            {
                if (_IsOverLife(f))
                {
                    for (int i = 0, icnt = f.m_BakedObjDescList.Count; i < icnt; ++i)
                    {
                        UnityEngine.Object.Destroy(f.m_BakedObjDescList[i].m_BakedObject);
                        UnityEngine.Object.Destroy(f.m_BakedObjDescList[i].m_Mesh);
                        Material[] aMat = f.m_BakedObjDescList[i].m_BakedMaterials;

                        for (int j = 0, jcnt = aMat.Length; j < jcnt; ++j)
                            UnityEngine.Object.Destroy(aMat[j]);
                    }

                    return true;
                }
                else
                    return false;
            }
            );
    }
    
    protected List<GeSnapModelDesc> m_SnapModelList = new List<GeSnapModelDesc>();
    protected Shader m_Shader;
}
