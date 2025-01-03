using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.UI.Extensions;

namespace GameClient
{

    public class ChallengeMapModelItem : MonoBehaviour
    {
        private const string teamDuplicationNormalTabImagePath = "UI/Image/NewPacked/Mijing.png:Mijing_Tab_02";

        private const string teamDuplicationSelectedTabImagePath =
            "UI/Image/NewPacked/Mijing.png:Mijing_Tab_01";


        private bool _isSelected = false;
        private ChallengeMapModelDataModel _challengeMapModelDataModel;
        private OnChallengeMapToggleClick _onChallengeMapToggleClick;

        [Space(10)]
        [HeaderAttribute("TabName")]
        [Space(10)]
        [SerializeField] private Text normalTabName;            //名字
        [SerializeField] private ComChangeColor nameComChangeColor;     //选中和默认的颜色
        [SerializeField] private NicerOutline nameNicerOutLine;     //字体的描边

        [Space(10)]
        [HeaderAttribute("ToggleName")]
        [Space(10)]
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image normalTabImage;
        [SerializeField] private Image selectedTabImage;

        private void Awake()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnTabClicked);
            }
        }

        private void ResetData()
        {
            _isSelected = false;
            _challengeMapModelDataModel = null;
            _onChallengeMapToggleClick = null;
        }

        private void OnDestroy()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveAllListeners();

            ResetData();
        }


        public void Init(ChallengeMapModelDataModel mapModelDataModel,
            OnChallengeMapToggleClick onChallengeMapToggleClick,
            bool isSelected = false)
        {
            //首先数据的重置
            ResetData();

            _challengeMapModelDataModel = mapModelDataModel;

            if (_challengeMapModelDataModel == null)
                return;

            _onChallengeMapToggleClick = onChallengeMapToggleClick;

            UpdateTabImage();       //针对团队本的页签进行更新

            if (normalTabName != null)
            {
                if (string.IsNullOrEmpty(_challengeMapModelDataModel.ToggleName) == false)
                    normalTabName.text = _challengeMapModelDataModel.ToggleName;
            }

            if (isSelected == true)
            {
                if (toggle != null)
                {
                    toggle.isOn = true;
                }
            }
        }

        private void UpdateTabImage()
        {
            if (_challengeMapModelDataModel.ModelType != DungeonModelTable.eType.TeamDuplicationModel)
                return;

            //图本页签特殊处理

            //字体默认和选中的颜色
            // Color normalColor;
            // ColorUtility.TryParseHtmlString("#988E88", out normalColor);

            // Color selectedColor;
            // ColorUtility.TryParseHtmlString("#eadba0", out selectedColor);

            // if (nameComChangeColor != null)
            // {
            //     nameComChangeColor.NormalColor = normalColor;
            //     nameComChangeColor.ClickColor = selectedColor;
            // }

            // if (normalTabName != null)
            // {
            //     normalTabName.color = normalColor;
            // }

            // //团本选中和默认的图片
            // if (normalTabImage != null)
            // {
            //     normalTabImage.type = Image.Type.Simple;
            //     ETCImageLoader.LoadSprite(ref normalTabImage, teamDuplicationNormalTabImagePath);
            //     var normalTabImageRtf = normalTabImage.transform as RectTransform;
            //     if (normalTabImageRtf != null)
            //         normalTabImageRtf.sizeDelta = new Vector2(257, 104);
            // }

            // if (selectedTabImage != null)
            // {
            //     ETCImageLoader.LoadSprite(ref selectedTabImage, teamDuplicationSelectedTabImagePath);
            //     var selectedTabImageRtf = selectedTabImage.transform as RectTransform;
            //     if (selectedTabImageRtf != null)
            //         selectedTabImageRtf.sizeDelta = new Vector3(257, 104);
            // }
        }

        private void OnTabClicked(bool value)
        {
            if (_challengeMapModelDataModel == null)
                return;

            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {
                if (_onChallengeMapToggleClick != null)
                {
                    _onChallengeMapToggleClick(_challengeMapModelDataModel.ModelType);
                }
            }
        }

    }
}
