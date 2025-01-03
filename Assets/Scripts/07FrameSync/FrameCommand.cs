using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using UnityEngine;

namespace GameClient
{
    public enum SpecialSkillID
    {
        JUMP = int.MaxValue,
        JUMP_BACK = int.MaxValue - 1,
        NORMAL_ATTACK = int.MaxValue - 2
    }

    public enum ButtonState
    {
        NONE = 0,
        PRESS = 1,
        RELEASE = 2,
        PRESS_AGAIN = 3,
        PRESS_JOYSTICK =4,
    }

    // 地下城结束原因
    public enum CrossDungeonEndReason
    {
        CDER_INVALID,
        CDER_TEAMCOPY_FILED_DESTORY, //团本据点被歼灭
        CDER_TEAMCOPY_SETTLE,        //团本结算
    };

    public class FrameCommandFactory
    {
        public static IFrameCommand CreateCommand(UInt32 Type)
        {
            FrameCommandID id = (FrameCommandID)Type;

            switch(id)
            {
                case FrameCommandID.Move:
                    return new MoveFrameCommand();
                case FrameCommandID.Stop:
                    return new StopFrameCommand();
                case FrameCommandID.Skill:
                    return new SkillFrameCommand();
                case FrameCommandID.StopSkill:
                    return new StopSkillCommand();
                case FrameCommandID.DoAttack:
                    return new DoAttackCommand();
                case FrameCommandID.Reborn:
                    return new RebornFrameCommand();
                case FrameCommandID.Leave:
                    return new LeaveFrameCommand();
                case FrameCommandID.ReconnectEnd:
                    return new ReconnectFrameCommand();
                case FrameCommandID.UseItem:
                    return new UseItemFrameCommand();
				case FrameCommandID.LevelChange:
					return new LevelChangeCommand();
				case FrameCommandID.AutoFight:
					return new AutoFightCommand();
				case FrameCommandID.DoublePressConfig:
					return new DoublePressConfigCommand();
                case FrameCommandID.GameStart:
                    return new GameStartFrame();
                case FrameCommandID.RaceEnd:
                    return new RaceEndCommand();
                case FrameCommandID.PlayerQuit:
                    return new PlayerQuitCommand();
                case FrameCommandID.NetQuality:
                    return new NetQualityCommand();
                case FrameCommandID.RacePause:
                    return new RacePuaseFrame();
                case FrameCommandID.SceneChangeArea:
                    return new SceneChangeArea();
                case FrameCommandID.MatchRoundVote:
                    return new MatchRoundVote();
                case FrameCommandID.ChangeWeapon:
                    return new ChangeWeaponCommand();
                case FrameCommandID.SyncSight:
                    return new DoSyncSightCommand();
                case FrameCommandID.PassDoor:
                    return new PassDoorCommand();
                case FrameCommandID.BossPhaseChange:
                    return new BossPhaseChange();
                case FrameCommandID.DungeonDestory:
                    return new DungeonDestory();
                case FrameCommandID.TeamCopyRaceEnd:
                    return new TeamCopyRaceEnd();
                case FrameCommandID.TeamCopyBimsProgress:
                    return new DungeonProcess();
                case FrameCommandID.CloseFilm:
                    return new CloseFilmCommand();
            }

            return null;
        }
    }

    public class FrameCommandPool
    {
        Dictionary<uint, List<IFrameCommand>> m_FrameCommandPool = new Dictionary<uint, List<IFrameCommand>>();

        public IFrameCommand GetFrameCommand(uint typeID)
        {
            IFrameCommand newCommand = null;
            List<IFrameCommand> commandList = null;
            if (!m_FrameCommandPool.TryGetValue(typeID, out commandList))
            {
                commandList = new List<IFrameCommand>();
                m_FrameCommandPool.Add(typeID, commandList);
            }

            if(commandList.Count > 0)
            {
                int lastIdx = commandList.Count - 1;
                newCommand = commandList[lastIdx];
                commandList.RemoveAt(lastIdx);

                newCommand.Reset();
                return newCommand;
            }

            return FrameCommandFactory.CreateCommand(typeID);
        }

        public void RecycleFrameCommand(IFrameCommand command)
        {
            if(null != command)
            {
                uint typeID = (uint)command.GetID();
                
                List<IFrameCommand> commandList = null;
                if (!m_FrameCommandPool.TryGetValue(typeID, out commandList))
                {
                    commandList = new List<IFrameCommand>();
                    m_FrameCommandPool.Add(typeID, commandList);
                }

                commandList.Add(command);
            }
        }
    }


    public interface IOnExecCommand
    {
        void BeforeExecFrameCommand(byte seat, IFrameCommand cmd);

        void AfterExecFrameCommand(byte seat, IFrameCommand cmd);
    }

    public interface IFrameCommand
    {
        FrameCommandID GetID();
        UInt32         GetFrame();
        byte           GetSeat();
        UInt32         GetSendTime();

        void           ExecCommand();
        _inputData     GetInputData();
        void SetValue(UInt32 frame,byte seat,_inputData data);
        string GetString();

        void Reset();
    }

    public class BaseFrameCommand
    {
        public UInt32 frame;
        public byte   seat = 0xFF;
        public UInt32 sendTime;

        public BaseBattle battle;
        public bool isValid()
        {
            if (null == battle || null == battle.dungeonPlayerManager)
            {
                return false;
            }
            return true;
        }
        public BeActor GetActorBySeat(byte seatData)
        {
            if (!isValid())
            {
                return null;
            }

            BattlePlayer player = battle.dungeonPlayerManager.GetPlayerBySeat(seatData);

            if (player == null)
            {
                player = battle.dungeonPlayerManager.GetMainPlayer();
            }

            if(player == null)
            {
                Logger.LogErrorFormat("Seat error {0}\n",seatData);
                return null;
            }
            return player.playerActor;
        }

        public void Record(BeActor actor, string cmd)
        {
            if (actor != null && actor.IsProcessRecord())
            {
                actor.GetRecordServer().RecordProcess("[CMD]pid:{0}-{1} real ExecCommand:{2} {3}", actor.m_iID, actor.GetName(), cmd, actor.GetInfo());
                actor.GetRecordServer().Mark(0x8779804, actor.GetEntityRecordAttribute(), actor.GetName(), cmd);
                // Mark:0x8779804 [CMD]PID:{0}-{12} real ExecCommand:{13} Pos:({1},{2},{3}),Speed:({4},{5},{6}),Face: {7},Hp: {8},Mp: {9},Flag: {10},curSkillId: {11}
            }
        }

        public byte GetSeat()
        {
            return seat;
        }

        public UInt32 GetSendTime()
        {
            return sendTime;
        }

        protected void BaseReset()
        {
            frame = 0;
            seat = 0xFF;
            sendTime = 0;
        }

		protected void _callRandomWithHpMp()
		{
            if (!isValid())
            {
                return;
            }

            List<BattlePlayer> allBattlePlayer = battle.dungeonPlayerManager.GetAllPlayers();

			if (null == allBattlePlayer) 
			{
				return ;
			}

			for (int i = 0; i < allBattlePlayer.Count; ++i)
			{
				BeActor beActor = allBattlePlayer[i].playerActor;
				if (null != beActor) 
				{
					BeEntityData data = beActor.GetEntityData();
					if (null != data) 
					{
						battle.FrameRandom.RandomCallNum ((uint)(data.GetMP() / GlobalLogic.VALUE_100));
						battle.FrameRandom.RandomCallNum ((uint)(data.GetHP() / GlobalLogic.VALUE_100));
					}
				}
			}
		}
    }

    public class MatchRoundVote : BaseFrameCommand, IFrameCommand
    {
        public bool isVote { set; get; }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            if (null != battle && battle.dungeonPlayerManager != null)
            {
                IDungeonPlayerDataManager playerDataMgr = battle.dungeonPlayerManager;

                if (null != playerDataMgr)
                {
                    playerDataMgr.SetPlayerVoteFightState(seat, isVote);
                }
                else
                {
                    Logger.LogErrorFormat("[MatchRoundVote] 出战投票 无法找到玩家数据管理 {0} {1}", seat, isVote);
                }
            }

            
			if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public uint GetFrame()
        {
            return frame;
        }

        public FrameCommandID GetID()
        {
            return FrameCommandID.MatchRoundVote;
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.MatchRoundVote,
                data2 = isVote ? 1u : 0u,
                data3 = 0,
                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;

            isVote = data.data2 != 0;
        }

        public string GetString()
        {
            return string.Format("[MatchRoundVote] {0} {1}", seat, isVote);
        }

        public void Reset()
        {
            BaseReset();
        }
    }

    public class SceneChangeArea : BaseFrameCommand, IFrameCommand
    {
        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

			_callRandomWithHpMp();
            
			if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public uint GetFrame()
        {
            return frame;
        }

        public FrameCommandID GetID()
        {
            return FrameCommandID.SceneChangeArea;
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.SceneChangeArea,
                data2 = 0,
                data3 = 0,

                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;
        }

        public string GetString()
        {
            return "[SceneChangeArea]";
        }

        public void Reset()
        {
            BaseReset();
        }
    }

    public class RacePuaseFrame : BaseFrameCommand, IFrameCommand
    {
        public bool isPauseLogic = false;
        public void ExecCommand()
        {
            if (!isValid()) return;
            BeScene scene = battle.dungeonManager.GetBeScene();

            if (null == scene)
            {
                return ;
            }
            
            if (isPauseLogic)
            {
                scene.PauseLogic();
            }
            else
            {
                scene.ResumeLogic();
            }
        }

        public uint GetFrame()
        {
            return frame;
        }

        public FrameCommandID GetID()
        {
            return FrameCommandID.RacePause;
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.RacePause,
                data2 = isPauseLogic ? 1u : 0u,

                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;

            isPauseLogic = data.data2 > 0u;
        }

        public string GetString()
        {
            return string.Format("[RacePuaseFrame] Seat {0} Frame {1} {2}", seat, frame, isPauseLogic);
        }

        public void Reset()
        {
            BaseReset();
            isPauseLogic = false;
        }
    }

    public class GameStartFrame : BaseFrameCommand,IFrameCommand
    {
        public uint startTime;
        
        public FrameCommandID GetID()
        {
            return FrameCommandID.GameStart;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            //UnityEngine.Debug.LogError("GameStart exc");

			int tempSeat = seat >= 255?0:seat;

			BeActor actor = GetActorBySeat((byte)tempSeat);
            if (actor != null && actor.IsProcessRecord())
            {
                Record(actor, "[GAMESTART]" + GetString());
            }


            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

#if !SERVER_LOGIC

            FrameSync.instance.OnRelayGameStart(startTime);
#else
            if (null != battle.logicServer)
            {
                battle.logicServer.OnRelayGameStart(startTime);
            }
#endif

			_callRandomWithHpMp();

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }

            if (battle != null)
            {
                battle.InitLevelManager();
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.GameStart,
                data2 = (UInt32)startTime,

                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;
            startTime = (UInt32)data.data2;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} StartTime:{2}", frame, seat, startTime);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            startTime = 0;
        }
    }

    public class PlayerQuitCommand : BaseFrameCommand, IFrameCommand
    {
        public FrameCommandID GetID()
        {
            return FrameCommandID.PlayerQuit;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            BeActor actor = GetActorBySeat(seat);


			if (GameClient.SwitchFunctionUtility.IsOpen(10))
			{
				//只有深渊才进行自动战斗
				if (actor != null && battle != null && battle.GetBattleType() == BattleType.Hell)  
				{
					actor.SetAutoFight(true);
#if !LOGIC_SERVER
                    if (battle.dungeonPlayerManager != null)
                    {
                        BattlePlayer player = battle.dungeonPlayerManager.GetPlayerBySeat(seat);

                        if(player != null && player.playerInfo != null)
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("hell_player_quit", player.playerInfo.name));
                    }
 #endif

				}
			}


            if (actor != null && actor.IsProcessRecord())
            {
                Record(actor, "[PlayerQuit]" + GetString());
            }

            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }


            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.PlayerQuit,

                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} PlayerQuit !!", frame, seat);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
        }
    }

    public class NetQualityCommand : BaseFrameCommand, IFrameCommand
    {
        public uint quality = 0;

        public FrameCommandID GetID()
        {
            return FrameCommandID.NetQuality;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            BeActor actor = GetActorBySeat(seat);
			if (actor != null && actor.IsProcessRecord())
            {
                Record(actor, "[NetQuality update]" + GetString());
            }

            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.NetQuality,
                data2 = (UInt32)quality,

                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame   = frame;
            this.seat    = seat;
            this.quality = (UInt32)data.data2;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} Quality Change To {2} !!", frame, seat, quality);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            quality = 0;
        }
    }

    public class RaceEndCommand : BaseFrameCommand, IFrameCommand
    {
        uint reasonCode = 0;
        public FrameCommandID GetID()
        {
            return FrameCommandID.RaceEnd;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
// 			int tempSeat = seat >= 255?0:seat;
// 
// 			BeActor actor = GetActorBySeat((byte)tempSeat);
// 			if (actor != null && actor.IsProcessRecord())
//             {
//                 Record(actor, "[RaceEnd]" + GetString());
//             }
//  
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            ClientReconnectManager.instance.canReconnectRelay = false;

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }

			
#if !SERVER_LOGIC 
            //游戏结束！！！！！，如果是录像回放就在这里打开界面
			if (ReplayServer.GetInstance().IsReplay())
			{
				ReplayServer.GetInstance().Stop(true,"RaceEndCmd");
			}

 #endif
            //!! 待定
			//actor.GetRecordServer().EndRecord();
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.RaceEnd,

                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;
            this.reasonCode = data.data2;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} Code:{2} RaceEnd!!", frame, seat,reasonCode);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
        }
    }


    public class MoveFrameCommand : BaseFrameCommand,IFrameCommand
    {
        public short  degree;
        public bool   run;
        
        public FrameCommandID GetID()
        {
            return FrameCommandID.Move;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            BeActor actor = GetActorBySeat(seat);
			if (actor != null && !actor.IsDead())
            {
				if (actor != null && actor.IsProcessRecord())
				{
					Record(actor, GetString());
				}
					
                actor.ModifyMoveDirection(true, degree);
				if (run)
					actor.ChangeRunMode(run);

				actor.RecordPressCount();
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.Move,
                data2 = (UInt32)degree,
                data3 = (UInt32)(run ? 1 : 0),

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;
            degree =(short)(data.data2);
            run = (data.data3 == 1);
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} Move Direction:{2} Run:{3}", frame, seat, degree, run);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            degree = 0;
            run = false;
        }
    }

    public class StopFrameCommand : BaseFrameCommand, IFrameCommand
    {
        public FrameCommandID GetID()
        {
            return FrameCommandID.Stop;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            BeActor actor = GetActorBySeat(seat);
			if (actor != null)
			{
				if (actor != null && actor.IsProcessRecord())
				{
					Record(actor, GetString());
				}

				actor.ModifyMoveDirection(false);
				actor.RecordPressCount();
			}
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.Stop,
                data2 = (UInt32)0,
                data3 = (UInt32)0,

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} StopMove", frame, seat);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
        }
    }
		
    public class SkillFrameCommand : BaseFrameCommand, IFrameCommand
    {
        public UInt32 skillSolt;
        public UInt32 skillSlotUp = 0;

        public enum SkillFrameDataType
        {
            None,
            Joystick_SpecialChoice,
            Joystick_ForwardBackChoice,
            Joystick_Release,
            Joystick_Position,
            Joystick_Degree,
            Joystick_ModeIndex,
            Joystick_PlayerIndex,
            Joystick_ActionIndex,
        }
        /// <summary>
        /// 注意：不要溢出
        /// </summary>
        public struct SkillFrameData
        {
            public bool isUp;                    // 是否抬起     1位
            public SkillFrameDataType type;      // 数据类型     5位
            public uint data1;                    // 数据1       11位
            public uint data2;                    // 数据2       11位
            public uint data3;                    // 数据2       4位
        }

        public static SkillFrameData Parse(UInt32 data)
        {
            SkillFrameData ret = new SkillFrameData();
            ret.isUp = BeUtility.GetByte(data, 0, 1) > 0;
            ret.type = (SkillFrameDataType)BeUtility.GetByte(data, 1, 6);
            ret.data1 = BeUtility.GetByte(data, 6, 17);
            ret.data2 = BeUtility.GetByte(data, 17, 28);
            ret.data3 = BeUtility.GetByte(data, 28, 32);
            //Debug.LogErrorFormat("Parse Data{0}, {1},{2},{3},{4}", data, ret.isUp, ret.type, ret.data1, ret.data2);
            return ret;
        }

        public static UInt32 Assemble(SkillFrameData data)
        {
            UInt32 ret;
            ret = (data.isUp ? (UInt32)1 : (UInt32)0);
            ret += (UInt32) data.type << 1;
            ret += (UInt32) data.data1 << 6;
            ret += (UInt32) data.data2 << 17;
            ret += (UInt32) data.data3 << 28;
            //Debug.LogErrorFormat("Assemble Data{0}, {1},{2},{3},{4}", ret, data.isUp, data.type, data.data1, data.data2);
            return ret;
        }
        
        public FrameCommandID GetID()
        {
            return FrameCommandID.Skill;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.Skill,
                data2 = (UInt32)skillSolt,
                data3 = (UInt32)skillSlotUp,

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;
            skillSolt  = data.data2;
            skillSlotUp = data.data3;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} Skill Slot:{2}", frame, seat, skillSolt);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void ExecCommand()
        {
            SkillFrameData skillData = Parse(skillSlotUp);
            BeActor actor = GetActorBySeat(seat);
            if (battle == null || actor == null || actor.IsDead())
                return;
            var dungeon = this.battle.dungeonManager as BeDungeon;
            if (dungeon != null && dungeon.IsInFade)
            {
                if (actor.IsProcessRecord())
                {
                    actor.GetRecordServer().RecordProcess("[CMD]pid:{0}-{1} ignore SkillCommand {2}", actor.m_iID, actor.GetName(), GetString());
                    actor.GetRecordServer().Mark(0x12980286, new[] { actor.m_iID, (int)frame, seat, (int)skillSolt }, actor.GetName());
                    // Mark:0x12980286 [CMD]PID:{0}-{4} Ignore SkillCommand Frame:{1} Seat:{2} Skill Slot:{3}
                }
                return;
            }

            if (!actor.stateController.HasAbility(BeAbilityType.CAN_DO_SKILL_CMD))
            {
                return;
            }

            if (actor.isMainActor)
				actor.RecordPressCount();

			if (actor != null && actor.IsProcessRecord())
			{
				Record(actor, GetString());
			}

            var eventData = actor.TriggerEventNew(BeEventType.onExecSkillFrame, new EventParam(){m_Bool2 = false, m_Bool = skillData.isUp, m_Int = (int) skillSolt});
            if(eventData.m_Bool2)
                return;
            
            if (skillSolt == (int)SpecialSkillID.JUMP)
            {
                if (actor.CanJump())
                {
                    BeStateData state = new BeStateData((int)ActionState.AS_JUMP);
                    actor.Locomote(state);

					//actor.SummonHelp(4070071, 30, 9025);
                }
				//actor.buffController.TryAddBuff(8, 3000);

            }
            //jump back
            else if (skillSolt == (int)SpecialSkillID.JUMP_BACK)
            {
                //改成按下去的时候就释放，松开的时候不释放
				if (skillData.isUp)
					return;

                if (actor.CanJumpBack())
                {
                   BeStateData state = new BeStateData((int)ActionState.AS_JUMPBACK);
                   actor.Locomote(state);


					//actor.Locomote(new BeStateData((int)ActionState.AS_FALL, 0, 2, 5f, 0, 300), true);

                    //actor.aiManager.tree.SendEvent("TestEvent");
					//actor.buffController.TryAddBuff(1200046, 100000);

					//actor.TriggerEvent(BeEventType.onHit);

                }
                //后退取消技能
                else
                {
                    if (actor.IsCastingSkill())
                    {
                        var skill = actor.GetCurrentSkill();
                        if (skill != null && !skill.IsCanceled())
                        {
                            if (skill.canPressJumpBackCancel)
                                actor.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                        }
                    }
                }
            }
            //attack
            else if (skillSolt == (int)SpecialSkillID.NORMAL_ATTACK)
            {
                //Logger.LogErrorFormat("press skill{0} is up {1}", 1, skillSlotUp);

                if (skillData.isUp)
                {
                    actor.SetAttackButtonState(ButtonState.RELEASE);
                    //普攻也支持蓄力功能
                    if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                    {
                        var skill = actor.GetCurrentSkill();
                        if (skill != null && skill.charge)
                        {
                            skill.SetButtonRelease();
                            actor.GetStateGraph().FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_RELEASE_SKILL_BUTTON);
                        }
                    }
                }
                else
					actor.SetAttackButtonState(ButtonState.PRESS);
            }
            else
            {
                int skillID = (int)skillSolt;

				if (skillID == Global.HELP_SKILL_ID)
				{
					actor.UseHelpSkill();
					return;
				}

                if (skillID > 0)
                {
                    if (skillData.isUp)
                    {
                        if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                skill.SetButtonRelease();
                            }
                            BeSkill skillCasting = actor.GetCurrentSkill();
                            if (skillCasting != null && (skillCasting.comboSkillSourceID == skillID))
                            {
                                skillCasting.SetButtonRelease();
                            }
                            actor.TriggerEventNew(BeEventType.OnReleaseButtonTrigger);
                            actor.GetStateGraph().FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_RELEASE_SKILL_BUTTON);
                        }
                    }
                    else
                    {
                        if (skillData.type == SkillFrameDataType.Joystick_ModeIndex)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                UInt32 select = skillData.data1;
                                var modeSkill = skill as IModeSelectSkill;
                                if (modeSkill != null)
                                {
                                    modeSkill.OnSelectMode((int) select);
                                }
                            }
                        }
                        else if (skillData.type == SkillFrameDataType.Joystick_Degree)
						{
							var skill = actor.GetSkill(skillID);
							if (skill != null)
							{
								var degree = skillData.data1;
								skill.UpdateJoystick((int)degree);
							}
						}
						else if (skillData.type == SkillFrameDataType.Joystick_Position)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                int vx = (int) skillData.data1;
                                int vy = (int) skillData.data2;

								vx -= GlobalLogic.VALUE_1000;
								vy -= GlobalLogic.VALUE_1000;

								skill.UpdateJoystick(vx, vy);
                            }

                        }
                        else if (skillData.type == SkillFrameDataType.Joystick_Release)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                skill.ReleaseJoystick();
                            }
                        }
                        else if (skillData.type == SkillFrameDataType.Joystick_SpecialChoice)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                skill.specialChoice = (int)skillData.data1;
                            }
                            actor.UseSkill(skillID);
                        }
                        else if (skillData.type == SkillFrameDataType.Joystick_ForwardBackChoice)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                skill.specialChoice = (int) skillData.data1;
                            }
                            actor.UseSkill(skillID);
                        }
                        else if (skillData.type == SkillFrameDataType.Joystick_PlayerIndex)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                byte seat = (byte) skillData.data1;
                                BeActor selectActor = GetActorBySeat(seat);
                                if (selectActor != null)
                                {
                                    skill.joystickSelectActor = selectActor;
                                }
                            }
                            actor.UseSkill(skillID);
                        }
                        else if (skillData.type == SkillFrameDataType.Joystick_ActionIndex)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null)
                            {
                                skill.actionChoice = (int)skillData.data1;
                            }
                            actor.UseSkill(skillID);
                        }
                        else
                        {
							{
								var skill = actor.GetSkill(skillID);
								if (skill != null && skill.skillButtonState == BeSkill.SkillState.WAIT_FOR_NEXT_PRESS)
								{
                                    if(skill.pressMode == SkillPressMode.TWO_PRESS)
                                    {
                                        skill.PressAgain();
                                    }
                                    else if(skill.pressMode == SkillPressMode.PRESS_MANY_TWO)
                                    {
                                        skill.PressMany();
                                        return;
                                    }
									//return;
								}

								if (skill != null && skill.skillButtonState == BeSkill.SkillState.WAIT_FOR_NEXT_PRESS)
								{
									if (skill.pressMode == SkillPressMode.PRESS_AGAIN_CANCEL_OUT || 
										skill.pressMode == SkillPressMode.TWO_PRESS_OUT || 
										skill.pressMode == SkillPressMode.PRESS_MANY||
                                        skill.pressMode == SkillPressMode.PRESS_JOYSTICK)
									{
										if (actor.IsInPassiveState())
											return;

                                        if (skill.pressMode == SkillPressMode.PRESS_AGAIN_CANCEL_OUT)
                                            skill.PressAgainCancel();
                                        else if (skill.pressMode == SkillPressMode.TWO_PRESS_OUT)
                                            skill.PressAgainRelease();
                                        else if (skill.pressMode == SkillPressMode.PRESS_MANY)
                                            skill.PressMany();
                                        else if (skill.pressMode == SkillPressMode.PRESS_JOYSTICK)
                                            skill.PressJoystick();
                                        return;
									}
										
								}
							}
								
                            bool flag = false;
                            if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                            {
                                var skill = actor.GetSkill(skillID);
                                var curSkill = actor.GetCurrentSkill();
                                if (skill != null && skill.buttonState == ButtonState.RELEASE && null != curSkill && curSkill.skillData.IsAttackCombo != 1)
                                {
                                    if (actor.TriggerComboSkills(skillID))
                                        return;
                                }

                                if (skill != null && actor.GetCurSkillID()== skillID && skill.buttonState == ButtonState.RELEASE)
                                {
                                    skill.SetButtonPressAgain();
                                    actor.GetStateGraph().FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_PRESS_BUTTON_AGAIN);
                                    flag = true;
                                }
                            }
								
                            if (!flag)
							{
								//Logger.LogErrorFormat("skill id:{0}", skillID);
								actor.UseSkill(skillID);
							}
                                
                        }

                        
                    }
                    
                }

               
            }
        }

        public void Reset()
        {
            BaseReset();
            skillSolt = 0;
            skillSlotUp = 0;
        }             
    }

    public class UseItemFrameCommand : BaseFrameCommand, IFrameCommand
    {
        public UInt32 itemID;
        public UInt32 itemNum = 0;

        public FrameCommandID GetID()
        {
            return FrameCommandID.UseItem;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.UseItem,
                data2 = (UInt32)itemID,
                data3 = (UInt32)itemNum,

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat  = seat;
            itemID  = data.data2;
            itemNum = data.data3;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} item type :{2}, item num {3}", frame, seat, itemID, itemNum);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void ExecCommand()
        {
            BeActor actor = GetActorBySeat(seat);
            if (actor == null || actor.IsDead())
                return;

			if (actor != null && actor.IsProcessRecord())
			{
				Record(actor, GetString());
			}

            if (null != actor.buffController)
            {
                var item = TableManager.instance.GetTableItem<ProtoTable.ItemTable>((int)itemID);
                if (null != item && item.OnUseBuffId > 0)
                {
                    var buffData = TableManager.instance.GetTableItem<ProtoTable.BuffTable>(item.OnUseBuffId);
                    if (null != buffData)
                    {
                        int leftMS = GlobalLogic.VALUE_1000;

                        if (buffData.ValueA.Count > 0)
                        {
                            leftMS = TableManager.GetValueFromUnionCell(buffData.ValueA[0], 0);
                        }

                        if (0 == leftMS)
                        {
                            leftMS = GlobalLogic.VALUE_1000;
                        }

						BeBuff buff = actor.buffController.TryAddBuff(buffData.ID, leftMS);
                        if (null != buff)
                        {
                            buff.passive = true;
                        }

                        if (null != actor.m_pkGeActor && Utility.IsStringValid(buffData.BirthEffectLocate))
                        {
                            DAssetObject asset;

                            asset.m_AssetObj = null;
                            asset.m_AssetPath = buffData.BirthEffect;

                            EffectsFrames effectInfo = new EffectsFrames();
                            effectInfo.localPosition = new Vector3(0, 0, 0);

                            effectInfo.timetype = EffectTimeType.BUFF;

                            if (Utility.IsStringValid(buffData.BirthEffectLocate))
                            {
                                effectInfo.attachname = buffData.BirthEffectLocate;
                            }

                            actor.m_pkGeActor.CreateEffect(asset, effectInfo, 0, new Vec3(0, 0, 0), 1f, 1f, false);
                        }
                    }
                }
            }
        }
        public void Reset()
        {
            BaseReset();
            itemID = 0;
            itemNum = 0;
        }
    }

    public class RebornFrameCommand : BaseFrameCommand, IFrameCommand
    {
        /// <summary>
        /// 复活的目标
        /// </summary>
        public byte targetSeat;
       
        public FrameCommandID GetID()
        {
            return FrameCommandID.Reborn;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            BeActor actor = GetActorBySeat(targetSeat);
            if (actor != null)
            {
				if (actor != null && actor.IsProcessRecord())
                {
                    Record(actor, GetString());
                }

                if (!actor.IsDead())
                {
                    Logger.LogErrorFormat("cmd:reborn frame:{0} targetSeat:{1} actor is not dead!", frame, targetSeat);
                }
                IOnExecCommand onExecCmd = battle as IOnExecCommand;
                if (null != onExecCmd)
                {
                    onExecCmd.AfterExecFrameCommand(seat, this);
                }
                actor.Reborn();

                Logger.LogProcessFormat("[复活帧命令] {0} {1}", seat, targetSeat);
#if !LOGIC_SERVER
                if (seat != targetSeat)
                {
                    BattlePlayer mainPlayer = battle.dungeonPlayerManager.GetMainPlayer();
                    BattlePlayer originPlayer = battle.dungeonPlayerManager.GetPlayerBySeat(seat);
                    BattlePlayer targetPlayer = battle.dungeonPlayerManager.GetPlayerBySeat(targetSeat);

                    //  1.复活队友：义薄云天，你复活了队友XXX，XXX对你感激涕零						
                    //  2.被队友复活：感动苍天，队友XXX复活了你，给他一个友谊之吻吧！						
                    //  3.第三者：情义感人，队友XXX复活了队友XXXXX，赶紧抱紧大腿吧！						

                    if (targetPlayer.playerInfo.seat == mainPlayer.playerInfo.seat)
                    {
                        // 2.被队友复活：感动苍天，队友XXX复活了你，给他一个友谊之吻吧！						
                        SystemNotifyManager.SystemNotify(8101, originPlayer.playerInfo.name);
                    }
                    else if (originPlayer.playerInfo.seat == mainPlayer.playerInfo.seat)
                    {
                        //  1.复活队友：义薄云天，你复活了队友XXX，XXX对你感激涕零						
                        SystemNotifyManager.SystemNotify(8102, targetPlayer.playerInfo.name);
                    }
                    else 
                    {
                        //  3.第三者：情义感人，队友XXX复活了队友XXXXX，赶紧抱紧大腿吧！						
                        SystemNotifyManager.SystemNotify(8103, originPlayer.playerInfo.name, targetPlayer.playerInfo.name);
                    }
                }  
#endif
            }
            else
            {
                Logger.LogErrorFormat("cmd:reborn frame:{0} targetSeat:{1} actor is null!", frame, targetSeat);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.Reborn,
                data2 = (uint)targetSeat,
                data3 = 0
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.targetSeat = (byte)data.data2;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} Reborn targetSeat:{2}", frame, seat, targetSeat);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void Reset()
        {
            BaseReset();
            targetSeat = 0;
        }
    }

    public class ReconnectFrameCommand : BaseFrameCommand, IFrameCommand
    {
        public FrameCommandID GetID()
        {
            return FrameCommandID.ReconnectEnd;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }
            if(isValid())
            {
                BattlePlayer mainPlayer = battle.dungeonPlayerManager.GetMainPlayer();
                BattlePlayer players = battle.dungeonPlayerManager.GetPlayerBySeat(seat);


                if (null != players && null != mainPlayer)
                {
					var actor = players.playerActor;

					if (actor != null && actor.IsProcessRecord())
					{
						Record(players.playerActor, "reconnect" + GetString());
					}

					if (actor != null && actor.IsProcessRecord())
                    {
                        actor.GetRecordServer().RecordProcess("[BATTLE] {0} reconnect", players.playerInfo.name);
                        actor.GetRecordServer().MarkString(0x12980285, players.playerInfo.name);
                        // Mark:0x12980285 [BATTLE] {0} reconnect
                    }

#if !LOGIC_SERVER

                    if (mainPlayer.playerInfo.seat == seat)
                    {
                        // 您回来了哟
                        SystemNotifyManager.SystemNotify(8304);
                    }
                    else
                    {
                        // whowho 回来了哟
                        SystemNotifyManager.SystemNotify(8302, players.playerInfo.name);
                    }

 #endif

                }
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }

            //if (BattleSceneMain.GetInstance().GetSceneType() == EBattleSceneType.TeamDungeon)
            {
                //List<BattlePlayer> players = BattleSceneMain.GetInstance().GetBattlePlayers();
                //int index = players.FindIndex((data) => { return data.playerInfo.seat == seat; });
                //if (index >= 0 && index < players.Count)
                //{
                //    players[index].playerActor.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
                //    players[index].playerActor.m_iRemoveTime = 0;
                //    players.RemoveAt(index);
                //}

                //// TODO
                //int index2 = BattleDataManager.GetInstance().DungeonTeamMembers.FindIndex((data) => { return data.seat == seat; });
                //if (index2 >= 0 && index2 < BattleDataManager.GetInstance().DungeonTeamMembers.Count)
                //{
                //    BattleDataManager.GetInstance().DungeonTeamMembers.RemoveAt(index2);
                //}
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.ReconnectEnd,
                data2 = 0,
                data3 = 0,

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} reconnect", frame, seat);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void Reset()
        {
            BaseReset();
        }
    }


	public class LevelChangeCommand : BaseFrameCommand, IFrameCommand
	{
		public int newLevel;

		public FrameCommandID GetID()
		{
			return FrameCommandID.LevelChange;
		}

		public UInt32 GetFrame()
		{
			return frame;
		}

		public void ExecCommand()
		{
			BeActor actor = GetActorBySeat(seat);
			if (actor == null || actor.IsDead())
				return;

			#if !LOGIC_SERVER
			if (RecordServer.GetInstance().IsProcessRecord())
			{
				Record(actor, GetString());
			}
			#endif

			actor.LevelUpTo(newLevel);
		}

		public _inputData GetInputData()
		{
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.LevelChange,
                data2 = (uint)newLevel,
                data3 = 0,

                sendTime = sendTime
            };

            return data;
		}


		public void SetValue(UInt32 frame, byte seat, _inputData data)
		{
			this.frame = frame;
			this.seat = seat;
			this.newLevel = (int)data.data2;
		}

		public string GetString()
		{
			StringBuilder builder = StringBuilderCache.Acquire();
			builder.Clear();
			builder.AppendFormat("Frame:{0} Seat:{1} level change=>{2}", frame, seat, newLevel);
			string str = builder.ToString();
			StringBuilderCache.Release(builder);
			return str;
		}

        public void Reset()
        {
            BaseReset();
            newLevel = 0;
        }
    }

    public class LeaveFrameCommand : BaseFrameCommand, IFrameCommand
    {
        public FrameCommandID GetID()
        {
            return FrameCommandID.Leave;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {

            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            {
                if (battle == null || battle.dungeonPlayerManager == null) return;
                BattlePlayer mainPlayer = battle.dungeonPlayerManager.GetMainPlayer();
                BattlePlayer players = battle.dungeonPlayerManager.GetPlayerBySeat(seat);

                if (null != players && null != mainPlayer)
                {

					var actor = players.playerActor;

					if (actor != null && actor.IsProcessRecord())
					{
						Record(players.playerActor, GetString());
					}


					if (actor != null && actor.IsProcessRecord())
                    {
						actor.GetRecordServer().RecordProcess("[BATTLE] {0} leave", players.playerInfo.name);
                        actor.GetRecordServer().MarkString(0x7779802, players.playerInfo.name);
                        // Mark:0x7779802 [BATTLE] {0} leave
                    }

#if !LOGIC_SERVER

                    if (mainPlayer.playerInfo.seat == seat)
                    {
                        // 您离开了哟
                        //SystemNotifyManager.SystemNotify(8303);
                    }
                    else
                    {
                        // whowho 离开了哟
                        SystemNotifyManager.SystemNotify(8301, players.playerInfo.name);
                    }

 #endif

                }
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.Leave,
                data2 = 0,
                data3 = 0,

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} Leave", frame, seat);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void Reset()
        {
            BaseReset();
        }
    }
    public class DoublePressConfigCommand : BaseFrameCommand, IFrameCommand
	{
		public bool hasDoublePress = false;//是否是双击摇杆操作
        public bool hasRunAttackConfig = false;//是否有前冲配置
        public bool attackReplaceLigui = false;//普攻替换里鬼
        public bool canUseCrystalSkill = false;//自动战斗能否使用使用40级无色技能
        public bool paladinAttackCharge = false;//驱魔师普攻是否蓄力
        public bool backHitConfig = false;
        public bool autoHitConfig = false;
        public byte chaserSwitch; 

        public FrameCommandID GetID()
		{
			return FrameCommandID.DoublePressConfig;
		}
        
		public UInt32 GetFrame()
		{
			return frame;
		}

		public void ExecCommand()
		{
			BeActor actor = GetActorBySeat(seat);
			if (actor != null && !actor.IsDead())
			{
				if (actor.IsProcessRecord())
				{
					Record(actor, GetString());
				}

                var ret = actor.TriggerEventNew(BeEventType.RaidBattleChangeMonster,new EventParam(){m_Obj = actor});
                BeActor curActor = (BeActor) ret.m_Obj;
                
                if(curActor != null)
                {
                    curActor.recieveConfigCmd = true;
                    curActor.hasDoublePress = hasDoublePress;
                    curActor.hasRunAttackConfig = hasRunAttackConfig;
                    curActor.attackReplaceLigui = attackReplaceLigui;
                    curActor.canUseCrystalSkill = canUseCrystalSkill;

                    curActor.paladinAttackCharge = paladinAttackCharge;
                    curActor.backHitConfig = backHitConfig;
                    curActor.autoHitConfig = autoHitConfig;
                    curActor.chaserSwitch = chaserSwitch;
                    curActor.TriggerEventNew(BeEventType.ConfigCommand);
                }
            }
		}

		public _inputData GetInputData()
		{
            _inputData data = new _inputData();

            data.data1 = (uint)FrameCommandID.DoublePressConfig;
            data.data2 |= (uint)(hasDoublePress ? 1 : 0)<<1;
            data.data2 |= (uint)(attackReplaceLigui ? 1 : 0) << 2;
			data.data2 |= (uint)(canUseCrystalSkill ? 1 : 0) << 3;
            data.data2 |= (uint)(paladinAttackCharge ? 1 : 0) << 4;
            data.data2 |= (uint)(backHitConfig ? 1 : 0) << 5;
            data.data2 |= (uint)(autoHitConfig ? 1 : 0) << 6;
            data.data3 |= (uint)(hasRunAttackConfig ? 1 : 0);
            data.data3 |= (uint) (chaserSwitch << 1); 
            data.sendTime = sendTime;

            return data;
		}

		public void SetValue(UInt32 frame, byte seat, _inputData data)
		{
			this.frame = frame;
			this.seat = seat;

            this.hasDoublePress = (data.data2 & (1 << 1)) == 0 ? false : true;
            this.attackReplaceLigui = (data.data2 & (1 << 2)) == 0 ? false : true;
			this.canUseCrystalSkill = (data.data2 & (1 << 3)) == 0 ? false : true;
            this.paladinAttackCharge = (data.data2 & (1 << 4)) == 0 ? false : true;
            this.backHitConfig = (data.data2 & (1 << 5)) == 0 ? false : true;
            this.autoHitConfig = (data.data2 & (1 << 6)) == 0 ? false : true;
            this.hasRunAttackConfig = (data.data3 & 1 ) == 0 ? false : true;
            this.chaserSwitch = (byte) ((data.data3 & ((32-1) << 1)) >> 1);//5位数据
        }

		public string GetString()
		{
			StringBuilder builder = StringBuilderCache.Acquire();
			builder.Clear();
			builder.AppendFormat("Frame:{0} Seat:{1} hasDoublePress:{2} hasRunAttackConfig:{3} autoUseCrystalSkill: {4}", frame, seat, hasDoublePress, hasRunAttackConfig, canUseCrystalSkill);
			string str = builder.ToString();
			StringBuilderCache.Release(builder);
			return str;
        }
        public void Reset()
        {
            BaseReset();
            hasDoublePress = false;
            hasRunAttackConfig = false;
            attackReplaceLigui = false;
        }
    }

    public class StopSkillCommand : BaseFrameCommand, IFrameCommand
    {
        public int skillID = 0;
        public FrameCommandID GetID()
        {
            return FrameCommandID.StopSkill;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            BeActor actor = GetActorBySeat(seat);
            if (skillID != 0 && actor != null && !actor.IsDead())
            {
                actor.CancelSkill(skillID);
                actor.Locomote(new BeStateData((int)ActionState.AS_IDLE));
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.StopSkill,
                data2 = (uint)skillID,
                data3 = 0
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.skillID = (int)data.data2;
            //this.openAutoFight = data.data2 == 0 ? false : true;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} stop skillid:{2}", frame, seat, skillID);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void Reset()
        {
            BaseReset();
            skillID = 0;
        }
    }

    public class DoAttackCommand : BaseFrameCommand, IFrameCommand
    {
        public int skillID; //技能
        public int bulletCount; //当前子弹数量
        public int pid;    //要攻击的怪物pid
        public FrameCommandID GetID()
        {
            return FrameCommandID.DoAttack;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;
//             if(GetFrame() == 7737u)
//             {
//                 Logger.LogErrorFormat("ExecCommand : {0}", skillID);
//             }
            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            BeActor actor = GetActorBySeat(seat);
            if (actor != null)
            {
                //actor.TriggerEvent(BeEventType.onSyncDungeonOperation, new object[] { skillID, bulletCount, pid });
                actor.TriggerEventNew(BeEventType.onSyncDungeonOperation, new EventParam{m_Int = skillID, m_Int2 = bulletCount, m_Int3 = pid});
                var skill = actor.GetSkill(skillID);
                if (skill != null)
                {
                    try
                    {

                        //尼尔狙击
                        Skill1310 skill1310 = skill as Skill1310;
                        if (skill1310 != null)
                        {
                            skill1310.DoRealAttack(bulletCount, pid);

                        }
                        /*Skill1406 skill1406 = skill as Skill1406;
                        if (skill1406 != null)
                        {
                            skill1406.DoRealAttack(bulletCount, pid);
                        }*/
                    }
                    catch (System.Exception e)
                    {
                        Logger.LogErrorFormat("DoAttackCommand skill id:{0} bulletCnt:{1} pid:{2} error:{3}", skillID, bulletCount, pid, e.ToString());
                    }
                }
            }
                
            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.DoAttack,
                data2 = (uint)skillID,
                data3 = (uint)(pid + 10000 * bulletCount)
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.skillID = (int)data.data2;
            this.pid = (int)data.data3 % 10000;
            this.bulletCount = ((int)data.data3 - this.pid) / 10000;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} doattack skillid:{2} pid:{3} bullet:{4}", frame, seat, skillID, pid, bulletCount);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void Reset()
        {
            BaseReset();
            skillID = 0;
            pid = 0;
        }
    }

    public class DoSyncSightCommand : BaseFrameCommand, IFrameCommand
    {
        public int skillID; //技能
        public int x;       //瞄准镜的X轴坐标
        public int z;       //瞄准镜的Z轴坐标
        public FrameCommandID GetID()
        {
            return FrameCommandID.SyncSight;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            BeActor actor = GetActorBySeat(seat);
            if (actor != null)
            {
                var skill = actor.GetSkill(skillID);
                if (skill != null)
                {
                    try
                    {
                        Skill1310 skill1310 = skill as Skill1310;
                        if (skill1310 != null)
                        {
                            skill1310.DoSyncSight(x, z);
                        }
                        
                        /*Skill1406 skill1406 = skill as Skill1406;
                        if (skill1406 != null)
                        {
                            skill1406.DoSyncSight(x,z);
                        }*/
                    }
                    catch (System.Exception e)
                    {
                        Logger.LogErrorFormat("DoSyncSightCommand skill id:{0} X:{1} Z:{2} error:{3}", skillID, x, z, e.ToString());
                    }
                }
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.SyncSight,
                data2 = (uint)skillID,
                data3 = (uint)(x + 100000 * z),
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.skillID = (int)data.data2;
            this.x = (int)data.data3 % 100000;
            this.z = ((int)data.data3 - this.x) / 100000;  
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} DoSyncSightCommand skillid:{2} x:{3} z:{4}", frame, seat, skillID, x, z);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void Reset()
        {
            BaseReset();
            skillID = 0;
            x = 0;
            z = 0;
        }
    }



    public class AutoFightCommand : BaseFrameCommand, IFrameCommand
	{
		public bool openAutoFight;
		public FrameCommandID GetID()
		{
			return FrameCommandID.AutoFight;
		}

		public UInt32 GetFrame()
		{
			return frame;
		}

		public void ExecCommand()
		{
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

			BeActor actor = GetActorBySeat(seat);
			if (actor != null && !actor.IsDead())
            {
				actor.SetAutoFight(openAutoFight);
            }
            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
		}

		public _inputData GetInputData()
		{
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.AutoFight,
                data2 = (uint)(openAutoFight ? 1 : 0),
                data3 = 0,
                sendTime = sendTime
            };

            return data;
		}

		public void SetValue(UInt32 frame, byte seat, _inputData data)
		{
			this.frame = frame;
			this.seat = seat;
			this.openAutoFight = data.data2==0?false:true;
		}

		public string GetString()
		{
			StringBuilder builder = StringBuilderCache.Acquire();
			builder.Clear();
			builder.AppendFormat("Frame:{0} Seat:{1} autofight:{2}", frame, seat, openAutoFight);
			string str = builder.ToString();
			StringBuilderCache.Release(builder);
			return str;
        }
        public void Reset()
        {
            BaseReset();
            openAutoFight = false;
        }
    }


    public class ChangeWeaponCommand : BaseFrameCommand, IFrameCommand
    {
        public int weaponIndex = 0;

        /// <summary>
        /// 装备序号
        /// </summary>
        public int equipIndex = 0;

        public FrameCommandID GetID()
        {
            return FrameCommandID.ChangeWeapon;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            BeActor actor = GetActorBySeat(seat);
            if (actor != null && !actor.IsDead())
            {
                if (equipIndex > 0)
                {
                    actor.ChangeEquip(equipIndex - 1);
                }
                else
                {
                    actor.ChangeWeapon(weaponIndex);
                }
            }
                
            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.ChangeWeapon,
                data2 = (uint)weaponIndex,
                data3 = (uint)equipIndex,
                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.weaponIndex = (int)data.data2;
            this.equipIndex = (int)data.data3;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} weapon index:{2} equipIndex:{3}", frame, seat, weaponIndex, equipIndex);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }
        public void Reset()
        {
            BaseReset();
            weaponIndex = 0;
            equipIndex = 0;
        }
    }



    public class PassDoorCommand : BaseFrameCommand, IFrameCommand
    {
        public FrameCommandID GetID()
        {
            return FrameCommandID.PassDoor;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            BeActor actor = GetActorBySeat(seat);
            if (actor == null || actor.IsDead())
                return;

            if (actor != null && actor.IsProcessRecord())
            {
                Record(actor, GetString());
            }
            actor.Reset();
            actor.SetAttackButtonState(ButtonState.RELEASE);
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (UInt32)FrameCommandID.PassDoor,

                sendTime = sendTime
            };
            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} PassDoor !!", frame, seat);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
        }
    }

    /// <summary>
    /// 上报服务器关卡阶段改变，此同步帧客户端不做处理
    /// </summary>
    public class BossPhaseChange : BaseFrameCommand, IFrameCommand
    {
        public int phase = -1;
        public FrameCommandID GetID()
        {
            return FrameCommandID.BossPhaseChange;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }


            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.BossPhaseChange,
                data2 = (uint)phase,

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.phase = (int)data.data2;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} BossPhaseChange phase:{2}", frame, seat, phase);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            phase = -1;
        }
    }

    /// <summary>
    /// 服务器下发附属关卡通关，改变boss状态
    /// </summary>
    public class DungeonDestory : BaseFrameCommand, IFrameCommand
    {
        public int dungeonID = -1;
        public FrameCommandID GetID()
        {
            return FrameCommandID.DungeonDestory;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }
            var raidBattle = battle as RaidBattle;
            if (raidBattle != null && dungeonID != -1) 
            {
                raidBattle.DungeonDestroyNotify(dungeonID);
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.DungeonDestory,
                data2 = (uint)dungeonID,
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.dungeonID = (int)data.data2;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} DungeonDestory dungeonID:{2}", frame, seat, dungeonID);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            dungeonID = -1;
        }
    }

    /// <summary>
    /// 团本比赛结束,服务器主动下发的副本结束协议，客户端不上报
    /// </summary>
    public class TeamCopyRaceEnd : BaseFrameCommand, IFrameCommand
    {
        public int reason = -1;
        public FrameCommandID GetID()
        {
            return FrameCommandID.TeamCopyRaceEnd;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {

            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            ClientReconnectManager.instance.canReconnectRelay = false;

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
#if !SERVER_LOGIC
            //游戏结束！！！！！，如果是录像回放就在这里打开界面
            if (ReplayServer.GetInstance().IsReplay())
            {
                ReplayServer.GetInstance().Stop(true, "TeamCopyRaceEndCmd");
            }
#endif
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.TeamCopyRaceEnd,
                data2 = (uint)reason,

                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.reason = (int)data.data2;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} TeamCopyRaceEnd reason:{2}", frame, seat, reason);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            reason = -1;
        }
    }

    /// <summary>
    /// 团本地下城进度
    /// </summary>
    public class DungeonProcess : BaseFrameCommand, IFrameCommand
    {
        public int dungeonID = -1;
        public int process = 0;//千分比
        public FrameCommandID GetID()
        {
            return FrameCommandID.TeamCopyBimsProgress;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }
            var raidBattle = battle as RaidBattle;
            if (raidBattle != null && dungeonID != -1)
            {
                raidBattle.SetDungeonRecoverProcess(dungeonID, process);
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.TeamCopyBimsProgress,
                data2 = (uint)process,
                data3 = (uint)dungeonID,
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            this.process = (int)data.data2;
            this.dungeonID = (int)data.data3;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} DungeonProcess dungeonID:{2} process:{3}", frame, seat, dungeonID, process);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            dungeonID = -1;
            process = 0;
        }
    }
    
    public class CloseFilmCommand : BaseFrameCommand, IFrameCommand
    {
        public bool closeDialog = false;

        public FrameCommandID GetID()
        {
            return FrameCommandID.CloseFilm;
        }

        public UInt32 GetFrame()
        {
            return frame;
        }

        public void ExecCommand()
        {
            IOnExecCommand onExecCmd = battle as IOnExecCommand;

            if (null != onExecCmd)
            {
                onExecCmd.BeforeExecFrameCommand(seat, this);
            }

            if (battle != null)
            {
                if (closeDialog)
                {
                    battle.CloseDialog();
                }
                else
                {
                    //battle.CloseFilm(true);
                }
            }

            if (null != onExecCmd)
            {
                onExecCmd.AfterExecFrameCommand(seat, this);
            }
        }

        public _inputData GetInputData()
        {
            _inputData data = new _inputData
            {
                data1 = (uint)FrameCommandID.CloseFilm,
                data2 = (uint)(closeDialog ? 1 : 0),
                data3 = 0,
                sendTime = sendTime
            };

            return data;
        }

        public void SetValue(UInt32 frame, byte seat, _inputData data)
        {
            this.frame = frame;
            this.seat = seat;
            closeDialog = data.data2 == 1;
        }

        public string GetString()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();
            builder.AppendFormat("Frame:{0} Seat:{1} closeDialog:{2} ", frame, seat, closeDialog);
            string str = builder.ToString();
            StringBuilderCache.Release(builder);
            return str;
        }

        public void Reset()
        {
            BaseReset();
            closeDialog = false;
        }
    }
}
