using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 商城道具
	/// </summary>
	/// <summary>
	/// 购买商城道具请求
	/// </summary>
	[AdvancedInspector.Descriptor("购买商城道具请求", "购买商城道具请求")]
	public class WorldMallBuy : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602801;
		public UInt32 Sequence;

		public UInt32 itemId;
		/// <summary>
		/// 商城道具ID
		/// </summary>
		[AdvancedInspector.Descriptor("商城道具ID", "商城道具ID")]
		public UInt16 num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint16(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref num);
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
