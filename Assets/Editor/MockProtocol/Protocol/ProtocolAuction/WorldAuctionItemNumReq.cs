using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  查询拍卖行道具数量
	/// </summary>
	[AdvancedInspector.Descriptor(" 查询拍卖行道具数量", " 查询拍卖行道具数量")]
	public class WorldAuctionItemNumReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603920;
		public UInt32 Sequence;

		public AuctionQueryCondition cond = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			cond.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			cond.decode(buffer, ref pos_);
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
