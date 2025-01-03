using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 远征地图查询请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 远征地图查询请求", " client->world 远征地图查询请求")]
	public class WorldQueryExpeditionMapReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608611;
		public UInt32 Sequence;
		/// <summary>
		///  地图id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图id", " 地图id")]
		public byte mapId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, mapId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
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
