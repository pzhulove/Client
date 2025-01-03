using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using DG.DemiEditor;
using DG.DOTweenEditor.Core;

namespace DG.DOTweenEditor
{
    [CustomEditor(typeof(DOTweenSequenceComponent))]
    public class DOTweenSequenceComponentInspector : Editor
    {
        public static ColorPalette colors = new ColorPalette();
        public static StylePalette styles = new StylePalette();

        public bool s_fold = true;
        public DOTweenSequenceComponent sequence;
        public int index = 0;
        Sequence mySequence;
        public Dictionary<int, List<DOTweenSequenceInspector>> sequenceInspectorInfoDic = new Dictionary<int, List<DOTweenSequenceInspector>>();
        private void OnEnable()
        {
            sequence = target as DOTweenSequenceComponent;
            index = 0;
            sequenceInspectorInfoDic.Clear();
            _Reload();
        }

        private void _Reload()
        {
            foreach (var tweenGroup in sequence.sequenceDic)
            {
                List<DOTweenSequenceInspector> groupInspector = new List<DOTweenSequenceInspector>();
                foreach (var tween in tweenGroup.Value.sequenceList)
                {
                    DOTweenSequenceInspector dotweenInspector = new DOTweenSequenceInspector();
                    dotweenInspector.OnEnable(tween, sequence);
                    groupInspector.Add(dotweenInspector);
                }
                sequenceInspectorInfoDic.Add(index++, groupInspector);
            }
        }
        public override void OnInspectorGUI()
        {
            DeGUI.BeginGUI(ABSAnimationInspector.colors, ABSAnimationInspector.styles);           
            Undo.RecordObject(sequence, "DOTween Squence");
            sequence.id = EditorGUILayout.TextField("SquenceID", sequence.id);
            EditorGUI.BeginChangeCheck();
            var backColor = GUI.backgroundColor;
            int ixs = 0;
            var it = sequenceInspectorInfoDic.GetEnumerator();
            while (it.MoveNext())
            {
                var value = it.Current.Value;
                sequence.listFold[ixs] = EditorGUILayout.Foldout(sequence.listFold[ixs], "Append:  " + ixs.ToString());
                if (sequence.listFold[ixs])
                {
                    GUI.backgroundColor = backColor;
                    for (int i = 0; i < value.Count; i++)
                    {
                        if (i == 0)
                        {
                            using (var horizontalScope = new GUILayout.VerticalScope("box"))
                            {
                                value[i].OnInspectorGUI();
                            }
                        }
                        else
                        {
                            using (var horizontalScope = new GUILayout.VerticalScope("box"))
                            {
                                GUIStyle style = new GUIStyle();
                                style.normal.textColor = Color.yellow;
                                EditorGUILayout.LabelField("Join:  " + i.ToString(), style);
                                value[i].OnInspectorGUI();
                            }
                        }
                    }
                    GUI.backgroundColor = Color.red;
                    using (var horizontalScope = new GUILayout.HorizontalScope("box"))
                    {
                        if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            DOTweenSequence se = new DOTweenSequence(sequence.gameObject);
                            sequence.sequenceDic[ixs].sequenceList.Add(se);
                            DOTweenSequenceInspector sequences = new DOTweenSequenceInspector();
                            sequences.OnEnable(se, sequence);
                            value.Add(sequences);
                        }
                        else if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            if (value.Count > 0)
                            {
                                sequence.sequenceDic[ixs].sequenceList.RemoveAt(value.Count - 1);
                                value.RemoveAt(value.Count - 1);
                            }
                        }
                    }
                }
                ixs++;
                GUI.backgroundColor = backColor;
            }
            GUI.backgroundColor = Color.cyan;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(20), GUILayout.Height(20)))
            {
                DOTweenSequence se = new DOTweenSequence(sequence.gameObject);
                DOTweenSequenceComponentList seList = new DOTweenSequenceComponentList();
                seList.sequenceList.Add(se);
                sequence.sequenceDic.Add(index, seList);
                DOTweenSequenceInspector sequences = new DOTweenSequenceInspector();
                sequences.OnEnable(se, sequence);
                List<DOTweenSequenceInspector> list = new List<DOTweenSequenceInspector>();
                list.Add(sequences);
                sequenceInspectorInfoDic.Add(index, list);
                sequence.listFold.Add(true);
                index++;
            }
            else if (GUILayout.Button("-", EditorStyles.miniButtonMid, GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (sequenceInspectorInfoDic.Count > 0)
                {
                    int ix = --index;
                    sequence.sequenceDic.Remove(ix);
                    sequenceInspectorInfoDic.Remove(ix);
                    sequence.listFold.RemoveAt((sequence.listFold.Count - 1));
                }
            }
            if (GUILayout.Button("PlaySequence"))
            {
                mySequence = DOTween.Sequence();
                DOTweenSequenceComponentDic dic = sequence.sequenceDic;
                var its = dic.GetEnumerator();
                while (its.MoveNext())
                {
                    var values = its.Current.Value;
                    for (int i = 0; i < values.sequenceList.Count; i++)
                    {
                        DOTweenSequence se = values.sequenceList[i];
                        Tween t = se.CreateEditorPreview();
                        if (i == 0)
                        {
                            mySequence.Append(t);
                        }
                        else
                        {
                            mySequence.Join(t);
                        }
                    }
                }
                DOTweenEditorPreview.Start();
                DOTweenEditorPreview.PrepareTweenForPreview(mySequence);
            }
            if (GUILayout.Button("StopSequence"))
            {
                mySequence.Rewind();
                mySequence.Kill();
                DOTweenEditorPreview.Stop();
            }
            sequence.autoPlay = DeGUILayout.ToggleButton(sequence.autoPlay, new GUIContent("AutoPlay", "If selected, the sequence will play automatically"));
            sequence.autoKill = DeGUILayout.ToggleButton(sequence.autoKill, new GUIContent("AutoKill", "If selected, the sequence will be killed when it completes, and won't be reusable"));
            GUILayout.EndHorizontal();
            GUI.backgroundColor = backColor;

            bool innerChanged = EditorGUI.EndChangeCheck();
            if (innerChanged)
            {
                GUI.changed = true;
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(sequence);
            }
        }
    }
}