using System;
using System.Text;

namespace Protocol
{
	public enum RetinueUpType
	{
		/// <summary>
		///  升级
		/// </summary>
		RUT_UPLEVEL = 1,
		/// <summary>
		/// 升星
		/// </summary>
		RUT_UPSTAR = 2,
	}

	public class RetinueSkill : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 bufferId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, bufferId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bufferId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, bufferId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bufferId);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// bufferId
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class RetinueInfo : Protocol.IProtocolStream
	{
		public UInt64 id;
		public UInt32 dataId;
		public byte level;
		public byte starLevel;
		public RetinueSkill[] skills = new RetinueSkill[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, starLevel);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref starLevel);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new RetinueSkill[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new RetinueSkill();
					skills[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, starLevel);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref starLevel);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new RetinueSkill[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new RetinueSkill();
					skills[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// dataId
				_len += 4;
				// level
				_len += 1;
				// starLevel
				_len += 1;
				// skills
				_len += 2;
				for(int j = 0; j < skills.Length; j++)
				{
					_len += skills[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class SceneRetinue : Protocol.IProtocolStream
	{
		public UInt32 id;
		public byte level;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// level
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncRetinueList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507001;
		public UInt32 Sequence;
		public UInt64 id;
		public UInt64[] offRetinueIds = new UInt64[0];
		public RetinueInfo[] retinueList = new RetinueInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)offRetinueIds.Length);
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, offRetinueIds[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)retinueList.Length);
				for(int i = 0; i < retinueList.Length; i++)
				{
					retinueList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 offRetinueIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref offRetinueIdsCnt);
				offRetinueIds = new UInt64[offRetinueIdsCnt];
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref offRetinueIds[i]);
				}
				UInt16 retinueListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref retinueListCnt);
				retinueList = new RetinueInfo[retinueListCnt];
				for(int i = 0; i < retinueList.Length; i++)
				{
					retinueList[i] = new RetinueInfo();
					retinueList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)offRetinueIds.Length);
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, offRetinueIds[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)retinueList.Length);
				for(int i = 0; i < retinueList.Length; i++)
				{
					retinueList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 offRetinueIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref offRetinueIdsCnt);
				offRetinueIds = new UInt64[offRetinueIdsCnt];
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref offRetinueIds[i]);
				}
				UInt16 retinueListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref retinueListCnt);
				retinueList = new RetinueInfo[retinueListCnt];
				for(int i = 0; i < retinueList.Length; i++)
				{
					retinueList[i] = new RetinueInfo();
					retinueList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// offRetinueIds
				_len += 2 + 8 * offRetinueIds.Length;
				// retinueList
				_len += 2;
				for(int j = 0; j < retinueList.Length; j++)
				{
					_len += retinueList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncRetinue : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507002;
		public UInt32 Sequence;
		public RetinueInfo info = new RetinueInfo();

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

	[Protocol]
	public class SceneChanageRetinueReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507003;
		public UInt32 Sequence;
		public UInt64 id;
		public byte index;

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
				BaseDLL.encode_int8(buffer, ref pos_, index);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, index);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// index
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneChanageRetinueRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507004;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt64 id;
		public UInt64[] offRetinueIds = new UInt64[0];

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)offRetinueIds.Length);
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, offRetinueIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 offRetinueIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref offRetinueIdsCnt);
				offRetinueIds = new UInt64[offRetinueIdsCnt];
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref offRetinueIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)offRetinueIds.Length);
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, offRetinueIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 offRetinueIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref offRetinueIdsCnt);
				offRetinueIds = new UInt64[offRetinueIdsCnt];
				for(int i = 0; i < offRetinueIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref offRetinueIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// id
				_len += 8;
				// offRetinueIds
				_len += 2 + 8 * offRetinueIds.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneRetinueChangeSkillReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507005;
		public UInt32 Sequence;
		public UInt64 id;
		public UInt32[] skillIds = new UInt32[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillIds.Length);
				for(int i = 0; i < skillIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, skillIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 skillIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillIdsCnt);
				skillIds = new UInt32[skillIdsCnt];
				for(int i = 0; i < skillIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref skillIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillIds.Length);
				for(int i = 0; i < skillIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, skillIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 skillIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillIdsCnt);
				skillIds = new UInt32[skillIdsCnt];
				for(int i = 0; i < skillIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref skillIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// skillIds
				_len += 2 + 4 * skillIds.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneRetinueChangeSkillRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507006;
		public UInt32 Sequence;
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

	[Protocol]
	public class SceneRetinueUnlockReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507007;
		public UInt32 Sequence;
		public UInt32 id;

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
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneRetinueUnlockRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507008;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 dataId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// dataId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneRetinueUpLevelReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507009;
		public UInt32 Sequence;
		public byte type;
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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneRetinueUpLevelRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507010;
		public UInt32 Sequence;
		public UInt32 result;
		public byte type;
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
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// type
				_len += 1;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

}
