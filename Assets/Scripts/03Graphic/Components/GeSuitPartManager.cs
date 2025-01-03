using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GeSuitPartManager
{
    public class GeSuitPartDesc
    {
        public GeSuitPartDesc(DAssetObject a, EModelPartChannel c)
        {
            m_SuitPartAsset = a;
            m_SuitPartChannel = c;
        }

        public DAssetObject m_SuitPartAsset;
        public EModelPartChannel m_SuitPartChannel;
    }

    public bool Init(DModelData modelData,GameObject actorNode,GameObject avatarObject)
    {
        if (null != actorNode)
        {
            m_AvatarObject = avatarObject;

            Utility.AttachTo(m_AvatarObject, actorNode);

            for(int i = 0; i < (int)EModelPartChannel.eMaxChannelNum; ++ i)
            {
                m_SuitPartDescList.Add(new GeSuitPartDesc(new DAssetObject(null,""), (EModelPartChannel)i));
                m_SuitPartList.Add(new SuitPartModelDesc());
            }

            int nCount = 0;
            int nPartNum = modelData.partsChunk.Length;
            for(int i = 0; i < nPartNum; ++ i)
            {
                int idx = (int)modelData.partsChunk[i].partChannel;
                if (idx < m_SuitPartDescList.Count)
                {
                    m_SuitPartDescList[idx].m_SuitPartAsset = modelData.partsChunk[i].partAsset;
                    //m_SuitPartList[idx] = AssetLoader.instance.CreateGameObject(modelData.partsChunk[i].partAsset.m_AssetPath);
                    m_SuitPartList[idx].m_MeshObject = CGameObjectPool.instance.GetGameObject(modelData.partsChunk[i].partAsset.m_AssetPath, enResourceType.BattleScene,(uint)GameObjectPoolFlag.None);
                    m_NeedRebakeMesh = true;

                    if (null != m_SuitPartList[idx].m_MeshObject)
                    {
                        m_RenderMeshList.Add(m_SuitPartList[idx].m_MeshObject);
                        m_SuitPartList[idx].m_MeshObject.transform.SetParent(m_AvatarObject.transform, false);
                    }
                    ++nCount;
                    //Utility.AttachTo(m_SuitPartList[idx].m_MeshObject, m_AvatarObject);
                }
            }

            //if(nCount > 1)
            //    Bake();
        }
        else
            Logger.LogError("actor node can not be null!");

        return false;
    }

    public void Deinit()
    {
        for (int i = 0; i < m_SuitPartList.Count; ++i)
        {
            SuitPartModelDesc curModelPart = m_SuitPartList[i];

            if (null != curModelPart && null != curModelPart.m_MeshObject)
            {
                SkinnedMeshRenderer[] asmr = curModelPart.m_MeshRendererList;

                if(null != asmr)
                {
                    for (int j = 0; j < asmr.Length; ++j)
                    {
                        if(null != asmr[j])
                            asmr[j].gameObject.SetActive(true);
                    }
                }

                GameObject[] aba = curModelPart.m_BoneAll;
                if (null != aba)
                {
                    for (int j = 0; j < aba.Length; ++j)
                    {
                        if (null != aba[j])
                            aba[j].SetActive(true);
                    }
                }

                curModelPart.m_MeshObject.transform.SetParent(null, false);
                CGameObjectPool.instance.RecycleGameObject(curModelPart.m_MeshObject);
                curModelPart.m_MeshObject = null;

                curModelPart.m_MeshRendererList = null;
                curModelPart.m_BoneAll = null;
            }
        }

        if(null != m_AvatarObject)
        {
            UnityEngine.Object.Destroy(m_AvatarObject);
            m_AvatarObject = null;
        }
    }

    public bool ChangeSuitParts(EModelPartChannel suitPartChannel, DAssetObject assertObj)
    {
        int idx = (int)suitPartChannel;
        if (idx < m_SuitPartDescList.Count)
        {
            if(0 != ((1 << idx) & m_BakedMask))
            {/// 要换的part在BakedMesh中 需要删除之前bake的mesh 重新bakeMesh
                if(m_AvatarObject)
                    UnityEngine.Object.Destroy(m_AvatarObject);
                m_RenderMeshList.Clear();
            }

            GeSuitPartDesc curDesc = m_SuitPartDescList[idx];
            curDesc.m_SuitPartAsset = assertObj;

            SuitPartModelDesc curModelPart = m_SuitPartList[idx];
            if (null != curModelPart)
            {
                SkinnedMeshRenderer[] asmr = curModelPart.m_MeshRendererList;
                if(null != asmr)
                {
                    for (int j = 0; j < asmr.Length; ++j)
                    {
                        if (null != asmr[j])
                            asmr[j].gameObject.SetActive(true);
                    }
                }

                GameObject[] aba = curModelPart.m_BoneAll;
                if (null != aba)
                {
                    for (int j = 0; j < aba.Length; ++j)
                    {
                        if (null != aba[j])
                            aba[j].SetActive(true);
                    }
                }

                //UnityEngine.Object.Destroy(m_SuitPartList[idx]);
                CGameObjectPool.instance.RecycleGameObject(curModelPart.m_MeshObject);
                //m_SuitPartList[idx] = AssetLoader.instance.CreateGameObject(curDesc.m_SuitPartAsset.m_AssetPath);
                curModelPart.m_MeshObject = CGameObjectPool.instance.GetGameObject(curDesc.m_SuitPartAsset.m_AssetPath, enResourceType.BattleScene,(uint)GameObjectPoolFlag.None);
                curModelPart.m_MeshRendererList = null;
                curModelPart.m_BoneAll = null;

                m_RenderMeshList.Add(curModelPart.m_MeshObject);
            }

            m_NeedRebakeMesh = true;
            return true;
        }

        return false;
    }

    public void Bake()
    {
        List<SkinnedMeshRenderer> skinMeshRenderer = new List<SkinnedMeshRenderer>();
        for (int i = 0; i < m_SuitPartList.Count; ++i)
        {
            SuitPartModelDesc curModelPart = m_SuitPartList[i];
            if (null != curModelPart && null != curModelPart.m_MeshObject)
            {
                SkinnedMeshRenderer[] asmr = curModelPart.m_MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int j = 0; j < asmr.Length; ++j)
                {
                    skinMeshRenderer.Add(asmr[j]);
                }
                curModelPart.m_MeshRendererList = asmr;

                Animation[] animation = curModelPart.m_MeshObject.GetComponentsInChildren<Animation>();
                for (int j = 0; j < animation.Length; ++j)
                    Object.Destroy(animation[j]);

                List<GameObject> boneAllList = new List<GameObject>();
                int nChildNum = curModelPart.m_MeshObject.transform.childCount;
                for(int j = 0; j < nChildNum; ++ j )
                {
                    GameObject boneAll = curModelPart.m_MeshObject.transform.GetChild(j).gameObject;
                    if (null != boneAll && boneAll.name.Contains("BoneAll"))
                    {
                        boneAllList.Add(boneAll);
                        boneAll.SetActive(false);
                    }
                }
                curModelPart.m_BoneAll = boneAllList.ToArray();
            }
        }

        SkinnedMeshRenderer[] smRenderers = skinMeshRenderer.ToArray();

        List<Transform> bones = new List<Transform>();
        m_AvatarObject.transform.rotation = Quaternion.identity;
        Transform[] newSkeleton = m_AvatarObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
        List<BoneWeight> boneWeights = new List<BoneWeight>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Texture2D> albedoTex = new List<Texture2D>();
        List<Texture2D> bumpTex = new List<Texture2D>();

        int numSubs = 0;
        for (int k = 0; k < smRenderers.Length; ++k)
            numSubs += smRenderers[k].sharedMesh.subMeshCount;

        int[] meshIndex = new int[numSubs];
        int boneOffset = 0;

        Material combinedMat = null;
        int subMeshCnt = 0;
        for (int s = 0; s < smRenderers.Length; s++)
        {
            SkinnedMeshRenderer smr = smRenderers[s];
            if (null == combinedMat)
                combinedMat = new Material(smr.material);

            if (smr.material.mainTexture != null)
            {
                Material[] smat = smr.materials;
                for (int t = 0; t < smat.Length; ++t)
                {
                    if (smat[t].HasProperty("_MainTex"))
                        albedoTex.Add(smat[t].GetTexture("_MainTex") as Texture2D);
                    if (smat[t].HasProperty("_BumpMap"))
                        bumpTex.Add(smat[t].GetTexture("_BumpMap") as Texture2D);
                }
            }

            for (int sbm = 0; sbm < smr.sharedMesh.subMeshCount; ++sbm)
            {
                BoneWeight[] meshBoneweight = smr.sharedMesh.boneWeights;

                for (int sub = 0; sub < meshBoneweight.Length; ++sub)
                {
                    BoneWeight bWeight = meshBoneweight[sub];
                    bWeight.boneIndex0 += boneOffset;
                    bWeight.boneIndex1 += boneOffset;
                    bWeight.boneIndex2 += boneOffset;
                    bWeight.boneIndex3 += boneOffset;

                    boneWeights.Add(bWeight);
                }
                boneOffset += smr.bones.Length;

                Transform[] meshBones = smr.bones;
                for (int k = 0; k < meshBones.Length; ++k)
                {
                    Transform bone = _FindSkeletonBone(newSkeleton, meshBones[k]);//meshBones[k];//          
                    bones.Add(bone);
                }

                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sbm;
                //ci.subMeshIndex = 0;
                ci.transform = smr.transform.localToWorldMatrix;
                combineInstances.Add(ci);
                meshIndex[subMeshCnt++] = smr.sharedMesh.vertexCount;
            }

            //Object.Destroy(smr.gameObject);
            smr.gameObject.SetActive(false);
        }

        List<Matrix4x4> bindposes = new List<Matrix4x4>();
        for (int b = 0; b < bones.Count; b++)
        {
            //bindposes.Add(bones[b].worldToLocalMatrix * m_AvatarObject.transform.worldToLocalMatrix);
            bindposes.Add(bones[b].worldToLocalMatrix * m_AvatarObject.transform.worldToLocalMatrix);
        }

        SkinnedMeshRenderer BakedMeshRender = m_AvatarObject.AddComponent<SkinnedMeshRenderer>();
        BakedMeshRender.sharedMesh = new Mesh();
        BakedMeshRender.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        Texture2D albedoTexAtlas = new Texture2D(512, 512);
        Rect[] albedoPackingResult = albedoTexAtlas.PackTextures(albedoTex.ToArray(), 0,512);
        Texture2D bumpTexAtlas = new Texture2D(512, 512,TextureFormat.ARGB32,false);
        Rect[] bumpPackingResult = bumpTexAtlas.PackTextures(bumpTex.ToArray(), 0, 512);

        Vector2[] originalUVs = BakedMeshRender.sharedMesh.uv;
        Vector2[] atlasUVs = new Vector2[originalUVs.Length];

        int rectIndex = 0;
        int vertTracker = 0;
        for (int j = 0; j < atlasUVs.Length; j++)
        {
            if(rectIndex >= albedoPackingResult.Length)
                continue;

            if(rectIndex >= meshIndex.Length)
                continue;

            atlasUVs[j].x = Mathf.Lerp(albedoPackingResult[rectIndex].xMin, albedoPackingResult[rectIndex].xMax, originalUVs[j].x);
            atlasUVs[j].y = Mathf.Lerp(albedoPackingResult[rectIndex].yMin, albedoPackingResult[rectIndex].yMax, originalUVs[j].y);

            if (j >= meshIndex[rectIndex] + vertTracker)
            {
                vertTracker += meshIndex[rectIndex];
                rectIndex++;
            }
        }

        //Material combinedMat = new Material(Shader.Find("HeroGo/General/OneDirLight/Bumped"));
        //Material combinedMat = new Material(Shader.Find("Diffuse"));

        if(null != combinedMat)
        {
            if (combinedMat.HasProperty("_MainTex"))
                combinedMat.SetTexture("_MainTex", albedoTexAtlas);

            if (combinedMat.HasProperty("_BumpMap"))
                combinedMat.SetTexture("_BumpMap", bumpTexAtlas);
        }

        //combinedMat.mainTexture = albedoTexAtlas;
        BakedMeshRender.sharedMesh.uv = atlasUVs;
        BakedMeshRender.sharedMaterial = combinedMat;

        BakedMeshRender.rootBone = m_AvatarObject.transform;
        BakedMeshRender.bones = bones.ToArray();
        BakedMeshRender.sharedMesh.boneWeights = boneWeights.ToArray();
        BakedMeshRender.sharedMesh.bindposes = bindposes.ToArray();
        BakedMeshRender.sharedMesh.RecalculateBounds();

        m_RenderMeshList.Clear();
        m_RenderMeshList.Add(m_AvatarObject);

        m_NeedRebakeMesh = false;
    }


    Transform _FindSkeletonBone(Transform[] skeleton,Transform bone)
    {
        for(int i = 0; i < skeleton.Length; ++ i)
        {
            if(skeleton[i].name == bone.name)
            {
                return skeleton[i];
            }
        }

        return null;
    }

    protected class SuitPartModelDesc
    {
        public SkinnedMeshRenderer[] m_MeshRendererList = null;
        public Animation[] m_Animation = null;
        public GameObject[] m_BoneAll = null;
        public GameObject m_MeshObject = null;
    }

    protected List<SuitPartModelDesc> m_SuitPartList = new List<SuitPartModelDesc>();
    protected List<GeSuitPartDesc> m_SuitPartDescList = new List<GeSuitPartDesc>();

    protected bool m_NeedRebakeMesh = true;
    protected GameObject m_AvatarObject = null;
    protected int m_BakedMask = 0;

    protected List<GameObject> m_RenderMeshList = new List<GameObject>();

    public GameObject avatarObject
    {
        get
        {
            return m_AvatarObject;
        }
    }

    public GameObject[] suitPartModel
    {
        get
        {
            return m_RenderMeshList.ToArray();
        }
    }
}
