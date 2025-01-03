#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Utils;

namespace DSkillEditor
{
    public class DEditorAttach
    {
        protected GameObject _AttachPoint;
        protected GameObject _rootObject;
        protected GameObject _gameObjectPrefab;
        protected GameObject _gameObject;
        protected GameObject _parent;
        protected EntityAttachFrames _data;
        protected bool _show = false;
        protected float _pretime = 0.0f;

        protected Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
        protected string[] anims = new string[0];

        private GeAnimDesc[] animClips = new GeAnimDesc[0];

        public bool HaveAnim
        {
            get { return anims.Length > 0; }
        }

        public string[] animsarray
        {
            get { return anims; }
        }

        public AnimationClip Clip(string key)
        {
            AnimationClip v = null;
            if( clips.TryGetValue(key, out v) )
            {
                return v;
            }

            return null;
        }

        public int FindIndex(string name)
        {
            for (int i = 0; i < anims.Length; ++i)
            {
                if( name == anims[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public void UpdateAttach(ref List<string> names,ref Dictionary<string,GameObject> ps)
        {
            if(_gameObject)
            {
                Transform[] trans = _gameObject.GetComponentsInChildren<Transform>();
                string key;
                foreach (Transform t in trans)
                {
                    if (t.gameObject.CompareTag("Dummy"))
                    {
                        key = "["+  _data.name + "]" + t.gameObject.name;
                        names.Add(key);
                        ps.Add(key, t.gameObject);
                    }
                }
            }
        }

        protected void InitAnim()
        {
            clips.Clear();
            List<string> namelist = new List<string>();

            if(_gameObject)
            {
                GeAnimDescProxy animProxy = _gameObject.GetComponent<GeAnimDescProxy>();
                if(null != animProxy)
                {
                    animClips = animProxy.animDescArray;
                    if (null != animClips)
                    {
                        for (int i = 0, icnt = animClips.Length; i < icnt; ++i)
                        {
                            animProxy.PreloadAction(animClips[i].animClipName);

                            clips.Add(animClips[i].animClipName,animClips[i].animClipData);
                            namelist.Add(animClips[i].animClipName);
                        }
                    }
                }
                else
                {
                    Animation animations = _gameObject.GetComponent<Animation>();
                    if (animations)
                    {
                        if (animations != null || animations.GetClipCount() > 0)
                        {
                            foreach (AnimationState ans in animations)
                            {
                                namelist.Add(ans.name);
                                clips.Add(ans.name, ans.clip);
                            }
                        }
                    }
                }
            }

            anims = namelist.ToArray();
        }
        public DEditorAttach(EntityAttachFrames _data)
        {
            this._data = _data;
        }

        public GameObject RootObject
        {
            get { return _rootObject;  }
        }

        public void UpdateParent(GameObject obj)
        {
            _parent = obj;
        }

        public bool Visiable
        {
            get { return _show; }
        }

        public bool Show(bool bShow)
        {
            if(_show != bShow)
            {
                if(bShow)
                {
                    Create();
                }
                else
                {
                    Destroy();
                }

                return true;
            }

            return false;
        }
        public GameObject ObjectPrefab
        {
            set
            {
                if(_gameObjectPrefab != value)
                {
                    _gameObjectPrefab = value;
                    Destroy();
                    if(_show)
                    Create();
                }
            }

            get
            {
                return _gameObjectPrefab;
            }
        }
        public void Destroy()
        {
            if (_AttachPoint)
            {
                _AttachPoint.transform.SetParent(null, false);
                Editor.DestroyImmediate(_AttachPoint);
            }
            _AttachPoint = null;
            _gameObject = null;
            _rootObject = null;
            _show       = false;
            InitAnim();
        }

        void Create()
        {
            if(_gameObjectPrefab)
            {
                if(_rootObject == null)
                {
                    string p_name = "";
                    if (_parent != null) p_name = _parent.name;

                    _AttachPoint = new GameObject();
                    _rootObject = new GameObject();
                    _rootObject.name = "Att[" + p_name + "]" + _gameObjectPrefab.name;
                    _AttachPoint.name = _rootObject.name;
                    _gameObject = (GameObject)PrefabUtility.InstantiatePrefab(_gameObjectPrefab);
                    _gameObject.transform.SetParent(_rootObject.transform, false);
                    _rootObject.transform.SetParent(_AttachPoint.transform, false);
                    _AttachPoint.transform.SetParent(DSkillData.attach.transform, false);
                }
            }
            _show = true;
            _pretime = 0.0f;
            InitAnim();
        }
        public void Sampler(float fTime,float fps)
        {
            if(_show == false)
            {
                Create();
            }

            if(_AttachPoint && _parent)
            {
                var matrix = _parent.transform.localToWorldMatrix;
                if (matrix.m03 > 500)
                    matrix.m03 -= 1000;
                
                _AttachPoint.transform.GetTransformFromMatrix(matrix);

                //_AttachPoint.transform.localPosition = Utils.TransformUtil.GetTranslation(_parent.transform.localToWorldMatrix);
                //_AttachPoint.transform.localRotation = Utils.TransformUtil.GetRotation(_parent.transform.localToWorldMatrix);
                //_AttachPoint.transform.localScale = Utils.TransformUtil.GetScale(_parent.transform.localToWorldMatrix);
                /*
                _AttachPoint.transform.localPosition =
                 _parent.transform.position;

                _AttachPoint.transform.localRotation =
                _parent.transform.rotation;

                _AttachPoint.transform.localScale =
                _parent.transform.lossyScale;
                */
            }

 

            AnimationClip curClip = null;
            float curTime = 0.0f;
            float speed = 1.0f;

            //if (fTime >= _pretime)
            {
                for(int j = 0; j < _data.animations.Length; ++j)
                {
                    AnimationFrames frames = _data.animations[j];
                    float start = frames.start / fps;

                    if (start <= fTime)
                    {
                        if( curClip == null
                            || curTime < start)
                        {
                            curTime = start;
                            curClip = frames.clip;
                            speed = frames.speed;
                        }
                    }
                }
            }

            if (curClip != null && _gameObject != null)
            {
                curClip.SampleAnimation(_gameObject, (fTime - curTime) * speed);
            }

            _pretime = fTime;
        }
    }
}
#endif