using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using GameClient;
using UnityEngine.Events;

public class VanityBuffBonusView : MonoBehaviour {
    
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text des;
    [SerializeField]
    private GameObject mPosRoot;
    [SerializeField]
    private Vector3 mScaleVec3;
   
    private Vector3 mPosVec3;

    public void Init(VanityBuffBonusModel data)
    {
        if (data == null)
        {
            return;
        }
        if (icon != null)
        {
            ETCImageLoader.LoadSprite(ref icon, data.iconPath);
        }
        
        if (des != null)
        {
            des.text = data.des.ToString();
        }

        mPosVec3 = data.pos;
    }

    public void PlayAnimation()
    {
        Invoke("PlayPosMoveScaleAnimation", 1f);
    }
    
    void PlayPosMoveScaleAnimation()
    {
        RectTransform rect = mPosRoot.GetComponent<RectTransform>();

        DOTween.To(() => rect.position, r =>
        {
            rect.position = r;
        }, mPosVec3, 1f).SetEase(Ease.OutQuart);

        DOTween.To(() => rect.localScale, r =>
        {
            rect.localScale = r;
        }, mScaleVec3, 1f).SetEase(Ease.OutQuart).OnComplete(()=> {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VanityBonusAnimationEnd);
        });
    }
}
