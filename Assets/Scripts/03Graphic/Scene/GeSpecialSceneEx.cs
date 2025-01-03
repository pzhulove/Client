using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 特殊场景操作处理类
    /// </summary>
    public class GeSpecialSceneEx
    {
        protected GameObject m_SceneObject;
        
        public void Init(GameObject go)
        {
            m_SceneObject = go;
            OnInit();
        }

        public void Update(int delta)
        {
            OnUpdate(delta);
        }

        protected virtual void OnInit() { }
        protected virtual void OnUpdate(int delta) { }
        protected virtual void OnDeInit() { }
    }

    /// <summary>
    /// 旋转场景的背景 -1代表不倾斜 0代表左倾斜 1代表右倾斜
    /// </summary>
    public class GeSkyRotateEx : GeSpecialSceneEx
    {
        public Transform sky = null;    //旋转背景预制体
        public float angleSpeed = 0.05f;    //旋转速度
        public float maxRotateAngle = 15;   //最大旋转角度

        private int targetType;     //目标旋转类型
        private float curAngleSpeed = 0;    //当前旋转速度
        private float curAngle = 0; //当前旋转角度

        protected override void OnInit()
        {
            base.OnInit();
            InitSceneSky();
        }

        protected override void OnUpdate(int deltaTime)
        {
            base.OnUpdate(deltaTime);
            UpdateSkyRotate();
        }

        protected override void OnDeInit()
        {
            base.OnDeInit();
        }

        /// <summary>
        /// 设置当前旋转类型
        /// </summary>
        /// <param name="type"></param>
        public void SetSkyRotateData(int type)
        {
            targetType = type;
            switch (type)
            {
                case -1:
                    {
                        if (curAngle >= 0)
                        {
                            curAngleSpeed = -angleSpeed;
                        }
                        else if (curAngle < 0)
                        {
                            curAngleSpeed = angleSpeed;
                        }
                    }
                    break;
                case 0:
                    {
                        curAngleSpeed = angleSpeed;
                    }
                    break;
                case 1:
                    {
                        curAngleSpeed = -angleSpeed;
                    }
                    break;
            }
        }

        /// <summary>
        /// 更新旋转
        /// </summary>
        private void UpdateSkyRotate()
        {
            if (sky == null)
                return;
            if (targetType == -1 && curAngle >= -0.1 && curAngle <= 0.1)
            {
                curAngle = 0;
                return;
            }

            if (targetType == 0 && curAngle > maxRotateAngle)
            {
                return;
            }

            if (targetType == 1 && curAngle <= -maxRotateAngle)
            {
                return;
            }

            curAngle += curAngleSpeed;
            sky.localRotation = Quaternion.Euler(0, 0, curAngle);
        }

        /// <summary>
        /// 获取天空背景
        /// </summary>
        private void InitSceneSky()
        {
            if (sky != null)
                return;
            if (m_SceneObject == null)
                return;
            sky = m_SceneObject.transform.Find("Model/Sky/Tb_sky01");
        }
    }
}