using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComSetMaterial : MonoBehaviour
    {
        public string enableMaterial;
        public string disableMaterial;
        public Image[] imgs = new Image[0];

        Material _enableMaterial = null;
        Material _disableMaterial = null;

        void Awake()
        {
            _enableMaterial = ETCImageLoader.LoadMaterialFromSpritePath(enableMaterial);
            _disableMaterial = GeMaterialPool.instance.CreateMaterialInstance(disableMaterial);
        }

        public void SetMaterialEnable(bool bEnable)
        {
            for (int i = 0; i < imgs.Length;++i)
            {
                if(null != imgs[i])
                {
                    imgs[i].material = bEnable ? _enableMaterial : _disableMaterial;
                }
            }
        }

        void OnDestroy()
        {
            _enableMaterial = null;
            if(null != _disableMaterial)
            {
                GeMaterialPool.instance.RecycleMaterialInstance(disableMaterial, _disableMaterial);
                _disableMaterial = null;
            }
        }
    }
}