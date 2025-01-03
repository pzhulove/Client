using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ProtoTable;
using Tenmove.Runtime;

namespace GameClient
{
    public enum PreloadSpecialType
    {
        Skill,
        Buff,
        Mechanism,
    }

    class PreloadManager
    {
		public static BattleType battleType = BattleType.None;
		
		public static List<int> cachedResID = new List<int>();
		public static void ClearCache()
		{
			cachedResID.Clear();
		}

		public static void AddCache(int key)
		{
			if (!cachedResID.Contains(key))
				cachedResID.Add(key);
		}

		public static bool IsCached(int key)
		{
			return cachedResID.Contains(key);
		}

		public static void PreloadActionInfo(BDEntityActionInfo data, BeActionFrameMgr frameMgr,SkillFileListCache fileCache, bool useCube=false)
        {
			if (Utility.IsStringValid(data.hitEffectAsset.m_AssetPath))
			{
				CResPreloader.instance.AddRes(data.hitEffectAsset.m_AssetPath, false);		
			}

            //预加载技能特写
            PreloadManager.PreloadSpecialScripts(PreloadSpecialType.Skill, data.skillID, frameMgr, fileCache);

             //被击音效
            if (!useCube)
            	AudioManager.instance.PreloadSound(data.hitSFXID);
			
			//UnityEngine.Profiling.Profiler.BeginSample("PreLoad vFramesData");
            for (int j = 0; j < data.vFramesData.Count; ++j)
            {
                BDEntityActionFrameData frameData = data.vFramesData[j];
                if (frameData != null)
                {
                    for (int k = 0; k < frameData.pEvents.Count; ++k)
                    {
						frameData.pEvents[k].PreparePreload(frameMgr, fileCache,useCube);
                    }

                    //预加载攻击框触发效果Id
                    if (frameData.pAttackData != null && frameData.pAttackData.hurtID > 0)
                        PreloadEffectID(frameData.pAttackData.hurtID, frameMgr, fileCache);
                }
            }
			if (!useCube)
			{
            //挂件
            for (int j = 0; j < data.vAttachFrames.Count; ++j)
            {
                BeAttachFrames attachData = data.vAttachFrames[j];
                if (attachData != null)
                {

					//预加载挂件动画
					List<string> attachAnimationList = null;
					for(int k=0; k<attachData.animations.Length; ++k)
					{
						if (attachAnimationList == null)
							attachAnimationList = new List<string>();

						if (attachData.animations[k].anim.Length > 0)
						{
							attachAnimationList.Add(attachData.animations[k].anim);
							//Logger.LogErrorFormat("attach {0} animation:{1}", attachData.entityAsset.m_AssetPath, attachData.animations[k].anim);
						}
							

					}

					int extData = 0;
					if (attachAnimationList != null && attachAnimationList.Count > 0)
						extData = 2;

					CResPreloader.instance.AddRes(attachData.entityAsset.m_AssetPath, false, 1, null, extData, attachAnimationList);
                }
            }
        }

        }
		public static void PreloadResIDOnly(int resID)
		{
			if (resID == 0)
				return;

			var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
			if (resData == null)
				return;

			CResPreloader.instance.AddRes(resData.ModelPath, false);
		}

		/*public static void PreloadSkillFile(int resID)
		{
			var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
			if (resData == null)
				return;
			
			for(int index=0; index<resData.ActionConfigPath.Count; ++index)
			{
				var path = resData.ActionConfigPath[index];
				if (!Utility.IsStringValid(path))
					continue;

                var loadedList = GamePool.ListPool<BDEntityActionInfo>.Get();
                BDEntityActionInfo.SaveLoad(
                    path, null, true, false, loadedList, null
                );
                GamePool.ListPool<BDEntityActionInfo>.Release(loadedList);
            }

		}
*/
		public static void PreloadResID(int resID, BeActionFrameMgr frameMgr,SkillFileListCache fileCache,bool needPreloadAnimation = false)
        {
            if (resID == 0)
                return;

			if (IsCached(resID))
				return;

			AddCache(resID);

            var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
			if (resData == null)
				return;
			
            

            List<string> animationNames = null;
            if (needPreloadAnimation)
            {
                animationNames = GamePool.ListPool<string>.Get();
            }

			for(int index=0; index<resData.ActionConfigPath.Count; ++index)
			{
				var path = resData.ActionConfigPath[index];
				if (!Utility.IsStringValid(path))
					continue;

                var loadedList = GamePool.ListPool<BDEntityActionInfo>.Get();
                BDEntityActionInfo.SaveLoad(
                    battleType, path, null, true, needPreloadAnimation, loadedList, animationNames
#if LOGIC_SERVER
                    , frameMgr, fileCache
#endif
                );
                GamePool.ListPool<BDEntityActionInfo>.Release(loadedList);
            }
			UnityEngine.Profiling.Profiler.BeginSample("PreLoad Model");
            if (needPreloadAnimation)
                CResPreloader.instance.AddRes(resData.ModelPath, false, 1, null, 1, animationNames);
            else
                CResPreloader.instance.AddRes(resData.ModelPath, false);
			UnityEngine.Profiling.Profiler.EndSample();

			UnityEngine.Profiling.Profiler.BeginSample("PreLoad ICON");
            //头像
            if (Utility.IsStringValid(resData.IconPath))
            {
                CResPreloader.instance.AddRes(resData.IconPath, false, 1, null, 0, null, CResPreloader.ResType.RES, typeof(Sprite));
                //AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite));
            }
			UnityEngine.Profiling.Profiler.EndSample();

			if (needPreloadAnimation)
            {
                GamePool.ListPool<string>.Release(animationNames);
            }
        }

        public static void PreloadEffectID(int effectID, BeActionFrameMgr frameMgr,SkillFileListCache fileCache)
        {
            if (effectID <= 0)
                return;

            var effectData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(effectID);
            if (effectData == null)
                return;

            //召唤怪物
            if (effectData.SummonID > 0)
            {
                if (effectData.SummonRandList.Count >= 2)
                {
                    for (int i = 0; i < effectData.SummonRandList.Count; ++i)
                        PreloadMonsterID(effectData.SummonRandList[i],frameMgr, fileCache);
                }
                else
                {
                    PreloadMonsterID(effectData.SummonID,frameMgr, fileCache);
                }
            }

            //召唤buff
            PreloadBuffID(effectData.BuffID,frameMgr, fileCache);
            for (int i = 0; i < effectData.BuffInfoID.Count; ++i)
                PreloadBuffInfoID(effectData.BuffInfoID[i],frameMgr, fileCache);

			//召唤实体
			for(int i=0; i<effectData.AttachEntity.Count; ++i)
			{
				int eid = effectData.AttachEntity[i];
				if (eid > 0)
					PreloadResID(eid, frameMgr, fileCache);
			}
        }

        public static void PreloadBuffID(int buffID, BeActionFrameMgr frameMgr, SkillFileListCache fileCache, bool useCube = false)
        {
            if (useCube)
                return;

            if (buffID <= 0)
                return;

            var buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffID);
            if (buffData == null)
                return;

            //buff特效 出生特效 持续特效 结束特效预加载
            string birthEffectPath = Utility.GetEffectPathByBuffTable(buffData,0);
            if (Utility.IsStringValid(birthEffectPath))
                CResPreloader.instance.AddRes(birthEffectPath);
            
            string effectPath = Utility.GetEffectPathByBuffTable(buffData,1);
            if (Utility.IsStringValid(effectPath))
                CResPreloader.instance.AddRes(effectPath);
            
            string endEffectPath = Utility.GetEffectPathByBuffTable(buffData,2);
            if (Utility.IsStringValid(endEffectPath))
                CResPreloader.instance.AddRes(endEffectPath);

            if (buffData.Type == (int)BuffType.SUMMON)
            {
                PreloadMonsterID(buffData.summon_monsterID, frameMgr, fileCache);
                for (int i = 0; i < buffData.summon_entity.Count; ++i)
                    PreloadResID(buffData.summon_entity[i], frameMgr, fileCache);
            }

            //预加载机制资源
            for (int i = 0; i < buffData.MechanismID.Length; i++)
            {
                PreloadManager.PreloadSpecialScripts(PreloadSpecialType.Mechanism, buffData.MechanismID[i], frameMgr, fileCache);
            }
            
            PreloadSpecialScripts(PreloadSpecialType.Buff, buffID, frameMgr, fileCache);
        }

        private static object[] _commmonParam = new object[1];

        public static void PreloadSpecialScripts(PreloadSpecialType specialType,int id, BeActionFrameMgr frameMgr, SkillFileListCache fileCache)
        {
            string className = null;
            string methodName = null;
            switch (specialType)
            {
                case PreloadSpecialType.Skill:
                    var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(id);
                    if (skillData == null) return;
                    className = string.Format("{0}{1}", "Skill", id);
                    methodName = "SkillPreloadRes";
                    _commmonParam[0] = skillData;
                    break;
                case PreloadSpecialType.Buff:
                    var buffData = TableManager.GetInstance().GetTableItem<BuffTable>(id);
                    if (buffData == null) return;
                    className = string.Format("{0}{1}", "Buff", id);
                    methodName = "BuffPreloadRes";
                    _commmonParam[0] = buffData;
                    break;
                case PreloadSpecialType.Mechanism:
                    var mechanismData = TableManager.GetInstance().GetTableItem<MechanismTable>(id);
                    if (mechanismData == null) return;
                    className = string.Format("{0}{1}", "Mechanism", mechanismData.Index);
                    methodName = "MechanismPreloadRes";
                    _commmonParam[0] = mechanismData;
                    break;
            }
            Type targetType = Tenmove.Runtime.Utility.Assembly.GetType(className);
            if (targetType == null) return;

            MethodInfo method = targetType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (method == null) return;
            
            method.Invoke(null, _commmonParam);
        }

        public static void PreloadPrefab(string path)
        {
            CResPreloader.instance.AddRes(path);
        }

        public static void PreloadSprite(string path)
        {
            CResPreloader.instance.AddRes(path, false, 1, null, 0, null, CResPreloader.ResType.RES, typeof(Sprite));
        }

        public static void PreloadBuffInfoID(int buffInfoID, BeActionFrameMgr frameMgr, SkillFileListCache fileCache)
        {
            if (buffInfoID <= 0)
                return;

            var buffInfoData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfoID);
            if (buffInfoData == null)
                return;

            PreloadBuffID(buffInfoData.BuffID,frameMgr, fileCache);
        }

		public static void PreloadMonsterID(int monsterID, BeActionFrameMgr frameMgr,SkillFileListCache fileCache,bool onlyResID=false)
        {
            if (monsterID <= 0)
                return;
            int level = (monsterID - monsterID / 10000 * 10000) / 100;
            monsterID -= level * 100;

            var monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monsterID);
            if (monsterData == null)
                return;

            //因为线上分支使用的AI数据文件是CS脚本数据 不需要预加载
			//PreloadAI(monsterData);

            for (int i = 0; i < monsterData.BornBuff.Count; i++)
            {
                PreloadManager.PreloadBuffID(monsterData.BornBuff[i], frameMgr, fileCache);
            }

            for (int i = 0; i < monsterData.BornBuff2.Count; i++)
            {
                PreloadManager.PreloadBuffInfoID(monsterData.BornBuff2[i], frameMgr, fileCache);
            }

            for (int i = 0; i < monsterData.BornMechanism.Count; i++)
            {
                PreloadManager.PreloadBuffInfoID(monsterData.BornMechanism[i], frameMgr, fileCache);
            }

            if (onlyResID)
				PreloadResIDOnly(monsterData.Mode);
			else
            	PreloadResID(monsterData.Mode,frameMgr, fileCache,true);
        }

        /// <summary>
        /// 根据特效信息表Id 预加载特效
        /// </summary>
        /// <param name="effectInfoId"></param>
        public static void PreloadEffectInfo(int effectInfoId)
        {
            var data = TableManager.GetInstance().GetTableItem<EffectInfoTable>(effectInfoId);
            if (data == null)return;
            CResPreloader.instance.AddRes(data.Path, false);
        }

		public static void PreloadAI(ProtoTable.UnitTable monsterData)
		{
			if (monsterData == null)
				return;

			List<string> ais = new List<string>();
			ais.Add(monsterData.AIActionPath);
			ais.Add(monsterData.AIDestinationSelectPath);
			ais.Add(monsterData.AIEventPath);

			for(int i=0; i<ais.Count; ++i)
			{
				if (Utility.IsStringValid(ais[i]))
				{
					//Logger.LogErrorFormat("prelaod ai:{0}", ais[i]);
					CResPreloader.instance.AddRes(ais[i], false);
				}
			}
		}

		public static void PreloadTriggerBuff(Dictionary<int, List<BuffInfoData>> list, BeActionFrameMgr frameMgr, SkillFileListCache fileCache, bool useCube=false)
        {
			if (useCube)
				return;

            if (list == null)
                return;

            Dictionary<int, List<BuffInfoData>>.Enumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var buffInfoList = enumerator.Current.Value as List<BuffInfoData>;
                for (int i = 0; i < buffInfoList.Count; ++i)
                    PreloadBuffID(buffInfoList[i].buffID,frameMgr,fileCache);
            }
        }

		public static void PreloadAnimations(BeActor actor)
		{
			foreach (var info in actor.m_cpkEntityInfo._vkActionsMap)
			{
				var data = info.Value;

				actor.m_pkGeActor.ProloadAction(data.actionName);
				//Logger.LogErrorFormat("actor:{0} preload action:{1}", actor.GetName(), data.actionName);
			}
		}

        public static void PreloadActor(BeActor actor, BeActionFrameMgr frameMgr, SkillFileListCache fileCache, bool useCube=false)
        {
            if (actor == null)
                return;

			if (IsCached(actor.m_iResID))
				return;
			AddCache(actor.m_iResID);

            var enumerator2 = actor.m_cpkEntityInfo._vkActionsMap.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                var current = enumerator2.Current;
                var data = current.Value;
                PreloadActionInfo(data, frameMgr, fileCache,useCube);
            }


            for (int i=0; i<actor.m_cpkEntityInfo.tagActionInfo.Length; ++i)
            {
                var info = actor.m_cpkEntityInfo.tagActionInfo[i];
                if (info != null)
                {
                    var enumerator = info.actionMap.GetEnumerator();
                    while(enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        var data = current.Value;
                        PreloadActionInfo(data, frameMgr,fileCache, useCube);
                    }
                }
            }

            if (actor.accompanyData.id > 0)
            {
                PreloadMonsterID(actor.accompanyData.id,frameMgr, fileCache);
            }

			PreloadTriggerBuff(actor.buffController.GetTriggerBuffList(),frameMgr, fileCache, useCube);
            PreloadAnimations(actor);
        }

    }
}
