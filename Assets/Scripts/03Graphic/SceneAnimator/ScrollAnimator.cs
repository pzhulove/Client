using UnityEngine;

public class ScrollAnimator : MonoBehaviour, ISceneAnimator
{
    [Tooltip("速度范围：-1 ~ 1")]
    public Vector2 ScrollSpeed;
    public void SetSpeed(float speedRate, float duration)
    {
        m_ScrollAnimation.SetSpeed(speedRate, duration);
    }

    ScrollAnimation m_ScrollAnimation;
    private void Start()
    {
        int UVOffsetID = Shader.PropertyToID("_MainTex");
        Material scrollMaterial = GetComponent<Renderer>().material;
        m_ScrollAnimation = new ScrollAnimation(scrollMaterial, UVOffsetID, ScrollSpeed);
    }

    private void Update()
    {
        m_ScrollAnimation.Update(Time.deltaTime);
    }
}
