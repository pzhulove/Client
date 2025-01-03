using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))][ExecuteInEditMode]
public class SpriteRenderScale : MonoBehaviour
{
    public RectTransform target;
    public bool bNeedUpdate = false;
    SpriteRenderer render;
    Vector3 scale = Vector3.one;
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        if(bNeedUpdate)
        {
            InvokeRepeating("UpdateScale", 0, 0.333f);
        }
    }

    void UpdateScale()
    {
        if (target != null)
        {
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(target.transform);
            Vector3 targetSize = bounds.size;
            scale.x = targetSize.x / render.sprite.bounds.size.x;
            scale.y = targetSize.y / render.sprite.bounds.size.y;
            transform.localScale = scale;
        }
    }
}