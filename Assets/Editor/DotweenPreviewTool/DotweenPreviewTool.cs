using UnityEngine;
using UnityEditor;
using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Reflection;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DotweenPreviewTool : EditorWindow
{
    private bool isInit = false;

    private void Awake()
    {
        CheckInit();
    }

    private void OnDisable()
    {
        if (isInit)
        {
            isInit = false;
            Stop();
            EditorApplication.update -= EditorUpdate;
            //Delete dotween Component
            GameObject dotweenGO = GameObject.Find("[DOTween]");
            if (dotweenGO != null)
                GameObject.DestroyImmediate(dotweenGO);
        }
    }

    DOTweenComponent dotweenComponent;
    MethodInfo dotweenComponentMethod;
    private void EditorUpdate()
    {
        if (dotweenComponent != null)
        {
            dotweenComponent.transform.localPosition = new Vector3(0, 0, UnityEngine.Random.Range(0.1f, 10.0f));
        }

        if (dotweenComponent == null)
        {
            var dotweenGO = GameObject.Find("[DOTween]");
            if (dotweenGO != null)
            {
                dotweenComponent = dotweenGO.GetComponent<DOTweenComponent>();
            }
        }

        if (dotweenComponentMethod == null)
            dotweenComponentMethod = typeof(DOTweenComponent).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);

        if (dotweenComponent != null && dotweenComponentMethod != null)
        {
            dotweenComponentMethod.Invoke(dotweenComponent, null);
        }
    }


    [MenuItem("[TM工具集]/ArtTools/DotweenPreview")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<DotweenPreviewTool>(false, "DotweenPreview", true);
    }

    private bool played;
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play", GUILayout.Width(50), GUILayout.Height(50)))
        {
            if (played == false)
            {
                played = true;
                Play();
            }
        }
        if (GUILayout.Button("Stop", GUILayout.Width(50), GUILayout.Height(50)))
        {
            if (played)
            {
                played = false;
                Stop();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private List<DOTweenAnimation> anims;
    private List<bool> animAutokills;
    private void SetBackState()
    {
        for (int i = 0; i < anims.Count; i++)
        {
            anims[i].autoKill = animAutokills[i];
        }
    }

    private void CheckInit()
    {
        if (isInit == false)
        {
            isInit = true;
            played = false;
            anims = new List<DOTweenAnimation>();
            animAutokills = new List<bool>();

            EditorApplication.update += EditorUpdate;
            //Create dotweenComponent
            GameObject dotweenGO1 = GameObject.Find("[DOTween]");
            if (dotweenGO1 == null)
            {
                Assembly ass = Assembly.LoadFile(Application.dataPath + @"\ThirdParty\Demigiant\DOTween\DOTween.dll");

                Type t = ass.GetType("DG.Tweening.DOTween");
                MethodInfo mi = t.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Static);
                object o = Activator.CreateInstance(t);

                object[] a = new object[] { null, true, true, LogBehaviour.Default };
                try
                {
                    mi.Invoke(o, a);
                }
                catch
                {
                    GameObject dotweenGO = GameObject.Find("[DOTween]");
                    dotweenGO.transform.localPosition = new Vector3(1, 0, 0);
                    dotweenGO.AddComponent<DOTweenComponent>();
                }
            }
        }
    }

    private void Play()
    {
        anims.Clear();
        animAutokills.Clear();

        var go = Selection.activeGameObject;
        if (go != null)
        {
            foreach (var anim in go.GetComponentsInChildren<DOTweenAnimation>())
            {
                if (anim.isActive)
                {
                    anims.Add(anim);
                    animAutokills.Add(anim.autoKill);
                    anim.autoKill = false;
                    anim.CreateTween();
                }
            }
        }
    }

    private void Stop()
    {
        for (int i = 0; i < anims.Count; i++)
        {
            anims[i].autoKill = animAutokills[i];
            anims[i].DORewind();
            anims[i].DOKill();
        }
    }
}
