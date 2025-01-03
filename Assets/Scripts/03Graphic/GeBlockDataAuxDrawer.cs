#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.ComponentModel;
using Protocol;
using behaviac;

public class GeBlockDataAuxDrawer : MonoBehaviour
{
    protected Material blockMaterial;
    protected Mesh blockMesh;
    protected Texture2D blockTexture;
    protected Material grassMaterial;
    protected Mesh grassMesh;
    protected Texture2D grassTexture;
    protected Material ecosystemMaterial;
    protected Mesh ecosystemMesh;
    protected Texture2D ecosystemTexture;
    protected Material eventAreaMaterial;
    protected Mesh eventAreaMesh;
    protected Texture2D eventAreaTexture;
    protected Vector3[] gridsv;

    public SceneEditorMode editmode;
    public bool showblock = true;
    public bool showecosystem = false;
    public bool showgrass = false;
    public bool showeventarea = true;
    public bool showgrid = false;
    public bool showmonster = true;
    public bool showentity = true;
    public bool showregion = true;
    public bool showtransport = false;

    public int brushSize = 1;

    public GameObject dragObject;

    public Vector2 m_GridSize = Vector2.zero;
    public VInt2 m_LogicMin = VInt2.zero;
    public VInt2 m_LogicMax = VInt2.zero;
    public Vector3 m_SceneLogicPos = Vector2.zero;
    public byte[] m_BlockData = null;
    public ushort[] m_GrassData = null;
    public ushort[] m_EcosystemData = null;
    public byte[] m_EventAreaData = null;
    private List<BeRegionBase> m_doorData = null;


    public int logicLength
    {
        get { return m_LogicMax.x - m_LogicMin.x; }
    }
    public int logicWidth
    {
        get { return m_LogicMax.y - m_LogicMin.y; }
    }

    public void DestroyObjects<T>(ref T obj) where T : UnityEngine.Object
    {
        if (obj == null) return;
        Editor.DestroyImmediate(obj);
        obj = null;
    }

    private bool _IsStarted = false;

    void Start()
    {
        _IsStarted = true;

#if UNITY_EDITOR
        _GetKey("showblock", ref showblock);// = true;
        _GetKey("showecosystem", ref showecosystem);// = false;
        _GetKey("showgrass", ref showgrass);// = false;
        _GetKey("showeventarea", ref showeventarea);// = true;
        _GetKey("showgrid", ref showgrid);// = false;
        _GetKey("showmonster", ref showmonster);// = true;
        _GetKey("showentity", ref showentity);// = true;
        _GetKey("showregion", ref showregion);// = true;
        _GetKey("showtransport", ref showtransport);// = false;
        _GetKey("editmode", ref editmode);// this.value = (SceneEditorMode))
#endif
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!_IsStarted)
        {
            return;
        }

        _SetKey("showblock", showblock);// = true;
        _SetKey("showecosystem", showecosystem);// = false;
        _SetKey("showgrass", showgrass);// = false;
        _SetKey("showgrass", showgrass);// = true;
        _SetKey("showgrid", showgrid);// = false;
        _SetKey("showmonster", showmonster);// = true;
        _SetKey("showentity", showentity);// = true;
        _SetKey("showregion", showregion);// = true;
        _SetKey("showtransport", showtransport);// = false;
        _SetKey("editmode", editmode);// this.value = (SceneEditorMode))
#endif
    }

    private void _GetKey(string key, ref SceneEditorMode value)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogFormat("Get key {0} = {1}", key, value);
        value = (SceneEditorMode)EditorPrefs.GetInt("AuxDrawer."+key, (int)value);
#endif
    }
    private void _SetKey(string key, SceneEditorMode value)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogFormat("Set key {0} = {1}", key, value);
        EditorPrefs.SetInt("AuxDrawer."+key, (int)value);
#endif
    }

    private void _GetKey(string key, ref bool value)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogFormat("Get key {0} = {1}", key, value);
        value = EditorPrefs.GetBool("AuxDrawer."+key, value);
#endif
    }

    private void _SetKey(string key, bool value)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogFormat("Set key {0} = {1}", key, value);
        EditorPrefs.SetBool("AuxDrawer."+key, value);
#endif
    }

    void OnDestroy()
    {
        DestroyObjects(ref blockMaterial);
        DestroyObjects(ref blockMesh);
        DestroyObjects(ref blockTexture);
        DestroyObjects(ref grassMaterial);
        DestroyObjects(ref grassMesh);
        DestroyObjects(ref grassTexture);
        DestroyObjects(ref ecosystemMaterial);
        DestroyObjects(ref ecosystemMesh);
        DestroyObjects(ref ecosystemTexture);
        DestroyObjects(ref eventAreaMaterial);
        DestroyObjects(ref eventAreaMesh);
        DestroyObjects(ref eventAreaTexture);
    }
    public void SetDoorData(List<BeRegionBase> doordata)
    {
        m_doorData = doordata;
    }

    public void RefreshBlockData(ISceneData sceneData,byte[] blockData)
    {
        m_GridSize = sceneData.GetGridSize();

        m_LogicMin.x = sceneData.GetLogicXmin();
        m_LogicMax.x = sceneData.GetLogicXmax();

        m_LogicMin.y = sceneData.GetLogicZmin();
        m_LogicMax.y = sceneData.GetLogicZmax();

        m_SceneLogicPos = sceneData.GetLogicPos();

        if (blockData.Length == logicLength * logicWidth)
        {
            m_BlockData = blockData;
            UpdateBlockLayer();
        }
        else
        {
            m_BlockData = null;
            Logger.LogError("Block data missing match with scene data!");
        }
    }
    public void RefreshEventAreaData(ISceneData sceneData, byte[] eventAreaData)
    {
        if (eventAreaData == null)
        {
            return;
        }

        m_GridSize = sceneData.GetGridSize();

        m_LogicMin.x = sceneData.GetLogicXmin();
        m_LogicMax.x = sceneData.GetLogicXmax();

        m_LogicMin.y = sceneData.GetLogicZmin();
        m_LogicMax.y = sceneData.GetLogicZmax();

        m_SceneLogicPos = sceneData.GetLogicPos();

        if (eventAreaData.Length == logicLength * logicWidth)
        {
            m_EventAreaData = eventAreaData;
            UpdateEventAreaLayer();
        }
        else
        {
            m_EventAreaData = null;
        }
    }
    public void RefreshGrassData(ISceneData sceneData, ushort[] grassData)
    {
        if (grassData == null) return;
        m_GridSize = sceneData.GetGridSize();

        m_LogicMin.x = sceneData.GetLogicXmin();
        m_LogicMax.x = sceneData.GetLogicXmax();

        m_LogicMin.y = sceneData.GetLogicZmin();
        m_LogicMax.y = sceneData.GetLogicZmax();

        m_SceneLogicPos = sceneData.GetLogicPos();

        if (grassData.Length == logicLength * logicWidth)
        {
            m_GrassData = grassData;
            UpdateGrassData();
        }
        else
        {
            m_GrassData = null;
        }
    }
    public void RefreshEcosystemData(ISceneData sceneData, ushort[] ecosystemData)
    {
        if (ecosystemData == null) return;
        m_GridSize = sceneData.GetGridSize();

        m_LogicMin.x = sceneData.GetLogicXmin();
        m_LogicMax.x = sceneData.GetLogicXmax();

        m_LogicMin.y = sceneData.GetLogicZmin();
        m_LogicMax.y = sceneData.GetLogicZmax();

        m_SceneLogicPos = sceneData.GetLogicPos();

        if (ecosystemData.Length == logicLength * logicWidth)
        {
            m_EcosystemData = ecosystemData;
            UpdateEcosystemData();
        }
        else
        {
            m_EcosystemData = null;
        }
    }
    public void UpdateEcosystemData()
    {
        CreateEcosystemMaterial();
        CreateEcosystemTexture();
        UpdateEcosystemMeshTexture();
        UpdateEcosystemMesh();
    }
    void CreateEcosystemMaterial()
    {
        if (!ecosystemMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
#if UNITY_EDITOR
            var shader = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
            ecosystemMaterial = new Material(shader);
            ecosystemMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            ecosystemMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            ecosystemMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            ecosystemMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            ecosystemMaterial.SetInt("_ZWrite", 0);
            ecosystemMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            ecosystemMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(ecosystemMesh);
            }
            ecosystemMesh.RecalculateBounds();
            ecosystemMesh.RecalculateNormals();

            ecosystemMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            ecosystemMaterial.SetColor("_DyeColor", new Color(1.0f, 1.0f, 1.0f, 0.5f));


#else
            var shader = AssetShaderLoader.Find("Legacy Shaders/Transparent/Diffuse");
            ecosystemMaterial = new Material(shader);
            ecosystemMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            ecosystemMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            ecosystemMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            ecosystemMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            ecosystemMaterial.SetInt("_ZWrite", 0);
            ecosystemMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            ecosystemMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(ecosystemMesh);
            }
            ecosystemMesh.RecalculateBounds();
            ecosystemMesh.RecalculateNormals();

            ecosystemMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            ecosystemMaterial.SetColor("Main Color", new Color(1.0f, 1.0f, 1.0f, 0.5f));
#endif
        }
    }
    void CreateEcosystemTexture()
    {
        if (ecosystemTexture)
        {
            Editor.DestroyImmediate(ecosystemTexture);
        }

        ecosystemTexture = new Texture2D(logicLength, logicWidth, TextureFormat.RGBA32, false);
        ecosystemTexture.filterMode = FilterMode.Point;
        ecosystemMaterial.mainTexture = ecosystemTexture;
    }
    void UpdateEcosystemMeshTexture()
    {
        if (m_EcosystemData == null)
        {
            return;
        }

        if (logicLength != ecosystemTexture.width
            || logicWidth != ecosystemTexture.height)
        {
            CreateEcosystemTexture();
        }

        for (int y = 0; y < logicWidth; ++y)
        {
            for (int x = 0; x < logicLength; ++x)
            {
                ecosystemTexture.SetPixel(x, y, Ecosystem2Color(GetEcosystemId(x, y)));
            }
        }

        ecosystemTexture.Apply();
    }
    void UpdateEcosystemMesh()
    {
        CreateEcosystemMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);
        temp[1] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[2] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[3] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);

        ecosystemMesh.vertices = temp;

        ecosystemMesh.RecalculateBounds();
        ecosystemMesh.RecalculateNormals();
        ecosystemMesh.UploadMeshData(false);

        int gridsv_num = ((logicLength + 1) + (logicWidth + 1)) * 2;
        gridsv = new Vector3[gridsv_num];

        for (int j = m_LogicMin.x; j <= m_LogicMax.x; ++j)
        {
            int index = j - m_LogicMin.x;
            gridsv[2 * index] = new Vector3(j * m_GridSize.x, 0, m_LogicMin.y * m_GridSize.y);
            gridsv[2 * index + 1] = new Vector3(j * m_GridSize.x, 0, m_LogicMax.y * m_GridSize.y);
        }

        for (int j = m_LogicMin.y; j <= m_LogicMax.y; ++j)
        {
            int index = j - m_LogicMin.y;
            int offset = 2 * (logicLength + 1);
            gridsv[2 * index + offset] = new Vector3(m_LogicMin.x * m_GridSize.x, 0, j * m_GridSize.y);
            gridsv[2 * index + 1 + offset] = new Vector3(m_LogicMax.x * m_GridSize.x, 0, j * m_GridSize.y);
        }
    }
    public void UpdateGrassData()
    {
        CreateGrassMaterial();
        CreateGrassMeshTexture();
        UpdateGrassMeshTexture();
        UpdateGrassMesh();
    }
    void CreateGrassMeshTexture()
    {
        if (grassTexture)
        {
            Editor.DestroyImmediate(grassTexture);
        }

        grassTexture = new Texture2D(logicLength, logicWidth, TextureFormat.RGBA32, false);
        grassTexture.filterMode = FilterMode.Point;
        grassMaterial.mainTexture = grassTexture;
    }

    void CreateGrassMaterial()
    {
        if (!grassMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
#if UNITY_EDITOR
            var shader = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
            grassMaterial = new Material(shader);
            grassMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            grassMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            grassMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            grassMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            grassMaterial.SetInt("_ZWrite", 0);
            grassMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            grassMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(grassMesh);
            }
            grassMesh.RecalculateBounds();
            grassMesh.RecalculateNormals();

            grassMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            grassMaterial.SetColor("_DyeColor", new Color(1.0f, 1.0f, 1.0f, 0.5f));


#else
            var shader = AssetShaderLoader.Find("Legacy Shaders/Transparent/Diffuse");
            grassMaterial = new Material(shader);
            grassMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            grassMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            grassMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            grassMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            grassMaterial.SetInt("_ZWrite", 0);
            grassMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            grassMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(grassMesh);
            }
            grassMesh.RecalculateBounds();
            grassMesh.RecalculateNormals();

            grassMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            grassMaterial.SetColor("Main Color", new Color(1.0f, 1.0f, 1.0f, 0.5f));
#endif
        }
    }
    void UpdateGrassMeshTexture()
    {
        if (m_GrassData == null)
        {
            return;
        }

        if (logicLength != grassTexture.width
            || logicWidth != grassTexture.height)
        {
            CreateGrassMeshTexture();
        }

        for (int y = 0; y < logicWidth; ++y)
        {
            for (int x = 0; x < logicLength; ++x)
            {
                grassTexture.SetPixel(x, y, GrassId2Color(GetGrassId(x, y)));
            }
        }

        grassTexture.Apply();
    }
    ushort GetEcosystemId(int x,int y)
    {
        return m_EcosystemData[y * logicLength + x];
    }
    Color Ecosystem2Color(ushort ecosystemId)
    {
        return ecosystemId > 0 ? Color.blue : Color.clear;
    }
    ushort GetGrassId(int x, int y)
    {
        return m_GrassData[y * logicLength + x];
    }
    Color GrassId2Color(ushort grassId)
    {
        return grassId > 0 ? Color.green : Color.clear;
    }
    void UpdateGrassMesh()
    {
        CreateGrassMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);
        temp[1] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[2] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[3] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);

        grassMesh.vertices = temp;

        grassMesh.RecalculateBounds();
        grassMesh.RecalculateNormals();
        grassMesh.UploadMeshData(false);

        int gridsv_num = ((logicLength + 1) + (logicWidth + 1)) * 2;
        gridsv = new Vector3[gridsv_num];

        for (int j = m_LogicMin.x; j <= m_LogicMax.x; ++j)
        {
            int index = j - m_LogicMin.x;
            gridsv[2 * index] = new Vector3(j * m_GridSize.x, 0, m_LogicMin.y * m_GridSize.y);
            gridsv[2 * index + 1] = new Vector3(j * m_GridSize.x, 0, m_LogicMax.y * m_GridSize.y);
        }

        for (int j = m_LogicMin.y; j <= m_LogicMax.y; ++j)
        {
            int index = j - m_LogicMin.y;
            int offset = 2 * (logicLength + 1);
            gridsv[2 * index + offset] = new Vector3(m_LogicMin.x * m_GridSize.x, 0, j * m_GridSize.y);
            gridsv[2 * index + 1 + offset] = new Vector3(m_LogicMax.x * m_GridSize.x, 0, j * m_GridSize.y);
        }
    }

    public void UpdateBlockLayer()
    {
        CreateBlockMaterial();
        CreateBlockMeshTexture();
        UpdateBlockMeshTexture();
        UpdateBlockMesh();
    }

    void CreateBlockMeshTexture()
    {
        if (blockTexture)
        {
            Editor.DestroyImmediate(blockTexture);
        }

        blockTexture = new Texture2D(logicLength, logicWidth, TextureFormat.RGBA32, false);
        blockTexture.filterMode = FilterMode.Point;
        blockMaterial.mainTexture = blockTexture;
    }

    static Color BlockByte2Color(byte bt)
    {
        return bt > 0 ? Color.red : Color.black;
    }

    byte GetBlockByte(int x, int y)
    {
        return m_BlockData[y * logicLength + x];
    }

    void UpdateBlockMeshTexture()
    {
        if (m_BlockData == null)
        {
            return;
        }

        if (logicLength != blockTexture.width
            || logicWidth != blockTexture.height)
        {
            CreateBlockMeshTexture();
        }

        for (int y = 0; y < logicWidth; ++y)
        {
            for (int x = 0; x < logicLength; ++x)
            {
                blockTexture.SetPixel(x, y, BlockByte2Color(GetBlockByte(x, y)));
            }
        }

        blockTexture.Apply();
    }

    void UpdateBlockMesh()
    {
        CreateBlockMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);
        temp[1] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[2] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[3] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);

        blockMesh.vertices = temp;

        blockMesh.RecalculateBounds();
        blockMesh.RecalculateNormals();
        blockMesh.UploadMeshData(false);

        int gridsv_num = ((logicLength + 1) + (logicWidth + 1)) * 2;
        gridsv = new Vector3[gridsv_num];

        for (int j = m_LogicMin.x; j <= m_LogicMax.x; ++j)
        {
            int index = j - m_LogicMin.x;
            gridsv[2 * index] = new Vector3(j * m_GridSize.x, 0, m_LogicMin.y * m_GridSize.y);
            gridsv[2 * index + 1] = new Vector3(j * m_GridSize.x, 0, m_LogicMax.y * m_GridSize.y);
        }

        for (int j = m_LogicMin.y; j <= m_LogicMax.y; ++j)
        {
            int index = j - m_LogicMin.y;
            int offset = 2 * (logicLength + 1);
            gridsv[2 * index + offset] = new Vector3(m_LogicMin.x * m_GridSize.x, 0, j * m_GridSize.y);
            gridsv[2 * index + 1 + offset] = new Vector3(m_LogicMax.x * m_GridSize.x, 0, j * m_GridSize.y);
        }
    }

    void CreateBlockMaterial()
    {
        if (!blockMaterial)
        {
#if UNITY_EDITOR
            var shader = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
            blockMaterial = new Material(shader);
            blockMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            blockMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            blockMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            blockMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            blockMaterial.SetInt("_ZWrite", 0);
            blockMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            blockMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(blockMesh);
            }
            blockMesh.RecalculateBounds();
            blockMesh.RecalculateNormals();

            blockMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            blockMaterial.SetColor("_DyeColor", new Color(1.0f, 1.0f, 1.0f, 0.5f));
#else
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = AssetShaderLoader.Find("Legacy Shaders/Transparent/Diffuse");
            blockMaterial = new Material(shader);
            blockMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            blockMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            blockMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            blockMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            blockMaterial.SetInt("_ZWrite", 0);
            blockMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            blockMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(blockMesh);
            }
            blockMesh.RecalculateBounds();
            blockMesh.RecalculateNormals();

            blockMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            blockMaterial.SetColor("Main Color", new Color(1.0f, 1.0f, 1.0f, 0.5f));
#endif
        }

    }

    public void UpdateEventAreaLayer()
    {
        CreateEventAreaMaterial();
        CreateEventAreaMeshTexture();
        UpdateEventAreaMeshTexture();
        UpdateEventAreaMesh();
    }

    void CreateEventAreaMeshTexture()
    {
        if (eventAreaTexture)
        {
            Editor.DestroyImmediate(eventAreaTexture);
        }

        eventAreaTexture = new Texture2D(logicLength, logicWidth, TextureFormat.RGBA32, false);
        eventAreaTexture.filterMode = FilterMode.Point;
        eventAreaMaterial.mainTexture = eventAreaTexture;
    }

    void CreateEventAreaMaterial()
    {
        if (!eventAreaMaterial)
        {
#if UNITY_EDITOR
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            //var shader = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
            var shader = Shader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
            eventAreaMaterial = new Material(shader);
            eventAreaMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            eventAreaMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            eventAreaMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            eventAreaMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            eventAreaMaterial.SetInt("_ZWrite", 0);
            eventAreaMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            eventAreaMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(eventAreaMesh);
            }
            eventAreaMesh.RecalculateBounds();
            eventAreaMesh.RecalculateNormals();

            eventAreaMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            eventAreaMaterial.SetColor("_DyeColor", new Color(1.0f, 1.0f, 1.0f, 0.5f));
#else
            var shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
            eventAreaMaterial = new Material(shader);
            eventAreaMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            eventAreaMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            eventAreaMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            eventAreaMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            eventAreaMaterial.SetInt("_ZWrite", 0);
            eventAreaMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);


            eventAreaMesh = new Mesh();

            Color32 color32 = Color.white;
            using (var vh = new VertexHelper())
            {
                vh.AddVert(new Vector3(-0.5f, 0, -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f, 0, 0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, 0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f, 0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(eventAreaMesh);
            }
            eventAreaMesh.RecalculateBounds();
            eventAreaMesh.RecalculateNormals();

            eventAreaMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            eventAreaMaterial.SetColor("Main Color", new Color(1.0f, 1.0f, 1.0f, 0.5f));
#endif
        }
    }

    void UpdateEventAreaMeshTexture()
    {
        if (m_EventAreaData == null)
        {
            return;
        }

        if (logicLength != eventAreaTexture.width
            || logicWidth != eventAreaTexture.height)
        {
            CreateEventAreaMeshTexture();
        }

        for (int y = 0; y < logicWidth; ++y)
        {
            for (int x = 0; x < logicLength; ++x)
            {
                eventAreaTexture.SetPixel(x, y, EventAreaByte2Color(GetEventAreaByte(x, y)));
            }
        }

        eventAreaTexture.Apply();
    }

    void UpdateEventAreaMesh()
    {
        CreateEventAreaMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);
        temp[1] = new Vector3(m_LogicMin.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[2] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMax.y * m_GridSize.x);
        temp[3] = new Vector3(m_LogicMax.x * m_GridSize.x, 0.05f, m_LogicMin.y * m_GridSize.x);

        eventAreaMesh.vertices = temp;

        eventAreaMesh.RecalculateBounds();
        eventAreaMesh.RecalculateNormals();
        eventAreaMesh.UploadMeshData(false);

        int gridsv_num = ((logicLength + 1) + (logicWidth + 1)) * 2;
        gridsv = new Vector3[gridsv_num];

        for (int j = m_LogicMin.x; j <= m_LogicMax.x; ++j)
        {
            int index = j - m_LogicMin.x;
            gridsv[2 * index] = new Vector3(j * m_GridSize.x, 0, m_LogicMin.y * m_GridSize.y);
            gridsv[2 * index + 1] = new Vector3(j * m_GridSize.x, 0, m_LogicMax.y * m_GridSize.y);
        }

        for (int j = m_LogicMin.y; j <= m_LogicMax.y; ++j)
        {
            int index = j - m_LogicMin.y;
            int offset = 2 * (logicLength + 1);
            gridsv[2 * index + offset] = new Vector3(m_LogicMin.x * m_GridSize.x, 0, j * m_GridSize.y);
            gridsv[2 * index + 1 + offset] = new Vector3(m_LogicMax.x * m_GridSize.x, 0, j * m_GridSize.y);
        }
    }

    Color EventAreaByte2Color(byte bt)
    {
        return bt > 0 ? Color.magenta : Color.clear;
    }

    byte GetEventAreaByte(int x, int y)
    {
        var index = y * logicLength + x;
        if (m_EventAreaData.Length <= index)
            return 0;
        return m_EventAreaData[index];
    }

    void DrawGirds(Color color)
    {
        if (gridsv == null)
        {
            return;
        }
        Handles.color = color;
        //Handles.DrawPolyLine(gridsv);
        for (int j = 0; j < gridsv.Length / 2; ++j)
        {
            Handles.DrawLine(gridsv[2 * j] + m_SceneLogicPos, gridsv[2 * j + 1] + m_SceneLogicPos);
        }
    }

    void DrawDoor(BeRegionBase door)
    {
        var curDoor = door as BeRegionTransportDoor;
        if (curDoor == null) return;

        if (curDoor.regionInfo.GetRegiontype() == DRegionInfo.RegionType.Circle)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(curDoor.GetRegionPos().vector3, Vector3.up, curDoor.regionInfo.GetRadius());
        }
    }
    void OnDrawGizmos()
    {

        //Gizmos.matrix = DSkillData.root.transform.localToWorldMatrix;
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        if (showgrid)
        {
            DrawGirds(Color.yellow);
        }

        if (showblock && editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateBlockMaterial();
            // Apply the line material
            blockMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(m_SceneLogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(blockMesh, mat);
        }
        if (showgrass && editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateGrassMaterial();
            // Apply the line material
            grassMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(m_SceneLogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(grassMesh, mat);
        }
        if (showecosystem && editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateEcosystemMaterial();
            // Apply the line material
            ecosystemMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(m_SceneLogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(ecosystemMesh, mat);
        }
        if (showeventarea || editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateEventAreaMaterial();
            // Apply the line material
            eventAreaMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(m_SceneLogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(eventAreaMesh, mat);
        }

        DEntityInfo[] dinfos = DSceneEntitySelection.GetSelected();

        if (dinfos != null)
        {
            foreach (var dinfo in dinfos)
            {
                Handles.color = Color.yellow;
                Handles.DrawSolidDisc(dinfo.Position, Vector3.up, 0.1f);
            }
        }

        if (showtransport)
        {
            if (m_doorData != null)
            {
                for (int i = 0; i < m_doorData.Count; i++)
                {
                    var curDoor = m_doorData[i];
                    DrawDoor(curDoor);
                }
            }
        }
    }
}
#endif
