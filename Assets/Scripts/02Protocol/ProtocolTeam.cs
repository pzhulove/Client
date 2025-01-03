using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  队伍目标类型
	/// </summary>
	public enum TeamTargetType
	{
		/// <summary>
		///  地下城
		/// </summary>
		Dungeon = 0,
	}

	/// <summary>
	///  队员属性
	/// </summary>
	public enum TeamMemberProperty
	{
		/// <summary>
		///  等级
		/// </summary>
		Level = 0,
		/// <summary>
		///  公会ID
		/// </summary>
		GuildID = 1,
		/// <summary>
		///  剩余次数
		/// </summary>
		RemainTimes = 2,
		/// <summary>
		///  职业
		/// </summary>
		Occu = 3,
		/// <summary>
		///  状态
		/// </summary>
		StatusMask = 4,
		/// <summary>
		///  vip等级
		/// </summary>
		VipLevel = 5,
		/// <summary>
		///  抗磨值
		/// </summary>
		ResistMagic = 6,
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		PlayerLabelInfo = 7,
	}

	/// <summary>
	///  成员状态掩码
	/// </summary>
	public enum TeamMemberStatusMask
	{
		/// <summary>
		///  是否在线
		/// </summary>
		Online = 1,
		/// <summary>
		///  准备
		/// </summary>
		Ready = 2,
		/// <summary>
		///  助战
		/// </summary>
		Assist = 4,
		/// <summary>
		///  是否在战斗中
		/// </summary>
		Racing = 8,
	}

	/// <summary>
	///  队伍选项
	/// </summary>
	public enum TeamOption
	{
		/// <summary>
		///  深渊自动关闭
		/// </summary>
		HellAutoClose = 1,
	}

	/// <summary>
	///  队伍选项修改类型
	/// </summary>
	public enum TeamOptionOperType
	{
		/// <summary>
		///  目标
		/// </summary>
		Target = 0,
		/// <summary>
		///  自动同意
		/// </summary>
		AutoAgree = 1,
		/// <summary>
		///  深渊自动关闭功能
		/// </summary>
		HellAutoClose = 2,
	}

	/// <summary>
	///  队伍成员基本信息
	/// </summary>
	public class TeammemberBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// id
		/// </summary>
		public UInt64 id;
		/// <summary>
		/// 名字
		/// </summary>
		public string name;
		/// <summary>
		/// 等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		/// 职业
		/// </summary>
		public byte occu;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// level
				_len += 2;
				// occu
				_len += 1;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  创建队伍
	/// </summary>
	[Protocol]
	public class WorldCreateTeam : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601610;
		public UInt32 Sequence;
		/// <summary>
		///  队伍目标
		/// </summary>
		public UInt32 target;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, target);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref target);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, target);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref target);
			}

			public int getLen()
			{
				int _len = 0;
				// target
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  创建队伍返回
	/// </summary>
	[Protocol]
	public class WorldCreateTeamRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601627;
		public UInt32 Sequence;
		/// <summary>
		///  返回码(ErrorCode)
		/// </summary>
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  加入队伍返回
	/// </summary>
	[Protocol]
	public class WorldJoinTeamRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601628;
		public UInt32 Sequence;
		/// <summary>
		///  返回码(ErrorCode)
		/// </summary>
		public UInt32 result;
		public byte inTeam;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, inTeam);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref inTeam);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, inTeam);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref inTeam);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// inTeam
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  队伍基础信息
	/// </summary>
	public class TeamBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  队伍编号
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		///  队伍目标
		/// </summary>
		public UInt32 target;
		/// <summary>
		///  队长信息
		/// </summary>
		public TeammemberBaseInfo masterInfo = new TeammemberBaseInfo();
		/// <summary>
		///  成员数量
		/// </summary>
		public byte memberNum;
		/// <summary>
		///  成员上限
		/// </summary>
		public byte maxMemberNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, target);
				masterInfo.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, memberNum);
				BaseDLL.encode_int8(buffer, ref pos_, maxMemberNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref target);
				masterInfo.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref memberNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxMemberNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, target);
				masterInfo.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, memberNum);
				BaseDLL.encode_int8(buffer, ref pos_, maxMemberNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref target);
				masterInfo.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref memberNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxMemberNum);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// target
				_len += 4;
				// masterInfo
				_len += masterInfo.getLen();
				// memberNum
				_len += 1;
				// maxMemberNum
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  队伍成员信息
	/// </summary>
	public class TeammemberInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// id
		/// </summary>
		public UInt64 id;
		/// <summary>
		/// 名字
		/// </summary>
		public string name;
		/// <summary>
		/// 等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		/// 职业
		/// </summary>
		public byte occu;
		/// <summary>
		///  状态掩码（对应枚举TeamMemberStatusMask）
		/// </summary>
		public byte statusMask;
		/// <summary>
		///  外观
		/// </summary>
		public PlayerAvatar avatar = new PlayerAvatar();
		/// <summary>
		///  剩余次数
		/// </summary>
		public UInt32 remainTimes;
		/// <summary>
		///  公会ID
		/// </summary>
		public UInt64 guildId;
		/// <summary>
		///  vip等级
		/// </summary>
		public byte vipLevel;
		/// <summary>
		///  抗魔值
		/// </summary>
		public UInt32 resistMagic;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, statusMask);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, remainTimes);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, resistMagic);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref statusMask);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainTimes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref resistMagic);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, statusMask);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, remainTimes);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, resistMagic);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref statusMask);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainTimes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref resistMagic);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// level
				_len += 2;
				// occu
				_len += 1;
				// statusMask
				_len += 1;
				// avatar
				_len += avatar.getLen();
				// remainTimes
				_len += 4;
				// guildId
				_len += 8;
				// vipLevel
				_len += 1;
				// resistMagic
				_len += 4;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步队伍信息
	/// </summary>
	[Protocol]
	public class WorldSyncTeamInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601601;
		public UInt32 Sequence;
		/// <summary>
		///  队伍编号
		/// </summary>
		public UInt16 id;
		/// <summary>
		///  队伍目标
		/// </summary>
		public UInt32 target;
		/// <summary>
		///  是否自动同意
		/// </summary>
		public byte autoAgree;
		/// <summary>
		///  队长
		/// </summary>
		public UInt64 master;
		/// <summary>
		///  队伍成员链表
		/// </summary>
		public TeammemberInfo[] members = new TeammemberInfo[0];
		/// <summary>
		/// 队伍选项(对应TeamOption的位组合)
		/// </summary>
		public UInt32 options;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, target);
				BaseDLL.encode_int8(buffer, ref pos_, autoAgree);
				BaseDLL.encode_uint64(buffer, ref pos_, master);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, options);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref target);
				BaseDLL.decode_int8(buffer, ref pos_, ref autoAgree);
				BaseDLL.decode_uint64(buffer, ref pos_, ref master);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new TeammemberInfo[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new TeammemberInfo();
					members[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref options);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, target);
				BaseDLL.encode_int8(buffer, ref pos_, autoAgree);
				BaseDLL.encode_uint64(buffer, ref pos_, master);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, options);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref target);
				BaseDLL.decode_int8(buffer, ref pos_, ref autoAgree);
				BaseDLL.decode_uint64(buffer, ref pos_, ref master);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new TeammemberInfo[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new TeammemberInfo();
					members[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref options);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 2;
				// target
				_len += 4;
				// autoAgree
				_len += 1;
				// master
				_len += 8;
				// members
				_len += 2;
				for(int j = 0; j < members.Length; j++)
				{
					_len += members[j].getLen();
				}
				// options
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知新成员加入
	/// </summary>
	[Protocol]
	public class WorldNotifyNewTeamMember : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601602;
		public UInt32 Sequence;
		/// <summary>
		///  队员信息
		/// </summary>
		public TeammemberInfo info = new TeammemberInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求离开队伍
	/// </summary>
	[Protocol]
	public class WorldLeaveTeamReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601603;
		public UInt32 Sequence;
		/// <summary>
		///  踢人或自己
		/// </summary>
		public UInt64 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知成员离开
	/// </summary>
	[Protocol]
	public class WorldNotifyMemberLeave : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601604;
		public UInt32 Sequence;
		/// <summary>
		///  队员ID
		/// </summary>
		public UInt64 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知成员上下线
	/// </summary>
	[Protocol]
	public class WorldSyncTeamMemberStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601605;
		public UInt32 Sequence;
		/// <summary>
		///  队员ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  状态掩码（对应枚举TeamMemberStatusMask）
		/// </summary>
		public byte statusMask;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, statusMask);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref statusMask);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, statusMask);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref statusMask);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// statusMask
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  队伍如果有密码发起请求密码
	/// </summary>
	[Protocol]
	public class WorldTeamPasswdReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601612;
		public UInt32 Sequence;
		/// <summary>
		/// 目标
		/// </summary>
		public UInt64 target;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, target);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref target);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, target);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref target);
			}

			public int getLen()
			{
				int _len = 0;
				// target
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置队伍属性
	/// </summary>
	[Protocol]
	public class WorldSetTeamOption : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601625;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型（TeamOptionOperType）
		/// </summary>
		public byte type;
		/// <summary>
		///  下面不同情况下代表不同的意义
		/// </summary>
		public string str;
		public UInt32 param1;
		public UInt32 param2;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] strBytes = StringHelper.StringToUTF8Bytes(str);
				BaseDLL.encode_string(buffer, ref pos_, strBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 strLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strLen);
				byte[] strBytes = new byte[strLen];
				for(int i = 0; i < strLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref strBytes[i]);
				}
				str = StringHelper.BytesToString(strBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] strBytes = StringHelper.StringToUTF8Bytes(str);
				BaseDLL.encode_string(buffer, ref pos_, strBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 strLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strLen);
				byte[] strBytes = new byte[strLen];
				for(int i = 0; i < strLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref strBytes[i]);
				}
				str = StringHelper.BytesToString(strBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// str
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(str);
					_len += 2 + _strBytes.Length;
				}
				// param1
				_len += 4;
				// param2
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步队伍属性
	/// </summary>
	[Protocol]
	public class WorldSyncTeamOption : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601626;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型（TeamOptionOperType）
		/// </summary>
		public byte type;
		/// <summary>
		///  下面不同情况下代表不同的意义
		/// </summary>
		public string str;
		public UInt32 param1;
		public UInt32 param2;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] strBytes = StringHelper.StringToUTF8Bytes(str);
				BaseDLL.encode_string(buffer, ref pos_, strBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 strLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strLen);
				byte[] strBytes = new byte[strLen];
				for(int i = 0; i < strLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref strBytes[i]);
				}
				str = StringHelper.BytesToString(strBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] strBytes = StringHelper.StringToUTF8Bytes(str);
				BaseDLL.encode_string(buffer, ref pos_, strBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 strLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strLen);
				byte[] strBytes = new byte[strLen];
				for(int i = 0; i < strLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref strBytes[i]);
				}
				str = StringHelper.BytesToString(strBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// str
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(str);
					_len += 2 + _strBytes.Length;
				}
				// param1
				_len += 4;
				// param2
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求转让队长
	/// </summary>
	[Protocol]
	public class WorldTransferTeammaster : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601608;
		public UInt32 Sequence;
		/// <summary>
		///  队员ID
		/// </summary>
		public UInt64 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步新队长
	/// </summary>
	[Protocol]
	public class WorldSyncTeammaster : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601609;
		public UInt32 Sequence;
		/// <summary>
		///  队长ID
		/// </summary>
		public UInt64 master;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, master);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref master);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, master);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref master);
			}

			public int getLen()
			{
				int _len = 0;
				// master
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  解散队伍
	/// </summary>
	[Protocol]
	public class WorldDismissTeam : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601611;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  查询队伍列表
	/// </summary>
	[Protocol]
	public class WorldQueryTeamList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601623;
		public UInt32 Sequence;
		/// <summary>
		///  根据队伍编号搜索
		/// </summary>
		public UInt16 teamId;
		/// <summary>
		///  队伍目标
		/// </summary>
		public UInt32 targetId;
		/// <summary>
		///  根据地下城搜索
		/// </summary>
		public UInt32[] targetList = new UInt32[0];
		/// <summary>
		///  查询起始位置
		/// </summary>
		public UInt16 startPos;
		/// <summary>
		///  请求个数
		/// </summary>
		public byte num;
		/// <summary>
		///  参数
		/// </summary>
		public byte param;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, targetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)targetList.Length);
				for(int i = 0; i < targetList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, targetList[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, startPos);
				BaseDLL.encode_int8(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
				UInt16 targetListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetListCnt);
				targetList = new UInt32[targetListCnt];
				for(int i = 0; i < targetList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref targetList[i]);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref startPos);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, targetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)targetList.Length);
				for(int i = 0; i < targetList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, targetList[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, startPos);
				BaseDLL.encode_int8(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
				UInt16 targetListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetListCnt);
				targetList = new UInt32[targetListCnt];
				for(int i = 0; i < targetList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref targetList[i]);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref startPos);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 2;
				// targetId
				_len += 4;
				// targetList
				_len += 2 + 4 * targetList.Length;
				// startPos
				_len += 2;
				// num
				_len += 1;
				// param
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回队伍列表
	/// </summary>
	[Protocol]
	public class WorldQueryTeamListRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601624;
		public UInt32 Sequence;
		/// <summary>
		///  队伍目标
		/// </summary>
		public UInt32 targetId;
		public TeamBaseInfo[] teamList = new TeamBaseInfo[0];
		public UInt16 pos;
		public UInt16 maxNum;
		/// <summary>
		///  参数
		/// </summary>
		public byte param;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, targetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamList.Length);
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, pos);
				BaseDLL.encode_uint16(buffer, ref pos_, maxNum);
				BaseDLL.encode_int8(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
				UInt16 teamListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamListCnt);
				teamList = new TeamBaseInfo[teamListCnt];
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i] = new TeamBaseInfo();
					teamList[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref pos);
				BaseDLL.decode_uint16(buffer, ref pos_, ref maxNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, targetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamList.Length);
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, pos);
				BaseDLL.encode_uint16(buffer, ref pos_, maxNum);
				BaseDLL.encode_int8(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
				UInt16 teamListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamListCnt);
				teamList = new TeamBaseInfo[teamListCnt];
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i] = new TeamBaseInfo();
					teamList[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref pos);
				BaseDLL.decode_uint16(buffer, ref pos_, ref maxNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// targetId
				_len += 4;
				// teamList
				_len += 2;
				for(int j = 0; j < teamList.Length; j++)
				{
					_len += teamList[j].getLen();
				}
				// pos
				_len += 2;
				// maxNum
				_len += 2;
				// param
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步队长操作
	/// </summary>
	[Protocol]
	public class WorldTeamMasterOperSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601629;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型
		/// </summary>
		public byte type;
		/// <summary>
		///  具体操作
		/// </summary>
		public UInt32 param;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// param
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求修改位置状态（打开或关闭）
	/// </summary>
	[Protocol]
	public class WorldTeamChangePosStatusReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601630;
		public UInt32 Sequence;
		/// <summary>
		///  位置
		/// </summary>
		public byte pos;
		/// <summary>
		///  0代表打开位置，1代表关闭位置
		/// </summary>
		public byte open;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, open);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref open);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, open);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref open);
			}

			public int getLen()
			{
				int _len = 0;
				// pos
				_len += 1;
				// open
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步位置状态改变
	/// </summary>
	[Protocol]
	public class WorldTeamChangePosStatusSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601631;
		public UInt32 Sequence;
		/// <summary>
		///  位置
		/// </summary>
		public byte pos;
		/// <summary>
		///  1代表打开位置，0代表关闭位置
		/// </summary>
		public byte open;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, open);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref open);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, open);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref open);
			}

			public int getLen()
			{
				int _len = 0;
				// pos
				_len += 1;
				// open
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  准备
	/// </summary>
	[Protocol]
	public class WorldTeamReadyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601632;
		public UInt32 Sequence;
		/// <summary>
		///  是否准备好(0:取消 1:准备)
		/// </summary>
		public byte ready;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, ready);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref ready);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, ready);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref ready);
			}

			public int getLen()
			{
				int _len = 0;
				// ready
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步外观信息
	/// </summary>
	[Protocol]
	public class WorldSyncTeammemberAvatar : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601636;
		public UInt32 Sequence;
		/// <summary>
		///  成员ID
		/// </summary>
		public UInt64 memberId;
		/// <summary>
		///  外观
		/// </summary>
		public PlayerAvatar avatar = new PlayerAvatar();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, memberId);
				avatar.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref memberId);
				avatar.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, memberId);
				avatar.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref memberId);
				avatar.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// memberId
				_len += 8;
				// avatar
				_len += avatar.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知有新申请入队的人
	/// </summary>
	[Protocol]
	public class WorldTeamNotifyNewRequester : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601637;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  获取申请列表
	/// </summary>
	[Protocol]
	public class WorldTeamRequesterListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601638;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回申请列表
	/// </summary>
	[Protocol]
	public class WorldTeamRequesterListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601639;
		public UInt32 Sequence;
		public TeammemberBaseInfo[] requesters = new TeammemberBaseInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)requesters.Length);
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 requestersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref requestersCnt);
				requesters = new TeammemberBaseInfo[requestersCnt];
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i] = new TeammemberBaseInfo();
					requesters[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)requesters.Length);
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 requestersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref requestersCnt);
				requesters = new TeammemberBaseInfo[requestersCnt];
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i] = new TeammemberBaseInfo();
					requesters[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// requesters
				_len += 2;
				for(int j = 0; j < requesters.Length; j++)
				{
					_len += requesters[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  处理申请者（同意、拒绝）
	/// </summary>
	[Protocol]
	public class WorldTeamProcessRequesterReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601640;
		public UInt32 Sequence;
		/// <summary>
		///  目标ID
		/// </summary>
		public UInt64 targetId;
		/// <summary>
		///  是否同意
		/// </summary>
		public byte agree;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// targetId
				_len += 8;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  处理请求者返回
	/// </summary>
	[Protocol]
	public class WorldTeamProcessRequesterRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601641;
		public UInt32 Sequence;
		/// <summary>
		///  目标ID
		/// </summary>
		public UInt64 targetId;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// targetId
				_len += 8;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知开始地下城投票
	/// </summary>
	[Protocol]
	public class WorldTeamRaceVoteNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601642;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		public UInt32 dungeonId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  玩家上报投票选项
	/// </summary>
	[Protocol]
	public class WorldTeamReportVoteChoice : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601643;
		public UInt32 Sequence;
		/// <summary>
		///  是否同意
		/// </summary>
		public byte agree;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  队伍邀请返回
	/// </summary>
	[Protocol]
	public class WorldTeamInviteRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601644;
		public UInt32 Sequence;
		/// <summary>
		///  目标玩家ID
		/// </summary>
		public UInt64 targetId;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// targetId
				_len += 8;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知玩家队伍邀请
	/// </summary>
	[Protocol]
	public class WorldTeamInviteNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601645;
		public UInt32 Sequence;
		/// <summary>
		///  队伍信息
		/// </summary>
		public TeamBaseInfo info = new TeamBaseInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知玩家队伍请求处理结果
	/// </summary>
	[Protocol]
	public class WorldTeamRequestResultNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601646;
		public UInt32 Sequence;
		/// <summary>
		///  是否同意
		/// </summary>
		public byte agree;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  广播玩家玩家投票选项
	/// </summary>
	[Protocol]
	public class WorldTeamVoteChoiceNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601647;
		public UInt32 Sequence;
		/// <summary>
		///  角色ID
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		///  是否同意
		/// </summary>
		public byte agree;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知玩家组队快速匹配结果
	/// </summary>
	[Protocol]
	public class WorldTeamMatchResultNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601648;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		public UInt32 dungeonId;
		/// <summary>
		///  是否同意
		/// </summary>
		public PlayerIcon[] players = new PlayerIcon[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new PlayerIcon[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new PlayerIcon();
					players[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new PlayerIcon[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new PlayerIcon();
					players[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// players
				_len += 2;
				for(int j = 0; j < players.Length; j++)
				{
					_len += players[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求取消组队匹配
	/// </summary>
	[Protocol]
	public class WorldTeamMatchCancelReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601650;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  取消组队匹配返回
	/// </summary>
	[Protocol]
	public class WorldTeamMatchCancelRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601651;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求开始组队匹配
	/// </summary>
	[Protocol]
	public class SceneTeamMatchStartReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501604;
		public UInt32 Sequence;
		/// <summary>
		///  目标地下城ID
		/// </summary>
		public UInt32 dungeonId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开始组队匹配返回
	/// </summary>
	[Protocol]
	public class SceneTeamMatchStartRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501605;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步队员属性
	/// </summary>
	[Protocol]
	public class WorldSyncTeamMemberProperty : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601654;
		public UInt32 Sequence;
		/// <summary>
		///  成员ID
		/// </summary>
		public UInt64 memberId;
		/// <summary>
		///  属性类型，对应枚举TeamMemberProperty
		/// </summary>
		public byte type;
		/// <summary>
		///  新的值
		/// </summary>
		public UInt64 value;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, memberId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, value);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref memberId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref value);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, memberId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, value);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref memberId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref value);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// memberId
				_len += 8;
				// type
				_len += 1;
				// value
				_len += 8;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步队员属性
	/// </summary>
	[Protocol]
	public class WorldChangeAssistModeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601655;
		public UInt32 Sequence;
		/// <summary>
		///  是否助战
		/// </summary>
		public byte isAssist;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isAssist);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isAssist);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isAssist);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isAssist);
			}

			public int getLen()
			{
				int _len = 0;
				// isAssist
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  清除队伍邀请
	/// </summary>
	[Protocol]
	public class WorldTeamInviteClearNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601656;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		public Int32 teamId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, teamId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref teamId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, teamId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref teamId);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步队伍属性给被邀请者
	/// </summary>
	[Protocol]
	public class WorldTeamInviteSyncAttr : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601657;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		public Int32 teamId;
		/// <summary>
		///  队伍目标
		/// </summary>
		public Int32 target;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, teamId);
				BaseDLL.encode_int32(buffer, ref pos_, target);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_int32(buffer, ref pos_, ref target);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, teamId);
				BaseDLL.encode_int32(buffer, ref pos_, target);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_int32(buffer, ref pos_, ref target);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// target
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  组队解散倒计时状态
	/// </summary>
	[Protocol]
	public class WorldNotifyTeamKick : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601660;
		public UInt32 Sequence;
		/// <summary>
		///  非0是踢出时间 0是停止
		/// </summary>
		public UInt64 endTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, endTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref endTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, endTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref endTime);
			}

			public int getLen()
			{
				int _len = 0;
				// endTime
				_len += 8;
				return _len;
			}
		#endregion

	}

}
