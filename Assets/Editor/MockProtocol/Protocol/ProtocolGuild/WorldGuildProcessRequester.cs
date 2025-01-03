using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  处理公会成员请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 处理公会成员请求", " 处理公会成员请求")]
	public class WorldGuildProcessRequester : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601912;
		public UInt32 Sequence;
		/// <summary>
		/// id(如果是0代表清空列表)
		/// </summary>
		[AdvancedInspector.Descriptor("id(如果是0代表清空列表)", "id(如果是0代表清空列表)")]
		public UInt64 id;
		/// <summary>
		/// 同意进入(0:不同意，1:同意)
		/// </summary>
		[AdvancedInspector.Descriptor("同意进入(0:不同意，1:同意)", "同意进入(0:不同意，1:同意)")]
		public byte agree;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, agree);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
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
