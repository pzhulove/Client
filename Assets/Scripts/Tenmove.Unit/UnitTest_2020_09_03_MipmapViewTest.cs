


using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tenmove.Runtime.Unity;
using UnityEngine;

public class UnitTest_2020_09_03_MipmapViewTest : MonoBehaviour
{
    UnityGameProfileSightEffect.Mipmap m_Mipmap;
    bool m_Display;
    GameObject m_EffectTarget;

    // Use this for initialization
    void Start()
    {
        m_Display = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (null == m_Mipmap)
        {
            m_Mipmap = new UnityGameProfileSightEffect.Mipmap();
            m_EffectTarget = GameObject.Instantiate(Resources.Load<GameObject>("Effects/Hero_Yuansu/Heidong/Prefab/Eff_heidong_chixu"));
            m_Mipmap.AddMipmapViewObject(m_EffectTarget);
        }
    }

    private void OnGUI()
    {
        Rect rect = new Rect(0, Screen.height - Screen.dpi * 0.5f, Screen.width * 0.5f, Screen.dpi * 0.5f);
        GUILayout.BeginArea(rect);

        int originButtonFontSize = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 28;

        GUILayout.BeginHorizontal();
        string text = m_Display ? "正常模式" : "Mipmap模式";
        if (GUILayout.Button(text, GUILayout.Width(Screen.dpi * 1.6f), GUILayout.Height(Screen.dpi * 0.5f)))
            m_Display = !m_Display;
        GUI.skin.button.fontSize = originButtonFontSize;

        if (null != m_Mipmap)
        {
            if (m_Display)
                m_Mipmap.EnableMipmapView();
            else
                m_Mipmap.DisableMipmapView();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private class SkillEffectDesc
    {
        public string AssetPath { set; get; }
        public Vector3 InitPosition { set; get; }
        public Quaternion InitRotation { set; get; }
        public Vector3 InitScale { set; get; }
        public float TimeLength { set; get; }
    }

    private bool _GetAllSkillEffectRes(int id, List<SkillEffectDesc> resList)
    {
        /// ResTable
        /// |||
        /// _FileList.json
        /// .asset DSkillData
        ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(id);

        if (null == resData)
        {
            if (0 != id)
                Debugger.LogWarning("模型资源表中没有ID为{0}的项目", id);

            return false;
        }

        for (int i = 0, icnt = resData.ActionConfigPath.Count; i < icnt; ++i)
        {
            string actionConfigPath = resData.ActionConfigPath[i];

            if (null != actionConfigPath && 0 < actionConfigPath.Length)
                _GetSkillData(actionConfigPath, resList);
        }

        return true;
    }

    //根据模型资源表的动作配置路径获取SkillData
    private void _GetSkillData(string actionConfigPath, List<SkillEffectDesc> resList)
    {
        string folderName = Tenmove.Runtime.Utility.Path.GetFileNameWithoutExtension(actionConfigPath);
        StringBuilder pathBuilder = new StringBuilder();
        pathBuilder.Append(actionConfigPath);
        pathBuilder.Append("/");
        pathBuilder.Append(folderName);
        pathBuilder.Append("_FileList");
        string fileListPath = pathBuilder.ToString();

        if (null != fileListPath && 0 < fileListPath.Length)
        {
            Object fileListObject = AssetLoader.instance.LoadRes(fileListPath).obj;

            if (null != fileListObject)
            {
                string content = ASCIIEncoding.Default.GetString((fileListObject as TextAsset).bytes);
                ArrayList list = XUPorterJSON.MiniJSON.jsonDecode(content) as ArrayList;
                List<SkillFileName> skillFileNames = new List<SkillFileName>();

                for (int i = 0, icnt = list.Count; i < icnt; ++i)
                {
                    SkillFileName skillFileName = new SkillFileName(list[i] as string, actionConfigPath);
                    skillFileNames.Add(skillFileName);
                }

                if (null != skillFileNames)
                {
                    for (int index = 0, iCount = skillFileNames.Count; index < iCount; ++index)
                    {
                        SkillFileName currentSkillFileName = skillFileNames[index];
                        string fullPath = currentSkillFileName.fullPath;
                        DSkillData skillData = AssetLoader.instance.LoadRes(fullPath, typeof(DSkillData)).obj as DSkillData;

                        if (null != skillData)
                            _GetSkillEffectWithSkillData(skillData, resList);
                    }
                }
                else
                    Debugger.LogWarning("cant find file name list in path:{0}", fileListPath);
            }
        }
    }

    //根据SkillData存储特效资源信息
    private void _GetSkillEffectWithSkillData(DSkillData skillData, List<SkillEffectDesc> resList)
    {
        EffectsFrames[] effectsFrames = skillData.effectFrames;
        EntityFrames[] entityFrames = skillData.entityFrames;
        DSkillBuff[] skillBuffs = skillData.buffs;
        //TODO 下面三种情况下的流程
        DSkillSummon[] skillSummons = skillData.summons;
        DSkillFrameEffect[] frameEffects = skillData.frameEffects;
        HurtDecisionBox[] hurtBlocks = skillData.HurtBlocks;

        if (0 < effectsFrames.Length)
        {
            for (int i = 0, icnt = effectsFrames.Length; i < icnt; ++i)
            {
                EffectsFrames effectFrame = effectsFrames[i];
                SkillEffectDesc skillEffectDesc = new SkillEffectDesc();
                GameObject effectAsset = AssetLoader.GetInstance().LoadResAsGameObject(effectFrame.effectAsset.m_AssetPath);

                if (null != effectAsset)
                {
                    StringBuilder assetPath = new StringBuilder();
                    assetPath.Append(effectFrame.effectAsset.m_AssetPath);
                    assetPath.Append(".prefab");
                    skillEffectDesc.AssetPath = assetPath.ToString();
                    DestroyImmediate(effectAsset);
                }

                skillEffectDesc.InitPosition = effectFrame.localPosition;
                skillEffectDesc.InitRotation = effectFrame.localRotation;
                skillEffectDesc.InitScale = effectFrame.localScale;
                skillEffectDesc.TimeLength = effectFrame.time;

                resList.Add(skillEffectDesc);
            }
        }

        //skillData中entityFrames存在  则根据resID继续获取skillData中的特效  需要考虑的情况：如果配置相同的resID 就会一直循环
        if (0 < entityFrames.Length)
            for (int i = 0, icnt = entityFrames.Length; i < icnt; ++i)
                _GetAllSkillEffectRes(entityFrames[i].resID, resList);

        // buff
        if (0 < skillBuffs.Length)
        {
            for (int i = 0, icnt = skillBuffs.Length; i < icnt; ++i)
            {
                DSkillBuff skillBuff = skillBuffs[i];
                _GetBuffEffectRes(skillBuff.buffID, skillBuff.buffTime, resList);
            }
        }

    }

    //存储buff中的特效资源信息
    private void _GetBuffEffectRes(int buffID, float buffTime, List<SkillEffectDesc> resList)
    {
        ProtoTable.BuffTable buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffID);

        if (null == buffData)
        {
            if (0 != buffID)
                Debugger.LogWarning("Buff表中没有ID为{0}的项目", buffID);

            return;
        }

        //TODO buff特效大小受到挂点模型的影响 1.如何获取挂点对象？ 
        _AddBuffEffectList(buffData.BirthEffect, buffTime, resList);
        _AddBuffEffectList(buffData.EffectName, buffTime, resList);
        _AddBuffEffectList(buffData.EndEffect, buffTime, resList);

        if (0 < buffData.summon_entity.Count)
        {
            ProtoTable.FlatBufferArray<int> summonEntity = buffData.summon_entity;

            for (int i = 0, icnt = summonEntity.Count; i < icnt; ++i)
                _GetAllSkillEffectRes(summonEntity[i], resList);
        }

        if (0 < buffData.summon_monsterID)
        {
            ProtoTable.UnitTable monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(buffData.summon_monsterID);
            _GetAllSkillEffectRes(monsterData.Mode, resList);
        }

        if (0 < buffData.TriggerBuffInfoIDs.Count)
        {
            for (int index = 0, iCount = buffData.TriggerBuffInfoIDs.Count; index < iCount; ++index)
            {
                int triggerBuffInfoID = buffData.TriggerBuffInfoIDs[index];
                ProtoTable.BuffInfoTable triggerBuffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(triggerBuffInfoID);

                if (null != triggerBuffData)
                {
                    //TODO 这里需要知道获取的技能等级，因为有的buff持续时间和技能等级有关联
                    int buffLevel = 1;
                    int attachBuffTime = TableManager.GetValueFromUnionCell(triggerBuffData.AttachBuffTime, buffLevel);
                    _GetBuffEffectRes(triggerBuffData.BuffID, attachBuffTime, resList);
                }

            }
        }
    }

    //存储buff特效资源信息 buff特效挂点模型Scale会影响到特效大小，这里需要知道能否通过获取挂点对象的scale作为buff特效的大小
    private void _AddBuffEffectList(string effectPath, float buffTime, List<SkillEffectDesc> resList)
    {
        if (_IsTableDataValid(effectPath))
        {
            SkillEffectDesc skillEffectDesc = new SkillEffectDesc();

            skillEffectDesc.AssetPath = effectPath;
            skillEffectDesc.InitPosition = Vector3.zero;
            skillEffectDesc.InitRotation = Quaternion.identity;
            skillEffectDesc.InitScale = Vector3.one;
            skillEffectDesc.TimeLength = buffTime;

            resList.Add(skillEffectDesc);
        }
    }

    private bool _IsTableDataValid(string str)
    {
        return str != null && str.Length > 0 && str != "-";
    }
}
