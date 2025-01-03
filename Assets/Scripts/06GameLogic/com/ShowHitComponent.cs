using System;
using System.Collections.Generic;
using System.Collections;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;

using DG.Tweening;
using UnityEngine.EventSystems;
namespace GameClient
{
	public enum HitTextType
	{
		NORMAL = 0,		//普通
		CRITICAL,       //暴击
		MISS,			//miss
		BUFF_HURT,      //出血
		RECOVE,         //治疗
		MP_RECOVER,		//mp回复
		SPECIAL_ATTACK,	//特殊攻击
		BUFF_TEXT,		//buff文字
		GET_EXP,		//获得经验
		GET_GOLD,		//获得金币
		SKILL_CANNOTUSE,//技能不能使用
        FRIEND_HURT,    // 不触发保护/显示白字/不累计连击(持续伤害使用,如杀意波动)
    }

	public enum HitTextAniType
	{
		NORMAL = 0,
		CRITIAL,
		OWN,
		FRIEND,
		BUFF,
		MISS,
		ATTACH,
		OTHER,
        BUFFNAME
	}
		
	public class ShowHitComponent
	{

		public enum HitResType
		{
			NORMAL = 0,
			CRITICAL,
			MISS,
			OWN_HURT,
			BUFF_HURT,
			FRIEND_HURT,
			TEXT_HP,
			TEXT_MP,
			TEXT_BUFF_NAME,
			NORMAL_ATTACH,
			CRITICAL_ATTACH,
            MAX_COUNT,
		}

		public struct ResTypeInfo
		{
			public string resPath;
			public int maxCount;

			public ResTypeInfo(string path, int count)
			{
				resPath = path;
				maxCount = count;
			}
		}

		#region Hurt
		private int showHitNumberCurFrame = 0;

		private GeCamera worldCamera = null;
		private RectTransform ui3drootRect = null;

		private static int ORDER = 0;
		private Dictionary<int, Dictionary<int, List<HitData>>> goNumberList = new Dictionary<int, Dictionary<int, List<HitData>>>();

        private List<HitData>[] m_hitUpdateList = new List<HitData>[(int)HitResType.MAX_COUNT];

		private Queue<HitData> hitDataPool = new Queue<HitData>();
		static int MAX_GO_COUNT = 25;


		public Dictionary<int/*HitResType*/, ResTypeInfo> ResInfoMap = new Dictionary<int/*HitResType*/, ResTypeInfo>{
			{(int)HitResType.NORMAL, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/NormalHurtText", MAX_GO_COUNT)},
			{(int)HitResType.CRITICAL, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/CriticleHurtText", MAX_GO_COUNT)},
			{(int)HitResType.OWN_HURT, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/TextHurtOwn", MAX_GO_COUNT)},
			{(int)HitResType.BUFF_HURT, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/TextBuffHurt", MAX_GO_COUNT)},
			{(int)HitResType.FRIEND_HURT, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/TextHurtFriend", MAX_GO_COUNT)},
			{(int)HitResType.MISS, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/MissText", MAX_GO_COUNT)},
			{(int)HitResType.TEXT_HP, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/TextHP", MAX_GO_COUNT)},
			{(int)HitResType.TEXT_MP, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/TextMP", MAX_GO_COUNT)},
			{(int)HitResType.TEXT_BUFF_NAME, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/TextBuffName", MAX_GO_COUNT)},
			{(int)HitResType.NORMAL_ATTACH, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/NormalAttachText", MAX_GO_COUNT)},
			{(int)HitResType.CRITICAL_ATTACH, new ResTypeInfo("UIFlatten/Prefabs/Battle_Digit/CriticleAttachText", MAX_GO_COUNT)},
		};

        //public bool userNewText = true;

        private TextRendererManager m_TextRendererManger;
			
		public class HitData
		{
			public static int REMOVE_DURATION = 1000;
			public GameObject go;
			public bool canRemove;
			public Tween tween;
			public Tween attachTween;
			public GameObject attachGo;
            public Text text;

			private int acc;

			public HitData()
			{
				Reset();
			}

			public void Reset()
			{
				go = null;
				canRemove = false;
				acc = 0;
				tween = null;
				attachGo = null;
                text = null;
				attachTween = null;
			}

			public void Update(int delta)
			{
				if (canRemove)
					return;

				acc += delta;
				if (acc >= REMOVE_DURATION)
				{
					Remove();
				}
			}

			public void Remove()
			{

				if (tween != null)
				{
					//tween.Kill(true);
					tween = null;
				}
					
				if (attachTween != null)
				{
					//attachTween.Kill(true);
					attachTween = null;
				}

                if (null != text)
                {
                    text.text = string.Empty;
                }
					
				if(attachGo != null)
                {
					CGameObjectPool.instance.RecycleGameObject(attachGo);
                }
                attachGo = null;
                if (null != go)
                {
                    CGameObjectPool.instance.RecycleGameObject(go);
                    go = null;
                }

				canRemove = true;
               
			}
		}

		static int poolCount = 0;
		public HitData GetHitData()
		{
#if UNITY_EDITOR
			poolCount++;
			//Logger.LogErrorFormat("gethitdata:{0}", poolCount);
#endif

			HitData data = null;
			if (hitDataPool.Count > 0)
				data = hitDataPool.Dequeue();
			else
				data = new HitData();
			data.Reset();

			return data;
		}
		public void PutHitData(HitData hitData)
		{
			

#if UNITY_EDITOR
			poolCount--;
			//Logger.LogErrorFormat("poolCount:{0}", poolCount);
			if (hitDataPool.Contains(hitData))
			{
				Logger.LogErrorFormat("hitDataPool try to put element already in pool");
				return;	
			}
#endif

			hitDataPool.Enqueue(hitData);
		}

		void AddHitUpdateList(HitResType type, HitData hitData)
		{
            if (m_hitUpdateList[(int)type] == null)
            {
                m_hitUpdateList[(int)type] = new List<HitData>();
            }
            m_hitUpdateList[(int)type].Add(hitData);
        }

		GameObject GetGameObject(HitResType resPath)
		{
			GameObject go = null;
            var curItemList = m_hitUpdateList[(int)resPath];
            if (curItemList != null)
            {
                if (curItemList.Count >= ResInfoMap[(int)resPath].maxCount)
                {
                    var hitData = curItemList[0];
                    hitData.Remove();
                    curItemList.RemoveAt(0);
                }
            }

            go = CGameObjectPool.instance.GetGameObject(ResInfoMap[(int)resPath].resPath, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);

			return go;
		}

		bool isLowGraphic = false;

#if !LOGIC_SERVER

		public ShowHitComponent()
		{
			RefreshGraphicSetting();

            //if(userNewText)
            if(EngineConfig.useNewHitText)
            {
                m_TextRendererManger = new TextRendererManager();
                m_TextRendererManger.Init();
            }
		}

        public void Clear()
        {
            ClearHitNumber();
            while (hitDataPool.Count > 0)
            {
                HitData item = hitDataPool.Dequeue();
                item.Remove();
            }
           
#if UNITY_EDITOR
            poolCount -= hitDataPool.Count;
#endif
            hitDataPool.Clear();
            for (int i = 0; i < m_hitUpdateList.Length; i++)
            {
                var curItemList = m_hitUpdateList[i];
                if (curItemList == null) continue;
                for (int j = 0; j < curItemList.Count; j++)
                {
                    curItemList[j].Remove();
                }
                curItemList.Clear();
                m_hitUpdateList[i] = null;
            }
        }

		public void RefreshGraphicSetting()
		{
			//低画质不显示好友的伤害冒字
			//GeGraphicSetting.instance.GetSetting("GraphicLevel", ref graphicLevel);
			isLowGraphic = GeGraphicSetting.instance.IsLowLevel() && !BattleMain.IsModePvP(BattleMain.battleType);
		}

		public void Update(int delta)
		{
            if(EngineConfig.useNewHitText)
            {
                m_TextRendererManger.Update();
                //return;
            }

			ResetNumber();
            for (int i = 0; i < m_hitUpdateList.Length; i++)
            {
                var curItemList = m_hitUpdateList[i];
                if (curItemList == null) continue;
                bool needRemove = false;
                for (int j = 0; j < curItemList.Count; j++)
                {
                    var curItem = curItemList[j];
                    curItem.Update(delta);
                    if (curItem.canRemove)
                    {
                        needRemove = true;
                    }
                }
                if (needRemove)
                {
                    _removeItem(curItemList);
                }
            }
        }
        private void _removeItem(List<HitData> removeData)
        {
            for (int i = removeData.Count - 1; i >= 0; i++)
            {
                if (removeData[i].canRemove)
                {
                    removeData.RemoveAt(i);
                }
            }
        }


        private void _moveUpAllNum(int id, int type, HitData go)
		{
			if (!goNumberList.ContainsKey(id))
			{
				goNumberList.Add(id, new Dictionary<int, List<HitData>>());
			}

			if (!goNumberList[id].ContainsKey(type))
				goNumberList[id].Add(type, new List<HitData>());

			goNumberList[id][type].Add(go);

			float offset = 15f;
			if (type == 5)
				offset = 30f;

			for(int i=0; i<goNumberList[id][type].Count; ++i)
			{
				var hitData = goNumberList[id][type][i];
				if (hitData.canRemove)
				{
					PutHitData(hitData);
					goNumberList[id][type].RemoveAt(i);
					i--;
					continue;
				}

				var obj = goNumberList[id][type][i].go;
				if (obj != null)
				{
					var pos = obj.transform.localPosition;
					//obj.transform.DOLocalMoveY(pos.y + offset, 0.1f);
					pos.y += offset;
					obj.transform.localPosition = pos;
				}
			}
		}

		public void SetHitNumber(GameObject go, int number,  List<int> attachNumbers, HitTextType type, HitData hitData=null)
        {
#if DEBUG_REPORT_ROOT
            if (DebugSettings.instance.DisableHitText)
                return;
#endif

            var bind = go.GetComponent<ComCommonBind>();
			GameObject objText = null;
			if (bind != null)
			{
				var text = bind.GetCom<Text>("text");
				if (text != null)
				{
					text.text = number.ToString();
					objText = text.gameObject;
				}
					
				var textWhite = bind.GetCom<Text>("textWhite");
				if (textWhite != null)
					textWhite.text = number.ToString();
			}
				
			if (attachNumbers != null && attachNumbers.Count>0)
			{
				int index = 0;
				for(int i=0; i<attachNumbers.Count; ++i)
				{
					if (attachNumbers[i] <= 0)
						continue;

					GameObject attachText = null;
                    HitResType resType = HitResType.NORMAL_ATTACH;
					if (type == HitTextType.NORMAL)
					{
						resType =  HitResType.NORMAL_ATTACH;
					}
					else if (type == HitTextType.CRITICAL)
					{
						resType = HitResType.CRITICAL_ATTACH;
					}

					attachText = CGameObjectPool.instance.GetGameObject(ResInfoMap[(int)resType].resPath, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);

					if (attachText != null)
					{
						hitData.attachGo = attachText;
					}


					Utility.AttachTo(attachText, objText);

					var pos = attachText.transform.localPosition;
					pos.y -= 20* (index) + 10;
					attachText.transform.localPosition = pos;


					RunHitNumberAnimation(HitTextAniType.ATTACH, attachText, hitData);

					if (attachText != null)
					{
						Text at = attachText.GetComponent<Text>();
                        if (null != at)
                        {
                            at.text = attachNumbers[i].ToString();
                            hitData.text = at;
                        }
					}

					index++;
				}
			}
		}

        ///////////////////////////////////////////////////////////////
		public Vector2 ConvertWorldPosToWorldSpaceCanvas(Vec3 pos)
		{
            Vector2 newPos = Vector2.zero;
            if (BattleMain.instance != null)
            {
                if (worldCamera == null)
                {
                    if (BattleMain.instance != null && BattleMain.instance.Main != null
                        && BattleMain.instance.Main.currentGeScene != null
                        && BattleMain.instance.Main.currentGeScene.GetCamera() != null)
                        worldCamera = BattleMain.instance.Main.currentGeScene.GetCamera();
                }

                if (ui3drootRect == null)
                {
                    if (ClientSystemManager.GetInstance() != null && ClientSystemManager.GetInstance().Layer3DRoot != null)
                        ui3drootRect = ClientSystemManager.GetInstance().Layer3DRoot.GetComponent<RectTransform>();
                }

                if (worldCamera != null && ui3drootRect != null)
                {
					newPos = worldCamera.ProjectPositionToCanvas(new Vector3(pos.x, pos.z, pos.y), ui3drootRect);
                }
            }

            return newPos;
        }

		public void ShowBuffText(string picName, int id, Vec3 pos, BeActor owner)
        {
#if DEBUG_REPORT_ROOT
			if (DebugSettings.instance.DisableHitText)
                return;
#endif

            if(EngineConfig.useNewHitText)
            {
                ShowBuffTextNew(picName, id, pos, owner);
                return;
            }

                //友军，并且是低配
            if (isLowGraphic && (owner != null && owner.m_pkGeActor != null && owner.m_pkGeActor.GetUseCube()))
				return;

            HitResType type = HitResType.TEXT_BUFF_NAME;
			var go = GetGameObject(type);
			if (go == null)
				return;

			HitData hitData = null;
			int hitEffectType = 5;
			if (go != null)
			{
				hitData = GetHitData();
				hitData.go = go;

				AddHitUpdateList(type, hitData);
			}

			var bind = go.GetComponent<ComCommonBind>();
			if (bind != null)
			{
				var image = bind.GetCom<Text>("text");
				image.text = picName;
			}

			Vector2 newPos = ConvertWorldPosToWorldSpaceCanvas(pos);

			Utility.AttachTo(go, ClientSystemManager.instance.Layer3DRoot);

			go.transform.localPosition = new Vector3(newPos.x, newPos.y, -10);//最前面
			go.transform.localScale = new Vector3(1, 1, 1);

			_moveUpAllNum(id, hitEffectType, hitData);
		}

        public void ShowBuffTextNew(string picName, int id, Vec3 pos, BeActor owner)
        {
            //友军，并且是低配
            if (isLowGraphic && (owner != null && owner.m_pkGeActor != null && owner.m_pkGeActor.GetUseCube()))
                return;
            if (FrameSync.instance.IsInChasingMode) return;
            HitResType type = HitResType.TEXT_BUFF_NAME;

            int hitEffectType = 5;

            Vector2 newPos = ConvertWorldPosToWorldSpaceCanvas(pos);

            m_TextRendererManger.AddNameText(picName, new Vec3(newPos.x, newPos.y, -10), id, type, HitTextAniType.BUFFNAME, hitEffectType, 0);

            m_TextRendererManger.MoveUpAll(hitEffectType, id, HitTextAniType.BUFFNAME);
        }

        public void ResetNumber()
		{
			showHitNumberCurFrame = 0;
		}

		public void ClearHitNumber()
		{
            //hitUpdateList
            for (int i = 0; i < this.m_hitUpdateList.Length; i++)
            {
                var curItemList = m_hitUpdateList[i];
                if (curItemList == null) continue;
                for (int j = 0; j < curItemList.Count; j++)
                {
                    var curItem = curItemList[j];
                    if (!curItem.canRemove)
                    {
                        curItem.Remove();
                    }
                }
            }

            var emu2 = goNumberList.GetEnumerator();
			while(emu2.MoveNext())
			{
				var dic2 = emu2.Current.Value as Dictionary<int, List<HitData>>;
				var emu3 = dic2.GetEnumerator();
				while(emu3.MoveNext())
				{
					var hitDataList = emu3.Current.Value as List<HitData>;
					for(int i=0; i<hitDataList.Count; ++i)
					{
						if (hitDataList[i].canRemove)
						{
							PutHitData(hitDataList[i]);
						}
					}
				}
			}
			goNumberList.Clear();
		}

		int normalCount = 0;
		public void ShowHitNumber(int number, List<int> attachNumbers, int id, Vec3 pos, bool isFaceLeft, 
			HitTextType type, BeEntity attacker = null, BeEntity defender = null)
        {
#if DEBUG_REPORT_ROOT
			if (DebugSettings.instance.DisableHitText)
                return;
#endif
            if (EngineConfig.useNewHitText)
            {
                ShowHitNumberNew(number, attachNumbers, id, pos, isFaceLeft, type, attacker, defender);
                return;
            }


            HitTextAniType aniType = HitTextAniType.OTHER;

			showHitNumberCurFrame++;
			BeActor actorAttacker = attacker as BeActor;
			BeActor actorDefender = defender as BeActor;

            bool isLocalSummer = false;
            if (actorAttacker != null)
            {
                /*bool[] tempBoolArray = new bool[1];
                tempBoolArray[0] = isLocalSummer;
                actorAttacker.TriggerEvent(BeEventType.onChangeHitNumberType, new object[] { tempBoolArray });
                isLocalSummer = tempBoolArray[0];*/
                
                var ret = actorAttacker.TriggerEventNew(BeEventType.onChangeHitNumberType, new EventParam(){m_Bool = isLocalSummer});
                isLocalSummer = ret.m_Bool;
            }

            GameObject go = null;

			bool needSumHurt = false;

			string resPath = null;
			HitResType resType = HitResType.NORMAL;

			bool flag = false;


            //主角被击
            if (actorDefender != null && actorDefender.isLocalActor)
            {
                switch (type)
                {
                    case HitTextType.CRITICAL:
                    case HitTextType.NORMAL:
                    case HitTextType.BUFF_HURT:
                    case HitTextType.FRIEND_HURT:
                        resType = HitResType.OWN_HURT;
                        aniType = HitTextAniType.OWN;
                        needSumHurt = true;
                        break;
                }
            }
            //主角攻击
            else if (actorAttacker != null && (actorAttacker.isLocalActor || isLocalSummer)) 
			{
				switch (type)
				{
				case HitTextType.CRITICAL:
					resType = HitResType.CRITICAL;
					aniType = HitTextAniType.CRITIAL;
					break;
				case HitTextType.NORMAL:
					flag = true;
					aniType = HitTextAniType.NORMAL;
					resType = HitResType.NORMAL;
					break;
				case HitTextType.BUFF_HURT:
					aniType = HitTextAniType.BUFF;
					resType = HitResType.BUFF_HURT;
					break;
                case HitTextType.FRIEND_HURT:
                    resType = HitResType.FRIEND_HURT;
                    aniType = HitTextAniType.FRIEND;
                    break;
                }


			}
			//怪物或召唤攻击,被击
			else {
				//画质选择
				if (isLowGraphic)
					return;

				switch (type)
				{
				case HitTextType.CRITICAL:
				case HitTextType.NORMAL:
                case HitTextType.FRIEND_HURT:
					resType = HitResType.FRIEND_HURT;
					needSumHurt = true;
					aniType = HitTextAniType.FRIEND;
					break;
				case HitTextType.BUFF_HURT:
					resType = HitResType.BUFF_HURT;
					aniType = HitTextAniType.BUFF;
					break;
				}
			}

			switch (type)
			{
			case HitTextType.MISS:
				resType = HitResType.MISS;
				aniType = HitTextAniType.MISS;
				break;
			case HitTextType.RECOVE:
				resType = HitResType.TEXT_HP;
				break;
			case HitTextType.MP_RECOVER:
				resType = HitResType.TEXT_MP;
				break;
			}

			int hitEffectType = 0;
			switch (type)
			{
			case HitTextType.CRITICAL:
			case HitTextType.NORMAL:
				hitEffectType = 1;
				break;
			case HitTextType.BUFF_HURT:
				hitEffectType = 2;
				break;
			case HitTextType.RECOVE:
				hitEffectType = 3;
				break;
			case HitTextType.MP_RECOVER:
				hitEffectType = 4;
				break;
			}


			HitData hitData = null;
			{
				go = GetGameObject(resType);
				if (go != null)
				{
					if (flag)
					{
						normalCount++;
						//Logger.LogErrorFormat("normal count:{0}", normalCount);
					}

					hitData = GetHitData();
					hitData.go = go;

					AddHitUpdateList(resType, hitData);
				}
			}
			if (go == null)
				return;

			if (needSumHurt && null != attachNumbers && attachNumbers.Count > 0)
			{
				for(int i=0; i<attachNumbers.Count; ++i)
					number += attachNumbers[i];
			}

			Utility.AttachTo(go, ClientSystemManager.instance.Layer3DRoot);

			ORDER += 1;
			ORDER %= 100;

			pos.z = Mathf.Clamp(pos.z, 0f, 3.0f);

			Vector2 newPos = ConvertWorldPosToWorldSpaceCanvas(pos);

			go.transform.localPosition = new Vector3(newPos.x, newPos.y, -(float)ORDER * 0.625f);
			go.transform.localScale = new Vector3(1, 1, 1);

			if (showHitNumberCurFrame >= 2)
			{
				var p = go.transform.localPosition;
				p.y += 40 * (showHitNumberCurFrame - 1);
				go.transform.localPosition = p;
			}
				
			{
				RunHitNumberAnimation(aniType, go, hitData);
			}

			if (type != HitTextType.MISS)
			{
				_moveUpAllNum(id, hitEffectType, hitData);

				SetHitNumber(go, number, attachNumbers, type, hitData);
			}

            //Logger.LogErrorFormat("show hit component:restype:{0} anitype:{1} hitEffectType:{2} time:{3}", resType, aniType, hitEffectType, Time.realtimeSinceStartup*1000);
		}

        public void ShowHitNumberNew(int number, List<int> attachNumbers, int id, Vec3 pos, bool isFaceLeft,
            HitTextType type, BeEntity attacker = null, BeEntity defender = null)
        {
            if (!CheckHitNumIsNeedCreate(attacker, defender))
                return;

                HitTextAniType aniType = HitTextAniType.OTHER;

                showHitNumberCurFrame++;
                BeActor actorAttacker = attacker as BeActor;
                BeActor actorDefender = defender as BeActor;

                bool isLocalSummer = false;
                if (actorAttacker != null)
                {
                    /*bool[] tempBoolArray = new bool[1];
                    tempBoolArray[0] = isLocalSummer;
                    actorAttacker.TriggerEvent(BeEventType.onChangeHitNumberType, new object[] { tempBoolArray });
                    isLocalSummer = tempBoolArray[0];*/
                    
                    var ret = actorAttacker.TriggerEventNew(BeEventType.onChangeHitNumberType, new EventParam(){m_Bool = isLocalSummer});
                    isLocalSummer = ret.m_Bool;
                }

                GameObject go = null;

                bool needSumHurt = false;

                string resPath = null;
                HitResType resType = HitResType.NORMAL;

                bool flag = false;


                //主角被击
                if (actorDefender != null && actorDefender.isLocalActor)
                {
                    switch (type)
                    {
                        case HitTextType.CRITICAL:
                        case HitTextType.NORMAL:
                        case HitTextType.BUFF_HURT:
                            resType = HitResType.OWN_HURT;
                            aniType = HitTextAniType.OWN;
                            needSumHurt = true;
                            break;
                    case HitTextType.FRIEND_HURT:
                        resType = HitResType.FRIEND_HURT;
                        aniType = HitTextAniType.FRIEND;
                        needSumHurt = true;
                        break;
                }
                }
                //主角攻击
                else if (actorAttacker != null && (actorAttacker.isLocalActor || isLocalSummer))
                {
                    switch (type)
                    {
                        case HitTextType.CRITICAL:
                            resType = HitResType.CRITICAL;
                            aniType = HitTextAniType.CRITIAL;
                            break;
                        case HitTextType.NORMAL:
                            flag = true;
                            aniType = HitTextAniType.NORMAL;
                            resType = HitResType.NORMAL;
                            break;
                        case HitTextType.BUFF_HURT:
                            aniType = HitTextAniType.BUFF;
                            resType = HitResType.BUFF_HURT;
                            break;
                        case HitTextType.FRIEND_HURT:
                            resType = HitResType.FRIEND_HURT;
                            aniType = HitTextAniType.FRIEND;
                            break;
                    }
                }
                //怪物或召唤攻击,被击
                else
                {
                    //画质选择
                    if (isLowGraphic)
                        return;

                    switch (type)
                    {
                        case HitTextType.CRITICAL:
                        case HitTextType.NORMAL:
                        case HitTextType.FRIEND_HURT:
                            resType = HitResType.FRIEND_HURT;
                            aniType = HitTextAniType.FRIEND;
                            break;
                        case HitTextType.BUFF_HURT:
                            resType = HitResType.BUFF_HURT;
                            aniType = HitTextAniType.BUFF;
                            break;
                    }
                }

                switch (type)
                {
                    case HitTextType.MISS:
                        resType = HitResType.MISS;
                        aniType = HitTextAniType.MISS;
                        break;
                    case HitTextType.RECOVE:
                        resType = HitResType.TEXT_HP;
                        break;
                    case HitTextType.MP_RECOVER:
                        resType = HitResType.TEXT_MP;
                        break;
                }

                int hitEffectType = 0;
                switch (type)
                {
                    case HitTextType.CRITICAL:
                    case HitTextType.NORMAL:
                        hitEffectType = 1;
                        break;
                    case HitTextType.BUFF_HURT:
                        hitEffectType = 2;
                        break;
                    case HitTextType.RECOVE:
                        hitEffectType = 3;
                        break;
                    case HitTextType.MP_RECOVER:
                        hitEffectType = 4;
                        break;
                }

                ORDER += 1;
                ORDER %= 100;

                pos.z = Mathf.Clamp(pos.z, 0f, 3.0f);

                Vector2 newPos = ConvertWorldPosToWorldSpaceCanvas(pos);

            if (needSumHurt && null != attachNumbers && attachNumbers.Count > 0)
            {
                for (int i = 0; i < attachNumbers.Count; ++i)
                    number += attachNumbers[i];
            }

            m_TextRendererManger.AddText(number, new Vec3(newPos.x, newPos.y + 40 * (showHitNumberCurFrame - 1), -(float)ORDER * 0.625f), id, resType, aniType, hitEffectType, 0);

                if (type != HitTextType.MISS)
                {
                    //Add My Attach Number
                    if (attachNumbers != null && attachNumbers.Count > 0)
                    {
                        int index = 0;
                        for (int i = 0; i < attachNumbers.Count; ++i)
                        {
                            if (attachNumbers[i] <= 0)
                                continue;

                            HitResType attachResType = HitResType.NORMAL_ATTACH;
                            if (type == HitTextType.NORMAL)
                            {
                                attachResType = HitResType.NORMAL_ATTACH;
                            }
                            else if (type == HitTextType.CRITICAL)
                            {
                                attachResType = HitResType.CRITICAL_ATTACH;
                            }
                            if (attachResType == HitResType.NORMAL_ATTACH)
                            {
                                switch (resType)
                                {
                                    case HitResType.NORMAL:
                                        m_TextRendererManger.AddText(attachNumbers[i], new Vec3(newPos.x, newPos.y + 40 * (showHitNumberCurFrame - 1) - 20 * index * 0.65f/*attach 父物体缩放*/ + 10 - 15/*相对父物体的偏移*/, -(float)ORDER * 0.625f + 2f), id, attachResType, aniType, hitEffectType, 0);
                                        break;

                                    case HitResType.FRIEND_HURT:
                                        m_TextRendererManger.AddText(attachNumbers[i], new Vec3(newPos.x, newPos.y + 40 * (showHitNumberCurFrame - 1) - 20 * index + 10 - 20, -(float)ORDER * 0.625f + 2f), id, attachResType, aniType, hitEffectType, 1);
                                        break;

                                    case HitResType.OWN_HURT:
                                        m_TextRendererManger.AddText(attachNumbers[i], new Vec3(newPos.x - 20, newPos.y + 40 * (showHitNumberCurFrame - 1) - 20 * index + 10 - 50, -(float)ORDER * 0.625f + 2f), id, attachResType, aniType, hitEffectType, 2);
                                        break;

                                    case HitResType.TEXT_HP:
                                        m_TextRendererManger.AddText(attachNumbers[i], new Vec3(newPos.x - 20, newPos.y + 40 * (showHitNumberCurFrame - 1) - 20 * index * 0.7f + 10 - 50, -(float)ORDER * 0.625f + 2f), id, attachResType, aniType, hitEffectType, 3);
                                        break;

                                    case HitResType.TEXT_MP:
                                        m_TextRendererManger.AddText(attachNumbers[i], new Vec3(newPos.x - 20, newPos.y + 40 * (showHitNumberCurFrame - 1) - 20 * index + 10 - 50, -(float)ORDER * 0.625f + 2f), id, attachResType, aniType, hitEffectType, 4);
                                        break;

                                    case HitResType.BUFF_HURT:
                                        m_TextRendererManger.AddText(attachNumbers[i], new Vec3(newPos.x, newPos.y + 40 * (showHitNumberCurFrame - 1) - 20 * index * 0.7f - 35, -(float)ORDER * 0.625f + 2f), id, attachResType, aniType, hitEffectType, 5);
                                        break;

                                    default:
                                        break;
                                }

                            }
                            else if (attachResType == HitResType.CRITICAL_ATTACH)
                            {
                                m_TextRendererManger.AddText(attachNumbers[i], new Vec3(newPos.x, newPos.y + 40 * (showHitNumberCurFrame - 1) - 20 * index * 0.73f + 10, -(float)ORDER * 0.625f + 2f), id, attachResType, aniType, hitEffectType, 0);
                            }
                            index++;
                        }
                    }
                }
                m_TextRendererManger.MoveUpAll(hitEffectType, id, aniType);
        }


        public void RunHitNumberAnimation(HitTextAniType type, GameObject go, HitData hitData = null)
		{
			if (type != HitTextAniType.NORMAL && 
				type != HitTextAniType.ATTACH && 
				type != HitTextAniType.FRIEND)
				return;

			Color color;
			Text comImage = null;
			Text comImageWhite = null;

			var bind = go.GetComponent<ComCommonBind>();

			GameObject goText = null;
			if (bind != null)
			{
				comImage = bind.GetCom<Text>("text");
				if (comImage != null)
					goText = comImage.gameObject;
			}
			if (goText != null)
			{
				if (type != HitTextAniType.ATTACH)
					goText.transform.localPosition = Vector3.zero;
				color = comImage.color;
				color.a = 1.0f;
				comImage.color = color;
			}


			GameObject goWhite = null;
			if (bind != null)
			{
				comImageWhite= bind.GetCom<Text>("textWhite");
				if (comImageWhite != null)
					goWhite = comImageWhite.gameObject;
			}

			if (goWhite != null)
			{
				if (type != HitTextAniType.ATTACH)
					goWhite.transform.localPosition = Vector3.zero;
				color = comImageWhite.color;
				color.a = 1.0f;
				comImageWhite.color = color;
			}


			if (type == HitTextAniType.NORMAL)
			{
				var originScale = goText.transform.localScale;
				var originScaleWhite = goWhite.transform.localScale;

				var seq = DOTween.Sequence();
				seq.Append(goWhite.transform.DOScale(0.7f, 0.2f));
				seq.Join(comImage.DOFade(0, 0.01f));
				seq.Append(comImageWhite.DOFade(0, 0.01f));
				seq.AppendCallback(()=>{
					goWhite.transform.localScale = originScaleWhite;
				});

				seq.Append(comImage.DOFade(1.0f, 0.01f));
				seq.Append(goText.transform.DOLocalMoveY(25f, 0.7f));
				seq.Append(comImage.DOFade(0, 0.08f));
				seq.Join(goText.transform.DOLocalMoveY(25f, 0.08f));
             
				if (hitData != null)
					hitData.tween = seq;
			}
			else if (type == HitTextAniType.FRIEND)
			{
				var seq = DOTween.Sequence();
				seq.Append(comImage.DOFade(1.0f, 0.01f));
				seq.Append(goText.transform.DOLocalMoveY(25f, 0.7f));
				seq.Append(comImage.DOFade(0, 0.08f));
				seq.Join(goText.transform.DOLocalMoveY(25f, 0.08f));

				if (hitData != null)
					hitData.tween = seq;
			}
			/*			else if (type == HitTextAniType.CRITIAL)
			{
				var seq = DOTween.Sequence();
				seq.Append(goText.transform.DOScale(0.4f, 0.01f));
				seq.Append(goText.transform.DOScale(1.2f, 0.27f));
				seq.Join(goText.transform.DOLocalMove(new Vector3(-30f, 30f, 0f), 0.27f));
				seq.Append(comImage.DOFade(0f, 0.1f));
			}*/
			else if (type == HitTextAniType.ATTACH)
			{
				var seq = DOTween.Sequence();
				seq.Append(comImage.DOFade(1.0f, 0.01f));
				seq.AppendInterval(0.9f);
				seq.Append(comImage.DOFade(0f, 0.08f));

				if (hitData != null)
					hitData.attachTween = seq;
			}

		}

        /// <summary>
        /// 检查伤害冒字是否需要创建
        /// </summary>
        private bool CheckHitNumIsNeedCreate(BeEntity attacker,BeEntity target)
        {
            if (BattleMain.instance == null)
                return true;
            if (BattleMain.IsModePvP(BattleMain.battleType))
                return true;
            if (SettingManager.instance.GetCommmonSet(SettingManager.STR_HITNUMDISPLAY) != SettingManager.SetCommonType.Close)
                return true;
            if (attacker == null)
                return true;
            var attackerTopOwner = attacker.GetTopOwner(attacker) as BeActor;
            if (attackerTopOwner != null && attackerTopOwner.isLocalActor)
                return true;
            var targetActor = target as BeActor;
            if (targetActor != null && targetActor.isLocalActor)
                return true;
            return false;
        }

#else

        public ShowHitComponent(){}
		public void RefreshGraphicSetting(){}
		public void Update(int delta){}
		private void _moveUpAllNum(int id, int type, HitData go){}
		public void SetHitNumber(GameObject go, int number,  List<int> attachNumbers, HitTextType type, HitData hitData=null){}
		public Vector2 ConvertWorldPosToWorldSpaceCanvas(Vec3 pos){return Vector2.zero;}
		public void ShowBuffText(string picName, int id, Vec3 pos, BeActor owner){}
		public void ResetNumber(){}
		public void ClearHitNumber(){}
		public void ShowHitNumber(int number, List<int> attachNumbers, int id, Vec3 pos, bool isFaceLeft, 
			HitTextType type, BeEntity attacker = null, BeEntity defender = null){}
		public void RunHitNumberAnimation(HitTextAniType type, GameObject go, HitData hitData = null){}
        public void Clear() { }
        #endif


        #endregion
    }
}
