using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Tenmove.Editor.Unity
{
    public class SceneEffectiveAreaWindow : EditorWindow
    {
        const float CONST_CAMERA_SIZE = 2.35f;
        const float CONST_CAMERA_HEIGHT = 4.7f;
        const float CONST_CAMERA_WIDTH = 4.7f;
        const int CONST_RENDERTEXTURE_WIDTH = 940;
        const int CONST_RENDERTEXTURE_HEIGHT = 940;
        public const bool CONST_DEBUG = false;

        GameObject sceneRoot;
        Camera m_Camera;
        RenderTexture m_RenderTexture;
        List<SceneEffectiveAreaObject> m_SceneObjects = new List<SceneEffectiveAreaObject>();

        private SceneEffectiveAreaTreeView m_TreeView;
        private Vector2 scrollPos;
        private bool standalone;

        [MenuItem("[TM工具集]/场景工具/场景物体有效面积")]
        public static void CreateWindow()
        {
            SceneEffectiveAreaWindow window = GetWindow(typeof(SceneEffectiveAreaWindow), false, "统计场景物体有效面积") as SceneEffectiveAreaWindow;
            window.minSize = new Vector2(500, 800);
            window.standalone = true;
            window.Show();
        }

        private void OnEnable()
        {
            var treeViewHeaderState = SceneEffectiveAreaTreeView.DefaultHeaderState();
            MultiColumnHeader treeViewHeader = new MultiColumnHeader(treeViewHeaderState);
            m_TreeView = new SceneEffectiveAreaTreeView(treeViewHeader);
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (standalone)
            {
                sceneRoot = EditorGUILayout.ObjectField("场景根节点", sceneRoot, typeof(GameObject), true) as GameObject;
                if (GUILayout.Button("开始检查"))
                {
                    _StartAnalyse();
                }
            }
            m_TreeView.OnGUI(new Rect(0, standalone ? 36 : 0, position.width, position.height));
            EditorGUILayout.EndScrollView();
        }

        public static void AnalyseEffectiveArea(GameObject sceneRoot)
        {
            SceneEffectiveAreaWindow window = GetWindow(typeof(SceneEffectiveAreaWindow), false, "统计场景物体有效面积") as SceneEffectiveAreaWindow;
            window.minSize = new Vector2(500, 800);
            window.Show();
            window.sceneRoot = sceneRoot;
            window.standalone = false;
            window._StartAnalyse();
        }

        private void _StartAnalyse()
        {
            if (sceneRoot == null)
            {
                Debug.LogErrorFormat("场景根节点为空");
                return;
            }

            // 创建跟游戏中一致的相机
            m_Camera = _CreateCamera();
            m_RenderTexture = new RenderTexture(CONST_RENDERTEXTURE_WIDTH, CONST_RENDERTEXTURE_HEIGHT, 24, RenderTextureFormat.ARGB32);
            m_Camera.targetTexture = m_RenderTexture;

            // 1.0相机有旋转，这里把场景跟节点旋转
            Quaternion oldRotation = sceneRoot.transform.localRotation;
            sceneRoot.transform.localRotation = Quaternion.Euler(-20, 0, 0);

            // 收集场景中的Renderer
            _CollectSceneObjects(m_SceneObjects);

            if (m_SceneObjects.Count == 0)
            {
                Debug.LogError("没有场景物体");
                return;
            }

            // 统计每个物体的总面积
            _CaculateTotalArea(m_SceneObjects, m_Camera, m_RenderTexture);

            // 计算整个场景的AABB
            Bounds sceneBounds = _CaculateSceneBound(m_SceneObjects);
            if (sceneBounds.size.x == 0 || sceneBounds.size.y == 0)
            {
                Debug.LogErrorFormat("场景包围盒计算错误：{0}", sceneBounds);
                return;
            }
            // 统计场景正常渲染时每个物体的面积
            _CaculateEffectiveArea(m_SceneObjects, sceneBounds, m_Camera, m_RenderTexture);

            // 计算每个物体的利用率,重置场景物体的材质
            foreach (SceneEffectiveAreaObject sceneObject in m_SceneObjects)
            {
                sceneObject.effectiveRate = (float)sceneObject.effectiveArea / (float)sceneObject.totalArea;
                sceneObject.ResetMaterial();
            }

            m_SceneObjects.Sort((x, y) => { return x.effectiveRate.CompareTo(y.effectiveRate); });

            m_TreeView.SetData(m_SceneObjects);

            // Reset
            Object.DestroyImmediate(m_Camera.gameObject);
            Object.DestroyImmediate(m_RenderTexture);

            // 恢复场景根节点的旋转
            sceneRoot.transform.localRotation = oldRotation;
        }

        /// <summary>
        /// 收集场景物体
        /// </summary>
        /// <param name="sceneObjects"></param>
        private void _CollectSceneObjects(List<SceneEffectiveAreaObject> sceneObjects)
        {
            sceneObjects.Clear();
            // 收集所有SpriteRenderer
            Renderer[] renderers = sceneRoot.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogError("没有Renderer");
                return;
            }

            foreach (Renderer renderer in renderers)
            {
                // 只收集Enable的
                if(renderer.enabled)
                {
                    SceneEffectiveAreaObject sceneObject = new SceneEffectiveAreaObject(sceneObjects.Count + 1, renderer);
                    sceneObjects.Add(sceneObject);
                }
            }
        }

        /// <summary>
        /// 统计每个物体单独渲染时的总面积
        /// </summary>
        /// <param name="sceneObjects"></param>
        /// <param name="captureCamera"></param>
        private void _CaculateTotalArea(List<SceneEffectiveAreaObject> sceneObjects, Camera captureCamera, RenderTexture rt)
        {
            List<Vector3> cameraCapturePoses = new List<Vector3>();
            List<Texture2D> resultTexs = new List<Texture2D>();

            // 统计每个物体的总面积
            foreach (SceneEffectiveAreaObject sceneObject in sceneObjects)
            {
                foreach (SceneEffectiveAreaObject sceneObject2 in sceneObjects)
                {
                    if (sceneObject2 != null)
                        sceneObject2.EnableRenderer(false);
                }
                sceneObject.EnableRenderer(true);
                // 设置ObjectID
                sceneObject.SetObjectID2Material();

                Bounds curObjBounds = sceneObject.GetBounds();
                cameraCapturePoses.Clear();

                // 计算需要Capture的相机的位置
                _CaculateCapturePoses(cameraCapturePoses, curObjBounds);

                if (cameraCapturePoses.Count > resultTexs.Count)
                {
                    // 创建足够的Texture2D，保存结果
                    for (int i = 0, icnt = cameraCapturePoses.Count - resultTexs.Count; i < icnt; ++i)
                    {
                        resultTexs.Add(new Texture2D(CONST_RENDERTEXTURE_WIDTH, CONST_RENDERTEXTURE_HEIGHT, TextureFormat.ARGB32, false));
                    }
                }

                // Capture，然后把数据从GPU拷贝到CPU
                for (int i = 0; i < cameraCapturePoses.Count; ++i)
                {
                    captureCamera.transform.position = new Vector3(cameraCapturePoses[i].x, cameraCapturePoses[i].y, captureCamera.transform.position.z);
                    captureCamera.Render();
                    RenderTexture.active = rt;
                    resultTexs[i].ReadPixels(new Rect(0, 0, CONST_RENDERTEXTURE_WIDTH, CONST_RENDERTEXTURE_HEIGHT), 0, 0, false);
                    RenderTexture.active = null;
                }

                // 统计总面积
                for (int i = 0; i < cameraCapturePoses.Count; ++i)
                {
                    Color[] colors = resultTexs[i].GetPixels();
                    foreach (Color color in colors)
                    {
                        int id = SceneEffectiveAreaUtility.DecodeObjectID(color);
                        if (id == sceneObject.id)
                            sceneObject.totalArea++;
                    }
                }
            }

            foreach (Texture2D tex in resultTexs)
            {
                DestroyImmediate(tex);
            }
        }

        /// <summary>
        /// 统计正常渲染场景时的有效面积
        /// </summary>
        /// <param name="sceneObjects"></param>
        /// <param name="captureCamera"></param>
        private void _CaculateEffectiveArea(List<SceneEffectiveAreaObject> sceneObjects, Bounds sceneBounds, Camera captureCamera, RenderTexture rt)
        {
            List<Vector3> cameraCapturePoses = new List<Vector3>();
            List<Texture2D> resultTexs = new List<Texture2D>();

            // 如果场景大小超过了摄像机的范围，就需要Capture多次
            cameraCapturePoses.Clear();
            // 计算正常渲染场景时相机Capture的位置
            _CaculateCapturePoses(cameraCapturePoses, sceneBounds);


            // 创建足够的Texture2D，保存结果
            for (int i = 0, icnt = cameraCapturePoses.Count - resultTexs.Count; i < icnt; ++i)
            {
                resultTexs.Add(new Texture2D(CONST_RENDERTEXTURE_WIDTH, CONST_RENDERTEXTURE_HEIGHT, TextureFormat.ARGB32, false));
            }

            // 开启所有场景物体，设置自己的ID
            foreach (SceneEffectiveAreaObject sceneObject in sceneObjects)
            {
                sceneObject.EnableRenderer(true);
                sceneObject.SetObjectID2Material();
            }

            // Capture
            for (int i = 0; i < cameraCapturePoses.Count; ++i)
            {
                captureCamera.transform.position = new Vector3(cameraCapturePoses[i].x, cameraCapturePoses[i].y, captureCamera.transform.position.z);
                captureCamera.Render();
                RenderTexture.active = rt;
                resultTexs[i].ReadPixels(new Rect(0, 0, CONST_RENDERTEXTURE_WIDTH, CONST_RENDERTEXTURE_HEIGHT), 0, 0, false);
                RenderTexture.active = null;
            }

            // 统计场景正常渲染时，每个Object的面积
            for (int i = 0; i < cameraCapturePoses.Count; ++i)
            {
                Color[] colors = resultTexs[i].GetPixels();
                foreach (Color color in colors)
                {
                    int id = SceneEffectiveAreaUtility.DecodeObjectID(color);
                    // 物体的ID = (Index In m_SceneObjects) + 1
                    if (id > 0 && id <= sceneObjects.Count)
                    {
                        sceneObjects[id - 1].effectiveArea++;
                    }
                }
            }

            foreach (Texture2D tex in resultTexs)
            {
                DestroyImmediate(tex);
            }
        }

        /// <summary>
        /// 根据包围盒计算需要Capture的位置
        /// </summary>
        /// <param name="capturePoses"></param>
        /// <param name="bounds"></param>
        private void _CaculateCapturePoses(List<Vector3> capturePoses, Bounds bounds)
        {
            // 如果当前物体大小超过了摄像机的范围，就需要Capture多次
            if (bounds.extents.x * 2 > CONST_CAMERA_WIDTH
                || bounds.extents.y * 2 > CONST_CAMERA_HEIGHT)
            {
                // 计算需要检测的相机的位置
                int xCaptureTimes1 = Mathf.CeilToInt(bounds.extents.x * 2f / CONST_CAMERA_WIDTH);
                int yCaptureTimes1 = Mathf.CeilToInt(bounds.extents.y * 2f / CONST_CAMERA_HEIGHT);

                xCaptureTimes1 = Mathf.Max(1, xCaptureTimes1);
                yCaptureTimes1 = Mathf.Max(1, yCaptureTimes1);

                // 相机初始位置，场景包围盒的左下部分
                Vector3 cameraCaptureStartPos1 = new Vector3(bounds.min.x + CONST_CAMERA_WIDTH / 2,
                                                            bounds.min.y + CONST_CAMERA_HEIGHT / 2,
                                                            0);
                for (int i = 0; i < xCaptureTimes1; ++i)
                {
                    for (int j = 0; j < yCaptureTimes1; ++j)
                    {
                        capturePoses.Add(cameraCaptureStartPos1 + new Vector3(i * CONST_CAMERA_WIDTH, j * CONST_CAMERA_HEIGHT, 0));
                    }
                }
            }
            else
            {
                capturePoses.Add(new Vector3(bounds.center.x, bounds.center.y, 0));
            }
        }



        /// <summary>
        /// 计算整个场景的AABB
        /// </summary>
        /// <returns></returns>
        private Bounds _CaculateSceneBound(List<SceneEffectiveAreaObject> sceneObjects)
        {
            Bounds bounds = sceneObjects[0].GetBounds();
            foreach (SceneEffectiveAreaObject sceneObject in sceneObjects)
            {
                bounds.Encapsulate(sceneObject.GetBounds());
            }

            return bounds;
        }

        private Camera _CreateCamera()
        {
            GameObject cameraGO = new GameObject("SceneProfileCamera");
            cameraGO.transform.position = new Vector3(0, 0, -10);
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = CONST_CAMERA_SIZE;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.clear;
            camera.useOcclusionCulling = false;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.allowDynamicResolution = false;
            return camera;
        }
    }
}


