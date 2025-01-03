using UnityEngine;

public class RotateAnimation
{
    Vector3 m_RotateSpeed;

    Vector3 m_CurrentAngle;

    Vector3 m_CurrentSpeed;                                   //当前旋转的速度
    Vector3 m_BeginSpeed;                                     //旋转开始时的速度
    Vector3 m_TargetSpeed;                                    //旋转目标速度
    float m_AccelerateTime;                                 //从当前速度加速到目标速度的总时间
    float m_AccelerateElaspedTime = float.MaxValue;         //从当前速度加速到目标速度经过的时间
    bool m_IsAccelerate;

    Transform m_Transform;

    public RotateAnimation(Transform transform, Vector3 rotateSpeed)
    {
        m_Transform = transform;
        m_RotateSpeed = rotateSpeed;
        m_CurrentAngle = transform.localRotation.eulerAngles;
    }

    public void SetSpeed(float speedRate, float duration)
    {
        m_IsAccelerate = true;
        m_AccelerateElaspedTime = 0f;
        m_AccelerateTime = duration;
        m_BeginSpeed = m_CurrentSpeed;
        m_TargetSpeed = m_RotateSpeed * speedRate;
    }

    public void Update(float deltaTime)
    {
        if (m_IsAccelerate)
        {
            m_AccelerateElaspedTime += deltaTime;
            m_CurrentSpeed = Vector3.Lerp(m_BeginSpeed, m_TargetSpeed, m_AccelerateElaspedTime / m_AccelerateTime);
            if (m_AccelerateElaspedTime >= m_AccelerateTime)
            {
                m_IsAccelerate = false;
                m_CurrentSpeed = m_TargetSpeed;
            }
        }

        // 旋转
        if (m_CurrentSpeed != Vector3.zero)
        {
            m_CurrentAngle += m_CurrentSpeed * deltaTime;
            m_CurrentAngle.x = m_CurrentAngle.x % 360;
            m_CurrentAngle.y = m_CurrentAngle.y % 360;
            m_CurrentAngle.z = m_CurrentAngle.z % 360;
            m_Transform.localRotation = Quaternion.Euler(m_CurrentAngle);
        }
    }
}
