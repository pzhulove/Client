using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using ProtoTable;

namespace GameClient
{
    class ChampionMatchNode
    {
        public Text name;
        public Image icon;
        public Image bg;
        public Transform wenhao;
        public ChampionMatchNode(Text name, Image icon, Image bg, Transform wenhao)
        {
            this.name = name;
            this.icon = icon;
            this.bg = bg;
            this.wenhao = wenhao;
        }
        public void SetName(string strName)
        {
            if (name != null)
                name.text = strName;
        }
        public void SetNameColor(Color color)
        {
            if (name != null)
                name.color = color;
        }
        public void SetIcon(string path)
        {
            if (icon != null)
            {
                if (path != null && path != "")
                    ETCImageLoader.LoadSprite(ref icon, path);
                else
                    Logger.LogError("Icon 地址为空！！！！！");
            }
        }
        public void SetActive(bool active)
        {
            if (name != null)
                name.gameObject.SetActive(active);
            if (wenhao != null)
                wenhao.gameObject.SetActive(!active);
            if (icon != null)
                icon.gameObject.SetActive(active);
        }
        public void SetLose()
        {
            if (bg != null)
            {
                ETCImageLoader.LoadSprite(ref bg, "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_Zhandou_Mingzi_Di_Huang");
                name.color = new Color32(154, 154, 154, 255);
            }
        }
    }

    // 用一个数组来表示对战的二叉树
    // 若根节点为(i)，则左子树为(i * 2 + 1)，右子树为(i * 2 + 2)
    // 玩家节点固定为 0,1,3,7,15
    // 则真实对手为 2,4,8,16
    //
    //                                  00
    //                  ┌---------------┴---------------┐
    //                  01                              02
    //          ┌-------┴-------┐               ┌-------┴-------┐
    //          03              04              05              06
    //      ┌---┴---┐       ┌---┴---┐       ┌---┴---┐       ┌---┴---┐
    //      07      08      09      10      11      12      13      14
    //    ┌-┴-┐   ┌-┴-┐   ┌-┴-┐   ┌-┴-┐   ┌-┴-┐   ┌-┴-┐   ┌-┴-┐   ┌-┴-┐
    //    15  16  17  18  19  20  21  22  23  24  25  26  27  28  29  30

    public class ChampionMatchFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Champion/ChampionMatch";
        }

        public static bool inited;
        private static string[] matchTree = new string[31];
        private static bool[] lineState = new bool[31];
        private static int curPhase = 0;
        private static string bossIcon;
        private ChampionMatchBattle battle;
        private BattlePlayer player;

        private void _initMatchData()//生成数据算法
        {
            List<string> nameList = _getNameList();

            List<string> matchList = _getMatchList();

            for (int i = 0; i < matchList.Count; ++i)
            {
                nameList.Remove(matchList[i]);
            }

            nameList.Sort((x, y) =>
            {
                return Random.Range(0, 3) - 1;
            });

            matchTree[0] = player.GetPlayerName();//树的根节点设置成玩家
            for (int i = 0; i < 15; ++i)
            {
                string node = matchTree[i];
                string newNode;
                if (node == player.GetPlayerName())//如果节点是玩家，则分配真实的对手
                {
                    newNode = matchList[0];
                    matchList.RemoveAt(0);

                    matchTree[i * 2 + 1] = node;
                    matchTree[i * 2 + 2] = newNode;

                    lineState[i * 2 + 1] = true;
                    lineState[i * 2 + 2] = false;
                }
                else//否则分配随机名字
                {
                    newNode = nameList[0];
                    nameList.RemoveAt(0);
                    if (Random.Range(0, 2) == 0)
                    {
                        matchTree[i * 2 + 1] = node;
                        matchTree[i * 2 + 2] = newNode;

                        lineState[i * 2 + 1] = true;
                        lineState[i * 2 + 2] = false;
                    }
                    else
                    {
                        matchTree[i * 2 + 1] = newNode;
                        matchTree[i * 2 + 2] = node;

                        lineState[i * 2 + 1] = false;
                        lineState[i * 2 + 2] = true;
                    }
                }
            }
        }

        private List<string> _getNameList()//获取擂台关卡表里面所有的名字
        {
            List<string> nameList = new List<string>();

            var table = TableManager.instance.GetTable<ChampionBattleTable>();
            foreach (ChampionBattleTable item in table.Values)
            {
                if (!nameList.Contains(item.Name))
                {
                    nameList.Add(item.Name);
                }
            }

            return nameList;
        }

        int matchRound = 4;
        private List<string> _getMatchList()//获取每个房间对应的对手信息
        {
            List<string> matchList = new List<string>();
            var areas = battle.dungeonManager.GetDungeonDataManager().battleInfo.areas;
            if (areas.Count != matchRound)
            {
                Logger.LogError("房间数量不为 4 ！！！！！");
            }
            else
            {
                for (int i = 0; i < matchRound; i++)
                {
                    var idWithoutDiff = areas[i].id / 10;
                    var data = TableManager.instance.GetTableItem<ChampionBattleTable>(idWithoutDiff);
                    if (data != null)
                    {
                        matchList.Add(data.Name);
                        if (i == matchRound - 1)
                        {
                            bossIcon = data.Icon;
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("擂台关卡表里不包含ID为 {0} 的行！！！！！", idWithoutDiff);
                    }
                }
            }
            matchList.Reverse();
            return matchList;
        }

        private Button btnPass;
        private Text txtTimer;
        private Transform leftPart;
        private Transform rightPart;
        private GameObject dui;
        private GameObject jue;
        private GameObject effect1;
        private GameObject effect2;
        private GameObject effect3;
        private GameObject effect4;
        private DOTweenAnimation leftAni;
        private DOTweenAnimation rightAni;
        private DOTweenAnimation topAni;
        protected override void _bindExUI()
        {
            btnPass = mBind.GetCom<Button>("pass");
            btnPass.onClick.AddListener(_onPassClicked);

            txtTimer = mBind.GetCom<Text>("timer");

            leftPart = mBind.GetCom<RectTransform>("leftPart");
            rightPart = mBind.GetCom<RectTransform>("rightPart");

            leftAni = mBind.GetCom<DOTweenAnimation>("leftAni");
            rightAni = mBind.GetCom<DOTweenAnimation>("rightAni");
            topAni = mBind.GetCom<DOTweenAnimation>("topAni");

            dui = mBind.GetGameObject("dui");
            jue = mBind.GetGameObject("jue");

            effect1 = mBind.GetGameObject("effect1");
            effect1.SetActive(false);
            effect2 = mBind.GetGameObject("effect2");
            effect2.SetActive(false);
            effect3 = mBind.GetGameObject("effect3");
            effect3.SetActive(false);
            effect4 = mBind.GetGameObject("effect4");
            effect4.SetActive(false);
        }

        protected override void _unbindExUI()
        {
            btnPass.onClick.RemoveListener(_onPassClicked);
            btnPass = null;

            txtTimer = null;

            leftPart = null;
            rightPart = null;

            leftAni = null;
            rightAni = null;
            topAni = null;

            dui = null;
            jue = null;

            effect1 = null;
            effect2 = null;
            effect3 = null;
            effect4 = null;
        }

        private void _onPassClicked()
        {
            Close();
        }

        protected override bool _isLoadFromPool()
        {
            return true;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        private void _initUIObjects()
        {
            if (frame == null)
                return;

            _initNodes();
            _initLines();
        }

        private List<ChampionMatchNode> nodeList = new List<ChampionMatchNode>();
        private void _initNodes()
        {
            if (leftPart == null || rightPart == null)
                return;

            nodeList.Clear();
            nodeList.Add(new ChampionMatchNode(null, null, null, null));
            for (int i = 1; i <= 30; i++)
            {
                if (i == 1)//玩家默认在左子树
                {
                    var icon = leftPart.Find("Nodes/Icon").GetComponent<Image>();
                    nodeList.Add(new ChampionMatchNode(null, icon, null, null));
                    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(player.playerInfo.occupation);
                    if (null != jobData)
                    {
                        ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                        if (null != resData)
                        {
                            nodeList[i].SetIcon(resData.IconPath);
                        }
                    }
                    icon.gameObject.SetActive(false);
                }
                else if (i == 2)//Boss在右子树
                {
                    var icon = rightPart.Find("Nodes/Icon").GetComponent<Image>();
                    nodeList.Add(new ChampionMatchNode(null, icon, null, null));
                    nodeList[i].SetIcon(bossIcon);
                    icon.gameObject.SetActive(false);
                }
                else
                {
                    var tran = leftPart.Find("Nodes/Node" + i);
                    if (tran == null)
                        tran = rightPart.Find("Nodes/Node" + i);

                    var name = tran.Find("Text").GetComponent<Text>();
                    var bg = tran.GetComponent<Image>();
                    var wenhao = tran.Find("Image");

                    var node = new ChampionMatchNode(name, null, bg, wenhao);
                    nodeList.Add(node);
                }
            }
        }

        private List<GameObject> lineList = new List<GameObject>();
        private void _initLines()
        {
            if (leftPart == null || rightPart == null)
                return;

            lineList.Clear();
            for (int i = 0; i <= 30; i++)
            {
                if (i < 3)
                {
                    lineList.Add(new GameObject());
                }
                else
                {
                    var tran = leftPart.Find("Lines/Line" + i);
                    if (tran == null)
                        tran = rightPart.Find("Lines/Line" + i);

                    lineList.Add(tran.gameObject);
                    tran.gameObject.SetActive(false);
                }
            }
        }

        private void _showNextPhase()
        {
            curPhase++;
            if (curPhase >= 1)
            {
                _setNodesActive(15, 30);

                effect1.SetActive(true);
            }
            if (curPhase >= 2)
            {
                _setNodesResult(15, 30);
                _setNodesActive(7, 14);

                effect1.SetActive(false);
                effect2.SetActive(true);
            }
            if (curPhase >= 3)
            {
                _setNodesResult(7, 14);
                _setNodesActive(3, 6);

                effect2.SetActive(false);
                effect3.SetActive(true);
            }
            if (curPhase >= 4)
            {
                _setNodesResult(3, 6);
                _setNodesActive(1, 2);

                effect3.SetActive(false);
                effect4.SetActive(true);
            }
        }

        private void _setNodesActive(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                if (nodeList[i].name != null)
                {
                    nodeList[i].SetName(matchTree[i]);
                }
                if (matchTree[i] == player.GetPlayerName())
                {
                    nodeList[i].SetNameColor(Color.white);
                }
                if (nodeList[i].wenhao != null)
                {
                    nodeList[i].wenhao.gameObject.SetActive(false);
                }
                if (nodeList[i].icon != null)
                {
                    nodeList[i].icon.gameObject.SetActive(true);
                    dui.gameObject.SetActive(false);
                    jue.gameObject.SetActive(false);
                }
            }
        }

        private void _setNodesResult(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                lineList[i].SetActive(lineState[i]);
                if (!lineState[i])
                {
                    _setLose(i);
                }
            }
        }

        private void _setLose(int index)
        {
            if (index >= nodeList.Count)
                return;

            nodeList[index].SetLose();
            lineList[index].SetActive(false);

            _setLose(index * 2 + 1);//子节点也要设置成失败状态
            _setLose(index * 2 + 2);//子节点也要设置成失败状态
        }

        protected override void _OnOpenFrame()
        {
            battle = BattleMain.instance.GetBattle() as ChampionMatchBattle;
            player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat();

            if (!inited)
            {
                _initMatchData();
                curPhase = 0;
                inited = true;
            }

            if (InputManager.instance != null && !InputManager.instance.isAttackButtonOnly)
            {
                InputManager.instance.SetButtonStateActive(0);
            }

            _initUIObjects();
            _showNextPhase();

            if (leftAni != null)
            {
                leftAni.DORestart();
            }
            if (rightAni != null)
            {
                rightAni.DORestart();
            }
            if (topAni != null)
            {
                topAni.DORestart();
            }
            if (btnPass != null)
            {
                btnPass.gameObject.SetActive(false);
                ClientSystemManager.instance.delayCaller.DelayCall(1500, () =>
                {
                    if (btnPass != null)
                    {
                        btnPass.gameObject.SetActive(true);
                    }
                    timer = 6f;
                });
            }
        }

        protected override void _OnCloseFrame()
        {
            if (InputManager.instance != null && InputManager.instance.isAttackButtonOnly)
            {
                InputManager.instance.ResetButtonState();
            }

            InputManager.instance.SetEnable(false);

            ClientSystemManager.instance.PlayUIEffect(FrameLayer.Top, "UIFlatten/Prefabs/Pk/StartFight");
            ClientSystemManager.instance.delayCaller.DelayCall(3000, () =>
            {
                InputManager.instance.SetEnable(true);

                if (battle != null)
                    battle.ResumeGameCmd();
            });
        }

        float timer;
        protected override void _OnUpdate(float timeElapsed)
        {
            if (timer > 0)
            {
                timer -= timeElapsed;
                if (timer < 0)
                {
                    Close();
                    return;
                }
                if (txtTimer != null)
                {
                    txtTimer.text = ((int)timer).ToString();
                }
            }
        }
    }
}
