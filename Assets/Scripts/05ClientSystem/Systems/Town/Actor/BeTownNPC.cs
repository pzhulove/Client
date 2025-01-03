using Protocol;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameClient
{

    public class BeTownNPCData : BeBaseActorData
    {
        public int ResourceID { get; set; }
        public int NpcID { get; set; }

        public PlayerAvatar avatorInfo = null;

        // 目前用于城镇雕像,正常的npc用不上这个参数
        public int JobID { get; set; }

        // 目前用于城镇雕像,正常的npc用不上这个参数
        public string StatueName { get; set; }
        
        //Npc的类型，分为正常NPC和怪物攻城的NPC。默认为正常NPC
        public ESceneActorType TownNpcType = ESceneActorType.Npc;
    }

    class BeTownNPC : BeBaseActor
    {
        
        private List<int> mNeedShowDialogNPCIdList = new List<int>() { 2095 }; //需要显示冒泡功能的npc
        public BeTownNPC(BeTownNPCData data, ClientSystemTown systemTown)
            :base(data, systemTown)
        {
            
        }

        public void AddActorPostLoadCommand(PostLoadCommand async)
        {
            if (null != _geActor)
            {
                _geActor.PushPostLoadCommand(async);
            }
        }

        public override void InitGeActor(GeSceneEx geScene)
        {
            if (geScene == null)
            {
                return;
            }

            try
            {
                if (_geActor == null)
                {
                    BeTownNPCData townData = _data as BeTownNPCData;

                    ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(townData.NpcID);

                    _geScene = geScene;

                    string name = "";

                    if (npcItem.Function == ProtoTable.NpcTable.eFunction.Townstatue || npcItem.Function == ProtoTable.NpcTable.eFunction.guildGuardStatue)
                    {
                        _geActor = geScene.CreateActor(townData.ResourceID, 0, npcItem.Height, false, false);
                        if (!string.IsNullOrEmpty(townData.StatueName))
                        {
                            name = townData.StatueName;
                        }
                    }
                    else
                    {
                        if (EngineConfig.useTMEngine)
                            _geActor = geScene.CreateActorAsyncEx(townData.ResourceID, 0, npcItem.Height, false, false);
                        else
                            _geActor = geScene.CreateActorAsync(townData.ResourceID, 0, npcItem.Height, false, false);
                        name = townData.Name;
                    }           

                    if (_geActor == null)
                    {
                        return;
                    }

                    ActorData.MoveData.TransformDirty = true;
                    UpdateGeActor(0.0f);

                    //_geActor.CreateInfoBar(name, townData.NameColor, 0);
                    //_geActor.AddNPCBoxCollider(townData);
                    _geActor.AddNpcInteraction(townData.NpcID,townData.GUID);

                    float defaultNamePosY = 0.0f;
                    if (npcItem.NameLocalPosY > 0)
                        defaultNamePosY = (float) npcItem.NameLocalPosY / 1000.0f;

                    _geActor.AddNPCFunction(name, townData.NameColor, 0, null,defaultNamePosY);
                    

                    if (npcItem != null)
                    {

                        if (townData.avatorInfo != null)
                        {
                            PlayerBaseData.GetInstance().AvatarEquipFromItems(null, townData.avatorInfo.equipItemIds, townData.JobID, (int)(townData.avatorInfo.weaponStrengthen), _geActor);
                        }
                        _geActor.SuitAvatar();
                        //加载NPCVoice脚本
                        _geActor.AddNpcVoiceComponent(townData.NpcID);
                        //加载箭头
                        _geActor.AddNpcArrowComponent();

                        ////加载NPCDialog脚本
                        //_geActor.AddComponentDialog(townData.NpcID, NpcDialogComponent.IdBelong2.IdBelong2_NpcTable);
                        AddComponentDialog(townData);
                    }
                    else
                    {
                        Logger.LogError("[BeTownNPC] npcItem is null");
                    }
                }
            }
            catch(System.Exception e)
            {
                _geActor = null;
                Logger.LogError(e.ToString());
            }
        }

        protected virtual void AddComponentDialog(BeTownNPCData townData)
        {

            if(townData!=null)
            {
                if (mNeedShowDialogNPCIdList.Contains(townData.NpcID))
                {
                    _geActor.PushPostLoadCommand(() => { _geActor.AddComponentDialog(townData.NpcID, NpcDialogComponent.IdBelong2.IdBelong2_NpcTable); });
                }
            }
           
           
            //_geActor.PushPostLoadCommand(() => { _geActor.AddComponentDialog(townData.NpcID, NpcDialogComponent.IdBelong2.IdBelong2_NpcTable); });
        }

        public override void Update(float deltaTime)
        {
            if(_geActor != null)
            {
                _geActor.Update(0,0);
                _geActor.UpdateNpcInteraction(deltaTime);
                _geActor.UpdateDialogComponet(deltaTime);
                _geActor.UpdateVoiceComponent(deltaTime);
            }

            base.Update(deltaTime);
        }
    }
}
