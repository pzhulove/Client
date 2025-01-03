using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  商城查询单个商品返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 商城查询单个商品返回", " 商城查询单个商品返回")]
	public class WorldMallQuerySingleItemRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602824;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 retCode;
		/// <summary>
		///  商城道具信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城道具信息", " 商城道具信息")]
		public MallItemInfo mallItemInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			mallItemInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			mallItemInfo.decode(buffer, ref pos_);
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
