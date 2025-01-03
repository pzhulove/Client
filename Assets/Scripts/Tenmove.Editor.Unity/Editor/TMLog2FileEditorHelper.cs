
using Tenmove.Runtime;
using UnityEditor;

namespace Tenmove.Editor.Unity
{
    [InitializeOnLoad]
    public class TMLog2FileEditorHelper
    {
        static TMLog2FileEditorHelper()
        {
            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }

        static void OnPlaymodeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (PlayModeStateChange.ExitingPlayMode == playModeStateChange)
                Log2File.Shutdown();
        }
    }
}