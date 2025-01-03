using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using ProtoTable;

namespace GameClient
{
    public class ComBuffIcon : MonoBehaviour
    {
        [SerializeField] GameObject mBuffIconGroup;
        [SerializeField] Image mMask;
        [SerializeField] Image mBuffIcon;
        [SerializeField] GameObject mOverFlowIcon;

        //public BeBuff BattleBuff
        //{
        //    get { return m_beBuff; }
        //    set { m_beBuff = value; }
        //}
        public int BeBuffPid
        {
            get { return m_beBuffPid; }
            set { m_beBuffPid = value; }
        }

        public byte ActorSeat
        {
            get { return m_ActorSeat; }
            set { m_ActorSeat = value; }
        }

        public BuffTable BattleBuffTableData
        {
            get { return m_beBuffTable; }
            set { m_beBuffTable = value; }
        }


        bool m_dirty = false;
        bool m_countDirty = false;
        //BeBuff m_beBuff = null;
        int m_beBuffPid = 0;
        byte m_ActorSeat = byte.MaxValue;
        BuffTable m_beBuffTable = null;

        bool m_needRecycle = false;
        public bool needRecycle { get { return m_needRecycle; } set { m_needRecycle = value; } }

        bool m_overFlowFlag = false;
        public bool overFlowFlag { get { return m_overFlowFlag; } set { m_overFlowFlag = value; } }

        public void MarkDirty()
        {
            m_dirty = true;
        }

        public void MarkCountDirty()
        {
            m_countDirty = true;
        }

        void Update()
        {
            if (m_dirty)
            {
                _Refresh();
                m_dirty = false;
            }
            if (m_countDirty)
            {
                _UpdatePercent();
                m_countDirty = false;
            }
        }

        void OnDestroy()
        {
            if(mBuffIconGroup != null)
            {
                var buffIconImage = mBuffIconGroup.GetComponent<Image>();
                if(buffIconImage != null)
                {
                    buffIconImage.sprite = null;
                }
                mBuffIconGroup = null;
            }
            if(mBuffIcon != null)
            {
                mBuffIcon.sprite = null;
                mBuffIcon = null;
            }
            if(mMask != null)
            {
                mMask.sprite = null;
                mMask = null;
            }
        }

        public void ResetIcon()
        {
            if(m_beBuffTable == null)
            {
                return;
            }
            gameObject.name = "comBuffIcon";
            m_dirty = false;
            m_countDirty = false;
            //m_beBuff = null;
            m_beBuffPid = 0;
            m_ActorSeat = byte.MaxValue;
            m_beBuffTable = null;

            Component[] allComs = gameObject.GetComponents<Component>();
            for (int i = 0; i < allComs.Length; i++)
            {
                if (!(
                    allComs[i] is ComBuffIcon ||
                    allComs[i] is RectTransform ||
                    allComs[i] is ComButtonEnbale ||
                    allComs[i] is AssetProxy ||
                    allComs[i] is CPooledGameObjectScript
                    ))
                {
                    GameObject.Destroy(allComs[i]);
                }
            }

            if (mBuffIconGroup != null)
            {
                gameObject.SetActive(false);
            }
        }

        protected void _Refresh()
        {
            _UpdateBuffIconGroup();
        }

        void _UpdateBuffIconGroup()
        {
            if (BattleBuffTableData != null)
            {
                gameObject.name = BattleBuffTableData.ID.ToString();
            }
            else
            {
                gameObject.name = "comBuffIcon";
            }
            
            _UpdateBuffIcon();
            _UpdatePercentImage();

            if (mOverFlowIcon != null && mOverFlowIcon.activeSelf == true)
            {
                mOverFlowIcon.SetActive(overFlowFlag);
            }

            if (mBuffIcon != null)
            {
                mBuffIcon.gameObject.SetActive(!overFlowFlag);
            }

            if (mMask != null)
            {
                mMask.gameObject.SetActive(!overFlowFlag);
            }
        }

        void _UpdateBuffIcon()
        {
            if(BattleBuffTableData == null)
            {
                return;
            }
            if (mBuffIcon != null)
            {
                mBuffIcon.CustomActive(true);
                ETCImageLoader.LoadSprite(ref mBuffIcon, BattleBuffTableData.Icon);
            }
        }

        void _UpdatePercentImage()
        {
            if (mMask != null) 
            {
                if (null != mMask.sprite)
                {
                    if (mMask.type != Image.Type.Filled) 
                    {
                        mMask.type = Image.Type.Filled;
                        mMask.fillMethod = Image.FillMethod.Radial360;
                        mMask.fillOrigin = (int)Image.Origin360.Top;
                        mMask.fillClockwise = false;
                    }
                    mMask.fillAmount = Mathf.Clamp01(0);
                }
            }
        }

        void _UpdateCount()
        {
            //TODO 需求未提
        }

        void _UpdatePercent()
        {
            if(mMask != null)
            {
                mMask.fillAmount = Mathf.Clamp01(1 - _GetBuffDurationForate());
            }
        }

        float _GetBuffDurationForate()
        {
            var actor = DungeonBuffDisplayFrame.GetPlayerBySeat(ActorSeat);
            if (actor == null)
                return 1;
            var BattleBuff = actor.buffController.GetBuffByPID(BeBuffPid);
            if(BattleBuff != null)
            {
                if(BattleBuff.duration == -1 || BattleBuff.duration >= 3600000)
                {
                    return 1;
                }
                return BattleBuff.GetLeftTime() * 1.0f / BattleBuff.duration;
            }
            return 1;
        }

        public void SetOverFlowIconActive(bool flag)
        {
            if(overFlowFlag != flag)
            {
                overFlowFlag = flag;
                if (mOverFlowIcon != null)
                {
                    mOverFlowIcon.SetActive(flag);
                }
                if (mMask != null)
                {
                    mMask.gameObject.SetActive(!flag);
                }
                if (mBuffIcon != null)
                {
                    mBuffIcon.gameObject.SetActive(!flag);
                }
            }
        }
    }
}
