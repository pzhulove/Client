using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    class ComWhiteCircle : MonoBehaviour
    {

      ComMapScene m_scene = null;    // Use this for initialization
      RectTransform imageTrans;
      Image m_Image;
      public void Setup(Vector2 pos,float radius, ComMapScene a_comScene)
      {
            m_scene = a_comScene;
        //    gameObject.transform.localPosition = new Vector3(pos.x * m_scene.XRate, pos.y * m_scene.ZRate, 0.0f);
            if (imageTrans == null)
            {
                m_Image = GetComponent<Image>();
                imageTrans = m_Image.GetComponent<RectTransform>();
            }
            imageTrans.anchoredPosition = new Vector2(pos.x * m_scene.XRate, pos.y * m_scene.ZRate);
            imageTrans.sizeDelta = new Vector2(radius * 12.8f, radius * 10);
       }
    }
}
