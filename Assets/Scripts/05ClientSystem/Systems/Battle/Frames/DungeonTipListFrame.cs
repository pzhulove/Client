using UnityEngine;
using System.Collections.Generic;


namespace GameClient
{
    // 
    // TODO 这里修改成一个Com组件
    //
    [LoggerModel("Chapter")]
    public class DungeonTipListFrame : ClientFrame
    {
        private readonly int kMaxShowBuffUnit = 6;

        private const string kPath = "UIFlatten/Prefabs/BattleUI/DungeonTipUnit";
        private const string kPath2 = "UIFlatten/Prefabs/BattleUI/DungeonTipInfoUnit";

        private class BuffUnit : System.IComparable<BuffUnit>
        {
            public int id { get; set; }
            public int sortOrder { get; set; }
            public ComTipsUnit comBuffTipsUnit {get;set;}
            public ComTipInfoUnit comBuffInfosUnit {get;set;}

            public int CompareTo(BuffUnit other)
            {
                if (sortOrder == other.sortOrder)
                {
                    return id - other.id;
                }
                else
                {
                    //return other.sortOrder - sortOrder;
                    return sortOrder - other.sortOrder;
                }
            }

            public void Clear()
            {
                if (null != comBuffInfosUnit)
                {
                    GameObject.Destroy(comBuffInfosUnit.gameObject);
                    comBuffInfosUnit = null;
                }

                if (null != comBuffTipsUnit)
                {
                    GameObject.Destroy(comBuffTipsUnit.gameObject);
                    comBuffTipsUnit = null;
                }
            }
        }

        private List<BuffUnit> mBuffs = new List<BuffUnit>();

        private GameObject mRoot;
        private GameObject mRoot2;

        [UIObject("InfoRoot")]
        private GameObject mInfoRoot;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/DungeonTipList";
        }

        protected override void _OnOpenFrame()
        {
            mInfoRoot.SetActive(false);

            for (int i = 0; i < mBuffs.Count; ++i)
            {
                mBuffs[i].Clear();
            }
            mBuffs.Clear();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleBuffAdded,   _addBuffEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleBuffRemoved, _removeBuffEvent);
        }
        
        protected override void _OnCloseFrame()
        {
            _clearRoot();

            for (int i = 0; i < mBuffs.Count; ++i)
            {
                mBuffs[i].Clear();
            }
            mBuffs.Clear();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleBuffAdded, _addBuffEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleBuffRemoved, _removeBuffEvent);
        }

        private void _addBuffEvent(UIEvent ui)
        {
            int buffId = (int)ui.Param1;

            _updateTipsRoot();

            var data = TableManager.instance.GetTableItem<ProtoTable.BuffTable>(buffId);
            if (null == data || data.IconSortOrder <= -1)
            {
                return ;
            }

            BuffUnit unit = _addBuffUnit(buffId);
            if (null == unit)
            {
                return ;
            }

            unit.sortOrder = data.IconSortOrder;


            Sprite sprite = null;
            var node = AssetLoader.instance.LoadRes(data.Icon, typeof(Sprite));
            if (node != null)
            {
                var go = AssetLoader.instance.LoadResAsGameObject(kPath);
                var com = go.GetComponent<ComTipsUnit>();
                go.name = string.Format(unit.sortOrder.ToString());
                Utility.AttachTo(go, mRoot);
                unit.comBuffTipsUnit = com;

                var go2 = AssetLoader.instance.LoadResAsGameObject(kPath2);
                var com2 = go2.GetComponent<ComTipInfoUnit>();
                go2.name = go.name;
                Utility.AttachTo(go2, mRoot2);
                unit.comBuffInfosUnit = com2;

                sprite = node.obj as Sprite;

                com.SetPercent(1.0f);
                com.SetSprite(sprite);

                com2.SetData(data);
                com2.SetSprite(sprite);
            }

            _sortBuffUnits();
            _updateBuffGraphicOrder();
        }

        private void _removeBuffEvent(UIEvent ui)
        {
            int buffId = (int)ui.Param1;
            BuffUnit unit = _removeBuffUnit(buffId);

            if (null == unit)
            {
                return;
            }

            unit.Clear();
        }

        private void _updateBuffGraphicOrder()
        {
            for (int i = 0; i < mBuffs.Count; ++i)
            {
                if (null != mBuffs[i].comBuffTipsUnit)
                {
                    mBuffs[i].comBuffTipsUnit.gameObject.transform.SetAsLastSibling();
                }
            }

            for (int i = 0; i < mBuffs.Count; ++i)
            {
                mBuffs[i].comBuffTipsUnit.gameObject.CustomActive(i < kMaxShowBuffUnit);
            }
        }

        private BeActor _getPlayerActor()
        {
            if (null != BattleMain.instance)
            {
                var playerManager = BattleMain.instance.GetPlayerManager();
                if (null != playerManager)
                {
                    var mainPlayer = playerManager.GetMainPlayer();
                    if (null != mainPlayer)
                    {
                        return mainPlayer.playerActor;
                    }
                }
            }
            return null;
        }

        #region BuffUnit
        private BuffUnit _findBuffUnit(int id)
        {
            for (int i = 0; i < mBuffs.Count; ++i)
            {
                if (id == mBuffs[i].id)
                {
                    return mBuffs[i];
                }
            }

            return null;
        }

        private void _sortBuffUnits()
        {
            mBuffs.Sort();
        }

        private bool _existBuffUnit(int id)
        {
            return null != _findBuffUnit(id);
        }

        private BuffUnit _addBuffUnit(int id)
        {
            if (_existBuffUnit(id))
            {
                return null;
            }

            BuffUnit unit = new BuffUnit();
            unit.id = id;
            mBuffs.Add(unit);
            return unit;
        }

        private BuffUnit _removeBuffUnit(int id)
        {
            BuffUnit unit = _findBuffUnit(id);

            if (null == unit)
            {
                return null; 
            }

            mBuffs.Remove(unit);

            return unit;
        }
        #endregion

        private void _updateTipsRoot()
        {
            if (null == frame)
            {
                return;
            }

            if (null == mRoot)
            {
                mRoot = Utility.FindGameObject(frame, "Root", true);
            }

            if (null == mRoot2)
            {
                mRoot2 = Utility.FindGameObject(frame, "InfoRoot", true);
            }
        }

        private void _clearRoot()
        {
            mRoot2 = null;
            mRoot = null;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            var player = _getPlayerActor();
            if (player == null)
            {
                return;
            }

            for (int i = 0; i < mBuffs.Count; ++i)
            {
                BuffUnit item = mBuffs[i];
                if (null == item || null == item.comBuffTipsUnit)
                {
                    continue;
                }

                BeBuff buff = player.buffController.HasBuffByID(item.id);
                if (buff != null)
                {
                    float rate = buff.GetLeftTime() * 1.0f / buff.duration;

                    item.comBuffTipsUnit.SetPercent(rate);
                }
            }
        }


        public override bool IsNeedUpdate()
        {
            return true;
        }



        [UIEventHandle("Root")]
        private void _onHiddenShow()
        {
            //mInfoRoot.SetActive(!mInfoRoot.activeSelf);
        }

        //[UIEventHandle("Info")]
        private void _onOpenInfo()
        {
            ClientSystemManager.instance.OpenFrame<DungeonTipInfoFrame>();
        }
    }
}
