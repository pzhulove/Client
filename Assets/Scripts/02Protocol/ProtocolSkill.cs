using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  技能配置方案
	/// </summary>
	public enum SkillConfigType
	{
		SKILL_CONFIG_INVALID = 0,
		SKILL_CONFIG_PVE = 1,
		SKILL_CONFIG_PVP = 2,
		SKILL_CONFIG_EQUAL_PVP = 3,
	}

	public class Sp : Protocol.IProtocolStream
	{
		public UInt32[] spList = new UInt32[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)spList.Length);
				for(int i = 0; i < spList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, spList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 spListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref spListCnt);
				spList = new UInt32[spListCnt];
				for(int i = 0; i < spList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref spList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)spList.Length);
				for(int i = 0; i < spList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, spList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 spListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref spListCnt);
				spList = new UInt32[spListCnt];
				for(int i = 0; i < spList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref spList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// spList
				_len += 2 + 4 * spList.Length;
				return _len;
			}
		#endregion

	}

	public class Skill : Protocol.IProtocolStream
	{
		public UInt16 id;
		public byte level;
		public UInt32 talentId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, talentId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref talentId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, talentId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref talentId);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 2;
				// level
				_len += 1;
				// talentId
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class SkillPage : Protocol.IProtocolStream
	{
		public Skill[] skillList = new Skill[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillList.Length);
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 skillListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillListCnt);
				skillList = new Skill[skillListCnt];
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i] = new Skill();
					skillList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillList.Length);
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 skillListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillListCnt);
				skillList = new Skill[skillListCnt];
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i] = new Skill();
					skillList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// skillList
				_len += 2;
				for(int j = 0; j < skillList.Length; j++)
				{
					_len += skillList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class SkillMgr : Protocol.IProtocolStream
	{
		public UInt32 pageCnt;
		public UInt32 currentPage;
		public SkillPage[] skillPages = new SkillPage[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pageCnt);
				BaseDLL.encode_uint32(buffer, ref pos_, currentPage);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillPages.Length);
				for(int i = 0; i < skillPages.Length; i++)
				{
					skillPages[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pageCnt);
				BaseDLL.decode_uint32(buffer, ref pos_, ref currentPage);
				UInt16 skillPagesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillPagesCnt);
				skillPages = new SkillPage[skillPagesCnt];
				for(int i = 0; i < skillPages.Length; i++)
				{
					skillPages[i] = new SkillPage();
					skillPages[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pageCnt);
				BaseDLL.encode_uint32(buffer, ref pos_, currentPage);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillPages.Length);
				for(int i = 0; i < skillPages.Length; i++)
				{
					skillPages[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pageCnt);
				BaseDLL.decode_uint32(buffer, ref pos_, ref currentPage);
				UInt16 skillPagesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillPagesCnt);
				skillPages = new SkillPage[skillPagesCnt];
				for(int i = 0; i < skillPages.Length; i++)
				{
					skillPages[i] = new SkillPage();
					skillPages[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// pageCnt
				_len += 4;
				// currentPage
				_len += 4;
				// skillPages
				_len += 2;
				for(int j = 0; j < skillPages.Length; j++)
				{
					_len += skillPages[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class ChangeSkill : Protocol.IProtocolStream
	{
		public UInt16 id;
		public byte dif;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, dif);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref dif);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, dif);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref dif);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 2;
				// dif
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class Buff : Protocol.IProtocolStream
	{
		public UInt64 uid;
		public UInt32 id;
		public UInt32 overlay;
		public UInt32 duration;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, overlay);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref overlay);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, overlay);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref overlay);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// id
				_len += 4;
				// overlay
				_len += 4;
				// duration
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneChangeSkillsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500701;
		public UInt32 Sequence;
		public byte configType;
		public ChangeSkill[] skills = new ChangeSkill[0];

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
				BaseDLL.encode_int8(buffer, ref pos_, configType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref configType);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new ChangeSkill[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new ChangeSkill();
					skills[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, configType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref configType);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new ChangeSkill[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new ChangeSkill();
					skills[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// configType
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

	[Protocol]
	public class SceneChangeSkillsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500702;
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
	public class SceneAddBuff : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500711;
		public UInt32 Sequence;
		public UInt32 buffId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			}

			public int getLen()
			{
				int _len = 0;
				// buffId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyRemoveBuff : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500712;
		public UInt32 Sequence;
		public UInt32 buffId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			}

			public int getLen()
			{
				int _len = 0;
				// buffId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求初始化技能
	/// </summary>
	[Protocol]
	public class SceneInitSkillsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500713;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		public UInt32 configType;

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
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求初始化技能返回
	/// </summary>
	[Protocol]
	public class SceneInitSkillsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500714;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
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
	///  请求推荐技能配置
	/// </summary>
	[Protocol]
	public class SceneRecommendSkillsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500715;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		public UInt32 configType;

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
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求推荐技能配置返回
	/// </summary>
	[Protocol]
	public class SceneRecommendSkillsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500716;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
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
	///  技能槽解锁
	/// </summary>
	[Protocol]
	public class SceneSkillSlotUnlockNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500717;
		public UInt32 Sequence;
		/// <summary>
		///  槽位
		/// </summary>
		public UInt32 slot;

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
				BaseDLL.encode_uint32(buffer, ref pos_, slot);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref slot);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, slot);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref slot);
			}

			public int getLen()
			{
				int _len = 0;
				// slot
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置技能页
	/// </summary>
	[Protocol]
	public class SceneSetSkillPageReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500718;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		public UInt32 configType;
		/// <summary>
		/// 修改的技能页
		/// </summary>
		public byte page;

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
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_int8(buffer, ref pos_, page);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_int8(buffer, ref pos_, ref page);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_int8(buffer, ref pos_, page);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_int8(buffer, ref pos_, ref page);
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 4;
				// page
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置技能页返回
	/// </summary>
	[Protocol]
	public class SceneSetSkillPageRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500719;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		public UInt32 configType;
		/// <summary>
		/// 修改的技能页
		/// </summary>
		public byte page;
		/// <summary>
		///  返回值
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
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_int8(buffer, ref pos_, page);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_int8(buffer, ref pos_, ref page);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_int8(buffer, ref pos_, page);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_int8(buffer, ref pos_, ref page);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 4;
				// page
				_len += 1;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  购买技能页
	/// </summary>
	[Protocol]
	public class SceneBuySkillPageReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500720;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		public UInt32 configType;
		/// <summary>
		/// 购买的技能页
		/// </summary>
		public byte page;

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
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_int8(buffer, ref pos_, page);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_int8(buffer, ref pos_, ref page);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_int8(buffer, ref pos_, page);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_int8(buffer, ref pos_, ref page);
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 4;
				// page
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  购买技能页返回
	/// </summary>
	[Protocol]
	public class SceneBuySkillPageRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500721;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
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
	///  设置天赋
	/// </summary>
	[Protocol]
	public class SceneSetTalentReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500722;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置方案
		/// </summary>
		public UInt32 configType;
		/// <summary>
		///  技能id
		/// </summary>
		public UInt16 skillId;
		/// <summary>
		///  天赋id
		/// </summary>
		public UInt32 talentId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_uint16(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, talentId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref talentId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, configType);
				BaseDLL.encode_uint16(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, talentId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref talentId);
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 4;
				// skillId
				_len += 2;
				// talentId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置天赋返回
	/// </summary>
	[Protocol]
	public class SceneSetTalentRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500723;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
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

}
