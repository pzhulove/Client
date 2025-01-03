using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

public enum BuffEffectWorkType
{
    NONE = 0,
    EFFECT,     //特效
    SHADER,     //shader
    CONFIG      //技能配置文件
}
	
public class BuffEffect{

	public struct EffectConfig
	{
		public string name;
		public string locator;
		public bool loop;
		public int scale;
	}

    public struct BuffEffectConfig
    {
        public BuffEffectWorkType type;
        public string shaderName;
		public EffectConfig birthEffect;
		public EffectConfig effect;
		public EffectConfig endEffect;
        public string effectConfigPath;

        public void InitWithShader(string name)
        {
            type = BuffEffectWorkType.SHADER;
            shaderName = name;
        }

		public void InitWithEffect(ProtoTable.BuffTable data)
        {
            type = BuffEffectWorkType.EFFECT;

            if (data.BirthEffectInfoID > 0)
            {
	            
	            EffectInfoTable effectInfoTable =
		            TableManager.GetInstance().GetTableItem<EffectInfoTable>(data.BirthEffectInfoID);
	            if (effectInfoTable == null)
	            {
		            Logger.LogError(string.Format("在buff表里面填写的触发特效的id:{0} 不在触发效果表里面",data.BirthEffectInfoID));
	            }
	            else
	            {
		            birthEffect.name = effectInfoTable.Path;
		            birthEffect.locator = effectInfoTable.Locator;
		            birthEffect.scale = effectInfoTable.Scale;
	            }
            }
            else
            {
	            birthEffect.name = data.BirthEffect;
	            birthEffect.locator = data.BirthEffectLocate;
	            birthEffect.scale = 1000;
            }
			

			if (data.EffectInfoID > 0)
			{
				EffectInfoTable effectInfoTable =
					TableManager.GetInstance().GetTableItem<EffectInfoTable>(data.EffectInfoID);
				if (effectInfoTable == null)
				{
					Logger.LogError(string.Format("在buff表里面填写的触发特效的id:{0} 不在触发效果表里面",data.EffectInfoID));
				}
				else
				{
					effect.name = effectInfoTable.Path;
					effect.locator = effectInfoTable.Locator;
					effect.loop = effectInfoTable.Loop;
					effect.scale = effectInfoTable.Scale;
				}
			}
			else
			{
				effect.name = data.EffectName;
				effect.locator = data.EffectLocateName;
				effect.loop = !data.EffectLoop;
				effect.scale = 1000;
			}
		
			if (data.EndEffectInfoID > 0)
			{
	            
				EffectInfoTable effectInfoTable =
					TableManager.GetInstance().GetTableItem<EffectInfoTable>(data.EndEffectInfoID);
				if (effectInfoTable == null)
				{
					Logger.LogError(string.Format("在buff表里面填写的特效信息的id:{0} 不在特效信息表里面",data.EndEffectInfoID));
				}
				else
				{
					endEffect.name = effectInfoTable.Path;
					endEffect.locator = effectInfoTable.Locator;
					endEffect.scale = effectInfoTable.Scale;
				}
			}
			else
			{
				endEffect.name = data.EndEffect;
				endEffect.locator = data.EndEffectLocate;
				endEffect.scale = 1000;
			}
			
        }

        public void InitWithConfig(string path)
        {
            type = BuffEffectWorkType.CONFIG;
            effectConfigPath = path;
        }
    }

	BuffEffectConfig[] configs = new BuffEffectConfig[3];
	public int configNum = 0;

    bool buffEffectAni = true;
    public BeBuff buff;

    public GeEffectEx geEffect = null;

    public string hurtEffectName;
    public string hurtEffectLocator;

	uint recordShaderHandle = ~0u;
	public string headName;
	public bool showHPNumber = true;

	System.UInt32 audioHandle = 0;
	int sfxID = 0;

	
	public void Reset()
	{
		buffEffectAni = true;
		buff = null;
		geEffect = null;
		hurtEffectName = null;
		hurtEffectLocator = null;
		recordShaderHandle = 0;
		headName = "";
		showHPNumber = true;
		configNum = 0;
		audioHandle = 0;
		sfxID = 0;
	}

#if !LOGIC_SERVER

	public static bool NeedCreateBuffEffect(int buffid)
	{
		bool ret = false;

		var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffid);
		if (data != null)
		{
			if (Utility.IsStringValid(data.EffectShaderName) ||
				Utility.IsStringValid(data.EffectName) ||
				Utility.IsStringValid(data.EffectConfigPath) ||
				Utility.IsStringValid(data.HurtEffectName) || 
				Utility.IsStringValid(data.HeadName) ||
				data.EffectInfoID>0||
				data.SfxID > 0)
				ret = true;
		}

		return ret;
	}

    public BuffEffect()
    {
        
    }

	public void Init(int buffid, bool buffEffectAni=true)
	{
		this.buffEffectAni = buffEffectAni;
		var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffid);
		if (data != null)
		{
			if (Utility.IsStringValid(data.EffectShaderName))
			{
				BuffEffectConfig config = new BuffEffectConfig();
				config.InitWithShader(data.EffectShaderName);
				AddConfig(config);
			}

			if (data.EffectInfoID > 0||Utility.IsStringValid(data.EffectName))
			{
				BuffEffectConfig config = new BuffEffectConfig();
				config.InitWithEffect(data);
				AddConfig(config);
			}
			
			
			if (Utility.IsStringValid(data.EffectConfigPath))
			{
				BuffEffectConfig config = new BuffEffectConfig();
				config.InitWithConfig(data.EffectConfigPath);
				AddConfig(config);
			}

			hurtEffectName = data.HurtEffectName;
			hurtEffectLocator = data.HurtEffectLocateName;
			headName = data.HeadName;

			sfxID = data.SfxID;
		}
	}

	public void AddConfig(BuffEffectConfig config)
	{
		configs[configNum++] = config;
	}

	public void ResetDuration(int d)
	{
		if (geEffect != null)
		{
			geEffect.SetTimeLen(d);
		}
	}

    public void ShowHurtEffect()
    {
        if (Utility.IsStringValid(hurtEffectName) && buff != null)
        {
            buff.owner.m_pkGeActor.CreateEffect(hurtEffectName, hurtEffectLocator, 0, new Vec3(0, 0, 0));
        }
    }

    DelayCallUnitHandle visibleHandle;
    public void ShowEffect(BeActor actor)
    {
        if (actor == null)
            return;

		PlaySfx(sfxID);

        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();

        if (Utility.IsStringValid(headName))
		{
            //如果是异常Buff并且该Buff已经存在则不重复飘Buff头顶名字
            bool abnormalExist = actor.buffController.IsAbnormalBuff(buff.buffType) && actor.buffController.HasBuffByID(buff.buffID) != null;
            if (!abnormalExist)
            {
                string picName = headName;
                if (picName != null)
                {
                    if (battleUI != null)
                        battleUI.comShowHit.ShowBuffText(picName, actor.m_iID, actor.GetGePosition(PositionType.OVERHEAD), buff.owner);
                }
            }
		}

        for (int i=0; i<configNum; ++i)
        {
            switch (configs[i].type)
            {
                case BuffEffectWorkType.SHADER:
				if (configs[i].shaderName == "失明")
				{
					if (actor.isLocalActor && !actor.IsDead())
						actor.CurrentBeScene.StartBlindMask();
				}
                else if (configs[i].shaderName == "隐身")
                {
					actor.m_pkGeActor.SetShadowVisible(buff.owner.CurrentBeScene.currentGeScene, false);
                    if (~0u == recordShaderHandle)
                        recordShaderHandle = actor.m_pkGeActor.ChangeSurface(configs[i].shaderName, 0, buffEffectAni);
					visibleHandle = actor.delayCaller.DelayCall(1000, ()=>{
						actor.m_pkGeActor.SetActorVisible(false);
					});
                }
				else if (configs[i].shaderName == "全屏闪白")
				{
					
					if (battleUI != null)
                        battleUI.ShowFlashWhite();
                }
                else if (configs[i].shaderName == "全屏闪白2")
                {
                    if (actor.isLocalActor && battleUI != null)
                        battleUI.ShowFlashWhite();
                }
				else if (configs[i].shaderName == "不显示冒字")
					showHPNumber = false;
				else
                {
                    if (configs[i].shaderName == "持续霸体")
                        buffEffectAni = true;
                        if (actor.m_pkGeActor != null)
                        {
                            if (~0u == recordShaderHandle)
                                recordShaderHandle = actor.m_pkGeActor.ChangeSurface(configs[i].shaderName, 0, buffEffectAni);
                        }
                }
                    
                    break;
                case BuffEffectWorkType.EFFECT:
                {
					if (Utility.IsStringValid(configs[i].birthEffect.name))
					{
						var geEffect = PlayEffect(configs[i].birthEffect);
						buff.TriggerEventNew(BeEventType.onBuffCreateEffect, new EventParam{m_Obj = this.geEffect});
	                    if (geEffect != null)
	                    {
	                        float timeLen = geEffect.GetTimeLen();
	                        var eff = configs[i].effect;
	                        actor.delayCaller.DelayCall(IntMath.Float2Int(timeLen) - 10, () =>
	                        {
								// marked by ckm
								if(buff != null && buff.state != null)
								{
									if (!buff.state.IsDead())
									{
										this.geEffect = PlayEffect(eff, buff.duration, EffectTimeType.BUFF, eff.loop);
										buff.TriggerEventNew(BeEventType.onBuffReplaceEffect, new EventParam{m_Obj = geEffect, m_Obj2 = this.geEffect });
									}
								}
	                        });
	                    }
	                }
					else
					{
						geEffect = PlayEffect(configs[i].effect, buff.duration, EffectTimeType.BUFF, configs[i].effect.loop);
						buff.TriggerEventNew(BeEventType.onBuffCreateEffect, new EventParam{m_Obj = geEffect});
					}
                }
                    break;
                case BuffEffectWorkType.CONFIG:
                    //TODO 暂时不支持
                    break;
            }
        }


        
    }

    // 隐藏特效(适合没有出生特效的情况)
    public void HideEffect()
    {
        if (geEffect != null)
            geEffect.SetVisible(false);
    }

    public void ResetElapsedTime()
    {
        if (geEffect != null)
            geEffect.ResetElapsedTime();
    }

	GeEffectEx PlayEffect(EffectConfig effect, int duration = 0, EffectTimeType timeType = EffectTimeType.SYNC_ANIMATION, bool loop=false)
	{
		if (buff.owner == null)
			return null;

		if (buff.owner.m_pkGeActor == null)
			return null;

		DAssetObject asset;

		asset.m_AssetObj = null;
		asset.m_AssetPath = effect.name;

		EffectsFrames effectInfo = new EffectsFrames();
		effectInfo.localPosition = new Vector3(0, 0, 0);
		effectInfo.timetype = timeType;
		if (Utility.IsStringValid(effect.locator))
		{
			effectInfo.attachname = effect.locator;
		} 
		
		float scale= effect.scale / 1000f;
		if (effect.scale <= 0)
		{
			scale = 1f;
		}
		effectInfo.localScale =new Vector3(scale,scale,scale);
		
        bool forceDisplay = false;
        if (buff.buffData != null)
        {
            forceDisplay = buff.buffData.IsLowLevelShow;
        }

        float d = duration / 1000f;
		GeEffectEx geEffect = buff.owner.m_pkGeActor.CreateEffect(asset, effectInfo, d, new Vec3(0, 0, 0), 1f, 1f, loop,false, forceDisplay);
		return geEffect;
	}

    public void RemoveEffect(BeActor actor)
    {
        if (actor == null)
            return;

		for (int i = 0; i < configNum; ++i)
        {
            switch (configs[i].type)
            {
                case BuffEffectWorkType.SHADER:
					if (configs[i].shaderName == "失明")
					{
						if (actor.isLocalActor && actor.CurrentBeScene != null)
						{
							actor.CurrentBeScene.StopBlindMask();
						}

					}
	                else if (configs[i].shaderName == "隐身")
	                {
						
						if (actor.CurrentBeScene != null && actor.m_pkGeActor != null)
							actor.m_pkGeActor.SetShadowVisible(actor.CurrentBeScene.currentGeScene, true);
                        if (actor.m_pkGeActor != null)
                        {
                            actor.m_pkGeActor.RemoveSurface(recordShaderHandle);
                            recordShaderHandle = ~0u;
                        }
						if (actor.m_pkGeActor != null)
							actor.m_pkGeActor.SetActorVisible(true);
                        visibleHandle.SetRemove(true);
	                }
					else
					{
                        if (actor.m_pkGeActor != null)
                        {
                            actor.m_pkGeActor.RemoveSurface(recordShaderHandle);
                            recordShaderHandle = ~0u;
                        }
					}
                    
                    break;
                case BuffEffectWorkType.EFFECT:

                    if (geEffect != null)
                    {
                        buff.TriggerEventNew(BeEventType.onBuffRemoveEffect, new EventParam{m_Obj = geEffect});
						if (actor.m_pkGeActor != null)
	                    	actor.m_pkGeActor.DestroyEffect(geEffect);
                        geEffect = null;

                        if (Utility.IsStringValid(configs[i].endEffect.name))
						{
							PlayEffect(configs[i].endEffect);
						}

	                }

                    break;
                case BuffEffectWorkType.CONFIG:
                    //TODO 暂时不支持
                    break;
            }
        }

		StopSfx();
    }

	public void PlaySfx(int sfxID)
	{
		if (sfxID <= 0)
			return;

		BaseBattle battle = null;
		if (buff != null && buff.owner != null)
			battle = buff.owner.CurrentBeBattle;

		if (battle != null)
		audioHandle = battle.PlaySound(sfxID);
	}

	public void StopSfx()
	{
		if (audioHandle > 0)
		{
			AudioManager.instance.Stop(audioHandle);
			audioHandle = 0;
		}
	}
#else

	public static bool NeedCreateBuffEffect(int buffid){return false;}
	public BuffEffect(){}
	public void Init(int buffid, bool buffEffectAni=true){}
	public void AddConfig(BuffEffectConfig config){}
	public void ResetDuration(int d){}
	public void ShowHurtEffect(){}
	public void ShowEffect(BeActor actor){}
	public void HideEffect(){}
	public void ResetElapsedTime(){}
	GeEffectEx PlayEffect(EffectConfig effect, int duration = 0, EffectTimeType timeType = EffectTimeType.SYNC_ANIMATION, bool loop=false){return null;}
	public void RemoveEffect(BeActor actor){}
	public void PlaySfx(int sfxID){}
	public void StopSfx(){}

#endif

}
