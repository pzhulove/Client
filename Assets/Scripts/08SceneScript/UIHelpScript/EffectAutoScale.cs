using UnityEngine;
using System.Collections;
[ExecuteAlways]
public class EffectAutoScale : MonoBehaviour
{
    public float fOffsetScaleX = 0.0f;
    public float fOffsetScaleY = 0.0f;
    public float fScaleX = 1.0f;
    public float fScaleY = 1.0f;
    public float fScaleBase = 100.0f;
    public RectTransform target;

    float fRadio = 0.01f;

    void Start()
    {
        AutoScale();
    }

    void AutoScale()
    {
        if (target != null)
        {
            fRadio = 1.0f / fScaleBase * 10.0f;
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(target.transform);

            float fTargetScaleX = bounds.size.x * fRadio * 0.50f / fScaleX;
            float fTargetScaleY = bounds.size.y * fRadio * 0.50f / fScaleY;
            transform.localScale = new Vector3(fTargetScaleX, fTargetScaleY, 1.0f);
            transform.localPosition = new Vector3(-bounds.size.x * 0.50f + bounds.size.x * fOffsetScaleX, -bounds.size.y * 0.50f + bounds.size.y * fOffsetScaleY, transform.localPosition.z);
        }
    }

    void Update()
    {
        AutoScale();
    }
}