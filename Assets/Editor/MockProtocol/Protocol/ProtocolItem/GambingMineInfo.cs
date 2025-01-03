using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  我的夺宝信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 我的夺宝信息", " 我的夺宝信息")]
	public class GambingMineInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
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
		///  组id
		/// </summary>
		[AdvancedInspector.Descriptor(" 组id", " 组id")]
		public UInt16 groupId;
		/// <summary>
		///  已售出份数
		/// </summary>
		[AdvancedInspector.Descriptor(" 已售出份数", " 已售出份数")]
		public UInt32 soldCopies;
		/// <summary>
		///  总份数
		/// </summary>
		[AdvancedInspector.Descriptor(" 总份数", " 总份数")]
		public UInt32 totalCopies;
		/// <summary>
		///  我的夺宝数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 我的夺宝数据", " 我的夺宝数据")]
		public GambingParticipantInfo mineGambingInfo = null;
		/// <summary>
		///  获得者夺宝数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得者夺宝数据", " 获得者夺宝数据")]
		public GambingParticipantInfo gainersGambingInfo = null;
		/// <summary>
		///  参与者夺宝数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与者夺宝数据", " 参与者夺宝数据")]
		public GambingParticipantInfo[] participantsGambingInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
			BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
			BaseDLL.encode_uint16(buffer, ref pos_, sortId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
			BaseDLL.encode_uint16(buffer, ref pos_, groupId);
			BaseDLL.encode_uint32(buffer, ref pos_, soldCopies);
			BaseDLL.encode_uint32(buffer, ref pos_, totalCopies);
			mineGambingInfo.encode(buffer, ref pos_);
			gainersGambingInfo.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)participantsGambingInfo.Length);
			for(int i = 0; i < participantsGambingInfo.Length; i++)
			{
				participantsGambingInfo[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
			BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref soldCopies);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalCopies);
			mineGambingInfo.decode(buffer, ref pos_);
			gainersGambingInfo.decode(buffer, ref pos_);
			UInt16 participantsGambingInfoCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref participantsGambingInfoCnt);
			participantsGambingInfo = new GambingParticipantInfo[participantsGambingInfoCnt];
			for(int i = 0; i < participantsGambingInfo.Length; i++)
			{
				participantsGambingInfo[i] = new GambingParticipantInfo();
				participantsGambingInfo[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
