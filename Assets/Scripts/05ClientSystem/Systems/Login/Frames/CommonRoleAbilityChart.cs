using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{
    public enum EAbilityChartType
    {
        /// <summary>
        /// 操作难度
        /// </summary>
        OperationDifficulty = 0,
        /// <summary>
        /// 攻击力
        /// </summary>
        Attack,
        /// <summary>
        /// 攻击范围
        /// </summary>
        AttackRange,
        /// <summary>
        /// 攻击距离
        /// </summary>
        AttackDistance,
        /// <summary>
        /// 控制
        /// </summary>
        Control,
        /// <summary>
        /// 辅助
        /// </summary>
        Auxiliary,
        /// <summary>
        /// 速度
        /// </summary>
        Speed,
    }

    public class AbilityChartData
    {
        public EAbilityChartType Type;
        public int Value;
    }

    public class CommonRoleAbilityChart : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mAbilityChartScrollView;
        /// <summary>
        /// 能力图集合
        /// </summary>
        private List<AbilityChartData> AbilityChartList = new List<AbilityChartData>();

        private void Awake()
        {
            _InitAbilityChartScrollListBind();
        }

        private void OnDestroy()
        {
            _UnInitAbilityChartScrollListBind();

            if (AbilityChartList != null)
                AbilityChartList.Clear();
        }

        public void _OnRefreshAbilityChartList(int jobTableId)
        {
            var tableData = TableManager.GetInstance().GetTableItem<JobTable>(jobTableId);
            if(tableData != null)
            {
                if (AbilityChartList != null)
                {
                    AbilityChartList.Clear();
                }

                for (int i = 0; i < tableData.AbilityChart.Count; i++)
                {
                    int value = tableData.AbilityChart[i];
                    if (value == 0)
                        continue;

                    AbilityChartData data = new AbilityChartData();
                    data.Type = (EAbilityChartType)i;
                    data.Value = value;
                    AbilityChartList.Add(data);
                }

                if (mAbilityChartScrollView != null)
                {
                    mAbilityChartScrollView.SetElementAmount(AbilityChartList.Count);
                }
            }
        }

        void _InitAbilityChartScrollListBind()
        {
            if (mAbilityChartScrollView != null)
            {
                mAbilityChartScrollView.Initialize();
                mAbilityChartScrollView.onItemVisiable += _OnItemVisiableDelegate;
            }
        }

        void _UnInitAbilityChartScrollListBind()
        {
            if (mAbilityChartScrollView != null)
            {
                mAbilityChartScrollView.onItemVisiable -= _OnItemVisiableDelegate;
            }
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= AbilityChartList.Count)
            {
                return;
            }

            AbilityChartData data = AbilityChartList[item.m_index];
            Text name = combind.GetCom<Text>("Name");
            Slider slider = combind.GetCom<Slider>("Progress");

            if (data.Type == EAbilityChartType.OperationDifficulty)
            {
                if (slider != null)
                {
                    slider.CustomActive(false);
                }

                if (name != null)
                {
                    name.text = string.Format("{0} ( {1} )", _GetAbilityChartDesc(data.Type), _GetOperationDifficultyDesc(data.Value));
                }
            }
            else
            {
                if (slider != null)
                {
                    slider.CustomActive(true);
                }

                if (name != null)
                {
                    name.text = _GetAbilityChartDesc(data.Type);
                }

                if (slider != null)
                {
                    slider.value = data.Value / 3.0f;
                }
            }
        }

        string _GetAbilityChartDesc(EAbilityChartType type)
        {
            string desc = string.Empty;
            switch (type)
            {
                case EAbilityChartType.OperationDifficulty:
                    desc = TR.Value("creat_role_abilitychart_operationdifficulty");
                    break;
                case EAbilityChartType.Attack:
                    desc = TR.Value("creat_role_abilitychart_attack");
                    break;
                case EAbilityChartType.AttackRange:
                    desc = TR.Value("creat_role_abilitychart_attackrange");
                    break;
                case EAbilityChartType.AttackDistance:
                    desc = TR.Value("creat_role_abilitychart_attackdiscount");
                    break;
                case EAbilityChartType.Control:
                    desc = TR.Value("creat_role_abilitychart_control");
                    break;
                case EAbilityChartType.Auxiliary:
                    desc = TR.Value("creat_role_abilitychart_auxiliary");
                    break;
                case EAbilityChartType.Speed:
                    desc = TR.Value("creat_role_abilitychart_speed");
                    break;
            }

            return desc;
        }

        string _GetOperationDifficultyDesc(int value)
        {
            string desc = string.Empty;
            if (value == 1)
            {
                desc = "低";
            }
            else if (value == 2)
            {
                desc = "中";
            }
            else if (value == 3)
            {
                desc = "高";
            }

            return desc;
        }
    }
}