
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 城镇动作使用技能配置文件播放
/// </summary>
public class BeTownActionPlay : IBeTownActionPlay
{
    protected GeActorEx _geActor;

    protected BDEntityRes _entityRes = new BDEntityRes();
    protected BDEntityActionInfo _curEntityActionInfo;
    protected BDEntityActionFrameData _curFrameData;
    protected float _curTime;
    protected int _curLogicFrame;
    protected bool _actionIsLoop;

    protected string _curActionName;
    protected List<GeEffectEx> _notGlobalEffectList = new List<GeEffectEx>();
    protected List<GeEffectEx> _globalEffectList = new List<GeEffectEx>();


    public void Init(GeActorEx geActor, string actionPath)
    {
        LoadOneSkillConfig(actionPath);
        _geActor = geActor;
    }

    public void Update(float timeElapsed)
    {
        UpdateEntityInfo(timeElapsed);
    }

    public void DeInit()
    {
        if (_entityRes != null)
            _entityRes = null;
        if (_notGlobalEffectList != null)
            _notGlobalEffectList = null;
        if (_globalEffectList != null)
            _globalEffectList = null;
    }

    /// <summary>
    /// 加载一个技能配置文件
    /// </summary>
    protected void LoadOneSkillConfig(string path)
    {
#if !LOGIC_SERVER
        _entityRes.Reset();
        var loadedList = GamePool.ListPool<BDEntityActionInfo>.Get();
        BDEntityActionInfo.SaveLoad(BattleType.Dungeon, path, null, false, false, loadedList, null);
        //处理加载完成后的城镇技能配置文件
        for (int i = 0; i < loadedList.Count; i++)
        {
            var current = loadedList[i];
            _entityRes.AddActionInfo(current, path);
        }
        GamePool.ListPool<BDEntityActionInfo>.Release(loadedList);
#endif
    }

    /// <summary>
    /// 播放技能配置文件
    /// </summary>
    /// <param name="actionName">技能配置文件名称</param>
    /// /// <returns>如果返回True则表明使用技能配置文件播放 返回False表示使用老的方式</returns>
    public bool PlayAction(string actionName)
    {
        if (_geActor == null) return false;
        if (actionName == _curActionName) return false;
        if (_entityRes == null || !_entityRes._vkActionsMap.ContainsKey(actionName))
            return false;

        _curEntityActionInfo = _entityRes._vkActionsMap[actionName];
        if (_curEntityActionInfo == null) return false;

        _curTime = 0;
        _curLogicFrame = 0;
        _actionIsLoop = false;
        ClearEffects(_notGlobalEffectList);
        _geActor.ChangeAction(_curEntityActionInfo.actionName, _curEntityActionInfo.animationspeed, _curEntityActionInfo.bLoop);
        _curActionName = actionName;
        return true;
    }

    /// <summary>
    /// 轮询技能配置文件
    /// </summary>
    /// <param name="timeElapsed"></param>
    protected void UpdateEntityInfo(float timeElapsed)
    {
        if (_curEntityActionInfo == null)
            return;
        int totalLogicFrame = _curEntityActionInfo.iLogicFramesNum;

        _curTime += timeElapsed;
        while (_curTime > 0.033f)
        {
            if (_curLogicFrame >= totalLogicFrame)
            {
                if (_curEntityActionInfo.bLoop)
                {
                    _curLogicFrame = 0;
                    _actionIsLoop = true;
                }
                else
                {
                    _curEntityActionInfo = null;
                    return;
                }
            }
            UpdateFrameEvent(_curLogicFrame);
            _curLogicFrame++;
            _curTime -= 0.033f;
        }
    }

    /// <summary>
    /// 轮询帧事件
    /// </summary>
    /// <param name="frame">当前帧</param>
    protected void UpdateFrameEvent(int frame)
    {
        _curFrameData = _curEntityActionInfo.vFramesData[frame];
        if (_curFrameData == null || _curFrameData.pEvents.Count <= 0)
            return;
        for (int i = 0; i < _curFrameData.pEvents.Count; ++i)
        {
            var eventPlayEffect = _curFrameData.pEvents[i] as BDPlayEffect;
            if (eventPlayEffect != null)
            {
                PlayEffect(eventPlayEffect.EffectInffo, _curEntityActionInfo.animationspeed);
                continue;
            }

            if (_curFrameData.kFlag.HasFlag((int)DSFFrameTags.TAG_REMOVEEFFECT))
            {
                string path = _curFrameData.kFlag.GetFlagData();
                if (_geActor != null && !string.IsNullOrEmpty(path))
                    _geActor.DestroyEffectByName(path);
            }
        }
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="info"></param>
    protected void PlayEffect(EffectsFrames info,float actionSpeed)
    {
        if (_geActor == null)
            return;
        if (info == null || info.effectAsset.m_AssetPath == null)
            return;
        if (_actionIsLoop && !info.loopLoop)
            return;
        
        if (GetGlobalEffect(info.effectAsset.m_AssetPath) != null)
            return;

        float time = 0;
        if (info.playtype == EffectPlayType.GivenTime)
            time = info.time;
        var effect = _geActor.CreateEffect(info.effectAsset, info, time, Vec3.zero, 1, actionSpeed, info.loop);
        if(info.timetype != EffectTimeType.BUFF)
            _notGlobalEffectList.Add(effect);
        else
            _globalEffectList.Add(effect);

#if !LOGIC_SERVER
        if (_curActionName == "Bidle" && effect.GetRootNode() != null)
        {
            effect.GetRootNode().SetLayer(5);
        }
#endif
    }

    /// <summary>
    /// 获取之前创建的全局特效
    /// </summary>
    protected GeEffectEx GetGlobalEffect(string name)
    {
        if (_globalEffectList == null)
            return null;
        for (int i = 0; i < _globalEffectList.Count; i++)
        {
            var effect = _globalEffectList[i];
            if (effect == null)
                continue;
            if (effect.IsDead())
                continue;
            if (effect.GetEffectName() == name)
                return effect;
        }
        return null;
    }

    /// <summary>
    /// 清理特效
    /// </summary>
    protected void ClearEffects(List<GeEffectEx> list)
    {
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)
        {
            var effect = list[i];
            if (effect != null && effect.IsDead())
                continue;
            list[i].Remove();
        }
        list.Clear();
    }

    public float GetActionTime(string name)
    {
        if (_entityRes == null)
            return 0;
        if (string.IsNullOrEmpty(name))
            return 0;
        if (!_entityRes._vkActionsMap.ContainsKey(name))
            return 0;
        if (_entityRes._vkActionsMap[name] == null)
            return 0;
        return _entityRes._vkActionsMap[name].iLogicFramesNum * 0.033f;
    }
}