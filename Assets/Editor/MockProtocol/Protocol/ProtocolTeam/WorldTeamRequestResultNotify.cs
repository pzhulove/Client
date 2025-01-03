using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知玩家队伍请求处理结果
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知玩家队伍请求处理结果", " 通知玩家队伍请求处理结果")]
	public class WorldTeamRequestResultNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601646;
		public UInt32 Sequence;
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
			BaseDLL.encode_int8(buffer, ref pos_, agree);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
