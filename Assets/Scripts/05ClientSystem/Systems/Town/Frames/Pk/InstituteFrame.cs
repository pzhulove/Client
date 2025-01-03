using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Network;
///////删除linq
using System;

namespace GameClient
{
    public class InstituteFrame : ClientFrame
    {
        private GameObject prefabObject;
        private GameObject prefabContainer;

        private GameObject comboSkillContainer;
        private GameObject awardContainer;
        private GameObject skillComboItem;
        private GameObject tip;
        private Text skillName;
        private Text skillLevel;
        private InstituteTable curData;

        private Dictionary<int, GameObject> btnList = new Dictionary<int, GameObject>();
        private List<GameObject> comboSkillList = new List<GameObject>();

        private Button challengeBtn;
        private Button previewBtn;
        private Button lockState;
        private Button levelLimit;

        private Button primaryBtn;
        private Button advanceBtn;

        private GameObject select;
        private Text des;
        public static int type = 0;
        public static int id = 0;

        private Text mSkillChangeTipTxt;

        private GameObject mSkillChangeGo;

        private Button mTrainPVEBtn;

        private Button mFreedomTrainBtn;

        /// <summary>
        /// 是否从武研院里面进入修炼场
        /// </summary>
        public static bool IsEnterFromYanWUYuan = false;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/InstituteFrame";
        }

        protected override void _bindExUI()
        {
            base._bindExUI();
            prefabObject = mBind.GetGameObject("prefabObject");
            prefabContainer = mBind.GetGameObject("prefabContainer");
            comboSkillContainer = mBind.GetGameObject("comboContainer");
            awardContainer = mBind.GetGameObject("awardContainer");
            skillComboItem = mBind.GetGameObject("skillComboItem");
            primaryBtn = mBind.GetCom<Button>("primaryBtn");
            advanceBtn = mBind.GetCom<Button>("advanceBtn");
            select = mBind.GetGameObject("select");
            challengeBtn = mBind.GetCom<Button>("challengeBtn");
            previewBtn = mBind.GetCom<Button>("previewBtn");
            levelLimit = mBind.GetCom<Button>("levelLimit");
            lockState = mBind.GetCom<Button>("lockState");
            tip = mBind.GetGameObject("tip");
            skillName = mBind.GetCom<Text>("skillName");
            skillLevel = mBind.GetCom<Text>("skillLevel");
            des = mBind.GetCom<Text>("des");
            primaryBtn.onClick.AddListener(() =>
            {
                InitTabPrefab(1);
            });
            advanceBtn.onClick.AddListener(() =>
            {
                InitTabPrefab(2);
            });

            mSkillChangeTipTxt = mBind.GetCom<Text>("SkillChangedTip");
            mSkillChangeGo = mBind.GetGameObject("SkillChange");

            mTrainPVEBtn = mBind.GetCom<Button>("PVETrain");
            mFreedomTrainBtn = mBind.GetCom<Button>("btFreeTrain");
            mTrainPVEBtn.SafeAddOnClickListener(OnPVETrainBtnClcik);
            mFreedomTrainBtn.SafeAddOnClickListener(OnFreedomTrainBtnClick);
        }
        protected override void _unbindExUI()
        {
            base._unbindExUI();
            prefabObject = null;
            prefabContainer = null;
            comboSkillContainer = null;
            awardContainer = null;
            skillComboItem = null;
           
           
            select = null;
            challengeBtn = null;
            previewBtn = null;
            levelLimit = null;
            lockState = null;
            tip = null;
            skillName = null;
            skillLevel = null;
            des = null;
            primaryBtn.onClick.RemoveListener(() =>
            {
                InitTabPrefab(1);
            });
            primaryBtn = null;
            advanceBtn.onClick.RemoveListener(() =>
            {
                InitTabPrefab(2);
            });
            advanceBtn = null;
            mSkillChangeTipTxt = null;
            mSkillChangeGo = null;

            mTrainPVEBtn.SafeRemoveOnClickListener(OnPVETrainBtnClcik);
            mFreedomTrainBtn.SafeRemoveOnClickListener(OnFreedomTrainBtnClick);
            mTrainPVEBtn = null;
            mFreedomTrainBtn= null;
          
        }
      

        protected override void _OnOpenFrame()
        {
            InstituteTable data = SelectLast();
            if (data == null)
            {
                InitTabPrefab(1);
            }
            else
            {
                InitTabPrefab(data.DifficultyType);
                ShowInstituteInfo(data);
            }
            IsEnterFromYanWUYuan = false;
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }

        private InstituteTable SelectLast()
        {
            List<InstituteTable> list = TableManager.instance.GetJobAndTypeInstitue(PlayerBaseData.GetInstance().JobTableID, type);
            for (int i = 0; i < list.Count; i++)
            {
                int state = MissionManager.GetInstance().GetState(list[i]);
                if (state == 0)
                    return list[i];
            }

            if (type == 2)
            {
                list = TableManager.instance.GetJobAndTypeInstitue(PlayerBaseData.GetInstance().JobTableID, 1);
                for (int i = 0; i < list.Count; i++)
                {
                    int state = MissionManager.GetInstance().GetState(list[i]);
                    if (state == 0)
                        return list[i];
                }
            }
            else
            {
                list = TableManager.instance.GetJobAndTypeInstitue(PlayerBaseData.GetInstance().JobTableID, 2);
                for (int i = 0; i < list.Count; i++)
                {
                    int state = MissionManager.GetInstance().GetState(list[i]);
                    if (state == 0)
                        return list[i];
                }
            }

            return null;
        }

        private void SetBtnState(int type)
        {
            if (type == 1)
            {
                primaryBtn.image.color = Color.white;
                primaryBtn.GetComponentInChildren<Text>().color = Color.white;

                advanceBtn.image.color = new Color(176 / 255.0f, 176 / 255.0f, 176 / 255.0f);
                advanceBtn.GetComponentInChildren<Text>().color = new Color(159 / 255.0f, 162 / 255.0f, 184 / 255.0f);
            }
            else
            {
                advanceBtn.image.color = Color.white;
                advanceBtn.GetComponentInChildren<Text>().color = Color.white;

                primaryBtn.image.color = new Color(176 / 255.0f, 176 / 255.0f, 176 / 255.0f);
                primaryBtn.GetComponentInChildren<Text>().color = new Color(159 / 255.0f, 162 / 255.0f, 184 / 255.0f);
            }
        }

        private void InitTabPrefab(int type)
        {
            //SetBtnState(type);
            InstituteFrame.type = type;
            Utility.AttachTo(select, mSkillChangeGo);
            DestroyObjList();

            List<InstituteTable> list = TableManager.instance.GetJobAndTypeInstitue(PlayerBaseData.GetInstance().JobTableID, type);
            if(list!=null&&list.Count!=0)
            {
                mSkillChangeGo.CustomActive(true);
                mSkillChangeTipTxt.CustomActive(false);
                for (int i = 0; i < list.Count; i++)
                {
                    InstituteTable data = list[i];
                    GameObject obj = GameObject.Instantiate(prefabObject) as GameObject;
                    if(!btnList.ContainsKey(data.ID))
                    {
                        btnList.Add(data.ID, obj);
                    }
                    else
                    {
                        btnList[data.ID] = obj;
                    }
                    obj.CustomActive(true);
                    Utility.AttachTo(obj, prefabContainer);
                    GameObject complete = Utility.FindChild("complete", obj);
                    GameObject lockObj = Utility.FindChild("lock", obj);
                    Button btn = obj.GetComponent<Button>();

                Text name = Utility.FindChild("name", obj).GetComponent<Text>();
                name.text = data.Title;
                int state = MissionManager.GetInstance().GetState(data);
                complete.SetActive(state == 1);
                lockObj.SetActive(state == 2 || state == 3);

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    ShowInstituteInfo(data);

                    });
                }
                ShowInstituteInfo(list[0]);
            }
            else
            {
                mSkillChangeGo.CustomActive(false);
                mSkillChangeTipTxt.CustomActive(true);
                des.SafeSetText(TR.Value("yawuyan_noskills"));

            }
         
        }

        private void SetParent(GameObject child, GameObject parent)
        {
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector2.zero;
            child.transform.localScale = Vector3.one;
            child.transform.SetAsLastSibling();
        }

        private void DestroyObjList()
        {

            foreach (var item in btnList)
            {
                GameObject.Destroy(item.Value);
            }
            btnList.Clear();
        }

        private void ShowInstituteInfo(InstituteTable data)
        {
            DungeonTable dungeon = TableManager.instance.GetTableItem<DungeonTable>(data.DungeonID);
            if (dungeon != null)
            {
                des.text = dungeon.Description;
            }
            SetParent(select, btnList[data.ID]);
            curData = data;
            id = curData.ID;
            ResetBtnState(data);
            ShowAwardList(data);
            ShowComboList(data);
        }

        private void ResetBtnState(InstituteTable data)
        {
            int state = MissionManager.GetInstance().GetState(data);
            challengeBtn.gameObject.CustomActive(state == 0);
            previewBtn.gameObject.CustomActive(state == 1);
            lockState.CustomActive(state == 2);
            levelLimit.CustomActive(state == 3);
            if (state == 3)
            {
                levelLimit.GetComponentInChildren<Text>().text = string.Format("需求等级:{0}", data.LevelLimit);
            }
        }

        private void ShowAwardList(InstituteTable data)
        {
            List<ItemData> list = new List<ItemData>();
            MissionTable missionTableData = TableManager.GetInstance().GetTableItem<MissionTable>(data.MissionID);
            if (missionTableData != null)
            {
                string[] awards = missionTableData.Award.Split(new char[] { ',' });
                ComItemList.Items[] items = new ComItemList.Items[awards.Length];
                for (int i = 0; i < awards.Length; i++)
                {
                    var award = awards[i].Split(new char[] { '_' });
                    if (award.Length == 2)
                    {

                        items[i] = new ComItemList.Items();
                        items[i].id = int.Parse(award[0]);
                        items[i].count = uint.Parse(award[1]);
                        var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(items[i].id);
                        itemData.Count = (int)items[i].count;
                        list.Add(itemData);
                    }
                }

                awardContainer.GetComponent<ComItemList>().SetItems(items);

                ComItem[] comItems = awardContainer.GetComponentsInChildren<ComItem>();
                for (int i = 0; i < comItems.Length; i++)
                {
                    comItems[i].Setup(list[i], _OnItemClicked);
                    if (MissionManager.GetInstance().GetState(data) == 1)
                    {
                        comItems[i].ItemData.IsSelected = true;
                        comItems[i].SetShowSelectState(true);
                    }
                }
            }
        }
        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if (item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        private void ShowComboList(InstituteTable arg)
        {
            DestroyComboSkillList();
            ComboTeachData data = TableManager.instance.GetComboData(arg.DungeonID);
            if (data == null) return;
            ComboData[] tabelList = data.datas;
            for (int i = 0; i < tabelList.Length; i++)
            {
                ComboData table = tabelList[i];
                if (table.showUI == 0) continue;
                GameObject obj = GameObject.Instantiate(skillComboItem);
                obj.CustomActive(true);
                Utility.AttachTo(obj, comboSkillContainer);
                Image skillIcon = Utility.FindChild("skillIcon", obj).GetComponent<Image>();

                int id = table.skillID;
                if (table.sourceID != 0)
                {
                    id = table.sourceID;
                }

                ComLongPress longPress = obj.GetComponent<ComLongPress>();
                if (longPress != null)
                {
                    int[] args = new int[] { id, table.skillLevel };
                    longPress.SetArgs(args);
                    longPress.pointDownCallBack = PointDownCallBack;
                    longPress.pointUpCallBack = PointUpCallBack;
                }

                var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(id);
                ETCImageLoader.LoadSprite(ref skillIcon, skillData.Icon);
                GameObject guideArrow = Utility.FindChild("guideArrow", obj);
                guideArrow.CustomActive(i < tabelList.Length - 1);
                comboSkillList.Add(obj);
            }
        }

        private void PointDownCallBack(Transform trans, object args)
        {
            int[] array = args as int[];

            tip.CustomActive(true);
            tip.transform.position = trans.position + new Vector3(-8, 55, 0);
            var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(array[0]);
            skillName.text = skillData.Name;
            skillLevel.text = string.Format("Lv.{0}", array[1]);
        }

        private void PointUpCallBack(object args)
        {
            tip.CustomActive(false);
        }

        private static bool mIsSendMessage = false;
        public static IEnumerator _commonStart(int dungeonID, byte restart = 0)
        {
            if (!mIsSendMessage)
            {
                SceneDungeonStartReq req = new SceneDungeonStartReq
                {
                    dungeonId = (uint)(dungeonID),
                    isRestart = restart
                };



                var msg = new MessageEvents();
                var res = new SceneDungeonStartRes();

                mIsSendMessage = true;
                //NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);

                yield return (MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(ServerType.GATE_SERVER, msg, req, res, true, 5));

                mIsSendMessage = false;
            }
        }

        private void DestroyComboSkillList()
        {
            for (int i = 0; i < comboSkillList.Count; i++)
            {
                GameObject.Destroy(comboSkillList[i]);
            }
            comboSkillList.Clear();
        }

        [UIEventHandle("SkillChange/buttonPart/challengeBtn")]
        void ChallengeInstitute()
        {
            GameFrameWork.instance.StartCoroutine(_commonStart(curData.DungeonID));
        }

        [UIEventHandle("SkillChange/buttonPart/previewBtn")]
        void ChallengeAgain()
        {
            GameFrameWork.instance.StartCoroutine(_commonStart(curData.DungeonID));
        }
        /// <summary>
        /// 自由练习的点击
        /// </summary>
        private void OnFreedomTrainBtnClick()
        {
         
            BattleMain.OpenBattle(BattleType.Training, eDungeonMode.LocalFrame, 0, "1000");
            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
        }
        /// <summary>
        /// 修炼场的点击
        /// </summary>
        private void OnPVETrainBtnClcik()
        {
            IsEnterFromYanWUYuan = true;
            BattleMain.OpenBattle(BattleType.TrainingPVE, eDungeonMode.LocalFrame, 0, "1000");
            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
        }

    }
}