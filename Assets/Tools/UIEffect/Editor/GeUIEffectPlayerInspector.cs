using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GeUIEffectPlayer), true)]
public class GeUIEffectPlayerInspector : Editor
{
    private SerializedObject m_Object;
    private bool m_InPlayMode = false;
    private float emiterEditorPreviousTime = 0;

    protected void OnEnable()
    {
        EditorApplication.update += EmitterUpdate;
        m_Object = new SerializedObject(target);
    }

    protected void OnDisable()
    {
        m_InPlayMode = false;
        EditorApplication.update -= EmitterUpdate;
    }

    public override void OnInspectorGUI()
    {
        string buttonStatePlay = "Play";
        string buttonStateStop = "Stop";
        string buttonTitle = m_InPlayMode ? buttonStateStop : buttonStatePlay;
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button(buttonTitle))
        {
            m_InPlayMode = !m_InPlayMode;

            GeUIEffectPlayer targUIEffectPlayer = (GeUIEffectPlayer)target;
            if (null != targUIEffectPlayer)
            {
                targUIEffectPlayer.Reinit();
                GeUIEffectParticle[] apar = targUIEffectPlayer.UIParticles;

                for(int i = 0,icnt = apar.Length; i<icnt;++i)
                {
                    GeUIEffectParticle part = apar[i];
                    if(null == part) continue;

                    if (!m_InPlayMode)
                        part.EditorStop();
                    else
                        part.EditorPlay();
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    private void EmitterUpdate()
    {
        if (Application.isPlaying == true)
            return;

        GeUIEffectParticleEditorPlayer.Time += Time.realtimeSinceStartup - emiterEditorPreviousTime;
        GeUIEffectPlayer targUIEffectPlayer = (GeUIEffectPlayer)target;
        if (null != targUIEffectPlayer)
        {
            GeUIEffectParticle[] apar = targUIEffectPlayer.UIParticles;
            if(null != apar)
            {
                for (int i = 0, icnt = apar.Length; i < icnt; ++i)
                {
                    GeUIEffectParticle part = apar[i];
                    if (null == part) continue;

                    part.UpdateFromEditor(Time.realtimeSinceStartup - emiterEditorPreviousTime);
                }
            }
        }
        emiterEditorPreviousTime = Time.realtimeSinceStartup;
    }

}
