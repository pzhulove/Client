using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine.UI;
/*
 * 波动刻印
*/


public class Rune
{
	public enum State
	{
		BORN = 0,
		STAND = 1,
		DEAD = 2,
	}

	int acc = 0;
	int lifeTime = 0;//30*1000;
	bool isDead = false;
	bool isConsumed = false;//是否是被消耗
	public State state = State.BORN;


	Mechanism22 mechanism = null;
	BeEntity owner = null;
	GameObject rotateDummy = null;
	Vector3 center = Vector3.zero;

	int resIndex = -1;

	static string[] runeRes = {
		"Effects/Hero_Axiuluo/bodongyinke/Perfab/Eff_bodongyinke_blue",
		"Effects/Hero_Axiuluo/bodongyinke/Perfab/Eff_bodongyinke_red",
		"Effects/Hero_Axiuluo/bodongyinke/Perfab/Eff_bodongyinke_yellow",
		"Effects/Hero_Axiuluo/bodongyinke/Perfab/Eff_bodongyinke_green",
		"Effects/Hero_Axiuluo/bodongyinke/Perfab/Eff_bodongyinke_brown",
	};

	public static Queue<int> runeResIndexQueue = new Queue<int>();



	string resStart = "Effects/Hero_Axiuluo/bodongyinke/Perfab/Eff_bodongyinke_fwfire";
	string resEnd = "Effects/Hero_Axiuluo/bodongyinke/Perfab/Eff_bodongyinke_fwend";

	public GeEffectEx effectRune = null;
	GeEffectEx effectEnd = null;

	public float angle = 0;


	float height = 2.4f;//Global.Settings.offset.x; 		//2f
	float initX = -0.45f;//Global.Settings.offset.y;		//-1f
	float rotateSpeed = 2.0f;//Global.Settings.offset.z;	//1f;
	float scale = 0.7f;//scale;

#if !LOGIC_SERVER

	public static void InitQueue()
	{
		runeResIndexQueue.Clear();
		for(int i=0; i<runeRes.Length; ++i)
			runeResIndexQueue.Enqueue(i);
	}

	public static int Dequeue()
	{
		if (runeResIndexQueue.Count <= 0)
			return 0;

		return runeResIndexQueue.Dequeue();
	}

	public static void Enqueue(int index)
	{
		runeResIndexQueue.Enqueue(index);
	}

	

	GeEffectEx  _CreateEffect(string res)
	{
		var effectStart = owner.m_pkGeActor.CreateEffect(res, null, 0f, new Vec3(0, 0, height), 1, 1, false, false, EffectTimeType.BUFF);
		effectStart.SetScale(scale);

		Battle.GeUtility.AttachTo(effectStart.GetRootNode(), owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));

		return effectStart;
	}
		
	void CreateEffect()
	{
		var effectRune2 = _CreateEffect(runeRes[resIndex]);
		effectRune2.SetTimeLen(700);

		owner.delayCaller.DelayCall(100, ()=>{
			_CreateEffect(resStart);
		});
			
		owner.delayCaller.DelayCall(700, ()=>{
			owner.m_pkGeActor.DestroyEffect(effectRune2);

			StartRotation();
		});

        // 音效
		if (owner.CurrentBeBattle != null)
			owner.CurrentBeBattle.PlaySound(3026);

    }

	void StartRotation()
	{
		effectRune = owner.m_pkGeActor.CreateEffect(runeRes[resIndex], null, 100000, new Vec3(0, 0, 0), 1, 1, false, false, EffectTimeType.BUFF);
		effectRune.SetScale(scale);

		rotateDummy = new GameObject("rotateDummy");

		Battle.GeUtility.AttachTo(rotateDummy, owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));

		rotateDummy.transform.localPosition = new Vector3(initX, height, 0);

		center = new Vector3(owner.GetPosition().fx, height, owner.GetPosition().fy);

		state = State.STAND;

		mechanism.RepositionRunes();
	}

	void UpdateRotation(float specifyAngle=0, int delta = 0)
	{
		/*
		center.x = owner.GetPosition().x;
		center.z = owner.GetPosition().y;
		center.y = height;
*/


		center = owner.m_pkGeActor.GetPosition();
		center.y = height;


		if (effectRune.isVisible())
		{
			float tmpAngle = (specifyAngle>0?specifyAngle:rotateSpeed);

			int fpsDelta = 16;

			//delta 16ms
			if (specifyAngle <= 0)
			{
				tmpAngle = rotateSpeed * delta / (float)fpsDelta;
			}

			rotateDummy.transform.RotateAround(center, Vector3.up, -tmpAngle);
			if (specifyAngle <= 0)
				angle += tmpAngle;
			if (angle >= 360)
				angle -= 360;
		}
			
		effectRune.SetPosition(rotateDummy.transform.position);
	}

	public void SetRotationAngle(float specifyAngle)
	{
		var newAngle = specifyAngle - angle;
		newAngle = newAngle<0?(360+newAngle):newAngle;

		UpdateRotation(newAngle);
		angle = specifyAngle;


	}

    public void UpdateGraphic(int delta)
	{
		if (effectRune != null)
		{
			UpdateRotation(0, delta);
		}
	}

	public void OnRemove()
	{
		state = State.DEAD;
		//Logger.LogErrorFormat("onremove rune:{0}", resIndex);

		if (effectRune != null)
		{
			if (isConsumed)
			{
				if (effectRune != null)
				{
					Enqueue(resIndex);
					owner.m_pkGeActor.DestroyEffect(effectRune);
					GameObject.Destroy(rotateDummy);
					effectRune = null;
				}

			}
			else {
				effectEnd = owner.m_pkGeActor.CreateEffect(resEnd, null, 0f, new Vec3(0, 0, 0), 1, 1, false, false, EffectTimeType.BUFF);
				//effectEnd.SetScale(scale);

				owner.delayCaller.DelayCall(100, ()=>{
					effectRune.SetVisible(false);
				});

				rotateDummy.transform.localRotation = Quaternion.identity;
				Battle.GeUtility.AttachTo(effectEnd.GetRootNode(), rotateDummy);

				owner.delayCaller.DelayCall(700, ()=>{
					if (effectEnd != null)
					{
						owner.m_pkGeActor.DestroyEffect(effectEnd);
						effectEnd = null;
					}

					if (effectRune != null)
					{
						Enqueue(resIndex);
						owner.m_pkGeActor.DestroyEffect(effectRune);
						GameObject.Destroy(rotateDummy);
						effectRune = null;
					}

					mechanism.RepositionRunes();
				});
			}



		}

		if (owner.CurrentBeBattle != null)
			owner.CurrentBeBattle.PlaySound(3027);

    }


#else
    public static void InitQueue(){}
	public static int Dequeue(){return 0;}
	public static void Enqueue(int index){}
	GeEffectEx  _CreateEffect(string res){return null;}
	void CreateEffect(){}
	void StartRotation(){}
	void UpdateRotation(float specifyAngle=0, int delta = 0){}
	public void SetRotationAngle(float specifyAngle){}
	public void UpdateGraphic(int delta){}
	public void OnRemove(){}
#endif

    public Rune(int lt, BeEntity owner, Mechanism22 m)
    {
        lifeTime = lt + 1300;

#if !LOGIC_SERVER
        this.owner = owner;
		this.mechanism = m;

		resIndex = Dequeue();
	
		CreateEffect();
#endif
    }

    public void Update(int delta)
    {
        if (isDead)
			return;

		/*if (effectRune != null)
		{
			UpdateRotation();
		}*/
		
		acc += delta;
		if (acc >= lifeTime)
		{
			acc = 0;
			isDead = true;
		}
	}



    public bool IsDead()
    {
        return isDead;
    }

    public void SetDead(bool dead)
    {
        isDead = dead;
        isConsumed = true;
    }
}

public class Mechanism22 : BeMechanism {
	public Mechanism22(int mid, int lv):base(mid, lv){
		canRemove = false;
	}

	CrypticInt32 maxRuneNum = 0;
	int buffID = 0;
	int runeLiftTime = 0;
	readonly int targetSkillID = 1710;

	List<Rune> runes = new List<Rune>();

    IBeEventHandle handle1 = null;
    IBeEventHandle handle2 = null;

/*	GameObject go = null;
	Text txtNum = null;*/

    System.UInt32 audioHandle = 0;
    public List<Rune> getRuneList()
    {
        return runes;
    }

	public override void OnReset()
	{
		maxRuneNum = 0;
		buffID = 0;
		runes.Clear();
		handle1 = null;
		handle2 = null;
		audioHandle = 0;
	}

    public override void OnInit ()
	{
		maxRuneNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		runeLiftTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		buffID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
	}

	public override void OnStart ()
	{
		Rune.InitQueue();

		//Logger.LogErrorFormat("onAdd Mechanism22");
		var skill = owner.GetSkill(targetSkillID);
		if (skill != null)
		{
			level = skill.level;
		}

        RemoveHandle();
		if (owner != null)
		{
            handle1 = owner.RegisterEventNew(BeEventType.onAddRune, (args)=>{
				AddRune();	
			});

            handle2 = owner.RegisterEventNew(BeEventType.onClearRune, (args)=>{
				RemoveRune(true);	
			});
		}
    }

    void RemoveHandle()
    {
        if (handle1 != null)
        {
            handle1.Remove();
            handle1 = null;
        }

        if (handle2 != null)
        {
            handle2.Remove();
            handle2 = null;
        }
    }


		
	public override void OnUpdate (int deltaTime)
	{
		for(int i=0; i<runes.Count; ++i)
		{
			var rune = runes[i];
			if (rune.IsDead())
			{
				rune.OnRemove();
				runes.RemoveAt(i);
				OnRemoveRune();
				i--;
				//Logger.LogErrorFormat("remove runes:{0}", runes.Count);
			}
			else {
				rune.Update(deltaTime);
			}
		}
	}

	public override void OnUpdateGraphic (int deltaTime)
	{
		for(int i=0; i<runes.Count; ++i)
		{
			var rune = runes[i];
			if (!rune.IsDead())
			{
				rune.UpdateGraphic(deltaTime);
			}
		}
	}

	public int GetRuneCount()
	{
		return runes.Count;
	}
		
	public void AddRune()
	{
		if (runes.Count < maxRuneNum)
		{
			runes.Add(new Rune(runeLiftTime, owner, this));

			OnAddRune();

			//Logger.LogErrorFormat("add runes:{0}", runes.Count);
		}

        if (runes.Count == 1)
		{
			if (owner.CurrentBeBattle != null)
				owner.CurrentBeBattle.PlaySound(3025);
		}

    }

	public void RemoveRune(bool all=false)
	{
		for(int i=0; i<runes.Count; ++i)
		{
			var rune = runes[i];
			if (!rune.IsDead())
			{
				rune.SetDead(true);
			}
		}
        //owner.TriggerEvent(BeEventType.OnConsumeRune, new object[] { runes.Count });
        owner.TriggerEventNew(BeEventType.OnConsumeRune, new EventParam(){m_Int = runes.Count});
    }

	public void RepositionRunes()
	{

		if (runes.Count >= 1)
		{
			float startAngle = -1f;//runes[0].angle;

			int count = 0;
			for(int i=0; i<runes.Count; ++i)
			{
				if (runes[i].state == Rune.State.STAND)
				{
					count++;
					if (startAngle <= -1f)
						startAngle = runes[i].angle;
				}
			}

			for(int i=0, j=0; i<runes.Count; ++i)
			{
				if (runes[i].state == Rune.State.STAND)
				{
					runes[i].SetRotationAngle((startAngle + j*(360/count)) % 360);
					j++;

					if (runes[i].effectRune != null)
						runes[i].effectRune.Play(true);
				}

			}
		}
	}

	protected void OnAddRune()
	{
		owner.buffController.TryAddBuff(buffID, int.MaxValue, level);
	}

	protected void OnRemoveRune()
	{
		owner.buffController.RemoveBuff(buffID, 1);

		if (runes.Count <= 0)
			Rune.InitQueue();

        if (runes.Count < 1 && audioHandle != 0)
            AudioManager.instance.Stop(audioHandle);
    }

	public override void OnFinish ()
	{
		RemoveRune(true);
	}

    public override void OnDead()
    {
        RemoveRune(true);
    }
}
