using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  购买拍卖行栏位返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 购买拍卖行栏位返回", " 购买拍卖行栏位返回")]
	public class SceneAuctionBuyBoothRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503904;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		[AdvancedInspector.Descriptor(" 结果", " 结果")]
		public UInt32 result;
		/// <summary>
		///  栏位数
		/// </summary>
		[AdvancedInspector.Descriptor(" 栏位数", " 栏位数")]
		public UInt32 boothNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, boothNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boothNum);
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
