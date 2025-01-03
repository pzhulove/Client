using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;


public class ComDrugSubItem : MonoBehaviour, IDropHandler, IPointerUpHandler
{
    public ComDrugMain mainItem = null;

    public UnityEvent onClick = new UnityEvent();
    public UnityEvent onSelect = new UnityEvent();
    public UnityEvent onUnSelect = new UnityEvent();

    private enum eState
    {
       UnSelect = 0,
       Select
    }

    private eState mState = eState.UnSelect;

    public void OnDrop(PointerEventData data)
    {
        if (null == mainItem)
        {
            return;
        }

        if (data.pointerDrag == mainItem.gameObject)
        {
           OnEffect();
        }
    }

    public bool isSelect()
    {
        return mState == eState.Select;
    }
    public void OnEffect()
    {
        if(isSelect())
        {
            _onRealClick();
            UnSelect();
        }
    }
    public void Select()
    {
        if(mState == eState.Select)
        {
            return;
        }

        mState = eState.Select;

        onSelect.Invoke();
        mainItem.Mark();

    }
    public void OnPointerUp(PointerEventData data)
    {
        mainItem.UnMark();
    }

    private void _onRealClick()
    {
        if (null == mainItem)
        {
            return;
        }

        Logger.LogProcessFormat("[ComDrugSubItem] onClick");

        onClick.Invoke();

        mainItem.Hidden();
    }


    public void UnSelect()
    {
        if(mState == eState.UnSelect)
        {
            return;
        }

        mState = eState.UnSelect;

        onUnSelect.Invoke();
        mainItem.UnMark();
    }

    public void Show(bool enable)
    {
        if (null != this.gameObject)
        {
            gameObject.SetActive(enable);
        }
    }
}
