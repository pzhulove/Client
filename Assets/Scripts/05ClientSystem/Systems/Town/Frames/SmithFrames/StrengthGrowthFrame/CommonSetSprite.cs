using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonSetSprite : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private string onPath;
        [SerializeField] private string offPath;

        public void OnSetImage(bool isOn)
        {
            if(image != null)
            {
                if(isOn)
                {
                    ETCImageLoader.LoadSprite(ref image, onPath);
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref image, offPath);
                }
            }
        }
    }
}