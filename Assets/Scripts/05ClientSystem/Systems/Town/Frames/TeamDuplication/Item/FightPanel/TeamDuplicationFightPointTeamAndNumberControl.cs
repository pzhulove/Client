using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{

    //据点中的小队队伍和挑战次数的控制器
    public class TeamDuplicationFightPointTeamAndNumberControl : MonoBehaviour
    {

        private List<GameObject> _currentTeamItemList = new List<GameObject>();

        [Space(25)] [HeaderAttribute("teamItem")] [Space(15)]
        //保存1，2，3，4
        [SerializeField] private List<GameObject> teamItemList = new List<GameObject>();
        [Space(10)]
        [HeaderAttribute("teamOddRoot")]
        [Space(5)]
        [SerializeField] private GameObject fightPointTeamOddRoot;
        [SerializeField] private List<RectTransform> fightPointTeamOddOnePositionList = new List<RectTransform>();
        [SerializeField] private List<RectTransform> fightPointTeamOddThreePositionList = new List<RectTransform>();
        [Space(10)]
        [HeaderAttribute("teamEvenRoot")]
        [Space(5)]
        [SerializeField] private GameObject fightPointTeamEvenRoot;
        [SerializeField] private List<RectTransform> fightPointTeamEvenTwoPositionList = new List<RectTransform>();
        [SerializeField] private List<RectTransform> fightPointTeamEvenFourPositionList = new List<RectTransform>();

        //完成的次数
        [Space(25)] [HeaderAttribute("fightPointNumber")] [Space(15)] [SerializeField]
        private List<TeamDuplicationFightPointNumberItem> fightPointNumberItemList =
            new List<TeamDuplicationFightPointNumberItem>();


        public void ClearControl()
        {
        }

        //据点中队伍的数量和位置
        public void UpdateFightPointTeamItemList(List<uint> teamIndexList)
        {
            ResetFightPointTeamItemList();

            //队伍列表无效
            if (teamIndexList == null || teamIndexList.Count <= 0)
            {
                return;
            }

            var teamIndexNumber = teamIndexList.Count;

            //奇数
            if (teamIndexNumber % 2 == 1)
            {
                CommonUtility.UpdateGameObjectVisible(fightPointTeamOddRoot, true);
            }
            else
            {
                //偶数
                CommonUtility.UpdateGameObjectVisible(fightPointTeamEvenRoot, true);
            }

            //获得当前据点的队伍
            for (var i = 0; i < teamIndexList.Count; i++)
            {
                //队伍的索引，1，2，3，4
                var curTeamIndex = teamIndexList[i];
                if (curTeamIndex >= 1 && curTeamIndex <= teamItemList.Count)
                {
                    //获得队伍的icon
                    var curTeamItem = teamItemList[(int)curTeamIndex - 1];
                    if (curTeamItem != null)
                    {
                        CommonUtility.UpdateGameObjectVisible(curTeamItem.gameObject, true);
                        _currentTeamItemList.Add(curTeamItem);
                    }
                }
            }

            //设置位置
            if (teamIndexNumber == 1)
            {
                UpdateFightPointTeamPosition(fightPointTeamOddOnePositionList);
            }
            else if (teamIndexNumber == 2)
            {
                UpdateFightPointTeamPosition(fightPointTeamEvenTwoPositionList);
            }
            else if (teamIndexNumber == 3)
            {
                UpdateFightPointTeamPosition(fightPointTeamOddThreePositionList);
            }
            else if (teamIndexNumber == 4)
            {
                UpdateFightPointTeamPosition(fightPointTeamEvenFourPositionList);
            }
        }

        private void ResetFightPointTeamItemList()
        {
            _currentTeamItemList.Clear();
            for (var i = 0; i < teamItemList.Count; i++)
            {
                var teamItem = teamItemList[i];
                if (teamItem != null)
                    CommonUtility.UpdateGameObjectVisible(teamItem.gameObject, false);
            }
            CommonUtility.UpdateGameObjectVisible(fightPointTeamOddRoot, false);
            CommonUtility.UpdateGameObjectVisible(fightPointTeamEvenRoot, false);
        }

        private void UpdateFightPointTeamPosition(List<RectTransform> teamPositionList)
        {
            for (var i = 0; i < teamPositionList.Count && i < _currentTeamItemList.Count; i++)
            {
                var teamPositionRtf = teamPositionList[i];
                var teamItem = _currentTeamItemList[i];
                if (teamPositionRtf == null || teamItem == null)
                    continue;

                var teamItemRtf = teamItem.gameObject.transform as RectTransform;
                if (teamItemRtf == null)
                    continue;

                teamItemRtf.anchoredPosition = teamPositionRtf.localPosition;
            }
        }


        //战斗次数
        public void UpdateFightPointFightNumberItemList(int finishedNumber, int totalNumber)
        {
            ResetFightPointFightNumberItemList();

            //总的挑战次数不大于1的时候，不显示小格子
            if (totalNumber <= 1)
            {
                return;
            }

            for (var i = 0; i < fightPointNumberItemList.Count && i < totalNumber; i++)
            {
                //展示当前总数量
                var fightPointNumberItem = fightPointNumberItemList[i];
                CommonUtility.UpdateGameObjectVisible(fightPointNumberItem.gameObject, true);

                //完成的次数进行更新
                if (i < finishedNumber)
                    fightPointNumberItem.Init(true);
            }
        }

        private void ResetFightPointFightNumberItemList()
        {
            for (var i = 0; i < fightPointNumberItemList.Count; i++)
            {
                var fightNumberItem = fightPointNumberItemList[i];
                if (fightNumberItem != null)
                {
                    fightNumberItem.Init(false);
                    CommonUtility.UpdateGameObjectVisible(fightNumberItem.gameObject, false);
                }
            }
        }

    }
}
