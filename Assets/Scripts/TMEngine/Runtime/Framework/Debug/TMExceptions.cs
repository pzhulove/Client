using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Tenmove.Runtime
{
    [Serializable]
    public class EngineException : Exception
    {
        public EngineException()
            : base()
        {
        }

        public EngineException(string message)
            : base(message)
        {
        }

        public EngineException(string fmt, params object[] args)
            : base(string.Format(fmt, args))
        {
        }

        public EngineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EngineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
