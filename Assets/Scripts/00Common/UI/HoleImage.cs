using UnityEngine;
using UnityEngine.UI;

public class HoleImage : Image
{
    private Rect m_HoleRect = new Rect(1000, 400, 600, 500);  // x: 距左边界距离; y: 距下边界距离; width: 宽; height: 高

    public void SetHoleRect(Rect rect)
    {
        // 参数校准，负数，宽高是否越界
        m_HoleRect = rect;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Vector2 pivot = rectTransform.pivot;
        //base.OnPopulateMesh(vh);
        vh.Clear();
        float imgW = rectTransform.sizeDelta.x;
        float imgH = rectTransform.sizeDelta.y;
        // 原4顶点
        UIVertex vertex0 = new UIVertex();
        vertex0.color = color;
        vertex0.position = new Vector2(imgW * -pivot.x, imgH * -pivot.y);
        vertex0.uv0 = Vector2.zero;
        UIVertex vertex1 = new UIVertex();
        vertex1.color = color;
        vertex1.position = new Vector2(imgW * -pivot.x, imgH * pivot.y);
        vertex1.uv0 = Vector2.up;
        UIVertex vertex2 = new UIVertex();
        vertex2.color = color;
        vertex2.position = new Vector2(imgW * pivot.x, imgH * pivot.y);
        vertex2.uv0 = Vector2.one;
        UIVertex vertex3 = new UIVertex();
        vertex3.color = color;
        vertex3.position = new Vector2(imgW * pivot.x, imgH * -pivot.y);
        vertex3.uv0 = Vector2.right;
        // 洞4顶点
        UIVertex vertex4 = new UIVertex();
        vertex4.color = color;
        vertex4.position = vertex0.position + new Vector3(m_HoleRect.x, m_HoleRect.y);
        vertex4.uv0 = new Vector2(vertex4.position.x / imgW + 0.5f, vertex4.position.y / imgH + 0.5f);
        UIVertex vertex5 = new UIVertex();
        vertex5.color = color;
        vertex5.position = vertex4.position + new Vector3(0, m_HoleRect.height);
        vertex5.uv0 = new Vector2(vertex5.position.x / imgW + 0.5f, vertex5.position.y / imgH + 0.5f);
        UIVertex vertex6 = new UIVertex();
        vertex6.color = color;
        vertex6.position = vertex4.position + new Vector3(m_HoleRect.width, m_HoleRect.height);
        vertex6.uv0 = new Vector2(vertex6.position.x / imgW + 0.5f, vertex6.position.y / imgH + 0.5f);
        UIVertex vertex7 = new UIVertex();
        vertex7.color = color;
        vertex7.position = vertex4.position + new Vector3(m_HoleRect.width, 0);
        vertex7.uv0 = new Vector2(vertex7.position.x / imgW + 0.5f, vertex7.position.y / imgH + 0.5f);
        vh.AddVert(vertex0);
        vh.AddVert(vertex1);
        vh.AddVert(vertex2);
        vh.AddVert(vertex3);
        vh.AddVert(vertex4);
        vh.AddVert(vertex5);
        vh.AddVert(vertex6);
        vh.AddVert(vertex7);
        vh.AddTriangle(0, 1, 4);
        vh.AddTriangle(4, 1, 5);
        vh.AddTriangle(1, 2, 5);
        vh.AddTriangle(5, 2, 6);
        vh.AddTriangle(2, 3, 6);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(3, 0, 7);
        vh.AddTriangle(7, 0, 4);
    }
}
