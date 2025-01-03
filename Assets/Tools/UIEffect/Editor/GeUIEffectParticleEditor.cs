using UnityEditor;
using UnityEditor.UI;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



namespace UnityEditor.UI
{
//	[ExecuteInEditMode]
	[CustomEditor(typeof(GeUIEffectParticle), true)]
	public class GeUIEffectParticleEditor : GraphicEditor
    {
		private SerializedObject m_Object;

		//editor update
		private SerializedProperty m_editorTime;
		private float emiterEditorPlayTime = 0;
		private float emiterEditorPreviousTime = 0;

        /// 只读
        private SerializedProperty m_IsPlaying;
        private SerializedProperty m_IsActive;
        private SerializedProperty m_CurParticleNum;

        //emitter properties
        private SerializedProperty m_PlayOnAwake;
        private SerializedProperty m_EffMaterial;
        private SerializedProperty m_TextureArray;
        private SerializedProperty m_IsAnimatedTex;
        private SerializedProperty m_AnimMode;

        private SerializedProperty m_LifeTime;
        private SerializedProperty m_LifeTimeRangeRate;

        private SerializedProperty m_ParticleColor;
        private SerializedProperty m_ColorRangeRate;
        private SerializedProperty m_ColorRamp;
        private SerializedProperty m_AlphaCurve;
        private SerializedProperty m_UseLifeColor;

        private SerializedProperty m_Size;
        private SerializedProperty m_SizeRangeRate;
        private SerializedProperty m_SizeCurve;

        private SerializedProperty m_Speed;
        private SerializedProperty m_SpeedRangeRate;
        private SerializedProperty m_SpinVelocity;
        private SerializedProperty m_SpinVelRangeValue;
        
        private SerializedProperty m_Rotate;
        private SerializedProperty m_RotateRangeValue;
        private SerializedProperty m_AlignedSpeed;
        private SerializedProperty m_VectorParticle;
        private SerializedProperty m_VectorScalar;

        private SerializedProperty m_Gravity;
        private SerializedProperty m_WaveFreq;
        private SerializedProperty m_WaveAmplitude;
        private SerializedProperty m_TurbulenceFreq;
        private SerializedProperty m_TurbulenceAmplitude;

        private SerializedProperty m_FrameCellX;
        private SerializedProperty m_FrameCellY;
        private SerializedProperty m_FrameNum;
        private SerializedProperty m_FrameRate;
        private SerializedProperty m_AlignToLife;

        private SerializedProperty m_EmitterShape;
        private SerializedProperty m_EmitDataBlockDesc;

        protected GeUIEffectDataBlock[] m_EmitDataBlock = null;
        protected GeUIParticleEmitterBase m_ParticleEmitter = null;

		protected static bool FieldsFoldout = true;
        protected static bool ParticleEmitterFoldout = true;
        protected static bool ParticlePropertyFoldout = true;
        protected static bool ParticleLifeSpanFoldout = true;
		protected static bool ParticleSizeFoldout = true;
        protected static bool ParticleSpeedFoldout = true;
        protected static bool ParticleOrientationFoldout = true;
		protected static bool ParticleColorFoldout = true;

        protected static bool m_InPlayMode = false;
        protected uint m_CurTimeMS = 0;


		public bool PlayOnEditor;

		protected override void OnEnable()
		{
			base.OnEnable();
            
            EditorApplication.update += EmitterUpdate;

            m_Object = new SerializedObject (target);

            //editor update
            //m_editorTime = m_Object.FindProperty("m_editorEmitterTime");

            //read only
            m_IsPlaying                 = m_Object.FindProperty("m_IsPlaying");
            m_IsActive                  = m_Object.FindProperty("m_IsActive");
            m_CurParticleNum            = m_Object.FindProperty("m_CurParticleNum");

            m_PlayOnAwake               = m_Object.FindProperty("m_PlayOnAwake");
            m_EffMaterial               = m_Object.FindProperty("m_EffMaterial");
            m_TextureArray              = m_Object.FindProperty("m_TextureArray");
            m_IsAnimatedTex             = m_Object.FindProperty("m_IsAnimatedTex");
            m_AnimMode                  = m_Object.FindProperty("m_AnimMode");

            m_LifeTime                  = m_Object.FindProperty("m_LifeTime");
            m_LifeTimeRangeRate         = m_Object.FindProperty("m_LifeTimeRangeRate");

            m_ParticleColor             = m_Object.FindProperty("m_ParticleColor");
            m_ColorRangeRate            = m_Object.FindProperty("m_ColorRangeRate");
            m_ColorRamp                 = m_Object.FindProperty("m_ColorRamp");
            m_AlphaCurve                = m_Object.FindProperty("m_AlphaCurve");
            m_UseLifeColor              = m_Object.FindProperty("m_UseLifeColor");

            m_Size                      = m_Object.FindProperty("m_Size");
            m_SizeRangeRate             = m_Object.FindProperty("m_SizeRangeRate");
            m_SizeCurve                 = m_Object.FindProperty("m_SizeCurve");

            m_Speed                     = m_Object.FindProperty("m_Speed");
            m_SpeedRangeRate            = m_Object.FindProperty("m_SpeedRangeRate");
            m_SpinVelocity              = m_Object.FindProperty("m_SpinVelocity");
            m_SpinVelRangeValue         = m_Object.FindProperty("m_SpinVelRangeValue");

            m_Rotate                    = m_Object.FindProperty("m_Rotate");
            m_RotateRangeValue          = m_Object.FindProperty("m_RotateRangeValue");
            m_AlignedSpeed              = m_Object.FindProperty("m_AlignedSpeed");

            m_VectorParticle            = m_Object.FindProperty("m_VectorParticle");
            m_VectorScalar              = m_Object.FindProperty("m_VectorScalar");

            m_Gravity                   = m_Object.FindProperty("m_Gravity");
            m_WaveFreq                  = m_Object.FindProperty("m_WaveFreq");
            m_WaveAmplitude             = m_Object.FindProperty("m_WaveAmplitude");
            m_TurbulenceFreq            = m_Object.FindProperty("m_TurbulenceFreq");
            m_TurbulenceAmplitude       = m_Object.FindProperty("m_TurbulenceAmplitude");

            m_FrameCellX                = m_Object.FindProperty("m_FrameCellX");
            m_FrameCellY                = m_Object.FindProperty("m_FrameCellY");
            m_FrameNum                  = m_Object.FindProperty("m_FrameNum");
            m_FrameRate                 = m_Object.FindProperty("m_FrameRate");
            m_AlignToLife               = m_Object.FindProperty("m_AlignToLife");

            m_EmitterShape              = m_Object.FindProperty("m_EmitterShape");
            m_EmitDataBlockDesc         = m_Object.FindProperty("m_EmitDataBlockDesc");

            string[] str = new string[m_EmitDataBlockDesc.arraySize];
            for (int i = 0; i < m_EmitDataBlockDesc.arraySize; ++i)
            {
                SerializedProperty cur = m_EmitDataBlockDesc.GetArrayElementAtIndex(i);
                str[i] = cur.stringValue;
            }

            m_EmitDataBlock = GeUIEffectDataBlockSerializer.FromString(str);

            GeUIEffectParticle targParticle = (GeUIEffectParticle)target;
            if(null != targParticle)
            {
                m_ParticleEmitter = targParticle.emitter;
                if(null == m_ParticleEmitter)
                {
                    targParticle.RebuildEmitter();
                    m_ParticleEmitter = targParticle.emitter;
                }
            }

            m_InPlayMode = false;
        }
		
		protected override void OnDisable()
        {
            GeUIEffectParticle targParticle = (GeUIEffectParticle)target;
            if (null != targParticle)
                targParticle.EditorStop();
            m_InPlayMode = false;

            EditorApplication.update -= EmitterUpdate;
            base.OnDisable();
		}

		private void EmitterUpdate()
		{
			if (Application.isPlaying == true)
				return;

			GeUIEffectParticleEditorPlayer.Time += Time.realtimeSinceStartup - emiterEditorPreviousTime;
			//m_editorTime.floatValue = GeUIEffectParticleEditorPlayer.Time;

			GeUIEffectParticle myScript = (GeUIEffectParticle)target;

            float timeScale = Time.timeScale;


            myScript.UpdateFromEditor(Time.realtimeSinceStartup - emiterEditorPreviousTime);

			m_Object.Update();

            Repaint();
            SceneView.RepaintAll();
            
			emiterEditorPreviousTime = Time.realtimeSinceStartup;
        }
		
		public override bool HasPreviewGUI() { return true; }


		public override void OnInspectorGUI()
		{
			m_Object.Update();

            //emitter properties
            EditorGUILayout.LabelField("Emiter stats");
			++EditorGUI.indentLevel;
			{
				EditorGUILayout.LabelField("Particles number: ", m_CurParticleNum.intValue.ToString());
				if (m_ParticleEmitter.maxParticles > 0)
                    _ProgressBar( (float)m_CurParticleNum.intValue / (float)m_ParticleEmitter.maxParticles, "Number of particles");
				EditorGUILayout.LabelField("Emiter is active: " + m_IsActive.boolValue.ToString());
				EditorGUILayout.LabelField("Emiter is playing: " + m_IsPlaying.boolValue.ToString());
			}
			--EditorGUI.indentLevel;


            EditorGUILayout.Space();

            string buttonStatePlay = "Play";
            string buttonStateStop = "Stop";
            string buttonTitle = m_InPlayMode ? buttonStateStop : buttonStatePlay;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if(GUILayout.Button(buttonTitle))
            {
                m_InPlayMode = !m_InPlayMode;

                GeUIEffectParticle targParticle = (GeUIEffectParticle)target;
                if (null != targParticle)
                {
                    if (!m_InPlayMode)
                        targParticle.EditorStop();
                    else
                        targParticle.EditorPlay();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(m_PlayOnAwake);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            ParticleEmitterFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), ParticleEmitterFoldout, "Emitter Properties", true);
            if (ParticleEmitterFoldout == true)
            {
                EditorGUILayout.Space();
                ++EditorGUI.indentLevel;

                bool bHasChanged = false;
                float oldDelayEmit = m_ParticleEmitter.delayEmit;
                m_ParticleEmitter.delayEmit = EditorGUILayout.FloatField("Delay Emit(seconds):", m_ParticleEmitter.delayEmit);
				if (m_ParticleEmitter.delayEmit < 0)
					m_ParticleEmitter.delayEmit = 0;
                if (oldDelayEmit != m_ParticleEmitter.delayEmit)
                    bHasChanged = true;

                float oldEmitRate = m_ParticleEmitter.emitRate;
                m_ParticleEmitter.emitRate = EditorGUILayout.FloatField("Emit Rate(Percentage(%)):", m_ParticleEmitter.emitRate);
                if (m_ParticleEmitter.emitRate < 0)
                    m_ParticleEmitter.emitRate = 0;
                if (oldEmitRate != m_ParticleEmitter.emitRate)
                    bHasChanged = true;

                int oldPartiPerEmission = m_ParticleEmitter.partiPerEmission;
                m_ParticleEmitter.partiPerEmission = EditorGUILayout.IntField("Particles Per Emission:", m_ParticleEmitter.partiPerEmission);
                if (m_ParticleEmitter.partiPerEmission < 0)
                    m_ParticleEmitter.partiPerEmission = 0;
                if (oldPartiPerEmission != m_ParticleEmitter.partiPerEmission)
                    bHasChanged = true;

                int oldMaxParticles = m_ParticleEmitter.maxParticles;
                m_ParticleEmitter.maxParticles = EditorGUILayout.IntField("Max Particles:", m_ParticleEmitter.maxParticles);
                if (m_ParticleEmitter.maxParticles < 0)
                    m_ParticleEmitter.maxParticles = 0;
                if (oldMaxParticles != m_ParticleEmitter.maxParticles)
                    bHasChanged = true;

                uint oldDurationMS = m_ParticleEmitter.durationMS;
                m_ParticleEmitter.durationMS = (uint)(EditorGUILayout.FloatField("Emit Duration(seconds):", m_ParticleEmitter.durationMS * 0.001f) * 1000.0f);
                if (m_ParticleEmitter.durationMS < 0)
                    m_ParticleEmitter.durationMS = 0;
                if (oldDurationMS != m_ParticleEmitter.durationMS)
                    bHasChanged = true;

                bool oldRelative = m_ParticleEmitter.relative;
                m_ParticleEmitter.relative = EditorGUILayout.Toggle("Relative Mode:", m_ParticleEmitter.relative);
                if (oldRelative != m_ParticleEmitter.relative)
                    bHasChanged = true;

                int oldEmit = m_EmitterShape.enumValueIndex;
                EditorGUILayout.PropertyField(m_EmitterShape);
                if (m_EmitterShape.enumValueIndex != oldEmit)
                    bHasChanged = true;

                if (bHasChanged)
                {
                    GeUIEffectDataBlock[] res = null;
                    m_ParticleEmitter.SaveDataBlock(ref res);

                    string[] data = GeUIEffectDataBlockSerializer.ToString(res);

                    m_EmitDataBlockDesc.ClearArray();
                    for (int i = 0; i < data.Length; ++i)
                    {
                        m_EmitDataBlockDesc.InsertArrayElementAtIndex(i);
                        SerializedProperty cur = m_EmitDataBlockDesc.GetArrayElementAtIndex(i);
                        cur.stringValue = data[i];
                    }

                    m_Object.ApplyModifiedProperties();

                    GeUIEffectParticle targParticle = (GeUIEffectParticle)target;
                    if (null != targParticle)
                    {
                        targParticle.RebuildEmitter();
                        m_ParticleEmitter = targParticle.emitter;
                    }
                }

                switch ((EUIEffEmitShape)m_EmitterShape.enumValueIndex)
                {
                    case EUIEffEmitShape.Point:
                        _EmitterLayoutPoint();
                        break;
                    case EUIEffEmitShape.Circle:
                        _EmitterLayoutCircle();
                        break;
                    case EUIEffEmitShape.Rect:
                        _EmitterLayoutRect();
                        break;
                    case EUIEffEmitShape.Segment:
                        _EmitterLayoutSegment();
                        break;
                    case EUIEffEmitShape.Directional:
                        _EmitterLayoutDirectional();
                        break;
                }

                -- EditorGUI.indentLevel;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            FieldsFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), FieldsFoldout, "Force Fields", true);
            if (FieldsFoldout == true)
            {
                EditorGUILayout.Space();
                ++EditorGUI.indentLevel;
                {
                    EditorGUILayout.PropertyField(m_Gravity);
                    EditorGUILayout.PropertyField(m_WaveFreq, new GUIContent("Wave frequency"));
                    EditorGUILayout.PropertyField(m_WaveAmplitude, new GUIContent("Wave amplitude"));
                    EditorGUILayout.PropertyField(m_TurbulenceFreq, new GUIContent("Turbulence frequency"));
                    EditorGUILayout.PropertyField(m_TurbulenceAmplitude, new GUIContent("Turbulence amplitude"));
                }
                --EditorGUI.indentLevel;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            ParticlePropertyFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), ParticlePropertyFoldout, "Particle Properties", true);
            if (true == ParticlePropertyFoldout)
            {
                EditorGUILayout.Space();
                ++EditorGUI.indentLevel;
                {

                    ParticleLifeSpanFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), ParticleLifeSpanFoldout, "Particles Life Span", true);
                    if (ParticleLifeSpanFoldout == true)
                    {
                        ++EditorGUI.indentLevel;
                        {
                            EditorGUILayout.PropertyField(m_LifeTime, new GUIContent("Life"));
                            if (m_LifeTime.floatValue < 0)
                                m_LifeTime.floatValue = 0;

                            EditorGUILayout.PropertyField(m_LifeTimeRangeRate, new GUIContent("Life Range Rate"));
                            if (m_LifeTimeRangeRate.floatValue < 0)
                                m_LifeTimeRangeRate.floatValue = 0;
                            if (m_LifeTimeRangeRate.floatValue > 1)
                                m_LifeTimeRangeRate.floatValue = 1;
                        }
                        --EditorGUI.indentLevel;
                    }


                    ParticleSizeFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), ParticleSizeFoldout, "Particles Size", true);
                    if (ParticleSizeFoldout == true)
                    {
                        ++EditorGUI.indentLevel;
                        {
                            EditorGUILayout.PropertyField(m_Size, new GUIContent("Size"));
                            if (m_Size.floatValue < 0)
                                m_Size.floatValue = 0;
                            EditorGUILayout.LabelField("Life Size Multiplier");
                            EditorGUILayout.PropertyField(m_SizeCurve, new GUIContent(""));
                            EditorGUILayout.PropertyField(m_SizeRangeRate, new GUIContent("Size Range Rate"));
                            if (m_SizeRangeRate.floatValue < 0)
                                m_SizeRangeRate.floatValue = 0;
                            if (m_SizeRangeRate.floatValue > 1)
                                m_SizeRangeRate.floatValue = 1;
                        }
                        --EditorGUI.indentLevel;
                    }

                    ParticleSpeedFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), ParticleSpeedFoldout, "Particles Speed", true);
                    if (ParticleSpeedFoldout == true)
                    {
                        ++EditorGUI.indentLevel;
                        {
                            EditorGUILayout.PropertyField(m_Speed, new GUIContent("Speed"));
                            if (m_Speed.floatValue < 0)
                                m_Speed.floatValue = 0;
                            EditorGUILayout.PropertyField(m_SpeedRangeRate, new GUIContent("Speed Range Rate"));
                            if (m_SpeedRangeRate.floatValue < 0)
                                m_SpeedRangeRate.floatValue = 0;
                            if (m_SpeedRangeRate.floatValue > 1)
                                m_SpeedRangeRate.floatValue = 1;
                        }
                        --EditorGUI.indentLevel;
                    }


                    ParticleOrientationFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), ParticleOrientationFoldout, "Particles Orientation", true);
                    if (ParticleOrientationFoldout == true)
                    {
                        ++EditorGUI.indentLevel;
                        {
                            EditorGUILayout.PropertyField(m_AlignedSpeed, new GUIContent("Aligned With Speed"));
                            if (m_AlignedSpeed.boolValue == false)
                            {
                                EditorGUILayout.PropertyField(m_Rotate, new GUIContent("Angle"));
                                EditorGUILayout.PropertyField(m_RotateRangeValue, new GUIContent("Angle Range Value"));
                                if (m_RotateRangeValue.floatValue < 0)
                                    m_RotateRangeValue.floatValue = 0;
                                if (m_RotateRangeValue.floatValue > 180)
                                    m_RotateRangeValue.floatValue = 180;

                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(m_SpinVelocity, new GUIContent("Spin Velocity"));
                                EditorGUILayout.PropertyField(m_SpinVelRangeValue, new GUIContent("Spin Velocity Range Value"));
                                if (m_SpinVelRangeValue.floatValue < 0)
                                    m_SpinVelRangeValue.floatValue = 0;
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(m_VectorParticle, new GUIContent("Vector Velocity"));
                                if (m_VectorParticle.boolValue)
                                    m_VectorScalar.floatValue = EditorGUILayout.Slider("Vector Scalar",m_VectorScalar.floatValue, 0,2);
                            }
                        }
                        --EditorGUI.indentLevel;
                    }

                    ParticleColorFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), ParticleColorFoldout, "Particles Color / Texture", true);
                    if (ParticleColorFoldout == true)
                    {
                        ++EditorGUI.indentLevel;
                        {
                            EditorGUILayout.PropertyField(m_EffMaterial, new GUIContent("Material"));

                            EditorGUIUtility.LookLikeInspector();
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(m_TextureArray, new GUIContent("Texture List"), true, null);
                            if (EditorGUI.EndChangeCheck())
                            {
                                serializedObject.ApplyModifiedProperties();
                                GeUIEffectParticle particle = (GeUIEffectParticle)target;
                                particle.RebuildAtlas();
                            }
                            EditorGUIUtility.LookLikeControls();

                            if (m_TextureArray.arraySize > 0)
                            {
                                EditorGUILayout.PropertyField(m_IsAnimatedTex, new GUIContent("Animated Texture"));
                            }

                            if (m_IsAnimatedTex.boolValue == true)
                            {
                                EditorGUILayout.PropertyField(m_AnimMode, new GUIContent("Animate Mode"));

                                EditorGUILayout.PropertyField(m_AlignToLife, new GUIContent("Align To Life"));

                                if (!m_AlignToLife.boolValue)
                                {
                                    EditorGUILayout.PropertyField(m_FrameRate, new GUIContent("Frame Rate"));
                                    if (m_FrameRate.floatValue < 1)
                                        m_FrameRate.floatValue = 1;
                                }
                                else
                                {
                                    m_FrameRate.floatValue = (int)(m_FrameNum.intValue / m_LifeTime.floatValue);
                                }

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(m_FrameCellX, new GUIContent("Frame Cell X:"));
                                if (m_FrameCellX.intValue < 1)
                                    m_FrameCellX.intValue = 1;
                                EditorGUILayout.PropertyField(m_FrameCellY, new GUIContent("Frame Cell Y:"));
                                if (m_FrameCellY.intValue < 1)
                                    m_FrameCellY.intValue = 1;
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.PropertyField(m_FrameNum, new GUIContent("Frame Cell Num:"));
                                if (m_FrameNum.intValue < 1)
                                    m_FrameNum.intValue = 1;
                            }

                            EditorGUILayout.PropertyField(m_UseLifeColor, new GUIContent("Use Life Color"));
                            if (m_UseLifeColor.boolValue == true)
                            {
                                EditorGUILayout.LabelField("Life Color");
                                EditorGUILayout.PropertyField(m_ColorRamp, new GUIContent(""));
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(m_ParticleColor, new GUIContent("Color"));
                            }

                            m_ColorRangeRate.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.Slider("Random Range Rate Red:", m_ColorRangeRate.GetArrayElementAtIndex(0).floatValue, 0.0f, 1.0f);
                            m_ColorRangeRate.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.Slider("Random Range Rate Green:", m_ColorRangeRate.GetArrayElementAtIndex(1).floatValue, 0.0f, 1.0f);
                            m_ColorRangeRate.GetArrayElementAtIndex(2).floatValue = EditorGUILayout.Slider("Random Range Rate Blue:", m_ColorRangeRate.GetArrayElementAtIndex(2).floatValue, 0.0f, 1.0f);

                            EditorGUILayout.LabelField("Life Opacity Multiplier:");
                            EditorGUILayout.PropertyField(m_AlphaCurve, new GUIContent(""));
                        }
                        --EditorGUI.indentLevel;
                    }
                    --EditorGUI.indentLevel;
                }
            }

            if(EditorGUI.EndChangeCheck())
            {
                GeUIEffectDataBlock[] res = null;
                m_ParticleEmitter.SaveDataBlock(ref res);

                string[] data = GeUIEffectDataBlockSerializer.ToString(res);

                m_EmitDataBlockDesc.ClearArray();
                for (int i = 0; i < data.Length; ++i)
                {
                    m_EmitDataBlockDesc.InsertArrayElementAtIndex(i);
                    SerializedProperty cur = m_EmitDataBlockDesc.GetArrayElementAtIndex(i);
                    cur.stringValue = data[i];
                }

                GeUIEffectParticle particle = (GeUIEffectParticle)target;
                particle.ReCookCurves();

                m_Object.ApplyModifiedProperties();
            }
            
            GeUIEffectParticle targParticle1 = (GeUIEffectParticle)target;
            if (null != targParticle1)
            {
                string buttonStateTrue = "raycastTarget True";
                string buttonStateFalse = "raycastTarget False";
                string buttonRaycastTarget = targParticle1.raycastTarget ? buttonStateTrue : buttonStateFalse;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button(buttonRaycastTarget))
                {
                    if (targParticle1.raycastTarget)
                        targParticle1.raycastTarget = false;
                    else
                        targParticle1.raycastTarget = true;

                }
            }
        }

		void ArrayGUI(SerializedObject obj, SerializedProperty property, string propertyName)
		{
			int size = property.arraySize;
			
			int newSize = EditorGUILayout.IntField(propertyName, size);
			
			if (newSize != size)
			{
				property.arraySize = newSize;
			}
			
			EditorGUI.indentLevel = 3;
			
			for (int i=0;i<newSize;i++)
			{
				var prop = property.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(prop);
			}
			EditorGUI.indentLevel = 2;
		}

        private EUIParticleEmitMode _PopupEmitMode(EUIParticleEmitMode emitMode)
        {
            string[] enumList = new string[] 
            {
                EUIParticleEmitMode.Edge.ToString(),
                EUIParticleEmitMode.Vertices.ToString(),
                EUIParticleEmitMode.Area.ToString(),
            };

            return (EUIParticleEmitMode)EditorGUILayout.Popup("Emitter Mode:", (int)emitMode, enumList);
        }

        private EUIParticleVelMode _PopupVelocityMode(EUIParticleVelMode velMode)
        {
            string[] enumList = new string[]
            {
                EUIParticleVelMode.None.ToString(),
                EUIParticleVelMode.Radiant.ToString(),
                EUIParticleVelMode.RadiantFlip.ToString(),
                EUIParticleVelMode.RadiantTwoSide.ToString(),
            };

            return (EUIParticleVelMode)EditorGUILayout.Popup("Velocity Mode:", (int)velMode, enumList);
        }

        private void _EmitterLayoutPoint()
		{

        }

        private void _EmitterLayoutCircle()
        {
            GeUIParticleEmitterCircle emitter = m_ParticleEmitter as GeUIParticleEmitterCircle;
            if (null == emitter)
                return;

            ++EditorGUI.indentLevel;
        	{
                emitter.emitMode = _PopupEmitMode(emitter.emitMode);
                emitter.velocityMode = _PopupVelocityMode(emitter.velocityMode);
                emitter.radius = EditorGUILayout.FloatField("Radius",emitter.radius);
                emitter.circleSegments = EditorGUILayout.IntField("Segments",emitter.circleSegments);
                if (emitter.circleSegments < 3)
                    emitter.circleSegments = 3;
                if (emitter.circleSegments > 128)
                    emitter.circleSegments = 128;

            }
        	--EditorGUI.indentLevel;
        }
        
        private void _EmitterLayoutRect()
        {
            GeUIParticleEmitterRect emitter = m_ParticleEmitter as GeUIParticleEmitterRect;
            if (null == emitter)
                return;

            ++EditorGUI.indentLevel;
            {
                emitter.emitMode = _PopupEmitMode(emitter.emitMode);
                emitter.velocityMode = _PopupVelocityMode(emitter.velocityMode);
                emitter.dimension = EditorGUILayout.Vector2Field("Dimension:", emitter.dimension);
        	}
        	--EditorGUI.indentLevel;
        }
        
        private void _EmitterLayoutSegment()
        {
            GeUIParticleEmitterSegment emitter = m_ParticleEmitter as GeUIParticleEmitterSegment;
            if (null == emitter)
                return;

            ++EditorGUI.indentLevel;
            {
                emitter.emitDirection   = EditorGUILayout.Slider("Emit Direction", emitter.emitDirection,0,360.0f);
                emitter.emitSpread      = EditorGUILayout.Slider("Emit Spread", emitter.emitSpread, 0, 180.0f);
                emitter.segmentBegin    = EditorGUILayout.Vector2Field("Segment Begin Point", emitter.segmentBegin);
                emitter.segmentEnd      = EditorGUILayout.Vector2Field("Segment End Point", emitter.segmentEnd);
            }
            --EditorGUI.indentLevel;
        }
        
        
        private void _EmitterLayoutDirectional()
        {
            GeUIParticleEmitterDirectional emitter = m_ParticleEmitter as GeUIParticleEmitterDirectional;
            if (null == emitter)
                return;

            ++EditorGUI.indentLevel;
            {
                emitter.emitDirection = EditorGUILayout.Slider("Emit Direction", emitter.emitDirection,0,360);
                emitter.emitSpread = EditorGUILayout.Slider("Emit Spread", emitter.emitSpread,0,180);
            }
            --EditorGUI.indentLevel;
        }
        // 
        // 		private void ProgressBar (float value, string label) {
        // 			Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
        // 			EditorGUI.ProgressBar (rect, value, label);
        // 			EditorGUILayout.Space ();
        // 		}
        // 
        // 
        private void OnSceneGUI()
		{
            GeUIEffectParticle particle = target as GeUIEffectParticle;

			switch ((EUIEffEmitShape)m_EmitterShape.enumValueIndex)
			{
                case EUIEffEmitShape.Point:
                    _DrawGizmoPoint(particle);
                    break;
                case EUIEffEmitShape.Circle:
                    _DrawGizmoCircle(particle);
                    break;
                case EUIEffEmitShape.Rect:
                    _DrawGizmoRect(particle);
                    break;
                case EUIEffEmitShape.Segment:
                    _DrawGizmoSegment(particle);
                    break;
                case EUIEffEmitShape.Directional:
                    _DrawGizmoDirectional(particle);
                    break;
            }

		}
        
        private void _DrawGizmoPoint(GeUIEffectParticle particle)
        {
            if (null == particle)
                return;

            GeUIParticleEmitterPoint emitter = particle.emitter as GeUIParticleEmitterPoint;
            if (null == emitter)
                return;

        	Handles.DrawLine(particle.transform.position + new Vector3(0, -1, 0), particle.transform.position + new Vector3(0, 1, 0));
        	Handles.DrawLine(particle.transform.position + new Vector3(-1, 0, 0), particle.transform.position + new Vector3(1, 0, 0));
        }

        private void _DrawGizmoCircle(GeUIEffectParticle particle)
        {
            if (null == particle)
                return;

            GeUIParticleEmitterCircle emitter = particle.emitter as GeUIParticleEmitterCircle;
            if (null == emitter)
                return;

            Vector3[] circlePoints = _CirclePoints(particle.transform.position, emitter.circleSegments, emitter.radius).ToArray();
            Handles.DrawPolyLine(circlePoints);
            Handles.DrawLine(circlePoints[circlePoints.Length - 1], circlePoints[0]);
        }

        private void _DrawGizmoRect(GeUIEffectParticle particle)
        {
            if (null == particle)
                return;

            GeUIParticleEmitterRect emitter = particle.emitter as GeUIParticleEmitterRect;
            if (null == emitter)
                return;

            Vector3[] rectPoints = _RectPoints(emitter.dimension, particle.transform.position);
            Handles.DrawPolyLine(rectPoints);
            Handles.DrawLine(rectPoints[3], rectPoints[0]);
        }

        private void _DrawGizmoDirectional(GeUIEffectParticle particle)
        {
            if (null == particle)
                return;

            GeUIParticleEmitterDirectional emitter = particle.emitter as GeUIParticleEmitterDirectional;
            if (null == emitter)
                return;

            Handles.DrawLine(particle.transform.position, RotatePointAroundPivot(particle.transform.position + new Vector3(100, 0, 0), particle.transform.position, new Vector3(0, 0, - emitter.emitDirection - (emitter.emitSpread * 0.5f) + 90)));
            Handles.DrawLine(particle.transform.position, RotatePointAroundPivot(particle.transform.position + new Vector3(100, 0, 0), particle.transform.position, new Vector3(0, 0, - emitter.emitDirection + (emitter.emitSpread * 0.5f) + 90)));
        }

        private void _DrawGizmoSegment(GeUIEffectParticle particle)
        {
            if (null == particle)
                return;

            GeUIParticleEmitterSegment emitter = particle.emitter as GeUIParticleEmitterSegment;
            if (null == emitter)
                return;

            Vector3 lineCenter = particle.transform.position + (new Vector3(emitter.segmentBegin.x, emitter.segmentBegin.y, 0) + new Vector3(emitter.segmentEnd.x, emitter.segmentEnd.y, 0)) * 0.5f;
            Handles.DrawLine(new Vector3(emitter.segmentBegin.x, emitter.segmentBegin.y, 0) + particle.transform.position ,  new Vector3(emitter.segmentEnd.x, emitter.segmentEnd.y, 0) + particle.transform.position);
            Handles.DrawLine(lineCenter, RotatePointAroundPivot(lineCenter + new Vector3(100, 0, 0), lineCenter, new Vector3(0, 0, - emitter.emitDirection - (emitter.emitSpread * 0.5f) + 90)));
            Handles.DrawLine(lineCenter, RotatePointAroundPivot(lineCenter + new Vector3(100, 0, 0), lineCenter, new Vector3(0, 0, -emitter.emitDirection + (emitter.emitSpread * 0.5f) + 90)));
        }
        
        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
        	Vector3 dir = point - pivot; 
        	dir = Quaternion.Euler(angles)*dir; 
        	point = dir + pivot; 
        	return point; 
        }

        private Vector3[] _RectPoints (Vector2 dimension, Vector3 center)
		{
			Vector3[] rectPoints = new Vector3[4];

			rectPoints[0] = new Vector3(-dimension.x, -dimension.y, 0) + center;
			rectPoints[1] = new Vector3( dimension.x, -dimension.y, 0) + center;
			rectPoints[2] = new Vector3( dimension.x,  dimension.y, 0) + center;
			rectPoints[3] = new Vector3(-dimension.x,  dimension.y, 0) + center;

			return rectPoints;
		}

		private List<Vector3> _CirclePoints(Vector3 center, int sides, float radius)
		{
			List<Vector3> circleVertices = new List<Vector3>();
			float x, y, t;
			for (int i = 0; i < sides; i++){
				t = 2 * Mathf.PI * ((float)i / (float)sides); 
				x = Mathf.Cos(t) * radius;
				y = Mathf.Sin(t) * radius;
				
				Vector3 vertice = new Vector3(x, y, 0);
				vertice += center;
				circleVertices.Add(vertice);
			}
			return circleVertices;
		}

        #region 内部函数
        private void _ProgressBar(float value, string label)
        {
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }
        #endregion
    }
}
