using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MyImage
{
    public Vec3 position;

    public float passedTime;

    //实现效果：同一个单位产生同一种类型的Text后，之前的往上移动
    public int actorID;

    public int hitEffectType;

    public int animCurveIndex;

    public MyImage(Vec3 position,float passedTime,int actorID,int hitEffectType,int animCurveIndex)
    {
        this.position = position;
        this.passedTime = passedTime;
        this.actorID = actorID;
        this.hitEffectType = hitEffectType;
        this.animCurveIndex = animCurveIndex;
    }
}

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ImageRenderer : MonoBehaviour
{
    public Sprite sprite;

    public Material material;

    public float imageXOffset;

    public float imageYOffset;

    public AnimationCurve[] moveXCurves;

    public AnimationCurve[] moveYCurves;

    public AnimationCurve[] fadeCurves;

    public AnimationCurve[] scaleCurves;

    public float lifeTime;

    public int Max_Num;

    //在同一个单位添加同种类型Text往上移动的距离
    private float moveUpOffset = 15f;

    private Mesh m_Mesh;

    private MeshFilter m_MeshFilter;

    private MeshRenderer m_MeshRenderer;

    private List<MyImage> m_Images;

    //Mesh数据
    private Vector2[] m_SpriteVertices;

    private Vector2[] m_SpriteUVs;

    private ushort[] m_SpriteTriangles;

    private List<Vector3> m_Vertices;

    private List<Vector3> m_OriginVertices;

    private List<Vector2> m_UVs;

    private List<int> m_Triangles;

    private List<Color> m_Colors;

    private RectTransform m_RectTransform;

    private int m_CurrentImageNum;

    private static int m_DefaultCapacity = 12;

    public void Init()
    {
        m_Mesh = new Mesh();

        m_MeshFilter = GetComponent<MeshFilter>();

        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_RectTransform = GetComponent<RectTransform>();

        m_MeshFilter.mesh = m_Mesh;

        m_MeshRenderer.material = material;

        m_MeshRenderer.material.mainTexture = sprite.texture;

        m_Images = new List<MyImage>(m_DefaultCapacity);

        m_Vertices = new List<Vector3>(m_DefaultCapacity * 4);

        m_OriginVertices = new List<Vector3>(m_DefaultCapacity * 4);

        m_UVs = new List<Vector2>(m_DefaultCapacity * 4);

        m_Triangles = new List<int>(m_DefaultCapacity * 6);

        m_Colors = new List<Color>(m_DefaultCapacity * 4);
        
        GenerateSimpleSprite(true);

        m_CurrentImageNum = 0;
    }

    public void AddImage(Vec3 position, int actorID, int hitEffectType, int animCurveIndex)
    {
        UnityEngine.Profiling.Profiler.BeginSample("AATextAddImage");

        m_Images.Add(new MyImage(position, 0.0f, actorID, hitEffectType, animCurveIndex));

        for(int i = 0;i < m_SpriteVertices.Length;++i)
        {
            m_Vertices.Add(m_SpriteVertices[i]);
            m_OriginVertices.Add(m_SpriteVertices[i]);

            m_UVs.Add(m_SpriteUVs[i]);

            m_Colors.Add(new Color(1, 1, 1, 1));
        }

        for (int i = 0; i < m_SpriteTriangles.Length; ++i)
        {
            m_Triangles.Add(m_SpriteTriangles[i] + m_CurrentImageNum * m_SpriteVertices.Length);
        }

        m_CurrentImageNum++;

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void MoveUpAll(int actorID, int animType)
    {
        UnityEngine.Profiling.Profiler.BeginSample("AATextMoveUpAllImage");

        for (int i = 0; i < m_Images.Count; ++i)
        {
            var image = m_Images[i];
            if (image.actorID == actorID && image.hitEffectType == animType)
            {
                m_Images[i] = new MyImage(new Vec3(image.position.x, image.position.y + moveUpOffset, image.position.z),
                                                   image.passedTime, image.actorID, image.hitEffectType, image.animCurveIndex);
            }
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void UpdateMesh()
    {
        UnityEngine.Profiling.Profiler.BeginSample("AATextUpdateImage");

        //Remove
        int removeImageNum = 0;
        int removeStartIndex = 0;

        //删除生命周期到的Image
       // for (int i = m_Images.Count - 1;i >= 0;--i)
        for(int i = 0;i < m_Images.Count;++i)
        {
            if(m_Images[i].passedTime >= lifeTime)
            {
                removeImageNum++;
            }
        }

        //超过数量限制，移除开始添加的text
        int overFlowCount = m_Images.Count - Max_Num;
        if(Max_Num != 0 && overFlowCount > 0)
        {
            //             for(int i = overFlowCount - 1 + removeImageNum;i >= 0;--i)
            //             {
            //                 removeImageNum++;
            //             }
            int removeImageNumCache = removeImageNum;
            for(int i = removeImageNumCache;i < removeImageNumCache + overFlowCount;++i)
            {
                removeImageNum++;
            }
        }

        if(removeImageNum != 0)
        {
            m_CurrentImageNum -= removeImageNum;

            m_Vertices.RemoveRange(removeStartIndex * 4, removeImageNum * 4);
            m_OriginVertices.RemoveRange(removeStartIndex * 4, removeImageNum * 4);
            m_UVs.RemoveRange(removeStartIndex * 4, removeImageNum * 4);
            m_Colors.RemoveRange(removeStartIndex * 4, removeImageNum * 4);

            m_Images.RemoveRange(removeStartIndex, removeImageNum);

            int triangleNums = m_Triangles.Count;
            m_Triangles.RemoveRange(triangleNums - removeImageNum * 6, removeImageNum * 6);
        }

        for (int i = 0;i < m_Images.Count;++i)
        {
            var image = m_Images[i];

            float tempMoveX = moveXCurves[image.animCurveIndex].Evaluate(image.passedTime);
            float tempMoveY = moveYCurves[image.animCurveIndex].Evaluate(image.passedTime);
            float tempScale = scaleCurves[image.animCurveIndex].Evaluate(image.passedTime);
            float tempFade  = fadeCurves[image.animCurveIndex].Evaluate(image.passedTime);

            for(int j = i * 4;j < i * 4 + m_SpriteVertices.Length;++j)
            {
                var originVertex = m_OriginVertices[j];

                m_Vertices[j] = new Vector3(originVertex.x * tempScale + image.position.x + tempMoveX + imageXOffset,
                                            originVertex.y * tempScale + image.position.y + tempMoveY + imageYOffset,
                                            image.position.z);

                m_Colors[j] = new Color(1, 1, 1, 1 * tempFade);
            }

            m_Images[i] = new MyImage(image.position, image.passedTime + Time.deltaTime, image.actorID, image.hitEffectType, image.animCurveIndex);
        }

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

    void GenerateSimpleSprite(bool lPreserveAspect)
    {
        Vector4 v = GetDrawingDimensions(lPreserveAspect);
        var uv = (sprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;

        m_SpriteVertices = new Vector2[]{new Vector2(v.x,v.y),new Vector2(v.x,v.w),new Vector2(v.z,v.w),new Vector2(v.z,v.y) };
        m_SpriteUVs = new Vector2[] { new Vector2(uv.x, uv.y), new Vector2(uv.x, uv.w), new Vector2(uv.z, uv.w), new Vector2(uv.z, uv.y) };

        m_SpriteTriangles = new ushort[] { 0, 1, 2, 2, 3, 0 };
    }

    private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
    {
        var padding = sprite == null ? Vector4.zero : UnityEngine.Sprites.DataUtility.GetPadding(sprite);
        var size = sprite == null ? Vector2.zero : new Vector2(sprite.rect.width, sprite.rect.height);

        Rect r = m_RectTransform.rect;

        int spriteW = Mathf.RoundToInt(size.x);
        int spriteH = Mathf.RoundToInt(size.y);

        var v = new Vector4(
                padding.x / spriteW,
                padding.y / spriteH,
                (spriteW - padding.z) / spriteW,
                (spriteH - padding.w) / spriteH);

        if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
        {
            var spriteRatio = size.x / size.y;
            var rectRatio = r.width / r.height;

            if (spriteRatio > rectRatio)
            {
                var oldHeight = r.height;
                r.height = r.width * (1.0f / spriteRatio);
                r.y += (oldHeight - r.height) * m_RectTransform.pivot.y;
            }
            else
            {
                var oldWidth = r.width;
                r.width = r.height * spriteRatio;
                r.x += (oldWidth - r.width) * m_RectTransform.pivot.x;
            }
        }

        v = new Vector4(
                r.x + r.width * v.x,
                r.y + r.height * v.y,
                r.x + r.width * v.z,
                r.y + r.height * v.w
                );

        return v;
    }
}
