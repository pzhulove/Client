using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace GameClient
{
    public class ComDungeonBuffTipList : MonoBehaviour
    {
        private const string kPath = "UIFlatten/Prefabs/BattleUI/DungeonTipUnit";

        public GameObject mRoot;

        private List<BuffUnit> mBuffs = new List<BuffUnit>();

        private class BuffUnit : System.IComparable<BuffUnit>
        {
            public BuffUnit()
            {
                this.uid = 0;
            }

            public ulong uid { get; set; }
            public int sortOrder { get; set; }
            public int buffId { get; set; }
            public int buffType { get; set; }
            public float buffDuration { get; set; }
            public float buffLeftTime { get; set; }
            public ComTipsUnit buffCom { get; set; }

            public int CompareTo(BuffUnit other)
            {
                return other.sortOrder - this.sortOrder;
            }
        }

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffListChanged, _onUpdateBuffList);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffAdded, _addBuffEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleBuffRemoved, _removeBuffEvent);
			_onUpdateBuffList(null);
        }

        void OnDestroy()
        {
            _clearBuff();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffListChanged, _onUpdateBuffList);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffAdded, _addBuffEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleBuffRemoved, _removeBuffEvent);
        }

        private void _removeBuffEvent(UIEvent ui)
        {
            int buffID = (int)ui.Param1;
            BuffUnit unit = mBuffs.Find(x => { return x.buffId == buffID; });
            if (null != unit)
            {
                _destoryBuffUnit(unit);
                mBuffs.Remove(unit);
            }
        }

        private void _destoryBuffUnit(BuffUnit unit)
        {
            if (null != unit.buffCom)
            {
                GameObject.Destroy(unit.buffCom.gameObject);
                unit.buffCom = null;
            }
        }

        #region buffupdate
        private void _onUpdateBuffList(UIEvent uiEvent)
        {
            _clearBuff();

            for (int i = 0; i < PlayerBaseData.GetInstance().buffList.Count; ++i)
            {
                var unit = PlayerBaseData.GetInstance().buffList[i];
                _addBuff(unit);
            }
        }

        private void _clearBuff()
        {
            mBuffs.RemoveAll(x => 
            {
                if (null != x.buffCom)
                {
                    _destoryBuffUnit(x);
                }
                return true;
            });
        }

        private void _addBuff(Battle.DungeonBuff buff)
        {
            BuffUnit buffUnit     = new BuffUnit();
            buffUnit.buffId       = buff.id;
            buffUnit.buffDuration = buff.duration;
            buffUnit.buffLeftTime = buff.lefttime;
            buffUnit.buffType     = (int)buff.type;

            Logger.LogProcessFormat("[buffdrug] 城镇添加Buff {0}", buffUnit.buffId);

            ProtoTable.BuffTable data = TableManager.instance.GetTableItem<ProtoTable.BuffTable>(buffUnit.buffId);

            if (null == data || data.IconSortOrder <= -1)
            {
                return;
            }

            Sprite sprite = null;

            var node = AssetLoader.instance.LoadRes(data.Icon, typeof(Sprite));

            if (null != node && null != node.obj)
            {
                GameObject go = AssetLoader.instance.LoadResAsGameObject(kPath);
                ComTipsUnit com = go.GetComponent<ComTipsUnit>();
                go.name = string.Format(buff.id.ToString());

                Utility.AttachTo(go, mRoot);
                sprite = node.obj as Sprite;

                com.SetPercent(1.0f);
                com.SetSprite(sprite);

                buffUnit.buffCom = com;
                buffUnit.sortOrder = data.IconSortOrder;

                mBuffs.Add(buffUnit);
            }
        }
        #endregion

        void Update()
        {
            float timeElapsed = Time.deltaTime;

            for (int i = 0; i < mBuffs.Count; ++i)
            {
                BuffUnit item = mBuffs[i];
                if (item.buffType != 1)
                {
                    item.buffLeftTime -= timeElapsed;

                    if (item.buffLeftTime >= 0)
                    {
                        float rate = item.buffLeftTime / item.buffDuration;
                        if (null != item.buffCom)
                        {
                            item.buffCom.SetPercent(rate);
                        }
                    }
                }
            }
        }
    }
}
