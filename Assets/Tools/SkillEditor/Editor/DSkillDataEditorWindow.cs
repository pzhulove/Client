using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Management.Instrumentation;

public class DSkillDataEditorWindow : EditorWindow
{
    [MenuItem("Assets/战斗相关/技能配置文件/查找所有路径错误的技能配置文件", false, 100)]
    public static void FindDismatchDAssetObject()
    {
        var data = Selection.activeObject;
        var dataPath = AssetDatabase.GetAssetPath(data);
        if (!(!File.Exists(dataPath) && Directory.Exists(dataPath)))
        {
            if (!EditorUtility.DisplayDialog("是否全局查找", "", "ok", "no"))
            {
                return ;
            }

            dataPath = "";
        }
        
        var all = AssetDatabase.FindAssets("t:DSkillData ", new string[] { dataPath });

        int i = 0;

        if (!EditorUtility.DisplayDialog(dataPath, "是否查找 " + all.Length.ToString() +"个文件", "ok", "no"))
        {
            return;
        }

        string cacheStr = "";//string>();
        List<UnityEngine.ScriptableObject> misMatchObjects = new List<UnityEngine.ScriptableObject>();
        foreach (var guid in all)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar(path, i.ToString(), 1.0f * i++ / all.Length);
            var so = AssetDatabase.LoadAssetAtPath<DSkillData>(path);
            if (FixOneDSkillData(so, false))
            {
                cacheStr += path+"\n";
                misMatchObjects.Add(so);
            }
        }
        EditorUtility.ClearProgressBar();

        if (misMatchObjects.Count <= 0)
        {
            return;
        }
        
        Selection.objects = misMatchObjects.ToArray();
        
        if (EditorUtility.DisplayDialog("是否替换对应", cacheStr, "ok", "no"))
        {
            foreach (var o in misMatchObjects)
            {
                FixOneDSkillData(o, true);
                EditorUtility.SetDirty(o);
            }
            
            AssetDatabase.SaveAssets();
        }
    }

    public static bool FixOneDSkillData(ScriptableObject so, bool needModify)
    {
        bool hasOne = false;
        UnityEditor.SerializedObject s = new SerializedObject(so);
        
        var iter = s.GetIterator();

        while (iter.Next(true))
        {
            if (iter.type == "DAssetObject")
            {
                UnityEngine.Debug.LogFormat("Find Path:{0}, Name:{1}", iter.propertyPath, iter.name);
                //UnityEngine.Debug.LogFormat(
                //    "type:{0} name:{1} depth:{2} editable:{3} displayName:{4} hasChildren:{5} objvalue:{6}",
                //    iter.type, iter.name, iter.depth, iter.editable, iter.displayName, iter.hasChildren,
                //    iter.objectReferenceInstanceIDValue);

                var pObject = iter.FindPropertyRelative("m_AssetObj");
                if (null != pObject.objectReferenceValue)
                {
                    var pObjectPath = AssetDatabase.GetAssetPath(pObject.objectReferenceValue);
                    pObjectPath = pObjectPath.Substring("Assets/Resources/".Length);
                    var extension = System.IO.Path.GetExtension(pObjectPath);
                    pObjectPath = pObjectPath.Substring(0, pObjectPath.Length - extension.Length);
                    
                    var pPath   = iter.FindPropertyRelative("m_AssetPath");
                    if (pPath.stringValue != pObjectPath)
                    {
                        UnityEngine.Debug.LogErrorFormat("file:{0}\n--:{1}\n--:{2}\n", AssetDatabase.GetAssetPath(so), pObjectPath, pPath.stringValue);
                        if (needModify)
                        {
                            pPath.stringValue = pObjectPath;
                            s.ApplyModifiedProperties();
                        }
                        hasOne = true;
                    }
                }
            }
        }

        return hasOne;
    }

    readonly static string SKILLEDITOR_V = "V1.8Alpha";

    public static DSkillDataEditorWindow skillActionDataEditorWindow;
    public static DSkillData sendSkillData;
    public static DSkillData sSkillData;
    public static bool bShowGizmo = false;

    public XlsxDataManager xlsxDataManager;

    private Vector2 scrollPos;
    private DSkillData skillData;
    private string mSelectSkillDataPath = string.Empty;
    public GameObject character;


    private float animationSpeedTemp;
    private int totalFramesTemp;
    private float animFrame;
    private int animFrameInt;
    private float animTime;
    private int effectnums;
    private int entitynums;
    private bool animIsPlaying;
    private bool smoothPreview;
    private double playBeginTime;
    private double prePlayTime;

    private string[] animClipsName = new string[0];
    private GeAnimDesc[] animClips = new GeAnimDesc[0];
    private int animClipsSelected = -1;
    private int xlsClipsSelected = -1;
    private AnimationClip animClip;
    private Animation animations;
    private GameObject animPack = null;

    private float camTime;
    private float camStart;
    public Vector3 initialCamPosition;
    public Quaternion initialCamRotation;
    public bool initialCamOrc;
    public float initialSize;

    private bool characterWarning;
    private string errorMsg;

    private int fpsTemp;
    private float speedTemp = 1.0f;

    private bool generalOptions;
    private bool animationOptions = true;
    private bool xlsxdataOptions = false;
    private bool inputOptions;
    private bool moveLinksOptions;
    private bool particleEffectsOptions;
    private bool selfAppliedForceOptions;
    private bool soundOptions;
    private bool cameraOptions;
    private bool pullInOptions;
    private bool activeFramesOptions;
    private bool invincibleFramesOptions;
    private bool projectileOptions;


    private bool hitsToggle;
    private bool defsToggle;
    private bool effectsToggle;
    private bool EntityToggle;
    private bool attachToggle;
    private bool FrameEventToggle;
    private bool grapToggle = false;
    private bool chargeToggle = false;
    private bool specialOperationToggle = false;
    private bool skillJoystickToggle = false;
    private bool skillEventsToggle = false;
    private bool skillPhaseToggle = false;
    private bool skillComboToggle = false;
    private bool projectileShakeToggle = false;
    private int FrameEventSelected = 0;

    private string titleStyle;
    private string removeButtonStyle;
    private string addButtonStyle;
    private string borderBarStyle;
    private string rootGroupStyle;
    private string subGroupStyle;
    private string arrayElementStyle;
    private string subArrayElementStyle;
    private string toggleStyle;
    private string[] desc = {"单向屏震", "双向屏震", "随机屏震"};
    private int[] mode = {3, 4, 5};
    private string foldStyle;
    private string enumStyle;
    private string fillBarStyle1;
    private string fillBarStyle2;
    private string fillBarStyle3;
    private string fillBarStyle4;
    private GUIStyle labelStyle;
    private GUIStyle TablabelStyle;
    private GUIStyle SelectedTablabelStyle;
    private GUIStyle showlableStyle;
    private GUIStyle _warningLableStyle;
    private bool bNew = true;
    private bool bShowAllEffects = true;
    bool bShowAllEntity = true;

    private bool frameTagsToggle;
    private bool frameGrapToggle;
    private bool stateopToggle;
    private bool properModifyToggle;
    private bool faceAttackerToggle;
    private bool shocksToggle;
    private bool sfxToggle;
    private bool frameEffectToggle;
    private bool cameraMoveToggle;
    private bool walkControlToggle;
    private bool actionToggle;
    private bool addBuffInfoOrBuffToggle;
    private bool summonToggle;
    private bool mechanismToggle;
    private bool curFrameToggle;
    private bool bEditorView = true;
    private float GameViewAngle = 20.0f;
    private TransformParam transcopy = new TransformParam();

    private bool bSkillTotalPreview = false;

    private bool _Play = false;

    private bool mIsPlayAllStep = false;
    private int mCircleAnimaPlayTime = 3;
    private int mCurSkillStep = 0; //当前播放到哪一步 
    private Dictionary<int, DSkillData> mAllSkillStepDic = new Dictionary<int, DSkillData>(); //该目录下面所有的技能配置文件。包括没有用到的
    private Dictionary<int, List<int>> mMainSkillWithStepSkills = new Dictionary<int, List<int>>(); //保存主技能对应的技能阶段

    // private bool _PlayLoop = false;
    // private float _PlaySpeed = 1.0f;
    private bool _smoothviewback;

    private float interpolationSpeed = 1.0f;
    private Quaternion rotatetion_global = Quaternion.identity;

    private string[] _attachnames = new string[0];
    private Dictionary<string, GameObject> _attachparent = new Dictionary<string, GameObject>();

    private string undoAnimName = "";

    private Dictionary<string, DSkillData> _parentSkillDataDic = new Dictionary<string, DSkillData>();
    private List<string> _parentFolderSkillNameList = new List<string>();
    private List<int> _parentFolderSkillIdList = new List<int>();

    private TableInspetor<ProtoTable.EffectTable> mAttackHurtEffectTable = new TableInspetor<ProtoTable.EffectTable>("伤害效果表ID");
    private TableInspetor<ProtoTable.BuffTable> mBuffTable = new TableInspetor<ProtoTable.BuffTable>("BuffID");
    private TableInspetor<ProtoTable.MechanismTable> mMechanismTable = new TableInspetor<ProtoTable.MechanismTable>("机制ID", "Description");
    private TableInspetor<ProtoTable.ResTable> mResTable = new TableInspetor<ProtoTable.ResTable>("模型资源表ID", true);
    private TableInspetor<ProtoTable.SoundTable> mSoundTable = new TableInspetor<ProtoTable.SoundTable>("音效资源表ID", "Descrip");
    private TableInspetor<ProtoTable.BuffInfoTable> mBuffInfoTable = new TableInspetor<ProtoTable.BuffInfoTable>("Buff信息表ID", "Description");

    public static void ShowGizmo(bool bShow)
    {
        bShowGizmo = bShow;

        if (bShow)
        {
            //Tools.current = Tool.View;
            Tools.hidden = false;
        }
        else
        {
            //Tools.current = Tool.None;
            Tools.hidden = true;
        }
    }

    public void OnEditChangeOrSelectChange()
    {
        rotatetion_global = Quaternion.identity;
    }

    public static void BackSceneCamera(DSkillDataEditorWindow windows)
    {
        windows.initialCamOrc = DEditorCamera.GetOrthographic();
        windows.initialCamPosition = DEditorCamera.GetPosition();
        windows.initialCamRotation = DEditorCamera.GetRotation();
        windows.initialSize = DEditorCamera.GetSize();
    }

    public void Initial()
    {
        _DefaultStateStyle = new GUIStyle(UnityEditor.EditorStyles.label);
        _DefaultStateStyle.fontStyle = FontStyle.Bold;
        _DefaultStateStyle.normal.textColor = Color.green;
    }

    public static void Init()
    {
        skillActionDataEditorWindow = EditorWindow.GetWindow<DSkillDataEditorWindow>(
            "SkillEditor" + DSkillDataEditorWindow.SKILLEDITOR_V, true,
            new Type[] {typeof(Editor), typeof(EditorWindow), typeof(DSceneDataEditorWindow)});

        skillActionDataEditorWindow.Show();
        EditorWindow.FocusWindowIfItsOpen<SceneView>();
        BackSceneCamera(skillActionDataEditorWindow);
        skillActionDataEditorWindow.Populate();
        skillActionDataEditorWindow.Initial();
        skillActionDataEditorWindow.mLoadCopySkillDataAsset();
    }

    void ClearAttachNodes()
    {
        _attachnames = new string[0];
        _attachparent.Clear();
    }

    GameObject GetAttachNodes(string key)
    {
        if (_attachparent != null && key != null)
        {
            GameObject obj = null;
            _attachparent.TryGetValue(key, out obj);

            if (obj != null)
            {
                return obj;
            }
        }

        return DSkillData.actor;
    }

    void UpdateAttachNodes()
    {
        if (character)
        {
            List<string> name = new List<string>();
            _attachparent.Clear();
            Transform[] trans = character.GetComponentsInChildren<Transform>();
            string key = "None";
            name.Add(key);
            _attachparent.Add(key, null);
            foreach (Transform t in trans)
            {
                if (t.gameObject.CompareTag("Dummy"))
                {
                    key = "[actor]" + t.gameObject.name;
                    name.Add(key);
                    _attachparent.Add(key, t.gameObject);
                }
            }

            for (int i = 0; i < GetCurSkillData().attachFrames.Length; ++i)
            {
                EntityAttachFrames atts = GetCurSkillData().attachFrames[i];

                if (atts == null || atts.attach == null)
                {
                    continue;
                }

                float fBeginTime = GetFramesTime(atts.start);

                float fEndTime = GetFramesTime(atts.end);

                if (animTime >= fBeginTime && animTime <= fEndTime)
                {
                    atts.attach.UpdateAttach(ref name, ref _attachparent);
                }
            }

            _attachnames = name.ToArray();
        }
        else
        {
            ClearAttachNodes();
        }
    }

    void OnSelectionChange()
    {
        if (IsEditorPlaying())
        {
            Clear(true, false);
            return;
        }

        if (CheckSelectedNoChange())
        {
            return;
        }

        Clear(true, false);
        Populate();
        Repaint();
        Clear(false, false);
        Preview();
    }

    void UpdateScene(SceneView sceneview)
    {
        Camera[] c = SceneView.GetAllSceneCameras();
        //sceneview.camera.nearClipPlane = 1000.01f;
        //sceneview.camera.farClipPlane = 100.01f;
        ProcessSceneGUI(sceneview);
    }

    void OnEnable()
    {
        AudioManager.instance.Init();
        Populate();
        SceneView.onSceneGUIDelegate += UpdateScene;
    }

    private void _OnSetCamera()
    {
    }

    private void _OnRevertCamera()
    {
    }

    void SkillDataUndo(string name)
    {
        Undo.RecordObject(skillData, name);
        EditorUtility.SetDirty(skillData);
    }

    void Refresh()
    {
        ClearChar();
        CreateChar();
        GC.Collect();
        Repaint();
        SceneView.RepaintAll();
    }

    void OnFocus()
    {

        if (EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
           // EditorApplication.isPaused = true;
        }
        //Refresh();
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= UpdateScene;
        Clear(true, true);
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= UpdateScene;

        Clear(true, true);
    }

    void OnLostFocus()
    {
        if (EditorApplication.isPlaying && EditorApplication.isPaused)
        {
            //EditorApplication.isPaused = false;
        }
    }

    void ClearChar()
    {
        if (character != null)
        {
            DSkillDataEditorDrawer proxyScript = character.GetComponent<DSkillDataEditorDrawer>();
            if (proxyScript) DestroyImmediate(proxyScript);
            //Editor.DestroyImmediate(character);
            character = null;
        }


        if (GetCurSkillData() != null)
        {
            for (int i = 0; i < GetCurSkillData().effectFrames.Length; ++i)
            {
                EffectsFrames effectFrames = GetCurSkillData().effectFrames[i];
                if (effectFrames != null)
                    effectFrames.effects.Show(false);
            }

            ClearEntityFramesData();
        }

        ClearAttachNodes();
        ClearAttachFramesData();
        ClearGrabObjData();

        DSkillData.attach = null;
        DSkillData.actor = null;

        if (DSkillData.root)
        {
            Editor.DestroyImmediate(DSkillData.root);
            DSkillData.root = null;
        }

        if (null != animPack)
        {
            DestroyImmediate(animPack);
            animPack = null;
        }
    }

    void Clear(bool destroyChar, bool resetCam)
    {
        _Play = false;
        animFrame = 0.0f;
        ShowGizmo(true);

        if (destroyChar)
        {
            Undo.undoRedoPerformed -= Refresh;
            ClearChar();
        }

        if (resetCam)
        {
            DEditorCamera.SetOrthographic(
                initialCamOrc
            );

            DEditorCamera.SetPosition(
                initialCamPosition
            );

            DEditorCamera.SetRotation(
                initialCamRotation
            );

            DEditorCamera.SetSize(
                initialSize
            );
        }

        mIsPlayAllStep = false;
        mCurSkillStep = 0;

        AudioManager.instance.ClearOnEditor();
    }

    void helpButton(string page)
    {
    }

    private double mSkillTime = 0f;
    void Update()
    {
        if (IsEditorPlaying() && character != null)
        {
            Clear(true, false);
        }

        //0.01s
        if ((_Play || mIsPlayAllStep) && GetCurSkillData() != null)
        {
            Repaint();
            double currentTime = EditorApplication.timeSinceStartup;
            float fDeltaTime = (float) (currentTime - prePlayTime) * interpolationSpeed;
            prePlayTime = currentTime;
            if (_Play)
            {
                float fDeltaFrame = fDeltaTime * GetCurSkillData().fps;
                animFrame += fDeltaFrame;
                DSkillData.time += fDeltaTime;
                if (animFrame >= GetCurSkillData().totalFrames - 1)
                {
                    if (animClip && animClip.wrapMode == WrapMode.Loop)
                    {
                        animFrame = 0.0f;
                    }
                    else
                    {
                        _Play = false;
                        smoothPreview = _smoothviewback;
                        animFrame = 0.0f;
                    }
                }
            }
            else if (mIsPlayAllStep)
            {
                var dSkillData = GetCurSkillData();
                if (dSkillData != null)
                {
                    float fDeltaFrame = fDeltaTime * dSkillData.fps;
                    animFrame += fDeltaFrame;
                    DSkillData.time += fDeltaTime;
                    if (animFrame >= dSkillData.totalFrames - 1)
                    {
                        if (animClip != null && animClip.wrapMode == WrapMode.Loop&&!dSkillData.notLoopLastFrame)
                        {
                            bool isCanNext = false;
                            mSkillTime = EditorApplication.timeSinceStartup - playBeginTime;
                            if (dSkillData.triggerType == TriggerNextPhaseType.PRESS_AGAIN ||
                                dSkillData.triggerType == TriggerNextPhaseType.RELEASE_BUTTON)
                            { 
                                isCanNext = mSkillTime >=mCircleAnimaPlayTime;
                            }
                            else
                            {
                                isCanNext = mSkillTime >= dSkillData.skilltime;
                            }
                         
                            if (isCanNext)
                            {
                                prePlayTime = EditorApplication.timeSinceStartup;
                                playBeginTime = EditorApplication.timeSinceStartup;
                                ClearChar();
                                animFrame = 0.0f;
                                mCurSkillStep++;
                                if (mCurSkillStep >= skillData.skillPhases.Length)
                                {
                                    mIsPlayAllStep = false;
                                    smoothPreview = _smoothviewback;
                                    mCurSkillStep = 0;
                                    animFrame = 0;
                                }

                                RefreashSkillData(GetCurSkillData());
                                CreateChar();
                            }
                            else
                            {
                                animFrame = 0.0f;
                            }
                        }
                        else
                        {
                            prePlayTime = EditorApplication.timeSinceStartup;
                            playBeginTime = EditorApplication.timeSinceStartup;
                            ClearChar();
                            animFrame = 0.0f;
                            mCurSkillStep++;
                            if (mCurSkillStep >= skillData.skillPhases.Length)
                            {
                                mIsPlayAllStep = false;
                                smoothPreview = _smoothviewback;
                                mCurSkillStep = 0;
                                animFrame = 0;
                            }

                            RefreashSkillData(GetCurSkillData());
                            CreateChar();
                        }
                    }
                }
            }

            AnimationSampler(GetCurSkillData());
            PlaySound(GetCurSkillData());
        }


        if (needCreateChar)
        {
            needCreateChar = false;
            Refresh();
        }
    }

    private void PlaySound(DSkillData skillData)
    {
        if (skillData == null) return;
        for (int i = 0, icnt = skillData.sfx.Length; i < icnt; ++i)
        {
            DSkillSfx curSfx = skillData.sfx[i];
            if ((int) animFrame == curSfx.startframe)
            {
                if (curSfx.soundID > 0)
                    AudioManager.instance.PlaySound(curSfx.soundID);
                else
                    AudioManager.instance.PlaySound(curSfx.soundClipAsset.m_AssetPath, AudioType.AudioEffect, 1);
            }
        }
    }

    bool CheckSelectedNoChange()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DSkillData), UnityEditor.SelectionMode.Assets);
        if (selection.Length > 0)
        {
            FocusWindowIfItsOpen<DSkillDataEditorWindow>();
            if (skillData == (DSkillData) selection[0])
            {
                return true;
            }
            else
            {
                //fLastSkillTipTime = 10.0f;
                return false;
            }
        }

        return true;
    }

    //编辑器优化——复制
    private DSkillData mCopySkillData;
    private object mCopyData;
    //编辑器优化——过滤动画
    private string mAnimationFilterString = "";
    private int mAnimationFilterInt = -1;

    private void mLoadCopySkillDataAsset()
    {
        string path = "";

        if (string.IsNullOrEmpty(path))
        {
            path = Application.dataPath + "Editor";
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = "Assets/Editor/copyskilldata.asset";

        mCopySkillData = AssetDatabase.LoadAssetAtPath<DSkillData>(path);
        if (mCopySkillData == null)
        {
            mCopySkillData = ScriptableObject.CreateInstance<DSkillData>();
            AssetDatabase.CreateAsset(mCopySkillData, path);

            EditorUtility.SetDirty(mCopySkillData);
            AssetDatabase.SaveAssets();
        }
    }

    GUIStyle _DefaultStateStyle;

    void Populate()
    {
        this.titleContent = new GUIContent("SkillEditor" + DSkillDataEditorWindow.SKILLEDITOR_V);
        camTime = 0;
        // Style Definitions
        titleStyle = "MeTransOffRight";
        removeButtonStyle = "TL SelectionBarCloseButton";
        addButtonStyle = "CN CountBadge";
        rootGroupStyle = "GroupBox";
        subGroupStyle = "ObjectFieldThumb";
        arrayElementStyle = "flow overlay box";
        subArrayElementStyle = "HelpBox";
        foldStyle = "Foldout";
        enumStyle = "MiniPopup";
        toggleStyle = "BoldToggle";
        borderBarStyle = "ProgressBarBack";
        fillBarStyle1 = "ProgressBarBar";
        fillBarStyle2 = "flow node 2 on";
        fillBarStyle3 = "flow node 4 on";
        fillBarStyle4 = "flow node 6 on";

        labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.white;

        TablabelStyle = new GUIStyle();
        TablabelStyle.alignment = TextAnchor.MiddleLeft;
        TablabelStyle.fontStyle = FontStyle.Normal;
        TablabelStyle.normal.textColor = Color.green;
        TablabelStyle.fontSize = 16;

        SelectedTablabelStyle = new GUIStyle();
        SelectedTablabelStyle.alignment = TextAnchor.MiddleLeft;
        SelectedTablabelStyle.fontStyle = FontStyle.Normal;
        SelectedTablabelStyle.normal.textColor = Color.red;
        SelectedTablabelStyle.fontSize = 16;

        showlableStyle = new GUIStyle();
        showlableStyle.fontSize = 20;
        showlableStyle.normal.textColor = Color.green;

        _warningLableStyle = new GUIStyle();
        _warningLableStyle.normal.textColor = Color.red;


        mCurSkillStep = 0;
        if (sendSkillData != null)
        {
            EditorGUIUtility.PingObject(sendSkillData);
            Selection.activeObject = sendSkillData;
            sendSkillData = null;
        }

        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DSkillData), UnityEditor.SelectionMode.Assets);
        if (selection.Length > 0)
        {
            string assetPath = AssetDatabase.GetAssetPath(selection[0]);
            mSelectSkillDataPath = assetPath;
            _RefreshParentSkillDataList(assetPath);
            skillData = (DSkillData) selection[0];
            SetAllSkillData();
            if (skillData.characterPrefab == null && Utility.IsStringValid(skillData.characterAsset.m_AssetPath))
            {
                skillData.characterPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        "Assets/Resources/" + skillData.characterAsset.m_AssetPath + ".prefab");
            }


            for (int i = 0; i < skillData.attachFrames.Length; ++i)
            {
                var attach = skillData.attachFrames[i];
                if (attach != null && attach.entityPrefab == null && attach.entityAsset.m_AssetPath != null)
                {
                    attach.entityPrefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            "Assets/Resources/" + attach.entityAsset.m_AssetPath + ".prefab");
                    attach.attach.ObjectPrefab = attach.entityPrefab;
                }
            }


            fpsTemp = skillData.fps;
            speedTemp = skillData.animationSpeed;
            animationSpeedTemp = skillData.animationSpeed;
            totalFramesTemp = skillData.totalFrames;
            RefreashSkillData(skillData);
        }

//         if (null == xlsxDataManager)
//         {
//             xlsxDataManager = XlsxDataManager.Instance();
//             xlsxDataManager.AddMainEnterPoint("SkillTable");
//             xlsxDataManager.AddMainEnterPoint("EffectTable");
//         }
    }

    private string mLastDirName = string.Empty;

    private void SetAllSkillData()
    {
        string dirPath = Path.GetDirectoryName(mSelectSkillDataPath);
        if (string.IsNullOrEmpty(dirPath))
        {
            Debug.LogError(string.Format("所选的技能配置文件:{0}   处于的文件夹为null", mSelectSkillDataPath));
            return;
        }

        mAllSkillStepDic.Clear();
        mMainSkillWithStepSkills.Clear();
        if (!dirPath.Equals(mLastDirName))
        {
            mAllSkillStepDic.Clear();
            mMainSkillWithStepSkills.Clear();
        }

        mLastDirName = dirPath;
        var allGUIDs = AssetDatabase.FindAssets("t:DSkillData", new string[] {dirPath});
        foreach (var guid in allGUIDs)
        {
            string fileFullPath = AssetDatabase.GUIDToAssetPath(guid);
            DSkillData dSkillData = AssetDatabase.LoadAssetAtPath<DSkillData>(fileFullPath);
            if (dSkillData != null && dSkillData.skillID != 0)
            {
                if (!mAllSkillStepDic.ContainsKey(dSkillData.skillID))
                {
                    mAllSkillStepDic.Add(dSkillData.skillID, dSkillData);
                }

                if (dSkillData.skillPhases.Length != 0) //拥有技能阶段的技能
                {
                    if (!mMainSkillWithStepSkills.ContainsKey(dSkillData.skillID))
                    {
                        List<int> stepSkills = new List<int>();
                        stepSkills = dSkillData.skillPhases.ToList();
                        mMainSkillWithStepSkills.Add(dSkillData.skillID, stepSkills);
                    }
                    else
                    {
                        mMainSkillWithStepSkills[dSkillData.skillID] = dSkillData.skillPhases.ToList();
                    }
                }
            }
        }
    }

    private void _RefreshParentSkillDataList(string path)
    {
        string[] pathArr = path.Split('/');
        if (pathArr.Length < 2) return;

        string parentPath = null;
        for (int i = 0; i < pathArr.Length - 1; i++)
        {
            parentPath += pathArr[i] + "/";
        }

        string[] pathList = Directory.GetFiles(parentPath, "*.asset", SearchOption.AllDirectories);

        _parentFolderSkillNameList.Clear();
        _parentFolderSkillIdList.Clear();

        if (_parentSkillDataDic.Count > 100)
            _parentSkillDataDic.Clear();

        for (int i = 0; i < pathList.Length; i++)
        {
            var fullPath = pathList[i];
            if (path == fullPath) continue;

            DSkillData data = null;
            if (!_parentSkillDataDic.ContainsKey(fullPath))
            {
                data = AssetDatabase.LoadAssetAtPath(fullPath, typeof(DSkillData)) as DSkillData;
                _parentSkillDataDic.Add(fullPath, data);
            }
            else
            {
                data = _parentSkillDataDic[fullPath];
            }

            if (data == null) continue;

            if (!_parentFolderSkillNameList.Contains(data.moveName))
                _parentFolderSkillNameList.Add(data.moveName);
            else
                Logger.LogErrorFormat("父目录下存在重名技能配置文件 path:{0}", fullPath);

            if (!_parentFolderSkillIdList.Contains(data.skillID))
                _parentFolderSkillIdList.Add(data.skillID);
            else if (data.skillID != 0)
                Logger.LogErrorFormat("父目录下存在Id重复技能配置文件 path:{0}", fullPath);
        }
    }

    public void RefreashSkillData(DSkillData skillData)
    {
        if (skillData.characterPrefab)
        {
            animClip = null;
            animClipsName = new string[0];
            animClipsSelected = -1;
            List<AnimationClip> listac = new List<AnimationClip>();
            List<string> listname = new List<string>();

            animations = skillData.characterPrefab.GetComponent<Animation>();
            GeAnimDescProxy proxy = skillData.characterPrefab.GetComponent<GeAnimDescProxy>();
            if (null != proxy)
            {
                animClips = proxy.animDescArray;
                if (null != animClips)
                {
                    for (int i = 0, icnt = animClips.Length; i < icnt; ++i)
                    {
                        animClips[i].animClipData =
                            AssetDatabase.LoadAssetAtPath("Assets/Resources/" + animClips[i].animClipPath,
                                typeof(AnimationClip)) as AnimationClip;
                        listac.Add(animClips[i].animClipData);
                        listname.Add(animClips[i].animClipName);
                    }

                    animClipsName = listname.ToArray();
                    animClipsSelected = listname.IndexOf(skillData.animationName);

                    if (0 <= animClipsSelected && animClipsSelected < listac.Count)
                        animClip = listac[animClipsSelected];
                    else
                        animClip = null;
                }
            }
            else
            {
                if (null != animPack)
                    DestroyImmediate(animPack);

                animPack = _LoadAnimPackage(skillData.characterAsset.m_AssetPath);
                if (null != animPack)
                {
                    proxy = animPack.GetComponent<GeAnimDescProxy>();
                    if (null != proxy)
                    {
                        animClips = proxy.animDescArray;
                        if (null != animClips)
                        {
                            for (int i = 0, icnt = animClips.Length; i < icnt; ++i)
                            {
                                animClips[i].animClipData =
                                    AssetDatabase.LoadAssetAtPath("Assets/Resources/" + animClips[i].animClipPath,
                                        typeof(AnimationClip)) as AnimationClip;
                                listac.Add(animClips[i].animClipData);
                                listname.Add(animClips[i].animClipName);
                            }

                            animClipsName = listname.ToArray();
                            animClipsSelected = listname.IndexOf(skillData.animationName);

                            if (0 <= animClipsSelected && animClipsSelected < listac.Count)
                                animClip = listac[animClipsSelected];
                            else
                                animClip = null;
                        }
                    }
                    else
                    {
                        if (null != animPack)
                        {
                            DestroyImmediate(animPack);
                            animPack = null;
                        }
                    }
                }
                else
                {
                    if (animations)
                    {
                        if (animations != null && animations.GetClipCount() > 0)
                        {
                            foreach (AnimationState ans in animations)
                            {
                                listac.Add(ans.clip);
                                listname.Add(ans.name);
                            }
                        }

                        animClipsName = listname.ToArray();
                        animClipsSelected = listname.IndexOf(skillData.animationName);

                        AnimationState state = animations[skillData.animationName];

                        if (state)
                        {
                            animClip = state.clip;
                        }
                    }
                }
            }
        }
    }

    public GameObject _LoadAnimPackage(string actorRes)
    {
        if (!string.IsNullOrEmpty(actorRes))
        {
            string path = "Assets/Resources/" + GeDemoActor.GetAvatarResPath(actorRes) + ".asset";
#if !LOGIC_SERVER
            DModelData avatarData = AssetDatabase.LoadAssetAtPath(path, typeof(DModelData)) as DModelData;
            if (null != avatarData)
            {
                path = "Assets/Resources/" + avatarData.modelAvatar.m_AssetPath + "_AnimPack02.prefab";
                return GameObject.Instantiate(AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject);
            }
#endif
        }

        return null;
    }

    public bool PosInRef(Vector3 vPos, Vector3 vRef, float fWidth)
    {
        float fDeltaX, fDeltaY;
        fDeltaX = Mathf.Abs(vPos.x - vRef.x);
        fDeltaY = Mathf.Abs(vPos.y - vRef.y);

        if (fDeltaX <= fWidth && fDeltaY <= fWidth)
        {
            return true;
        }

        return false;
    }

    public bool OnSceneGUI_EffectEdit(DSkillDataEditorDrawer proxyScript, SceneView sceneview)
    {
        if (proxyScript.skilleditmode == SkillSenceEditMode.EffectEdit)
        {
            if (proxyScript.currentEditObject != null)
            {
                EffectsFrames ef = proxyScript.currentEditObject as EffectsFrames;

                if (ef == null)
                {
                    return true;
                }

                DSkillEditor.DEditorEffects effecttest = ef.effects;

                if (Tools.current == Tool.Move)
                {
                    if (effecttest != null && effecttest.rootObject != null)
                    {
                        Vector3 worldPostion = effecttest.rootObject.transform.position;
                        Vector3 position = Handles.PositionHandle(worldPostion,
                            Tools.pivotRotation == PivotRotation.Global
                                ? Quaternion.identity
                                : effecttest.rootObject.transform.rotation);

                        if (position != worldPostion)
                        {
                            SkillDataUndo("Effect");

                            Vector3 delta = position - worldPostion;
                            if (effecttest.rootObject.transform.parent != null)
                            {
                                delta = effecttest.rootObject.transform.parent.InverseTransformVector(delta);
                            }

                            ef.localPosition += delta;
                        }

                        //Undo.undoRedoPerformed

                        if (effecttest.rootObject.transform.localPosition != ef.localPosition)
                        {
                            effecttest.rootObject.transform.localPosition = ef.localPosition;
                            OnSamplerEffectFrames(GetCurSkillData());
                            EditorUtility.SetDirty(effecttest.rootObject);
                            sceneview.Repaint();
                        }
                    }
                }
                else if (Tools.current == Tool.Rotate)
                {
                    if (effecttest != null && effecttest.rootObject != null)
                    {
                        Quaternion rotation = Handles.RotationHandle(Tools.pivotRotation == PivotRotation.Global
                            ? rotatetion_global
                            : effecttest.rootObject.transform.rotation, effecttest.rootObject.transform.position);
                        Quaternion localrotation = ef.localRotation;

                        if (Tools.pivotRotation == PivotRotation.Global)
                        {
                            localrotation *= Quaternion.Inverse(rotatetion_global) * rotation;
                            rotatetion_global = rotation;
                        }
                        else
                        {
                            localrotation *= Quaternion.Inverse(effecttest.rootObject.transform.rotation) * rotation;
                        }


                        if (effecttest.rootObject.transform.localRotation != localrotation)
                        {
                            SkillDataUndo("Effect");

                            ef.localRotation = localrotation;
                            effecttest.rootObject.transform.localRotation = ef.localRotation;
                            OnSamplerEffectFrames(GetCurSkillData());
                            EditorUtility.SetDirty(effecttest.rootObject);
                            sceneview.Repaint();
                        }
                    }
                }
                else if (Tools.current == Tool.Scale)
                {
                    if (effecttest != null && effecttest.rootObject != null)
                    {
                        if (ef.localScale == Vector3.zero)
                        {
                            ef.localScale = Vector3.one;
                        }

                        Vector3 localScale;

                        localScale = Handles.ScaleHandle(
                            ef.localScale,
                            effecttest.rootObject.transform.position,
                            Quaternion.identity,
                            1.0f);

                        if (effecttest.rootObject.transform.localScale != localScale)
                        {
                            SkillDataUndo("Effect");

                            ef.localScale = localScale;
                            ef.AdjustScale();
                            effecttest.UpdateData(ef);
                            OnSamplerEffectFrames(GetCurSkillData());
                            EditorUtility.SetDirty(effecttest.rootObject);
                            sceneview.Repaint();
                        }
                    }
                }


                return true;
            }
        }

        return false;
    }


    public void ProcessSceneGUI(SceneView sceneview)
    {
        if (character == null)
        {
            return;
        }

        DSkillDataEditorDrawer proxyScript = character.GetComponent<DSkillDataEditorDrawer>();
        if (proxyScript != null)
        {
            Handles.BeginGUI();
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 50),
                skillData.moveName + " 帧" + animFrame + "\n当前模式:" +
                (_Play ? "播放模式" : proxyScript.skilleditmode.ToString()), showlableStyle);
            GUI.Label(new Rect(0, 50, 200, 50), "视角：" + (bEditorView ? "编辑视角" : ("游戏视角" + GameViewAngle + "度")),
                showlableStyle);
            Handles.EndGUI();

            Event evt = Event.current;
            if (OnSenceGUI_EntityEdit(proxyScript))
            {
                return;
            }

            if (OnSceneGUI_EffectEdit(proxyScript, sceneview))
            {
                return;
            }

            if (evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.LeftArrow)
                {
                    animFrame = animFrame - 1;
                    if (animFrame < 0)
                    {
                        if (skillData.skillPhases.Length > 0)
                        {
                            ClearChar();
                            mCurSkillStep--;
                            if (mCurSkillStep <= 0)
                            {
                                mCurSkillStep = 0;
                            }

                            RefreashSkillData(GetCurSkillData());
                            CreateChar();
                            animFrame = GetCurSkillData().totalFrames - 1;
                            // needCreateChar = true;
                        }
                        else
                        {
                            animFrame = 0;
                        }
                    }

                    AnimationSampler(GetCurSkillData());
                    evt.Use();
                    this.Repaint();
                    return;
                }
                else if (evt.keyCode == KeyCode.RightArrow)
                {
                    animFrame = animFrame + 1;
                    if (animFrame > GetCurSkillData().totalFrames - 1)
                    {
                        if (skillData.skillPhases.Length > 0)
                        {
                            ClearChar();
                            mCurSkillStep++;
                            if (mCurSkillStep >= skillData.skillPhases.Length - 1)
                            {
                                mCurSkillStep = skillData.skillPhases.Length - 1;
                            }

                            RefreashSkillData(GetCurSkillData());
                            CreateChar();
                            animFrame = 0;
                            //needCreateChar = true;
                        }
                        else
                        {
                            animFrame = GetCurSkillData().totalFrames - 1;
                        }

                        // Refresh();
                    }

                    AnimationSampler(GetCurSkillData());
                    evt.Use();
                    this.Repaint();
                    return;
                }
                else if (evt.keyCode == KeyCode.Space)
                {
                    if (skillData.skillPhases.Length > 0)
                    {
                        PlayAllStep(!mIsPlayAllStep);
                    }
                    else
                    {
                        Play(!_Play);
                    }

                    evt.Use();
                    this.Repaint();
                    return;
                }
            }
            else if (evt.type == EventType.MouseDown)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
                if (ray.origin.x > 500) 
                    ray.origin = new Vector3(ray.origin.x-1000, ray.origin.y, ray.origin.z);

                if (evt.button == 0)
                {
                    if (proxyScript.skilleditmode == SkillSenceEditMode.HurtBoxEdit
                        && DSkillData.CheckVFliter(DSkillData.vfliter, VisiableFliter.HurtBox))
                    {
                        if (skillData.HurtBlocks.Length == skillData.totalFrames)
                        {
                            BaseDecisionBox block = skillData.HurtBlocks[GetCurrentFrame()];

                            foreach (ShapeBox data in block.boxs)
                            {
                                float fx = Mathf.Abs(ray.origin.x - data.center.x);
                                float fy = Mathf.Abs(ray.origin.y - data.center.y);

                                if (fx <= data.size.x / 2 && fy <= data.size.y / 2)
                                {
                                    proxyScript.currentEditObject = data;
                                    goto end; //之前这里直接Return;而之后的MouseUp，MouseDrag都会对Event.current做拦截，直接引起Unity编辑器GUI卡死；
                                }
                            }
                        }
                    }

                    if (proxyScript.skilleditmode == SkillSenceEditMode.DefBoxEdit
                        && DSkillData.CheckVFliter(DSkillData.vfliter, VisiableFliter.DefBox))
                    {
                        if (skillData.DefenceBlocks.Length == skillData.totalFrames)
                        {
                            BaseDecisionBox block = skillData.DefenceBlocks[GetCurrentFrame()];

                            foreach (ShapeBox data in block.boxs)
                            {
                                float fx = Mathf.Abs(ray.origin.x - data.center.x);
                                float fy = Mathf.Abs(ray.origin.y - data.center.y);

                                if (fx <= data.size.x / 2 && fy <= data.size.y / 2)
                                {
                                    proxyScript.currentEditObject = data;
                                    goto end;
                                }
                            }
                        }
                    }
                }

                proxyScript.editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.Null;

                if (proxyScript.currentEditObject == null)
                {
                    goto end;
                }

                if (
                    (proxyScript.skilleditmode == SkillSenceEditMode.DefBoxEdit
                     && DSkillData.CheckVFliter(DSkillData.vfliter, VisiableFliter.DefBox))
                    ||
                    (proxyScript.skilleditmode == SkillSenceEditMode.HurtBoxEdit
                     && DSkillData.CheckVFliter(DSkillData.vfliter, VisiableFliter.HurtBox))
                )
                {
                    ShapeBox boxdata = proxyScript.currentEditObject as ShapeBox;

                    if (boxdata != null)
                    {
                        Vector3 lefttop, midtop, righttop, rightmid, rightbottom, midbottom, leftbottom, leftmid;
                        Vector3 center;

                        Vector3 hurtBoxPosition = new Vector3(boxdata.center.x, boxdata.center.y, 0);
                        //if (!detect3dHits) hurtBoxPosition.z = -1;
                        Vector3 hurtBoxSize = new Vector3(boxdata.size.x, boxdata.size.y, 0);

                        lefttop = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(-0.5f, 0.5f, 0));
                        rightbottom = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(0.5f, -0.5f, 0));
                        leftbottom = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(-0.5f, -0.5f, 0));
                        righttop = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(0.5f, 0.5f, 0));

                        center = hurtBoxPosition;
                        midtop = (lefttop + righttop) / 2;
                        rightmid = (righttop + rightbottom) / 2;
                        midbottom = (rightbottom + leftbottom) / 2;
                        leftmid = (leftbottom + lefttop) / 2;
                        float fWidth = DSkillDataEditorDrawer.Blocks_EditWidth;
                        //proxyScript.editBoxesMode = DSkillDataEditorProxy.BlocksEditMode.Null;
                        DSkillDataEditorDrawer.BlocksEditMode editBoxesMode =
                            DSkillDataEditorDrawer.BlocksEditMode.Null;
                        if (PosInRef(ray.origin, center, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.Center;
                        }
                        else if (PosInRef(ray.origin, lefttop, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.LeftTop;
                        }
                        else if (PosInRef(ray.origin, midtop, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.MidTop;
                        }
                        else if (PosInRef(ray.origin, righttop, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.RightTop;
                        }
                        else if (PosInRef(ray.origin, rightmid, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.RightMid;
                        }
                        else if (PosInRef(ray.origin, rightbottom, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.RightBottom;
                        }
                        else if (PosInRef(ray.origin, midbottom, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.MidBottom;
                        }
                        else if (PosInRef(ray.origin, leftbottom, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.LeftBottom;
                        }
                        else if (PosInRef(ray.origin, leftmid, fWidth))
                        {
                            editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.LeftMid;
                        }

                        if (proxyScript.editBoxesMode != editBoxesMode)
                        {
                            Undo.RecordObject(proxyScript, "EditMode");
                            proxyScript.editBoxesMode = editBoxesMode;
                        }
                    }
                }
            }
            else if (evt.type == EventType.MouseUp)
            {
                if (proxyScript.currentEditObject == null)
                {
                    goto end;
                }

                proxyScript.editBoxesMode = DSkillDataEditorDrawer.BlocksEditMode.Null;
            }
            else if (evt.type == EventType.MouseDrag)
            {
                if (proxyScript.currentEditObject == null)
                {
                    goto end;
                }

                if (proxyScript.editBoxesMode != DSkillDataEditorDrawer.BlocksEditMode.Null)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
                    if (ray.origin.x > 500) 
                        ray.origin = new Vector3(ray.origin.x-1000, ray.origin.y, ray.origin.z);

                    ShapeBox boxdata = proxyScript.currentEditObject as ShapeBox;

                    if (boxdata == null)
                    {
                        goto end;
                    }

                    Vector3 lefttop, midtop, righttop, rightmid, rightbottom, midbottom, leftbottom, leftmid;
                    Vector3 center;

                    Vector3 hurtBoxPosition = new Vector3(boxdata.center.x, boxdata.center.y, 0);
                    //if (!detect3dHits) hurtBoxPosition.z = -1;
                    Vector3 hurtBoxSize = new Vector3(boxdata.size.x, boxdata.size.y, 0);

                    lefttop = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(-0.5f, 0.5f, 0));
                    rightbottom = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(0.5f, -0.5f, 0));
                    leftbottom = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(-0.5f, -0.5f, 0));
                    righttop = hurtBoxPosition + Vector3.Scale(hurtBoxSize, new Vector3(0.5f, 0.5f, 0));

                    center = hurtBoxPosition;
                    midtop = (lefttop + righttop) / 2;
                    rightmid = (righttop + rightbottom) / 2;
                    midbottom = (rightbottom + leftbottom) / 2;
                    leftmid = (leftbottom + lefttop) / 2;

                    if (proxyScript.editBoxesMode != DSkillDataEditorDrawer.BlocksEditMode.Null)
                    {
                        SkillDataUndo("BOX");
                    }

                    if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.Center)
                    {
                        boxdata.center.x = ray.origin.x;
                        boxdata.center.y = ray.origin.y;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.LeftTop)
                    {
                        lefttop = ray.origin;
                        boxdata.center = (lefttop + rightbottom) / 2;
                        boxdata.size.x = rightbottom.x - lefttop.x;
                        boxdata.size.y = lefttop.y - rightbottom.y;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.LeftBottom)
                    {
                        leftbottom = ray.origin;
                        boxdata.center = (leftbottom + righttop) / 2;
                        boxdata.size.x = righttop.x - leftbottom.x;
                        boxdata.size.y = righttop.y - leftbottom.y;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.RightTop)
                    {
                        righttop = ray.origin;
                        boxdata.center = (leftbottom + righttop) / 2;
                        boxdata.size.x = righttop.x - leftbottom.x;
                        boxdata.size.y = righttop.y - leftbottom.y;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.RightBottom)
                    {
                        rightbottom = ray.origin;
                        boxdata.center = (lefttop + rightbottom) / 2;
                        boxdata.size.x = rightbottom.x - lefttop.x;
                        boxdata.size.y = lefttop.y - rightbottom.y;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.LeftMid)
                    {
                        leftmid = ray.origin;
                        boxdata.center.x = (leftmid.x + rightmid.x) / 2;
                        boxdata.size.x = rightmid.x - leftmid.x;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.RightMid)
                    {
                        rightmid = ray.origin;
                        boxdata.center.x = (leftmid.x + rightmid.x) / 2;
                        boxdata.size.x = rightmid.x - leftmid.x;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.MidTop)
                    {
                        midtop = ray.origin;
                        boxdata.center.y = (midtop.y + midbottom.y) / 2;
                        boxdata.size.y = midtop.y - midbottom.y;
                    }
                    else if (proxyScript.editBoxesMode == DSkillDataEditorDrawer.BlocksEditMode.MidBottom)
                    {
                        midbottom = ray.origin;
                        boxdata.center.y = (midtop.y + midbottom.y) / 2;
                        boxdata.size.y = midtop.y - midbottom.y;
                    }
                }
            }


            end:
            if (proxyScript.skilleditmode == SkillSenceEditMode.HurtBoxEdit
                ||
                proxyScript.skilleditmode == SkillSenceEditMode.DefBoxEdit)
            {
                if (evt.type != EventType.Repaint && evt.type != EventType.Layout)
                    evt.Use(); //这么写主要是对GUI事件做拦截，让鼠标键盘的事件尽量只有技能编辑器做响应；如果正在编辑攻击框，鼠标拖动时，sceneView里的的视角并不会随着移动；
            }
        }
    }

    Rect HurtEditPopupRect;
    Rect DefEditPopupRect;
    Rect EffectEditPopupRect;

    public static void CreateAsset<T>() where T : DSkillData
    {
        T asset = FileTools.CreateAsset<T>("New SkillData");

        if (asset is DSkillData)
        {
            DSkillData skilldata = asset as DSkillData;
            skilldata.moveName = "Skill";
            DSkillDataEditorWindow.Init();
        }
    }

    public static void CreateAsset<T>(int skillId, GameObject characterPrefab) where T : DSkillData
    {
        T asset = FileTools.CreateAsset<T>("New SkillData");

        if (asset is DSkillData)
        {
            DSkillData skilldata = asset as DSkillData;
            skilldata.moveName = "Skill";
            skilldata.skillID = skillId;
            skilldata.characterPrefab = characterPrefab;
            Init();
        }
    }

    Vector3 initRotation;

    void CreateChar()
    {
        if (GetCurSkillData().characterPrefab == null)
        {
            GetCurSkillData().characterPrefab = skillData.characterPrefab;
        }
        character = (GameObject) PrefabUtility.InstantiatePrefab(GetCurSkillData().characterPrefab);
        character.transform.localPosition = new Vector3(0, 0, 0);
        initRotation = character.transform.localRotation.eulerAngles;
        bNew = true;
        if (bNew == false) character.transform.Rotate(0, 90, 0);

        DSkillData.root = new GameObject();
        
        DSkillData.root.name = "SkillEditorRoot"; 

        DSkillData.actor = new GameObject();
        DSkillData.actor.name = "ActorRoot"; 

        DSkillData.attach = new GameObject();
        DSkillData.attach.name = "AttachRoot";

        DSkillData.attach.transform.SetParent(DSkillData.root.transform);
        DSkillData.actor.transform.SetParent(DSkillData.root.transform);

        DSkillData.root.hideFlags = HideFlags.DontSaveInEditor;
        DSkillData.actor.hideFlags = HideFlags.DontSaveInEditor;
        DSkillData.attach.hideFlags = HideFlags.DontSaveInEditor;

        character.transform.SetParent(DSkillData.actor.transform, true);
        DSkillData.root.transform.SetSiblingIndex(0);
        //DSkillData.attach.transform.SetSiblingIndex(1);

        DSkillData.root.transform.localPosition = new Vector3(1000, 0, 0);

        interpolationSpeed = 1.0f;

        DSkillDataEditorDrawer proxy = character.GetComponent<DSkillDataEditorDrawer>();
        if (proxy)
        {
            Editor.DestroyImmediate(proxy);
        }

        character.AddComponent<DSkillDataEditorDrawer>();
        AnimationSampler(GetCurSkillData());
        UpdateAttachNodes();
    }

    bool needCreateChar = false;

    void Preview()
    {
        if (skillData.characterPrefab == null)
        {
            UnityEngine.Debug.Log("Drag a character into 'Character Prefab' first.");
            return;
        }
        else if (IsEditorPlaying())
        {
            UnityEngine.Debug.Log("You can't preview animations while in play mode.");
            return;
        }

        Undo.undoRedoPerformed += Refresh;

        ShowGizmo(false);

        BackSceneCamera(this);

        DEditorCamera.SetPosition(DEditorCamera.SkillEditorDefaultCameraPos);
        if (bEditorView)
        {
            DEditorCamera.SetRotation(Quaternion.identity);
        }
        else
        {
            DEditorCamera.SetRotation(Quaternion.Euler(GameViewAngle, 0, 0));
        }

        DEditorCamera.SetOrthographic(true);
        DEditorCamera.SetSize(DSkillDataEditorDrawer.Camera_ViewSize);
        needCreateChar = true;
    }

    void Play(bool bPlay)
    {
        if (bPlay)
        {
            RefreashSkillData(GetCurSkillData());
            _Play = true;
            _smoothviewback = smoothPreview;
            smoothPreview = true;
            animFrame = 0.0f;
            DSkillData.play = true;
            DSkillData.time = 0.0f;
            playBeginTime = EditorApplication.timeSinceStartup;
            prePlayTime = playBeginTime;
        }
        else
        {
            _Play = false;
            smoothPreview = _smoothviewback;
            DSkillData.play = false;
            playBeginTime = EditorApplication.timeSinceStartup;
            prePlayTime = playBeginTime;
        }
    }


    void PlayAllStep(bool isPlay)
    {
        if (isPlay)
        {
            ClearChar();
            mCurSkillStep = 0;
            RefreashSkillData(skillData);
            CreateChar();
            mIsPlayAllStep = true;
            _smoothviewback = smoothPreview;
            smoothPreview = true;
            animFrame = 0.0f;
            DSkillData.play = true;
            DSkillData.time = 0.0f;
            playBeginTime = EditorApplication.timeSinceStartup;
            prePlayTime = playBeginTime;
        }
        else
        {
            mIsPlayAllStep = false;
            smoothPreview = _smoothviewback;
            DSkillData.play = false;
            playBeginTime = EditorApplication.timeSinceStartup;
            prePlayTime = playBeginTime;
        }
    }

    public void SetEditorView()
    {
        if (bEditorView)
        {
            return;
        }

        DEditorCamera.SetPosition(DEditorCamera.SkillEditorDefaultCameraPos);
        DEditorCamera.SetRotation(Quaternion.identity);
        DEditorCamera.SetOrthographic(true);
        DEditorCamera.SetSize(DSkillDataEditorDrawer.Camera_ViewSize);
        bEditorView = true;
    }

    private DSkillData GetCurSkillData()
    {
        DSkillData tmpSkillData = skillData;
        int skillId = 0;
        if (bSkillTotalPreview)
        {
            if (mAllSkillStepDic != null && skillData.skillPhases.Length > 0)
            {
                if (mCurSkillStep < skillData.skillPhases.Length && mCurSkillStep >= 0)
                {
                    skillId = skillData.skillPhases[mCurSkillStep];
                    mAllSkillStepDic.TryGetValue(skillId, out tmpSkillData);
                }
                else
                {
                    Debug.LogError(string.Format("当前索引：{0}越界", mCurSkillStep));
                }
            }
        }

        return tmpSkillData;
    }

    void TabLabel(System.Object obj, string name, DSkillDataEditorDrawer proxyScript)
    {
        EditorGUILayout.LabelField(name, proxyScript.currentEditObject == obj ? SelectedTablabelStyle : TablabelStyle);
    }

    bool HandleGUIEvent()
    {
        Event evt = Event.current;
        if (evt.type == EventType.KeyDown)
        {
            if (evt.keyCode == KeyCode.LeftArrow)
            {
                animFrame = animFrame - 1;
                if (animFrame < 0)
                {
                    if (bSkillTotalPreview)
                    {
                        ClearChar();
                        mCurSkillStep--;
                        if (mCurSkillStep <= 0)
                        {
                            mCurSkillStep = 0;
                        }

                        RefreashSkillData(GetCurSkillData());
                        CreateChar();
                        animFrame = GetCurSkillData().totalFrames - 1;
                        // needCreateChar = true;
                    }
                    else
                    {
                        animFrame = 0;
                    }
                }

                AnimationSampler(GetCurSkillData());
                evt.Use();
                this.Repaint();
                return true;
            }
            else if (evt.keyCode == KeyCode.RightArrow)
            {
                animFrame = animFrame + 1;
                if (animFrame > GetCurSkillData().totalFrames - 1)
                {
                    if (bSkillTotalPreview)
                    {
                        ClearChar();
                        mCurSkillStep++;
                        if (mCurSkillStep >= skillData.skillPhases.Length - 1)
                        {
                            mCurSkillStep = skillData.skillPhases.Length - 1;
                        }

                        RefreashSkillData(GetCurSkillData());
                        CreateChar();
                        animFrame = 0;
                        //needCreateChar = true;
                    }
                    else
                    {
                        animFrame = GetCurSkillData().totalFrames - 1;
                    }

                    // Refresh();
                }

                AnimationSampler(GetCurSkillData());
                evt.Use();
                this.Repaint();
                return true;
            }
        }

        return false;
    }

    bool bToggle;

    private Dictionary<string, Dictionary<int, bool>> foldeDicts = new Dictionary<string, Dictionary<int, bool>>();

    private bool GetFoldeDict(string key, int idx, bool defaultValue = false)
    {
        Dictionary<int, bool> itemDict = null;
        if (!foldeDicts.ContainsKey(key))
        {
            foldeDicts.Add(key, new Dictionary<int, bool>());
        }

        itemDict = foldeDicts[key];

        if (!itemDict.ContainsKey(idx))
        {
            itemDict.Add(idx, defaultValue);
        }

        return itemDict[idx];
    }

    private bool SetFoldeDict(string key, int idx, bool value)
    {
        Dictionary<int, bool> itemDict = null;

        if (!foldeDicts.ContainsKey(key))
        {
            foldeDicts.Add(key, new Dictionary<int, bool>());
        }

        itemDict = foldeDicts[key];

        if (!itemDict.ContainsKey(idx))
        {
            itemDict.Add(idx, false);
        }

        itemDict[idx] = value;

        return itemDict[idx];
    }

    //private int iSkillSelectedLine = -1;
    //private float fLastSkillTipTime = 10.0f;

    public void OnGUI()
    {
        GUIControls.UIStyles.UpdateEditorStyles();

        if (mCopySkillData == null)
        {
            mLoadCopySkillDataAsset();
        }

        if (skillData == null)
        {
            GUILayout.BeginHorizontal("GroupBox");
            GUILayout.Label("Create or Select a SkillActionData File", "CN EntryInfo");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("创建技能配置文件"))
                CreateAsset<DSkillData>();

            if (GUILayout.Button("定位默认路径"))
            {
                UnityEngine.Object obj =
                    AssetDatabase.LoadAssetAtPath("Assets/Resources/Data/SkillData", typeof(UnityEngine.Object));
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }

            GUI.color = Color.yellow;
            if (GUILayout.Button(m_kStateString))
            {
                /// 在这里更换升级函数
                // v1.7a to v1.8 
                _UpdateAsset(_UpdateFunc_AddDataChunk_DAssetObject);
            }

            if (GUILayout.Button("选择目录资源升级"))
            {
                UnityEngine.Object[] selection =
                    Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
                string fullPath = FileTools.GetAssetFullPath(selection[0]);

                _UpdateAsset(_UpdateFunc_AddDataChunk_DAssetObject, fullPath);
            }

            GUI.color = Color.white;

            return;
        }


        if (character != null)
        {
            if (HandleGUIEvent() == true)
            {
                return;
            }
        }

        EditorGUIUtility.labelWidth = 200;
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.font = (Font) EditorGUIUtility.Load("PingFangBold.TTF");
        fontStyle.fontSize = 24;
        fontStyle.alignment = TextAnchor.UpperCenter;
        fontStyle.normal.textColor = Color.white;
        fontStyle.hover.textColor = Color.white;

        EditorGUILayout.BeginVertical(titleStyle);
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("", skillData.name, fontStyle, GUILayout.Height(32));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        if (animClip != null && animClip.length > 0.0f)
        {
            skillData.totalFrames =
                (int) Mathf.Abs(Mathf.Ceil((skillData.fps * animClip.length) / skillData.animationSpeed));
            totalFramesTemp = (int) Mathf.Abs(Mathf.Ceil((skillData.fps * animClip.length) / animationSpeedTemp));

            //这里计算总时间大于帧总数的情况，并且技能不循环
            if (skillData.isLoop == 0 && skillData.notLoopLastFrame)
            {
                var realFrames = (int) Mathf.Ceil(skillData.skilltime * skillData.fps);
                if (realFrames > skillData.totalFrames)
                {
                    skillData.totalFrames = realFrames;
                    totalFramesTemp = realFrames;
                }
            }
        }
        else
        {
            skillData.totalFrames = (int) Mathf.Ceil(skillData.skilltime * skillData.fps);
            skillData.totalFrames = Math.Max(1, skillData.totalFrames);
            totalFramesTemp = skillData.totalFrames;
        }
        /*
        GUIControls.IGUIElement[] element = {
                new GUIControls.GUIScrollViewBlock(),
                new GUIControls.GUIScrollViewBlock()
            };
            */

        using (new EditorGUILayout.VerticalScope(rootGroupStyle))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.color = Color.green;
                if (GUILayout.Button("显示技能文件"))
                {
                    EditorGUIUtility.PingObject(skillData);
                }

                GUI.color = Color.red;
                if (GUILayout.Button("返回主页面"))
                {
                    skillData = null;
                    mAllSkillStepDic.Clear();
                    mMainSkillWithStepSkills.Clear();
                    Clear(true, true);
                    return;
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _OnGUIDrawHeaderUtility();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        {
            EditorGUILayout.BeginVertical(rootGroupStyle);
            bSkillTotalPreview = EditorGUILayout.Toggle("打开技能整体预览", bSkillTotalPreview, toggleStyle);
            if (bSkillTotalPreview)
            {
                if (skillData.skillPhases.Length > 0)
                {
                    mCircleAnimaPlayTime = EditorGUILayout.IntField("时长很长的循环动画预览的时间:", mCircleAnimaPlayTime);
                }

                if (skillData.skillPhases.Length>0)
                {
                    if (mIsPlayAllStep)
                    {
                        if (StyledButton("暂停全部阶段动画"))
                        {
                            PlayAllStep(false);
                        }
                    }
                    else
                    {
                        if (StyledButton("播放全部阶段动画"))
                        {
                            PlayAllStep(true);
                        }
                    }
                }
            }
            

            #region 技能阶段的显示
           
            
                
                    
                    {
                        
                        GUILayout.Label("技能阶段的显示:");
                        int mainSkillId = 0;
                        if (skillData.skillPhases.Length > 0)
                        {
                            mainSkillId = skillData.skillID;
                        }
                        else
                        {
                            mainSkillId = skillData.skillID / 10;
                        }

                        if (mMainSkillWithStepSkills.ContainsKey(mainSkillId))
                        {
                            List<int> skillSteps = mMainSkillWithStepSkills[mainSkillId];
                            if (skillSteps != null)
                            {
                                for (int i = 0; i < skillSteps.Count; i++)
                                {
                                    EditorGUILayout.BeginHorizontal(rootGroupStyle);
                                    {
                                        //画出技能id对应的配置文件
                                        DSkillData dSkillData = null;
                                        if (mAllSkillStepDic != null &&
                                            mAllSkillStepDic.TryGetValue(skillSteps[i], out dSkillData))
                                        {
                                            EditorGUILayout.IntField("技能阶段" + (i + 1) + " 技能ID",
                                                skillSteps[i]);

                                            EditorGUILayout.ObjectField("技能id所对应的配置技能配置文件", dSkillData,
                                                typeof(DSkillData), true);
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }

                            EditorGUILayout.BeginHorizontal();
                            if (StyledButton("刷新"))
                            {
                                SetAllSkillData();
                            }
                            EditorGUILayout.EndHorizontal();
                            
                        }
                    }
                    EditorGUILayout.EndVertical();
                
            
            #endregion

            #region skill

            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                // Begin General Options
                EditorGUILayout.BeginHorizontal();
                {
                    generalOptions = EditorGUILayout.Foldout(generalOptions, "技能: " + skillData.moveName, foldStyle);
                }
                EditorGUILayout.EndHorizontal();

                if (generalOptions)
                {
                    EditorGUILayout.BeginVertical(rootGroupStyle);
                    {
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel += 1;

                        //var skillUnit = xlsxDataManager.GetXlsxByName("SkillTable");

                        //string[] optionList = skillUnit.GetKeyNameList();

                        //EditorGUIUtility.labelWidth = 180;
                        string warningInfo = "警告:父目录下存在";
                        if (_parentFolderSkillNameList.Contains(skillData.moveName))
                            warningInfo += "重名技能配置文件";
                        if (skillData.skillID != 0 && _parentFolderSkillIdList.Contains(skillData.skillID))
                            warningInfo += "ID重复技能配置文件";

                        if (warningInfo.Length > 9)
                            EditorGUILayout.LabelField("", warningInfo, _warningLableStyle);
                        else
                            EditorGUILayout.LabelField("", "", _warningLableStyle);
                        skillData.moveName = EditorGUILayout.TextField("技能名:", skillData.moveName);
                        skillData.description = EditorGUILayout.TextField("技能描述:", skillData.description);
                        skillData.skilltime = EditorGUILayout.FloatField("技能持续总时间:", skillData.skilltime);
                        skillData.skillID = EditorGUILayout.IntField("技能ID:", skillData.skillID);

                        //skillData.skillPriority = EditorGUILayout.IntField("技能优先级:", skillData.skillPriority);

                        int skillPhaseNum = EditorGUILayout.IntField("技能阶段数量:", skillData.skillPhases.Length);
                        skillPhaseNum = Mathf.Clamp(skillPhaseNum, 0, 20); //限制一下技能阶段数量范围，避免填错
                        if (skillPhaseNum != skillData.skillPhases.Length)
                        {
                            var tmp = skillData.skillPhases;
                            skillData.skillPhases = new int[skillPhaseNum];
                            for (int i = 0; i < Mathf.Min(tmp.Length, skillPhaseNum); ++i)
                                skillData.skillPhases[i] = tmp[i];
                        }

                        skillPhaseToggle = EditorGUILayout.Foldout(skillPhaseToggle,
                            "技能阶段 (" + skillData.skillPhases.Length + ")", EditorStyles.foldout);
                        if (skillPhaseToggle)
                        {
                            EditorGUILayout.BeginVertical(rootGroupStyle);
                            {
                                for (int i = 0; i < skillData.skillPhases.Length; ++i)
                                {
                                    EditorGUILayout.BeginHorizontal(rootGroupStyle);
                                    {
                                        skillData.skillPhases[i] = EditorGUILayout.IntField("技能阶段" + (i + 1) + " 技能ID",
                                            skillData.skillPhases[i]);

                                        //画出技能id对应的配置文件
                                        DSkillData dSkillData = null;
                                        if (mAllSkillStepDic != null &&
                                            mAllSkillStepDic.TryGetValue(skillData.skillPhases[i], out dSkillData))
                                        {
                                            EditorGUILayout.ObjectField("技能id所对应的配置技能配置文件", dSkillData,
                                                typeof(DSkillData), true);
                                        }
                                        else
                                        {
                                            EditorGUILayout.BeginHorizontal(rootGroupStyle);
                                            {
                                                EditorGUILayout.LabelField(string.Format("该技能id={0},没有对应的技能配置文件",
                                                    skillData.skillPhases[i]));
                                            }
                                            if (StyledButton("自动生成"))
                                            {
                                                CreateAsset<DSkillData>(skillData.skillPhases[i],
                                                    skillData.characterPrefab);
                                            }

                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }

                        skillData.relatedAttackSpeed = EditorGUILayout.Toggle(new GUIContent("此阶段是否受攻速影响:"),
                            skillData.relatedAttackSpeed, toggleStyle);
                        if (skillData.relatedAttackSpeed)
                        {
                            skillData.attackSpeed =
                                EditorGUILayout.IntField(new GUIContent("攻速加成:"), skillData.attackSpeed);
                        }

                        skillData.triggerType = GetChangeEnumType("下阶段触发条件:", skillData.triggerType);
                        //skillData.triggerType = (TriggerNextPhaseType)EditorGUILayout.EnumPopup("下阶段触发条件:", skillData.triggerType);
                        skillData.isLoop = EditorGUILayout.Toggle("技能是否循环:", (skillData.isLoop > 0), toggleStyle)
                            ? 1
                            : 0;

                        skillData.notLoopLastFrame =
                            EditorGUILayout.Toggle("不循环最后一帧:", skillData.notLoopLastFrame, toggleStyle);
                        skillData.loopAnimation = EditorGUILayout.Toggle(new GUIContent("是否循环动画:", "注释填写处"),
                            skillData.loopAnimation, toggleStyle);

                        skillComboToggle = EditorGUILayout.Foldout(skillComboToggle, "技能Combo", EditorStyles.foldout);
                        if (skillComboToggle)
                        {
                            skillData.comboSkillID = EditorGUILayout.IntField("Combo 技能ID", skillData.comboSkillID);
                            skillData.comboStartFrame =
                                EditorGUILayout.IntField("Combo 技能开始判定帧", skillData.comboStartFrame);
                        }

                        skillData.goHitEffect = (GameObject) EditorGUILayout.ObjectField("被击特效:", skillData.goHitEffect,
                            typeof(UnityEngine.GameObject), true);
                        skillData.goHitEffectAsset = new DAssetObject(skillData.goHitEffect);
                        skillData.hitEffectInfoTableId =
                            EditorGUILayout.IntField(new GUIContent("被击特效参数表ID:", "被击特效位置扩展"),
                                skillData.hitEffectInfoTableId);
                        skillData.goSFX = (UnityEngine.Object) EditorGUILayout.ObjectField("被击音效:", skillData.goSFX,
                            typeof(UnityEngine.Object), true);
                        skillData.goSFXAsset = new DAssetObject(skillData.goSFX);
                        skillData.hitSFXID = EditorGUILayout.IntField("击中音效ID:", skillData.hitSFXID);


                        grapToggle = EditorGUILayout.Foldout(grapToggle, "抓取信息配置", EditorStyles.foldout);
                        if (grapToggle)
                        {
                            EditorGUILayout.BeginVertical(rootGroupStyle);
                            {
                                EditorGUILayout.LabelField("*抓取数据必须在填写[抓取数量]后才会生效，且[抓取帧]只读取当前阶段的[抓取信息配置]",
                                    _warningLableStyle);
                                skillData.grabNum = EditorGUILayout.IntField("抓取数量:", skillData.grabNum);
                                skillData.grabPosx = EditorGUILayout.FloatField("抓取位置X:", skillData.grabPosx);
                                skillData.grabPosy = EditorGUILayout.FloatField("抓取位置Y:", skillData.grabPosy);
                                skillData.grabEndForceX =
                                    EditorGUILayout.FloatField("抓取结束力X:", skillData.grabEndForceX);
                                skillData.grabEndForceY =
                                    EditorGUILayout.FloatField("抓取结束力Y:", skillData.grabEndForceY);
                                skillData.hitSpreadOut = EditorGUILayout.Toggle("是否击散:", skillData.hitSpreadOut);
                                skillData.grabTime = EditorGUILayout.FloatField("抓取时间:", skillData.grabTime);
                                skillData.grabEndEffectType =
                                    EditorGUILayout.IntField("抓取结束中招反应类型:", skillData.grabEndEffectType);
                                ActionType tmpActionType = (ActionType) EditorGUILayout.EnumPopup("抓取执行动作:",
                                    (ActionType) (skillData.grabAction), enumStyle);
                                skillData.grabMoveSpeed = EditorGUILayout.FloatField("抓取移动速度", skillData.grabMoveSpeed);
                                skillData.grabSupportQuickPressDismis = EditorGUILayout.Toggle("抓取是否支持快速点按挣脱",
                                    skillData.grabSupportQuickPressDismis);
                                skillData.grabAction = (int) tmpActionType;
                                skillData.notGrabBati = EditorGUILayout.Toggle(new GUIContent("不抓取霸体单位", "不抓取霸体单位"),
                                    skillData.notGrabBati);
                                skillData.notGrabGeDang = EditorGUILayout.Toggle(new GUIContent("不抓取格挡单位", "不抓取格挡单位"),
                                    skillData.notGrabGeDang);
                                skillData.notUseGrabSetPos = EditorGUILayout.Toggle(
                                    new GUIContent("不使用抓取配置中的位置", "不使用抓取配置中的位置"), skillData.notUseGrabSetPos);
                                skillData.notGrabToBlock = EditorGUILayout.Toggle(new GUIContent("不能抓取进阻挡", "不能抓取进阻挡"),
                                    skillData.notGrabToBlock);
                                skillData.buffInfoId = EditorGUILayout.IntField(
                                    new GUIContent("给目标添加的BuffInfoId,结束移除", "给目标添加的BuffInfoId,结束移除"),
                                    skillData.buffInfoId);
                                skillData.buffInfoIdToSelf = EditorGUILayout.IntField(
                                    new GUIContent("判定到目标自己添加BuffInfoId", "判定到目标自己添加BuffInfoId"),
                                    skillData.buffInfoIdToSelf);
                                skillData.buffInfoIDToOther = EditorGUILayout.IntField(
                                    new GUIContent("判定到目标,目标添加BuffInfoId", "判定到目标,目标添加BuffInfoId"),
                                    skillData.buffInfoIDToOther);
                            }
                            EditorGUILayout.EndVertical();
                        }

                        skillData.useSpellBar = EditorGUILayout.Toggle("是否使用读条", skillData.useSpellBar, toggleStyle);
                        if (skillData.useSpellBar)
                        {
                            skillData.spellBarTime = EditorGUILayout.FloatField("读条时间(s)", skillData.spellBarTime);
                        }


                        chargeToggle = EditorGUILayout.Foldout(chargeToggle, "技能蓄力配置", EditorStyles.foldout);
                        if (chargeToggle)
                        {
                            EditorGUILayout.BeginVertical(rootGroupStyle);
                            {
                                skillData.isCharge = EditorGUILayout.Toggle("此技能是否蓄力", skillData.isCharge);
                                skillData.chargeConfig.repeatPhase =
                                    EditorGUILayout.IntField("蓄力循环的阶段:", skillData.chargeConfig.repeatPhase);
                                skillData.chargeConfig.changePhase =
                                    EditorGUILayout.IntField("蓄力会变化的阶段:", skillData.chargeConfig.changePhase);
                                skillData.chargeConfig.switchPhaseID = EditorGUILayout.IntField("蓄力成功切换的技能ID:",
                                    skillData.chargeConfig.switchPhaseID);
                                skillData.chargeConfig.chargeDuration =
                                    EditorGUILayout.FloatField("蓄力判定时间(s):", skillData.chargeConfig.chargeDuration);
                                skillData.chargeConfig.chargeMinDuration = EditorGUILayout.FloatField("不蓄力的时候蓄力阶段的最少时间",
                                    skillData.chargeConfig.chargeMinDuration);
                                skillData.chargeConfig.effect =
                                    EditorGUILayout.TextField("蓄力完成的特效播放名字:", skillData.chargeConfig.effect);
                                skillData.chargeConfig.effectPos =
                                    EditorGUILayout.Vector3Field("蓄力完成的特效偏移位置:", skillData.chargeConfig.effectPos);
                                skillData.chargeConfig.locator =
                                    EditorGUILayout.TextField("蓄力完成的特效播放挂点:", skillData.chargeConfig.locator);
                                skillData.chargeConfig.buffInfo = EditorGUILayout.IntField("蓄力完成添加的BuffInfo：",
                                    skillData.chargeConfig.buffInfo);
                                skillData.chargeConfig.playBuffAni =
                                    EditorGUILayout.Toggle("蓄力完成播放霸体动画", skillData.chargeConfig.playBuffAni);
                            }
                            EditorGUILayout.EndVertical();
                        }

                        specialOperationToggle =
                            EditorGUILayout.Foldout(specialOperationToggle, "技能操作配置", EditorStyles.foldout);
                        if (specialOperationToggle)
                        {
                            EditorGUILayout.BeginVertical(rootGroupStyle);
                            {
                                skillData.isSpeicalOperate =
                                    EditorGUILayout.Toggle("此技能是否特殊操作释放", skillData.isSpeicalOperate);
                                skillData.operationConfig.changePhase =
                                    EditorGUILayout.IntField("特殊操作改变的阶段:", skillData.operationConfig.changePhase);
                                if (skillData.operationConfig.changeSkillIDs == null ||
                                    (skillData.operationConfig.changeSkillIDs != null &&
                                     skillData.operationConfig.changeSkillIDs.Length < 5))
                                {
                                    skillData.operationConfig.changeSkillIDs = new int[5];
                                }

                                for (int i = 0; i < 3; ++i)
                                {
                                    string name = "备用";
                                    if (i == 0)
                                        name = "前摇杆";
                                    else if (i == 1)
                                        name = "上摇杆";
                                    else if (i == 2)
                                        name = "下摇杆";
                                    skillData.operationConfig.changeSkillIDs[i] =
                                        EditorGUILayout.IntField(name + " 对应的技能Id",
                                            skillData.operationConfig.changeSkillIDs[i]);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }

                        skillData.isUseSelectSeatJoystick = EditorGUILayout.Toggle(
                            new GUIContent("使用选择玩家技能摇杆", "使用选择玩家技能摇杆"), skillData.isUseSelectSeatJoystick);

                        skillJoystickToggle =
                            EditorGUILayout.Foldout(skillJoystickToggle, "技能摇杆配置", EditorStyles.foldout);
                        if (skillJoystickToggle)
                        {
                            EditorGUILayout.BeginVertical(rootGroupStyle);
                            {
                                skillData.skillJoystickConfig.mode =
                                    GetChangeEnumType("技能摇杆类型:", skillData.skillJoystickConfig.mode);
                                skillData.skillJoystickConfig.notDisplayLineEffect =
                                    EditorGUILayout.Toggle(new GUIContent("不显示瞄准线特效:", "不显示瞄准线特效:"),
                                        skillData.skillJoystickConfig.notDisplayLineEffect);
                                //skillData.skillJoystickConfig.mode = (SkillJoystickMode)EditorGUILayout.EnumPopup("摇杆模式", skillData.skillJoystickConfig.mode);
                                if (skillData.skillJoystickConfig.mode == SkillJoystickMode.FREE)
                                {
                                    skillData.skillJoystickConfig.effectName = EditorGUILayout.TextField("瞄准特效路径:",
                                        skillData.skillJoystickConfig.effectName);
                                    skillData.skillJoystickConfig.effectMoveSpeed =
                                        EditorGUILayout.Vector3Field("瞄准特效移动速度",
                                            skillData.skillJoystickConfig.effectMoveSpeed);
                                    //skillData.skillJoystickConfig.effectMoveRange = EditorGUILayout.Vector3Field("瞄准特效移动范围", skillData.skillJoystickConfig.effectMoveRange);
                                    skillData.skillJoystickConfig.effectMoveRadius =
                                        EditorGUILayout.IntField("瞄准特效移动范围半径",
                                            skillData.skillJoystickConfig.effectMoveRadius);
                                    skillData.skillJoystickConfig.dontRemoveJoystick = EditorGUILayout.Toggle(new GUIContent("按钮抬起时是否不移除摇杆:", "按钮抬起时是否不移除摇杆:"),
                                        skillData.skillJoystickConfig.dontRemoveJoystick);
                                }
                                if (skillData.skillJoystickConfig.mode == SkillJoystickMode.ACTIONSELECT)
                                {
                                    skillData.actionSelectPhase = EditorGUILayout.IntField("替换阶段", skillData.actionSelectPhase);
                                    int actionNum = EditorGUILayout.IntField("技能阶段数量", skillData.actionSelect.Length);
                                    actionNum = Mathf.Clamp(actionNum, 0, 4);
                                    if (actionNum != skillData.actionSelect.Length)
                                    {
                                        var temp1 = skillData.actionSelect;
                                        skillData.actionSelect = new int[actionNum];
                                        var temp2 = skillData.actionIconPath;
                                        skillData.actionIconPath = new string[actionNum];
                                        for (int i = 0; i < Mathf.Min(temp1.Length, actionNum); ++i)
                                        {
                                            skillData.actionSelect[i] = temp1[i];
                                            skillData.actionIconPath[i] = temp2[i];
                                        }
                                    }
                                    for (int i = 0; i < actionNum; i++)
                                    {
                                        skillData.actionSelect[i] = EditorGUILayout.IntField($"技能阶段 {i}", skillData.actionSelect[i]);
                                        skillData.actionIconPath[i] = EditorGUILayout.TextField($"图标路径 {i}", skillData.actionIconPath[i]);
                                    }
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }

                        skillData.cameraRestore = EditorGUILayout.Toggle("是否启用摄像机恢复", skillData.cameraRestore);
                        if (skillData.cameraRestore)
                        {
                            skillData.cameraRestoreTime =
                                EditorGUILayout.FloatField("恢复时间(s)", skillData.cameraRestoreTime);
                        }

                        int skillEventNum = EditorGUILayout.IntField("技能事件数量:", skillData.skillEvents.Length);

                        if (skillEventNum != skillData.skillEvents.Length)
                        {
                            var old = skillData.skillEvents;
                            skillData.skillEvents = new SkillEvent[skillEventNum];
                            for (int i = 0; i < old.Length && i < skillData.skillEvents.Length; ++i)
                            {
                                skillData.skillEvents[i] = old[i];
                            }
                        }

                        if (skillEventNum > 0)
                        {
                            skillEventsToggle =
                                EditorGUILayout.Foldout(skillEventsToggle, "技能事件:", EditorStyles.foldout);
                            if (skillEventsToggle)
                            {
                                for (int i = 0; i < skillData.skillEvents.Length; ++i)
                                {
                                    EditorGUILayout.BeginVertical(rootGroupStyle);
                                    {
                                        var se = skillData.skillEvents[i];
                                        if (se == null)
                                        {
                                            se = new SkillEvent();
                                            skillData.skillEvents[i] = se;
                                        }

                                        se.eventType = (EventCommand) EditorGUILayout.EnumPopup("技能事件:", se.eventType);
                                        se.eventAction =
                                            (SkillAction) EditorGUILayout.EnumPopup("技能行为:", se.eventAction);
                                        se.paramter = EditorGUILayout.TextField("参数:", se.paramter);
                                        se.workPhase = EditorGUILayout.IntField("生效阶段:", se.workPhase);
                                        se.attachActionName =
                                            EditorGUILayout.TextField("挂件动画对应的技能配置文件名称:", se.attachActionName);
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }


                        EditorGUILayout.BeginHorizontal();
                        {
                            string unsaved = speedTemp != skillData.animationSpeed ? "*" : "";
                            speedTemp = EditorGUILayout.FloatField("AnimationSpeed:" + unsaved, speedTemp);
                            if (speedTemp != skillData.animationSpeed)
                            {
                                if (StyledButton("Apply"))
                                {
                                    skillData.animationSpeed = speedTemp;
                                    return;
                                }

                                if (StyledButton("Cancle"))
                                {
                                    speedTemp = skillData.animationSpeed;
                                    return;
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            string unsaved = fpsTemp != skillData.fps ? "*" : "";
                            GUI.SetNextControlName("FPS Architecture");
                            fpsTemp = EditorGUILayout.IntSlider("FPS Architecture:" + unsaved, fpsTemp, 10, 120);
                            if (StyledButton("Apply")) skillData.fps = fpsTemp;
                            if (StyledButton("导入特效"))
                            {
                                Clear(true, true);
                                EffectCopyWizardDisplayWizard.CreateWindow(skillData);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            #endregion

            #region skill table

            //EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                //Begin xlsx data unit Options

                //if (skillData.skillID <= 0)
                //{
                //    fLastSkillTipTime += Time.deltaTime;
                //    if (fLastSkillTipTime > 5.0f)
                //    {
                //        fLastSkillTipTime = 0.0f;
                //        Logger.LogErrorFormat("请检查 {0} SkillID 格式是否有效 : {1} ", skillData.name, skillData.skillID);
                //    }
                //}
                if (xlsxDataManager != null)
                {
                    xlsxDataManager.OnGUIUpdate("SkillTable", "数据表", skillData.skillID);
                }
            }
            //EditorGUILayout.EndVertical();

            #endregion

            #region animation clip

            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                // Begin Animation Options
                EditorGUILayout.BeginHorizontal();
                {
                    animationOptions = EditorGUILayout.Foldout(animationOptions, "动作", foldStyle);
                }
                EditorGUILayout.EndHorizontal();

                if (animationOptions)
                {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUI.indentLevel += 1;

                        if (StyledButton("一键替换模型"))
                        {
                            OneKeyReplaceModel();
                        }

                        // if (skillData.skillPhases.Length > 0)
                        // {
                        //     mIsUsePlayAllStep =
                        //         EditorGUILayout.Toggle("是否开启一键播放所有的技能阶段", mIsUsePlayAllStep, toggleStyle);
                        // }

                       
                        GameObject newCharacterPrefab = (GameObject) EditorGUILayout.ObjectField("角色模型:",
                            GetCurSkillData()?.characterPrefab, typeof(UnityEngine.GameObject), true);
                        if (newCharacterPrefab != null && GetCurSkillData()?.characterPrefab != newCharacterPrefab &&
                            !IsEditorPlaying())
                        {
                            if (PrefabUtility.GetPrefabType(newCharacterPrefab) != PrefabType.Prefab)
                            {
                                characterWarning = true;
                                errorMsg = "not prefab";
                            }
                            else
                            {
                                characterWarning = false;
                                GetCurSkillData().characterPrefab = newCharacterPrefab;
                                GetCurSkillData().characterAsset = new DAssetObject(newCharacterPrefab);
                                Clear(true, true);
                                RefreashSkillData(GetCurSkillData());
                            }
                        }
                        else if (GetCurSkillData().characterPrefab != newCharacterPrefab && IsEditorPlaying())
                        {
                            characterWarning = true;
                            errorMsg = "in play mode";
                        }
                        else if (newCharacterPrefab == null)
                        {
                            GetCurSkillData().characterPrefab = null;
                            animClipsName = new string[0];
                        }

                        EditorGUILayout.Space();

                        mAnimationFilterString =
                            EditorGUILayout.TextField(new GUIContent("过滤名称:", "忽略大小写"), mAnimationFilterString);
                        List<string> tempListName = new List<string>();
                        for (int i = 0; i < animClipsName.Length; ++i)
                        {
                            if (animClipsName[i].Contains(mAnimationFilterString, StringComparison.OrdinalIgnoreCase))
                            {
                                tempListName.Add(animClipsName[i]);
                            }
                        }

                        string[] filteredAnimClipsName = tempListName.ToArray();
                        mAnimationFilterInt = tempListName.IndexOf(GetCurSkillData().animationName);
                        int newClip = EditorGUILayout.Popup("动作:", mAnimationFilterInt, filteredAnimClipsName);
                        if (newClip != -1 &&
                            (newClip != mAnimationFilterInt || undoAnimName != skillData.animationName))
                        {
                            undoAnimName = skillData.animationName;
                            SkillDataUndo("anim clip");
                            mAnimationFilterInt = newClip;
                            skillData.animationName = filteredAnimClipsName[newClip];
                            List<string> animClipsNameList = new List<string>(animClipsName);
                            animClipsSelected = animClipsNameList.IndexOf(skillData.animationName);
                            animClip = null;

                            GeAnimDesc cur = animClips[animClipsSelected];
                            if (cur != null)
                            {
                                animClip = cur.animClipData;
                            }

                            /*
                             AnimationState state = animations[skillData.animationName];
                             if(null == state)
                             {
                                 GeAnimDesc cur = animClips[animClipsSelected];
                                 animations.AddClip(cur.animClipData, cur.animClipName);
                                 state = animations[skillData.animationName];
                             }
 
                             if (state)
                             {
                                 animClip = state.clip;
                             }*/
                        }


                        EditorGUILayout.LabelField("总帧数:", GetCurSkillData().totalFrames.ToString());

                        /*用不太到，暂时不显示了
                        if (animClip != null)
                        {
                            skillData.wrapMode =
                                (WrapMode) EditorGUILayout.EnumPopup("Wrap Mode: ", GetCurSkillData().wrapMode,
                                    enumStyle);
                            interpolationSpeed = EditorGUILayout.Slider(
                                new GUIContent("Iterpolation Speed:", "动画播放速度，攻速参照，不影响逻辑"), interpolationSpeed, 0, 2);
                        }
                        */

                        if (character == null)
                        {
                            if (StyledButton("预览"))
                            {
                                if (skillData.characterPrefab == null)
                                {
                                    characterWarning = true;
                                    errorMsg = "Drag a character into 'Character Prefab' first.";
                                }
                                else if (IsEditorPlaying())
                                {
                                    characterWarning = true;
                                    errorMsg = "You can't preview animations while in play mode.";
                                }
                                else
                                {
                                    characterWarning = false;
                                    Preview();
                                }
                            }
                        }
                        else
                        {
                            EditorGUI.indentLevel++;
                            string tip = string.Empty;
                            if (bSkillTotalPreview)
                            {
                                tip = string.Format("动画帧  当前阶段:{0}/{1}", mCurSkillStep + 1,
                                    skillData.skillPhases.Length);
                            }
                            else
                            {
                                tip = "动画帧";
                            }

                            if (smoothPreview)
                            {
                                animFrame = GUIControls.UICommon.AnimFrameSlider(tip, animFrame,
                                    EditorGUI.indentLevel, 0, GetCurSkillData().totalFrames - 1);
                            }
                            else
                            {
                                animFrame = GUIControls.UICommon.AnimFrameSlider(tip,
                                    (int) animFrame, EditorGUI.indentLevel, 0, GetCurSkillData().totalFrames - 1);
                            }

                            EditorGUI.indentLevel--;

                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayoutUtility.GetRect(1, 1);
                                GUILayout.Label(string.Format("{0:0.000} 秒", animFrame * (1.0f / fpsTemp)),
                                    GUILayout.Width(60.0f));
                            }
                            EditorGUILayout.EndHorizontal();

                            if (cameraOptions)
                            {
                                GUILayout.BeginHorizontal("GroupBox");
                                GUILayout.Label("You must close 'Camera Preview' first.", "CN EntryError");
                                GUILayout.EndHorizontal();
                            }

                            bool bOld = EditorGUILayout.Toggle("Z轴模型(新)", bNew, toggleStyle);

                            if (bNew != bOld)
                            {
                                bNew = bOld;

                                if (bNew == false)
                                    character.transform.localRotation
                                        = Quaternion.Euler(0, 90, 0);
                                else
                                    character.transform.localRotation
                                        = Quaternion.Euler(initRotation);

                                return;
                            }

                            bool bMirror = DSkillData.actor.transform.localScale.x < 0;
                            bool bChange = EditorGUILayout.Toggle("镜像", bMirror, toggleStyle);

                            if (bChange != bMirror)
                            {
                                if (bChange)
                                {
                                    Vector3 scale = DSkillData.actor.transform.localScale;
                                    scale.x = -1.0f;
                                    DSkillData.actor.transform.localScale = scale;
                                }
                                else
                                {
                                    Vector3 scale = DSkillData.actor.transform.localScale;
                                    scale.x = 1.0f;
                                    DSkillData.actor.transform.localScale = scale;
                                }

                                AnimationSampler(GetCurSkillData());
                                ApplyEffectMirror(bChange);
                                return;
                            }
                            
                            /*
                            smoothPreview = EditorGUILayout.Toggle("Smooth Preview", smoothPreview, toggleStyle);
                            DSkillData.vfliter =
                                (VisiableFliter) EditorGUILayout.EnumMaskField("Visable Fliter:", DSkillData.vfliter);
                            */
                            AnimationSampler(GetCurSkillData());

                            EditorGUILayout.Space();
                            
                            EditorGUILayout.BeginHorizontal();
                            {
                                if (StyledButton("编辑器视角"))
                                {
                                    DEditorCamera.SetPosition(DEditorCamera.SkillEditorDefaultCameraPos);
                                    DEditorCamera.SetRotation(Quaternion.identity);
                                    DEditorCamera.SetOrthographic(true);
                                    DEditorCamera.SetSize(DSkillDataEditorDrawer.Camera_ViewSize);
                                    bEditorView = true;
                                }

                                if (StyledButton("游戏视角"))
                                {
                                    DEditorCamera.SetPosition(DEditorCamera.SkillEditorDefaultCameraPos);
                                    DEditorCamera.SetRotation(Quaternion.Euler(GameViewAngle, 0, 0));
                                    DEditorCamera.SetOrthographic(true);
                                    DEditorCamera.SetSize(DSkillDataEditorDrawer.Camera_ViewSize);
                                    bEditorView = false;
                                }


                                if (_Play)
                                {
                                    if (StyledButton("动画暂停"))
                                    {
                                        Play(false);
                                    }
                                }
                                else
                                {
                                    if (StyledButton("动画播放"))
                                    {
                                        Play(true);
                                    }
                                }


                                


                                if (StyledButton("关闭预览")) Clear(true, true);

                                if (StyledButton("保存文件") || IsSaveFileDirty)
                                {
                                    IsSaveFileDirty = false;
                                    IsAutoFreshDirty = true;

                                    if (totalFramesTemp >= 500)
                                    {
                                        EditorUtility.DisplayDialog("警告", "总帧数大于500帧", "知道了");
                                    }
                                    else
                                    {
                                        SaveCurrentFrameData<HurtDecisionBox>(ref skillData.HurtBlocks,
                                            (HurtDecisionBox newBox, HurtDecisionBox oldBox) =>
                                            {
                                                ShapeBox[] UpperBox = oldBox.boxs;
                                                newBox.boxs = CopyShapeBox(UpperBox);
                                                newBox.zDim = oldBox.zDim;
                                                newBox.hurtID = oldBox.hurtID;
                                            });
                                        SaveCurrentFrameData<DefenceDecisionBox>(ref skillData.DefenceBlocks,
                                            (DefenceDecisionBox newBox, DefenceDecisionBox oldBox) =>
                                            {
                                                ShapeBox[] UpperBox = oldBox.boxs;
                                                newBox.boxs = CopyShapeBox(UpperBox);
                                                newBox.type = oldBox.type;
                                            });
                                        AssetDatabase.SaveAssets();
                                        UnityEngine.Debug.LogWarning("Skill " + skillData.name + " Saved!");
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();
                        }

                        if (characterWarning)
                        {
                            GUILayout.BeginHorizontal("GroupBox");
                            GUILayout.Label(errorMsg, "CN EntryWarn");
                            GUILayout.EndHorizontal();
                        }

                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.EndVertical();
                }
                else if (character != null && !cameraOptions)
                {
                    //Clear(true, true);
                }
            }
            EditorGUILayout.EndVertical();

            #endregion

            DSkillDataEditorDrawer proxyScript = null;
            if (character)
            {
                proxyScript = character.GetComponent<DSkillDataEditorDrawer>();

                if (proxyScript)
                {
                    proxyScript.skillData = skillData;
                }
            }


            #region Hurt Box

            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                HurtDecisionBox currentHurtBox = ArrayGetCurrentFrameData<HurtDecisionBox>(ref skillData.HurtBlocks,
                    (HurtDecisionBox newBox, HurtDecisionBox oldBox) =>
                    {
                        ShapeBox[] UpperBox = oldBox.boxs;
                        newBox.boxs = CopyShapeBox(UpperBox);
                        newBox.zDim = oldBox.zDim;
                        newBox.hurtID = oldBox.hurtID;
                    }
                );

                hitsToggle = EditorGUILayout.Foldout(hitsToggle, "攻击框 (" + currentHurtBox.boxs.Length + "/全部"+ GetTotalValidDataCount(skillData.HurtBlocks) + ")",EditorStyles.foldout);
                if (hitsToggle && proxyScript)
                {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUILayout.Space();

                        EditorGUI.indentLevel += 1;

                        EditorGUILayout.BeginHorizontal();
                        if (StyledButton("添加"))
                        {
                            currentHurtBox.boxs = AddElement<ShapeBox>(currentHurtBox.boxs, new ShapeBox());
                        }

                        if (StyledButton("清除"))
                        {
                            SkillDataUndo("HurtBox Remove");
                            currentHurtBox.boxs = new ShapeBox[0];
                        }

                        FramePickData Hurtdata = new FramePickData();
                        Hurtdata.minValue = (int) animFrame;
                        Hurtdata.maxValue = (int) animFrame;
                        Hurtdata.curValue = (int) animFrame;
                        Hurtdata.totalFrames = (int) skillData.totalFrames;

                        Hurtdata.dCopy = (FramePickData data) =>
                        {
                            SkillDataUndo("HurtBox dCopy");
                            for (int i = data.minValue; i <= data.maxValue; ++i)
                            {
                                ShapeBox[] UpperBox = skillData.HurtBlocks[data.curValue].boxs;
                                skillData.HurtBlocks[i].boxs = CopyShapeBox(UpperBox);
                                skillData.HurtBlocks[i].zDim = skillData.HurtBlocks[data.curValue].zDim;
                                skillData.HurtBlocks[i].hurtID = skillData.HurtBlocks[data.curValue].hurtID;
                            }

                            EditorUtility.SetDirty(skillData);
                        };

                        Hurtdata.dClear = (FramePickData data) =>
                        {
                            SkillDataUndo("HurtBox dClear");
                            for (int i = data.minValue; i <= data.maxValue; ++i)
                            {
                                skillData.HurtBlocks[i] = new HurtDecisionBox();
                            }

                            EditorUtility.SetDirty(skillData);
                        };

                        StyledButtonEx("修改", () => { PickFramePopupWindow.ShowPopup(HurtEditPopupRect, Hurtdata); },
                            ref HurtEditPopupRect);

                        if (StyledButton("CopyU"))
                        {
                            int iFrames = (int) animFrame;
                            if (iFrames > 0)
                            {
                                ShapeBox[] UpperBox = skillData.HurtBlocks[iFrames - 1].boxs;
                                currentHurtBox.boxs = CopyShapeBox(UpperBox);
                                currentHurtBox.zDim = skillData.HurtBlocks[iFrames - 1].zDim;
                                currentHurtBox.hurtID = skillData.HurtBlocks[iFrames - 1].hurtID;
                            }
                        }

                        bool bPreToggle = (proxyScript.skilleditmode == SkillSenceEditMode.HurtBoxEdit);
                        bool bToggle = StyledToggle(proxyScript.skilleditmode == SkillSenceEditMode.HurtBoxEdit,
                            "编辑攻击框");
                        if (bToggle)
                        {
                            proxyScript.skilleditmode = SkillSenceEditMode.HurtBoxEdit;
                            SetEditorView();

                            if (bPreToggle != bToggle)
                            {
                                proxyScript.currentEditObject = null;
                            }

                            OnEditChangeOrSelectChange();
                        }

                        if (bPreToggle && !bToggle)
                        {
                            proxyScript.skilleditmode = SkillSenceEditMode.Normal;
                            proxyScript.currentEditObject = null;

                            OnEditChangeOrSelectChange();
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.BeginHorizontal();

                        if (StyledButton("跨文件复制"))
                        {
                            if (skillData.HurtBlocks != null)
                            {
                                mCopySkillData.HurtBlocks = skillData.HurtBlocks;
                                EditorUtility.SetDirty(mCopySkillData);
                            }
                        }

                        if (StyledButton("跨文件粘贴"))
                        {
                            if (mCopySkillData.HurtBlocks != null)
                            {
                                int length = Mathf.Min(mCopySkillData.HurtBlocks.Length, skillData.HurtBlocks.Length);
                                for (int i = 0; i < length; ++i)
                                {
                                    ShapeBox[] UpperBox = mCopySkillData.HurtBlocks[i].boxs;
                                    skillData.HurtBlocks[i].boxs = CopyShapeBox(UpperBox);
                                    skillData.HurtBlocks[i].zDim = mCopySkillData.HurtBlocks[i].zDim;
                                    skillData.HurtBlocks[i].hurtID = mCopySkillData.HurtBlocks[i].hurtID;
                                }
                            }

                            EditorUtility.SetDirty(skillData);
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.BeginHorizontal();
                        if (currentHurtBox.boxs.Length > 0)
                        {
                            currentHurtBox.zDim = EditorGUILayout.FloatField("Z Dim(伤害判断Z深度):", currentHurtBox.zDim);

                            if (StyledButton("zdim修改应用到所有帧"))
                            {
                                for (int i = 0; i < skillData.HurtBlocks.Length; ++i)
                                {
                                    SkillDataUndo("HurtBox applyAll");
                                    HurtDecisionBox hb = skillData.HurtBlocks[i];
                                    hb.zDim = currentHurtBox.zDim;
                                    hb.hurtID = currentHurtBox.hurtID;
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        //currentHurtBox.hurtID = EditorGUILayout.IntField("伤害效果表ID:", currentHurtBox.hurtID);
                        currentHurtBox.hurtID = mAttackHurtEffectTable.OnGUIWithID(currentHurtBox.hurtID);


                        for (int i = 0; i < currentHurtBox.boxs.Length; i++)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical(arrayElementStyle);
                            {
                                EditorGUILayout.Space();

                                EditorGUILayout.BeginVertical(subArrayElementStyle);
                                {
                                    currentHurtBox.boxs[i].center =
                                        EditorGUILayout.Vector2Field("中心点:", currentHurtBox.boxs[i].center);
                                    currentHurtBox.boxs[i].size =
                                        EditorGUILayout.Vector2Field("长宽:", currentHurtBox.boxs[i].size);

                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();

                                    if (StyledButton("选中框"))
                                    {
                                        if (proxyScript)
                                        {
                                            proxyScript.skilleditmode = SkillSenceEditMode.HurtBoxEdit;
                                            proxyScript.currentEditObject = currentHurtBox.boxs[i];
                                            SetEditorView();
                                            OnEditChangeOrSelectChange();
                                        }
                                    }

                                    if (StyledButton("移除框"))
                                    {
                                        SkillDataUndo("HurtBox removeShapeBox");
                                        currentHurtBox.boxs = RemoveElement<ShapeBox>(currentHurtBox.boxs,
                                            currentHurtBox.boxs[i]);
                                        return;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 150;
                                EditorGUILayout.Space();

                                EditorGUILayout.BeginVertical();
                                {
                                    //xlsxDataManager.OnGUIUpdate("EffectTable", "触发效果表", currentHurtBox.hurtID);
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            #endregion

            #region Def Box

            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                DefenceDecisionBox currentDefenceBox = ArrayGetCurrentFrameData<DefenceDecisionBox>(
                    ref skillData.DefenceBlocks
                    ,
                    (DefenceDecisionBox newBox, DefenceDecisionBox oldBox) =>
                    {
                        ShapeBox[] UpperBox = oldBox.boxs;
                        newBox.boxs = CopyShapeBox(UpperBox);
                        newBox.type = oldBox.type;
                    }
                );

                defsToggle = EditorGUILayout.Foldout(defsToggle, "被击框 (" + currentDefenceBox.boxs.Length + "/全部"+GetTotalValidDataCount(skillData.DefenceBlocks) + ")", EditorStyles.foldout);
                if (defsToggle && proxyScript)
                {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUILayout.Space();

                        EditorGUI.indentLevel += 1;

                        EditorGUILayout.BeginHorizontal();
                        if (StyledButton("添加"))
                        {
                            currentDefenceBox.boxs = AddElement<ShapeBox>(currentDefenceBox.boxs, new ShapeBox());
                        }

                        if (StyledButton("清除"))
                        {
                            SkillDataUndo("DefBox Remove");
                            currentDefenceBox.boxs = new ShapeBox[0];
                        }

                        FramePickData DefData = new FramePickData();
                        DefData.minValue = (int) animFrame;
                        DefData.maxValue = (int) animFrame;
                        DefData.curValue = (int) animFrame;
                        DefData.totalFrames = (int) skillData.totalFrames;

                        DefData.dCopy = (FramePickData data) =>
                        {
                            SkillDataUndo("DefBox dCopy");
                            for (int i = data.minValue; i <= data.maxValue; ++i)
                            {
                                ShapeBox[] UpperBox = skillData.DefenceBlocks[data.curValue].boxs;
                                skillData.DefenceBlocks[i].boxs = CopyShapeBox(UpperBox);
                                skillData.DefenceBlocks[i].type = skillData.DefenceBlocks[data.curValue].type;
                            }

                            EditorUtility.SetDirty(skillData);
                        };

                        DefData.dClear = (FramePickData data) =>
                        {
                            SkillDataUndo("DefBox dClear");
                            for (int i = data.minValue; i <= data.maxValue; ++i)
                            {
                                skillData.DefenceBlocks[i] = new DefenceDecisionBox();
                            }

                            EditorUtility.SetDirty(skillData);
                        };

                        StyledButtonEx("修改", () => { PickFramePopupWindow.ShowPopup(DefEditPopupRect, DefData); },
                            ref DefEditPopupRect);

                        if (StyledButton("CopyU"))
                        {
                            int iFrames = (int) animFrame;
                            if (iFrames > 0)
                            {
                                ShapeBox[] UpperBox = skillData.DefenceBlocks[iFrames - 1].boxs;
                                currentDefenceBox.boxs = CopyShapeBox(UpperBox);
                            }
                        }

                        bool bPreToggle = (proxyScript.skilleditmode == SkillSenceEditMode.DefBoxEdit);
                        bool bToggle = StyledToggle(proxyScript.skilleditmode == SkillSenceEditMode.DefBoxEdit, "编辑被击框");
                        if (bToggle)
                        {
                            proxyScript.skilleditmode = SkillSenceEditMode.DefBoxEdit;
                            SetEditorView();
                            if (bPreToggle != bToggle)
                            {
                                proxyScript.currentEditObject = null;
                            }

                            OnEditChangeOrSelectChange();
                        }

                        if (bPreToggle && !bToggle)
                        {
                            proxyScript.skilleditmode = SkillSenceEditMode.Normal;
                            proxyScript.currentEditObject = null;
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.BeginHorizontal();

                        if (StyledButton("跨文件复制"))
                        {
                            if (skillData.DefenceBlocks != null)
                            {
                                mCopySkillData.DefenceBlocks = skillData.DefenceBlocks;
                                EditorUtility.SetDirty(mCopySkillData);
                            }
                        }

                        if (StyledButton("跨文件粘贴"))
                        {
                            if (mCopySkillData.DefenceBlocks != null)
                            {
                                int length = Mathf.Min(mCopySkillData.DefenceBlocks.Length,
                                    skillData.DefenceBlocks.Length);
                                for (int i = 0; i < length; ++i)
                                {
                                    ShapeBox[] UpperBox = mCopySkillData.DefenceBlocks[i].boxs;
                                    skillData.DefenceBlocks[i].boxs = CopyShapeBox(UpperBox);
                                    skillData.DefenceBlocks[i].type = mCopySkillData.DefenceBlocks[i].type;
                                }
                            }

                            EditorUtility.SetDirty(skillData);
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.BeginHorizontal();
                        if (currentDefenceBox.boxs.Length > 0)
                        {
                            currentDefenceBox.type = EditorGUILayout.IntField("Type:", currentDefenceBox.type);
                        }

                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < currentDefenceBox.boxs.Length; i++)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical(arrayElementStyle);
                            {
                                EditorGUILayout.Space();

                                EditorGUILayout.BeginVertical(subArrayElementStyle);
                                {
                                    currentDefenceBox.boxs[i].center =
                                        EditorGUILayout.Vector2Field("Center:", currentDefenceBox.boxs[i].center);
                                    currentDefenceBox.boxs[i].size =
                                        EditorGUILayout.Vector2Field("Size:", currentDefenceBox.boxs[i].size);
                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();

                                    if (StyledButton("选择被击框"))
                                    {
                                        if (proxyScript)
                                        {
                                            proxyScript.skilleditmode = SkillSenceEditMode.DefBoxEdit;
                                            proxyScript.currentEditObject = currentDefenceBox.boxs[i];
                                            SetEditorView();
                                            OnEditChangeOrSelectChange();
                                        }
                                    }

                                    if (StyledButton("移除被击框"))
                                    {
                                        SkillDataUndo("DefBox removeDefBox");
                                        currentDefenceBox.boxs = RemoveElement<ShapeBox>(currentDefenceBox.boxs,
                                            currentDefenceBox.boxs[i]);
                                        return;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 150;
                                EditorGUILayout.Space();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            #endregion


            OnGUIAttachFrames(proxyScript);

            //PlayEffect

            #region PlayEffect

            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                //EffectsFrames currentEffectFrame = ArrayGetCurrentFrameData<EffectsFrames>(ref skillData.effectFrames);

                effectsToggle = EditorGUILayout.Foldout(effectsToggle, "播放特效 (" + effectnums + "/全部" + skillData.effectFrames.Length + ")", EditorStyles.foldout);
                if (effectsToggle && proxyScript)
                {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUILayout.Space();

                        EditorGUI.indentLevel += 1;

                        EditorGUILayout.BeginHorizontal();
                        if (StyledButton("添加特效"))
                        {
                            SkillDataUndo("Add Effect");

                            EffectsFrames frame = new EffectsFrames();
                            frame.startFrames = (int) animFrame;
                            frame.name = "[" + skillData.effectFrames.Length + "]" + "Effect";
                            skillData.effectFrames = AddElement<EffectsFrames>(skillData.effectFrames, frame);
                        }

                        if (StyledButton("清除特效"))
                        {
                            for (int i = 0; i < skillData.effectFrames.Length; ++i)
                            {
                                EffectsFrames effectFrames = skillData.effectFrames[i];
                                effectFrames.effects.Show(false);
                            }

                            SkillDataUndo("Clear Effect");
                            skillData.effectFrames = new EffectsFrames[0];
                        }

                        bool bShow = EditorGUILayout.Toggle("显示全部特效", bShowAllEffects);

                        if (bShow != bShowAllEffects)
                        {
                            bShowAllEffects = bShow;
                            return;
                        }


                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.BeginHorizontal();

                        if (StyledButton("跨文件保存特效"))
                        {
                            if (skillData.effectFrames != null && skillData.effectFrames.Length != 0)
                            {
                                mCopySkillData.effectFrames = new EffectsFrames[skillData.effectFrames.Length];
                                for (int i = 0; i < skillData.effectFrames.Length; ++i)
                                {
                                    mCopySkillData.effectFrames[i] = new EffectsFrames();
                                    mCopySkillData.effectFrames[i].Copy(skillData.effectFrames[i]);
                                }

                                EditorUtility.SetDirty(mCopySkillData);
                            }
                        }

                        if (StyledButton("跨文件粘贴特效"))
                        {
                            if (mCopySkillData.effectFrames != null && mCopySkillData.effectFrames.Length != 0)
                            {
                                skillData.effectFrames = new EffectsFrames[mCopySkillData.effectFrames.Length];
                                for (int i = 0; i < mCopySkillData.effectFrames.Length; ++i)
                                {
                                    skillData.effectFrames[i] = new EffectsFrames();
                                    skillData.effectFrames[i].Copy(mCopySkillData.effectFrames[i]);
                                }

                                EditorUtility.SetDirty(skillData);
                            }
                        }

                        if (StyledButton("粘贴特效"))
                        {
                            if (mCopyData is EffectsFrames)
                            {
                                SkillDataUndo("Paste Effect");
                                EffectsFrames mFakeData = new EffectsFrames();
                                mFakeData.Copy(mCopyData as EffectsFrames);
                                skillData.effectFrames = AddElement<EffectsFrames>(skillData.effectFrames, mFakeData);
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < GetCurSkillData().effectFrames.Length; ++i)
                        {
                            EffectsFrames effectFrames = GetCurSkillData().effectFrames[i];

                            if (effectFrames == null)
                            {
                                continue;
                            }

                            effectFrames.Check();

                            float fBeginTime = GetFramesTime(effectFrames.startFrames);

                            float fEndTime = fBeginTime + effectFrames.time;

                            bool bEffectShow = (animTime >= fBeginTime && animTime <= fEndTime);

                            if (bEffectShow || bShowAllEffects)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    bool bOldFoldeDict = GetFoldeDict("effectFrames", i);
                                    bool bNewFoldeDict = EditorGUILayout.Foldout(bOldFoldeDict, effectFrames.name);
                                    if (bNewFoldeDict != bOldFoldeDict)
                                    {
                                        SetFoldeDict("effectFrames", i, bNewFoldeDict);
                                        EditorGUIUtility.PingObject(effectFrames.effects.rootObject);
                                    }

                                    if (bEffectShow)
                                    {
                                        bool bOldToggleDict = GetFoldeDict("effectFrames_toggle", i, true);
                                        bool bNewToggleDict = EditorGUILayout.Toggle(bOldToggleDict);
                                        if (bNewToggleDict != bOldToggleDict)
                                        {
                                            SetFoldeDict("effectFrames_toggle", i, bNewToggleDict);
                                            effectFrames.effects.Show(bNewToggleDict);
                                            EditorGUIUtility.PingObject(effectFrames.effects.rootObject);
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                if (!GetFoldeDict("effectFrames", i))
                                {
                                    continue;
                                }

                                EditorGUILayout.Space();
                                EditorGUILayout.BeginVertical(arrayElementStyle);
                                {
                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginVertical(subArrayElementStyle);
                                    {
                                        //EditorGUILayout.LabelField(effectFrames.name, TablabelStyle);
                                        TabLabel(effectFrames, effectFrames.name, proxyScript);

                                        float current = animFrame;
                                        float endframes = (effectFrames.time * GetCurSkillData().fps);
                                        endframes = effectFrames.startFrames + endframes;
                                        endframes = Mathf.Min(endframes, GetCurSkillData().totalFrames - 1);
                                        float oldcurrent = current;
                                        StyledTimePeriodShow(effectFrames.name, ref current, effectFrames.startFrames,
                                            endframes, 0, GetCurSkillData().totalFrames - 1, EditorGUI.indentLevel);

                                        if (current != oldcurrent && bEffectShow)
                                        {
                                            float iDelta = current - oldcurrent;
                                            float iNewstartFrames = effectFrames.startFrames + iDelta;

                                            if (iNewstartFrames >= 0 && iNewstartFrames < skillData.totalFrames - 1)
                                            {
                                                effectFrames.startFrames = (int) iNewstartFrames;
                                                animFrame = current;
                                                AnimationSampler(GetCurSkillData());
                                            }

                                            return;
                                        }

                                        string name = EditorGUILayout.TextField("名字:", effectFrames.name);
                                        if (name != effectFrames.name)
                                        {
                                            effectFrames.name = name;
                                            return;
                                        }

                                        //effectFrames.effectResID = EditorGUILayout.IntField("ResID", effectFrames.effectResID);

                                        EffectPlayType type = effectFrames.playtype;

                                        SetEffectPlayType(effectFrames);

                                        if (effectFrames.playtype != type)
                                        {
                                            if (effectFrames.playtype == EffectPlayType.EffectTime)
                                            {
                                                animFrame = effectFrames.startFrames;
                                                AnimationSampler(GetCurSkillData());
                                            }

                                            effectFrames.effects.type = effectFrames.playtype;
                                            effectFrames.time = effectFrames.effects.timeLength;

                                            return;
                                        }

                                        SetEffectTimeType(effectFrames);


                                        EditorGUILayout.BeginHorizontal();

                                        float time = EditorGUILayout.FloatField("持续时间", effectFrames.time);

                                        if (effectFrames.playtype == EffectPlayType.GivenTime)
                                        {
                                            if (GUILayout.Button("EndTime", "miniButton"))
                                            {
                                                time = Mathf.Max(0, animTime - GetFramesTime(effectFrames.startFrames));
                                            }
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        effectFrames.loop = EditorGUILayout.Toggle("是否循环", effectFrames.loop);
                                        effectFrames.loopLoop =
                                            EditorGUILayout.Toggle("如果技能循环此特效是否循环", effectFrames.loopLoop);
                                        effectFrames.onlyLocalSee = EditorGUILayout.Toggle(
                                            new GUIContent("特效只有本地玩家才能看到", "特效只有本地玩家才能看到"), effectFrames.onlyLocalSee);
                                        effectFrames.useActorSpeed = EditorGUILayout.Toggle(
                                            new GUIContent("播放速度是否受角色攻速影响", "播放速度是否受角色攻速影响"),
                                            effectFrames.useActorSpeed);

                                        if (effectFrames.playtype == EffectPlayType.GivenTime)
                                        {
                                            effectFrames.effects.timeLength = time;
                                            effectFrames.time = time;
                                        }

                                        if (bEffectShow)
                                        {
                                            string names = AttachmentPopup("挂点选择", effectFrames.attachname);
                                            if (names != effectFrames.attachname)
                                            {
                                                effectFrames.attachname = names;
                                                effectFrames.effects.UpdateParent(
                                                    GetAttachNodes(names)
                                                );
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            EditorGUILayout.LabelField("挂点选择", effectFrames.attachname);
                                        }


                                        //effectFrames.attachPoint = (EffectAttachPoint)EditorGUILayout.EnumPopup("AttachPoint", effectFrames.attachPoint, enumStyle);
                                        GameObject newEffectPrefab =
                                            (GameObject) EditorGUILayout.ObjectField("特效预制体:",
                                                effectFrames.effectGameObjectPrefeb, typeof(UnityEngine.GameObject),
                                                true);

                                        if (effectFrames.effectGameObjectPrefeb == null &&
                                            Utility.IsStringValid(effectFrames.effectAsset.m_AssetPath))
                                        {
                                            effectFrames.effectGameObjectPrefeb =
                                                AssetDatabase.LoadAssetAtPath<GameObject>(
                                                    "Assets/Resources/" + effectFrames.effectAsset.m_AssetPath +
                                                    ".prefab");
                                        }

                                        if (newEffectPrefab)
                                        {
                                            if (PrefabUtility.GetPrefabType(newEffectPrefab) == PrefabType.Prefab)
                                            {
                                                bool bNew = false;
                                                if (effectFrames.effectGameObjectPrefeb != newEffectPrefab)
                                                {
                                                    bNew = true;
                                                }

                                                effectFrames.effectGameObjectPrefeb = newEffectPrefab;
                                                effectFrames.effectAsset = new DAssetObject(newEffectPrefab);
                                                effectFrames.effects.gameObjectPrefeb = newEffectPrefab;


                                                if (bNew)
                                                {
                                                    animFrame = effectFrames.startFrames;
                                                    AnimationSampler(GetCurSkillData());
                                                    effectFrames.playtype = EffectPlayType.EffectTime;
                                                    effectFrames.effects.type = effectFrames.playtype;
                                                    effectFrames.time = effectFrames.effects.timeLength;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (effectFrames.effects != null)
                                                effectFrames.effects.gameObjectPrefeb = null;
                                        }

                                        GameObject obj = effectFrames.effects.rootObject;

                                        if (obj && bEffectShow)
                                        {
                                            effectFrames.localPosition =
                                                EditorGUILayout.Vector3Field("位置", effectFrames.localPosition);
                                            Quaternion rot = effectFrames.localRotation;
                                            rot.eulerAngles =
                                                EditorGUILayout.Vector3Field("旋转", rot.eulerAngles);
                                            effectFrames.localRotation = rot;


                                            if (effectFrames.localScale == Vector3.zero)
                                            {
                                                effectFrames.localScale = Vector3.one;
                                            }

                                            effectFrames.localScale =
                                                EditorGUILayout.Vector3Field("缩放", effectFrames.localScale);
                                            effectFrames.AdjustScale();
                                            effectFrames.effects.UpdateData(effectFrames);
                                        }


                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();


                                        if (StyledButton("定位起始帧"))
                                        {
                                            animFrame = effectFrames.startFrames;
                                            AnimationSampler(skillData);
                                            return;
                                        }

                                        if (StyledButton("定为当前帧"))
                                        {
                                            effectFrames.startFrames = (int) animFrame;
                                            AnimationSampler(skillData);
                                            return;
                                        }

                                        if (
                                            proxyScript.skilleditmode == SkillSenceEditMode.EffectEdit
                                            &&
                                            proxyScript.currentEditObject == effectFrames
                                        )
                                        {
                                            if (StyledButton("终止编辑"))
                                            {
                                                proxyScript.currentEditObject = null;
                                                proxyScript.skilleditmode = SkillSenceEditMode.Normal;
                                                OnEditChangeOrSelectChange();
                                            }
                                        }
                                        else
                                        {
                                            if (StyledButton("编辑"))
                                            {
                                                proxyScript.skilleditmode = SkillSenceEditMode.EffectEdit;
                                                proxyScript.currentEditObject = effectFrames;
                                                if (Tools.current < Tool.Move || Tools.current > Tool.Scale)
                                                {
                                                    Tools.current = Tool.Move;
                                                }

                                                OnEditChangeOrSelectChange();
                                            }
                                        }

                                        if (StyledButton("移除"))
                                        {
                                            effectFrames.effects.Show(false);
                                            SkillDataUndo("Remove Effect");
                                            skillData.effectFrames =
                                                RemoveElement<EffectsFrames>(skillData.effectFrames, effectFrames);
                                            return;
                                        }

                                        if (StyledButton("复制坐标"))
                                        {
                                            transcopy.localPosition = effectFrames.localPosition;
                                            transcopy.localRotation = effectFrames.localRotation;
                                            transcopy.localScale = effectFrames.localScale;
                                        }

                                        if (StyledButton("粘贴坐标"))
                                        {
                                            SkillDataUndo("Modify Effect");
                                            effectFrames.localPosition = transcopy.localPosition;
                                            effectFrames.localRotation = transcopy.localRotation;
                                            effectFrames.localScale = transcopy.localScale;
                                            OnSamplerEffectFrames(skillData);
                                        }

                                        if (StyledButton("复制特效"))
                                        {
                                            SkillDataUndo("Copy Effect");
                                            mCopyData = effectFrames;
                                        }

                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.Space();
                                    EditorGUIUtility.labelWidth = 150;
                                    EditorGUILayout.Space();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }

                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            #endregion

            OnGUIEntityFrames(proxyScript);
            //OnGUIFramesEvent(proxyScript);

            OnGUIFramesEvent<DSkillFrameTag>(proxyScript, ref skillData.frameTags, "帧标签", ref frameTagsToggle,
                ref mCopySkillData.frameTags);
            OnGUIFramesEvent<DSkillFrameGrap>(proxyScript, ref skillData.frameGrap, "抓取配置", ref frameGrapToggle,
                ref mCopySkillData.frameGrap,
                item =>
                {
                    if (item != null && item.obj != null)
                    {
                        item.obj.Destroy();
                    }
                }
            );
            /*用不到
            OnGUIFramesEvent<DSkillFrameStateOp>(proxyScript, ref skillData.stateop, "状态栈操作", ref stateopToggle,
                ref mCopySkillData.stateop);
                */
            OnGUIFramesEvent<DSkillPropertyModify>(proxyScript, ref skillData.properModify, "属性操作",
                ref properModifyToggle, ref mCopySkillData.properModify);
            OnGUIFramesEvent<DSkillFrameEventSceneShock>(proxyScript, ref skillData.shocks, "屏震", ref shocksToggle,
                ref mCopySkillData.shocks);
            OnGUIFramesEvent<DSkillSfx>(proxyScript, ref skillData.sfx, "音效配置", ref sfxToggle, ref mCopySkillData.sfx);
            OnGUIFramesEvent<DSkillFrameEffect>(proxyScript, ref skillData.frameEffects, "帧效果", ref frameEffectToggle,
                ref mCopySkillData.frameEffects);
            OnGUIFramesEvent<DSkillCameraMove>(proxyScript, ref skillData.cameraMoves, "摄像机偏移", ref cameraMoveToggle,
                ref mCopySkillData.cameraMoves);
            OnGUIFramesEvent<DSkillWalkControl>(proxyScript, ref skillData.walkControl, "移动控制", ref walkControlToggle,
                ref skillData.walkControl);
            OnGUIFramesEvent<DActionData>(proxyScript, ref skillData.actions, "简单动画", ref actionToggle,
                ref mCopySkillData.actions);

            OnGUIFramesEvent<DSkillBuff>(proxyScript, ref skillData.buffs, "添加Buff信息或者Buff",
                ref addBuffInfoOrBuffToggle, ref mCopySkillData.buffs);
            OnGUIFramesEvent<DSkillSummon>(proxyScript, ref skillData.summons, "召唤怪物（实体技能配置文件不能召唤怪物！！！）",
                ref summonToggle, ref mCopySkillData.summons);
            OnGUIFramesEvent<DSkillMechanism>(proxyScript, ref skillData.mechanisms, "添加机制", ref mechanismToggle,
                ref mCopySkillData.mechanisms);
        }
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            //Undo.RecordObject(skillData, "SkillData Editor Modify");
            EditorUtility.SetDirty(skillData);
        }
    }

    private bool IsAutoFreshOpen = true;
    private bool IsAutoFreshDirty = false;
    private bool IsSaveFileDirty = false;

    private void _OnGUIDrawHeaderUtility()
    {
        if (null != character)
        {
            if (GUILayout.Button("保存", GUILayout.Width(40)))
            {
                IsSaveFileDirty = true;
            }
        }

        var iter = DSkillDataMapper.GetRefEntitys(this.skillData);
        while (iter.MoveNext())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                IsAutoFreshOpen = GUILayout.Toggle(IsAutoFreshOpen, "", GUILayout.Width(40));
                GUILayout.Label("点保存直接刷新", GUILayout.Width(120));

                if ((IsAutoFreshOpen && IsAutoFreshDirty) || GUILayout.Button("刷新", GUILayout.Width(40)))
                {
                    IsAutoFreshDirty = false;
                    DSkillDataMapper.RefreshLinkData(iter.Current, skillData);
                }

                GUILayout.Label(iter.Current.GetPID().ToString(), GUILayout.Width(100));
                GUILayout.Label(iter.Current.GetName(), GUILayout.Width(300));
            }
        }
    }

    /// <summary>
    /// 传入指定参数获取改变后的枚举值
    /// </summary>
    /// <typeparam name="P">类型枚举</typeparam>
    /// <param name="dsc">下拉列表描述</param>
    /// <param name="p">当前选择值</param>
    /// <returns>返回修改后的值</returns>
    protected P GetChangeEnumType<P>(string dsc, P p)
    {
        string[] stringArr = typeof(P).GetDescriptions();
        Array valueArr = typeof(P).GetValues();
        int index = Array.BinarySearch(valueArr, p);
        index = EditorGUILayout.Popup(dsc, index, stringArr);
        if (index >= 0)
        {
            return (P) valueArr.GetValue(index);
        }

        return default(P);
    }

    public string AttachmentPopup(string lable, string attachname)
    {
        int iSelect = -1;

        for (int j = 0; j < _attachnames.Length; ++j)
        {
            if (_attachnames[j] == attachname)
            {
                iSelect = j;
            }
        }

        iSelect = EditorGUILayout.Popup(lable, iSelect, _attachnames);

        string names = "";
        if (iSelect >= 0 && iSelect < _attachnames.Length)
        {
            names = _attachnames[iSelect];
        }

        return names;
    }

    public bool StyledButton(string label)
    {
        EditorGUILayout.Space();
        GUILayoutUtility.GetRect(1, 20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool clickResult = GUILayout.Button(label, "miniButton");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        return clickResult;
    }

    public delegate void OnButtonClick();

    public void StyledButtonEx(string label, OnButtonClick onClick, ref Rect rect)
    {
        EditorGUILayout.Space();
        GUILayoutUtility.GetRect(1, 20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(label, "miniButton"))
        {
            if (onClick != null)
            {
                onClick();
            }
        }

        if (Event.current.type == EventType.Repaint)
        {
            rect = GUILayoutUtility.GetLastRect();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }


    public bool StyledToggle(bool value, string label)
    {
        EditorGUILayout.Space();
        GUILayoutUtility.GetRect(1, 20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool clickResult = GUILayout.Toggle(value, label, (GUIStyle) "miniButton");
        //bool clickResult = GUILayout.SelectionGrid(value ? 0 : -1, new Texture[] { Skill.Editor.Resources.UITextures.SelectedEventBorder }, 1) == 0;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        return clickResult;
    }

    public void StyledTimePeriodShow(string label, ref float current, float min, float max, float minLimit,
        float maxLimit, int indentLevel, string style = "flow node 2 on")
    {
        int indentSpacing = 25 * indentLevel;
        //indentSpacing += 30;
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        float minValueFloat = (float) min;
        float maxValueFloat = (float) max;
        float minLimitFloat = (float) minLimit;
        float maxLimitFloat = (float) maxLimit;

        Rect tempRect = GUILayoutUtility.GetRect(1, 10);
        float width = Screen.width / EditorGUIUtility.pixelsPerPoint;
        Rect rect = new Rect(indentSpacing, tempRect.y, width - indentSpacing - 100 - 20, 20);

        float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
        float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
        float fillWidth = fillRightPos - fillLeftPos;

        // Border
        GUI.Box(rect, "", borderBarStyle);

        // Overlay
        GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), style);

        // Text
        //GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        //centeredStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(rect, label + " : " + current + "[" + (minValueFloat) + " - " + (maxValueFloat) + "]", labelStyle);
        labelStyle.alignment = TextAnchor.MiddleCenter;

        // Slider
        rect.y += 20;
        rect.x = indentLevel * 10;
        rect.width = (width - (indentLevel * 20) - 100) + 55;

        current = EditorGUI.Slider(rect, (float) current, minLimitFloat, maxLimitFloat);
        tempRect = GUILayoutUtility.GetRect(1, 30);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    public void StyledTimePeriodShow(string label, ref int current, int min, int max, int minLimit, int maxLimit,
        int indentLevel)
    {
        int indentSpacing = 25 * indentLevel;
        //indentSpacing += 30;
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        float minValueFloat = (float) min;
        float maxValueFloat = (float) max;
        float minLimitFloat = (float) minLimit;
        float maxLimitFloat = (float) maxLimit;

        Rect tempRect = GUILayoutUtility.GetRect(1, 10);

        float width = Screen.width / EditorGUIUtility.pixelsPerPoint;
        Rect rect = new Rect(indentSpacing, tempRect.y, width - indentSpacing - 100, 20);

        float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
        float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
        float fillWidth = fillRightPos - fillLeftPos;

        // Border
        GUI.Box(rect, "", borderBarStyle);

        // Overlay
        GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);

        // Text
        //GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        //centeredStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(rect,
            label + " : " + current + "[" + Mathf.Floor(minValueFloat) + " - " + Mathf.Floor(maxValueFloat) + "]",
            labelStyle);
        labelStyle.alignment = TextAnchor.MiddleCenter;

        // Slider
        rect.y += 20;
        rect.x = indentLevel * 10;
        rect.width = (width - (indentLevel * 10) - 100) + 55;

        current = (int) EditorGUI.Slider(rect, (float) current, minLimitFloat, maxLimitFloat);
        tempRect = GUILayoutUtility.GetRect(1, 30);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    public void StyledMinMaxSlider(string label, ref int minValue, ref int maxValue, int minLimit, int maxLimit,
        int indentLevel)
    {
        int indentSpacing = 25 * indentLevel;
        //indentSpacing += 30;
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (minValue < 1) minValue = 1;
        if (maxValue < 2) maxValue = 2;
        if (maxValue > maxLimit) maxValue = maxLimit;
        if (minValue == maxValue) minValue--;
        float minValueFloat = (float) minValue;
        float maxValueFloat = (float) maxValue;
        float minLimitFloat = (float) minLimit;
        float maxLimitFloat = (float) maxLimit;

        Rect tempRect = GUILayoutUtility.GetRect(1, 10);


        float width = Screen.width / EditorGUIUtility.pixelsPerPoint;
        Rect rect = new Rect(indentSpacing, tempRect.y, width - indentSpacing - 100, 20);
        //Rect rect = new Rect(indentSpacing + 15,tempRect.y, Screen.width - indentSpacing - 70, 20);
        float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
        float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
        float fillWidth = fillRightPos - fillLeftPos;

        fillWidth += (rect.width / maxLimitFloat);
        fillLeftPos -= (rect.width / maxLimitFloat);

        // Border
        GUI.Box(rect, "", borderBarStyle);

        // Overlay
        GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);

        // Text
        //GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        //centeredStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(rect, label + " between " + Mathf.Floor(minValueFloat) + " and " + Mathf.Floor(maxValueFloat),
            labelStyle);
        labelStyle.alignment = TextAnchor.MiddleCenter;

        // Slider
        rect.y += 10;
        rect.x = indentLevel * 10;
        rect.width = (width - (indentLevel * 10) - 100);

        EditorGUI.MinMaxSlider(rect, ref minValueFloat, ref maxValueFloat, minLimitFloat, maxLimitFloat);
        minValue = (int) minValueFloat;
        maxValue = (int) maxValueFloat;

        tempRect = GUILayoutUtility.GetRect(1, 20);
    }

    public void StyledMarker(string label, int[] locations, int maxValue, int indentLevel)
    {
        if (indentLevel == 1) indentLevel++;
        int indentSpacing = 25 * indentLevel;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Rect tempRect = GUILayoutUtility.GetRect(1, 20);
        Rect rect = new Rect(indentSpacing, tempRect.y, Screen.width - indentSpacing - 60, 20);

        // Border
        GUI.Box(rect, "", borderBarStyle);

        // Overlay
        foreach (int i in locations)
        {
            float xPos = ((rect.width / (float) maxValue) * (float) i) + rect.x;
            if (xPos + 5 > rect.width + rect.x) xPos -= 5;
            GUI.Box(new Rect(xPos, rect.y, 5, rect.height), new GUIContent(), fillBarStyle2);
        }

        // Text
        GUI.Label(rect, new GUIContent(label), labelStyle);

        tempRect = GUILayoutUtility.GetRect(1, 20);
    }

    public void StyledMarker(string label, Vector2[] locations, int maxValue, int indentLevel, bool fillBounds)
    {
        if (indentLevel == 1) indentLevel++;
        int indentSpacing = 25 * indentLevel;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Rect tempRect = GUILayoutUtility.GetRect(1, 20);

        float width = Screen.width / EditorGUIUtility.pixelsPerPoint;

        Rect rect = new Rect(indentSpacing, tempRect.y, width - indentSpacing - 60, 20);

        // Border
        GUI.Box(rect, "", borderBarStyle);

        if (fillBounds && locations.Length > 0)
        {
            float firstLeftPos = ((rect.width / maxValue) * locations[0].x);
            firstLeftPos -= (rect.width / maxValue);
            GUI.Box(new Rect(rect.x, rect.y, firstLeftPos, rect.height), new GUIContent(), fillBarStyle3);
        }

        // Overlay
        float fillLeftPos = 0;
        float fillRightPos = 0;
        foreach (Vector2 i in locations)
        {
            fillLeftPos = ((rect.width / maxValue) * i.x) + rect.x;
            fillRightPos = ((rect.width / maxValue) * i.y) + rect.x;

            float fillWidth = fillRightPos - fillLeftPos;
            fillWidth += (rect.width / maxValue);
            fillLeftPos -= (rect.width / maxValue);

            GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);
        }

        if (fillBounds && locations.Length > 0)
        {
            float fillWidth = rect.width - fillRightPos + rect.x;
            GUI.Box(new Rect(fillRightPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle4);
        }

        // Text
        GUI.Label(rect, new GUIContent(label), labelStyle);

        if (fillBounds && locations.Length > 0)
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal(subArrayElementStyle);
            /*{
                labelStyle.normal.textColor = Color.yellow;
                skillData.startUpFrames = (skillData.HurtBlocks[0].activeFramesBegin - 1);
                GUILayout.Label("Start Up: " + skillData.startUpFrames, labelStyle);
                labelStyle.normal.textColor = Color.cyan;
                skillData.activeFrames = (skillData.HurtBlocks[skillData.HurtBlocks.Length - 1].activeFramesEnds - skillData.HurtBlocks[0].activeFramesBegin);
                GUILayout.Label("Active: " + skillData.activeFrames, labelStyle);
                labelStyle.normal.textColor = Color.red;
                skillData.recoveryFrames = (skillData.totalFrames - skillData.HurtBlocks[skillData.HurtBlocks.Length - 1].activeFramesEnds + 1);
                GUILayout.Label("Recovery: " + skillData.recoveryFrames, labelStyle);
            }*/
            GUILayout.EndHorizontal();
        }

        labelStyle.normal.textColor = Color.white;

        //GUI.skin.label.normal.textColor = new Color(.706f, .706f, .706f, 1);
        tempRect = GUILayoutUtility.GetRect(1, 20);
    }

    public float GetFramesTime(int iFrame)
    {
        float time = 0.0f;
        if (skillData.animationSpeed < 0)
        {
            time = (((float) iFrame / skillData.fps) * skillData.animationSpeed) + animClip.length;
        }
        else
        {
            time = ((float) iFrame / skillData.fps) * skillData.animationSpeed;
        }

        return time;
    }

    public float GetFramesTime(float fFrame)
    {
        float time = 0.0f;
        if (GetCurSkillData().animationSpeed < 0)
        {
            time = (((float) fFrame / GetCurSkillData().fps) * GetCurSkillData().animationSpeed) + animClip.length;
        }
        else
        {
            time = ((float) fFrame / GetCurSkillData().fps) * GetCurSkillData().animationSpeed;
        }

        return time;
    }

    public void AnimationSampler(DSkillData skillData)
    {
        float oldAnimTime = animTime;

        if (skillData.animationSpeed < 0)
        {
            animTime = ((animFrame / skillData.fps) * skillData.animationSpeed) + animClip.length;
        }
        else
        {
            animTime = (animFrame / skillData.fps) * skillData.animationSpeed;
        }


        DSkillDataEditorDrawer proxyScript = character.GetComponent<DSkillDataEditorDrawer>();
        if (proxyScript != null)
        {
            if (oldAnimTime != animTime)
            {
                proxyScript.currentEditObject = null;
            }
            //float animFrameIntel = animFrame + 1.0f;

            proxyScript.activeHurtBoxes = null;

            int iFrame = (int) animFrame;

            if (iFrame >= 0 && iFrame < skillData.HurtBlocks.Length)
            {
                proxyScript.activeHurtBoxes = skillData.HurtBlocks[iFrame];
            }

            if (iFrame >= 0 && iFrame < skillData.DefenceBlocks.Length)
            {
                proxyScript.activeDefBoxes = skillData.DefenceBlocks[iFrame];
            }
        }


        if (skillData.loopAnimation && animClip != null)
        {
            animTime = animTime % animClip.length;
        }


        if (animClip)
        {
            animClip.SampleAnimation(character, animTime /* * skillData.animationSpeed*/);
        }


        OnSamplerAttachFrames(skillData);
        OnSamplerEffectFrames(skillData);
        OnSamplerEntityFrames(skillData);
        OnSampleGrapObj(skillData);

        //有些特效在play模式下不能播放，怀疑是没有刷新
        if (_Play||mIsPlayAllStep)
            SceneView.RepaintAll();
    }

    public T[] RemoveElement<T>(T[] elements, T element, Action<T> action = null)
    {
        List<T> elementsList = new List<T>(elements);
        elementsList.Remove(element);
        action?.Invoke(element);
        return elementsList.ToArray();
    }

    public T[] AddElement<T>(T[] elements, T element)
    {
        List<T> elementsList;


        if (elements == null)
        {
            elementsList = new List<T>();
        }
        else
        {
            elementsList = new List<T>(elements);
        }

        elementsList.Add(element);
        return elementsList.ToArray();
    }

    public delegate void CopyData<T>(T newdata, T olddata);

    public int GetTotalValidDataCount(BaseDecisionBox[] elements)
    {
        int count = 0;
        for(int i=0; i<elements.Length; ++i)
        {
            if (elements[i].boxs.Length > 0)
                count++;
        }

        return count;
    }

    public T ArrayGetCurrentFrameData<T>(ref T[] elements, CopyData<T> copyFunc) where T : new()
    {
        int iTotoalFrames = skillData.totalFrames;

        if (elements.Length != iTotoalFrames)
        {
            T[] news = new T[iTotoalFrames];
            for (int i = 0; i < iTotoalFrames; ++i)
            {
                news[i] = new T();
            }

            int iMin = Mathf.Min(elements.Length, iTotoalFrames);

            for (int i = 0; i < iMin; ++i)
            {
                copyFunc(news[i], elements[i]);
            }

            if (elements.Length < iTotoalFrames)
            {
                elements = news;
            }
        }

        int iFrames = (int) animFrame;
        iFrames = Mathf.Max(Mathf.Min(iFrames, iTotoalFrames - 1), 0);
        return elements[iFrames];
    }

    //编辑器优化——攻击框丢失
    public void SaveCurrentFrameData<T>(ref T[] elements, CopyData<T> copyFunc) where T : new()
    {
        int iTotoalFrames = skillData.totalFrames;

        if (elements.Length != iTotoalFrames)
        {
            T[] news = new T[iTotoalFrames];
            for (int i = 0; i < iTotoalFrames; ++i)
            {
                news[i] = new T();
            }

            int iMin = Mathf.Min(elements.Length, iTotoalFrames);

            for (int i = 0; i < iMin; ++i)
            {
                copyFunc(news[i], elements[i]);
            }

            elements = news;
        }
    }

    public ShapeBox[] CopyShapeBox(ShapeBox[] elements)
    {
        ShapeBox[] newShape = new ShapeBox[elements.Length];

        for (int i = 0; i < elements.Length; ++i)
        {
            newShape[i] = new ShapeBox();
            newShape[i].center = elements[i].center;
            newShape[i].size = elements[i].size;
        }

        return newShape;
    }

    public int GetCurrentFrame()
    {
        int iFrames = (int) animFrame;
        iFrames = Mathf.Max(Mathf.Min(iFrames, skillData.totalFrames - 1), 0);
        return iFrames;
    }

    void OnGUIAttachFrames(DSkillDataEditorDrawer proxyScript)
    {
        EditorGUILayout.BeginVertical(rootGroupStyle);
        {
            attachToggle = EditorGUILayout.Foldout(attachToggle, "挂件 (" + GetCurSkillData().attachFrames.Length + ")",
                EditorStyles.foldout);
            if (attachToggle && proxyScript)
            {
                EditorGUILayout.BeginVertical(subGroupStyle);
                {
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel += 1;

                    EditorGUILayout.BeginHorizontal();

                    if (StyledButton("添加挂件"))
                    {
                        EntityAttachFrames frame = new EntityAttachFrames();
                        frame.name = "[" + skillData.attachFrames.Length + "]";
                        frame.start = animFrame;
                        frame.end = frame.start + 5;
                        skillData.attachFrames = AddElement<EntityAttachFrames>(skillData.attachFrames, frame);
                    }

                    if (StyledButton("清空挂件"))
                    {
                        for (int i = 0; i < skillData.attachFrames.Length; ++i)
                        {
                            EntityAttachFrames Frames = skillData.attachFrames[i];
                            Frames.attach.Destroy();
                        }

                        skillData.attachFrames = new EntityAttachFrames[0];
                    }

                    if (StyledButton("一键挂件"))
                    {
                        OneKeyReplaceAttach();
                    }

                    if (StyledButton("一键清除挂件"))
                    {
                        OneKeyRemoveAttach();
                    }

                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < GetCurSkillData().attachFrames.Length; ++i)
                    {
                        EntityAttachFrames attachFrame = GetCurSkillData().attachFrames[i];

                        if (attachFrame == null || attachFrame.attach == null)
                        {
                            continue;
                        }

                        float fBeginTime = GetFramesTime(attachFrame.start);
                        float fEndTime = GetFramesTime(attachFrame.end);

                        bool bShow = animTime >= fBeginTime && animTime <= fEndTime;
                        if (true)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical(arrayElementStyle);
                            {
                                EditorGUILayout.Space();

                                EditorGUILayout.BeginVertical(subArrayElementStyle);
                                {
                                    //EditorGUILayout.LabelField(attachFrame.name, TablabelStyle);
                                    TabLabel(attachFrame, attachFrame.name, proxyScript);
                                    float current = animFrame;
                                    float endframes = attachFrame.end;
                                    float oldcurrent = current;
                                    StyledTimePeriodShow(attachFrame.name, ref current, attachFrame.start,
                                        attachFrame.end, 0, GetCurSkillData().totalFrames - 1, EditorGUI.indentLevel,
                                        fillBarStyle1);

                                    if (current != oldcurrent)
                                    {
                                        float Delta = current - oldcurrent;
                                        float newstart = attachFrame.start + Delta;

                                        if (newstart >= 0 && newstart < GetCurSkillData().totalFrames - 1)
                                        {
                                            attachFrame.end = attachFrame.end - attachFrame.start + newstart;
                                            attachFrame.end = Mathf.Min(attachFrame.end,
                                                GetCurSkillData().totalFrames - 1);
                                            attachFrame.start = newstart;
                                            animFrame = current;
                                            AnimationSampler(GetCurSkillData());
                                            return;
                                        }
                                    }

                                    string tname = EditorGUILayout.TextField("名字:", attachFrame.name);
                                    if (tname != attachFrame.name)
                                    {
                                        attachFrame.name = tname;
                                        UpdateAttachNodes();
                                        return;
                                    }

                                    //attachFrame.resID = EditorGUILayout.IntField("ResID", attachFrame.resID);
                                    attachFrame.resID = mResTable.OnGUIWithID(attachFrame.resID);


                                    float value;
                                    value = EditorGUILayout.FloatField("开始帧序号: ", attachFrame.start);

                                    if (value != attachFrame.start)
                                    {
                                        attachFrame.start = value;
                                        AnimationSampler(GetCurSkillData());
                                        return;
                                    }

                                    value = EditorGUILayout.FloatField("结束帧序号: ", attachFrame.end);

                                    if (value != attachFrame.end)
                                    {
                                        attachFrame.end = value;
                                        attachFrame.end = Mathf.Min(attachFrame.end, GetCurSkillData().totalFrames - 1);
                                        AnimationSampler(GetCurSkillData());
                                        return;
                                    }

                                    string names = AttachmentPopup("挂点选择", attachFrame.attachName);
                                    if (names != attachFrame.attachName)
                                    {
                                        attachFrame.attach.UpdateParent
                                        (
                                            GetAttachNodes(names)
                                        );

                                        attachFrame.attachName = names;

                                        AnimationSampler(GetCurSkillData());
                                        return;
                                    }

                                    GameObject newEffectPrefab = (GameObject) EditorGUILayout.ObjectField("预制体: ",
                                        attachFrame.entityPrefab, typeof(UnityEngine.GameObject), true);


                                    if (newEffectPrefab)
                                    {
                                        if (attachFrame.entityPrefab != newEffectPrefab &&
                                            PrefabUtility.GetPrefabType(newEffectPrefab) == PrefabType.Prefab)
                                        {
                                            attachFrame.entityPrefab = newEffectPrefab;
                                            attachFrame.entityAsset = new DAssetObject(newEffectPrefab);
                                            attachFrame.attach.ObjectPrefab = newEffectPrefab;
                                        }
                                    }
                                    else
                                    {
                                        attachFrame.entityPrefab = null;
                                        attachFrame.attach.ObjectPrefab = null;
                                    }


                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginHorizontal();

                                    if (StyledButton("定位起始帧"))
                                    {
                                        animFrame = attachFrame.start;
                                        AnimationSampler(skillData);
                                        return;
                                    }

                                    if (StyledButton("设定起始帧"))
                                    {
                                        attachFrame.start = animFrame;
                                        if (attachFrame.end < attachFrame.start)
                                        {
                                            attachFrame.end = attachFrame.start;
                                        }

                                        AnimationSampler(skillData);
                                        return;
                                    }

                                    if (StyledButton("设定终止帧"))
                                    {
                                        attachFrame.end = animFrame;

                                        if (attachFrame.end < attachFrame.start)
                                        {
                                            attachFrame.end = attachFrame.start;
                                        }

                                        AnimationSampler(skillData);
                                        return;
                                    }

                                    if (StyledButton("移除挂件"))
                                    {
                                        SkillDataUndo("EntityAttach remove");
                                        attachFrame.attach.Destroy();
                                        skillData.attachFrames =
                                            RemoveElement<EntityAttachFrames>(skillData.attachFrames, attachFrame);
                                        return;
                                    }

                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginVertical(arrayElementStyle);
                                    if (attachFrame.attach.Visiable)
                                    {
                                        EditorGUILayout.Space();
                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();


                                        EditorGUILayout.LabelField("Animations： ");

                                        if (StyledButton("添加动画"))
                                        {
                                            AnimationFrames af = new AnimationFrames();
                                            af.start = animFrame;
                                            attachFrame.animations =
                                                AddElement<AnimationFrames>(attachFrame.animations, af);
                                            return;
                                        }

                                        if (StyledButton("清空动画"))
                                        {
                                            SkillDataUndo("EntityAttach clearAll anim");
                                            attachFrame.animations = new AnimationFrames[0];
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginVertical(subArrayElementStyle);
                                        {
                                            for (int k = 0; k < attachFrame.animations.Length; ++k)
                                            {
                                                AnimationFrames anim = attachFrame.animations[k];
                                                float current2 = animFrame;
                                                float length = 1.0f;
                                                if (anim.clip != null)
                                                {
                                                    length = anim.clip.length * GetCurSkillData().fps;
                                                }

                                                float endframes2 = anim.start + length;
                                                endframes2 = Mathf.Min(endframes2, GetCurSkillData().totalFrames - 1);

                                                float oldcurrent2 = current2;
                                                StyledTimePeriodShow(anim.anim, ref current2, anim.start, endframes2, 0,
                                                    GetCurSkillData().totalFrames - 1, EditorGUI.indentLevel,
                                                    fillBarStyle4);
                                                if (current2 != oldcurrent2)
                                                {
                                                    float Delta = current2 - oldcurrent2;
                                                    float newstart = anim.start + Delta;

                                                    if (newstart >= 0 && newstart < GetCurSkillData().totalFrames - 1)
                                                    {
                                                        anim.start = newstart;
                                                        animFrame = current2;
                                                        AnimationSampler(GetCurSkillData());
                                                        return;
                                                    }
                                                }


                                                int index = attachFrame.attach.FindIndex(anim.anim);
                                                index = EditorGUILayout.Popup("Animation:", index,
                                                    attachFrame.attach.animsarray);
                                                string anim_name = "";

                                                if (index >= 0 && index < attachFrame.attach.animsarray.Length)
                                                {
                                                    anim_name = attachFrame.attach.animsarray[index];
                                                }

                                                if (anim_name != anim.anim)
                                                {
                                                    anim.anim = anim_name;
                                                    anim.clip = attachFrame.attach.Clip(anim_name);
                                                }

                                                anim.start = EditorGUILayout.FloatField("Start:", anim.start);
                                                anim.speed = EditorGUILayout.FloatField("Speed:", anim.speed);

                                                EditorGUILayout.BeginHorizontal();
                                                if (StyledButton("定位起始"))
                                                {
                                                    animFrame = anim.start;
                                                    AnimationSampler(skillData);
                                                    return;
                                                }

                                                if (StyledButton("设定起始"))
                                                {
                                                    anim.start = animFrame;
                                                    AnimationSampler(skillData);
                                                    return;
                                                }

                                                if (StyledButton("移除动画"))
                                                {
                                                    SkillDataUndo("EntityAttach remove anim");
                                                    attachFrame.animations =
                                                        RemoveElement<AnimationFrames>(attachFrame.animations, anim);
                                                    return;
                                                }

                                                EditorGUILayout.EndHorizontal();
                                            }
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField("Animations(" + attachFrame.animations.Length + ")");
                                    }

                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 150;
                                EditorGUILayout.Space();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndVertical();
    }

    //Entity Frames
    void OnGUIEntityFrames(DSkillDataEditorDrawer proxyScript)
    {
        //EntityFrames
        EditorGUILayout.BeginVertical(rootGroupStyle);
        {
            EntityToggle = EditorGUILayout.Foldout(EntityToggle, "创建实体 (全部" + skillData.entityFrames.Length + ")",
                EditorStyles.foldout);
            if (EntityToggle && proxyScript)
            {
                EditorGUILayout.BeginVertical(subGroupStyle);
                {
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel += 1;

                    EditorGUILayout.BeginHorizontal();

                    if (StyledButton("添加实体"))
                    {
                        EntityFrames frame = new EntityFrames();
                        frame.name = "[" + skillData.entityFrames.Length + "]" + frame.type;
                        frame.startFrames = (int) animFrame;
                        skillData.entityFrames = AddElement<EntityFrames>(skillData.entityFrames, frame);
                    }

                    if (StyledButton("清除实体"))
                    {
                        for (int i = 0; i < skillData.entityFrames.Length; ++i)
                        {
                            EntityFrames Frames = skillData.entityFrames[i];
                            Frames.entity.Destroy();
                        }

                        skillData.entityFrames = new EntityFrames[0];
                    }

                    bool bShow = EditorGUILayout.Toggle("显示全部Entity", bShowAllEntity);
                    if (bShow != bShowAllEntity)
                    {
                        bShowAllEntity = bShow;
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    if (StyledButton("跨文件保存实体"))
                    {
                        if (skillData.entityFrames != null && skillData.entityFrames.Length != 0)
                        {
                            mCopySkillData.entityFrames = new EntityFrames[skillData.entityFrames.Length];
                            for (int i = 0; i < skillData.entityFrames.Length; ++i)
                            {
                                mCopySkillData.entityFrames[i] = new EntityFrames();
                                mCopySkillData.entityFrames[i].Copy(skillData.entityFrames[i]);
                            }

                            EditorUtility.SetDirty(mCopySkillData);
                        }
                    }

                    if (StyledButton("跨文件粘贴实体"))
                    {
                        if (mCopySkillData.entityFrames != null && mCopySkillData.entityFrames.Length != 0)
                        {
                            skillData.entityFrames = new EntityFrames[mCopySkillData.entityFrames.Length];
                            for (int i = 0; i < mCopySkillData.entityFrames.Length; ++i)
                            {
                                skillData.entityFrames[i] = new EntityFrames();
                                skillData.entityFrames[i].Copy(mCopySkillData.entityFrames[i]);
                            }

                            EditorUtility.SetDirty(skillData);
                        }
                    }

                    if (StyledButton("粘贴实体"))
                    {
                        if (mCopyData is EntityFrames)
                        {
                            SkillDataUndo("Paste Entity");
                            EntityFrames mFakeData = new EntityFrames();
                            mFakeData.Copy(mCopyData as EntityFrames);
                            skillData.entityFrames = AddElement<EntityFrames>(skillData.entityFrames, mFakeData);
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < skillData.entityFrames.Length; ++i)
                    {
                        EntityFrames entityFrame = skillData.entityFrames[i];

                        if (entityFrame == null || entityFrame.entity == null)
                        {
                            continue;
                        }

                        if (!SetFoldeDict("entityFrame", i,
                            EditorGUILayout.Foldout(GetFoldeDict("entityFrame", i), entityFrame.name)))
                        {
                            continue;
                        }

                        float fBeginTime = GetFramesTime(entityFrame.startFrames);

                        float fEndTime = fBeginTime + entityFrame.entity.timelength;

                        if (animTime >= fBeginTime && animTime <= fEndTime || bShowAllEntity)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical(arrayElementStyle);
                            {
                                EditorGUILayout.Space();

                                EditorGUILayout.BeginVertical(subArrayElementStyle);
                                {
                                    //EditorGUILayout.LabelField(entityFrame.name, TablabelStyle);
                                    TabLabel(entityFrame, entityFrame.name, proxyScript);
                                    int current = (int) animFrame;
                                    int endframes = Mathf.CeilToInt(entityFrame.entity.timelength * skillData.fps);
                                    endframes = entityFrame.startFrames + endframes;
                                    endframes = Mathf.Min(endframes, skillData.totalFrames - 1);
                                    int oldcurrent = current;
                                    EditorGUI.indentLevel++;
                                    StyledTimePeriodShow(entityFrame.name, ref current, entityFrame.startFrames,
                                        endframes, 0, skillData.totalFrames - 1, EditorGUI.indentLevel);
                                    EditorGUI.indentLevel--;

                                    if (current != oldcurrent)
                                    {
                                        int iDelta = current - oldcurrent;
                                        int iNewstartFrames = entityFrame.startFrames + iDelta;

                                        if (iNewstartFrames >= 0 && iNewstartFrames < skillData.totalFrames - 1)
                                        {
                                            entityFrame.startFrames = iNewstartFrames;
                                            animFrame = current;
                                            AnimationSampler(skillData);
                                        }

                                        return;
                                    }

                                    string name = EditorGUILayout.TextField("Name:", entityFrame.name);
                                    if (name != entityFrame.name)
                                    {
                                        entityFrame.name = name;
                                        return;
                                    }

                                    entityFrame.randomResID =
                                        EditorGUILayout.Toggle(new GUIContent("是否随机实体ID", "用|隔开实体ID"),
                                            entityFrame.randomResID);
                                    if (entityFrame.randomResID)
                                    {
                                        int resIDNum = EditorGUILayout.IntField(new GUIContent("随机实体数量:", "用于随机发射实体"),
                                            entityFrame.resIDList.Length);


                                        if (resIDNum != entityFrame.resIDList.Length)
                                        {
                                            var tmp = entityFrame.resIDList;
                                            entityFrame.resIDList = new int[resIDNum];

                                            for (int j = 0; j < Mathf.Min(tmp.Length, resIDNum); ++j)
                                                entityFrame.resIDList[j] = tmp[j];
                                        }

                                        EditorGUILayout.BeginVertical(rootGroupStyle);
                                        {
                                            for (int j = 0; j < entityFrame.resIDList.Length; ++j)
                                            {
                                                entityFrame.resIDList[j] =
                                                    EditorGUILayout.IntField(new GUIContent("实体ID"),
                                                        entityFrame.resIDList[j]);
                                            }
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    else
                                    {
                                        //entityFrame.resID = EditorGUILayout.IntField(new GUIContent("ResID", "实体ID，对应模型资源表ID"), entityFrame.resID);
                                        entityFrame.resID =
                                            mResTable.OnGUIWithID(entityFrame
                                                .resID); //EditorGUILayout.IntField("模型资源表ID", entityFrame.resID);
                                    }

                                    //entityFrame.type =
                                    //    (EntityType) EditorGUILayout.EnumPopup("PlayType", entityFrame.type, enumStyle);


                                    //entityFrame.entity.timelength = EditorGUILayout.FloatField("Time", entityFrame.entity.timelength);
                                    entityFrame.lifeTime = EditorGUILayout.FloatField("存在时间", entityFrame.lifeTime);
                                    entityFrame.entity.timelength = entityFrame.lifeTime;


                                    GameObject newEffectPrefab =
                                        (GameObject) EditorGUILayout.ObjectField("特效预制体:",
                                            entityFrame.entityPrefab, typeof(UnityEngine.GameObject), true);

                                    if (newEffectPrefab)
                                    {
                                        if (PrefabUtility.GetPrefabType(newEffectPrefab) == PrefabType.Prefab)
                                        {
                                            entityFrame.entityPrefab = newEffectPrefab;
                                            entityFrame.entityAsset = new DAssetObject(newEffectPrefab);
                                            entityFrame.entity.ObjectPrefab = newEffectPrefab;
                                        }
                                    }
                                    else
                                    {
                                        entityFrame.entityPrefab = null;
                                        entityFrame.entity.ObjectPrefab = null;
                                    }

                                    float speed, angle;

                                    Vector3 launchPos = new Vector3(entityFrame.emitposition.x,
                                        entityFrame.emitposition.y, entityFrame.emitPositionZ);
                                    launchPos = EditorGUILayout.Vector3Field("发射位置偏移:", launchPos);
                                    entityFrame.emitposition.x = launchPos.x;
                                    entityFrame.emitposition.y = launchPos.y;
                                    entityFrame.emitPositionZ = launchPos.z;

                                    speed = EditorGUILayout.FloatField("速度:", entityFrame.speed);
                                    if (speed != entityFrame.speed)
                                    {
                                        entityFrame.speed = speed;
                                    }

                                    if (entityFrame.axisType != AxisType.Y)
                                    {
                                        float realAngle = (entityFrame.angle / (2 * Mathf.PI) * Mathf.Rad2Deg) % 360;
                                        realAngle = EditorGUILayout.FloatField("角度:", realAngle);
                                        entityFrame.angle = realAngle * (2 * Mathf.PI) * Mathf.Deg2Rad;
                                    }
                                    else
                                    {
                                        entityFrame.angle = EditorGUILayout.FloatField("角度:", entityFrame.angle);
                                    }

                                    entityFrame.isAngleWithEffect =
                                        EditorGUILayout.Toggle("角度是否影响特效:", entityFrame.isAngleWithEffect);

                                    entityFrame.axisType =
                                        (AxisType) EditorGUILayout.EnumPopup("角度轴向", entityFrame.axisType, enumStyle);
                                    entityFrame.gravity = EditorGUILayout.Vector2Field("重力加速度:", entityFrame.gravity);


                                    projectileShakeToggle = EditorGUILayout.Foldout(projectileShakeToggle, "震动配置",
                                        EditorStyles.foldout);
                                    if (projectileShakeToggle)
                                    {
                                        EditorGUILayout.BeginVertical(rootGroupStyle);
                                        {
                                            //毫秒
                                            entityFrame.shockTime =
                                                EditorGUILayout.FloatField("目标震动时间:", entityFrame.shockTime);
                                            entityFrame.shockSpeed =
                                                EditorGUILayout.FloatField("目标震动速度:", entityFrame.shockSpeed);
                                            entityFrame.shockRangeX =
                                                EditorGUILayout.FloatField("目标震动X范围:", entityFrame.shockRangeX);
                                            entityFrame.shockRangeY =
                                                EditorGUILayout.FloatField("目标震动Y范围:", entityFrame.shockRangeY);

                                            //场景震动
                                            entityFrame.sceneShock.shockTime =
                                                EditorGUILayout.FloatField("屏幕震动时间:", entityFrame.sceneShock.shockTime);
                                            entityFrame.sceneShock.shockSpeed =
                                                EditorGUILayout.FloatField("屏幕震动速度:",
                                                    entityFrame.sceneShock.shockSpeed);
                                            entityFrame.sceneShock.shockRangeX = EditorGUILayout.FloatField("屏幕震动X范围:",
                                                entityFrame.sceneShock.shockRangeX);
                                            entityFrame.sceneShock.shockRangeY = EditorGUILayout.FloatField("屏幕震动Y范围:",
                                                entityFrame.sceneShock.shockRangeY);
                                        }
                                        EditorGUILayout.EndVertical();
                                    }

                                    entityFrame.hitFallUP = EditorGUILayout.Toggle("空中击中敌人是否有浮力:", entityFrame.hitFallUP > 0, toggleStyle)? 1: 0;
                                    //entityFrame.hitFallUP = EditorGUILayout.IntField("空中击中敌人是否有浮力:", entityFrame.hitFallUP);
                                    entityFrame.forceY = EditorGUILayout.FloatField("空中击中敌人是浮力大小:", entityFrame.forceY);

                                    //entityFrame.hurtID = EditorGUILayout.IntField("伤害效果表ID:", entityFrame.hurtID);
                                    entityFrame.hurtID = mAttackHurtEffectTable.OnGUIWithID(entityFrame.hurtID);

                                    EditorGUILayout.BeginHorizontal();
                                    entityFrame.hitCount = EditorGUILayout.IntField("攻击次数:", entityFrame.hitCount);
                                    entityFrame.attackCountExceedPlayExtDead =
                                        EditorGUILayout.Toggle("攻击次数达到是否播ExpDead",
                                            entityFrame.attackCountExceedPlayExtDead);
                                    EditorGUILayout.EndHorizontal();

                                    entityFrame.distance = EditorGUILayout.FloatField("最长移动距离：", entityFrame.distance);
                                    entityFrame.delayDead =
                                        EditorGUILayout.FloatField("延时删除时间：", entityFrame.delayDead);
                                    entityFrame.hitThrough =
                                        EditorGUILayout.Toggle("是否有贯穿：", entityFrame.hitThrough, toggleStyle);
                                    entityFrame.offsetType =
                                        (OffsetType) EditorGUILayout.EnumPopup("子弹偏移类型", entityFrame.offsetType,
                                            enumStyle);

                                    entityFrame.isRotation = EditorGUILayout.Toggle("是否绕点旋转", entityFrame.isRotation);
                                    if (entityFrame.isRotation)
                                    {
                                        EditorGUILayout.BeginVertical(rootGroupStyle);
                                        {
                                            entityFrame.rotateSpeed =
                                                EditorGUILayout.FloatField("旋转的速度:", entityFrame.rotateSpeed);
                                            entityFrame.moveSpeed =
                                                EditorGUILayout.FloatField("移动的速度:", entityFrame.moveSpeed);
                                            entityFrame.rotateInitDegree =
                                                EditorGUILayout.IntField("选择初始角度", entityFrame.rotateInitDegree);
                                        }
                                        EditorGUILayout.EndVertical();
                                    }


                                    entityFrame.hitGroundClick = EditorGUILayout.Toggle("是否触底弹一下：",
                                        entityFrame.hitGroundClick, toggleStyle);

                                    entityFrame.targetChooseType =
                                        GetChangeEnumType("选择目标类型:", entityFrame.targetChooseType);
                                    //entityFrame.targetChooseType =(TargetChooseType) EditorGUILayout.EnumPopup("目标选择类型", entityFrame.targetChooseType, enumStyle);
                                    if (entityFrame.targetChooseType == TargetChooseType.SMART_NEAREST)
                                    {
                                        entityFrame.range = EditorGUILayout.Vector2Field("自动判定范围", entityFrame.range);
                                    }
                                    else if (entityFrame.targetChooseType == TargetChooseType.PARABOLA_TARGET_POS)
                                    {
                                        entityFrame.paraSpeed =
                                            EditorGUILayout.FloatField("抛物线直线速度", entityFrame.paraSpeed);
                                        entityFrame.paraGravity =
                                            EditorGUILayout.FloatField("抛物线重力加速度", entityFrame.paraGravity);
                                    }
                                    else if (entityFrame.targetChooseType == TargetChooseType.BOOMERANGE)
                                    {
                                        entityFrame.range = EditorGUILayout.Vector2Field("自动判定范围", entityFrame.range);
                                        entityFrame.boomerangeDistance =
                                            EditorGUILayout.FloatField("回旋最大距离", entityFrame.boomerangeDistance);
                                        entityFrame.stayDuration =
                                            EditorGUILayout.FloatField("停留时间", entityFrame.stayDuration);
                                    }
                                    else if (entityFrame.targetChooseType == TargetChooseType.CHASE_TARGET)
                                    {
                                        entityFrame.offset = EditorGUILayout.Vector2Field("追踪偏移量", entityFrame.offset);
                                    }
                                    else if (entityFrame.targetChooseType == TargetChooseType.ROTATE_CHASE_TARGET)
                                    {
                                        entityFrame.changeMaxAngle = EditorGUILayout.FloatField("每帧转动角度", entityFrame.changeMaxAngle);
                                        entityFrame.chaseTime = EditorGUILayout.FloatField("追踪时长", entityFrame.chaseTime);
                                    }
                                    else if (entityFrame.targetChooseType == TargetChooseType.REBOUND)
                                    {
                                        entityFrame.chaseTime = EditorGUILayout.FloatField("最大反弹次数", entityFrame.chaseTime);
                                    }

                                    entityFrame.useRandomLaunch = EditorGUILayout.Toggle("是否使用随机发射机制",
                                        entityFrame.useRandomLaunch, toggleStyle);
                                    if (entityFrame.useRandomLaunch)
                                    {
                                        EditorGUILayout.BeginVertical(rootGroupStyle);
                                        {
                                            entityFrame.randomLaunchInfo.num =
                                                EditorGUILayout.IntField("随机个数", entityFrame.randomLaunchInfo.num);
                                            entityFrame.randomLaunchInfo.interval =
                                                EditorGUILayout.FloatField("随机间隔",
                                                    entityFrame.randomLaunchInfo.interval);
                                            entityFrame.randomLaunchInfo.rangeRadius =
                                                EditorGUILayout.FloatField("随机半径",
                                                    entityFrame.randomLaunchInfo.rangeRadius);

                                            entityFrame.randomLaunchInfo.isNumRand = EditorGUILayout.Toggle("是否随机随机个数",
                                                entityFrame.randomLaunchInfo.isNumRand, toggleStyle);
                                            if (entityFrame.randomLaunchInfo.isNumRand)
                                            {
                                                entityFrame.randomLaunchInfo.numRandRange.x =
                                                    EditorGUILayout.IntField("随机个数MIN",
                                                        (int) entityFrame.randomLaunchInfo.numRandRange.x);
                                                entityFrame.randomLaunchInfo.numRandRange.y =
                                                    EditorGUILayout.IntField("随机个数MAX",
                                                        (int) entityFrame.randomLaunchInfo.numRandRange.y);
                                            }

                                            entityFrame.randomLaunchInfo.isFullScene =
                                                EditorGUILayout.Toggle("是否全场景随机取点",
                                                    entityFrame.randomLaunchInfo.isFullScene);
                                        }
                                        EditorGUILayout.EndVertical();
                                    }

                                    entityFrame.onCollideDie = EditorGUILayout.Toggle(
                                        new GUIContent("接触被攻击者进入死亡状态", "播放死亡配置文件，造成伤害等操作"), entityFrame.onCollideDie);
                                    entityFrame.onXInBlockDie = EditorGUILayout.Toggle(
                                        new GUIContent("接触X轴阻挡进入死亡状态死亡", "播放死亡配置文件，造成伤害等操作"),
                                        entityFrame.onXInBlockDie);

                                    entityFrame.changForceBehindOther = EditorGUILayout.Toggle(
                                        new GUIContent("反向施力", "当子弹在受击者身后时将配置力反向"), entityFrame.changForceBehindOther);
                                    string[] stringArr = null;
                                    Array valueArr = null;
                                    if (stringArr == null)
                                    {
                                        stringArr = typeof(EntityFace).GetDescriptions();
                                        valueArr = typeof(EntityFace).GetValues();
                                    }

                                    int index = Array.BinarySearch(valueArr, (EntityFace) entityFrame.changeFace);
                                    index = EditorGUILayout.Popup("位置类型:", index, stringArr);
                                    if (index >= 0)
                                        entityFrame.changeFace = index;

                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginHorizontal();

                                    if (StyledButton("定位起始帧"))
                                    {
                                        animFrame = entityFrame.startFrames;
                                        AnimationSampler(skillData);
                                        return;
                                    }

                                    if (StyledButton("定为当前帧"))
                                    {
                                        entityFrame.startFrames = (int) animFrame;
                                        AnimationSampler(skillData);
                                        return;
                                    }

                                    if (
                                        proxyScript.skilleditmode == SkillSenceEditMode.EntityEdit
                                        &&
                                        proxyScript.currentEditObject == entityFrame
                                    )
                                    {
                                        if (StyledButton("终止编辑"))
                                        {
                                            proxyScript.currentEditObject = null;
                                            proxyScript.skilleditmode = SkillSenceEditMode.Normal;
                                            OnEditChangeOrSelectChange();
                                        }
                                    }
                                    else
                                    {
                                        if (StyledButton("编辑"))
                                        {
                                            proxyScript.skilleditmode = SkillSenceEditMode.EntityEdit;
                                            proxyScript.currentEditObject = entityFrame;
                                            if (Tools.current < Tool.Move || Tools.current > Tool.Scale)
                                            {
                                                Tools.current = Tool.Move;
                                            }

                                            OnEditChangeOrSelectChange();
                                        }
                                    }

                                    if (StyledButton("移除"))
                                    {
                                        entityFrame.entity.Destroy();
                                        skillData.entityFrames =
                                            RemoveElement<EntityFrames>(skillData.entityFrames, entityFrame);
                                        return;
                                    }

                                    if (StyledButton("复制实体"))
                                    {
                                        SkillDataUndo("Copy Entity");
                                        mCopyData = entityFrame;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 150;
                                EditorGUILayout.Space();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndVertical();
    }

    void ApplyEffectMirror(bool bMirror)
    {
        for (int i = 0; i < skillData.effectFrames.Length; ++i)
        {
            EffectsFrames effectFrames = skillData.effectFrames[i];

            if (effectFrames == null || effectFrames.effects == null)
            {
                continue;
            }

            effectFrames.effects.ApplyMirror(bMirror);
        }
    }

    bool IsMirror()
    {
        if (DSkillData.actor)
        {
            return DSkillData.actor.transform.localScale.x < 0;
        }

        return false;
    }

    void OnSampleGrapObj(DSkillData skillData)
    {
        var currentFrameNum = GetCurrentFrame();

        for (int i = 0; i < skillData.frameGrap.Length; ++i)
        {
            var frame = skillData.frameGrap[i];

            if (frame == null || frame.obj == null)
                continue;

            frame.obj.Sampler(0);
            frame.obj.Show(frame.startframe == currentFrameNum);
        }
    }

    void OnSamplerEffectFrames(DSkillData dSkillData)
    {
        effectnums = 0;
        for (int i = 0; i < dSkillData.effectFrames.Length; ++i)
        {
            if (!GetFoldeDict("effectFrames_toggle", i, true))
            {
                continue;
            }

            EffectsFrames effectFrames = dSkillData.effectFrames[i];

            if (effectFrames == null || effectFrames.effects == null)
            {
                continue;
            }


            float fBeginTime = GetFramesTime(effectFrames.startFrames);

            float fEndTime = fBeginTime + effectFrames.time;

            if (animTime < fBeginTime || animTime > fEndTime)
            {
                effectFrames.effects.Show(false);
            }
            else
            {
                if (effectFrames.effectGameObjectPrefeb == null &&
                    Utility.IsStringValid(effectFrames.effectAsset.m_AssetPath))
                {
                    effectFrames.effectGameObjectPrefeb =
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            "Assets/Resources/" + effectFrames.effectAsset.m_AssetPath + ".prefab");
                }

                effectnums++;
                if (effectFrames.localScale == Vector3.zero)
                {
                    effectFrames.localScale = Vector3.one;
                }

                effectFrames.AdjustScale();
                effectFrames.effects.UpdateParent(
                    GetAttachNodes(effectFrames.attachname));
                effectFrames.effects.UpdateData(effectFrames);

                bool bShow = effectFrames.effects.IsShow;
                effectFrames.effects.Show(true);
                if (bShow == false)
                {
                    effectFrames.effects.ApplyMirror(IsMirror());
                }

                float t = animTime - fBeginTime;
                effectFrames.effects.Sampler(t);
            }
        }
    }

    void OnSamplerAttachFrames(DSkillData skillData)
    {
        for (int i = 0; i < skillData.attachFrames.Length; ++i)
        {
            EntityAttachFrames atts = skillData.attachFrames[i];

            if (atts == null || atts.attach == null)
            {
                continue;
            }

            float fBeginTime = GetFramesTime(atts.start);

            float fEndTime = GetFramesTime(atts.end);

            if (animTime >= fBeginTime && animTime <= fEndTime)
            {
                float fTime = animTime - fBeginTime;

                //bool bUpdateAttach = false;
                if (atts.attach.Visiable == false)
                {
                    atts.attach.ObjectPrefab = atts.entityPrefab;
                    atts.attach.Show(true);
                    UpdateAttachNodes();
                    atts.attach.UpdateParent(
                        GetAttachNodes(atts.attachName));
                    //bUpdateAttach = true;
                }

                atts.attach.Sampler(fTime, skillData.fps);
            }
            else
            {
                bool bUpdateAttach = false;
                if (atts.attach.Visiable == true)
                {
                    bUpdateAttach = true;
                }

                atts.attach.Show(false);

                if (bUpdateAttach)
                    UpdateAttachNodes();
            }
        }
    }

    void OnSamplerEntityFrames(DSkillData skillData)
    {
        entitynums = 0;
        for (int i = 0; i < skillData.entityFrames.Length; ++i)
        {
            EntityFrames entityFrame = skillData.entityFrames[i];

            if (entityFrame == null || entityFrame.entity == null)
            {
                continue;
            }

            float fBeginTime = GetFramesTime(entityFrame.startFrames);

            float fEndTime = fBeginTime + entityFrame.entity.timelength;

            if (animTime >= fBeginTime && animTime <= fEndTime)
            {
                float fTime = animTime - fBeginTime;
                entityFrame.entity.ObjectPrefab = entityFrame.entityPrefab;
                entityFrame.entity.Sampler(fTime);
                entitynums++;
            }
            else
            {
                entityFrame.entity.Show(false);
            }
        }
    }

    void ClearGrabObjData()
    {
        if (GetCurSkillData() == null)
            return;
        for (int i = 0; i < GetCurSkillData().frameGrap.Length; ++i)
        {
            var frame = GetCurSkillData().frameGrap[i];
            if (frame == null || frame.obj == null)
                continue;
            frame.obj.Destroy();
        }
    }

    void ClearEntityFramesData()
    {
        for (int i = 0; i < GetCurSkillData().entityFrames.Length; ++i)
        {
            EntityFrames entityFrame = GetCurSkillData().entityFrames[i];

            if (entityFrame == null || entityFrame.entity == null)
            {
                continue;
            }

            entityFrame.entity.Destroy();
        }
    }

    void ClearAttachFramesData()
    {
        if (GetCurSkillData() == null)
        {
            return;
        }

        for (int i = 0; i < GetCurSkillData().attachFrames.Length; ++i)
        {
            EntityAttachFrames entityFrame = GetCurSkillData().attachFrames[i];

            if (entityFrame == null || entityFrame.attach == null)
            {
                continue;
            }

            entityFrame.attach.Destroy();
        }
    }

    bool OnSenceGUI_EntityEdit(DSkillDataEditorDrawer proxyScript)
    {
        if (proxyScript.skilleditmode == SkillSenceEditMode.EntityEdit)
        {
            if (proxyScript.currentEditObject != null)
            {
                EntityFrames ef = proxyScript.currentEditObject as EntityFrames;

                if (ef == null)
                {
                    return true;
                }

                DSkillEditor.DEditorEntity entity = ef.entity;

                if (Tools.current == Tool.Move)
                {
                    if (entity != null && entity.RootObject != null)
                    {
                        Vector3 position = new Vector3(ef.emitposition.x, ef.emitposition.y, 0.0f);
                        position = Handles.PositionHandle(position, Quaternion.identity);

                        if (
                            position.x != ef.emitposition.x
                            ||
                            position.y != ef.emitposition.y
                        )
                        {
                            SkillDataUndo("Entity");
                            ef.emitposition.x = position.x;
                            ef.emitposition.y = position.y;

                            OnSamplerEntityFrames(skillData);
                            EditorUtility.SetDirty(entity.RootObject);
                            SceneView.RepaintAll();
                        }
                    }
                }
                else if (Tools.current == Tool.Rotate)
                {
                    if (entity != null && entity.RootObject != null)
                    {
                        Vector3 position = new Vector3(ef.emitposition.x, ef.emitposition.y, 0.0f);
                        Quaternion q = Quaternion.Euler(new Vector3(0, 0, ef.angle));
                        Quaternion n = Handles.RotationHandle(q, position);

                        if (n != q)
                        {
                            SkillDataUndo("Entity");
                            Vector3 euler = n.eulerAngles;
                            ef.angle = euler.z;
                            OnSamplerEntityFrames(skillData);
                            EditorUtility.SetDirty(entity.RootObject);
                            SceneView.RepaintAll();
                        }
                    }
                }
            }

            return true;
        }

        return false;
    }

    /*
    DSFFrameEventTypes.Check();

                        int index = EditorGUILayout.Popup("FrameEvent：", 0, DSFFrameEventTypes.showList);
                        System.Object obj = Activator.CreateInstance(DSFFrameEventTypes.types[index].type);
                        */
    GUIStyle style;

    void OnGUIFramesEvent<T>(DSkillDataEditorDrawer proxyScript, ref T[] dataframe, string name, ref bool toggle,
        ref T[] copyDataFrame, Action<T> onRemoveCallBack = null)
        where T : DSkillFrameEvent, new()
    {
        //EntityFrames
        EditorGUILayout.BeginVertical(rootGroupStyle);
        {
            toggle = EditorGUILayout.Foldout(toggle, name + " (" + dataframe.Length + ")", EditorStyles.foldout);
            if (toggle && proxyScript)
            {
                EditorGUILayout.BeginVertical(subGroupStyle);
                {
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel += 1;
                    // DSFFrameEventTypes.Check();
                    // FrameEventSelected = EditorGUILayout.Popup("选择:", FrameEventSelected, DSFFrameEventTypes.showList, EditorStyles.popup);

                    EditorGUILayout.BeginHorizontal();

                    if (StyledButton("创建"))
                    {
                        DSkillFrameEvent frame = new T() as DSkillFrameEvent;
                        frame.name = name;
                        frame.startframe = (int) animFrame;
                        dataframe = AddElement<T>(dataframe, (T) frame);
                    }

                    if (StyledButton("清空"))
                    {
                        SkillDataUndo("Frame Clear");
                        dataframe = new T[0];
                    }

                    if (StyledButton("粘帖"))
                    {
                        SkillDataUndo("Frame Paste");
                        if (mCopyData is T)
                        {
                            DSkillFrameEvent frame = new T() as DSkillFrameEvent;
                            frame.copyFrameEvent(mCopyData as T);
                            dataframe = AddElement<T>(dataframe, (T)frame);
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //  if(copyDataFrame != null)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (typeof(T).IsAssignableFrom(typeof(DSkillFrameEffect)) ||
                            typeof(T).IsAssignableFrom(typeof(DSkillSfx)) ||
                            typeof(T).IsAssignableFrom(typeof(DSkillFrameEffect)) ||
                            typeof(T).IsAssignableFrom(typeof(DSkillBuff)) || 
                            typeof(T).IsAssignableFrom(typeof(DSkillSummon)) || 
                            typeof(T).IsAssignableFrom(typeof(DSkillMechanism)) || 
                            typeof(T).IsAssignableFrom(typeof(DSkillPropertyModify)))
                        {
                            if (StyledButton("跨文件复制"))
                            {
                                if (dataframe == null || dataframe.Length == 0)
                                {
                                }
                                else
                                {
                                    copyDataFrame = new T[dataframe.Length];
                                    for (int i = 0; i < dataframe.Length; ++i)
                                    {
                                        copyDataFrame[i] = new T();
                                        copyDataFrame[i].copyFrameEvent(dataframe[i]);
                                    }

                                    EditorUtility.SetDirty(mCopySkillData);
                                }
                            }

                            if (StyledButton("跨文件粘帖"))
                            {
                                if (copyDataFrame == null || copyDataFrame.Length == 0)
                                {
                                }
                                else
                                {
                                    dataframe = new T[copyDataFrame.Length];
                                    for (int i = 0; i < copyDataFrame.Length; ++i)
                                    {
                                        dataframe[i] = new T();
                                        dataframe[i].copyFrameEvent(copyDataFrame[i]);
                                    }

                                    EditorUtility.SetDirty(skillData);
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                    }

                    for (int i = 0; i < dataframe.Length; ++i)
                    {
                        DSkillFrameEvent frame = dataframe[i] as DSkillFrameEvent;

                        if (frame == null)
                        {
                            continue;
                        }

                        if (!SetFoldeDict(name, i, EditorGUILayout.Foldout(GetFoldeDict(name, i), frame.name)))
                        {
                            continue;
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical(arrayElementStyle);
                        {
                            EditorGUILayout.Space();
                            EditorGUIUtility.labelWidth = 250;
                            EditorGUILayout.BeginVertical(subArrayElementStyle);
                            {
                                int current = frame.startframe;
                                int endframes = frame.length;
                                endframes = frame.startframe + endframes;
                                endframes = Mathf.Min(endframes, skillData.totalFrames - 1);
                                int oldcurrent = current;
                                EditorGUI.indentLevel++;
                                StyledTimePeriodShow(frame.name, ref current, frame.startframe, endframes, 0,
                                    skillData.totalFrames - 1, EditorGUI.indentLevel);
                                EditorGUI.indentLevel--;

                                if (current != oldcurrent)
                                {
                                    int iNewstartFrames = current;

                                    if (iNewstartFrames >= 0 && iNewstartFrames < skillData.totalFrames - 1)
                                    {
                                        frame.startframe = iNewstartFrames;
                                        animFrame = current;
                                        AnimationSampler(skillData);
                                    }

                                    return;
                                }

                                string tname = EditorGUILayout.TextField("Name:", frame.name);
                                if (tname != frame.name)
                                {
                                    frame.name = tname;
                                    return;
                                }

                                frame.length = EditorGUILayout.IntField("Length:", frame.length);
//                                 frame.frameType = EditorGUILayout.IntField("Type:", frame.frameType);
//                                 //grap
//                                 frame.grapOp = EditorGUILayout.IntField("Grap Op:", frame.grapOp);
//                                 //state stack
//                                 frame.op = EditorGUILayout.IntField("state stack Op:", frame.op);
//                                 frame.state = EditorGUILayout.IntField("state stack state:", frame.state);
//                                 frame.idata1 = EditorGUILayout.IntField("state stack idata1:", frame.idata1);
//                                 frame.idata2 = EditorGUILayout.IntField("state stack idata2:", frame.idata2);
//                                 frame.fdata1 = EditorGUILayout.IntField("state stack fdata1:", frame.fdata1);
//                                 frame.fdata2 = EditorGUILayout.IntField("state stack fdata2:", frame.fdata2);
//                                 frame.statetag = EditorGUILayout.IntField("state stack statetag:", frame.statetag);
// 
//                                 frame.frameTag = EditorGUILayout.IntField("frameTag:", frame.frameTag);

                                OnFramesEventRoot(proxyScript, frame);

                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();

                                if (StyledButton("定位 "))
                                {
                                    return;
                                }

                                if (StyledButton("移除 "))
                                {
                                    SkillDataUndo("Frame Remove");
                                    dataframe = RemoveElement<T>(dataframe, (T) frame, onRemoveCallBack);
                                    return;
                                }

                                if (StyledButton("复制 "))
                                {
                                    SkillDataUndo("Frame Copy");
                                    mCopyData = frame;
                                    return;
                                }

                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.Space();
                            EditorGUIUtility.labelWidth = 150;
                            EditorGUILayout.Space();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndVertical();
    }

    public void OnFramesEventRoot(DSkillDataEditorDrawer proxyScript, DSkillFrameEvent data)
    {
        if (data is DSkillPropertyModify)
        {
            DSkillPropertyModify frame = data as DSkillPropertyModify;

            frame.tag = EditorGUILayout.IntField(new GUIContent("Tag:", "唯一标识"), frame.tag);
            SetPropertyModifyType(frame);
            frame.value = EditorGUILayout.FloatField("Value:", frame.value);
            frame.valueAcc = EditorGUILayout.FloatField("Value成长值:", frame.valueAcc);
            frame.jumpToTargetPos = EditorGUILayout.Toggle("是否跳到攻击对象的位置:", frame.jumpToTargetPos);
            frame.joyStickControl = EditorGUILayout.Toggle("是否用摇杆操控方向:", frame.joyStickControl);
            if (frame.joyStickControl)
            {   
                EditorGUILayout.BeginVertical(rootGroupStyle);
                
                frame.movedValue = EditorGUILayout.FloatField("按方向键的X轴Value:", frame.movedValue);
                frame.movedValueAcc = EditorGUILayout.FloatField("按方向键的X轴Value成长值:", frame.movedValueAcc);
                SetModifyXBackward(frame);
                frame.useMovedYValue = EditorGUILayout.Toggle("是否单独控制Y轴(默认根据X轴计算):", frame.useMovedYValue);
                if (frame.useMovedYValue)
                {
                    frame.movedYValue = EditorGUILayout.FloatField("按方向键的Y轴Value:", frame.movedYValue);
                    frame.movedYValueAcc = EditorGUILayout.FloatField("按方向键的Y轴Value成长值:", frame.movedYValueAcc);
                }

                frame.eachFrameModify =
                    EditorGUILayout.Toggle(new GUIContent("是否每帧响应操作:", "是否每帧响应操作:"), frame.eachFrameModify);


                EditorGUILayout.EndVertical();
                
            }
            else
            {
                frame.modifyXBackward = DModifyXBackward.NONE;
                frame.eachFrameModify = false;
            }
        }
        else if (data is DSkillFrameTag)
        {
            DSkillFrameTag frame = data as DSkillFrameTag;

            frame.tag = (DSFFrameTags) EditorGUILayout.EnumMaskPopup(
                new GUIContent("标签选择:",
                    "从上到下依次为：不生效" + "\r\n" + "                      全选" + "\r\n" +
                    "                      新的攻击判定，该帧标签重置改技能的攻击判定，一个技能多次伤害次数时使用；例：攻击对象第0帧到第30帧一直在攻击框内，第十帧设置NEWDAMAGE就第十帧会再次受到伤害" +
                    "\r\n" + "                      从该帧起锁定Z轴的位置，不受重力影响。若怪物在空中不会掉下来" + "\r\n" +
                    "                      解除Z轴锁定判定，恢复受重力影响" + "\r\n" + "                      其他待确定"), frame.tag);
            frame.tagFlag = EditorGUILayout.TextField(new GUIContent("帧标签标识内容:", "填入注释"), frame.tagFlag);
        }
        else if (data is DSkillFrameGrap)
        {
            DSkillFrameGrap frame = data as DSkillFrameGrap;
            SetGrabData(frame);
            if (frame.op == DSFGrapOp.GRAP_CHANGE_TARGETPOS)
            {
                frame.faceGraber = EditorGUILayout.Toggle(new GUIContent("是否朝向抓取者", "是否朝向抓取者"), frame.faceGraber);
                Vector3 tmp = new Vector3(frame.targetPos.x, frame.targetPos.y, frame.targetPos.z);
                tmp = EditorGUILayout.Vector3Field(new GUIContent("调整目标的坐标", "调整目标的坐标"), tmp);
                frame.targetPos.x = tmp.x;
                frame.targetPos.y = tmp.y;
                frame.targetPos.z = tmp.z;

                bool bOldToggleDict = GetFoldeDict("grapFrames_toggle", frame.startframe, true);
                bool bNewToggleDict = EditorGUILayout.Toggle("显示抓取点", bOldToggleDict);
                if (bNewToggleDict != bOldToggleDict)
                {
                    SetFoldeDict("grapFrames_toggle", frame.startframe, bNewToggleDict);

                    if (frame.obj != null)
                    {
                        frame.obj.visible = bNewToggleDict;
                        EditorGUIUtility.PingObject(frame.obj.RootObject);
                    }
                }
            }
            else if (frame.op == DSFGrapOp.GRAP_CHANGE_TARGETACTION)
            {
                frame.targetAction =
                    (ActionType) EditorGUILayout.EnumPopup(new GUIContent("目标动作:", "目标动作"), frame.targetAction);
            }
            else if (frame.op == DSFGrapOp.GRAP_CHANGE_TARGETROTATION)
            {
                frame.targetAngle = EditorGUILayout.IntField("目标角度", frame.targetAngle);
            }
        }
        else if (data is DSkillFrameStateOp)
        {
            DSkillFrameStateOp frame = data as DSkillFrameStateOp;
            frame.op = (DSFEntityStateOp) EditorGUILayout.EnumPopup("栈操作:", frame.op);
            frame.state = (DSFEntityStates) EditorGUILayout.EnumPopup("目标行为:", frame.state);
            frame.idata1 = EditorGUILayout.IntField("idata1", frame.idata1);
            frame.idata2 = EditorGUILayout.IntField("idata2", frame.idata2);
            frame.fdata1 = EditorGUILayout.FloatField("fdata1", frame.fdata1);
            frame.fdata2 = EditorGUILayout.FloatField("fdata2", frame.fdata2);
            frame.statetag = (DSFEntityStateTag) EditorGUILayout.EnumMaskField("Tags:", frame.statetag);
        }
        else if (data is DSkillFrameEventSceneShock)
        {
            DSkillFrameEventSceneShock frame = data as DSkillFrameEventSceneShock;
            frame.isNew = EditorGUILayout.Toggle("是否使用新版屏震效果", frame.isNew);
            if (frame.isNew)
            {
                frame.mode = EditorGUILayout.IntPopup("模式:", frame.mode, desc, mode);
                frame.time = EditorGUILayout.FloatField("时间:", frame.time);
                if (frame.mode == 5)
                {
                    frame.radius = EditorGUILayout.FloatField("随机震屏范围:", frame.radius);
                    frame.decelerate = EditorGUILayout.Toggle("振幅范围是否递减", frame.decelerate);
                }
                else
                {
                    frame.num = EditorGUILayout.IntField("震屏次数:", frame.num);
                    frame.xrange = EditorGUILayout.FloatField("X轴范围:", frame.xrange);
                    frame.yrange = EditorGUILayout.FloatField("Y轴范围:", frame.yrange);
                    frame.decelerate = EditorGUILayout.Toggle("振幅范围是否递减", frame.decelerate);
                    if (frame.decelerate)
                    {
                        frame.xreduce = EditorGUILayout.FloatField("X每次递减:", frame.xreduce);
                        frame.yreduce = EditorGUILayout.FloatField("Y每次递减:", frame.yreduce);
                    }
                }
            }
            else
            {
                frame.time = EditorGUILayout.FloatField("时间:", frame.time);
                frame.speed = EditorGUILayout.FloatField("速度:", frame.speed);
                frame.xrange = EditorGUILayout.FloatField("X轴范围:", frame.xrange);
                frame.yrange = EditorGUILayout.FloatField("Y轴范围:", frame.yrange);
            }
        }

        else if (data is DSkillSfx)
        {
            DSkillSfx frame = data as DSkillSfx;
            frame.soundClip = EditorGUILayout.ObjectField(new GUIContent("音效:", "声音文件位置（可拖动到此位置）"), frame.soundClip,
                typeof(UnityEngine.Object), true);
            frame.soundClipAsset = new DAssetObject(frame.soundClip);
            frame.soundID = EditorGUILayout.IntField(new GUIContent("音效ID", "对应音效资源表ID"), frame.soundID);
            frame.loop = EditorGUILayout.Toggle(new GUIContent("技能循环时是否循环", "技能循环时是否循环"), frame.loop, toggleStyle);
            frame.useActorSpeed = EditorGUILayout.Toggle(new GUIContent("音效是否受攻速影响", "音效是否受攻速影响"), frame.useActorSpeed);
            frame.phaseFinishDelete =
                EditorGUILayout.Toggle(new GUIContent("技能阶段结束删除", "技能阶段结束删除"), frame.phaseFinishDelete);
            frame.finishDelete = EditorGUILayout.Toggle(new GUIContent("技能中断或结束时删除", "技能中断或结束时删除"), frame.finishDelete);
            frame.volume = EditorGUILayout.FloatField(new GUIContent("音量(默认1，填浮点数)", "音量(默认1，填浮点数)"), frame.volume);
        }
        else if (data is DSkillFrameEffect)
        {
            DSkillFrameEffect frame = data as DSkillFrameEffect;
            //frame.effectID = EditorGUILayout.IntField("效果ID:", frame.effectID);
            frame.effectID = mAttackHurtEffectTable.OnGUIWithID(frame.effectID);
            frame.buffTime = EditorGUILayout.FloatField("buff时间(秒)", frame.buffTime);
            frame.phaseDelete = EditorGUILayout.Toggle("是否在阶段结束删除buff", frame.phaseDelete, toggleStyle);
            frame.finishDelete = EditorGUILayout.Toggle("是否在技能结束(中断)删除膝撞霸体buff", frame.finishDelete, toggleStyle);
            frame.finishDeleteAll = EditorGUILayout.Toggle("是否技能结束(中断)删除帧效果所有buff", frame.finishDeleteAll, toggleStyle);
            frame.useBuffAni = EditorGUILayout.Toggle("是否使用霸体动画", frame.useBuffAni, toggleStyle);
            frame.usePause = EditorGUILayout.Toggle("是否顿帧", frame.usePause, toggleStyle);
            frame.pauseTime = EditorGUILayout.FloatField("顿帧时间", frame.pauseTime);

            frame.mechanismId = mMechanismTable.OnGUIWithID(frame.mechanismId);

            //frame.mechanismId = EditorGUILayout.IntField(new GUIContent("机制ID", "机制ID"), frame.mechanismId);
        }
        else if (data is DSkillCameraMove)
        {
            DSkillCameraMove frame = data as DSkillCameraMove;
            frame.offset = EditorGUILayout.FloatField("偏移距离", frame.offset);
            frame.duraction = EditorGUILayout.FloatField("时间", frame.duraction);
        }
        else if (data is DSkillWalkControl)
        {
            DSkillWalkControl frame = data as DSkillWalkControl;
            SetMoveControlData(frame);
            frame.walkSpeedPercent = EditorGUILayout.FloatField("移动速度百分比", frame.walkSpeedPercent);
            frame.useSkillSpeed = EditorGUILayout.Toggle("是否使用技能表的速度百分比", frame.useSkillSpeed);
            if (frame.walkMode == SkillWalkMode.XYDiffRate)
            {
                frame.walkSpeedPercent2 = EditorGUILayout.FloatField(new GUIContent("移动速度百分比2", "当类型为XY速率不同时使用，不用就不填"),
                    frame.walkSpeedPercent2);
            }
        }
        else if (data is DActionData)
        {
            DActionData frame = data as DActionData;
            frame.actionType = (BeActionType) EditorGUILayout.EnumPopup("动画类型", frame.actionType);
            frame.duration = EditorGUILayout.FloatField("时间", frame.duration);
            if (frame.actionType == BeActionType.MoveBy ||
                frame.actionType == BeActionType.MoveTo)
            {
                frame.ignoreBlock = EditorGUILayout.Toggle("是否无视阻挡", frame.ignoreBlock);
                Vector3 tmp = new Vector3(frame.deltaPos.x, frame.deltaPos.y, frame.deltaPos.z);
                tmp = EditorGUILayout.Vector3Field("要移动的位置偏移", tmp);
                frame.deltaPos = new Vec3(tmp.x, tmp.y, tmp.z);
            }
            else if (frame.actionType == BeActionType.ScaleBy ||
                     frame.actionType == BeActionType.ScaleTo)
            {
                frame.deltaScale = EditorGUILayout.FloatField("缩放值", frame.deltaScale);
            }
        }
        else if (data is DSkillBuff)
        {
            DSkillBuff frame = data as DSkillBuff;

            frame.buffTime = EditorGUILayout.FloatField(new GUIContent("buff时间（秒）", "注释填写"), frame.buffTime);
            frame.phaseDelete =
                EditorGUILayout.Toggle(new GUIContent("该buff将在技能阶段结束后删除", "在技能阶段内buff时间为永久"), frame.phaseDelete);
            frame.finishDeleteAll = EditorGUILayout.Toggle(new GUIContent("该buff将在技能结束或取消后删除", "在技能结束前buff时间为永久"),
                frame.finishDeleteAll, toggleStyle);
            //frame.buffID = EditorGUILayout.IntField(new GUIContent("BuffID", "只能填一个"), frame.buffID);
            frame.buffID = mBuffTable.OnGUIWithID(frame.buffID);

            string buffInfoString = "";
            for (int i = 0; i < frame.buffInfoList.Length; ++i)
            {
                if (i != frame.buffInfoList.Length - 1)
                {
                    buffInfoString += frame.buffInfoList[i] + "|";
                }
                else
                {
                    buffInfoString += frame.buffInfoList[i];
                }
            }

            string buffInfoString2 =
                EditorGUILayout.TextField(new GUIContent("BUFF 信息表 ID", "可以填多个,以|分隔开"), buffInfoString);
            if (buffInfoString != buffInfoString2)
            {
                string[] buffInfo = buffInfoString2.Split('|');
                List<int> tempBuffInfoList = new List<int>();
                for (int i = 0; i < buffInfo.Length; ++i)
                {
                    int b;
                    if (int.TryParse(buffInfo[i], out b))
                    {
                        tempBuffInfoList.Add(b);
                    }
                }

                frame.buffInfoList = tempBuffInfoList.ToArray();
            }

            frame.level = EditorGUILayout.IntField(new GUIContent("buff等级:", ""), frame.level);
            frame.levelBySkill = EditorGUILayout.Toggle(new GUIContent("buff等级跟随技能等级", ""), frame.levelBySkill);
        }
        else if (data is DSkillSummon)
        {
            DSkillSummon frame = data as DSkillSummon;

            //summon
            frame.summonID = EditorGUILayout.IntField(new GUIContent("召唤ID", "注释填写"), frame.summonID);
            frame.summonLevel = EditorGUILayout.IntField(new GUIContent("召唤等级", ""), frame.summonLevel);
            frame.levelGrowBySkill = EditorGUILayout.Toggle(new GUIContent("召唤等级使用技能等级"), frame.levelGrowBySkill);
            frame.summonNum = EditorGUILayout.IntField(new GUIContent("一次召唤数量", ""), frame.summonNum);
            string[] stringArr = null;
            Array valueArr = null;
            if (stringArr == null)
            {
                stringArr = typeof(SummonPosType).GetDescriptions();
                valueArr = typeof(SummonPosType).GetValues();
            }

            int index = Array.BinarySearch(valueArr, (SummonPosType) frame.posType);
            index = EditorGUILayout.Popup("位置类型:", index, stringArr);
            if (index >= 0)
                frame.posType = index;
            string posType2string = "";
            if (frame.posType2 != null && frame.posType2.Length == 2)
            {
                posType2string = frame.posType2[0] + "|" + frame.posType2[1].ToString();
            }

            string posType2string2 =
                EditorGUILayout.TextField(new GUIContent("召唤位置类型2", "是否考虑阻挡 | 召唤位置类型距离,1：考虑阻挡\n2：不考虑阻挡"),
                    posType2string);
            if (posType2string != posType2string2)
            {
                string[] posType2 = posType2string2.Split('|');
                List<int> tempPosType2 = new List<int>();
                for (int i = 0; i < posType2.Length; ++i)
                {
                    int b;
                    if (int.TryParse(posType2[i], out b))
                    {
                        tempPosType2.Add(b);
                    }
                }

                frame.posType2 = tempPosType2.ToArray();
            }

            frame.isSameFace = EditorGUILayout.Toggle(new GUIContent("怪物朝向是否与召唤者一致", ""), frame.isSameFace);
            //frame.sumDistance = EditorGUILayout.IntField(new GUIContent("召唤距离:", "注释填写处"), frame.sumDistance);
        }
        else if (data is DSkillMechanism)
        {
            DSkillMechanism frame = data as DSkillMechanism;

            //frame.id = EditorGUILayout.IntField(new GUIContent("机制ID:", ""), frame.id);
            frame.id = mMechanismTable.OnGUIWithID(frame.id);

            frame.time = EditorGUILayout.FloatField(new GUIContent("机制时间(秒):", ""), frame.time);
            frame.level = EditorGUILayout.IntField(new GUIContent("机制等级:", ""), frame.level);
            frame.levelBySkill = EditorGUILayout.Toggle(new GUIContent("机制等级跟随技能等级", ""), frame.levelBySkill);
            frame.phaseDelete = EditorGUILayout.Toggle(new GUIContent("是否在技能阶段结束删除机制", ""), frame.phaseDelete);
            frame.finishDeleteAll = EditorGUILayout.Toggle(new GUIContent("是否在技能结束中断删除机制", ""), frame.finishDeleteAll);
        }
    }

    /// <summary>
    ///  TO DO: 作成异步的
    /// </summary>
    int m_ProcPercent = 0;

    const string kStateProcess = "升级资源中……[已完成：{0}%]";
    const string kStateNormal = "升级资源（时间可能会很长）";
    string m_kStateString = kStateNormal;
    const string kSkillDataRootPath = "Assets/Resources/Data/SkillData/";

    delegate void _UpdateFunc(DSkillData skillData);

    void _UpdateAsset(_UpdateFunc updateFunc, string path = kSkillDataRootPath)
    {
        m_kStateString = string.Format(kStateProcess, m_ProcPercent);
        string[] pathList = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
        for (int i = 0; i < pathList.Length; ++i)
        {
            DSkillData curSkillData = AssetDatabase.LoadAssetAtPath<DSkillData>(pathList[i]);
            if (curSkillData)
            {
                updateFunc(curSkillData);
                EditorUtility.SetDirty(curSkillData);
            }
        }

        AssetDatabase.SaveAssets();
        m_kStateString = kStateNormal;
    }

    void _UpdateFunc_AddDataChunk_DAssetObject(DSkillData skillDataUpdate)
    {
        if (skillDataUpdate.characterPrefab)
        {
            skillDataUpdate.characterAsset = new DAssetObject(skillDataUpdate.characterPrefab);
            //EditorUtility.SetDirty(skillDataUpdate.characterPrefab);
        }

        if (skillDataUpdate.goHitEffect)
        {
            skillDataUpdate.goHitEffectAsset = new DAssetObject(skillDataUpdate.goHitEffect);
            //EditorUtility.SetDirty(skillDataUpdate.goHitEffect);
        }

        foreach (EntityAttachFrames eaf in skillDataUpdate.attachFrames)
        {
            if (eaf.entityPrefab)
                eaf.entityAsset = new DAssetObject(eaf.entityPrefab);
            //EditorUtility.SetDirty(eaf.entityAsset.m_AssetObj);
        }

        foreach (EntityFrames ef in skillDataUpdate.entityFrames)
        {
            if (ef.entityPrefab)
                ef.entityAsset = new DAssetObject(ef.entityPrefab);
            //EditorUtility.SetDirty(ef.entityAsset.m_AssetObj);
        }

        foreach (EffectsFrames ef in skillDataUpdate.effectFrames)
        {
            if (ef.effectGameObjectPrefeb)
                ef.effectAsset = new DAssetObject(ef.effectGameObjectPrefeb);
            //EditorUtility.SetDirty(ef.effectAsset.m_AssetObj);
        }
    }

    //一键复制挂件
    protected void OneKeyReplaceAttach()
    {
        ArrayList list = new ArrayList();
        string fullPath = GetParentFilePath();
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DSkillData), UnityEditor.SelectionMode.Assets);
        DSkillData skillData = (DSkillData) selection[0];
        HeroGo.UtilityTools.GetSkillFiles(list, fullPath);
        UnityEngine.Object[] undoObjects = new UnityEngine.Object[list.Count - 1];
        for (int i = 0, j = 0; i < list.Count; ++i)
        {
            if (list[i].ToString() == selection[0].name)
                continue;
            string path = fullPath + "/" + list[i] + ".asset";
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(DSkillData));
            undoObjects[j++] = obj;
        }

        Undo.RecordObjects(undoObjects, "one-button copy entity");
        for (int j = 0; j < undoObjects.Length; ++j)
        {
            var data = undoObjects[j] as DSkillData;
            if (data != null)
            {
                for (int i = 0; i < skillData.attachFrames.Length; i++)
                {
                    EntityAttachFrames curAttachFrame = skillData.attachFrames[i];
                    EntityAttachFrames frame = new EntityAttachFrames();
                    frame.name = "[" + data.attachFrames.Length + "]";
                    frame.start = animFrame;
                    frame.end = data.totalFrames - 1;
                    frame.attachName = curAttachFrame.attachName;
                    frame.entityPrefab = curAttachFrame.entityPrefab;
                    frame.entityAsset = new DAssetObject(curAttachFrame.entityPrefab);
                    data.attachFrames = AddElement<EntityAttachFrames>(data.attachFrames, frame);
                    EditorUtility.SetDirty(undoObjects[j]);
                }
            }
        }

        AssetDatabase.SaveAssets();
    }

    //一键移除挂件
    protected void OneKeyRemoveAttach()
    {
        ArrayList list = new ArrayList();
        string fullPath = GetParentFilePath();
        HeroGo.UtilityTools.GetSkillFiles(list, fullPath);
        UnityEngine.Object[] undoObjects = new UnityEngine.Object[list.Count];
        for (int i = 0; i < list.Count; ++i)
        {
            string path = fullPath + "/" + list[i] + ".asset";
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(DSkillData));
            undoObjects[i] = obj;
        }

        Undo.RecordObjects(undoObjects, "one-button clear entity");
        for (int i = 0; i < undoObjects.Length; ++i)
        {
            var data = undoObjects[i] as DSkillData;
            if (data.attachFrames.Length > 0)
            {
                for (int j = 0; j < data.attachFrames.Length; ++j)
                {
                    EntityAttachFrames Frames = data.attachFrames[j];
                    Frames.attach.Destroy();
                }

                data.attachFrames = new EntityAttachFrames[0];
                EditorUtility.SetDirty(undoObjects[i]);
            }
        }

        AssetDatabase.SaveAssets();
    }

    //一键替换模型
    protected void OneKeyReplaceModel()
    {
        ArrayList list = new ArrayList();
        string fullPath = GetParentFilePath();
        HeroGo.UtilityTools.GetSkillFiles(list, fullPath);
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DSkillData), UnityEditor.SelectionMode.Assets);
        DSkillData curSkillData = (DSkillData) selection[0];
        foreach (var file in list)
        {
            string path = fullPath + "/" + file + ".asset";
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(DSkillData));
            var data = obj as DSkillData;
            data.characterPrefab = curSkillData.characterPrefab;
            data.characterAsset = new DAssetObject(curSkillData.characterPrefab);
            EditorUtility.SetDirty(obj);
        }

        AssetDatabase.SaveAssets();
    }


    //设置特效时间类型
    protected void SetEffectPlayType(EffectsFrames effect)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(EffectPlayType).GetDescriptions();
            valueArr = typeof(EffectPlayType).GetValues();
        }

        int index = Array.BinarySearch(valueArr, effect.playtype);
        index = EditorGUILayout.Popup("给定时间类型:", index, stringArr);
        if (index >= 0)
            effect.playtype = (EffectPlayType) valueArr.GetValue(index);
    }

    //设置生命周期类型
    protected void SetEffectTimeType(EffectsFrames effect)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(EffectTimeType).GetDescriptions();
            valueArr = typeof(EffectTimeType).GetValues();
        }

        int index = Array.BinarySearch(valueArr, effect.timetype);
        index = EditorGUILayout.Popup("特效生命周期类型:", index, stringArr);
        if (index >= 0)
            effect.timetype = (EffectTimeType) valueArr.GetValue(index);
    }

    //设置抓取的枚举数据
    protected void SetGrabData(DSkillFrameGrap frameGrap)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(DSFGrapOp).GetDescriptions();
            valueArr = typeof(DSFGrapOp).GetValues();
        }

        int index = Array.BinarySearch(valueArr, frameGrap.op);
        index = EditorGUILayout.Popup("操作:", index, stringArr);
        if (index >= 0)
            frameGrap.op = (DSFGrapOp) valueArr.GetValue(index);
    }

    //设置移动控制的枚举数据
    protected void SetMoveControlData(DSkillWalkControl frameWalk)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(SkillWalkMode).GetDescriptions();
            valueArr = typeof(SkillWalkMode).GetValues();
        }

        int index = Array.BinarySearch(valueArr, frameWalk.walkMode);
        index = EditorGUILayout.Popup("移动模式:", index, stringArr);
        if (index >= 0)
            frameWalk.walkMode = (SkillWalkMode) valueArr.GetValue(index);
    }

    //设置属性操作的类型
    protected void SetPropertyModifyType(DSkillPropertyModify modify)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(DSkillPropertyModifyType).GetDescriptions();
            valueArr = typeof(DSkillPropertyModifyType).GetValues();
        }

        int index = Array.BinarySearch(valueArr, modify.modifyfliter);
        index = EditorGUILayout.Popup("设置速度的方向:", index, stringArr);
        if (index >= 0)
            modify.modifyfliter = (DSkillPropertyModifyType) valueArr.GetValue(index);
    }

    //设置摇杆控制X轴方向的类型
    protected void SetModifyXBackward(DSkillPropertyModify modify)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(DModifyXBackward).GetDescriptions();
            valueArr = typeof(DModifyXBackward).GetValues();
        }

        int index = Array.BinarySearch(valueArr, modify.modifyXBackward);
        index = EditorGUILayout.Popup("摇杆指向X轴反方向:", index, stringArr);
        if (index >= 0)
            modify.modifyXBackward = (DModifyXBackward) valueArr.GetValue(index);
    }

    //设置下阶段触发方式类型
    protected void SetNextPhaseTriggerType(DSkillData data)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(TriggerNextPhaseType).GetDescriptions();
            valueArr = typeof(TriggerNextPhaseType).GetValues();
        }

        int index = Array.BinarySearch(valueArr, data.triggerType);
        index = EditorGUILayout.Popup("下阶段触发条件:", index, stringArr);
        if (index >= 0)
            data.triggerType = (TriggerNextPhaseType) valueArr.GetValue(index);
    }

    //设置摇杆类型
    protected void SetSkillJoystickType(DSkillData data)
    {
        string[] stringArr = null;
        Array valueArr = null;
        if (stringArr == null)
        {
            stringArr = typeof(SkillJoystickMode).GetDescriptions();
            valueArr = typeof(SkillJoystickMode).GetValues();
        }

        int index = Array.BinarySearch(valueArr, data.skillJoystickConfig.mode);
        index = EditorGUILayout.Popup("技能摇杆类型:", index, stringArr);
        if (index >= 0)
            data.skillJoystickConfig.mode = (SkillJoystickMode) valueArr.GetValue(index);
    }

    protected string GetParentFilePath()
    {
        UnityEngine.Object[] selection =
            Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
        if (selection.Length <= 0)
            return null;
        string fullPath = FileTools.GetAssetFullPath(selection[0]);
        return fullPath.Replace("/" + selection[0].name + ".asset", "");
    }

    private bool IsEditorPlaying()
    {
        return false;
        //return EditorApplication.isPlayingOrWillChangePlaymode;
    }

    public int GetIndexOfMask(DSFFrameTags tag)
    {
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_NEWDAMAGE))
            return 0;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_LOCKZSPEED))
            return 1;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_LOCKZSPEEDFREE))
            return 2;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_IGNORE_GRAVITY))
            return 3;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_RESTORE_GRAVITY))
            return 4;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_SET_TARGET_POS_XY))
            return 5;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_CURFRAME))
            return 6;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_CHANGEFACE))
            return 7;
        if (BeUtility.CheckHaveTag((int) tag, (int) DSFFrameTags.TAG_CHANGEFACEBYDIR))
            return 8;

        return 0;
    }

    public DSFFrameTags GetMaskOfIndex(int index)
    {
        if (index == 0)
            return DSFFrameTags.TAG_NEWDAMAGE;
        if (index == 1)
            return DSFFrameTags.TAG_LOCKZSPEED;
        if (index == 2)
            return DSFFrameTags.TAG_LOCKZSPEEDFREE;
        if (index == 3)
            return DSFFrameTags.TAG_IGNORE_GRAVITY;
        if (index == 4)
            return DSFFrameTags.TAG_RESTORE_GRAVITY;
        if (index == 5)
            return DSFFrameTags.TAG_SET_TARGET_POS_XY;
        if (index == 6)
            return DSFFrameTags.TAG_CURFRAME;
        if (index == 7)
            return DSFFrameTags.TAG_CHANGEFACE;
        if (index == 8)
            return DSFFrameTags.TAG_CHANGEFACEBYDIR;

        return DSFFrameTags.TAG_NEWDAMAGE;
    }
}


public class EffectCopyWizardDisplayWizard : ScriptableWizard
{
    public DSkillData objectToCopy = null;
    public DSkillData objectDest = null;

    public int numberOfCopies = 2;

    public static void CreateWindow(DSkillData dst)
    {
        // Creates the wizard for display
        EffectCopyWizardDisplayWizard wiz = (EffectCopyWizardDisplayWizard) ScriptableWizard.DisplayWizard(
            "Copy an Effect.",
            typeof(EffectCopyWizardDisplayWizard),
            "Copy!");

        wiz.objectDest = dst;
    }

    void OnWizardUpdate()
    {
        helpString = "Clones an object a number of times";
        if (objectToCopy == null)
        {
            errorString = "Please assign an object";
            isValid = false;
        }
        else
        {
            errorString = "";
            isValid = true;
        }
    }

    void OnWizardCreate()
    {
        if (objectDest == null || objectToCopy == null)
        {
            return;
        }

        List<EffectsFrames> effectframe = new List<EffectsFrames>();

        for (int i = 0; i < objectToCopy.effectFrames.Length; ++i)
        {
            EffectsFrames ef = new EffectsFrames();
            ef.Copy(objectToCopy.effectFrames[i]);
            effectframe.Add(ef);
        }

        objectDest.effectFrames = effectframe.ToArray();

        List<EntityAttachFrames> attachframe = new List<EntityAttachFrames>();
        for (int j = 0; j < objectToCopy.attachFrames.Length; ++j)
        {
            EntityAttachFrames af = new EntityAttachFrames();
            af.Copy(objectToCopy.attachFrames[j]);
            attachframe.Add(af);
        }

        objectDest.attachFrames = attachframe.ToArray();
    }
}