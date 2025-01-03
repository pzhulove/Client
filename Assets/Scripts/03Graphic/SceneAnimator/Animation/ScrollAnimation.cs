using UnityEngine;

public class ScrollAnimation
{
    Vector2 m_ScrollSpeed;

    Material m_ScrollMaterial;
    Vector2 m_CurrentUVOffset;

    Vector2 m_CurrentSpeed;                             //当前滚动的速度
    Vector2 m_BeginSpeed;                               //滚动开始时的速度
    Vector2 m_TargetSpeed;                              //目标速度
    float m_AccelerateTime;                             //从当前速度加速到目标速度的总时间
    float m_AccelerateElaspedTime = float.MaxValue;     //从当前速度加速到目标速度经过的时间
    bool m_IsAccelerate;

    int MainTexID;

    public ScrollAnimation(Material material, int paramID, Vector2 scrollSpeed)
    {
        m_ScrollMaterial = material;
        MainTexID = paramID;
        m_ScrollSpeed = scrollSpeed;
        m_ScrollMaterial.SetTextureOffset(MainTexID, m_CurrentUVOffset);
    }

    public void SetSpeed(float speedRate, float duration)
    {
        m_IsAccelerate = true;
        m_AccelerateElaspedTime = 0f;
        m_AccelerateTime = duration;
        m_BeginSpeed = m_CurrentSpeed;
        m_TargetSpeed = m_ScrollSpeed * speedRate;
    }

    public void Update(float deltaTime)
    {
        if (m_IsAccelerate)
        {
            m_AccelerateElaspedTime += deltaTime;
            m_CurrentSpeed = Vector2.Lerp(m_BeginSpeed, m_TargetSpeed, m_AccelerateElaspedTime / m_AccelerateTime);
            if (m_AccelerateElaspedTime >= m_AccelerateTime)
            {
                m_IsAccelerate = false;
                m_CurrentSpeed = m_TargetSpeed;
            }
        }

        // 滚动
        if (m_CurrentSpeed != Vector2.zero)
        {
            m_CurrentUVOffset += m_CurrentSpeed * deltaTime;
            m_CurrentUVOffset.x = m_CurrentUVOffset.x % 1f;
            m_CurrentUVOffset.y = m_CurrentUVOffset.y % 1f;
            m_ScrollMaterial.SetTextureOffset(MainTexID, m_CurrentUVOffset);
        }
    }
}
