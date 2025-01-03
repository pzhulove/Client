using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  广播玩家玩家投票选项
	/// </summary>
	[AdvancedInspector.Descriptor(" 广播玩家玩家投票选项", " 广播玩家玩家投票选项")]
	public class WorldTeamVoteChoiceNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601647;
		public UInt32 Sequence;
		/// <summary>
		///  角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色ID", " 角色ID")]
		public UInt64 roleId;
		/// <summary>
		///  是否同意
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否同意", " 是否同意")]
		public byte agree;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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
