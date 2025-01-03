using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  远征地图基本信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 远征地图基本信息", " 远征地图基本信息")]
	public class ExpeditionMapBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
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
		/// <summary>
		///  远征队员数
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征队员数", " 远征队员数")]
		public UInt16 memberNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, mapId);
			BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
			BaseDLL.encode_uint16(buffer, ref pos_, memberNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
			BaseDLL.decode_uint16(buffer, ref pos_, ref memberNum);
		}


		#endregion

	}

}
