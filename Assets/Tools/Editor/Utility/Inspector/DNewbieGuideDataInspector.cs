using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using GameClient;

[CustomEditor(typeof(DNewbieGuideData))]
public class DNewbieGuideDataInspector : Editor
{
    public static string GetNewAssetPath(string assetName)
    {
        //string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //if (path == "")
        //{
        //    path = "Assets";
        //}
        //else if (System.IO.Path.GetExtension(path) != "")
        //{
        //    Debug.Log(path);
        //    path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        //}

        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
                break;
            }
        }

        string assetPathAndName = System.IO.Path.Combine(path, string.Format("New{0}.asset", assetName));
        string uniqueAssetPathAndName = AssetDatabase.GenerateUniqueAssetPath(assetPathAndName);
        return uniqueAssetPathAndName;
    }
  
    [MenuItem("[TM工具集]/新手引导编辑器/创建新手引导",false,1)]
    public static void CreateAsset()
    {
       FileTools.CreateAsset<DNewbieGuideData>("NewbieGuide");
    }

    GUIStyle fontstyleWarnning;
    GUIStyle fontstyleUnitInfo;
    GUIStyle fontstyleUnitInfoSelect;
    GUIStyle headerStyle;
    GUIStyle radioButtonStyle;
   // DG.DemiEditor.DeStylePalette    deStylePalette;
    GUIStyle                        deToggleButtonStyle;
    void CreateFontStyle()
    {
        if (fontstyleWarnning == null)
        {
            fontstyleWarnning = new GUIStyle(EditorStyles.label);
            fontstyleWarnning.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleWarnning.fontSize = 12;
            fontstyleWarnning.alignment = TextAnchor.LowerLeft;
            fontstyleWarnning.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.active.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.focused.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }

        if (fontstyleUnitInfo == null)
        {
            fontstyleUnitInfo = new GUIStyle(EditorStyles.label);
            fontstyleUnitInfo.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfo.fontSize = 12;
            //fontstyleUnitInfo.fontStyle = FontStyle.Bold;
            fontstyleUnitInfo.alignment = TextAnchor.MiddleLeft;
            fontstyleUnitInfo.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleUnitInfo.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }

        if(fontstyleUnitInfoSelect == null)
        {
            fontstyleUnitInfoSelect = new GUIStyle(EditorStyles.label);
            fontstyleUnitInfoSelect.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfoSelect.fontSize = 12;
            //fontstyleUnitInfo.fontStyle = FontStyle.Bold;
            fontstyleUnitInfoSelect.alignment = TextAnchor.MiddleLeft;
            fontstyleUnitInfoSelect.normal.textColor = Color.green;
            fontstyleUnitInfoSelect.hover.textColor = Color.green;
        }

        // if(deStylePalette == null)
        // {
        //     deStylePalette = new DG.DemiEditor.DeStylePalette();
        //     var type = deStylePalette.GetType();
        //     MethodInfo info = type.GetMethod("Init",
        //         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

        //     info.Invoke(deStylePalette,new object[0]);
        // }

        if(deToggleButtonStyle == null)
        {
            deToggleButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
        }

        if(headerStyle == null)
        {
            headerStyle = "RL Header";
        }
 
        if(radioButtonStyle == null)
        {
			radioButtonStyle = new GUIStyle(EditorStyles.miniButton);//new GUIStyle(DG.DemiEditor.DeGUI.styles.button.def);
        }
    }

    ComArrayListDrawer UnitDrawer = new ComArrayListDrawer();
    ComArrayListDrawer ConditionDrawer = new ComArrayListDrawer();

    string textField(string name,string text)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name,GUILayout.Width(80.0f));
        text = EditorGUILayout.TextField(text);
        EditorGUILayout.EndHorizontal();
        return text;
    }

    eNewbieGuideAgrsName guideAgrsEnumToggle(string name,eNewbieGuideAgrsName curValue,eNewbieGuideAgrsName onValue)
    {
        bool bOld = curValue == onValue;
        bool bNew = EditorGUILayout.Toggle(name,bOld);
    
        if(bNew != bOld)
        {
            if(bNew)
            {
                return onValue;
            }
            else
            {
                return eNewbieGuideAgrsName.None;
            }
        }

        return curValue;
    }

     string FixedTextArea(string name,string text,float height = 50.0f)
    {
        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true;
        //float height = style.CalcScreenSize(new GUIContent(guide.mTextTips),Screen.width);
        style.fixedHeight = height;
        EditorGUILayout.LabelField(name);
        return EditorGUILayout.TextArea(text,style,GUILayout.Height(height));
    } 
    void OnUnitDataGUI(ComArrayListDrawer drawer)
    {
        DNewbieGuideData data = target as DNewbieGuideData;

        if(data == null)
        {
            return;
        }

        if(string.IsNullOrEmpty(data.GuideName))
             GUILayout.Label("请输入引导名称说明",EditorStyles.helpBox);
        int oldSize  = GUI.skin.textField.fontSize;
        GUI.skin.textField.fontSize = 20;
        Color oldColor = GUI.skin.textField.normal.textColor;
        GUI.skin.textField.normal.textColor = Color.green;
        float oldHeight = GUI.skin.textField.fixedHeight;
        GUI.skin.textField.fixedHeight = 30;
        data.GuideName = EditorGUILayout.TextField(data.GuideName);
        GUI.skin.textField.fontSize  = oldSize;
        GUI.skin.textField.normal.textColor = oldColor;
        GUI.skin.textField.fixedHeight = oldHeight;

        GUILayout.Space(30.0f);
        oldSize = fontstyleUnitInfoSelect.fontSize;
        fontstyleUnitInfoSelect.fontSize = 15;
        EditorGUILayout.LabelField("引导步骤",fontstyleUnitInfoSelect);
        fontstyleUnitInfoSelect.fontSize = oldSize;
        EditorGUILayout.Space();
        drawer.DrawHeader("",data.UnitData.Length,this);
        StringBuilder builder = StringBuilderCache.Acquire();
        for(int i = 0; i < data.UnitData.Length; ++i)
        {
            drawer.BeginDrawElement(i);
            var cur = data.UnitData[i];
            
            builder.Clear();
            builder.AppendFormat("[{0}] {1} {2}",i,cur.stepName,cur.Type == NewbieGuideComType.NULL ? "":cur.Type.ToString());
            GUILayout.Label(builder.ToString(),drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);

            if(cur.Type == NewbieGuideComType.NULL)
             GUILayout.Label("请输入引导步骤名称以及类型",EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            cur.stepName= EditorGUILayout.TextField(cur.stepName);
            int oldModifyDataCount = 0;
            if( cur.modifyData != null )
            {
                oldModifyDataCount = cur.modifyData.Length;
            } 

            NewbieGuideComType curType = cur.Type;

            var newType = (NewbieGuideComType)EditorGUILayout.EnumPopup(curType);

            if(newType != curType)
            {
                cur.ChangeType(newType);
            }
            EditorGUILayout.EndHorizontal();

            if(newType == NewbieGuideComType.NULL)
            {
                data.UnitData[i] = cur;
                drawer.EndDrawElement();
                continue;
            }

            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel+=2;
            int count = EditorGUILayout.IntField("参数列表个数: ",oldModifyDataCount);
            if(count != oldModifyDataCount)
            {
                if(count <= 0)
                {
                    cur.modifyData = null;
                }
                else
                {
                    cur.modifyData =  new ModifyData[count];
                }
            }

            //EditorGUI.indentLevel+=2;
            for(int k = 0; k < count; ++k)
            {
                EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField("参数"+k);
                cur.modifyData[k].ModifyDataType
                 = (NewBieModifyDataType)EditorGUILayout.EnumPopup(cur.modifyData[k].ModifyDataType,GUILayout.MinWidth(100.0f));
                cur.modifyData[k].iIndex = EditorGUILayout.IntField("索引:",cur.modifyData[k].iIndex);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(10.0f);
            EditorGUI.indentLevel-=2;
            switch (newType)
            {
                case NewbieGuideComType.TOGGLE:
                case NewbieGuideComType.BUTTON:
                    {
                        var guide = cur.buttonGuide;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("界面名",GUILayout.Width(80.0f));
                        guide.mFrameType = EditorGUILayout.TextField(guide.mFrameType);
                        EditorGUILayout.EndHorizontal();
                        guide.mComRoot = textField(
                            "控件",guide.mComRoot);

                        guide.mTextTips = FixedTextArea("引导文字",guide.mTextTips);
                        
                        guide.mAnchor = ( ComNewbieGuideBase.eNewbieGuideAnchor )
                        EditorGUILayout.EnumPopup("位置锚点",guide.mAnchor);
                        guide.mTextTipType = (TextTipType)EditorGUILayout.EnumPopup("文本类型",guide.mTextTipType);
                        guide.mLocalPos = EditorGUILayout.Vector3Field("位置",guide.mLocalPos);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点",guide.mSaveBoot,eNewbieGuideAgrsName.SaveBoot);
                         guide.mPauseBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "暂停单局",guide.mPauseBattle,eNewbieGuideAgrsName.PauseBattle);
                        guide.mHighLightPointPath = textField("高亮显示",guide.mHighLightPointPath);
                    }
                    break;
                case NewbieGuideComType.COVER:
                    {
                        var guide = cur.coverGuide;
                        guide.mFrameType = textField(
                            "界面名",guide.mFrameType);
                    }
                    break;
                case NewbieGuideComType.ETC_BUTTON:
                    {
                        var guide = cur.etcButtonGuide;
                        guide.mButtonName = textField(
                            "按键名",guide.mButtonName);
                        guide.mAnchor = ( ComNewbieGuideBase.eNewbieGuideAnchor )
                        EditorGUILayout.EnumPopup("位置锚点",guide.mAnchor);
                        guide.mContent = FixedTextArea("提示文字",guide.mContent);
                    }
                    break;
                case NewbieGuideComType.ETC_JOYSTICK:
                    {
                        var guide = cur.etcJoystickGuide;
                        guide.mPosX = EditorGUILayout.FloatField(
                            "偏移",guide.mPosX); 
                    }
                    break;
                case NewbieGuideComType.INTRODUCTION:
                  {
                        var guide = cur.introductionGuide;
                        guide.mFrameType = textField(
                            "界面名",guide.mFrameType);
                        guide.mComRoot = textField(
                            "控件",guide.mComRoot);
                        guide.mTextTips = FixedTextArea("引导文字",guide.mTextTips);
                        guide.mAnchor = ( ComNewbieGuideBase.eNewbieGuideAnchor )
                        EditorGUILayout.EnumPopup("位置锚点",guide.mAnchor);
                        guide.mTextTipType = (TextTipType)EditorGUILayout.EnumPopup("文本类型",guide.mTextTipType);
                        guide.mLocalPos = EditorGUILayout.Vector3Field("位置",guide.mLocalPos);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点",guide.mSaveBoot,eNewbieGuideAgrsName.SaveBoot);
                         guide.mPauseBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "暂停单局",guide.mPauseBattle,eNewbieGuideAgrsName.PauseBattle);
                        guide.mHighLightPointPath = textField("高亮显示",guide.mHighLightPointPath);
                        guide.mAutoClose = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "自动关闭",guide.mAutoClose,eNewbieGuideAgrsName.AutoClose);
                        guide.mWaitTime = EditorGUILayout.FloatField("等待时间（秒）",guide.mWaitTime);
                    }
                    break;
                case NewbieGuideComType.INTRODUCTION2:
                    {
                        var guide = cur.introduction2Guide;
                        guide.mTextTips = FixedTextArea("引导文字",guide.mTextTips);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点",guide.mSaveBoot,eNewbieGuideAgrsName.SaveBoot);
                        guide.mHighLightPointPath = textField("高亮显示",guide.mHighLightPointPath);
                        guide.mAutoClose = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "自动关闭",guide.mAutoClose,eNewbieGuideAgrsName.AutoClose);
                        guide.mWaitTime = EditorGUILayout.FloatField("等待时间（秒）",guide.mWaitTime);
                    }
                    break;
                case NewbieGuideComType.NEWICON_UNLOCK:
                   {
                        var guide = cur.newiconUnlockGuide;
                        guide.loadResFile = textField(
                            "prefab",guide.loadResFile);
                        guide.TargetObjPath = textField(
                            "TargetObjPath", guide.TargetObjPath);
                        guide.iconPath = textField(
                            "icon",guide.iconPath);
                        guide.iconName = textField("name",guide.iconName);
                        guide.waittime = EditorGUILayout.FloatField("等待时间（秒）",guide.waittime);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点",guide.mSaveBoot,eNewbieGuideAgrsName.SaveBoot);
                         guide.mTryPauseBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "暂停单局",guide.mTryPauseBattle,eNewbieGuideAgrsName.PauseBattle);
                    }
                    break;
                case NewbieGuideComType.PASS_THROUGH:
                    {
                        var guide = cur.passThroughGuide;
                        guide.mFrameType = textField(
                            "界面名",guide.mFrameType);
                        guide.mComRoot = textField(
                            "控件",guide.mComRoot);
                        guide.mAutoClose = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "自动关闭",guide.mAutoClose,eNewbieGuideAgrsName.AutoClose);
                        guide.mWaitTime = EditorGUILayout.FloatField("等待时间（秒）",guide.mWaitTime);
                        guide.mShowBindObjName = textField("绑定路径",
                        guide.mShowBindObjName);
                        guide.mTextTips = FixedTextArea("引导文字",guide.mTextTips);
                        guide.mAnchor = ( ComNewbieGuideBase.eNewbieGuideAnchor )
                        EditorGUILayout.EnumPopup("位置锚点",guide.mAnchor);
                        guide.mTextTipType = (TextTipType)EditorGUILayout.EnumPopup("文本类型",guide.mTextTipType);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点",guide.mSaveBoot,eNewbieGuideAgrsName.SaveBoot);
                    }
                    break;
                case NewbieGuideComType.STRESS:
                    {
                        var guide = cur.stressGuide;
                        guide.mFrameType = textField(
                            "界面名",guide.mFrameType);
                        guide.mComRoot = textField(
                            "控件",guide.mComRoot);
                        guide.mWaitTime = EditorGUILayout.FloatField("等待时间",guide.mWaitTime);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点",guide.mSaveBoot,eNewbieGuideAgrsName.SaveBoot);
                         guide.mTryPauseBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "暂停单局",guide.mTryPauseBattle,eNewbieGuideAgrsName.PauseBattle);
                         guide.mTryResumeBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "恢复单局",guide.mTryResumeBattle,eNewbieGuideAgrsName.ResumeBattle);
                    }
                    break;
                case NewbieGuideComType.TALK_DIALOG:
                    {
                        var guide = cur.talkDialogGuide;
                        guide.id = EditorGUILayout.IntField(
                            "对话ID",guide.id);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点",guide.mSaveBoot,eNewbieGuideAgrsName.SaveBoot);
                         guide.mTryPauseBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "暂停单局",guide.mTryPauseBattle,eNewbieGuideAgrsName.PauseBattle); 
                    }
                    break;
                case NewbieGuideComType.WAIT:
                    {
                        var guide = cur.waitGuide;
                        guide.mWaitTime = EditorGUILayout.FloatField("等待时间",guide.mWaitTime);
                        guide.mPathThorugh = EditorGUILayout.Toggle(
                        "穿透",guide.mPathThorugh);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "保存点", guide.mSaveBoot, eNewbieGuideAgrsName.SaveBoot);
                        guide.mPauseBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "暂停单局", guide.mPauseBattle, eNewbieGuideAgrsName.PauseBattle);
                        guide.mTryResumeBattle = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                        "恢复单局", guide.mTryResumeBattle, eNewbieGuideAgrsName.ResumeBattle);
                    }
                    break;
                case NewbieGuideComType.PLAY_EFFECT:
                    {
                        var guide = cur.playEffectGuide;

                        guide.loadResFile = textField("挂载特效文件的预制体路径", guide.loadResFile);
                        guide.mSaveBoot = (eNewbieGuideAgrsName)guideAgrsEnumToggle(
                       "保存点", guide.mSaveBoot, eNewbieGuideAgrsName.SaveBoot);
                        guide.mWaitTime = EditorGUILayout.FloatField("等待时间", guide.mWaitTime);
                    }
                    break;
            }
            //EditorGUILayout.PropertyField()
            data.UnitData[i] = cur;
            drawer.EndDrawElement();
        }
        drawer.DrawFooter(ref data.UnitData,
        (value)=>{value.Type = NewbieGuideComType.NULL; return value;});

        StringBuilderCache.Release(builder);
    }

    void OnConditionDataGUI(ComArrayListDrawer drawer)
    {
        DNewbieGuideData data = target as DNewbieGuideData;

        if(data == null)
        {
            return;
        }

        EditorGUILayout.Space();
        int oldSize = fontstyleUnitInfoSelect.fontSize;
        fontstyleUnitInfoSelect.fontSize = 15;
        EditorGUILayout.LabelField("引导条件",fontstyleUnitInfoSelect);
        fontstyleUnitInfoSelect.fontSize = oldSize;
        EditorGUILayout.Space();

        //GUI.skin.textArea.wordWrap = true;
        //GUI.skin.textArea.stretchHeight = true;
        
        drawer.DrawHeader("",data.ConditionData.Length,this);
        StringBuilder builder = StringBuilderCache.Acquire();

        for(int i = 0; i < data.ConditionData.Length; ++i)
        {
            drawer.BeginDrawElement(i);
            var cur = data.ConditionData[i];

            cur.condition = (eNewbieGuideCondition)EditorGUILayout.EnumPopup("条件",cur.condition);

            int oldModifyDataCount = 0;

//             if (cur.condition == eNewbieGuideCondition.CloseKeyFrame)
//             {
//                 if (cur.LimitFramesList != null)
//                 {
//                     oldModifyDataCount = cur.LimitFramesList.Length;
//                 }
//             }
//            else
//            {
                if (cur.LimitArgsList != null)
                {
                    oldModifyDataCount = cur.LimitArgsList.Length;
                }
//            }

            int count = EditorGUILayout.IntField("参数个数: ", oldModifyDataCount);

            if(count != oldModifyDataCount)
            {
//                 if(cur.condition == eNewbieGuideCondition.CloseKeyFrame)
//                 {
//                     if (count <= 0)
//                     {
//                         cur.LimitFramesList = null;
//                     }
//                     else
//                     {
//                         cur.LimitFramesList = new string[count];
//                     }
//                 }
//                 else
//                 {
                    if (count <= 0)
                    {
                        cur.LimitArgsList = null;
                    }
                    else
                    {
                        cur.LimitArgsList = new int[count];
                    }
//                }
            }
      
            for(int k = 0; k < count; ++k)
            {
                EditorGUILayout.BeginHorizontal();
                 //EditorGUI.indentLevel+=2;
                EditorGUILayout.LabelField("参数"+k);

//                 if(cur.condition == eNewbieGuideCondition.CloseKeyFrame)
//                 {
//                     cur.LimitFramesList[k] = EditorGUILayout.TextField(cur.LimitFramesList[k]);
//                 }
//                 else
//                 {
                    cur.LimitArgsList[k] = EditorGUILayout.IntField(cur.LimitArgsList[k]);
//                }
           
                 //EditorGUI.indentLevel-=2;
                EditorGUILayout.EndHorizontal();
            }

            //EditorGUI.indentLevel-=2;
            data.ConditionData[i] = cur;
            drawer.EndDrawElement();
        }

        drawer.DrawFooter(ref data.ConditionData);
        StringBuilderCache.Release(builder);
    }
    public override void OnInspectorGUI()
    {
        DNewbieGuideData data = target as DNewbieGuideData;

        GUIControls.UIStyles.UpdateEditorStyles();
        CreateFontStyle();
        OnUnitDataGUI(UnitDrawer);
        OnConditionDataGUI(ConditionDrawer);
        if(EditorStyles.helpBox.fontSize != 15)
            EditorStyles.helpBox.fontSize = 15;
        if (GUI.changed){
			EditorUtility.SetDirty(data);
		}
    }
}