#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace DSkillEditor
{
    public class DEditorEntity
    {
        protected GameObject _rootObject;
        protected GameObject _gameObjectPrefab;
        protected GameObject _gameObject;
        protected bool _bCreate = false;

 
        public float     timelength = 1.0f;
        protected EntityFrames _data;

        public DEditorEntity(EntityFrames data)
        {
            _data = data;
        }

        public GameObject RootObject
        {
            get { return _rootObject;  }
        }

        public void Show(bool bShow)
        {
            if(_bCreate != bShow)
            {
                if(bShow)
                {
                    Create();
                }
                else
                {
                    Destroy();
                }
            }
        }
        public GameObject ObjectPrefab
        {
            set
            {
                if(_gameObjectPrefab != value)
                {
                    _gameObjectPrefab = value;
                    Destroy();
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
            if (_rootObject)
            {
                _rootObject.transform.SetParent(null, true);
                Editor.DestroyImmediate(_rootObject);
            }
            _gameObject = null;
            _rootObject = null;
            _bCreate = false;
        }

        void Create()
        {
            if(_gameObjectPrefab)
            {
                if(_rootObject == null)
                {
                    _rootObject = new GameObject();
                    _rootObject.name = "Entity_" + _gameObjectPrefab.name;
                    _gameObject = (GameObject)PrefabUtility.InstantiatePrefab(_gameObjectPrefab);
                    _gameObject.transform.SetParent(_rootObject.transform, true);
                    _rootObject.transform.SetParent(DSkillData.actor.transform, false);
                }
            }
            _bCreate = true;
        }
        public void Sampler(float fTime)
        {
            if(_bCreate == false)
            {
                Create();
            }
            if(fTime > timelength)
            {
                fTime = timelength;
            }

            if(fTime < 0)
            {
                fTime = 0;
            }

            float degree = _data.angle / (2 * Mathf.PI);
            Vector2 speedv;
            speedv.x = _data.speed * Mathf.Cos(degree);
            speedv.y = _data.speed * Mathf.Sin(degree);

            //  s = v * t + 1/2 * a * t~2 
            float fTime2 = fTime * fTime;


            if(_rootObject)
            {
                if (_data.axisType == AxisType.Y)
                {
                    float radian = _data.angle * Mathf.Deg2Rad;
                    speedv.x = _data.speed * Mathf.Cos(radian);
                    speedv.y = _data.speed * Mathf.Sin(radian);
                    float x = speedv.x * fTime + 0.5f * _data.gravity.x * fTime2;
                    float z = speedv.y * fTime;
                    float y = _data.emitposition.y - 0.5f * _data.gravity.y * fTime2;
                    _rootObject.transform.localPosition = new Vector3(_data.emitposition.x + x, y, _data.emitPositionZ + z);
                }
                else
                {
                    Vector2 deltaPos = speedv * fTime + 0.5f * new Vector2(_data.gravity.x, -_data.gravity.y) * fTime2;
                    Vector2 newpos = _data.emitposition + deltaPos;
                    _rootObject.transform.localPosition = new Vector3(newpos.x, newpos.y, _data.emitPositionZ);
                }
                if (_data.isAngleWithEffect)
                {
                    float dangle = 0;
                    if (speedv.y != 0)
                        dangle = Vector2.Angle(speedv, new Vector2(1, 0));
                    if (speedv.y < 0)
                    {
                        dangle = -dangle;
                    }

                    if (speedv.x == 0 && speedv.y == 0)
                        dangle = 0;

                    _rootObject.transform.localRotation = Quaternion.AngleAxis(dangle, Vector3.forward);
                }
            }
        }
    }
}
#endif