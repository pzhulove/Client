using UnityEngine;
using System.Collections.Generic;


namespace GameClient
{
    public class DungeonBuffInfoFrame : ClientFrame
    {
        private const string kPath2 = "UIFlatten/Prefabs/BattleUI/DungeonBuffInfoUnit";
        private List<int> mBuffList = new List<int>();
        private List<ComTipInfoUnit> mComBuffInfoList = new List<ComTipInfoUnit>();

        [UIObject("Root/Center/Scroll/Viewport/Content")]
        private GameObject mContentRoot;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/DungeonBuffInfoFrame";
        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffListChanged, _updateAllBuff);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffRemoved, _removeBuff);

            _updateAllBuff(null);
        }
        
        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffListChanged, _updateAllBuff);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffRemoved, _removeBuff);
        }

        private void _clearAll()
        {
            mComBuffInfoList.RemoveAll(x =>
            {
                if (null != x)
                {
                    GameObject.Destroy(x);
                    x = null;
                }
                return true;
            });

            mBuffList.Clear();
        }

        private void _updateAllBuff(UIEvent ui)
        {
            _clearAll();

            for (int i = 0; i < PlayerBaseData.GetInstance().buffList.Count; ++i)
            {
                var buff = PlayerBaseData.GetInstance().buffList[i];
                if (buff.lefttime > 0)
                {
                    _realAddBuff(buff.id);
                }
            }

            _updateBuffBoard();
        }


        private void _removeBuff(UIEvent ui)
        {
            int buffId = (int)ui.Param1;
            _removeBuff(buffId);
            _updateBuffBoard();
        }


        private void _realAddBuff(int id)
        {
            var idx = mBuffList.BinarySearch(id);
            if (idx < 0)
            {
                if (_addBuffUnit(~idx, id))
                {
                    mBuffList.Insert(~idx, id);
                }
                else 
                {
                    Logger.LogErrorFormat("[bufflist] 添加buff {0} 失败", id);
                }
            }
        }

        private bool _addBuffUnit(int idx, int id)
        {
            var data = TableManager.instance.GetTableItem<ProtoTable.BuffTable>(id);
            if (data != null)
            {
				if (!Utility.IsStringValid(data.Icon))
					return false;

                Sprite sprite = null;

                var node = AssetLoader.instance.LoadRes(data.Icon, typeof(Sprite));
                if (node != null && node.obj != null)
                {
                    var go2 = AssetLoader.instance.LoadResAsGameObject(kPath2);
                    var com2 = go2.GetComponent<ComTipInfoUnit>();
                    go2.name = string.Format(idx.ToString());
                    Utility.AttachTo(go2, mContentRoot);

                    sprite = node.obj as Sprite;

                    com2.SetData(data);
                    com2.SetSprite(sprite);

                    mComBuffInfoList.Insert(idx, com2);

                    return true;
                }
                else
                {
                    Logger.LogProcessFormat("[bufflist] buff表 Icon {0} 失效", data.Icon);
                }
            }
            else 
            {
                Logger.LogProcessFormat("[bufflist] buff表 {0} 失败", id);
            }

            return false;
        }

        private void _updateBuffBoard()
        {
            for (int i = 0; i < mComBuffInfoList.Count; ++i)
            {
                mComBuffInfoList[i].SetBgActive(i % 2 > 0);
            }
        }

        private void _removeBuff(int id)
        {
            var idx = mBuffList.BinarySearch(id);
            if (idx >= 0)
            {
                if (mComBuffInfoList[idx].gameObject != null)
                {
                    GameObject.Destroy(mComBuffInfoList[idx].gameObject);
                }

                mBuffList.RemoveAt(idx);
                mComBuffInfoList.RemoveAt(idx);
            }
        }

        [UIEventHandle("Root/Close")]
        private void _onClose()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }

        [UIEventHandle("Bg")]
        private void _onBgClose()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }
    }
}
