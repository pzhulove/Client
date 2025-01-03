using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  请求类型
	/// </summary>
	public enum RequestType
	{
		/// <summary>
		///  邀请组队
		/// </summary>
		InviteTeam = 1,
		/// <summary>
		///  根据玩家ID请求入队
		/// </summary>
		JoinTeam = 2,
		/// <summary>
		///  请求加好友
		/// </summary>
		RequestFriend = 3,
		/// <summary>
		///  请求拜师
		/// </summary>
		RequestMaster = 4,
		/// <summary>
		///  请求收徒
		/// </summary>
		RequestDisciple = 5,
		/// <summary>
		///  根据队伍ID加入队伍
		/// </summary>
		JoinTeamByTeamID = 21,
		/// <summary>
		///  请求通过名字加好友
		/// </summary>
		RequestFriendByName = 29,
		/// <summary>
		///  挑战
		/// </summary>
		Request_Challenge_PK = 30,
		/// <summary>
		///  邀请公会
		/// </summary>
		InviteJoinGuild = 31,
		/// <summary>
		/// 公平竞技场邀请
		/// </summary>
		Request_Equal_PK = 32,
	}

	/// <summary>
	///  刷新类型
	/// </summary>
	public enum RefreshType
	{
		/// <summary>
		///  不刷新
		/// </summary>
		REFRESH_TYPE_NONE = 0,
		/// <summary>
		///  每月刷新
		/// </summary>
		REFRESH_TYPE_PER_MONTH = 1,
		/// <summary>
		///  每周刷新
		/// </summary>
		REFRESH_TYPE_PER_WEEK = 2,
		/// <summary>
		///  每日刷新
		/// </summary>
		REFRESH_TYPE_PER_DAY = 3,
	}

	/// <summary>
	///  新手引导选择标志
	/// </summary>
	public enum NoviceGuideChooseFlag
	{
		/// <summary>
		///  初态
		/// </summary>
		NGCF_INIT = 0,
		/// <summary>
		///  弹出选择
		/// </summary>
		NGCF_POPUP = 1,
		/// <summary>
		///  选择跳过引导
		/// </summary>
		NGCF_PASS = 2,
		/// <summary>
		///  选择不跳过
		/// </summary>
		NGCF_NOT_PASS = 3,
	}

	[Protocol]
	public class HeartBeatMsg : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 0;
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

	[Protocol]
	public class GateSyncServerTime : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300309;
		public UInt32 Sequence;
		public UInt32 time;

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
				BaseDLL.encode_uint32(buffer, ref pos_, time);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, time);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public int getLen()
			{
				int _len = 0;
				// time
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class SockAddr : Protocol.IProtocolStream
	{
		public string ip;
		public UInt16 port;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] ipBytes = StringHelper.StringToUTF8Bytes(ip);
				BaseDLL.encode_string(buffer, ref pos_, ipBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, port);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 ipLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ipLen);
				byte[] ipBytes = new byte[ipLen];
				for(int i = 0; i < ipLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ipBytes[i]);
				}
				ip = StringHelper.BytesToString(ipBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref port);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] ipBytes = StringHelper.StringToUTF8Bytes(ip);
				BaseDLL.encode_string(buffer, ref pos_, ipBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, port);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 ipLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ipLen);
				byte[] ipBytes = new byte[ipLen];
				for(int i = 0; i < ipLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ipBytes[i]);
				}
				ip = StringHelper.BytesToString(ipBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref port);
			}

			public int getLen()
			{
				int _len = 0;
				// ip
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(ip);
					_len += 2 + _strBytes.Length;
				}
				// port
				_len += 2;
				return _len;
			}
		#endregion

	}

	public class PlayerAvatar : Protocol.IProtocolStream
	{
		public UInt32[] equipItemIds = new UInt32[0];
		/// <summary>
		///  武器强化等级
		/// </summary>
		public byte weaponStrengthen;
		/// <summary>
		///  是否显示时装武器
		/// </summary>
		public byte isShoWeapon;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipItemIds.Length);
				for(int i = 0; i < equipItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, equipItemIds[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, weaponStrengthen);
				BaseDLL.encode_int8(buffer, ref pos_, isShoWeapon);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 equipItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipItemIdsCnt);
				equipItemIds = new UInt32[equipItemIdsCnt];
				for(int i = 0; i < equipItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref equipItemIds[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref weaponStrengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref isShoWeapon);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipItemIds.Length);
				for(int i = 0; i < equipItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, equipItemIds[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, weaponStrengthen);
				BaseDLL.encode_int8(buffer, ref pos_, isShoWeapon);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 equipItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipItemIdsCnt);
				equipItemIds = new UInt32[equipItemIdsCnt];
				for(int i = 0; i < equipItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref equipItemIds[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref weaponStrengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref isShoWeapon);
			}

			public int getLen()
			{
				int _len = 0;
				// equipItemIds
				_len += 2 + 4 * equipItemIds.Length;
				// weaponStrengthen
				_len += 1;
				// isShoWeapon
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  玩家标签信息
	/// </summary>
	public class PlayerLabelInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  觉醒状态
		/// </summary>
		public byte awakenStatus;
		/// <summary>
		///  回归状态
		/// </summary>
		public byte returnStatus;
		/// <summary>
		///  新手引导选择标志
		/// </summary>
		public byte noviceGuideChooseFlag;
		/// <summary>
		///  头像框
		/// </summary>
		public UInt32 headFrame;
		/// <summary>
		///  公会ID
		/// </summary>
		public UInt64 guildId;
		/// <summary>
		///  回归周年称号
		/// </summary>
		public UInt32 returnAnniversaryTitle;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, awakenStatus);
				BaseDLL.encode_int8(buffer, ref pos_, returnStatus);
				BaseDLL.encode_int8(buffer, ref pos_, noviceGuideChooseFlag);
				BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_uint32(buffer, ref pos_, returnAnniversaryTitle);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref awakenStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref returnStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref noviceGuideChooseFlag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref returnAnniversaryTitle);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, awakenStatus);
				BaseDLL.encode_int8(buffer, ref pos_, returnStatus);
				BaseDLL.encode_int8(buffer, ref pos_, noviceGuideChooseFlag);
				BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_uint32(buffer, ref pos_, returnAnniversaryTitle);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref awakenStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref returnStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref noviceGuideChooseFlag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref returnAnniversaryTitle);
			}

			public int getLen()
			{
				int _len = 0;
				// awakenStatus
				_len += 1;
				// returnStatus
				_len += 1;
				// noviceGuideChooseFlag
				_len += 1;
				// headFrame
				_len += 4;
				// guildId
				_len += 8;
				// returnAnniversaryTitle
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class RoleInfo : Protocol.IProtocolStream
	{
		public UInt64 roleId;
		public string strRoleId;
		public string name;
		public byte sex;
		public byte occupation;
		public UInt16 level;
		public UInt32 offlineTime;
		public UInt32 deleteTime;
		public PlayerAvatar avatar = new PlayerAvatar();
		public UInt32 newboot;
		public byte preOccu;
		/// <summary>
		///  是否是预约角色
		/// </summary>
		public byte isAppointmentOccu;
		/// <summary>
		/// 是否老兵回归
		/// </summary>
		public byte isVeteranReturn;
		/// <summary>
		/// 是否收藏
		/// </summary>
		public byte isCollection;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] strRoleIdBytes = StringHelper.StringToUTF8Bytes(strRoleId);
				BaseDLL.encode_string(buffer, ref pos_, strRoleIdBytes, (UInt16)(buffer.Length - pos_));
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, sex);
				BaseDLL.encode_int8(buffer, ref pos_, occupation);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, offlineTime);
				BaseDLL.encode_uint32(buffer, ref pos_, deleteTime);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, newboot);
				BaseDLL.encode_int8(buffer, ref pos_, preOccu);
				BaseDLL.encode_int8(buffer, ref pos_, isAppointmentOccu);
				BaseDLL.encode_int8(buffer, ref pos_, isVeteranReturn);
				BaseDLL.encode_int8(buffer, ref pos_, isCollection);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				UInt16 strRoleIdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strRoleIdLen);
				byte[] strRoleIdBytes = new byte[strRoleIdLen];
				for(int i = 0; i < strRoleIdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref strRoleIdBytes[i]);
				}
				strRoleId = StringHelper.BytesToString(strRoleIdBytes);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref sex);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref offlineTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref deleteTime);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newboot);
				BaseDLL.decode_int8(buffer, ref pos_, ref preOccu);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAppointmentOccu);
				BaseDLL.decode_int8(buffer, ref pos_, ref isVeteranReturn);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCollection);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] strRoleIdBytes = StringHelper.StringToUTF8Bytes(strRoleId);
				BaseDLL.encode_string(buffer, ref pos_, strRoleIdBytes, (UInt16)(buffer.Length - pos_));
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, sex);
				BaseDLL.encode_int8(buffer, ref pos_, occupation);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, offlineTime);
				BaseDLL.encode_uint32(buffer, ref pos_, deleteTime);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, newboot);
				BaseDLL.encode_int8(buffer, ref pos_, preOccu);
				BaseDLL.encode_int8(buffer, ref pos_, isAppointmentOccu);
				BaseDLL.encode_int8(buffer, ref pos_, isVeteranReturn);
				BaseDLL.encode_int8(buffer, ref pos_, isCollection);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				UInt16 strRoleIdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strRoleIdLen);
				byte[] strRoleIdBytes = new byte[strRoleIdLen];
				for(int i = 0; i < strRoleIdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref strRoleIdBytes[i]);
				}
				strRoleId = StringHelper.BytesToString(strRoleIdBytes);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref sex);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref offlineTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref deleteTime);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newboot);
				BaseDLL.decode_int8(buffer, ref pos_, ref preOccu);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAppointmentOccu);
				BaseDLL.decode_int8(buffer, ref pos_, ref isVeteranReturn);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCollection);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// strRoleId
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(strRoleId);
					_len += 2 + _strBytes.Length;
				}
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// sex
				_len += 1;
				// occupation
				_len += 1;
				// level
				_len += 2;
				// offlineTime
				_len += 4;
				// deleteTime
				_len += 4;
				// avatar
				_len += avatar.getLen();
				// newboot
				_len += 4;
				// preOccu
				_len += 1;
				// isAppointmentOccu
				_len += 1;
				// isVeteranReturn
				_len += 1;
				// isCollection
				_len += 1;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  发出请求
	/// </summary>
	[Protocol]
	public class SceneRequest : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500804;
		public UInt32 Sequence;
		/// <summary>
		///  类型(对应枚举RequestType)
		/// </summary>
		public byte type;
		/// <summary>
		///  目标ID
		/// </summary>
		public UInt64 target;
		/// <summary>
		///  目标名字
		/// </summary>
		public string targetName;
		/// <summary>
		///  附加参数
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
				BaseDLL.encode_uint64(buffer, ref pos_, target);
				byte[] targetNameBytes = StringHelper.StringToUTF8Bytes(targetName);
				BaseDLL.encode_string(buffer, ref pos_, targetNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref target);
				UInt16 targetNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetNameLen);
				byte[] targetNameBytes = new byte[targetNameLen];
				for(int i = 0; i < targetNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref targetNameBytes[i]);
				}
				targetName = StringHelper.BytesToString(targetNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, target);
				byte[] targetNameBytes = StringHelper.StringToUTF8Bytes(targetName);
				BaseDLL.encode_string(buffer, ref pos_, targetNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref target);
				UInt16 targetNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetNameLen);
				byte[] targetNameBytes = new byte[targetNameLen];
				for(int i = 0; i < targetNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref targetNameBytes[i]);
				}
				targetName = StringHelper.BytesToString(targetNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// target
				_len += 8;
				// targetName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(targetName);
					_len += 2 + _strBytes.Length;
				}
				// param
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步请求
	/// </summary>
	[Protocol]
	public class SceneSyncRequest : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500805;
		public UInt32 Sequence;
		/// <summary>
		///  类型(对应枚举RequestType)
		/// </summary>
		public byte type;
		/// <summary>
		///  请求者
		/// </summary>
		public UInt64 requester;
		/// <summary>
		///  请求者名字
		/// </summary>
		public string requesterName;
		/// <summary>
		///  请求者性别
		/// </summary>
		public byte requesterOccu;
		/// <summary>
		///  请求者等级
		/// </summary>
		public UInt16 requesterLevel;
		/// <summary>
		///  附带参数1
		/// </summary>
		public string param1;
		/// <summary>
		/// vip等级
		/// </summary>
		public byte requesterVipLv;
		/// <summary>
		/// 外观信息
		/// </summary>
		public PlayerAvatar avatar = new PlayerAvatar();
		/// <summary>
		/// 在线时间类型
		/// </summary>
		public byte activeTimeType;
		/// <summary>
		/// 师傅类型
		/// </summary>
		public byte masterType;
		/// <summary>
		/// 地区id
		/// </summary>
		public byte regionId;
		/// <summary>
		/// 收徒或拜师宣言
		/// </summary>
		public string declaration;

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
				BaseDLL.encode_uint64(buffer, ref pos_, requester);
				byte[] requesterNameBytes = StringHelper.StringToUTF8Bytes(requesterName);
				BaseDLL.encode_string(buffer, ref pos_, requesterNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, requesterOccu);
				BaseDLL.encode_uint16(buffer, ref pos_, requesterLevel);
				byte[] param1Bytes = StringHelper.StringToUTF8Bytes(param1);
				BaseDLL.encode_string(buffer, ref pos_, param1Bytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, requesterVipLv);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref requester);
				UInt16 requesterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref requesterNameLen);
				byte[] requesterNameBytes = new byte[requesterNameLen];
				for(int i = 0; i < requesterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref requesterNameBytes[i]);
				}
				requesterName = StringHelper.BytesToString(requesterNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref requesterOccu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref requesterLevel);
				UInt16 param1Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param1Len);
				byte[] param1Bytes = new byte[param1Len];
				for(int i = 0; i < param1Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param1Bytes[i]);
				}
				param1 = StringHelper.BytesToString(param1Bytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref requesterVipLv);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, requester);
				byte[] requesterNameBytes = StringHelper.StringToUTF8Bytes(requesterName);
				BaseDLL.encode_string(buffer, ref pos_, requesterNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, requesterOccu);
				BaseDLL.encode_uint16(buffer, ref pos_, requesterLevel);
				byte[] param1Bytes = StringHelper.StringToUTF8Bytes(param1);
				BaseDLL.encode_string(buffer, ref pos_, param1Bytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, requesterVipLv);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref requester);
				UInt16 requesterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref requesterNameLen);
				byte[] requesterNameBytes = new byte[requesterNameLen];
				for(int i = 0; i < requesterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref requesterNameBytes[i]);
				}
				requesterName = StringHelper.BytesToString(requesterNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref requesterOccu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref requesterLevel);
				UInt16 param1Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param1Len);
				byte[] param1Bytes = new byte[param1Len];
				for(int i = 0; i < param1Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param1Bytes[i]);
				}
				param1 = StringHelper.BytesToString(param1Bytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref requesterVipLv);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// requester
				_len += 8;
				// requesterName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(requesterName);
					_len += 2 + _strBytes.Length;
				}
				// requesterOccu
				_len += 1;
				// requesterLevel
				_len += 2;
				// param1
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(param1);
					_len += 2 + _strBytes.Length;
				}
				// requesterVipLv
				_len += 1;
				// avatar
				_len += avatar.getLen();
				// activeTimeType
				_len += 1;
				// masterType
				_len += 1;
				// regionId
				_len += 1;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  答复
	/// </summary>
	[Protocol]
	public class SceneReply : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500806;
		public UInt32 Sequence;
		/// <summary>
		/// 类型(对应枚举RequestType)
		/// </summary>
		public byte type;
		/// <summary>
		/// 请求者
		/// </summary>
		public UInt64 requester;
		/// <summary>
		/// 结果	1为接收 0为拒接
		/// </summary>
		public byte result;

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
				BaseDLL.encode_uint64(buffer, ref pos_, requester);
				BaseDLL.encode_int8(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref requester);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, requester);
				BaseDLL.encode_int8(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref requester);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// requester
				_len += 8;
				// result
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  头像
	/// </summary>
	public class PlayerIcon : Protocol.IProtocolStream
	{
		/// <summary>
		///  ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  名字
		/// </summary>
		public string name;
		/// <summary>
		///  职业
		/// </summary>
		public byte occu;
		/// <summary>
		///  等级
		/// </summary>
		public UInt16 level;
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
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
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
				// occu
				_len += 1;
				// level
				_len += 2;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// Counter项
	/// </summary>
	public class CounterItem : Protocol.IProtocolStream
	{
		/// <summary>
		/// 货币id
		/// </summary>
		public UInt32 currencyId;
		/// <summary>
		/// 名字
		/// </summary>
		public string name;
		/// <summary>
		/// 值
		/// </summary>
		public UInt32 value;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, currencyId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref currencyId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, currencyId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref currencyId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public int getLen()
			{
				int _len = 0;
				// currencyId
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// value
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  客户端埋点
	/// </summary>
	[Protocol]
	public class SceneClientLog : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500631;
		public UInt32 Sequence;
		/// <summary>
		///  名字
		/// </summary>
		public string name;
		/// <summary>
		///  参数1
		/// </summary>
		public string param1;
		/// <summary>
		///  参数2
		/// </summary>
		public string param2;
		/// <summary>
		///  参数3
		/// </summary>
		public string param3;

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
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] param1Bytes = StringHelper.StringToUTF8Bytes(param1);
				BaseDLL.encode_string(buffer, ref pos_, param1Bytes, (UInt16)(buffer.Length - pos_));
				byte[] param2Bytes = StringHelper.StringToUTF8Bytes(param2);
				BaseDLL.encode_string(buffer, ref pos_, param2Bytes, (UInt16)(buffer.Length - pos_));
				byte[] param3Bytes = StringHelper.StringToUTF8Bytes(param3);
				BaseDLL.encode_string(buffer, ref pos_, param3Bytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 param1Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param1Len);
				byte[] param1Bytes = new byte[param1Len];
				for(int i = 0; i < param1Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param1Bytes[i]);
				}
				param1 = StringHelper.BytesToString(param1Bytes);
				UInt16 param2Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param2Len);
				byte[] param2Bytes = new byte[param2Len];
				for(int i = 0; i < param2Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param2Bytes[i]);
				}
				param2 = StringHelper.BytesToString(param2Bytes);
				UInt16 param3Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param3Len);
				byte[] param3Bytes = new byte[param3Len];
				for(int i = 0; i < param3Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param3Bytes[i]);
				}
				param3 = StringHelper.BytesToString(param3Bytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] param1Bytes = StringHelper.StringToUTF8Bytes(param1);
				BaseDLL.encode_string(buffer, ref pos_, param1Bytes, (UInt16)(buffer.Length - pos_));
				byte[] param2Bytes = StringHelper.StringToUTF8Bytes(param2);
				BaseDLL.encode_string(buffer, ref pos_, param2Bytes, (UInt16)(buffer.Length - pos_));
				byte[] param3Bytes = StringHelper.StringToUTF8Bytes(param3);
				BaseDLL.encode_string(buffer, ref pos_, param3Bytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 param1Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param1Len);
				byte[] param1Bytes = new byte[param1Len];
				for(int i = 0; i < param1Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param1Bytes[i]);
				}
				param1 = StringHelper.BytesToString(param1Bytes);
				UInt16 param2Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param2Len);
				byte[] param2Bytes = new byte[param2Len];
				for(int i = 0; i < param2Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param2Bytes[i]);
				}
				param2 = StringHelper.BytesToString(param2Bytes);
				UInt16 param3Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref param3Len);
				byte[] param3Bytes = new byte[param3Len];
				for(int i = 0; i < param3Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref param3Bytes[i]);
				}
				param3 = StringHelper.BytesToString(param3Bytes);
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// param1
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(param1);
					_len += 2 + _strBytes.Length;
				}
				// param2
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(param2);
					_len += 2 + _strBytes.Length;
				}
				// param3
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(param3);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

}
