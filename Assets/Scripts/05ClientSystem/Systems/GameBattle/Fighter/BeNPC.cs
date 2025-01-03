using System;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
namespace GameClient
{
    public sealed class BeNPCData : BeBaseActorData
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
    public class BeNPC : BeBaseFighter
    {
        public BeNPC(BeNPCData data, ClientSystemGameBattle systemTown)
            :base(data, systemTown)
        {

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
                    BeNPCData townData = _data as BeNPCData;

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
                    _geActor.AddNpcInteraction(townData.NpcID, townData.GUID);

                    float defaultNamePosY = 0.0f;
                    if (npcItem.NameLocalPosY > 0)
                        defaultNamePosY = (float)npcItem.NameLocalPosY / 1000.0f;

                    _geActor.AddNPCFunction(name, townData.NameColor, 0, null, defaultNamePosY);


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
                    }
                    else
                    {
                        Logger.LogError("[BeTownNPC] npcItem is null");
                    }
                }
            }
            catch (System.Exception e)
            {
                _geActor = null;
                Logger.LogError(e.ToString());
            }
        }
        public override void Update(float deltaTime)
        {
            if (_geActor != null)
            {
                _geActor.Update(0, 0);
                _geActor.UpdateNpcInteraction(deltaTime);
                _geActor.UpdateDialogComponet(deltaTime);
                _geActor.UpdateVoiceComponent(deltaTime);
            }

            base.Update(deltaTime);
        }
    }
}
