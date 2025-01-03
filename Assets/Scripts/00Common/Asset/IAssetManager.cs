using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IAssetManager
{
    /// <summary>
    /// 加载游戏运行是对象(该过程包含实例化)
    /// </summary>
    /// <param name="strResPath"></param>
    /// <returns></returns>
    GameObject GetGameObject(string strResPath, Vector3 pos, Quaternion rot, enResourceType resourceType);
    GameObject GetGameObject(string strResPath, Vector3 pos, enResourceType resourceType);

    /// <summary>
    /// 添加预加载对象
    /// </summary>
    /// <param name="strResPath"></param>
    void AddPreloadObject(string strResPath);

    /// <summary>
    /// 同步加载预加载对象
    /// </summary>
    /// <returns></returns>
    bool ProcessPreload();

    /// <summary>
    /// 异步加载预加载对象
    /// </summary>
    /// <returns></returns>
    IEnumerator StepProcessPreload();

    /// <summary>
    /// 同步模式加载场景
    /// </summary>
    /// <param name="strScene"></param>
    void LoadScene(string strScene);

    /// <summary>
    /// 异步模式加载场景
    /// </summary>
    /// <param name="strScene"></param>
    /// <returns></returns>
    IEnumerator StepLoadScene(string strScene);
}

