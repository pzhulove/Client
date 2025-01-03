using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.EventSystems;
using ProtoTable;

public class OnNpcHit : MonoBehaviour
{
    void Update()
    {
        if (!GameFrameWork.IsGameFrameWorkInited)
            return;

        if (!(ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown) && !(ClientSystemManager.GetInstance().CurrentSystem is ClientSystemGameBattle))
        {
            return;
        }

        bool bPick = false;

        if(Input.GetMouseButtonDown(0))
		{
			// Check if the mouse was clicked over a UI element
			if(null != EventSystem.current && EventSystem.current.IsPointerOverGameObject())
			{
                bPick = false;
				return;
			}

            bPick = true;
		}	

        // Check if there is a touch
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			// Check if finger is over a UI element
			if(null != EventSystem.current && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
			{
                bPick = false;
                return;
				//Debug.Log("Touched the UI");
			}

            bPick = true;
		}	

        if(bPick)
        {
            Camera cameraMain = Camera.main;
            if(null != cameraMain)
            {
                Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
                {
                    if(null != hitInfo.collider)
                    {
                        Transform gameObj = hitInfo.collider.transform;
                        if (gameObj != null)
                        {
                            RaycastObject raycastObject = gameObj.GetComponent<RaycastObject>();
                            if (raycastObject != null)
                            {
                                if (raycastObject.ObjectType == RaycastObject.RaycastObjectType.ROT_NPC)
                                {
                                    TaskNpcAccess.OnClickNpc(raycastObject.Data as BeTownNPCData);
                                }
                                else if (raycastObject.ObjectType == RaycastObject.RaycastObjectType.ROT_TOWNPLAYER)
                                {
                                    var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                                    if (current != null)
                                    {
                                        CitySceneTable tableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(current.CurrentSceneID);
                                        if (tableData != null)
                                        {
                                            TaskNpcAccess.OnClickFightPlayer(raycastObject.Data as BeFighterData, tableData.SceneType, hitInfo.transform);
                                        }
                                    }
                                    else
                                    {
                                        TaskNpcAccess.OnClickTownPlayer(raycastObject.Data as BeTownPlayerData, hitInfo.transform);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}