using UnityEngine;
using System.Collections;
using Spine.Unity;

public class GeEffectProxy : MonoBehaviour {

	[HideInInspector]
	public EffectAudioProxy m_AudioProxy;
	[HideInInspector]
	public Animation[] m_Animations;
	[HideInInspector]
	public ParticleSystem[] m_ParticleSys;
	[HideInInspector]
	public TrailRenderer[] m_Trails;
	[HideInInspector]
	public FrameMaterials[] m_SeqFrames;
	//[HideInInspector]
	//public ParticleEmitter[] m_ParticleEmitter;
	//[HideInInspector]
	//public ParticleAnimator[] m_ParticleAnimator;
	[HideInInspector]
	public Animator[] m_Animators;
	[HideInInspector]
	public MeshRenderer[] m_MeshRenderer;

	[HideInInspector]
	public SkeletonAnimation[] m_SpineAnimations = null;
	[HideInInspector]
	public SkeletonGraphic[] m_SpineAnimationsUI = null;
	
	[HideInInspector]
	public float defaultTimeLen = 0f;

    public bool m_LockLoop = false;

	public void DoCookData(bool ingame=false)
	{
		m_AudioProxy = gameObject.GetComponent<EffectAudioProxy>();
		m_Animations = gameObject.GetComponentsInChildren<Animation>(true);
		m_ParticleSys = gameObject.GetComponentsInChildren<ParticleSystem>(true);

		m_Trails = gameObject.GetComponentsInChildren<TrailRenderer>(true);
		m_SeqFrames = gameObject.GetComponentsInChildren<FrameMaterials>(true);

		//m_ParticleEmitter = gameObject.GetComponentsInChildren<ParticleEmitter>(true);
		//m_ParticleAnimator = gameObject.GetComponentsInChildren<ParticleAnimator>(true);
		m_Animators = gameObject.GetComponentsInChildren<Animator>(true);

		m_MeshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();

		m_SpineAnimations = gameObject.GetComponentsInChildren<SkeletonAnimation>();
		m_SpineAnimationsUI = gameObject.GetComponentsInChildren<SkeletonGraphic>();

		defaultTimeLen = GetDefaultTimeLen(m_ParticleSys, m_Animations, m_SeqFrames, m_Animators);
		_removeDestroyDelay();
		//Logger.LogErrorFormat("GeEffectProxy:{0} len:{1}", gameObject.name, defaultTimeLen);

		var coms = gameObject.GetComponentsInChildren<ComAnimatorAutoClose>(true);
		if (coms != null && coms.Length > 0)
		{
			#if UNITY_EDITOR	
			Logger.LogErrorFormat("GeEffectProxy {0} 含有 ComAnimatorAutoClose", gameObject.name);
			#endif
			if (ingame)
			{
				for(int i=0; i<coms.Length; ++i)
					coms[i].enabled = false;
			}
		}
	}

	void _removeDestroyDelay(bool ingame=false)
	{
		DestroyDelay[] destroyDelay = gameObject.GetComponentsInChildren<DestroyDelay>();
		if(null != destroyDelay && destroyDelay.Length > 0)
		{
			#if UNITY_EDITOR			
			Logger.LogErrorFormat("{0}还有DestroyDelay!!!!", gameObject.name);
			#endif
			for (int i = 0; i < destroyDelay.Length; ++i)
			{
				destroyDelay[i].enabled = false;
				if (ingame)
					UnityEngine.Object.Destroy(destroyDelay[i].gameObject);
			}
		}
	}

	public static float GetDefaultTimeLen(
		ParticleSystem[] m_ParticleSys, 
		Animation[] m_Animations,
		FrameMaterials[] m_SeqFrames,
		Animator[] m_Animators
	)
	{
		float defTimeLen = 0.0f;

		if (null != m_ParticleSys)
		{
			ParticleSystem ps = null;
			float timeLen = 0.0f;
			for (int i = 0; i < m_ParticleSys.Length; ++i)
			{
                ps = m_ParticleSys[i];

                if (ps == null)
                {
                    continue;
                }

                timeLen = ps.duration + ps.startDelay + ps.startLifetime;
				if (timeLen > defTimeLen)
					defTimeLen = timeLen;
			}
		}

		if (null != m_Animations)
		{
			Animation ani = null;
			for (int i = 0; i < m_Animations.Length; ++i)
			{
				ani = m_Animations[i];
				if (ani != null && ani.clip != null && defTimeLen < ani.clip.length)
					defTimeLen = ani.clip.length;
			}
		}

		if (null != m_SeqFrames)
		{
			FrameMaterials fm = null;
			for (int i = 0; i < m_SeqFrames.Length; ++i)
			{
				fm = m_SeqFrames[i];
				if (fm != null && defTimeLen < fm.Duration())
					defTimeLen = fm.Duration();
			}
		}

		if (m_Animators != null)
		{
			Animator anr = null;
			for (int i = 0; i < m_Animators.Length; ++i)
			{
				anr = m_Animators[i];
				if(null == anr) continue;

				anr.Rebind();
				if (anr.runtimeAnimatorController == null)
					continue;

				AnimatorClipInfo[] clipinfo = anr.GetCurrentAnimatorClipInfo(0);

				if (clipinfo.Length > 0)
				{
					for (int j = 0; j < clipinfo.Length; ++j)
					{
						if (clipinfo[j].clip != null && defTimeLen < clipinfo[j].clip.length)
						{
							defTimeLen = clipinfo[j].clip.length;
						}
					}
				}
				else {
					AnimationClip[] aclip = anr.runtimeAnimatorController.animationClips;
					for(int j=0; j<aclip.Length; ++j)
					{
						if (aclip[j] != null && defTimeLen < aclip[j].length)
						{
							defTimeLen = aclip[j].length;
						}
					}
				}

			}
		}

		return defTimeLen;
	}
}
