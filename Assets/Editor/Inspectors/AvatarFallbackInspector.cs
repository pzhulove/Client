using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System.Diagnostics;
using System.Reflection;

/*
public class CreateFirstPackageFileAssetHelper
{
    [MenuItem("[TM工具集]/生成小包资源索引")]
    public static void CreateFirstPackageFileAsset()
    {
        TextAsset tx = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Actor/AvatarFallback/first_package_filelist.txt");
        if(tx != null)
        {
            FirstPackageFileAsset spObj = ScriptableObject.CreateInstance(typeof(FirstPackageFileAsset)) as FirstPackageFileAsset;
            string text = tx.text;
            string[] filelist = text.Split(new string[] { "\r\n" }, 99999, System.StringSplitOptions.RemoveEmptyEntries);
            foreach(string file in filelist)
            {
                spObj.avatarDic.Add(file);
            }

            AssetDatabase.CreateAsset(spObj, "Assets/Resources/Actor/AvatarFallback/FirstPackageFile.asset");
        }
    }
} */

[CustomEditor(typeof(GeAvatarFallbackAsset))]
public class AvatarFallbackInspector : ComEditor 
{
    enum PageEnum
    {
        SwordMan = 10,
        GunMan = 20,
        MagicMan = 30,
        Gedoujia = 40,
        Gungirl = 50,
        Paladin = 60,
        Num
    }
 
    GameObject[] m_AvatarPrefabs = new GameObject[(int)GeAvatarChannel.AvatarRoot + 1];
    PageEnum current = PageEnum.Num;
    PageEnum preSelect = PageEnum.Num;

    void PageShow(PageEnum key, GUIContent contentshow)
    {
        bool bCheck = ToggleButton(current == key, contentshow);
        if (bCheck)
        {
            current = key;
        }
    }

    Dictionary<int, string> m_UsedAvatarPart = new Dictionary<int, string>{
        {(int)GeAvatarChannel.Head, "头部Avatar文件："},
        {(int)GeAvatarChannel.UpperPart, "上半身Avatar文件："},
        {(int)GeAvatarChannel.LowerPart, "下半身Avatar文件："},
    };

    public override void OnInspectorGUI() 
    {
        OnBaseInspectorGUI();
        if (current < PageEnum.SwordMan || current >= PageEnum.Num)
        {
            current = PageEnum.SwordMan;
        }

        GeAvatarFallbackAsset data = target as GeAvatarFallbackAsset;

        if (data == null)
        {
            return;
        }

        if(preSelect != current)
        {
            AvatarNames avatar;
            if(!data.avatarDic.TryGetValue((int)current, out avatar))
            {
                avatar = new AvatarNames();

                data.avatarDic.Add((int)current, avatar);
            }

            for(int i = 0; i <= (int)GeAvatarChannel.AvatarRoot; ++i)
            {
                if(i < avatar.m_Names.Length && !string.IsNullOrEmpty(avatar.m_Names[i]))
                    m_AvatarPrefabs[i] = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", avatar.m_Names[i]), typeof(GameObject)) as GameObject;
                else
                    m_AvatarPrefabs[i] = null;
            }

            preSelect = current;
        }


        GUILayout.BeginHorizontal();
        PageShow(PageEnum.SwordMan, new GUIContent("鬼武士", ""));
        PageShow(PageEnum.GunMan, new GUIContent("枪手", ""));
        PageShow(PageEnum.MagicMan, new GUIContent("女法师", ""));
        PageShow(PageEnum.Gedoujia, new GUIContent("女格斗", ""));
        PageShow(PageEnum.Gungirl, new GUIContent("女枪手", ""));
        PageShow(PageEnum.Paladin, new GUIContent("圣职者", ""));
        GUILayout.EndHorizontal();

        ShowSetting(data, current);
    }

    private void ShowSetting(GeAvatarFallbackAsset data, PageEnum current)
    {
        AvatarNames avatar;
        if(!data.avatarDic.TryGetValue((int)current, out avatar))
        {
            avatar = new AvatarNames();

            data.avatarDic.Add((int)current, avatar);
        }

        bool bChanged = false;
        foreach(var itr in m_UsedAvatarPart)
        {
            int partIndex = itr.Key;
            GameObject oldAnimRes = m_AvatarPrefabs[partIndex];
            m_AvatarPrefabs[partIndex] = EditorGUILayout.ObjectField(itr.Value, oldAnimRes, typeof(GameObject)) as GameObject;
            if (oldAnimRes != m_AvatarPrefabs[partIndex])
            {
                string assetName = AssetDatabase.GetAssetPath(m_AvatarPrefabs[partIndex]).Replace("Assets/Resources/", null);
                avatar.m_Names[partIndex] = assetName;

                bChanged = true;
            }
        } 

        if(bChanged)
        { 
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        } 
    }
}