using UnityEngine;
using UnityEditor;
using System.Collections;


public class GeUIEffectParticleEditorPlayer
{

	public static float Time {
		get {
			if (Application.isPlaying == true){
				return UnityEngine.Time.timeSinceLevelLoad;
			}

			return EditorPrefs.GetFloat("EditorEmitterTime", 0);
		}

		set {
			EditorPrefs.SetFloat("EditorEmitterTime", value);
		}


	}
}
