using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // 精英地下城预览界面中的评价item
    public class EliteDungeonPreviewEvaluationItem : MonoBehaviour
    {
        [SerializeField]
        Text dungeonName = null;

        [SerializeField]
        Text lockTip = null;  

        [SerializeField]
        Image gouflag = null; // 对勾

        [SerializeField]
        ComCommonBind bind = null;

        [SerializeField]
        GameObject evaluationRoot = null; // 评价 SSS AA 等

        [SerializeField]
        Image bg = null;

        [SerializeField]
        Button enterDungeon = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUp(object data,int index)
        {
            if(!(data is int))
            {
                return;
            }

            int dungeonID = (int)data;
            DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonID);
            if(dungeonTable == null)
            {
                return;
            }

            dungeonName.SafeSetText(dungeonTable.Name);
       
            if(EliteDungeonPreviewFrame.GetDungeonState(dungeonID) == ComChapterDungeonUnit.eState.Locked)
            {
                lockTip.CustomActive(true);
                evaluationRoot.CustomActive(false);
                gouflag.CustomActive(false);

                lockTip.SafeSetText(TR.Value("elite_dungeon_preview_unopen"));
            }
            else if(EliteDungeonPreviewFrame.GetDungeonState(dungeonID) == ComChapterDungeonUnit.eState.Unlock)
            {
                lockTip.CustomActive(true);
                evaluationRoot.CustomActive(false);
                gouflag.CustomActive(false);

                lockTip.SafeSetText(TR.Value("elite_dungeon_preview_unpass"));
            }
            else
            {
                lockTip.CustomActive(false);

                evaluationRoot.CustomActive(true);
                SetScore(EliteDungeonPreviewFrame.GetBestScore(dungeonID));
                gouflag.CustomActive(EliteDungeonPreviewFrame.HasSSS(dungeonID));
            }

            bg.CustomActive(index % 2 != 0);

            enterDungeon.SafeSetOnClickListener(() => 
            {
                List<int> ids = EliteDungeonPreviewFrame.GetCurrentChapterNormalDungeonIDs();
                if(ids == null)
                {
                    return;
                }

                int idx = ids.FindIndex((id) => 
                {
                    return id == dungeonID;
                });
                if(idx >= 0)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SelectEnterDungeon, idx);
                    ClientSystemManager.GetInstance().CloseFrame<EliteDungeonPreviewFrame>();
                }                   
            });

            if(EliteDungeonPreviewFrame.GetDungeonState(dungeonID) == ComChapterDungeonUnit.eState.Locked)
            {
                enterDungeon.CustomActive(false);
            }
            else
            {
                enterDungeon.CustomActive(
                    EliteDungeonPreviewFrame.GetDungeonState(dungeonID) == ComChapterDungeonUnit.eState.Unlock || 
                    EliteDungeonPreviewFrame.GetBestScore(dungeonID) != DungeonScore.SSS);
            }
        }

        void SetScore(Protocol.DungeonScore score)
        {
            if (bind == null)
            {
                return;
            }

            GameObject scoreImage0 = bind.GetGameObject("scoreImage0");
            GameObject scoreImage1 = bind.GetGameObject("scoreImage1");
            GameObject scoreImage2 = bind.GetGameObject("scoreImage2");

            scoreImage0.CustomActive(false);
            scoreImage1.CustomActive(false);
            scoreImage2.CustomActive(false);
 
            Image img0 = scoreImage0.GetComponent<Image>();
            Image img1 = scoreImage1.GetComponent<Image>();
            Image img2 = scoreImage2.GetComponent<Image>();
            bind.GetSprite("s", ref img0);
            bind.GetSprite("s", ref img1);
            bind.GetSprite("s", ref img2);

            switch (score)
            {
                case Protocol.DungeonScore.SSS:
                    scoreImage0.CustomActive(true);
                    scoreImage1.CustomActive(true);
                    scoreImage2.CustomActive(true);
                    break;
                case Protocol.DungeonScore.SS:
                    scoreImage0.CustomActive(true);
                    scoreImage1.CustomActive(true);
                    break;
                case Protocol.DungeonScore.S:
                    scoreImage0.CustomActive(true);
                    break;
                case Protocol.DungeonScore.A:
                case Protocol.DungeonScore.B:
                case Protocol.DungeonScore.C:
                    scoreImage0.CustomActive(true);
                    // scoreImage0.GetComponent<Image>().sprite = bind.GetSprite("a");
                    Image image = scoreImage0.GetComponent<Image>();
                    bind.GetSprite("a", ref image);
                    break;
            }
        }
    }
}


