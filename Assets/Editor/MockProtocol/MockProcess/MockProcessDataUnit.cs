using UnityEngine;
using UnityEngine.Events;

namespace Mock
{
    public class MockProcessDataUnit : ScriptableObject, IMockProcess
    {
        public uint               waitMsgId;

        public ScriptableObject[] sendObjects = new ScriptableObject[0];

        public uint               processCount = 1;

        private int               mProcessIndex = -1;

        public uint WaitMsgId
        {
            get
            {
                return waitMsgId;
            }
        }

        public ScriptableObject CurrentMockData
        {
            get
            {
                if (mProcessIndex >= 0 && mProcessIndex < sendObjects.Length)
                {
                    return sendObjects[mProcessIndex];
                }

                if (sendObjects.Length > 0)
                {
                    return sendObjects[sendObjects.Length - 1];
                }

                return null;
            }
        }

        public object Current
        {
            get
            {
                return CurrentMockData;
            }
        }

        public bool MoveNext()
        {
            mProcessIndex++;
            return mProcessIndex >= 0 && mProcessIndex > processCount; 
        }

        public void Reset()
        {
            mProcessIndex = -1;
        }
    }
}
