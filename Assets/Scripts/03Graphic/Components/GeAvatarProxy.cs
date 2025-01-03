using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeAvatarProxy : MonoBehaviour
{
    [SerializeField]
    protected Object m_Avatar = null;

    [SerializeField]
    protected int[] m_SkelRemapOffest = new int[0];

    [SerializeField]
    protected int[] m_SkelRemapTable = new int[0];

    public Object avatar  { get { return m_Avatar; } set { m_Avatar = value; } }
    public int[] skelRemapOffset { get { return m_SkelRemapOffest; } }
    public int[] skelRemapTable { get { return m_SkelRemapTable; } }

    public void RefreshAvatarDesc()
    {
        Transform[] avatarSkeleton = null;

        List<int> newRemapTable = new List<int>();
        List<int> newOffsetTable = new List<int>();
        if (null == avatarSkeleton)
        {
            GameObject avatar = m_Avatar as GameObject;
            avatarSkeleton = avatar.transform.GetChild(0).GetComponentsInChildren<Transform>();
        }
        /// 重定位骨骼
        /// 
        SkinnedMeshRenderer[] asmr = null;

        asmr = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        int offset = 0;
        for (int i = 0; i < asmr.Length; ++i)
        {
            newOffsetTable.Add(offset);
            SkinnedMeshRenderer smr = asmr[i];
            if (null != smr)
            {
                Transform[] meshBones = smr.bones;
                for (int k = 0; k < meshBones.Length; ++k)
                {
                    newRemapTable.Add(_FindSkeletonBone(avatarSkeleton, meshBones[k]));
                }

                offset += smr.bones.Length;
            }
        }
        m_SkelRemapTable = new int[newRemapTable.Count];
        for (int i = 0; i < newRemapTable.Count; ++i)
        {
            m_SkelRemapTable[i] = newRemapTable[i];
        }
            

        m_SkelRemapOffest = new int[newOffsetTable.Count];
        for (int i = 0; i < newOffsetTable.Count; ++i)
            m_SkelRemapOffest[i] = newOffsetTable[i];
    }

    int _FindSkeletonBone(Transform[] skeleton, Transform bone)
    {
        if (null != skeleton && null != bone)
        {
            for (int i = 0; i < skeleton.Length; ++i)
            {
                if (null == skeleton[i])
                    continue;

                if (skeleton[i].name.Equals(bone.name))
                    return i;
            }
        }

        return -1;
    }
}
