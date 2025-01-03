using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 玩家拥有的商城购买获得物同步
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 玩家拥有的商城购买获得物同步", " world->client 玩家拥有的商城购买获得物同步")]
	public class WorldPlayerMallBuyGotInfoSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602825;
		public UInt32 Sequence;

		public MallBuyGotInfo mallBuyGotInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			mallBuyGotInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			mallBuyGotInfo.decode(buffer, ref pos_);
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
