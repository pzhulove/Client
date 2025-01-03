using DG.Tweening;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class PackageAttrDetailView : MonoBehaviour
    {
        [SerializeField] private Text mTextStrength;
        [SerializeField] private Text mTextIntelligence;
        [SerializeField] private Text mTextSta;
        [SerializeField] private Text mTextMind;
        
        [SerializeField] private Text mTextHp;
        [SerializeField] private Text mTextMp;
        [SerializeField] private Text mTextAttack;
        [SerializeField] private Text mTextMagicAttack;
        [SerializeField] private Text mTextIndependence;
        [SerializeField] private Text mTextDefence;
        [SerializeField] private Text mTextMagicDefence;
        [SerializeField] private Text mTextCritical;
        [SerializeField] private Text mTextMagicCritical;
        [SerializeField] private Text mTextAttackRate;
        [SerializeField] private Text mTextSpellRate;
        [SerializeField] private Text mTextMoveSpeed;
        [SerializeField] private Text mTextDex;//命中
        [SerializeField] private Text mTextDodge;//闪避
        [SerializeField] private Text mTextHpRecover;
        [SerializeField] private Text mTextMpRecover;
        [SerializeField] private Text mTextFrozen;
        [SerializeField] private Text mTextHard;
        [SerializeField] private Text mTextEquipScore;
        [SerializeField] private Text mTextResistMagic;
        
        [SerializeField] private Text mTextWeaponAttackAttributeType;
        [SerializeField] private Text mTextLightAttack;
        [SerializeField] private Text mTextLightDefense;
        [SerializeField] private Text mTextFireAttack;
        [SerializeField] private Text mTextFireDefense;
        [SerializeField] private Text mTextIceAttack;
        [SerializeField] private Text mTextIceDefense;
        [SerializeField] private Text mTextDarkAttack;
        [SerializeField] private Text mTextDarkDefense;
        [SerializeField] private Color mColorGood;
        [SerializeField] private Color mColorBad;
        [SerializeField] private Color mColorNormal;
        [SerializeField] private DOTweenAnimation mDetailAttrDotween;
        [SerializeField] private string mTipPrefabPath;
        private ClientFrame mFrame;

        public void Init(ClientFrame frame)
        {
            mFrame = frame;
            Refresh();
        }

        public void Refresh()
        {
            DisplayAttribute attribute = null;
            var system = GameClient.ClientSystemManager.instance.CurrentSystem as GameClient.ClientSystemBattle;
            if (system != null)
            {
                var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                if (mainPlayer != null && mainPlayer.playerActor != null)
                {
                    BeEntityData data = mainPlayer.playerActor.GetEntityData();
                    attribute = BeEntityData.GetActorAttributeForDisplay(data);
                }
            }
            else
            {
                attribute = BeUtility.GetMainPlayerActorAttribute();
            }

            _SetAttribute(attribute, attribute.baseAtk, mTextStrength, GameUtility.Item.GetAttributeEnumString(AttributeType.baseAtk));
            _SetAttribute(attribute, attribute.baseInt, mTextIntelligence, GameUtility.Item.GetAttributeEnumString(AttributeType.baseInt));
            _SetAttribute(attribute, attribute.baseSta, mTextSta, GameUtility.Item.GetAttributeEnumString(AttributeType.baseSta));
            _SetAttribute(attribute, attribute.baseSpr, mTextMind, GameUtility.Item.GetAttributeEnumString(AttributeType.baseSpr));

            _SetAttribute(attribute, attribute.maxHp, mTextHp, GameUtility.Item.GetAttributeEnumString(AttributeType.maxHp), false, false);
            _SetAttribute(attribute, attribute.maxMp, mTextMp, GameUtility.Item.GetAttributeEnumString(AttributeType.maxMp), false, false);
            _SetAttribute(attribute, attribute.attack, mTextAttack, GameUtility.Item.GetAttributeEnumString(AttributeType.attack));
            _SetAttribute(attribute, attribute.magicAttack, mTextMagicAttack, GameUtility.Item.GetAttributeEnumString(AttributeType.magicAttack));
			_SetAttribute(attribute, attribute.baseIndependence, mTextIndependence, GameUtility.Item.GetAttributeEnumString(AttributeType.baseIndependence));
            _SetAttribute(attribute, attribute.defence, mTextDefence, GameUtility.Item.GetAttributeEnumString(AttributeType.defence));
            _SetAttribute(attribute, attribute.magicDefence, mTextMagicDefence, GameUtility.Item.GetAttributeEnumString(AttributeType.magicDefence));
            _SetAttribute(attribute, attribute.ciriticalAttack, mTextCritical, GameUtility.Item.GetAttributeEnumString(AttributeType.ciriticalAttack), true);
            _SetAttribute(attribute, attribute.ciriticalMagicAttack, mTextMagicCritical, GameUtility.Item.GetAttributeEnumString(AttributeType.ciriticalMagicAttack), true);
            _SetAttribute(attribute, attribute.attackSpeed, mTextAttackRate, GameUtility.Item.GetAttributeEnumString(AttributeType.attackSpeed), true);
            _SetAttribute(attribute, attribute.spellSpeed, mTextSpellRate, GameUtility.Item.GetAttributeEnumString(AttributeType.spellSpeed), true);
            _SetAttribute(attribute, attribute.moveSpeed, mTextMoveSpeed, GameUtility.Item.GetAttributeEnumString(AttributeType.moveSpeed), true);
            _SetAttribute(attribute, attribute.dex, mTextDex, GameUtility.Item.GetAttributeEnumString(AttributeType.dex), true);
            _SetAttribute(attribute, attribute.dodge, mTextDodge, GameUtility.Item.GetAttributeEnumString(AttributeType.dodge), true);
            _SetAttribute(attribute, attribute.hpRecover, mTextHpRecover, GameUtility.Item.GetAttributeEnumString(AttributeType.hpRecover));
            _SetAttribute(attribute, attribute.mpRecover, mTextMpRecover, GameUtility.Item.GetAttributeEnumString(AttributeType.mpRecover));
            _SetAttribute(attribute, attribute.frozen, mTextFrozen, GameUtility.Item.GetAttributeEnumString(AttributeType.frozen));
            _SetAttribute(attribute, attribute.hard, mTextHard, GameUtility.Item.GetAttributeEnumString(AttributeType.hard));
            _SetAttribute(attribute, attribute.resistMagic, mTextResistMagic, GameUtility.Item.GetAttributeEnumString(AttributeType.resistMagic));
            RefreshEquipScore();

            _RefreshWeaponAttackAttributeTypeInfo();
            _SetAttribute(attribute, attribute.lightAttack, mTextLightAttack, "lightAttack");
            _SetAttribute(attribute, attribute.lightDefence, mTextLightDefense, "lightDefence");
            _SetAttribute(attribute, attribute.fireAttack, mTextFireAttack, "fireAttack");
            _SetAttribute(attribute, attribute.fireDefence, mTextFireDefense, "fireDefence");
            _SetAttribute(attribute, attribute.iceAttack, mTextIceAttack, "iceAttack");
            _SetAttribute(attribute, attribute.iceDefence, mTextIceDefense, "iceDefence");
            _SetAttribute(attribute, attribute.darkAttack, mTextDarkAttack, "darkAttack");
            _SetAttribute(attribute, attribute.darkDefence, mTextDarkDefense, "darkDefence");
        }

        public void OnClickClose()
        {
            mCloseCB?.Invoke();
        }

        private Action mCloseCB;
        public void SetCloseCB(Action cb)
        {
            mCloseCB = cb;
        }

        public void RefreshEquipScore()
        {
            mTextEquipScore.SafeSetText(PlayerBaseData.GetInstance().TotalEquipScore.ToString());
        }

        private RoleAttrTipsPanel mTipPanel;
        private bool mIsLoadingTip = false;
        private int mDefaultTipIndex = (int)EEquipProp.HPMax;
        public void OnTipClick()
        {
            if (mTipPanel == null && !mIsLoadingTip)
            {
                UIManager.instance.LoadObject(mFrame, mTipPrefabPath, null, new AssetLoadCallbacks<object>(OnLoadTipPanelSuccess, OnLoadTipPanelFailure), typeof(GameObject));
                mIsLoadingTip = true;
            }
            else
            {
                mTipPanel.Refresh(mDefaultTipIndex);
                mTipPanel.CustomActive(true);
                //mDetailAttrDotween.DORewind();
                mDetailAttrDotween.DORestart();
            }
        }

        private void OnLoadTipPanelSuccess(string assetPath, object obj, int grpID, float duration, object userData)
        {
            var go = obj as GameObject;
            if (go != null)
            {
                // todo: 修改显隐
                go.SetActive(true);
                mTipPanel = go.GetComponent<RoleAttrTipsPanel>();
                go.transform.SetParent(transform, false);
                mTipPanel?.Init(mDefaultTipIndex, _OnTipPanelClose);
            }
            mDetailAttrDotween.DOPlay();
            mIsLoadingTip = false;
        }

        private void _OnTipPanelClose()
        {
            mDetailAttrDotween.DOPlayBackwards();
        }

        private void OnLoadTipPanelFailure(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
        {
            mIsLoadingTip = false;
        }

        public void PlayForward()
        {
            //m_detailAttrDotween?.DOPlayForward();
            gameObject.CustomActive(true);
        }

        public void PlayBackwards()
        {
            gameObject.CustomActive(false);
        }

        /// <summary>
        /// 刷新武器攻击属性信息
        /// </summary>
        void _RefreshWeaponAttackAttributeTypeInfo()
        {
            List<ulong> mWearEquips = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            if (mWearEquips == null)
            {
                return;
            }

            ItemData mWearWeaponItemData = null;

            for (int i = 0; i < mWearEquips.Count; i++)
            {
                var mItem = ItemDataManager.GetInstance().GetItem(mWearEquips[i]);
                if (mItem == null)
                {
                    continue;
                }

                if (mItem.Type != ItemTable.eType.EQUIP)
                {
                    continue;
                }

                if (mItem.SubType != (int)ItemTable.eSubType.WEAPON)
                {
                    continue;
                }

                mWearWeaponItemData = mItem;
            }

            if (mWearWeaponItemData == null)
            {
                if (mTextWeaponAttackAttributeType != null)
                {
                    mTextWeaponAttackAttributeType.text = "无";
                }
                return;
            }
            string mStrWeaponAttackAttr = mWearWeaponItemData.GetWeaponAttackAttributeDescs();

            if (mTextWeaponAttackAttributeType != null)
            {
                mTextWeaponAttackAttributeType.text = mStrWeaponAttackAttr;
            }
        }


        string _GetPercentValue(float value)
        {
            return string.Format("{0:F1}%", value);
        }

        void _SetSpecialAttribute(DisplayAttribute attribute, float fixValue, float percentValue, Text textFix, Text textPercent, string fixChildName, string percentChildName)
        {
            textFix.SafeSetText(fixValue.ToString());
            if (percentValue != 0)
            {
                textPercent.SafeSetText(string.Format(" + {0:F1}%", percentValue / 10));
            }
            else
            {
                textPercent.SafeSetText("");
            }

            textFix.color = _GetAttributeColor(attribute, fixChildName);
            textPercent.color = _GetAttributeColor(attribute, percentChildName);
        }

        Color _GetAttributeColor(DisplayAttribute attribute, string childName)
        {
            if (attribute.attachValue.ContainsKey(childName))
            {
                if (attribute.attachValue[childName] > 0)
                {
                    return mColorGood;
                }
                else if (attribute.attachValue[childName] < 0)
                {
                    return mColorBad;
                }
            }

            return mColorNormal;
        }

        void _SetAttribute(DisplayAttribute attribute, float value, Text text, string childName, bool isPercent = false, bool isChangeColor = true)
        {
            if (text == null)
                return;

            if (isChangeColor)
                text.color = _GetAttributeColor(attribute, childName);

            if (isPercent)
                text.SafeSetText(_GetPercentValue(value));
            else
                text.SafeSetText(string.Format("{0}", value));
        }

        public void OnClick(int id)
        {
            mDefaultTipIndex = id;
            OnTipClick();
        }
    }
}