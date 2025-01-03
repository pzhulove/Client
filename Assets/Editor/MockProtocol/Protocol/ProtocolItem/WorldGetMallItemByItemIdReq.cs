using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 根据道具id获取商城道具请求
	/// </summary>
	[AdvancedInspector.Descriptor("根据道具id获取商城道具请求", "根据道具id获取商城道具请求")]
	public class WorldGetMallItemByItemIdReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602821;
		public UInt32 Sequence;
		/// <summary>
		///  道具id(不是商城道具的id)
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具id(不是商城道具的id)", " 道具id(不是商城道具的id)")]
		public UInt32 itemId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
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
