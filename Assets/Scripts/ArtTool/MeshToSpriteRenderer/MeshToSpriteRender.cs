#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MeshToSpriteRender
{
    private static Color m_WithoutAlphaColor = new Color(0, 0, 0, 0);
    private static List<GameObject> m_NoMainTexObjs = new List<GameObject>();
    private static List<string> m_WriteReadTexPath = new List<string>();
    private static List<string> m_AllChildNames = new List<string>();
    private static List<Material> m_UsedMats = new List<Material>();

    public MeshRenderer Mr { get; }
    public MeshFilter Mf { get; }
    public Sprite MainSprite { get; private set; }
    public Material NewMat { get; private set; }
    public float UScale { get; private set; }
    public float VScale { get; private set; }
    public bool FlipX { get; private set; }
    public bool FlipY { get; private set; }
    public Vector3 PosOffset { get; private set; }
    public Vector3 Scale { get; private set; }
    public bool YEffective { get; private set; }

    private int m_TexCount = 0;
    private bool m_XEffective;
    private Mesh m_Mesh;
    private Material m_OldMat;
    private Vector2[] m_Uvs;
    private float m_MinU, m_MaxU, m_MinV, m_MaxV;
    private float m_UOffset, m_VOffset;
    private bool m_SplitTexFlipX;  // 生成的Tex翻转X轴
    private bool m_SplitTexFlipY;  // 生成的Tex翻转Y轴

    public MeshToSpriteRender(MeshRenderer mr, bool splitTexFlipX, bool splitTexFlipY)
    {
        Mr = mr;
        m_SplitTexFlipX = splitTexFlipX;
        m_SplitTexFlipY = splitTexFlipY;
        Mf = mr.GetComponent<MeshFilter>();
        if (Mf == null)
        {
            Debug.Log(mr.name + "无Mesh Filter组件", mr.gameObject);
            return;
        }
        m_Mesh = Mf.sharedMesh;
    }

    public void SplitAtlas()
    {
        if (Mr == null || Mf == null)
            return;
        if (Mr.sharedMaterials.Length > 1)
        {
            Debug.LogError(Mr.name + "含多个材质请自行处理或将其修改为单个材质", Mr.gameObject);
            return;
        }
        m_OldMat = Mr.sharedMaterial;
        if (m_OldMat == null)
            return;
        string matPath = AssetDatabase.GetAssetPath(m_OldMat);
        Dictionary<string, Texture2D> texNameToData = GetTexNameToDataMapByMaterial(m_OldMat);
        //return;
        NewMat = new Material(m_OldMat.shader);
        NewMat.CopyPropertiesFromMaterial(m_OldMat);
        if (texNameToData.Count == 0)
            return;
        // 初始化Mat属性
        MatNormalized(NewMat, texNameToData);
        AnalysisMesh();

        int hashCode = 0;
        string firstTexPath = ""; // 目前发现部分材质不包含_MainTex，暂时记录第一张贴图作为sprite图片
        string renderPrefabName = GetNewTexName();
        if (renderPrefabName == null)
            Debug.Log(Mr.name + "未能匹配到名称，请确认是否存在问题");
        bool hasMainTex = false;
        foreach (string key in texNameToData.Keys)
        {
            if (key == "_MainTex")
            {
                hasMainTex = true;
                break;
            }
        }
        if (!hasMainTex)
            m_NoMainTexObjs.Add(Mr.gameObject);
        foreach (KeyValuePair<string, Texture2D> keyValuePair in texNameToData)
        {
            m_TexCount++;
            Texture2D tex2D = keyValuePair.Value;
            string texPath = AssetDatabase.GetAssetPath(tex2D);
            int texPointIndex = texPath.LastIndexOf('.');
            if (texPointIndex < 0)
                continue;
            int splitTexW = (int)((m_MaxU - m_MinU) * tex2D.width);
            int splitTexH = (int)((m_MaxV - m_MinV) * tex2D.height);
            //Scale = _mesh.bounds.extents * 2;
            int xStart = (int)(m_MinU * tex2D.width);
            int yStart = (int)(m_MinV * tex2D.height);
            hashCode = (tex2D.name + xStart.ToString() + splitTexW.ToString() + yStart.ToString() +
                 splitTexH.ToString()).GetHashCode();
            string newTexPath;
            int texSlashIndex = texPath.LastIndexOf('/');
            string texDirPath = texPath.Substring(0, texSlashIndex) + "/NewTex";
            if (!Directory.Exists(texDirPath))
                Directory.CreateDirectory(texDirPath);
            if (renderPrefabName == null)
                newTexPath = texDirPath + texPath.Substring(texSlashIndex, texPointIndex - texSlashIndex) + "_" + hashCode.ToString() + ".png";
            else
            {
                if (m_TexCount > 1)
                    newTexPath = texDirPath + "/T_" + renderPrefabName + "_" + m_TexCount.ToString() + ".png";
                else
                    newTexPath = texDirPath + "/T_" + renderPrefabName + ".png";
            }
            string newTexFullPath = Application.dataPath + newTexPath.Substring(newTexPath.IndexOf('/'));
            bool isMainTex = keyValuePair.Key == "_MainTex";
            bool isAlbedoTex = keyValuePair.Key == "_Albedo";
            bool isLutTexture = keyValuePair.Key == "_LutTexture";
            isMainTex = isMainTex || !hasMainTex && isAlbedoTex || !hasMainTex && isLutTexture;
            if (isMainTex)
            {
                Scale = new Vector3(m_XEffective ? Scale.x * 100 / splitTexW / UScale : Scale.y * 100 / splitTexW / UScale, m_XEffective && YEffective ? Scale.y * 100 / splitTexH / VScale : Scale.z * 100 / splitTexH / VScale, 1);
            }
            if (tex2D.name == "tx_texture_0026" || tex2D.name == "T_35qh_lut01" || tex2D.name == "T_chapter6_h_lut01" || tex2D.name == "T_chapter6_h_lut02")  // 这些图不需要拆分
            {
                SplitOneTexEnd(keyValuePair.Key, texPath, NewMat, isMainTex);
                continue;
            }
            if (File.Exists(newTexFullPath))
            {
                SplitOneTexEnd(keyValuePair.Key, newTexPath, NewMat, isMainTex);
                continue;
            }
            Texture2D splitTex = new Texture2D(splitTexW, splitTexH, TextureFormat.RGBA32, false);
            Vector2[,] triangles = GetAllTriangles(m_Uvs, m_Mesh, new Vector2(tex2D.width, tex2D.height));

            AssetImporter aiOld = AssetImporter.GetAtPath(texPath);
            TextureImporter tiOld = aiOld as TextureImporter;
            if (!tiOld.isReadable)
            {
                tiOld.isReadable = true;
                AssetDatabase.ImportAsset(texPath);
                m_WriteReadTexPath.Add(texPath);
            }

            for (int j = 0; j < splitTexH; j++)
            {
                for (int k = 0; k < splitTexW; k++)
                {
                    bool result = IsPointInTrianglesVec2(triangles, new Vector2(xStart + k, yStart + j));
                    //bool result = true;
                    Color col = result ? tex2D.GetPixel(xStart + k, yStart + j) : m_WithoutAlphaColor;
                    splitTex.SetPixel(m_SplitTexFlipX ? splitTexW - 1 - k : k, m_SplitTexFlipY ? splitTexH - 1 - j : j, col);
                }
            }
            OutputTex(splitTex, newTexFullPath);
            AssetDatabase.Refresh();
            if (isMainTex)
            {
                AssetImporter ai = AssetImporter.GetAtPath(newTexPath);
                if (ai == null)
                    continue;
                TextureImporter ti = ai as TextureImporter;
                ti.textureType = TextureImporterType.Sprite;
                ti.mipmapEnabled = false;
                AssetDatabase.ImportAsset(newTexPath);
                AssetDatabase.Refresh();
            }
            SplitOneTexEnd(keyValuePair.Key, newTexPath, NewMat, isMainTex);
        }
        // save mat
        //材质去重
        //int matCount;
        //int useMatCount = m_UsedMats.Count;
        //Material sameMat = null;
        //for (matCount = 0; matCount < useMatCount; matCount++)
        //{
        //    if (MeshToSpriteRenderToolEditor.IsSameMaterial(NewMat, m_UsedMats[matCount]))
        //    {
        //        sameMat = m_UsedMats[matCount];
        //        break;
        //    }
        //}
        //if (sameMat == null)
        //    m_UsedMats.Add(NewMat);
        string newMatPath;
        int matSlashIndex = matPath.LastIndexOf('/');
        int matPointIndex = matPath.LastIndexOf('.');
        string matDirPath = matPath.Substring(0, matSlashIndex) + "/NewMat";
        if (!Directory.Exists(matDirPath))
            Directory.CreateDirectory(matDirPath);
        if (renderPrefabName == null)
            newMatPath = matDirPath + matPath.Substring(matSlashIndex, matPointIndex - matSlashIndex) + "_" + hashCode.ToString() + ".mat";
        else
            newMatPath = matDirPath + "/M_" + renderPrefabName + ".mat";
        string newMatFullPath = Application.dataPath + newMatPath.Substring(newMatPath.IndexOf('/'));
        if (!File.Exists(newMatFullPath))
        {
            AssetDatabase.CreateAsset(NewMat, newMatPath);
        }
        else
        {
            NewMat = AssetDatabase.LoadAssetAtPath<Material>(newMatPath);
        }
    }

    private void MatNormalized(Material newMat, Dictionary<string, Texture2D> data)
    {
        foreach (string texName in data.Keys)
        {
            NewMat.SetTexture(texName, null);
        }

        newMat.mainTextureOffset = Vector2.zero;
        newMat.mainTextureScale = Vector2.one;
    }

    private void SplitOneTexEnd(string texNameAtMat, string assetPath, Material newMat, bool isMainTex = false)
    {
        if (isMainTex)
        {
            MainSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }
        Texture2D splitTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        newMat.SetTexture(texNameAtMat, splitTex);
    }

    private void AnalysisMesh()
    {
        m_Uvs = new Vector2[m_Mesh.uv.Length];
        CopyUv(m_Mesh.uv, m_Uvs);
        int uvCount = m_Uvs.Length;
        m_MinU = m_Uvs[0].x; m_MaxU = m_Uvs[0].x; m_MinV = m_Uvs[0].y; m_MaxV = m_Uvs[0].y;
        m_XEffective = m_Mesh.bounds.size.x > 0.0001f;
        YEffective = m_Mesh.bounds.size.y > 0.0001f;
        float minUPosX = m_XEffective ? m_Mesh.vertices[0].x : m_Mesh.vertices[0].y;
        float maxUPosX = m_XEffective ? m_Mesh.vertices[0].x : m_Mesh.vertices[0].y;
        float minVPosY = m_XEffective && YEffective ? m_Mesh.vertices[0].y : m_Mesh.vertices[0].z;
        float maxVPosY = m_XEffective && YEffective ? m_Mesh.vertices[0].y : m_Mesh.vertices[0].z;
        float uPosOffset;
        float vPosOffset;
        for (int i = 1; i < uvCount; i++)
        {
            Vector2 uv = m_Uvs[i];
            if (uv.x > m_MaxU)
            { m_MaxU = uv.x; maxUPosX = m_XEffective ? m_Mesh.vertices[i].x : m_Mesh.vertices[i].y; }
            if (uv.x < m_MinU)
            { m_MinU = uv.x; minUPosX = m_XEffective ? m_Mesh.vertices[i].x : m_Mesh.vertices[i].y; }
            if (uv.y > m_MaxV)
            { m_MaxV = uv.y; maxVPosY = m_XEffective && YEffective ? m_Mesh.vertices[i].y : m_Mesh.vertices[i].z; }
            if (uv.y < m_MinV)
            { m_MinV = uv.y; minVPosY = m_XEffective && YEffective ? m_Mesh.vertices[i].y : m_Mesh.vertices[i].z; }
        }
        FlipX = minUPosX > maxUPosX;
        FlipY = minVPosY > maxVPosY;
        if (m_SplitTexFlipX)
            FlipX = !FlipX;
        if (m_SplitTexFlipY)
            FlipY = !FlipY;
        // 将Uv修正到 0~1
        UScale = m_MaxU - m_MinU;
        VScale = m_MaxV - m_MinV;
        if (UScale >= 1)
        {
            m_MaxU = 1;
            m_MinU = 0;
            m_UOffset = 0;  // 此类不修正
        }
        else
        {
            UScale = 1;
            if (m_MinU < 0)
                m_UOffset = m_MinU;
            if (m_MaxU > 1)
                m_UOffset = m_MaxU - 1;
            m_MinU = m_MinU - m_UOffset;
            m_MaxU = m_MaxU - m_UOffset;
        }
        if (VScale >= 1)
        {
            m_MaxV = 1;
            m_MinV = 1;
            m_VOffset = 0;  // 此类不修正
        }
        else
        {
            VScale = 1;
            if (m_MinV < 0)
                m_VOffset = m_MinV;
            if (m_MaxV > 1)
                m_VOffset = m_MaxV - 1;
            m_MinV = m_MinV - m_VOffset;
            m_MaxV = m_MaxV - m_VOffset;
        }
        for (int i = 0; i < uvCount; i++)
        {
            m_Uvs[i] = new Vector2(m_Uvs[i].x - m_UOffset, m_Uvs[i].y - m_VOffset);
        }
        uPosOffset = m_UOffset * m_Mesh.bounds.size.x;
        vPosOffset = m_VOffset * m_Mesh.bounds.size.y;
        CalculatePosOffset(uPosOffset, vPosOffset);
    }

    private void CalculatePosOffset(float uPosOffset, float vPosOffset)
    {
        Transform transRender = Mr.transform;  // 计算旋转对中心的偏移的影响，后续可改为矩阵
        float centroidOffsetX = Mf.sharedMesh.bounds.center.x * transRender.lossyScale.x;
        float centroidOffsetY = Mf.sharedMesh.bounds.center.y * transRender.lossyScale.y;
        float centroidOffsetZ = Mf.sharedMesh.bounds.center.z * transRender.lossyScale.z;

        Matrix4x4 matrixR = Matrix4x4.Rotate(Mr.transform.localRotation);
        float xRotateChange = centroidOffsetX * matrixR.m00 + centroidOffsetY * matrixR.m01 + centroidOffsetZ * matrixR.m02;
        float yRotateChange = centroidOffsetX * matrixR.m10 + centroidOffsetY * matrixR.m11 + centroidOffsetZ * matrixR.m12;
        float zRotateChange = centroidOffsetX * matrixR.m20 + centroidOffsetY * matrixR.m21 + centroidOffsetZ * matrixR.m22;

        Scale = new Vector3(m_Mesh.bounds.extents.x * 2, m_Mesh.bounds.extents.y * 2, m_Mesh.bounds.extents.z * 2);
        PosOffset = new Vector3(uPosOffset + xRotateChange, vPosOffset + yRotateChange, zRotateChange);
    }

    public static void InitLogAbout()
    {
        //_flipXyObjs.Clear();
        m_NoMainTexObjs.Clear();
        //_matNameToObjs.Clear();
        m_WriteReadTexPath.Clear();
        m_AllChildNames.Clear();
    }
    public static void OutputLogAbout()
    {
        if (m_NoMainTexObjs.Count > 0)
        {
            Debug.Log("输出无MainTex的Mesh Renderer");
            foreach (GameObject obj in m_NoMainTexObjs)
            {
                Debug.Log(obj.name, obj);
            }
        }
        foreach (string texPath in m_WriteReadTexPath)
        {
            AssetImporter aiOld = AssetImporter.GetAtPath(texPath);
            TextureImporter tiOld = aiOld as TextureImporter;
            if (tiOld.isReadable)
            {
                tiOld.isReadable = false;
                AssetDatabase.ImportAsset(texPath);
            }
        }
    }

    private static void CopyUv(Vector2[] source, Vector2[] target)
    {
        int uvCount = source.Length;
        for (int i = 0; i < uvCount; i++)
        {
            target[i] = source[i];
        }
    }
    private static Vector2[,] GetAllTriangles(Vector2[] uvs, Mesh mesh, Vector2 size)
    {
        int triangleCount = mesh.triangles.Length / 3;
        Vector2[,] triangles = new Vector2[triangleCount, 3];
        for (int i = 0; i < triangleCount; i++)
        {
            triangles[i, 0] = new Vector2(uvs[mesh.triangles[i * 3]].x * size.x, uvs[mesh.triangles[i * 3]].y * size.y);
            triangles[i, 1] = new Vector2(uvs[mesh.triangles[i * 3 + 1]].x * size.x, uvs[mesh.triangles[i * 3 + 1]].y * size.y);
            triangles[i, 2] = new Vector2(uvs[mesh.triangles[i * 3 + 2]].x * size.x, uvs[mesh.triangles[i * 3 + 2]].y * size.y);
        }
        return triangles;
    }
    private static void OutputTex(Texture2D tex, string savePath)
    {
        byte[] dataBytes = tex.EncodeToPNG();
        using (FileStream fs = File.Open(savePath, FileMode.OpenOrCreate))
        {
            fs.Write(dataBytes, 0, dataBytes.Length);
        }
    }
    private static bool IsPointInTrianglesVec2(Vector2[,] triangles, Vector2 point)
    {
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            if (IsPointInTriangle2D(triangles[i, 0], triangles[i, 1], triangles[i, 2], point))
                return true;
        }
        return false;
    }
    private static bool IsPointInTriangle2D(Vector2 vec2A, Vector2 vec2B, Vector2 vec2C, Vector2 vec22Point)
    {
        return SameSideVec2(vec2A, vec2B, vec2C, vec22Point) &&
               SameSideVec2(vec2B, vec2C, vec2A, vec22Point) &&
               SameSideVec2(vec2C, vec2A, vec2B, vec22Point);
    }
    private static bool SameSideVec2(Vector2 vec2A, Vector2 vec2B, Vector2 vec2C, Vector2 vec22Point)
    {
        Vector2 vec2Ab = vec2B - vec2A;
        Vector2 vec2Ac = vec2C - vec2A;
        Vector2 vec2Ap = vec22Point - vec2A;
        float result1 = Vec2Cross(vec2Ab, vec2Ac);
        float result2 = Vec2Cross(vec2Ab, vec2Ap);
        return result1 * result2 >= 0;
    }
    private static float Vec2Cross(Vector2 vec1, Vector2 vec2)
    {
        // 23 - 32; 31 - 13; 12 - 21;  三维
        // 12 - 21;  二维
        return vec1.x * vec2.y - vec1.y * vec2.x;
    }
    private static Dictionary<string, Texture2D> GetTexNameToDataMapByMaterial(Material mat)
    {
        if (mat == null)
            return null;
        Dictionary<string, Texture2D> texNameToDataMap = new Dictionary<string, Texture2D>();
        SerializedObject so = new SerializedObject(mat);
        SerializedProperty sp = so.GetIterator();
        while (sp.Next(true))
        {
            if (sp.isArray && sp.name == "m_TexEnvs")
            {
                int arrCount = sp.arraySize;
                for (int i = 0; i < arrCount; i++)
                {
                    SerializedProperty texProperty = sp.GetArrayElementAtIndex(i);
                    if (texProperty == null)
                        continue;
                    SerializedProperty texNameProperty = texProperty.FindPropertyRelative("first");
                    if (texNameProperty == null)
                        continue;
                    string texName = texNameProperty.stringValue;
                    if (mat.HasProperty(texName))
                    {
                        Texture tex = mat.GetTexture(texName);
                        Texture2D tex2D = tex as Texture2D;
                        if (tex2D == null)
                            continue;
                        texNameToDataMap.Add(texName, tex2D);
                    }
                }
                break;
            }
        }
        return texNameToDataMap;
    }

    private string GetNewTexName()
    {
        // Renderer Name
        string renderName = Mr.name;
        if (renderName.EndsWith("(Clone)"))
            renderName = renderName.Substring(0, renderName.Length - 7);
        // 规范化Renderer Name
        int index = renderName.IndexOf(' ');
        if (index > 0)
            renderName = renderName.Substring(0, index);
        // 第一次获取所有名称
        if (m_AllChildNames.Count == 0)
        {
            string rootPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Mr.gameObject);
            if (string.IsNullOrEmpty(rootPath))
            {
                if (renderName.StartsWith("P_"))
                    return renderName.Substring(2);
                else
                    return null;
            }
            // ……/Chapter3_Dungeons/scenes/…….prefab为例
            rootPath = rootPath.Substring(0, rootPath.LastIndexOf('/'));
            if (!rootPath.EndsWith("_Perfab"))
            {
                // 找到Chapter3_
                rootPath = rootPath.Substring(0, rootPath.LastIndexOf('_') + 1);
                // 后缀+Prefab
                rootPath += "Perfab";
            }
            // 获取目录下所有prefab路径
            string[] searchFolder = { rootPath };
            string[] allAssetsGuid = AssetDatabase.FindAssets("t:prefab", searchFolder);  // 5.6版本需去重
            int guidCount = allAssetsGuid.Length;
            for (int i = 0; i < guidCount; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
                int pointIndex = assetPath.LastIndexOf('.');
                int slashIndex = assetPath.LastIndexOf('/');
                m_AllChildNames.Add(assetPath.Substring(slashIndex + 1, pointIndex - slashIndex - 1));
            }
        }
        // 匹配
        int childCount = m_AllChildNames.Count;
        for (int i = 0; i < childCount; i++)
        {
            string assetName = m_AllChildNames[i];
            if (assetName == renderName)
            {
                // 以P_开头
                return assetName.Substring(2);
            }
        }
        return null;
    }
}
#endif