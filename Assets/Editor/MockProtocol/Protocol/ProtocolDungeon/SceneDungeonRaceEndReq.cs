using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonRaceEndReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506811;
		public UInt32 Sequence;

		public byte score;

		public byte[] md5 = new byte[16];

		public UInt32 usedTime;

		public UInt16 beHitCount;
		/// <summary>
		///  最大伤害
		/// </summary>
		[AdvancedInspector.Descriptor(" 最大伤害", " 最大伤害")]
		public UInt32 maxDamage;
		/// <summary>
		///  造成最大伤害的技能
		/// </summary>
		[AdvancedInspector.Descriptor(" 造成最大伤害的技能", " 造成最大伤害的技能")]
		public UInt32 skillId;
		/// <summary>
		///  透传信息（随便用）
		/// </summary>
		[AdvancedInspector.Descriptor(" 透传信息（随便用）", " 透传信息（随便用）")]
		public UInt32 param;
		/// <summary>
		///  总伤害
		/// </summary>
		[AdvancedInspector.Descriptor(" 总伤害", " 总伤害")]
		public UInt64 totalDamage;
		/// <summary>
		///  最后一帧
		/// </summary>
		[AdvancedInspector.Descriptor(" 最后一帧", " 最后一帧")]
		public UInt32 lastFrame;
		/// <summary>
		///  对boss的伤害
		/// </summary>
		[AdvancedInspector.Descriptor(" 对boss的伤害", " 对boss的伤害")]
		public UInt64 bossDamage;
		/// <summary>
		///  玩家剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家剩余血量", " 玩家剩余血量")]
		public UInt32 playerRemainHp;
		/// <summary>
		///  玩家剩余蓝量
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家剩余蓝量", " 玩家剩余蓝量")]
		public UInt32 playerRemainMp;
		/// <summary>
		///  BOSS1的ID
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS1的ID", " BOSS1的ID")]
		public UInt32 boss1ID;
		/// <summary>
		///  BOSS2的ID
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS2的ID", " BOSS2的ID")]
		public UInt32 boss2ID;
		/// <summary>
		///  BOSS1的剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS1的剩余血量", " BOSS1的剩余血量")]
		public UInt32 boss1RemainHp;
		/// <summary>
		///  BOSS2的剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS2的剩余血量", " BOSS2的剩余血量")]
		public UInt32 boss2RemainHp;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, score);
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
			BaseDLL.encode_uint16(buffer, ref pos_, beHitCount);
			BaseDLL.encode_uint32(buffer, ref pos_, maxDamage);
			BaseDLL.encode_uint32(buffer, ref pos_, skillId);
			BaseDLL.encode_uint32(buffer, ref pos_, param);
			BaseDLL.encode_uint64(buffer, ref pos_, totalDamage);
			BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
			BaseDLL.encode_uint64(buffer, ref pos_, bossDamage);
			BaseDLL.encode_uint32(buffer, ref pos_, playerRemainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, playerRemainMp);
			BaseDLL.encode_uint32(buffer, ref pos_, boss1ID);
			BaseDLL.encode_uint32(buffer, ref pos_, boss2ID);
			BaseDLL.encode_uint32(buffer, ref pos_, boss1RemainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, boss2RemainHp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref score);
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
			BaseDLL.decode_uint16(buffer, ref pos_, ref beHitCount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxDamage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			BaseDLL.decode_uint64(buffer, ref pos_, ref totalDamage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
			BaseDLL.decode_uint64(buffer, ref pos_, ref bossDamage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainMp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss1ID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss2ID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss1RemainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss2RemainHp);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
