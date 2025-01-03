using System.Collections.Generic;
using System;
using System.Reflection;

namespace Mock
{
    class MocksManager
    {
        private List<MockUnit> mAllMockUnits = new List<MockUnit>();
        private List<MockUnit> mAllMsgUnits = new List<MockUnit>();

        private string mFilter = null;
        private string[] mSelectedOption = new string[0];

        public MocksManager()
        {
            Type interfaceType = typeof(global::Mock.Protocol.IMockProtocol);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (interfaceType.IsAssignableFrom(type) && type != interfaceType)
                    {
                        mAllMockUnits.Add(new MockUnit(type));
                    }
                }
            }
        }

        public MockUnit GetMockUnit(Type mockType)
        {
            foreach (var unit in mAllMockUnits)
            {
                if (unit.mockMsgType == mockType)
                {
                    return unit;
                }
            }

            return null;
        }

        public MockUnit GetMockUnitByMockMsgTypeName(string msgTypeName)
        {
            foreach (var unit in mAllMockUnits)
            {
                if (msgTypeName.Contains(unit.mockMsgType.Name))
                {
                    return unit;
                }
            }

            return null;
        }

        public MockUnit GetMockUnitByMsgId(uint msgId)
        {
            for (int i = 0; i < mAllMsgUnits.Count; i++)
            {
                if (mAllMsgUnits[i].msgID == msgId)
                {
                    return mAllMsgUnits[i];
                }
            }

            return null;
        }

        public void Clear()
        {
            mAllMockUnits.Clear();
        }

        public List<MockUnit> GetAllMgsMock()
        {
            mAllMsgUnits.Clear();

            for (int i = 0; i < mAllMockUnits.Count; ++i)
            {
                if (mAllMockUnits[i].isMsg)
                {
                    mAllMsgUnits.Add(mAllMockUnits[i]);
                }
            }

            return mAllMsgUnits;
        }

        public string[] GetOptionsByFilter(string filter)
        {
            if (mFilter != filter)
            {
                mFilter = filter;
                _updateSelectedOption();
            }

            return mSelectedOption;
        }

        public int GetOptionIndex(string msgTypeName)
        {
            for (int i = 0; i < mSelectedOption.Length; i++)
            {
                if (mSelectedOption[i].Contains(msgTypeName))
                {
                    return i;
                }
            }

            return -1;
        }

        private void _updateSelectedOption()
        {
            List<MockUnit> mockUnit = GetAllMgsMock();

            List<string> selectedOption = new List<string>();

            for (int i = 0; i < mockUnit.Count; ++i)
            {
                if (string.IsNullOrEmpty(mFilter) || mockUnit[i].mockMsgClsName.Contains(mFilter))
                {
                    selectedOption.Add(mockUnit[i].optionString);
                }
            }

            mSelectedOption = selectedOption.ToArray();
        }
    }
}
