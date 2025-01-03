using UnityEngine;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public static class DefaultControlsEx
    {
        public struct Resources
        {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
        }

        private const float  kWidth       = 160f;
        private const float  kThickHeight = 30f;
        private const float  kThinHeight  = 20f;
        private static Vector2 s_ThickElementSize       = new Vector2(kWidth, kThickHeight);
        private static Vector2 s_ThinElementSize        = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageElementSize       = new Vector2(100f, 100f);
        private static Color   s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static Color   s_PanelColor             = new Color(1f, 1f, 1f, 0.392f);
        private static Color   s_TextColor              = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        // Helper methods at top

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;

            // Reset() is not called when playing. We still want the default font to be assigned
            //lbl.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            lbl.font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/UIFlatten/Font/PFCT.ttf");
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor     = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor    = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        // Actual controls

        public static GameObject CreatePanel(Resources resources)
        {
            GameObject panelRoot = CreateUIElementRoot("Panel", s_ThickElementSize);

            // Set RectTransform to stretch
            RectTransform rectTransform = panelRoot.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            ImageEx image = panelRoot.AddComponent<ImageEx>();
            image.sprite = resources.background;
            image.type = ImageEx.Type.Sliced;
            image.color = s_PanelColor;

            return panelRoot;
        }

        public static GameObject CreateButton(Resources resources)
        {
            GameObject buttonRoot = CreateUIElementRoot("ButtonEx", s_ThickElementSize);

            GameObject childText = new GameObject("TextEx");
            childText.AddComponent<RectTransform>();
            SetParentAndAlign(childText, buttonRoot);

            ImageEx image = buttonRoot.AddComponent<ImageEx>();
            image.sprite = resources.standard;
            image.type = ImageEx.Type.Sliced;
            image.color = s_DefaultSelectableColor;
            image.raycastTarget = true;
            
            ButtonEx bt = buttonRoot.AddComponent<ButtonEx>();
            SetDefaultColorTransitionValues(bt);

            Text text = childText.AddComponent<TextEx>();
            text.text = "Button";
            text.alignment = TextAnchor.MiddleCenter;
            text.supportRichText = false;
            text.raycastTarget = false;
            SetDefaultTextValues(text);

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return buttonRoot;
        }

        public static GameObject CreateText(Resources resources)
        {
            GameObject go = CreateUIElementRoot("TextEx", s_ThickElementSize);

            Text lbl = go.AddComponent<TextEx>();
            lbl.supportRichText = false;
            lbl.raycastTarget = false;
            lbl.text = "New Text";
            SetDefaultTextValues(lbl);

            return go;
        }

        public static GameObject CreateImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("ImageEx", s_ImageElementSize);
            var image = go.AddComponent<ImageEx>();
            image.raycastTarget = false;
            return go;
        }

        public static GameObject CreateNullImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("NullImage", s_ImageElementSize);
            go.AddComponent<NullImage>();
            return go;
        }

        public static GameObject CreateRawImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("RawImage", s_ImageElementSize);
            var image = go.AddComponent<RawImage>();
            image.raycastTarget = false;
            return go;
        }

        public static GameObject CreateSlider(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject root = CreateUIElementRoot("Slider", s_ThinElementSize);

            GameObject background = CreateUIObject("Background", root);
            GameObject fillArea = CreateUIObject("Fill Area", root);
            GameObject fill = CreateUIObject("Fill", fillArea);
            GameObject handleArea = CreateUIObject("Handle Slide Area", root);
            GameObject handle = CreateUIObject("Handle", handleArea);

            // Background
            ImageEx backgroundImage = background.AddComponent<ImageEx>();
            backgroundImage.sprite = resources.background;
            backgroundImage.type = ImageEx.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            ImageEx fillImage = fill.AddComponent<ImageEx>();
            fillImage.sprite = resources.standard;
            fillImage.type = ImageEx.Type.Sliced;
            fillImage.color = s_DefaultSelectableColor;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            ImageEx handleImage = handle.AddComponent<ImageEx>();
            handleImage.sprite = resources.knob;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup slider component
            Slider slider = root.AddComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);

            return root;
        }

        public static GameObject CreateScrollbar(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElementRoot("Scrollbar", s_ThinElementSize);

            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot);
            GameObject handle = CreateUIObject("Handle", sliderArea);

            ImageEx bgImage = scrollbarRoot.AddComponent<ImageEx>();
            bgImage.sprite = resources.background;
            bgImage.type = ImageEx.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            ImageEx handleImage = handle.AddComponent<ImageEx>();
            handleImage.sprite = resources.standard;
            handleImage.type = ImageEx.Type.Sliced;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            Scrollbar scrollbar = scrollbarRoot.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

            return scrollbarRoot;
        }

        public static GameObject CreateToggle(Resources resources)
        {
            // Set up hierarchy
            GameObject toggleRoot = CreateUIElementRoot("Toggle", s_ThinElementSize);

            GameObject background = CreateUIObject("Background", toggleRoot);
            GameObject checkmark = CreateUIObject("Checkmark", background);
            GameObject childLabel = CreateUIObject("Label", toggleRoot);

            // Set up components
            Toggle toggle = toggleRoot.AddComponent<ToggleEx>();
            toggle.isOn = true;

            ImageEx bgImage = background.AddComponent<ImageEx>();
            bgImage.sprite = resources.standard;
            bgImage.type = ImageEx.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            ImageEx checkmarkImage = checkmark.AddComponent<ImageEx>();
            checkmarkImage.sprite = resources.checkmark;

            Text label = childLabel.AddComponent<TextEx>();
            label.text = "Toggle";
            label.raycastTarget = false;
            label.supportRichText = false;

            SetDefaultTextValues(label);

            toggle.graphic = checkmarkImage;
            toggle.targetGraphic = bgImage;
            SetDefaultColorTransitionValues(toggle);

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin        = new Vector2(0f, 1f);
            bgRect.anchorMax        = new Vector2(0f, 1f);
            bgRect.anchoredPosition = new Vector2(10f, -10f);
            bgRect.sizeDelta        = new Vector2(kThinHeight, kThinHeight);

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(20f, 20f);

            RectTransform labelRect = childLabel.GetComponent<RectTransform>();
            labelRect.anchorMin     = new Vector2(0f, 0f);
            labelRect.anchorMax     = new Vector2(1f, 1f);
            labelRect.offsetMin     = new Vector2(23f, 1f);
            labelRect.offsetMax     = new Vector2(-5f, -2f);

            return toggleRoot;
        }

        public static GameObject CreateInputField(Resources resources)
        {
            GameObject root = CreateUIElementRoot("InputField", s_ThickElementSize);

            GameObject childPlaceholder = CreateUIObject("Placeholder", root);
            GameObject childText = CreateUIObject("Text", root);

            ImageEx image = root.AddComponent<ImageEx>();
            image.sprite = resources.inputField;
            image.type = ImageEx.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            InputField inputField = root.AddComponent<InputField>();
            SetDefaultColorTransitionValues(inputField);

            Text text = childText.AddComponent<TextEx>();
            text.text = "";
            text.supportRichText = false;
            SetDefaultTextValues(text);

            Text placeholder = childPlaceholder.AddComponent<TextEx>();
            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyle.Italic;
            // Make placeholder color half as opaque as normal text color.
            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -7);

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = new Vector2(10, 6);
            placeholderRectTransform.offsetMax = new Vector2(-10, -7);

            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            return root;
        }

        public static GameObject CreateDropdown(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Dropdown", s_ThickElementSize);

            GameObject label = CreateUIObject("Label", root);
            GameObject arrow = CreateUIObject("Arrow", root);
            GameObject template = CreateUIObject("Template", root);
            GameObject viewport = CreateUIObject("Viewport", template);
            GameObject content = CreateUIObject("Content", viewport);
            GameObject item = CreateUIObject("Item", content);
            GameObject itemBackground = CreateUIObject("Item Background", item);
            GameObject itemCheckmark = CreateUIObject("Item Checkmark", item);
            GameObject itemLabel = CreateUIObject("Item Label", item);

            // Sub controls.

            GameObject scrollbar = CreateScrollbar(resources);
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);

            Scrollbar scrollbarScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollbarScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);

            RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup item UI components.

            Text itemLabelText = itemLabel.AddComponent<TextEx>();
            SetDefaultTextValues(itemLabelText);
            itemLabelText.alignment = TextAnchor.MiddleLeft;

            ImageEx itemBackgroundImage = itemBackground.AddComponent<ImageEx>();
            itemBackgroundImage.color = new Color32(245, 245, 245, 255);

            ImageEx itemCheckmarkImage = itemCheckmark.AddComponent<ImageEx>();
            itemCheckmarkImage.sprite = resources.checkmark;

            Toggle itemToggle = item.AddComponent<ToggleEx>();
            itemToggle.targetGraphic = itemBackgroundImage;
            itemToggle.graphic = itemCheckmarkImage;
            itemToggle.isOn = true;

            // Setup template UI components.

            ImageEx templateImage = template.AddComponent<ImageEx>();
            templateImage.sprite = resources.standard;
            templateImage.type = ImageEx.Type.Sliced;

            ScrollRect templateScrollRect = template.AddComponent<ScrollRect>();
            templateScrollRect.content = (RectTransform)content.transform;
            templateScrollRect.viewport = (RectTransform)viewport.transform;
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;

            Mask scrollRectMask = viewport.AddComponent<MaskEx>();
            scrollRectMask.showMaskGraphic = false;

            ImageEx viewportImage = viewport.AddComponent<ImageEx>();
            viewportImage.sprite = resources.mask;
            viewportImage.type = ImageEx.Type.Sliced;

            // Setup dropdown UI components.

            Text labelText = label.AddComponent<TextEx>();
            SetDefaultTextValues(labelText);
            labelText.alignment = TextAnchor.MiddleLeft;

            ImageEx arrowImage = arrow.AddComponent<ImageEx>();
            arrowImage.sprite = resources.dropdown;

            ImageEx backgroundImage = root.AddComponent<ImageEx>();
            backgroundImage.sprite = resources.standard;
            backgroundImage.color = s_DefaultSelectableColor;
            backgroundImage.type = ImageEx.Type.Sliced;

            Dropdown dropdown = root.AddComponent<Dropdown>();
            dropdown.targetGraphic = backgroundImage;
            SetDefaultColorTransitionValues(dropdown);
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = labelText;
            dropdown.itemText = itemLabelText;

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.options.Add(new Dropdown.OptionData {text = "Option A"});
            dropdown.options.Add(new Dropdown.OptionData {text = "Option B"});
            dropdown.options.Add(new Dropdown.OptionData {text = "Option C"});
            dropdown.RefreshShownValue();

            // Set up RectTransforms.

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin           = Vector2.zero;
            labelRT.anchorMax           = Vector2.one;
            labelRT.offsetMin           = new Vector2(10, 6);
            labelRT.offsetMax           = new Vector2(-25, -7);

            RectTransform arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin           = new Vector2(1, 0.5f);
            arrowRT.anchorMax           = new Vector2(1, 0.5f);
            arrowRT.sizeDelta           = new Vector2(20, 20);
            arrowRT.anchoredPosition    = new Vector2(-15, 0);

            RectTransform templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin        = new Vector2(0, 0);
            templateRT.anchorMax        = new Vector2(1, 0);
            templateRT.pivot            = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 2);
            templateRT.sizeDelta        = new Vector2(0, 150);

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin        = new Vector2(0, 0);
            viewportRT.anchorMax        = new Vector2(1, 1);
            viewportRT.sizeDelta        = new Vector2(-18, 0);
            viewportRT.pivot            = new Vector2(0, 1);

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin         = new Vector2(0f, 1);
            contentRT.anchorMax         = new Vector2(1f, 1);
            contentRT.pivot             = new Vector2(0.5f, 1);
            contentRT.anchoredPosition  = new Vector2(0, 0);
            contentRT.sizeDelta         = new Vector2(0, 28);

            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin            = new Vector2(0, 0.5f);
            itemRT.anchorMax            = new Vector2(1, 0.5f);
            itemRT.sizeDelta            = new Vector2(0, 20);

            RectTransform itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin  = Vector2.zero;
            itemBackgroundRT.anchorMax  = Vector2.one;
            itemBackgroundRT.sizeDelta  = Vector2.zero;

            RectTransform itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin   = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax   = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta   = new Vector2(20, 20);
            itemCheckmarkRT.anchoredPosition = new Vector2(10, 0);

            RectTransform itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin       = Vector2.zero;
            itemLabelRT.anchorMax       = Vector2.one;
            itemLabelRT.offsetMin       = new Vector2(20, 1);
            itemLabelRT.offsetMax       = new Vector2(-10, -2);

            template.SetActive(false);

            return root;
        }

        public static GameObject CreateScrollView(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Scroll View", new Vector2(200, 200));

            GameObject viewport = CreateUIObject("Viewport", root);
            GameObject content = CreateUIObject("Content", viewport);

            // Sub controls.

            GameObject hScrollbar = CreateScrollbar(resources);
            hScrollbar.name = "Scrollbar Horizontal";
            SetParentAndAlign(hScrollbar, root);
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar(resources);
            vScrollbar.name = "Scrollbar Vertical";
            SetParentAndAlign(vScrollbar, root);
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup RectTransforms.

            // Make viewport fill entire scroll view.
            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            // Make context match viewpoprt width and be somewhat taller.
            // This will show the vertical scrollbar and not the horizontal one.
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;

            // Setup UI components.

            ScrollRect scrollRect = root.AddComponent<ScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = viewportRT;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;

            ImageEx rootImage = root.AddComponent<ImageEx>();
            rootImage.sprite = resources.background;
            rootImage.type = ImageEx.Type.Sliced;
            rootImage.color = s_PanelColor;

            // 将内置的Mask换成RectMask2D，并去掉Image组件
            RectMask2D viewportMask = viewport.AddComponent<RectMask2D>();

/*
            ImageEx viewportImage = viewport.AddComponent<ImageEx>();
            viewportImage.sprite = resources.mask;
            viewportImage.type = ImageEx.Type.Sliced;*/

            return root;
        }
    }
}
