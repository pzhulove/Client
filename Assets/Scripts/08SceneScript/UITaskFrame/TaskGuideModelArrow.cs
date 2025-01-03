using UnityEngine;
using System.Collections;
using System;
using GameClient;

class TaskGuideModelArrow : MonoBehaviour
{
    GameObject[] goArrowArray = new GameObject[(Int32)TaskGuideArrow.TaskGuideDir.TGD_COUNT];
    Quaternion orgQuaterion = Quaternion.Euler(Vector3.zero);
    Vector3 orgPosition = Vector3.zero;
    ClientSystemTown currentSystem = null;
    BeTownPlayerMain mainPlayer = null;
    GameObject goActor = null;
    TaskGuideArrow.TaskGuideDir m_eDir = TaskGuideArrow.TaskGuideDir.TGD_INVALID;

    Int32 iNpcID = 0;
    float fMaxDistance = 5.20f;
    Vector3 target = Vector3.zero;
    bool bNeedUpdate = false;

    public void Initialize(BeTownPlayerMain mainPlayer)
    {
        try
        {
            this.mainPlayer = mainPlayer;
            goActor = transform.parent.gameObject;
            if(goActor != null)
            {
                string[] goNames = new string[(Int32)TaskGuideArrow.TaskGuideDir.TGD_COUNT] { "ArrowLeft", "ArrowRight", "ArrowTop", "ArrowBottom" };
                for (int i = 0; i < (Int32)TaskGuideArrow.TaskGuideDir.TGD_COUNT; ++i)
                {
                    goArrowArray[i] = Utility.FindChild(goNames[i], transform.gameObject);
                }
                orgQuaterion = transform.rotation;
                orgPosition = transform.position;
            }
            currentSystem = ClientSystem.GetTargetSystem<ClientSystemTown>();
        }
        catch(Exception e)
        {
            Logger.Log(e.ToString());
        }

        EndTrace();
    }

    public void BeginTrace(Int32 iNpcID,Vector3 target)
    {
        if(this.iNpcID != 0)
        {
            NpcArrowComponent.DeActiveNpcArrow(this.iNpcID);
            this.iNpcID = 0;
        }
        bNeedUpdate = true;
        this.target = target;
        this.iNpcID = iNpcID;
    }

    public void EndTrace()
    {
        bNeedUpdate = false;
        for (int i = 0; i < goArrowArray.Length; ++i)
        {
            if (goArrowArray[i] && goArrowArray[i].activeSelf)
            {
                goArrowArray[i].SetActive(false);
            }
        }
        NpcArrowComponent.DeActiveNpcArrow(iNpcID);
        iNpcID = 0;
        target = Vector3.zero;
    }

    void Update()
    {
        if (!bNeedUpdate)
        {
            return;
        }
        for (int i = 0; i < goArrowArray.Length; ++i)
        {
            if (goArrowArray[i] && goArrowArray[i].activeSelf)
            {
                goArrowArray[i].SetActive(false);
            }
        }

        if (null == mainPlayer)
            return;

        Vector3 dir = target - mainPlayer.ActorData.MoveData.Position;
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            if(dir.x > 0)
            {
                m_eDir = TaskGuideArrow.TaskGuideDir.TGD_RIGHT;
            }
            else
            {
                m_eDir = TaskGuideArrow.TaskGuideDir.TGD_LEFT;
            }
        }
        else
        {
            if(dir.z > 0)
            {
                m_eDir = TaskGuideArrow.TaskGuideDir.TGD_TOP;
            }
            else
            {
                m_eDir = TaskGuideArrow.TaskGuideDir.TGD_BOTTOM;
            }
        }

        transform.localScale = new Vector3(1.0f / goActor.transform.localScale.x, 1.0f, 1.0f);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = goActor.transform.position;

        if (m_eDir != TaskGuideArrow.TaskGuideDir.TGD_INVALID)
        {
            if (goArrowArray[(Int32)m_eDir] != null && !goArrowArray[(Int32)m_eDir].activeSelf)
            {
                goArrowArray[(Int32)m_eDir].SetActive(true);
            }
        }

        if(iNpcID != 0)
        {
            var currentTownNpc = currentSystem.GetTownNpcByNpcId(iNpcID);
            if(currentTownNpc != null && 
                Mathf.Abs((currentTownNpc.ActorData.MoveData.Position - mainPlayer.ActorData.MoveData.Position).x) 
                <= fMaxDistance)
            {
                for (int i = 0; i < goArrowArray.Length; ++i)
                {
                    if (goArrowArray[i])
                    {
                        goArrowArray[i].SetActive(false);
                    }
                }
                NpcArrowComponent.ActiveNpcArrow(iNpcID);
            }
            else
            {
                NpcArrowComponent.DeActiveNpcArrow(iNpcID);
            }
        }
    }
}