using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(ComConsumeItemGroup))]
    public class ComConsumeItemGroupInspector : Editor
    {
        //public enum eGroupState
        //{
        //    eIn,
        //    eOut,
        //}

        //private eGroupState state = eGroupState.eIn;

        //private ComConsumeToggleGroup comConsumeGroup = null;
        //bool[] isToggleOnBools = null;


        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();

        //    GUI.color = Color.green;

        //    comConsumeGroup = this.target as ComConsumeToggleGroup;

        //    if(comConsumeGroup == null)
        //    {
        //        return;
        //    }

        //    var consumes = comConsumeGroup.GetConsumeGroup();
        //    if (consumes == null || consumes.Count == 0)
        //    {
        //        return;
        //    }

        //    if (null == isToggleOnBools || isToggleOnBools.Length == 0)
        //    {
        //        isToggleOnBools = new bool[consumes.Count];
        //    }

        //    state = (eGroupState)EditorGUILayout.EnumPopup("组状态", state);

        //    if (state == eGroupState.eIn)
        //    {
        //        for (int i = 0; i < isToggleOnBools.Length; i++)
        //        {
        //            ComCommonConsume cons = consumes[i];
        //            if (cons == null)
        //            {
        //                continue;
        //            }
        //            if (cons.mType == ComCommonConsume.eType.Item)
        //            {
        //                isToggleOnBools[i] = EditorGUILayout.Toggle(string.Format("组员：{0}", cons.mItemID.ToString()), isToggleOnBools[i]);
        //                if(isToggleOnBools[i])
        //                {
        //                }
        //            }
        //        }
        //    }


        //    GUI.color = Color.white;
        //}
    }
}