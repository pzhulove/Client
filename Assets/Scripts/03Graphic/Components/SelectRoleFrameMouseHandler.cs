using UnityEngine;
using UnityEngine.EventSystems;
using GameClient;

public class SelectRoleFrameMouseHandler : MonoBehaviour , IDragHandler
{
    public void Bind(SelectRoleFrame selectRoleFrame,CreateRoleFrame createActorFrame)
    {
        m_SelectRoleFrame = selectRoleFrame;
        m_CreateRoleFrame = createActorFrame;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(null != m_SelectRoleFrame && eventData.dragging)
            m_SelectRoleFrame.OnDragSelectRole(eventData.delta.x * 0.6f);

        if (null != m_CreateRoleFrame && eventData.dragging)
            m_CreateRoleFrame.OnDragDemoRole(eventData.delta.x * 0.6f);
    }

    protected float m_Rotate = 0.0f;
    protected SelectRoleFrame m_SelectRoleFrame = null;
    protected CreateRoleFrame m_CreateRoleFrame = null;
}
