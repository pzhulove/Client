using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 拉取珍品交易记录
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 拉取珍品交易记录", "client->world 拉取珍品交易记录")]
	public class WorldAuctionGetTreasTransactionReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603924;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
