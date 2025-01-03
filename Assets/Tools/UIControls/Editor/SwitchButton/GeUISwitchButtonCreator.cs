using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEditor.UI
{

	public class GeUISwitchButtonCreator : MonoBehaviour
    {

        [MenuItem("GameObject/UI相关/SwitchButton", false)]
        static public void AddSwitchButton(MenuCommand menuCommand)
        {
            GameObject go = _CreateSwitchButton(_GetStandardResources());
            _PlaceUIElementRoot(go, menuCommand);
        }

        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";

        static DefaultControls.Resources s_StandardResources;
        static private DefaultControls.Resources _GetStandardResources()
        {
            if (s_StandardResources.standard == null)
            {
                s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
                s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
                s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
                s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
                s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
                s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
                s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
            }
            return s_StandardResources;
        }

        static protected GameObject _CreateSwitchButton(DefaultControls.Resources res)
        {
            int HEIGHT = 30;
            int WIDTH = 50;

            // Create GOs Hierarchy
            GameObject root = new GameObject("SwitchButton");
            RectTransform rectTransform = root.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(WIDTH, HEIGHT);

            GameObject background = _CreateUIObject("Background", root);
            GameObject fillArea = _CreateUIObject("Fill Area", root);
            GameObject fill = _CreateUIObject("Fill", fillArea);
            GameObject handleArea = _CreateUIObject("Handle Slide Area", root);
            GameObject handle = _CreateUIObject("Handle", handleArea);
            GameObject handleText = _CreateUIObject("Text", handle);

            // Background
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = res.background;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = Color.white;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-HEIGHT * 0.25f, 0);
            fillAreaRect.sizeDelta = new Vector2(-HEIGHT*0.5f, 0);

            // Fill
            Image fillImage = fill.AddComponent<Image>();
            fillImage.sprite = res.standard;
            fillImage.type = Image.Type.Sliced;
            fillImage.color = Color.white;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(HEIGHT * 0.5f, 0);
            fillRect.anchoredPosition = new Vector2(HEIGHT * 0.25f, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-HEIGHT, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            Image handleImage = handle.AddComponent<Image>();
            handleImage.sprite = res.knob;
            handleImage.color = Color.white;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(HEIGHT, 0);

            Text text = handleText.AddComponent<Text>();
            text.text = "Off";
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;

            // Setup slider component
            GeUISwitchButton switchBtn = root.AddComponent<GeUISwitchButton>();
            switchBtn.fillRect = fill.GetComponent<RectTransform>();
            switchBtn.handleRect = handle.GetComponent<RectTransform>();
            switchBtn.handleImage = handleImage;
            switchBtn.handleText = text;
            switchBtn.targetGraphic = handleImage;
            switchBtn.direction = GeUISwitchButton.Direction.LeftToRight;

            switchBtn.onImage = handleImage.sprite;
            switchBtn.offImage = handleImage.sprite;
            switchBtn.onText = "On";
            switchBtn.offText = "Off";

            ColorBlock colors = switchBtn.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);

            return root;
        }

        static GameObject _CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            _SetParentAndAlign(go, parent);
            return go;
        }

        private static void _SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            _SetLayerRecursively(child, parent.layer);
        }

        private static void _SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                _SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        private static void _PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = _GetOrCreateCanvasGameObject();
            }

            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            if (parent != menuCommand.context) // not a context click, so center in sceneview
                _SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

            Selection.activeGameObject = element;
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        static public GameObject _GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.

            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer("UI");
            canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            _CreateEventSystem(false);
            return root;
        }

        private static void _CreateEventSystem(bool select, GameObject parent = null)
        {
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }


        private static void _SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }
    }
}