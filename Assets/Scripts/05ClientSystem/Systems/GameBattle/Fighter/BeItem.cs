using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine.UI;
namespace GameClient
{
    public sealed class BeItemData : BeBaseActorData
    {
        public SceneItem mDropItem;
    }
    public sealed class BeItem : BeBaseFighter
    {
        private SceneRegionTable mRegionTable = null;
        private ItemData mItemTable = null;
        int itemId = 0;
        UInt64 guid = 0;
        private string mDropItemExtraDesc = string.Empty;
        Vector3 mPos = Vector3.zero;
        Vector3 mServerPos = Vector3.zero;
        bool isBuffItem = false;
#if !SERVER_LOGIC

        private GameObject objText;
        private GameObject objEffect;
        private GameObject objEffect2;
        private GameObject dropModel = null;
#endif
        public Vector3 Pos
        {
            get { return mPos; }
        }
        public int ItemID
        {
            get { return itemId; }
        }
        public bool IsBuffItem
        {
            get { return isBuffItem; }
        }
        public ItemData ItemTableData { get { return mItemTable; } }
        public BeItem(BeItemData data, ClientSystemGameBattle systemTown)
            : base(data, systemTown)
        {
            itemId = (int)data.mDropItem.data_id;
            mServerPos.x = data.mDropItem.pos.x;
            mServerPos.z = data.mDropItem.pos.y;
            ActorData.GUID = data.mDropItem.guid;
        }
        public override void OnRemove()
        {
           
            if(itemId == 401000005)
            {
                var itemData = ActorData as BeItemData;
                if (itemData.mDropItem.owner != 0)
                {
                    //陷阱播放爆炸特效
                }
            }
            Dispose();
        }
        public void AddActorPostLoadCommand(PostLoadCommand async)
        {
            if (null != _geActor)
            {
                _geActor.PushPostLoadCommand(async);
            }
        }
        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);
            if (isBuffItem && _battle != null && _battle.MainPlayer != null)
            {
                var dir = ActorData.MoveData.Position.xz() - _battle.MainPlayer.ActorData.MoveData.Position.xz();
                if (dir.magnitude <= 1.0f)
                {
                    ChijiDataManager.GetInstance().SendPickUpBuffItems(ActorData.GUID);

                    Remove();
                    //Logger.LogErrorFormat("item uid {0}", ActorData.GUID);
                }
            }
        }

        public override void InitGeActor(GeSceneEx geScene)
        {
            if (geScene == null)
            {
                return;
            }
#if !LOGIC_SERVER
            try
            {
                if (_geActor == null)
                {
                    ISceneData levelData = _battle.LevelData;
                    bool bCreated = false;
                    var dropTableItem = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(itemId);
                    mItemTable = GameClient.ItemDataManager.CreateItemDataFromTable(itemId);
                    if (dropTableItem != null)
                    {
                        if (dropTableItem.SubType == ItemTable.eSubType.ChijiBuff)
                        {
                            isBuffItem = true;
                            var resTable = TableManager.instance.GetTableItem<ProtoTable.ResTable>(dropTableItem.ResID);
                            if (resTable != null)
                            {
                                _geActor = geScene.CreateActor(dropTableItem.ResID, 0, 0, false, false);
                                if (_geActor != null)
                                {
                                    bCreated = true;
                                    if (isBuffItem)
                                    {
                                        var go = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Battle_Digit/SpritiBuffText", enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
                                        if (go != null)
                                        {
                                            Battle.GeUtility.AttachTo(go, _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));

                                            var bind = go.GetComponent<ComCommonBind>();
                                            if (bind != null)
                                            {
                                                Text txtDesc = bind.GetCom<Text>("txtDesc");
                                                if (txtDesc != null)
                                                    txtDesc.text = dropTableItem.Name /*+ ActorData.GUID.ToString()*/;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("BeItem itemId {0} is not valid !", itemId); 
                    }

                    if (!bCreated && dropTableItem != null)
                    {
//                         mRegionTable = TableManager.GetInstance().GetTableItem<SceneRegionTable>(4);
//                         if (mRegionTable == null)
//                         {
//                             Logger.LogErrorFormat("sceneregion table not contain the item with id {0}", 4);
//                             return;
//                         }

                        // 创建宝箱
                        _geActor = geScene.CreateActor(dropTableItem.ResID, 0, 0, false, false);
                        // 添加宝箱名字
                        if (_geActor != null)
                        {
                            _geActor.ChangeAction("Anim_Idle", 1, true);
                            bool isBox = dropTableItem.ThirdType == ItemTable.eThirdType.ChijiGiftPackage ||
                                         dropTableItem.ThirdType == ItemTable.eThirdType.UseToOther ||
                                         dropTableItem.ThirdType == ItemTable.eThirdType.UseToOther; //宝箱类

                            var node = _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);

                            GameObject box = null;

                            if (isBox && node != null && node.transform != null && node.transform.childCount > 0)
                            {
                                // 取到宝箱节点
                                var boxNode= node.transform.GetChild(0);

                                if(boxNode != null)
                                {
                                    box = boxNode.gameObject;
                                }
                            }

                            if (null != box)
                            {
                                // 对宝箱的模型朝向，文字朝向要特殊处理
                                box.transform.rotation = Quaternion.Euler(0, 90, 0);

                                // 是否已经绑定过了name
                                Utility.ClearChild(box, 2);

                                if (null != dropTableItem)
                                {
                                    GameObject textName = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Chiji/ChijiDropBoxText");
                                    var bind = textName.GetComponent<ComCommonBind>();
                                    objText = textName;
                                    if (null == bind)
                                    {
                                        return;
                                    }

                                    Battle.GeUtility.AttachTo(textName, box);

                                    textName.transform.rotation = Quaternion.Euler(0, 360, 0);

                                    Text dropName = bind.GetCom<Text>("dropName");
                                    //Image dropIcon = bind.GetCom<Image>("dropIcon");
                                    GameObject _dropModel = bind.GetGameObject("dropModel");
                                    GameObject uniformTips = bind.GetGameObject("uniformTips");
                                    Text uniformText = bind.GetCom<Text>("uniformText");
                                    uniformTips.SetActive(false);

                                    dropName.text = string.Format("<color={0}>{1}</color>", mItemTable.GetQualityInfo().ColStr, dropTableItem.Name);
                                    dropModel = _dropModel;

                                    string modelPath = dropTableItem.ModelPath;
                                    if (IsGold(itemId))
                                    {
                                        ClientSystemManager.GetInstance().delayCaller.DelayCall(800, () =>
                                        {
                                            AudioManager.instance.PlaySound(2);
                                        });
                                    }

                                    //ETCImageLoader.LoadSprite(ref dropIcon, modelPath);
                                }

                                GeMeshRenderManager.GetInstance().AddMeshObject(node);
                            }
                        }
                        else
                        {
                            Logger.LogErrorFormat("resid = 0导致_geActor == null, 让鑫伟配吃鸡道具表的resId字段, 道具id：{0},name = {1}", itemId, dropTableItem.Name);
                        }
                    }

                    mPos = mServerPos;
                    _geScene = geScene;
                    ActorData.MoveData.TransformDirty = true;
                    UpdateGeActor(0.0f);
                }
            }
            catch (System.Exception e)
            {
                _geActor = null;
                Logger.LogError(e.ToString());
            }
#endif
        }
        private bool IsGold(int type)
        {
            return type == Global.GOLD_ITEM_ID || type == Global.GOLD_ITEM_ID2;
        }
    }
}
