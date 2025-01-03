using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(DDungeonMapData))]
public class DDungeonMapInspector : Editor
{

    [MenuItem("[关卡编辑器]/地下城/创建地下城地图数据")]
    public static void Create()
    {
        var pinObject = FileTools.CreateAsset<DDungeonMapData>("DDungeonMapData");

        EditorGUIUtility.PingObject(pinObject);
        Selection.activeObject = pinObject;
    }

    #region Update


    private DDungeonMapData mMapData = null;

    private void _updateDungeonData(DDungeonMapUnitData data, int id)
    {
        id = id / 10 * 10;

        var dungeonTable = XlsxDataManager.Instance().GetXlsxByName("DungeonTable");
        var item = dungeonTable.GetRowData(id);
        data.dungeonid = id ;
        data.dungeon = AssetLoader.instance.LoadRes(item["DungeonConfig"].ToString()).obj as DDungeonData;

        _setDataDirty();
    }

    private void _setDataDirty()
    {
        if (mMapData != null)
        {
            EditorUtility.SetDirty(mMapData);
        }
    }

    private int[] mIDList = new int[0];

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.fieldWidth = 50;

        var dungeondata = target as DDungeonMapData;
        if (dungeondata == null)
        {
            return;
        }

        mMapData = dungeondata;

        EditorGUILayout.BeginHorizontal();
        {
            HeroGo.CustomGUIUtility.InitFiledAfterEnter("w", ref mMapData.weith, 1, 100);
            HeroGo.CustomGUIUtility.InitFiledAfterEnter("h", ref mMapData.heigth, 1, 100);
        }
        EditorGUILayout.EndHorizontal();

        var dungeonTable = XlsxDataManager.Instance().GetXlsxByName("DungeonTable");
        if (dungeonTable.EditorPopup("地下城数据", ref mMapData.dungeonid, (int idx) => { return (idx % 1000 == 0); }))
        {
            var cell = dungeonTable.GetRowData(mMapData.dungeonid);
            if (cell != null)
            {
                var value = cell["storyDungeonIDs"];

                var dataLen = 1;
                if (value != null)
                {
                    var str = value.ToString();
                    if (str.Length > 0)
                    {
                        var strArray = str.Split(XlsxDataUnit.SPLITE_REPEATED);

                        mIDList = new int[0];

                        HeroGo.ArrayUtility.ArrayFiled<int>(strArray.Length, ref mIDList, (int idx) =>
                        {
                            return int.Parse(strArray[idx]);
                        });
                    }
                }

                dataLen += mIDList.Length;

                HeroGo.ArrayUtility.ArrayFiled<DDungeonMapUnitData>(dataLen, ref mMapData.dungeonList);
                HeroGo.ArrayUtility.ArrayFiled<int>(dataLen, ref mIDList);

                mIDList[dataLen - 1] = mMapData.dungeonid;

                for (int i = 0; i < dataLen; i++)
                {
                    _updateDungeonData(mMapData.dungeonList[i], mIDList[i]);
                }
            }
        }

        for (int i = 0; i < mMapData.dungeonList.Length; i++)
        {
            var item = mMapData.dungeonList[i];
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(item.dungeonid.ToString());
                EditorGUILayout.BeginHorizontal();
                {
                    HeroGo.CustomGUIUtility.InitFiledAfterEnter("x", ref item.posx, -11111, 11111);
                    HeroGo.CustomGUIUtility.InitFiledAfterEnter("y", ref item.posy, -11111, 11111);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        if (GUI.changed)
        {
            _setDataDirty();
        }
    }
    #endregion
}
