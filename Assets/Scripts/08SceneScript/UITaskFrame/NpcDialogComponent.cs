using System;
using UnityEngine;
using System.Collections;
using GameClient;

public class NpcDialogComponent : MonoBehaviour
{
    static float ms_common_interval = 30.0f;
    GameObject goChildText = null;
    LinkParse uiText = null;
    float fLastTick = 0;
    float fBeginTick = 0;
    float fTickInterval = 3.0f;
    int iTickPower = 0;
    string[] contents = new string[] { };
    Int32 iCurIndex = 0;
    ProtoTable.NpcTable npcTable = null;
    ProtoTable.NpcTable.eDialogType eDialogShowType = ProtoTable.NpcTable.eDialogType.trival;
    Int32 iNpcID = 0;
    public enum IdBelong2
    {
        IdBelong2_NpcTable = 0,
        IdBelong2_UnitTable,
        IdBelong2_FollowPetTable,
    }
    IdBelong2 eIdBelong2 = IdBelong2.IdBelong2_NpcTable;
    bool m_bInterval = false;
    public Int32 ID
    {
        get { return iNpcID; }
    }

    public float NextTick
    {
        get { return fLastTick + fTickInterval; }
    }

    public int TickPower
    {
        get
        {
            return iTickPower;
        }
    }

    public void BeginTick()
    {
        ++iTickPower;
        if (contents.Length > 0)
        {
            fBeginTick = Time.time;

            transform.gameObject.CustomActive(true);
            if (eDialogShowType == ProtoTable.NpcTable.eDialogType.random)
            {
                iCurIndex = UnityEngine.Random.Range(0, contents.Length - 1);
            }
            else if (eDialogShowType == ProtoTable.NpcTable.eDialogType.trival)
            {
                iCurIndex = (iCurIndex + 1) % (contents.Length);
            }
            if(uiText != null)
            {
                uiText.SetText(contents[iCurIndex], false);
            }
            m_bInterval = true;
        }
    }

    public void EndTick()
    {
        fLastTick += fTickInterval;
        //transform.gameObject.CustomActive(false);
    }

    public bool InTick
    {
        get { return m_bInterval; }
    }

    public void Initialize(Vector3 worldPos, Int32 iNpcID, IdBelong2 eIdBelong2 = IdBelong2.IdBelong2_NpcTable)
    {
        eDialogShowType = ProtoTable.NpcTable.eDialogType.trival;
        goChildText = transform.Find("content").gameObject;
        uiText = goChildText.GetComponent<LinkParse>();
        transform.gameObject.SetActive(false);
        iCurIndex = 0;
        fLastTick = 0;

        if (eIdBelong2 == IdBelong2.IdBelong2_NpcTable)
        {
            this.iNpcID = iNpcID;
            npcTable = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
            if (npcTable != null)
            {
                contents = GetContents(npcTable.TalkContent);
                this.fTickInterval = npcTable.Interval * 1.0f / 1000.0f;
            }
        }
        else if (eIdBelong2 == IdBelong2.IdBelong2_FollowPetTable)
        {
//             this.iNpcID = iNpcID;
//             var table = TableManager.GetInstance().GetTableItem<ProtoTable.FollowPetTable>(iNpcID);
//             if (table != null)
//             {
//                 contents = GetContents("");
//                 this.fTickInterval = 2000.0f / 1000.0f;
//             }
        }

        UpdateWorldPosition(worldPos);
        m_bInterval = false;
    }

    public void UpdateWorldPosition(Vector3 worldPos)
    {
        if (worldPos != Vector3.zero)
        {
            transform.position = worldPos;

            //if (transform.parent == null)
            //{
            //    transform.localPosition = worldPos;
            //}
            //else
            //{
            //    transform.localPosition = transform.parent.InverseTransformPoint(worldPos);
            //}
        }
    }

    public bool Tick(float fDelta)
    {
        if (m_bInterval)
        {
            if (fBeginTick + fTickInterval + ms_common_interval < Time.time)
            {
                m_bInterval = false;
            }
            else if(fBeginTick + fTickInterval < Time.time)
            {
                transform.gameObject.CustomActive(false);
            }
        }

        return m_bInterval;
    }


    public void SetContentText()
    {
        if (contents.Length > 0)
        {
            if (uiText != null)
            {
                uiText.SetText(contents[0], false);
            }
        }
    }

    string[] GetContents(string orgContent)
    {
        string[] akContents = new string[] { };
        if (orgContent != null && orgContent.Length > 0)
        {
            akContents = orgContent.Split(new char[] { '\r', '\n' });
        }
        return akContents;
    }
}
