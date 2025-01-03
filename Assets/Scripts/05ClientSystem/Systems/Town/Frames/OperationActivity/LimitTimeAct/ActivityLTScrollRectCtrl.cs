using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActivityLTScrollRectCtrl : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public enum DragDirection
    {
        Ready,
        None,
        Horizonal,
        Vertical
    }
    public event System.Action<DragDirection> OnDragDirHandler;
    private DragDirection currDragDir;
    public DragDirection CurrDragDir
    {
        get { return currDragDir; }
    }


    private Canvas horizonScrollCanvas;
    private RectTransform horizonScrollRect;
    private Image viewportImage;

    private Vector2 mousePosOnRect;
    private Vector2 startPos = Vector2.zero;
   
    public float touchMoveDistance = 10f;

    void Start()
    {
        horizonScrollCanvas = GameObject.FindObjectOfType<Canvas>();
        horizonScrollRect = this.gameObject.GetComponent<RectTransform>();
        viewportImage = this.gameObject.GetComponentInChildren<Image>();
    }

    void Update()
    {
        #if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currDragDir = DragDirection.Ready;
            if (OnDragDirHandler != null)
                OnDragDirHandler(currDragDir);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 currVec = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - startPos;
            OnDragMove(currVec);

        }
        else if (Input.GetMouseButtonUp(0))
        {
            currDragDir = DragDirection.None;
            if (OnDragDirHandler != null)
                OnDragDirHandler(currDragDir);
        }
        
        #elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                currDragDir = DragDirection.Ready;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                OnDragMove(touch.deltaPosition);
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                currDragDir = DragDirection.None;
            }

            if (OnDragDirHandler != null)
                OnDragDirHandler(currDragDir);
        }

        #endif

      
       

        /*
        if (Input.touchCount > 1)
        {
            Touch touch = Input.GetTouch(0);
            if(horizonScrollRect != null && horizonScrollCanvas !=null)
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(horizonScrollRect, touch.deltaPosition, horizonScrollCanvas.worldCamera, out mousePosOnRect))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    currDragDir = DragDirection.Ready;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    OnDragMove(touch.deltaPosition);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    currDragDir = DragDirection.None;
                }

                if (OnDragDirHandler != null)
                    OnDragDirHandler(currDragDir);
            }
        }*/
    }

    public Image GetScrollViewportImg()
    {
        return viewportImage;
    }

    private void OnDragMove(Vector2 deltaPos)
    {
        var dotWithUpVec = Vector2.Dot(deltaPos.normalized, Vector2.up);
        if (dotWithUpVec > 0.7f ||
            dotWithUpVec <  -0.7f)
        {
            currDragDir = DragDirection.Vertical;
           // Logger.Log("vertical...");

        }
        else if ((dotWithUpVec < 0.7f && dotWithUpVec >= 0f) ||
            (dotWithUpVec > -0.7f && dotWithUpVec <= 0f))
        {
            currDragDir = DragDirection.Horizonal;
            //Logger.Log("horizonal...");
        }

        if (OnDragDirHandler != null)
            OnDragDirHandler(currDragDir);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var dotWithUpVec = Vector2.Dot(eventData.delta.normalized, Vector2.up);
        if (dotWithUpVec > 0.7f ||
            dotWithUpVec <  -0.7f)
        {
            currDragDir = DragDirection.Vertical;
            
        }
        else if ((dotWithUpVec < 0.7f && dotWithUpVec > 0f) ||
          (dotWithUpVec > -0.7f && dotWithUpVec < 0f))
        {
            currDragDir = DragDirection.Horizonal;
        }
        if (OnDragDirHandler != null)
            OnDragDirHandler(currDragDir);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currDragDir = DragDirection.Ready;
        if (OnDragDirHandler != null)
            OnDragDirHandler(currDragDir);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        currDragDir = DragDirection.None;
        if (OnDragDirHandler != null)
            OnDragDirHandler(currDragDir);
    }

    void OnDisable()
    {
        currDragDir = DragDirection.None;
        if (OnDragDirHandler != null)
            OnDragDirHandler(currDragDir);
        RemoveAllDragDirListener();
    }

    public void AddDragDirListener(System.Action<DragDirection> handler)
    {
        RemoveAllDragDirListener();
        if (OnDragDirHandler == null)
            OnDragDirHandler += handler;
    }

    public void RemoveAllDragDirListener()
    {
        if (OnDragDirHandler != null)
        {
            foreach (System.Delegate d in OnDragDirHandler.GetInvocationList())
            {
                OnDragDirHandler -= d as System.Action<DragDirection>;
            }
        }
    }
}
