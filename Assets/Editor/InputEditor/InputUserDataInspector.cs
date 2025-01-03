using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(InputUserData))]
public class InputUsreDataInspector : InputDataInspector
{
    //[MenuItem("按钮配置/创建角色按钮数据")]
    public static new void Create()
    {
        var pinObject = FileTools.CreateAsset<InputUserData>("inputuserdata");

        EditorGUIUtility.PingObject(pinObject);
        Selection.activeObject = pinObject;
    }

    private string[] mSlotMapSelect = new string[0];

    private Dictionary<int, int[]> mSlotSelectDict = new Dictionary<int, int[]>();

    private int[] _getSlotSelectedArray(InputSkillComboData[] list)
    {
        var hashcode = list.GetHashCode();

        if (!mSlotSelectDict.ContainsKey(hashcode))
        {
            var select = new int[1000];
            mSlotSelectDict.Add(hashcode, select);
            for (int i = 0; i < list.Length; i++)
            {
                select[i] = list[i].slot - 1;
            }
        }

        return mSlotSelectDict[hashcode];
    }

    protected void _onSlotPopup(ref int slot, string name = "槽位")
    {
        EditorGUILayout.BeginVertical();
        {
            slot = EditorGUILayout.IntField(name, slot);
            slot = EditorGUILayout.Popup("", slot - 1, mSlotMapSelect) + 1;
        }
        EditorGUILayout.EndVertical();
    }

    protected void OnComboDataGUI(string groupname, ref bool bFlag, ref InputSkillComboData[] list)
    {
        bFlag = EditorGUILayout.BeginToggleGroup(groupname, bFlag);
        EditorGUILayout.BeginVertical();
        if (bFlag)
        {
            int cnt = EditorGUILayout.IntField("连招数目", list.Length);
            HeroGo.ArrayUtility.ArrayFiled(cnt, ref list, (int idx) =>
            {
                mIsDirty = true;
                return new InputSkillComboData() { slot = 1, time = 1000 };
            });

            _getSlotSelectedArray(list);

            for (int j = 0; j < cnt; j++)
            {
                var comboTime = list[j];
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                {
                    _onSlotPopup(ref comboTime.slot);
                    comboTime.time = EditorGUILayout.IntField("时间", comboTime.time);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndToggleGroup();
    }


    private void _getGameObjectPrefab(InputManager.ButtonMode mode, ref GameObject prefab, ref int len)
    {
        GameObject obj = (GameObject)EditorGUILayout.ObjectField(string.Format("{0}预制体", mode.GetDescription()), prefab, typeof(GameObject), false);

        if (obj != prefab)
        {
            prefab = obj;
        }

        obj = prefab;

        int btnLen = 0;
        if (obj != null)
        {
            for (int i = 0; i < obj.transform.childCount; ++i)
            {
                var name = obj.transform.GetChild(i).name;
                if (name.StartsWith("Btn_"))
                {
                    ++btnLen;
                }
                else if (name.StartsWith("Fn_"))
                {
                }
            }
        }

        if (obj != null)
        {
            if (btnLen != len)
            {
                len = btnLen;
            }

            buttonList = new string[len];

            for (int i = 0; i < len; i++)
            {
                buttonList[i] = obj.transform.GetChild(i).name;
            }
        }
        else
        {
            buttonList = new string[0];
        }
    }

    private bool[] mDirectionMap = new bool[1000];
    private bool mIsShowSlotSkillIDMap = false;

    private bool mIsDirty = false;

    public override void OnInspectorGUI()
    {
        InputUserData data = target as InputUserData;

        #region mode prefab

        _getGameObjectPrefab(InputManager.ButtonMode.NORMAL, ref data.normalPrefab, ref data.normalDataLen);

        InputManager.ButtonMode type = (InputManager.ButtonMode)EditorGUILayout.EnumPopup("按钮类型", data.buttonMode);
        //if (type != data.buttonMode)
        {
            switch (type)
            {
                case InputManager.ButtonMode.NORMAL:
                    _getGameObjectPrefab(InputManager.ButtonMode.NORMAL, ref data.normalPrefab, ref data.normalDataLen);
                    break;
                case InputManager.ButtonMode.NORMALEIGHT:
                    _getGameObjectPrefab(InputManager.ButtonMode.NORMALEIGHT, ref data.normalEightPrefab, ref data.normalEightDataLen);
                    break;
                default:
                    Debug.LogError("未处理类型 ：" + type.ToString());
                    break;
            }
            data.buttonMode = type;
        }
        #endregion

        //if (curObject != data.prefabPath)
        //{
        //    //data.prefabPath = curObject;
        //}

        #region slot -> skillid map
        var skillTable = XlsxDataManager.Instance().GetXlsxByName("SkillTable");
        // slot -> skillId
        {
            mIsShowSlotSkillIDMap = EditorGUILayout.BeginToggleGroup("SlotSkillIDMap", mIsShowSlotSkillIDMap);
            if (mIsShowSlotSkillIDMap)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                {
                    int cnt = EditorGUILayout.IntField("槽位数目", data.slotMap.Length);
                    HeroGo.ArrayUtility.ArrayFiled(cnt, ref data.slotMap, (int idx) =>
                    {
                        mIsDirty = true;
                        return new InputSlotSkillMap() { slot = idx + 1, skillID = 1000 };
                    });

                    EditorGUI.indentLevel++;
                    for (int i = 0; i < cnt; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        var item = data.slotMap[i];
                        EditorGUILayout.LabelField("槽位", item.slot.ToString());
                        item.skillID = EditorGUILayout.IntField("技能ID", item.skillID);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();


                        int selectLine = EditorGUILayout.Popup(skillTable.GetLineByID(item.skillID), skillTable.GetKeyNameList(), GUILayout.Width(300));
                        var skillItemData = skillTable.GetRowDataByLine(selectLine + XlsxDataUnit.XLS_DATA_INDEX);
                        if (skillItemData != null)
                        {
                            item.skillID = int.Parse(skillItemData["ID"].ToString());
                            var iconPath = skillItemData["Icon"].ToString();
                            var texture = AssetLoader.instance.LoadRes(iconPath, typeof(Texture2D), false).obj as Texture2D;
                            if (texture != null)
                            {
                                EditorGUILayout.BeginHorizontal(GUILayout.Width(25));
                                GUI.DrawTexture(GUILayoutUtility.GetRect(25, 25), texture);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndToggleGroup();
        }

        #region Select Name
        {
            int cnt = data.slotMap.Length;
            mSlotMapSelect = new string[cnt];

            for (int i = 0; i < cnt; ++i)
            {
                var item = data.slotMap[i];
                mSlotMapSelect[i] = string.Format("{0}({1}", item.slot, item.skillID);

                var skillLine = skillTable.GetRowData(item.skillID);
                if (skillLine != null)
                {
                    var skillName = skillLine["Name"].ToString();
                    mSlotMapSelect[i] += "," + skillName;
                }

                mSlotMapSelect[i] += ")";
            }
        }
        #endregion

        #endregion

        if (data.buttonMode == InputManager.ButtonMode.NORMAL ||
            data.buttonMode == InputManager.ButtonMode.NORMALEIGHT)
        {
            var dataList = data.normalData;
            if (data.buttonMode == InputManager.ButtonMode.NORMALEIGHT)
            {
                dataList = data.normalEightData;
            }

            #region Normal
            if (dataList.Length != buttonList.Length)
            {
                int cnt = buttonList.Length;

                if (data.buttonMode == InputManager.ButtonMode.NORMAL)
                {
                    HeroGo.ArrayUtility.ArrayFiled(cnt, ref data.normalData, (int idx) =>
                    {
                        mIsDirty = true;
                        return new InputSlotClickNormalMap() { name = buttonList[idx], normalClickSlot = idx + 1 };
                    });
                    dataList = data.normalData;
                }
                else if (data.buttonMode == InputManager.ButtonMode.NORMALEIGHT)
                {
                    HeroGo.ArrayUtility.ArrayFiled(cnt, ref data.normalEightData, (int idx) =>
                    {
                        mIsDirty = true;
                        return new InputSlotClickNormalMap() { name = buttonList[idx], normalClickSlot = idx + 1 };
                    });
                    dataList = data.normalEightData;
                }
            }

            for (int i = 0; i < buttonList.Length; i++)
            {
                var item = dataList[i];
                EditorGUILayout.BeginHorizontal("GroupBox");
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginVertical();
                        item.name = buttonList[i];
                        EditorGUILayout.LabelField(item.name);
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUI.indentLevel++;
                            _onSlotPopup(ref item.normalClickSlot);
                            //item.normalClickSlot = EditorGUILayout.IntField("槽位", item.normalClickSlot);
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion
        }
        else
        {
            Debug.LogError("unknow button type : " + data.buttonMode.ToString());
        }

        if (GUI.changed || mIsDirty)
        {
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            mIsDirty = false;
        }
    }
}
