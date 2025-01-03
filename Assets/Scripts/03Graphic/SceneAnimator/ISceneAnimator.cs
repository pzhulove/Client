public interface ISceneAnimator
{
    /// <summary>
    /// 改变UV动画的速度
    /// </summary>
    /// <param name="speedRate">速率，动画的最终速度=Prefab的预设值*速率</param>
    /// <param name="duration">动画速度改变到指定速度的时间</param>
    void SetSpeed(float speedRate, float duration);
}
