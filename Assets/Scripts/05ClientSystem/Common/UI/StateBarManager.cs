using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClient;

class StateBarData
{
    public int id;
    public string text;
    public int time;
    public int duration;
    public bool reverse;
    public CStateBar.eBarColor color;

    public StateBarData(int id, string text, int duration, CStateBar.eBarColor color, bool reverse)
    {
        this.id = id;
        this.text = text;
        this.duration = duration;
        this.reverse = reverse;
        this.color = color;
        this.time = duration;
    }
}

public class StateBarManager
{
    public static int kInvalidStateBarId = -1;

    private int idAcc = 0;
    private List<StateBarData> listData = new List<StateBarData>();
    private int currentId = 0;
    private StateBarData currentData;
    private GameObject goStateBar;
    private CStateBar stateBar;

    public GeActorEx currentActor;

    public StateBarManager() { }

    public void CreateStateBar()
    {
        if (goStateBar == null)
        {
            string path = "UIFlatten/Prefabs/BattleUI/DungeonBar/StateBar";
            goStateBar = AssetLoader.instance.LoadResAsGameObject(path);
            if (goStateBar != null)
            {
                GameObject barRoot = _getStateBarRoot();
                if (barRoot != null)
                    Utility.AttachTo(goStateBar, barRoot);

                stateBar = goStateBar.GetComponent<CStateBar>();
            }
        }
    }

    private GameObject _getStateBarRoot()
    {
        GameClient.ClientSystemBattle system = GameClient.ClientSystemManager.GetInstance().TargetSystem as GameClient.ClientSystemBattle;

        if (null == system)
        {
            system = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemBattle;
        }

        if (null == system)
        {
            return null;
        }

        return system.MonsterBossRoot;
    }
    
    public void SetBarActive(bool active)
    {
        if (stateBar != null)
        {
            stateBar.SetActive(active);
        }
    }

    public void SetCurrentId(int id)
    {
        if (stateBar == null)
            return;

        if (currentId == id)
            return;

        currentId = id;
        currentData = null;

        if (currentId == kInvalidStateBarId)
        {
            SetBarActive(false);
            return;
        }

        for (int i = 0; i < listData.Count; i++)
        {
            if (listData[i].id == currentId)
            {
                currentData = listData[i];
                stateBar.SetStateBarInfo(currentData.text, currentData.color);
                break;
            }
        }
    }

    public int AddStateBar(string text, int duration, CStateBar.eBarColor color = CStateBar.eBarColor.Yellow, bool reverse = false)
    {
        var id = _getStateBarId();

        var data = new StateBarData(id, text, duration, color, reverse);

        listData.Add(data);

        SetCurrentId(id);

        return id;
    }

    private int _getStateBarId()
    {
        return ++idAcc;
    }

    public void RemoveStateBar(int id)
    {
        var data = listData.Find(x => { return x.id == id; });
        if (data != null)
        {
            listData.Remove(data);
            if (currentData != null && data.id == currentData.id)
            {
                SetCurrentId(kInvalidStateBarId);
            }
        }
    }

    public void Update(int deltaTime)
    {
        if (stateBar == null)
            return;

        for (int i = 0; i < listData.Count; i++)
        {
            var data = listData[i];
            if (data != null)
            {
                if (data.reverse)
                {
                    data.time += deltaTime;
                }
                else
                {
                    data.time -= deltaTime;
                }
            }
        }

        if (currentData != null)
        {
            if (currentData.time <= 0 || currentData.time >= currentData.duration)
            {
                SetCurrentId(kInvalidStateBarId);
            }
            else
            {
                stateBar.SetPercent(1f * currentData.time / currentData.duration);
                stateBar.SetTimeText(currentData.time);
            }
        }
        else if (stateBar.GetActive())
        {
            stateBar.SetActive(false);
        }
    }

    public void Unload()
    {
        listData.Clear();
        currentActor = null;
        stateBar = null;
        if (goStateBar != null)
        {
            GameObject.Destroy(goStateBar);
            goStateBar = null;
        }
    }

}
