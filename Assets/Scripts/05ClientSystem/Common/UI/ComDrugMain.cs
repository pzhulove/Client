
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ComDrugMain : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{


    public ComDrugSubItem[] mSubDrugs = new ComDrugSubItem[0];

    public UnityEvent onClick = new UnityEvent();

    private enum eState
    {
        onOpen,
        onClose,
    }

    private eState mState = eState.onClose;

    private void Awake()
    {
        _updateSubDrugStatus();
    }

    private void _updateSubDrugStatus()
    {
        for (int i = 0; i < mSubDrugs.Length; ++i)
        {
            if (null != mSubDrugs[i])
            {
                mSubDrugs[i].Show(eState.onOpen == mState);
            }
        }
    }

    private void _changeState()
    {
        switch (mState)
        {
            case eState.onClose:
                mState = eState.onOpen;
                break;
            case eState.onOpen:
                mState = eState.onClose;
                mIsMarkd = false;
                break;
        }
    }


    private bool mIsMarkd = false;
    public void Mark()
    {
        mIsMarkd = true;
    }

    public void UnMark()
    {
        mIsMarkd = false;
    }

    public void Hidden()
    {
        mState = eState.onClose;
        _updateSubDrugStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (null != onClick)
        {
            onClick.Invoke();
        }
        _changeState();
        _updateSubDrugStatus();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null && eventData.pointerEnter != gameObject)
        {
            var cur = eventData.pointerEnter.GetComponentInParent<ComDrugSubItem>();
            for (int i = 0; i < mSubDrugs.Length; ++i)
            {
                var it = mSubDrugs[i];

                if (cur == it)
                {
                    it.Select();
                }
                else
                {
                    it.UnSelect();
                }
            }
        }
        else
        {
            for (int i = 0; i < mSubDrugs.Length; ++i)
            {
                var it = mSubDrugs[i];


                it.UnSelect();
            }
        }
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Hidden();

        for (int i = 0; i < mSubDrugs.Length; ++i)
        {
            var it = mSubDrugs[i];

            if (it.isSelect())
            {
                it.OnEffect();
            }
            it.UnSelect();
        }
    }
}
