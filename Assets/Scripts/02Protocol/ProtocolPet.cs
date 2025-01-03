using System;
using System.Text;

namespace Protocol
{
	public class PetInfo : Protocol.IProtocolStream
	{
		public UInt64 id;
		public UInt32 dataId;
		public UInt16 level;
		public UInt32 exp;
		public UInt16 hunger;
		public byte skillIndex;
		public byte pointFeedCount;
		public byte goldFeedCount;
		public byte selectSkillCount;
		public UInt32 petScore;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
				BaseDLL.encode_uint16(buffer, ref pos_, hunger);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
				BaseDLL.encode_int8(buffer, ref pos_, pointFeedCount);
				BaseDLL.encode_int8(buffer, ref pos_, goldFeedCount);
				BaseDLL.encode_int8(buffer, ref pos_, selectSkillCount);
				BaseDLL.encode_uint32(buffer, ref pos_, petScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
				BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref pointFeedCount);
				BaseDLL.decode_int8(buffer, ref pos_, ref goldFeedCount);
				BaseDLL.decode_int8(buffer, ref pos_, ref selectSkillCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref petScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
				BaseDLL.encode_uint16(buffer, ref pos_, hunger);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
				BaseDLL.encode_int8(buffer, ref pos_, pointFeedCount);
				BaseDLL.encode_int8(buffer, ref pos_, goldFeedCount);
				BaseDLL.encode_int8(buffer, ref pos_, selectSkillCount);
				BaseDLL.encode_uint32(buffer, ref pos_, petScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
				BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref pointFeedCount);
				BaseDLL.decode_int8(buffer, ref pos_, ref goldFeedCount);
				BaseDLL.decode_int8(buffer, ref pos_, ref selectSkillCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref petScore);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// dataId
				_len += 4;
				// level
				_len += 2;
				// exp
				_len += 4;
				// hunger
				_len += 2;
				// skillIndex
				_len += 1;
				// pointFeedCount
				_len += 1;
				// goldFeedCount
				_len += 1;
				// selectSkillCount
				_len += 1;
				// petScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class PetBaseInfo : Protocol.IProtocolStream
	{
		public UInt32 dataId;
		public UInt16 level;
		public UInt16 hunger;
		public byte skillIndex;
		public UInt32 petScore;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint16(buffer, ref pos_, hunger);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, petScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref petScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint16(buffer, ref pos_, hunger);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, petScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref petScore);
			}

			public int getLen()
			{
				int _len = 0;
				// dataId
				_len += 4;
				// level
				_len += 2;
				// hunger
				_len += 2;
				// skillIndex
				_len += 1;
				// petScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ScenePet : Protocol.IProtocolStream
	{
		public UInt64 id;
		public UInt32 dataId;
		public UInt16 level;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// dataId
				_len += 4;
				// level
				_len += 2;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncPetList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502201;
		public UInt32 Sequence;
		public UInt64 followPetId;
		public UInt64[] battlePets = new UInt64[0];
		public PetInfo[] petList = new PetInfo[0];

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
				BaseDLL.encode_uint64(buffer, ref pos_, followPetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePets.Length);
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, battlePets[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)petList.Length);
				for(int i = 0; i < petList.Length; i++)
				{
					petList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref followPetId);
				UInt16 battlePetsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlePetsCnt);
				battlePets = new UInt64[battlePetsCnt];
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref battlePets[i]);
				}
				UInt16 petListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petListCnt);
				petList = new PetInfo[petListCnt];
				for(int i = 0; i < petList.Length; i++)
				{
					petList[i] = new PetInfo();
					petList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, followPetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePets.Length);
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, battlePets[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)petList.Length);
				for(int i = 0; i < petList.Length; i++)
				{
					petList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref followPetId);
				UInt16 battlePetsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlePetsCnt);
				battlePets = new UInt64[battlePetsCnt];
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref battlePets[i]);
				}
				UInt16 petListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petListCnt);
				petList = new PetInfo[petListCnt];
				for(int i = 0; i < petList.Length; i++)
				{
					petList[i] = new PetInfo();
					petList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// followPetId
				_len += 8;
				// battlePets
				_len += 2 + 8 * battlePets.Length;
				// petList
				_len += 2;
				for(int j = 0; j < petList.Length; j++)
				{
					_len += petList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncPet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502202;
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
	public class SceneDeletePet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502203;
		public UInt32 Sequence;
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

	[Protocol]
	public class SceneSetPetSoltReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502205;
		public UInt32 Sequence;
		public byte petType;
		public UInt64 petId;

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
				BaseDLL.encode_int8(buffer, ref pos_, petType);
				BaseDLL.encode_uint64(buffer, ref pos_, petId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref petType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref petId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, petType);
				BaseDLL.encode_uint64(buffer, ref pos_, petId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref petType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref petId);
			}

			public int getLen()
			{
				int _len = 0;
				// petType
				_len += 1;
				// petId
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSetPetSoltRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502206;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt64[] battlePets = new UInt64[0];
		public UInt64 followPetId;

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePets.Length);
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, battlePets[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, followPetId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 battlePetsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlePetsCnt);
				battlePets = new UInt64[battlePetsCnt];
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref battlePets[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref followPetId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePets.Length);
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, battlePets[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, followPetId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 battlePetsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlePetsCnt);
				battlePets = new UInt64[battlePetsCnt];
				for(int i = 0; i < battlePets.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref battlePets[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref followPetId);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// battlePets
				_len += 2 + 8 * battlePets.Length;
				// followPetId
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFeedPetReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502207;
		public UInt32 Sequence;
		public UInt64 id;
		public byte feedType;

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
				BaseDLL.encode_int8(buffer, ref pos_, feedType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref feedType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, feedType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref feedType);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// feedType
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFeedPetRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502208;
		public UInt32 Sequence;
		public UInt32 result;
		public byte feedType;
		public byte isCritical;
		public UInt32 value;

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
				BaseDLL.encode_int8(buffer, ref pos_, feedType);
				BaseDLL.encode_int8(buffer, ref pos_, isCritical);
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref feedType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCritical);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, feedType);
				BaseDLL.encode_int8(buffer, ref pos_, isCritical);
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref feedType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCritical);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// feedType
				_len += 1;
				// isCritical
				_len += 1;
				// value
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSellPetReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502209;
		public UInt32 Sequence;
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

	[Protocol]
	public class SceneSellPetRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502210;
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
	public class SceneChangePetSkillReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502211;
		public UInt32 Sequence;
		public UInt64 id;
		public byte skillIndex;

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
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// skillIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneChangePetSkillRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502212;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt64 petId;
		public byte skillIndex;

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
				BaseDLL.encode_uint64(buffer, ref pos_, petId);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref petId);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, petId);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref petId);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// petId
				_len += 8;
				// skillIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSetPetFollowReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502213;
		public UInt32 Sequence;
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

	[Protocol]
	public class SceneSetPetFollowRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502214;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt64 petId;

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
				BaseDLL.encode_uint64(buffer, ref pos_, petId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref petId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, petId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref petId);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// petId
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDevourPetReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502215;
		public UInt32 Sequence;
		public UInt64 id;
		public UInt64[] petIds = new UInt64[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)petIds.Length);
				for(int i = 0; i < petIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, petIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 petIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petIdsCnt);
				petIds = new UInt64[petIdsCnt];
				for(int i = 0; i < petIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref petIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)petIds.Length);
				for(int i = 0; i < petIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, petIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 petIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petIdsCnt);
				petIds = new UInt64[petIdsCnt];
				for(int i = 0; i < petIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref petIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// petIds
				_len += 2 + 8 * petIds.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDevourPetRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 502216;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 exp;

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
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// exp
				_len += 4;
				return _len;
			}
		#endregion

	}

}
