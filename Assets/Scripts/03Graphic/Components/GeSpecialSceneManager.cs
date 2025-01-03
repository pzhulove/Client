using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeSpecialSceneManager  {
    #region 加载玄幻场景
    private string scenePath = "";
    private int time = 0;
    private int reversTime = 0;
    private int curReversTime = 0;
    private GameObject scenePrefab;
    private MeshRenderer[] renders = null;
    private bool reverse = false;
    private float maxValue = 0;
    private GeSceneEx geScene = null;
    /// <summary>
    /// 加载魔幻场景
    /// </summary>
    /// <param name="_scenePath">魔幻场景的路径</param>
    /// <param name="_time">魔幻场景变换过程的时间</param>
    /// <param name="_reversTime">魔幻场景消失时间</param>
    /// <param name="maxValue">材质上面的值</param>
    public void LoadMagicScene(string _scenePath, int _time, int _reversTime, float maxValue,GeSceneEx scene)
    {
        geScene = scene;
        this.scenePath = _scenePath;
        this.time = _time;
        this.reversTime = _reversTime;
        this.maxValue = maxValue;
        LoadMagicScene();
    }
    private void LoadMagicScene()
    {
        scenePrefab = AssetLoader.instance.LoadResAsGameObject(scenePath);
        if (scenePrefab != null && geScene!=null)
        {
            Utility.AttachTo(scenePrefab, geScene.GetSceneRoot());
            renders = scenePrefab.GetComponentsInChildren<MeshRenderer>();
        }
    }
    private int timer = 0;
    public  void Update(int deltaTime)
    {
        if (renders == null || scenePrefab == null || geScene ==null) return;
        if (reverse)
        {
            curReversTime += deltaTime;
            if (curReversTime > reversTime)
            {
                DestroyScene();
            }
            else
            {
                TransformScenePrefab(true);
            }
        }
        else
        {
            if (timer < time)
            {
                timer += deltaTime;
                TransformScenePrefab();
                reversStartValue = mainValue;
            }
        }
    }
    private float mainValue = 0;
    private float reversStartValue = 0;
    private void TransformScenePrefab(bool revers = false)
    {
        if (renders != null)
        {
            for (int i = 0; i < renders.Length; i++)
            {
                MeshRenderer render = renders[i];

                if (revers)
                {
                    mainValue = reversStartValue + (1.3f - reversStartValue) * ((float)curReversTime / reversTime);
                }
                else
                {
                    mainValue = maxValue - maxValue * (timer / (float)time);
                }
                mainValue = Mathf.Clamp(mainValue, 0, maxValue);
                render.sharedMaterial.SetFloat("_MainValue", mainValue);
            }
        }
    }
    public void DestroyScene()
    {
        if (scenePrefab != null)
        {
            GameObject.Destroy(scenePrefab);
            scenePrefab = null;
            renders = null;
        }
    }
    public void ReverseScene()
    {
        reverse = true;
    }

    public void Deinit()
    {
        DestroyScene();
    }
    #endregion

}
