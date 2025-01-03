using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAction : MonoBehaviour
{
    public enum TweenType
    {
        None,
        Color,
        Size,
    }

    public bool GuiQiShader;

    public string rootName;
    private GameObject rootTimeLine;
    private SpriteRenderer sr;
    public string animationName;
    public bool endHide;
    //public bool tweenHide;
    public TweenType tweenType;
    public float tweenHideTime;
    private Animator animator;
    private AnimationClip clip;
    AnimatorStateInfo animatorInfo;
    public float hideAfter;
    public string randomAnimationSpeed;
    public string delayActive;
    public float EffectTime;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rootTimeLine = GameObject.Find(rootName);
        if(randomAnimationSpeed!="")
        {
            animator.speed = Random.Range(float.Parse(randomAnimationSpeed.Split(',')[0]), float.Parse(randomAnimationSpeed.Split(',')[1]));
        }
        if(delayActive!="")
        {
            StartCoroutine(DelayActive());
        }
        if (0==EffectTime)
        {
            EffectTime = 5f;
        }
        //Invoke("DES", EffectTime);
    }
    IEnumerator DelayActive()
    {
        yield return new WaitForSeconds(Random.Range(float.Parse(delayActive.Split(',')[0]), float.Parse(delayActive.Split(',')[1])));
        sr.enabled = true;
        animator.enabled = true;
        yield break;
    }

    public void DES()
    {
        if(name=="Start")
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(endHide) //�����������������Ч
        {
            if(animator!=null)
            {
                if (AnimationDone(animationName))
                {
                    if (hideAfter <= 0)
                        gameObject.SetActive(false);
                    else
                    {
                        hideAfter -= Time.deltaTime;
                        if(tweenType == TweenType.Color&& hideAfter<=tweenHideTime)
                        {
                            sr.DOColor(new Color(1, 1, 1, 0), tweenHideTime);
                        }
                        if (tweenType == TweenType.Size && hideAfter <= tweenHideTime)
                        {
                            transform.parent.DOScale(Vector3.zero, tweenHideTime);
                        }

                        if(GuiQiShader)//������ʧshader
                        {
                            DoShader((tweenHideTime-hideAfter) / hideAfter);
                        }
                    }
                }
            }
            else
            {
                if (hideAfter <= 0)
                    gameObject.SetActive(false);
                else
                {
                    hideAfter -= Time.deltaTime;
                    if (tweenType == TweenType.Color && hideAfter <= tweenHideTime)
                    {
                        sr.DOColor(new Color(1, 1, 1, 0), tweenHideTime);
                    }
                    
                    if (tweenType == TweenType.Size && hideAfter <= tweenHideTime)
                    {
                        transform.parent.DOScale(Vector3.zero, tweenHideTime);
                    }

                    if (GuiQiShader)//������ʧshader
                    {
                        DoShader((tweenHideTime - hideAfter) / hideAfter);
                    }
                }
            }
        }
        else//����Ҫ�����������ִ������
        {
            if (hideAfter > 0)
            {
                hideAfter -= Time.deltaTime;
                if (tweenType == TweenType.Color && hideAfter <= tweenHideTime) //��ʧʱ����ٵ�ִ��tweenʱ���ʼִ��tween
                {
                    sr.DOColor(new Color(1, 1, 1, 0), tweenHideTime);
                }
                if (tweenType == TweenType.Size && hideAfter <= tweenHideTime)
                {
                    transform.parent.DOScale(Vector3.zero, tweenHideTime);
                }
            }
        }

        // if(animator)
        //     animator.SetFloat("GFXPos_Y", transform.localPosition.y);

    }

    bool AnimationDone(string name ="") //�ж϶����Ƿ񲥷���
    {
        animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(name=="")
        {
            return animatorInfo.normalizedTime > 0.99f;
        }
        else
        {
            return animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName(name);
        }
    }



    public void CreatEffect(AnimationEvent animationEvent)
    {
        Transform rootParent = transform;
        if (gameObject.name != "Start")
        {
            if (animationEvent.intParameter == 0)
            {
                while (rootParent.parent != null && rootParent.parent.name == "Start")
                {
                    Debug.LogError(rootParent.parent.name);
                    rootParent = rootParent.parent;
                }
            }
        }


        GameObject go = Instantiate(animationEvent.objectReferenceParameter as GameObject, rootParent);

        //if (go.transform.localScale.x <= 1)
        //    go.transform.localScale = Vector3.one;
        //go.transform.SetParent(rootParent);


        if (go.transform.localScale.x < 0)
        {
            go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        }
        string[] ate = animationEvent.stringParameter.Split('&');
        if (ate.Length<3)
        {
            return;
        }

        string pos_s = animationEvent.stringParameter.Split('&')[0];
        string rot_s = animationEvent.stringParameter.Split('&')[1];
        string sca_s = animationEvent.stringParameter.Split('&').Length >= 3 ? animationEvent.stringParameter.Split('&')[2] : "";

        Vector3 pos = new Vector3(float.Parse(pos_s.Split(',')[0]), float.Parse(pos_s.Split(',')[1]), float.Parse(pos_s.Split(',')[2]));
        Vector3 rot = new Vector3(float.Parse(rot_s.Split(',')[0]), float.Parse(rot_s.Split(',')[1]), float.Parse(rot_s.Split(',')[2]));
        if (sca_s != "")
        {
            go.transform.localScale = new Vector3(float.Parse(sca_s.Split(',')[0]), float.Parse(sca_s.Split(',')[1]), float.Parse(sca_s.Split(',')[2]));
        }

        go.transform.localPosition = pos;
        go.transform.localEulerAngles = rot;
        //Transform rootParent = transform;
        //if (animationEvent.intParameter == 0)
        //{
        //    while (rootParent.parent != null)
        //    {
        //        rootParent = rootParent.parent;
        //    }
        //}

        //GameObject go = Instantiate(animationEvent.objectReferenceParameter as GameObject);

        ////if (go.transform.localScale.x <= 1)
        ////    go.transform.localScale = Vector3.one;
        //go.transform.SetParent(rootParent);


        //if (go.transform.localScale.x < 0)
        //{
        //    go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        //}

        ////go.transform.localScale = new Vector3(Mathf.Abs(go.transform.localScale.x/1f), go.transform.localScale.y/1f, go.transform.localScale.z);

        ////if(rootTimeLine.transform.localScale.x<0)
        ////{

        ////}

        //if (animationEvent.stringParameter == "")
        //{
        //    if (animationEvent.intParameter != 1)
        //    {
        //        go.transform.localPosition = Vector3.zero;
        //        return;
        //    }
        //    else
        //    {
        //        go.transform.localPosition = transform.parent.localPosition;
        //        return;
        //    }
        //}


        //if (animationEvent.intParameter != 1)
        //{

        //    Vector3 pos = new Vector3(float.Parse(animationEvent.stringParameter.Split(',')[0]), float.Parse(animationEvent.stringParameter.Split(',')[1]), 0);
        //    go.transform.localPosition = pos;

        //}



    }
    public void CreateEffect_2(AnimationEvent animationEvent)
    {
        Transform rootParent = transform;
        if(gameObject.name!="Start")
        {
            if (animationEvent.intParameter == 0)
            {
                while (rootParent.parent != null && rootParent.parent.name == "Start")
                {
                    Debug.LogError(rootParent.parent.name);
                    rootParent = rootParent.parent;
                }
            }
        }


        GameObject go = Instantiate(animationEvent.objectReferenceParameter as GameObject, rootParent);

        //if (go.transform.localScale.x <= 1)
        //    go.transform.localScale = Vector3.one;
        //go.transform.SetParent(rootParent);


        if (go.transform.localScale.x < 0)
        {
            go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        }
        string pos_s = animationEvent.stringParameter.Split('&')[0];
        string rot_s = animationEvent.stringParameter.Split('&')[1];
        string sca_s = animationEvent.stringParameter.Split('&').Length >= 3 ? animationEvent.stringParameter.Split('&')[2] : "";

        Vector3 pos = new Vector3(float.Parse(pos_s.Split(',')[0]), float.Parse(pos_s.Split(',')[1]), float.Parse(pos_s.Split(',')[2]));
        Vector3 rot = new Vector3(float.Parse(rot_s.Split(',')[0]), float.Parse(rot_s.Split(',')[1]), float.Parse(rot_s.Split(',')[2]));
        if(sca_s!="")
        {
            go.transform.localScale =new Vector3(float.Parse(sca_s.Split(',')[0]), float.Parse(sca_s.Split(',')[1]), float.Parse(sca_s.Split(',')[2]));
        }

        go.transform.localPosition = pos;
        go.transform.localEulerAngles = rot;

    }


    public void CreateEffect_3(AnimationEvent animationEvent)
    {
        Transform rootParent = transform;
        if (gameObject.name != "Start")
        {
            if (animationEvent.intParameter == 0)
            {
                while (rootParent.parent != null && rootParent.parent.name == "Start")
                {
                    Debug.LogError(rootParent.parent.name);
                    rootParent = rootParent.parent;
                }
            }
        }

        GameObject go = Instantiate(animationEvent.objectReferenceParameter as GameObject);

        //if (go.transform.localScale.x <= 1)
        //    go.transform.localScale = Vector3.one;
        go.transform.SetParent(rootParent);


        if (go.transform.localScale.x < 0)
        {
            go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        }

        //go.transform.localScale = new Vector3(Mathf.Abs(go.transform.localScale.x/1f), go.transform.localScale.y/1f, go.transform.localScale.z);

        //if(rootTimeLine.transform.localScale.x<0)
        //{

        //}

        if (animationEvent.stringParameter == "")
        {
            if (animationEvent.intParameter != 1)
            {
                go.transform.localPosition = Vector3.zero;
                return;
            }
            else
            {
                go.transform.localPosition = transform.parent.localPosition;
                return;
            }
        }


        if (animationEvent.intParameter != 1)
        {

            Vector3 pos = new Vector3(float.Parse(animationEvent.stringParameter.Split(',')[0]), float.Parse(animationEvent.stringParameter.Split(',')[1]), 0);
            go.transform.localPosition = pos;

        }
    }


    public void TweenMove(AnimationEvent animationEvent)
    {

        Vector3 targetPos = new Vector3(float.Parse(animationEvent.stringParameter.Split(',')[0]), float.Parse(animationEvent.stringParameter.Split(',')[1]), float.Parse(animationEvent.stringParameter.Split(',')[2]));

        Vector3 worldPos = rootTimeLine.transform.TransformPoint(targetPos);

        Vector3 finalPos = transform.parent.InverseTransformPoint(worldPos);

        transform.DOLocalMove(finalPos, animationEvent.floatParameter);

        Debug.Log("�ƶ��ɹ�"+ targetPos + "  "+finalPos);


    }

    public void SetPos(string pos)
    {
        transform.localPosition = new Vector3(float.Parse(pos.Split(',')[0]), float.Parse(pos.Split(',')[1]), float.Parse(pos.Split(',')[2]));
    }
    public void SetSize(string size)
    {
        transform.localScale = new Vector3(float.Parse(size.Split(',')[0]), float.Parse(size.Split(',')[1]), float.Parse(size.Split(',')[2]));
    }
    public void TweenColor(float t)
    {
        GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 1, 0), t);
    }

    public void DoShader(float f)
    {
        Material m = sr.material;
        m.mainTexture = sr.sprite.texture;
        m.SetFloat("_ClipUvDown", f);

    }
}
