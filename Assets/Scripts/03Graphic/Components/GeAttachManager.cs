using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeAttachManager
{
    public struct GeAttachNodeDesc
    {
        public GeAttachNodeDesc(string name, GameObject attNode)
        {
            nodeName = name;
            attachNode = attNode;
        }

        public string nodeName;
        public GameObject attachNode;
    }

    public static readonly GeAttachNodeDesc sInvalidAttachNodeDesc = new GeAttachNodeDesc("", null);

    protected List<GeAttachNodeDesc> m_AttachNodeDescList = new List<GeAttachNodeDesc>();

    protected List<GeAttach> m_AttachmentList = new List<GeAttach>();
    protected bool m_IsAttachNodeListDirty = true;
    protected bool m_IsAttachmentListDirty = false;
    protected GameObject m_RootNode = null;

	public bool airMode;
    private GeActorEx.DisplayMode m_DisplayMode = GeActorEx.DisplayMode.Normal;

	public List<GeAttach> GetAttachList()
	{
		return m_AttachmentList;
	}

    public void AddAttachNode(string nodeName, GameObject nodeObj)
    {
        GeAttachNodeDesc attachNode = GetAttchNodeDesc(nodeName);
        if (sInvalidAttachNodeDesc.nodeName != attachNode.nodeName &&
            sInvalidAttachNodeDesc.attachNode != attachNode.attachNode)
            return;

        m_AttachNodeDescList.Add(new GeAttachNodeDesc(nodeName, nodeObj));
    }

    public GeAttachNodeDesc GetAttchNodeDesc(string nodeName)
    {
        for (int i = 0; i < m_AttachNodeDescList.Count; ++i)
        {
            if (nodeName == m_AttachNodeDescList[i].nodeName)
                return m_AttachNodeDescList[i];
        }

        return sInvalidAttachNodeDesc;
    }

    public void RefreshAttachNode(GameObject skeletonRoot, bool bIsForce = false)
    {
		if (airMode)
			return;

        if (skeletonRoot == null)
            return;

        if (m_RootNode != skeletonRoot)
        {
            m_RootNode = skeletonRoot;
            m_IsAttachNodeListDirty = true;
        }

        if (!bIsForce && m_IsAttachNodeListDirty == false)
            return;

        m_AttachNodeDescList.Clear();

        /// 对应编辑器中的节点列表
        string key = "None";
        m_AttachNodeDescList.Add(new GeAttachNodeDesc(key, null));

        System.Text.StringBuilder sbuilder = StringBuilderCache.Acquire();
        /// 重新初始化角色本事的挂点列表
        /// 
        GameObject boneAll = _FindSkeletonObject(skeletonRoot);
        if (null == boneAll)
        {
            //Logger.LogWarningFormat("Root bone must be named with \"BoneAll\" res:{0}!", skeletonRoot.name);
           // return;
			boneAll = skeletonRoot;
        }

        var trans = GamePool.ListPool<Transform>.Get();

        boneAll.GetComponentsInChildren<Transform>(trans);
        for (int n = 0; n < trans.Count; ++n)
        {
            Transform t = trans[n];
            if (t.gameObject.CompareTag("Dummy"))
            {
                sbuilder.Clear();
                sbuilder.AppendFormat("[actor]{0}",t.gameObject.name);
                key = sbuilder.ToString();
                //key = "[actor]" + t.gameObject.name;
                AddAttachNode(key, t.gameObject);
            }
        }

        /// 初始化角色身上挂件的挂点列表平展到角色挂点列表中去维护（挂件本身也有挂点）
        for (int n = 0; n < m_AttachmentList.Count; ++n)
        {
            GeAttach curAttach = m_AttachmentList[n];
            if (!curAttach.NeedDestroy)
            {
                GameObject root = curAttach.Root;
                if (null == root)
                    continue;

                root.GetComponentsInChildren<Transform>(trans);
                for (int i = 0; i < trans.Count; ++i)
                {
                    Transform t = trans[i];
                    if (t.gameObject.CompareTag("Dummy"))
                    {
                        sbuilder.Clear();
                        sbuilder.AppendFormat("[{0}]{1}",curAttach.Name,t.gameObject.name);
                        key = sbuilder.ToString();
                        //key = "[" + curAttach.Name + "]" + t.gameObject.name;
                        AddAttachNode(key, t.gameObject);
                    }
                }
            }
        }

        GamePool.ListPool<Transform>.Release(trans);
        StringBuilderCache.Release(sbuilder);
        
        m_IsAttachNodeListDirty = false;
    }
    public GameObject GetAttchNodeDescWithRareName(string nodeRareName)
    {
        string keyNone = "None";
        if (!nodeRareName.Equals(keyNone))
        {
            GeAttachNodeDesc desc = GetAttchNodeDesc("[actor]" + nodeRareName);
            return desc.attachNode;
        }

        GeAttachNodeDesc descNone = GetAttchNodeDesc(nodeRareName);
        return descNone.attachNode;
    }

    public GeAttach GetAttachment(string name,string node = null)
    {
        GeAttach att = null;
        if (name == null)
            return att;

        for (int n = 0; n < m_AttachmentList.Count; ++n)
        {
            if (name == m_AttachmentList[n].Name)
            {
                if(!string.IsNullOrEmpty(node))
                {
                    if(!m_AttachmentList[n].AttachNodeName.Equals(node))
                        continue;
                }

                att = m_AttachmentList[n];
                return att;
                
            }
        }

        return null;
    }

    public GeAttach AddAttachment(string attachmentName, string attachRes, string attachNode,bool copyInPool = true,bool asyncLoad = true,bool highPriority = false)
    {
		if (airMode)
			return null;

        GeAttach att = GetAttachment(attachmentName, attachNode);
        bool bAdd = false;

        if (att == null)
        {
            att = new GeAttach(attachmentName);
            bAdd = true;
        }

        att.Create(attachRes, GetAttchNodeDesc(attachNode).attachNode, attachNode, copyInPool, asyncLoad, highPriority);
        att.SetDisplayMode(m_DisplayMode);

        if (bAdd)
        {
            m_AttachmentList.Add(att);
        }

        //attachmentsDirty = true;
        m_IsAttachNodeListDirty = true;
        return att;
    }

    public void SetAttachLayer(int layer)
    {
        for(int i = 0,icnt = m_AttachmentList.Count;i<icnt;++i)
        {
            GeAttach attach = m_AttachmentList[i];
            if(null == attach) continue;

            attach.SetLayer(layer);
        }
    }

    public void SetDisplayMode(GeActorEx.DisplayMode displayMode)
    {
        if(displayMode != m_DisplayMode)
        {
            m_DisplayMode = displayMode;

            for (int i = 0, icnt = m_AttachmentList.Count; i < icnt; ++i)
            {
                GeAttach attach = m_AttachmentList[i];
                if (null == attach) continue;

                attach.SetDisplayMode(m_DisplayMode);
            }
        }
    }

    public void RemoveAttachment(GeAttach attachment)
    {
		if (attachment == null)
			return;
		
        /// 加入标记 惰性批量删除
        m_IsAttachmentListDirty = true;
        attachment.Destroy();
    }

    public void Update()
    {
        for (int i = 0, icnt = m_AttachmentList.Count; i < icnt; ++i)
            m_AttachmentList[i].UpdateAsync();

        if (m_IsAttachmentListDirty)
        {
            _RemoveAttach();
            RefreshAttachNode(m_RootNode);
            m_IsAttachmentListDirty = false;
        }
    }
    public void DoBackToFront()
    {
        for (int i = 0, icnt = m_AttachmentList.Count; i < icnt; ++i)
        {
            var attach = m_AttachmentList[i];
            if (attach.isCreatedInBackMode)
            {
                //将追帧模式下创建的挂件，初始化父节点
                var curAttachNode = GetAttchNodeDesc(attach.AttachNodeName).attachNode;
                attach.ResetAttachNodeInBackMode(curAttachNode);
                m_AttachmentList[i].DoBackToFront();
            }
        }
    }

    protected void _RemoveAttach()
    {
        m_AttachmentList.RemoveAll(
            f =>
            {
                if (f.NeedDestroy)
                {
                    f.DeInit();
                    return true;
                }
                else
                    return false;
            }
            );
    }

	public void ChangeAction(string name,float speed, bool loop=false, float offset = 0.0f)
    {
        for (int n = 0,ncnt =  m_AttachmentList.Count; n <ncnt; ++n)
        {
            var attach = m_AttachmentList[n];
            //挂件不会移除 只是隐藏在人物模型挂点下 如果隐藏的挂件不要播放动画
            if (null == attach || attach.Root == null || !attach.Root.activeSelf) continue;

			m_AttachmentList[n].PlayAction(name, speed, loop, offset);
        }
    }

    public void StopAction()
    {
        for (int n = 0, ncnt = m_AttachmentList.Count; n < ncnt; ++n)
        {
            if (null == m_AttachmentList[n]) continue;

            m_AttachmentList[n].StopAction();
        }
    }

    public void Pause()
    {
		
		for (int n = 0,ncnt =  m_AttachmentList.Count; n <ncnt; ++n)
		{
			if (null == m_AttachmentList[n]) 
				continue;
			if (null == m_AttachmentList[n].AnimationManager) 
				continue;
			m_AttachmentList[n].PauseAnimation();
		}
    }

    public void Resume()
    {
		
		for (int n = 0,ncnt =  m_AttachmentList.Count; n <ncnt; ++n)
		{
			if (null == m_AttachmentList[n]) 
				continue;
			if (null == m_AttachmentList[n].AnimationManager) 
				continue;

			m_AttachmentList[n].ResumeAnimation();
		}
    }

    public void ClearAll()
    {
        m_AttachmentList.RemoveAll(
            f =>
            {
                f.DeInit();
                return true;
            }
            );

        RefreshAttachNode(m_RootNode);
        m_IsAttachmentListDirty = false;
        m_DisplayMode = GeActorEx.DisplayMode.Normal;
    }

    public void ClearAttachmentOnNode(string attachNode)
    {
        bool bDirty = false;
        m_AttachmentList.RemoveAll(
            f =>
            {
                if(f.AttachNodeName == attachNode)
                {
                    f.DeInit();
                    bDirty = true;
                    return true;
                }
                return false;
            }
            );

        if(bDirty)
            RefreshAttachNode(m_RootNode);
    }

    public void Deinit()
    {
        ClearAll();
    }

    GameObject _FindSkeletonObject(GameObject parent)
    {
		if (null != parent && parent.name.Contains("boneall", System.StringComparison.OrdinalIgnoreCase))
            return parent;

        GameObject skeleton = null;
        int nChildNum = parent.transform.childCount;
        for (int j = 0; j < nChildNum; ++j)
        {
            GameObject child = parent.transform.GetChild(j).gameObject;
            skeleton = _FindSkeletonObject(child);
            if (null != skeleton)
                return skeleton;
        }

        return null;
    }
}
