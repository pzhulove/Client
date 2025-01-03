using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform),typeof(LayoutElement))]
public class TittleAniSizeConvert : MonoBehaviour
{
    public Transform target;
    public Vector3 kScale = new Vector3(150, 150,1.0f);
    bool bInit = false;
    SpriteRenderer spriteRender;
    Animator animator;
    TittleHelpComponent tittleHelpComponent;
    LayoutElement kLayoutElement;
    string stopName = "";
    // Use this for initialization
    void Start ()
    {
        Initialize();
    }

    void FromScale(float fNewScaleX = 1.0f,float fNewScaleY = 1.0f)
    {
        if(target.transform.localScale.x < 25.0f)
        {
            kScale.x = 150.0f * fNewScaleX;
        }
        else
        {
            kScale.x = target.transform.localScale.x * fNewScaleX;
        }
        if(target.transform.localScale.y < 25.0f)
        {
            kScale.y = 150.0f * fNewScaleY;
        }
        else
        {
            kScale.y = target.transform.localScale.y * fNewScaleY;
        }

        target.transform.localScale = kScale;
    }

    public float GetAnimationLength()
    {
        if (animator != null)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            if(clips != null && clips.Length > 0)
            {
                return clips[0].length;
            }
        }
        return 0.0f;
    }

    public void Initialize()
    {
        if(!bInit)
        {
            if (target != null)
            {
                spriteRender = target.GetComponent<SpriteRenderer>();
                animator = target.GetComponent<Animator>();
                tittleHelpComponent = target.GetComponent<TittleHelpComponent>();
                if (spriteRender != null)
                {
                    spriteRender.sortingOrder = 100;
                }
                FromScale();
            }
            kLayoutElement = GetComponent<LayoutElement>();
            bInit = true;
        }
    }

    public void ResetTarget(Transform target,float fScaleX = 1.0f,float fScaleY = 1.0f, int sortingOrder = 100)
    {
        if (this.target != target)
        {
            this.target = target;
            spriteRender = null;
            if (target != null)
            {
                spriteRender = target.GetComponent<SpriteRenderer>();
                animator = target.GetComponent<Animator>();
                tittleHelpComponent = target.GetComponent<TittleHelpComponent>();
                if (spriteRender != null)
                {
                    spriteRender.sortingOrder = sortingOrder;
                }
                FromScale(fScaleX, fScaleY);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (spriteRender != null)
        {
            if (kLayoutElement != null && spriteRender.sprite != null)
            {
                kLayoutElement.preferredWidth = spriteRender.sprite.bounds.size.x * kScale.x;
                kLayoutElement.preferredHeight = spriteRender.sprite.bounds.size.y * kScale.y;
            }
        }
        else if(tittleHelpComponent != null)
        {
            if(kLayoutElement != null && kLayoutElement != null)
            {
                kLayoutElement.preferredWidth = kScale.x * tittleHelpComponent.Bounds.x;
                kLayoutElement.preferredHeight = kScale.y * tittleHelpComponent.Bounds.y;
            }
        }
    }
}
