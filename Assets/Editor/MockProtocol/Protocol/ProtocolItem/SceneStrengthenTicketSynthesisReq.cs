using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 强化券合成请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 强化券合成请求", " client->scene 强化券合成请求")]
	public class SceneStrengthenTicketSynthesisReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501031;
		public UInt32 Sequence;
		/// <summary>
		///  合成方案
		/// </summary>
		[AdvancedInspector.Descriptor(" 合成方案", " 合成方案")]
		public UInt32 synthesisPlan;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, synthesisPlan);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref synthesisPlan);
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
