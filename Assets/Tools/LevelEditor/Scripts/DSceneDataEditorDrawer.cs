#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.ComponentModel;

public enum SceneEditorMode
{
    [Description("信息")]
    NORMAL = 0,
    [Description("层级")]
    BLOCKLAYER,
    [Description("单位")]
    ENTITYS
}

[ExecuteInEditMode]
public class DSceneDataEditorDrawer : MonoBehaviour
{
    protected Material      blockMaterial;
    protected Mesh          blockMesh;
    protected Texture2D     blockTexture;
    protected DSceneData    sceneData;
    protected Vector3[]     gridsv;

    protected Material grassMaterial;
    protected Mesh grassMesh;
    protected Texture2D grassTexture;

    protected Material ecosystemMaterial;
    protected Mesh ecosystemMesh;
    protected Texture2D ecosystemTexture;

    protected Material eventAreaMaterial;
    protected Mesh eventAreaMesh;
    protected Texture2D eventAreaTexture;

    public  SceneEditorMode  editmode;
    private bool showblock   = true;
    public bool showgrid    = false;
    public bool showmonster = true;
    public bool showentity  = true;
    public bool showregion  = true;
    private bool showgrass = false;
    private bool showecosystem = false;
    private bool showeventarea = false;
    private bool artistMode = false;
    public int  brushSize = 1;
    private Color grassColor = Color.green;
    private Color ecosystemColor = Color.blue;
    public GameObject dragObject;


    public void DestroyObjects<T>(ref T obj) where T : UnityEngine.Object
    {
        Editor.DestroyImmediate(obj);
        obj = null;
    }
    public void SetArtistMode(bool isArtist)
    {
        if (isArtist)
        {
            //if ((showblock == showgrass || showblock == showecosystem) && showblock)
            //{
            //    showblock = true;
            //    showgrass = false;
            //    showecosystem = false;
            //    ResetTexture();
            //}
            //else if(showgrass && showgrass == showecosystem)
            //{
            //    showblock = false;
            //    showgrass = true;
            //    showecosystem = false;
            //}
            if (showblock)
            {
                showgrass = false;
                showecosystem = false;
                showeventarea = false;
            }
            else if (showgrass)
            {
                showecosystem = false;
                showeventarea = false;
            }
            else if (showecosystem)
            {
                showeventarea = false;
            }
            else if (!showeventarea)
            {
                showblock = true;
            }
            ResetTexture();
        }
        artistMode = isArtist;
    }
   
    public bool ShowBlock
    {
        get { return showblock; }
        set
        {
            if (artistMode && value)
            {
                showgrass = false;
                showecosystem = false;
                showeventarea = false;
            }
            bool isChange = (showblock != value);
            showblock = value;
            if (isChange)
            {
                ResetTexture();
            }
        }
    }
    public bool ShowGrass
    {
        get { return showgrass; }
        set
        {
            if (artistMode && value)
            {
                showblock = false;
                showecosystem = false;
                showeventarea = false;
            }
            bool isChange = (showgrass != value);
            showgrass = value;
            if (isChange)
            {
                ResetTexture();
            }
        }
    }
    public bool ShowEcoSystem
    {
        get { return showecosystem; }
        set
        {
            if (artistMode && value)
            {
                showgrass = false;
                showblock = false;
                showeventarea = false;
            }
            bool isChange = (showecosystem != value);
            showecosystem = value;
            if(isChange)
            {
                ResetTexture();
            }
        }
    }
    public bool ShowEventArea
    {
        get { return showeventarea; }
        set
        {
            if (artistMode && value)
            {
                showblock = false;
                showgrass = false;
                showecosystem = false;
            }
            bool isChange = (showeventarea != value);
            showeventarea = value;
            if (isChange)
            {
                ResetTexture();
            }
        }
    }

    private void ResetTexture()
    {
        if (ShowBlock)
        {
            UpdateBlockLayer();
        }
        else
        {
            if (blockMaterial)
                DestroyObjects(ref blockMaterial);
            if (blockMesh)
                DestroyObjects(ref blockMesh);
            if (blockTexture)
                DestroyObjects(ref blockTexture);
        }
        if (ShowGrass)
        {
            UpdateGrassLayer();
        }
        else
        {
            if (grassMaterial)
            {
                DestroyObjects(ref grassMaterial);
            }
            if (grassMesh)
            {
                DestroyObjects(ref grassMesh);
            }
            if (grassTexture)
            {
                DestroyObjects(ref grassTexture);
            }
        }

        if(ShowEcoSystem)
        {
            UpdateEcosystemLayer();
        }
        else
        {
            if(ecosystemMaterial)
            {
                DestroyObjects(ref ecosystemMaterial);
            }
            if(ecosystemMesh)
            {
                DestroyObjects(ref ecosystemMesh);
            }
            if(ecosystemTexture)
            {
                DestroyObjects(ref ecosystemTexture);
            }
        }

        if (ShowEventArea)
        {
            UpdateEventAreaLayer();
        }
        else
        {
            if (eventAreaMaterial)
            {
                DestroyObjects(ref eventAreaMaterial);
            }
            if (eventAreaMesh)
            {
                DestroyObjects(ref eventAreaMesh);
            }
            if (eventAreaTexture)
            {
                DestroyObjects(ref eventAreaTexture);
            }
        }
    }

    void OnDestroy()
    {
        DestroyObjects(ref blockMaterial);
        DestroyObjects(ref blockMesh);
        DestroyObjects(ref blockTexture);
        if (grassMaterial)
        {
            DestroyObjects(ref grassMaterial);
        }
        if (grassMesh)
        {
            DestroyObjects(ref grassMesh);
        }
        if (grassTexture)
        {
            DestroyObjects(ref grassTexture);
        }

        if (ecosystemMaterial)
        {
            DestroyObjects(ref ecosystemMaterial);
        }
        if (ecosystemMesh)
        {
            DestroyObjects(ref ecosystemMesh);
        }
        if (ecosystemTexture)
        {
            DestroyObjects(ref ecosystemTexture);
        }

        if (eventAreaMaterial)
        {
            DestroyObjects(ref eventAreaMaterial);
        }
        if (eventAreaMesh)
        {
            DestroyObjects(ref eventAreaMesh);
        }
        if (eventAreaTexture)
        {
            DestroyObjects(ref eventAreaTexture);
        }

    }

    public DSceneData   SceneData
    {
        set
        {
            if(sceneData != value)
            {
                sceneData = value;
                UpdateData();
            }
        }
    }

    public void OnLogicSizeChange()
    {
        ResetTexture();
        //CreateBlockMaterial();
        //CreateBlockMeshTexture();
        //UpdateBlockMeshTexture();
        //UpdateBlockMesh();
    }

    public void OnRefeashData()
    {
        UpdateBlockMesh();
        UpdateBlockMeshTexture();

        UpdateEventAreaMesh();
        UpdateEventAreaMeshTexture();
    }

    public void UpdateBlockLayer()
    {
        if(sceneData != null )
        {
            CreateBlockMaterial();
            CreateBlockMeshTexture();
            UpdateBlockMeshTexture();
            UpdateBlockMesh();
        }
    }

    public void UpdateGrassLayer()
    {
        if (sceneData != null)
        {
            CreateGrassMaterial();
            CreateGrassMeshTexture();
            UpdateGrassMeshTexture();
            UpdateGrassMesh();
        }
    }

    public void UpdateEcosystemLayer()
    {
        if (sceneData != null)
        {
            CreateEcosystemMaterial();
            CreateEcosystemMeshTexture();
            UpdateEcosystemMeshTexture();
            UpdateEcosystemMesh();
        }
    }

    public void UpdateEventAreaLayer()
    {
        if (sceneData != null)
        {
            CreateEventAreaMaterial();
            CreateEventAreaMeshTexture();
            UpdateEventAreaMeshTexture();
            UpdateEventAreaMesh();
        }
    }

    void CreateEcosystemMaterial()
    {
        if (!ecosystemMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
            ecosystemMaterial = new Material(shader);
            ecosystemMaterial.name = "ecosystem";
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
            ecosystemMesh.name = "ecosystem";
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
        }
    }
    void CreateEcosystemMeshTexture()
    {
        if (ecosystemTexture)
        {
            Editor.DestroyImmediate(ecosystemTexture);
        }

        ecosystemTexture = new Texture2D(sceneData.LogicX, sceneData.LogicZ, TextureFormat.RGBA32, false);
        ecosystemTexture.name = "ecosystem";
        ecosystemTexture.filterMode = FilterMode.Point;
        ecosystemMaterial.mainTexture = ecosystemTexture;
    }
    void CreateGrassMaterial()
    {
        if (!grassMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
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
        }
    }
    void CreateGrassMeshTexture()
    {
        if (grassTexture)
        {
            Editor.DestroyImmediate(grassTexture);
        }

        grassTexture = new Texture2D(sceneData.LogicX, sceneData.LogicZ, TextureFormat.RGBA32, false);
        grassTexture.filterMode = FilterMode.Point;
        grassMaterial.mainTexture = grassTexture;
    }

    void UpdateEcosystemMeshTexture()
    {
        if (sceneData == null)
        {
            return;
        }

        if (sceneData.LogicX != ecosystemTexture.width
            || sceneData.LogicZ != ecosystemTexture.height)
        {
            CreateEcosystemMeshTexture();
        }

        for (int y = 0; y < sceneData.LogicZ; ++y)
        {
            for (int x = 0; x < sceneData.LogicX; ++x)
            {
                ecosystemTexture.SetPixel(x, y, EcoSystem2Color(GetEcosystemId(x, y)));
            }
        }

        ecosystemTexture.Apply();
    }

    void UpdateGrassMeshTexture()
    {
        if (sceneData == null)
        {
            return;
        }

        if (sceneData.LogicX != grassTexture.width
            || sceneData.LogicZ != grassTexture.height)
        {
            CreateGrassMeshTexture();
        }

        for (int y = 0; y < sceneData.LogicZ; ++y)
        {
            for (int x = 0; x < sceneData.LogicX; ++x)
            {
                grassTexture.SetPixel(x, y, GrassId2Color(GetGrassId(x, y)));
            }
        }

        grassTexture.Apply();
    }
    ushort GetGrassId(int x, int y)
    {
        if (sceneData._grasslayer.Length <= 0)
        {
            return 0;
        }
        int index = y * sceneData.LogicX + x;
        if (index < 0 || index >= sceneData._grasslayer.Length) return 0;
        return sceneData._grasslayer[y * sceneData.LogicX + x];
    }
    ushort GetEcosystemId(int x, int y)
    {
        if (sceneData._ecosystemLayer.Length <= 0)
        {
            return 0;
        }
        int index = y * sceneData.LogicX + x;
        if (index < 0 || index >= sceneData._ecosystemLayer.Length) return 0;
        return sceneData._ecosystemLayer[y * sceneData.LogicX + x];
    }
    public ushort GetEcosystemId(float x,float y)
    {
        if (sceneData == null)
        {
            return 0;
        }
        if (!IsEcosystemSetted()) return 0;
        int radius = 1;
        Vector2 v2 = GetGridCenter(radius, x, y);
        float xMin, xMax;
        float yMin, yMax;
        xMin = v2.x - radius * sceneData._GridSize.x * 0.5f;
        xMax = v2.x + radius * sceneData._GridSize.x * 0.5f;

        yMin = v2.y - radius * sceneData._GridSize.y * 0.5f;
        yMax = v2.y + radius * sceneData._GridSize.y * 0.5f;
        for (int i = 0; i < radius; ++i)
        {
            for (int j = 0; j < radius; ++j)
            {
                x = xMin + (i + 0.5f) * sceneData._GridSize.x;
                y = yMin + (j + 0.5f) * sceneData._GridSize.y;

                x -= sceneData._CenterPostionNew.x;
                y -= sceneData._CenterPostionNew.z;

                x = x / sceneData._GridSize.x;
                y = y / sceneData._GridSize.y;

                x -= sceneData._LogicXmin;
                y -= sceneData._LogicZmin;
                int ix = (int)x;
                int iy = (int)y;
                return GetEcosystemId(ix, iy);
            }
        }
        return 0;
    }
    public ushort GetGrassId(float x, float y)
    {
        if (sceneData == null)
        {
            return 0;
        }
        if (!IsGrassSetted()) return 0;
        int radius = 1;
        Vector2 v2 = GetGridCenter(radius, x, y);
        float xMin, xMax;
        float yMin, yMax;
        xMin = v2.x - radius * sceneData._GridSize.x * 0.5f;
        xMax = v2.x + radius * sceneData._GridSize.x * 0.5f;

        yMin = v2.y - radius * sceneData._GridSize.y * 0.5f;
        yMax = v2.y + radius * sceneData._GridSize.y * 0.5f;
        for (int i = 0; i < radius; ++i)
        {
            for (int j = 0; j < radius; ++j)
            {
                x = xMin + (i + 0.5f) * sceneData._GridSize.x;
                y = yMin + (j + 0.5f) * sceneData._GridSize.y;

                x -= sceneData._CenterPostionNew.x;
                y -= sceneData._CenterPostionNew.z;

                x = x / sceneData._GridSize.x;
                y = y / sceneData._GridSize.y;

                x -= sceneData._LogicXmin;
                y -= sceneData._LogicZmin;
                int ix = (int)x;
                int iy = (int)y;
                return GetGrassId(ix, iy);
            }
        }
        return 0;
    }
    Color GrassId2Color(ushort bt)
    {
        return bt > 0 ? grassColor : Color.black;
    }
    Color EcoSystem2Color(ushort bt)
    {
        return bt > 0 ? ecosystemColor : Color.black;
    }
    public bool IsGrassSetted()
    {
        return sceneData._grasslayer.Length > 0;
    }
    public bool IsEcosystemSetted()
    {
        return sceneData._ecosystemLayer.Length > 0;
    }
    void SetGrassByte(int x, int y, ushort bt)
    {
        sceneData._grasslayer[y * sceneData.LogicX + x] = bt;
    }
    void UpdateGrassMesh()
    {
        if (sceneData == null)
        {
            return;
        }

        CreateGrassMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);
        temp[1] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
        temp[2] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
        temp[3] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);

        grassMesh.vertices = temp;

        grassMesh.RecalculateBounds();
        grassMesh.RecalculateNormals();
        grassMesh.UploadMeshData(false);

        int gridsv_num = ((sceneData.LogicX + 1) + (sceneData.LogicZ + 1)) * 2;
        gridsv = new Vector3[gridsv_num];

        for (int j = sceneData._LogicXmin; j <= sceneData._LogicXmax; ++j)
        {
            int index = j - sceneData._LogicXmin;
            gridsv[2 * index] = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmin * sceneData._GridSize.y);
            gridsv[2 * index + 1] = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmax * sceneData._GridSize.y);
        }

        for (int j = sceneData._LogicZmin; j <= sceneData._LogicZmax; ++j)
        {
            int index = j - sceneData._LogicZmin;
            int offset = 2 * (sceneData.LogicX + 1);
            gridsv[2 * index + offset] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
            gridsv[2 * index + 1 + offset] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
        }
    }

    void UpdateEcosystemMesh()
    {
        if (sceneData == null)
        {
            return;
        }

        CreateEcosystemMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);
        temp[1] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
        temp[2] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
        temp[3] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);

        ecosystemMesh.vertices = temp;

        ecosystemMesh.RecalculateBounds();
        ecosystemMesh.RecalculateNormals();
        ecosystemMesh.UploadMeshData(false);

        int gridsv_num = ((sceneData.LogicX + 1) + (sceneData.LogicZ + 1)) * 2;
        gridsv = new Vector3[gridsv_num];

        for (int j = sceneData._LogicXmin; j <= sceneData._LogicXmax; ++j)
        {
            int index = j - sceneData._LogicXmin;
            gridsv[2 * index] = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmin * sceneData._GridSize.y);
            gridsv[2 * index + 1] = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmax * sceneData._GridSize.y);
        }

        for (int j = sceneData._LogicZmin; j <= sceneData._LogicZmax; ++j)
        {
            int index = j - sceneData._LogicZmin;
            int offset = 2 * (sceneData.LogicX + 1);
            gridsv[2 * index + offset] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
            gridsv[2 * index + 1 + offset] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
        }
    }


    public void UpdateData()
    {
        if (ShowGrass)
        {
            UpdateGrassLayer();
        }

        if (ShowBlock)
        {
            UpdateBlockLayer();
        }

        if (ShowEcoSystem)
        {
            UpdateEcosystemLayer();
        }

        if (ShowEventArea)
        {
            UpdateEventAreaLayer();
        }
    }

    private Vector2 GetGridCenter(int radius, float x, float y)
    {
        x -= sceneData._LogicXmin * sceneData._GridSize.x;
        y -= sceneData._LogicZmin * sceneData._GridSize.y;


        if (radius % 2 == 0)
        {
            x += sceneData._GridSize.x * 0.5f;
            y += sceneData._GridSize.y * 0.5f;
        }


        x = ((int)(x / sceneData._GridSize.x) * sceneData._GridSize.x);
        y = ((int)(y / sceneData._GridSize.y) * sceneData._GridSize.y);

        if (radius % 2 != 0)
        {
            x += sceneData._GridSize.x * 0.5f;
            y += sceneData._GridSize.y * 0.5f;
        }

        x += sceneData._LogicXmin * sceneData._GridSize.x;
        y += sceneData._LogicZmin * sceneData._GridSize.y;
        
        /*
        x -= sceneData._CenterPostion.x;
        y -= sceneData._CenterPostion.z;

        x = x / sceneData._GridSize.x;
        y = y / sceneData._GridSize.y;

        x -= sceneData._LogicXmin;
        y -= sceneData._LogicZmin;
        */
        return new Vector2(x, y);
    }

    public void SetGrassLayer(int raduis, float x, float y, ushort ds)
    {
        if (sceneData == null)
        {
            return;
        }
        if (!IsGrassSetted()) return;
        Vector2 v2 = GetGridCenter(raduis, x, y);
        float xMin, xMax;
        float yMin, yMax;

        xMin = v2.x - raduis * sceneData._GridSize.x * 0.5f;
        xMax = v2.x + raduis * sceneData._GridSize.x * 0.5f;

        yMin = v2.y - raduis * sceneData._GridSize.y * 0.5f;
        yMax = v2.y + raduis * sceneData._GridSize.y * 0.5f;

        for (int i = 0; i < raduis; ++i)
        {
            for (int j = 0; j < raduis; ++j)
            {
                x = xMin + (i + 0.5f) * sceneData._GridSize.x;
                y = yMin + (j + 0.5f) * sceneData._GridSize.y;

                x -= sceneData._CenterPostionNew.x;
                y -= sceneData._CenterPostionNew.z;

                x = x / sceneData._GridSize.x;
                y = y / sceneData._GridSize.y;

                x -= sceneData._LogicXmin;
                y -= sceneData._LogicZmin;

                if (x < 0 || x >= grassTexture.width)
                {
                    continue;
                }

                if (y < 0 || y >= grassTexture.height)
                {
                    continue;
                }

                int ix = (int)x;
                int iy = (int)y;


                SetGrassByte(ix, iy, ds);
                grassTexture.SetPixel(ix, iy, GrassId2Color(ds));
            }
        }


        grassTexture.Apply();
        grassMaterial.mainTexture = grassTexture;
        SceneView.currentDrawingSceneView.Repaint();
    }

    public void SetEcosystemLayer(int raduis, float x, float y, ushort ds)
    {
        if (sceneData == null)
        {
            return;
        }
        if (!IsEcosystemSetted()) return;
        Vector2 v2 = GetGridCenter(raduis, x, y);
        float xMin, xMax;
        float yMin, yMax;

        xMin = v2.x - raduis * sceneData._GridSize.x * 0.5f;
        xMax = v2.x + raduis * sceneData._GridSize.x * 0.5f;

        yMin = v2.y - raduis * sceneData._GridSize.y * 0.5f;
        yMax = v2.y + raduis * sceneData._GridSize.y * 0.5f;

        for (int i = 0; i < raduis; ++i)
        {
            for (int j = 0; j < raduis; ++j)
            {
                x = xMin + (i + 0.5f) * sceneData._GridSize.x;
                y = yMin + (j + 0.5f) * sceneData._GridSize.y;

                x -= sceneData._CenterPostionNew.x;
                y -= sceneData._CenterPostionNew.z;

                x = x / sceneData._GridSize.x;
                y = y / sceneData._GridSize.y;

                x -= sceneData._LogicXmin;
                y -= sceneData._LogicZmin;

                if (x < 0 || x >= ecosystemTexture.width)
                {
                    continue;
                }

                if (y < 0 || y >= ecosystemTexture.height)
                {
                    continue;
                }

                int ix = (int)x;
                int iy = (int)y;


                SetEcosystemByte(ix, iy, ds);
                ecosystemTexture.SetPixel(ix, iy, EcoSystem2Color(ds));
            }
        }


        ecosystemTexture.Apply();
        ecosystemMaterial.mainTexture = ecosystemTexture;
        SceneView.currentDrawingSceneView.Repaint();
    }

    public void SetBlockLayer(int raduis,float x,float y,byte ds)
    {
        if(sceneData == null)
        {
            return;
        }

        Vector2 v2 = GetGridCenter(raduis, x, y);
        float xMin, xMax;
        float yMin, yMax;

        xMin = v2.x - raduis * sceneData._GridSize.x * 0.5f;
        xMax = v2.x + raduis * sceneData._GridSize.x * 0.5f;

        yMin = v2.y - raduis * sceneData._GridSize.y * 0.5f;
        yMax = v2.y + raduis * sceneData._GridSize.y * 0.5f;

        for(int i = 0; i < raduis; ++i)
        {
            for(int j = 0; j < raduis; ++j)
            {
                x = xMin + (i + 0.5f)* sceneData._GridSize.x;
                y = yMin + (j + 0.5f)* sceneData._GridSize.y;

                x -= sceneData._CenterPostionNew.x;
                y -= sceneData._CenterPostionNew.z;

                x = x / sceneData._GridSize.x;
                y = y / sceneData._GridSize.y;

                x -= sceneData._LogicXmin;
                y -= sceneData._LogicZmin;

                if (x < 0 || x >= blockTexture.width)
                {
                    continue;
                }

                if (y < 0 || y >= blockTexture.height)
                {
                    continue;
                }

                int ix = (int)x;
                int iy = (int)y;


                SetBlockByte(ix, iy, ds);
                blockTexture.SetPixel(ix, iy, BlockByte2Color(ds));
            }
        }

      
        blockTexture.Apply();
        blockMaterial.mainTexture = blockTexture;
        SceneView.currentDrawingSceneView.Repaint();
    }

    void CreateBlockMeshTexture()
    {
        if(blockTexture)
        {
            Editor.DestroyImmediate(blockTexture);
        }

        blockTexture = new Texture2D(sceneData.LogicX, sceneData.LogicZ, TextureFormat.RGBA32, false);
        blockTexture.filterMode = FilterMode.Point;
        blockMaterial.mainTexture = blockTexture;
    }

    static Color BlockByte2Color(byte bt)
    {
        return bt > 0 ? Color.red : Color.black;
    }


    byte GetBlockByte(int x,int y)
    {
        return sceneData._blocklayer[y * sceneData.LogicX + x];
    }

    void SetBlockByte(int x,int y,byte bt)
    {
        sceneData._blocklayer[y * sceneData.LogicX + x] = bt;
    }

    void SetEcosystemByte(int x,int y,ushort id)
    {
        sceneData._ecosystemLayer[y * sceneData.LogicX + x] = id;
    }
 

    void UpdateBlockMeshTexture()
    {
        if( sceneData == null )
        {
            return;
        }

        if(sceneData.LogicX != blockTexture.width
            || sceneData.LogicZ != blockTexture.height )
        {
            CreateBlockMeshTexture();
        }

        for(int y = 0; y < sceneData.LogicZ; ++y)
        {
            for(int x = 0; x < sceneData.LogicX; ++x)
            {
                blockTexture.SetPixel(x, y, BlockByte2Color(GetBlockByte(x,y)));
            }
        }

        blockTexture.Apply();
    }

    void UpdateBlockMesh()
    {
        if (sceneData == null)
        {
            return;
        }
        
        CreateBlockMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);
		temp[1] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
		temp[2] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
		temp[3] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);

        blockMesh.vertices = temp;

        blockMesh.RecalculateBounds();
        blockMesh.RecalculateNormals();
        blockMesh.UploadMeshData(false);

        int gridsv_num = ( (sceneData.LogicX + 1)  +  (sceneData.LogicZ + 1) ) * 2;
        gridsv = new Vector3[gridsv_num];

        for(int j = sceneData._LogicXmin; j <= sceneData._LogicXmax; ++j)
        {
            int index = j - sceneData._LogicXmin;
            gridsv[2 * index]       = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmin * sceneData._GridSize.y);     
            gridsv[2 * index + 1]   = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmax * sceneData._GridSize.y);
        }

        for (int j = sceneData._LogicZmin; j <= sceneData._LogicZmax; ++j)
        {  
            int index = j - sceneData._LogicZmin;
            int offset = 2 * (sceneData.LogicX + 1);
            gridsv[2 * index + offset]       = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
            gridsv[2 * index + 1 + offset]   = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
        }
    }

    void CreateBlockMaterial()
    {
        if (!blockMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            //var shader = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
            var shader = Shader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
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
                vh.AddVert(new Vector3(-0.5f,   0,  -0.5f), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(-0.5f,   0,  0.5f), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(0.5f,    0,  0.5f), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(0.5f,    0, -0.5f), color32, new Vector2(1f, 0f));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.FillMesh(blockMesh);
            }
            blockMesh.RecalculateBounds();
            blockMesh.RecalculateNormals();

            blockMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            blockMaterial.SetColor("_DyeColor", new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }
    }

    void CreateEventAreaMeshTexture()
    {
        if (eventAreaTexture)
        {
            Editor.DestroyImmediate(eventAreaTexture);
        }

        eventAreaTexture = new Texture2D(sceneData.LogicX, sceneData.LogicZ, TextureFormat.RGBA32, false);
        eventAreaTexture.filterMode = FilterMode.Point;
        eventAreaMaterial.mainTexture = eventAreaTexture;
    }

    void CreateEventAreaMaterial()
    {
        if (!eventAreaMaterial)
        {
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
        }
    }

    void UpdateEventAreaMeshTexture()
    {
        if (sceneData == null)
        {
            return;
        }

        if (sceneData.LogicX != eventAreaTexture.width
            || sceneData.LogicZ != eventAreaTexture.height)
        {
            CreateEventAreaMeshTexture();
        }

        for (int y = 0; y < sceneData.LogicZ; ++y)
        {
            for (int x = 0; x < sceneData.LogicX; ++x)
            {
                eventAreaTexture.SetPixel(x, y, EventAreaByte2Color(GetEventAreaByte(x, y)));
            }
        }

        eventAreaTexture.Apply();
    }

    void UpdateEventAreaMesh()
    {
        if (sceneData == null)
        {
            return;
        }

        CreateEventAreaMaterial();

        Vector3[] temp = new Vector3[4];

        temp[0] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);
        temp[1] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
        temp[2] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmax * sceneData._GridSize.x);
        temp[3] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0.05f, sceneData._LogicZmin * sceneData._GridSize.x);

        eventAreaMesh.vertices = temp;

        eventAreaMesh.RecalculateBounds();
        eventAreaMesh.RecalculateNormals();
        eventAreaMesh.UploadMeshData(false);

        int gridsv_num = ((sceneData.LogicX + 1) + (sceneData.LogicZ + 1)) * 2;
        gridsv = new Vector3[gridsv_num];

        for (int j = sceneData._LogicXmin; j <= sceneData._LogicXmax; ++j)
        {
            int index = j - sceneData._LogicXmin;
            gridsv[2 * index] = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmin * sceneData._GridSize.y);
            gridsv[2 * index + 1] = new Vector3(j * sceneData._GridSize.x, 0, sceneData._LogicZmax * sceneData._GridSize.y);
        }

        for (int j = sceneData._LogicZmin; j <= sceneData._LogicZmax; ++j)
        {
            int index = j - sceneData._LogicZmin;
            int offset = 2 * (sceneData.LogicX + 1);
            gridsv[2 * index + offset] = new Vector3(sceneData._LogicXmin * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
            gridsv[2 * index + 1 + offset] = new Vector3(sceneData._LogicXmax * sceneData._GridSize.x, 0, j * sceneData._GridSize.y);
        }
    }

    static Color EventAreaByte2Color(byte bt)
    {
        return bt > 0 ? Color.magenta : Color.clear;
    }

    byte GetEventAreaByte(int x, int y)
    {
        if (!IsEventAreaSet())
            return 0;
        return sceneData._eventAreaLayer[y * sceneData.LogicX + x];
    }

    void SetEventAreaByte(int x, int y, byte bt)
    {
        if (!IsEventAreaSet())
            return;
        sceneData._eventAreaLayer[y * sceneData.LogicX + x] = bt;
    }

    public void SetEventAreaLayer(int raduis, float x, float y, byte ds)
    {
        if (sceneData == null)
        {
            return;
        }

        Vector2 v2 = GetGridCenter(raduis, x, y);
        float xMin, xMax;
        float yMin, yMax;

        xMin = v2.x - raduis * sceneData._GridSize.x * 0.5f;
        xMax = v2.x + raduis * sceneData._GridSize.x * 0.5f;

        yMin = v2.y - raduis * sceneData._GridSize.y * 0.5f;
        yMax = v2.y + raduis * sceneData._GridSize.y * 0.5f;

        for (int i = 0; i < raduis; ++i)
        {
            for (int j = 0; j < raduis; ++j)
            {
                x = xMin + (i + 0.5f) * sceneData._GridSize.x;
                y = yMin + (j + 0.5f) * sceneData._GridSize.y;

                x -= sceneData._CenterPostionNew.x;
                y -= sceneData._CenterPostionNew.z;

                x = x / sceneData._GridSize.x;
                y = y / sceneData._GridSize.y;

                x -= sceneData._LogicXmin;
                y -= sceneData._LogicZmin;

                if (x < 0 || x >= eventAreaTexture.width)
                {
                    continue;
                }

                if (y < 0 || y >= eventAreaTexture.height)
                {
                    continue;
                }

                int ix = (int)x;
                int iy = (int)y;


                SetEventAreaByte(ix, iy, ds);
                eventAreaTexture.SetPixel(ix, iy, EventAreaByte2Color(ds));
            }
        }


        eventAreaTexture.Apply();
        eventAreaMaterial.mainTexture = eventAreaTexture;
        SceneView.currentDrawingSceneView.Repaint();
    }

    public bool IsEventAreaSet()
    {
        return sceneData._eventAreaLayer.Length > 0;
    }

    void DrawGameObjects()
    {
        if (dragObject)
        {
            SkinnedMeshRenderer[] renderers = dragObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach(var r in renderers)
            {
                Graphics.DrawMeshNow(r.sharedMesh, Matrix4x4.identity);
            }
        }
    }
    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
     

        //DrawGameObjects();
    }

    void DrawGirds(Color color)
    {
        if(sceneData == null)
        {
            return;
        }

        if(gridsv == null)
        {
            return;
        }
        Handles.color = color;
        //Handles.DrawPolyLine(gridsv);
        for(int j = 0; j < gridsv.Length / 2; ++ j)
        {
            Handles.DrawLine(gridsv[2 * j] + sceneData._LogicPos, gridsv[2 * j + 1] + sceneData._LogicPos);
        }
    }

    static public Ray Screen2Ray(Vector2 mousePos, SceneView sceneview)
    {
        return sceneview.camera.ScreenPointToRay(new Vector3(mousePos.x, sceneview.camera.pixelHeight - mousePos.y));
    }

    static public Vector3 ScreenPickGround(SceneView sceneview)
    {
        Event evt = Event.current;
        Vector2 mousePos = evt.mousePosition;

        Ray r = Screen2Ray(mousePos, sceneview);

        float fdis = -(r.origin.y / r.direction.y);
        return r.GetPoint(fdis);
    }

    public static void DrawRect(Vector3 position, Vector2 size)
    {
        Vector3 b = Quaternion.identity * Vector3.up;
        Vector3 vector = Quaternion.identity * Vector3.right;
        Vector3 vector2 = Quaternion.identity * Vector3.forward;
        float num = 0.5f * size.x;
        float num2 = 0.5f * size.y;
        Vector3 vector3 = position + vector * num2 + vector2 * num;
        Vector3 vector4 = position - vector * num2 + vector2 * num;
        Vector3 vector5 = position - vector * num2 - vector2 * num;
        Vector3 vector6 = position + vector * num2 - vector2 * num;
        Handles.DrawLine(vector3, vector4);
        Handles.DrawLine(vector4, vector5);
        Handles.DrawLine(vector5, vector6);
        Handles.DrawLine(vector6, vector3);
    }
    void Update()
    {
        //SceneView.get
        //SceneView.GetAllSceneCameras
        //SceneView.RepaintAll();
        //HostView.Invoke()
    }

    GameObject mainCameraObj;
    
    public void DragMainCamera(Vector2 delta)
    {
        if (mainCameraObj == null)
        {
            mainCameraObj = GameObject.Find("Main Camera");
        }
        
        if (mainCameraObj == null || sceneData == null)
        {
            return;
        }
        
        Vector3 pos = mainCameraObj.transform.localPosition;
        pos.x += delta.x;
        pos.z += delta.y;
        mainCameraObj.transform.localPosition = pos;
        sceneData.animCameraPostion.target = pos;
    }
  
    /*
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(100, 0, 300, 100));
        //if (_setButton)
        {
           if (GUILayout.Button("TEST BUTTON"))
           {
              //_setButton = false;  
              Debug.Log(Event.current.ToString());         
           }
           GUILayout.EndArea();
        }
        
        if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
        {
            EditorUtility.SetDirty(this); // this is important, if omitted, "Mouse down" will not be display
        }
        else if (Event.current.type == EventType.mouseDrag)
        {
            DragMainCamera(Event.current.delta);
        }
        else 
        {
            Debug.Log(Event.current.ToString());
        }
    }
    */

    void OnDrawGizmos()
    {
        if (null == sceneData)
        {
            return;
        }

        //Gizmos.matrix = DSkillData.root.transform.localToWorldMatrix;
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        if(showgrid)
        DrawGirds(Color.yellow);

        if (ShowBlock || editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateBlockMaterial();
            // Apply the line material
            blockMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(sceneData._LogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(blockMesh, mat);
        }

        if (ShowGrass || editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateGrassMaterial();
            // Apply the line material
            grassMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(sceneData._LogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(grassMesh, mat);
        }

        if (ShowEcoSystem || editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateEcosystemMaterial();
            // Apply the line material
            ecosystemMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(sceneData._LogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(ecosystemMesh, mat);
        }

        if (ShowEventArea || editmode == SceneEditorMode.BLOCKLAYER)
        {
            CreateEventAreaMaterial();
            // Apply the line material
            eventAreaMaterial.SetPass(0);
            Matrix4x4 mat = Matrix4x4.TRS(sceneData._LogicPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            Graphics.DrawMeshNow(eventAreaMesh, mat);
        }

        DEntityInfo[] dinfos = DSceneEntitySelection.GetSelected();

        if(dinfos != null)
        {
            for (int i = 0;i < dinfos.Length; i++)
            {
                Handles.color = Color.yellow;
                var dinfo = dinfos[i];
                Handles.DrawSolidDisc(dinfo.position, Vector3.up, 0.1f);
            }
        }

        if (editmode == SceneEditorMode.BLOCKLAYER)
        {
            Event evt = Event.current;

            if ( (evt.alt || evt.shift) &&  SceneView.currentDrawingSceneView != null && SceneView.currentDrawingSceneView.camera != null )
            {
                Vector3 v3 = ScreenPickGround(SceneView.currentDrawingSceneView);
                Vector2 v2 = GetGridCenter(brushSize, v3.x, v3.z);

                Handles.color = Color.cyan;
                DrawRect(new Vector3(v2.x, 0, v2.y), new Vector3(brushSize * sceneData._GridSize.x, brushSize * sceneData._GridSize.y));
            }
        }
    }
}
#endif
