using System;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
namespace GameClient
{
    public sealed class BeBattleProjectile : BeBaseFighter
    {
        public sealed class BeBulletData : BeBaseActorData
        {
            public ulong attackId;
            public ulong targetId;
            public int typeId;
            public int damageValue;
        }
        public float durTime;
        public float speed = 10.0f;
        public BeBattleProjectile(BeBulletData data, ClientSystemGameBattle systemTown)
            : base(data, systemTown)
        {

        }
        private void OnDestroy()
        {
            Remove();
#if !LOGIC_SERVER
            if (_geActor != null && _geScene != null)
            {
                string str = "Effects/Common/Sfx/Hit/Prefab/Eff_hit_guai02_guo";
                if (_geActor.GetResID() == 730039) //手雷
                {
                    str = "Effects/Scene_effects/Scene_Chiji/Prefab/Eff_Scene_Chiji_shoulei_shouji";
                }
                else if (_geActor.GetResID() == 730040)
                {
                    //毒镖
                    str = "Effects/Scene_effects/Scene_Chiji/Prefab/Eff_Scene_Chiji_feibiao_shouji";
                }
                var gePos = _geActor.GetPosition();
                var effectPos = new Vec3(gePos.x, gePos.z, gePos.y);
                _geScene.CreateEffect(str, 0.0f, effectPos);
            }
#endif
        }
        public override void OnRemove()
        {
            Dispose();
        }
        public override void UpdateMove(float timeElapsed)
        {
            base.UpdateMove(timeElapsed);
            if (_geActor != null)
            {
                durTime += timeElapsed;
                var pos = _geActor.GetPosition();
                var data = _data as BeBulletData;
                if (data == null) return;
                var targetPlayer = GetPlayer(data.targetId);
                if (targetPlayer == null)
                {
                    OnDestroy();
                    return;
                }
                var targetPos = targetPlayer.ActorData.MoveData.Position;
                var srcVec = targetPos.xz() - pos.xz();
                Vector2 dir = srcVec;
                dir.Normalize();
                Vector2 offset = dir * speed * timeElapsed;
                pos.x += offset.x;
                pos.z += offset.y;
                var tarVec = targetPos.xz() - pos.xz();

                Vector3 targetDirPos = new Vector3(targetPlayer.ActorData.MoveData.Position.x, 0.0f, targetPlayer.ActorData.MoveData.Position.z);
                Vector3 srcDirPos = new Vector3(this.ActorData.MoveData.Position.x, 0.0f, this.ActorData.MoveData.Position.z);
                var forwardDir = targetDirPos - srcDirPos;
                //计算朝向这个正前方时的物体四元数值
                forwardDir.Normalize();
                if (forwardDir != Vector3.zero && _geActor != null)
                {
                    Quaternion lookAtRot = Quaternion.LookRotation(forwardDir);
                    //把四元数值转换成角度
                    Vector3 resultEuler = lookAtRot.eulerAngles;
                    float rotateY = resultEuler.y - 90;
                    _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Root).transform.localRotation = Quaternion.Euler(0, rotateY, 0);
                }
                if (srcVec.sqrMagnitude <= tarVec.sqrMagnitude)
                {
                    pos.x = targetPos.x;
                    pos.z = targetPos.z;
                    this.ActorData.MoveData.Position = pos;
                    OnDestroy();
                }
                else
                {
                    this.ActorData.MoveData.Position = pos;
                }
            }
        }
        public override void InitGeActor(GeSceneEx geScene)
        {
            _geScene = geScene;
            if (_geScene == null)
            {
                OnDestroy();
                return;
            }
            var data = _data as BeBulletData;
            if (data == null)
            {
                OnDestroy();
                return;
            }
            var attacker = GetPlayer(data.attackId);
            if (attacker == null)
            {
                OnDestroy();
                return;
            }
            var table = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(data.typeId);
            if (table == null)
            {
                Logger.LogErrorFormat("Projectile id {0} is Invalid", data.typeId);
                OnDestroy();
                return;
            }
            _geActor = _geScene.CreateActor(table.ResID);
            if (_geActor == null)
            {
                OnDestroy();
                return;
            }
            var srcPos = attacker.ActorData.MoveData.Position;
            srcPos.y = 1.0f;
            this.ActorData.MoveData.Position = srcPos;
            base.InitGeActor(geScene);

        }
        private BeFighter GetPlayer(ulong objId)
        {
            if (_battle == null)
                return null;
            return _battle.GetPlayer(objId);
        }
    }
}
