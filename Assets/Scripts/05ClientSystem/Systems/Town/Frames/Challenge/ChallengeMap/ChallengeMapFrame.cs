using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ChallengeMapFrame : ClientFrame
    {
        //超链接格式
        //<type=framename param=2|800000|800000 value=GameClient.ChallengeMapFrame>
        public static void OpenLinkFrame(string strParam)
        {
            ChallengeUtility.OnCloseChallengeMapFrame();

            try
            {

                //Logger.LogErrorFormat("OpenLinkFrame and param is {0}", strParam);

                var challengeParamData = strParam.Split(new char[] { '|' });

                if (challengeParamData.Length <= 0)
                    return;

                var challengeModelType = 1;
                var baseDungeonId = 0;
                var detailDungeonId = 0;

                var count = challengeParamData.Length;
                if (count == 1)
                {
                    //类型
                    challengeModelType = int.Parse(challengeParamData[0]);      
                    // 如果第一个参数不是ModelType 而是地下城id则通过地下城id反查ModelType
                    //第一个参数按照目前需求有两种含义(1：页签的含义 2:是地下城Id)
                    int param0 = int.Parse(challengeParamData[0]);
                    if (param0 > 1000)
                    {
                        DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(param0);
                        if (dungeonTable != null)
                        {
                            challengeModelType = (int)GetModelType(dungeonTable.SubType);
                            DungeonID tempDungeonID = new DungeonID(param0);
                            if (tempDungeonID != null)
                            {
                                baseDungeonId = tempDungeonID.dungeonIDWithOutDiff;
                                detailDungeonId = tempDungeonID.dungeonID;
                            }
                        }
                    }
                }
                else if (count == 2)
                {
                    //类型，基础地下城ID
                    challengeModelType = int.Parse(challengeParamData[0]);
                    baseDungeonId = int.Parse(challengeParamData[1]);
                }
                else if (count == 3)
                {
                    //类型，基础地下城ID， 详细地下城ID（需要打开的关卡界面）
                    challengeModelType = int.Parse(challengeParamData[0]);
                    baseDungeonId = int.Parse(challengeParamData[1]);
                    detailDungeonId = int.Parse(challengeParamData[2]);
                }

                
                    
                ChallengeUtility.OnOpenChallengeMapFrame((DungeonModelTable.eType) challengeModelType,
                    baseDungeonId,
                    detailDungeonId);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("ChallengeSelectFrame OpenLinkFrame throw exception {0}", e.ToString());
            }

        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Challenge/ChallengeMapFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mChallengeMapView != null)
            {
                ChallengeMapParamDataModel paramData = null;
                if (userData != null)
                    paramData = (ChallengeMapParamDataModel)userData;

                mChallengeMapView.InitView(paramData);
            }
        }

        static DungeonModelTable.eType GetModelType(DungeonTable.eSubType subType)
        {
            switch (subType)
            {
                case DungeonTable.eSubType.S_HELL:
                case DungeonTable.eSubType.S_HELL_ENTRY:
                case DungeonTable.eSubType.S_LIMIT_TIME_HELL:
                case DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL:
                    return DungeonModelTable.eType.DeepModel;
                case DungeonTable.eSubType.S_YUANGU:
                    return DungeonModelTable.eType.AncientModel;
                case DungeonTable.eSubType.S_WEEK_HELL:
                case DungeonTable.eSubType.S_WEEK_HELL_ENTRY:
                case DungeonTable.eSubType.S_WEEK_HELL_PER:
                    return DungeonModelTable.eType.WeekHellModel;
                case DungeonTable.eSubType.S_DEVILDDOM:
                    return DungeonModelTable.eType.VoidCrackModel;
                case DungeonTable.eSubType.S_RAID_DUNGEON:
                    return DungeonModelTable.eType.TeamDuplicationModel;                
                default:
                    return DungeonModelTable.eType.DeepModel;
            }       
        }
        #region ExtraUIBind
        private ChallengeMapView mChallengeMapView = null;

        protected override void _bindExUI()
        {
            mChallengeMapView = mBind.GetCom<ChallengeMapView>("ChallengeMapView");
        }

        protected override void _unbindExUI()
        {
            mChallengeMapView = null;
        }
        #endregion

    }

}
