using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System.Diagnostics;
using System.Reflection;

[CustomEditor(typeof(ComClientFrame))]
public class ComClientFrameInspector : Editor
{
    private Canvas mCanvas;
    public override void OnInspectorGUI()
    {
        if (mCanvas == null)
        {
            mCanvas = (target as Component).gameObject.GetOrAddComponent<Canvas>();
            if (mCanvas != null)
            {
                mCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
            }
            (target as Component).gameObject.GetOrAddComponent<CanvasGroup>();
            (target as Component).gameObject.GetOrAddComponent<GraphicRaycaster>();
            if ((target as Component).gameObject.layer != LayerMask.NameToLayer("UI"))
            {
                (target as Component).gameObject.layer = LayerMask.NameToLayer("UI");
            }
        }
        base.OnInspectorGUI();
    }
}
 