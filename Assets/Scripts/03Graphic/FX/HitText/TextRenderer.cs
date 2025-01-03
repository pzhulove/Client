using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MyText
{
    public int characterCount;

    public float positionX;

    public float positionY;

    public float positionZ;

    public float passedTime;

    //实现效果：同一个单位产生同一种类型的Text后，之前的往上移动
    public int actorID;

    public int hitEffectType;

    public int animType;

    public int animCurveIndex;

    public int charIndex;

    public MyText(float positionX, float positionY, float positionZ, int characterCount, float passedTime, int actorID, int hitEffectType, int animType, int animCurveIndex, int charIndex)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.positionZ = positionZ;
        this.characterCount = characterCount;
        this.passedTime = passedTime;
        this.actorID = actorID;
        this.hitEffectType = hitEffectType;
        this.animType = animType;
        this.animCurveIndex = animCurveIndex;
        this.charIndex = charIndex;
    }
}

public struct MyTextVertices
{
    public Vector3 position0;
    public Vector3 position1;
    public Vector3 position2;
    public Vector3 position3;

    public Vector2 uv0;
    public Vector2 uv1;
    public Vector2 uv2;
    public Vector2 uv3;


    public float width;

    public MyTextVertices(Vector3 position0, Vector3 position1, Vector3 position2, Vector3 position3,
                            Vector2 uv0,Vector2 uv1,Vector2 uv2,Vector2 uv3)
    {
        this.position0 = position0;
        this.position1 = position1;
        this.position2 = position2;
        this.position3 = position3;

        this.uv0 = uv0;
        this.uv1 = uv1;
        this.uv2 = uv2;
        this.uv3 = uv3;

        this.width = position1.x - position0.x;
    }
}



[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TextRenderer : MonoBehaviour
{
    enum TextType
    {
        number,
        text
    }

    //类型
    [SerializeField]
    private TextType type = TextType.number;

    //字体显示设置
    [SerializeField]
    private Font font;

    [SerializeField]
    private int fontSize;

    [SerializeField]
    private float fontYOffset;

    [SerializeField]
    private float fontXOffset;

    [SerializeField]
    private Material fontMaterial;

    [SerializeField]
    private Color textColor;

    [SerializeField]
    private AnimationCurve[] moveXCurves;

    [SerializeField]
    private AnimationCurve[] moveYCurves;

    [SerializeField]
    private AnimationCurve[] fadeCurves;

    [SerializeField]
    private AnimationCurve[] scaleCurves;

    [SerializeField]
    private float lifeTime;

    //文字样式
    [SerializeField]
    private bool gradient;

    [SerializeField]
    private Color32 topColor;

    [SerializeField]
    private Color32 bottomColor;

    [SerializeField]
    private bool outLine;

    [SerializeField]
    private Color32 outLineColor;

    [SerializeField]
    private float outLineXOffset;

    [SerializeField]
    private float outLineYOffset;

    //Text最大数量
    [SerializeField]
    private int Max_Number;

    [SerializeField]
    public TextAnchor textAnchor;

    //在同一个单位添加同种类型Text往上移动的距离
    private float moveUpOffset = 15f;

    private List<MyText> m_Texts;

    private MeshFilter m_MeshFilter;

    private MeshRenderer m_MeshRenderer;

    private TextGenerator m_TextGenerator;

    private TextGenerationSettings m_TextGenerationSettings;

    //Mesh数据
    private Mesh m_Mesh;

    private List<Vector3> m_Vertices;

    private List<Vector2> m_UVs;

    private List<int> m_Triangles;

    private List<Color> m_Colors;

    //当前数字个数
    private int m_CurrentNumCount;

    private List<Vector3> m_OriginVertices;

    //list初始容量
    private static int m_DefaultCapacity = 12/*text数量*/ * 4/*text字数*/;

    //数字顶点缓存
    private MyTextVertices[] m_NumberVertexCache;

    private List<int> m_EveryNums;

    //文字缓存 只在显示汉字时缓存 用于在font texture重建后重新生成mesh
    private List<string> m_StringCache;

    //每个字拥有的顶点数，如果没有描边就是4，有描边就是36
    private int verticesNumPerChar;

    //每个字拥有的索引数，如果没有描边就是6，有描边就是54
    private int indicesNumPerChar;

    public void Init()
    {
        verticesNumPerChar = outLine ? 36 : 4;

        indicesNumPerChar = outLine ? 54 : 6;

        m_Mesh = new Mesh();

        m_Vertices = new List<Vector3>(m_DefaultCapacity * verticesNumPerChar);

        m_UVs = new List<Vector2>(m_DefaultCapacity * verticesNumPerChar);

        m_Triangles = new List<int>(m_DefaultCapacity * indicesNumPerChar);

        m_Colors = new List<Color>(m_DefaultCapacity * verticesNumPerChar);

        m_Texts = new List<MyText>(m_DefaultCapacity);

        m_OriginVertices = new List<Vector3>(m_DefaultCapacity * verticesNumPerChar);

        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_TextGenerator = new TextGenerator();

        m_MeshFilter.mesh = m_Mesh;
        m_MeshRenderer.material = fontMaterial;
        m_MeshRenderer.material.mainTexture = font.material.mainTexture;


        m_CurrentNumCount = 0;

        //文字顶点生成设置
        m_TextGenerationSettings.font = font;

        m_TextGenerationSettings.fontSize = fontSize;

        var rectTransform = gameObject.GetComponent<RectTransform>();

        Vector2 extents = rectTransform.rect.size;
        m_TextGenerationSettings.generationExtents = extents;

        if (font.dynamic)
        {
            m_TextGenerationSettings.scaleFactor = GameClient.ClientSystemManager.instance.Layer3DRoot.GetComponent<Canvas>().scaleFactor;
        }
        else
        {
            m_TextGenerationSettings.scaleFactor = 1;
        }
        m_TextGenerationSettings.textAnchor = textAnchor;
        m_TextGenerationSettings.alignByGeometry = false;
        m_TextGenerationSettings.pivot = rectTransform.pivot;


        if(type == TextType.number)
        {
            m_EveryNums = new List<int>(10);
            GenerateNumberVertices();
        }
        else if( type == TextType.text)
        {
            m_StringCache = new List<string>(12);
            Font.textureRebuilt += RebuildMesh;
        }

    }

    private void OnDestroy()
    {
        Font.textureRebuilt -= RebuildMesh;
    }

    public void MoveUpAll(int actorID, int hitEffectType, int animType)
    {
        UnityEngine.Profiling.Profiler.BeginSample("AATextMoveUpAllText");

        moveUpOffset = 15f;
        if (hitEffectType == 5)
            moveUpOffset = 30f;

        for (int i = 0; i < m_Texts.Count; ++i)
        {
            var text = m_Texts[i];
            if (text.actorID == actorID && text.hitEffectType == hitEffectType /*&& text.animType == animType*/)
            {
                m_Texts[i] = new MyText(text.positionX, text.positionY + moveUpOffset, text.positionZ,
                                                text.characterCount, text.passedTime,
                                                text.actorID, text.hitEffectType, text.animType, text.animCurveIndex, text.charIndex);
            }
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void AddText(int num, Vec3 position, int actorID, int hitEffectType, int animType, int animCurveIndex)
    {
        UnityEngine.Profiling.Profiler.BeginSample("AATextAddText");

        num = Mathf.Abs(num);

        int numLength = 0;

        m_EveryNums.Clear();
        do
        {
            m_EveryNums.Add(num % 10);
            num = num / 10;
            numLength++;
        } while (num != 0);

        float numWidth = 0.0f;
        foreach(var everyNum in m_EveryNums)
        {
            numWidth += m_NumberVertexCache[everyNum].width;
        }
        numWidth -= numLength;

        float lastWidth = 0.0f;
        float currentWidth = 0.0f;
        int count = 0;
        foreach(var everyNum in m_EveryNums)
        {
            var vertex = m_NumberVertexCache[everyNum];

            count++;
            currentWidth += vertex.width;

            m_Vertices.Add(new Vector3(vertex.position0.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position0.y,vertex.position0.z));
            m_Vertices.Add(new Vector3(vertex.position1.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position1.y, vertex.position1.z));
            m_Vertices.Add(new Vector3(vertex.position2.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position2.y, vertex.position2.z));
            m_Vertices.Add(new Vector3(vertex.position3.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position3.y, vertex.position3.z));
            m_OriginVertices.Add(new Vector3(vertex.position0.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position0.y, vertex.position0.z));
            m_OriginVertices.Add(new Vector3(vertex.position1.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position1.y, vertex.position1.z));
            m_OriginVertices.Add(new Vector3(vertex.position2.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position2.y, vertex.position2.z));
            m_OriginVertices.Add(new Vector3(vertex.position3.x - (currentWidth + lastWidth) / 2 + count + numWidth / 2, vertex.position3.y, vertex.position3.z));

            m_UVs.Add(vertex.uv0);
            m_UVs.Add(vertex.uv1);
            m_UVs.Add(vertex.uv2);
            m_UVs.Add(vertex.uv3);

            m_Colors.Add(textColor);
            m_Colors.Add(textColor);
            m_Colors.Add(textColor);
            m_Colors.Add(textColor);

            lastWidth += vertex.width;
        }

        m_Texts.Add(new MyText(position.x, position.y, position.z, numLength, 0.0f, actorID, hitEffectType, animType, animCurveIndex, m_CurrentNumCount));

        //添加索引
        for (int i = m_CurrentNumCount; i < m_CurrentNumCount + numLength; ++i)
        {
            int vertIndexStart = i * 4;
            int trianglesIndexStart = i * 6;

            m_Triangles.Add(vertIndexStart + 0);
            m_Triangles.Add(vertIndexStart + 1);
            m_Triangles.Add(vertIndexStart + 2);
            m_Triangles.Add(vertIndexStart + 2);
            m_Triangles.Add(vertIndexStart + 3);
            m_Triangles.Add(vertIndexStart + 0);
        }

        m_CurrentNumCount += numLength;

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void AddNameText(string content, Vec3 position, int actorID, int hitEffectType, int animType, int animCurveIndex)
    {
        UnityEngine.Profiling.Profiler.BeginSample("AATextAddNameText");

        m_TextGenerator.Populate(content, m_TextGenerationSettings);

        var verts = GamePool.ListPool<UIVertex>.Get();
        m_TextGenerator.GetVertices(verts);
        
        // 2019生成的字体没有多4个
#if UNITY_2019
        int vertCount = verts.Count;
#else
        int vertCount = verts.Count - 4;
#endif

        float topY = verts[0].position.y;
        float bottomY = verts[0].position.y;
        //添加顶点数据
        if (gradient)
        {
            for (int i = 1; i < vertCount; ++i)
            {
                float y = verts[i].position.y;

                if (topY < y)
                {
                    topY = y;
                }
                else if (bottomY > y)
                {
                    bottomY = y;
                }
            }
        }
        float elementHeight = topY - bottomY;

        if (outLine)
        {
            Color32 lerpResult;

            for (int i = 0; i < vertCount; ++i)
            {
                UIVertex vertex = verts[i];

                if (gradient)
                {
                    lerpResult = Color32.Lerp(bottomColor, topColor, (vertex.position.y - bottomY) / elementHeight);
                }
                else
                {
                    lerpResult = textColor;
                }

                m_Vertices.Add(new Vector3(vertex.position.x + outLineXOffset, vertex.position.y + outLineYOffset, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x + outLineXOffset, vertex.position.y + outLineYOffset, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x + outLineXOffset, vertex.position.y - outLineYOffset, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x + outLineXOffset, vertex.position.y - outLineYOffset, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x - outLineXOffset, vertex.position.y + outLineYOffset, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x - outLineXOffset, vertex.position.y + outLineYOffset, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x - outLineXOffset, vertex.position.y - outLineYOffset, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x - outLineXOffset, vertex.position.y - outLineYOffset, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x + outLineXOffset, vertex.position.y, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x + outLineXOffset, vertex.position.y, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x - outLineXOffset, vertex.position.y, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x - outLineXOffset, vertex.position.y, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x, vertex.position.y + outLineYOffset, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x, vertex.position.y + outLineYOffset, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x, vertex.position.y - outLineYOffset, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x, vertex.position.y - outLineYOffset, vertex.position.z));
                m_Colors.Add(outLineColor);
                m_UVs.Add(vertex.uv0);

                m_Vertices.Add(new Vector3(vertex.position.x, vertex.position.y, vertex.position.z));
                m_OriginVertices.Add(new Vector3(vertex.position.x, vertex.position.y, vertex.position.z));
                m_Colors.Add(lerpResult);
                m_UVs.Add(vertex.uv0);
            }
        }
        else
        {
            Color32 lerpResult;

            for (int i = 0; i < vertCount; ++i)
            {
                UIVertex vertex = verts[i];

                if (gradient)
                {
                    lerpResult = Color32.Lerp(bottomColor, topColor, (vertex.position.y - bottomY) / elementHeight);
                }
                else
                {
                    lerpResult = textColor;
                }

                m_Vertices.Add(vertex.position);
                m_OriginVertices.Add(vertex.position);
                m_Colors.Add(lerpResult);
                m_UVs.Add(vertex.uv0);
            }
        }

        GamePool.ListPool<UIVertex>.Release(verts);
        m_Texts.Add(new MyText(position.x, position.y, position.z, m_TextGenerator.characterCount - 1, 0.0f, actorID, hitEffectType, animType, animCurveIndex, m_CurrentNumCount));
        m_StringCache.Add(content);

        //添加索引数据
        if(outLine)
        {
            for (int i = m_CurrentNumCount; i < m_CurrentNumCount + m_TextGenerator.characterCount - 1; ++i)
            {
                int vertIndexStart = i * 4 * 9;
                int trianglesIndexStart = i * 6;

                for (int j = 0; j < 9; ++j)
                {
                    vertIndexStart = i * 4 * 9 + j;
                    m_Triangles.Add(vertIndexStart + 0);
                    m_Triangles.Add(vertIndexStart + 1 * 9);
                    m_Triangles.Add(vertIndexStart + 2 * 9);
                    m_Triangles.Add(vertIndexStart + 2 * 9);
                    m_Triangles.Add(vertIndexStart + 3 * 9);
                    m_Triangles.Add(vertIndexStart + 0);
                }
            }
        }
        else
        {
            for (int i = m_CurrentNumCount; i < m_CurrentNumCount + m_TextGenerator.characterCount - 1; ++i)
            {
                int vertIndexStart = i * 4;
                int trianglesIndexStart = i * 6;

                m_Triangles.Add(vertIndexStart + 0);
                m_Triangles.Add(vertIndexStart + 1);
                m_Triangles.Add(vertIndexStart + 2);
                m_Triangles.Add(vertIndexStart + 2);
                m_Triangles.Add(vertIndexStart + 3);
                m_Triangles.Add(vertIndexStart + 0);
            }
        }
   
        m_CurrentNumCount += m_TextGenerator.characterCount - 1;

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void UpdateMesh()
    {
        UnityEngine.Profiling.Profiler.BeginSample("AATextRemoveAt");
        int removeCharacterNum = 0;
        int removeTextNum = 0;
        int removeTextStartIndex = 0;

        //删除生命周期到的Text
        for(int i = 0;i < m_Texts.Count;++i)
        {
            var text = m_Texts[i];

            if (text.passedTime >= lifeTime)
            {
                m_CurrentNumCount -= text.characterCount;

                removeCharacterNum += text.characterCount;

                removeTextNum++;
            }
        }
        //超过数量限制，移除开始添加的text
        int overFlowCount = m_Texts.Count - Max_Number;
        if (Max_Number != 0 && overFlowCount > 0)
        {
            int removeTextNumCache = removeTextNum;
            for(int i = removeTextNumCache; i < removeTextNumCache + overFlowCount;++i)
            {
                var text = m_Texts[i];

                m_CurrentNumCount -= text.characterCount;

                removeCharacterNum += text.characterCount;

                removeTextNum++;
            }
        }

        if (removeCharacterNum != 0)
        {
            m_Vertices.RemoveRange(0, removeCharacterNum * verticesNumPerChar);
            m_OriginVertices.RemoveRange(0, removeCharacterNum * verticesNumPerChar);
            m_UVs.RemoveRange(0, removeCharacterNum * verticesNumPerChar);
            m_Colors.RemoveRange(0, removeCharacterNum * verticesNumPerChar);

            m_Texts.RemoveRange(removeTextStartIndex, removeTextNum);
            if(type == TextType.text)
                m_StringCache.RemoveRange(removeTextStartIndex, removeTextNum);

            int triangleNums = m_Triangles.Count;
            m_Triangles.RemoveRange(triangleNums - removeCharacterNum * indicesNumPerChar, removeCharacterNum * indicesNumPerChar);
        }

        for (int i = 0; i < m_Texts.Count; ++i)
        {
            var text = m_Texts[i];

            m_Texts[i] = new MyText(text.positionX,text.positionY,text.positionZ, text.characterCount, text.passedTime, text.actorID, text.hitEffectType, text.animType, text.animCurveIndex, text.charIndex - removeCharacterNum);
        }

        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("AATextForEachText");

        for (int i = 0; i < m_Texts.Count; ++i)
        {
            var text = m_Texts[i];

            //计算动画曲线的值
            float tempScale = scaleCurves[text.animCurveIndex].Evaluate(text.passedTime);
            float tempMoveX = moveXCurves[text.animCurveIndex].Evaluate(text.passedTime);
            float tempMoveY = moveYCurves[text.animCurveIndex].Evaluate(text.passedTime);
            float tempFade = fadeCurves[text.animCurveIndex].Evaluate(text.passedTime);

            UnityEngine.Profiling.Profiler.BeginSample("AAText1");
            //修改顶点数据
            int charIndex = text.charIndex;
            int charracterCount = text.characterCount;
            for (int j = charIndex * verticesNumPerChar; j < charIndex * verticesNumPerChar + charracterCount * verticesNumPerChar; j++)
            {
                var originVertex = m_OriginVertices[j];
                m_Vertices[j] = new Vector3(originVertex.x * tempScale + tempMoveX + fontXOffset + text.positionX,
                                            originVertex.y * tempScale + tempMoveY + fontYOffset + text.positionY,
                                            originVertex.z * tempScale + text.positionZ);
                var tempColor = m_Colors[j];
                m_Colors[j] = new Color(tempColor.r, tempColor.g, tempColor.b, tempFade);
            }
            UnityEngine.Profiling.Profiler.EndSample();

            m_Texts[i] = new MyText(text.positionX,text.positionY,text.positionZ, text.characterCount, text.passedTime + Time.deltaTime, text.actorID, text.hitEffectType, text.animType, text.animCurveIndex, text.charIndex);
        }
        UnityEngine.Profiling.Profiler.EndSample();


        UnityEngine.Profiling.Profiler.BeginSample("AATextSetUpMesh");
        if (m_Vertices.Count > 0)
        {
            m_Mesh.Clear();
            m_Mesh.SetVertices(m_Vertices);
            m_Mesh.SetUVs(0, m_UVs);
            m_Mesh.SetColors(m_Colors);
            m_Mesh.SetTriangles(m_Triangles, 0);
        }
        else
        {
            m_Mesh.Clear();
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    //预先生成每个数字的顶点
    private void GenerateNumberVertices()
    {
        m_NumberVertexCache = new MyTextVertices[10];

        for(int i = 0;i < 10;++i)
        {
            m_TextGenerator.Populate(i.ToString(), m_TextGenerationSettings);

            var verts = m_TextGenerator.verts;

            m_NumberVertexCache[i] = new MyTextVertices(verts[0].position, verts[1].position, verts[2].position, verts[3].position,
                                                        verts[0].uv0, verts[1].uv0, verts[2].uv0, verts[3].uv0);
        }
    }

    public void PreLoadFontCharacter(string preloadString)
    {
        font.RequestCharactersInTexture(preloadString, fontSize);
    }

    /// <summary>
    /// font texture 重建时 重新生成mesh
    /// </summary>
    private void RebuildMesh(Font font)
    {
        if (font != this.font)
            return;

        UnityEngine.Profiling.Profiler.BeginSample("AATextRebuildmesh");

        //生成一个不存在的字 防止textGenerator直接返回缓存的数据
        m_TextGenerator.Populate("龙减加印", m_TextGenerationSettings);

        for(int i = 0;i < m_StringCache.Count;++i)
        {
            m_TextGenerator.Populate(m_StringCache[i], m_TextGenerationSettings);

            IList<UIVertex> verts = m_TextGenerator.verts;

            MyText text = m_Texts[i];

            int count = 0;
            for(int iUV = text.charIndex * 4; iUV < text.charIndex * 4 + text.characterCount * 4; ++iUV)
            {
                if(outLine)
                {
                    m_UVs[iUV * 9] = verts[count].uv0;
                    m_UVs[iUV * 9 + 1] = verts[count].uv0;
                    m_UVs[iUV * 9 + 2] = verts[count].uv0;
                    m_UVs[iUV * 9 + 3] = verts[count].uv0;
                    m_UVs[iUV * 9 + 4] = verts[count].uv0;
                    m_UVs[iUV * 9+ 5] = verts[count].uv0;
                    m_UVs[iUV * 9 + 6] = verts[count].uv0;
                    m_UVs[iUV * 9 + 7] = verts[count].uv0;
                    m_UVs[iUV * 9 + 8] = verts[count].uv0;
                }
                else
                {
                    m_UVs[iUV] = verts[count].uv0;
                }
                count++;
            }
            
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
