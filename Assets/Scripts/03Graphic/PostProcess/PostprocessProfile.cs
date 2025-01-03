using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    [Serializable]
    public class PostprocessProfile : ScriptableObject
    {
        public List<PostprocessEffectSettings> Effects = new List<PostprocessEffectSettings>(0);
    }
}



