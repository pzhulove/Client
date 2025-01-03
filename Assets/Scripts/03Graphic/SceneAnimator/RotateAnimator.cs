using UnityEngine;

public class RotateAnimator : MonoBehaviour, ISceneAnimator
{
    [Tooltip("速度范围：0 ~ 360")]
    public Vector3 RotateSpeed;
    public void SetSpeed(float speedRate, float duration)
    {
        m_RotateAnimation.SetSpeed(speedRate, duration);
    }

    RotateAnimation m_RotateAnimation;
    private void Start()
    {
        m_RotateAnimation = new RotateAnimation(transform, RotateSpeed);
    }

    private void Update()
    {
        m_RotateAnimation.Update(Time.deltaTime);
    }

}
