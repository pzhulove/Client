using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 取消远征返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 取消远征返回", " world->client 取消远征返回")]
	public class WorldCancelExpeditionRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608618;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  地图id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图id", " 地图id")]
		public byte mapId;
		/// <summary>
		///  远征状态(对应枚举ExpeditionStatus)
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征状态(对应枚举ExpeditionStatus)", " 远征状态(对应枚举ExpeditionStatus)")]
		public byte expeditionStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			BaseDLL.encode_int8(buffer, ref pos_, mapId);
			BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
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
