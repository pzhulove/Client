using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(GeAnimFrameBillboard), true)]
public class GeAnimFrameBillboardInspector : Editor
{
    private SerializedObject m_Object;

    private SerializedProperty m_FrameRate;
//    private SerializedProperty m_FrameSprites;

    private SerializedProperty m_FrameCount;
    private SerializedProperty m_AtlasName;

//    private int m_FrameResNum = 0;
 //   private List<Sprite> m_SpriteResObjList = new List<Sprite>();
    private Texture2D m_Altas;

    protected void OnEnable()
    {
        m_Object = new SerializedObject(target);

        m_FrameRate = m_Object.FindProperty("m_FrameRate");
 //       m_FrameSprites = m_Object.FindProperty("m_FrameSprites");
        m_AtlasName = m_Object.FindProperty("m_AtlasName"); 
        m_FrameCount = m_Object.FindProperty("m_FrameCount");

        _Reload();
    }

    protected void OnDisable()
    {
    }

    public override bool HasPreviewGUI() { return true; }

    public override void OnInspectorGUI()
    {
        m_Object.Update();

/*
        if (GUILayout.Button("刷新数据"))
        {
            GeAnimFrameBillboard targAvatarProxy = (GeAnimFrameBillboard)target;
            if (null != targAvatarProxy)
            {
                targAvatarProxy.RefreshFrameRes();
            }
            m_Object.Update();
            _Reload();
        }*/

        EditorGUILayout.Space();
        _OnDrawFrameSpriteList();
        EditorGUILayout.Space();
    }

    protected void _Reload()
    {
/*
        m_FrameResNum = m_FrameSprites.arraySize;
        for (int i = 0; i < m_FrameResNum; ++i)
        {
            SerializedProperty curResObj = m_FrameSprites.GetArrayElementAtIndex(i);
            m_SpriteResObjList.Add(AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", curResObj.stringValue), typeof(Sprite)) as Sprite);
        }*/

        m_Altas = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", m_AtlasName.stringValue), typeof(Texture2D)) as Texture2D;
    }

    protected void _OnDrawFrameSpriteList()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_FrameRate);
        EditorGUILayout.PropertyField(m_FrameCount);

        Texture2D oldTexture = m_Altas;
        m_Altas = EditorGUILayout.ObjectField("Texture:", m_Altas, typeof(Texture2D)) as Texture2D;
        if(oldTexture != m_Altas)
        {
            m_AtlasName.stringValue = AssetDatabase.GetAssetPath(m_Altas).Replace("Assets/Resources/", null);
            EditorGUILayout.Space();
        }

        if (EditorGUI.EndChangeCheck())
            m_Object.ApplyModifiedProperties();


        /*bool bChanged = false;
        m_FrameResNum = EditorGUILayout.IntField("动画资源数量：", m_FrameResNum);
        if (m_FrameResNum > 0)
        {
            for (int i = 0; i < m_FrameResNum; ++i)
            {
                if (m_SpriteResObjList.Count <= i)
                {
                    bChanged = true;
                    m_SpriteResObjList.Add(EditorGUILayout.ObjectField("第" + i + "帧图片:", null, typeof(Sprite)) as Sprite);
                }
                else
                {
                    Sprite oldFrameRes = m_SpriteResObjList[i];
                    m_SpriteResObjList[i] = EditorGUILayout.ObjectField("第" + i + "帧图片:", oldFrameRes, typeof(Sprite)) as Sprite;
                    if (oldFrameRes != m_SpriteResObjList[i])
                        bChanged = true;
                }
            }
        }

        if (bChanged)
        {
            m_FrameSprites.ClearArray();
            for (int i = 0; i < m_FrameResNum; ++i)
            {
                m_FrameSprites.InsertArrayElementAtIndex(i);
                SerializedProperty curFrameRes = m_FrameSprites.GetArrayElementAtIndex(i);
                curFrameRes.stringValue = AssetDatabase.GetAssetPath(m_SpriteResObjList[i]).Replace("Assets/Resources/", null);
            }

            m_Object.ApplyModifiedProperties();
            GeAnimFrameBillboard targAvatarProxy = (GeAnimFrameBillboard)target;
            if (null != targAvatarProxy)
            {
                targAvatarProxy.RefreshFrameRes();
            }
            m_Object.Update();
            _Reload();

            bChanged = false;
        }*/

        EditorGUILayout.Space();
    }

    static public int _Comparison(Sprite x,Sprite y)
    {
        string xName = x.name;
        string yName = y.name;

        int xFrame = 0, yFrame = 0;
        int idx = 0;

        idx =xName.LastIndexOf('_');
        xFrame = xName.Length;
        if (0 <= idx && idx < xFrame)
        {
            xName = xName.Substring(idx + 1);
            xFrame = int.Parse(xName);
        }
        idx = yName.LastIndexOf('_');
        yFrame = yName.Length;
        if (0 <= idx && idx < yFrame)
        {
            yName = yName.Substring(idx + 1);
            yFrame = int.Parse(yName);
        }

        return xFrame - yFrame;
    }

   /* [MenuItem("Assets/Cook Anim Frame Billboard")]
    static public void CookAnimFrameBillBoard()
    {
        //string[] prefabList = Directory.GetFiles(SCENE_RES_PATH, "*.prefab", SearchOption.TopDirectoryOnly);
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);

        for (int i = 0; i < selection.Length; ++i)
        {
            GameObject curPrefab = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == curPrefab)
                continue;

            string curPrefabName = curPrefab.name;

            string animFramePath = AssetDatabase.GetAssetPath(curPrefab);
            animFramePath = Path.GetDirectoryName(animFramePath);

            List<Sprite> spriteList = new List<Sprite>();
            string[] textAll = AssetDatabase.FindAssets("t:texture", new string[] { animFramePath });

            for (int j = 0, jcnt = textAll.Length;j < jcnt; ++j)
            {
                string texture = AssetDatabase.GUIDToAssetPath(textAll[j]);
                Sprite animFrameRes = AssetDatabase.LoadAssetAtPath<Sprite>(texture);
                if (null != animFrameRes)
                    spriteList.Add(animFrameRes);
            }

            //    Sprite animFrameRes =  AssetDatabase.LoadAssetAtPath<Sprite>(animFramePath);
            //if (null != animFrameRes)
            //    spriteList.Add(animFrameRes);

            spriteList.Sort(_Comparison);
            List<string> spriteResList = new List<string>();
            for(int j = 0 ,jcnt = spriteList.Count;j<jcnt;++j)
                spriteResList.Add(AssetDatabase.GetAssetPath(spriteList[j]).Replace("Assets/Resources/", null));

            GeAnimFrameBillboard curAnimFrameBillboard = curPrefab.GetComponent<GeAnimFrameBillboard>();
            if (null == curAnimFrameBillboard)
                curAnimFrameBillboard = curPrefab.AddComponent<GeAnimFrameBillboard>();

            if (null != curAnimFrameBillboard)
            {
                curAnimFrameBillboard.frameSprites = spriteResList.ToArray();
                curAnimFrameBillboard.frameRate = 10;
            }
            Editor.DestroyImmediate(curPrefab, false);
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
    }*/
}
