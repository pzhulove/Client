using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    /// <summary>
    /// Image is a textured element in the UI hierarchy.
    /// </summary>

    [AddComponentMenu("UI/TextEx", 11)] 
    public class TextEx : Text
    {
#if UNITY_EDITOR
        public TextEx()
        {
            var type = typeof(Text);
            var filed = type.GetField("m_FontData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var data = filed.GetValue(this) as FontData;
            data.richText = false;
            //supportRichText = false;
            raycastTarget = false;
            data.fontSize = 30;
        }
#endif
    }
}
