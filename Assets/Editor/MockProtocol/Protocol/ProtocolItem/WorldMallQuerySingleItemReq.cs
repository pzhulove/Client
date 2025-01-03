using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  商城查询单个商品请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 商城查询单个商品请求", " 商城查询单个商品请求")]
	public class WorldMallQuerySingleItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602823;
		public UInt32 Sequence;
		/// <summary>
		///  商城道具id
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城道具id", " 商城道具id")]
		public UInt32 mallItemId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, mallItemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemId);
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
