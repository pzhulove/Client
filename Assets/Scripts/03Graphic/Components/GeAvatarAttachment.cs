using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GeAttachDesc
{
    public string m_AttachmentRes = null;
    public string m_AttachNode = null;

    [System.NonSerialized]
    public GeAttach m_Attachment = null;
}

public class GeAvatarAttachment : MonoBehaviour
{
    [SerializeField]
    private string m_RefAvatar = null;

    [SerializeField]
    private GeAttachDesc[] m_AttachDescArray = new GeAttachDesc[0];

    public string refAvatar
    {
        set { m_RefAvatar = value; }
        get { return m_RefAvatar; }
    }

    public GeAttachDesc[] attachDescArray
    {
        set { m_AttachDescArray = value; }
        get { return m_AttachDescArray; }
    }
}
