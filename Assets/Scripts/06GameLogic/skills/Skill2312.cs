using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameClient;
using UnityEngine;

/// <summary>
/// 炫纹强压
/// 
///"1.施法读条与释放时自身霸体
///2.施法有读条
///3.会对目标造成y轴的浮空力
///4.此技能的魔法攻击力为所有炫纹攻击力之和的N%（可成长）
///5.魔法攻击力的百分比 随技能等级提升
///1.攻击范围随炫纹的数量递增
///（数值公式）——徐鑫伟
///技能魔法攻击力为 所有炫纹攻击力之和的N%(不考虑排列组合，只考虑数量)"
/// </summary>
public class Skill2312: BeSkill
{
   private readonly Vector3 mCenterPos = new Vector3(-0.5f, 2.5f, 0f);
   private readonly int mRotateRadius = 1;
   private List<Mechanism2072.ChaserData> mChaserList = new List<Mechanism2072.ChaserData>();
   private int mChaserCount = 0;
   private Mechanism2072 mChaserMgr = null;
   private IBeEventHandle mGenBulletHandle;
   private IBeEventHandle mDamageHandle;
   
   private int mTargetBulletId = 0;
   private List<int> mBulletScale = new List<int>();
   private HashSet<int> mTargetHurtID = new HashSet<int>();
   private VFactor mHurtRate = VFactor.one;
   protected int[] m_NormalHurtIdArr = new int[(int)Mechanism2072.ChaserType.Max];
   protected int[] m_BigHurtIdArr = new int[(int)Mechanism2072.ChaserType.Max];

   private VRate m_SkillAttackAddRate = VRate.zero;
   private bool m_Skilling = false;
   private int m_ChaserLevel = 1;
   public Skill2312(int sid, int skillLevel) : base(sid, skillLevel)
   {
   }

   public override void OnInit()
   {
      base.OnInit();
      mTargetBulletId = !BattleMain.IsModePvP(owner.battleType)
       ? TableManager.GetValueFromUnionCell(skillData.ValueA[0], level)
       : TableManager.GetValueFromUnionCell(skillData.ValueA[1], level);

      if (BattleMain.IsModePvP(battleType))
      {
         for (int i = 0; i < skillData.ValueG.Count; i++)
         {
            mBulletScale.Add(TableManager.GetValueFromUnionCell(skillData.ValueG[i], level));
         }
      }
      else
      {
         for (int i = 0; i < skillData.ValueB.Count; i++)
         {
            mBulletScale.Add(TableManager.GetValueFromUnionCell(skillData.ValueB[i], level));
         }
      }
      
      for (int i = 0; i < skillData.ValueC.Count; i++)
      {
         mTargetHurtID.Add(TableManager.GetValueFromUnionCell(skillData.ValueC[i], level));
      }

      mHurtRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(skillData.ValueD[0], level), GlobalLogic.VALUE_1000);
      
      for (int i = 0; i < skillData.ValueE.Count && i < m_NormalHurtIdArr.Length; i++)
      {
         m_NormalHurtIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueE[i], level);
      }

      for (int i = 0; i < skillData.ValueF.Count && i < m_BigHurtIdArr.Length; i++)
      {
         m_BigHurtIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueF[i], level);
      }
   }
   
   public override void OnPostInit()
   {
      if(owner == null)
         return;
      
      base.OnPostInit();
      GetChaserMgr();
   }
   
   /// <summary>
   /// 装备机制，修改技能伤害百分比
   /// </summary>
   private void UpdateEquipData()
   {
      if(owner == null)
         return;

      m_SkillAttackAddRate = VRate.zero;
      List<BeMechanism> mechanismList = owner.MechanismList;
      if (mechanismList == null)
         return;
      for (int i = 0; i < mechanismList.Count; i++)
      {
         if(!mechanismList[i].isRunning)
            continue;
         
         var mechanism = mechanismList[i] as Mechanism2091;
         if (mechanism == null)
            continue;

         m_SkillAttackAddRate += mechanism.SkillAttackAddRate;
      }
   }

   private void AddSkillAttackRate(int count)
   {
      this.attackAddRate += m_SkillAttackAddRate.i * count;
      m_Skilling = true;

   }
   
   private void ClearSkillAttackRate(int count)
   {
      this.attackAddRate -= m_SkillAttackAddRate.i * count;
      m_Skilling = false;
   }

   public override bool CanUseSkill()
   {
      bool baseCanUse = base.CanUseSkill();
      // 技能时序问题，可能取不到
      GetChaserMgr();
      if (mChaserMgr == null)
         return false;
      
      return mChaserMgr.GetChaserCount() > 0 && baseCanUse;
   }

   protected void RegisterHandle()
   {
      if(owner == null)
         return;
      
      mGenBulletHandle = owner.RegisterEventNew(BeEventType.onAfterGenBullet, OnAfterGenBullet);
      mDamageHandle = owner.RegisterEventNew(BeEventType.onReplaceHurtTableDamageData, ReplaceHurtTableDamageData);
   }
   
   public override void OnStart()
   {
      base.OnStart();
      
      GetChaserMgr();
      if (mChaserMgr == null)
         return;
      
      ClearSkillAttackRate(mChaserCount);
      GetChaserList();
      CompoundChaser();
      UpdateEquipData();
      AddSkillAttackRate(mChaserCount);
      GetChaserLevel();
      Mechanism2072.AddBuff(mChaserList, battleType, owner, m_ChaserLevel);
      
      RemoveHandle();
      RegisterHandle();
   }

   /// <summary>
   /// 取下炫纹发射的等级,如果没有技能取1级
   /// </summary>
   private void GetChaserLevel()
   {
      if (owner != null)
      {
         int level = owner.GetSkillLevel(2302);
         if (level == 0)
         {
            m_ChaserLevel = 1;
         }
         else
         {
            m_ChaserLevel = level;
         }
      }
   }
   
   /// <summary>
   /// 根据炫纹数量，修改爆炸大小
   /// </summary>
   /// <param name="args"></param>
   private void OnAfterGenBullet(BeEvent.BeEventParam param)
   {
      BeProjectile proj = param.m_Obj as BeProjectile;
      if(proj == null)
         return;

      if (proj.m_iResID == mTargetBulletId)
      {
         int countIndex = mChaserList.Count - 1;
         if (countIndex < mBulletScale.Count)
         {
            proj.SetScale(mBulletScale[countIndex]);
         }
         else
         {
            Logger.LogErrorFormat("炫纹强压 大小修正错误：{0}缩放系数没配", countIndex);
         }
      }
   }
   
   
   /// <summary>
   /// 根据炫纹数量，修改伤害
   /// </summary>
   /// <param name="param"></param>
   protected void ReplaceHurtTableDamageData(BeEvent.BeEventParam param)
   {
         int hurtId = param.m_Int;
         if (mTargetHurtID.Contains(hurtId))
         {
            int[] hurtIdArr = GetChaserMixHurtIdArr();
            if (hurtIdArr == null)
               return;
            
            int damageValue = 0;
            VPercent damagePervent = VPercent.zero;
            bool isPvPMode = BattleMain.IsModePvP(battleType);
            for(int i=0;i< hurtIdArr.Length; i++)
            {
               ProtoTable.EffectTable hurtData = TableManager.instance.GetTableItem<ProtoTable.EffectTable>(hurtIdArr[i]);
               damageValue += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageFixedValuePVP : hurtData.DamageFixedValue, m_ChaserLevel);
               damagePervent += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageRatePVP : hurtData.DamageRate, m_ChaserLevel);
            }
            param.m_Int2 = damageValue * mHurtRate;
            var addFactor = damagePervent.precent * mHurtRate;
            param.m_Percent = new VPercent(addFactor.single);
         }
   }
   
   /// <summary>
   /// 获取炫纹类型对应的触发效果ID列表
   /// </summary>
   protected int[] GetChaserMixHurtIdArr()
   {
      if (mChaserList == null)
      {
         Logger.LogErrorFormat("炫纹强压数据为空，请检查,当前技能ID{0}", owner.GetCurSkillID());
         return null;
      }
      
      var hurtIdList = GamePool.ListPool<int>.Get();
      for(int i = 0; i < mChaserList.Count; i++)
      {
         var chaserData = mChaserList[i];
         int typeIndex = (int)chaserData.type;
         int sizeType = (int)chaserData.sizeType;
         hurtIdList.Add(sizeType == 0 ? m_NormalHurtIdArr[typeIndex] : m_BigHurtIdArr[typeIndex]);
      }

      var hurtIdArr = hurtIdList.ToArray();
      GamePool.ListPool<int>.Release(hurtIdList);
      return hurtIdArr;
   }
   
   public override void OnFinish()
   {
      mChaserList.Clear();
      base.OnFinish();
   }

   public override void OnCancel()
   {
      base.OnCancel();
      mChaserList.Clear();
   }
   
   /// <summary>
   /// 炫纹出队,获取当前炫纹数量等数据
   /// </summary>
   private void GetChaserList()
   {
      if (mChaserMgr == null)
         return ;

      mChaserList.Clear();
      mChaserCount = mChaserMgr.GetChaserCount();
      mChaserMgr.LaunchChaser(mChaserCount, mChaserList);
      mChaserMgr.ReduceChaserCount(mChaserCount);
   }
   
   /// <summary>
   /// 融合炫纹，飞行动画
   /// 1个炫纹，直接飞到中心点
   /// 多个炫纹，飞到中心点外一圈，然后飞到中心点
   /// </summary>
   private void CompoundChaser()
   {
#if !LOGIC_SERVER
      if(owner == null)
         return;
      
      if (mChaserList == null)
         return ;
      
      float speed = 1f;
      if (skillSpeed > 0)
      {
         speed = 1000f / skillSpeed;
      }
      float flyOutTime = 0.5f * speed;
      float flyInTime = 0.5f * speed;
      Vector3 centerPos = mCenterPos;
      centerPos.x = owner.GetFace() ? mCenterPos.x * -1 : mCenterPos.x;
      
      if (mChaserCount == 1)
      {
         if (mChaserList.Count == 1)
         {
            var data = mChaserList[0];
            var quence = DOTween.Sequence();
            if (data != null && data.effect != null)
            {
               if (data.effect.GetRootNode() != null)
               { 
                  quence.Append(data.effect.GetRootNode().transform.DOLocalMove( centerPos, (flyInTime + flyOutTime)).SetEase(Ease.InQuint));
               }

               if (owner != null && owner.m_pkGeActor != null)
               {
                  quence.OnComplete(() =>
                  {
                     if (data.effect != null && owner != null && owner.m_pkGeActor != null)
                     {
                        owner.m_pkGeActor.DestroyEffect(data.effect);
                     }
                  });
               }
               quence.Play();   
            }
         }
      }
      else
      {
         float averageAngle = 360.0f / mChaserList.Count;
      
         var mainQuence = DOTween.Sequence();
         for (int i = 0; i < mChaserList.Count; i++)
         {
            float curAngle = averageAngle * i;
            float x = mRotateRadius * Mathf.Cos(curAngle * 3.14f / 180);
            float y = mRotateRadius * Mathf.Sin(curAngle * 3.14f / 180);
            var pos = new Vector3(x, y, 0);
         
            var data = mChaserList[i];
            var quence = DOTween.Sequence();
            if (data != null && data.effect != null)
            {
               if (data.effect.GetRootNode() != null)
               {
                  quence.Append(data.effect.GetRootNode().transform.DOLocalMove(centerPos + pos, flyOutTime).SetEase(Ease.OutQuad));
                  quence.Append(data.effect.GetRootNode().transform.DOLocalMove( centerPos, flyInTime).SetEase(Ease.InQuint));   
               }

               if (owner != null && owner.m_pkGeActor != null)
               {
                  quence.OnComplete(() =>
                  {
                     if (data.effect != null && owner != null && owner.m_pkGeActor != null)
                     {
                        owner.m_pkGeActor.DestroyEffect(data.effect);
                     }
                  });
               }
            }
            mainQuence.Join(quence);
         }
         mainQuence.Play();
      }
#endif
   }


   
   private void GetChaserMgr()
   {
      if(owner == null)
         return ;

      if (mChaserMgr != null)
         return;
      
      var baseMech = owner.GetMechanism(Mechanism2072.ChaserMgrID);
      if (baseMech == null)
         return;
        
      var mechanism = baseMech as Mechanism2072;
      mChaserMgr = mechanism;
   }
   
   private void RemoveHandle()
   {
      if (mGenBulletHandle != null)
      {
         mGenBulletHandle.Remove();
         mGenBulletHandle = null;
      }
            
      if (mDamageHandle != null)
      {
         mDamageHandle.Remove();
         mDamageHandle = null;
      }
   }
}
