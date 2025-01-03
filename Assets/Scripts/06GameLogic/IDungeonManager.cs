using UnityEngine;
using System.Collections;

public delegate void DungeonFadeCallback();

/// <summary>
/// 地下城相关获取接口
/// </summary>
public interface IDungeonManager
{
    BeScene CreateBeScene();

    void DestoryBeScene();

    BeScene GetBeScene();

    GeSceneEx GetGeScene();

    /// <summary>
    /// 这个需要改改
    /// </summary>
    /// <returns></returns>
    DungeonDataManager GetDungeonDataManager();

    void StartFight(bool isFinishFight = false);

    void FinishFight();

    bool IsFinishFight();

    void PauseFight(bool pauseAnimation = true, string tag = "", bool force = false);

    void ResumeFight(bool pauseAnimation = true, string tag = "", bool force = false);

    void OpenFade(DungeonFadeCallback fadein, DungeonFadeCallback load, DungeonFadeCallback fadeout, uint intime, uint outime);

    void Update(int delta);
    void UpdateGraphic(int delta);
    void DoGraphicBackToFront();
}
