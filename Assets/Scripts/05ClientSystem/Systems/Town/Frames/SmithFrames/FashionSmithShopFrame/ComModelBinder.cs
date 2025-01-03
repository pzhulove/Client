using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GeAvatarRendererExIdleController
    {
        public const float skWaitTime = 6.0f;

        private readonly string[] mActionTable = new string[2] { "Anim_Show_Idle", "Anim_Show_Idle_special01" };
        private readonly int[] mPlayList       = new int[] { 0, 1 };

        private int   mLastActionIdx = 0;
        private float mTickWaitTime = 0;
        private int   mPlayIdx = 0;
        
        private GeAvatarRendererEx mAvatar;

        public void Init(GeAvatarRendererEx avatar)
        {
            mAvatar = avatar;
        }

        public void UnInit()
        {
            mAvatar = null;
        }

        public void Clear()
        {
            mPlayIdx = 0;
        }

        public void OnUpdate(float timeElapsed)
        {
            if (null != mAvatar)
            {
                while (global::Global.Settings.avatarLightDir.x > 360)
                    global::Global.Settings.avatarLightDir.x -= 360;
                while (global::Global.Settings.avatarLightDir.x < 0)
                    global::Global.Settings.avatarLightDir.x += 360;

                while (global::Global.Settings.avatarLightDir.y > 360)
                    global::Global.Settings.avatarLightDir.y -= 360;
                while (global::Global.Settings.avatarLightDir.y < 0)
                    global::Global.Settings.avatarLightDir.y += 360;

                while (global::Global.Settings.avatarLightDir.z > 360)
                    global::Global.Settings.avatarLightDir.z -= 360;
                while (global::Global.Settings.avatarLightDir.z < 0)
                    global::Global.Settings.avatarLightDir.z += 360;

                mAvatar.m_LightRot = global::Global.Settings.avatarLightDir;

                mTickWaitTime -= timeElapsed;

                if (mTickWaitTime <= 0.0f && mAvatar.IsCurActionEnd())
                {
                    if (mLastActionIdx != mPlayList[mPlayIdx])
                    {
                        mAvatar.ChangeAction(mActionTable[mPlayList[mPlayIdx]]);

                        mLastActionIdx = mPlayList[mPlayIdx];
                        if (mLastActionIdx == 0)
                        {
                            mTickWaitTime = skWaitTime;
                        }
                        else
                        {
                            mTickWaitTime = 0.0f;
                        }
                    }

                    ++mPlayIdx;
                    mPlayIdx = mPlayIdx % mPlayList.Length;
                }
            }
        }

    }

    public class ComModelBinder : MonoBehaviour
    {
        public RawImage mRawImage;
        public GeAvatarRendererEx mAvatar;
        private GeAvatarRendererExIdleController mAvatarController;
        bool mDirty = false;

        #region avatar
        int mOccu = 0;
        public void LoadAvatar(int occu)
        {
            mOccu = occu;
            mDirty = true;
        }

        void _LoadAvatar(int occu)
        {
            ProtoTable.JobTable job = TableManager.instance.GetTableItem<ProtoTable.JobTable>(occu);
            if (null != job)
            {
                ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(job.Mode);
                if (null != res)
                {
                    if (null != mAvatar)
                    {
                        mAvatar.LoadAvatar(res.ModelPath);
                        //mAvatar.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
                        //mAvatar.ChangeAction(ms_ActionTable[0], 1.0f, true);
                    }
                }
            }
        }
        #endregion

        #region fashions
        List<ItemData> mFashions = null;
        public void SetFashions(List<ItemData> datas)
        {
            mFashions = datas;
            mDirty = true;
        }
        public void SetFashion(ItemData data)
        {
            if (_IsFashion(data) && null != mAvatar)
            {
                EFashionWearSlotType slot = EFashionWearSlotType.Invalid;
                GeAvatarChannel channel = GeAvatarChannel.MaxChannelNum;

                PlayerBaseData.GetInstance().GetFashionSlotChangedType(ref slot, ref channel, data.TableData, true);

                if (EFashionWearSlotType.Invalid != slot)
                {
                    PlayerBaseData.GetInstance().AvatarEquipPart(mAvatar, slot, data.TableID, null);
                }
            }
        }

        void _SetFashions(List<ItemData> datas)
        {
            if (null != mAvatar)
            {
                List<int> slots = GamePool.ListPool<int>.Get();
                slots.Add((int)EFashionWearSlotType.UpperBody);
                slots.Add((int)EFashionWearSlotType.LowerBody);
                slots.Add((int)EFashionWearSlotType.Head);

                if (null != datas)
                {
                    for (int i = 0; i < datas.Count; ++i)
                    {
                        if (!_IsFashion(datas[i]))
                        {
                            continue;
                        }

                        EFashionWearSlotType slot = EFashionWearSlotType.Invalid;
                        GeAvatarChannel channel = GeAvatarChannel.MaxChannelNum;
                        PlayerBaseData.GetInstance().GetFashionSlotChangedType(ref slot, ref channel, datas[i].TableData, true);

                        if (slot == EFashionWearSlotType.Invalid)
                        {
                            continue;
                        }

                        slots.Remove((int)slot);

                        PlayerBaseData.GetInstance().AvatarEquipPart(mAvatar, slot, datas[i].TableID, null);
                    }

                    bool hasWind = false;
                    bool hasHalo = false;
                    for (int i = 0; i < datas.Count; ++i)
                    {
                        if (!_IsFashion(datas[i]))
                        {
                            continue;
                        }

                        if (datas[i].SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_HAIR)
                        {
                            //continue;
                            PlayerBaseData.GetInstance().AvatarEquipWing(mAvatar, datas[i].TableID, null);
                            hasWind = true;
                            //break;
                        }
                        else if (datas[i].SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_AURAS)
                        {
                            PlayerBaseData.GetInstance().AvatarEquipHalo(mAvatar, datas[i].TableID, null);
                            hasHalo = true;
                            //break;
                        }                  
                    }

                    if (!hasWind)
                    {
                        PlayerBaseData.GetInstance().AvatarEquipWing(mAvatar, 0, null);
                    }

                    if (!hasHalo)
                        PlayerBaseData.GetInstance().AvatarEquipHalo(mAvatar, 0, null);
                }

                for (int i = 0; i < slots.Count; ++i)
                {
                    EFashionWearSlotType slot = (EFashionWearSlotType)slots[i];
                    GeAvatarChannel channel = _getChannelBySlot(slot);
                    PlayerBaseData.GetInstance().AvatarEquipPart(mAvatar, slot, 0, null);
                }

                if (null != mAvatar)
                    mAvatar.SuitAvatar();
                GamePool.ListPool<int>.Release(slots);
            }
        }

        GeAvatarChannel _getChannelBySlot(EFashionWearSlotType slot)
        {
            GeAvatarChannel channel = GeAvatarChannel.MaxChannelNum;
            switch (slot)
            {
                case EFashionWearSlotType.Head:
                    {
                        channel = GeAvatarChannel.Head;
                    }
                    break;
                case EFashionWearSlotType.UpperBody:
                    {
                        channel = GeAvatarChannel.UpperPart;
                    }
                    break;
                case EFashionWearSlotType.LowerBody:
                    {
                        channel = GeAvatarChannel.LowerPart;
                    }
                    break;
            }
            return channel;
        }

        bool _IsFashion(ItemData data)
        {
            if (null != data && data.SubType >= (int)ProtoTable.ItemTable.eSubType.FASHION_HAIR && data.SubType <= (int)ProtoTable.ItemTable.eSubType.FASHION_EPAULET)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region equipments
        public void LoadWeapon()
        {
            mDirty = true;
        }
        void _LoadWeapon()
        {
            if (null != mAvatar)
            {
                PlayerBaseData.GetInstance().AvatarEquipCurrentWeapon(mAvatar, null);
            }
        }
        #endregion

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemsAttrChanged, _OnItemAttrChanged);
        }

        void Start()
        {
            _Refresh();
            mDirty = false;
            mAvatarController = new GeAvatarRendererExIdleController();
            mAvatarController.Init(mAvatar);
        }

        void _Refresh()
        {
            _LoadAvatar(mOccu);
            _LoadWeapon();
            _SetFashions(mFashions);
        }

        void Update()
        {
            if(mDirty)
            {
                _Refresh();
                mDirty = false;
                return;
            }

            if (null != mAvatarController)
            {
                mAvatarController.OnUpdate(Time.deltaTime);
            }
        }

        void _OnItemAttrChanged(UIEvent uiEvent)
        {
            LoadWeapon();
        }

        void OnDestroy()
        {
            if (null != mAvatarController)
            {
                mAvatarController.UnInit();
            }
            mAvatarController = null;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemsAttrChanged, _OnItemAttrChanged);
        }
    }
}
