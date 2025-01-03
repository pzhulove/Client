using FFmpeg.AutoGen;
using GameClient;
using System.Collections.Generic;
using UnityEditor;

public class DSkillDataMapper
{
    public static IEnumerator<BeEntity> GetRefEntitys(DSkillData data)
    {
        if (!EditorApplication.isPlaying)
        {
            yield break;
        }

        if (null == data)
        {
            yield break;
        }

        if (null == BattleMain.instance)
        {
            yield break;
        }

        var battle = BattleMain.instance.GetBattle() as BaseBattle;
        if (null == battle)
        {
            yield break;
        }

        var bescene = battle.dungeonManager.GetBeScene();
        if (null == bescene)
        {
            yield break;
        }

        var all = bescene.GetEntityCount();
        for (int i = 0; i < all; ++i)
        {
            var beEntity = bescene.GetEntityAt(i);

            if (IsEntityLinkData(beEntity, data))
            {
                yield return beEntity;
            }
        }

        if (null == bescene.BeProjectilePool)
        {
            yield break;
        }

        var values = bescene.BeProjectilePool.GetPoolValues();
        foreach (var que in values)
        {
            foreach (var beEntity in que)
            {
                if (IsEntityLinkData(beEntity, data))
                {
                    yield return beEntity;
                }
            }
        }
    }

    public static void RefreshLinkData(BeEntity entity, DSkillData data)
    {
        if (!IsEntityLinkData(entity, data))
        {
            return;
        }

        if (!entity.m_cpkEntityInfo._vkActionsMap.ContainsKey(data.moveName))
        {
            return;
        }

        entity.m_cpkEntityInfo._vkActionsMap.Remove(data.moveName);

        string path = AssetDatabase.GetAssetPath(data);
        path = path.Replace("Assets/Resources/", "");
        string pathWithOutEx = PathUtil.EraseExtension(path);

        var newInfo = new BDEntityActionInfo(pathWithOutEx);
        newInfo.InitWithDataRes(data);
        entity.m_cpkEntityInfo._vkActionsMap.Add(data.moveName, newInfo);
    }

    public static bool IsEntityLinkData(BeEntity entity, DSkillData data)
    {
        if (null == entity || null == data)
        {
            return false;
        }

        BDEntityRes info = entity.m_cpkEntityInfo;
        if (null == info)
        {
            return false;
        }

        if (!info._vkActionsMap.ContainsKey(data.moveName))
        {
            return false;
        }

        BDEntityActionInfo item = info._vkActionsMap[data.moveName];
        if (null == item)
        {
            return false;
        }

        return IsActionInfoLinkData(item, data);
    }

    public static bool IsActionInfoLinkData(BDEntityActionInfo item, DSkillData data)
    {
        if (null == item || null == data)
        {
            return false;
        }

        string path = AssetDatabase.GetAssetPath(data);
        path = path.Replace("Assets/Resources/", "");

        string pathWithOutEx = PathUtil.EraseExtension(path);

        return path == item.key || pathWithOutEx == item.key;
    }
}
