using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Tenmove.Editor.Unity
{
    public class ShadowFramesEditor : EditorWindow
    {
        private class ActorInfo
        {
            public GameObject actor;
            public List<string> clips;
            public string framesName;
            public string frameKey;

            public ActorInfo(GameObject actorTemp, List<string> clipsTemp, string framesNameTemp, string frameKeyTemp)
            {
                actor = actorTemp;
                clips = clipsTemp;
                framesName = framesNameTemp;
                frameKey = frameKeyTemp;
            }
        }
        private int m_SingleSize = 256;
        private int m_FullSize = 1024;
        private string m_MarkCenterImage = "Assets/Resources/Shadow/MarkCenter.png";
        private List<ActorInfo> m_ActorList=new List<ActorInfo>();
        private GameObject m_ActorGameobject;
        private string m_UnityPath=null; //接受转成功后的路径 也就是Unity所需要的路径
        private float m_BlurSize = 0.6f;
        private int m_Interator = 4;

        //当前操作对象信息
        private string m_FramesName;
        private string m_FrameKey;
        private Animation m_Animation;
        private AnimationClip m_Clip;
        private Dictionary<string, AnimationClip> m_CurrentClipsDictionary = new Dictionary<string, AnimationClip>();
        private List<string> m_CurrentClipsNameList = new List<string>();
        private int m_ClipIndex;
        private GameObject m_Actor;
        //当前操作对象信息

        private Silhouette m_SilhouetteProperty;
        private GameObject m_CameraObject;
        private Camera m_CropCamera;
        private int m_CountNumber = 1;
        private bool m_Capture;
        private List<Color[]> m_ShadowTexs = new List<Color[]>();
        private FramesLookTable m_FrameTableInstance;
        private Texture2D m_Texture;
        private int m_totalFrame;
        private string[] m_preloadPlayerAnimations = { "Anim_Idle01", "Anim_Walk" };
        private string[] m_preloadNpcAnimations = { "Anim_Idle01" };
        [MenuItem("[TM工具集]/角色工具/角色动作阴影序列帧")]
        public static void CreateWindow()
        {
            ShadowFramesEditor window = GetWindow(typeof(ShadowFramesEditor), false, "角色动作阴影序列帧") as ShadowFramesEditor;
            window.minSize = new Vector2(500, 300);
            window.Show();
           
        }

        

        private void OnGUI()
        {
            if (m_UnityPath == null)
                m_UnityPath = Application.dataPath + "/Resources/Shadow/ShadowTextures";
            //if (m_Actor == null && m_ActorGameobject != null)
            //    _InitializeActor();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("加载录制信息"))
            {
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("保存录制信息"))
            {
                GUIUtility.ExitGUI();
            }
            
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("初始录制场景"))
                {
                    Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    newScene.name = "阴影序列帧录制场景";
                    m_CurrentClipsDictionary.Clear();
                    m_CurrentClipsNameList.Clear();
                    m_actorsInstantiates.Clear();
                    m_Clip = null;
                    _StartLoad();
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button("+录制角色"))
                {
                    m_ActorList.Add(new ActorInfo(null,new List<string>(),"","" ));
                    GUIUtility.ExitGUI();
                }

                if (GUILayout.Button("-录制角色"))
                {
                    if (m_ActorList.Count > 0)
                    {
                        m_ActorList.RemoveAt(m_ActorList.Count-1);
                    }
                    GameObject removingActor = null;
                    for (int i = 0; i < m_actorsInstantiates.Count; i++)
                    {
                        if (m_Actor.name.Contains(m_actorsInstantiates[i].name))
                        {
                            m_actorsInstantiates.RemoveAt(i);
                        }
                    }
                    foreach (Transform actorTemp in m_ActorsRoot.transform)
                    {
                        if (actorTemp.name.Contains(m_Actor.name))
                        {
                            removingActor = actorTemp.gameObject;
                        }
                    }
                    if (null != removingActor)
                    {
                        DestroyImmediate(removingActor);
                    }
                    GUIUtility.ExitGUI();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("选择阴影图存放文件夹"))
                {
                    m_UnityPath = EditorUtility.OpenFolderPanel("选择文件夹",
                        string.IsNullOrEmpty(m_UnityPath) ? "" : m_UnityPath, "");
                    if (m_UnityPath == null)
                    {
                        return;
                    }

                    GUIUtility.keyboardControl = 0;
                    GUIUtility.ExitGUI();
                }

                m_UnityPath = EditorGUILayout.TextField("", m_UnityPath, GUILayout.Width(position.width));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                m_BlurSize = EditorGUILayout.FloatField("模糊范围:", m_BlurSize);
                m_Interator = EditorGUILayout.IntField("迭代次数范围:", m_Interator);
            }
            if (EditorGUI.EndChangeCheck() && m_SilhouetteProperty != null)
            {
                m_SilhouetteProperty.m_BlurSize = m_BlurSize;
                m_SilhouetteProperty.m_Interator = m_Interator;

                EditorUtility.SetDirty(m_SilhouetteProperty.gameObject);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < m_ActorList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        m_ActorList[i].actor =
                            EditorGUILayout.ObjectField("", m_ActorList[i].actor, typeof(GameObject), true) as
                                GameObject;
                        if (m_ActorList[i].actor != null && GUILayout.Button(m_ActorList[i].actor.name))
                        {
                            m_ActorGameobject = m_ActorList[i].actor;
                            _InitializeActor();
                            SetActorInfo(m_ActorList[i]);
                            _InitializeClips();
                            GUIUtility.ExitGUI();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.Space(50);


                if (m_Actor == null )
                {
                    if ( m_ActorList.Count > 0 && m_ActorList[0].actor != null)
                    {
                        m_ActorGameobject = m_ActorList[0].actor;
                        _InitializeActor();
                        SetActorInfo(m_ActorList[0]);
                        _InitializeClips();
                    }
                    else
                    {
                        GUILayout.EndVertical();
                        return;
                    }
                }
                
                if (m_Animation != null )
                {
                    if (m_Animation.GetClipCount() == 0 && m_CurrentClipsDictionary.Count != 0)
                    {
                        foreach (var i in m_CurrentClipsDictionary)
                        {
                            m_Animation.AddClip(i.Value, i.Key);
                        }
                    }
                }
                m_FramesName = EditorGUILayout.TextField("帧名称(角色+动作):", m_FramesName);
                m_FrameKey = EditorGUILayout.TextField("键名称(对应角色):", m_FrameKey);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("+动作(从下方拖选框加)"))
                    {
                        string name = m_Clip.name;
                        if (!m_CurrentClipsNameList.Contains(name))
                        {
                            m_CurrentClipsDictionary.Add(name, m_Clip);
                            m_CurrentClipsNameList.Add(name);
                            m_Animation.AddClip(m_Clip, m_Clip.name);
                        }

                        GUIUtility.ExitGUI();
                    }

                    if (GUILayout.Button("-动作(从列表当前去除)"))
                    {
                        string name = m_CurrentClipsNameList[m_ClipIndex];
                        AnimationClip clip = m_CurrentClipsDictionary[name];
                        if (m_CurrentClipsNameList.Contains(name))
                        {
                            m_CurrentClipsDictionary.Remove(name);
                            m_CurrentClipsNameList.Remove(name);
                            m_Animation.RemoveClip(clip);
                            

                            m_Animation.clip = null;
                        }

                        GUIUtility.ExitGUI();
                    }
                    m_ClipIndex = EditorGUILayout.Popup("当前动画组件默认片段:", m_ClipIndex, m_CurrentClipsNameList.ToArray());
                    if ( m_CurrentClipsNameList.Count > m_ClipIndex && !m_BeingCircle)
                    {
                        string selectClipName = m_CurrentClipsNameList[m_ClipIndex];
                        if (m_CurrentClipsDictionary.ContainsKey(selectClipName))
                        {
                            AnimationClip selectClip = m_CurrentClipsDictionary[selectClipName];
                            m_Animation.clip = selectClip;
                            selectClip.SampleAnimation(m_Actor, 0);
                        }

                        if (m_Actor != null)
                            m_FramesName = m_FrameKey + "_" + selectClipName;
                    }
                }
                GUILayout.EndHorizontal();
                m_Clip = EditorGUILayout.ObjectField("动画:", m_Clip, typeof(AnimationClip), true) as AnimationClip;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("启动录制"))
                {
                    m_BeingCircle = true;
                    RecordClips();
                    m_ClipIndex = 0;
                    CirculationClips();
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button("启动录制所有"))
                {
                    GUIUtility.ExitGUI();
                }
                
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        private bool m_BeingCircle = false;
        private void RecordClips()
        {
            m_Clips.Clear();
            foreach (var value in m_CurrentClipsDictionary.Values)
            {
                m_Clips.Add(value);
            }
        }
        private List<AnimationClip> m_Clips = new List<AnimationClip>();
        //private int m_AnimationClipIndex = 0;
        private void CirculationClips()
        {
            if (m_ClipIndex != m_Clips.Count)
            {
                AnimationClip selectClip = m_Clips[ m_ClipIndex];
                m_Animation.clip = selectClip;
                selectClip.SampleAnimation(m_Actor, 0);
                m_CountNumber = 1;
                m_Capture = true;
            }
            else
            {
                m_ClipIndex = 0;
                _Save();
                m_BeingCircle = false;
            }
            
        }

        private void SetActorInfo(ActorInfo actorInfo)
        {
            if (m_Actor == null)
                return;
            m_Animation = m_Actor.GetComponent<Animation>();
            int prefix = m_Actor.name.LastIndexOf("_");
            if (actorInfo.frameKey == "")
            {
                m_FrameKey = m_ActorGameobject.name.Remove(0, prefix + 1);
            }
            else
            {
                m_FrameKey = actorInfo.frameKey;
            }
            m_FramesName = actorInfo.framesName;
            m_Animation.playAutomatically = false;
            m_CurrentClipsNameList = actorInfo.clips;
        }

        private void Update()
        {
            if (m_Capture)
            {
                _SampleAnimation();
                Repaint();
                _RenderToMultiPNG();
            }
        }

        private void _SampleAnimation()
        {
            float length = m_Animation.clip.length;
            float frameRate = m_Animation.clip.frameRate;
            m_totalFrame = (int)(length * frameRate + 1);
            if (m_CountNumber == 1)
            {
                m_ShadowTexs.Clear();
            }

            if (m_CountNumber < m_totalFrame)
            {                                      
                float time = m_CountNumber / frameRate;
                m_Animation.clip.SampleAnimation(m_Actor, time);
                m_CountNumber++;                                         
            }
            else
            {
                m_Capture = false;
                _GenerateCombineTex();
                m_ClipIndex++;
                CirculationClips();
            }
        }


        private void _GenerateCombineTex()
        {
            Texture2D texture2D = new Texture2D(m_FullSize, m_FullSize, TextureFormat.RGBA32, false);
            Color[] mainColors = new Color[texture2D.GetPixels().Length];
            int startIndex = 0;
            int colorChannel = 0;
            int offsetX = 0, offsetY = 0;
            int lineCountIndex = (m_FullSize / m_SingleSize) * 4;

            int Y = 0;
            for (int i = 0; i < m_ShadowTexs.Count; i++)
            {
                var tex = m_ShadowTexs[i];
                Color[] colors = tex;
                int column = 0, row = 0, indexX = 0, indexY = 0;
                try
                {
                    for (int j = 0; j < colors.Length; j++)
                    {
                        column = j % m_SingleSize;
                        row = j / m_SingleSize;
                        indexX = offsetX * m_SingleSize + column;
                        indexY = offsetY * (m_SingleSize - 1) * m_FullSize + row * m_FullSize;
                        mainColors[indexY + indexX][colorChannel] = colors[j].a;
                        Y = (indexY + indexX) / m_FullSize;
                    }
                }
                catch
                {
                    Debug.LogError("column, row, indexX, indexY" + "--" + column + "--" + row + "--" + indexX + "--" +
                                   indexY + "--" + (indexY + indexX));

                    texture2D.SetPixels(mainColors);
                    texture2D.Apply();
                    byte[] bytes1 = texture2D.EncodeToPNG();
                    File.WriteAllBytes(m_UnityPath + "//" + m_FramesName + ".png", bytes1);
                }

                startIndex++;
                colorChannel = startIndex % 4;
                if (startIndex % 4 == 0)
                    offsetX++;

                if (startIndex % lineCountIndex == 0)
                    offsetY++;
            }

            texture2D.SetPixels(mainColors);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            string tempPath = m_UnityPath + "//" + m_FrameKey + "_" + m_CurrentClipsNameList[m_ClipIndex]+ ".png";
            File.WriteAllBytes(tempPath, bytes);
            int prefix = tempPath.IndexOf("Assets");
            tempPath = tempPath.Remove(0, prefix);
            tempPath = tempPath.Replace("//", "/");
            _FormatTexture(tempPath);
            _RecordToLookTable(tempPath);
        }

        private void _RecordToLookTable(string path)
        {
            Debug.Log(path);
            m_Texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            m_FrameTableInstance = _InitFrameTable();
            m_FrameTableInstance._Add(m_FrameKey, m_CurrentClipsNameList[m_ClipIndex], m_Texture, m_totalFrame - 1);
        }

        private void _Save()
        {
            m_FrameTableInstance = _InitFrameTable();
            EditorUtility.SetDirty(m_FrameTableInstance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void _FormatTexture(string path)
        {
            
            TextureImporter texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (null != texImporter)
            {
                texImporter.mipmapEnabled = false;
                texImporter.crunchedCompression = true;
                texImporter.maxTextureSize= 256;
                texImporter.compressionQuality = 2;
                texImporter.textureCompression = TextureImporterCompression.Compressed;
                texImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(path);
            }
        }

        private void _StartLoad()
        {
            Camera m_MainCamera;
            RenderTexture m_RenderTexture;
            _InitializeCameras(out m_MainCamera, out m_RenderTexture);
            _InitializeUI(m_MainCamera, m_RenderTexture);

        }

        private void _InitializeClips()
        {
            if (null == m_Actor)
                return;
            var anims = m_Actor.GetComponent<GeAnimDescProxy>().animDescArray;
            List<string> preLoadAnims = m_preloadPlayerAnimations.ToList();
            m_CurrentClipsDictionary.Clear();
            m_CurrentClipsNameList.Clear();
            for (int i = 0; i < anims.Length; i++)
            {
                var tempAnim = anims[i];
                int preFix = anims[i].animClipPath.LastIndexOf('/');
                string name = anims[i].animClipPath.Remove(0, preFix + 1);
                int postFix = name.LastIndexOf('.');
                name = name.Remove(postFix, name.Length - postFix);
                if (preLoadAnims.Contains(name))
                {
                    var tempClip= AssetLoader.instance.LoadRes(tempAnim.animClipPath, typeof(AnimationClip)).obj as AnimationClip;
                    m_CurrentClipsNameList.Add(name);
                    m_Animation.AddClip(tempClip, name);
                    m_CurrentClipsDictionary.Add(name,tempClip);
                }
            }

        }

        private void _InitializeUI(Camera m_MainCamera, RenderTexture m_RenderTexture)
        {
            GameObject canvasObject = new GameObject();
            canvasObject.name = "Canvas";
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = m_MainCamera;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            Texture2D markCenter = (Texture2D) AssetDatabase.LoadAssetAtPath(m_MarkCenterImage, typeof(Texture2D));
            GameObject markObject = new GameObject();
            markObject.name = "MarkCenterBackground";
            markObject.transform.SetParent(GameObject.Find("Canvas").transform);
            RawImage m_MarkCenterUI = markObject.AddComponent<RawImage>();
            m_MarkCenterUI.GetComponent<RectTransform>().position = Vector2.zero;
            m_MarkCenterUI.texture = markCenter;
            m_MarkCenterUI.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            m_MarkCenterUI.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            m_MarkCenterUI.rectTransform.localScale = new Vector3(1, 1, 1);
            m_MarkCenterUI.rectTransform.pivot = new Vector2(1, 1);
            m_MarkCenterUI.rectTransform.sizeDelta = new Vector2(m_SingleSize * 2, m_SingleSize * 2);

            GameObject uiObject = new GameObject();
            uiObject.name = "UI";
            uiObject.transform.SetParent(GameObject.Find("Canvas").transform);
            RawImage m_PreviewUI = uiObject.AddComponent<RawImage>();
            m_PreviewUI.GetComponent<RectTransform>().position = Vector2.zero;
            m_PreviewUI.GetComponent<RectTransform>().sizeDelta = new Vector2(m_SingleSize * 2, m_SingleSize * 2);
            m_PreviewUI.texture = m_RenderTexture;
            m_PreviewUI.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            m_PreviewUI.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            m_PreviewUI.rectTransform.localScale = new Vector3(1, 1, 1);
            m_PreviewUI.rectTransform.pivot = new Vector2(1, 1);
            m_PreviewUI.rectTransform.sizeDelta = new Vector2(m_SingleSize * 2, m_SingleSize * 2);
        }

        private void _InitializeCameras(out Camera m_MainCamera, out RenderTexture m_RenderTexture)
        {
            GameObject CameraMain = new GameObject();
            CameraMain.name = "场景相机";
            m_MainCamera = CameraMain.AddComponent<Camera>();
            m_MainCamera.orthographic = true;
            m_MainCamera.clearFlags = CameraClearFlags.SolidColor;
            m_MainCamera.backgroundColor = new Color(0.16F, 0.43F, 0.35F, 0);
            CameraMain.transform.position = new Vector3(0.02f, 0.8f, -2.18f);
            m_MainCamera.orthographicSize = 1.3f;

            m_CameraObject = new GameObject();
            m_CameraObject.name = "录屏相机";
            
            m_CropCamera = m_CameraObject.AddComponent<Camera>();
            m_CropCamera.orthographic = true;
            m_CropCamera.clearFlags = CameraClearFlags.SolidColor;
            m_CropCamera.backgroundColor = new Color(0, 0, 0, 0);
            m_CropCamera.orthographicSize = 1.0f;
            m_CropCamera.transform.localPosition = new Vector3(0.0f, 0.96f, 1.03f);
            m_CropCamera.allowMSAA = false;
            m_CropCamera.allowHDR = false;
            m_CropCamera.transform.localEulerAngles = new Vector3(180, 0, 0);
            m_SilhouetteProperty = m_CameraObject.AddComponent<Silhouette>();
            m_RenderTexture = new RenderTexture(m_SingleSize, m_SingleSize, 32);
            m_CropCamera.targetTexture = m_RenderTexture;
            m_ActorsRoot = new GameObject("角色管理器");
            m_ActorsRoot.transform.position = Vector3.zero;
        }

        private GameObject m_ActorsRoot;
        private List<GameObject> m_actorsInstantiates = new List<GameObject>();
        private void _InitializeActor()
        {
            if (!m_actorsInstantiates.Contains(m_ActorGameobject))
            {
                m_Actor = Instantiate(m_ActorGameobject, Vector3.zero, Quaternion.identity);
                m_Actor.transform.SetParent(m_ActorsRoot.transform);
                m_actorsInstantiates.Add(m_ActorGameobject);
            }
            foreach (Transform actorTemp in m_ActorsRoot.transform)
            {
                if (actorTemp.name.Contains(m_ActorGameobject.name))
                {
                    actorTemp.gameObject.SetActive(true);
                    m_Actor = actorTemp.gameObject;
                }
                else
                {
                    actorTemp.gameObject.SetActive(false);
                }
            }
            
            
        }


        private void _RenderToMultiPNG()
        {
            RenderTexture renderTexture = new RenderTexture(m_SingleSize, m_SingleSize, 32);
            m_CropCamera.targetTexture = renderTexture;
            m_CropCamera.Render();
            RenderTexture.active = renderTexture;
            GameObject.Find("UI").GetComponent<RawImage>().texture = renderTexture;
            Texture2D texture2D = new Texture2D(m_SingleSize, m_SingleSize, TextureFormat.RGBA32, false);
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            m_ShadowTexs.Add(texture2D.GetPixels());
        }
        
        public  FramesLookTable _InitFrameTable()
        {
                if (!m_FrameTableInstance)
                {
                    // 如果为空，先试着从Resource中找到该对象
                    m_FrameTableInstance = Resources.Load<FramesLookTable>("Shadow/FramesLookTable");
                }
                if (!m_FrameTableInstance)
                {
                    // 如果仍然没有，就从默认状态中创建一个新的
                    m_FrameTableInstance = CreateDefaultGameState();
                    AssetDatabase.CreateAsset(m_FrameTableInstance, "Assets/Resources/Shadow/FramesLookTable.asset");
                    AssetDatabase.SaveAssets();
                }
                return m_FrameTableInstance;
        }

        private FramesLookTable CreateDefaultGameState()
        {
            return ScriptableObject.CreateInstance<FramesLookTable>();
        }
    }
}
