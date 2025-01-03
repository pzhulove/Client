using UnityEngine;
using System.Collections.Generic;

namespace HeroInput
{
    public class ClickSkillBaseItem
    {
        public InputManager inputManager { set; protected get; }

        public delegate void AbortHandle();

        #region Button
        protected ETCButton mButton;
        public ETCButton button
        {
            set
            {
                mButton = value;
            }

            protected get
            {
                if (mButton == null)
                {
                    Logger.LogError("Button is nil");
                }
                return mButton;
            }
        }
        #endregion

        public int index { get; protected set; }

        public int pid { set; protected get; }

        public bool isDead { set; get; }

        public AbortHandle abortHandle { set; protected get; }

        protected bool mIsBeginClickChain = false;

        protected List<InputSkillComboData> mClickList = new List<InputSkillComboData>();
        protected List<int> mClickSkillIDList = new List<int>();


        public void SetClickList(InputSkillComboData[] list)
        {
            mClickList = new List<InputSkillComboData>(list);

            mClickSkillIDList.Clear();

            for (int i = 0; i < mClickList.Count; ++i)
            {
                var item = mClickList[i];
                //var skillID = inputManager.GetSkillIDBySlot(pid, item.slot);
                //mClickSkillIDList.Add(skillID);
            }

            _refreshButtonIcon();
        }

        protected int _getCurrentSkillID()
        {
            if (index >= mClickSkillIDList.Count)
            {
                Logger.LogErrorFormat("out of range with index {0}", index);
                return -1;
            }

            return mClickSkillIDList[index];
        }

        protected void _refreshButtonIcon()
        {
            if (button == null)
            {
                Logger.LogErrorFormat("nil button");
                return;
            }

            var skillID = _getCurrentSkillID();

            var skillItem = TableManager.instance.GetTableItem<ProtoTable.SkillTable>(skillID);
            if (skillItem == null)
            {
                Logger.LogErrorFormat("can't find skil item with id : {0}", skillID);
                return;
            }

            // button.SetFgImage(AssetLoader.instance.LoadRes(skillItem.Icon,typeof(Sprite)).obj as Sprite);
            button.SetFgImage("skillItem.Icon");
        }

        protected bool _realNextSkill()
        {
            ++index;

            Logger.LogFormat("real next skill {0}", index);

            if (index >= mClickList.Count)
            {
                return false;
            }

            return true;
        }

        protected void _resetSkill()
        {
            Logger.LogFormat("reset skill");
            index = 0;
            _refreshButtonIcon();
        }

        protected int _useSkill()
        {
            var skillID = _getCurrentSkillID();
            inputManager.CreateSkillFrameCommand(skillID);
            return skillID;
        }

        protected void _updateCurrentLeftTime(int deltaTime)
        {
            var skillID = _getCurrentSkillID();

            var skill = inputManager.controllActor.GetSkill(skillID);

            if (skill != null && skill.isCooldown)
            {
                button.UpdateFakeCoolDown(skill.CDTimeAcc, (int)skill.GetCurrentCD(),skill.isBuffSkill, inputManager.controllActor);
            }
            else
            {
                button.StopFakeCoolDown();
            }
        }

        #region virtual 
        // next skill 
        protected virtual void _nextSkill()
        {
            Logger.LogFormat("next skill");

            if (!_realNextSkill())
            {
                _abortSkill();
                return;
            }

            // change the new icon
            _refreshButtonIcon();
        }

        protected virtual void _abortSkill()
        {
            Logger.Log("Abort Skill");
            _resetSkill();

            if (abortHandle != null)
            {
                abortHandle();
            }

            mIsBeginClickChain = false;
        }

        protected virtual bool _canUseSkill()
        {
            var skillID = _getCurrentSkillID();
            var skill = inputManager.controllActor.GetSkill(skillID);

            if (skill == null)
            {
                return false;
            }

            if (!skill.CanUseSkill())
            {
                return false;
            }

            return true;
        }

        public virtual int UseSkill()
        {
            Logger.LogFormat("use skill with idx {0}", index);

            int skillID = -1;
            // use skill
            if (_canUseSkill() && !inputManager.controllActor.IsCastingSkill())
            {
                skillID = _useSkill();
                mIsBeginClickChain = true;

                _nextSkill();
            }

            return skillID;
        }

        public virtual void SkipSkill(int skillID)
        {
            var curSkillID = _getCurrentSkillID();
            if (skillID == curSkillID)
            {
                mIsBeginClickChain = true;
                _nextSkill();
            }
        }

        public virtual void Update(int deltaTime)
        {
            if (isDead)
            {
                return;
            }

            _updateCurrentLeftTime(deltaTime);
        }
        #endregion
    }

    public class ClickSkillItem : ClickSkillBaseItem
    {
        protected int mCurrentTime = 0;
        protected int _getCurrentTime()
        {
            if (index >= mClickList.Count)
            {
                Logger.LogErrorFormat("out of range with {0}", index);
                return -1;
            }

            return mClickList[index].time;
        }

        protected void _updateLeftTime(int deltaTime)
        {
            mCurrentTime -= deltaTime;
            if (mCurrentTime <= 0)
            {
                _abortSkill();
            }
        }

        // next skill 
        protected override void _nextSkill()
        {
            base._nextSkill();

            // current time;
            mCurrentTime = _getCurrentTime();

            if (!_canUseSkill())
            {
                _abortSkill();
            }
        }

        protected override bool _canUseSkill()
        {
            if (!base._canUseSkill())
            {
                return false;
            }

            var skillID = _getCurrentSkillID();
            var skill = inputManager.controllActor.GetSkill(skillID);

            if (mIsBeginClickChain && skill.isCooldown)
            {
                var curTime = _getCurrentTime();
                var leftTime = skill.CDLeftTime;
                return leftTime < curTime;
            }

            return true;
        }

        public override void Update(int deltaTime)
        {
            base.Update(deltaTime);

            if (!mIsBeginClickChain)
            {
                return;
            }

            _updateLeftTime(deltaTime);
        }
    }

    public class DragClickSkillItem : ClickSkillBaseItem
    {
        public override void Update(int deltaTime)
        {
            if (isDead)
            {
                return;
            }

            base.Update(deltaTime);
        }
    }

    public class InputButtonClickMode
    {
    }

    public class DirectionTree
    {
        private const int kDirectionCount = 4;
        private DirectionTree[] mDirection = new DirectionTree[kDirectionCount];
        private DirectionTree mParent;

        private int mSlot;

        public int slot
        {
            get
            {
                return mSlot;
            }

            set
            {
                mSlot = value;
            }
        }


        public DirectionTree AddChild(InputDirection dir, int slot)
        {
            var idx = (int)dir;
            if (idx >= mDirection.Length)
            {
                Logger.LogError("out of range");
                return null;
            }

            if (mDirection[idx] == null)
            {
                mDirection[idx] = new DirectionTree() { mParent = this, mSlot = slot};
            }

            return mDirection[idx];
        }

        public DirectionTree GetChild(InputDirection dir)
        {
            var idx = (int)dir;
            if (idx >= mDirection.Length)
            {
                Logger.LogError("out of range");
                return null;
            }

            return mDirection[(int)dir];
        }

        public void Clear()
        {
            mParent = null;
            for (int i = 0; i < kDirectionCount; ++i)
            {
                mDirection[i] = null;
            }
        }
    }
}
