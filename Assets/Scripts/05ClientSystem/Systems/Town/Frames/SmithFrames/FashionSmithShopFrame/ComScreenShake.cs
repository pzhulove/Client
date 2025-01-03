using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class ComScreenShake : MonoBehaviour
    {
        public float len = 1.0f;
        float shakeTime = 1.0f;
        public float fps = 20.0f;
        public float frameTime = 0.00f;
        public float shakeDelta = 0.005f;
        public Camera cam;
        public bool isshakeCamera = false;
        public void Shake()
        {
            isshakeCamera = true;
            shakeTime = len;
        }

        void Start()
        {
            if (cam == null)
            {
                GameObject goItem = GameObject.Find("UIRoot/UICamera");
                if (null != goItem)
                    cam = goItem.GetComponent<Camera>();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                isshakeCamera = !isshakeCamera;
            }
            if (isshakeCamera && null != cam)
            {
                if (shakeTime > 0)
                {
                    shakeTime -= Time.deltaTime;
                    if (shakeTime <= 0)
                    {
                        cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                        isshakeCamera = false;
                        shakeTime = len;
                        frameTime = 0.03f;
                    }
                    else
                    {
                        frameTime += Time.deltaTime;
                        if (frameTime > 1.0 / fps)
                        {
                            frameTime = 0;
                            //cam.rect = new Rect(shakeDelta * (-1.0f + 2.0f * Random.value), shakeDelta * (-1.0f + 2.0f * Random.value), 1.0f, 1.0f);
                            float tarV = 1.0f + shakeDelta * Random.value;
                            cam.rect = new Rect(0.0f, 0.0f, tarV, tarV);
                        }
                    }
                }
            }
        }
    }
}