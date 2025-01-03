using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    public class ComDungeonComboTips : MonoBehaviour
    {
        public ComDungeonComboUnit[] mUnits;
        public float mDealta = 0.8f;

        private void _useSkill(UIEvent ui)
        {
            int skillid = (int)ui.Param1;
            _pushSkill(skillid);
        }

        public List<int> mSkills = new List<int>();
        private float mCurTime = 0.0f;
        private int mCurIndex = 0;

        public void SetSkills(int[] skills)
        {
            mSkills = new List<int>(skills);
            mUnits = new ComDungeonComboUnit[mSkills.Count];
            for (int i = 0; i < mSkills.Count; ++i)
            {
                GameObject go = AssetLoader.instance.LoadRes("UIFlatten/Prefabs/BattleUI/DungeonComboUnit", typeof(GameObject)).obj as GameObject;
                Utility.AttachTo(go, this.gameObject);

                mUnits[i] = go.GetComponent<ComDungeonComboUnit>();
                mUnits[i].SetSkill(mSkills[i]);
            }

            _reset();
        }

        public void BindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonPlayerUseSkill, _useSkill);
        }

        public void UnbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonPlayerUseSkill, _useSkill);
        }

        public enum State
        {
            Wait,
            TimeOut,
            Fail,
            Succes,
            Reset,
        }

        public State mState = State.Wait;

        private void _tickTime()
        {
            mCurTime += Time.deltaTime;
            if (mCurTime >= mDealta)
            {
                mState = State.Reset;
            }
        }

        private void _reset()
        {
            mCurTime  = 0.0f;
            mCurIndex = 0;

            for (int i = 0; i < mUnits.Length; ++i)
            {
                mUnits[i].Reset();
            }

            mState = State.Wait;
        }

        private void _timeout()
        {
            mState = State.Reset;
        }

        private void _matchSkillFail()
        {
            for (int i = 0; i <= mCurIndex; ++i)
            {
                mUnits[i].PlayState(false);
            }

            mState = State.Fail;
        }

        private void _matchSkillSucces()
        {
            mCurTime = 0;

            mState = State.Wait;

            if (mCurIndex == mSkills.Count - 1)
            {
                mState = State.Succes;
            }

            mUnits[mCurIndex].PlayState(true);

            mCurIndex++;
            mCurIndex %= mSkills.Count;

        }

        private void _pushSkill(int id)
        {
            if (mState == State.Wait)
            {
				if (mCurIndex >= mSkills.Count)
					return;
				
                if (id == mSkills[mCurIndex])
                {
                    _matchSkillSucces();
                }
                else
                {
                    _matchSkillFail();
                }
            }
        }

        void Update()
        {
            _tickTime();

            switch (mState)
            {
                case State.Wait:
                    break;
                case State.TimeOut:
                    _timeout();
                    break;
                case State.Reset:
                    _reset();
                    break;
                case State.Fail:
                case State.Succes:
                    if (mCurTime > 1.0f)
                    {
                        mState = State.Reset;
                    }
                    break;
            }
        }
    }
}
