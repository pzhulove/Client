using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 夺宝记录
	/// </summary>
	[AdvancedInspector.Descriptor("夺宝记录", "夺宝记录")]
	public class GambingItemRecordData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  商品id
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品id", " 商品id")]
		public UInt32 gambingItemId;
		/// <summary>
		///  商品数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品数量", " 商品数量")]
		public UInt32 gambingItemNum;
		/// <summary>
		///  排序
		/// </summary>
		[AdvancedInspector.Descriptor(" 排序", " 排序")]
		public UInt16 sortId;
		/// <summary>
		///  道具表id
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具表id", " 道具表id")]
		public UInt32 itemDataId;
		/// <summary>
		///  售罄时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 售罄时间", " 售罄时间")]
		public UInt32 soldOutTimestamp;
		/// <summary>
		///  组记录
		/// </summary>
		[AdvancedInspector.Descriptor(" 组记录", " 组记录")]
		public GambingGroupRecordData[] groupRecordData = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
			BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
			BaseDLL.encode_uint16(buffer, ref pos_, sortId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
			BaseDLL.encode_uint32(buffer, ref pos_, soldOutTimestamp);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)groupRecordData.Length);
			for(int i = 0; i < groupRecordData.Length; i++)
			{
				groupRecordData[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
			BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref soldOutTimestamp);
			UInt16 groupRecordDataCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref groupRecordDataCnt);
			groupRecordData = new GambingGroupRecordData[groupRecordDataCnt];
			for(int i = 0; i < groupRecordData.Length; i++)
			{
				groupRecordData[i] = new GambingGroupRecordData();
				groupRecordData[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
