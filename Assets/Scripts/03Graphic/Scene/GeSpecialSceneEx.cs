using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// ���ⳡ������������
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
    /// ��ת�����ı��� -1������б 0��������б 1��������б
    /// </summary>
    public class GeSkyRotateEx : GeSpecialSceneEx
    {
        public Transform sky = null;    //��ת����Ԥ����
        public float angleSpeed = 0.05f;    //��ת�ٶ�
        public float maxRotateAngle = 15;   //�����ת�Ƕ�

        private int targetType;     //Ŀ����ת����
        private float curAngleSpeed = 0;    //��ǰ��ת�ٶ�
        private float curAngle = 0; //��ǰ��ת�Ƕ�

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
        /// ���õ�ǰ��ת����
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
        /// ������ת
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
        /// ��ȡ��ձ���
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