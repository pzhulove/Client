/// <summary>
/// 公共CD基类
/// </summary>
public abstract class CommonCDSkill : BeSkill
{
   private bool mIsInCommonCD;
   private int mCommonCDTimeAcc;
   
   protected CommonCDSkill(int sid, int skillLevel) : base(sid, skillLevel) { }

   /// <summary>
   /// 公共CD轨道，同轨道技能共享CD
   /// </summary>
   /// <returns></returns>
   protected abstract int CommonCDTrack { get; }

   /// <summary>
   /// 公共CD时间
   /// </summary>
   /// <returns></returns>
   protected abstract int CommonCD { get; }

   public bool IsInCommonCd
   {
      get => mIsInCommonCD;
      set => mIsInCommonCD = value;
   }

   public override bool isCooldown
   {
      get => base.isCooldown || IsInCommonCd;
      set => base.isCooldown = value;
   }

   public override int CDTimeAcc {
      get
      {
         if (IsInCommonCd)
         {
            return mCommonCDTimeAcc;
         }
         else
         {
            return base.CDTimeAcc;
         }
      }
   }

   public override int GetCurrentCD()
   {
      if (mIsInCommonCD)
      {
         return CommonCD;
      }
      else
      {
         return base.GetCurrentCD();
      }
   }

   public void StartCommonCD()
   {
      mCommonCDTimeAcc = 0;
      IsInCommonCd = true;
   }
   
   protected override void UpdateCoolDown(int deltaTime)
   {
      if (IsInCommonCd)
      {
         mCommonCDTimeAcc += deltaTime;
         if (mCommonCDTimeAcc >= CommonCD)
         {
            mIsInCommonCD = false;
         }
      }
      
      if (base.isCooldown)
      {
         cdTimeAcc += deltaTime;
         if (cdTimeAcc >= GetCurrentCD())
         {
            FinishCoolDown();
         }
      }
   }

   public override void OnStart()
   {
      base.OnStart();
      var skills = owner.skillController.GetSkillList();
      for (int i = 0; i < skills.Count; i++)
      {
         var skill = skills[i];
         
         if(skill == this)
            continue;
         
         var commonCDSkill = skill as CommonCDSkill;
         if(commonCDSkill == null)
            continue;

         if (commonCDSkill.CommonCDTrack == CommonCDTrack)
         {
            commonCDSkill.StartCommonCD();
         }
      }
   }
}
