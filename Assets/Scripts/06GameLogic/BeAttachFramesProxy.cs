using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeAttachFramesProxy
{
#if !LOGIC_SERVER
 
	public class AttachFrame
	{
		public BeAttachFrames frame;
		public GeAttach       attach;
	}
    //AttachFrame 缓存
    public class AttachFrameDataPool
    {
        private static readonly GamePool.ObjectPool<AttachFrame> m_EventPool = new GamePool.ObjectPool<AttachFrame>(null, null);

        public static AttachFrame Get()
        {
            return m_EventPool.Get();
        }

        public static void Release(AttachFrame inst)
        {
            m_EventPool.Release(inst);
        }
        public static void Clear()
        {
            m_EventPool.Clear();
        }
    }

    public List<AttachFrame> att_frame = new List<AttachFrame>();

	
 #endif
	public GeActorEx              actor;
	public float pretime            = 0.0f;

	public string replaceWeaponPath;
	public string samepleWeaponPath;
	public bool needReplace = false;
	public int tag = 0;
	public int strengthenLevel = 0;
    public string fashionWeaponPath = null;                             //是否显示时装武器
	public string roleDefaultWeaponName = null;

	public void SetWeaponReplace(string rp, string sp, int t, int slevel=0)
	{
        replaceWeaponPath = rp;
        strengthenLevel = slevel;
        needReplace = true;
        samepleWeaponPath = sp;
        tag = t;
    }

    public void SetShowFashionWeapon(string rp,string rdw)
    {
        fashionWeaponPath = rp;
        roleDefaultWeaponName = rdw;
    }

	public void Clear()
	{
#if !LOGIC_SERVER
 
		if(actor == null)
		{
			return;
		}

		for(int i = 0; i < att_frame.Count; ++i)
		{
			AttachFrame af = att_frame[i];
			if(af.attach != null)
				actor.DestroyAttachment(af.attach);

			af.attach = null;
            AttachFrameDataPool.Release(af);
        }

		att_frame.Clear();

 #endif
	}

	public void Init(BDEntityActionInfo info)
	{
#if !LOGIC_SERVER
 
		Clear();
        if ( info != null )
		{
			for(int i = 0; i < info.vAttachFrames.Count; ++i)
			{
				AttachFrame frame = AttachFrameDataPool.Get();
                frame.frame = info.vAttachFrames[i];
				att_frame.Add(frame);
			}
		}

		pretime = 0.0f;
 #endif

	}
		
	public void Update(float time)
	{
#if !LOGIC_SERVER
		if(actor == null)
		{
			return;
		}

		if(pretime > time)
		{
			pretime = 0.0f;
		}

		for(int i = 0; i < att_frame.Count; ++i)
		{
			AttachFrame af = att_frame[i];
			if ( af.frame.start >= pretime
				&& af.frame.start <= time )
			{
				if( af.attach == null )
				{     
					//进行武器模型替换
					bool replaced = false;
					string path = af.frame.entityAsset.m_AssetPath;
					if (needReplace && path == samepleWeaponPath)
					{
                        //Logger.LogErrorFormat("replace weapon path with:{0}", replaceWeaponPath);
                        replaced = true;
                        if (fashionWeaponPath != null)
                            path = fashionWeaponPath;
                        else
                        {
                            path = replaceWeaponPath;
                        }
					}

                    //处理不装备武器的情况
                    if(samepleWeaponPath == null 
                        && path == roleDefaultWeaponName
                        && fashionWeaponPath != null)
                    {
                        path = fashionWeaponPath;
                    }

                    string tmpName = af.frame.name;
					if (tmpName.Contains("["))
					{
						tmpName += path;
					}

					af.attach = actor.CreateAttachment(tmpName, path, af.frame.attachName, true,false);
                    //穿戴时装武器时  因为不会更换武器模型 所以由高强化等级切换到0级时也要更改强化特效
					if (replaced && (strengthenLevel > 0 || !string.IsNullOrEmpty(fashionWeaponPath)) && af.attach != null)
						af.attach.ChangePhase(BeUtility.GetStrengthenEffectName(af.attach.ResPath), strengthenLevel);
				}

				if (af.attach != null)
				{
					//af.attach.SetBindingPose();
					for(int j = 0; j < af.frame.animations.Length; ++j)
					{
						BeAnimationFrame anf = af.frame.animations[j];
						if (anf.start >= pretime && anf.start <= time)
						{
                            string attachActionName = actor.GetAttachmentCurActionName(af.attach.Name, af.attach.AttachNodeName);
                            if (anf.anim != attachActionName)/// 表现层同步 注意逻辑分离
                            {
                                actor.PlayAttachmentAnimation(af.attach.Name, anf.anim, anf.speed * actor.GetCurActionSpeed());
                                //   af.attach.PlayAction(anf.anim, anf.speed * actor.GetCurActionSpeed(), actor.IsActionLoop(anf.anim), actor.GetCurActionOffset());
                            }
                        }
	                }
				}
			}
			else if( time > af.frame.end || af.frame.start > time )
			{
				if(af.attach != null)
				{
					actor.DestroyAttachment(af.attach);
					//Logger.LogErrorFormat("delete attach:{0}", af.attach.ResPath);
					af.attach = null;
				} 
			}
		}
		pretime = time;
        //actor.Update(0, (int)GeEntity.GeEntityRes.Attach);
        actor.UpdateAttach();

 #endif

    }

//     public void ClearAttach()
//     {
// #if !LOGIC_SERVER
// 
//         if (actor == null)
//         {
//             return;
//         }
//         for (int i = 0; i < att_frame.Count; ++i)
//         {
//             AttachFrame af = att_frame[i];
//             //如果是默认的武器
//             if (af != null && af.attach != null/* && af.frame.entityAsset.m_AssetPath == samepleWeaponPath*/)
//             {
//                 actor.DestroyAttachment(af.attach);
//                 af.attach = null;
//             }
//         }
// #endif
//    	
}

