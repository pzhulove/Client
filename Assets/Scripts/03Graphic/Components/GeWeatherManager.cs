using UnityEngine;
using System.Collections;

public enum EWeatherMode
{
    None,
    Rain,
    Wind,
    Snow,

    MaxModeNum,
}

public class GeWeatherManager : Singleton<GeWeatherManager>
{
    #region 方法
    public override void Init()
    {
        if(null == m_WeatherNode)
        {
            GameObject environment = GameObject.Find("Environment");
            if (null != environment)
            {
                m_WeatherNode = new GameObject("Weather");
                m_WeatherNode.transform.SetParent(environment.transform);
            }
        }

        int nWeatherEffNodeNum = (int)EWeatherMode.MaxModeNum;
        for (int i = 0; i < nWeatherEffNodeNum; ++i)
        {
            if (i == (int)EWeatherMode.None)
            {
                m_WeatherEffNodeTbl[i] = null;
                continue;
            }

            m_WeatherEffNodeTbl[i] = AssetLoader.instance.LoadResAsGameObject(m_WeatherResTable[i],false);
            if(null == m_WeatherEffNodeTbl[i])
                continue;

            GeMeshRenderManager.instance.AddMeshObject(m_WeatherEffNodeTbl[i]);

            m_WeatherEffNodeTbl[i].SetActive(false);
            if (null != m_WeatherNode)
                m_WeatherEffNodeTbl[i].transform.SetParent(m_WeatherNode.transform);
        }
    }

    public void Deinit()
    {
        int nWeatherEffNodeNum = (int)EWeatherMode.MaxModeNum;
        for (int i = 0; i < nWeatherEffNodeNum; ++i)
            GameObject.Destroy(m_WeatherEffNodeTbl[i]);
    }

    public void ChangeWeather(EWeatherMode weatherMode)
    {
        if (null != m_CurEffNode)
            m_CurEffNode.SetActive(false);

        int effectIdx = (int)weatherMode;
        if (effectIdx < (int)EWeatherMode.MaxModeNum)
            m_CurEffNode = m_WeatherEffNodeTbl[effectIdx];

        if (null != m_CurEffNode)
            m_CurEffNode.SetActive(true);
    }

    #endregion

    #region 变量
    protected readonly string[] m_WeatherResTable = new string[]
    {
        "",
        "Effects/Scene_effects/Weather/Rain/FX_Rain",
        "Effects/Scene_effects/Weather/Wind/FX_Wind",
        "Effects/Scene_effects/Weather/Snow/FX_Snow",
    };

    protected GameObject m_WeatherNode = null;
    protected GameObject m_CurEffNode = null;
    protected GameObject[] m_WeatherEffNodeTbl = new GameObject[(int)EWeatherMode.MaxModeNum];

    #endregion
}
