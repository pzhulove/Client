using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

namespace GameClient
{
    public struct BattleInputSetting
    {
        public static string LayoutType = "LayoutType";
        public static string ButtonScale = "ButtonScale";
        public static string ButtonInterval = "ButtonInterval";
        public static string ButtonOffsetX = "ButtonOffsetX";
        public static string ButtonOffsetY = "ButtonOffsetY";
        public static string JoystickScale = "JoystickScale";
        public static string JoystickOffsetX = "JoystickOffsetX";
        public static string JoystickOffsetY = "JoystickOffsetY";

        public static void SetInt(string key, int val)
        {
            PlayerPrefs.SetInt(key, val);
        }

        public static int GetInt(string key, int val)
        {
            val = PlayerPrefs.GetInt(key, val);
            return val;
        }

        public static void SetFloat(string key, float val)
        {
            PlayerPrefs.SetInt(key, Mathf.RoundToInt(val * 1000));
        }

        public static float GetFloat(string key, float val)
        {
            val = PlayerPrefs.GetInt(key, Mathf.RoundToInt(val * 1000)) / 1000f;
            return val;
        }
    }

    public class BattleInputFrame : ClientFrame
    {
        RectTransform loadRoot;
        Slider sliderInterval;
        Slider sliderScale;
        Text textButtonSize;
        ComDragPad dragPadButtons;
        ComDragPad dragPadJoystick;
        ImageEx mask1;
        ImageEx mask2;
        List<ComButtonEx> listButtonLayout = new List<ComButtonEx>();
        List<ImageEx> listImageLayout = new List<ImageEx>();
        ComButtonEx btnSelectJoystick;
        ComButtonEx btnSelectButtons;
        ComButtonEx btnEsc;
        ComButtonEx btnSave;
        ComButtonEx btnReset;

        Color32 colorMask = new Color32(0, 0, 0, 100);
        Vector3 joystickOriginPos;
        Vector3 buttonOriginPos;
        string spritePath0 = "UI/Image/NewPacked/Common.png:Common_Btn_05";
        string spritePath1 = "UI/Image/NewPacked/Common.png:Common_Btn_06";
        int notifyClose = 2232;
        int notifySave = 2233;
        Sprite modeSprite0;
        Sprite modeSprite1;
        GameObject objButtons;
        GameObject objJoystick;
        List<Transform> listButtonTran = new List<Transform>();
        float minButtonScale = 0.8f;
        float maxButtonScale = 1.2f;
        float minButtonInterval = 0.8f;
        float maxButtonInterval = 1.2f;
        float buttonScaleStep;
        float buttonIntervalStep;

        Transform curScaleTran;

        int curIndex = -1;
        float joystickScale = 1f;
        Vector3 joystickOffset = Vector3.zero;
        Vector3 buttonOffset = Vector3.zero;
        int layoutType = 0;
        float buttonScale = 1f;
        float buttonInterval = 1f;

        float joystickMinX = float.MinValue;
        float joystickMaxX = float.MaxValue;
        float joystickMinY = float.MinValue;
        float joystickMaxY = float.MaxValue;

        float buttonMinX = float.MinValue;
        float buttonMaxX = float.MaxValue;
        float buttonMinY = float.MinValue;
        float buttonMaxY = float.MaxValue;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleInputFrame/BattleInputFrame";
        }

        protected override void _bindExUI()
        {
            loadRoot = mBind.GetCom<RectTransform>("LoadRoot");
            sliderInterval = mBind.GetCom<Slider>("SliderInterval");
            if (sliderInterval != null)
            {
                sliderInterval.onValueChanged.AddListener(_onChangePadSize);
            }
            sliderScale = mBind.GetCom<Slider>("SliderScale");
            if (sliderScale != null)
            {
                sliderScale.onValueChanged.AddListener(_onChangeButtonSize);
            }
            textButtonSize = mBind.GetCom<Text>("TextButtonSize");
            dragPadButtons = mBind.GetCom<ComDragPad>("DragPadButtons");
            dragPadJoystick = mBind.GetCom<ComDragPad>("DragPadJoystick");
            mask1 = mBind.GetCom<ImageEx>("Mask1");
            mask2 = mBind.GetCom<ImageEx>("Mask2");

            listButtonLayout.Clear();
            listImageLayout.Clear();
            for (int i = 1; i <= 3; i++)
            {
                var button = mBind.GetCom<ComButtonEx>("ButtonLayout" + i.ToString());
                if (button != null)
                {
                    int index = i;
                    button.onClick.AddListener(() => { ChangeLayoutType(index); });
                    listButtonLayout.Add(button);
                }
                var image = mBind.GetCom<ImageEx>("ImageLayout" + i.ToString());
                listImageLayout.Add(image);
            }

            btnSelectJoystick = mBind.GetCom<ComButtonEx>("SelectJoystick");
            if (btnSelectJoystick != null)
            {
                btnSelectJoystick.onClick.AddListener(() => { SelectPad(0); });
            }
            btnSelectButtons = mBind.GetCom<ComButtonEx>("SelectButtons");
            if (btnSelectButtons != null)
            {
                btnSelectButtons.onClick.AddListener(() => { SelectPad(1); });
            }

            btnEsc = mBind.GetCom<ComButtonEx>("ButtonEsc");
            if (btnEsc != null)
            {
                btnEsc.onClick.AddListener(_onButtonEscClicked);
            }
            btnSave = mBind.GetCom<ComButtonEx>("ButtonSave");
            if (btnSave != null)
            {
                btnSave.onClick.AddListener(_onButtonSaveClicked);
            }
            btnReset = mBind.GetCom<ComButtonEx>("ButtonReset");
            if (btnReset != null)
            {
                btnReset.onClick.AddListener(_onButtonResetClicked);
            }
        }

        protected override void _unbindExUI()
        {
            if (sliderInterval != null)
            {
                sliderInterval.onValueChanged.RemoveAllListeners();
            }
            if (sliderScale != null)
            {
                sliderScale.onValueChanged.RemoveAllListeners();
            }
            for (int i = 0; i < listButtonLayout.Count; i++)
            {
                if (listButtonLayout[i] != null)
                {
                    listButtonLayout[i].onClick.RemoveAllListeners();
                }
            }
            if (btnSelectButtons != null)
            {
                btnSelectButtons.onClick.RemoveAllListeners();
            }
            if (btnSelectJoystick != null)
            {
                btnSelectJoystick.onClick.RemoveAllListeners();
            }
            if (btnEsc != null)
            {
                btnEsc.onClick.RemoveAllListeners();
            }
            if (btnSave != null)
            {
                btnSave.onClick.RemoveAllListeners();
            }
            if (btnReset != null)
            {
                btnReset.onClick.RemoveAllListeners();
            }

            listButtonLayout.Clear();
            listImageLayout.Clear();
            loadRoot = null;
            sliderInterval = null;
            sliderScale = null;
            textButtonSize = null;
            dragPadButtons = null;
            dragPadJoystick = null;
            mask1 = null;
            mask2 = null;
            btnSelectJoystick = null;
            btnSelectButtons = null;
            btnEsc = null;
            btnSave = null;
            btnReset = null;
        }

        protected override void _OnOpenFrame()
        {
            var assetHandle0 = AssetLoader.instance.LoadRes(spritePath0, typeof(Sprite));
            modeSprite0 = assetHandle0.obj as Sprite;
            var assetHandle1 = AssetLoader.instance.LoadRes(spritePath1, typeof(Sprite));
            modeSprite1 = assetHandle1.obj as Sprite;

            InitSliderStep();
            InitLocalLayout();
            LoadJoystick();
            LoadButtons();
            InitLimitArea();

            if (curIndex == -1)
            {
                SelectPad(1);
            }
        }

        protected override void _OnCloseFrame()
        {
            modeSprite0 = null;
            modeSprite1 = null;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        void _onButtonEscClicked()
        {
            if (CheckChangedLayout())
            {
                SystemNotifyManager.SystemNotifyOkCancel(notifyClose,
                () =>
                {
                    SaveData();
                    Close(true);
                },
                () =>
                {
                    Close(true);
                });
            }
            else
            {
                Close(true);
            }
        }

        void _onButtonSaveClicked()
        {
            SaveLayout();
        }

        void _onButtonResetClicked()
        {
            ResetLayout();
        }

        void _onChangeButtonSize(float value)
        {
            if (curIndex == 1)
            {
                buttonScale = value * buttonScaleStep + minButtonScale;
                _changeButtonSize();
            }
            else
            {
                joystickScale = value * buttonScaleStep + minButtonScale;
                if (curScaleTran != null)
                {
                    curScaleTran.localScale = Vector3.one * joystickScale;
                }
            }
        }

        void _changeButtonSize()
        {
            for (int i = 0; i < listButtonTran.Count; i++)
            {
                var tran = listButtonTran[i];
                if (tran != null)
                {
                    tran.localScale = Vector3.one * (buttonScale / objButtons.transform.localScale.x);
                }
            }
        }

        void _onChangePadSize(float value)
        {
            if (curIndex == 1)
            {
                buttonInterval = value * buttonIntervalStep + minButtonInterval;
                if (curScaleTran != null)
                {
                    curScaleTran.localScale = Vector3.one * buttonInterval;
                }
                _changeButtonSize();
            }
        }

        void ChangeLayoutType(int type)
        {
            layoutType = type;
            for (int i = 0; i < 3; i++)
            {
                if (i + 1 == type)
                {
                    listButtonLayout[i].interactable = false;
                    listImageLayout[i].sprite = modeSprite1;
                }
                else
                {
                    listButtonLayout[i].interactable = true;
                    listImageLayout[i].sprite = modeSprite0;
                }
            }

            LoadButtons();
        }

        void InitSliderStep()
        {
            buttonScaleStep = (maxButtonScale - minButtonScale) / 4;
            buttonIntervalStep = (maxButtonInterval - minButtonInterval) / 4;
        }

        void InitLocalLayout()
        {
            layoutType = BattleInputSetting.GetInt(BattleInputSetting.LayoutType, 1);
            buttonScale = BattleInputSetting.GetFloat(BattleInputSetting.ButtonScale, 1f);
            buttonInterval = BattleInputSetting.GetFloat(BattleInputSetting.ButtonInterval, 1f);
            buttonOffset.x = BattleInputSetting.GetFloat(BattleInputSetting.ButtonOffsetX, 0);
            buttonOffset.y = BattleInputSetting.GetFloat(BattleInputSetting.ButtonOffsetY, 0);
            joystickScale = BattleInputSetting.GetFloat(BattleInputSetting.JoystickScale, 1f);
            joystickOffset.x = BattleInputSetting.GetFloat(BattleInputSetting.JoystickOffsetX, 0);
            joystickOffset.y = BattleInputSetting.GetFloat(BattleInputSetting.JoystickOffsetY, 0);

            ChangeLayoutType(layoutType);
        }

        void LoadJoystick()
        {
            string path = "UIFlatten/Prefabs/ETCInput/ETCJoystickStatic";
            objJoystick = AssetLoader.instance.LoadResAsGameObject(path);
            if (objJoystick != null)
            {
                Utility.AttachTo(objJoystick, loadRoot.gameObject);

                objJoystick.transform.localScale = Vector3.one * joystickScale;
                var localPos = objJoystick.transform.localPosition;
                localPos.z = 0;
                objJoystick.transform.localPosition = localPos;
                joystickOriginPos = objJoystick.transform.position;
                var pos = joystickOriginPos + joystickOffset;
                if (dragPadJoystick != null)
                {
                    dragPadJoystick.SetDragTransform(objJoystick.transform as RectTransform);
                    dragPadJoystick.SetPosition(pos);
                }

                var joystick = objJoystick.GetComponent<ETCJoystick>();
                if (joystick != null)
                {
                    joystick.activated = false;
                }
            }
        }

        void LoadButtons()
        {
            if (objButtons != null)
            {
                Object.DestroyImmediate(objButtons);
            }

            string path = "UIFlatten/Prefabs/ETCInput/Layout/Type" + layoutType.ToString();
            objButtons = AssetLoader.instance.LoadResAsGameObject(path);

            if (objButtons != null)
            {
                Utility.AttachTo(objButtons, loadRoot.gameObject);

                objButtons.transform.localScale = Vector3.one * buttonInterval;
                var localPos = objButtons.transform.localPosition;
                localPos.z = 0;
                objButtons.transform.localPosition = localPos;
                buttonOriginPos = objButtons.transform.position;
                var pos = buttonOriginPos + buttonOffset;
                if (dragPadButtons != null)
                {
                    dragPadButtons.SetDragTransform(objButtons.transform as RectTransform);
                    dragPadButtons.SetPosition(pos);
                }

                if (curIndex == 1)
                {
                    curScaleTran = objButtons.transform;
                }

                var comSkillRoot = objButtons.GetComponent<ComInputSkillRoot>();
                if (comSkillRoot != null && comSkillRoot.ButtonRoot != null)
                {
                    listButtonTran.Clear();
                    var buttonRoot = Utility.FindGameObject(comSkillRoot.ButtonRoot, "ButtonRoot");
                    InitSkillButton(buttonRoot);
                }
            }
        }

        void InitSkillButton(GameObject buttonRoot)
        {
            if (buttonRoot != null)
            {
                for (int i = 0; i < buttonRoot.transform.childCount; ++i)
                {
                    var name = buttonRoot.transform.GetChild(i).name;
                    var child = Utility.FindGameObject(buttonRoot, name);
                    if (name.StartsWith("Btn_"))
                    {
                        listButtonTran.Add(child.transform);

                        var button = child.GetComponent<ETCButton>();
                        if (button != null)
                        {
                            button.activated = false;
                        }
                        var eventTrigger = child.GetComponent<EventTrigger>();
                        if (eventTrigger != null)
                        {
                            eventTrigger.enabled = false;
                        }
                    }
                }
            }

            _changeButtonSize();
        }

        void InitLimitArea()
        {
            var tran = frame.transform as RectTransform;
            if (tran != null)
            {
                joystickMinX = (tran.rect.xMin + 140) * 0.49f;
                joystickMinY = (tran.rect.yMin + 140) * 0.49f;

                buttonMaxX = (tran.rect.xMax + 20) * 0.49f;
                buttonMinY = (tran.rect.yMin - 20) * 0.49f;
            }

            var line1 = Utility.FindChild(frame, "LineRoot/Line1");
            if (line1 != null)
            {
                buttonMaxY = (line1.transform.position.y - 480) * 0.49f;
            }
            var line2 = Utility.FindChild(frame, "LineRoot/Line2");
            if (line2 != null)
            {
                buttonMinX = (line2.transform.position.x + 980) * 0.49f;
                joystickMaxX = (line2.transform.position.x - 350) * 0.49f;
            }
            var line3 = Utility.FindChild(frame, "LineRoot/Line3");
            if (line3 != null)
            {
                joystickMaxY = (line3.transform.position.y - 140) * 0.49f;
            }
        }

        void SelectPad(int index)
        {
            if (index == curIndex)
            {
                return;
            }
            curIndex = index;

            if (curIndex == 0)
            {
                dragPadJoystick.SetCanDrag(true);
                dragPadButtons.SetCanDrag(false);
                mask1.color = colorMask;
                mask2.color = Color.clear;
                curScaleTran = objJoystick.transform;
                sliderScale.value = Mathf.RoundToInt((curScaleTran.localScale.x - minButtonScale) / buttonScaleStep);
                sliderInterval.transform.parent.gameObject.CustomActive(false);
                if (textButtonSize != null)
                {
                    textButtonSize.text = "摇杆大小";
                }
            }
            else
            {
                dragPadJoystick.SetCanDrag(false);
                dragPadButtons.SetCanDrag(true);
                mask1.color = Color.clear;
                mask2.color = colorMask;
                curScaleTran = objButtons.transform;
                sliderScale.value = Mathf.RoundToInt((buttonScale - minButtonScale) / buttonScaleStep);
                sliderInterval.transform.parent.gameObject.CustomActive(true);
                sliderInterval.value = Mathf.RoundToInt((curScaleTran.localScale.x - minButtonInterval) / buttonIntervalStep);
                if (textButtonSize != null)
                {
                    textButtonSize.text = "按键大小";
                }
            }
        }

        void ResetLayout()
        {
            buttonScale = 1f;
            buttonInterval = 1f;
            joystickScale = 1f;
            joystickOffset = Vector3.zero;
            buttonOffset = Vector2.zero;
            if (dragPadJoystick != null)
            {
                dragPadJoystick.SetPosition(joystickOriginPos);
            }
            if (dragPadButtons != null)
            {
                dragPadButtons.SetPosition(buttonOriginPos);
            }
            sliderInterval.value = 2;
            sliderScale.value = 2;
            objJoystick.transform.localScale = Vector3.one;
            objButtons.transform.localScale = Vector3.one;
            _changeButtonSize();
            SaveData();
        }

        bool CheckChangedLayout()
        {
            if (layoutType != BattleInputSetting.GetInt(BattleInputSetting.LayoutType, 1) ||
                Mathf.RoundToInt(buttonScale * 1000) != BattleInputSetting.GetInt(BattleInputSetting.ButtonScale, 1000) ||
                Mathf.RoundToInt(buttonInterval * 1000) != BattleInputSetting.GetInt(BattleInputSetting.ButtonInterval, 1000) ||
                Mathf.RoundToInt(buttonOffset.x * 1000) != BattleInputSetting.GetInt(BattleInputSetting.ButtonOffsetX, 0) ||
                Mathf.RoundToInt(buttonOffset.y * 1000) != BattleInputSetting.GetInt(BattleInputSetting.ButtonOffsetY, 0) ||
                Mathf.RoundToInt(joystickScale * 1000) != BattleInputSetting.GetInt(BattleInputSetting.JoystickScale, 1000) ||
                Mathf.RoundToInt(joystickOffset.x * 1000) != BattleInputSetting.GetInt(BattleInputSetting.JoystickOffsetX, 0) ||
                Mathf.RoundToInt(joystickOffset.y * 1000) != BattleInputSetting.GetInt(BattleInputSetting.JoystickOffsetY, 0))
            {
                return true;
            }
            return false;
        }

        void SaveLayout()
        {
            SaveData();
            SystemNotifyManager.SystemNotify(notifySave);
        }

        void SaveData()
        {
            BattleInputSetting.SetInt(BattleInputSetting.LayoutType, layoutType);
            BattleInputSetting.SetFloat(BattleInputSetting.ButtonScale, buttonScale);
            BattleInputSetting.SetFloat(BattleInputSetting.ButtonInterval, buttonInterval);
            BattleInputSetting.SetFloat(BattleInputSetting.ButtonOffsetX, buttonOffset.x);
            BattleInputSetting.SetFloat(BattleInputSetting.ButtonOffsetY, buttonOffset.y);
            BattleInputSetting.SetFloat(BattleInputSetting.JoystickScale, joystickScale);
            BattleInputSetting.SetFloat(BattleInputSetting.JoystickOffsetX, joystickOffset.x);
            BattleInputSetting.SetFloat(BattleInputSetting.JoystickOffsetY, joystickOffset.y);
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (objJoystick != null && dragPadJoystick != null)
            {
                if (objJoystick.transform.position.x < joystickMinX ||
                    objJoystick.transform.position.x > joystickMaxX ||
                    objJoystick.transform.position.y < joystickMinY || 
                    objJoystick.transform.position.y > joystickMaxY)
                {
                    var pos = new Vector3(
                        Mathf.Clamp(objJoystick.transform.position.x, joystickMinX, joystickMaxX),
                        Mathf.Clamp(objJoystick.transform.position.y, joystickMinY, joystickMaxY),
                        objJoystick.transform.position.z);
                    dragPadJoystick.SetPosition(pos);
                }
                joystickOffset = objJoystick.transform.position - joystickOriginPos;
            }

            if (objButtons != null && dragPadButtons != null)
            {
                if (objButtons.transform.position.x < buttonMinX ||
                    objButtons.transform.position.x > buttonMaxX ||
                    objButtons.transform.position.y < buttonMinY ||
                    objButtons.transform.position.y > buttonMaxY)
                {
                    var pos = new Vector3(
                        Mathf.Clamp(objButtons.transform.position.x, buttonMinX, buttonMaxX),
                        Mathf.Clamp(objButtons.transform.position.y, buttonMinY, buttonMaxY),
                        objButtons.transform.position.z);
                    dragPadButtons.SetPosition(pos);
                }
                buttonOffset = objButtons.transform.position - buttonOriginPos;
            }
        }
    }
}