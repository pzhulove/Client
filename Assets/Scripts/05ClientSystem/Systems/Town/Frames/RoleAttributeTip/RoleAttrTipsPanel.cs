using ProtoTable;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GameClient
{
    public class RoleAttrTipsPanel : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mListScript;
        [SerializeField] private string mPerfabPath;
        [SerializeField] private Text mContent;
        [SerializeField] private GameObject mArrtTipItem;
        [SerializeField] private Image mImageSelect;
        private List<AttrDescTable> mDateList = null;
        List<Vector2> mSize = new List<Vector2>();
        private BeEntityData mEntityData;
        private DisplayAttribute mDisplayAttribute;
        private ComUIListElementScript mSelectItem;
        private int mSelectId;
        private Action mOnCloseCB;

        public void Init(int id, Action onCloseCB)
        {
            mOnCloseCB = onCloseCB;
            gameObject.CustomActive(true);
            mDateList = new List<AttrDescTable>();
            var tableDic = TableManager.GetInstance().GetTable<AttrDescTable>();
            foreach (var table in tableDic.Values)
            {
                mDateList.Add((AttrDescTable)table);
            }

            if (null != mListScript)
            {
                mListScript.InitialLizeWithExternalElement(mPerfabPath);
                mListScript.onItemVisiable = _ShowItem;
                mListScript.OnItemUpdate = _ShowItem;
                mListScript.onItemSelected = _OnItemSelected;
            }
            Refresh(id);
        }

        public void Refresh(int id)
        {
            mEntityData = BeUtility.GetMainPlayerActor().GetEntityData();
            mDisplayAttribute = BeUtility.GetMainPlayerActorAttribute(mEntityData);
            _GetSize();
            int i = 0;
            int selectId = 0;
            mSelectItem = null;
            mImageSelect.enabled = false;

            var tableDic = TableManager.GetInstance().GetTable<AttrDescTable>();
            foreach (var table in tableDic.Values)
            {
                if ((table as AttrDescTable).ID == id)
                {
                    selectId = i;
                    break;
                }
                i++;
            }
            mSelectId = selectId;
            mListScript.SetElementAmount(mDateList.Count, mSize);
            mListScript.MoveElementInScrollArea(selectId, true);
            mListScript.SelectElement(selectId);
        }

        private void _GetSize()
        {
            mSize.Clear();
            foreach(var table in mDateList)
            {
                if (null == table)
                    return;
                float heightText = mContent.GetComponent<RectTransform>().sizeDelta.y;
                
                float height = StaticUtility.GetTextPreferredHeight(mContent, _GetAttrDesc(table, mDisplayAttribute, mEntityData));
                float widthItem = mArrtTipItem.GetComponent<RectTransform>().sizeDelta.x;
                float heightItem = mArrtTipItem.GetComponent<RectTransform>().sizeDelta.y;

                mSize.Add(new Vector2(widthItem, heightItem + height - heightText));
            }
        }

        private void _OnItemSelected(ComUIListElementScript item)
        {
            mSelectItem = item;
            mImageSelect.enabled = true;
            mImageSelect.transform.SetParent(item.transform, false);
        }


        private void _ShowItem(ComUIListElementScript item)
        {
            if (null == item)
                return;
            var script = item.GetComponent<RoleAttrTipItem>();
            if (null == script)
                return;
            if (item.m_index >= mDateList.Count)
                return;

            var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            var attrIndexList = jobTableItem.RecommendedAttributeIndex;
            string title = mDateList[item.m_index].Name;
            bool isNeedRich = false;
            for (int i = 0; i < attrIndexList.Count; i++)
            {
                if (mDateList[item.m_index].ID == attrIndexList[i])
                {
                    title = (string.Format(TR.Value("package_special_attributes_desc"), mDateList[item.m_index].Name));
                    isNeedRich = true;
                    break;
                }
            }

            if (mSelectItem == item && item.m_index != mSelectId)
            {
                mImageSelect.enabled = false;
            }

            if (item.m_index == mSelectId)
            {
                _OnItemSelected(item);
            }

            script.Init(title, _GetAttrDesc(mDateList[item.m_index], mDisplayAttribute, mEntityData), isNeedRich);
        }

        public void OnHideTips()
        {
            mImageSelect.enabled = false;
            mSelectItem = null;
            this.CustomActive(false);
            mOnCloseCB?.Invoke();
        }

        private string _GetAttrDesc(AttrDescTable tableData, DisplayAttribute attribute, BeEntityData entityData)
        {
            var bData = entityData.battleData;
            float param = 0f;
            switch ((EEquipProp)tableData.ID)
            {
                case EEquipProp.PhysicsAttack:
                    return string.Format(tableData.Desc, bData.displayAttack, bData.ignoreDefAttackAdd);
                case EEquipProp.MagicAttack:
                    return string.Format(tableData.Desc, bData.displayMagicAttack, bData.ignoreDefMagicAttackAdd);
                case EEquipProp.PhysicsDefense:
                    return string.Format(tableData.Desc, GameUtility.Item.GetReduceRate(entityData.level, bData.fDefence));
                case EEquipProp.MagicDefense:
                    return string.Format(tableData.Desc, GameUtility.Item.GetReduceRate(entityData.level, bData.fMagicDefence));
                case EEquipProp.PhysicCritRate:
                    param = attribute.ciriticalAttack;
                    break;
                case EEquipProp.MagicCritRate:
                    param = attribute.ciriticalMagicAttack;
                    break;
                case EEquipProp.AttackSpeedRate:
                    param = attribute.attackSpeed;
                    break;
                case EEquipProp.FireSpeedRate:
                    param = attribute.spellSpeed;
                    break;
                case EEquipProp.MoveSpeedRate:
                    param = attribute.moveSpeed;
                    break;
                case EEquipProp.HitRate:
                    param = attribute.dex;
                    break;
                case EEquipProp.AvoidRate:
                    param = attribute.dodge;
                    break;
                case EEquipProp.HPRecover:
                    param = (attribute.hpRecover);
                    break;
                case EEquipProp.MPRecover:
                    param = (attribute.mpRecover);
                    break;
                default:
                    return tableData.Desc;
            }

            if (param < 0f)
            {
                return string.Format(tableData.DescNagative, Mathf.Abs(param));
            }
            else
            {
                return string.Format(tableData.Desc, param);
            }
        }
    }

}
