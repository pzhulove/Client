using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 抽奖请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 抽奖请求", " client->server 抽奖请求")]
	public class SceneDrawPrizeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501006;
		public UInt32 Sequence;
		/// <summary>
		///  抽奖表id
		/// </summary>
		[AdvancedInspector.Descriptor(" 抽奖表id", " 抽奖表id")]
		public UInt32 id;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
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
