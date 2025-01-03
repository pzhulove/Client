using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ComControlView : MonoBehaviour
    {

        [SerializeField] private Button closeButton;

        //通用Toggle的测试数据
        private ComControlData _comToggleData;
        private List<ComControlData> _comToggleDataList = new List<ComControlData>();

        //通用下拉单的测试数据
        private ComControlData _dropDownData;
        private List<ComControlData> _dropDownDataList = new List<ComControlData>();


        //二级Toggle的的测试数据
        private List<ComTwoLevelToggleData> _comTwoLevelToggleDataList = new List<ComTwoLevelToggleData>();

        //二级Toggle的的测试数据
        private List<ComTwoLevelToggleData> _comTwoLevelToggleDataListWithExample = new List<ComTwoLevelToggleData>();

        [Space(10)]
        [HeaderAttribute("ComToggleControl")]
        [Space(5)]
        [SerializeField]
        private ComToggleControl comToggleControl;

        [Space(10)]
        [HeaderAttribute("ComTwoLevelToggleControl")]
        [Space(5)]
        [SerializeField] private ComTwoLevelToggleControl comTwoLevelToggleControl;

        [Space(10)]
        [HeaderAttribute("ComTwoLevelToggleControlWithExample")]
        [Space(5)]
        [SerializeField] private ComTwoLevelToggleControl comTwoLevelToggleControlWithExample;

        [Space(10)]
        [HeaderAttribute("ComDropDownControl")]
        [Space(5)]
        [SerializeField] private ComDropDownControl comDropDownControl;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }
        }

        private void UnBindEvents()
        {
            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            if (_comTwoLevelToggleDataList != null)
            {
                for (var i = 0; i < _comTwoLevelToggleDataList.Count; i++)
                {
                    if(_comTwoLevelToggleDataList[i].SecondLevelToggleDataList != null)
                        _comTwoLevelToggleDataList[i].SecondLevelToggleDataList.Clear();
                }
                _comTwoLevelToggleDataList.Clear();
            }

            if (_comTwoLevelToggleDataListWithExample != null)
            {
                for (var i = 0; i < _comTwoLevelToggleDataListWithExample.Count; i++)
                {
                    if (_comTwoLevelToggleDataListWithExample[i].SecondLevelToggleDataList != null)
                        _comTwoLevelToggleDataListWithExample[i].SecondLevelToggleDataList.Clear();
                }
                _comTwoLevelToggleDataListWithExample.Clear();
            }

            if (_comToggleDataList != null)
                _comToggleDataList.Clear();

            _comToggleData = null;
        }

        public void InitView()
        {
            //InitTestComToggle();
            //InitTestDropDown();
            //InitTestTwoLevelToggle();
            InitTestTwoLevelToggleWithExample();
        }
        
        #region Test ComToggle

        private void InitTestComToggle()
        {
            for (var i = 0; i < 10; i++)
            {
                var testLevelData = new TestLevelData
                {
                    Id = i + 1,
                    Name = "CT_" + (i + 1),
                    IsSelected = false,
                    LevelName = "LV_" + (i + 1),
                };
                testLevelData.IsSelected = i == 2 ? true : false;
                _comToggleDataList.Add(testLevelData);
            }

            if (comToggleControl != null
                && _comToggleDataList != null && _comToggleDataList.Count > 0)
            {
                comToggleControl.InitComToggleControl(_comToggleDataList,
                    OnTestToggleClick);
            }

        }

        private void OnTestToggleClick(ComControlData comToggleData)
        {
            if (comToggleData == null)
                return;

            if (_comToggleData != null && _comToggleData.Id == comToggleData.Id)
                return;

            _comToggleData = comToggleData;

            var testLevelData = _comToggleData as TestLevelData;
            if (testLevelData != null)
            {
                Logger.LogErrorFormat("OnComToggleClick and id is {0}, name is {1}, levelName is {2}", testLevelData.Id,
                    testLevelData.Name, testLevelData.LevelName);
            }
        }


        #endregion

        #region Test DropDownControl

        private void InitTestDropDown()
        {
            for (var i = 0; i < 10; i++)
            {
                var testDropDownData = new TestDropDownData
                {
                    Id = i + 1,
                    Name = "DD_" + (i + 1),
                    DropDownName = "NN_" + (i + 2),
                };
                _dropDownDataList.Add(testDropDownData);
            }

            var defaultDropDownData = _dropDownDataList[8];
            if (comDropDownControl != null)
            {
                comDropDownControl.InitComDropDownControl(defaultDropDownData,
                    _dropDownDataList,
                    OnDropDownItemClicked);
            }

        }

        private void OnDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            if (_dropDownData != null && _dropDownData.Id == comControlData.Id)
                return;

            _dropDownData = comControlData;

            var testDropDownData = _dropDownData as TestDropDownData;
            if (testDropDownData != null)
            {
                Logger.LogErrorFormat("The selected DropDownData id is {0}, name is {1}, dropDownName is {2}",
                    testDropDownData.Id, testDropDownData.Name, testDropDownData.DropDownName);
            }

        }

        #endregion

        #region TestTwoLevelToggle
        //测试二级Toggle
        private void InitTestTwoLevelToggle()
        {
            for (var i = 0; i < 5; i++)
            {
                ComTwoLevelToggleData twoLevelToggleData = new ComTwoLevelToggleData();

                var comToggleData = new ComControlData
                {
                    Id = i + 1,
                    Name = "FL_" + (i + 1),
                    IsSelected = false,
                };
                comToggleData.IsSelected = i == 1 ? true : false;

                twoLevelToggleData.FirstLevelToggleData = comToggleData;

                var comSecondToggleDataList = new List<ComControlData>();
                var secondToggleNumber = i;
                for (var j = 0; j < secondToggleNumber; j++)
                {
                    var secondToggleData = new ComControlData
                    {
                        Id = j + 1,
                        Name = "SL_" + (j + 1),
                        IsSelected = false,
                    };
                    secondToggleData.IsSelected = j == 0 ? true : false;
                    comSecondToggleDataList.Add(secondToggleData);
                }

                twoLevelToggleData.SecondLevelToggleDataList = comSecondToggleDataList;

                _comTwoLevelToggleDataList.Add(twoLevelToggleData);
            }

            if (comTwoLevelToggleControl != null)
                comTwoLevelToggleControl.InitTwoLevelToggleControl(_comTwoLevelToggleDataList,
                    OnFirstLevelToggleClick,
                    OnSecondLevelToggleClick);

        }


        private void OnFirstLevelToggleClick(ComControlData comToggleData)
        {
            if (comToggleData != null)
            {
                Logger.LogErrorFormat("firstLevel ToggleClick and id is {0}, name is {1}",
                    comToggleData.Id, comToggleData.Name);
            }
        }

        private void OnSecondLevelToggleClick(ComControlData comToggleData)
        {
            if (comToggleData != null)
            {
                Logger.LogErrorFormat("secondLevel ToggleClick and id is {0}, name is {1}",
                    comToggleData.Id, comToggleData.Name);
            }
        }

        #endregion

        #region TestTwoLevelToggleWithExample
        //测试二级Toggle
        private void InitTestTwoLevelToggleWithExample()
        {
            for (var i = 0; i < 5; i++)
            {
                ComTwoLevelToggleData twoLevelToggleData = new ComTwoLevelToggleData();

                ComFirstLevelToggleDataWithExample comToggleData = new ComFirstLevelToggleDataWithExample
                {
                    Id = i + 1,
                    Name = "FL_" + (i * 10 + 1),
                    IsSelected = false,
                    FirstLevelExampleName = "一" + ( i*10 + 1),
                    FirstLevelIndex = 1000 + (i * 10 + 1),
                };
                comToggleData.IsSelected = i == 1 ? true : false;

                twoLevelToggleData.FirstLevelToggleData = comToggleData;

                var comSecondToggleDataList = new List<ComControlData>();
                var secondToggleNumber = i;
                for (var j = 0; j < secondToggleNumber; j++)
                {
                    ComSecondLevelToggleDataWithExample secondToggleData = new ComSecondLevelToggleDataWithExample
                    {
                        Id = j + 1,
                        Name = "SL_" + (j * 10 + 1),
                        IsSelected = false,
                        SecondLevelExampleName = "二" +(j * 10  + 1),
                        SecondLevelIndex = 200 + (j * 10 + 1),
                    };
                    secondToggleData.IsSelected = j == 0 ? true : false;
                    comSecondToggleDataList.Add(secondToggleData);
                }

                twoLevelToggleData.SecondLevelToggleDataList = comSecondToggleDataList;

                _comTwoLevelToggleDataListWithExample.Add(twoLevelToggleData);
            }

            if (comTwoLevelToggleControlWithExample != null)
                comTwoLevelToggleControlWithExample.InitTwoLevelToggleControl(_comTwoLevelToggleDataListWithExample,
                    OnFirstLevelToggleClickWithExample,
                    OnSecondLevelToggleClickWithExample);

        }


        private void OnFirstLevelToggleClickWithExample(ComControlData comToggleData)
        {
            if (comToggleData != null)
            {
                Logger.LogErrorFormat("firstLevel ToggleClick and id is {0}, name is {1}",
                    comToggleData.Id, comToggleData.Name);

                ComFirstLevelToggleDataWithExample dataWithExample = comToggleData as ComFirstLevelToggleDataWithExample;
                if (dataWithExample != null)
                    Logger.LogErrorFormat("firstLevel ToggleClick and dataWithExample name is {0}, index is {1}",
                        dataWithExample.FirstLevelExampleName,
                        dataWithExample.FirstLevelIndex);
            }
        }

        private void OnSecondLevelToggleClickWithExample(ComControlData comToggleData)
        {
            if (comToggleData != null)
            {
                Logger.LogErrorFormat("secondLevel ToggleClick and id is {0}, name is {1}",
                    comToggleData.Id, comToggleData.Name);

                ComSecondLevelToggleDataWithExample dataWithExample = comToggleData as ComSecondLevelToggleDataWithExample;
                if (dataWithExample != null)
                {
                    Logger.LogErrorFormat("secondLevel ToggleClick and dataWithExample name is {0} index is {1}",
                        dataWithExample.SecondLevelExampleName,
                        dataWithExample.SecondLevelIndex);
                }
            }
        }

        #endregion

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<ComControlFrame>();
        }



    }
}