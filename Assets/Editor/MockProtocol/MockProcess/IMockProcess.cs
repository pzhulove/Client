using System.Collections;
using System.Collections.Generic;
using Mock.Protocol;
using UnityEngine;

namespace Mock
{

    public enum MockType
    {
        WaitMessage,
    }

    public interface IMockProcess : IEnumerator
    {
        uint WaitMsgId { get; }

        ScriptableObject CurrentMockData { get; } 
    }
}
