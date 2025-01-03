using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步队员属性
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步队员属性", " 同步队员属性")]
	public class WorldSyncTeamMemberProperty : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601654;
		public UInt32 Sequence;
		/// <summary>
		///  成员ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员ID", " 成员ID")]
		public UInt64 memberId;
		/// <summary>
		///  属性类型，对应枚举TeamMemberProperty
		/// </summary>
		[AdvancedInspector.Descriptor(" 属性类型，对应枚举TeamMemberProperty", " 属性类型，对应枚举TeamMemberProperty")]
		public byte type;
		/// <summary>
		///  新的值
		/// </summary>
		[AdvancedInspector.Descriptor(" 新的值", " 新的值")]
		public UInt64 value;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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
