using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 强化券融合请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 强化券融合请求", " client->scene 强化券融合请求")]
	public class SceneStrengthenTicketFuseReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501037;
		public UInt32 Sequence;
		/// <summary>
		///  选择要融合的强化券
		/// </summary>
		[AdvancedInspector.Descriptor(" 选择要融合的强化券", " 选择要融合的强化券")]
		public UInt64 pickTicketA;

		public UInt64 pickTicketB;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, pickTicketA);
			BaseDLL.encode_uint64(buffer, ref pos_, pickTicketB);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref pickTicketA);
			BaseDLL.decode_uint64(buffer, ref pos_, ref pickTicketB);
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
