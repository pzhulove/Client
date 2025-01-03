using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ResJungle;
using NPOI.OpenXmlFormats;


[ResJungleDesc(true)]
public class RJDSkillDataManager : ResJungle.RJScriptObjectManager<DSkillData>
{
    public override uint TypeID
    {
        get
        {
            return RJConst.ID.TypeScriptBase + 1;
        }
    }
   
    public override void PostLoadAll()
    {

    }

    protected override void _OnUnInit()
    {
    }

    //public override IEnumerator<ResObject> GetEnumerator()
    //{
    //    var allTypeData = AssetDatabase.FindAssets("t:" + typeof(DSkillData).FullName);
    //    int cnt = 200;
    //    foreach (var guid in allTypeData)
    //    {
    //        var obj = RJManager.RJFile.GetResIns(AssetDatabase.GUIDToAssetPath(guid));

    //        obj.Type = TypeID;

    //        yield return obj;
    //        cnt--;
    //        if (cnt < 0)
    //        {
    //            yield break;
    //        }
    //    }

    //}
}
