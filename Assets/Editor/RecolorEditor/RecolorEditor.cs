using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RecolorEditor : EditorWindow
{
    //配置文件名
    private string m_RecolorDataName = "Unnamed";
    //需要染色的预制体
    private UnityEngine.Object m_PrefabObject;
    private Color32 m_CurrentTargetColor;
    private Color32 m_PreviousTargetColor;
    //染色区域索引
    private int m_RecolorAreaIndex = 0;
    private string m_CurrentMask = "";
    private bool m_ParamOpened = false;
    //染色区域
    private static readonly string[] m_MaskArea = new string[] { "区域1", "区域2", "区域3", "区域4", "区域5", "区域6", "区域7", "区域8" };
    private static readonly Color[] m_MaskColors = new Color[] {
        new Color(0, 0, 0, 0),
        new Color(1, 0, 0, 0),
        new Color(0, 1, 0, 0),
        new Color(1, 1, 0, 0),
        new Color(0, 0, 1, 0),
        new Color(1, 0, 1, 0),
        new Color(0, 1, 1, 0),
        new Color(1, 1, 1, 0)
    };

    private Color m_CurrMaskColor;
    private Matrix4x4[] m_MatrixArray = new Matrix4x4[8];
    //初始矩阵
    private Matrix4x4 m_OriginMatrix;
    private Material m_Material;
    //当前选中的源颜色
    private Color32 m_CurrentSourceColor;
    private string m_AssetPath = "";

    [MenuItem("[TM工具集]/ArtTools/染色系统编辑器")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<RecolorEditor>("染色系统编辑器");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    private void OnEnable()
    {
        _InitMatrixArrayData();
    }

    private void OnGUI()
    {
        m_RecolorDataName = EditorGUILayout.TextField("配置文件名:", m_RecolorDataName);
        m_PrefabObject = EditorGUILayout.ObjectField("染色预制体:", m_PrefabObject, typeof(GameObject), true);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("染色区域", GUILayout.Width(120)))
            _CreateDropDownList();

        if(string.Empty != m_CurrentMask)
        {
            m_PreviousTargetColor = m_CurrentTargetColor;
            m_CurrentTargetColor = EditorGUILayout.ColorField(new GUIContent("目标颜色："), m_CurrentTargetColor);
        }
        else
            GUILayout.Label("请选择一个染色区域！", EditorStyles.boldLabel);

        EditorGUILayout.EndHorizontal();

        if ((Color)m_CurrentTargetColor != m_PreviousTargetColor)
            _CalcMixChannelParam();

        m_ParamOpened = EditorGUILayout.Toggle("开启通道混合参数", m_ParamOpened);

        if(m_ParamOpened)
        {
            GUILayout.Space(4);
            GUILayout.Label("通道R混合参数:", EditorStyles.boldLabel);
            m_MatrixArray[m_RecolorAreaIndex].m00 = EditorGUILayout.Slider("通道R来源:", m_MatrixArray[m_RecolorAreaIndex].m00, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m01 = EditorGUILayout.Slider("通道G来源:", m_MatrixArray[m_RecolorAreaIndex].m01, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m02 = EditorGUILayout.Slider("通道B来源:", m_MatrixArray[m_RecolorAreaIndex].m02, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m03 = EditorGUILayout.Slider("常数K来源:", m_MatrixArray[m_RecolorAreaIndex].m03, -2.0f, 2.0f);
            GUILayout.Space(4);
            GUILayout.Label("通道G混合参数:", EditorStyles.boldLabel);
            m_MatrixArray[m_RecolorAreaIndex].m10 = EditorGUILayout.Slider("通道R来源:", m_MatrixArray[m_RecolorAreaIndex].m10, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m11 = EditorGUILayout.Slider("通道G来源:", m_MatrixArray[m_RecolorAreaIndex].m11, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m12 = EditorGUILayout.Slider("通道B来源:", m_MatrixArray[m_RecolorAreaIndex].m12, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m13 = EditorGUILayout.Slider("常数K来源:", m_MatrixArray[m_RecolorAreaIndex].m13, -2.0f, 2.0f);

            GUILayout.Space(4);
            GUILayout.Label("通道B混合参数:", EditorStyles.boldLabel);
            m_MatrixArray[m_RecolorAreaIndex].m20 = EditorGUILayout.Slider("通道R来源:", m_MatrixArray[m_RecolorAreaIndex].m20, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m21 = EditorGUILayout.Slider("通道G来源:", m_MatrixArray[m_RecolorAreaIndex].m21, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m22 = EditorGUILayout.Slider("通道B来源:", m_MatrixArray[m_RecolorAreaIndex].m22, -2.0f, 2.0f);
            m_MatrixArray[m_RecolorAreaIndex].m23 = EditorGUILayout.Slider("常数K来源:", m_MatrixArray[m_RecolorAreaIndex].m23, -2.0f, 2.0f);
            GUILayout.Space(10);
        }

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("还原当前染色", GUILayout.Width(120)))
            m_MatrixArray[m_RecolorAreaIndex] = m_OriginMatrix;

        if (GUILayout.Button("保存配置", GUILayout.Width(100)))
        {
            if(null == m_PrefabObject)
            {
                EditorUtility.DisplayDialog("提示", "预制体是空的！", "好的");
                return;
            }
            _CreateAndSaveRecolorData(m_MatrixArray, m_RecolorDataName);
        }

        EditorGUILayout.EndHorizontal();
    }

    
    private void Update()
    {
        int curTime = (int)Time.realtimeSinceStartup * 1000;

        if (null != m_Material)
            m_Material.SetFloat("_CurrentTime", curTime);

        SceneView.RepaintAll();
        _SetMaterialData();
    }

    private void OnDisable()
    {
        if (null != m_Material)
        {
            if (m_Material.IsKeywordEnabled("_DYECOLOR_ON"))
                m_Material.DisableKeyword("_DYECOLOR_ON");

            if (m_Material.IsKeywordEnabled("_HIGHLIGHTCOLOR"))
                m_Material.DisableKeyword("_HIGHLIGHTCOLOR");

            m_Material = null;
        }

        if (null != m_PrefabObject)
            m_PrefabObject = null;
    }


    //初始化矩阵数组数据
    private void _InitMatrixArrayData()
    {
        m_OriginMatrix.m00 = 1.0f;
        m_OriginMatrix.m01 = 0;
        m_OriginMatrix.m02 = 0;
        m_OriginMatrix.m03 = 0;

        m_OriginMatrix.m10 = 0;
        m_OriginMatrix.m11 = 1.0f;
        m_OriginMatrix.m12 = 0;
        m_OriginMatrix.m13 = 0;

        m_OriginMatrix.m20 = 0;
        m_OriginMatrix.m21 = 0;
        m_OriginMatrix.m22 = 1.0f;
        m_OriginMatrix.m23 = 0;

        for (int i = 0, icnt = m_MaskArea.Length; i < icnt; ++i)
            m_MatrixArray[i] = m_OriginMatrix;
    }

    //把数据传入材质的矩阵数组
    private void _SetMaterialData()
    {
        GameObject prefab = m_PrefabObject as GameObject;

        if (null != prefab)
        {
            Renderer renderer = prefab.GetComponentInChildren<Renderer>();

            if(null == m_Material)
                m_Material = renderer.sharedMaterial;

            m_Material.EnableKeyword("_DYECOLOR_ON");

            m_Material.SetMatrixArray("_MixingMatrices", m_MatrixArray);

        }
    }

    //保存并创建染色数据配置文件
    private void _CreateAndSaveRecolorData(Matrix4x4[] matrixArray, string name)
    {
        DRecolorData recolorData = ScriptableObject.CreateInstance<DRecolorData>();
        recolorData.Save(matrixArray);
        m_AssetPath = EditorUtility.SaveFilePanelInProject("SaveRecolorData", name, "asset", "Please input recolor data name.", "Assets/Resources/");
        if (!string.IsNullOrEmpty(m_AssetPath))
        {
            AssetDatabase.CreateAsset(recolorData, m_AssetPath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(recolorData);
        }
    }

    //基本思想是改变最少的参数 得到目标颜色值
    //ret对应颜色通用道的默认值是 r[1,0,0], g[0,1,0], b[0,0,1] 
    //1.当目标值比源染色值总和的2倍（三个参数的最大值）还要大 就把每个参数都设置为2, 常数K补齐减去最大值的部分
    //2.当目标值小于源颜色值 则 源颜色值*r(比例) = 目标值
    //3.当目标值大于源颜色值 则 把目标值 从大到小依次分配到三个参数，如果大的分配不完则继续向小的分配（三个参数和源颜色值对应位置相乘再相加等于目标值, 先把最大值的部分分配完，再把剩余的部分分配到其他位置） 
    //计算单个通道的通道参数
    private List<float> _CalculateTargetColor(List<int> srcColor, int targetValue, int targetIndex)
    {
        List<float> ret = new List<float>{ 0.0f, 0.0f, 0.0f};
        ret[targetIndex] = 1.0f;

        List<int> sColor2 = new List<int>();
        for(int j = 0, jcnt = srcColor.Count; j < jcnt; ++j )
            sColor2.Insert(j, srcColor[j]);

        sColor2.Sort();

        int maxLimt = 0;

        for (int i = 0, icnt = sColor2.Count; i < icnt; ++i)
            maxLimt += sColor2[i];

        maxLimt = 2 * maxLimt;

        if (targetValue > maxLimt)
            return new List<float> { 2.0f, 2.0f, 2.0f, (float)((targetValue - maxLimt) / 255.0) };

        int curColor = srcColor[targetIndex];

        if(curColor < targetValue)
        {
            for(int i = 0, icnt = sColor2.Count; i < icnt; ++ i)
            {
                int value = sColor2[2 - i];
                int retIdx = srcColor.IndexOf(value);
                int diff = targetValue - curColor;
                float mRatio = 2.0f;

                if (retIdx == targetIndex)
                    mRatio = 1.0f;

                float r = (float)diff / sColor2[2 - i];

                if (r > mRatio)
                {
                    curColor += (int)(mRatio * sColor2[2 - i]);
                    ret[retIdx] = 2.0f;
                }
                else
                {
                    ret[retIdx] += r;
                    break;
                }

            }
        }
        else
        {
            if(curColor > targetValue)
            {
                float r = targetValue / curColor;
                ret[targetIndex] = r;
            }
        }

        ret.Add(0.0f);

        return ret;
    }

    //计算通用混合算法的通道参数
    private void _CalcMixChannelParam()
    {
        List<int> srcColor = new List<int>();
        srcColor.Add(m_CurrentSourceColor.r);
        srcColor.Add(m_CurrentSourceColor.g);
        srcColor.Add(m_CurrentSourceColor.b);
        List<float> ret1 = new List<float>();
        List<float> ret2 = new List<float>();
        List<float> ret3 = new List<float>();

        ret1 = _CalculateTargetColor(srcColor, m_CurrentTargetColor.r, 0);
        ret2 = _CalculateTargetColor(srcColor, m_CurrentTargetColor.g, 1);
        ret3 = _CalculateTargetColor(srcColor, m_CurrentTargetColor.b, 2);

        m_MatrixArray[m_RecolorAreaIndex].m00 = ret1[0];
        m_MatrixArray[m_RecolorAreaIndex].m01 = ret1[1];
        m_MatrixArray[m_RecolorAreaIndex].m02 = ret1[2];
        m_MatrixArray[m_RecolorAreaIndex].m03 = ret1[3];

        m_MatrixArray[m_RecolorAreaIndex].m10 = ret2[0];
        m_MatrixArray[m_RecolorAreaIndex].m11 = ret2[1];
        m_MatrixArray[m_RecolorAreaIndex].m12 = ret2[2];
        m_MatrixArray[m_RecolorAreaIndex].m13 = ret2[3];

        m_MatrixArray[m_RecolorAreaIndex].m20 = ret3[0];
        m_MatrixArray[m_RecolorAreaIndex].m21 = ret3[1];
        m_MatrixArray[m_RecolorAreaIndex].m22 = ret3[2];
        m_MatrixArray[m_RecolorAreaIndex].m23 = ret3[3];
    }


    //创建染色区域下拉菜单
    private void _CreateDropDownList()
    {
        GenericMenu genericMenu = new GenericMenu();

        for(int i = 0, icnt = m_MaskArea.Length; i < icnt; ++i)
            genericMenu.AddItem(new GUIContent(m_MaskArea[i]), m_CurrentMask.Equals(m_MaskArea[i]), _OnSelected, m_MaskArea[i]);

        genericMenu.ShowAsContext();
    }

    private void _OnSelected(object target)
    {
        if (null != m_Material)
        {
            m_CurrentMask = target as string;

            for (int i = 0, icnt = m_MaskArea.Length; i < icnt; ++i)
                if (m_MaskArea[i] == m_CurrentMask)
                    m_RecolorAreaIndex = i;

            m_Material.SetColor("_TargetMaskColor", m_MaskColors[m_RecolorAreaIndex]);
            //切换染色区域时记录的时间
            int startTime = (int)Time.realtimeSinceStartup * 1000;
            m_Material.SetFloat("_StartTime", startTime);
            m_Material.EnableKeyword("_HIGHLIGHTCOLOR");

            _GetCurrentPickedColor();
        }
        else
            EditorUtility.DisplayDialog("提示", "预制体是空的！", "好的");
    }

    //获取临时可读的Texture
    private Texture2D _GetReadableTexture(string textureName)
    {
        Texture currentTexture = m_Material.GetTexture(textureName);
        RenderTexture tmpRenderTexture = RenderTexture.GetTemporary(
        currentTexture.width,
        currentTexture.height,
        0,
        RenderTextureFormat.Default,
        RenderTextureReadWrite.Linear
        );

        Graphics.Blit(currentTexture, tmpRenderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmpRenderTexture;
        Texture2D tmpTexture = new Texture2D(currentTexture.width, currentTexture.height);
        tmpTexture.ReadPixels(new Rect(0, 0, tmpRenderTexture.width, tmpRenderTexture.height), 0, 0);
        tmpTexture.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmpRenderTexture);
        return tmpTexture;
    }

    //获取遮罩区域下所有颜色的平均值
    private Color _GetTextureColor(Texture2D masktexture, Texture2D albedoTexture, Color curMaskcolor)
    {
        Color curColor = new Color(0, 0, 0);

        if(null!= masktexture && null != albedoTexture)
        {
            int width = masktexture.width;
            int height = masktexture.height;
            List<Color> colorList = new List<Color>();

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Color maskColor = masktexture.GetPixel(x, y);

                    if (maskColor == curMaskcolor)
                    {
                        Color albedoColor = albedoTexture.GetPixel(x, y);
                        colorList.Add(albedoColor);
                    }
                }
            }

            int count = colorList.Count;

            for (int i = 0; i < count; ++i)
            {
                curColor += colorList[i];
            }

            curColor /= count;

            DestroyImmediate(masktexture);
            DestroyImmediate(albedoTexture);
        }

        return curColor;
    }

    //获取当前染色区域的原颜色
    private void _GetCurrentPickedColor()
    {
        Texture2D maskTexture = _GetReadableTexture("_Mask");
        Texture2D albedoTexture = _GetReadableTexture("_Albedo");
        m_CurrentSourceColor = _GetTextureColor(maskTexture, albedoTexture, m_MaskColors[m_RecolorAreaIndex]);
    }
}
