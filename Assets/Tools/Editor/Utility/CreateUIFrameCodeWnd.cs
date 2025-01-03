using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using GameClient;
using System.Collections;
using System.Diagnostics;

public class CreateUIFrameCodeWnd : EditorWindow
{ 
    static public void ShowWnd(string path)
    {
        CreateUIFrameCodeWnd window = (CreateUIFrameCodeWnd)EditorWindow.GetWindow(typeof(CreateUIFrameCodeWnd),true,"创建Frame代码文件");
        prefabPath = path;
        string frame = path.Substring(path.LastIndexOf("/") + 1);
        frameClassName = frame + "Frame";
        frameNameDesc = frame;
        frameCodeFilePath = defaultFrameFilePath;

        window.Show();   
    }

    static string prefabPath = "";
    static string frameClassName = "";
    static string frameNameDesc = "";
    static string frameCodeFilePath = "";

    const string templateFrameFilePath = "Assets/Scripts/00Common/UI/TemplateFrame.cs";
    const string defaultFrameFilePath = "Assets/Scripts/05ClientSystem/Systems/Town/Frames";

    bool bNeedUpdate = false;

    void OnGUI()
    {
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        frameClassName = EditorGUILayout.TextField(frameClassName);
        GUILayout.Label("界面类名");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        frameNameDesc = EditorGUILayout.TextField(frameNameDesc);
        GUILayout.Label("界面描述");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        frameCodeFilePath = GUILayout.TextField(frameCodeFilePath);
        GUILayout.Label("代码文件存放目录");
        if (GUILayout.Button("浏览文件夹"))
        {
            string path = UnityEditor.EditorUtility.OpenFolderPanel("选择代码文件存放目录", defaultFrameFilePath, "");
            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            frameCodeFilePath = path.Substring(path.IndexOf("/Assets/") + 1);

            int kkk = 0;
        }       

        GUILayout.EndHorizontal();
        
        bNeedUpdate = GUILayout.Toggle(bNeedUpdate, "需要_OnUpdate");

        GUILayout.Space(20);
        if (GUILayout.Button("创建"))
        {
            if(!File.Exists(templateFrameFilePath))
            {
                return;               
            }

            bool ret = false;
            string dstFile = frameCodeFilePath + "/" + frameClassName + ".cs";
            if(File.Exists(dstFile))
            {
                ret =  EditorUtility.DisplayDialog("提示", "文件已经存在,确定要覆盖吗", "确定","取消");
                if(!ret)
                {
                    return;
                }

                File.Delete(dstFile);
            }

            File.Copy(templateFrameFilePath, dstFile);
            if (!File.Exists(dstFile))
            {
                return;              
            }

            string fileContent = File.ReadAllText(dstFile);
            if (string.IsNullOrEmpty(fileContent))
            {
                File.Delete(dstFile);
                return;
            }

            fileContent = fileContent.Replace("FrameClassName", frameClassName);
            fileContent = fileContent.Replace("FrameDesc", frameNameDesc);
            fileContent = fileContent.Replace("FramePrefabPath", prefabPath);    

            if(bNeedUpdate)
            {
                fileContent = fileContent.Replace("return false;//false means _OnUpdate is invalid", "return true;//false means _OnUpdate is invalid");
            }

            File.WriteAllText(dstFile, fileContent);

            ret = EditorUtility.DisplayDialog("提示", "创建成功,是否立即打开cs文件?", "确定","取消");
            if(ret)
            {
                FileInfo fileInfo = new FileInfo(dstFile);
                
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.FileName = fileInfo.FullName;
                processInfo.Arguments = "";

                Process process = new Process();
                process.StartInfo = processInfo;
                process.Start();
            }
        }
    }

    void Update()
    {
        //Repaint();
    }
}
