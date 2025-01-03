#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DSkillEditor
{
    public class DEditorEffects 
    {
        ~DEditorEffects()
        {
            Show(false);
        }

        protected GameObject _rootObject;
        protected GameObject _gameObjectPrefeb;
        protected GameObject _gameObject;
        private ParticleSystem[] pss;
        //private ParticleEmitter[] emitters;
//        private ParticleAnimator[] psanimators;
        private TrailRenderer[] trails;
        private Animation[] anis;
        private FrameMaterials[] frames;
        private TransformParam transform;
        private Animator[] animators;
        private GameObject _parent;
        private EffectsFrames _data;
        private float duration;
        private float giventime;
        private float pretime = Mathf.NegativeInfinity;

        private bool visiable = false;
        protected string _name = "effect";
        protected EffectPlayType _type = EffectPlayType.EffectTime;

        List<float> m_AnimatorSpeed = new List<float>();

        public void UpdateParent(GameObject p)
        {
            if(_parent != p)
            {
                _parent = p;

                if (_rootObject)
                {
                    //_rootObject.transform.parent = null;
                    _rootObject.transform.SetParent(p.transform, false);
                }
            }
        }

        public void ApplyMirror(bool bMirror)
        {
            if(_rootObject == null)
            {
                return;
            }

            MirroEffect[] mes = _rootObject.GetComponentsInChildren<MirroEffect>();
            for (int i = 0; i < mes.Length; ++i)
            {
                mes[i].Apply(bMirror);
            }
        }

        public DEditorEffects()
        {
            gameObjectPrefeb = null;
            visiable = false;
            transform = new TransformParam();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            
        }

        public void SetData(EffectsFrames data)
        {
            _data = data;
        }
        public T[] InitDatas<T>(T[] data)  where T : new()
        {
            return new T[0];
        }

        public void ClearCacheData()
        {
            pss     = InitDatas(pss);
            anis    = InitDatas(anis);
            frames  = InitDatas(frames);

            animators = null;
            //get all emitters we need to do scaling on
            //emitters = null;
            //psanimators = null;
            trails = null;

        }

        public void UpdateData(EffectsFrames framedata)
        {
            if(framedata.localPosition != transform.localPosition)
            {
                transform.localPosition = framedata.localPosition;

                if(_rootObject)
                {
                    _rootObject.transform.localPosition = transform.localPosition;
                }
            }

            if (framedata.localRotation != transform.localRotation)
            {
                transform.localRotation = framedata.localRotation;

                if (_rootObject)
                {
                    _rootObject.transform.localRotation = transform.localRotation;
                }
            }

            if (framedata.localScale != transform.localScale)
            {
                transform.localScale = framedata.localScale;

                if (_rootObject)
                {
                    Vector3 preScale = _rootObject.transform.localScale;
                    _rootObject.transform.localScale = transform.localScale;
                    Vector3 scale = _rootObject.transform.localScale;
                    Vector3 scaleFactor = scale;
                    scaleFactor.x /= preScale.x;
                    scaleFactor.y /= preScale.y;
                    scaleFactor.z /= preScale.z;
                    UpdateParticleScale(scaleFactor);
                }
            }

            if(_type != framedata.playtype)
            {
                _type = framedata.playtype;
                giventime = framedata.time;
                if(visiable)
                duration = SampleDuration();
            }

            gameObjectPrefeb = framedata.effectGameObjectPrefeb;
            giventime = framedata.time;
        }

        public void UpdateParticleScale(Vector3 scaleFactor)
        {
            //scale legacy particle systems
            ScaleLegacySystems(scaleFactor.x);
            //scale shuriken particle systems
            ScaleShurikenSystems(scaleFactor);
            //scale trail renders
            ScaleTrailRenderers(scaleFactor.x);
        }

        public EffectPlayType  type
        {
            get
            {
                return _type;
            }

            set
            {
                if(_type != value)
                {
                    _type = value;

                    if(_type == EffectPlayType.EffectTime)
                    {
                        duration = SampleDuration();
                    }
                }
            }
        }
        public string name
        {
            get
            {
                return _name;
            }
        }

        public GameObject gameObjectPrefeb
        {
            set
            {
                if(_gameObjectPrefeb != value)
                {
                    if(_rootObject)
                    {
#if !LOGIC_SERVER
                        _rootObject.transform.DetachChildren();
#endif
                    }

                    if(_gameObject)
                    {

                        Editor.DestroyImmediate(_gameObject);
                    }

                    _gameObject = null;
                    _gameObjectPrefeb = value;

                    if (value == null)
                    {
                        Clear(true);
                    }
                    else
                    {
                        if(visiable)
                        {
                            if (_rootObject == null)
                                _rootObject = new GameObject();
                            Init();
                        }
                    }
                }
            }

            get
            {
                return _gameObjectPrefeb;
            }
        }

        public GameObject rootObject
        {
            get
            {
                return _rootObject;
            }
        }

        public float timeLength
        {
            get
            {
                if(type == EffectPlayType.GivenTime)
                {
                    return giventime;

                }
                return duration;
            }

            set
            {
                if(type == EffectPlayType.GivenTime)
                {
                    giventime = value;
                }
            }
        }

        public static DEditorEffects Create(GameObject gameObject)
        {
            DEditorEffects effect = new DEditorEffects();
            effect._gameObjectPrefeb = gameObject;
            return effect;
        }
        void RemoveEmptyAnimator()
        {
            List<Animator> emptyAnimators = new List<Animator>();
            List<Animator> anim = new List<Animator>();

            for (int i = 0; i < animators.Length; ++i)
            {
                if (animators[i].GetCurrentAnimatorClipInfo(0).Length <= 0)
                {
                    emptyAnimators.Add(animators[i]);
                }

                anim.Add(animators[i]);
            }

            for(int i = 0; i < emptyAnimators.Count; ++i)
            {
                anim.Remove(emptyAnimators[i]);
            }
            animators = anim.ToArray();
        }

        void Pause()
        {
            if (pss != null)
            {
                for (int i = 0; i < pss.Length; ++i)
                {
                    if(pss[i] == null) continue;
                    pss[i].Pause();
                }
            }
            

            if (anis != null)
            {
                Animation ani = null;
                for (int i = 0; i < anis.Length; ++i)
                {
                    ani = anis[i];

                    if(ani == null) continue;

                    foreach (AnimationState ans in ani)
                        ans.speed = 0.0f;
                }
            }
            
            if (animators != null)
            {
                for (int i = 0; i < animators.Length; ++i)
                {
                    Animator ani = animators[i];
                    
                    if (ani != null)
                    {
                        ani.speed = 0.0f;
                    }
                }
            }
            
        }

        void Resume()
        {
            if (pss != null)
            {
                for (int i = 0; i < pss.Length; ++i)
                {
                    if(pss[i] == null) continue;
                    pss[i].Play();
                }
            }

            if (anis != null)
            {
                Animation ani = null;
                for (int i = 0; i < anis.Length; ++i)
                {
                    ani = anis[i];

                    if(ani == null) continue;

                    foreach (AnimationState ans in ani)
                        ans.speed = 1.0f;
                }
            }

            if (animators != null)
            {
                for (int i = 0; i < animators.Length; ++i)
                {
                    Animator ani = animators[i];
                    
                    if (ani != null)
                    {
                        ani.speed = 1.0f;
                    }
                }
            }
            
        }
        

        void Init()
        {
            if (_rootObject != null)
            {
                _name            = _gameObjectPrefeb.name;
                _rootObject.name = "Effect_" + _gameObjectPrefeb.name;
                //_gameObject = (GameObject)PrefabUtility.InstantiatePrefab(_gameObjectPrefeb);
                _gameObject = GameObject.Instantiate(_gameObjectPrefeb);
                _gameObject.transform.SetParent(_rootObject.transform, true);
                pss = _gameObject.GetComponentsInChildren<ParticleSystem>(true);
                anis = _gameObject.GetComponentsInChildren<Animation>(true);
                frames = _gameObject.GetComponentsInChildren<FrameMaterials>(true);
                animators = _gameObject.GetComponentsInChildren<Animator>(true);
                //get all emitters we need to do scaling on
               // emitters    = _gameObject.GetComponentsInChildren<ParticleEmitter>();
                //psanimators   = _gameObject.GetComponentsInChildren<ParticleAnimator>();
                trails      = _gameObject.GetComponentsInChildren<TrailRenderer>();


                RemoveEmptyAnimator();

                _rootObject.transform.localPosition = transform.localPosition;
                _rootObject.transform.localRotation = transform.localRotation;
       
                Vector3 preScale = _rootObject.transform.localScale;
                _rootObject.transform.localScale = transform.localScale;
                Vector3 scale = _rootObject.transform.localScale;
                Vector3 scaleFactor = scale;
                scaleFactor.x /= preScale.x;
                scaleFactor.y /= preScale.y;
                scaleFactor.z /= preScale.z;
                UpdateParticleScale(scaleFactor);

                if(_parent)
                {
                    _rootObject.transform.SetParent(_parent.transform, false);
                }
             }
            if(pss == null) pss = new ParticleSystem[0];
            if(anis == null) anis = new Animation[0];
            if (frames == null) frames = new FrameMaterials[0];

            duration = SampleDuration();
            visiable = true;

            if (EditorApplication.isPlaying)
                Pause();
        }

        public float SampleDuration()
        {
            float d = 0;
            
            for (int i = 0; i < pss.Length; ++i)
            {
                float tmp = pss[i].duration + pss[i].startDelay + pss[i].startLifetime;
                if (tmp > d)
                    d = tmp;
            }

            for (int i = 0; i < anis.Length; ++i)
            {
                if (anis[i].clip != null && d < anis[i].clip.length)
                {
                    d = anis[i].clip.length;
                }
            }

            for (int i = 0; i < frames.Length; ++i)
            {
                if (frames[i]  != null && d < frames[i].Duration())
                {
                    d = frames[i].Duration();
                }
            }

            if(animators != null)
            {
                for(int i = 0; i < animators.Length; ++i)
                {
                    AnimatorClipInfo[]  clipinfo = animators[i].GetCurrentAnimatorClipInfo(0);

                    for(int j = 0; j < clipinfo.Length; ++j)
                    {
                        if( clipinfo[j].clip != null && d < clipinfo[j].clip.length )
                        {
                            d = clipinfo[j].clip.length;
                        }
                    }
                }
            }
            return d;
        }
        float fPreTime = 0.0f;
        public void Sampler(float fTime)
        {
            // if(_data.timetype == EffectTimeType.GLOBAL_ANIMATION)
            // {
            //     if(DSkillData.play == true)
            //     {
            //         fTime = DSkillData.time;
            //     }
            // }

            if(pss == null
                && anis == null && frames == null )
            {
                return;
            }

			if (EditorApplication.isPlaying)
	            Resume();


            for(int i = 0; i < pss.Length; ++i)
            {
                if(pss[i] != null)
                pss[i].Simulate(fTime);
            }

            for(int i = 0; i < anis.Length; ++i)
            {
                if (anis[i] != null && anis[i].clip)
                {
                    anis[i].clip.SampleAnimation(anis[i].gameObject, fTime);
                }
                
            }

            for(int i = 0; i < frames.Length; ++i)
            {
                if(frames[i])
                {
                    frames[i].Sampler(fTime);
                }
            }

            float fDelta = fTime - fPreTime;
            fPreTime = fTime;
            if (animators != null)
            {
                for (int i = 0; i < animators.Length; ++i)
                {
                    Animator animator = animators[i];
                    if (animator!= null)
                    {
                        animator.Update(fDelta);
                    }
                     
                } 
            }
            
			if (EditorApplication.isPlaying)
	            Pause();
        }

        public void Clear(bool bClearData)
        {
            if (_rootObject != null)
            {
                //if(EditorApplication.isPlayingOrWillChangePlaymode)
                //{
                //    Object.Destroy(_rootObject);
               // }
               // else
               // {
                    Object.DestroyImmediate(_rootObject);
              //  }
                
                _rootObject = null;
                _gameObject = null;
            }

            fPreTime = 0f;
            ClearCacheData();

            if (bClearData)
            {  
                duration = 0.0f;
            }
        }

        public bool IsShow
        {
            get { return visiable; }
        }

        public void Show(bool show)
        {
            if(visiable != show)
            {
                visiable = show;

                if(_gameObjectPrefeb && visiable)
                {
                    if(_rootObject == null)
                    {
                        _rootObject = new GameObject();
                        Init();
                    }
                }
                else if(visiable == false)
                {
                    Clear(false);
                }  
            }
        }


        void ScaleShurikenSystems(Vector3 scaleFactor)
        {
            if(pss != null)
            foreach (ParticleSystem system in pss)
            {
                system.startSpeed *= Mathf.Abs(scaleFactor.x);
                system.startSize *= Mathf.Abs(scaleFactor.x);
                system.gravityModifier *= Mathf.Abs(scaleFactor.x);
 
                //some variables cannot be accessed through regular script, we will acces them through a serialized object
                SerializedObject so = new SerializedObject(system);
                so.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor.x;
                so.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor.y;
                so.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor.z;
                so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= Mathf.Abs(scaleFactor.x);
                so.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor.x;
                so.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor.y;
                so.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor.z;
                so.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor.x;
                so.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor.y;
                so.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor.z;
                so.FindProperty("ColorBySpeedModule.range").vector2Value *= Mathf.Abs(scaleFactor.x);
                so.FindProperty("SizeBySpeedModule.range").vector2Value *= Mathf.Abs(scaleFactor.x);
                so.FindProperty("RotationBySpeedModule.range").vector2Value *= Mathf.Abs(scaleFactor.x);

                so.ApplyModifiedProperties();
                 
            }
        }

        void ScaleLegacySystems(float scaleFactor)
        {
            //apply scaling to emitters
            // if(emitters != null)
            // foreach (ParticleEmitter emitter in emitters)
            // {
            //     emitter.minSize *= scaleFactor;
            //     emitter.maxSize *= scaleFactor;
            //     emitter.worldVelocity *= scaleFactor;
            //     emitter.localVelocity *= scaleFactor;
            //     emitter.rndVelocity *= scaleFactor;

            //     //some variables cannot be accessed through regular script, we will acces them through a serialized object
            //     SerializedObject so = new SerializedObject(emitter);

            //     so.FindProperty("m_Ellipsoid").vector3Value *= scaleFactor;
            //     so.FindProperty("tangentVelocity").vector3Value *= scaleFactor;
            //     so.ApplyModifiedProperties();
            // }

            //apply scaling to animators
            // if(psanimators != null)
            // foreach (ParticleAnimator animator in psanimators)
            // {
            //     animator.force *= scaleFactor;
            //     animator.rndForce *= scaleFactor;
            // }
        }

        void ScaleTrailRenderers(float scaleFactor)
        {
            //apply scaling to animators
            if(trails != null)
            foreach (TrailRenderer trail in trails)
            {
                trail.startWidth *= scaleFactor;
                trail.endWidth *= scaleFactor;
            }
        }
    }

}

#endif
