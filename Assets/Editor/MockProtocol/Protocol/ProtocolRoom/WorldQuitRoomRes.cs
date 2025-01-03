using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 请求退出房间返回
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 请求退出房间返回", " server->client 请求退出房间返回")]
	public class WorldQuitRoomRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607818;
		public UInt32 Sequence;
		/// <summary>
		/// 返回值
		/// </summary>
		[AdvancedInspector.Descriptor("返回值", "返回值")]
		public UInt32 result;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
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
