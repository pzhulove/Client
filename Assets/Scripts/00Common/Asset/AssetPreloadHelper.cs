using UnityEngine;
using System.Collections;

public class AssetPreloadHelper
{
    static public void PreloadSkillDataRes()
    {
        ProtoTable.JobTable jobData = TableManager.instance.GetTableItem<ProtoTable.JobTable>(GameClient.PlayerBaseData.GetInstance().JobTableID);
        if (null != jobData)
        {
            ProtoTable.ResTable resData = TableManager.instance.GetTableItem<ProtoTable.ResTable>(jobData.Mode);
            if (null != resData)
            {
                for (int i = 0; i < resData.ActionConfigPath.Count; ++i)
                {
                    string skillDataPath = resData.ActionConfigPath[i];
                    if (!string.IsNullOrEmpty(skillDataPath))
                    {
                        string fileListName = skillDataPath + "/FileList";
                        //UnityEngine.Object ob = AssetLoader.instance.LoadRes(fileListName);
                        AssetInst assetInst = AssetLoader.instance.LoadRes(fileListName);
                        if(null == assetInst)
                            continue;
                        UnityEngine.Object obj = assetInst.obj;
                        if(null == obj)
                            continue;
                        string content = System.Text.ASCIIEncoding.Default.GetString((obj as TextAsset).bytes);
                        ArrayList list = XUPorterJSON.MiniJSON.jsonDecode(content) as ArrayList;

                        for (int j = 0; j < list.Count; ++j)
                        {
                            // DSkillData skillData = AssetLoader.instance.LoadRes(skillDataPath + "/" + list[j]) as DSkillData;
                            AssetInst skillAsset = AssetLoader.instance.LoadRes(skillDataPath + "/" + list[j]);
                            DSkillData skillData = skillAsset.obj as DSkillData;
                            if (null == skillData)
                                continue;

                            CResPreloader.instance.AddRes(skillData.goHitEffectAsset.m_AssetPath);

                            for (int k = 0; k < skillData.effectFrames.Length; ++k)
                                CResPreloader.instance.AddRes(skillData.effectFrames[k].effectAsset.m_AssetPath);

                            for (int k = 0; k < skillData.entityFrames.Length; ++k)
                                CResPreloader.instance.AddRes(skillData.entityFrames[k].entityAsset.m_AssetPath);
                        }
                    }
                }
            }
        }
    }
}
