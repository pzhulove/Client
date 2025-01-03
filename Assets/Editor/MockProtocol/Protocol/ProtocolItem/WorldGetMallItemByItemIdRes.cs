using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 根据道具id获取商城道具返回
	/// </summary>
	[AdvancedInspector.Descriptor("根据道具id获取商城道具返回", "根据道具id获取商城道具返回")]
	public class WorldGetMallItemByItemIdRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602822;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 retCode;
		/// <summary>
		///  道具id
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具id", " 道具id")]
		public UInt32 itemId;
		/// <summary>
		///  映射的商城道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 映射的商城道具", " 映射的商城道具")]
		public MallItemInfo mallItem = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			mallItem.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			mallItem.decode(buffer, ref pos_);
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
