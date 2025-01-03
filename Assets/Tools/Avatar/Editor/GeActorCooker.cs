using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using GameObject = UnityEngine.GameObject;
using Object = UnityEngine.Object;

public class GeActorCooker : Editor
{
    private static string[] DUMMY_LIST = new string[]
        {
            "OverHead",
            "Orign",
            "OrignBuff",
            "LHand",
            "RHand",
            "LWeapon",
            "RWeapon",
            "Body",
            "LFoot",
            "RFoot",
            "Back",
            "Crotch",
            "LClavicle",
            "RClavicle",
        };

    //_GenerateActorPrefabForThunkAndMarkDummyNodeGUI

    //[MenuItem("Assets/生成角色信息并标记骨骼挂点")]
    private static void _GenerateActorPrefabAndMarkDummyNodeGUI()
    {
        List<GameObject> selectObjs = _GetSelectAssetByType<GameObject>("t:prefab");
        int selectObjCount = selectObjs.Count;
        for (int i = 0; i < selectObjCount; i++)
        {
            EditorUtility.DisplayProgressBar("制作角色资源", "正在制作第" + i + "个资源...", (i / selectObjCount));
            _GenerateActorPrefab(selectObjs[i]);
            EditorUtility.DisplayProgressBar("标记骨骼挂点", "正在标记第" + i + "个资源...", (i / selectObjCount));
            _MarkDummyNode(selectObjs[i]);
            Selection.activeGameObject = selectObjs[i];
        }
        EditorUtility.ClearProgressBar();
    }

    private static List<T> _GetSelectAssetByType<T>(string typeFilter) where T : Object
    {
        List<T> selectAssets = new List<T>();
        string[] selectIds = Selection.assetGUIDs;
        List<string> selectDirPaths = new List<string>();
        foreach (string id in selectIds)
        {
            string selectPath = AssetDatabase.GUIDToAssetPath(id);
            if (Directory.Exists(selectPath))
                selectDirPaths.Add(selectPath);
            if (File.Exists(selectPath))
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(selectPath);
                if (asset != null)
                    selectAssets.Add(asset);
            }
        }
        if (selectDirPaths.Count > 0)
        {
            string[] objInDirIds = AssetDatabase.FindAssets(typeFilter, selectDirPaths.ToArray());
            foreach (string id in objInDirIds)
            {
                string selectPath = AssetDatabase.GUIDToAssetPath(id);
                T obj = AssetDatabase.LoadAssetAtPath<T>(selectPath);
                if (obj != null && !selectAssets.Contains(obj))
                    selectAssets.Add(obj);
            }
        }
        return selectAssets;
    }

    public static void _GenerateActorPrefab(GameObject data)
    {
        if (null == data)
            return;
        string path = AssetDatabase.GetAssetPath(data);
        if (path.Contains("_camera"))
            return;
        if (path.Contains("/Weapon/") || path.Contains("/WeaponShow/"))
            return;

        //GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
        GameObject temp = Instantiate(data);
        if (null != temp)
        {
            SkinnedMeshRenderer[] asmr = temp.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (null != asmr)
            {
                for (int j = 0, icnt = asmr.Length; j < icnt; ++j)
                {
                    if (null == asmr[j]) continue;

                    GeMeshDescProxy newProxy = asmr[j].gameObject.GetComponent<GeMeshDescProxy>();
                    if (null == newProxy)
                    {
                        newProxy = asmr[j].gameObject.AddComponent<GeMeshDescProxy>();
                        newProxy.m_Surface_IsOpaque = true;
                    }

                    Material[] am = asmr[j].sharedMaterials;
                    for (int k = 0, kcnt = am.Length; k < kcnt; ++k)
                    {
                        Material m = am[k];
                        if (null == m) continue;

                        if (m.shader.name.Contains("PBR"))
                        {
                            newProxy.surfaceAnimatRes = "Animat/CommonBuffEffectPBR.asset";
                            break;
                        }
                    }

                    asmr[j].shadowCastingMode = ShadowCastingMode.Off;
                    asmr[j].lightProbeUsage = LightProbeUsage.Off;
                    asmr[j].motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    asmr[j].receiveShadows = false;
                    asmr[j].reflectionProbeUsage = ReflectionProbeUsage.Off;
                }
            }

            MeshRenderer[] amr = temp.GetComponentsInChildren<MeshRenderer>();
            if (null != amr)
            {
                for (int j = 0, jcnt = amr.Length; j < jcnt; ++j)
                {
                    if (null == amr[j]) continue;

                    GeMeshDescProxy newProxy = amr[j].gameObject.GetComponent<GeMeshDescProxy>();
                    if (null == newProxy)
                        newProxy = amr[j].gameObject.AddComponent<GeMeshDescProxy>();

                    if (null != newProxy)
                        newProxy.m_Surface_IsOpaque = true;

                    Material[] am = amr[j].sharedMaterials;
                    for (int k = 0, kcnt = am.Length; k < kcnt; ++k)
                    {
                        Material m = am[k];
                        if (null == m) continue;

                        if (m.shader.name.Contains("HeroGo/PBR/Surface"))
                        {
                            newProxy.m_ShadeModel = MatAnimatShadeModel.PBR;
                            newProxy.surfaceAnimatRes = "Animat/CommonBuffEffectPBR.asset";
                            break;
                        }
                        else if (m.shader.name.Contains("HeroGo/SimplePBR"))
                        {
                            newProxy.m_ShadeModel = MatAnimatShadeModel.Simple;
                            break;
                        }
                    }

                    amr[j].shadowCastingMode = ShadowCastingMode.Off;
                    amr[j].lightProbeUsage = LightProbeUsage.Off;
                    amr[j].motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    amr[j].receiveShadows = false;
                    amr[j].reflectionProbeUsage = ReflectionProbeUsage.Off;
                }
            }

            /// 修改骨骼 如果是角色部件的话 先添加AvatarProxy 在删除骨骼
            if (path.Contains("_Head", System.StringComparison.OrdinalIgnoreCase) ||
                path.Contains("_Body", System.StringComparison.OrdinalIgnoreCase) ||
                path.Contains("_Pant", System.StringComparison.OrdinalIgnoreCase) ||
                path.Contains("_Jewelry", System.StringComparison.OrdinalIgnoreCase))
            {
                Animation anim = temp.GetComponent<Animation>();
                if (null != anim)
                    GameObject.DestroyImmediate(anim);

                GeAnimDescProxy proxy = temp.GetComponent<GeAnimDescProxy>();
                if (null != proxy)
                    GameObject.DestroyImmediate(proxy);

                /// 添加AvatarProxy
                GameObject avatar = _GetAvatarPrefab(path);
                if (null != avatar)
                {
                    GeAvatarProxy avatarProxy = temp.GetComponent<GeAvatarProxy>();
                    if (null == avatarProxy)
                        avatarProxy = temp.AddComponent<GeAvatarProxy>();

                    avatarProxy.avatar = avatar;
                    avatarProxy.RefreshAvatarDesc();
                }

                GameObject boneAll = _FindSkeletonRoot(temp);
                //PrefabUtility.UnpackPrefabInstance(temp, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                GameObject.DestroyImmediate(boneAll);
                temp.SetActive(false);
            }
            else
            {
                GeAnimDescProxy proxy = temp.GetComponent<GeAnimDescProxy>();
                if (null == proxy)
                    proxy = temp.AddComponent<GeAnimDescProxy>();

                List<string> fbxLst = new List<string>();
                _GetNearestAnimationPath(path, ref fbxLst);

                for (int j = 0, jcnt = fbxLst.Count; j < jcnt; ++j)
                    fbxLst[j] = fbxLst[j].Replace('\\', '/').Replace("Assets/Resources/", null);

                Animation anim = temp.GetComponent<Animation>();
                if (null != anim)
                {
                    AnimationClip defClip = anim.clip;

                    Object.DestroyImmediate(anim);
                    anim = temp.AddComponent<Animation>();
                    if (null != anim)
                    {
                        anim.clip = defClip;
                        anim.cullingType = AnimationCullingType.BasedOnRenderers;
                    }
                }

                if (fbxLst.Count > 0)
                {
                    proxy.animDataResFile = fbxLst.ToArray();
                    proxy.GenAnimDesc();
                }
            }

            temp.transform.position = Vector3.zero;

            AssetProxy assetProxy = temp.GetComponent<AssetProxy>();
            if (null == assetProxy)
                temp.AddComponent<AssetProxy>();
        }

        AssetDatabase.SaveAssets();
        PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
        GameObject.DestroyImmediate(temp);
    }

    [MenuItem("Assets/角色工具/20、生成角色信息（主干）并标记骨骼挂点", false, 20)]
    private static void _GenerateActorPrefabForThunkAndMarkDummyNodeGUI()
    {
        List<GameObject> selectObjs = _GetSelectAssetByType<GameObject>("t:prefab");
        int selectObjCount = selectObjs.Count;
        for (int i = 0; i < selectObjCount; i++)
        {
            EditorUtility.DisplayProgressBar("制作角色资源", "正在制作第" + i + "个资源...", (i / selectObjCount));
            _GenerateActorPrefabForThunk(selectObjs[i]);
            EditorUtility.DisplayProgressBar("标记骨骼挂点", "正在标记第" + i + "个资源...", (i / selectObjCount));
            _MarkDummyNode(selectObjs[i]);
            Selection.activeGameObject = selectObjs[i];
        }
        EditorUtility.ClearProgressBar();
    }

    private static void _GenerateActorPrefabForThunk(GameObject data)
    {
        if (null == data)
            return;
        string path = AssetDatabase.GetAssetPath(data);
        if (path.Contains("_camera"))
            return;
        if (path.Contains("/Weapon/") || path.Contains("/WeaponShow/"))
            return;
        if (path.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
            return;
        //GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
        GameObject temp = Instantiate(data);
        if (null != temp)
        {
            SkinnedMeshRenderer[] asmr = temp.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (null != asmr)
            {
                for (int j = 0, icnt = asmr.Length; j < icnt; ++j)
                {
                    if (null == asmr[j]) continue;

                    GeMeshDescProxy newProxy = asmr[j].gameObject.GetComponent<GeMeshDescProxy>();
                    if (null == newProxy)
                    {
                        newProxy = asmr[j].gameObject.AddComponent<GeMeshDescProxy>();
                        newProxy.m_Surface_IsOpaque = true;
                    }

                    Material[] am = asmr[j].sharedMaterials;
                    for (int k = 0, kcnt = am.Length; k < kcnt; ++k)
                    {
                        Material m = am[k];
                        if (null == m) continue;

                        if (m.shader.name.Contains("HeroGo/PBR/Surface"))
                        {
                            newProxy.m_ShadeModel = MatAnimatShadeModel.PBR;
                            newProxy.surfaceAnimatRes = "Animat/CommonBuffEffectPBR.asset";
                            break;
                        }
                        else if (m.shader.name.Contains("HeroGo/SimplePBR"))
                        {
                            newProxy.m_ShadeModel = MatAnimatShadeModel.Simple;
                            break;
                        }
                    }

                    asmr[j].shadowCastingMode = ShadowCastingMode.Off;
                    asmr[j].lightProbeUsage = LightProbeUsage.Off;
                    asmr[j].motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    asmr[j].receiveShadows = false;
                    asmr[j].reflectionProbeUsage = ReflectionProbeUsage.Off;
                }
            }

            MeshRenderer[] amr = temp.GetComponentsInChildren<MeshRenderer>();
            if (null != amr)
            {
                for (int j = 0, jcnt = amr.Length; j < jcnt; ++j)
                {
                    if (null == amr[j]) continue;

                    GeMeshDescProxy newProxy = amr[j].gameObject.GetComponent<GeMeshDescProxy>();
                    if (null == newProxy)
                        newProxy = amr[j].gameObject.AddComponent<GeMeshDescProxy>();

                    if (null != newProxy)
                        newProxy.m_Surface_IsOpaque = true;

                    Material[] am = amr[j].sharedMaterials;
                    for (int k = 0, kcnt = am.Length; k < kcnt; ++k)
                    {
                        Material m = am[k];
                        if (null == m) continue;

                        if (m.shader.name.Contains("PBR"))
                        {
                            newProxy.surfaceAnimatRes = "Animat/CommonBuffEffectPBR.asset";
                            break;
                        }
                    }

                    amr[j].shadowCastingMode = ShadowCastingMode.Off;
                    amr[j].lightProbeUsage = LightProbeUsage.Off;
                    amr[j].motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    amr[j].receiveShadows = false;
                    amr[j].reflectionProbeUsage = ReflectionProbeUsage.Off;
                }
            }

            /// 修改骨骼 如果是角色部件的话 先添加AvatarProxy 在删除骨骼
            if (path.Contains("_Head", System.StringComparison.OrdinalIgnoreCase) ||
                path.Contains("_Body", System.StringComparison.OrdinalIgnoreCase) ||
                path.Contains("_Pant", System.StringComparison.OrdinalIgnoreCase) ||
                path.Contains("_Jewelry", System.StringComparison.OrdinalIgnoreCase))
            {
                Animation anim = temp.GetComponent<Animation>();
                if (null != anim)
                    GameObject.DestroyImmediate(anim);

                GeAnimDescProxy proxy = temp.GetComponent<GeAnimDescProxy>();
                if (null != proxy)
                    GameObject.DestroyImmediate(proxy);

                /// 添加AvatarProxy
                GameObject avatar = _GetAvatarPrefab(path);
                if (null != avatar)
                {
                    GeAvatarProxy avatarProxy = temp.GetComponent<GeAvatarProxy>();
                    if (null == avatarProxy)
                        avatarProxy = temp.AddComponent<GeAvatarProxy>();

                    avatarProxy.avatar = avatar;
                    avatarProxy.RefreshAvatarDesc();
                }
            }
            else
            {
                GeAnimDescProxy proxy = temp.GetComponent<GeAnimDescProxy>();
                if (null == proxy)
                    proxy = temp.AddComponent<GeAnimDescProxy>();

                List<string> fbxLst = new List<string>();
                _GetNearestAnimationPath(path, ref fbxLst);

                for (int j = 0, jcnt = fbxLst.Count; j < jcnt; ++j)
                    fbxLst[j] = fbxLst[j].Replace('\\', '/').Replace("Assets/Resources/", null);

                Animation anim = temp.GetComponent<Animation>();
                if (null != anim)
                {
                    AnimationClip defClip = anim.clip;

                    Object.DestroyImmediate(anim);
                    anim = temp.AddComponent<Animation>();
                    if (null != anim)
                    {
                        anim.clip = defClip;
                        anim.cullingType = AnimationCullingType.BasedOnRenderers;
                    }
                }

                if (fbxLst.Count > 0)
                {
                    proxy.animDataResFile = fbxLst.ToArray();
                    proxy.GenAnimDesc();
                }

                string prefabDirPath = Path.GetDirectoryName(path);
                if (prefabDirPath.EndsWith(sm_AnimDirName, StringComparison.Ordinal))
                {  // 如果Avatar Prefab上无SkinMeshRenderer，则添加之
                    SkinnedMeshRenderer smr = temp.GetComponent<SkinnedMeshRenderer>();
                    if (smr == null)
                    {
                        smr = temp.AddComponent<SkinnedMeshRenderer>();
                        smr.lightProbeUsage = LightProbeUsage.Off;
                        smr.reflectionProbeUsage = ReflectionProbeUsage.Off;
                        smr.shadowCastingMode = ShadowCastingMode.Off;
                        smr.sharedMaterials = new Material[0];
                    }
                }
            }

            temp.transform.position = Vector3.zero;
            AssetProxy assetProxy = temp.GetComponent<AssetProxy>();
            if (null == assetProxy)
                temp.AddComponent<AssetProxy>();
        }

        AssetDatabase.SaveAssets();
        PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
        GameObject.DestroyImmediate(temp);
    }

    [MenuItem("Assets/角色工具/25、标记骨骼挂点", false, 25)]
    private static void MarkDummyNodeGUI()
    {
        List<GameObject> selectObjs = _GetSelectAssetByType<GameObject>("t:prefab");
        int selectObjCount = selectObjs.Count;
        for (int i = 0; i < selectObjCount; i++)
        {
            EditorUtility.DisplayProgressBar("标记骨骼挂点", "正在标记第" + i + "个资源...", (i / selectObjCount));
            _MarkDummyNode(selectObjs[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    private static void _MarkDummyNode(GameObject data)
    {
        if (null == data)
            return;
        string path = AssetDatabase.GetAssetPath(data);
        if (path.Contains("_camera"))
            return;
        if (path.Contains("/Weapon/") || path.Contains("/WeaponShow/"))
            return;
        if (path.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
            return;
        //GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
        GameObject temp = Instantiate(data);
        if (null != temp)
        {
            GameObject boneAll = _FindSkeletonRoot(temp);
            Transform[] bones = boneAll.GetComponentsInChildren<Transform>();
            if (null != bones)
            {
                for (int j = 0, jcnt = bones.Length; j < jcnt; ++j)
                {
                    if (null == bones[j])
                        continue;

                    for (int k = 0, kcnt = DUMMY_LIST.Length; k < kcnt; ++k)
                    {
                        if (bones[j].gameObject.name.Equals(DUMMY_LIST[k], System.StringComparison.OrdinalIgnoreCase))
                        {
                            bones[j].gameObject.tag = "Dummy";
                            break;
                        }
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
        GameObject.DestroyImmediate(temp);
    }

    private static GameObject _GetAvatarPrefab(string partPath)
    {
        List<string> allAnimDirPath = GetAnimationDirPath(partPath);
        if (allAnimDirPath.Count <= 0)
            return null;
        string animDirPath = allAnimDirPath[0];
        string animDirParentPath = animDirPath.Substring(0, animDirPath.LastIndexOf('/'));
        string animDirParentName = animDirParentPath.Substring(animDirParentPath.LastIndexOf('/'));  // 带'/'
        string pathAvatar = allAnimDirPath[0] + animDirParentName +
                            "_Avatar.prefab";
        GameObject avatar = AssetDatabase.LoadAssetAtPath(pathAvatar, typeof(GameObject)) as GameObject;
        return avatar;
    }

    private static List<string> GetAnimationDirPath(string path)
    {
        path = path.Replace('\\', '/');
        string[] dicList = path.Split('/');

        List<string> allAnimDir = new List<string>();
        for (int i = dicList.Length - 1; i >= 0; --i)
        {
            string serchPath = "";
            int j = 0;
            while (j < i)
            {
                serchPath += dicList[j++] + '/';
            }
            string animDirPath = serchPath + sm_AnimDirName;
            if (Directory.Exists(animDirPath))
                allAnimDir.Add(animDirPath);
        }
        return allAnimDir;
    }

    private static void _GetNearestAnimationPath(string path, ref List<string> outFileList)
    {
        List<string> allAnimDirPath = GetAnimationDirPath(path);
        foreach (string dirPath in allAnimDirPath)
        {
            string[] animFile = Directory.GetFiles(dirPath);
            for (int m = 0, mcnt = animFile.Length; m < mcnt; ++m)
            {
                string ext = Path.GetExtension(animFile[m]);
                if (ext.Contains("meta")) continue;

                if (ext.Contains("fbx") || ext.Contains("FBX"))
                {
                    GameObject curFBX = AssetDatabase.LoadAssetAtPath(animFile[m], typeof(GameObject)) as GameObject;
                    if (null != curFBX)
                    {
                        outFileList.Add(animFile[m]);
                        //DestroyImmediate(curFBX);
                    }
                }

                if (ext.Contains("anim") || ext.Contains("ANIM"))
                {
                    AnimationClip curAnimClip = AssetDatabase.LoadAssetAtPath(animFile[m], typeof(AnimationClip)) as AnimationClip;
                    if (null != curAnimClip)
                    {
                        outFileList.Add(animFile[m]);
                    }
                }
            }
            if (outFileList.Count > 0)
                return;
        }
    }

    private static GameObject _FindSkeletonRoot(GameObject parent)
    {
        if (null != parent && parent.name.ToLower().Contains("boneall"))
            return parent;

        GameObject skeleton = null;
        int nChildNum = parent.transform.childCount;
        for (int j = 0; j < nChildNum; ++j)
        {
            GameObject child = parent.transform.GetChild(j).gameObject;
            skeleton = _FindSkeletonRoot(child);
            if (null != skeleton)
                return skeleton;
        }

        return null;
    }

    [MenuItem("Assets/角色工具/一键生成角色信息（主干）不兼容老资源", false, 1)]  // 新增工具，总入口。
    private static void _CreateAndSetActorWorkflow()
    {
        _CreateAllActorDirGUI();
        _CreateAvatarPrefabGUI();
        _RefreshAvatarPrefabGUI();
        _GenerateActorPrefabForThunkAndMarkDummyNodeGUI();
    }

    private static string sm_LimitSubPath = "Assets/Resources/_NEW_RESOURCES/Actor/";
    private static string sm_AnimDirName = "Animations";
    private static string sm_SkinDirName = "Skins";
    [MenuItem("Assets/角色工具/5、根据Skin创建默认目录", false, 05)]
    private static void _CreateAllActorDirGUI()
    {
        string[] selectIds = Selection.assetGUIDs;
        if (selectIds.Length == 0)
            return;
        string selectDirPath = AssetDatabase.GUIDToAssetPath(selectIds[0]);
        _CreateAllActorDir(selectDirPath);
    }

    private enum ActorTypeForCoker
    {
        Hero,
        Monster,
        NPC,
        Other,
        Pet,
        Weapon,
        Unknow
    }

    private static void _CreateAllActorDir(string selectDirPath)
    {  // 根据Skin信息创建所有文件夹
        if (string.IsNullOrEmpty(selectDirPath))
            return;
        if (!selectDirPath.StartsWith(sm_LimitSubPath, StringComparison.Ordinal))
            return;
        if (!Directory.Exists(selectDirPath))
            return;
        string skinDirPath = selectDirPath + "/" + sm_SkinDirName;
        if (!Directory.Exists(skinDirPath))
        {
            Debug.Log("*****提醒*****: 请选择" + sm_SkinDirName + "的上级目录");
            return;
        }
        ActorTypeForCoker actorType = ActorTypeForCoker.Unknow;
        string[] allTypes = Enum.GetNames(typeof(ActorTypeForCoker));
        for (int i = 0; i < allTypes.Length; i++)
        {
            string subPath = "/Actor/" + allTypes[i] + "/";
            if (selectDirPath.Contains(subPath))
                actorType = (ActorTypeForCoker)Enum.Parse(typeof(ActorTypeForCoker), allTypes[i]);
        }
        if (actorType == ActorTypeForCoker.Unknow)
        {
            Debug.LogError("*****错误*****, 选中目录非有效的Actor类型");
            return;
        }
        _CreateActorDirByActorType(actorType, selectDirPath);
    }

    private static void _CreateActorDirByActorType(ActorTypeForCoker actorType, string selectDirPath)
    {
        switch (actorType)
        {
            case ActorTypeForCoker.Hero:
                _CreateHeroDirs(selectDirPath);
                break;
            case ActorTypeForCoker.Monster:
                _CreateMonsterDirs(selectDirPath);
                break;
            case ActorTypeForCoker.NPC:
                _CreateNPCDirs(selectDirPath);
                break;
            case ActorTypeForCoker.Other:
                _CreateOtherDirBySkin(selectDirPath);
                break;
            case ActorTypeForCoker.Pet:
                _CreatePetDirBySkin(selectDirPath);
                break;
            case ActorTypeForCoker.Weapon:
                _CreateWeaponDirBySkin(selectDirPath);
                break;
            case ActorTypeForCoker.Unknow:
                break;
            default:
                break;
        }
    }

    private static void _CreateHeroDirs(string selectDirPath)
    {  // 命名规则: Skin_职业编号_时装编号[_身体部件]
        string[] objIds = AssetDatabase.FindAssets("t:gameobject", new[] { selectDirPath + "/" + sm_SkinDirName });
        foreach (string id in objIds)
        {
            string skinObjPath = AssetDatabase.GUIDToAssetPath(id);
            if (!skinObjPath.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
                continue;
            GameObject skinObj = AssetDatabase.LoadAssetAtPath<GameObject>(skinObjPath);
            if (skinObj == null)
                continue;
            string skinName = Path.GetFileNameWithoutExtension(skinObjPath);
            int skinNameLen = skinName.Length;
            int underScoreCount = 0;
            string partName = "";
            for (int i = 4; i < skinNameLen - 1; i++)
            {
                if (skinName[i] == '_')
                {
                    underScoreCount++;
                    if (underScoreCount == 3)
                        partName = skinName.Substring(i + 1);
                }
            }// for

            if (underScoreCount < 2)
            {
                Debug.LogError("*****错误*****, Skin" + skinObjPath + "Player名称小于三段, 正确命名示例: Skin_职业编号_时装编号[_身体部件]");
                return;
            }
            if (underScoreCount > 3)
            {
                Debug.Log("*****注意*****, Skin" + skinObjPath + "Player名称大于四段, 正确命名示例: Skin_职业编号_时装编号[_身体部件]");
            }

            if (string.Compare(partName, "", StringComparison.Ordinal) == 0)
            {  // 创建Actor目录及默认Prefab
                string prefabDirPath = selectDirPath + "/Prefabs/";
                _CreateDirectory(prefabDirPath);
                _CreateActorPrefabByGameObj(skinObj, prefabDirPath + skinName.Replace("Skin", "Model") + ".prefab");
            }
            else
            {  // 创建Actor部件目录及默认Prefab
                string needCreatePartDirPath = selectDirPath + "/" + partName;
                _CreateDirectory(needCreatePartDirPath + "/Material");
                string prefabDirPath = needCreatePartDirPath + "/Prefabs/";
                _CreateDirectory(prefabDirPath);
                _CreateActorPrefabByGameObj(skinObj, prefabDirPath + skinName.Replace("Skin", "Model") + ".prefab");
                _CreateDirectory(needCreatePartDirPath + "/Textures");
            }
        }// foreach
    }

    private static void _CreateMonsterDirs(string selectDirPath)
    {  // 命名规则: Skin_怪物信息或Skin_Arms_怪物信息
        _CreateDirectory(selectDirPath + "/Materials");
        string prefabDirPath = selectDirPath + "/Prefabs/";
        _CreateDirectory(prefabDirPath);
        _CreateDirectory(selectDirPath + "/Textures");
        string[] objIds = AssetDatabase.FindAssets("t:gameobject", new[] { selectDirPath + "/" + sm_SkinDirName });
        foreach (string id in objIds)
        {
            string skinObjPath = AssetDatabase.GUIDToAssetPath(id);
            if (!skinObjPath.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
                continue;
            GameObject skinObj = AssetDatabase.LoadAssetAtPath<GameObject>(skinObjPath);
            if (skinObj == null)
                continue;
            string skinName = Path.GetFileNameWithoutExtension(skinObjPath);
            string prefabPath = "";
            if (skinName.StartsWith("Skin_Arms_"))
                prefabPath = prefabDirPath + skinName.Replace("Skin_Arms_", "Arms") + ".prefab";
            else
                prefabPath = prefabDirPath + skinName.Replace("Skin", "Model") + ".prefab";
            _CreateActorPrefabByGameObj(skinObj, prefabPath);
        }
    }

    private static void _CreateNPCDirs(string selectDirPath)
    {  // 命名规则: Skin_NPC_编号
        _CreateDirectory(selectDirPath + "/Animations");
        _CreateDirectory(selectDirPath + "/Materials");
        string prefabDirPath = selectDirPath + "/Prefabs/";
        _CreateDirectory(prefabDirPath);
        _CreateDirectory(selectDirPath + "/Textures");
        string[] objIds = AssetDatabase.FindAssets("t:gameobject", new[] { selectDirPath + "/" + sm_SkinDirName });
        foreach (string id in objIds)
        {
            string skinObjPath = AssetDatabase.GUIDToAssetPath(id);
            if (!skinObjPath.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
                continue;
            GameObject skinObj = AssetDatabase.LoadAssetAtPath<GameObject>(skinObjPath);
            if (skinObj == null)
                continue;
            string skinName = Path.GetFileNameWithoutExtension(skinObjPath);
            string prefabPath = prefabDirPath + skinName.Replace("Skin", "Model") + ".prefab";
            _CreateActorPrefabByGameObj(skinObj, prefabPath);
        }
    }

    private static void _CreateOtherDirBySkin(string selectDirPath)
    {
    }

    private static void _CreatePetDirBySkin(string selectDirPath)
    {
    }

    private static void _CreateWeaponDirBySkin(string selectDirPath)
    {
    }

    private static void _CreateDirectory(string needCreateDirPath)
    {
        if (Directory.Exists(needCreateDirPath))
            return;
        Directory.CreateDirectory(needCreateDirPath);
        AssetDatabase.Refresh();
    }

    private static void _CreateActorPrefabByGameObj(GameObject gameObj, string prefabPath)
    {
        if (File.Exists(prefabPath))
            return;
        //GameObject insPrefab = PrefabUtility.InstantiatePrefab(gameObj) as GameObject;  // 角色工具美术不让使用嵌套预制体
        GameObject insPrefab = Instantiate(gameObj);
        PrefabUtility.SaveAsPrefabAsset(insPrefab, prefabPath);
        AssetDatabase.Refresh();
        DestroyImmediate(insPrefab);
    }

    [MenuItem("Assets/角色工具/10、创建Avatar Prefab", false, 10)]
    private static void _CreateAvatarPrefabGUI()
    {
        string[] selectIds = Selection.assetGUIDs;
        if (selectIds.Length == 0)
            return;
        string selectDirPath = AssetDatabase.GUIDToAssetPath(selectIds[0]);
        _CreateAvatarPrefab(selectDirPath);
    }

    private static void _CreateAvatarPrefab(string selectDirPath)
    {  // 根据Skin信息创建所有文件夹
        if (string.IsNullOrEmpty(selectDirPath))
            return;
        if (!selectDirPath.StartsWith(sm_LimitSubPath, StringComparison.Ordinal))
            return;
        if (!Directory.Exists(selectDirPath))
            return;
        if (!selectDirPath.Contains("/Actor/Hero/"))  // 仅Actor目录下需要创建Avatar
            return;
        while (!Directory.Exists(selectDirPath + "/Animations"))
        {
            if (selectDirPath == sm_LimitSubPath)
                return;
            selectDirPath = selectDirPath.Substring(0, selectDirPath.LastIndexOf('/'));
        }
        string animDirPath = selectDirPath + "/Animations";
        if (!Directory.Exists(animDirPath))
            return;
        string selectDirName = selectDirPath.Substring(selectDirPath.LastIndexOf('/') + 1);
        string avatarPrefabPath = animDirPath + "/" + selectDirName + "_Avatar.prefab";
        if (File.Exists(avatarPrefabPath))
            return;

        string[] childDirPathAry = Directory.GetDirectories(selectDirPath);
        int childDirCount = childDirPathAry.Length;
        for (int i = 0; i < childDirCount; i++)
        {
            string childDirPath = childDirPathAry[i];
            if (childDirPath.EndsWith(sm_AnimDirName, StringComparison.Ordinal))
                continue;
            childDirPath = childDirPath.Replace('\\', '/');
            //string childDirName = Path.GetDirectoryName(childDirPath);
            string childDirName = childDirPath.Substring(childDirPath.LastIndexOf('/') + 1);
            string actorPrefabName = "Model" + childDirName.Substring(childDirName.IndexOf('_'));
            string actorPrefabPath = childDirPath + "/Prefabs/" + actorPrefabName + ".prefab";
            if (!File.Exists(actorPrefabPath))
                continue;
            GameObject objActorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(actorPrefabPath);
            if (objActorPrefab == null)
                continue;
            _CreateActorPrefabByGameObj(objActorPrefab, avatarPrefabPath);
            break;
        }

        if (File.Exists(avatarPrefabPath))
        {
            GameObject avatarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(avatarPrefabPath);
            GameObject insAvatarPrefab = Instantiate(avatarPrefab);
            int childCount = insAvatarPrefab.transform.childCount;
            if (childCount > 1)
            {
                List<GameObject> needDestroyTrans = new List<GameObject>(childCount - 1);
                for (int i = 0; i < childCount; i++)
                {
                    Transform transChild = insAvatarPrefab.transform.GetChild(i);
                    string childName = transChild.name;
                    if (!childName.EndsWith("BoneAll", StringComparison.OrdinalIgnoreCase))
                        needDestroyTrans.Add(transChild.gameObject);
                }
                //PrefabUtility.UnpackPrefabInstance(insAvatarPrefab, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                for (int i = 0; i < childCount - 1; i++)
                {
                    DestroyImmediate(needDestroyTrans[i]);
                }
                PrefabUtility.SaveAsPrefabAsset(insAvatarPrefab, avatarPrefabPath);
                AssetDatabase.Refresh();
                DestroyImmediate(insAvatarPrefab);
            }
        }
    }

    [MenuItem("Assets/角色工具/15、刷新Avatar Prefab动画", false, 15)]
    private static void _RefreshAvatarPrefabGUI()
    {
        string[] selectIds = Selection.assetGUIDs;
        if (selectIds.Length == 0)
            return;
        string selectDirPath = AssetDatabase.GUIDToAssetPath(selectIds[0]);
        _RefreshAvatarPrefab(selectDirPath);
    }

    private static void _RefreshAvatarPrefab(string selectDirPath)
    {  // 根据Skin信息创建所有文件夹
        if (string.IsNullOrEmpty(selectDirPath))
            return;
        if (!selectDirPath.StartsWith(sm_LimitSubPath, StringComparison.Ordinal))
            return;
        if (!Directory.Exists(selectDirPath))
            return;
        while (!Directory.Exists(selectDirPath + "/Animations"))
        {
            if (selectDirPath == sm_LimitSubPath)
                return;
            selectDirPath = selectDirPath.Substring(0, selectDirPath.LastIndexOf('/'));
        }
        string animDirPath = selectDirPath + "/Animations";
        if (!Directory.Exists(animDirPath))
            return;
        string selectDirName = selectDirPath.Substring(selectDirPath.LastIndexOf('/') + 1);
        string avatarPrefabPath = animDirPath + "/" + selectDirName + "_Avatar.prefab";
        if (!File.Exists(avatarPrefabPath))
            return;

        GameObject avatarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(avatarPrefabPath);
        _GenerateActorPrefabForThunk(avatarPrefab);
        _MarkDummyNode(avatarPrefab);
    }
}
