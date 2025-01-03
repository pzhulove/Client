using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拍卖行道具基本信息（类型，数量）
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行道具基本信息（类型，数量）", " 拍卖行道具基本信息（类型，数量）")]
	public class AuctionItemBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  道具ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具ID", " 道具ID")]
		public UInt32 itemTypeId;
		/// <summary>
		///  道具数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具数量", " 道具数量")]
		public UInt32 num;
		/// <summary>
		///  是否是珍品
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否是珍品", " 是否是珍品")]
		public byte isTreas;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, isTreas);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref isTreas);
		}


		#endregion

	}

}
